using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Threading;

/*********** rule for coding *************//* 
* #class    # #st# 
* #struct   # #st# 
* #list     # #ls#
* #bool     # #b #
* #string   # #s #
* #int      # #i #
* #float    # #f #
* #vector3  # #v3#
* #vector2  # #v2#
* #semaphore# #se#
* #mutex    # #m #
* #double   # #d #
* #Thread   # #th#
* #transfer # #tr#
* ## ##
* ## ##
* ## ##
* ## ##
* ## ##
* ## ##
*//******************end********************/

/* 多线程时子线程内不能出现Unity API，涉及到Unity API的加入到此类 */
public static class CompileParas_Unity{
	public static string sCodeFilePath 		= Application.streamingAssetsPath + "/Code/";
	public static string sToolFilePath 		= Application.streamingAssetsPath + "/Sysinfo/TOOL.T";
	public static string sPresetTablePath 	= Application.streamingAssetsPath + "/Sysinfo/PRESET_TABLE.P";
	public static string sCooZeroPath 		= Application.streamingAssetsPath + "/Sysinfo/CooZero.ini";
	public static string sCooLimitPath 		= Application.streamingAssetsPath + "/Sysinfo/CooLimit.ini";
	public static string sSysinfoPath 		= Application.streamingAssetsPath + "/Sysinfo/sysinfo.ini";
} 

/* ###########Warning 此类内不能出现Unity API################ */
public static class CompileParas{
	public static bool DEBUG						= false; /* % ErrorDBG-MoudleName-Max % ||Max is 143 Now||once add debug info, updates the value of Max */
	
	#region Protected Area
	public static Move_Motion st_Move;
	public static Rot_Motion st_Rot;
	#endregion
	
	#region Protected Area
//	public static HeidenhainCompileBase CompileIFace = new HeidenhainCompileBase();
	public static int iStructMode					= 1; /* 默认指定为摆头式 */
	public static ICompile ifCompileIFace 	  		= null;
	public static List<MotionInfo> lsMotionList		= new List<MotionInfo>();
	public static List<List<string>> lsCode_All		= new List<List<string>>();
	public static int iCompileIndex 				= 0;
	public static ModalState st_Modal 				= new ModalState();
	public static bool bLoadSuccess 				= false;
	public static bool bRunning_flag 				= false;
	public static bool bMotion_flag 				= false;
	
	public static Vector3 v3GetPosForThread 		= Vector3.zero;
	public static Vector3 v3GetRotForThread 		= Vector3.zero;
	public static Vector3 v3GetCooRotForThread		= Vector3.zero;
	public static Vector3 v3GetLocalPosX 			= Vector3.zero;
	public static Vector3 v3GetLocalPosY 			= Vector3.zero;
	public static Vector3 v3GetLocalPosZ			= Vector3.zero;
	public static Vector3 v3GetLocalAngX			= Vector3.zero;
	public static Vector3 v3GetLocalAngY 			= Vector3.zero;
	public static Vector3 v3GetLocalAngZ 			= Vector3.zero;
	public static Vector3 v3GetCooAngX				= Vector3.zero;
	public static Vector3 v3GetCooAngY				= Vector3.zero;
	public static Vector3 v3GetCooAngZ				= Vector3.zero;
	
	public static Matrix3x3 matrixRA				= new Matrix3x3(); /* Matrix for A rot */
	public static Matrix3x3 matrixRB				= new Matrix3x3(); /* Matrix for B rot */
	public static Matrix3x3 matrixRC				= new Matrix3x3(); /* Matrix for C rot */
	public static Matrix3x3 matrixR					= new Matrix3x3(); /* Dependent on the machine struct Mode */
	public static Matrix3x3 matrixRA_Coo			= new Matrix3x3(); /* Matrix for A_Coo rot */ /* Part to Machine */
	public static Matrix3x3 matrixRB_Coo			= new Matrix3x3(); /* Matrix for B_Coo rot */ /* Part to Machine */
	public static Matrix3x3 matrixRC_Coo			= new Matrix3x3(); /* Matrix for C_Coo rot */ /* Part to Machine */
	public static Matrix3x3 matrixR_Coo				= new Matrix3x3(); /* Dependent on the machine struct Mode */
	public static Matrix3x3 matrixRA_Coo_T			= new Matrix3x3(); /* Matrix for A_Coo_T rot */ /* Machine to Part */
	public static Matrix3x3 matrixRB_Coo_T			= new Matrix3x3(); /* Matrix for B_Coo_T rot */ /* Machine to Part */
	public static Matrix3x3 matrixRC_Coo_T			= new Matrix3x3(); /* Matrix for C_Coo_T rot */ /* Machine to Part */
	public static Matrix3x3 matrixR_Coo_T			= new Matrix3x3(); /* Dependent on the machine struct Mode */
	#endregion
	
