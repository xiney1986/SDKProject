using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/// <summary>
/// 解锁替补接口
/// </summary>
public class TeamEmtpyFPort : BaseFPort {
	private CallBack callback;
	int ind;
	public TeamEmtpyFPort(){
	}
	public void access (int index,CallBack callback)
	{   
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.TEAM_OPEN);
		int[] indexx=TeamUnluckManager.Instance.getindex();
		index=indexx[index-1];
		ind=index;
		message.addValue ("local", new ErlInt (index));
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

		ErlType type = message.getValue ("msg") as ErlType;
		string state =type.getValueString();
		if(state=="ok"){
			if(GuideManager.Instance.openIndex==null){
				GuideManager.Instance.openIndex=new List<int>();
			}
			GuideManager.Instance.openIndex.Add(ind);
			UiManager.Instance.openDialogWindow<MessageLineWindow>((winnn)=>{
				winnn.Initialize(LanguageConfigManager.Instance.getLanguage("teamEdit_err04l2"));
			});
			return true;
		}else{
			UiManager.Instance.openDialogWindow<MessageLineWindow>((winnn)=>{
				winnn.Initialize(LanguageConfigManager.Instance.getLanguage("teamEdit_err04l3"));
			});
		}
		return false;
	}

}
