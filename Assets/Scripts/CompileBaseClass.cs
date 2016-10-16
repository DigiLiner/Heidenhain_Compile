using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class ModalState{
	#region Para
	private int _Mode 			= 2; 					/* machine struct type Mode */
	public string PGMName 		= ""; 					/* program name to compile */
	public bool PGM_Start 		= false; 				/* program start flag */
	public MUnit Unit 			= MUnit.Metric; 		/* the Unit use in the program,Metric and Inch,Inch not support now */
	public int RCompensation 	= (int)RCompEnum.R0;	/* 半径补偿 */
	public float L_Value 		= 0; 					/* L value signed in tool call segment */
	public float R_Value 		= 0; 					/* R value signed in tool call segment */
	public float DL_Value 		= 0; 					/* DL value signed in tool call segment or get from tool table */
	public float DR_Value 		= 0; 					/* DR value signed in tool call segment or get from tool table */
	public float L_Value_bak 	= 0; 					/* L value signed in tool call segment */
	public float R_Value_bak 	= 0; 					/* R value signed in tool call segment */
	public float DL_Value_bak 	= 0; 					/* DL value signed in tool call segment or get from tool table */
	public float DR_Value_bak 	= 0; 					/* DR value signed in tool call segment or get from tool table */
	public bool TValueBakFlag 	= false; 
	public int ToolAxis 		= (int)Axis.NO; 		/* Main Axis Name X/Y/Z */
	public Vector3 ToolVec 		= new Vector3(0,0,1); 	/* Main Axis Direction Vector */
	public Vector3 ToolVecBak	= new Vector3(0,0,1);
	public Vector3 PlaneAng 	= Vector3.zero; 		/* The Ang of Plane to be cutting */
	public Matrix3x3 RX 		= Matrix3x3.identity; 	/* the RX Matrix of the Plane */
	public Matrix3x3 RY 		= Matrix3x3.identity; 	/* the RY Matrix of the Plane */
	public Matrix3x3 RZ 		= Matrix3x3.identity; 	/* the RZ Matrix of the Plane */
	
	public List<int> Line_Index 							= new List<int>();//每行指令的原始索引
	public Dictionary<string,float> Q_Value 				= new Dictionary<string, float>();
	public Dictionary<string,List<List<string>>> LBL_List 	= new Dictionary<string, List<List<string>>>();
	public Dictionary<string,LineArea> LBL_LineNumber 		= new Dictionary<string, LineArea>();
	public List<CALL_LBL> CallLBL_info 						= new List<CALL_LBL>();
	public Dictionary<string,ToolInfo> ToolList 			= new Dictionary<string, ToolInfo>();
	public List<PresetTableInfo> PresetTable_List 			= new List<PresetTableInfo>();
	
	private Dictionary<string,string> Code_Type 			= new Dictionary<string, string>();
	
	public Vector3 BLKFORM_1 	= Vector3.zero; /* blank form 1 signed in program */
	public Vector3 BLKFORM_2 	= Vector3.zero; /* blank form 2 signed in program */
	
	public bool M116 			= true;			/* ABC轴以mm/min  M117以°/min */
	public bool M126			= false;		/* 旋转轴短路径运动 */
	public bool M128 			= false;		/* 保持刀尖位置 */
	public float M128_speed 	= 0;
	public float M140_speed 	= 0;
	public bool M136 			= false;		/* 运动mm/pr  M137mm/min */
	public bool M94 			= false;
	public bool M91 			= false;
	public Vector3 M91_vec 		= Vector3.zero;
	
	private int currentMotion;  				/* 当前运动方式 */
	public float FeedSpeed 		= 0; 			/* speed for line motion */
	public float F_Value 		= 0; 			/* F value to calculate line motion */
	private float FMax 			= 10000;		/* 最大速度 */
	private float rotspeed 		= 0; 			/* speed for circle motion */
	public float RotSpeed_bac 	= 0; 			/* speed for Rot backup */
	public float RotVelocity 	= 10000; 		/* speed for Rot */
	public Vector3 CooZero; 					/* part Coordi region origin */
	private Vector3 CooMin;						/* part Coordi region min */
	private Vector3 CooMax; 					/* Part Coordi region max */
	public string ToolName; 					/* tool name be called */
	//圆运动用
	//public bool[] C_State = new bool[3]{false,false,false};
	public bool CC_Flag 		= false;
	public Vector3 CC 			= Vector3.zero;
	//public Vector3 C_Start = Vector3.zero;
	#endregion
	
	public ModalState(){
		ToolAxis 			= (int)Axis.Z;
		
		CodeType_Initial ();
		
		ToolList 			= Load.ToolLoad ();
		PresetTable_List 	= Load.PresetTableLoad ();
		CooZero 			= Load.CooZeroLoad ();
		
		Load.CooLimitLoad (ref CooMin,ref CooMax);
		Load.sysinfoLoad (ref FMax,ref RotVelocity);
		
		M140_speed 			= FMax;
		M128_speed 			= FMax;
	}
	
	public ModalState(int mode){
		_Mode 				= mode;
		
		CodeType_Initial ();
		
		ToolList 			= Load.ToolLoad ();
		PresetTable_List 	= Load.PresetTableLoad ();
		CooZero 			= Load.CooZeroLoad ();
		
		Load.CooLimitLoad (ref CooMin,ref CooMax);
		Load.sysinfoLoad (ref FMax,ref RotVelocity);
		
		M140_speed 			= FMax;
		M128_speed 			= FMax;
	}
	
	public int CurrentMotion{
		get{
			return currentMotion;
		}
		set{
			currentMotion = value;
		}
	}
	
	public void CC_Clear(){
//		for(int i = 0;i < 3;i++){
//			C_State[i] = false;
//		}
		CC 		= Vector3.zero;
		CC_Flag = false;
//		C_Start = Vector3.zero;
	}

	public int MODE{
		set{
			_Mode = value;
		}
	}

	public float FMAX{
		get{
			return FMax;
		}
	}

	public float RotSpeed{
		set{
			RotSpeed_bac	= rotspeed;
			rotspeed 		= value;
		}get{
			return rotspeed;
		}
	}
	
	public float RotSpeedReview(){
		rotspeed = RotSpeed_bac;
		return rotspeed;
	}
	
	private void CodeType_Initial(){
		string[] Code = new string[]{"BEGIN", "END", "BLKFORM", "CYCLDEF", "PLANE", "CR", 
									"L", "CC", "CP", "C", "DR+", "DR-", "R0", "RR", "RL", 
									"FN0", "TOOL", "FMAX", "MB", "MAX"};
									
		string[] Type = new string[]{"BEGIN", "END", "BLKFORM", "CYCLDEF", "PLANE", "CR", 
									"Line", "CC", "CP", "Circle", "DR_CW", "DR_CW", "R0", 
									"RR", "RL", "FN0", "TOOL", "FMAX", "MB", "MAX"};
		
		for(int i = 0;i < Code.Length;i++){
			Code_Type.Add (Code[i],Type[i]);
		}
	}
	
	public string CodeTypeCheck(string aim_code){
		aim_code = aim_code.ToUpper ();
		string RET = "";
		
		if(Code_Type.ContainsKey (aim_code)){
			return Code_Type[aim_code];
		}else if(aim_code.StartsWith ("DR") || aim_code.StartsWith ("DL")){
			RET = aim_code.Substring (0,2);
		}else if(aim_code.StartsWith ("RND") || aim_code.StartsWith ("CHF")){
			RET = aim_code.Substring (0,3);
		}else{
			Regex reg = new Regex(@"(^((I?[ABCXYZ])|([RLMFS])|(IPA)){1}[+-]?Q?\d*.?\d*)");
			
			if(reg.IsMatch (aim_code)){
				if(aim_code.StartsWith ("IPA")){
					RET = aim_code.Substring (0,3);
				}else if(aim_code.StartsWith ("I")){
					RET = aim_code.Substring (0,2);
				}else{
					RET = aim_code.Substring (0,1);
				}
			}else{
				Regex reg1=new Regex(@"^Q[0-9]+( )*=( )*[+-]?\d+.?\d*");
				if(reg1.IsMatch (aim_code)){
					RET = "QSet";
				}else
					RET = aim_code;
			}
		}
		
		return RET;
	}

	public PresetTableInfo LocalCoordinate(){
		int Index = -1;
		if(Q_Value.ContainsKey("Q339")){
			Index = (int)Q_Value["Q339"];
		}else{
			Q_Value.Add ("Q339",0);
			Index = 0;
		}
		if(Index > PresetTable_List.Count){
			Index = 0;
		}
		return PresetTable_List[Index];
	}
	
	public void SetCooZero(Vector3 vec){
		CooZero.x = vec.x;
		CooZero.y = vec.y;
		CooZero.z = vec.z;
	}
	
	public Vector3 CooLimit_Max{
		get{
			return CooMax;
		}
	}
	
	public Vector3 CooLimit_Min{
		get{
			return CooMin;
		}
	}
	
	public Vector3 RotA(float Alpha){
		Alpha 			= Alpha * Mathf.Deg2Rad;
		float cos 		= Mathf.Cos (Alpha);
		float sin 		= Mathf.Sin (Alpha);
		Vector3 v0 		= new Vector3(1,0,0);
		Vector3 v1 		= new Vector3(0,cos,sin);
		Vector3 v2 		= new Vector3(0,-sin,cos);
		
		RX.SetRow (0,v0);
		RX.SetRow (1,v1);
		RX.SetRow (2,v2);
		
		ToolVecBak		= ToolVec;
		ToolVec 		= new Vector3(0,0,1);
		ToolVec 		= RX * ToolVec;
		
		return ToolVec;
	}
	
	public static Matrix3x3 RX_Get(float Alpha){
		/*Alpha 			= Alpha * Mathf.Deg2Rad;
		float cos 		= Mathf.Cos (Alpha);
		float sin 		= Mathf.Sin (Alpha);
		Vector3 v0 		= new Vector3(1,0,0);
		Vector3 v1 		= new Vector3(0,cos,sin);
		Vector3 v2 		= new Vector3(0,-sin,cos);
		Matrix3x3 RX_R 	= new Matrix3x3();
		
		RX_R.SetRow (0,v0);
		RX_R.SetRow (1,v1);
		RX_R.SetRow (2,v2);
		
		return RX_R;*/
		return Matrix3x3.Rx (Alpha);
	}
	
	public Vector3 RotB(float Beta){
		Beta 			= Beta * Mathf.Deg2Rad;
		float cos 		= Mathf.Cos (Beta);
		float sin 		= Mathf.Sin (Beta);
		Vector3 v0 		= new Vector3(cos,0,-sin);
		Vector3 v1 		= new Vector3(0,1,0);
		Vector3 v2 		= new Vector3(sin,0,cos);
		
		RY.SetRow (0,v0);
		RY.SetRow (1,v1);
		RY.SetRow (2,v2);
		
		ToolVecBak		= ToolVec;
		ToolVec 		= new Vector3(0,0,1);
		ToolVec 		= RY * ToolVec;
		
		return ToolVec;
	}
	
	public static Matrix3x3 RY_Get(float Beta){
		/*Beta 			= Beta * Mathf.Deg2Rad;
		float cos 		= Mathf.Cos (Beta);
		float sin 		= Mathf.Sin (Beta);
		Vector3 v0 		= new Vector3(cos,0,-sin);
		Vector3 v1 		= new Vector3(0,1,0);
		Vector3 v2 		= new Vector3(sin,0,cos);
		Matrix3x3 RY_R 	= new Matrix3x3();
		
		RY_R.SetRow (0,v0);
		RY_R.SetRow (1,v1);
		RY_R.SetRow (2,v2);
		
		return RY_R;*/
		return Matrix3x3.Ry (Beta);
	}
	
	public Vector3 RotC(float Gamma){
		Gamma 			= Gamma * Mathf.Deg2Rad;
		float cos 		= Mathf.Cos (Gamma);
		float sin 		= Mathf.Sin (Gamma);
		Vector3 v0 		= new Vector3(cos,sin,0);
		Vector3 v1 		= new Vector3(-sin,cos,0);
		Vector3 v2 		= new Vector3(0,0,1);
		
		RZ.SetRow (0,v0);
		RZ.SetRow (1,v1);
		RZ.SetRow (2,v2);
		
		ToolVecBak		= ToolVec;
		ToolVec 		= new Vector3(0,0,1);
		ToolVec 		= RZ * ToolVec;
		
		return ToolVec;
	}
	
	public static Matrix3x3 RZ_Get(float Gamma){
		/*Gamma 			= Gamma * Mathf.Deg2Rad;
		float cos 		= Mathf.Cos (Gamma);
		float sin 		= Mathf.Sin (Gamma);
		Vector3 v0 		= new Vector3(cos,sin,0);
		Vector3 v1 		= new Vector3(-sin,cos,0);
		Vector3 v2 		= new Vector3(0,0,1);
		Matrix3x3 RZ_R 	= new Matrix3x3();
		
		RZ_R.SetRow (0,v0);
		RZ_R.SetRow (1,v1);
		RZ_R.SetRow (2,v2);
		
		return RZ_R;*/
		return Matrix3x3.Rz (Gamma);
	}
	
	public void RotReset(){
		RX 		= Matrix3x3.identity;
		RY 		= Matrix3x3.identity;
		RZ 		= Matrix3x3.identity;
		
		ToolVecBak = ToolVec;
		ToolVec    = new Vector3(0,0,1);
	}
	
	public Vector3 PosRot(Vector3 vec){
		switch(_Mode){
			case 1:
				vec = RX * vec;
				break;
			case 2:break;
			case 3:
				vec = RY * vec;
				break;
			case 4:break;
			case 5:
				//vec = RX * vec;
				break;
			case 6:break;
			default:
				break;
		}
		return vec;
	}
	
	private Matrix3x3 R_N(Matrix3x3 ma){
		switch(_Mode){
			case 1:
				ma.v1.z *= -1;
				ma.v2.y *= -1;
				break;
			case 2:break;
			case 3:
				ma.v0.z *= -1;
				ma.v2.x *= -1;
				break;
			case 4:break;
			case 5:
				//vec = RX * vec;
				break;
			case 6:break;
			default:
				break;
		}
		return ma;
	}
	
	public Vector3 PosRot_N(Vector3 vec){
		switch(_Mode){
			case 1:
				vec = R_N (RX) * vec;
				break;
			case 2:break;
			case 3:
				vec = R_N (RY) * vec;
				break;
			case 4:break;
			case 5:
				//vec = RX * vec;
				break;
			case 6:break;
			default:
				break;
		}
		return vec;
	}
	
	public Matrix3x3 GetMatri3{
		get{
			switch(_Mode){
			case 1:
			case 2:
				return RX;
			case 3:
			case 4:
				return RY;
			case 5:
			case 6:
				return RZ;
			default:
				return Matrix3x3.identity;
			}
		}
	}
	
	public static Vector3 _PosRot(int mode,Vector3 vec,Matrix3x3 Ma){
		switch(mode){
			case 1:
				vec = Ma * vec;
				break;
			case 2:break;
			case 3:
				vec = Ma * vec;
				break;
			case 4:break;
			case 5:
				//vec = Vec4T3 (RX * Vec3T4 (vec));
				break;
			case 6:break;
			default:
				break;
		}
		return vec;
	}
	
	public static Vector3 _PosRot(Vector3 Ang,Vector3 PlaneAng,Vector3 Pos){
		Vector3 deltaAng 	= Ang - PlaneAng;
		Matrix3x3 MaA 		= RX_Get (deltaAng.x);
		Matrix3x3 MaB 		= RY_Get (deltaAng.y);
		Matrix3x3 MaC 		= RZ_Get (deltaAng.z);
		Pos 				= MaA*Pos;
		Pos					= MaB*Pos;
		Pos 				= MaC*Pos;
		
		return Pos;
	}
	
	public void ModeClone(ModalState _Modal){
		_Mode 			= _Modal._Mode;
		PGMName 		= _Modal.PGMName;
		PGM_Start 		= _Modal.PGM_Start;
		Unit 			= _Modal.Unit;
		RCompensation 	= _Modal.RCompensation;//半径补偿
		L_Value 		= _Modal.L_Value;
		R_Value 		= _Modal.R_Value;
		DL_Value 		= _Modal.DL_Value;
		DR_Value 		= _Modal.DR_Value;
		L_Value_bak 	= _Modal.L_Value_bak;
		R_Value_bak 	= _Modal.R_Value_bak;
		DL_Value_bak 	= _Modal.DL_Value_bak;
		DR_Value_bak 	= _Modal.DR_Value_bak;
		TValueBakFlag	= _Modal.TValueBakFlag;
		ToolAxis 		= _Modal.ToolAxis;
		ToolVec 		= _Modal.ToolVec;
		ToolVecBak		= _Modal.ToolVecBak;
		RX 				= _Modal.RX;
		RY 				= _Modal.RY;
		RZ 				= _Modal.RZ;
		
		Line_Index.Clear ();
		for(int i =0;i < _Modal.Line_Index.Count;i++){
			Line_Index.Add (_Modal.Line_Index[i]);
		}
		
		Q_Value.Clear ();
		foreach(string str in _Modal.Q_Value.Keys){
			Q_Value.Add (str,_Modal.Q_Value[str]);
		}
		
		LBL_List.Clear ();
		foreach(string str in _Modal.LBL_List.Keys){
			LBL_List.Add (str,_Modal.LBL_List[str]);
		}
		
		LBL_LineNumber.Clear ();
		foreach(string str in _Modal.LBL_LineNumber.Keys){
			LBL_LineNumber.Add (str,_Modal.LBL_LineNumber[str]);
		}
		
		CallLBL_info.Clear ();
		for(int i =0;i < _Modal.CallLBL_info.Count;i++){
			CallLBL_info.Add (_Modal.CallLBL_info[i]);
		}
		
		ToolList.Clear ();
		foreach(string str in _Modal.ToolList.Keys){
			ToolList.Add (str,_Modal.ToolList[str]);
		}
		
		PresetTable_List.Clear();
		for(int i =0;i < _Modal.PresetTable_List.Count;i++){
			PresetTable_List.Add (_Modal.PresetTable_List[i]);
		}
		
		Code_Type.Clear ();
		foreach(string str in _Modal.Code_Type.Keys){
			Code_Type.Add (str,_Modal.Code_Type[str]);
		}
		
		BLKFORM_1 		= _Modal.BLKFORM_1;
		BLKFORM_2 		= _Modal.BLKFORM_2;
		
		M116 			= _Modal.M116; /* ABC轴以mm/min  M117以°/min */
		M126 			= _Modal.M126; /* 旋转轴短路径运动 */
		M128 			= _Modal.M128; /* 保持刀尖位置 */
		M136 			= _Modal.M136; /* 运动mm/pr  M137mm/min */
		M94 			= _Modal.M94;
		
		currentMotion 	= _Modal.currentMotion;  /* 当前运动方式 */
		FeedSpeed 		= _Modal.FeedSpeed;
		F_Value 		= _Modal.F_Value;
		FMax 			= _Modal.FMax; /*最大速度*/
		rotspeed 		= _Modal.rotspeed;
		RotSpeed_bac 	= _Modal.RotSpeed_bac;
		RotVelocity 	= _Modal.RotVelocity;
		CooZero 		= _Modal.CooZero;
		ToolName 		= _Modal.ToolName;
		//圆运动用
		//public bool[] C_State = new bool[3]{false,false,false};
		CC_Flag 		= _Modal.CC_Flag;
		CC 				= _Modal.CC;
	}
}

