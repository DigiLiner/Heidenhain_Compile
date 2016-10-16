using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;

#region enum Define
public enum MotionType
{
	NO = 0, Line=1, LineM91, CC, CR, CP, CR_P, CR_N, CHF, RND, 
	TOOLCALL, BLKFORM1, BLKFORM2, CYCL19, CYCL32, PLANE, M140,
	RR, RL, R0, LIST,
}

public enum ImmediateMotionType
{
	M00='a', M01, M02, M03, M04, M05, M06, M08, M09, M13, M14, M30, M91, M116, M117, M126,
	M127, M128, M129, M136, M137, M140, CYCL7, CYCL247, RadiusCompLeft, RadiusCompRight, 
	RadiusCompCancel, RotateSpeed, FeedSpeed, TOOLCALL, TOOLDEF
}

public enum RCompEnum
{
	R0 = 0, 
	RL, 
	RR
}

public enum CompState
{
	NO = 0, 
	Start, 
	Cancel, 
	Normal, 
	CancelInMiddle
}

public enum MUnit
{
	Metric=0,
	Inch
}

public enum PlaneType
{
	SPATIAL=0, 
	PROJECTED,
	EULER,
	VECTOR,
	POINTS,
	RELATIVE,
	RESET
}

public enum PlaneMoveType
{
	STAY = 0, 
	MOVE, 
	TURN
}

public enum Axis
{
	NO=0, 
	X, 
	Y, 
	Z
}
#endregion

public class CompileStuct_S{
	CompileStuct_S CStc_Next = null;
	CompileStuct_S CStc_Former = null;
	DataStore stepData = null;
	MotionInfo stepMotion = null;
	
	public CompileStuct_S(CompileStuct_S Former){
		if(Former != null){
			Former.CStc_Next = this;
		}
		CStc_Former = Former;
	}
	
	public bool Compile(ICompile Entr,List<List<string>> code,ref int Pindex){
		return Entr.CompileEntrance (code, ref stepMotion, ref stepData, ref Pindex);
	}
	
	public bool HaseMotion{
		get{
			return (stepMotion.HasMotion () && stepData.HasMotion ());
		}
	}
	
	public bool NotEmpty{
		get{
			return stepMotion.NotEmpty ();
		}
	}
	
	public CompileStuct_S Former{
		get{
			return CStc_Former;
		}
	}
	
	public CompileStuct_S Next{
		get{
			return CStc_Next;
		}
	}
	
	public MotionInfo Motion{
		get{
			return stepMotion;
		}
	}
	
	public DataStore Data{
		get{
			return stepData;
		}
	}
}

public class Matrix3x3{
	public Vector3 v0 = new Vector3(1,0,0);
	public Vector3 v1 = new Vector3(0,1,0);
	public Vector3 v2 = new Vector3(0,0,1);
	
	public void SetRow(int i,Vector3 ve){
		switch(i){
		case 0:
			v0 = ve;
			break;
		case 1:
			v1 = ve;
			break;
		case 2:
			v2 = ve;
			break;
		default:
			break;
		}
	}
	
	public static Matrix3x3 identity{
		get{
			Matrix3x3 ma = new Matrix3x3();
			return ma;
		}
	}
	
	/* x轴旋转矩阵,传入为角度值 */
	public static Matrix3x3 Rx(float theta){
		theta = (float)(Math.PI * theta / 180.0f);
		Matrix3x3 ma = new Matrix3x3();
		ma.v0 = new Vector3(1, 0, 0);
		ma.v1 = new Vector3(0, (float)Math.Cos (theta), (float)Math.Sin (theta));
		ma.v2 = new Vector3(0, (float)-Math.Sin (theta), (float)Math.Cos (theta));
		ma.v0 = VecRound (ma.v0);
		ma.v1 = VecRound (ma.v1);
		ma.v2 = VecRound (ma.v2);
		return ma;
	}
	
	/* y轴旋转矩阵,传入为角度值 */
	public static Matrix3x3 Ry(float theta){
		theta = (float)(Math.PI * theta / 180.0f);
		Matrix3x3 ma = new Matrix3x3();
		ma.v0 = new Vector3((float)Math.Cos (theta), 0, (float)-Math.Sin (theta));
		ma.v1 = new Vector3(0, 1, 0);
		ma.v2 = new Vector3((float)Math.Sin (theta), 0, (float)Math.Cos (theta));
		ma.v0 = VecRound (ma.v0);
		ma.v1 = VecRound (ma.v1);
		ma.v2 = VecRound (ma.v2);
		return ma;
	}
	
	/* z轴旋转矩阵,传入为角度值 */
	public static Matrix3x3 Rz(float theta){
		theta = (float)(Math.PI * theta / 180.0f);
		Matrix3x3 ma = new Matrix3x3();
		ma.v0 = new Vector3((float)Math.Cos (theta), (float)Math.Sin (theta), 0);
		ma.v1 = new Vector3((float)-Math.Sin (theta), (float)Math.Cos (theta), 0);
		ma.v2 = new Vector3(0, 0, 1);
		ma.v0 = VecRound (ma.v0);
		ma.v1 = VecRound (ma.v1);
		ma.v2 = VecRound (ma.v2);
		return ma;
	}
	
	public static Vector3 operator*(Matrix3x3 ma,Vector3 ve){
		Vector3 ret = Vector3.zero;
		ret.x = ve.x*ma.v0.x + ve.y*ma.v0.y + ve.z*ma.v0.z;
		ret.y = ve.x*ma.v1.x + ve.y*ma.v1.y + ve.z*ma.v1.z;
		ret.z = ve.x*ma.v2.x + ve.y*ma.v2.y + ve.z*ma.v2.z;
		ret = VecRound(ret);
		return ret;
	}
	
