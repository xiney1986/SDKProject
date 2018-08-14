using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/// <summary>
/// 替补席位开放信息
/// </summary>
public class TeamEmtpyInfoFPort  : BaseFPort {
	private CallBack<List<int>> callback;
	private List<int> ids;
	public TeamEmtpyInfoFPort (){
	}
	public void access (CallBack<List<int>> callback)
	{   
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.TEAM_GET);
//		Debug.LogWarning("WeeklyAwardFPort begin");
		access (message);
	}

	public override void read (ErlKVMessage message)
	{
		    parseMsg (message);
			callback(ids);
	}
	//解析ErlKVMessgae
	public bool parseMsg (ErlKVMessage message)
	{
		ErlArray arr1;
		ErlType type = message.getValue ("msg") as ErlType;
		if (type is ErlArray) {
			ErlArray stateArray=type as ErlArray;
			ids=new List<int>();
			for(int i=0;i<stateArray.Value.Length;i++){
				int index=StringKit.toInt((stateArray.Value[i] as ErlByte).getValueString());
				ids.Add(getindex(index));
			}
			return true;
		} else {
			MessageWindow.ShowAlert (type.getValueString ());
			if (callback != null)
				callback = null;
		}
		return false;
	}
	private int getindex(int s){
		int[] indexx=TeamUnluckManager.Instance.getindex();
		for(int i=0;i<indexx.Length;i++){
			if(indexx[i]==s)return i+1;
		}
		return 3;
	}

}
