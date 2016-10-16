using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;

//"^((BEGIN|END){1}( )+PGM( )+[\w-]+){1}( )+(MM|INCH)"
//"^BLK( )+FORM( )+0.[12]{1}"
//"^*( )+-( )+"
//"^CYCL( )+DEF"
//"^PLANE{1}( )+(RESET|SPITIAL|PROJECTED|EULER|VECTOR|POINTS|RELATIVE|AXIAL){1}"
//"^(LBL){1}( )+(\d+|(""\w+""))$"
//"^FN 0:Q[0-9]+( )*=( )*[+-]?\d+.?\d*"
//"(^((I?[ABCXYZ])|([RMFS])|(IPA)){1}[+-]?Q?\d+.?\d*)|(^Q[0-9]+( )*=( )*[+-]?\d+.?\d*)"
//"^CALL( )+LBL( )+\d+"
//"^TOOL{1}( )+(CALL|DEF)( )+\d+"
//"^(L|CC|C|CR|FMAX){1} "
//"^(CHF|RND){1}\d+.?\d*"

//该格式化只能用于编译，不能用于显示
public static class Load{
	
	#region REGX
	public static Regex reg_1  = new Regex(@"^((BEGIN|END){1}( )+PGM( )+[\w-]+){1}( )+(MM|INCH)");
	public static Regex reg_2  = new Regex(@"^BLK( )+FORM( )+0.[12]{1}");
	public static Regex reg_3  = new Regex(@"^\*( )+-( )+");
	public static Regex reg_4  = new Regex(@"^CYCL( )+DEF");
	public static Regex reg_5  = new Regex(@"^PLANE{1}( )+(RESET|SPITIAL|PROJECTED|EULER|VECTOR|POINTS|RELATIVE|AXIAL){1}");
	public static Regex reg_6  = new Regex(@"^(LBL){1}( )+(\d+|(""\w+""))$");
	public static Regex reg_7  = new Regex(@"^CALL( )+LBL( )+\d+");
	public static Regex reg_8  = new Regex(@"^FN 0:Q[0-9]+( )*=( )*[+-]?\d*.?\d*");
	public static Regex reg_9  = new Regex(@"(^((I?[ABCXYZ])|([RMFS])|(IPA)){1}[+-]?Q?\d*.?\d*)|(^Q[0-9]+( )*=( )*[+-]?\d*.?\d*)");
	public static Regex reg_10 = new Regex(@"^TOOL{1}( )+(CALL|DEF)( )+\d+");
	public static Regex reg_11 = new Regex(@"^(L|CC|C|CR|FMAX){1}( )*");
	public static Regex reg_12 = new Regex(@"^(CHF|RND){1}\d*.?\d*");
	#endregion
	