	public static Matrix3x3 operator*(Matrix3x3 ma1,Matrix3x3 ma2){
		Matrix3x3 ma_ret = new Matrix3x3();
		float x,y,z;
		x = ma1.v0.x*ma2.v0.x + ma1.v0.y*ma2.v1.x + ma1.v0.z*ma2.v2.x;
		y = ma1.v0.x*ma2.v0.y + ma1.v0.y*ma2.v1.y + ma1.v0.z*ma2.v2.y;
		z = ma1.v0.x*ma2.v0.z + ma1.v0.y*ma2.v1.z + ma1.v0.z*ma2.v2.z;
		ma_ret.v0 = VecRound(new Vector3(x,y,z));
		
		x = ma1.v1.x*ma2.v0.x + ma1.v1.y*ma2.v1.x + ma1.v1.z*ma2.v2.x;
		y = ma1.v1.x*ma2.v0.y + ma1.v1.y*ma2.v1.y + ma1.v1.z*ma2.v2.y;
		z = ma1.v1.x*ma2.v0.z + ma1.v1.y*ma2.v1.z + ma1.v1.z*ma2.v2.z;
		ma_ret.v1 = VecRound(new Vector3(x,y,z));
		
		x = ma1.v2.x*ma2.v0.x + ma1.v2.y*ma2.v1.x + ma1.v2.z*ma2.v2.x;
		y = ma1.v2.x*ma2.v0.y + ma1.v2.y*ma2.v1.y + ma1.v2.z*ma2.v2.y;
		z = ma1.v2.x*ma2.v0.z + ma1.v2.y*ma2.v1.z + ma1.v2.z*ma2.v2.z;
		ma_ret.v2 = VecRound(new Vector3(x,y,z));
		return ma_ret;
	}
	
	public static Vector3 VecRound(Vector3 vec){
		float x = (float)Math.Round (vec.x, 6);
		float y = (float)Math.Round (vec.y, 6);
		float z = (float)Math.Round (vec.z, 6);
		Vector3 ret = new Vector3(x, y, z);
		return ret;
	}
}

//表示区域
public class LineArea{
	public string name;
	public int Start;
	public int End;
	
	public LineArea(){
		name = "";
		Start = -1;
		End = -1;
	}
}

//调用信息
public class CALL_LBL{
	public int Index = -1;
	public string name = "";
	public int REP = 1;
	
	public CALL_LBL(){
		Index = -1;
		name = "";
		REP = 1;
	}
}

public class ToolInfo{
	public string Name;
	public float R_Value;
	public float L_Value;
	public float DR_Value;
	public float DL_Value;
}

public class PresetTableInfo{
	public Vector3 PosAxis;
	public Vector3 RotAxis;
}

//坐标系平移
public class CYCL7:ICycl
{
	#region Para
	public bool[] Axis_State;
	public bool[] IAxis_State;
	public float IX;
	public float IY;
	public float IZ;
	public float X;
	public float Y;
	public float Z;
	#endregion
	
	public CYCL7(){
		Axis_State = new bool[4]{false,false,false,false};
		IAxis_State = new bool[4]{false,false,false,false};
		IX = IY = IZ = 0;
		X = Y = Z = 0;
	}
	
	public bool Compile(List<List<string>> _List,ref int index,ref List<string> errorMes){
		float Cycl_Value1=0f;
		float Cycl_Value2=0f;
		try{
			Cycl_Value1 = float.Parse (_List[index][1]);
		}catch{
			string _error = "Line:"+(index+1)+" CYCL DEF 7指令错误！";
			if(CompileParas.DEBUG) _error += "% ErrorDBG-CYCLEDEF7-7 %";
			errorMes.Add (_error);
			return false;
		}
		
		if(!Mathf.Approximately ((float)Math.Round(Cycl_Value1,1),7.0f)){
			string _error = "Line:"+(index+1)+" CYCL DEF 7指令错误！";
			if(CompileParas.DEBUG) _error += "% ErrorDBG-CYCLEDEF7-8 %";
			errorMes.Add (_error);
			return false;
		}
		
		while(_List[index+1][0] == "CYCLDEF"){
			try{
				Cycl_Value2 = float.Parse (_List[index+1][1]);
			}catch{
				string _error = "Line:"+(index+2)+" CYCL DEF 7指令错误！";
				if(CompileParas.DEBUG) _error += "% ErrorDBG-CYCLEDEF7-9 %";
				errorMes.Add (_error);
				return false;
			}
			if(Mathf.FloorToInt (Cycl_Value2) == 7){
				if(Mathf.Approximately((float)Math.Round(Cycl_Value2-Cycl_Value1,1),0.1f)){
					string _Value = "";
					float Value_F = 0f;
					string VType = TypeGet (_List[index+1][2],ref _Value);
					if(_Value != ""){
						try{
							Value_F = float.Parse (_Value);
						}catch{
							string _error = "Line:"+(index+2)+" CYCL DEF 7指令错误！";
							if(CompileParas.DEBUG) _error += "% ErrorDBG-CYCLEDEF7-10 %";
							errorMes.Add (_error);
							return false;
						}
						ValueSet (VType,Value_F);
					}else{
						string _error = "Line:"+(index+2)+" CYCL DEF 7指令错误！";
						if(CompileParas.DEBUG) _error += "% ErrorDBG-CYCLEDEF7-11 %";
						errorMes.Add (_error);
						return false;
					}
				}else{
					if(Mathf.Approximately ((float)Math.Round(Cycl_Value2,1),7.0f)){
						return true;
					}else{
						string _error = "Line:"+(index+2)+" CYCL DEF 7指令错误！";
						if(CompileParas.DEBUG) _error += "% ErrorDBG-CYCLEDEF7-12 %";
						errorMes.Add (_error);
						return false;
					}
				}
				Cycl_Value1 = Cycl_Value2;
				index++;
			}else{
				break;
			}
			
		}
		
		return true;
	}
	