	#region Protected Area
	public static int iCompileCount 				= 0;
	public static int iRunCount 					= 0;
	public static int iCompileRunCount 				= 0;
	#endregion
	
	#region Protected Area
	public static int iSyncCompileLine 				= 1;
	public static bool bErrorStateFlag 				= false;//to check this flag to know whether the compiling program is error or not
	public static CompileStuct_S stCopStcMoving 	= null;
	public static CompileStuct_S stCopStcCompiling 	= null;
	public static Semaphore seCompile_SemCompile 	= new Semaphore(0, iSyncCompileLine);
	public static Semaphore seCompile_SemMotion 	= new Semaphore(iSyncCompileLine, iSyncCompileLine);
	public static bool bMutex_MotionReady 			= true;//new Mutex();//= new Mutex(true);
	public static bool bMutex_RotReady 				= true;//= new Mutex();//= new Mutex(true);
	public static bool bMutex_ToolReady 			= true;//= new Mutex();//= new Mutex(true);
	public static Thread thCompileThread;
	public static Thread thMotionThread;
	public static Thread thLineThread;
	public static Thread thRotThread;
	public static Thread thLoadThread; /* For Loading Code */
	#endregion
	
	#region Protected Area
	public static void UpdatePosValue(){
		v3GetPosForThread 	 = MachineAxis.GetPos;
		v3GetRotForThread 	 = MachineAxis.GetRot;
		v3GetCooRotForThread = MachineAxis.GetCooRot;
		v3GetLocalPosX   	 = MachineAxis.VecRound (MachineAxis.X_Axis.localPosition);
		v3GetLocalPosY    	 = MachineAxis.VecRound (MachineAxis.Y_Axis.localPosition);
		v3GetLocalPosZ 	  	 = MachineAxis.VecRound (MachineAxis.Z_Axis.localPosition);
		v3GetLocalAngX 	  	 = MachineAxis.VecRound (MachineAxis.A_Axis.localEulerAngles);
		v3GetLocalAngY 	  	 = MachineAxis.VecRound (MachineAxis.B_Axis.localEulerAngles);
		v3GetLocalAngZ 	 	 = MachineAxis.VecRound (MachineAxis.C_Axis.localEulerAngles);
		v3GetCooAngX 	 	 = MachineAxis.VecRound (MachineAxis.A_Axis_Coo.localEulerAngles);
		v3GetCooAngY 	 	 = MachineAxis.VecRound (MachineAxis.B_Axis_Coo.localEulerAngles);
		v3GetCooAngZ 	 	 = MachineAxis.VecRound (MachineAxis.C_Axis_Coo.localEulerAngles);
		matrixRA		 	 = Matrix3x3.Rx (v3GetRotForThread.x);
		matrixRB		 	 = Matrix3x3.Ry (v3GetRotForThread.y);
		matrixRC 		 	 = Matrix3x3.Rz (v3GetRotForThread.z);
		matrixRA_Coo	 	 = Matrix3x3.Rx (v3GetCooRotForThread.x);
		matrixRB_Coo	 	 = Matrix3x3.Ry (v3GetCooRotForThread.y);
		matrixRC_Coo	 	 = Matrix3x3.Rz (v3GetCooRotForThread.z);
		matrixRA_Coo_T	 	 = Matrix3x3.Rx (-v3GetCooRotForThread.x);
		matrixRB_Coo_T	 	 = Matrix3x3.Ry (-v3GetCooRotForThread.y);
		matrixRC_Coo_T	 	 = Matrix3x3.Rz (-v3GetCooRotForThread.z);
		
		switch(iStructMode){
		case 1:
		case 2:
			matrixR 	  = matrixRC * matrixRA;
			matrixR_Coo	  = matrixRC_Coo * matrixRA_Coo;
			matrixR_Coo_T = matrixRC_Coo_T * matrixRA_Coo_T;
			break;
		case 3:
		case 4:
			matrixR 	  = matrixRC * matrixRB;
			matrixR_Coo   = matrixRC_Coo * matrixRB_Coo;
			matrixR_Coo_T = matrixRC_Coo_T * matrixRB_Coo_T;
			break;
		case 5:
			matrixR 	  = matrixRB * matrixRA;
			matrixR_Coo   = matrixRB_Coo * matrixRA_Coo;
			matrixR_Coo_T = matrixRB_Coo_T * matrixRA_Coo_T;
			break;
		}
	}
	#endregion
	