	public static List<List<string>> CodeLoad(string name,ref ModalState Modal_State, ref List<string> Error_Mes,ref bool Success){
		Success = true;
		Modal_State = new ModalState();
		int LineNumber = 0;
		LineArea LArea = new LineArea();
		List<List<string>> RET = new List<List<string>>();
		string path = CompileParas_Unity.sCodeFilePath + name;
		FileStream _File = new FileStream(path,FileMode.Open,FileAccess.Read);
		StreamReader SR = new StreamReader(_File);
		string Temp_str = SR.ReadLine ();
		bool IsStarted = false;
		bool IsEnd = false;
		while(Temp_str != null){
			Temp_str = Temp_str.ToUpper ();
			List<string> Temp_SList = new List<string>();
			int Space_Index = Temp_str.IndexOf (';');
			if(Space_Index != -1)
				Temp_str = Temp_str.Remove (Space_Index);
			char[] _ch = new char[1]{' '};
			string[] Temp_Array = Temp_str.Split (_ch,StringSplitOptions.RemoveEmptyEntries);
//			Debug.Log (Temp_str);
			while(Temp_str.StartsWith (" ")){
				Temp_str = Temp_str.Remove (0,1);
			}
			if(Temp_Array.Length > 0){
				//去除前面序号
				int start_Array = 0;
				try{
					int.Parse (Temp_Array[0]);
					int _index = Temp_str.IndexOf (Temp_Array[0]);
					Temp_str = Temp_str.Remove (_index,Temp_Array[0].Length+1);
					start_Array = 1;
				}catch{}
				if(start_Array < Temp_Array.Length){
					//进行匹配区分
					#region 起始或终结段
					if(reg_1.IsMatch (Temp_str)){//起始或终结段
						if(Temp_Array[start_Array] == "BEGIN"){
							IsStarted = true;
							Modal_State.PGMName = Temp_Array[start_Array+2];
						}else{
							IsEnd = true;
						}
						Temp_SList.Add (Temp_Array[start_Array]);
						Temp_SList.Add (Temp_Array[start_Array+2]);
						if(Temp_Array[start_Array+3] != "MM" && Temp_Array[start_Array+3] != "INCH"){
							string error = "Line:"+LineNumber+ " 程序单位定义错误";
							if(CompileParas.DEBUG) error += "% ErrorDBG-Load-141 %";
							Error_Mes.Add (error);
							Success = false;
						}else{
							Temp_SList.Add (Temp_Array[start_Array+3]);
						}
					#endregion
					#region 毛坯定义段
					}else if(reg_2.IsMatch (Temp_str)){//毛坯定义段
						Temp_SList.Add ("BLKFORM");
						Temp_SList.Add (Temp_Array[start_Array+2]);
						for(int i = 1;i<=3;i++){
							Temp_SList.Add (Temp_Array[Temp_Array.Length-i]);
						}
					#endregion
					#region *开头
					}else if(reg_3.IsMatch (Temp_str)){//*开头 暂不知用途
						//不处理
					#endregion
					#region 循环段
					}else if(reg_4.IsMatch (Temp_str)){//循环段
						Temp_SList.Add ("CYCLDEF");
						Temp_SList.Add (Temp_Array[start_Array+2]);
						for(int i = start_Array+3;i<Temp_Array.Length;i++)
							Temp_SList.Add (Temp_Array[i]);
					#endregion
					#region 加工平面定义
					}else if(reg_5.IsMatch (Temp_str)){//加工平面定义
						Temp_SList.Add ("PLANE");
						Temp_SList.Add (Temp_Array[start_Array+1]);
						for(int i = start_Array+2;i<Temp_Array.Length;i++)
							Temp_SList.Add (Temp_Array[i]);
					#endregion
					#region LBL
					}else if(reg_6.IsMatch (Temp_str)){//LBL
						Temp_SList.Add ("LBL");
						if(Temp_Array[start_Array+1].ToString ().StartsWith ("\"")){
//							Temp_SList.Add("1");
							if(LArea.Start != -1){
								string error = "Line:"+LineNumber+ " LBL定义错误";
								if(CompileParas.DEBUG) error += "% ErrorDBG-Load-140 %";
								Error_Mes.Add (error);
								Success = false;
							}else{
								LArea.name = Temp_Array[start_Array+1];
								LArea.Start = LineNumber;
							}
						}else{
//							Temp_SList.Add("0");
							int Num=-1;
							try{
								Num = int.Parse (Temp_Array[start_Array+1]);
								if(Num == 0){
									if(LArea.Start != -1){
										LArea.End = LineNumber;
										try{
											Modal_State.LBL_LineNumber.Add (LArea.name,LArea);
										}catch{
											string error = "Line:"+LineNumber+ " LBL定义错误";
											if(CompileParas.DEBUG) error += "% ErrorDBG-Load-139 %";
											Error_Mes.Add (error);
											Success = false;
										}
										LArea = new LineArea();
									}else{
										string error = "Line:"+LineNumber+ " LBL定义错误";
										if(CompileParas.DEBUG) error += "% ErrorDBG-Load-138 %";
										Error_Mes.Add (error);
										Success = false;
									}
								}else{
									if(LArea.Start != -1){
										string error = "Line:"+LineNumber+ " LBL定义错误";
										if(CompileParas.DEBUG) error += "% ErrorDBG-Load-137 %";
										Error_Mes.Add (error);
										Success = false;
									}else{
										LArea.name = Temp_Array[start_Array+1];
										LArea.Start = LineNumber;
									}
								}
							}catch{
								string error = "Line:"+LineNumber+ " LBL定义错误";
								if(CompileParas.DEBUG) error += "% ErrorDBG-Load-136 %";
								Error_Mes.Add (error);
								Success = false;
							}
						}
						Temp_SList.Add (Temp_Array[start_Array+1]);
					#endregion
					#region CALL
					}else if(reg_7.IsMatch (Temp_str)){//CALL
						CALL_LBL LBL_info = new CALL_LBL();
						LBL_info.name = Temp_Array[start_Array+2];
						LBL_info.Index = LineNumber;
						int _Index = 3;
						Regex Temp_Reg = new Regex(@"^REP\d+");
						while((start_Array+_Index)<Temp_Array.Length){
							if(Temp_Reg.IsMatch(Temp_Array[start_Array+_Index])){
								string Temp_Str = Temp_Array[start_Array+_Index].ToString ().Trim ('R','E','P');
								try{
									LBL_info.REP = int.Parse (Temp_Str);
								}catch{
									string error = "Line:"+LineNumber+ " CALL LBL定义错误！";
									if(CompileParas.DEBUG) error += "% ErrorDBG-Load-135 %";
									Error_Mes.Add (error);
									Success = false;
								}
								break;
							}
							++_Index;
						}
						Modal_State.CallLBL_info.Add (LBL_info);
//						for(int i = start_Array;i<Temp_Array.Length;i++)
//							Temp_SList.Add (Temp_Array[i]);
					#endregion
					#region FN0
					}else if(reg_8.IsMatch (Temp_str)){//FN0
						Temp_SList.Add ("FN0");
						int Qstart = Temp_str.IndexOf ("Q");
						if(Qstart != -1){
							string QStr = Temp_str.Substring (Qstart);
							QStr = QStr.Trim (' ');
							string[] QArray = QStr.Split ('=');
							Temp_SList.Add (QArray[0]);
							Temp_SList.Add (QArray[1]);
						}else{
							string error = "Line:"+LineNumber+ " FN0定义错误！";
							if(CompileParas.DEBUG) error += "% ErrorDBG-Load-134 %";
							Error_Mes.Add (error);
							Success = false;
						}
					#endregion
					#region XYZABC等
					}else if(reg_9.IsMatch (Temp_str)){//XYZABC等
						for(int i = start_Array;i<Temp_Array.Length;i++)
							Temp_SList.Add (Temp_Array[i]);
					#endregion
					#region TOOL CALL/DEF
					}else if(reg_10.IsMatch (Temp_str)){//TOOL CALL/DEF
						for(int i = start_Array;i<Temp_Array.Length;i++)
							Temp_SList.Add (Temp_Array[i]);
					#endregion
					#region L C CC CR
					}else if(reg_11.IsMatch (Temp_str)){//L C CC CR
						for(int i = start_Array;i<Temp_Array.Length;i++)
							Temp_SList.Add (Temp_Array[i]);
					#endregion
					#region CHF RND
					}else if(reg_12.IsMatch (Temp_str)){//CHF RND
						for(int i = start_Array;i<Temp_Array.Length;i++)
							Temp_SList.Add (Temp_Array[i]);
					#endregion
					}else{
						string error = "Line:"+LineNumber+ " 未定义解析代码！";
						if(CompileParas.DEBUG) error += "% ErrorDBG-Load-133 %";
						Error_Mes.Add (error);
						Success = false;
					}
				}
			}
			if(IsStarted){
				RET.Add (Temp_SList);
				Modal_State.Line_Index.Add (LineNumber+1);
				++LineNumber;
			}
			Temp_str = SR.ReadLine ();
		}
		
		//_File.Close ();
		SR.Close ();
		_File.Close ();
		if(!IsStarted){
			string error = "程序开头不完整！";
			Debug.Log (error);
			Error_Mes.Add (error);
			Success = false;
		}
		if(!IsEnd){
			string error = "程序结尾不完整！";
			Debug.Log (error);
			Error_Mes.Add (error);
			Success = false;
		}

		if(Success && Modal_State.LBL_LineNumber.Count != 0){
			foreach(string str in Modal_State.LBL_LineNumber.Keys){
				int Start = Modal_State.LBL_LineNumber[str].Start;
				int End = Modal_State.LBL_LineNumber[str].End;
				List<List<string>> LNew = new List<List<string>>();
				for(int i = Start;i <= End;i++){
					if(i != Start && i != End)
						LNew.Add (RET[i]);
					RET[i] = new List<string>();
				}
				Modal_State.LBL_List.Add (Modal_State.LBL_LineNumber[str].name,LNew);
			}
		}
		if(Success && Modal_State.CallLBL_info.Count != 0){
			for(int i = Modal_State.CallLBL_info.Count-1;i>=0;--i){
				RET.RemoveAt (Modal_State.CallLBL_info[i].Index);
				Modal_State.Line_Index.RemoveAt (Modal_State.CallLBL_info[i].Index);
				if(Modal_State.LBL_List.ContainsKey (Modal_State.CallLBL_info[i].name)){
					List<List<string>> TempList = Modal_State.LBL_List[Modal_State.CallLBL_info[i].name];
					for(int j = 0;j < Modal_State.CallLBL_info[i].REP;j++){
						for(int k = TempList.Count-1;k>=0;k--){
							RET.Insert (Modal_State.CallLBL_info[i].Index,TempList[k]);
							Modal_State.Line_Index.Insert (Modal_State.CallLBL_info[i].Index,Modal_State.CallLBL_info[i].Index+1);
						}
					}
				}else{
					string error = "Line:"+Modal_State.CallLBL_info[i].Index+ " CALL LBL定义错误！";
					if(CompileParas.DEBUG) error += "% ErrorDBG-LBL-132 %";
					Error_Mes.Add (error);
					Success = false;
				}
			}
		}
//		Debug.Log (RET.Count);
//		Debug.Log (CompileParas.Modal_State.Line_Index.Count);
//		for(int i = 0;i < CompileParas.Modal_State.Line_Index.Count;i++){
//			Debug.Log (CompileParas.Modal_State.Line_Index[i]);
//		}
//		for(int i = 0;i < RET.Count;i++){
//			string sss= "";
//			for(int j = 0;j < RET[i].Count;j++){
//				sss+=RET[i][j]+"~";
//			}
//			Debug.Log (sss);
//		}
//		foreach(string str in CompileParas.Modal_State.LBL_LineNumber.Keys){
//			Debug.Log (CompileParas.Modal_State.LBL_LineNumber[str].name);
//			Debug.Log (CompileParas.Modal_State.LBL_LineNumber[str].Start);
//			Debug.Log (CompileParas.Modal_State.LBL_LineNumber[str].End);
//		}
		return RET;
	}