	void ValueSet(string str,float Value){
		switch(str){
		case "IX":
			IX = Value;
			IAxis_State[0] = true;
			IAxis_State[1] = true;
			break;
		case "IY":
			IY = Value;
			IAxis_State[0] = true;
			IAxis_State[2] = true;
			break;
		case "IZ":
			IZ = Value;
			IAxis_State[0] = true;
			IAxis_State[3] = true;
			break;
		case "X":
			X = Value;
			Axis_State[0] = true;
			Axis_State[1] = true;
			break;
		case "Y":
			Y = Value;
			Axis_State[0] = true;
			Axis_State[2] = true;
			break;
		case "Z":
			Z = Value;
			Axis_State[0] = true;
			Axis_State[3] = true;
			break;
		default:
			break;
		}
	}
	
	string TypeGet(string str,ref string Value){
		string RET = "";
		if(str.StartsWith ("I")){
			RET = "I"+str[1].ToString ();
			Value = str.Remove (0,2);
		}else{
			RET = str[0].ToString ();
			Value = str.Remove (0,1);
		}
		return RET;
	}
	
	public DataStore GetData(){
		DataStore RET = new DataStore();
		RET.motion_type = (int)MotionType.NO;
		RET.ImmediateAdd ((char)ImmediateMotionType.CYCL7);
		RET.XYZ_State = Axis_State;
		RET.IXYZ_State = IAxis_State;
		RET.X_value = X;
		RET.Y_value = Y;
		RET.Z_value = Z;
		RET.IX_value = IX;
		RET.IY_value = IY;
		RET.IZ_value = IZ;
		return RET;
	}
}

//3+2联动
public class CYCL19:ICycl
{
	public bool[] ABC_State = new bool[4]{false,false,false,false};
	public float A;
	public float B;
	public float C;
	public bool IsCancel = false;
	
	public bool Compile(List<List<string>> _List,ref int index,ref List<string> errorMes){
		float Value_1 = 0;
		float Value_2 = 0;
		try{
			Value_1 = float.Parse (_List[index][1]);
		}catch{
			string _error = "Line:"+(index+1)+" CYCL DEF 19指令错误！";
			if(CompileParas.DEBUG) _error += "% ErrorDBG-CYCLEDEF19-13 %";
			errorMes.Add (_error);
			return false;
		}
		if(!Mathf.Approximately ((float)Math.Round(Value_1,1),19.0f)){
			string _error = "Line:"+(index+1)+" CYCL DEF 19指令错误！";
			if(CompileParas.DEBUG) _error += "% ErrorDBG-CYCLEDEF19-14 %";
			errorMes.Add (_error);
			return false;
		}
		
		while(_List[index+1][0] == "CYCLDEF"){
			try{
				Value_2 = float.Parse (_List[index+1][1]);
			}catch{
				string _error = "Line:"+(index+1)+" CYCL DEF 19指令错误！";
				if(CompileParas.DEBUG) _error += "% ErrorDBG-CYCLEDEF19-15 %";
				errorMes.Add (_error);
				return false;
			}
			if(Mathf.FloorToInt (Value_2) == 19){
				if(Mathf.Approximately ((float)Math.Round(Value_2-Value_1,1), 0.1f)){
					if(_List[index+1].Count > 2){
						for(int i = 2;i < _List[index+1].Count;i++){
							string Str = _List[index+1][i].Remove (0,1);
							string _Type = _List[index+1][i][0].ToString ();
							float _Value = 0f;
							try{
								_Value = float.Parse (Str);
								ValueSet (_Type,_Value);
							}catch{
								string _error = "Line:"+(index+1)+" CYCL DEF 19指令错误！";
								if(CompileParas.DEBUG) _error += "% ErrorDBG-CYCLEDEF19-16 %";
								errorMes.Add (_error);
								return false;
							}
						}
						index++;
					}else{
						//if(ABC_State[0] && A == 0 && B == 0 && C == 0){
							IsCancel = true;
							index++;
							break;
						//}else{
						//	string _error = "Line:"+(index+1)+" CYCL DEF 19指令错误！";
						//	errorMes.Add (_error);
						//	return false;
						//}
					}
					Value_1 = Value_2;
				}else if(Mathf.Approximately ((float)Math.Round(Value_2,1),19.0f)){
					if(ABC_State[0] && A == 0 && B == 0 && C == 0){
						index ++;
						continue;
					}else{
						return true;
					}
				}
			}else{
				break;
			}
		}
		
		return true;
	}
	
	void ValueSet(string str,float Value){
		switch(str){
		case "A":
			A = Value;
			ABC_State[0] = true;
			ABC_State[1] = true;
			break;
		case "B":
			B = Value;
			ABC_State[0] = true;
			ABC_State[2] = true;
			break;
		case "C":
			C = Value;
			ABC_State[0] = true;
			ABC_State[3] = true;
			break;
		default:
			break;
		}
	}
	
