using UnityEngine;
using System.Collections;

public class NoticeActiveGetFPort : BaseFPort
{

	private CallBack callback;
	
	public void access (int activeSid, CallBack callback)
	{   
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.NOTICE_ACTIVE_INFO);
		message.addValue ("sid", new ErlString (activeSid.ToString ()));
		access (message);
	}

	public void access (int[] activeSid2, CallBack callback)
	{   
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.NOTICE_ACTIVE_INFO);
		message.addValue ("sid", new ErlString (sid2String (activeSid2)));
		access (message);
	}

	private string sid2String (int[] sid)
	{
		string str = string.Empty;
		for (int i = 0; i < sid.Length; i++) {
			if (i + 1 == sid.Length)
				str += sid [i];
			else
				str += sid [i] + ",";
		}
		return str;
	}

	public override void read (ErlKVMessage message)
	{
		//[[40,1,[[[40000002,1]],[[[consume_rmb,6701]],[[[role_times,40000002],1]]]]],...]
		ErlArray arr = message.getValue ("msg") as ErlArray;
		ErlArray temp;
		int sid;
		for (int i=0; i<arr.Value.Length; i++) {
			temp = arr.Value [i] as ErlArray;
			NoticeActiveManagerment.Instance.parseInfo (temp);
		}
	 	RechargeManagerment.Instance.updateRecharge ();
		if (callback != null) {
			callback ();
			callback = null;
		}
	}
}