	public static Dictionary<string,ToolInfo> ToolLoad(){
		Dictionary<string,ToolInfo> RET = new Dictionary<string, ToolInfo>();
		FileStream _File = new FileStream(CompileParas_Unity.sToolFilePath,FileMode.Open,FileAccess.Read);
		StreamReader SR = new StreamReader(_File);
		string Temp_Str = SR.ReadLine();
		Temp_Str = SR.ReadLine();
		while(Temp_Str != null){
			ToolInfo TInfo = new ToolInfo();
			string[] Temp_Array = Temp_Str.Split (' ');
			TInfo.Name = Temp_Array[0];
			float.TryParse (Temp_Array[1],out TInfo.R_Value);
			float.TryParse (Temp_Array[2],out TInfo.L_Value);
			float.TryParse (Temp_Array[3],out TInfo.DR_Value);
			float.TryParse (Temp_Array[4],out TInfo.DL_Value);
			if(RET.ContainsKey (TInfo.Name)){
				RET[TInfo.Name] = TInfo;
			}else{
				RET.Add (TInfo.Name,TInfo);
			}
			Temp_Str = SR.ReadLine ();
		}
		
		SR.Close ();
		return RET;
	}
	
	public static void ToolWrite(Dictionary<string,ToolInfo> _List){
		FileStream _File = new FileStream(CompileParas_Unity.sToolFilePath,FileMode.OpenOrCreate,FileAccess.Write);
		StreamWriter SW = new StreamWriter(_File);
		string Top = "No R L DR DL";
		SW.WriteLine (Top);
		foreach(string i in _List.Keys){
			string Str = _List[i].Name + " " + _List[i].R_Value.ToString () + " " + _List[i].L_Value.ToString () + 
			" " + _List[i].DR_Value.ToString () + " " + _List[i].DL_Value.ToString ();
			SW.WriteLine (Str);
		}
		SW.Close ();
	}

