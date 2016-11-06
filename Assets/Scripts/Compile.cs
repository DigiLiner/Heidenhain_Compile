using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;

public interface ICompile{
	bool CompileEntrance(List<List<string>> CodeSeg,ref MotionInfo StepMotion,ref DataStore StepData,ref int Pindex);
	List<string> CompileInfo{get;set;}
	string ErrorMessage{get;}
}


public class Lexical_Check{
	#region Para
	protected ModalState Modal_State;
	protected int Mode;
	protected bool error_flag;
	
	protected MotionInfo ChamferBak = new MotionInfo();
	protected bool NoChamfer = false;
	protected MotionInfo CompensationBak = new MotionInfo();
	protected Vector3 VecForCompensation = Vector3.zero;
	
	protected string _errorMessage;
	public virtual string ErrorMessage
	{
		get {return _errorMessage;}
	}
	
	public List<string> _compileInfo;
	
	public virtual List<string> CompileInfo
	{
		get {return _compileInfo;}
		set {}
	}
	
	protected void ErrorMessageAdd(string error_message)
	{
		_compileInfo.Add(error_message);
	}
	#endregion
	
	public Lexical_Check(){
		_errorMessage = "";
		_compileInfo = new List<string>();
		Modal_State = new ModalState(Mode);
		error_flag = false;
	}
	
	#region Check
	public bool G_Check(string CodeSeg,int index,ref DataStore StepData){
		
		return true;
	}
	
	private List<string> satisfactoryM = new List<string>() {"M0", "M00", "M1", "M01", "M2", "M02", "M3", "M03", "M4", "M04", 
		"M5", "M05", "M6", "M06", "M8", "M08", "M9", "M09", "M13", "M14", "M30", "M91", "M116", "M117", "M126", "M127", "M128",
		"M129", "M140"};
	public bool M_Check(string CodeSeg,int index,ref DataStore StepData){
		_errorMessage = "";
		string M_Str = "M";
		if(satisfactoryM.IndexOf(CodeSeg) == -1)
		{
			_errorMessage = "Line:" + (index+1) + " 系统中不存在该M指令或暂不支持该M指令: " + CodeSeg;
			if(CompileParas._DEBUG) _errorMessage += "% ErrorDBG-MCHK-70 %";
			return false;
		}else{
			try{
				int m_value = int.Parse (CodeSeg.Trim ('M'));
				if(m_value < 10)
					M_Str += "0";
				M_Str += m_value.ToString ();
			}catch{
				_errorMessage = "Line:" + (index+1) + " 系统中不存在该M指令: " + CodeSeg;
				if(CompileParas._DEBUG) _errorMessage += "% ErrorDBG-MCHK-71 %";
				return false;
			}
		}
		#region MStr
		switch(M_Str){
		#region M00
		case "M00":
			StepData.ImmediateAdd ((char)ImmediateMotionType.M00); 
			break;
		#endregion
		#region M01
		case "M01":
			StepData.ImmediateAdd ((char)ImmediateMotionType.M01); 
			break;
		#endregion
		#region M02
		case "M02":
			StepData.ImmediateAdd ((char)ImmediateMotionType.M02); 
			break;
		#endregion
		#region M03
		case "M03":
			StepData.ImmediateAdd ((char)ImmediateMotionType.M03); 
			break;
		#endregion
		#region M04
		case "M04":
			StepData.ImmediateAdd ((char)ImmediateMotionType.M04); 
			break;
		#endregion
		#region M05
		case "M05":
			StepData.ImmediateAdd ((char)ImmediateMotionType.M05); 
			break;
		#endregion
		#region M06
		case "M06":
			StepData.ImmediateAdd ((char)ImmediateMotionType.M06); 
			break;
		#endregion
		#region M08
		case "M08":
			StepData.ImmediateAdd ((char)ImmediateMotionType.M08);
			break;
		#endregion
		#region M09
		case "M09":
			StepData.ImmediateAdd ((char)ImmediateMotionType.M09);
			break;
		#endregion
		#region M13
		case "M13":
			StepData.ImmediateAdd ((char)ImmediateMotionType.M03);
			StepData.ImmediateAdd ((char)ImmediateMotionType.M08);
			break;
		#endregion
		#region M14
		case "M14":
			StepData.ImmediateAdd ((char)ImmediateMotionType.M04);
			StepData.ImmediateAdd ((char)ImmediateMotionType.M08);
			break;
		#endregion
		#region M30
		case "M30":
			StepData.ImmediateAdd ((char)ImmediateMotionType.M30);
			break;
		#endregion
		#region M91
		case "M91":
			StepData.ImmediateAdd ((char)ImmediateMotionType.M91);
			break;
		#endregion
		#region M116
		case "M116":
			StepData.ImmediateAdd ((char)ImmediateMotionType.M116);
			break;
		#endregion
		#region M117
		case "M117":
			StepData.ImmediateAdd ((char)ImmediateMotionType.M117);
			break;
		#endregion
		#region M126
		case "M126":
			StepData.ImmediateAdd ((char)ImmediateMotionType.M126);
			break;
		#endregion
		#region M127
		case "M127":
			StepData.ImmediateAdd ((char)ImmediateMotionType.M127);
			break;
		#endregion
		#region M128
		case "M128":
			StepData.ImmediateAdd ((char)ImmediateMotionType.M128);
			break;
		#endregion
		#region M129
		case "M129":
			StepData.ImmediateAdd ((char)ImmediateMotionType.M129);
			break;
		#endregion
		#region M136
		case "M136":
			StepData.ImmediateAdd ((char)ImmediateMotionType.M136);
			break;
		#endregion
		#region M137
		case "M137":
			StepData.ImmediateAdd ((char)ImmediateMotionType.M137);
			break;
		#endregion
		#region M140
		case "M140":
			StepData.ImmediateAdd ((char)ImmediateMotionType.M140);
			break;
		#endregion
		default:
			_errorMessage = "Line:" + (index+1) + " 系统暂不支持该M指令: " + M_Str;
			if(CompileParas._DEBUG) _errorMessage += "% ErrorDBG-MCHK-72 %";
			return false;
		}
		#endregion
		return true;
	}
	
	public bool I_Check(string CodeSeg,int index,ref DataStore StepData){
		_errorMessage = "";
		string address_value = CodeSeg[0].ToString().ToUpper();
		string CodeStr = CodeSeg.Remove(0, 1);
		int str_value = 0;
		try{
			str_value = int.Parse (CodeStr);
		}catch{
			_errorMessage = "Line:" + (index+1) + address_value + "地址后值的格式错误";
			if(CompileParas._DEBUG) _errorMessage += "% ErrorDBG-ICHK-73 %";
			return false;
		}
		switch(address_value){
		case "S":
			if(str_value < 0 || str_value > 99999){
				_errorMessage = "Line:" + (index+1) + " " + address_value + "地址后值为" + str_value + ", 超出规定范围(0~99999)";
				if(CompileParas._DEBUG) _errorMessage += "% ErrorDBG-ICHK_S-74 %";
				return false;
			}
			StepData.S_value = str_value;
			StepData.ImmediateAdd ((char)ImmediateMotionType.RotateSpeed);
			Modal_State.RotSpeed = str_value;
			if(Modal_State.M136){//mm/pr
				Modal_State.FeedSpeed = Modal_State.F_Value * Modal_State.RotSpeed;
			}
			break;
		default:
			break;
		}
		
		return true;
	}
	
	public bool F_Check(string CodeSeg,int index,ref DataStore StepData){
		_errorMessage = "";
		CodeSeg = CodeSeg.ToUpper ();
		string address_value = "";
		string CodeStr = "";
		float str_value = 0;
		int Q_Index = CodeSeg.IndexOf ('Q');
		if(Q_Index != -1){
			int PM = 1;//符号
			int P_Index = CodeSeg.IndexOf ('+');//+
			int M_Index = CodeSeg.IndexOf ('-');//-
			if(P_Index != -1 || M_Index != -1){
				if(M_Index != -1){
					PM = -1;
				}
				CodeSeg = CodeSeg.Trim ('+','-');
				Q_Index = CodeSeg.IndexOf ('Q');
			}
			address_value = CodeSeg.Substring (0,Q_Index+1);
			CodeStr = CodeSeg.Substring (Q_Index);
			if(Modal_State.Q_Value.ContainsKey (CodeStr)){
				str_value = Modal_State.Q_Value[CodeStr];
			}else{
				str_value = 0;
				Modal_State.Q_Value.Add (CodeStr,0);
//				Debug.Log ("");
			}
			str_value *= PM;
		}else{
			if(CodeSeg.IndexOf ("CHF") != -1 || CodeSeg.IndexOf ("RND") != -1){
				address_value = CodeSeg.Substring (0,3);
				CodeStr = CodeSeg.Remove(0, 3);
			}else if(CodeSeg.IndexOf ("DR") != -1 || CodeSeg.IndexOf ("DL") != -1){
				address_value = CodeSeg.Substring (0,2);
				CodeStr = CodeSeg.Remove(0, 2);
			}else{
				if(CodeSeg.StartsWith ("IPA")){
					address_value = CodeSeg.Substring (0,3);
					CodeStr = CodeSeg.Remove (0,3);
				}else if(CodeSeg.StartsWith ("I")){
					address_value = CodeSeg.Substring (0,2);
					CodeStr = CodeSeg.Remove (0, 2);
				}else{
					address_value = CodeSeg[0].ToString();
					CodeStr = CodeSeg.Remove(0, 1);
				}
			}
			try
			{
				str_value = (float)Convert.ToDouble(CodeStr);	
			}catch{
				_errorMessage = "Line:" + (index+1) + address_value + "地址后值的格式错误";
				if(CompileParas._DEBUG) _errorMessage += "% ErrorDBG-FCHK-75 %";
				return false;
			}
		}
		
		switch(address_value){
		case "X":
			StepData.XYZ_State[0] = true;
			StepData.XYZ_State[1] = true;
			StepData.X_value = str_value;
			break;
		case "Y":
			StepData.XYZ_State[0] = true;
			StepData.XYZ_State[2] = true;
			StepData.Y_value = str_value;
			break;
		case "Z":
			StepData.XYZ_State[0] = true;
			StepData.XYZ_State[3] = true;
			StepData.Z_value = str_value;
			break;
		case "A":
			StepData.ABC_State[0] = true;
			StepData.ABC_State[1] = true;
			StepData.A_value = str_value;
			break;
		case "B":
			StepData.ABC_State[0] = true;
			StepData.ABC_State[2] = true;
			StepData.B_value = str_value;
			break;
		case "C":
			StepData.ABC_State[0] = true;
			StepData.ABC_State[3] = true;
			StepData.C_value = str_value;
			break;
		case "IX":
			StepData.IXYZ_State[0] = true;
			StepData.IXYZ_State[1] = true;
			StepData.IX_value = str_value;
			break;
		case "IY":
			StepData.IXYZ_State[0] = true;
			StepData.IXYZ_State[2] = true;
			StepData.IY_value = str_value;
			break;
		case "IZ":
			StepData.IXYZ_State[0] = true;
			StepData.IXYZ_State[3] = true;
			StepData.IZ_value = str_value;
			break;
		case "IA":
			StepData.IABC_State[0] = true;
			StepData.IABC_State[1] = true;
			StepData.IA_value = str_value;
			break;
		case "IB":
			StepData.IABC_State[0] = true;
			StepData.IABC_State[2] = true;
			StepData.IB_value = str_value;
			break;
		case "IC":
			StepData.IABC_State[0] = true;
			StepData.IABC_State[3] = true;
			StepData.IC_value = str_value;
			break;
		case "F":
			if(StepData.immediate_execution.IndexOf ((char)ImmediateMotionType.M140) != -1){
				StepData.F2_value = str_value;
			}else if(StepData.immediate_execution.IndexOf((char)ImmediateMotionType.M128) != -1){
				StepData.F2_value = str_value;
			}else{
				StepData.F_value = str_value;
				StepData.ImmediateAdd ((char)ImmediateMotionType.FeedSpeed);
			}
			break;
		case "DR":
			StepData.DR_value = str_value;
			
			break;
		case "DL":
			StepData.DL_value = str_value;
			
			break;	
		case "R":
			StepData.R_value = str_value;
			
			break;
		case "L":
			StepData.L_value = str_value;
			
			break;
		case "CHF":
			StepData.CHF_value = str_value;
			StepData.motion_type = (int)MotionType.CHF;
			break;
		case "RND":
			StepData.RND_value = str_value;
			StepData.motion_type = (int)MotionType.RND;
			break;
		case "IPA":
			if(str_value < -5400 || str_value > 5400 || str_value == 0){
				_errorMessage = "Line:" + (index+1) + "IPA值超出范围!";
				if(CompileParas._DEBUG) _errorMessage += "% ErrorDBG-FCHK_IPA-76 %";
				return false;
			}
			StepData.IPA_Value = str_value;
			break;
		default:
			_errorMessage = "Line:" + (index+1) + " 系统暂不支持该指令: " + CodeStr;
			if(CompileParas._DEBUG) _errorMessage += "% ErrorDBG-FCHK-77 %";
			return false;
		}
		
		return true;
	}
	
	public bool FN0_Check(List<string> CodeSeg,ref int Index,ref int AllIndex){
		Regex reg = new Regex(@"^Q\d+$");
		bool RET = true;
		float Q_Value = 0f;
		string Q_Index = CodeSeg[Index+1];
		string Value = CodeSeg[Index+2];
		if(reg.IsMatch (Q_Index)){
			try{
				Q_Value = float.Parse (Value);
				if(Modal_State.Q_Value.ContainsKey (Q_Index)){
					Modal_State.Q_Value[Q_Index] = Q_Value;
				}else{
					Modal_State.Q_Value.Add (Q_Index,Q_Value);
				}
			}catch{
				RET = false;
			}
		}else{
			RET = false;
		}
		Index += 2;
		if(!RET){
			_errorMessage = "Line:" + (AllIndex+1) + "FN0指令错误！";
			if(CompileParas._DEBUG) _errorMessage += "% ErrorDBG-FN0CHK-78 %";
		}
		return RET;
	}
	
	public bool QSet_Check(string CodeStr,ref int Index){
		CodeStr = CodeStr.ToUpper ().Trim (' ');
		string[] StrArray = CodeStr.Split ('=');
		string Q_value = StrArray[0];
		float Value = 0;
		try{
			Value = float.Parse (StrArray[1]);
		}catch{
			_errorMessage = "Line:" + (Index+1) + " 系统暂不支持该指令: " + CodeStr;
			if(CompileParas._DEBUG) _errorMessage += "% ErrorDBG-QCHK-79 %";
			return false;
		}
		if(Modal_State.Q_Value.ContainsKey (Q_value)){
			Modal_State.Q_Value[Q_value] = Value;
		}else{
			Modal_State.Q_Value.Add (Q_value,Value);
		}
		return true;
	}
	
	public bool Tool_Check(List<string> Codeseg,ref int _Index,int AllIndex,ref DataStore step_data){
		string tool_num = "";
		if(Codeseg[_Index+1] == "CALL"){
			if(_Index + 3 >= Codeseg.Count){
				_errorMessage = "Line:" + (AllIndex+1) + " TOOL指令错误!";
				if(CompileParas._DEBUG) _errorMessage += "% ErrorDBG-TOOL-80 %";
				return false;
			}
			tool_num = Codeseg[_Index+2];
			if(Modal_State.ToolList.ContainsKey (tool_num)){
				step_data.ToolNum = tool_num;
				_Index += 3;
				step_data.motion_type = (int)MotionType.TOOLCALL;
				step_data.ImmediateAdd ((char)ImmediateMotionType.TOOLCALL);
			}else{
				_errorMessage = "Line:" + (AllIndex+1) + " TOOL指令错误!";
				if(CompileParas._DEBUG) _errorMessage += "% ErrorDBG-TOOL-81 %";
				return false;
			}
		}else if(Codeseg[_Index+1] == "DEF"){
			if(_Index + 2 >= Codeseg.Count){
				_errorMessage = "Line:" + (AllIndex+1) + " TOOL指令错误!";
				if(CompileParas._DEBUG) _errorMessage += "% ErrorDBG-TOOL-82 %";
				return false;
			}
			tool_num = Codeseg[_Index+2];
			step_data.ToolNum = tool_num;
			step_data.ImmediateAdd ((char)ImmediateMotionType.TOOLDEF);
		}else{
			_errorMessage = "Line:" + (AllIndex+1) + " TOOL指令错误!";
			if(CompileParas._DEBUG) _errorMessage += "% ErrorDBG-TOOL-83 %";
			return false;
		}
		return true;
	}
	