	#region Protected Area//to funcall throuth outer interface
	public static float GetAngleA{
		get{
			return v3GetLocalAngX.z;
		}
	}
	
	public static float GetAngleB{
		get{
			return v3GetLocalAngY.z;
		}
	}
	
	public static float GetAngleC{
		get{
			return v3GetLocalAngZ.z;
		}
	}
	
	public static float GetCooAngleA{
		get{
			return v3GetCooAngX.z;
		}
	}
	
	public static float GetCooAngleB{
		get{
			return v3GetCooAngY.z;
		}
	}
	
	public static float GetCooAngleC{
		get{
			return v3GetCooAngZ.z;
		}
	}
	
	public static bool GetRunningFlag{
		get{
			return bRunning_flag;
		}
	}
	
	public static string GetCurrentRunningCode{
		get{
			if(bRunning_flag){
				return stCopStcMoving.Motion.CodeStr;
			}else{
				return "";
			}
		}
	}
	
	public static Vector3 GetCurrentLocalPos{
		get{
			return v3GetPosForThread;
		}
	}
	
	public static Vector3 GetCurrentPartPos{
		get{
			return LocalPosToPartPos(v3GetPosForThread);
		}
	}
	
	public static Vector3 GetCurrentMachinePos{
		get{
			return LocalPosToMachinePos (v3GetPosForThread);
		}
	}
	
	public static Vector3 PartPosToMachinePos(Vector3 vec){
		Vector3 v3Ret = vec;
		v3Ret 		  = matrixR_Coo * v3Ret;
		v3Ret		  = v3Ret + (st_Modal.CooZero - st_Modal.CooLimit_Min)*1000;
		v3Ret 		  = Matrix3x3.VecRound (v3Ret);
		return v3Ret;
	}
	
	public static Vector3 MachinePosToPartPos(Vector3 vec){
		Vector3 v3Ret = vec;
		v3Ret		  = v3Ret - (st_Modal.CooZero - st_Modal.CooLimit_Min)*1000;
		v3Ret 		  = matrixR_Coo_T * v3Ret;
		v3Ret 		  = Matrix3x3.VecRound (v3Ret);
		return v3Ret;
	}
	
	public static Vector3 LocalPosToMachinePos(Vector3 vec){
		return (vec - st_Modal.CooLimit_Min) * 1000;
	}
	
	public static Vector3 MachinePosToLocalPos(Vector3 vec){
		return vec / 1000 + st_Modal.CooLimit_Min;
	}
	
	public static Vector3 LocalPosToPartPos(Vector3 vec){
		Vector3 v3Ret = (vec - Move_Motion.MIN) * 1000; /* MachinePos */
		return MachinePosToPartPos (v3Ret);
	}
	
	public static Vector3 PartPosToLocalPos(Vector3 vec){
		Vector3 v3Ret = PartPosToMachinePos (vec); /* MachinePos */
		return MachinePosToLocalPos (v3Ret);
	}
	
