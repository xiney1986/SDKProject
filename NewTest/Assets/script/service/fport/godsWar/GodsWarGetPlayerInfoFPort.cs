using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/**
 * 获取对战玩家信息接口
 * @author gc
 * */
public class GodsWarGetPlayerInfoFPort : BaseFPort
{
	CallBack callback;
  
	public void access (int type,int index,int localID,CallBack callback)
	{
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.GODSWAR_GETSUPORT_INFO);
		message.addValue("big_id",new ErlInt(type));
		message.addValue("yu_ming",new ErlInt(index));
		message.addValue("local",new ErlInt(localID));
		access (message);
	}
	public void access (string serverName,string uid,int type,int index,CallBack callback)
	{
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.GODSWAR_GETSUPORT_INFO);
		message.addValue("server_name",new ErlString(serverName));
		message.addValue("role_uid",new ErlString(uid));
		message.addValue("big_id",new ErlInt(type));
		message.addValue("yu_ming",new ErlInt(index));
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{ 
		ErlType erl = message.getValue("msg") as ErlType;

		if(erl is ErlArray)
		{
			ErlArray arr = erl as ErlArray;
			if(arr.Value.Length==2)
			{
				GodsWarFinalUserInfo user;
				List<GodsWarFinalUserInfo> infos = new List<GodsWarFinalUserInfo>();
				for (int i = 0; i < arr.Value.Length; i++) {
					ErlArray tmp = arr.Value[i] as ErlArray;
					user = new GodsWarFinalUserInfo();
					user.bytesReadThree(tmp);
					infos.Add(user);
				}
				if(infos.Count>0)
					GodsWarManagerment.Instance.pvpGodsWarFinalInfo = infos;
			}
			else {
				GodsWarFinalUserInfo baseUser = new GodsWarFinalUserInfo();
				baseUser.bytesReadFour(arr);
				GodsWarManagerment.Instance.singlePlayer = baseUser;
			}
			if(callback!=null)
				callback();
			MaskWindow.UnlockUI();
		}
		else
		{
			MessageWindow.ShowAlert(erl.getValueString());
			if(callback!=null)
				callback=null;
			MaskWindow.UnlockUI();
		}

	}

}