	public bool CYCL_Check(List<List<string>> _List,ref int Index,ref DataStore Step_Data){
		ICycl Cycle;
		bool Result = true;
		if(_List[Index][0] == "CYCLDEF"){
			string Str = _List[Index][1];
			switch(Str){
			case "7.0":
				Cycle = new CYCL7();
				break;
			case "19.0":
				Cycle = new CYCL19();
				break;
			case "32.0":
				Cycle = new CYCL32();
				break;
			case "247":
				Cycle = new CYCL247();
				break;
			default:
				_errorMessage = "Line:"+(Index+1) + "CYCL DEF指令错误！";
				if(CompileParas._DEBUG) _errorMessage += "% ErrorDBG-CYCLEDEF-84 %";
				ErrorMessageAdd (_errorMessage);
				return false;
			}
		}else{//PLANE
			Cycle = new PLANE();
		}
		
		Result = Cycle.Compile (_List,ref Index,ref _compileInfo);
		if(Result)
			Step_Data.Clone (Cycle.GetData ());
		return Result;
	}
	#endregion
	
	#region Cal
	//return angle unit is degree
	//cw(true) is clockwise
	private float CalculateDegree(Vector3 startPos,Vector3 EndPos,Vector3 CenterPos,bool cw){
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

	private int Calculate_Center(Vector3 startPos,Vector3 endPos,float Radius,ref Vector3 Center1,ref Vector3 Center2){
		if(endPos.x.Equals (startPos.x)){
			Center1.y = (startPos.y + endPos.y)/2.0f;
			Center2.y = (startPos.y + endPos.y)/2.0f;
			float ac4 = Mathf.Sqrt(Mathf.Pow(Radius, 2) - Mathf.Pow((endPos.y - startPos.y)/2.0f, 2));
			Center1.x = startPos.x + ac4;
			Center2.x = startPos.x - ac4;
			Center1.z = (startPos.z + endPos.z)/2.0f;
			Center2.z = (startPos.z + endPos.z)/2.0f;
		}else if(endPos.y.Equals (startPos.y)){
			Center1.x = (startPos.x + endPos.x)/2.0f;
			Center2.x = (startPos.x + endPos.x)/2.0f;
			float ac4 = Mathf.Sqrt(Mathf.Pow(Radius, 2) - Mathf.Pow((endPos.x - startPos.x)/2.0f, 2));
			Center1.y = startPos.y + ac4;
			Center2.y = startPos.y - ac4;
			Center1.z = (startPos.z + endPos.z)/2.0f;
			Center2.z = (startPos.z + endPos.z)/2.0f;
		}else{
			//			float k = (end_vec2.y - start_vec2.y) / (end_vec2.x - start_vec2.x);
			float k = -(endPos.x - startPos.x) / (endPos.y - startPos.y);
			//			float h = (start_vec2.y * end_vec2.x - end_vec2.y * start_vec2.x) / (end_vec2.x - start_vec2.x);
			//			float h = (end_vec2.y * end_vec2.x - start_vec2.y * start_vec2.x) / (end_vec2.x - start_vec2.x);
			float h = (endPos.y * endPos.y - startPos.y * startPos.y + endPos.x * endPos.x - startPos.x * startPos.x) / (2 * endPos.y - 2 * startPos.y);
			float a = 1f + k*k;
			float b = 2*k*h - 2*startPos.x - 2*startPos.y*k;
			float c = h*h - 2*startPos.y*h + startPos.x * startPos.x + startPos.y * startPos.y - Radius*Radius;
			Center1.x = (-b + Mathf.Sqrt(b*b - 4*a*c))/(2*a);
			Center2.x = (-b - Mathf.Sqrt(b*b - 4*a*c))/(2*a);
			Center1.y = k*Center1.x + h;
			Center2.y = k*Center2.x + h;
			Center1.z = (startPos.z + endPos.z)/2.0f;
			Center2.z = (startPos.z + endPos.z)/2.0f;
		}
		
		
		if(Vector3.Equals (Center1,Center2))
			return 1;
		else
			return 2;
	}
	
	//return value: 0 1 2
	//0:normal
	//1:no motion
	//2:code error
	private int PosCal(int Index,ref Vector3 displayPos,ref Vector3 programPos,ref DataStore stepData,bool Xvalid,bool Yvalid,bool Zvalid){
		if(stepData.XYZ_State[0]){
			programPos = stepData.AbsolutePos (displayPos,false,Xvalid,Yvalid,Zvalid);
			if(stepData.IXYZ_State[0]){
				if(stepData.IXYZ_State[1] && stepData.XYZ_State[1]){
					_errorMessage =  "Line:" + (Index+1) + " " + "指令格式有误！";
					if(CompileParas._DEBUG) _errorMessage += "% ErrorDBG-POSCal-85 %";
					_compileInfo.Add (ErrorMessage);
					return 2;
				}
				if(stepData.IXYZ_State[2] && stepData.XYZ_State[2]){
					_errorMessage =  "Line:" + (Index+1) + " " + "指令格式有误！";
					if(CompileParas._DEBUG) _errorMessage += "% ErrorDBG-POSCal-86 %";
					_compileInfo.Add (ErrorMessage);
					return 2;
				}
				if(stepData.IXYZ_State[3] && stepData.XYZ_State[3]){
					_errorMessage =  "Line:" + (Index+1) + " " + "指令格式有误！";
					if(CompileParas._DEBUG) _errorMessage += "% ErrorDBG-POSCal-87 %";
					_compileInfo.Add (ErrorMessage);
					return 2;
				}
				programPos = stepData.IncrecePos (programPos,false,Xvalid,Yvalid,Zvalid);
			}
		}else if(stepData.IXYZ_State[0]){
			programPos = stepData.IncrecePos (displayPos,false,Xvalid,Yvalid,Zvalid);
		}else
			return 1;
		
		return 0;
	}
	
	//return value: 0 1 2
	//0:normal
	//1:no motion
	//2:code error
	private int RotCal(int Index,ref Vector3 Rot,ref Vector3 ProgramRot,ref DataStore stepData,bool Xvalid,bool Yvalid,bool Zvalid){
		if(stepData.ABC_State[0]){
			ProgramRot = stepData.AbsolutePos (Rot,true,Xvalid,Yvalid,Zvalid);
			if(stepData.IABC_State[0]){
				if(stepData.IABC_State[1] && stepData.ABC_State[1]){
					_errorMessage =  "Line:" + (Index+1) + " " + "指令格式有误！";
					if(CompileParas._DEBUG) _errorMessage += "% ErrorDBG-RotCal-88 %";
					_compileInfo.Add (ErrorMessage);
					return 2;
				}
				if(stepData.IABC_State[2] && stepData.ABC_State[2]){
					_errorMessage =  "Line:" + (Index+1) + " " + "指令格式有误！";
					if(CompileParas._DEBUG) _errorMessage += "% ErrorDBG-RotCal-89 %";
					_compileInfo.Add (ErrorMessage);
					return 2;
				}
				if(stepData.IABC_State[3] && stepData.ABC_State[3]){
					_errorMessage =  "Line:" + (Index+1) + " " + "指令格式有误！";
					if(CompileParas._DEBUG) _errorMessage += "% ErrorDBG-RotCal-90 %";
					_compileInfo.Add (ErrorMessage);
					return 2;
				}
				ProgramRot = stepData.IncrecePos (ProgramRot,true,Xvalid,Yvalid,Zvalid);
			}
		}else if(stepData.IABC_State[0]){
			ProgramRot = stepData.IncrecePos (Rot,true,Xvalid,Yvalid,Zvalid);
		}else
			return 1;
		
		return 0;
	}
	
	private Vector3 RotDirection(DataStore StepData,Vector3 program_Rot,Vector3 display_Rot){
		Vector3 RET = Vector3.zero;
		if(StepData.IABC_State[0]){
			if(StepData.IABC_State[1]){
				RET.x = StepData.IA_value;
			}else{
				float delta = Mathf.Abs (program_Rot.x-display_Rot.x);
				int sign = program_Rot.x-display_Rot.x > 0? -1:1;
				RET.x = delta > (360-delta)? sign*(360-delta):(program_Rot.x-display_Rot.x);
			}
			if(StepData.IABC_State[2]){
				RET.y = StepData.IB_value;
			}else{
				float delta = Mathf.Abs (program_Rot.y-display_Rot.y);
				int sign = program_Rot.y-display_Rot.y > 0? -1:1;
				RET.y = delta > (360-delta)? sign*(360-delta):(program_Rot.y-display_Rot.y);
			}
			if(StepData.IABC_State[3]){
				RET.z = StepData.IC_value;
			}else{
				float delta = Mathf.Abs (program_Rot.z-display_Rot.z);
				int sign = program_Rot.z-display_Rot.z > 0? -1:1;
				RET.z = delta > (360-delta)? sign*(360-delta):(program_Rot.z-display_Rot.z);
			}
		}else{
			int sign = 1;
			float delta_x = Mathf.Abs (program_Rot.x-display_Rot.x);
			sign = program_Rot.x-display_Rot.x > 0? -1:1;
			RET.x = delta_x > (360-delta_x)? sign*(360-delta_x):(program_Rot.x-display_Rot.x);
			float delta_y = Mathf.Abs (program_Rot.y-display_Rot.y);
			sign = program_Rot.y-display_Rot.y > 0? -1:1;
			RET.y = delta_y > (360-delta_y)? sign*(360-delta_y):(program_Rot.y-display_Rot.y);
			float delta_z = Mathf.Abs (program_Rot.z-display_Rot.z);
			sign = program_Rot.z-display_Rot.z > 0? -1:1;
			RET.z = delta_z > (360-delta_z)? sign*(360-delta_z):(program_Rot.z-display_Rot.z);
		}
//		Debug.Log (RET);
		return RET;
	}
	
	private Vector3 SpindleCal(Vector3 start,Vector3 CC,float angle){
		angle = angle * Mathf.Deg2Rad;
		Vector3 R_dir_Start = start - CC;
		Vector3 R_dir_End = Vector3.zero;
		R_dir_End.z = start.z;
		R_dir_End.x = R_dir_Start.x * Mathf.Cos (angle) - R_dir_Start.y * Mathf.Sin (angle);
		R_dir_End.y = R_dir_Start.x * Mathf.Sin (angle) + R_dir_Start.y * Mathf.Cos (angle);
		Vector3 RET = CC + R_dir_End;
		return RET;
	}
	
	protected Vector3 PartPosToMachinePos(Vector3 vec){
		Vector3 v3Ret = vec;
		v3Ret 		  = CompileParas.matrixR_Coo * v3Ret;
		v3Ret		  = v3Ret + (Modal_State.CooZero - Modal_State.CooLimit_Min)*1000;
		v3Ret 		  = Matrix3x3.VecRound (v3Ret);
		return v3Ret;
	}
	
	protected Vector3 MachinePosToPartPos(Vector3 vec){
		Vector3 v3Ret = vec;
		v3Ret		  = v3Ret - (Modal_State.CooZero - Modal_State.CooLimit_Min)*1000;
		v3Ret 		  = CompileParas.matrixR_Coo_T * v3Ret;
		v3Ret 		  = Matrix3x3.VecRound (v3Ret);
		return v3Ret;
	}
	
	protected Vector3 LocalPosToMachinePos(Vector3 vec){
		return (vec - Modal_State.CooLimit_Min) * 1000;
	}
	
	protected Vector3 MachinePosToLocalPos(Vector3 vec){
		return vec / 1000 + Modal_State.CooLimit_Min;
	}
	
	protected Vector3 LocalPosToPartPos(Vector3 vec){
		Vector3 v3Ret = (vec - Modal_State.CooLimit_Min) * 1000; /* MachinePos */
		return MachinePosToPartPos (v3Ret);
	}
	
	protected Vector3 PartPosToLocalPos(Vector3 vec){
		Vector3 v3Ret = PartPosToMachinePos (vec); /* MachinePos */
		return MachinePosToLocalPos (v3Ret);
	}
	#endregion
	
	protected int MotionTypeCheck(List<string> CodeSeg, int Index, ref bool flag){
		int _MotionType = -1;
		#region CodeSeg
		for(int i = 0;i < CodeSeg.Count;i++){		
			if(CodeSeg[i] != ""){
				_errorMessage = "";
				string Address = Modal_State.CodeTypeCheck (CodeSeg[i]);
				
				switch(Address){
					#region PGM Start
				case "BEGIN":
				case "END":
					if(CodeSeg[2] == "MM"){
					}else{
						_errorMessage = "Line:" +(Index+1)+" 暂时不支持Inch单位";
						if(CompileParas._DEBUG) _errorMessage += "% ErrorDBG-CHK-91 %";
						_compileInfo.Add (_errorMessage);
						flag = false;
					}
					i = CodeSeg.Count;
					break;
					#endregion
					#region BLKFORM
				case "BLKFORM":
					if(CodeSeg[i+1].Equals ("0.1")){
						++i;
					}else if(CodeSeg[i+1].Equals ("0.2")){
						++i;
					}else{
						_errorMessage = "Line:" +(Index+1)+" BLK FORM指令错误！";
						if(CompileParas._DEBUG) _errorMessage += "% ErrorDBG-CHK-92 %";
						_compileInfo.Add (_errorMessage);
						flag = false;
					}
					break;
					#endregion
					#region PLANE CYCLDEF
				case "CYCLDEF":
				case "PLANE":
					_MotionType = (int)MotionType.PLANE;
					i = CodeSeg.Count;
					break;
					#endregion
					#region Line
				case "Line":
					_MotionType = (int)MotionType.Line;
					break;
					#endregion
					#region CR
				case "CR":
					_MotionType = (int)MotionType.CR;
					break;
					#endregion
					#region DR_CW
				case "DR_CW":
					if(CodeSeg[i][2] == '+'){
						if(_MotionType != (int)MotionType.CP){
							_MotionType = (int)MotionType.CR_P;
						}
					}else if(CodeSeg[i][2] == '-'){
						if(_MotionType != (int)MotionType.CP){
							_MotionType = (int)MotionType.CR_N;
						}
					}else{
						_errorMessage = "Line:" +(Index+1)+" DR指令定义错误";
						if(CompileParas._DEBUG) _errorMessage += "% ErrorDBG-CHK-93 %";
						_compileInfo.Add (_errorMessage);
						flag = false;
					}
					break;
					#endregion
					#region Circle
				case "Circle":
					_MotionType = (int)MotionType.CR;
					break;
					#endregion
					#region CC
				case "CC":
					_MotionType = (int)MotionType.CC;
					break;
					#endregion
					#region CP
				case "CP":
					_MotionType = (int)MotionType.CP;
					break;
					#endregion
					#region M_Check
				case "M":
					break;
					#endregion
					#region F_Check
				case "X":
				case "Y":
				case "Z":
				case "A":
				case "B":
				case "C":
				case "IX":
				case "IY":
				case "IZ":
				case "IA":
				case "IB":
				case "IC":
				case "DR":
				case "DL":
				case "L":
				case "R":
				case "F":
				case "IPA":
					break;
					#endregion
					#region CHF RND
				case "CHF":
					_MotionType = (int)MotionType.CHF;
					break;
				case "RND":
					_MotionType = (int)MotionType.RND;
					break;
					#endregion
					#region I_Check
				case "S":
					break;
					#endregion
					#region FN0
				case "FN0":
					break;
					#endregion
					#region TOOL
				case "TOOL":
					_MotionType = (int)MotionType.TOOLCALL;
					break;
					#endregion
					#region QSet
				case "QSet":
					break;
					#endregion
					#region Compensation
				case "RR":
					_MotionType = (int)MotionType.RR;
					break;
				case "RL":
					_MotionType = (int)MotionType.RL;
					break;
				case "R0":
					_MotionType = (int)MotionType.R0;
					break;
					#endregion
					#region FMAX
				case "FMAX":
					break;
					#endregion
					#region Max
				case "Max":
					break;
					#endregion
					#region MB
				case "MB":
					break;
					#endregion
				default:
					try{
						
					}catch{
						
					}
					break;
				}
				
				if(!flag){
					_compileInfo.Add (ErrorMessage);
				}
			}
			
		}
		return _MotionType;
		#endregion
	}
	
	/*
	 * Virtual Pos,sometimes called machine pos,the relative position from the origin whose value equals Min.
	 * Display Pos,the pos show on the screen of the machine.the relative position from the part origin.
	 */
	public bool CodeCheck(List<string> CodeSeg,List<List<string>> CodeAll,ref int Index,ref DataStore StepDate,ref MotionInfo StepMotion,ref Vector3 DisplayPos,ref Vector3 VirtualPos,ref Vector3 RotPos){
		
		bool temp_flag = true;
		Vector3 program_Pos = Vector3.zero;
		Vector3 program_Rot = Vector3.zero;
		
		
		#region CodeSeg
		for(int i = 0;i < CodeSeg.Count;i++){
			
			StepMotion.CodeStr += CodeSeg[i] + " ";
			if(CodeSeg[i] != ""){
				_errorMessage = "";
				string Address = Modal_State.CodeTypeCheck (CodeSeg[i]);
				
				switch(Address){
				#region PGM Start
				case "BEGIN":
				case "END":
					if(CodeSeg[2] == "MM")
						Modal_State.Unit = MUnit.Metric;
					else{
						_errorMessage = "Line:" +(Index+1)+" 暂时不支持Inch单位";
						if(CompileParas._DEBUG) _errorMessage += "% ErrorDBG-CHK-94 %";
						_compileInfo.Add (_errorMessage);
						temp_flag = false;
					}
					i = CodeSeg.Count;
					break;
				#endregion
				#region BLKFORM
				case "BLKFORM":
					if(CodeSeg[i+1].Equals ("0.1")){
						StepDate.motion_type = (int)MotionType.BLKFORM1;
						++i;
					}else if(CodeSeg[i+1].Equals ("0.2")){
						StepDate.motion_type = (int)MotionType.BLKFORM2;
						++i;
					}else{
						_errorMessage = "Line:" +(Index+1)+" BLK FORM指令错误！";
						if(CompileParas._DEBUG) _errorMessage += "% ErrorDBG-CHK-95 %";
						_compileInfo.Add (_errorMessage);
						temp_flag = false;
					}
					Modal_State.CurrentMotion = StepDate.motion_type;
					break;
				#endregion
				#region PLANE CYCLDEF
				case "CYCLDEF":
				case "PLANE":
					temp_flag = CYCL_Check (CodeAll,ref Index,ref StepDate);
					i = CodeSeg.Count;
					Modal_State.CurrentMotion = StepDate.motion_type;
					break;
				#endregion
				#region Line
				case "Line":
					StepDate.motion_type = (int)MotionType.Line;
					Modal_State.CurrentMotion = (int)MotionType.Line;
					break;
				#endregion
				#region CR
				case "CR":
					StepDate.motion_type = (int)MotionType.CR;
					Modal_State.CurrentMotion = (int)MotionType.CR;
					break;
				#endregion
				#region DR_CW
				case "DR_CW":
					if(CodeSeg[i][2] == '+'){
						if(StepDate.motion_type != (int)MotionType.CP){
							StepDate.motion_type = (int)MotionType.CR_P;
							Modal_State.CurrentMotion = (int)MotionType.CR_P;
						}
						StepDate.DR_PN = 1;
					}else if(CodeSeg[i][2] == '-'){
						if(StepDate.motion_type != (int)MotionType.CP){
							StepDate.motion_type = (int)MotionType.CR_N;
							Modal_State.CurrentMotion = (int)MotionType.CR_N;
						}
						StepDate.DR_PN = -1;
					}else{
						_errorMessage = "Line:" +(Index+1)+" DR指令定义错误";
						if(CompileParas._DEBUG) _errorMessage += "% ErrorDBG-CHK-96 %";
						_compileInfo.Add (_errorMessage);
						temp_flag = false;
					}
					break;
				#endregion
				#region Circle
				case "Circle":
					StepDate.motion_type = (int)MotionType.CR;
					Modal_State.CurrentMotion = (int)MotionType.CR;
					break;
				#endregion
				#region CC
				case "CC":
					StepDate.motion_type = (int)MotionType.CC;
					Modal_State.CurrentMotion = (int)MotionType.CC;
					break;
				#endregion
				#region CP
				case "CP":
					StepDate.motion_type = (int)MotionType.CP;
					break;
				#endregion
				#region M_Check
				case "M":
					temp_flag = M_Check (CodeSeg[i],Index,ref StepDate);
					break;
				#endregion
				#region F_Check
				case "X":
				case "Y":
				case "Z":
				case "A":
				case "B":
				case "C":
				case "IX":
				case "IY":
				case "IZ":
				case "IA":
				case "IB":
				case "IC":
				case "DR":
				case "DL":
				case "L":
				case "R":
				case "F":
				case "IPA":
					temp_flag = F_Check (CodeSeg[i],Index,ref StepDate);
					break;
				#endregion
				#region CHF RND
				case "CHF":
					StepDate.motion_type = (int)MotionType.CHF;
					break;
				case "RND":
					StepDate.motion_type = (int)MotionType.RND;
					break;
				#endregion
				#region I_Check
				case "S":
					temp_flag = I_Check (CodeSeg[i],Index,ref StepDate);
					break;
				#endregion
				#region FN0
				case "FN0":
					temp_flag = FN0_Check (CodeSeg,ref i,ref Index);
					break;
				#endregion
				#region TOOL
				case "TOOL":
					temp_flag = Tool_Check (CodeSeg,ref i,Index,ref StepDate);
					break;
				#endregion
				#region QSet
				case "QSet":
					temp_flag = QSet_Check (CodeSeg[i],ref Index);
					break;
				#endregion
				#region Compensation
				case "RR":
					StepDate.ImmediateAdd ((char)ImmediateMotionType.RadiusCompRight);
					break;
				case "RL":
					StepDate.ImmediateAdd ((char)ImmediateMotionType.RadiusCompLeft);
					break;
				case "R0":
					StepDate.ImmediateAdd ((char)ImmediateMotionType.RadiusCompCancel);
					break;
				#endregion
				#region FMAX
				case "FMAX":
					StepDate.F_value = Modal_State.FMAX;
					StepDate.ImmediateAdd ((char)ImmediateMotionType.FeedSpeed);
					break;
				#endregion
				#region Max
				case "Max":
					StepDate.Str_List.Add ("Max");
					break;
				#endregion
				#region MB
				case "MB":
					StepDate.Str_List.Add ("MB");
					break;
				#endregion
				default:
					try{
						StepDate.SingleValue = float.Parse (CodeSeg[i]);
						StepDate.IsSingleValue = true;
					}catch{
						StepDate.Str_List.Add (CodeSeg[i]);
					}
					break;
				}
			
				if(!temp_flag){
					_compileInfo.Add (ErrorMessage);
				}
			}
			
		}
		#endregion
		
		if(temp_flag){
		
			if(StepDate.IsEmpty ())
				return true;
			bool RadiusDeal_flag = false;
			if(StepDate.immediate_execution != ""){
				StepMotion.Immediate_Motion = StepDate.immediate_execution;
				#region ImmediateExecute
				for(int i = 0;i < StepDate.immediate_execution.Length;i++){
					switch(StepDate.immediate_execution[i]){
					#region M00//OK
					case (char)ImmediateMotionType.M00:
						StepMotion.M_code.Add (0);
						StepMotion.SetStartPos (DisplayPos,VirtualPos,RotPos);
						StepMotion.SetTargetPos (DisplayPos,VirtualPos,RotPos);
						break;
					#endregion
					#region M01//OK
					case (char)ImmediateMotionType.M01:
						StepMotion.M_code.Add (1);
						StepMotion.SetStartPos (DisplayPos,VirtualPos,RotPos);
						StepMotion.SetTargetPos (DisplayPos,VirtualPos,RotPos);
						break;
					#endregion
					#region M02//OK
					case (char)ImmediateMotionType.M02:
						StepMotion.M_code.Add (2);
						StepMotion.SetStartPos (DisplayPos,VirtualPos,RotPos);
						StepMotion.SetTargetPos (DisplayPos,VirtualPos,RotPos);
						break;
					#endregion
					#region M03//OK
					case (char)ImmediateMotionType.M03:
						StepMotion.M_code.Add (3);
						StepMotion.SetStartPos (DisplayPos,VirtualPos,RotPos);
						StepMotion.SetTargetPos (DisplayPos,VirtualPos,RotPos);
						break;
					#endregion
					#region M04//OK
					case (char)ImmediateMotionType.M04:
						StepMotion.M_code.Add (4);
						StepMotion.SetStartPos (DisplayPos,VirtualPos,RotPos);
						StepMotion.SetTargetPos (DisplayPos,VirtualPos,RotPos);
						break;
					#endregion
					#region M05//OK
					case (char)ImmediateMotionType.M05:
						StepMotion.M_code.Add (5);
						StepMotion.SetStartPos (DisplayPos,VirtualPos,RotPos);
						StepMotion.SetTargetPos (DisplayPos,VirtualPos,RotPos);
						break;
					#endregion
					#region M06//OK
					case (char)ImmediateMotionType.M06:
						StepMotion.M_code.Add (6);
						StepMotion.SetStartPos (DisplayPos,VirtualPos,RotPos);
						StepMotion.SetTargetPos (DisplayPos,VirtualPos,RotPos);
						break;
					#endregion
					#region M08//OK
					case (char)ImmediateMotionType.M08:
						StepMotion.M_code.Add (8);
						StepMotion.SetStartPos (DisplayPos,VirtualPos,RotPos);
						StepMotion.SetTargetPos (DisplayPos,VirtualPos,RotPos);
						break;
					#endregion
					#region M09//OK
					case (char)ImmediateMotionType.M09:
						StepMotion.M_code.Add (9);
						StepMotion.SetStartPos (DisplayPos,VirtualPos,RotPos);
						StepMotion.SetTargetPos (DisplayPos,VirtualPos,RotPos);
						break;
					#endregion
					#region M13//OK
					case (char)ImmediateMotionType.M13:
						//Have been parted to M03+M08
						break;
					#endregion
					#region M14//OK
					case (char)ImmediateMotionType.M14:
						//Have been parted to M04+M08
						break;
					#endregion
					#region M30//OK
					case (char)ImmediateMotionType.M30:
						StepMotion.M_code.Add (30);
						StepMotion.SetStartPos (DisplayPos,VirtualPos,RotPos);
						StepMotion.SetTargetPos (DisplayPos,VirtualPos,RotPos);
						break;
					#endregion
					#region M91
					case (char)ImmediateMotionType.M91:
						StepMotion.M_code.Add (91);
						StepMotion.Motion_Type = (int)MotionType.LineM91;
						break;
					#endregion
					#region M116//OK
					case (char)ImmediateMotionType.M116:
						StepMotion.M_code.Add (116);
						Modal_State.M116 = true;
						StepMotion.SetStartPos (DisplayPos,VirtualPos,RotPos);
						StepMotion.SetTargetPos (DisplayPos,VirtualPos,RotPos);
						break;
					#endregion
					#region M117//OK
					case (char)ImmediateMotionType.M117:
						StepMotion.M_code.Add (117);
						Modal_State.M116 = false;
						StepMotion.SetStartPos (DisplayPos,VirtualPos,RotPos);
						StepMotion.SetTargetPos (DisplayPos,VirtualPos,RotPos);
						break;
					#endregion
					#region M126//OK
					case (char)ImmediateMotionType.M126:
						StepMotion.M_code.Add (126);
						Modal_State.M126 = true;
						StepMotion.SetStartPos (DisplayPos,VirtualPos,RotPos);
						StepMotion.SetTargetPos (DisplayPos,VirtualPos,RotPos);
						break;
					#endregion
					#region M127//OK
					case (char)ImmediateMotionType.M127:
						StepMotion.M_code.Add (127);
						Modal_State.M126 = false;
						StepMotion.SetStartPos (DisplayPos,VirtualPos,RotPos);
						StepMotion.SetTargetPos (DisplayPos,VirtualPos,RotPos);
						break;
					#endregion
					#region M128
					case (char)ImmediateMotionType.M128:
						StepMotion.M_code.Add (128);
						Modal_State.M128 = true;
						StepMotion.SetStartPos (DisplayPos,VirtualPos,RotPos);
						StepMotion.SetTargetPos (DisplayPos,VirtualPos,RotPos);
						if(StepDate.F2_value != 0){
							if(Modal_State.M136){
								Modal_State.M128_speed = StepDate.F2_value * Modal_State.RotSpeed;
							}else{
								Modal_State.M128_speed = StepDate.F2_value;
							}
						}else{
							Modal_State.M128_speed = Modal_State.FeedSpeed;
						}
						break;
					#endregion
					#region M129//OK
					case (char)ImmediateMotionType.M129:
						StepMotion.M_code.Add (129);
						Modal_State.M128 = false;
						StepMotion.SetStartPos  (DisplayPos,VirtualPos,RotPos);
						StepMotion.SetTargetPos (DisplayPos,VirtualPos,RotPos);
						break;
					#endregion
					#region M136//OK
					case (char)ImmediateMotionType.M136:
						StepMotion.M_code.Add (136);
						Modal_State.M136 = true;
						StepMotion.SetStartPos  (DisplayPos,VirtualPos,RotPos);
						StepMotion.SetTargetPos (DisplayPos,VirtualPos,RotPos);
						break;
					#endregion
					#region M137//OK
					case (char)ImmediateMotionType.M137:
						StepMotion.M_code.Add (137);
						Modal_State.M136 = false;
						StepMotion.SetStartPos  (DisplayPos,VirtualPos,RotPos);
						StepMotion.SetTargetPos (DisplayPos,VirtualPos,RotPos);
						break;
					#endregion
					#region M140//OK
					case (char)ImmediateMotionType.M140:
						StepMotion.M_code.Add (140);
						StepMotion.Motion_Type = (int)MotionType.M140;
						if(StepDate.F2_value != 0){
							if(Modal_State.M136){
								Modal_State.M140_speed = StepDate.F2_value * Modal_State.RotSpeed;
							}else{
								Modal_State.M140_speed = StepDate.F2_value;
							}
						}else{
							Modal_State.M140_speed = Modal_State.FMAX;
						}
						break;
					#endregion
					#region CYCL7//OK
					case (char)ImmediateMotionType.CYCL7://坐标系平移
						if(StepDate.XYZ_State[0] || StepDate.IXYZ_State[0]){
							if(StepDate.XYZ_State[1]){
								if(StepDate.X_value == 0){
									DisplayPos.x = VirtualPos.x + Modal_State.CooZero.x;
								}else{
									DisplayPos.x = VirtualPos.x + Modal_State.CooZero.x - StepDate.X_value;
								}
							}else if(StepDate.IXYZ_State[1]){
								DisplayPos.x += StepDate.IX_value;
							}
							if(StepDate.XYZ_State[2]){
								if(StepDate.Y_value == 0){
									DisplayPos.y = VirtualPos.y + Modal_State.CooZero.y;
								}else{
									DisplayPos.y = VirtualPos.y + Modal_State.CooZero.y - StepDate.Y_value;
								}
							}else if(StepDate.IXYZ_State[2]){
								DisplayPos.y += StepDate.IY_value;
							}
							if(StepDate.XYZ_State[3]){
								if(StepDate.Z_value == 0){
									DisplayPos.z = VirtualPos.z + Modal_State.CooZero.z;
								}else{
									DisplayPos.z = VirtualPos.z + Modal_State.CooZero.z - StepDate.Z_value;
								}
							}else if(StepDate.IXYZ_State[3]){
								DisplayPos.z += StepDate.IZ_value;
							}
						}
						StepMotion.SetStartPos  (DisplayPos,VirtualPos,RotPos);
						StepMotion.SetTargetPos (DisplayPos,VirtualPos,RotPos);
					break;
					#endregion
					#region CYCL247//OK
					case (char)ImmediateMotionType.CYCL247://坐标系（类似于FanucG54-G59）
						if(StepDate.IsSingleValue){
							if(Modal_State.Q_Value.ContainsKey ("Q339")){
								Modal_State.Q_Value["Q339"] = (int)StepDate.SingleValue;
							}else{
								Modal_State.Q_Value.Add ("Q339",(int)StepDate.SingleValue);
							}
							PresetTableInfo Vec = Modal_State.LocalCoordinate ();
							Modal_State.SetCooZero (Vec.PosAxis);
							DisplayPos = VirtualPos-Modal_State.CooZero;
							StepMotion.SetStartPos  (DisplayPos,VirtualPos,RotPos);
							StepMotion.SetTargetPos (DisplayPos,VirtualPos,RotPos);
						}else{
							return false;
						}
					break;
					#endregion
					#region RotateSpeed //OK
					case (char)ImmediateMotionType.RotateSpeed:
						if(StepDate.S_value == 0){
							StepMotion.SpindleSpeed = Modal_State.RotSpeed;
						}else{
							StepMotion.SpindleSpeed = StepDate.S_value;
						}
						break;
					#endregion
					#region FeedSpeed //OK
					case (char)ImmediateMotionType.FeedSpeed:
						if(StepMotion.Motion_Type == (int)MotionType.CHF || StepMotion.Motion_Type == (int)MotionType.RND){
							if(Modal_State.M136){
								StepMotion.Velocity = StepDate.F_value * Modal_State.RotSpeed;
							}else{
								StepMotion.Velocity = StepDate.F_value;
							}
						}else{
							Modal_State.F_Value = StepDate.F_value;
							if(Modal_State.M136){
								Modal_State.FeedSpeed = StepDate.F_value * Modal_State.RotSpeed;
							}else{
								Modal_State.FeedSpeed = StepDate.F_value;
							}
						}
						break;
					#endregion
					#region TOOLDEF //OK
					case (char)ImmediateMotionType.TOOLDEF:
						if(StepDate.ToolNum != ""){
							ToolInfo T_Info = new ToolInfo();
							T_Info.Name 	= StepDate.ToolNum;
							T_Info.R_Value 	= StepDate.R_value;
							T_Info.L_Value 	= StepDate.L_value;
							T_Info.DR_Value = 0;
							T_Info.DL_Value = 0;
							if(Modal_State.ToolList.ContainsKey (T_Info.Name)){
								Modal_State.ToolList[T_Info.Name] = T_Info;
							}else{
								Modal_State.ToolList.Add (T_Info.Name,T_Info);
							}
						}else{
							_errorMessage =  "Line:" + (Index+1) + " " + "TOOL指令错误！";
							if(CompileParas._DEBUG) _errorMessage += "% ErrorDBG-Motion-97 %";
							_compileInfo.Add (ErrorMessage);
							return false;
						}
						break;
					#endregion
					#region TOOLCALL //OK
					case (char)ImmediateMotionType.TOOLCALL:
						if(Modal_State.RCompensation != (int)RCompEnum.R0){
							_errorMessage =  "Line:" + (Index+1) + " " + "换刀前请先取消半径补偿！";
							if(CompileParas._DEBUG) _errorMessage += "% ErrorDBG-Motion-98 %";
							_compileInfo.Add (ErrorMessage);
							return false;
						}
						if(StepDate.ToolNum != ""){
							Modal_State.ToolName 	  = StepDate.ToolNum;
							Modal_State.R_Value_bak   = Modal_State.R_Value;
							Modal_State.L_Value_bak   = Modal_State.L_Value;
							Modal_State.DR_Value_bak  = Modal_State.DR_Value;
							Modal_State.DL_Value_bak  = Modal_State.DL_Value;
							Modal_State.R_Value 	  = Modal_State.ToolList[Modal_State.ToolName].R_Value;
							Modal_State.L_Value 	  = Modal_State.ToolList[Modal_State.ToolName].L_Value;
							Modal_State.DR_Value 	  = Modal_State.ToolList[Modal_State.ToolName].DR_Value + StepDate.DR_value;
							Modal_State.DL_Value 	  = Modal_State.ToolList[Modal_State.ToolName].DL_Value + StepDate.DL_value;
							Modal_State.TValueBakFlag = true;
						}else{
							_errorMessage =  "Line:" + (Index+1) + " " + "TOOL指令错误！";
							if(CompileParas._DEBUG) _errorMessage += "% ErrorDBG-Motion-99 %";
							_compileInfo.Add (ErrorMessage);
							return false;
						}
						break;
					#endregion
					#region RadiusComp //OK
					case (char)ImmediateMotionType.RadiusCompRight:
						if(Modal_State.RCompensation == (int)RCompEnum.RL){
							_errorMessage =  "Line:" + (Index+1) + " " + "请先取消左侧刀补！";
							if(CompileParas._DEBUG) _errorMessage += "% ErrorDBG-Motion-100 %";
							_compileInfo.Add (ErrorMessage);
							return false;
						}
						Modal_State.RCompensation = (int)RCompEnum.RR;
						StepMotion.RadiusCompState = (int)CompState.Start;
						RadiusDeal_flag = true;
						break;
					case (char)ImmediateMotionType.RadiusCompLeft:
						if(Modal_State.RCompensation == (int)RCompEnum.RR){
							_errorMessage =  "Line:" + (Index+1) + " " + "请先取消右侧刀补！";
							if(CompileParas._DEBUG) _errorMessage += "% ErrorDBG-Motion-101 %";
							_compileInfo.Add (ErrorMessage);
							return false;
						}
						Modal_State.RCompensation = (int)RCompEnum.RL;
						StepMotion.RadiusCompState = (int)CompState.Start;
						RadiusDeal_flag = true;
						break;
					case (char)ImmediateMotionType.RadiusCompCancel:
						Modal_State.RCompensation = (int)RCompEnum.R0;
						StepMotion.RadiusCompState = (int)CompState.Cancel;
						RadiusDeal_flag = true;
						break;
					#endregion
					default:
						break;
					}
				}
				#endregion
			}
			
			if(StepDate.S_value == 0){
				StepMotion.SpindleSpeed = Modal_State.RotSpeed;
			}
			StepMotion.index 		= Index;
			StepMotion.ToolVec 		= Modal_State.ToolVec;
			StepMotion.Matri3 		= Modal_State.GetMatri3;
			StepMotion.DR_PN 		= StepDate.DR_PN;
			StepMotion.M128_Flag 	= Modal_State.M128;
			
			#region RadiusDeal
			StepMotion.RadiusCompInfo = Modal_State.RCompensation;
			StepMotion.R_Value 		  = Modal_State.R_Value;
			StepMotion.L_Value 		  = Modal_State.L_Value;
			StepMotion.DR_Value 	  = Modal_State.DR_Value;
			StepMotion.DL_Value 	  = Modal_State.DL_Value;
			if(!RadiusDeal_flag){
				switch(Modal_State.RCompensation){
				case (int)RCompEnum.RL:
				case (int)RCompEnum.RR:
					StepMotion.RadiusCompState = (int)CompState.Normal;
					break;
				case (int)RCompEnum.R0:
					StepMotion.RadiusCompState = (int)CompState.NO;
					break;
				default:
					break;
				}
			}
			#endregion
			
			
			if(StepDate.HasMotion ()){
				
				if(StepDate.motion_type == -1){
					if(Modal_State.CurrentMotion != -1){
						StepDate.motion_type = Modal_State.CurrentMotion;
						StepMotion.Motion_Type = Modal_State.CurrentMotion;
					}else{
						_errorMessage =  "Line:" + (Index+1) + " " + "未知运动方式错误！";
						if(CompileParas._DEBUG) _errorMessage += "% ErrorDBG-Motion-102 %";
						_compileInfo.Add (ErrorMessage);
						return false;
					}
				}else{
					StepMotion.Motion_Type = StepDate.motion_type;
				}
				
				switch(StepDate.motion_type){
				#region Motion
				#region BLKFORM1//OK
				case (int)MotionType.BLKFORM1://OK
					if(StepDate.XYZ_State[0] && StepDate.XYZ_State[1] && StepDate.XYZ_State[2] && StepDate.XYZ_State[3]){
						Vector3 Start = new Vector3(StepDate.X_value,StepDate.Y_value,StepDate.Z_value);
						Modal_State.BLKFORM_1 = Start;
					}else{
						_errorMessage = "Line:" +(Index+1)+" BLK FORM指令错误！";
						if(CompileParas._DEBUG) _errorMessage += "% ErrorDBG-Motion-103 %";
						_compileInfo.Add (_errorMessage);
					}
					break;
				#endregion
				#region BLKFORM2//OK
				case (int)MotionType.BLKFORM2://OK
					if(StepDate.XYZ_State[0] && StepDate.XYZ_State[1] && StepDate.XYZ_State[2] && StepDate.XYZ_State[3]){
						Vector3 End = new Vector3(StepDate.X_value,StepDate.Y_value,StepDate.Z_value);
						Modal_State.BLKFORM_2 = End;
					}else{
						_errorMessage = "Line:" +(Index+1)+" BLK FORM指令错误！";
						if(CompileParas._DEBUG) _errorMessage += "% ErrorDBG-Motion-104 %";
						_compileInfo.Add (_errorMessage);
					}
					break;
				#endregion
				#region TOOLCALL
				case (int)MotionType.TOOLCALL:
					if(StepDate.ToolNum == ""){
						_errorMessage =  "Line:" + (Index+1) + " " + "TOOL指令错误！";
						if(CompileParas._DEBUG) _errorMessage += "% ErrorDBG-Motion-105 %";
						_compileInfo.Add (ErrorMessage);
						return false;
					}
					StepMotion.ToolNum = StepDate.ToolNum;
					StepMotion.Velocity = Modal_State.FeedSpeed;
					break;
				#endregion
				#region M140//OK
				case (int)MotionType.M140:
					StepMotion.Motion_Type = (int)MotionType.Line;
					StepMotion.SetStartPos (DisplayPos,VirtualPos,RotPos);
					if(StepDate.IsSingleValue){
						program_Pos = DisplayPos + new Vector3(0,0,StepDate.SingleValue);
						StepMotion.Direction_D = new Vector3(0,0,StepDate.SingleValue);
						StepMotion.Direction_V = Modal_State.PosRot (StepMotion.Direction_D);
						StepMotion.Velocity = Modal_State.M140_speed;
						StepMotion.Time_Value = StepMotion.Direction_D.magnitude / StepMotion.Velocity * 60;
					}else if(StepDate.Str_List.IndexOf ("MAX") != -1){
						if(Mode % 2 == 0 || (Modal_State.ToolVec.x == 0 && Modal_State.ToolVec.y == 0)){
							float delta = Modal_State.CooLimit_Max.z - VirtualPos.z;
							program_Pos = DisplayPos + new Vector3(0,0,delta);
							StepMotion.Direction_D = new Vector3(0,0,delta);
							StepMotion.Direction_V = StepMotion.Direction_D;
							StepMotion.Velocity = Modal_State.M140_speed;
							StepMotion.Time_Value = StepMotion.Direction_D.magnitude / StepMotion.Velocity * 60;
						}else{
							float length = 0;
							if(Mode == 1){//A+C
								if(Modal_State.ToolVec.y > 0){
									float y = (Modal_State.CooLimit_Max.z-VirtualPos.z)*Modal_State.ToolVec.y / Modal_State.ToolVec.z + VirtualPos.y;
									if(y <= Modal_State.CooLimit_Max.y){
										length = Vector2.Distance (new Vector2(y,Modal_State.CooLimit_Max.z),new Vector2(VirtualPos.y,VirtualPos.z));
									}else{
										float z = (Modal_State.CooLimit_Max.y-VirtualPos.y)*Modal_State.ToolVec.z / Modal_State.ToolVec.y + VirtualPos.z;
										length = Vector2.Distance (new Vector2(Modal_State.CooLimit_Max.y,z),new Vector2(VirtualPos.y,VirtualPos.z));
									}
								}else if(Modal_State.ToolVec.y < 0){
									float y = (Modal_State.CooLimit_Max.z-VirtualPos.z)*Modal_State.ToolVec.y / Modal_State.ToolVec.z + VirtualPos.y;
									if(y >= Modal_State.CooLimit_Min.y){
										length = Vector2.Distance (new Vector2(y,Modal_State.CooLimit_Max.z),new Vector2(VirtualPos.y,VirtualPos.z));
									}else{
										float z = (Modal_State.CooLimit_Min.y-VirtualPos.y)*Modal_State.ToolVec.z / Modal_State.ToolVec.y + VirtualPos.z;
										length = Vector2.Distance (new Vector2(Modal_State.CooLimit_Min.y,z),new Vector2(VirtualPos.y,VirtualPos.z));
									}
								}
							}else if(Mode == 3){//B+C
								if(Modal_State.ToolVec.x > 0){
									float x = (Modal_State.CooLimit_Max.z-VirtualPos.z)*Modal_State.ToolVec.x / Modal_State.ToolVec.z + VirtualPos.x;
									if(x <= Modal_State.CooLimit_Max.x){
										length = Vector2.Distance (new Vector2(x,Modal_State.CooLimit_Max.z),new Vector2(VirtualPos.x,VirtualPos.z));
									}else{
										float z = (Modal_State.CooLimit_Max.x-VirtualPos.x)*Modal_State.ToolVec.z / Modal_State.ToolVec.x + VirtualPos.z;
										length = Vector2.Distance (new Vector2(Modal_State.CooLimit_Max.x,z),new Vector2(VirtualPos.x,VirtualPos.z));
									}
								}else if(Modal_State.ToolVec.x < 0){
									float x = (Modal_State.CooLimit_Max.z-VirtualPos.z)*Modal_State.ToolVec.x / Modal_State.ToolVec.z + VirtualPos.x;
									if(x >= Modal_State.CooLimit_Min.x){
										length = Vector2.Distance (new Vector2(x,Modal_State.CooLimit_Max.z),new Vector2(VirtualPos.x,VirtualPos.z));
									}else{
										float z = (Modal_State.CooLimit_Min.x-VirtualPos.x)*Modal_State.ToolVec.z / Modal_State.ToolVec.x + VirtualPos.z;
										length = Vector2.Distance (new Vector2(Modal_State.CooLimit_Min.x,z),new Vector2(VirtualPos.x,VirtualPos.z));
									}
								}
							}else if(Mode == 5){//A+B
								
							}
							program_Pos = DisplayPos + new Vector3(0,0,length);
							StepMotion.Direction_D = new Vector3(0,0,length);
							StepMotion.Direction_V = Modal_State.PosRot (StepMotion.Direction_D);
							StepMotion.Velocity = Modal_State.M140_speed;
							StepMotion.Time_Value = StepMotion.Direction_D.magnitude / StepMotion.Velocity * 60;
						}
					}else{
						_errorMessage =  "Line:" + (Index+1) + " " + "M140指令错误！";
						if(CompileParas._DEBUG) _errorMessage += "% ErrorDBG-Motion-106 %";
						_compileInfo.Add (ErrorMessage);
						return false;
					}
					DisplayPos = program_Pos;
					VirtualPos += StepMotion.Direction_V;
					StepMotion.SetTargetPos (DisplayPos,VirtualPos,RotPos);
					break;
				#endregion
				#region Line//OK
				case (int)MotionType.Line:
					if(Modal_State.FeedSpeed == 0 && StepDate.PosDef ()){
						_errorMessage = "Line:" + (Index +1) + " " + "未指定进给速率!";
						if(CompileParas._DEBUG) _errorMessage += "% ErrorDBG-Motion-107 %";
						_compileInfo.Add(ErrorMessage);
						return false;
					}
					StepMotion.SetStartPos (DisplayPos,VirtualPos,RotPos);
					int PosCal_flag = -1,RotCal_flag = -1;
					PosCal_flag = PosCal (Index,ref DisplayPos,ref program_Pos,ref StepDate,true,true,true);
					RotCal_flag = RotCal (Index,ref RotPos,ref program_Rot,ref StepDate,true,true,true);
					if(PosCal_flag == 2 || RotCal_flag == 2){
						return false;
					}
					if(PosCal_flag == 1){
						program_Pos = DisplayPos;
					}
					if(RotCal_flag == 1){
						program_Rot = RotPos;
					}
					StepMotion.Direction_D = program_Pos-DisplayPos;
					StepMotion.Direction_V = Modal_State.PosRot (StepMotion.Direction_D);
					StepMotion.RotDirection = RotDirection (StepDate,program_Rot,RotPos);
					StepMotion.Velocity = Modal_State.FeedSpeed;
					StepMotion.RotVelocity = Modal_State.RotVelocity;
					StepMotion.Time_Value = StepMotion.Direction_D.magnitude / StepMotion.Velocity * 60;
					StepMotion.Time_Value_Rot[0] = Mathf.Abs(StepMotion.RotDirection.x) / StepMotion.RotVelocity * 60;
                	StepMotion.Time_Value_Rot[1] = Mathf.Abs(StepMotion.RotDirection.y) / StepMotion.RotVelocity * 60;
               		StepMotion.Time_Value_Rot[2] = Mathf.Abs(StepMotion.RotDirection.z) / StepMotion.RotVelocity * 60;
					VirtualPos += StepMotion.Direction_V;
					DisplayPos = program_Pos;
					RotPos = program_Rot;
					StepMotion.SetTargetPos (DisplayPos,VirtualPos,RotPos);
					break;
				#endregion
				#region LineM91//OK
				case (int)MotionType.LineM91:
					StepMotion.Motion_Type = (int)MotionType.Line;
					StepMotion.SetStartPos (DisplayPos,VirtualPos,RotPos);
					if(StepDate.XYZ_State[0]){
						program_Pos = VirtualPos;
						program_Pos = StepDate.AbsolutePos (program_Pos,false,StepDate.XYZ_State[1],StepDate.XYZ_State[2],StepDate.XYZ_State[3]);
					}else if(StepDate.IXYZ_State[0]){
						if(Modal_State.M91){
							program_Pos = Modal_State.M91_vec;
							program_Pos = StepDate.IncrecePos (program_Pos,false,StepDate.IXYZ_State[1],StepDate.IXYZ_State[2],StepDate.IXYZ_State[3]);
						}else{
							program_Pos = VirtualPos;
							program_Pos = StepDate.IncrecePos (program_Pos,false,StepDate.IXYZ_State[1],StepDate.IXYZ_State[2],StepDate.IXYZ_State[3]);
						}
					}else{
						_errorMessage =  "Line:" + (Index+1) + " " + "M91未指定位置！";
						if(CompileParas._DEBUG) _errorMessage += "% ErrorDBG-Motion-108 %";
						_compileInfo.Add (ErrorMessage);
						return false;
					}
					StepMotion.Direction_V = program_Pos-VirtualPos;
					StepMotion.Direction_D = Modal_State.PosRot_N (StepMotion.Direction_V);
					VirtualPos = program_Pos;
					DisplayPos += StepMotion.Direction_D;
					StepMotion.Velocity = Modal_State.FeedSpeed;
					StepMotion.Time_Value = StepMotion.Direction_D.magnitude / StepMotion.Velocity * 60;
					StepMotion.SetTargetPos (DisplayPos,VirtualPos,RotPos);
					Modal_State.M91_vec = VirtualPos;
					Modal_State.M91 = true;
					break;
				#endregion
				#region CC//OK
				case (int)MotionType.CC:
					StepMotion.SetStartPos (DisplayPos,VirtualPos,RotPos);
					StepMotion.SetTargetPos (DisplayPos,VirtualPos,RotPos);
					int poscal_flag = PosCal (Index,ref DisplayPos,ref program_Pos,ref StepDate,true,true,true);
					if(poscal_flag == 2){
						return false;
					}
					if(poscal_flag == 1){
						program_Pos = DisplayPos;
					}
					Modal_State.CC_Flag = true;
					Modal_State.CC = program_Pos;
					DisplayPos = program_Pos;
					break;
				#endregion
				#region CR//OK
				case (int)MotionType.CR:
					_errorMessage =  "Line:" + (Index+1) + " " + "圆弧指令未指定圆弧运动方向！";
					if(CompileParas._DEBUG) _errorMessage += "% ErrorDBG-Motion-109 %";
					_compileInfo.Add (ErrorMessage);
					return false;
				#endregion
				#region CR_P CR_N//OK
				case (int)MotionType.CR_P:
				case (int)MotionType.CR_N:
					if(Modal_State.FeedSpeed == 0){
						_errorMessage = "Line:" + (Index +1) + " " + "未指定进给速率!";
						if(CompileParas._DEBUG) _errorMessage += "% ErrorDBG-Motion-110 %";
						_compileInfo.Add(ErrorMessage);
						return false;
					}
					StepMotion.SetStartPos (DisplayPos,VirtualPos,RotPos);
					int Poscal_flag = PosCal (Index,ref DisplayPos,ref program_Pos,ref StepDate,true,true,true);
					if(Poscal_flag == 2){
						return false;
					}
					if(Poscal_flag == 1){
						program_Pos = DisplayPos;
					}
					StepMotion.Direction_D = program_Pos-DisplayPos;
					StepMotion.Direction_V = Modal_State.PosRot (StepMotion.Direction_D);
					VirtualPos += StepMotion.Direction_V;
					if(StepDate.R_value == 0){//center
						if(Modal_State.CC_Flag){
							StepMotion.Center_Point_D = Modal_State.CC;
							if(StepMotion.Motion_Type == (int)MotionType.CR_N){//clockwise ccw
								StepMotion.Rotate_Degree = CalculateDegree (DisplayPos,program_Pos,Modal_State.CC,false);
							}else if(StepMotion.Motion_Type == (int)MotionType.CR_P){//cw
								StepMotion.Rotate_Degree = CalculateDegree (DisplayPos,program_Pos,Modal_State.CC,true);
							}
							StepMotion.Circle_r = (DisplayPos-Modal_State.CC).magnitude;
						}else{
							_errorMessage = "Line:" + (Index +1) + " " + "未指定圆心坐标!";
							if(CompileParas._DEBUG) _errorMessage += "% ErrorDBG-Motion-111 %";
							_compileInfo.Add(ErrorMessage);
							return false;
						}
					}else{//Radius
						Vector3 Cen_1 = Vector3.zero;
						Vector3 Cen_2 = Vector3.zero;
						if(Calculate_Center (DisplayPos,program_Pos,StepDate.R_value,ref Cen_1,ref Cen_2) == 1){
							StepMotion.Rotate_Degree = 180;
							StepMotion.Center_Point_D = Cen_1;
						}else{
							if(StepMotion.Motion_Type == (int)MotionType.CR_N){//clockwise cw
								StepMotion.Rotate_Degree = CalculateDegree (DisplayPos,program_Pos,Cen_1,false);
							}else if(StepMotion.Motion_Type == (int)MotionType.CR_P){//ccw
								StepMotion.Rotate_Degree = CalculateDegree (DisplayPos,program_Pos,Cen_1,true);
							}
							if(StepMotion.Rotate_Degree > 180f){
								StepMotion.Rotate_Degree = 360 - StepMotion.Rotate_Degree;
								StepMotion.Center_Point_D = Cen_2;
							}else{
								StepMotion.Center_Point_D = Cen_1;
							}
						}
						StepMotion.Circle_r = StepDate.R_value;
					}
					StepMotion.Velocity = Modal_State.FeedSpeed;
					StepMotion.Rotate_Speed = (StepMotion.Velocity / (60f * StepMotion.Circle_r)) * (180 / Mathf.PI);
					StepMotion.Time_Value = StepMotion.Rotate_Degree / StepMotion.Rotate_Speed;
					DisplayPos = program_Pos;
					StepMotion.SetTargetPos (DisplayPos,VirtualPos,RotPos);
					break;
				#endregion
				#region CP//OK
				case (int)MotionType.CP:
					if(StepDate.IPA_Value * StepDate.DR_PN < 0){
						_errorMessage =  "Line:" + (Index+1) + " " + "指令格式有误！";
						if(CompileParas._DEBUG) _errorMessage += "% ErrorDBG-Motion-112 %";
						_compileInfo.Add (ErrorMessage);
						return false;
					}
					if(!Modal_State.CC_Flag){
						_errorMessage =  "Line:" + (Index+1) + " " + "未指定圆心位置！";
						if(CompileParas._DEBUG) _errorMessage += "% ErrorDBG-Motion-113 %";
						_compileInfo.Add (ErrorMessage);
						return false;
					}
					if(!StepDate.IXYZ_State[0] && !StepDate.IXYZ_State[3]){
						_errorMessage =  "Line:" + (Index+1) + " " + "未指定IZ值！";
						if(CompileParas._DEBUG) _errorMessage += "% ErrorDBG-Motion-114 %";
						_compileInfo.Add (ErrorMessage);
						return false;
					}
					StepMotion.SetStartPos (DisplayPos,VirtualPos,RotPos);
					StepMotion.Center_Point_D = Modal_State.CC;
					
					program_Pos = SpindleCal (DisplayPos,Modal_State.CC,StepDate.IPA_Value);
					program_Pos.z += StepDate.IZ_value;
					StepMotion.Rotate_Degree = StepDate.IPA_Value;
					StepMotion.Direction_D = program_Pos-DisplayPos;
					StepMotion.Direction_V = Modal_State.PosRot (StepMotion.Direction_D);
					StepMotion.CP_Direction = Modal_State.PosRot (new Vector3(0,0,StepDate.IZ_value));
					VirtualPos += StepMotion.Direction_V;
					StepMotion.Circle_r = (DisplayPos-Modal_State.CC).magnitude;
					StepMotion.Velocity = Modal_State.FeedSpeed;
					StepMotion.Rotate_Speed = (StepMotion.Velocity / (60f * StepDate.R_value)) * (180 / Mathf.PI);
					StepMotion.Time_Value = StepMotion.Rotate_Degree / StepMotion.Rotate_Speed;
					DisplayPos = program_Pos;
					StepMotion.SetTargetPos (DisplayPos,VirtualPos,RotPos);
					break;
				#endregion
				#region CHF//OK
				case (int)MotionType.CHF:
					if(StepDate.IsSingleValue){
						StepDate.CHF_value = StepDate.SingleValue;
					}else{
						_errorMessage =  "Line:" + (Index+1) + " " + "指令格式有误！";
						if(CompileParas._DEBUG) _errorMessage += "% ErrorDBG-Motion-115 %";
						_compileInfo.Add (ErrorMessage);
						return false;
					}
					break;
				#endregion
				#region RND//OK
				case (int)MotionType.RND:
					if(StepDate.R_value > 0){
						StepDate.RND_value = StepDate.R_value;
					}else{
						_errorMessage =  "Line:" + (Index+1) + " " + "指令格式有误！";
						if(CompileParas._DEBUG) _errorMessage += "% ErrorDBG-Motion-116 %";
						_compileInfo.Add (ErrorMessage);
						return false;
					}
					break;
				#endregion
				#region CYCL19//OK
				case (int)MotionType.CYCL19:
					if(StepDate.ABC_State[0]){
						int Rotcal_flag = RotCal (Index,ref RotPos,ref program_Rot,ref StepDate,StepDate.ABC_State[1],StepDate.ABC_State[2],StepDate.ABC_State[3]);
						if(Rotcal_flag == 2){
							return false;
						}
						if(Rotcal_flag == 1){
							program_Rot = RotPos;
						}
						switch(Mode){
						case 1:
							StepMotion.ToolVec = Modal_State.RotA (program_Rot.x);
							break;
						case 3:
							StepMotion.ToolVec = Modal_State.RotB (program_Rot.y);
							break;
						case 5:
							break;
						default:
							break;
						}
						StepMotion.Matri3 = Modal_State.GetMatri3;
						DisplayPos = ModalState._PosRot (program_Rot,Modal_State.PlaneAng,DisplayPos);
						Modal_State.PlaneAng = program_Rot;
						StepMotion.SetStartPos (DisplayPos,VirtualPos,RotPos);
						StepMotion.Direction_D = Vector3.zero;
						StepMotion.Direction_V = Vector3.zero;
						StepMotion.RotDirection = RotDirection (StepDate,program_Rot,RotPos);
						StepMotion.RotVelocity = Modal_State.RotVelocity;
						StepMotion.Time_Value_Rot[0] = Mathf.Abs(StepMotion.RotDirection.x) / StepMotion.RotVelocity * 60;
						StepMotion.Time_Value_Rot[1] = Mathf.Abs(StepMotion.RotDirection.y) / StepMotion.RotVelocity * 60;
						StepMotion.Time_Value_Rot[2] = Mathf.Abs(StepMotion.RotDirection.z) / StepMotion.RotVelocity * 60;
						RotPos = program_Pos;
						StepMotion.SetTargetPos (DisplayPos,VirtualPos,RotPos);
					}else if(StepDate.IsCancel){
						program_Rot = Vector3.zero;
						Modal_State.RotReset ();
						StepMotion.Matri3 = Modal_State.GetMatri3;
						StepMotion.ToolVec = Modal_State.ToolVec;
						
						DisplayPos = ModalState._PosRot (program_Rot,Modal_State.PlaneAng,DisplayPos);
						Modal_State.PlaneAng = program_Rot;
						StepMotion.SetStartPos (DisplayPos,VirtualPos,RotPos);
						StepMotion.Direction_D = Vector3.zero;
						StepMotion.Direction_V = Vector3.zero;
						StepMotion.RotDirection = RotDirection (StepDate,program_Rot,RotPos);
						StepMotion.RotVelocity = Modal_State.RotVelocity;
						StepMotion.Time_Value_Rot[0] = Mathf.Abs(StepMotion.RotDirection.x) / StepMotion.RotVelocity * 60;
						StepMotion.Time_Value_Rot[1] = Mathf.Abs(StepMotion.RotDirection.y) / StepMotion.RotVelocity * 60;
						StepMotion.Time_Value_Rot[2] = Mathf.Abs(StepMotion.RotDirection.z) / StepMotion.RotVelocity * 60;
						RotPos = program_Pos;
						StepMotion.SetTargetPos (DisplayPos,VirtualPos,RotPos);
					}else{
						_errorMessage =  "Line:" + (Index+1) + " " + "CYCL DEF指令定义错误！";
						if(CompileParas._DEBUG) _errorMessage += "% ErrorDBG-Motion-117 %";
						_compileInfo.Add (ErrorMessage);
						return false;
					}
					
					if(Modal_State.Q_Value.ContainsKey ("Q120")){
						Modal_State.Q_Value["Q120"]=program_Rot.x;
					}else{
						Modal_State.Q_Value.Add ("Q120",program_Rot.x);
					}
					if(Modal_State.Q_Value.ContainsKey ("Q121")){
						Modal_State.Q_Value["Q121"]=program_Rot.y;
					}else{
						Modal_State.Q_Value.Add ("Q121",program_Rot.y);
					}
					if(Modal_State.Q_Value.ContainsKey ("Q122")){
						Modal_State.Q_Value["Q122"]=program_Rot.z;
					}else{
						Modal_State.Q_Value.Add ("Q122",program_Rot.z);
					}
					break;
				#endregion
				#region CYCL32
				case (int)MotionType.CYCL32:
					//暂不处理
					break;
				#endregion
				#region PLANE
				case (int)MotionType.PLANE:
					switch(StepDate._PLANE.Type){
					#region SPATIAL
					case PlaneType.SPATIAL:
						StepDate.ABC_State = StepDate._PLANE.SP_State;
						if(StepDate._PLANE.SP_State[0]){
							int Rotcal_flag  = RotCal (Index,ref RotPos,ref program_Rot,ref StepDate,StepDate._PLANE.SP_State[1],StepDate._PLANE.SP_State[2],StepDate._PLANE.SP_State[3]);
							if(Rotcal_flag == 2){
								return false;
							}
							if(Rotcal_flag == 1){
								program_Rot = RotPos;
							}
							DisplayPos = ModalState._PosRot (program_Rot,Modal_State.PlaneAng,DisplayPos);
							Modal_State.PlaneAng = program_Rot;
							StepMotion.SetStartPos (DisplayPos,VirtualPos,RotPos);
							if(StepDate._PLANE.MoveType == (int)PlaneMoveType.STAY){
								StepMotion.Direction_D = Vector3.zero;
								StepMotion.Direction_V = Vector3.zero;
							}else if(StepDate._PLANE.MoveType == (int)PlaneMoveType.MOVE){
								if(StepDate.IsSingleValue){
									StepMotion.Direction_D = Vector3.zero;
									StepMotion.Direction_V = Vector3.zero;
//									program_Pos = DisplayPos + new Vector3(0,0,StepDate.SingleValue);
//									StepMotion.Direction_D = new Vector3(0,0,StepDate.SingleValue);
//									StepMotion.Direction_V = Modal_State.PosRot (StepMotion.Direction_D);
//									if(StepDate.F2_value != 0){
//										if(Modal_State.M136){
//											StepMotion.Velocity = StepDate.F2_value * Modal_State.RotSpeed;
//										}else{
//											StepMotion.Velocity = StepDate.F2_value;
//										}
//									}else{
//										_errorMessage =  "Line:" + (Index+1) + " " + "PLANE SPATIAL指令定义错误！";
//										_compileInfo.Add (ErrorMessage);
//										return false;
//									}
//									StepMotion.Time_Value = StepMotion.Direction_D.magnitude / StepMotion.Velocity * 60;
								}else{
									_errorMessage =  "Line:" + (Index+1) + " " + "PLANE SPATIAL指令定义错误！";
									if(CompileParas._DEBUG) _errorMessage += "% ErrorDBG-Motion-118 %";
									_compileInfo.Add (ErrorMessage);
									return false;
								}
							}else if(StepDate._PLANE.MoveType == (int)PlaneMoveType.TURN){
								StepMotion.Direction_D = Vector3.zero;
								StepMotion.Direction_V = Vector3.zero;
							}
							Modal_State.RotA (program_Rot.x);
							Modal_State.RotB (program_Rot.y);
							Modal_State.RotC (program_Rot.z);
							StepMotion.RotDirection = RotDirection (StepDate,program_Rot,RotPos);
							StepMotion.RotVelocity = Modal_State.RotVelocity;
							StepMotion.Time_Value_Rot[0] = Mathf.Abs(StepMotion.RotDirection.x) / StepMotion.RotVelocity * 60;
							StepMotion.Time_Value_Rot[1] = Mathf.Abs(StepMotion.RotDirection.y) / StepMotion.RotVelocity * 60;
							StepMotion.Time_Value_Rot[2] = Mathf.Abs(StepMotion.RotDirection.z) / StepMotion.RotVelocity * 60;
							RotPos = program_Rot;
							DisplayPos = program_Pos;
							VirtualPos += StepMotion.Direction_V;
							StepMotion.SetTargetPos (DisplayPos,VirtualPos,RotPos);
						}else{
							_errorMessage =  "Line:" + (Index+1) + " " + "PLANE SPATIAL指令定义错误！";
							if(CompileParas._DEBUG) _errorMessage += "% ErrorDBG-Motion-119 %";
							_compileInfo.Add (ErrorMessage);
							return false;
						}
						break;
					#endregion
					#region PROJECTED
					case PlaneType.PROJECTED:
						
						break;
					#endregion
					#region EULER
					case PlaneType.EULER:
						
						break;
					#endregion
					#region VECTOR
					case PlaneType.VECTOR:
						Vector3 X_Axis = new Vector3(StepDate._PLANE.BX,StepDate._PLANE.BY,StepDate._PLANE.BZ);
						Vector3 Z_Axis = new Vector3(StepDate._PLANE.NX,StepDate._PLANE.NY,StepDate._PLANE.NZ);
						Vector3 Y_Axis = Vector3.Cross (Z_Axis,X_Axis);
						switch(Mode){
						case 1://A+C
						case 2:
							Vector3 vecY = new Vector3(0f,Y_Axis.y,Y_Axis.z);//Y_axis projected on YZ plane
							float Alpha_1 = Mathf.Asin (Vector3.Cross (new Vector3(0,1,0),vecY).magnitude/(1*vecY.magnitude));//A
							float Beta_1 = Mathf.Asin (Vector3.Cross (Y_Axis,vecY).magnitude/(Y_Axis.magnitude*vecY.magnitude));//C
							StepDate.ABC_State[0] = true;
							StepDate.ABC_State[1] = true;	StepDate.A_value = Alpha_1;
							StepDate.ABC_State[3] = true;	StepDate.C_value = Beta_1;
							if(Mode == 1){
								StepMotion.ToolVec = Modal_State.RotA (Alpha_1);
							}
							break;
						case 3://B+C
						case 4:
							Vector3 vecX = new Vector3(X_Axis.x, 0f, X_Axis.z);//X_axis pprojected on XZ plane
							float Alpha_2 = Mathf.Asin (Vector3.Cross (new Vector3(1,0,0),vecX).magnitude/(1*vecX.magnitude));//B
							float Beta_2 = Mathf.Asin (Vector3.Cross (X_Axis,vecX).magnitude/(X_Axis.magnitude*vecX.magnitude));//C
							StepDate.ABC_State[0] = true;
							StepDate.ABC_State[2] = true;	StepDate.B_value = Alpha_2;
							StepDate.ABC_State[3] = true;	StepDate.C_value = Beta_2;
							if(Mode == 3){
								StepMotion.ToolVec = Modal_State.RotB (Alpha_2);
							}
							break;
						case 5://A+B
						case 6:
							break;
						default:
							break;
						}
						int Rotcal_flag_vec  = RotCal (Index,ref RotPos,ref program_Rot,ref StepDate,true,true,true);
						if(Rotcal_flag_vec == 2){
							return false;
						}
						if(Rotcal_flag_vec == 1){
							program_Rot = RotPos;
						}
						DisplayPos = ModalState._PosRot (program_Rot,Modal_State.PlaneAng,DisplayPos);
						Modal_State.PlaneAng = program_Rot;
						StepMotion.SetStartPos (DisplayPos,VirtualPos,RotPos);
						if(StepDate._PLANE.MoveType == (int)PlaneMoveType.STAY){
							StepMotion.Direction_D = Vector3.zero;
							StepMotion.Direction_V = Vector3.zero;
						}else if(StepDate._PLANE.MoveType == (int)PlaneMoveType.MOVE){
							if(StepDate.IsSingleValue){
								StepMotion.Direction_D = Vector3.zero;
								StepMotion.Direction_V = Vector3.zero;
//									program_Pos = DisplayPos + new Vector3(0,0,StepDate.SingleValue);
//									StepMotion.Direction_D = new Vector3(0,0,StepDate.SingleValue);
//									StepMotion.Direction_V = Modal_State.PosRot (StepMotion.Direction_D);
//									if(StepDate.F2_value != 0){
//										if(Modal_State.M136){
//											StepMotion.Velocity = StepDate.F2_value * Modal_State.RotSpeed;
//										}else{
//											StepMotion.Velocity = StepDate.F2_value;
//										}
//									}else{
//										_errorMessage =  "Line:" + (Index+1) + " " + "PLANE SPATIAL指令定义错误！";
//										_compileInfo.Add (ErrorMessage);
//										return false;
//									}
//									StepMotion.Time_Value = StepMotion.Direction_D.magnitude / StepMotion.Velocity * 60;
							}else{
								_errorMessage =  "Line:" + (Index+1) + " " + "PLANE SPATIAL指令定义错误！";
								if(CompileParas._DEBUG) _errorMessage += "% ErrorDBG-Motion-120 %";
								_compileInfo.Add (ErrorMessage);
								return false;
							}
						}else if(StepDate._PLANE.MoveType == (int)PlaneMoveType.TURN){
							StepMotion.Direction_D = Vector3.zero;
							StepMotion.Direction_V = Vector3.zero;
						}
						StepMotion.Direction_D = Vector3.zero;
						StepMotion.RotDirection = RotDirection (StepDate,program_Rot,RotPos);
						StepMotion.RotVelocity = Modal_State.RotVelocity;
						StepMotion.Time_Value_Rot[0] = Mathf.Abs(StepMotion.RotDirection.x) / StepMotion.RotVelocity * 60;
						StepMotion.Time_Value_Rot[1] = Mathf.Abs(StepMotion.RotDirection.y) / StepMotion.RotVelocity * 60;
						StepMotion.Time_Value_Rot[2] = Mathf.Abs(StepMotion.RotDirection.z) / StepMotion.RotVelocity * 60;
						RotPos = program_Rot;
						StepMotion.SetTargetPos (DisplayPos,VirtualPos,RotPos);
						break;
					#endregion
					#region POINTS
					case PlaneType.POINTS:
						Vector3 vec1 = new Vector3(StepDate._PLANE.P2X-StepDate._PLANE.P1X,StepDate._PLANE.P2Y-StepDate._PLANE.P1Y,StepDate._PLANE.P2Z-StepDate._PLANE.P1Z);
						Vector3 vec2 = new Vector3(StepDate._PLANE.P3X-StepDate._PLANE.P1X,StepDate._PLANE.P3Y-StepDate._PLANE.P1Y,StepDate._PLANE.P3Z-StepDate._PLANE.P1Z);
						Vector3 Z_axis = Vector3.Cross (vec1,vec2);
						Vector3 X_axis = vec1;
						Vector3 Y_axis = Vector3.Cross (Z_axis,X_axis);
						switch(Mode){
						case 1://A+C
						case 2:
							Vector3 vecY = new Vector3(0f, Y_axis.y, Y_axis.z);//Y_axis projected on YZ plane
							float Alpha_1 = Mathf.Asin (Vector3.Cross (new Vector3(0,1,0),vecY).magnitude/(1*vecY.magnitude));//A
							float Beta_1 = Mathf.Asin (Vector3.Cross (Y_axis,vecY).magnitude/(Y_axis.magnitude*vecY.magnitude));//C
							StepDate.ABC_State[0] = true;
							StepDate.ABC_State[1] = true;	StepDate.A_value = Alpha_1;
							StepDate.ABC_State[3] = true;	StepDate.C_value = Beta_1;
							break;
						case 3://B+C
						case 4:
							Vector3 vecX = new Vector3(X_axis.x, 0f, X_axis.z);//X_axis pprojected on XZ plane
							float Alpha_2 = Mathf.Asin (Vector3.Cross (new Vector3(1,0,0),vecX).magnitude/(1*vecX.magnitude));//B
							float Beta_2 = Mathf.Asin (Vector3.Cross (X_axis,vecX).magnitude/(X_axis.magnitude*vecX.magnitude));//C
							StepDate.ABC_State[0] = true;
							StepDate.ABC_State[2] = true;	StepDate.B_value = Alpha_2;
							StepDate.ABC_State[3] = true;	StepDate.C_value = Beta_2;
							break;
						case 5://A+B
						case 6:
							break;
						default:
							break;
						}
						int Rotcal_flag_point  = RotCal (Index,ref RotPos,ref program_Rot,ref StepDate,true,true,true);
						if(Rotcal_flag_point == 2){
							return false;
						}						
						if(Rotcal_flag_point == 1){
							program_Rot = RotPos;
						}
						DisplayPos = ModalState._PosRot (program_Rot,Modal_State.PlaneAng,DisplayPos);
						Modal_State.PlaneAng = program_Rot;
						if(StepDate._PLANE.MoveType == (int)PlaneMoveType.STAY){
							StepMotion.Direction_D = Vector3.zero;
							StepMotion.Direction_V = Vector3.zero;
						}else if(StepDate._PLANE.MoveType == (int)PlaneMoveType.MOVE){
							if(StepDate.IsSingleValue){
								StepMotion.Direction_D = Vector3.zero;
								StepMotion.Direction_V = Vector3.zero;
								//									program_Pos = DisplayPos + new Vector3(0,0,StepDate.SingleValue);
								//									StepMotion.Direction_D = new Vector3(0,0,StepDate.SingleValue);
								//									StepMotion.Direction_V = Modal_State.PosRot (StepMotion.Direction_D);
								//									if(StepDate.F2_value != 0){
								//										if(Modal_State.M136){
								//											StepMotion.Velocity = StepDate.F2_value * Modal_State.RotSpeed;
								//										}else{
								//											StepMotion.Velocity = StepDate.F2_value;
								//										}
								//									}else{
								//										_errorMessage =  "Line:" + (Index+1) + " " + "PLANE SPATIAL指令定义错误！";
								//										_compileInfo.Add (ErrorMessage);
								//										return false;
								//									}
								//									StepMotion.Time_Value = StepMotion.Direction_D.magnitude / StepMotion.Velocity * 60;
							}else{
								_errorMessage =  "Line:" + (Index+1) + " " + "PLANE SPATIAL指令定义错误！";
								if(CompileParas._DEBUG) _errorMessage += "% ErrorDBG-Motion-121 %";
								_compileInfo.Add (ErrorMessage);
								return false;
							}
						}else if(StepDate._PLANE.MoveType == (int)PlaneMoveType.TURN){
							StepMotion.Direction_D = Vector3.zero;
							StepMotion.Direction_V = Vector3.zero;
						}
						Modal_State.RotA (program_Rot.x);
						Modal_State.RotB (program_Rot.y);
						Modal_State.RotC (program_Rot.z);
						StepMotion.SetStartPos (DisplayPos,VirtualPos,RotPos);
						StepMotion.Direction_D = Vector3.zero;
						StepMotion.RotDirection = RotDirection (StepDate,program_Rot,RotPos);
						StepMotion.RotVelocity = Modal_State.RotVelocity;
						StepMotion.Time_Value_Rot[0] = Mathf.Abs(StepMotion.RotDirection.x) / StepMotion.RotVelocity * 60;
						StepMotion.Time_Value_Rot[1] = Mathf.Abs(StepMotion.RotDirection.y) / StepMotion.RotVelocity * 60;
						StepMotion.Time_Value_Rot[2] = Mathf.Abs(StepMotion.RotDirection.z) / StepMotion.RotVelocity * 60;
						RotPos = program_Rot;
						StepMotion.SetTargetPos (DisplayPos,VirtualPos,RotPos);
						break;
					#endregion
					#region REL_SPA
					case PlaneType.RELATIVE:
						StepDate.IABC_State = StepDate._PLANE.SP_State;
						if(StepDate._PLANE.SP_State[0]){
							int Rotcal_flag  = RotCal (Index,ref RotPos,ref program_Rot,ref StepDate,StepDate._PLANE.SP_State[1],StepDate._PLANE.SP_State[2],StepDate._PLANE.SP_State[3]);
							if(Rotcal_flag == 2){
								return false;
							}
							if(Rotcal_flag == 1){
								program_Rot = RotPos;
							}
							DisplayPos = ModalState._PosRot (program_Rot,Modal_State.PlaneAng,DisplayPos);
							Modal_State.PlaneAng = program_Rot;
							StepMotion.SetStartPos (DisplayPos,VirtualPos,RotPos);
							if(StepDate._PLANE.MoveType == (int)PlaneMoveType.STAY){
								StepMotion.Direction_D = Vector3.zero;
								StepMotion.Direction_V = Vector3.zero;
							}else if(StepDate._PLANE.MoveType == (int)PlaneMoveType.MOVE){
								if(StepDate.IsSingleValue){
									StepMotion.Direction_D = Vector3.zero;
									StepMotion.Direction_V = Vector3.zero;
									//									program_Pos = DisplayPos + new Vector3(0,0,StepDate.SingleValue);
									//									StepMotion.Direction_D = new Vector3(0,0,StepDate.SingleValue);
									//									StepMotion.Direction_V = Modal_State.PosRot (StepMotion.Direction_D);
									//									if(StepDate.F2_value != 0){
									//										if(Modal_State.M136){
									//											StepMotion.Velocity = StepDate.F2_value * Modal_State.RotSpeed;
									//										}else{
									//											StepMotion.Velocity = StepDate.F2_value;
									//										}
									//									}else{
									//										_errorMessage =  "Line:" + (Index+1) + " " + "PLANE SPATIAL指令定义错误！";
									//										_compileInfo.Add (ErrorMessage);
									//										return false;
									//									}
									//									StepMotion.Time_Value = StepMotion.Direction_D.magnitude / StepMotion.Velocity * 60;
								}else{
									_errorMessage =  "Line:" + (Index+1) + " " + "PLANE SPATIAL指令定义错误！";
									if(CompileParas._DEBUG) _errorMessage += "% ErrorDBG-Motion-122 %";
									_compileInfo.Add (ErrorMessage);
									return false;
								}
							}else if(StepDate._PLANE.MoveType == (int)PlaneMoveType.TURN){
								StepMotion.Direction_D = Vector3.zero;
								StepMotion.Direction_V = Vector3.zero;
							}
							Modal_State.RotA (program_Rot.x);
							Modal_State.RotB (program_Rot.y);
							Modal_State.RotC (program_Rot.z);
							StepMotion.RotDirection = RotDirection (StepDate,program_Rot,RotPos);
							StepMotion.RotVelocity = Modal_State.RotVelocity;
							StepMotion.Time_Value_Rot[0] = Mathf.Abs(StepMotion.RotDirection.x) / StepMotion.RotVelocity * 60;
							StepMotion.Time_Value_Rot[1] = Mathf.Abs(StepMotion.RotDirection.y) / StepMotion.RotVelocity * 60;
							StepMotion.Time_Value_Rot[2] = Mathf.Abs(StepMotion.RotDirection.z) / StepMotion.RotVelocity * 60;
							RotPos = program_Rot;
							StepMotion.SetTargetPos (DisplayPos,VirtualPos,RotPos);
						}else{
							_errorMessage =  "Line:" + (Index+1) + " " + "PLANE SPATIAL指令定义错误！";
							if(CompileParas._DEBUG) _errorMessage += "% ErrorDBG-Motion-123 %";
							_compileInfo.Add (ErrorMessage);
							return false;
						}
						break;
					#endregion
					#region RESET
					case PlaneType.RESET:
						program_Rot = Vector3.zero;
						DisplayPos = ModalState._PosRot (program_Rot,Modal_State.PlaneAng,DisplayPos);
						Modal_State.PlaneAng = program_Rot;
						StepMotion.SetStartPos (DisplayPos,VirtualPos,RotPos);
						if(StepDate._PLANE.MoveType == (int)PlaneMoveType.STAY){
							StepMotion.Direction_D = Vector3.zero;
							StepMotion.Direction_V = Vector3.zero;
						}else if(StepDate._PLANE.MoveType == (int)PlaneMoveType.MOVE){
							if(StepDate.IsSingleValue){
								StepMotion.Direction_D = Vector3.zero;
								StepMotion.Direction_V = Vector3.zero;
//									program_Pos = DisplayPos + new Vector3(0,0,StepDate.SingleValue);
//									StepMotion.Direction_D = new Vector3(0,0,StepDate.SingleValue);
//									StepMotion.Direction_V = Modal_State.PosRot (StepMotion.Direction_D);
//									if(StepDate.F2_value != 0){
//										if(Modal_State.M136){
//											StepMotion.Velocity = StepDate.F2_value * Modal_State.RotSpeed;
//										}else{
//											StepMotion.Velocity = StepDate.F2_value;
//										}
//									}else{
//										_errorMessage =  "Line:" + (Index+1) + " " + "PLANE SPATIAL指令定义错误！";
//										_compileInfo.Add (ErrorMessage);
//										return false;
//									}
//									StepMotion.Time_Value = StepMotion.Direction_D.magnitude / StepMotion.Velocity * 60;
							}else{
								_errorMessage =  "Line:" + (Index+1) + " " + "PLANE SPATIAL指令定义错误！";
								if(CompileParas._DEBUG) _errorMessage += "% ErrorDBG-Motion-124 %";
								_compileInfo.Add (ErrorMessage);
								return false;
							}
						}else if(StepDate._PLANE.MoveType == (int)PlaneMoveType.TURN){
							StepMotion.Direction_D = Vector3.zero;
							StepMotion.Direction_V = Vector3.zero;
						}
						Modal_State.RotReset ();
						StepMotion.Matri3 = Modal_State.GetMatri3;
						StepMotion.ToolVec = Modal_State.ToolVec;
						StepMotion.RotDirection = RotDirection (StepDate,program_Rot,RotPos);
						StepMotion.RotVelocity = Modal_State.RotVelocity;
						StepMotion.Time_Value_Rot[0] = Mathf.Abs(StepMotion.RotDirection.x) / StepMotion.RotVelocity * 60;
						StepMotion.Time_Value_Rot[1] = Mathf.Abs(StepMotion.RotDirection.y) / StepMotion.RotVelocity * 60;
						StepMotion.Time_Value_Rot[2] = Mathf.Abs(StepMotion.RotDirection.z) / StepMotion.RotVelocity * 60;
						RotPos = program_Pos;
						StepMotion.SetTargetPos (DisplayPos,VirtualPos,RotPos);
						break;
					#endregion
					default:
						break;
					}
					StepMotion.Matri3 = Modal_State.GetMatri3;
					break;
				#endregion
				default:
					break;
				#endregion
				}
			}
			
		}
		
		
		return true;
	}
}



public class HeidenhainCompileBase:Lexical_Check,ICompile
{
	//Mode
	//1:旋转轴为A+C 刀具轴可旋转
	//2:旋转轴为A+C 刀具轴不可旋转
	//3:旋转轴为B+C 刀具轴可旋转
	//4:旋转轴为B+C 刀具轴不可旋转
	//5:旋转轴为A+B 刀具轴可旋转
	public HeidenhainCompileBase(int _Mode,ref ModalState MState):base()
	{
		Mode = _Mode;
		Modal_State.ModeClone (MState);
		Modal_State.MODE = _Mode;
	}
	