public class DataStore{
	#region Para
	public string immediate_execution;
	public int motion_type;
	public bool[] XYZ_State;
	public float X_value;
	public float Y_value;
	public float Z_value;
	public bool[] ABC_State;
	public float A_value;
	public float B_value;
	public float C_value;
	public bool[] IXYZ_State;
	public float IX_value;
	public float IY_value;
	public float IZ_value;
	public bool[] IABC_State;
	public float IA_value;
	public float IB_value;
	public float IC_value;
	public float F_value;
	public float F2_value;
	public float S_value;
	public string ToolNum;
	public float R_value;
	public float L_value;
	public float DR_value;
	public float DL_value;
	public float CHF_value;
	public float RND_value;
	public float IPA_Value;
	public List<string> Str_List;//将单独定义字母都存在这里面
	public bool IsSingleValue;//是否有定义单独数字程序字
	public float SingleValue;//单独定义数字均转化为float，需要时转回int
	public int DR_PN;
	//CYCL19
	public bool IsCancel;
	//CYCL32
	public float T_Value;
	public float TA_Value;
	public int HSC_MODE;
	//PLANE
	public PLANE _PLANE;
	#endregion 
	
	public DataStore(){
		immediate_execution = "";
		motion_type 		= -1;
		XYZ_State 			= new bool[4]{false,false,false,false};
		X_value 			= 0f;
		Y_value 			= 0f;
		Z_value 			= 0f;
		ABC_State 			= new bool[4]{false,false,false,false};
		A_value 			= 0f;
		B_value 			= 0f;
		C_value 			= 0f;
		IXYZ_State 			= new bool[4]{false,false,false,false};
		IX_value 			= 0f;
		IY_value 			= 0f;
		IZ_value 			= 0f;
		IABC_State 			= new bool[4]{false,false,false,false};
		IA_value 			= 0f;
		IB_value 			= 0f;
		IC_value 			= 0f;
		F_value 			= 0f;
		F2_value 			= 0f;
		S_value 			= 0f;
		ToolNum 			= "";
		R_value 			= 0f;
		L_value 			= 0f;
		DR_value 			= 0f;
		DL_value 			= 0f;
		CHF_value 			= 0;
		RND_value 			= 0;
		IPA_Value 			= 0;
		Str_List 			= new List<string>();
		IsSingleValue 		= false;
		SingleValue 		= 0f;
		DR_PN 				= 0;
		IsCancel 			= false;
		T_Value 			= 0;
		TA_Value 			= 0;
		HSC_MODE 			= 0;
		_PLANE 				= new PLANE();
	}
	
