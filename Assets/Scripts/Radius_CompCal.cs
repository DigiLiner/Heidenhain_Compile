using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

//(RL)G41工件在前进方向左侧（向右侧偏移）
//(RR)G42工件在前进方向右侧（向左偏移）
public class Radius_CompCal{
	private int MODE = 2;
	public bool HasCircle;
	public Vector3 Circle_center;
	public Vector3 Circle_startpos;
	public Vector3 Circle_endpos;
	public Vector3 Circle_Direction;
	public float RotateAngle;
	public float Circle_radius;
	public int G4x;
	public float Radius;
	Vector3 _RadiusVec1;
	Vector3 _RadiusVec2;
	Vector3 _VecForOuter;
	
	public Radius_CompCal()
	{
		HasCircle = false;
		Circle_center = Vector3.zero;
		Circle_startpos = Vector3.zero;
		Circle_endpos = Vector3.zero;
		Circle_Direction = Vector3.zero;
		RotateAngle = 0f;
		Circle_radius = 0f;
		G4x = (int)RCompEnum.R0;
		Radius = 0f;
		_RadiusVec1 = Vector3.zero;
		_RadiusVec2 = Vector3.zero;
		_VecForOuter = Vector3.zero;
	}
	
	public Radius_CompCal(int mode){
		MODE = mode;
		HasCircle = false;
		Circle_center = Vector3.zero;
		Circle_startpos = Vector3.zero;
		Circle_endpos = Vector3.zero;
		Circle_Direction = Vector3.zero;
		RotateAngle = 0f;
		Circle_radius = 0f;
		G4x = (int)RCompEnum.R0;
		Radius = 0f;
		_RadiusVec1 = Vector3.zero;
		_RadiusVec2 = Vector3.zero;
		_VecForOuter = Vector3.zero;
	}
	
	public Radius_CompCal(float r_value){
		HasCircle = false;
		Circle_center = Vector3.zero;
		Circle_startpos = Vector3.zero;
		Circle_endpos = Vector3.zero;
		Circle_Direction = Vector3.zero;
		RotateAngle = 0f;
		Circle_radius = 0f;
		G4x = (int)RCompEnum.R0;
		Radius = r_value;
		_RadiusVec1 = Vector3.zero;
		_RadiusVec2 = Vector3.zero;
		_VecForOuter = Vector3.zero;
	}
	
	public Radius_CompCal(float r_value,int mode){
		MODE = mode;
		HasCircle = false;
		Circle_center = Vector3.zero;
		Circle_startpos = Vector3.zero;
		Circle_endpos = Vector3.zero;
		Circle_Direction = Vector3.zero;
		RotateAngle = 0f;
		Circle_radius = 0f;
		G4x = (int)RCompEnum.R0;
		Radius = r_value;
		_RadiusVec1 = Vector3.zero;
		_RadiusVec2 = Vector3.zero;
		_VecForOuter = Vector3.zero;
	}
	
	public Vector3 GetVec(){
		return _VecForOuter;
	}
	
	public void SetRValue(float r_value){
		Radius = r_value;
	}
	
	public void Clear(){
		HasCircle = false;
		Circle_center = Vector3.zero;
		Circle_startpos = Vector3.zero;
		Circle_endpos = Vector3.zero;
		Circle_Direction = Vector3.zero;
		RotateAngle = 0f;
		Circle_radius = 0f;
		_RadiusVec1 = Vector3.zero;
		_RadiusVec2 = Vector3.zero;
	}

	public bool Calculate(ref MotionInfo motion_1,ref MotionInfo motion_2,MotionInfo motion_bac,bool radius_last,ref string _error, ref List<string> _compileInfo)
	{
		if(Radius == 0)
			return true;
		
		if(motion_1.List_flag){
			if(!motion_2.List_flag){
				if(motion_2.Motion_Type == (int)MotionType.CR_P || motion_2.Motion_Type == (int)MotionType.CR_N){
					_error = "Line:" + (motion_2.index+1) + " " + "执行循环指令或返回参考点指令后恢复半径补偿出错，起刀程序段不能为圆弧指令！";
					if(CompileParas.DEBUG) _error += "% ErrorDBG-Motion-131 %";
					_compileInfo.Add (_error);
					return false;
				}else{
					if(motion_2.RadiusCompState == (int)CompState.Cancel){
						motion_2.RadiusCompState = (int)CompState.NO;
						G4x = motion_2.RadiusCompInfo;
					}else if(motion_2.RadiusCompState == (int)CompState.Normal){
//						if(motion_1.Motion_Type != (int)MotionType.G70)
//							motion_2.RadiusCompState = (int)CompState.Start;
						G4x = motion_2.RadiusCompInfo;
					}
				}
			}
		}else{
//			if(motion_2.List_flag && motion_2.Motion_Type != (int)MotionType.AutoReturnRP && motion_2.Motion_Type != (int)MotionType.AutoReturnRP2)
//			{
				if(motion_1.Motion_Type == (int)MotionType.CR_P || motion_1.Motion_Type == (int)MotionType.CR_N){
					_error = "Line:" + (motion_2.index+1) + " " + "暂时取消刀尖半径补偿出错，取消半径补偿程序段不能为圆弧指令！";
					if(CompileParas.DEBUG) _error += "% ErrorDBG-Motion-130 %";
					_compileInfo.Add (_error);
					return false;
				}else{
					if(motion_1.RadiusCompState == (int)CompState.Normal || motion_1.RadiusCompState == (int)CompState.Start){
//						if(motion_2.Motion_Type == (int)MotionType.G70 || motion_2.Motion_Type == (int)MotionType.G71 ||motion_2.Motion_Type == (int)MotionType.G72 ||motion_2.Motion_Type == (int)MotionType.G73){
//							G4x = motion_1.RadiusCompInfo;
//							motion_1.RadiusCompState = (int)CompState.NO;
//						}else{
							G4x = motion_1.RadiusCompInfo;
							motion_1.RadiusCompState = (int)CompState.Cancel;
//						}
					}
				}
//			}
		}
		
//		if(motion_1.List_flag){
//			switch(motion_1.Motion_Type){
//			case (int)MotionType.AutoReturnRP:
//			case (int)MotionType.AutoReturnRP2:
//				CompensationCancel (ref motion_1,motion_bac);
//				break;
//			case (int)MotionType.Cycle_G90:
//			case (int)MotionType.Cycle_G94:
//				CompensationG9x (ref motion_1,motion_bac);
//				motion_2.RadiusCompState = (int)CompState.Start;
//				break;
//			case (int)MotionType.G70:break;
//			case (int)MotionType.G71:
//			case (int)MotionType.G72:
//			case (int)MotionType.G73:break;
//			case (int)MotionType.CYCLE81:
//				//??????????????????????
//				break;
//			default:break;
//			}
//			return true;
//		}
		
		switch(motion_1.RadiusCompState){
		case (int)CompState.Start:
			CompensationStart (ref motion_1,ref motion_2,radius_last);
			G4x = motion_1.RadiusCompInfo;
			break;
		case (int)CompState.Cancel:
			CompensationCancel (ref motion_1,motion_bac);
			G4x = motion_1.RadiusCompInfo;
			break;
		case (int)CompState.Normal:
			if(radius_last)
				CompensationLast (ref motion_1, motion_bac);
			else
				CompensationNormal (ref motion_1,ref motion_2,motion_bac);
			break;
		case (int)CompState.CancelInMiddle:break;
		case (int)CompState.NO:break;
		default:break;
		}
		return true;
	}
	
