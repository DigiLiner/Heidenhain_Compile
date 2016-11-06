using UnityEngine;
using System.Collections;
using System;
using System.Threading;

public class Move_Motion// : MonoBehaviour {
{
	#region Para
	static float start_time=0;
	static float end_time=0;
	static float auto_deltatime=0;
	static Vector3 _Direction;
	static Vector3 _CpDirection;
	static Vector3 _Target;
	static Vector3 _Center;
	static Vector3 MoveDelta;
	static bool x_move = false;
	static bool y_move = false;
	static bool z_move = false;
	static bool x_pn_flag;//x轴移动方向 true表往正向移动 false表往负向移动
	static bool y_pn_flag;//y轴移动方向 true表往正向移动 false表往负向移动
	static bool z_pn_flag;//z轴移动方向 true表往正向移动 false表往负向移动
	static float x_start_pos;
	static float y_start_pos;
	static float z_start_pos;
	static float x_end_pos;
	static float y_end_pos;
	static float z_end_pos;
	static float x_velocity;
	static float y_velocity;
	static float z_velocity;
	static Vector3 Temp_pos = Vector3.zero;
	static Vector3 x_temp_pos = Vector3.zero;
	static Vector3 y_temp_pos = Vector3.zero;
	static Vector3 z_temp_pos = Vector3.zero;
	
	static bool rotate_flag = false;
	static float Rotate_Speed = 0;
	static Vector3 Rotate_Axis;
	static Vector3 Rotate_deltaPos;
	static Transform reference;
	static bool CP_flag = false;
	
	static Vector3 Min; /* Value in Unity */
	static Vector3 Max; /* Value in Unity */
	
	static bool IsPause = false;
	static float rate = 1f;
	
	static bool wait_flag = false;
	
	static bool PosAdjust = false;
	static Vector3 AdjX = Vector3.zero;
	static Vector3 AdjY = Vector3.zero;
	static Vector3 AdjZ = Vector3.zero;
	static bool SetRef = false;
	static Vector3 RefPos = Vector2.zero;
	
	static bool M128Flag = false;
	static Vector3 v3LastAng = Vector3.zero;
	static Vector3 v3CurrentAng = Vector3.zero;
	static bool bMutex = false;
	#endregion
	
	/* Call By CompileMultiThread.cs To Init */
	public static void Start () {
		reference = GameObject.Find ("RotReference").transform;
		Load.CooLimitLoad (ref Min,ref Max);
	}
	
	#region ValueGet Attribute
	public static bool M128{
		set{
			M128Flag = value;
		}
		get{
			return M128Flag;
		}
	}
	
	public static Vector3 MIN{
		get{
			return Min;
		}
	}
	
	public static Vector3 MAX{
		get{
			return Max;
		}
	}
	#endregion
	
	#region PosDeal
	/* 从当前虚拟坐标转换到相对坐标(unity中相对坐标) */
	public static Vector3 VirtualPos_RelativePos(Vector3 pos_vec){
		return pos_vec / 1000 + Min;
	}
	
	/* 从当前相对坐标转换到虚拟坐标(unity中相对坐标) */
	public static Vector3 RelativePos_VirtualPos(Vector3 pos_vec){
		return (pos_vec - Min) * 1000;
	}
	
	/* 获取当前虚拟坐标, 精确到小数点后3位 */
	public static Vector3 CurrentVirtualPos(){
		Vector3 CurrentVirtual = (CurrentRealPos() - Min) * 1000;
		CurrentVirtual.x = (float)Math.Round(CurrentVirtual.x, 3);
		CurrentVirtual.y = (float)Math.Round(CurrentVirtual.y, 3);
		CurrentVirtual.z = (float)Math.Round(CurrentVirtual.z, 3);
		return CurrentVirtual;
	}
	
	/* 获取当前真实的相对坐标 */
	public static Vector3 CurrentRealPos(){
		Vector3 CurrentReal = Vector3.zero;
		CurrentReal.x = CompileParas.v3GetLocalPosX.x;//MachineAxis.X_Axis.localPosition.x; //获取X轴的相对坐标
		CurrentReal.y = CompileParas.v3GetLocalPosY.y;//MachineAxis.Y_Axis.localPosition.y; //获取Z轴的相对坐标
		CurrentReal.z = CompileParas.v3GetLocalPosZ.z;//MachineAxis.Z_Axis.localPosition.z;
		return CurrentReal;
	}
	
