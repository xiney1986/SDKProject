using UnityEngine;
using System.Collections;

/// <summary>
/// 签到通讯端口
/// </summary>
public class ShareDrawFport : BaseFPort {

	/** 回调函数 */
    private CallBack<int> callbacks;

    public void ShareDraw(int sid, CallBack<int> callback)
	{
		this.callbacks = callback;
        ErlKVMessage message = new ErlKVMessage(FrontPort.SHAREDRAW);
		message.addValue ("sid", new ErlInt (sid));//活动sid
		access (message);
	}
	public override void read (ErlKVMessage message) {
		base.read (message);
        ErlType type = message.getValue("msg") as ErlType;
        if (type is ErlInt) {
            ErlInt erl = type as ErlInt;
            //int index = 0;
            int checkdPoint = StringKit.toInt(erl.getValueString());
            if (callbacks != null)
                callbacks(checkdPoint);
            else MaskWindow.UnlockUI();
        } else {
            MessageWindow.ShowAlert((message.getValue("msg") as ErlType).getValueString());
            MaskWindow.UnlockUI();
            if (callbacks != null)
                callbacks = null;
        }
	}

}