	public static List<PresetTableInfo> PresetTableLoad(){
		List<PresetTableInfo> RET = new List<PresetTableInfo>();
		FileStream _File = new FileStream(CompileParas_Unity.sPresetTablePath,FileMode.Open,FileAccess.Read);
		StreamReader SR = new StreamReader(_File);
		string Str = SR.ReadLine ();
		Str = SR.ReadLine ();//两次读取是为了过滤表头
		while(Str != null){
			PresetTableInfo PTable = new PresetTableInfo();
			string[] _Array = Str.Split (' ');
			float.TryParse (_Array[1],out PTable.PosAxis.x);
			float.TryParse (_Array[2],out PTable.PosAxis.y);
			float.TryParse (_Array[3],out PTable.PosAxis.z);
			float.TryParse (_Array[4],out PTable.RotAxis.x);
			float.TryParse (_Array[5],out PTable.RotAxis.y);
			float.TryParse (_Array[6],out PTable.RotAxis.z);
			RET.Add (PTable);
			Str = SR.ReadLine ();
		}
		
		SR.Close ();
		return RET;
	}
	
	public static void PresetTableWrite(List<PresetTableInfo> _List){
		FileStream _File = new FileStream(CompileParas_Unity.sPresetTablePath,FileMode.OpenOrCreate,FileAccess.Write);
		StreamWriter SW = new StreamWriter(_File);
		string Top = "No X Y Z A B C";
		SW.WriteLine (Top);
		for(int i = 0;i < _List.Count;i++){
			string Str = i.ToString ()+" "+_List[i].PosAxis.x+" "+_List[i].PosAxis.y+" "+_List[i].PosAxis.z+" "+_List[i].RotAxis.x+" "+_List[i].RotAxis.y+" "+_List[i].RotAxis.z;
			SW.WriteLine (Str);
		}
		SW.Close ();
	}
	