	private void CompensationStart(ref MotionInfo motion_1,ref MotionInfo motion_2,bool last_flag){
		if(motion_1.RadiusCompInfo == (int)RCompEnum.RL){
			if(last_flag){
				_RadiusVec1 = VecRotate (motion_1.Direction_D,Mathf.PI/2,true);
				motion_1.DisplayTarget = motion_2.DisplayTarget + _RadiusVec1;
			}else{
				if(motion_2.Motion_Type == (int)MotionType.CR_P){
					_RadiusVec1 = motion_2.Center_Point_D-motion_2.DisplayStart;
					_RadiusVec1 = - Radius * _RadiusVec1/_RadiusVec1.magnitude;
				}else if(motion_2.Motion_Type == (int)MotionType.CR_N){
					_RadiusVec1 = motion_2.Center_Point_D-motion_2.DisplayStart;
					_RadiusVec1 = Radius * _RadiusVec1/_RadiusVec1.magnitude;
				}else{
					_RadiusVec1 = VecRotate (motion_2.Direction_D,Mathf.PI/2,true);
				}
				motion_1.DisplayTarget = motion_2.DisplayStart + _RadiusVec1;
			}
			
			motion_1.Direction_D = motion_1.DisplayTarget - motion_1.DisplayStart;
			motion_1.Direction_V = ModalState._PosRot (MODE,motion_1.Direction_D,motion_1.Matri3);
			motion_1.VirtualTarget = motion_1.VirtualStart + motion_1.Direction_V;
			motion_1.Time_Value = motion_1.Direction_D.magnitude/motion_1.Velocity*60;
		}else if(motion_1.RadiusCompInfo == (int)RCompEnum.RR){
			if(last_flag){
				_RadiusVec1 = VecRotate (motion_1.Direction_D,Mathf.PI/2,false);
				motion_1.DisplayTarget = motion_2.DisplayTarget + _RadiusVec1;
			}else{
				if(motion_2.Motion_Type == (int)MotionType.CR_P){
					_RadiusVec1 = motion_2.Center_Point_D-motion_2.DisplayStart;
					_RadiusVec1 = Radius * _RadiusVec1/_RadiusVec1.magnitude;
				}else if(motion_2.Motion_Type == (int)MotionType.CR_N){
					_RadiusVec1 = motion_2.Center_Point_D-motion_2.DisplayStart;
					_RadiusVec1 = - Radius * _RadiusVec1/_RadiusVec1.magnitude;
				}else{
					_RadiusVec1 = VecRotate (motion_2.Direction_D,Mathf.PI/2,false);
				}
				motion_1.DisplayTarget = motion_2.DisplayStart + _RadiusVec1;
			}
			
			motion_1.Direction_D = motion_1.DisplayTarget - motion_1.DisplayStart;
			motion_1.Direction_V = ModalState._PosRot (MODE,motion_1.Direction_D,motion_1.Matri3);
			motion_1.VirtualTarget = motion_1.VirtualStart + motion_1.Direction_V;
			motion_1.Time_Value = motion_1.Direction_D.magnitude/motion_1.Velocity*60;
		}
		_VecForOuter = _RadiusVec1;
	}
	
	private void CompensationCancel(ref MotionInfo motion_1,MotionInfo motion_bac){
		motion_1.DisplayStart = motion_bac.DisplayTarget;
		motion_1.VirtualStart = motion_bac.VirtualTarget;
		/*
		if(motion_1.Motion_Type == (int)MotionType.AutoReturnRP || motion_1.Motion_Type == (int)MotionType.AutoReturnRP2)
		{//返回参考点的暂时取消
			motion_1.DisplayStart_List[0] = motion_bac.DisplayTarget;
			motion_1.VirtualStart_List[0] = motion_bac.VirtualStart;
			motion_1.Direction_List[0] = motion_1.DisplayTarget_List[0] - motion_1.DisplayStart_List[0];
			motion_1.TimeValueList[0] = motion_1.Direction_List[0].magnitude/motion_1.VelocityList[0]*60;
		}else{
			motion_1.Direction = motion_1.DisplayTarget - motion_1.DisplayStart;
			motion_1.Time_Value = motion_1.Direction.magnitude/motion_1.Velocity*60;
		}
		*/
		motion_1.Direction_D = motion_1.DisplayTarget - motion_1.DisplayStart;
		motion_1.Direction_V = ModalState._PosRot (MODE,motion_1.Direction_D,motion_1.Matri3);
		motion_1.Time_Value = motion_1.Direction_D.magnitude/motion_1.Velocity*60;
		
		_VecForOuter = Vector3.zero;
	}
	