	public static Matrix3x3 EulerMatrixCal(Vector3 vec){
		Matrix3x3 Ret = new Matrix3x3();
		Matrix3x3 ma1 = new Matrix3x3();
		Matrix3x3 ma2 = new Matrix3x3();
		
		switch(iStructMode){
		case 1:
		case 2:
			ma1	= Matrix3x3.Rx (vec.x);
			ma2 = Matrix3x3.Rz (vec.z);
			Ret = ma2 * ma1;
			break;
		case 3:
		case 4:
			ma1	= Matrix3x3.Ry (vec.y);
			ma2 = Matrix3x3.Rz (vec.z);
			Ret = ma2 * ma1;
			break;
		case 5:
			ma1	= Matrix3x3.Rx (vec.x);
			ma2 = Matrix3x3.Ry (vec.y);
			Ret = ma2 * ma1;
			break;
		default:
			break;
		}
		
		return Ret;
	}
	#endregion
	
	#region Protected Area
	public static Vector3 DeltaRotCal(Vector3 vec){
		Vector3 Ret = vec;
		switch(iStructMode){
		case 1:
			Ret.x = -Ret.x;
			break;
		case 2:
			Ret.z = -Ret.z;
			break;
		case 3:
			//
			break;
		case 4:
			//
			break;
		case 5:
			//
			break;
		default:
			break;
		}
		
		return Ret;
	}
	#endregion
	
	#region Protected Area
	//给停止调用，释放所用资源并重新初始化信号量
	public static void SemReset(){
		seCompile_SemCompile.Close ();
		seCompile_SemMotion.Close  ();
		//Mutex_MotionReady 	= false;//.Close  ();
		//Mutex_RotReady		= false;//.Close 	 ();
		seCompile_SemCompile 	= null;
		seCompile_SemMotion 	= null;
		//Mutex_MotionReady	= null;
		//Mutex_RotReady 	= null;
		seCompile_SemCompile  	= new Semaphore(0,iSyncCompileLine);
		seCompile_SemMotion		= new Semaphore(iSyncCompileLine,iSyncCompileLine);
		bMutex_MotionReady 		= true;//= new Mutex();//new Mutex(true);
		bMutex_RotReady 		= true;//= new Mutex();//new Mutex(true);
	}
	
	//给刀具停止调用，释放所用资源并重新初始化信号量
	public static void SemReset_Tool(){
		bMutex_ToolReady = true;
		/*Mutex_ToolReady.Close ();
		Mutex_ToolReady = null;
		Mutex_ToolReady = new Mutex(true);*/
	}
	
	//其他地方禁止调用此函数
	public static void ThreadReset(){
		if(thCompileThread != null && thCompileThread.IsAlive){
			thCompileThread.Abort ();
		}
		if(thMotionThread != null  && thMotionThread.IsAlive){
			thMotionThread.Abort ();
		}
		if(thLineThread != null	   && thLineThread.IsAlive){
			thLineThread.Abort ();
		}
		if(thRotThread != null 	   && thRotThread.IsAlive){
			thRotThread.Abort ();
		}
		if(thLoadThread != null    && thLoadThread.IsAlive){
			thLoadThread.Abort ();
		}
	}