	public override string ErrorMessage
	{
		get {return _errorMessage;}
	}
	
	public override List<string> CompileInfo
	{
		get {return _compileInfo;}
		
		set {
			foreach(string str in value){
				_compileInfo.Add (str);
			}
		}
	}
	
	public bool CompileEntrance(List<List<string>> CodeSeg,ref MotionInfo StepMotion,ref DataStore StepData,ref int Pindex){
		StepMotion = new MotionInfo();
		StepData   = new DataStore();
		
		Vector3 VirtualPos = Vector3.zero;
		Vector3 CurrentPos = Vector3.zero;
		Vector3 Rot 	   = Vector3.zero;

		//VirtualPos = Move_Motion.CurrentVirtualPos ();
		//VirtualPos += VecForCompensation; /* to correct virtual pos to program pos before compensation */
		//CurrentPos = VirtualPos + Modal_State.CooZero;
		VirtualPos = CompileParas.GetCurrentMachinePos;
		CurrentPos = CompileParas.GetCurrentPartPos;
		Rot 	   = CompileParas.v3GetRotForThread;
			
		if(!CodeCheck (CodeSeg[Pindex], CodeSeg, ref Pindex, ref StepData, ref StepMotion, ref CurrentPos, ref VirtualPos, ref Rot)){
			return false;
		}
		
		//CHF RND
		if(StepMotion.Motion_Type == (int)MotionType.CHF || StepMotion.Motion_Type == (int)MotionType.RND){
			StepMotion.MotionCopy (ChamferBak);
			if(StepMotion.Motion_Type != (int)MotionType.Line || !StepMotion.HasMotion ()){
				string _error = "Line:" + (Pindex+1) + " " + "倒角(圆)指令错误！";
				if(CompileParas._DEBUG) _error += "% ErrorDBG-Motion-125 %";
				_compileInfo.Add (_error);
				return false;
			}
		}else{
			if(ChamferCal_OneLine (CodeSeg, ref StepMotion, ref Rot, Pindex) == 0){
				return false;
			}
		}
		
		//LengthCompensation
		if(LengthCompCal_OneLine (ref StepMotion, Modal_State) == 0)
			return false;
		
		//RadiusCompensation
		//if(RadiusCompCal_OneLine (CodeSeg, ref StepMotion, ref Rot, Pindex) == 0)
		//	return false;
		
		#region MotionDealForCompileInOne
		/*
//		motion_data.Clear ();
		Vector3 VirtualPos = MachineAxis.GetPos;
		Vector3 CurrentPos = VirtualPos + Modal_State.CooZero;
		Vector3 Rot = MachineAxis.GetRot;
		List<DataStore> StepData_List = new List<DataStore>();
		DataStore Step_data = new DataStore();
		MotionInfo Step_Motion = new MotionInfo();
		
		for(int i = 0;i < CodeSeg.Count;i++){
			Step_data = new DataStore();
			Step_Motion = new MotionInfo();
			
			if(!CodeCheck (CodeSeg[i],CodeSeg,ref i,ref Step_data,ref Step_Motion,ref motion_data,ref CurrentPos,ref VirtualPos,ref Rot)){
				return false;
			}else{
				StepData_List.Add (Step_data);
				motion_data.Add (Step_Motion);
			}
		}
		
		
		//CHF RND
		if(ChamferCal (ref motion_data,StepData_List) == 0)
			return false;
			
		//LengthCompensation
		if(LengthCompCal (ref motion_data) == 0)
			return false;
			
		//RadiusCompensation
		if(RadiusCompCal (ref motion_data) == 0)
			return false;
		*/
		#endregion
		Pindex ++;
		return true;
	}
	