	public void Clear(){
		immediate_execution = "";
		motion_type 		= -1;
		XYZ_State[0] 		= false;
		XYZ_State[1] 		= false;
		XYZ_State[2] 		= false;
		XYZ_State[3] 		= false;
		X_value 			= 0f;
		Y_value 			= 0f;
		Z_value 			= 0f;
		ABC_State[0] 		= false;
		ABC_State[1] 		= false;
		ABC_State[2] 		= false;
		ABC_State[3] 		= false;
		A_value 			= 0f;
		B_value 			= 0f;
		C_value 			= 0f;
		IXYZ_State[0] 		= false;
		IXYZ_State[1] 		= false;
		IXYZ_State[2] 		= false;
		IXYZ_State[3] 		= false;
		IX_value 			= 0f;
		IY_value 			= 0f;
		IZ_value 			= 0f;
		IABC_State[0] 		= false;
		IABC_State[1] 		= false;
		IABC_State[2] 		= false;
		IABC_State[3] 		= false;
		IA_value 			= 0f;
		IB_value 			= 0f;
		IC_value 			= 0f;
		F_value 			= 0f;
		F2_value 			= 0f;
		S_value 			= 0f;
		ToolNum 			= "";
		R_value 			= 0f;
		L_value 			= 0f;
		DR_value 			= 0f;
		DL_value 			= 0f;
		CHF_value 			= 0;
		RND_value 			= 0;
		IPA_Value 			= 0;
		Str_List.Clear ();
		IsSingleValue 		= false;
		SingleValue 		= 0f;
		DR_PN 				= 0;
		IsCancel 			= false;
		T_Value 			= 0;
		TA_Value 			= 0;
		HSC_MODE 			= 0;
		_PLANE 				= new PLANE();
	}
	
