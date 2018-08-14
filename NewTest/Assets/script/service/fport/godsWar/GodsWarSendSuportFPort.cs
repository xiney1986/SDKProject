using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/**
 * 竞猜接口
 * @author gc
 * */
public class GodsWarSendSuportFPort : BaseFPort
{
	CallBack callback;
  
	public void access (string serverName,string uid,int type,int index,int localID,int _type,CallBack callback)
	{   
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.GODSWAR_SENDSUPORT);
		message.addValue("server_name",new ErlString(serverName));
		message.addValue("role_uid",new ErlString(uid));
		message.addValue("big_id",new ErlInt(type));
		message.addValue("yu_ming",new ErlInt(index));
		message.addValue("local",new ErlInt(localID));
		message.addValue("flag",new ErlInt(_type));
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{ 
		ErlType erl = message.getValue("msg") as ErlType;

		if(erl.getValueString()=="ok")
		{
			if(callback!=null)
				callback();
		}
		else
		{
            if (erl.getValueString() == "time_limit") MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("godswar_1415151"));
			else MessageWindow.ShowAlert(erl.getValueString());
			if(callback!=null)
				callback=null;
		}

	}

}