	public static bool CompileEntrance(string name,int Mode){
		if(bRunning_flag){
			return false;
		}
		
		if(Mode < 1 || Mode > 5){
			return false;
		}
		
		iStructMode				  = Mode;
		ifCompileIFace 			  = new HeidenhainCompileBase(Mode,ref st_Modal);
		bLoadSuccess 	 		  = true;
		List<string> Error_Info   = new List<string>();
		
		/* Load Code From File and Format the Code */
		/* todo:seek a way to reduce the time spending on loading,waiting for optimise */
		CompileParas.thLoadThread = new Thread(()=>
			{
				lsCode_All = Load.CodeLoad (name,ref st_Modal,ref Error_Info,ref bLoadSuccess);
			}
		);
		CompileParas.thLoadThread.Start ();
		while(CompileParas.thLoadThread != null && CompileParas.thLoadThread.IsAlive);
		
		if (!bLoadSuccess){
			ifCompileIFace.CompileInfo = Error_Info;
			return false;
		}
		
		iCompileIndex 		= 0;
		iCompileCount 		= 0;
		iRunCount 			= 0;
		iCompileRunCount 	= CompileParas.lsCode_All.Count;
		stCopStcCompiling   = null;
		stCopStcMoving 		= null;
		
		CompileStuct_S temp_stc = new CompileStuct_S(stCopStcCompiling);
		if (!temp_stc.Compile (ifCompileIFace, lsCode_All, ref iCompileIndex)){
			return false;
		}
		stCopStcCompiling	= temp_stc;
		stCopStcMoving		= temp_stc;
		
		if(CompileParas.iSyncCompileLine > 1){
			temp_stc 			= new CompileStuct_S(stCopStcCompiling);
			if (!temp_stc.Compile (ifCompileIFace, lsCode_All, ref iCompileIndex)){
				return false;
			}
			stCopStcCompiling	= temp_stc;
		}
		
		if(CompileParas.iSyncCompileLine > 2){
			temp_stc 			= new CompileStuct_S(stCopStcCompiling);
			if (!temp_stc.Compile (ifCompileIFace, lsCode_All, ref iCompileIndex)){
				return false;
			}
			stCopStcCompiling	= temp_stc;
		}
		
		iCompileCount = CompileParas.iSyncCompileLine;
		
		return true;
	}
	
	public static void AutoRun(){
		bRunning_flag = true;
		bMotion_flag  = true;
//		CompileThread.Start ();
//		MotionThread.Start ();
	}
	
	public static void MotionStop(){
		Move_Motion.Stop ();
		Rot_Motion.Stop ();
	}
	
	public static void MotionFalgReset(){
		Move_Motion.MotionFlagReset ();
		Rot_Motion.MotionFlagReset ();
	}
	#endregion
}

public static class MachineAxis{
	public static Transform A_Axis_Coo 	= GameObject.Find ("A_Axis_Coo").transform;
	public static Transform B_Axis_Coo 	= GameObject.Find ("B_Axis_Coo").transform;
	public static Transform C_Axis_Coo 	= GameObject.Find ("C_Axis_Coo").transform;
	public static Transform A_Axis 		= GameObject.Find ("A_Axis").transform;
	public static Transform B_Axis 		= GameObject.Find ("B_Axis").transform;
	public static Transform C_Axis 		= GameObject.Find ("C_Axis").transform;
	public static Transform X_Axis 		= GameObject.Find ("X_Axis").transform;
	public static Transform Y_Axis 		= GameObject.Find ("Y_Axis").transform;
	public static Transform Z_Axis 		= GameObject.Find ("Z_Axis").transform;
//	public static Move_Motion st_Move ;//= GameObject.Find ("Axis_Script").gameObject.GetComponent<Move_Motion>();
//	public static Rot_Motion st_Rot ;//= GameObject.Find ("Axis_Script").gameObject.GetComponent<Rot_Motion>();
	
	#region PosInterface
	public static Vector3 GetPos{
		get{
			float x = (float)Math.Round (X_Axis.localPosition.x, 6);
			float y = (float)Math.Round (Y_Axis.localPosition.y, 6);
			float z = (float)Math.Round (Z_Axis.localPosition.z, 6);
			return new Vector3(x, y, z);
		}
	}
	
	public static Vector3 GetRot{
		get{
//			return new Vector3(Rot_Motion.A_Rot,Rot_Motion.B_Rot,Rot_Motion.C_Rot);
			float x = (float)Math.Round (A_Axis.localEulerAngles.z, 6);
			float y = (float)Math.Round (B_Axis.localEulerAngles.z, 6);
			float z = (float)Math.Round (C_Axis.localEulerAngles.z, 6);
			return new Vector3(x, y, z);
		}
	}
	
	public static Vector3 GetCooRot{
		get{
			float x = (float)Math.Round (A_Axis_Coo.localEulerAngles.z, 6);
			float y = (float)Math.Round (B_Axis_Coo.localEulerAngles.z, 6);
			float z = (float)Math.Round (C_Axis_Coo.localEulerAngles.z, 6);
			return new Vector3(x, y, z);
		}
	}
	