	public DataStore GetData(){
		DataStore RET = new DataStore();
		RET.motion_type = (int)MotionType.CYCL19;
		RET.ABC_State = ABC_State;
		RET.A_value = A;
		RET.B_value = B;
		RET.C_value = C;
		RET.IsCancel = IsCancel;
		return RET;
	}
}

//公差
public class CYCL32:ICycl
{
	public float T_Value = 0;
	public int HSC_MODE = 0;
	public float TA_Value = 0;
	public bool IsCancel = false;
	
	public CYCL32(){
		T_Value = 0;
		HSC_MODE = 0;
	}
	
	public bool Compile(List<List<string>> _List,ref int index,ref List<string> errorMes){
		float Value_1 = 0;
		float Value_2 = 0;
		try{
			Value_1 = float.Parse (_List[index][1]);
		}catch{
			string _error = "Line:"+(index+1)+" CYCL DEF 32指令错误！";
			if(CompileParas.DEBUG) _error += "% ErrorDBG-CYCLEDEF32-17 %";
			errorMes.Add (_error);
			return false;
		}
		if(!Mathf.Approximately ((float)Math.Round(Value_1,1), 32.0f)){
			string _error = "Line:"+(index+1)+" CYCL DEF 32指令错误！";
			if(CompileParas.DEBUG) _error += "% ErrorDBG-CYCLEDEF32-18 %";
			errorMes.Add (_error);
			return false;
		}
		
		while(_List[index+1][0] == "CYCLDEF"){
			try{
				Value_2 = float.Parse (_List[index+1][1]);
			}catch{
				string _error = "Line:"+(index+2)+" CYCL DEF 32指令错误！";
				if(CompileParas.DEBUG) _error += "% ErrorDBG-CYCLEDEF32-19 %";
				errorMes.Add (_error);
				return false;
			}
			if(Mathf.FloorToInt (Value_2) == 32){
				if(Mathf.Approximately ((float)Math.Round(Value_2-Value_1,1),0.1f)){
					if(Mathf.Approximately ((float)Math.Round(Value_2,1),32.1f)){
						if(_List[index+1].Count > 2){
							string TempStr = _List[index+1][2].Trim ('T');
							try{
								T_Value = float.Parse (TempStr);
							}catch{
								string _error = "Line:"+index+1+" CYCL DEF 32指令错误！";
								if(CompileParas.DEBUG) _error += "% ErrorDBG-CYCLEDEF32-20 %";
								errorMes.Add (_error);
								return false;
							}
							index ++;
						}else{
							IsCancel = true;
							index++;
							break;
						}
//						index++;
					}else if(Mathf.Approximately ((float)Math.Round(Value_2,1),32.2f)){
						if(_List[index+1][2].StartsWith ("HSC-MODE:") && _List[index+1][3].StartsWith ("T")){
							string Temp_Str1 = _List[index+1][2].Remove (0,9);
							string Temp_Str2 = _List[index+1][3].Remove (0,2);
							try{
								HSC_MODE = int.Parse (Temp_Str1);
								TA_Value = float.Parse (Temp_Str2);
							}catch{
								string _error = "Line:"+(index+2)+" CYCL DEF 32指令错误！";
								if(CompileParas.DEBUG) _error += "% ErrorDBG-CYCLEDEF32-21 %";
								errorMes.Add (_error);
								return false;
							}
						}else{
							string _error = "Line:"+(index+2)+" CYCL DEF 32指令错误！";
							if(CompileParas.DEBUG) _error += "% ErrorDBG-CYCLEDEF32-22 %";
							errorMes.Add (_error);
							return false;
						}
						index++;
					}else{
						string _error = "Line:"+(index+2)+" CYCL DEF 32指令错误！";
						if(CompileParas.DEBUG) _error += "% ErrorDBG-CYCLEDEF32-23 %";
						errorMes.Add (_error);
						return false;
					}
				}else{
					if(Mathf.Approximately ((float)Math.Round(Value_2,1),32.0f)){
//						index++;
						break;
					}else{
						string _error = "Line:"+(index+2)+" CYCL DEF 32指令错误！";
						if(CompileParas.DEBUG) _error += "% ErrorDBG-CYCLEDEF32-24 %";
						errorMes.Add (_error);
						return false;
					}
				}
			}else{
				break;
			}
			
		}
		
		return true;
	}
	
	
	public DataStore GetData(){
		DataStore RET = new DataStore();
		RET.motion_type = (int)MotionType.CYCL32;
		RET.T_Value = T_Value;
		RET.TA_Value = TA_Value;
		RET.HSC_MODE = HSC_MODE;
		RET.IsCancel = IsCancel;
		return RET;
	}
}

//坐标系（类似于FanucG54-G59）
public class CYCL247:ICycl
{
	public int CoorIndex = 0;
	
