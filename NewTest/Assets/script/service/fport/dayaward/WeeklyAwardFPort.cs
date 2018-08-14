using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/// <summary>
/// 天天送拿数据端口
/// </summary>
public class WeeklyAwardFPort : BaseFPort {
	private CallBack callback;
	public WeeklyAwardFPort(){
	}
	public void access (CallBack callback)
	{   
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.WEEKLY_AWARD_LIST);
//		Debug.LogWarning("WeeklyAwardFPort begin");
		access (message);
	}

	public override void read (ErlKVMessage message)
	{
		    parseMsg (message);
//		Debug.LogWarning("WeeklyAwardFPort over");
			callback ();
	}
	//解析ErlKVMessgae
	public bool parseMsg (ErlKVMessage message)
	{
		ErlArray arr1;
		ErlType type = message.getValue ("msg") as ErlType;
		if (type is ErlArray) {
			ErlArray stateArray=type as ErlArray;
			string state =stateArray.Value[0].getValueString();
			if(state=="ok"){
				TotalLoginManagerment.Instance.WeeklyState=true;
				TotalLoginManagerment.Instance.updateWeeklyAwarrdData(stateArray.Value[1] as ErlArray);
				return true;
			}else if(state=="close"){
				TotalLoginManagerment.Instance.WeeklyState=false;
				return true;
			}return false;
		} else {
			MessageWindow.ShowAlert (type.getValueString ());
			if (callback != null)
				callback = null;
		}
		return false;
	}

}