	/*
	private void CompensationG9x(ref MotionInfo motion_1,MotionInfo motion_bac)
	{
		bool rotate_dir = false;
		Vector2 temp_end = Vector2.zero;
		Vector2 dir_4 = motion_1.Direction_List[3];
		if(dir_4.x > 0)
			rotate_dir = true;
		else
			rotate_dir = false;
		
		_RadiusVec1 = VecRotate (motion_1.Direction_List[1],Mathf.PI/2,rotate_dir);
		_RadiusVec2 = VecRotate (motion_1.Direction_List[2],Mathf.PI/2,rotate_dir);
		//起刀
		motion_1.DisplayTarget_List[0] = motion_1.DisplayStart_List[1] + _RadiusVec1;
		motion_1.VirtualTarget_List[0] = motion_1.VirtualStart_List[1] + _RadiusVec1;
		motion_1.Direction_List[0] = motion_1.DisplayTarget_List[0] - motion_1.DisplayStart_List[0];
		motion_1.TimeValueList[0] = motion_1.Direction_List[0].magnitude/motion_1.VelocityList[0]*60;
		//第二步
		temp_end = CompensationPosCal_LL (motion_1.DisplayStart_List[1]+_RadiusVec1,motion_1.DisplayTarget_List[1]+_RadiusVec1,motion_1.Direction_List[1],motion_1.DisplayStart_List[2]+_RadiusVec2,motion_1.DisplayTarget_List[2]+_RadiusVec2,motion_1.Direction_List[2]);
		motion_1.DisplayStart_List[1] = motion_1.DisplayTarget_List[0];
		motion_1.VirtualStart_List[1] = motion_1.VirtualTarget_List[0];
		motion_1.DisplayTarget_List[1] = temp_end;
		motion_1.Direction_List[1] = motion_1.DisplayTarget_List[1] - motion_1.DisplayStart_List[1];
		motion_1.VirtualTarget_List[1] = motion_1.VirtualStart_List[1] + motion_1.Direction_List[1];
		motion_1.TimeValueList[1] = motion_1.Direction_List[1].magnitude/motion_1.VelocityList[1]*60;
		//第三步
		temp_end = motion_1.DisplayTarget_List[2]+_RadiusVec2;
		motion_1.DisplayStart_List[2] = motion_1.DisplayTarget_List[1];
		motion_1.VirtualStart_List[2] = motion_1.VirtualTarget_List[1];
		motion_1.DisplayTarget_List[2] = temp_end;
		motion_1.Direction_List[2] = motion_1.DisplayTarget_List[2] - motion_1.DisplayStart_List[2];
		motion_1.VirtualTarget_List[2] = motion_1.VirtualStart_List[2] + motion_1.Direction_List[2];
		motion_1.TimeValueList[2] = motion_1.Direction_List[2].magnitude/motion_1.VelocityList[2]*60;
		//最后一步
		motion_1.DisplayStart_List[3] = motion_1.DisplayTarget_List[2];
		motion_1.VirtualStart_List[3] = motion_1.VirtualTarget_List[2];
		motion_1.Direction_List[3] = motion_1.DisplayTarget_List[3] - motion_1.DisplayStart_List[3];
		motion_1.TimeValueList[3] = motion_1.Direction_List[3].magnitude/motion_1.VelocityList[3]*60;
	}
	*/
	
	private void CompensationLast(ref MotionInfo motion_1,MotionInfo motion_bac){
		float temp_radius = -Radius;
		if(motion_1.Motion_Type == (int)MotionType.CR_P || motion_1.Motion_Type == (int)MotionType.CR_N){
			_RadiusVec1 = motion_1.Center_Point_D - motion_1.DisplayTarget;
			_RadiusVec1 = Radius * _RadiusVec1/_RadiusVec1.magnitude;
		}else{
			_RadiusVec1 = VecRotate(motion_1.Direction_D,Mathf.PI/2,true);
		}
		if(G4x == (int)RCompEnum.RL){
			if(motion_1.Motion_Type == (int)MotionType.CR_P){
				temp_radius = -temp_radius;
				_RadiusVec1 = -_RadiusVec1;
			}
		}else if(G4x == (int)RCompEnum.RR){
			if(motion_1.Motion_Type == (int)MotionType.CR_N){
				temp_radius = -temp_radius;
				_RadiusVec1 = -_RadiusVec1;
			}else if(motion_1.Motion_Type != (int)MotionType.CR_P){
				_RadiusVec1 = -_RadiusVec1;
			}
		}
		
		motion_1.DisplayStart = motion_bac.DisplayTarget;
		motion_1.VirtualStart = motion_bac.VirtualTarget;
		motion_1.DisplayTarget += _RadiusVec1;
		motion_1.Direction_D = motion_1.DisplayTarget - motion_1.DisplayStart;
		motion_1.Direction_V = ModalState._PosRot (MODE,motion_1.Direction_D,motion_1.Matri3);
		motion_1.VirtualTarget = motion_1.VirtualStart + motion_1.Direction_V;
		if(motion_1.Motion_Type == (int)MotionType.CR_P || motion_1.Motion_Type == (int)MotionType.CR_N){
			motion_1.Circle_r += temp_radius;
			motion_1.Rotate_Speed = (motion_1.Velocity / (60f * motion_1.Circle_r)) * (180 / Mathf.PI);
			if(motion_1.Motion_Type == (int)MotionType.CR_P)
				motion_1.Rotate_Degree = CalculateDegree (motion_1.Center_Point_D,motion_1.DisplayStart,motion_1.DisplayTarget,true);
			else
				motion_1.Rotate_Degree = CalculateDegree (motion_1.Center_Point_D,motion_1.DisplayStart,motion_1.DisplayTarget,false);
			motion_1.Time_Value = motion_1.Rotate_Degree / motion_1.Rotate_Speed;
		}else{
			motion_1.Time_Value = motion_1.Direction_D.magnitude/motion_1.Velocity * 60;
		}
		
		_VecForOuter = _RadiusVec1;
	}
	
