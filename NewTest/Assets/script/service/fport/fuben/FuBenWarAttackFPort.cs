using UnityEngine;
using System.Collections;

/// <summary>
/// 讨伐挑战Boss端口
/// </summary>
public class FuBenWarAttackFPort : BaseFPort {

	private int intoSid;
	private CallBack callback;

	/// <summary>
	/// 进入副本--默认难度为普通=1
	/// <param name="sid">关卡sid</param>
	/// <param name="callback"></param>
	/// </summary>
	public void attackBoss (int sid, CallBack callback) {
		this.intoSid = sid;
		this.callback = callback;
		
		ErlKVMessage message = new ErlKVMessage (FrontPort.FUBEN_WAR_ATTACK);
		message.addValue ("fbid", new ErlInt (sid));//fuben sid
		access (message);
	}

	public override void read (ErlKVMessage message) {
		string str = (message.getValue ("msg") as ErlAtom).Value;

		if (str == "ok") {
			FuBenManagerment.Instance.isWarAttackBoss = true;
			if (callback != null) {
				callback ();
			}
		}
		else {
			MessageWindow.ShowAlert (str);
		}
	}
}
