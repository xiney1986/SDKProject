using UnityEngine;
using System.Collections;

/**
 * 公告获取接口
 * @author 汤琦
 * */
public class NoticeGetFPort : BaseFPort {

	private CallBack callback;
	
	public void access (string sids,CallBack callback)
	{   
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.NOTICE_GET);
		message.addValue ("affiches", new ErlString (sids));
		access (message);
	}
	public override void read (ErlKVMessage message)
	{
		ErlArray array = message.getValue ("msg") as ErlArray; 
		NoticeManagerment.Instance.updateAllNotice(array);
		callback();
	}
}
