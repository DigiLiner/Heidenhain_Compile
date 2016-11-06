using UnityEngine;
using System.Collections;
using System.Threading;
using System;
using System.IO;


//############Warning################
//Unity多线程时子线程内不能使用unity的API
//Unity内慎用多线程，使用StartCountine代替
//###########end Warning#############
public class CompilMultiThread : MonoBehaviour {

	static bool RunInitial = false;
	static bool ComInitial = false;

	void Start () {
		Move_Motion.Start ();
		
		CompileParas.UpdatePosValue ();
	
		CompileParas.thCompileThread = new Thread(Compile_Main);
		CompileParas.thMotionThread = new Thread(Motion_Main);
	}
	
	#region ThreadControl Interface
	static bool CompileThreadRestart(){
		if(CompileParas.thCompileThread != null){
			try{
				CompileParas.thCompileThread.Abort ();
				CompileParas.thCompileThread = new Thread(Compile_Main);
				CompileParas.thCompileThread.Start ();
			}catch{
				return false;
			}
		}else{
			try{
				CompileParas.thCompileThread = new Thread(Compile_Main);
				CompileParas.thCompileThread.Start ();
			}catch{
				return false;
			}
		}
		return true;
	}
	
	static bool MotionThreadRestart(){
		if(CompileParas.thMotionThread != null){
			try{
				CompileParas.thMotionThread.Abort ();
				CompileParas.thMotionThread = new Thread(Motion_Main);
				CompileParas.thMotionThread.Start ();
			}catch{
				return false;
			}
		}else{
			try{
				CompileParas.thMotionThread = new Thread(Motion_Main);
				CompileParas.thMotionThread.Start ();
			}catch{
				return false;
			}
		}
		return true;
	}
	
	public static bool ThreadRestart(){
		return (CompileThreadRestart() && MotionThreadRestart());
	}
	
	/* To Kill Thread When Application Quit */
	void OnApplicationQuit(){
		
		/* 程序退出时让线程自己消亡，Abort操作在windows上可能会让unity主程挂死 */
		if(false == CompileParas._IsProgExit){
			print ("Waitting for Threads exit!");
			CompileParas._IsProgExit = true;
		}
		
		int iCount = 0;
		const int iCountMax = 100000; /* 等待时间 */
		while (
			(true == CompileParas._IsProgExit)     && 
			((CompileParas.thCompileThread != null && CompileParas.thCompileThread.IsAlive) ||
			 (CompileParas.thMotionThread  != null && CompileParas.thMotionThread.IsAlive)  ||
			 (CompileParas.thRotThread     != null && CompileParas.thRotThread.IsAlive)	    ||
			 (CompileParas.thLineThread    != null && CompileParas.thLineThread.IsAlive)    ||
			 (CompileParas.thLoadThread    != null && CompileParas.thLoadThread.IsAlive))
		){
			if (iCount >= iCountMax){
				print ("Thread exit Failed! Somewhere may have Deadloop or Deadlock!");
				if(CompileParas.thCompileThread != null && CompileParas.thCompileThread.IsAlive) {
					print ("Killing thCompileThread!");
				}
				if(CompileParas.thMotionThread != null  && CompileParas.thMotionThread.IsAlive) {
					print ("Killing thMotionThread!");
				}
				if(CompileParas.thRotThread != null     && CompileParas.thRotThread.IsAlive) {
					print ("Killing thRotThread!");
				}
				if(CompileParas.thLineThread != null    && CompileParas.thLineThread.IsAlive) {
					print ("Killing thLineThread!");
				}
				if(CompileParas.thLoadThread != null    && CompileParas.thLoadThread.IsAlive) {
					print ("Killing thLoadThread!");
				}
				break;
			}
			iCount++;
		};
		
		if (iCount < iCountMax) {
			print ("All threads have exited normally!");
		}
		CompileParas._IsProgExit = false;
		
		if(CompileParas.thCompileThread != null && CompileParas.thCompileThread.IsAlive){
			CompileParas.thCompileThread.Abort ();
			CompileParas.thCompileThread = null;
			print ("thCompileThread killed!");
		}
		
		if(CompileParas.thMotionThread != null && CompileParas.thMotionThread.IsAlive){
			CompileParas.thMotionThread.Abort ();
			CompileParas.thMotionThread = null;
			print ("thMotionThread killed!");
		}
		
		if(CompileParas.thRotThread != null && CompileParas.thRotThread.IsAlive){
			CompileParas.thRotThread.Abort ();
			CompileParas.thRotThread = null;
			print ("thRotThread killed!");
		}
		
		if(CompileParas.thLineThread != null && CompileParas.thLineThread.IsAlive){
			CompileParas.thLineThread.Abort ();
			CompileParas.thLineThread = null;
			print ("thLineThread killed!");
		}
		
		if(CompileParas.thLoadThread != null && CompileParas.thLoadThread.IsAlive){
			CompileParas.thLoadThread.Abort ();
			CompileParas.thLoadThread = null;
			print ("thLoadThread killed!");
		}
	}
	#endregion
	
