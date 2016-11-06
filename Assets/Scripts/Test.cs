using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class Test : MonoBehaviour {

	// Use this for initialization
	void Start () {
		CompileParas._DEBUG = true;
//		CompileInterface.SetRate (5);
		/*
		bool flag = CompileInterface.CompileEntrance ("xiaojiubei-111.H",2);
		Debug.Log (flag);
		List<string> Cinfo = CompileParas.ifCompileIFace.CompileInfo;
		if(!flag){
			for(int i = 0;i < Cinfo.Count;i++){
				Debug.Log (Cinfo[i]);
			}
		}*/
	}
	
	void OnGUI(){
		if(!CompileParas.bRunning_flag && GUI.Button (new Rect(10,10,50,50),"Satrt")){
			CompileInterface.CompileEntrance ("test.H",1);//("xiaojiubei-1.H",1);//
			CompileInterface.AutoRun ();
		}
//		if(GUI.Button (new Rect(10,10,50,50),"Satrt")){
//			CompilMultiThread.CompileThreadRestart ();
//		}
		if(GUI.Button (new Rect(10,70,50,50),"pause")){
			CompileInterface.Pause ();
		}
		
		if(GUI.Button (new Rect(10,130,50,50),"Play")){
			CompileInterface.Play ();
		}
		
		if(GUI.Button (new Rect(10,190,50,50),"stop")){
			CompileInterface.Stop ();
		}
		
		if(GUI.RepeatButton (new Rect(210,70,50,50),"x+")){
			MachineAxis.X_Axis.Translate (0,0,5*Time.deltaTime);
		}
		if(GUI.RepeatButton (new Rect(90,70,50,50),"x-")){
			MachineAxis.X_Axis.Translate (0,0,-5*Time.deltaTime);
		}
		
		if(GUI.RepeatButton (new Rect(150,10,50,50),"y+")){
			MachineAxis.Y_Axis.Translate (0,5*Time.deltaTime,0);
		}
		if(GUI.RepeatButton (new Rect(150,130,50,50),"y-")){
			MachineAxis.Y_Axis.Translate (0,-5*Time.deltaTime,0);
		}
		
		if(GUI.RepeatButton (new Rect(210,10,50,50),"z+")){
			MachineAxis.Z_Axis.Translate (0,5*Time.deltaTime,0);
		}
		if(GUI.RepeatButton (new Rect(210,130,50,50),"z-")){
			MachineAxis.Z_Axis.Translate (0,-5*Time.deltaTime,0);
		}
		
		if(GUI.RepeatButton (new Rect(270,10,50,50),"A+")){
			MachineAxis.A_Axis.Rotate (0,0,7*Time.deltaTime);
		}
		if(GUI.RepeatButton (new Rect(330,10,50,50),"A-")){
			MachineAxis.A_Axis.Rotate (0,0,-7*Time.deltaTime);
		}
		if(GUI.RepeatButton (new Rect(270,70,50,50),"B+")){
			MachineAxis.B_Axis.Rotate (0,0,7*Time.deltaTime);
		}
		if(GUI.RepeatButton (new Rect(330,70,50,50),"B-")){
			MachineAxis.B_Axis.Rotate (0,0,-7*Time.deltaTime);
		}
		if(GUI.RepeatButton (new Rect(270,130,50,50),"C+")){
			MachineAxis.C_Axis.Rotate (0,0,7*Time.deltaTime);
//			MachineAxis.C_Axis_Coo.localEulerAngles += new Vector3(0,0,7*Time.deltaTime);
		}
		if(GUI.RepeatButton (new Rect(330,130,50,50),"C-")){
			MachineAxis.C_Axis.Rotate (0,0,-7*Time.deltaTime);
//			MachineAxis.C_Axis_Coo.localEulerAngles -= new Vector3(0,0,7*Time.deltaTime);
		}
		
		
		Vector3 vec_rel = CompileParas.GetCurrentLocalPos;
//		Vector3 vec_vir = CompileParas.GetCurrentMachinePos;
		if(vec_rel.x > Move_Motion.MAX.x){
			MachineAxis.X_Axis.localPosition = new Vector3(Move_Motion.MAX.x,MachineAxis.X_Axis.localPosition.y,MachineAxis.X_Axis.localPosition.z);
		}
		if(vec_rel.x < Move_Motion.MIN.x){
			MachineAxis.X_Axis.localPosition = new Vector3(Move_Motion.MIN.x,MachineAxis.X_Axis.localPosition.y,MachineAxis.X_Axis.localPosition.z);
		}
		
		if(vec_rel.y > Move_Motion.MAX.y){
			MachineAxis.Y_Axis.localPosition = new Vector3(MachineAxis.Y_Axis.localPosition.x,Move_Motion.MAX.y,MachineAxis.Y_Axis.localPosition.z);
		}
		if(vec_rel.y < Move_Motion.MIN.y){
			MachineAxis.Y_Axis.localPosition = new Vector3(MachineAxis.Y_Axis.localPosition.x,Move_Motion.MIN.y,MachineAxis.Y_Axis.localPosition.z);
		}
		
		if(vec_rel.z > Move_Motion.MAX.z){
			MachineAxis.Z_Axis.localPosition = new Vector3(MachineAxis.Z_Axis.localPosition.x,MachineAxis.Z_Axis.localPosition.y,Move_Motion.MAX.z);
		}
		if(vec_rel.z < Move_Motion.MIN.z){
			MachineAxis.Z_Axis.localPosition = new Vector3(MachineAxis.Z_Axis.localPosition.x,MachineAxis.Z_Axis.localPosition.y,Move_Motion.MIN.z);
		}
		
		GUI.Label (new Rect(170,200,50,20),"X");
		GUI.Label (new Rect(170,230,50,20),"Y");
		GUI.Label (new Rect(170,260,50,20),"Z");
		GUI.Label (new Rect(200,200,50,20),CompileParas.GetCurrentMachinePos.x.ToString ());//vec_vir.x.ToString ());
		GUI.Label (new Rect(200,230,50,20),CompileParas.GetCurrentMachinePos.y.ToString ());//vec_vir.y.ToString ());
		GUI.Label (new Rect(200,260,50,20),CompileParas.GetCurrentMachinePos.z.ToString ());//vec_vir.z.ToString ());
		
		GUI.Label (new Rect(250,200,50,20),"A");
		GUI.Label (new Rect(250,230,50,20),"B");
		GUI.Label (new Rect(250,260,50,20),"C");
		GUI.Label (new Rect(280,200,50,20),CompileParas.GetAngleA.ToString ());
		GUI.Label (new Rect(280,230,50,20),CompileParas.GetAngleB.ToString ());
		GUI.Label (new Rect(280,260,50,20),CompileParas.GetAngleC.ToString ());
		
		GUI.Label (new Rect(330,200,50,20),"R_A");
		GUI.Label (new Rect(330,230,50,20),"R_B");
		GUI.Label (new Rect(330,260,50,20),"R_C");
		GUI.Label (new Rect(360,200,50,20),MachineAxis.GetCurrentPartPos.x.ToString ());
		GUI.Label (new Rect(360,230,50,20),MachineAxis.GetCurrentPartPos.y.ToString ());
		GUI.Label (new Rect(360,260,50,20),MachineAxis.GetCurrentPartPos.z.ToString ());
		
		GUI.Label (new Rect(80,280,150,30),"Code Running:");
		if(CompileParas.stCopStcMoving != null)
			GUI.Label (new Rect(170,280,150,20),CompileParas.stCopStcMoving.Motion.CodeStr);
		
		if(CompileInterface.CompileErrorState){
			for(int i = 0;i < CompileInterface.GetErrorMessage.Count;i++){
				Debug.Log (CompileInterface.GetErrorMessage[i]);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