	public void Clone(DataStore aim_data){
		immediate_execution = aim_data.immediate_execution;
		motion_type 		= aim_data.motion_type;
		XYZ_State 			= aim_data.XYZ_State;
		X_value 			= aim_data.X_value;
		Y_value 			= aim_data.Y_value;
		Z_value 			= aim_data.Z_value;
		ABC_State 			= aim_data.ABC_State;
		A_value 			= aim_data.A_value;
		B_value 			= aim_data.B_value;
		C_value 			= aim_data.C_value;
		IXYZ_State 			= aim_data.IXYZ_State;
		IX_value 			= aim_data.IX_value;
		IY_value 			= aim_data.IY_value;
		IZ_value 			= aim_data.IZ_value;
		IABC_State 			= aim_data.IABC_State;
		IA_value 			= aim_data.IA_value;
		IB_value 			= aim_data.IB_value;
		IC_value 			= aim_data.IC_value;
		F_value 			= aim_data.F_value;
		F2_value 			= aim_data.F2_value;
		S_value 			= aim_data.S_value;
		ToolNum 			= aim_data.ToolNum;
		R_value 			= aim_data.R_value;
		L_value 			= aim_data.L_value;
		DR_value 			= aim_data.DR_value;
		DL_value			= aim_data.DL_value;
		CHF_value 			= aim_data.CHF_value;
		RND_value 			= aim_data.RND_value;
		IPA_Value 			= aim_data.IPA_Value;
		IsSingleValue 		= aim_data.IsSingleValue;
		SingleValue 		= aim_data.SingleValue;
		DR_PN 				= aim_data.DR_PN;
		IsCancel 			= aim_data.IsCancel;
		T_Value 			= aim_data.T_Value;
		TA_Value 			= aim_data.TA_Value;
		HSC_MODE 			= aim_data.HSC_MODE;
		_PLANE 				= aim_data._PLANE;
		ListCopy (aim_data);
	}
	