	public static Vector3 CooZeroLoad(){
		Vector3 RET = Vector3.zero;
		FileStream _File = new FileStream(CompileParas_Unity.sCooZeroPath,FileMode.Open,FileAccess.Read);
		StreamReader SR = new StreamReader(_File);
		string Str = SR.ReadLine ();
		Str = SR.ReadLine ();
		if(Str != null){
			string[] StrArray = Str.Split (' ');
			float.TryParse (StrArray[0],out RET.x);
			float.TryParse (StrArray[1],out RET.y);
			float.TryParse (StrArray[2],out RET.z);
		}
		SR.Close ();
		return RET;
	}
	
	public static void CooZeroWrite(Vector3 Vec){
		FileStream _File = new FileStream(CompileParas_Unity.sCooZeroPath,FileMode.OpenOrCreate,FileAccess.Write);
		StreamWriter SW = new StreamWriter(_File);
		string Top = "X Y Z";
		SW.WriteLine (Top);
		string Str = Vec.x.ToString ()+" "+Vec.y.ToString ()+" "+Vec.z.ToString ();
		SW.WriteLine (Str);
		SW.Close ();
	}

	public static void CooLimitLoad(ref Vector3 Min,ref Vector3 Max){
		FileStream _File = new FileStream(CompileParas_Unity.sCooLimitPath,FileMode.Open,FileAccess.Read);
		StreamReader SR = new StreamReader(_File);
		string Str = SR.ReadLine ();
		Str = SR.ReadLine ();
		if(Str != null){
			string[] StrArray = Str.Split (' ');
			float.TryParse (StrArray[0],out Min.x);
			float.TryParse (StrArray[1],out Min.y);
			float.TryParse (StrArray[2],out Min.z);
//			Min *= 1000;
			Str = SR.ReadLine ();
		}
		if(Str != null){
			string[] StrArray = Str.Split (' ');
			float.TryParse (StrArray[0],out Max.x);
			float.TryParse (StrArray[1],out Max.y);
			float.TryParse (StrArray[2],out Max.z);
//			Max *= 1000;
		}
		SR.Close ();
	}

	public static void sysinfoLoad(ref float fmax,ref float rotspeed){
		FileStream _File = new FileStream(CompileParas_Unity.sSysinfoPath,FileMode.Open,FileAccess.Read);
		StreamReader SR = new StreamReader(_File);
		string Str = SR.ReadLine ();
		if(Str != null){
			string[] _Array = Str.Split (' ');
			float.TryParse (_Array[0],out fmax);
			Str = SR.ReadLine ();
		}
		if(Str != null){
			string[] _Array = Str.Split (' ');
			float.TryParse (_Array[0],out rotspeed);
		}
		SR.Close ();
	}
}