	#region Compensation
	#region ChamferCal
	private int ChamferCal(ref List<MotionInfo> motion_data,List<DataStore> Data_List){
		int former = 0;
		int next = 0;
		string _error = "";
		MotionInfo motion1 = new MotionInfo();
		MotionInfo motion2 = new MotionInfo();
		MotionInfo motion3 = new MotionInfo();
		Chamfer Chamfer_cal = new Chamfer(Mode);
		for(int i = 0;i < motion_data.Count;i++){
			if(motion_data[i].Motion_Type == (int)MotionType.CHF || motion_data[i].Motion_Type == (int)MotionType.RND){
				if(i != 0 && i != motion_data.Count-1){
					former = i;next = i;
					for(int j = next+1;j < motion_data.Count;j++){
						if(motion_data[j].Motion_Type != -1 && motion_data[j].HasMotion ()){
							next = j;
							break;
						}
					}
					for(int k = former-1;k >= 0;k--){
						if(motion_data[k].Motion_Type != -1 && motion_data[k].HasMotion ()){
							former = k;
							break;
						}
					}
					if(former < i && motion_data[former].Motion_Type == (int)MotionType.Line 
						&& next > i && motion_data[next].Motion_Type == (int)MotionType.Line){
						motion1.MotionCopy (motion_data[former]);
						motion2.MotionCopy (motion_data[i]);
						motion3.MotionCopy (motion_data[next]);
						if(motion2.Motion_Type == (int)MotionType.CHF){
							if(!Chamfer_cal.Chamfer_C (ref motion1,ref motion2,ref motion3,Data_List[i].CHF_value,ref _compileInfo)){
								return 0;
							}
						}else if(motion2.Motion_Type == (int)MotionType.RND){
							if(!Chamfer_cal.Chamfer_R (ref motion1,ref motion2,ref motion3,Data_List[i].RND_value,ref _compileInfo)){
								return 0;
							}
						}
						motion_data[former].MotionCopy (motion1);
						motion_data[i].MotionCopy (motion2);
						motion_data[next].MotionCopy (motion3);
					}else{
						_error = "Line:" + (i+1) + " " + "倒角(圆)指令错误！";
						if(CompileParas._DEBUG) _error += "% ErrorDBG-Motion-126 %";
						_compileInfo.Add (_error);
						return 0;
					}
				}else{
					_error = "Line:" + (i+1) + " " + "倒角(圆)指令错误！";
					if(CompileParas._DEBUG) _error += "% ErrorDBG-Motion-127 %";
					_compileInfo.Add (_error);
					return 0;
				}
			}
			
		}
		return 1;
	}
	
