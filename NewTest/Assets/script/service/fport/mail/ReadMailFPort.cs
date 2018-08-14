using UnityEngine;
using System.Collections;

/**
 * 读取邮件接口
 * @author 汤琦
 * */
public class ReadMailFPort : BaseFPort
{

	private CallBack callback;
	
	public void access (string uid, CallBack callback)
	{   
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.MAIL_READ);
		message.addValue ("uid", new ErlString (uid));
		access (message);
	}

	public override void read (ErlKVMessage message)
	{
		string str = (message.getValue ("msg") as ErlType).getValueString (); 
		if (str == "ok") {
			callback ();
		} else {
			MessageWindow.ShowAlert (str);
			if (callback != null)
				callback = null;
		}
	}
}
