using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class Rot_Motion// : MonoBehaviour {
{
	#region Para
	public static float A_Rot = 0;
	public static float B_Rot = 0;
	public static float C_Rot = 0;
	
	static bool IsCooRot = false;
	static float start_time=0;
	static float end_time=0;
	static float auto_deltatime_A=0;
	static float auto_deltatime_B=0;
	static float auto_deltatime_C=0;
	static float time_A = 0;
	static float time_B = 0;
	static float time_C = 0;
	static Vector3 _Direction;
	static Vector3 _Target;
	static bool A_ROT_flag = false;
	static bool B_ROT_flag = false;
	static bool C_ROT_flag = false;
	static bool A_pn_flag;
	static bool B_pn_flag;
	static bool C_pn_flag;
	static float A_velocity;
	static float B_velocity;
	static float C_velocity;
	static float A_start_pos;
	static float B_start_pos;
	static float C_start_pos;
	static Vector3 A_start_Vec;
	static Vector3 B_start_Vec;
	static Vector3 C_start_Vec;
	static float A_end_pos;
	static float B_end_pos;
	static float C_end_pos;
	static float Temp = 0;
	static Vector3 Temp_Pos = Vector3.zero;
	static Vector3 A_temp_Pos = Vector3.zero;
	static Vector3 B_temp_Pos = Vector3.zero;
	static Vector3 C_temp_Pos = Vector3.zero;
	
	static bool IsPause = false;
	static float rate = 1f;
	
	static bool SetRotFlag = false;
	static Vector3 ARot_Set = Vector3.zero;
	static Vector3 BRot_Set = Vector3.zero;
	static Vector3 CRot_Set = Vector3.zero;
	
	static bool AaxisIsNull = false;
	static bool BaxisIsNull = false;
	static bool CaxisIsNull = false;
	
	static bool bMutex = false;
	#endregion
	
	#region TimeDeal
	static void Timer(){
		while(start_time <= end_time){
//			yield return new WaitForSeconds(0.01f);
			//Thread.Sleep (1);
		}
		IsCooRot = false;
		A_ROT_flag = false;
		B_ROT_flag = false;
		C_ROT_flag = false;
	}
	#endregion
	
	#region Control Interface
	public static void Stop(){
		CompileParas.bRunning_flag = false;
		IsPause = false;
		A_ROT_flag = false;
		B_ROT_flag = false;
		C_ROT_flag = false;
		
		Release ();/* semephore must be released,otherwise may be cause deadloop */
	}
	
	public static void MotionFlagReset(){
		CompileParas.bRunning_flag = false;
		IsPause = false;
		A_ROT_flag = false;
		B_ROT_flag = false;
		C_ROT_flag = false;
		
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
	
	public static bool RotState(){
		return (A_ROT_flag || B_ROT_flag || C_ROT_flag);
	}
	#endregion
	
	#region PosDeal
	static void SetRot(Vector3 vec){
//		MachineAxis.A_Axis.localEulerAngles = new Vector3(vec.x, A_start_Vec.y, A_start_Vec.z);
//		MachineAxis.B_Axis.localEulerAngles = new Vector3(B_start_Vec.x, vec.y, B_start_Vec.z);
//		MachineAxis.C_Axis.localEulerAngles = new Vector3(C_start_Vec.x, C_start_Vec.y, vec.z);
		ARot_Set = new Vector3(A_start_Vec.x, A_start_Vec.y, vec.x);
		BRot_Set = new Vector3(B_start_Vec.x, B_start_Vec.y, vec.y);
		CRot_Set = new Vector3(C_start_Vec.x, C_start_Vec.y, vec.z);
		A_Rot = vec.x%360; B_Rot = vec.y%360; C_Rot = vec.z%360;
		SetRotFlag = true;
	}
	
	static void SetRotFun(){
		MachineAxis.A_Axis.localEulerAngles = ARot_Set;
		MachineAxis.B_Axis.localEulerAngles = BRot_Set;
		MachineAxis.C_Axis.localEulerAngles = CRot_Set;
		SetRotFlag = false;
	}
	
	static void IsNullJudge(){
		if(MachineAxis.A_Axis == null){
			AaxisIsNull = true;
		}
		if(MachineAxis.B_Axis == null){
			BaxisIsNull = true;
		}
		if(MachineAxis.C_Axis == null){
			CaxisIsNull = true;
		}
	}
	
	static float TimeMax(float t1,float t2,float t3){
		float ret = t1 > t2 ? t1:t2;
		ret = ret > t3 ? ret:t3;
		return ret;
	}
	#endregion
	
	#region Motion Interface
	public static void Rotate(Vector3 RotDirection,Vector3 targetPos,float time1,float time2,float time3,bool _IsCooRot){
//		Debug.Log ("R_DIR " + RotDirection.ToString ("0.000000"));
//		Debug.Log ("Target " + targetPos.ToString ("0.000000"));
//		Debug.Log ("Time1  "+time1.ToString ("0.000000"));
//		Debug.Log ("Time2  "+time2.ToString ("0.000000"));
//		Debug.Log ("Time3  "+time3.ToString ("0.000000"));
		IsCooRot = _IsCooRot;
		_Direction = RotDirection;
		_Target = targetPos;
		auto_deltatime_A = 0;
		auto_deltatime_B = 0;
		auto_deltatime_C = 0;
		start_time = 0;
		time_A = time1/rate;
		time_B = time2/rate;
		time_C = time3/rate;
		end_time = TimeMax (time1,time2,time3) / rate;
		
		if(!AaxisIsNull){
			A_start_Vec = CompileParas.v3GetLocalAngX;//MachineAxis.A_Axis.localEulerAngles;
//			_Target.x = A_start_Vec.x + _Direction.x;
		}
		if(!BaxisIsNull){
			B_start_Vec = CompileParas.v3GetLocalAngY;//MachineAxis.B_Axis.localEulerAngles;
//			_Target.y = B_start_Vec.y + _Direction.y;
		}
		if(!CaxisIsNull){
			C_start_Vec = CompileParas.v3GetLocalAngZ;//MachineAxis.C_Axis.localEulerAngles;
//			_Target.z = C_start_Vec.z + _Direction.z;
		}
		
		if(!RotDirection.x.Equals (0.0f)){
			A_start_pos = CompileParas.v3GetRotForThread.x;//MachineAxis.GetRot.x;
			A_end_pos = targetPos.x;
			A_velocity = RotDirection.x * rate / time1;
			if(RotDirection.x < 0)
				A_pn_flag = false;
			else
				A_pn_flag = true;
			A_ROT_flag = true;
		}
		if(!RotDirection.y.Equals (0.0f)){
			B_start_pos = CompileParas.v3GetRotForThread.y;//MachineAxis.GetRot.y;
			B_end_pos = targetPos.y;
			B_velocity = RotDirection.y * rate / time2;
			if(RotDirection.y < 0)
				B_pn_flag = false;
			else
				B_pn_flag = true;
			B_ROT_flag = true;
		}
		if(!RotDirection.z.Equals (0.0f)){
			C_start_pos = CompileParas.v3GetRotForThread.z;//MachineAxis.GetRot.z;
			C_end_pos = targetPos.z;
			C_velocity = RotDirection.z * rate / time3;
			if(RotDirection.z < 0)
				C_pn_flag = false;
			else
				C_pn_flag = true;
			C_ROT_flag = true;
		}
		
//		yield return StartCoroutine (Timer ());
		Timer();
		SetRot (_Target);
		
		CompileParas.bMutex_RotReady = true;
	}
	#endregion
	
	#region Pretected Area Seriesly
	/* 模拟信号量，其他地方不能用 */
	public static void WaitOne(){
		while(bMutex){};
		Move_Motion.SetMutex ();
	}
	
	public static void Release(){
		bMutex = false;
	}
	
	public static void SetMutex(){
		bMutex = true;
	}
	#endregion
	
	// Update is called once per frame
	public static void Update () {
		#region update for outer use
		IsNullJudge();
		
		if(SetRotFlag){
			SetRotFun ();
		}
		
		//CompileParas.UpdatePosValue ();
		#endregion
		
		#region update for motion
		if(!IsPause && CompileParas.bRunning_flag){
			start_time += Time.deltaTime;
			
			if(end_time < 0.02f){
				auto_deltatime_A = end_time;
				auto_deltatime_B = end_time;
				auto_deltatime_C = end_time;
			}else{
				auto_deltatime_A += Time.deltaTime;
				auto_deltatime_B += Time.deltaTime;
				auto_deltatime_C += Time.deltaTime;
				if(auto_deltatime_A >= time_A)
					auto_deltatime_A = time_A;
				if(auto_deltatime_B >= time_B)
					auto_deltatime_B = time_B;
				if(auto_deltatime_C >= time_C)
					auto_deltatime_C = time_C;
			}
			
			#region A_rotCal
			if(A_ROT_flag){
				Temp = A_start_pos + auto_deltatime_A * A_velocity * rate;
//				Debug.Log ("TAB " + Temp);
				Temp = (Temp+360)%360;
//				Debug.Log ("TAA " + Temp);
				Temp_Pos = new Vector3(A_start_Vec.x, A_start_Vec.y, Temp);
				if(A_pn_flag){
					if(A_start_pos >= A_end_pos){
						if(Temp >= A_end_pos && Temp < A_start_pos)
							Temp_Pos.z = A_end_pos;
					}else{
						if(Temp >= A_end_pos)
							Temp_Pos.z = A_end_pos;
					}
				}else{
					if(A_end_pos >= A_start_pos){
						if(Temp <= A_end_pos && Temp > A_start_pos)
							Temp_Pos.z = A_end_pos;
					}else{
						if(Temp < A_end_pos)
							Temp_Pos.z = A_end_pos;
					}
				}
				A_temp_Pos = Temp_Pos;
			}
			#endregion
			
			#region B_rotCal
			if(B_ROT_flag){
				Temp = B_start_pos + auto_deltatime_B * B_velocity * rate;
				Temp = (Temp+360)%360;
				Temp_Pos = new Vector3(B_start_Vec.x, B_start_Vec.y, Temp);
				if(B_pn_flag){
					if(B_start_pos >= B_end_pos){
						if(Temp >= B_end_pos && Temp < B_start_pos)
							Temp_Pos.z = B_end_pos;
					}else{
						if(Temp >= A_end_pos)
							Temp_Pos.z = B_end_pos;
					}
				}else{
					if(B_end_pos >= B_start_pos){
						if(Temp <= B_end_pos && Temp > B_start_pos)
							Temp_Pos.z = B_end_pos;
					}else{
						if(Temp < B_end_pos)
							Temp_Pos.z = B_end_pos;
					}
				}
				B_temp_Pos = Temp_Pos;
			}
			#endregion
			
			#region C_rotCal
			if(C_ROT_flag){
				Temp = C_start_pos + auto_deltatime_C * C_velocity * rate;
				Temp = (Temp+360)%360;
				Temp_Pos = new Vector3(C_start_Vec.x, C_start_Vec.y, Temp);
				if(C_pn_flag){
					if(C_start_pos >= C_end_pos){
						if(Temp >= C_end_pos && Temp < C_start_pos)
							Temp_Pos.z = C_end_pos;
					}else{
						if(Temp >= C_end_pos)
							Temp_Pos.z = C_end_pos;
					}
				}else{
					if(C_end_pos >= C_start_pos){
						if(Temp <= C_end_pos && Temp > C_start_pos)
							Temp_Pos.z = C_end_pos;
					}else{
						if(Temp < C_end_pos)
							Temp_Pos.z = C_end_pos;
					}
				}
				C_temp_Pos = Temp_Pos;
			}
			#endregion
			
			#region Protected Area Seriesly
			/* 保护区域，动了可能会死锁 */
			Move_Motion.WaitOne ();
			if(A_ROT_flag || B_ROT_flag || C_ROT_flag){
				if(A_ROT_flag){
//					Debug.Log ("ATP " + A_temp_Pos.ToString ("0.000000"));
					if(IsCooRot){
						MachineAxis.A_Axis_Coo.localRotation = Quaternion.Euler (A_temp_Pos);
					}else{
						MachineAxis.A_Axis.localRotation = Quaternion.Euler (A_temp_Pos);
					}
					A_Rot = A_temp_Pos.x;
//					Debug.Log ("ROT " + MachineAxis.A_Axis.localRotation.ToString ("0.000000"));
//					Debug.Log ("EUL " + MachineAxis.A_Axis.localEulerAngles.ToString ("0.000000"));
				}
				if(B_ROT_flag){
//					Debug.Log ("BTP " + B_temp_Pos.ToString ("0.000000"));
					if(IsCooRot){
						MachineAxis.B_Axis_Coo.localRotation = Quaternion.Euler (B_temp_Pos);
					}else{
						MachineAxis.B_Axis.localRotation = Quaternion.Euler (B_temp_Pos);
					}
					B_Rot = B_temp_Pos.y;
				}
				if(C_ROT_flag){
//					Debug.Log ("CTP " + C_temp_Pos.ToString ("0.000000"));
					if(IsCooRot){
						MachineAxis.C_Axis_Coo.localRotation = Quaternion.Euler (C_temp_Pos);
					}else{
						MachineAxis.C_Axis.localRotation = Quaternion.Euler (C_temp_Pos);
					}
					C_Rot = C_temp_Pos.z;
				}
			}
			Release ();
			#endregion
		}
		#endregion
	}
	
}
