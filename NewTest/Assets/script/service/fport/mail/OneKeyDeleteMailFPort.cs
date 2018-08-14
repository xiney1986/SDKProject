using UnityEngine;
using System.Collections;

/**
 * 一键删除邮件接口
 * @author 汤琦
 * */
public class OneKeyDeleteMailFPort : BaseFPort
{

	private CallBack callback;
	
	public void access (CallBack callback)
	{   
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.MAIL_ONEKEYDELETE);
		access (message);
	}

	public override void read (ErlKVMessage message)
	{
		string str = (message.getValue ("msg") as ErlAtom).Value; 
		if (str == "ok") {
			callback ();
		} else {
			MessageWindow.ShowAlert (str);
			if (callback != null)
				callback = null;
		}
	}
}