	/* GetMachine relative Pos in Part Coordinary */
	public static Vector3 GetCurrentPartPos{
		get{
			Vector3 v3Ret = GetPos;
			v3Ret 		  = CompileParas.matrixR_Coo_T * v3Ret;
			v3Ret 		  = Matrix3x3.VecRound (v3Ret);
			return v3Ret;
		}
	}
	
	public static Vector3 GetCurrentMachinePos{
		get{
			return Move_Motion.CurrentVirtualPos ();
		}
	}
	
	public static Vector3 PartPosToMachinePos(Vector3 vec){
		Vector3 v3Ret = vec;
		v3Ret 		  = CompileParas.matrixR_Coo * v3Ret;
		v3Ret		  = v3Ret + (CompileParas.st_Modal.CooZero - CompileParas.st_Modal.CooLimit_Min)*1000;
		v3Ret 		  = Matrix3x3.VecRound (v3Ret);
		return v3Ret;
	}
	
	public static Vector3 MachinePosToPartPos(Vector3 vec){
		Vector3 v3Ret = vec;
		v3Ret		  = v3Ret - (CompileParas.st_Modal.CooZero - CompileParas.st_Modal.CooLimit_Min)*1000;
		v3Ret 		  = CompileParas.matrixR_Coo_T * v3Ret;
		v3Ret 		  = Matrix3x3.VecRound (v3Ret);
		return v3Ret;
	}
	
	public static Vector3 LocalPosToMachinePos(Vector3 vec){
		return Move_Motion.RelativePos_VirtualPos (vec);
	}
	
	public static Vector3 MachinePosToLocalPos(Vector3 vec){
		return Move_Motion.VirtualPos_RelativePos (vec);
	}
	
	public static Vector3 LocalPosToPartPos(Vector3 vec){
		Vector3 v3Ret = (vec - Move_Motion.MIN) * 1000; /* MachinePos */
		return MachinePosToPartPos (v3Ret);
	}
	
	public static Vector3 PartPosToLocalPos(Vector3 vec){
		Vector3 v3Ret = PartPosToMachinePos (vec); /* MachinePos */
		return MachinePosToLocalPos (v3Ret);
	}
	#endregion
	
	#region ObjGet
	public static Transform GetAxis_A{
		get{
			switch(CompileParas.iStructMode){
			case 1:
			case 2:
			case 5:
				return A_Axis;
			default:
				return null;
			}
		}
	}
	
	public static Transform GetAxis_B{
		get{
			switch(CompileParas.iStructMode){
			case 3:
			case 4:
			case 5:
				return B_Axis;
			default:
				return null;
			}
		}
	}
	
	public static Transform GetAxis_C{
		get{
			switch(CompileParas.iStructMode){
			case 1:
			case 2:
			case 3:
			case 4:
				return C_Axis;
			default:
				return null;
			}
		}
	}
	
	public static Transform GetAxis_A_Coo{
		get{
			switch(CompileParas.iStructMode){
			case 1:
			case 2:
			case 3:
			case 4:
				return A_Axis_Coo;
			default:
				return null;
			}
		}
	}
	
	public static Transform GetAxis_B_Coo{
		get{
			switch(CompileParas.iStructMode){
			case 1:
			case 2:
			case 3:
			case 4:
				return B_Axis_Coo;
			default:
				return null;
			}
		}
	}
	
	public static Transform GetAxis_C_Coo{
		get{
			switch(CompileParas.iStructMode){
			case 1:
			case 2:
			case 3:
			case 4:
				return C_Axis_Coo;
			default:
				return null;
			}
		}
	}
	#endregion
	
	public static Vector3 GetPosConverted(Vector3 vec){
		return (GetPos-vec)*1000;
	}
	
	public static Vector3 VecRound(Vector3 vec){
		float x = (float)Math.Round (vec.x, 6);
		float y = (float)Math.Round (vec.y, 6);
		float z = (float)Math.Round (vec.z, 6);
		Vector3 ret = new Vector3(x, y, z);
		return ret;
	}
}
