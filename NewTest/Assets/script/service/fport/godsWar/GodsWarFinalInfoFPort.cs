using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/**
 * 获取淘汰赛信息
 * @author gc
 * */
public class GodsWarFinalInfoFPort : BaseFPort
{
	CallBack callback;
  
	public void access (int type,int index,CallBack callback)
	{   
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.GODSWAR_GETIFINALBASE_INFO);
		message.addValue("big_id",new ErlInt(type));
		message.addValue("yu_ming",new ErlInt(index));
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{ 
		ErlType erl = message.getValue("msg") as ErlType;
		if(erl is ErlArray)
		{
			GodsWarFinalUserInfo user;
			List<GodsWarFinalUserInfo> infos = new List<GodsWarFinalUserInfo>();
			ErlArray erlarry = erl as ErlArray;
			for(int i=0;i<erlarry.Value.Length;i++)
			{
				user = new GodsWarFinalUserInfo();
				user.bytesRead(erlarry.Value[i] as ErlArray);
				infos.Add(user);
			}
			if(infos!=null)
				GodsWarManagerment.Instance.finalInfoList = infos;
			if(callback!=null)
				callback();
		}
		else
		{
			MessageWindow.ShowAlert(erl.getValueString());
			if(callback!=null)
				callback=null;
		}

	}

}