	/* 根据真实的相对坐标，设置各个轴的位置 */
	static void SetPosition(Vector3 relative_pos){
//		Vector3 rotate_deltaPos = Vector3.zero;
		Vector3 X_rotate_bac = CompileParas.v3GetLocalPosX;
		Vector3 Y_rotate_bac = CompileParas.v3GetLocalPosY;
		Vector3 Z_rotate_bac = CompileParas.v3GetLocalPosZ;
		
		Vector3 X_rotate_pos = new Vector3(relative_pos.x, X_rotate_bac.y, X_rotate_bac.z);
		Vector3 Y_rotate_pos = new Vector3(Y_rotate_bac.x, relative_pos.y, Y_rotate_bac.z);
		Vector3 Z_rotate_pos = new Vector3(Z_rotate_bac.x, Z_rotate_bac.y, relative_pos.z); 
		
		AdjX = X_rotate_pos;
		AdjY = Y_rotate_pos;
		AdjZ = Z_rotate_pos;
		PosAdjust = true;
	}
	
	static void PosAjustFun(){
		MachineAxis.X_Axis.localPosition = AdjX;
		MachineAxis.Y_Axis.localPosition = AdjY;
		MachineAxis.Z_Axis.localPosition = AdjZ;
		PosAdjust = false;
	}
	
	public static Vector3 GetLocalX(float x){
		Vector3 ret = Vector3.zero;
		ret.x = x;
		ret.y = MachineAxis.X_Axis.localPosition.y;
		ret.z = MachineAxis.X_Axis.localPosition.z;
		return ret;
	}
	
	public static Vector3 GetLocalY(float y){
		Vector3 ret = Vector3.zero;
		ret.x = MachineAxis.Y_Axis.localPosition.x;
		ret.y = y;
		ret.z = MachineAxis.Y_Axis.localPosition.z;
		return ret;
	}
	
	public static Vector3 GetLocalZ(float z){
		Vector3 ret = Vector3.zero;
		ret.x = MachineAxis.Z_Axis.localPosition.x;
		ret.y = MachineAxis.Z_Axis.localPosition.y;
		ret.z = z;
		return ret;
	}
	
	public static void SetVirtualPos(Vector3 virtual_pos){
		Vector3 relative_pos = VirtualPos_RelativePos (virtual_pos);
		SetPosition (relative_pos);
	}
	
	public static void SetMachinePos(Vector3 pos){
		SetVirtualPos (pos);
	}
	
	public static void SetReferencePos(Vector3 Pos){
//		reference.position = Pos;
		RefPos = Pos;
		SetRef = true;
	}
	
	static void SetRefPosFun(){
		reference.position = RefPos;
		SetRef = false;
	}
	#endregion
	
	#region TimeDeal
	static void Timer(){
		while((start_time <= end_time) && (false == CompileParas._IsProgExit)){
//			yield return new WaitForSeconds(0.01f);
			//Thread.Sleep (10);
			//Debug.Log ("start"+start_time);
			//Debug.Log ("end"+end_time);
		}
		x_move = false;
		y_move = false;
		z_move = false;
		rotate_flag = false;
		wait_flag = false;
	}
	
	IEnumerator Wait(){
		while(wait_flag){
			yield return new WaitForSeconds(0.01f); 
		}
	}
	#endregion
	
	#region Control Interface
	/* seriesly funcall */
	public static void Stop(){
		CompileParas.bRunning_flag = false;
		IsPause 				   = false;
		x_move 					   = false;
		y_move 					   = false;
		z_move 					   = false;
		rotate_flag 			   = false;
		wait_flag 				   = false;
		M128Flag 				   = false;
		
		//信号量复位必须在线程复位之前
		CompileParas.SemReset ();
		CompileParas.ThreadReset ();
		Release ();/* semephore must be released,otherwise may be cause deadloop */
	}
	
	/* general funcall */
	public static void MotionFlagReset(){
		CompileParas.bRunning_flag = false;
		IsPause 				   = false;
		x_move 					   = false;
		y_move 					   = false;
		z_move 					   = false;
		rotate_flag 			   = false;
		wait_flag 				   = false;
		M128Flag 				   = false;
		
		Release ();/* semephore must be released,otherwise may be cause deadloop */
	}
	
	public static void Pause(){
		IsPause = true;
	}
	
	public static void Play(){
		IsPause = false;
	}
	
	public static bool PauseState(){
		return IsPause;
	}
	
	public static void SetRate(float _rate){
		rate = _rate;
	}
	
	public static void RateReset(){
		rate = 1f;
	}
	
	public static bool MoveState(){
		return (x_move || y_move || z_move || rotate_flag);
	}
	#endregion
	