	private void CompensationNormal(ref MotionInfo motion_1,ref MotionInfo motion_2,MotionInfo motion_bac)
	{
		Vector3 temp_crosspo = Vector3.zero;
		float temp = 0f;
		float temp_radius1 = 0;
		float temp_radius2 = 0;
		int temp_flag = 1;
		if(motion_1.Motion_Type == (int)MotionType.CR_P || motion_1.Motion_Type == (int)MotionType.CR_N){
			if(motion_2.Motion_Type == (int)MotionType.CR_P || motion_2.Motion_Type == (int)MotionType.CR_N){
				_RadiusVec1 = motion_1.Center_Point_D - motion_1.DisplayTarget;
				_RadiusVec1 = Radius * _RadiusVec1/_RadiusVec1.magnitude;
				_RadiusVec2 = motion_2.Center_Point_D - motion_2.DisplayStart;
				_RadiusVec2 = Radius * _RadiusVec2/_RadiusVec2.magnitude;
				temp_radius1 = -Radius;
				temp_radius2 = -Radius;
				if(G4x == (int)RCompEnum.RL){
					temp_flag = 1;
					if(motion_1.Motion_Type == (int)MotionType.CR_P){
						_RadiusVec1 = - _RadiusVec1;
						temp_radius1 = -temp_radius1;
					}
					if(motion_2.Motion_Type == (int)MotionType.CR_P){
						_RadiusVec2 = - _RadiusVec2;
						temp_radius2 = -temp_radius2;
					}
				}else if(G4x == (int)RCompEnum.RR){
					temp_flag = -1;
					if(motion_1.Motion_Type == (int)MotionType.CR_N){
						_RadiusVec1 = - _RadiusVec1;
						temp_radius1 = -temp_radius1;
					}
					if(motion_2.Motion_Type == (int)MotionType.CR_N){
						_RadiusVec2 = - _RadiusVec2;
						temp_radius2 = -temp_radius2;
					}
				}
				
				temp = Vector3.Cross(_RadiusVec1,_RadiusVec2).y * temp_flag;
				temp = (float)Math.Round (temp,6);

				if(temp < 0){//加圆弧
					HasCircle = true;
					motion_1.Circle_r += temp_radius1;
					motion_1.DisplayStart = motion_bac.DisplayTarget;
					motion_1.VirtualStart = motion_bac.VirtualTarget;
					motion_1.DisplayTarget += _RadiusVec1;
					motion_1.Direction_D = motion_1.DisplayTarget - motion_1.DisplayStart;
					motion_1.Direction_V = ModalState._PosRot (MODE,motion_1.Direction_D,motion_1.Matri3);
					motion_1.VirtualTarget = motion_1.VirtualStart + motion_1.Direction_V;
					motion_1.Rotate_Speed = (motion_1.Velocity / (60f * motion_1.Circle_r)) * (180 / Mathf.PI);
					if(motion_1.Motion_Type == (int)MotionType.CR_P)
						motion_1.Rotate_Degree = CalculateDegree (motion_1.Center_Point_D,motion_1.DisplayStart,motion_1.DisplayTarget,true);
					else
						motion_1.Rotate_Degree = CalculateDegree (motion_1.Center_Point_D,motion_1.DisplayStart,motion_1.DisplayTarget,false);
					motion_1.Time_Value = motion_1.Rotate_Degree / motion_1.Rotate_Speed;
					
					Circle_center = motion_2.DisplayStart;
					Circle_startpos = motion_1.DisplayTarget;
					Circle_endpos = motion_2.DisplayStart + _RadiusVec2;
					Circle_radius = Radius;
				}else if(temp == 0){//直接连接
					motion_1.Circle_r += temp_radius1;
					motion_1.DisplayStart = motion_bac.DisplayTarget;
					motion_1.VirtualStart = motion_bac.VirtualTarget;
					motion_1.DisplayTarget += _RadiusVec1;
					motion_1.Direction_D = motion_1.DisplayTarget - motion_1.DisplayStart;
					motion_1.Direction_V = ModalState._PosRot (MODE,motion_1.Direction_D,motion_1.Matri3);
					motion_1.VirtualTarget = motion_1.VirtualStart + motion_1.Direction_V;
					motion_1.Rotate_Speed = (motion_1.Velocity / (60f * motion_1.Circle_r)) * (180 / Mathf.PI);
					if(motion_1.Motion_Type == (int)MotionType.CR_P)
						motion_1.Rotate_Degree = CalculateDegree (motion_1.Center_Point_D,motion_1.DisplayStart,motion_1.DisplayTarget,true);
					else
						motion_1.Rotate_Degree = CalculateDegree (motion_1.Center_Point_D,motion_1.DisplayStart,motion_1.DisplayTarget,false);
					motion_1.Time_Value = motion_1.Rotate_Degree / motion_1.Rotate_Speed;
				}else{//求两圆交点
					temp_crosspo = CompensationPosCal_CC (motion_1.Motion_Type,motion_1.Center_Point_D,motion_1.Circle_r+temp_radius1,motion_2.Motion_Type,motion_2.Center_Point_D,motion_2.Circle_r+temp_radius2);
					motion_1.Circle_r += temp_radius1;
					motion_1.DisplayStart = motion_bac.DisplayTarget;
					motion_1.VirtualStart = motion_bac.VirtualTarget;
					motion_1.DisplayTarget = temp_crosspo;
					motion_1.Direction_D = motion_1.DisplayTarget - motion_1.DisplayStart;
					motion_1.Direction_V = ModalState._PosRot (MODE,motion_1.Direction_D,motion_1.Matri3);
					motion_1.VirtualTarget = motion_1.VirtualStart + motion_1.Direction_V;
					motion_1.Rotate_Speed = (motion_1.Velocity / (60f * motion_1.Circle_r)) * (180 / Mathf.PI);
					if(motion_1.Motion_Type == (int)MotionType.CR_P)
						motion_1.Rotate_Degree = CalculateDegree (motion_1.Center_Point_D,motion_1.DisplayStart,motion_1.DisplayTarget,true);
					else
						motion_1.Rotate_Degree = CalculateDegree (motion_1.Center_Point_D,motion_1.DisplayStart,motion_1.DisplayTarget,false);
					motion_1.Time_Value = motion_1.Rotate_Degree / motion_1.Rotate_Speed;
				}
			}else{//先圆再直线
				_RadiusVec1 = motion_1.Center_Point_D - motion_1.DisplayTarget;
				_RadiusVec1 = Radius * _RadiusVec1/_RadiusVec1.magnitude;
				temp_radius1 = -Radius;
				if(G4x == (int)RCompEnum.RL){
					_RadiusVec2 = VecRotate (motion_2.Direction_D,Mathf.PI/2,true);
					temp_flag = -1;
					if(motion_1.Motion_Type == (int)MotionType.CR_P){
						_RadiusVec1 = - _RadiusVec1;
						temp_radius1 = -temp_radius1;
					}
				}else if(G4x == (int)RCompEnum.RR){
					_RadiusVec2 = VecRotate (motion_2.Direction_D,Mathf.PI/2,false);
					temp_flag = 1;
					if(motion_1.Motion_Type == (int)MotionType.CR_N){
						_RadiusVec1 = - _RadiusVec1;
						temp_radius1 = -temp_radius1;
					}
				}
				
				temp = Vector3.Cross(_RadiusVec1,_RadiusVec2).y * temp_flag;
				temp = (float)Math.Round (temp,6);
				
				if(temp > 0){//加圆弧
					HasCircle = true;
					motion_1.Circle_r += temp_radius1;
					motion_1.DisplayStart = motion_bac.DisplayTarget;
					motion_1.VirtualStart = motion_bac.VirtualTarget;
					motion_1.DisplayTarget += _RadiusVec1;
					motion_1.Direction_D = motion_1.DisplayTarget - motion_1.DisplayStart;
					motion_1.Direction_V = ModalState._PosRot (MODE,motion_1.Direction_D,motion_1.Matri3);
					motion_1.VirtualTarget = motion_1.VirtualStart + motion_1.Direction_V;
					motion_1.Rotate_Speed = (motion_1.Velocity / (60f * motion_1.Circle_r)) * (180 / Mathf.PI);
					if(motion_1.Motion_Type == (int)MotionType.CR_P)
						motion_1.Rotate_Degree = CalculateDegree (motion_1.Center_Point_D,motion_1.DisplayStart,motion_1.DisplayTarget,true);
					else
						motion_1.Rotate_Degree = CalculateDegree (motion_1.Center_Point_D,motion_1.DisplayStart,motion_1.DisplayTarget,false);
					motion_1.Time_Value = motion_1.Rotate_Degree / motion_1.Rotate_Speed;
					
					Circle_center = motion_2.DisplayStart;
					Circle_startpos = motion_1.DisplayTarget;
					Circle_endpos = motion_2.DisplayStart + _RadiusVec2;
					Circle_radius = Radius;
				}else if(temp == 0){//直接连接
					motion_1.Circle_r += temp_radius1;
					motion_1.DisplayStart = motion_bac.DisplayTarget;
					motion_1.VirtualStart = motion_bac.VirtualTarget;
					motion_1.DisplayTarget += _RadiusVec1;
					motion_1.Direction_D = motion_1.DisplayTarget - motion_1.DisplayStart;
					motion_1.Direction_V = ModalState._PosRot (MODE,motion_1.Direction_D,motion_1.Matri3);
					motion_1.VirtualTarget = motion_1.VirtualStart + motion_1.Direction_V;
					motion_1.Rotate_Speed = (motion_1.Velocity / (60f * motion_1.Circle_r)) * (180 / Mathf.PI);
					if(motion_1.Motion_Type == (int)MotionType.CR_P)
						motion_1.Rotate_Degree = CalculateDegree (motion_1.Center_Point_D,motion_1.DisplayStart,motion_1.DisplayTarget,true);
					else
						motion_1.Rotate_Degree = CalculateDegree (motion_1.Center_Point_D,motion_1.DisplayStart,motion_1.DisplayTarget,false);
					motion_1.Time_Value = motion_1.Rotate_Degree / motion_1.Rotate_Speed;
				}else{//求交点
					motion_1.Circle_r += temp_radius1;
					temp_crosspo = CompensationPosCal_LC (motion_2.DisplayStart+_RadiusVec2,motion_2.DisplayTarget+_RadiusVec2,motion_2.Direction_D,motion_1.Motion_Type,motion_1.Center_Point_D,motion_1.Circle_r,false);
					motion_1.DisplayStart = motion_bac.DisplayTarget;
					motion_1.VirtualStart = motion_bac.VirtualTarget;
					motion_1.DisplayTarget = temp_crosspo;
					motion_1.Direction_D = motion_1.DisplayTarget - motion_1.DisplayStart;
					motion_1.Direction_V = ModalState._PosRot (MODE,motion_1.Direction_D,motion_1.Matri3);
					motion_1.VirtualTarget = motion_1.VirtualStart + motion_1.Direction_V;
					if(motion_1.Motion_Type == (int)MotionType.CR_P)
						motion_1.Rotate_Degree = CalculateDegree (motion_1.Center_Point_D,motion_1.DisplayStart,motion_1.DisplayTarget,true);
					else
						motion_1.Rotate_Degree = CalculateDegree (motion_1.Center_Point_D,motion_1.DisplayStart,motion_1.DisplayTarget,false);
					motion_1.Rotate_Speed = (motion_1.Velocity / (60f * motion_1.Circle_r)) * (180 / Mathf.PI);
					motion_1.Time_Value = motion_1.Rotate_Degree / motion_1.Rotate_Speed;
				}
			}
		}else{//开始是直线
			if(motion_2.Motion_Type == (int)MotionType.CR_P || motion_2.Motion_Type == (int)MotionType.CR_N){
				if(G4x == (int)RCompEnum.RL){
					_RadiusVec1 = VecRotate (motion_1.Direction_D,Mathf.PI/2,true);
					_RadiusVec2 = motion_2.Center_Point_D - motion_2.DisplayStart;
					_RadiusVec2 = Radius * _RadiusVec2/_RadiusVec2.magnitude;
					temp_flag = 1;
					temp_radius1 = -Radius;
					if(motion_2.Motion_Type == (int)MotionType.CR_P){
						_RadiusVec2 = -_RadiusVec2;
						temp_radius1 = -temp_radius1;
					}
				}else if(G4x == (int)RCompEnum.RR){
					_RadiusVec1 = VecRotate (motion_1.Direction_D,Mathf.PI/2,false);
					_RadiusVec2 = motion_2.Center_Point_D - motion_2.DisplayStart;
					_RadiusVec2 = Radius * _RadiusVec2/_RadiusVec2.magnitude;
					temp_flag = -1;
					temp_radius1 = -Radius;
					if(motion_2.Motion_Type == (int)MotionType.CR_N){
						_RadiusVec2 = -_RadiusVec2;
						temp_radius1 = -temp_radius1;
					}
				}
				
				temp = Vector3.Cross(_RadiusVec1,_RadiusVec2).y * temp_flag;
				temp = (float)Math.Round (temp,6);
				
				if(temp < 0){//加圆弧
					HasCircle = true;
					motion_1.DisplayStart = motion_bac.DisplayTarget;
					motion_1.VirtualStart = motion_bac.VirtualTarget;
					motion_1.DisplayTarget += _RadiusVec1;
					motion_1.Direction_D = motion_1.DisplayTarget - motion_1.DisplayStart;
					motion_1.Direction_V = ModalState._PosRot (MODE,motion_1.Direction_D,motion_1.Matri3);
					motion_1.VirtualTarget = motion_1.VirtualStart + motion_1.Direction_V;
					motion_1.Time_Value = motion_1.Direction_D.magnitude/motion_1.Velocity*60;
					
					Circle_startpos = motion_1.DisplayTarget;
					Circle_endpos = motion_2.DisplayStart + _RadiusVec2;
					Circle_center = motion_2.DisplayStart;
					Circle_radius = Radius;
				}else if(temp == 0){//直接连接
					motion_1.DisplayStart = motion_bac.DisplayTarget;
					motion_1.VirtualStart = motion_bac.VirtualTarget;
					motion_1.DisplayTarget += _RadiusVec1;
					motion_1.Direction_D = motion_1.DisplayTarget - motion_1.DisplayStart;
					motion_1.Direction_V = ModalState._PosRot (MODE,motion_1.Direction_D,motion_1.Matri3);
					motion_1.VirtualTarget = motion_1.VirtualStart + motion_1.Direction_V;
					motion_1.Time_Value = motion_1.Direction_D.magnitude/motion_1.Velocity*60;
				}else{//求直线和圆交点
					temp_crosspo = CompensationPosCal_LC (motion_1.DisplayStart+_RadiusVec1,motion_1.DisplayTarget+_RadiusVec1,motion_1.Direction_D,motion_2.Motion_Type,motion_2.Center_Point_D,motion_2.Circle_r+temp_radius1,true);
					motion_1.DisplayStart = motion_bac.DisplayTarget;
					motion_1.VirtualStart = motion_bac.VirtualTarget;
					motion_1.DisplayTarget = temp_crosspo;
					motion_1.Direction_D = motion_1.DisplayTarget - motion_1.DisplayStart;
					motion_1.Direction_V = ModalState._PosRot (MODE,motion_1.Direction_D,motion_1.Matri3);
					motion_1.VirtualTarget = motion_1.VirtualStart + motion_1.Direction_V;
					motion_1.Time_Value = motion_1.Direction_D.magnitude/motion_1.Velocity*60;
					
				}
			}else{//直线直线
				if(G4x == (int)RCompEnum.RL){
					_RadiusVec1 = VecRotate (motion_1.Direction_D,Mathf.PI/2,true);
					_RadiusVec2 = VecRotate (motion_2.Direction_D,Mathf.PI/2,true);
					temp_flag = 1;
				}else if(G4x == (int)RCompEnum.RR){
					_RadiusVec1 = VecRotate (motion_1.Direction_D,Mathf.PI/2,false);
					_RadiusVec2 = VecRotate (motion_2.Direction_D,Mathf.PI/2,false);
					temp_flag = -1;
				}
				
				temp = Vector3.Cross(_RadiusVec1,_RadiusVec2).y * temp_flag;
				temp = (float)Math.Round (temp,6);
				
				if(temp < 0){//加圆弧
					HasCircle = true;
					motion_1.DisplayStart = motion_bac.DisplayTarget;
					motion_1.VirtualStart = motion_bac.VirtualTarget;
					motion_1.DisplayTarget += _RadiusVec1;
					motion_1.Direction_D = motion_1.DisplayTarget - motion_1.DisplayStart;
					motion_1.Direction_V = ModalState._PosRot (MODE,motion_1.Direction_D,motion_1.Matri3);
					motion_1.VirtualTarget = motion_1.VirtualStart + motion_1.Direction_V;
					motion_1.Time_Value = motion_1.Direction_D.magnitude/motion_1.Velocity*60;
					
					Circle_startpos = motion_1.DisplayTarget;
					Circle_endpos = motion_2.DisplayStart + _RadiusVec2;
					Circle_center = motion_2.DisplayStart;
					Circle_radius = Radius;
				}else if(temp == 0){//两直线同向或反向
					motion_1.DisplayStart = motion_bac.DisplayTarget;
					motion_1.VirtualStart = motion_bac.VirtualTarget;
					motion_1.DisplayTarget += _RadiusVec1;
					motion_1.Direction_D = motion_1.DisplayTarget - motion_1.DisplayStart;
					motion_1.Direction_V = ModalState._PosRot (MODE,motion_1.Direction_D,motion_1.Matri3);
					motion_1.VirtualTarget = motion_1.VirtualStart + motion_1.Direction_V;
					motion_1.Time_Value = motion_1.Direction_D.magnitude/motion_1.Velocity*60;
				}else{//求交点
					temp_crosspo = CompensationPosCal_LL (motion_1.DisplayStart+_RadiusVec1,motion_1.DisplayTarget+_RadiusVec1,motion_1.Direction_D,motion_2.DisplayStart+_RadiusVec2,motion_2.DisplayTarget+_RadiusVec2,motion_2.Direction_D);
					motion_1.DisplayStart = motion_bac.DisplayTarget;
					motion_1.VirtualStart = motion_bac.VirtualTarget;
					motion_1.DisplayTarget = temp_crosspo;
					motion_1.Direction_D = motion_1.DisplayTarget - motion_1.DisplayStart;
					motion_1.Direction_V = ModalState._PosRot (MODE,motion_1.Direction_D,motion_1.Matri3);
					motion_1.VirtualTarget = motion_1.VirtualStart + motion_1.Direction_V;
					motion_1.Time_Value = motion_1.Direction_D.magnitude/motion_1.Velocity*60;
				}
			}
		}
	}
	
