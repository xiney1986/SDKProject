using UnityEngine;
using System.Collections;

/// <summary>
/// 获得GM修改活动信息
/// </summary>
public class NoticeGetActiveAwardFPort:BaseFPort
{

	private CallBack<bool> callback;
	
	public void access (int sid, CallBack<bool> callback)
	{
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.NOTICE_GET_ACTIVE_AWARD);
		message.addValue ("sid", new ErlInt (sid));
		access (message);
	}
	/** 读取数据 */
	public override void read (ErlKVMessage message)
	{
        if (!(message.getValue("msg") is ErlType))
        {
            UiManager.Instance.openDialogWindow<MessageLineWindow>((win) =>
            {
                win.Initialize("Error" + message.getValue("msg").ToString());
            });
            return;
        }
		string msg = (message.getValue ("msg") as ErlType).getValueString ();
		if (callback != null) {
			callback (msg == "ok");
			callback = null;
		}
	}
}