	#region Motion Interface
	/* Target is DisplayTarget */
	public static void LineMove(Vector3 Direction, Vector3 TargetPos, float time){
//		Debug.Log (Direction.ToString ("0.000000"));
//		Debug.Log (TargetPos.ToString ("0.000000"));
//		Debug.Log ("Time " + time);
		wait_flag = true;
		_Direction = Direction;
		_Target = TargetPos;
		start_time = 0;
		auto_deltatime = 0;
		end_time = time / rate;
		MoveDelta = Vector3.zero;
		
		if(Direction.x != 0){
			x_start_pos = CompileParas.GetCurrentPartPos.x;//CompileParas.v3GetPosForThread.x;
			x_end_pos = TargetPos.x;
			x_velocity = _Direction.x * rate / time;
			if(Direction.x > 0)
				x_pn_flag = true;
			else
				x_pn_flag = false;
				
			x_move = true;
		}
		if(Direction.y != 0){
			y_start_pos = CompileParas.GetCurrentPartPos.y;//CompileParas.v3GetPosForThread.y;
			y_end_pos = TargetPos.y;
			y_velocity = _Direction.y * rate / time;
			if(Direction.y > 0)
				y_pn_flag = true;
			else
				y_pn_flag = false;
				
			y_move = true;
		}
		if(Direction.z != 0){
			z_start_pos = CompileParas.GetCurrentPartPos.z;//CompileParas.v3GetPosForThread.z;
			z_end_pos = TargetPos.z;
			z_velocity = _Direction.z * rate / time;
			if(Direction.z > 0)
				z_pn_flag = true;
			else
				z_pn_flag = false;
				
			z_move = true;
		}
		
		Timer();
		
		/* to correct the target position only when time not equals zero,which indicates that there are having motion */
		if(!time.Equals (0.000000f)){
			SetPosition (CompileParas.PartPosToLocalPos (_Target));//SetPosition (VirtualPos_RelativePos (_Target));
		}
		
		CompileParas.bMutex_MotionReady = true;
	}
	
	public static void CircleMove(Vector3 CenterPos, Vector3 Direction, Vector3 StartPos, Vector3 DisplayStart, 
								  Vector3 DisplayTarget, float RotSpeed, float time, bool cw, Vector3 rotAxis){
//		Debug.Log (CenterPos.ToString ("0.000000"));
//		Debug.Log (Direction.ToString ("0.000000"));
//		Debug.Log (TargetPos.ToString ("0.000000"));
//		Debug.Log (DisplayStart.ToString ("0.000000"));
//		Debug.Log (DisplayTarget.ToString ("0.000000"));
//		Debug.Log (RotSpeed.ToString ("0.000000"));
//		Debug.Log (rotAxis.ToString ("0.000000"));
//		Debug.Log ("Time " + time);
//		Debug.Log (cw);
		_Direction = Direction;
		_Target = DisplayTarget;
		_Center = CenterPos;
		auto_deltatime = 0;
		start_time = 0;
		end_time = time / rate;
		Rotate_Speed = RotSpeed * rate;
		
		Rotate_deltaPos = StartPos - DisplayStart;
		rotate_flag = true;
		if(cw){
			Rotate_Axis = -rotAxis;
		}else{
			Rotate_Axis = rotAxis;
		}
		SetReferencePos (DisplayStart);
		
//		yield return StartCoroutine (Timer ());
		Timer ();
		
		if(!time.Equals (0.000000f)){
			SetPosition (CompileParas.PartPosToLocalPos (_Target));//SetPosition (VirtualPos_RelativePos (_Target));
		}
		
		CompileParas.bMutex_MotionReady = true;
	}
	