	public bool Compile(List<List<string>> _List,ref int index,ref List<string> errorMes){
		int Value = 0;
		try{
			Value = int.Parse (_List[index][1]);
		}catch{
			string _error = "Line:"+(index+1)+" CYCL DEF 247指令错误！";
			if(CompileParas.DEBUG) _error += "% ErrorDBG-CYCLEDE247-25 %";
			errorMes.Add (_error);
			return false;
		}
		if(Value != 247){
			string _error = "Line:"+(index+1)+" CYCL DEF 247指令错误！";
			if(CompileParas.DEBUG) _error += "% ErrorDBG-CYCLEDE247-26 %";
			errorMes.Add (_error);
			return false;
		}
		Regex reg = new Regex(@"^Q\d+( )?=( )?\d+");
		if(reg.IsMatch (_List[index+1][0])){
			string[] Str_List = _List[index+1][0].Trim (' ').Split ('=');
			string Str1 = Str_List[0].Remove (0,1);
			string Str2 = Str_List[1];
			if(Str1.Equals ("339")){
				try{
					CoorIndex = int.Parse (Str2);
					if(CoorIndex < 0){
						string _error = "Line:"+(index+2)+" CYCL DEF 247指令错误！";
						if(CompileParas.DEBUG) _error += "% ErrorDBG-CYCLEDE247-27 %";
						errorMes.Add (_error);
						return false;
					}
				}catch{
					string _error = "Line:"+(index+2)+" CYCL DEF 247指令错误！";
					if(CompileParas.DEBUG) _error += "% ErrorDBG-CYCLEDE247-28 %";
					errorMes.Add (_error);
					return false;
				}
			}else{
				string _error = "Line:"+(index+2)+" CYCL DEF 247指令错误！";
				if(CompileParas.DEBUG) _error += "% ErrorDBG-CYCLEDE247-29 %";
				errorMes.Add (_error);
				return false;
			}
			++index;
		}else{
			string _error = "Line:"+(index+2)+" CYCL DEF 247指令错误！";
			if(CompileParas.DEBUG) _error += "% ErrorDBG-CYCLEDE247-30 %";
			errorMes.Add (_error);
			return false;
		}
		
		return true;
	}
	
	
	public DataStore GetData(){
		DataStore RET = new DataStore();
		RET.ImmediateAdd ((char)ImmediateMotionType.CYCL247);
		RET.IsSingleValue = true;
		RET.SingleValue = (float)CoorIndex;
		return RET;
	}
}

//倾斜加工面
public class PLANE:ICycl
{
	#region Para
	public PlaneType Type;
	public int MoveType = -1;
	public List<string> Str_List = new List<string>();
	public bool IsSingleValue = false;
	public float SingleValue = 0;
	
	//空间角|增量轴角
	public bool[] SP_State = new bool[4]{false,false,false,false};
	public float SPA;
	public float SPB;
	public float SPC;
	
	//投影
	public bool[] PR_State = new bool[4]{false,false,false,false};
	public float PROPR;
	public float PROMIN;
	public float PROROT;
	
	//欧拉角
	public bool[] EU_State = new bool[4]{false,false,false,false};
	public float EULPR;
	public float EULNU;
	public float EULROT;
	
	//矢量
	public bool[] BN_State = new bool[7]{false,false,false,false,false,false,false};
	public float BX;
	public float BY;
	public float BZ;
	public float NX;
	public float NY;
	public float NZ;
	
	//三点
	public float P1X;
	public float P1Y;
	public float P1Z;
	public float P2X;
	public float P2Y;
	public float P2Z;
	public float P3X;
	public float P3Y;
	public float P3Z;
	#endregion
	