	// Update is called once per frame
	void Update () {
		#region Update for Motion script
		Move_Motion.Update ();
		Move_Motion.FixedUpdate ();
		Rot_Motion.Update ();
		#endregion
		
		if(!CompileParas.thCompileThread.IsAlive && !CompileParas.thMotionThread.IsAlive){
			if(!RunInitial && !ComInitial){
				CompileParas.SemReset ();
				RunInitial = true;
				ComInitial = true;
			}
		}
	}
	
	
	
	#region Thread for Compile and Motion
	static void Compile_Main(){
		ComInitial = false;
		
		if (!CompileParas.bLoadSuccess){
			return;
		}
		
		while(false == CompileParas._IsProgExit){
			/* 防止编译完仍等待信号量造成死锁,正常情况下从这退出 */
			if(CompileParas.iCompileCount >= CompileParas.iCompileRunCount){
				break;
			}
			
			if(CompileParas.iCompileIndex < CompileParas.lsCode_All.Count){
				CompileParas.seCompile_SemCompile.WaitOne ();
//				print ("Compile");
				
				if(true == CompileParas._IsProgExit){
					CompileParas.seCompile_SemMotion.Release ();
					break;
				}
				
				CompileStuct_S temp_stc = new CompileStuct_S(CompileParas.stCopStcCompiling);
				CompileParas.bErrorStateFlag = !(temp_stc.Compile (CompileParas.ifCompileIFace, CompileParas.lsCode_All, ref CompileParas.iCompileIndex));
				//print ("ERROR STATE:"+CompileParas.ErrorStateFlag.ToString ());
				if(CompileParas.bErrorStateFlag){//CompileError Deal
					
					CompileParas.MotionStop ();
				}
				CompileParas.stCopStcCompiling = temp_stc;
				CompileParas.seCompile_SemMotion.Release ();
//				CompileParas.CompileIndex ++;
				CompileParas.iCompileCount ++;
			}else{
				break;
			}
		}
		Debug.Log ("Compile Thread Exits Normally!");
	}
	
	static void Motion_Main(){
		bool IsFirstMotion = true;
		RunInitial = false;
		
		if (!CompileParas.bLoadSuccess){
			return;
		}
		
		while(false == CompileParas._IsProgExit){
			/* 防止运行完仍等待信号量造成死锁,正常情况下从这退出 */
			if(CompileParas.iRunCount >= CompileParas.iCompileRunCount){
				break;
			}
			CompileParas.seCompile_SemMotion.WaitOne ();
			
			
			if(true == CompileParas._IsProgExit){
				CompileParas.seCompile_SemCompile.Release ();
				break;
			}
			
			if(!IsFirstMotion){
				CompileParas.stCopStcMoving = CompileParas.stCopStcMoving.Next;
			}
//			print ("Motion");
			if(CompileParas.stCopStcMoving == null){//运动结构next为null，则为运动到最后一行了，信号量保证运动追不上编译的速度
				CompileParas.seCompile_SemMotion.Release ();
				CompileParas.MotionStop ();
				break;
			}
			MotionStart (CompileParas.stCopStcMoving.Motion,CompileParas.stCopStcMoving.Data);
//			CompileParas.stCopStcMoving = CompileParas.stCopStcMoving.Next;
			
			CompileParas.seCompile_SemCompile.Release ();
			CompileParas.iRunCount ++;
			IsFirstMotion = false;
		}
		CompileParas.MotionFalgReset ();
		Debug.Log ("Motion Thread Exits Normally!");
	}
	#endregion
	
