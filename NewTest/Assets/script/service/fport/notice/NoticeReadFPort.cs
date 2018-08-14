using UnityEngine;
using System.Collections;

/**
 * 公告读取接口
 * @author 汤琦
 * */
public class NoticeReadFPort : BaseFPort {

	private CallBack callback;
	
	public void access (int sid,CallBack callback)
	{   
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.NOTICE_READ);
		message.addValue ("affiche", new ErlInt (sid));
		access (message);
	}
	public override void read (ErlKVMessage message)
	{
		string str = (message.getValue ("msg") as ErlType).getValueString(); 
		if(str == "ok")
		{
			callback();
		}
	}
}