	public bool Compile(List<List<string>> _List,ref int index,ref List<string> errorMes){
		if(TypeGet (_List[index][1],ref Type)){
			switch(Type){
			#region SPATIAL
			case PlaneType.SPATIAL:
				if(_List[index].Count < 6){
					string _error = "Line:"+(index+1)+" PLANE指令错误！";
					if(CompileParas.DEBUG) _error += "% ErrorDBG-PLANE-31 %";
					errorMes.Add (_error);
					return false;
				}
				for(int i = 2;i < _List[index].Count;i++){
					if(_List[index][i].StartsWith ("SPA")){
						string Str = _List[index][i].Remove (0,3);
						try{
							SPA = float.Parse (Str);
							SP_State[1] = true;
						}catch{
							string _error = "Line:"+(index+1)+" PLANE指令错误！";
							if(CompileParas.DEBUG) _error += "% ErrorDBG-PLANE-32 %";
							errorMes.Add (_error);
							return false;
						}
					}else if(_List[index][i].StartsWith ("SPB")){
						string Str = _List[index][i].Remove (0,3);
						try{
							SPB = float.Parse (Str);
							SP_State[2] = true;
						}catch{
							string _error = "Line:"+(index+1)+" PLANE指令错误！";
							if(CompileParas.DEBUG) _error += "% ErrorDBG-PLANE-33 %";
							errorMes.Add (_error);
							return false;
						}
					}else if(_List[index][i].StartsWith ("SPC")){
						string Str = _List[index][i].Remove (0,3);
						try{
							SPC = float.Parse (Str);
							SP_State[3] = true;
						}catch{
							string _error = "Line:"+(index+1)+" PLANE指令错误！";
							if(CompileParas.DEBUG) _error += "% ErrorDBG-PLANE-34 %";
							errorMes.Add (_error);
							return false;
						}
					}else{
						try{
							float Value = float.Parse (_List[index][i]);
							IsSingleValue = true;
							SingleValue = Value;
						}catch{
							Str_List.Add (_List[index][i]);
						}
					}
					if(!SP_State[1] || !SP_State[2] || !SP_State[3]){
						string _error = "Line:"+(index+1)+" PLANE指令错误！";
						if(CompileParas.DEBUG) _error += "% ErrorDBG-PLANE-35 %";
						errorMes.Add (_error);
						return false;
					}
				}
				break;
			#endregion
			#region PROJECTED
			case PlaneType.PROJECTED:
				if(_List[index].Count < 6){
					string _error = "Line:"+(index+1)+" PLANE指令错误！";
					if(CompileParas.DEBUG) _error += "% ErrorDBG-PLANE-36 %";
					errorMes.Add (_error);
					return false;
				}
				for(int i = 2;i < _List[index].Count;i++){
					if(_List[index][i].StartsWith ("PROPR")){
						string Str = _List[index][i].Remove (0,5);
						try{
							PROPR = float.Parse (Str);
							PR_State[1] = true;
						}catch{
							string _error = "Line:"+(index+1)+" PLANE指令错误！";
							if(CompileParas.DEBUG) _error += "% ErrorDBG-PLANE-37 %";
							errorMes.Add (_error);
							return false;
						}
					}else if(_List[index][i].StartsWith ("PROMIN")){
						string Str = _List[index][i].Remove (0,6);
						try{
							PROMIN = float.Parse (Str);
							PR_State[2] = true;
						}catch{
							string _error = "Line:"+(index+1)+" PLANE指令错误！";
							if(CompileParas.DEBUG) _error += "% ErrorDBG-PLANE-38 %";
							errorMes.Add (_error);
							return false;
						}
					}else if(_List[index][i].StartsWith ("PROROT")){
						string Str = _List[index][i].Remove (0,6);
						try{
							PROROT = float.Parse (Str);
							PR_State[3] = true;
						}catch{
							string _error = "Line:"+(index+1)+" PLANE指令错误！";
							if(CompileParas.DEBUG) _error += "% ErrorDBG-PLANE-39 %";
							errorMes.Add (_error);
							return false;
						}
					}else{
						try{
							float Value = float.Parse (_List[index][i]);
							IsSingleValue = true;
							SingleValue = Value;
						}catch{
							Str_List.Add (_List[index][i]);
						}
					}
					if(!PR_State[1] || !PR_State[2] || !PR_State[3]){
						string _error = "Line:"+(index+1)+" PLANE指令错误！";
						if(CompileParas.DEBUG) _error += "% ErrorDBG-PLANE-40 %";
						errorMes.Add (_error);
						return false;
					}
				}
				break;
			#endregion
			#region EULER
			case PlaneType.EULER:
				if(_List[index].Count < 6){
					string _error = "Line:"+(index+1)+" PLANE指令错误！";
					if(CompileParas.DEBUG) _error += "% ErrorDBG-PLANE-41 %";
					errorMes.Add (_error);
					return false;
				}
				for(int i = 2;i < _List[index].Count;i++){
					if(_List[index][i].StartsWith ("EULPR")){
						string Str = _List[index][i].Remove (0,5);
						try{
							EULPR = float.Parse (Str);
							EU_State[1] = true;
						}catch{
							string _error = "Line:"+(index+1)+" PLANE指令错误！";
							if(CompileParas.DEBUG) _error += "% ErrorDBG-PLANE-42 %";
							errorMes.Add (_error);
							return false;
						}
					}else if(_List[index][i].StartsWith ("EULNU")){
						string Str = _List[index][i].Remove (0,5);
						try{
							EULNU = float.Parse (Str);
							EU_State[2] = true;
						}catch{
							string _error = "Line:"+(index+1)+" PLANE指令错误！";
							if(CompileParas.DEBUG) _error += "% ErrorDBG-PLANE-43 %";
							errorMes.Add (_error);
							return false;
						}
					}else if(_List[index][i].StartsWith ("EULROT")){
						string Str = _List[index][i].Remove (0,6);
						try{
							EULROT = float.Parse (Str);
							EU_State[3] = true;
						}catch{
							string _error = "Line:"+(index+1)+" PLANE指令错误！";
							if(CompileParas.DEBUG) _error += "% ErrorDBG-PLANE-44 %";
							errorMes.Add (_error);
							return false;
						}
					}else{
						try{
							float Value = float.Parse (_List[index][i]);
							IsSingleValue = true;
							SingleValue = Value;
						}catch{
							Str_List.Add (_List[index][i]);
						}
					}
				}
				if(!EU_State[1] || !EU_State[2] || !EU_State[3]){
					string _error = "Line:"+(index+1)+" PLANE指令错误！";
					if(CompileParas.DEBUG) _error += "% ErrorDBG-PLANE-45 %";
					errorMes.Add (_error);
					return false;
				}
				break;
			#endregion
			#region VECTOR
			case PlaneType.VECTOR:
				if(_List[index].Count < 9){
					string _error = "Line:"+(index+1)+" PLANE指令错误！";
					if(CompileParas.DEBUG) _error += "% ErrorDBG-PLANE-46 %";
					errorMes.Add (_error);
					return false;
				}
				for(int i = 2;i < _List[index].Count;i++){
					if(_List[index][i].StartsWith ("BX")){
						string Str = _List[index][i].Remove (0,2);
						try{
							BX = float.Parse (Str);
							BN_State[1] = true;
						}catch{
							string _error = "Line:"+(index+1)+" PLANE指令错误！";
							if(CompileParas.DEBUG) _error += "% ErrorDBG-PLANE-47 %";
							errorMes.Add (_error);
							return false;
						}
					}else if(_List[index][i].StartsWith ("BY")){
						string Str = _List[index][i].Remove (0,2);
						try{
							BY = float.Parse (Str);
							BN_State[2] = true;
						}catch{
							string _error = "Line:"+(index+1)+" PLANE指令错误！";
							if(CompileParas.DEBUG) _error += "% ErrorDBG-PLANE-48 %";
							errorMes.Add (_error);
							return false;
						}
					}else if(_List[index][i].StartsWith ("BZ")){
						string Str = _List[index][i].Remove (0,2);
						try{
							BZ = float.Parse (Str);
							BN_State[3] = true;
						}catch{
							string _error = "Line:"+(index+1)+" PLANE指令错误！";
							if(CompileParas.DEBUG) _error += "% ErrorDBG-PLANE-49 %";
							errorMes.Add (_error);
							return false;
						}
					}else if(_List[index][i].StartsWith ("NX")){
						string Str = _List[index][i].Remove (0,2);
						try{
							NX = float.Parse (Str);
							BN_State[4] = true;
						}catch{
							string _error = "Line:"+(index+1)+" PLANE指令错误！";
							if(CompileParas.DEBUG) _error += "% ErrorDBG-PLANE-50 %";
							errorMes.Add (_error);
							return false;
						}
					}else if(_List[index][i].StartsWith ("NY")){
						string Str = _List[index][i].Remove (0,2);
						try{
							NY = float.Parse (Str);
							BN_State[5] = true;
						}catch{
							string _error = "Line:"+(index+1)+" PLANE指令错误！";
							if(CompileParas.DEBUG) _error += "% ErrorDBG-PLANE-51 %";
							errorMes.Add (_error);
							return false;
						}
					}else if(_List[index][i].StartsWith ("NZ")){
						string Str = _List[index][i].Remove (0,2);
						try{
							NZ = float.Parse (Str);
							BN_State[6] = true;
						}catch{
							string _error = "Line:"+(index+1)+" PLANE指令错误！";
							if(CompileParas.DEBUG) _error += "% ErrorDBG-PLANE-52 %";
							errorMes.Add (_error);
							return false;
						}
					}else{
						try{
							float Value = float.Parse (_List[index][i]);
							IsSingleValue = true;
							SingleValue = Value;
						}catch{
							Str_List.Add (_List[index][i]);
						}
					}
				}
				if(!BN_State[1] || !BN_State[2] || !BN_State[3] || !BN_State[4] || !BN_State[5] || !BN_State[6]){
					string _error = "Line:"+(index+1)+" PLANE指令错误！";
					if(CompileParas.DEBUG) _error += "% ErrorDBG-PLANE-53 %";
					errorMes.Add (_error);
					return false;
				}
				break;
			#endregion
			#region POINTS
			case PlaneType.POINTS:
				if(_List[index].Count < 12){
					string _error = "Line:"+(index+1)+" PLANE指令错误！";
					if(CompileParas.DEBUG) _error += "% ErrorDBG-PLANE-54 %";
					errorMes.Add (_error);
					return false;
				}
				for(int i = 2;i < _List[index].Count;i++){
					if(_List[index][i].StartsWith ("P1X")){
						string Str = _List[index][i].Remove (0,3);
						try{
							P1X = float.Parse (Str);
						}catch{
							string _error = "Line:"+(index+1)+" PLANE指令错误！";
							if(CompileParas.DEBUG) _error += "% ErrorDBG-PLANE-55 %";
							errorMes.Add (_error);
							return false;
						}
					}else if(_List[index][i].StartsWith ("P1Y")){
						string Str = _List[index][i].Remove (0,3);
						try{
							P1Y = float.Parse (Str);
						}catch{
							string _error = "Line:"+(index+1)+" PLANE指令错误！";
							if(CompileParas.DEBUG) _error += "% ErrorDBG-PLANE-56 %";
							errorMes.Add (_error);
							return false;
						}
					}else if(_List[index][i].StartsWith ("P1Z")){
						string Str = _List[index][i].Remove (0,3);
						try{
							P1Z = float.Parse (Str);
						}catch{
							string _error = "Line:"+(index+1)+" PLANE指令错误！";
							if(CompileParas.DEBUG) _error += "% ErrorDBG-PLANE-57 %";
							errorMes.Add (_error);
							return false;
						}
					}else if(_List[index][i].StartsWith ("P2X")){
						string Str = _List[index][i].Remove (0,3);
						try{
							P2X = float.Parse (Str);
						}catch{
							string _error = "Line:"+(index+1)+" PLANE指令错误！";
							if(CompileParas.DEBUG) _error += "% ErrorDBG-PLANE-58 %";
							errorMes.Add (_error);
							return false;
						}
					}else if(_List[index][i].StartsWith ("P2Y")){
						string Str = _List[index][i].Remove (0,3);
						try{
							P2Y = float.Parse (Str);
						}catch{
							string _error = "Line:"+(index+1)+" PLANE指令错误！";
							if(CompileParas.DEBUG) _error += "% ErrorDBG-PLANE-59 %";
							errorMes.Add (_error);
							return false;
						}
					}else if(_List[index][i].StartsWith ("P2Z")){
						string Str = _List[index][i].Remove (0,3);
						try{
							P2Z = float.Parse (Str);
						}catch{
							string _error = "Line:"+(index+1)+" PLANE指令错误！";
							if(CompileParas.DEBUG) _error += "% ErrorDBG-PLANE-60 %";
							errorMes.Add (_error);
							return false;
						}
					}else if(_List[index][i].StartsWith ("P3X")){
						string Str = _List[index][i].Remove (0,3);
						try{
							P3X = float.Parse (Str);
						}catch{
							string _error = "Line:"+(index+1)+" PLANE指令错误！";
							if(CompileParas.DEBUG) _error += "% ErrorDBG-PLANE-61 %";
							errorMes.Add (_error);
							return false;
						}
					}else if(_List[index][i].StartsWith ("P3Y")){
						string Str = _List[index][i].Remove (0,3);
						try{
							P3Y = float.Parse (Str);
						}catch{
							string _error = "Line:"+(index+1)+" PLANE指令错误！";
							if(CompileParas.DEBUG) _error += "% ErrorDBG-PLANE-62 %";
							errorMes.Add (_error);
							return false;
						}
					}else if(_List[index][i].StartsWith ("P3Z")){
						string Str = _List[index][i].Remove (0,3);
						try{
							P3Z = float.Parse (Str);
						}catch{
							string _error = "Line:"+(index+1)+" PLANE指令错误！";
							if(CompileParas.DEBUG) _error += "% ErrorDBG-PLANE-63 %";
							errorMes.Add (_error);
							return false;
						}
					}else{
						try{
							float Value = float.Parse (_List[index][i]);
							IsSingleValue = true;
							SingleValue = Value;
						}catch{
							Str_List.Add (_List[index][i]);
						}
					}
				}
				break;
			#endregion
			#region REL_SPA
			case PlaneType.RELATIVE:
				for(int i = 2;i < _List[index].Count;i++){
					if(_List[index][i].StartsWith ("SPA")){
						string Str = _List[index][i].Remove (0,3);
						try{
							SPA = float.Parse (Str);
							SP_State[0] = true;
							SP_State[1] = true;
						}catch{
							string _error = "Line:"+(index+1)+" PLANE指令错误！";
							if(CompileParas.DEBUG) _error += "% ErrorDBG-PLANE-64 %";
							errorMes.Add (_error);
							return false;
						}
					}else if(_List[index][i].StartsWith ("SPB")){
						string Str = _List[index][i].Remove (0,3);
						try{
							SPB = float.Parse (Str);
							SP_State[0] = true;
							SP_State[2] = true;
						}catch{
							string _error = "Line:"+(index+1)+" PLANE指令错误！";
							if(CompileParas.DEBUG) _error += "% ErrorDBG-PLANE-65 %";
							errorMes.Add (_error);
							return false;
						}
					}else if(_List[index][i].StartsWith ("SPC")){
						string Str = _List[index][i].Remove (0,3);
						try{
							SPC = float.Parse (Str);
							SP_State[0] = true;
							SP_State[3] = true;
						}catch{
							string _error = "Line:"+(index+1)+" PLANE指令错误！";
							if(CompileParas.DEBUG) _error += "% ErrorDBG-PLANE-66 %";
							errorMes.Add (_error);
							return false;
						}
					}else{
						try{
							float Value = float.Parse (_List[index][i]);
							IsSingleValue = true;
							SingleValue = Value;
						}catch{
							Str_List.Add (_List[index][i]);
						}
					}
				}
				break;
			#endregion
			#region RESET
			case PlaneType.RESET:
				if(_List[index].Count < 3){
					string _error = "Line:"+(index+1)+" PLANE指令错误！";
					if(CompileParas.DEBUG) _error += "% ErrorDBG-PLANE-67 %";
					errorMes.Add (_error);
					return false;
				}
				for(int i = 2;i < _List[index].Count;i++){
					try{
						float Value = float.Parse (_List[index][i]);
						IsSingleValue = true;
						SingleValue = Value;
					}catch{
						Str_List.Add (_List[index][i]);
					}
				}
				break;
			#endregion
			default:
				break;
			}
			if(!moveTypeGet (Str_List,ref MoveType)){
				string _error = "Line:"+(index+1)+" PLANE指令错误！";
				if(CompileParas.DEBUG) _error += "% ErrorDBG-PLANE-68 %";
				errorMes.Add (_error);
				return false;
			}
		}else{
			string _error = "Line:"+(index+1)+" PLANE指令错误！";
			if(CompileParas.DEBUG) _error += "% ErrorDBG-PLANE-69 %";
			errorMes.Add (_error);
			return false;
		}
		return true;
	}
	
