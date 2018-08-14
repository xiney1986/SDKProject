using UnityEngine;
using System.Collections;
/// <summary>
/// 神秘商店购买端口
/// </summary>
public class ActiveFuBenBuyFPort : BaseFPort {
	private CallBack<int,int> callback;
	private int sid = 0;
	private int num = 0;
    public ActiveFuBenBuyFPort()
    {

	}
	public void buyGoods (int sid,int index, CallBack<int,int> callback)
	{  
		this.sid = sid;
		this.callback = callback;
        this.num = index;
        ErlKVMessage message = new ErlKVMessage(FrontPort.ACTIVE_BUY_LW);
        message.addValue("crusade_type", new ErlInt(sid));
        message.addValue("number", new ErlInt(index));
		access (message);
	}
	public override void read (ErlKVMessage message)
	{
		string str = (message.getValue ("msg") as ErlType).getValueString ();
		if (str == "ok") {
			if (callback != null)
				callback (sid, num);
		} else {
			UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage("s0207"));
		}
		callback = null;
	}

}