	private int ChamferCal_OneLine(List<List<string>> CodeSeg, ref MotionInfo StepMotion, ref Vector3 Rot, int Pindex){
		if (StepMotion.Motion_Type == (int)MotionType.Line && StepMotion.HasMotion () && !NoChamfer){
			Chamfer Chamfer_cal 		= new Chamfer(Mode);
			string _error 				= "";
			MotionInfo TempMotion 		= new MotionInfo();
			DataStore TempData 			= new DataStore();
			MotionInfo TempMotion_next  = new MotionInfo();
			DataStore TempData_next 	= new DataStore();
			bool CheckFlag 				= true;
			int _MotionTypeNext 		= -1;
			int _Pindex 				= Pindex;
			int CHF_Index;
			
			while(_MotionTypeNext == -1){
				_Pindex++;
				if(_Pindex >= CodeSeg.Count){
					NoChamfer = true;
					return 1;
				}
				_MotionTypeNext = MotionTypeCheck (CodeSeg[_Pindex], _Pindex,ref CheckFlag);
				if(!CheckFlag){
					return 0;
				}
			}
			/* Next is CHF or RND then to Calculate */
			if(_MotionTypeNext == (int)MotionType.CHF || _MotionTypeNext == (int)MotionType.RND){
				if(!CodeCheck (CodeSeg[_Pindex], CodeSeg, ref _Pindex, ref TempData, ref TempMotion, ref StepMotion.DisplayTarget, ref StepMotion.VirtualTarget, ref Rot)){
					return 0;
				}
				CHF_Index = _Pindex;
			}else{
				return 1;
			}
			
			/* if find CHF or RND instruction,then to find next Line Motion */
			_MotionTypeNext = -1;
			while(_MotionTypeNext == -1){
				if(_Pindex >= CodeSeg.Count){
					_error = "Line:" + (CHF_Index+1) + " " + "倒角(圆)指令错误！";
					if(CompileParas._DEBUG) _error += "% ErrorDBG-Motion-142 %";
					_compileInfo.Add (_error);
					return 0;
				}
				_Pindex++;
				_MotionTypeNext = MotionTypeCheck (CodeSeg[_Pindex], _Pindex,ref CheckFlag);
				if(!CheckFlag){
					return 0;
				}
			}
			if(_MotionTypeNext == (int)MotionType.Line){
				if(!CodeCheck (CodeSeg[_Pindex], CodeSeg, ref _Pindex, ref TempData_next, ref TempMotion_next, ref StepMotion.DisplayTarget, ref StepMotion.VirtualTarget, ref Rot)){
					return 0;
				}
			}else{
				_error = "Line:" + (CHF_Index+1) + " " + "倒角(圆)指令错误！";
				if(CompileParas._DEBUG) _error += "% ErrorDBG-Motion-128 %";
				_compileInfo.Add (_error);
				return 0;
			}

			if(StepMotion.Motion_Type == (int)MotionType.Line && TempMotion_next.Motion_Type == (int)MotionType.Line){
				if(TempMotion.Motion_Type == (int)MotionType.CHF){
					if(!Chamfer_cal.Chamfer_C (ref StepMotion,ref TempMotion,ref TempMotion_next,TempData.CHF_value,ref _compileInfo)){
						return 0;
					}
				}else if(TempMotion.Motion_Type == (int)MotionType.RND){
					if(!Chamfer_cal.Chamfer_R (ref StepMotion,ref TempMotion,ref TempMotion_next,TempData.RND_value,ref _compileInfo)){
						return 0;
					}
				}
				
				/* velocity correct */
				if(TempData.F_value != 0){
					TempMotion.Velocity = TempData.F_value;
					TempMotion.Time_Value = TempMotion.Direction_D.magnitude/TempMotion.Velocity*60;
				}
				
				ChamferBak.MotionCopy (TempMotion);
			}else{
				_error = "Line:" + (CHF_Index+1) + " " + "倒角(圆)指令错误！";
				if(CompileParas._DEBUG) _error += "% ErrorDBG-Motion-129 %";
				_compileInfo.Add (_error);
				return 0;
			}
			return 1;
		}else{
			return 1;
		}
	}
	#endregion
	