	void ListCopy(DataStore aim_data){
		Str_List = new List<string>();
		for(int i =0;i < aim_data.Str_List.Count;++i){
			Str_List.Add (aim_data.Str_List[i]);		
		}
	}
	
	public void ImmediateAdd(char char_str){
		immediate_execution += char_str;
	}
	
	public bool IsEmpty(){
		if(motion_type != -1 || immediate_execution.Length != 0){
			return false;
		}else if(XYZ_State[0] || IXYZ_State[0] || ABC_State[0] || IABC_State[0]){
			return false;
		}else if(F_value != 0 || S_value != 0){
			return false;
		}
		return true;
	}
	
	public bool HasMotion(){
		if(XYZ_State[0] || ABC_State[0] || IXYZ_State[0] || IABC_State[0]){
			return true;
		}else if(motion_type != -1){
			return true;
		}else
			return false;
	}

	public bool PosDef(){
		if(XYZ_State[0] || IXYZ_State[0] || ABC_State[0] || IABC_State[0]){
			return true;
		}else{
			return false;
		}
	}

	public Vector3 AbsolutePos(Vector3 Pos, bool IsRot, bool X_Valid, bool Y_Valid, bool Z_Valid){
		if(!IsRot){
			if(XYZ_State[0]){
				if(XYZ_State[1] && X_Valid)
					Pos.x = X_value;
				if(XYZ_State[2] && Y_Valid)
					Pos.y = Y_value;
				if(XYZ_State[3] && Z_Valid)
					Pos.z = Z_value;
			}
		}else{
			if(ABC_State[0]){
				if(ABC_State[1] && X_Valid){
					Pos.x = A_value;
				}
				if(ABC_State[2] && Y_Valid){
					Pos.y = B_value;
				}
				if(ABC_State[3] && Z_Valid){
					Pos.z = C_value;
				}
			}
		}
		return Pos;
	}
	
	public Vector3 IncrecePos(Vector3 Pos, bool IsRot, bool X_valid, bool Y_valid, bool Z_valid){
		if(!IsRot){
			if(IXYZ_State[0]){
				if(IXYZ_State[1] && X_valid){
					Pos.x += IX_value;
				}
				if(IXYZ_State[2] && Y_valid){
					Pos.y += IY_value;
				}
				if(IXYZ_State[3] && Z_valid){
					Pos.z += IZ_value;
				}
			}
		}else{
			if(IABC_State[0]){
				if(IABC_State[1] && X_valid){
					Pos.x += IX_value;
					Pos.x %= 360;
				}
				if(IABC_State[2] && Y_valid){
					Pos.y += IY_value;
					Pos.y %= 360;
				}
				if(IABC_State[3] && Z_valid){
					Pos.z += IZ_value;
					Pos.z %= 360;
				}
			}
		}
		return Pos;
	}
}


