using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public interface ICycl{
	bool Compile(List<List<string>> _List,ref int index,ref List<string> error);
	DataStore GetData();
}

public static class CompileInterface{
	
	#region Compile Control Interface
	//程序编译入口
	//改后这里实现程序加载,并编译两行
	//name:program name
	//Mode:Machine Structure Mode
	public static bool CompileEntrance(string name,int Mode){
		return CompileParas.CompileEntrance (name,Mode);
	}
	
	/* 时刻监控这个状态判断编译是否出错 */
	public static bool CompileErrorState{
		get{
			return CompileParas.bErrorStateFlag;
		}
	}
	
	/* 当编译出错时，从这个接口获取错误信息 */
	public static List<string> GetErrorMessage{
		get{
			return CompileParas.ifCompileIFace.CompileInfo;
		}
	}
	
	public static string GetRunningCode{
		get{
			return CompileParas.GetCurrentRunningCode;
		}
	}
	
	public static void SetCooZero(Vector3 vec){
		CompileParas.st_Modal.SetCooZero (vec);
	}
	#endregion
	
	#region Motion Control Interface
	public static void AutoRun(){//开始运行
		CompileParas.AutoRun ();
	}
	
	public static bool PauseState{//返回暂停状态
		get{
			bool Moveflag = Move_Motion.PauseState ();
			bool Rotflag  = Rot_Motion.PauseState ();
			return (Moveflag || Rotflag);
		}
	}
	
	public static void Pause(){//运行暂停
		Move_Motion.Pause ();
		Rot_Motion.Pause  ();
	}
	
	public static void Play(){//运行暂停恢复
		Move_Motion.Play ();
		Rot_Motion.Play  ();
	}
	
	public static void Stop(){//运行中断,严重情况调用,非严重情况使用Reset
		Move_Motion.Stop ();
		Rot_Motion.Stop  ();
		CompileParas.bRunning_flag = false;
	}
	
	public static void Reset(){//清flag
		CompileParas.MotionFalgReset ();
	}
	
	public static bool ProgRunState{
		get{
			return CompileParas.GetRunningFlag;
		}
	}
	#endregion
	
	#region MotionRate Control Interface
	public static void SetRate(float _rate){//设置运行速率倍率
		Move_Motion.SetRate (_rate);
		Rot_Motion.SetRate  (_rate);
	}
	
	public static void RateReset(){//重置运行速率倍率（1.0）
		Move_Motion.RateReset ();
		Rot_Motion.RateReset  ();
	}
	#endregion
	
	#region Spindle Control Interface
	/* 程序运行时调用了这三个接口，具体需要你们外部去实现 */
	public static void SpindleSet(bool CW,float speed){
		//todo:spindleSet
		//CW:rotate direction
		//speed:rotate speed
	}
	
	public static void SpindleSet(float speed){
		//todo:spindleSet
		//speed:rotate speed
	}
	
	public static void SpindleStop(){
		//todo:spindleStop
		//to stop the spindle from rotating
	}
	#endregion
	
	//子线程调用，不要使用Unity API，不要使用协程
	//想并行执行也使用多线程实现
	//为什么要这么做？？wen Unity
	//提供了一个互斥锁供换刀用
	public static void ToolChange(string index){
		#region 这里需要具体实现换刀
		//todo:Toolchange
		#endregion 
		
		#region 动了打死你
		//不管有没有用到多线程，都把这行放函数最后，不要去动它
		CompileParas.SemReset_Tool ();
		#endregion
	}

	public static bool DEBUG_MODE{
		set{
			CompileParas._DEBUG = value;
		}
		get{
			return CompileParas._DEBUG;
		}
	}
}

public static class PosInterface{
	#region MachineObj Get
	public static Transform GetAxis_X{
		get{
			return MachineAxis.X_Axis;
		}
	}
	
	public static Transform GetAxis_Y{
		get{
			return MachineAxis.Y_Axis;
		}
	}
	
	public static Transform GetAxis_Z{
		get{
			return MachineAxis.Z_Axis;
		}
	}
	
	public static Transform GetAxis_A{
		get{
			return MachineAxis.GetAxis_A;
		}
	}
	
	public static Transform GetAxis_B{
		get{
			return MachineAxis.GetAxis_B;
		}
	}
	
	public static Transform GetAxis_C{
		get{
			return MachineAxis.GetAxis_C;
		}
	}
	
	public static Transform GetAxis_A_Coo{
		get{
			return MachineAxis.GetAxis_A_Coo;
		}
	}
	
	public static Transform GetAxis_B_Coo{
		get{
			return MachineAxis.GetAxis_B_Coo;
		}
	}
	
	public static Transform GetAxis_C_Coo{
		get{
			return MachineAxis.GetAxis_C_Coo;
		}
	}
	#endregion

	#region PosGet
	public static Vector3 GetCurrentLocalPos{
		get{
			return CompileParas.GetCurrentLocalPos;
		}
	}
	
	public static Vector3 GetCurrentMachinePos{
		get{
			return CompileParas.GetCurrentMachinePos;
		}
	}
	
	public static Vector3 GetCurrentPartPos{
		get{
			return CompileParas.GetCurrentPartPos;
		}
	}
	
	public static Vector3 GetCurrentRot{
		get{
			return new Vector3 (CompileParas.GetAngleA, CompileParas.GetAngleB, CompileParas.GetAngleC);
		}
	}
	
	public static Vector3 GetCurrentCooRot{
		get{
			return new Vector3 (CompileParas.GetCooAngleA, CompileParas.GetCooAngleB, CompileParas.GetCooAngleC);
		}
	}
	
	public static float GetAngA{
		get{
			return CompileParas.GetAngleA;
		}
	}
	
	public static float GetAngB{
		get{
			return CompileParas.GetAngleB;
		}
	}
	
	public static float GetAngC{
		get{
			return CompileParas.GetAngleC;
		}
	}
	
	public static float GetCooAngA{
		get{
			return CompileParas.GetCooAngleA;
		}
	}
	
	public static float GetCooAngB{
		get{
			return CompileParas.GetCooAngleB;
		}
	}
	
	public static float GetCooAngC{
		get{
			return CompileParas.GetCooAngleC;
		}
	}
	#endregion
	
	#region PosCovert
	public static Vector3 PartPosToMachinePos(Vector3 vec){
		return CompileParas.PartPosToMachinePos (vec);
	}
	
	public static Vector3 MachinePosToPartPos(Vector3 vec){
		return CompileParas.MachinePosToPartPos (vec);
	}
	
	public static Vector3 LocalPosToMachinePos(Vector3 vec){
		return CompileParas.LocalPosToMachinePos (vec);
	}
	
	public static Vector3 MachinePosToLocalPos(Vector3 vec){
		return CompileParas.MachinePosToLocalPos (vec);
	}
	
	public static Vector3 LocalPosToPartPos(Vector3 vec){
		return CompileParas.LocalPosToPartPos (vec);
	}
	
	public static Vector3 PartPosToLocalPos(Vector3 vec){
		return CompileParas.PartPosToLocalPos (vec);
	}
	#endregion
}

