using UnityEngine;
using System.Collections;

/**
 * 附加属性接口
 * @author 汤琦
 * */
public class AttributeFPort : BaseFPort
{

	private CallBack<string> callback;
	
	public AttributeFPort ()
	{
		
	}
	//副卡foodUID,格式为,用逗号分隔
	public void access (string mainUID, string foodUID, CallBack<string> back)
	{ 
		this.callback = back;
		ErlKVMessage message = new ErlKVMessage (FrontPort.ATTRIBUTE);
		message.addValue ("mainuid", new ErlString (mainUID));
		message.addValue ("fooduid", new ErlString (foodUID));
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{
		ErlType uid = message.getValue ("main_uid") as ErlType;
		ErlType msg = message.getValue ("msg") as ErlType;
		if (uid != null) {
			callback (uid.getValueString ());
		} else {
			MessageWindow.ShowAlert (msg.getValueString ());
			if (callback != null)
				callback = null;
		}
//				if (message.getValue ("main_uid") == null) {
//					MonoBase.print (" addon error!!");
//				}
//                else
//                {
//						string uid = (message.getValue ("main_uid") as ErlType).getValueString ();
//						callback (uid);
//				}
	}
}