public class MotionInfo{
	#region Para
	public int index;
	public string ToolNum;
	public bool List_flag;//true表示一步含多个动作
	public int Motion_Type;
	public List<int> Motion_Type_List;//
	public List<int> M_code;
	public string Immediate_Motion;
	public List<string> Immediate_Motion_list;//
	public Vector3 DisplayStart;
	public List<Vector3> DisplayStart_List;//
	public Vector3 VirtualStart;
	public List<Vector3> VirtualStart_List;//
	public Vector3 DisplayTarget;
	public List<Vector3> DisplayTarget_List;//
	public Vector3 VirtualTarget;
	public List<Vector3> VirtualTarget_List;//
	public Vector3 RotStart;
	public List<Vector3> RotStart_List;//
	public Vector3 RotTarget;
	public List<Vector3> RotTarget_List;//
	public Vector3 Direction_D;//Direction for Display
	public List<Vector3> Direction_D_List;//
	public Vector3 Direction_V;//Direction for Virtual
	public List<Vector3> Direction_V_List;//
	public Vector3 RotDirection;//Direction for Rotation
	public List<Vector3> RotDirection_List;//
	public Vector3 CP_Direction;//Direction for 螺旋线
	public float Velocity;
	public List<float> VelocityList;//
	public float RotVelocity;
	public List<float> RotVelocity_List;//
	public float Rotate_Speed;
	public List<float> Rotate_Speed_List;//
	public float Time_Value;
	public List<float> TimeValueList;//
	public float[] Time_Value_Rot;
	public List<float[]> TimeValueRot_List;//
	public Vector3 Center_Point_D; //暂定为Display Vector3
	public List<Vector3> Center_Point_D_List;//
	public Vector3 Center_Point_V;
	public List<Vector3> Center_Point_V_List;//
	public float Rotate_Degree;
	public List<float> Rotate_Degree_List;//
	public float SpindleSpeed;
	public List<float> SpindleSpeed_List;
	
	public float Circle_r;
	public List<float> Circle_r_List;
	
	public Vector3 ToolVec;
	public List<Vector3> ToolVec_List;
	
	public Matrix3x3 Matri3;
	public int RadiusCompInfo;//R0 RL RR
	public int RadiusCompState;//start cancel normal
	public float R_Value;
	public float L_Value;
	public float DR_Value;
	public float DL_Value;
	public int DR_PN;
	public int planeMoveType;
	
	public string CodeStr;
	
	public Vector3 CooZero;
	
	public bool M128_Flag;
	#endregion
	
	public MotionInfo(){
		index 					= 0;
		ToolNum 				= "";
		List_flag 				= false;
		Motion_Type 			= -1;
		Motion_Type_List 		= new List<int> ();
		M_code 					= new List<int>();
		Immediate_Motion 		= "";
		Immediate_Motion_list 	= new List<string> ();
		DisplayStart 			= Vector3.zero;
		DisplayStart_List 		= new List<Vector3> ();
		VirtualStart 			= Vector3.zero;
		VirtualStart_List 		= new List<Vector3> ();
		DisplayTarget 			= Vector3.zero;
		DisplayTarget_List 		= new List<Vector3> ();
		VirtualTarget 			= Vector3.zero;
		VirtualTarget_List 		= new List<Vector3> ();
		RotStart 				= Vector3.zero;
		RotStart_List 			= new List<Vector3> ();
		RotTarget 				= Vector3.zero;
		RotTarget_List 			= new List<Vector3> ();
		Direction_D 			= Vector3.zero;
		Direction_D_List		= new List<Vector3> ();
		Direction_V 			= Vector3.zero;
		Direction_V_List 		= new List<Vector3> ();
		CP_Direction 			= Vector3.zero;
		RotDirection 			= Vector3.zero;
		RotDirection_List 		= new List<Vector3> ();
		Velocity 				= 0;
		VelocityList 			= new List<float> ();
		RotVelocity 			= 0;
		RotVelocity_List 		= new List<float>();
		Rotate_Speed 			= 0;
		Rotate_Speed_List 		= new List<float> ();
		Time_Value 				= 0;
		TimeValueList 			= new List<float> ();
		Time_Value_Rot 			= new float[3]{0,0,0};
		TimeValueRot_List 		= new List<float[]>();
		Center_Point_D 			= Vector3.zero;
		Center_Point_D_List 	= new List<Vector3> ();
		Center_Point_V 			= Vector3.zero;
		Center_Point_V_List 	= new List<Vector3>();
		Rotate_Degree 			= 0;
		Rotate_Degree_List 		= new List<float> ();
		SpindleSpeed 			= 0;
		SpindleSpeed_List 		= new List<float> ();
		Circle_r 				= 0;
		Circle_r_List			= new List<float>();
		ToolVec 				= new Vector3(0,0,1);
		ToolVec_List			= new List<Vector3>();
		Matri3 					= Matrix3x3.identity;
		RadiusCompInfo 			= 0;
		RadiusCompState 		= 0;
		R_Value 				= 0;
		L_Value 				= 0;
		DR_Value 				= 0;
		DL_Value 				= 0;
		DR_PN 					= 0;
		planeMoveType 			= (int)PlaneMoveType.STAY;
		CodeStr					= "";
		CooZero					= Vector3.zero;
		M128_Flag				= false;
	}

	public void SetStartPos(Vector3 displayPos,Vector3 virtualPos,Vector3 rotPos){
		DisplayStart = displayPos;
		VirtualStart = virtualPos;
		RotStart = rotPos;
	}

	public void SetTargetPos(Vector3 displayPos,Vector3 virtualPos,Vector3 rotPos){
		DisplayTarget = displayPos;
		VirtualTarget = virtualPos;
		RotTarget = rotPos;
	}
	
	public bool NotEmpty(){
		if(DisplayStart != Vector3.zero || DisplayTarget != Vector3.zero || VirtualStart != Vector3.zero || VirtualTarget != Vector3.zero || Direction_D != Vector3.zero || Direction_V != Vector3.zero)
			return true;
		else if(Velocity != 0 || Rotate_Speed != 0 || Time_Value != 0 || Rotate_Degree != 0 || Motion_Type != -1)
			return true;
		else if(Immediate_Motion != "")
			return true;
		else
			return false;
	}
	
	public bool HasMotion(){
		if (DisplayStart != DisplayTarget || RotStart != RotTarget || (DisplayStart_List.Count != 0 && DisplayTarget_List.Count != 0) || 
			(RotStart_List.Count != 0 && RotTarget_List.Count != 0)) {
			return true;
		} else if (Motion_Type == (int)MotionType.Line || Motion_Type == (int)MotionType.CR_N || Motion_Type == (int)MotionType.CR_P ||
			Motion_Type == (int)MotionType.CHF || Motion_Type == (int)MotionType.RND || Motion_Type == (int)MotionType.TOOLCALL) {
			return true;
		} else {
			return false;
		}

	}
	