	#region LengthCom
	private int LengthCompCal(ref List<MotionInfo> motion_data){
		float L_Value = 0;
		Vector3 L_vec = Vector3.zero;
		Vector3 L_vec_bac = L_vec;
		Vector3 L_vec_standard = Vector3.zero;
		Vector3 L_vec_standard_bac = L_vec_standard;
		for(int i = 0;i < motion_data.Count;i++){
			if(motion_data[i].Motion_Type == (int)MotionType.TOOLCALL){
				L_Value = motion_data[i].L_Value + motion_data[i].DL_Value;
				L_vec_bac = L_vec;
				L_vec_standard_bac = L_vec_standard;
				L_vec = motion_data[i].ToolVec * L_Value;
				L_vec_standard = new Vector3(0,0,1) * L_Value;
				
				motion_data[i].DisplayStart += L_vec_standard_bac;
				motion_data[i].VirtualStart += L_vec_bac;
				motion_data[i].DisplayTarget += L_vec_standard;
				motion_data[i].VirtualTarget += L_vec;
				motion_data[i].Direction_D = motion_data[i].DisplayTarget-motion_data[i].DisplayStart;
				motion_data[i].Direction_V = motion_data[i].VirtualTarget-motion_data[i].VirtualStart;
				motion_data[i].Time_Value = motion_data[i].Direction_D.magnitude / motion_data[i].Velocity * 60;
			}else{
				motion_data[i].DisplayStart += L_vec_standard;
				motion_data[i].VirtualStart += L_vec;
				motion_data[i].DisplayTarget += L_vec_standard;
				motion_data[i].VirtualTarget += L_vec;
				//motion_data[i].Direction = motion_data[i].DisplayTarget-motion_data[i].DisplayStart;
				if(motion_data[i].Motion_Type != (int)MotionType.CR_P && motion_data[i].Motion_Type != (int)MotionType.CR_N){
					//motion_data[i].Time_Value = motion_data[i].Direction.magnitude / motion_data[i].Velocity * 60f;
				}else{
					motion_data[i].Center_Point_D += L_vec_standard;
				}
				if(motion_data[i].List_flag){
					for(int j = 0;j < motion_data[i].Motion_Type_List.Count;j++){
						motion_data[i].DisplayStart_List[j] += L_vec_standard;
						motion_data[i].VirtualStart_List[j] += L_vec;
						motion_data[i].DisplayTarget_List[j] += L_vec_standard;
						motion_data[i].VirtualTarget_List[j] += L_vec;
						//motion_data[i].Direction_List[j] = motion_data[i].DisplayTarget_List[j]-motion_data[i].DisplayStart_List[j];
						if(motion_data[i].Motion_Type_List[j] != (int)MotionType.CR_P && motion_data[i].Motion_Type_List[j] != (int)MotionType.CR_N){
							//motion_data[i].Time_Value = motion_data[i].Direction.magnitude / motion_data[i].Velocity * 60f;
						}else{
							motion_data[i].Center_Point_D_List[j] += L_vec_standard;
						}
					}
				}
				
			}
		}
		return 1;
	}
	
