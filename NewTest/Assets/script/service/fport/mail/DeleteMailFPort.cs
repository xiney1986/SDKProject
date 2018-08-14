using UnityEngine;
using System.Collections;

/**
 * 删除邮件接口
 * @author 汤琦
 * */
public class DeleteMailFPort : BaseFPort
{

	private CallBack callback;
	
	public void access (string uid, CallBack callback)
	{   
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.MAIL_DELETE);
		message.addValue ("uid", new ErlString (uid));
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