	//螺旋线
	public static void CPMove(Vector3 cp, Vector3 centerPos, Vector3 Direction, 
							  Vector3 StartPos, Vector3 DisplayStart, Vector3 DisplayTarget, 
							  float RotSpeed, float RotDeg, float time, bool cw, Vector3 rotAxis){
		CP_flag = true;
		_CpDirection = cp;
		_Direction = Direction;
		_Target = DisplayTarget;
		_Center = centerPos;
		auto_deltatime = 0;
		start_time = 0;
		end_time = time/rate;
		MoveDelta = Vector3.zero;
		
		Rotate_deltaPos = StartPos - DisplayTarget;
		rotate_flag = true;
		if(cw){
			Rotate_Axis = -rotAxis;
		}else{
			Rotate_Axis = rotAxis;
		}
		
		if(Direction.x != 0){
			x_start_pos = CompileParas.GetCurrentPartPos.x;//CompileParas.v3GetLocalPosX.x;//.localPosition.x;
			x_end_pos = 0;
			x_velocity = Direction.x * rate / time;
			if(Direction.x > 0)
				x_pn_flag = true;
			else
				x_pn_flag = false;
			
			x_move = true;
		}
		if(Direction.y != 0){
			y_start_pos = CompileParas.GetCurrentPartPos.y;//CompileParas.v3GetLocalPosY.y;//MachineAxis.Y_Axis.localPosition.y;
			y_end_pos = 0;
			y_velocity = Direction.y * rate / time;
			if(Direction.y > 0)
				y_pn_flag = true;
			else
				y_pn_flag = false;
			
			y_move = true;
		}
		if(Direction.z != 0){
			z_start_pos = CompileParas.GetCurrentPartPos.z;//CompileParas.v3GetLocalPosZ.z;//MachineAxis.Z_Axis.localPosition.z;
			z_end_pos = 0;
			z_velocity = Direction.z * rate / time;
			if(Direction.z > 0)
				z_pn_flag = true;
			else
				z_pn_flag = false;
			
			z_move = true;
		}
		SetReferencePos (DisplayStart);
//		yield return StartCoroutine (Timer ());
		Timer();
		
		if(!time.Equals (0.000000f)){
			SetPosition (CompileParas.PartPosToLocalPos (_Target));//SetPosition (VirtualPos_RelativePos (_Target));
		}
		CP_flag = false;
		
		CompileParas.bMutex_MotionReady = true;
	}
	#endregion
	
	#region Pretected Area Seriesly
	/* 模拟信号量，其他地方不能用 */
	public static void WaitOne(){
		while(bMutex){};
		Rot_Motion.SetMutex ();
	}
	
	public static void Release(){
		bMutex = false;
	}
	
	public static void SetMutex(){
		bMutex = true;
	}
	#endregion
	