	//SPATIAL=0,PROJECTED,EULER,VECTOR,POINTS,REL_SPA
	bool TypeGet(string Str,ref PlaneType Type_RET){
		switch(Str){
		case "SPATIAL":
			Type_RET = PlaneType.SPATIAL;
			break;
		case "PROJECTED":
			Type_RET = PlaneType.PROJECTED;
			break;
		case "EULER":
			Type_RET = PlaneType.EULER;
			break;
		case "VECTOR":
			Type_RET = PlaneType.VECTOR;
			break;
		case "POINTS":
			Type_RET = PlaneType.POINTS;
			break;
		case "RELATIVE":
			Type_RET = PlaneType.RELATIVE;
			break;
		case "RESET":
			Type_RET = PlaneType.RESET;
			break;
		default:
			return false;
		}
		return true;
	}
	
	bool moveTypeGet(List<string> _List,ref int MType){
		if(_List.IndexOf ("STAY") != -1){
			MType = (int)PlaneMoveType.STAY;
		}else if(_List.IndexOf ("TURN") != -1){
			MType = (int)PlaneMoveType.TURN;
		}else if(_List.IndexOf ("MOVE") != -1){
			MType = (int)PlaneMoveType.MOVE;
			if(!IsSingleValue)
				return false;
		}
		
		if(MType == -1)
			return false;
		else
			return true;
	}
	
	public DataStore GetData(){
		DataStore RET = new DataStore();
		RET.motion_type = (int)MotionType.PLANE;
		RET._PLANE = this;
		RET.IsSingleValue = IsSingleValue;
		RET.SingleValue = SingleValue;
		RET.Str_List = Str_List;
		return RET;
	}
}