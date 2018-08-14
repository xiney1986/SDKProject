using UnityEngine;
using System.Collections;

/// <summary>
/// 签到通讯端口
/// </summary>
public class SignInFport : BaseFPort {

	/** 回调函数 */
    private CallBack callback;
	private int sid;
    private int type;

    public void signIn(int sid, int type, CallBack callback)
	{
		this.sid = sid;
        this.type = type;
		this.callback = callback;
        ErlKVMessage message = new ErlKVMessage(FrontPort.SIGN_IN);
		message.addValue ("sid", new ErlInt (sid));//日期sid
        message.addValue("type", new ErlInt(type));//签到类型（补签or正常签到）
		access (message);
	}
	public override void read (ErlKVMessage message) {
		base.read (message);
		string erlArray = (message.getValue ("msg") as ErlType).getValueString();
        if (erlArray == null) return;
        if (erlArray == "ok") {
            if (callback != null) {
                callback();
                callback = null;
            }
        } else {
            UiManager.Instance.openDialogWindow<MessageWindow>((win) => {
                win.initWindow(1, LanguageConfigManager.Instance.getLanguage("s0093"),null, erlArray,null);
            });
        }
	}

}
