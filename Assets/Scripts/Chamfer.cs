using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Chamfer {
	int Mode = 2;
	
	public Chamfer(int _mode){
		Mode = _mode;
	}
	
	
	private Vector3 DirectionCal (Vector3 Direction,float c_value){
		float ratio = 0f;
		ratio = (Direction.magnitude - c_value) / Direction.magnitude;
		Direction = Direction * ratio;
		return Direction;
	}
	
	//两向量夹角计算,返回弧度值
	private float Ve3AngCal(Vector3 v1,Vector3 v2){
		float cos = Vector3.Dot (v1,v2)/(v1.magnitude*v2.magnitude);
		float ang = Mathf.Acos (cos);
		return ang;
	}
	
	//向量旋转并将长度调整为Radius
	//direction：true为顺时针，false为逆时针
	//_angle为弧度
	private Vector3 VecRotate(Vector3 _Vec,float _angle,bool direction,float Radius)
	{
		float temp_x = 0f;
		float temp_y = 0f;
		Vector3 rotated_vec = Vector3.zero;
		int dir_flag = 1;
		if(direction){
			dir_flag = 1;
		}else{
			dir_flag = -1;
		}
		
		temp_x = (float)(_Vec.x * Math.Cos (dir_flag*_angle) + _Vec.y * Math.Sin (dir_flag*_angle));
		temp_y = -(float)(_Vec.x * Math.Sin (dir_flag*_angle) + _Vec.y * Math.Cos (dir_flag*_angle));
		
		rotated_vec = new Vector3(temp_x,temp_y,_Vec.z);
		rotated_vec = rotated_vec / rotated_vec.magnitude * Radius;
		return rotated_vec;
	}
	
	//两直线求交点
	private Vector3 PosCal_LL(Vector3 Startpo_1,Vector3 Endpo_1,Vector3 Direction_1,Vector3 Startpo_2,Vector3 Endpo_2,Vector3 Direction_2)
	{
		if(Startpo_1 == Endpo_1){
			return Startpo_1;
		}else if(Startpo_2 == Endpo_2){
			return Startpo_2;
		}
		
		if((Direction_1.x/Direction_2.x) == (Direction_1.y/Direction_2.y)){
			return Endpo_1;
		}
		
		Vector3 cross_po = Vector3.zero;
		float k = 0f;
		float b = 0f;
		float temp_x = 0f;
		float temp_y = 0f;
		
		if(Startpo_1.x == Endpo_1.x){
			k = (Endpo_2.y - Startpo_2.y)/(Endpo_2.x - Startpo_2.x);
			b = Startpo_2.y - k * Startpo_2.x;
			temp_x = Startpo_1.x;
			temp_y = k*temp_x+b;
			cross_po = new Vector3(temp_x,temp_y,Startpo_1.z);
			return cross_po;
		}else if(Startpo_2.x == Endpo_2.x){
			k = (Endpo_1.y - Startpo_1.y)/(Endpo_1.x - Startpo_1.x);
			b = Startpo_1.y - k * Startpo_1.x;
			temp_x = Startpo_2.x;
			temp_y = k*temp_x+b;
			cross_po = new Vector3(temp_x,temp_y,Startpo_1.z);
			return cross_po;
		}else if(Startpo_1.y == Endpo_1.y){
			k = (Endpo_2.y - Startpo_2.y)/(Endpo_2.x - Startpo_2.x);
			b = Startpo_2.y - k * Startpo_2.x;
			temp_y = Startpo_1.y;
			temp_x = (temp_y-b)/k;
			cross_po = new Vector3(temp_x,temp_y,Startpo_1.z);
			return cross_po;
		}else if(Startpo_2.y == Endpo_2.y){
			k = (Endpo_1.y - Startpo_1.y)/(Endpo_1.x - Startpo_1.x);
			b = Startpo_1.y - k * Startpo_1.x;
			temp_y = Startpo_2.y;
			temp_x = (temp_y-b)/k;
			cross_po = new Vector3(temp_x,temp_y,Startpo_1.z);
			return cross_po;
		}else{
			float k0 = (Endpo_1.y - Startpo_1.y)/(Endpo_1.x - Startpo_1.x);
			float b0 = Startpo_1.y - k0 * Startpo_1.x;
			k = (Endpo_2.y - Startpo_2.y)/(Endpo_2.x - Startpo_2.x);
			b = Startpo_2.y - k * Startpo_2.x;
			temp_x = (b - b0)/(k0 - k);
			temp_y = k * temp_x + b;
			cross_po = new Vector3(temp_x,temp_y,Startpo_1.z);
			return cross_po;
		}
	}
	
	//cw(true) is clockwise
	private float CalculateDegree(Vector3 CenterPos, Vector3 startPos, Vector3 EndPos, bool cw){
		Vector3 Start_Vec = new Vector3 (startPos.x-CenterPos.x, startPos.y-CenterPos.y, startPos.z-CenterPos.z);
		Vector3 End_Vec = new Vector3 (EndPos.x-CenterPos.x, EndPos.y-CenterPos.y, EndPos.z-CenterPos.z);
		Vector3 Cross_vec = Vector3.Cross (Start_Vec, End_Vec);
		Vector3 Standard_vec = new Vector3(0,0,1);
		float Direction = Vector3.Dot (Standard_vec,Cross_vec);
		float RET = Mathf.Asin (Cross_vec.magnitude / (Start_Vec.magnitude * End_Vec.magnitude)) * Mathf.Rad2Deg;
		if(Direction < 0){
			if(!cw){
				RET = 360 - RET;
			}
		}else if(Direction > 0){
			if(cw){
				RET = 360 - RET;
			}
		}else{
			RET = 180;
		}
		return RET;
	}
	
	
	public bool Chamfer_R(ref MotionInfo motion_data1,ref MotionInfo motion_data2,ref MotionInfo motion_data3,float r_value,ref List<string> _compileInfo)
	{
		string _error = "";
		if((motion_data1.Motion_Type != (int)MotionType.Line && motion_data2.Motion_Type != (int)MotionType.CHF && motion_data3.Motion_Type != (int)MotionType.Line)){
			_error = "Line:" + (motion_data2.index+1) + " " + "倒角指令错误！";
			if(CompileParas.DEBUG) _error += "% ErrorDBG-RND-1 %";
			_compileInfo.Add (_error);
			return false;
		}
		
		if(motion_data1.Direction_D.magnitude <= r_value || motion_data3.Direction_D.magnitude <= r_value){
			_error = "Line:" + (motion_data2.index+1) + " " + "倒角(圆)指令错误,轨迹长度小于倒角(圆)值！";
			if(CompileParas.DEBUG) _error += "% ErrorDBG-RND-2 %";
			_compileInfo.Add (_error);
			return false;
		}
	
		Vector3 virtual_pos = motion_data1.VirtualStart;
		Vector3 display_pos = motion_data1.DisplayStart;
		Vector3 temp_dir1 = motion_data1.Direction_D;
		Vector3 temp_dir2 = motion_data3.Direction_D;
		Vector3 dir1;
		Vector3 dir2;
		Vector3 startpo_1;
		Vector3 startpo_2;
		Vector3 endpo_1;
		Vector3 endpo_2;
		Vector3 temp_center;
		float degree;
		float speed;
		float temp_cross = Vector3.Cross(temp_dir1,temp_dir2).z;
		bool type;//true为顺时针
//		Debug.Log (temp_cross);
		if(temp_cross < 0){
			type = true;//顺时针
			dir1 = VecRotate (motion_data1.Direction_D,Mathf.PI/2,true, r_value);
			dir2 = VecRotate (motion_data3.Direction_D,Mathf.PI/2,true, r_value);
		}else{
			type = false;//逆时针
			dir1 = VecRotate (motion_data1.Direction_D,Mathf.PI/2,false, r_value);
			dir2 = VecRotate (motion_data3.Direction_D,Mathf.PI/2,false, r_value);
		}
		startpo_1 = motion_data1.DisplayStart + dir1;
		startpo_2 = motion_data3.DisplayStart + dir2;
		endpo_1 = motion_data1.DisplayTarget + dir1;
		endpo_2 = motion_data3.DisplayTarget + dir2;
		
		temp_center = PosCal_LL (startpo_1,endpo_1,motion_data1.Direction_D,startpo_2,endpo_2,motion_data2.Direction_D);
		endpo_1 = temp_center - dir1;
		endpo_2 = temp_center - dir2;
		degree = CalculateDegree (temp_center,endpo_1,endpo_2,!type);//############### !type? #############
		
		motion_data1.Direction_D = endpo_1 - display_pos;
		motion_data1.Direction_V = ModalState._PosRot (Mode,motion_data1.Direction_D,motion_data1.Matri3);
		motion_data1.Time_Value = motion_data1.Direction_D.magnitude/motion_data1.Velocity*60;
		display_pos += motion_data1.Direction_D;
		virtual_pos += motion_data1.Direction_V;
		motion_data1.SetTargetPos (display_pos,virtual_pos,motion_data1.RotTarget);
		
		if(type)
			motion_data2.Motion_Type = (int)MotionType.CR_N;
		else
			motion_data2.Motion_Type = (int)MotionType.CR_P;
		motion_data2.SetStartPos (display_pos,virtual_pos,motion_data1.RotTarget);
		motion_data2.Direction_D = endpo_2 - endpo_1;
		motion_data2.Direction_V = ModalState._PosRot (Mode,motion_data2.Direction_D,motion_data2.Matri3);
		motion_data2.Center_Point_D = temp_center;
		motion_data2.Rotate_Degree = degree;
		if(motion_data2.Velocity == 0){
			motion_data2.Velocity = motion_data1.Velocity;
		}
		speed = (motion_data2.Velocity / (60f * r_value)) * (180 / Mathf.PI);
		motion_data2.Rotate_Speed = speed;
		motion_data2.Time_Value = degree/speed;
		display_pos += motion_data2.Direction_D;
		virtual_pos += motion_data2.Direction_V;
		motion_data2.SetTargetPos (display_pos,virtual_pos,motion_data2.RotStart);
		
		motion_data3.Direction_D = motion_data3.DisplayTarget - endpo_2;
		motion_data3.Direction_V = ModalState._PosRot (Mode,motion_data3.Direction_D,motion_data3.Matri3);
		motion_data3.DisplayStart = endpo_2;
		motion_data3.VirtualStart = motion_data3.VirtualTarget - motion_data3.Direction_V;
		motion_data3.Time_Value = motion_data3.Direction_D.magnitude / motion_data3.Velocity * 60;
		
		return true;
	}
	
	public bool Chamfer_C(ref MotionInfo motion_data1,ref MotionInfo motion_data2,ref MotionInfo motion_data3,float c_value,ref List<string> _compileInfo){
		string _error = "";
		if((motion_data1.Motion_Type != (int)MotionType.Line && motion_data2.Motion_Type != (int)MotionType.CHF && motion_data3.Motion_Type != (int)MotionType.Line)){
			_error = "Line:" + (motion_data2.index+1) + " " + "倒角指令错误！";
			if(CompileParas.DEBUG) _error += "% ErrorDBG-CHF-3 %";
			_compileInfo.Add (_error);
			return false;
		}
		
		if(motion_data1.Direction_D.magnitude <= c_value || motion_data3.Direction_D.magnitude <= c_value){
			_error = "Line:" + (motion_data2.index+1) + " " + "倒角(圆)指令错误,轨迹长度小于倒角(圆)值！";
			if(CompileParas.DEBUG) _error += "% ErrorDBG-CHF-4 %";
			_compileInfo.Add (_error);
			return false;
		}
		
		Vector3 display_pos = motion_data1.DisplayStart;
		Vector3 virtual_pos = motion_data1.VirtualStart;
		Vector3 temp_Dir = Vector2.zero;
		
		temp_Dir = DirectionCal (motion_data1.Direction_D,c_value);
		motion_data1.Direction_D = temp_Dir;
		motion_data1.Direction_V = ModalState._PosRot (Mode,temp_Dir,motion_data1.Matri3);
		display_pos += temp_Dir;
		virtual_pos += motion_data1.Direction_V;
		motion_data1.DisplayTarget = display_pos;
		motion_data1.VirtualTarget = virtual_pos;
		motion_data1.Time_Value = motion_data1.Direction_D.magnitude / motion_data1.Velocity*60;
		
		temp_Dir = DirectionCal (motion_data3.Direction_D,c_value);
		motion_data3.Direction_D = temp_Dir;
		motion_data3.Direction_V = ModalState._PosRot (Mode,temp_Dir,motion_data3.Matri3);
		motion_data3.DisplayStart = motion_data3.DisplayTarget - temp_Dir;
		motion_data3.VirtualStart = motion_data3.VirtualTarget - motion_data3.Direction_V;
		motion_data3.Time_Value = motion_data3.Direction_D.magnitude/motion_data3.Velocity*60;
		
		temp_Dir = motion_data3.DisplayStart - display_pos;
		motion_data2.Motion_Type = (int)MotionType.Line;
		motion_data2.SetStartPos (display_pos,virtual_pos,motion_data1.RotTarget);
		motion_data2.Direction_D = temp_Dir;
		motion_data2.Direction_V = ModalState._PosRot (Mode,temp_Dir,motion_data2.Matri3);
		if(motion_data2.Velocity == 0){
			motion_data2.Velocity = motion_data1.Velocity;
		}
		motion_data2.Time_Value = motion_data2.Direction_D.magnitude/motion_data2.Velocity*60;
		display_pos += temp_Dir;
		virtual_pos += motion_data2.Direction_V;
		motion_data2.SetTargetPos (display_pos,virtual_pos,motion_data2.RotStart);
		
		return true;
	}
	
	
	public bool Chamfer_CF(ref MotionInfo motion_data1,ref MotionInfo motion_data2,ref MotionInfo motion_data3, float c_value,ref List<string> _compileInfo)
	{
		string _error = "";
		if((motion_data1.Motion_Type != (int)MotionType.Line && motion_data2.Motion_Type != (int)MotionType.CHF && motion_data3.Motion_Type != (int)MotionType.Line)){
			_error = "Line:" + (motion_data2.index+1) + " " + "倒角指令错误！";
			if(CompileParas.DEBUG) _error += "% ErrorDBG-CF-5 %";
			_compileInfo.Add (_error);
			return false;
		}
		
		if(motion_data1.Direction_D.magnitude <= c_value || motion_data3.Direction_D.magnitude <= c_value){
			_error = "Line:" + (motion_data1.index+1) + " " + "倒角(圆)指令错误,轨迹长度小于倒角(圆)值！";
			if(CompileParas.DEBUG) _error += "% ErrorDBG-CF-6 %";
			_compileInfo.Add (_error);
			return false;
		}
		
		float Ang = Mathf.PI - Ve3AngCal (motion_data1.Direction_D,motion_data3.Direction_D);
		float r = (c_value/2)/Mathf.Sin(Ang/2);
		return Chamfer_C (ref motion_data1,ref motion_data2,ref motion_data3,r,ref _compileInfo);
	}
}