	public void CalculateForCircle(MotionInfo motion_data1, MotionInfo motion_data2, ref MotionInfo motion_circle){
		Vector3 temp_dir = Vector3.Cross(_RadiusVec1,_RadiusVec2);
		if(temp_dir.y > 0){
			if(G4x == (int)RCompEnum.RL){
				motion_circle.Motion_Type = (int)MotionType.CR_N;
			}else{
				motion_circle.Motion_Type = (int)MotionType.CR_P;
			}
		}else{
			if(G4x == (int)RCompEnum.RL){
				motion_circle.Motion_Type = (int)MotionType.CR_P;
			}else{
				motion_circle.Motion_Type = (int)MotionType.CR_N;
			}
		}
		
		switch(motion_circle.Motion_Type){
		case (int)MotionType.CR_P:
			motion_circle.Rotate_Degree = CalculateDegree(motion_circle.Center_Point_D,motion_circle.DisplayStart,motion_circle.DisplayTarget,true);
			break;
		case (int)MotionType.CR_N:
			motion_circle.Rotate_Degree = CalculateDegree(motion_circle.Center_Point_D,motion_circle.DisplayStart,motion_circle.DisplayTarget,false);
			break;
		default:
			break;
		}
	}
	
	//计算两向量夹角的补角，返回角度是弧度
	private float DegreeCal(Vector3 Direction_1,Vector3 Direction_2){
		float temp_deg = 0;
		temp_deg = Vector3.Dot(Direction_1,Direction_2)/(Direction_1.magnitude*Direction_2.magnitude);
		temp_deg = (float)Math.Acos (temp_deg);
		temp_deg = Mathf.PI - temp_deg;
		return temp_deg;
	}
	