	public static void Update () {
		#region Update for outer use
		//CompileParas.UpdatePosValue ();
		
		if(PosAdjust){
			PosAjustFun ();
		}
		
		if(SetRef){
			SetRefPosFun ();
		}
		#endregion
		
		#region Update for motion
		if(!IsPause && CompileParas.bRunning_flag){
			start_time += Time.deltaTime;
			
			if(end_time < 0.02f){
				auto_deltatime = end_time;
			}else{
				auto_deltatime += Time.deltaTime;
				if(auto_deltatime >= end_time){
					auto_deltatime = end_time;
				}
			}
			
			#region regionForLine
			if(x_move){
				Temp_pos = new Vector3(x_start_pos + auto_deltatime * x_velocity * rate, 0, 0);
									  // MachineAxis.X_Axis.localPosition.y, 
									  // MachineAxis.X_Axis.localPosition.z);
				if(x_pn_flag){
					if(Temp_pos.x > x_end_pos){
						Temp_pos.x = x_end_pos;
					}
				}else{
					if(Temp_pos.x < x_end_pos){
						Temp_pos.x = x_end_pos;
					}
				}
				x_temp_pos = Temp_pos; /* PartPos */
				/* For M128 Cal */
//				if(M128Flag){
//					x_temp_pos = CompileParas.LocalPosToPartPos (x_temp_pos); /* PartPos */
//					x_temp_pos = CompileParas.matrixR * x_temp_pos; /* M128Cal In Part Coord */
//					x_temp_pos = CompileParas.PartPosToLocalPos (x_temp_pos); /* LocalPos */
//				}
				/*   End M128   */
			}else{
				x_temp_pos = CompileParas.GetCurrentPartPos;//MachineAxis.X_Axis.localPosition;
			}
			
			if(y_move){
				Temp_pos = new Vector3(0, y_start_pos + auto_deltatime * y_velocity * rate, 0);
									  // MachineAxis.Y_Axis.localPosition.x,
									  // y_start_pos + auto_deltatime * y_velocity * rate, 
									  // MachineAxis.Y_Axis.localPosition.z);
				if(y_pn_flag){
					if(Temp_pos.y > y_end_pos){
						Temp_pos.y = y_end_pos;
					}
				}else{
					if(Temp_pos.y < y_end_pos){
						Temp_pos.y = y_end_pos;
					}
				}
				y_temp_pos = Temp_pos; /* PartPos */
				/* For M128 Cal */
//				if(M128Flag){
//					y_temp_pos = CompileParas.LocalPosToPartPos (y_temp_pos); /* PartPos */
//					y_temp_pos = CompileParas.matrixR * y_temp_pos; /* M128Cal In Part Coord */
//					y_temp_pos = CompileParas.PartPosToLocalPos (y_temp_pos); /* LocalPos */
//				}
				/*   End M128   */
			}else{
				y_temp_pos = CompileParas.GetCurrentPartPos;//MachineAxis.Y_Axis.localPosition;
			}
			
			if(z_move){
				Temp_pos = new Vector3(0, 0, z_start_pos + auto_deltatime * z_velocity * rate);
									  // MachineAxis.Z_Axis.localPosition.x, 
									  // MachineAxis.Z_Axis.localPosition.y, 
									  // z_start_pos + auto_deltatime * z_velocity * rate);
				if(z_pn_flag){
					if(Temp_pos.z > z_end_pos){
						Temp_pos.z = z_end_pos;
					}
				}else{
					if(Temp_pos.z < z_end_pos){
						Temp_pos.z = z_end_pos;
					}
				}
				z_temp_pos = Temp_pos; /* PartPos */
				/* For M128 Cal */
//				if(M128Flag){
//					z_temp_pos = CompileParas.LocalPosToPartPos (z_temp_pos); /* PartPos */
//					z_temp_pos = CompileParas.matrixR * z_temp_pos; /* M128Cal In Part Coord */
//					z_temp_pos = CompileParas.PartPosToLocalPos (z_temp_pos); /* LocalPos */
//				}
				/*   End M128   */
			}else{
				z_temp_pos = CompileParas.GetCurrentPartPos;//MachineAxis.Z_Axis.localPosition;
			}
			
			if(M128Flag || (x_move || y_move || z_move)){
				Vector3 vec_Part = new Vector3(x_temp_pos.x, y_temp_pos.y, z_temp_pos.z);
				
				#region Protected Area Seriesly
				/* 保护区域，动了可能会死锁 */
				Rot_Motion.WaitOne ();
				v3CurrentAng = CompileParas.v3GetRotForThread;
				if(M128Flag){
					Vector3 vec = -(v3CurrentAng - v3LastAng);
					vec = CompileParas.DeltaRotCal (vec);
					vec_Part = CompileParas.EulerMatrixCal(vec) * vec_Part;
				}
				v3LastAng = CompileParas.v3GetRotForThread;
				Release ();
				#endregion
				
				Vector3 vec_Local = CompileParas.PartPosToLocalPos (vec_Part); /* LocalPos */
				x_temp_pos = new Vector3(vec_Local.x, MachineAxis.X_Axis.localPosition.y, MachineAxis.X_Axis.localPosition.z);
				y_temp_pos = new Vector3(MachineAxis.Y_Axis.localPosition.x, vec_Local.y, MachineAxis.Y_Axis.localPosition.z);
				z_temp_pos = new Vector3(MachineAxis.Z_Axis.localPosition.x, MachineAxis.Z_Axis.localPosition.y, vec_Local.z);
				MachineAxis.X_Axis.localPosition = x_temp_pos;
				MachineAxis.Y_Axis.localPosition = y_temp_pos;
				MachineAxis.Z_Axis.localPosition = z_temp_pos;
			}
			/*else if(x_move || y_move || z_move){
				if(x_move){
					
					MachineAxis.X_Axis.localPosition = x_temp_pos;
					MoveDelta.x = x_temp_pos.x - x_start_pos;
				}
				if(y_move){
					MachineAxis.Y_Axis.localPosition = y_temp_pos;
					MoveDelta.y = y_temp_pos.y - y_start_pos;
				}
				if(z_move){
					MachineAxis.Z_Axis.localPosition = z_temp_pos;
					MoveDelta.z = z_temp_pos.z - z_start_pos;
				}
			}*/
			#endregion
			
			if(rotate_flag){
				reference.RotateAround (_Center,Rotate_Axis, Rotate_Speed*Time.deltaTime);
				if(CP_flag){
					SetPosition (VirtualPos_RelativePos (reference.position + Rotate_deltaPos+MoveDelta*1000));
				}else{
					SetPosition (VirtualPos_RelativePos (reference.position + Rotate_deltaPos));
				}
			}
		}
		#endregion
		
		if(!CompileParas.bRunning_flag || IsPause){
			v3LastAng = CompileParas.v3GetRotForThread;
		}
	}

	public static void FixedUpdate(){
		CompileParas.UpdatePosValue ();
		
		if(CompileParas.bMotion_flag){
			if(!CompilMultiThread.ThreadRestart ()){
				Debug.Log ("Critical err:ThreadRestart error!");	
			}
			CompileParas.bMotion_flag = false;
		}
	}

}
