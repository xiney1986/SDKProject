using UnityEngine;
using System.Collections;

/// <summary>
/// 获得GM修改活动信息
/// </summary>
public class NoticeGetGMActiveFPort:BaseFPort
{

	private CallBack callback;
	
	public void access (CallBack callback)
	{
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.NOTICE_GM_ACTIVE_INFO);
		access (message);
	}
	/** 读取数据 */
	public override void read (ErlKVMessage message)
	{
		ErlArray arr = message.getValue ("msg") as ErlArray;
		if (arr != null)
			NoticeActiveManagerment.Instance.parseGMDetailInfo (arr);
		if (callback != null) {
			callback ();
			callback = null;
		}
	}
}