	#region Motion Entrance
	public static void MotionStart(MotionInfo stcMotion,DataStore stcData){
			//print ("Here");
//		for(int index = 0;index < CompileParas.MotionList.Count;index++){
//			MotionInfo stcMotion = CompileParas.stCopStcMoving.Motion;
//			DataStore stcData = CompileParas.stCopStcMoving.Data;
			//Debug.Log ("index " + CompileParas.RunCount);
			//Debug.Log ((MotionType)stcMotion.Motion_Type);
			//Debug.Log (stcMotion.CodeStr);
			//Debug.Log ("Move" + stcMotion.Direction_V.ToString ("0.000000"));
			//Debug.Log ("ROT" + stcMotion.RotDirection.ToString ("0.000000"));
			
			Move_Motion.M128 = stcMotion.M128_Flag;
			
			#region ImmediateMotion
			if(stcMotion.Immediate_Motion != ""){
				for(int i = 0;i < stcMotion.Immediate_Motion.Length;i++){
					switch(stcMotion.Immediate_Motion[i]){
					case (char)ImmediateMotionType.M00:
						//todo:
						break;
					case (char)ImmediateMotionType.M01:
						//todo:
						break;
					case (char)ImmediateMotionType.M02:
						CompileInterface.Stop ();
						CompileInterface.SpindleStop ();
						break;
					case (char)ImmediateMotionType.M03:
						CompileInterface.SpindleSet (true,stcMotion.SpindleSpeed);
						break;
					case (char)ImmediateMotionType.M04:
						CompileInterface.SpindleSet (false,stcMotion.SpindleSpeed);
						break;
					case (char)ImmediateMotionType.M05:
						CompileInterface.SpindleStop ();
						break;
					case (char)ImmediateMotionType.M08:
						break;
					case (char)ImmediateMotionType.M09:
						break;						
					case (char)ImmediateMotionType.M30:
						CompileInterface.Stop ();
						CompileInterface.SpindleStop ();
						break;
					case (char)ImmediateMotionType.RotateSpeed:
						CompileInterface.SpindleSet (stcMotion.SpindleSpeed);
						break;
					case (char)ImmediateMotionType.CYCL7:
						break;
					case (char)ImmediateMotionType.CYCL247:
						CompileParas.st_Modal.SetCooZero (stcMotion.CooZero);
						break;
					}
				}
			}
			#endregion
			
			#region Motion
			if(stcMotion.Motion_Type != -1){
				switch(stcMotion.Motion_Type){
				#region BLKFORM
				case (int)MotionType.BLKFORM1:
				case (int)MotionType.BLKFORM2:
					break;
				#endregion
				#region TOOLCALL
				case (int)MotionType.TOOLCALL:
//					yield return StartCoroutine (CompileInterface.ToolChange (CompileParas.MotionList[index].ToolNum));
					CompileParas.bMutex_ToolReady = false;
					CompileInterface.ToolChange (stcMotion.ToolNum);
					while(!CompileParas.bMutex_ToolReady);
					break;
				#endregion
				#region Line
				case (int)MotionType.Line:
//					StartCoroutine(LineMove (CompileParas.MotionList[index].Direction_V,CompileParas.MotionList[index].VirtualTarget,CompileParas.MotionList[index].Time_Value));
//					yield return StartCoroutine (MachineAxis.st_Rot.Rotate (CompileParas.MotionList[index].RotDirection,CompileParas.MotionList[index].RotTarget,CompileParas.MotionList[index].Time_Value_Rot[0],CompileParas.MotionList[index].Time_Value_Rot[1],CompileParas.MotionList[index].Time_Value_Rot[2],false));
//					yield return StartCoroutine (Wait ());
					CompileParas.bMutex_MotionReady = false;
					CompileParas.bMutex_RotReady = false;
					CompileParas.thLineThread = new Thread(()=>
						{
				    		Move_Motion.LineMove (stcMotion.Direction_V,stcMotion.DisplayTarget,stcMotion.Time_Value);
				    	}
			    	);
			    	CompileParas.thRotThread = new Thread(()=>
			    		{
							Rot_Motion.Rotate (stcMotion.RotDirection, stcMotion.RotTarget, stcMotion.Time_Value_Rot[0],
											   stcMotion.Time_Value_Rot[1], stcMotion.Time_Value_Rot[2], false);
						}
			    	);
					CompileParas.thLineThread.Start ();
					CompileParas.thRotThread.Start ();
					while(!CompileParas.bMutex_MotionReady || !CompileParas.bMutex_RotReady);
					break;
				#endregion
				#region CR_P
				case (int)MotionType.CR_P:
//					yield return StartCoroutine (CircleMove (CompileParas.MotionList[index].Center_Point_D,CompileParas.MotionList[index].Direction_V,CompileParas.MotionList[index].VirtualTarget,CompileParas.MotionList[index].DisplayStart,CompileParas.MotionList[index].DisplayTarget,CompileParas.MotionList[index].Rotate_Speed,CompileParas.MotionList[index].Time_Value,false,CompileParas.MotionList[index].ToolVec));
					CompileParas.bMutex_MotionReady = false;
					Move_Motion.CircleMove (stcMotion.Center_Point_D, stcMotion.Direction_V, stcMotion.VirtualStart, stcMotion.DisplayStart,
											stcMotion.DisplayTarget, stcMotion.Rotate_Speed, stcMotion.Time_Value, true, stcMotion.ToolVec);
					while(!CompileParas.bMutex_MotionReady);
					break;
				#endregion
				#region CR_N
				case (int)MotionType.CR_N:
//					yield return StartCoroutine (CircleMove (CompileParas.MotionList[index].Center_Point_D,CompileParas.MotionList[index].Direction_V,CompileParas.MotionList[index].VirtualTarget,CompileParas.MotionList[index].DisplayStart,CompileParas.MotionList[index].DisplayTarget,CompileParas.MotionList[index].Rotate_Speed,CompileParas.MotionList[index].Time_Value,true,CompileParas.MotionList[index].ToolVec));
					CompileParas.bMutex_MotionReady = false;
					Move_Motion.CircleMove (stcMotion.Center_Point_D, stcMotion.Direction_V, stcMotion.VirtualStart, stcMotion.DisplayStart,
											stcMotion.DisplayTarget, stcMotion.Rotate_Speed, stcMotion.Time_Value, false, stcMotion.ToolVec);
					while(!CompileParas.bMutex_MotionReady);
					break;
				#endregion
				#region CP
				case (int)MotionType.CP:
					bool cw = false;
					if(stcMotion.DR_PN > 0)
						cw = true;
					else
						cw = false;
//					yield return StartCoroutine (CPMove (CompileParas.MotionList[index].CP_Direction,CompileParas.MotionList[index].Center_Point_D,CompileParas.MotionList[index].Direction_V,CompileParas.MotionList[index].VirtualTarget,CompileParas.MotionList[index].DisplayStart,CompileParas.MotionList[index].DisplayTarget,CompileParas.MotionList[index].Rotate_Speed,CompileParas.MotionList[index].Rotate_Degree,CompileParas.MotionList[index].Time_Value,cw,CompileParas.MotionList[index].ToolVec));
					CompileParas.bMutex_MotionReady = false;
					Move_Motion.CPMove (stcMotion.CP_Direction, stcMotion.Center_Point_D, stcMotion.Direction_V, stcMotion.VirtualStart, 
										stcMotion.DisplayStart, stcMotion.DisplayTarget, stcMotion.Rotate_Speed, stcMotion.Rotate_Degree,
										stcMotion.Time_Value, cw, stcMotion.ToolVec);
					while(!CompileParas.bMutex_MotionReady);
					break;
				#endregion
				#region CYCL19
				case (int)MotionType.CYCL19:
					Vector3 Dir = new Vector3(stcMotion.RotDirection.x,0,0);
					Vector3 tar = new Vector3(stcMotion.RotTarget.x,CompileParas.v3GetRotForThread.y,CompileParas.v3GetRotForThread.z);
//					yield return StartCoroutine (MachineAxis.st_Rot.Rotate (Dir,tar,CompileParas.MotionList[index].Time_Value_Rot[0],0,0,false));
					CompileParas.bMutex_RotReady = false;
					Rot_Motion.Rotate (Dir,tar,stcMotion.Time_Value_Rot[0],0,0,false);
					while(!CompileParas.bMutex_RotReady);
					
					Dir = new Vector3(0,stcMotion.RotDirection.y,0);
					tar = new Vector3(CompileParas.v3GetRotForThread.x,stcMotion.RotTarget.y,CompileParas.v3GetRotForThread.z);
//					yield return StartCoroutine (MachineAxis.st_Rot.Rotate (Dir,tar,0,CompileParas.MotionList[index].Time_Value_Rot[0],0,false));
					CompileParas.bMutex_RotReady = false;
					Rot_Motion.Rotate (Dir,tar,0,stcMotion.Time_Value_Rot[0],0,false);
					while(!CompileParas.bMutex_RotReady);
					
					Dir = new Vector3(0,0,stcMotion.RotDirection.z);
					tar = new Vector3(CompileParas.v3GetRotForThread.x,CompileParas.v3GetRotForThread.y,stcMotion.RotTarget.z);
//					yield return StartCoroutine (MachineAxis.st_Rot.Rotate (Dir,tar,0,0,CompileParas.MotionList[index].Time_Value_Rot[0],false));
					CompileParas.bMutex_RotReady = false;
					Rot_Motion.Rotate (Dir,tar,0,0,stcMotion.Time_Value_Rot[0],false);
					while(!CompileParas.bMutex_RotReady);
					break;
				#endregion
				#region PLANE
				case (int)MotionType.PLANE:
					Vector3 dir = new Vector3(stcMotion.RotDirection.x,0,0);
					Vector3 _tar = new Vector3(stcMotion.RotTarget.x,CompileParas.v3GetRotForThread.y,CompileParas.v3GetRotForThread.z);
//					yield return StartCoroutine (MachineAxis.st_Rot.Rotate (dir,_tar,CompileParas.MotionList[index].Time_Value_Rot[0],0,0,false));
					CompileParas.bMutex_RotReady = false;
					Rot_Motion.Rotate (dir,_tar,stcMotion.Time_Value_Rot[0],0,0,false);
					while(!CompileParas.bMutex_RotReady);
					
					dir = new Vector3(0,stcMotion.RotDirection.y,0);
					_tar = new Vector3(CompileParas.v3GetRotForThread.x,stcMotion.RotTarget.y,CompileParas.v3GetRotForThread.z);
//					yield return StartCoroutine (MachineAxis.st_Rot.Rotate (dir,_tar,0,stcMotion.Time_Value_Rot[0],0,false));
					CompileParas.bMutex_RotReady = false;//.WaitOne ();
					Rot_Motion.Rotate (dir,_tar,0,stcMotion.Time_Value_Rot[0],0,false);
					while(!CompileParas.bMutex_RotReady);//.WaitOne ();
					
					dir = new Vector3(0,0,stcMotion.RotDirection.z);
					_tar = new Vector3(CompileParas.v3GetRotForThread.x,CompileParas.v3GetRotForThread.y,stcMotion.RotTarget.z);
//					yield return StartCoroutine (MachineAxis.st_Rot.Rotate (dir,_tar,0,0,CompileParas.MotionList[index].Time_Value_Rot[0],false));
					CompileParas.bMutex_RotReady = false;//.WaitOne ();
					Rot_Motion.Rotate (dir,_tar,0,0,stcMotion.Time_Value_Rot[0],false);
					while(!CompileParas.bMutex_RotReady);//.WaitOne ();
					break;
				#endregion
				#region CYCL32
				case (int)MotionType.CYCL32:
					break;
				#endregion
				#region LIST
				case (int)MotionType.LIST:
					for(int i = 0;i < stcMotion.Motion_Type_List.Count;i++){
						switch(stcMotion.Motion_Type_List[i]){
						case (int)MotionType.Line:
							CompileParas.bMutex_MotionReady = false;
							CompileParas.bMutex_RotReady = false;
							CompileParas.thLineThread = new Thread(()=>
							{
								Move_Motion.LineMove (stcMotion.Direction_V_List[i], stcMotion.DisplayTarget_List[i], stcMotion.TimeValueList[i]);
							}
							);
							CompileParas.thRotThread = new Thread(()=>
							{
								Rot_Motion.Rotate (stcMotion.RotDirection_List[i], stcMotion.RotTarget_List[i], stcMotion.TimeValueRot_List[i][0], 
												   stcMotion.TimeValueRot_List[i][1], stcMotion.TimeValueRot_List[i][2], false);
							}
							);
							CompileParas.thLineThread.Start ();
							CompileParas.thRotThread.Start ();
							while(!CompileParas.bMutex_MotionReady || !CompileParas.bMutex_RotReady);
							break;
						case (int)MotionType.CR_P:
							CompileParas.bMutex_MotionReady = false;
							Move_Motion.CircleMove (stcMotion.Center_Point_D_List[i], stcMotion.Direction_V_List[i], stcMotion.VirtualStart_List[i], 
													stcMotion.DisplayStart_List[i], stcMotion.DisplayTarget_List[i], stcMotion.Rotate_Speed_List[i], 
													stcMotion.TimeValueList[i], true, stcMotion.ToolVec_List[i]);
							while(!CompileParas.bMutex_MotionReady);
						 	break;
						case (int)MotionType.CR_N:
							CompileParas.bMutex_MotionReady = false;
							Move_Motion.CircleMove (stcMotion.Center_Point_D_List[i], stcMotion.Direction_V_List[i], stcMotion.VirtualStart_List[i],
						                        	stcMotion.DisplayStart_List[i], stcMotion.DisplayTarget_List[i], stcMotion.Rotate_Speed_List[i],
						                        	stcMotion.TimeValueList[i], false, stcMotion.ToolVec_List[i]);
							while(!CompileParas.bMutex_MotionReady);
						 	break;
						default:
							Debug.Log ("UnSupportted Motion Type:"+(stcMotion.Motion_Type_List[i]));
						 	break; 
						}
					}
					break;
					#endregion	
				}
			}
			#endregion
//		}
		
//		CompileParas.Running_flag = false;
	}
	#endregion
}