	public bool PosMoved(){
		if(DisplayStart != DisplayTarget || (DisplayStart_List.Count != 0 && DisplayTarget_List.Count != 0)){
			return true;
		}else{
			return false;
		}
	}
	
	public void MotionCopy(MotionInfo aim_data){
		index 				= aim_data.index;
		ToolNum 			= aim_data.ToolNum;
		List_flag 			= aim_data.List_flag;
		Motion_Type 		= aim_data.Motion_Type;
		Immediate_Motion 	= aim_data.Immediate_Motion;
		DisplayStart 		= aim_data.DisplayStart;
		VirtualStart 		= aim_data.VirtualStart;
		DisplayTarget		= aim_data.DisplayTarget;
		VirtualTarget 		= aim_data.VirtualTarget;
		RotStart 			= aim_data.RotStart;
		RotTarget 			= aim_data.RotTarget;
		Direction_D 		= aim_data.Direction_D;
		Direction_V 		= aim_data.Direction_V;
		RotDirection 		= aim_data.RotDirection;
		Velocity 			= aim_data.Velocity;
		RotVelocity 		= aim_data.RotVelocity;
		Rotate_Speed 		= aim_data.Rotate_Speed;
		Time_Value 			= aim_data.Time_Value;
		Time_Value_Rot 		= aim_data.Time_Value_Rot;
		Center_Point_D 		= aim_data.Center_Point_D;
		Center_Point_V 		= aim_data.Center_Point_V;
		CP_Direction 		= aim_data.CP_Direction;
		Rotate_Degree 		= aim_data.Rotate_Degree;
		SpindleSpeed 		= aim_data.SpindleSpeed;
		Circle_r 			= aim_data.Circle_r;
		ToolVec 			= aim_data.ToolVec;
		Matri3 				= aim_data.Matri3;
		RadiusCompInfo 		= aim_data.RadiusCompInfo;
		RadiusCompState 	= aim_data.RadiusCompState;
		R_Value 			= aim_data.R_Value;
		L_Value 			= aim_data.L_Value;
		DR_Value 			= aim_data.DR_Value;
		DL_Value 			= aim_data.DL_Value;
		DR_PN 				= aim_data.DR_PN;
		planeMoveType 		= aim_data.planeMoveType;
		CodeStr				= aim_data.CodeStr;
		CooZero				= aim_data.CooZero;
		M128_Flag			= aim_data.M128_Flag;

		Motion_Type_List.Clear ();
		for(int i =0;i< aim_data.Motion_Type_List.Count;i++){
			Motion_Type_List.Add (aim_data.Motion_Type_List[i]);
		}
		
		M_code.Clear ();
		for(int i =0;i< aim_data.M_code.Count;i++){
			M_code.Add (aim_data.M_code[i]);
		}
		
		Immediate_Motion_list.Clear ();
		for(int i =0;i< aim_data.Immediate_Motion_list.Count;i++){
			Immediate_Motion_list.Add (aim_data.Immediate_Motion_list[i]);
		}
		
		DisplayStart_List.Clear ();
		for(int i =0;i< aim_data.DisplayStart_List.Count;i++){
			DisplayStart_List.Add (aim_data.DisplayStart_List[i]);
		}
		
		VirtualStart_List.Clear ();
		for(int i =0;i< aim_data.VirtualStart_List.Count;i++){
			VirtualStart_List.Add (aim_data.VirtualStart_List[i]);
		}
		
		DisplayTarget_List.Clear ();
		for(int i =0;i< aim_data.DisplayTarget_List.Count;i++){
			DisplayTarget_List.Add (aim_data.DisplayTarget_List[i]);
		}
		
		VirtualTarget_List.Clear ();
		for(int i =0;i< aim_data.VirtualTarget_List.Count;i++){
			VirtualTarget_List.Add (aim_data.VirtualTarget_List[i]);
		}
		
		RotStart_List.Clear ();
		for(int i =0;i< aim_data.RotStart_List.Count;i++){
			RotStart_List.Add (aim_data.RotStart_List[i]);
		}
		
		RotTarget_List.Clear ();
		for(int i =0;i< aim_data.RotTarget_List.Count;i++){
			RotTarget_List.Add (aim_data.RotTarget_List[i]);
		}
		
		Direction_D_List.Clear ();
		for(int i =0;i< aim_data.Direction_D_List.Count;i++){
			Direction_D_List.Add (aim_data.Direction_D_List[i]);
		}
		
		Direction_V_List.Clear ();
		for(int i = 0;i < aim_data.Direction_V_List.Count;i++){
			Direction_V_List.Add (aim_data.Direction_V_List[i]);
		}
		
		RotDirection_List.Clear ();
		for(int i =0;i< aim_data.RotDirection_List.Count;i++){
			RotDirection_List.Add (aim_data.RotDirection_List[i]);
		}
		
		VelocityList.Clear ();
		for(int i =0;i< aim_data.VelocityList.Count;i++){
			VelocityList.Add (aim_data.VelocityList[i]);
		}
		
		RotVelocity_List.Clear ();
		for(int i =0;i< aim_data.RotVelocity_List.Count;i++){
			RotVelocity_List.Add (aim_data.RotVelocity_List[i]);
		}
		
		Rotate_Speed_List.Clear ();
		for(int i =0;i< aim_data.Rotate_Speed_List.Count;i++){
			Rotate_Speed_List.Add (aim_data.Rotate_Speed_List[i]);
		}
		
		TimeValueList.Clear ();
		for(int i =0;i< aim_data.TimeValueList.Count;i++){
			TimeValueList.Add (aim_data.TimeValueList[i]);
		}
		
		TimeValueRot_List.Clear ();
		for(int i =0;i< aim_data.TimeValueRot_List.Count;i++){
			TimeValueRot_List.Add (aim_data.TimeValueRot_List[i]);
		}
		
		Center_Point_D_List.Clear ();
		for(int i =0;i< aim_data.Center_Point_D_List.Count;i++){
			Center_Point_D_List.Add (aim_data.Center_Point_D_List[i]);
		}
		
		Center_Point_V_List.Clear ();
		for(int i = 0;i < aim_data.Center_Point_V_List.Count;i++){
			Center_Point_V_List.Add (aim_data.Center_Point_V_List[i]);
		}
		
		Rotate_Degree_List.Clear ();
		for(int i =0;i< aim_data.Rotate_Degree_List.Count;i++){
			Rotate_Degree_List.Add (aim_data.Rotate_Degree_List[i]);
		}
		
		SpindleSpeed_List.Clear ();
		for(int i =0;i< aim_data.SpindleSpeed_List.Count;i++){
			SpindleSpeed_List.Add (aim_data.SpindleSpeed_List[i]);
		}
		
		Circle_r_List.Clear ();
		for(int i =0;i< aim_data.Circle_r_List.Count;i++){
			Circle_r_List.Add (aim_data.Circle_r_List[i]);
		}
		
		ToolVec_List.Clear ();
		for(int i =0;i< aim_data.ToolVec_List.Count;i++){
			ToolVec_List.Add (aim_data.ToolVec_List[i]);
		}
	}

