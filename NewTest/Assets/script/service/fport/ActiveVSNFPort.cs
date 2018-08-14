using UnityEngine;
using System.Collections;

/**
 * 获取版本号端口
 */
public class ActiveVSNFPort : BaseFPort {

	private CallBack callback;

	public void getActiveVSN(CallBack _callback)
	{
		this.callback = _callback;
		ErlKVMessage msg = new ErlKVMessage(FrontPort.ACTIVE_VSN);
		access(msg);
	}

	public override void read (ErlKVMessage message) {
		base.read (message);
		ErlType msg = message.getValue("msg") as ErlType;
		string str = msg.getValueString ();
		GameManager.CONFIG_VERSION = str == "none" ? "-1" : str;
//		Debug.LogWarning ("=========>>>>>CONFIG_VERSION ==" + str);
		if (callback != null) {
			callback ();
			callback = null;
		}
	}
}
