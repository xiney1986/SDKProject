using System;
using UnityEngine;

/**
 * 使用道具端口
 * @author zhoujie
 * */
public class UsePropPort:BaseFPort
{
	private CallBack<int> callback;
	
	public UsePropPort ()
	{
	}
	/**
	* 使用道具 access通讯
	* 
	* @param sid	道具sid
	* @param index	后台索引
	* @param num	使用数量
	* 
	* @return
	* */
	public void access (int sid, int num, CallBack<int> callback)
	{ 
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.USE_PROP);  
		message.addValue ("sid", new ErlInt (sid));//sid
		message.addValue ("num", new ErlInt (num));//数量
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{
		ErlType arr = message.getValue ("msg") as ErlType;
		if (arr.getValueString () == "ok") {
			ErlType num = message.getValue ("num") as ErlType;
			callback (StringKit.toInt (num.getValueString ()));
		} else if (arr.getValueString () == "limit_num") {
			callback (-1);
		} else {
			MessageWindow.ShowAlert (arr.getValueString ());
            if (callback != null)
                callback(1);
				callback = null;
		}
	}

}