	public void Convert_ListMode(){
		List_flag 				= true;
		Motion_Type_List 		= new List<int>();
		DisplayStart_List 		= new List<Vector3> ();
		VirtualStart_List 		= new List<Vector3> ();
		DisplayTarget_List 		= new List<Vector3> ();
		VirtualTarget_List 		= new List<Vector3> ();
		RotStart_List 			= new List<Vector3> ();
		RotTarget_List 			= new List<Vector3> ();
		Direction_D_List		= new List<Vector3> ();
		Direction_V_List 		= new List<Vector3> ();
		RotDirection_List 		= new List<Vector3> ();
		VelocityList 			= new List<float> ();
		RotVelocity_List 		= new List<float>();
		Rotate_Speed_List 		= new List<float> ();
		TimeValueList 			= new List<float> ();
		TimeValueRot_List 		= new List<float[]>();
		Center_Point_D_List 	= new List<Vector3> ();
		Center_Point_V_List 	= new List<Vector3>();
		Rotate_Degree_List 		= new List<float> ();
		SpindleSpeed_List 		= new List<float> ();
		Circle_r_List 			= new List<float> ();
		ToolVec_List 			= new List<Vector3>();
		
		Motion_Type_List.Add 	(Motion_Type);
		DisplayStart_List.Add 	(DisplayTarget);
		VirtualStart_List.Add 	(VirtualStart);
		DisplayTarget_List.Add 	(DisplayTarget);
		VirtualTarget_List.Add 	(VirtualTarget);
		RotStart_List.Add 		(RotStart);
		RotTarget_List.Add 		(RotTarget);
		Direction_D_List.Add 	(Direction_D);
		Direction_V_List.Add 	(Direction_V);
		RotDirection_List.Add 	(RotDirection);
		VelocityList.Add 		(Velocity);
		RotVelocity_List.Add 	(RotVelocity);
		Rotate_Speed_List.Add 	(Rotate_Speed);
		TimeValueList.Add 		(Time_Value);
		TimeValueRot_List.Add 	(Time_Value_Rot);
		Center_Point_D_List.Add (Center_Point_D);
		Center_Point_V_List.Add (Center_Point_V);
		Rotate_Degree_List.Add 	(Rotate_Degree);
		SpindleSpeed_List.Add 	(SpindleSpeed);
		Circle_r_List.Add 		(Circle_r);
		ToolVec_List.Add 		(ToolVec);
		
		Motion_Type = (int)MotionType.LIST;
	}

	/* to confirm has no gap between every two Motion */
	public bool ListMotionAdd(MotionInfo aim_data){
		if(!List_flag){
			return false;
		}
		
		Motion_Type_List.Add	(aim_data.Motion_Type);
		DisplayStart_List.Add 	(aim_data.DisplayTarget);
		VirtualStart_List.Add 	(aim_data.VirtualStart);
		DisplayTarget_List.Add 	(aim_data.DisplayTarget);
		VirtualTarget_List.Add 	(aim_data.VirtualTarget);
		RotStart_List.Add 		(aim_data.RotStart);
		RotTarget_List.Add 		(aim_data.RotTarget);
		Direction_D_List.Add 	(aim_data.Direction_D);
		Direction_V_List.Add 	(aim_data.Direction_V);
		RotDirection_List.Add 	(aim_data.RotDirection);
		VelocityList.Add 		(aim_data.Velocity);
		RotVelocity_List.Add 	(aim_data.RotVelocity);
		Rotate_Speed_List.Add 	(aim_data.Rotate_Speed);
		TimeValueList.Add 		(aim_data.Time_Value);
		TimeValueRot_List.Add 	(aim_data.Time_Value_Rot);
		Center_Point_D_List.Add (aim_data.Center_Point_D);
		Center_Point_V_List.Add (aim_data.Center_Point_V);
		Rotate_Degree_List.Add 	(aim_data.Rotate_Degree);
		SpindleSpeed_List.Add 	(aim_data.SpindleSpeed);
		Circle_r_List.Add 		(aim_data.Circle_r);
		ToolVec_List.Add 		(aim_data.ToolVec);
		
		DisplayTarget = aim_data.DisplayTarget;
		VirtualTarget = aim_data.VirtualTarget;
		Direction_D   = DisplayTarget - DisplayStart;
		Direction_V   = VirtualTarget - VirtualStart;
		
		return true;
	}
	
	public string GetCode(){
		return CodeStr;
	}
}