	//向量旋转并将长度调整为Radius
	//direction：true为顺时针，false为逆时针
	//_angle为弧度
	//G41顺时针旋转 G42逆时针旋转
	private Vector3 VecRotate(Vector3 _Vec,float _angle,bool direction)
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
	
	//直线和圆求交点
	//todo：返回点判断
	//LC_flag:true为先直线再圆   false为先圆再直线
	private Vector3 CompensationPosCal_LC(Vector3 LineStart_po,Vector3 LineEnd_po,Vector3 Direction,int motion_type,Vector3 C_center,float _radius,bool LC_flag){
		if(LineStart_po == LineEnd_po){
			return LineStart_po;
		}
		
		Vector3 cross_po1 = Vector3.zero;
		Vector3 cross_po2 = Vector2.zero;
		Vector3 temp_dir = Vector3.zero;
		float temp = 0f;
		float temp_x1 = 0f;
		float temp_y1 = 0f;
		float temp_x2 = 0f;
		float temp_y2 = 0f;
		
		if(LineStart_po.x == LineEnd_po.x){
			temp_x1 = LineStart_po.x;
			temp_y1 = (float)Math.Sqrt (Mathf.Abs(_radius*_radius - (temp_x1-C_center.x)*(temp_x1-C_center.x)));
			cross_po1 = new Vector3(temp_x1,C_center.y-temp_y1,C_center.z);
			cross_po2 = new Vector3(temp_x1,C_center.y+temp_y1,C_center.z);
		}else if(LineStart_po.y == LineEnd_po.y){
			temp_y1 = LineStart_po.y;
			temp_x1 = (float)Math.Sqrt (Mathf.Abs(_radius*_radius - (temp_y1-C_center.y)*(temp_y1-C_center.y)));
			cross_po1 = new Vector3(C_center.x-temp_x1,temp_y1,C_center.z);
			cross_po2 = new Vector3(C_center.x+temp_x1,temp_y1,C_center.z);
		}else{
			float a = (LineEnd_po.y - LineStart_po.y)/(LineEnd_po.x - LineStart_po.x);
			float b = LineStart_po.y - a * LineStart_po.x;
			float t1 = 1 + a*a;
			float t2 = 2*a*b - 2*a*C_center.y - 2*C_center.x;
			float t3 = C_center.x*C_center.x + (b-C_center.y)*(b-C_center.y) - _radius*_radius;
			temp = (float)Math.Sqrt(Mathf.Abs(t2*t2-4*t1*t3));
			temp_x1 = (-t2 - temp)/(2*t1);
			temp_y1 = a*temp_x1 + b;
			temp_x2 = (-t2 + temp)/(2*t1);
			temp_y2 = a*temp_x2 + b;
			cross_po1 = new Vector3(temp_x1,temp_y1,C_center.z);
			cross_po2 = new Vector3(temp_x2,temp_y2,C_center.z);
		}
		
		//只有一个交点
		if(cross_po1 == cross_po2){
			return cross_po1;
		}
		
		temp_dir = cross_po1 - cross_po2;
		float _temp = 0;
		if(temp_dir.x != 0){
			if(Direction.x/temp_dir.x < 0)
				_temp = -1;
			else
				_temp = 1;
		}else{
			if(Direction.y/temp_dir.y < 0)
				_temp = -1;
			else
				_temp = 1;
		}
		
		
		if((LC_flag && motion_type == (int)MotionType.CR_N) || (!LC_flag && motion_type == (int)MotionType.CR_P)){
			if((G4x == (int)RCompEnum.RL && _temp>0) || (G4x == (int)RCompEnum.RR && _temp<0))
				return cross_po1;
			else
				return cross_po2;
		}else{
			if((G4x == (int)RCompEnum.RL && _temp>0) || (G4x == (int)RCompEnum.RR && _temp<0))
				return cross_po2;
			else
				return cross_po1;
		}
	}
	