	private int LengthCompCal_OneLine(ref MotionInfo motion_data, ModalState _modal){
		float L_Value			 	= _modal.L_Value + _modal.DL_Value;
		float L_ValueBak 			= _modal.L_Value_bak + _modal.DL_Value_bak;
		Vector3 L_vec 				= _modal.ToolVec * L_Value;
		Vector3 L_vec_bac 			= _modal.ToolVecBak * L_ValueBak;
		Vector3 L_vec_standard 		= new Vector3(0,0,1) * L_Value;
		Vector3 L_vec_standard_bac  = new Vector3(0,0,1) * L_ValueBak;
		
		if(motion_data.Motion_Type == (int)MotionType.TOOLCALL){
			motion_data.DisplayStart  += L_vec_standard_bac;
			motion_data.VirtualStart  += L_vec_bac;
			motion_data.DisplayTarget += L_vec_standard;
			motion_data.VirtualTarget += L_vec;
			motion_data.Direction_D    = motion_data.DisplayTarget-motion_data.DisplayStart;
			motion_data.Direction_V    = motion_data.VirtualTarget-motion_data.VirtualStart;
			motion_data.Time_Value 	   = motion_data.Direction_D.magnitude / motion_data.Velocity * 60;
		}else{
			motion_data.DisplayStart  += L_vec_standard;
			motion_data.VirtualStart  += L_vec;
			motion_data.DisplayTarget += L_vec_standard;
			motion_data.VirtualTarget += L_vec;
			//motion_data[i].Direction = motion_data[i].DisplayTarget-motion_data[i].DisplayStart;
			if(motion_data.Motion_Type != (int)MotionType.CR_P && motion_data.Motion_Type != (int)MotionType.CR_N){
				//motion_data[i].Time_Value = motion_data[i].Direction.magnitude / motion_data[i].Velocity * 60f;
			}else{
				motion_data.Center_Point_D += L_vec_standard;
			}
			if(motion_data.List_flag){
				for(int j = 0;j < motion_data.Motion_Type_List.Count;j++){
					motion_data.DisplayStart_List[j]  += L_vec_standard;
					motion_data.VirtualStart_List[j]  += L_vec;
					motion_data.DisplayTarget_List[j] += L_vec_standard;
					motion_data.VirtualTarget_List[j] += L_vec;
					//motion_data[i].Direction_List[j] = motion_data[i].DisplayTarget_List[j]-motion_data[i].DisplayStart_List[j];
					if(motion_data.Motion_Type_List[j] != (int)MotionType.CR_P && motion_data.Motion_Type_List[j] != (int)MotionType.CR_N){
						//motion_data[i].Time_Value = motion_data[i].Direction.magnitude / motion_data[i].Velocity * 60f;
					}else{
						motion_data.Center_Point_D_List[j] += L_vec_standard;
					}
				}
			}
			
		}
		return 1;
	}
	#endregion
	