	//不考虑无交点及两圆重合的情况
	private Vector3 CompensationPosCal_CC(int Motion_Type_1, Vector3 C_center_1, float _r1, int Motion_Type_2, Vector3 C_center_2, float _r2)
	{
		Vector3 cross_po1 = Vector3.zero;
		Vector3 cross_po2 = Vector3.zero;
		Vector3 temp_dir1 = C_center_2 - C_center_1 ;
		Vector3 temp_dir2 = Vector3.zero;
		float temp_x1 = 0f;
		float temp_y1 = 0f;
		float temp_x2 = 0f;
		float temp_y2 = 0f;
		float temp = 0f;
		if(C_center_1.y == C_center_2.y){
			temp_x1 = Mathf.Pow (_r2,2) - Mathf.Pow (_r1,2) + Mathf.Pow (C_center_1.x,2) - Mathf.Pow (C_center_2.x,2) + Mathf.Pow (C_center_1.y,2) - Mathf.Pow (C_center_2.y,2);
			temp_x2 = temp_x1;
			temp = Mathf.Sqrt (Mathf.Abs(Mathf.Pow (_r1,2) - Mathf.Pow((temp_x1-C_center_1.x),2)));
			temp_y1 = C_center_1.y - temp;
			temp_y2 = C_center_1.y + temp;
			cross_po1 = new Vector3(temp_x1,temp_y1,C_center_1.z);
			cross_po2 = new Vector3(temp_x2,temp_y2,C_center_1.z);
		}else{
			float t1 = (Mathf.Pow(_r2,2)-Mathf.Pow(_r1,2)+Mathf.Pow(C_center_1.x,2)-Mathf.Pow(C_center_2.x,2)+Mathf.Pow(C_center_1.y,2)-Mathf.Pow(C_center_2.y,2)) / (2*(C_center_1.y-C_center_2.y));
			float t2 = -(C_center_1.x-C_center_2.x)/(C_center_1.y-C_center_2.y);
			float t3 = 1 + Mathf.Pow (t2,2);
			float t4 = 2*t1*t2 - 2*t2*C_center_1.y - 2*C_center_1.x;
			float t5 = Mathf.Pow(C_center_1.x,2) + Mathf.Pow((t1-C_center_1.y),2) - Mathf.Pow (_r1,2);
			temp = Mathf.Sqrt (Mathf.Abs(Mathf.Pow (t4,2) - 4*t3*t5));
			temp_x1 = (- t4 - temp)/(2*t3);
			temp_x2 = (- t4 + temp)/(2*t3);
			temp_y1 = t2*temp_x1 + t1;
			temp_y2 = t2*temp_x2 + t1;
			cross_po1 = new Vector3(temp_x1,temp_y1,C_center_1.z);
			cross_po2 = new Vector3(temp_x2,temp_y2,C_center_1.z);
		}
		
		if(cross_po1 == cross_po2){
			return cross_po1;
		}
		
		temp_dir2 = cross_po2 - cross_po1;
		temp = Vector3.Cross (temp_dir1,temp_dir2).y;
		temp = (float)Math.Round (temp,6);
		
		if(G4x == (int)RCompEnum.RL){
			if(Motion_Type_1 == Motion_Type_2){
				if(temp > 0)
					return cross_po2;
				else
					return cross_po1;
			}else{
				if(temp > 0)
					return cross_po1;
				else
					return cross_po2;
			}
		}else{
			if(Motion_Type_1 == Motion_Type_2){
				if(temp > 0)
					return cross_po1;
				else
					return cross_po2;
			}else{
				if(temp > 0)
					return cross_po2;
				else
					return cross_po1;
			}
		}
		
	}
	
	//两直线求交点
	private Vector3 CompensationPosCal_LL(Vector3 Startpo_1,Vector3 Endpo_1,Vector3 Direction_1,Vector3 Startpo_2,Vector3 Endpo_2,Vector3 Direction_2)
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
	
	//Circle02 :true 
	//Circle03 :false
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
	
//	private Vector3 Vec2To3(Vector2 OrigineVec){
//		Vector3 temp_vec = Vector3.zero;
//		temp_vec = new Vector3(OrigineVec.x, 0, OrigineVec.y);
//		return temp_vec;
//	}
	
}

//public class Compensation : MonoBehaviour {
//
//	// Use this for initialization
//	void Start () {
//	
//	}
//	
//	// Update is called once per frame
//	void Update () {
//	
//	}
//}