	#region RadiusComp
	private int RadiusCompCal(ref List<MotionInfo> motion_data){
		//		实例化该计算补偿的工具类
		Radius_CompCal CalFunction = new Radius_CompCal(Mode);
		
		MotionInfo motion_data1 = new MotionInfo();
		MotionInfo motion_data2 = new MotionInfo();
		MotionInfo motion_bac = new MotionInfo();
		int index1 = 0; //第一个数据结构的序号
		int index2 = 0; //第二个数据结构的序号
		//刀具半径补偿计算
		for(int i = 0; i < motion_data.Count; i++){
			//每次运行前都将创建圆弧的标志位置反
			CalFunction.Clear ();
			CalFunction.SetRValue (motion_data[i].R_Value+motion_data[i].DR_Value);
			//判断当前数据是否需要补偿
			if(motion_data[i].RadiusCompState != (int)CompState.NO && motion_data[i].Motion_Type != -1){
				index1 = i;
				motion_data1.MotionCopy (motion_data[index1]);

				//如果不是最后一个数据结构，则第二个参数传入下一个数据结构以判断补偿的类型
				if(i != motion_data.Count - 1){
					index2 = -1;
					for(int j = i + 1; j < motion_data.Count; j++){
						if(motion_data[j].Motion_Type != -1 && motion_data[j].HasMotion ()){
							index2 = j;
							break; //找到合适的数据结构，退出该循环
						}
					}
					if(index2 != -1){
						motion_data2 = motion_data[index2];
						if(!CalFunction.Calculate(ref motion_data1, ref motion_data2, motion_bac, false,ref _errorMessage,ref _compileInfo)){  //输入补偿数据，进行补偿
							return 0;
						}
						//判断是否要增加圆弧，需要则进入该分支
						if(CalFunction.HasCircle){
							MotionInfo motion_data_circle = new MotionInfo();
							motion_data_circle.index = motion_data[index2].index;
							motion_data_circle.Velocity = motion_data[index2].Velocity;
							motion_data_circle.Circle_r = CalFunction.Circle_radius;
							motion_data_circle.Center_Point_D = CalFunction.Circle_center;//motion_data1.DisplayTarget;
							motion_data_circle.DisplayStart = CalFunction.Circle_startpos;//motion_data1.DisplayTarget;
							motion_data_circle.DisplayTarget = CalFunction.Circle_endpos; //motion_data2.DisplayStart;
							motion_data_circle.Direction_D = motion_data_circle.DisplayTarget - motion_data_circle.DisplayStart;
							motion_data_circle.VirtualStart = motion_data1.VirtualTarget;
							motion_data_circle.VirtualTarget = motion_data_circle.VirtualStart + motion_data_circle.Direction_D;
							motion_data_circle.Rotate_Speed = motion_data_circle.Velocity / (60f * motion_data_circle.Circle_r) * (180 / Mathf.PI);
							CalFunction.CalculateForCircle (motion_data1,motion_data2,ref motion_data_circle);//计算方向和角度
							motion_data_circle.Time_Value = motion_data_circle.Rotate_Degree/motion_data_circle.Rotate_Speed;
							motion_data.Insert(index2, motion_data_circle);
							motion_bac.MotionCopy (motion_data_circle);
							
							motion_data[index2 + 1].MotionCopy(motion_data2);  //把修改好的数据传回给原始的数据结构
							i = index2;  //跳过新增的圆弧数据结构
						}else{
							motion_data[index2] = motion_data2; //把修改好的数据传回给原始的数据结构
							motion_bac.MotionCopy (motion_data1);
						}
					}else {
						motion_data2.MotionCopy(motion_data[i]);
						CalFunction.Calculate(ref motion_data1, ref motion_data2,motion_bac,true,ref _errorMessage,ref _compileInfo);
					}
				}
				//最后一个数据结构，一般为刀具补偿取消，传入同样的两个参数，只需要修改其中一个即可
				else{
					//考虑最后一个运动为圆弧
					motion_data2.MotionCopy(motion_data[i]);
					CalFunction.Calculate(ref motion_data1, ref motion_data2,motion_bac,true,ref _errorMessage,ref _compileInfo);
				}
				motion_data[index1].MotionCopy (motion_data1);  //把修改好的数据传回给原始的数据结构
			}
		}
		return 1;
	}
	
	private int RadiusCompCal_OneLine(List<List<string>> CodeSeg, ref MotionInfo StepMotion, ref Vector3 Rot, int Pindex){
		if(StepMotion.RadiusCompState != (int)CompState.NO && StepMotion.Motion_Type != -1){
			Radius_CompCal CalFunction  = new Radius_CompCal(Mode);
			string _error 				= "";
			MotionInfo TempMotion 		= new MotionInfo();
			DataStore TempData 			= new DataStore();
			MotionInfo MotionBak		= new MotionInfo();
			bool CheckFlag 				= true;
			bool Final					= false; /* to indicate whether the Motion is the last one or not */
			int _MotionTypeNext 		= -1;
			int _Pindex 				= Pindex;
			
			CalFunction.Clear ();
			CalFunction.SetRValue (StepMotion.R_Value+StepMotion.DR_Value);
			
			MotionBak.MotionCopy (StepMotion);
			
			while(_MotionTypeNext == -1){
				if(_Pindex >= CodeSeg.Count){
					Final = true;
					break;
				}
				_Pindex++;
				_MotionTypeNext = MotionTypeCheck (CodeSeg[_Pindex], _Pindex,ref CheckFlag);
				if(!CheckFlag){
					return 0;
				}
			}
			
			if(_MotionTypeNext != -1){
				if(_MotionTypeNext == (int)MotionType.CHF || _MotionTypeNext == (int)MotionType.RND){
					TempMotion.MotionCopy (ChamferBak);
				}else{
					if(!CodeCheck (CodeSeg[_Pindex], CodeSeg, ref _Pindex, ref TempData, ref TempMotion, ref StepMotion.DisplayTarget, ref StepMotion.VirtualTarget, ref Rot)){
						return 0;
					}
				}
				if(TempMotion.HasMotion ()){
					if(!CalFunction.Calculate(ref StepMotion, ref TempMotion, CompensationBak, false,ref _errorMessage,ref _compileInfo)){  //输入补偿数据，进行补偿
						return 0;
					}
					//判断是否要增加圆弧，需要则进入该分支
					if(CalFunction.HasCircle){
						MotionInfo motion_data_circle = new MotionInfo();
						motion_data_circle.index = TempMotion.index;
						motion_data_circle.Velocity = TempMotion.Velocity;
						motion_data_circle.Circle_r = CalFunction.Circle_radius;
						motion_data_circle.Center_Point_D = CalFunction.Circle_center;//motion_data1.DisplayTarget;
						motion_data_circle.DisplayStart = CalFunction.Circle_startpos;//motion_data1.DisplayTarget;
						motion_data_circle.DisplayTarget = CalFunction.Circle_endpos; //motion_data2.DisplayStart;
						motion_data_circle.Direction_D = motion_data_circle.DisplayTarget - motion_data_circle.DisplayStart;
						motion_data_circle.VirtualStart = StepMotion.VirtualTarget;
						motion_data_circle.VirtualTarget = motion_data_circle.VirtualStart + motion_data_circle.Direction_D;
						motion_data_circle.Rotate_Speed = motion_data_circle.Velocity / (60f * motion_data_circle.Circle_r) * (180 / Mathf.PI);
						CalFunction.CalculateForCircle (StepMotion,TempMotion,ref motion_data_circle);//计算方向和角度
						motion_data_circle.Time_Value = motion_data_circle.Rotate_Degree / motion_data_circle.Rotate_Speed;
//						motion_data.Insert(index2, motion_data_circle);
						CompensationBak.MotionCopy (motion_data_circle);
						StepMotion.Convert_ListMode ();
						StepMotion.ListMotionAdd (motion_data_circle);
					}else{
						CompensationBak.MotionCopy (StepMotion);
					}
				}else{
					return 1;
				}
			}else{
				if(Final){
					TempMotion.MotionCopy(StepMotion);
					CalFunction.Calculate(ref StepMotion, ref TempMotion, CompensationBak,true,ref _errorMessage,ref _compileInfo);
				}else{
					_error = "Line:" + (Pindex+1) + " " + "补偿指令错误！";
					if(CompileParas._DEBUG) _error += "% ErrorDBG-Motion-143 %";
					_compileInfo.Add (_error);
					return 0;
				}
			}
			
			VecForCompensation = MotionBak.DisplayTarget - StepMotion.DisplayTarget;
			
		}else{
			return 1;
		}

		return 1;
	}
	#endregion
	#endregion
}