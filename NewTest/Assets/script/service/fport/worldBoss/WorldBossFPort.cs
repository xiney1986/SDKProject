using UnityEngine;
using System.Collections;

/// <summary>
/// 世界首领通讯端口
/// </summary>
public class WorldBossFPort : BaseFPort {

	/** 回调函数 */
	private CallBack callback;

	/// <summary>
	/// 获取世界首领信息
	/// </summary>
	public void getWorldBossInfo (CallBack callback) {
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.WorldBoss_GETINFO);
		access (message);
	}

	/// <summary>
	/// 挑战世界首领
	/// </summary>
	public void attackWorldBoss (CallBack callback) {
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.WorldBoss_ATTACK);
		access (message);
	}

	/// <summary>
	/// 刷新挑战冷却时间
	/// </summary>
	public void resCDTime (CallBack callback) {
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.WorldBoss_RESCD);
		access (message);
	}

	public override void read (ErlKVMessage message) {
		base.read (message);
		ErlArray erlArray = message.getValue ("msg") as ErlArray;
		int index = 0;
		string returnType = erlArray.Value [index++].getValueString ();
		if (returnType == "getinfo") {
			doGetInfo (erlArray, index);
		}
		else if (returnType == "attack") {
			doAttack (erlArray, index);
		}
		else if (returnType == "resCD") {
			doResCD (erlArray, index);
		}

		if (callback != null) {
			callback ();
			callback = null;
		}
	}

	/// <summary>
	/// 获取世界首领信息处理
	/// </summary>
	/// <param name="erlArray">Erl array.</param>
	/// <param name="index">Index.</param>
	private void doGetInfo (ErlArray erlArray, int index) {
		string msgInfo = erlArray.Value [index++].getValueString ();
		if (msgInfo == "ok") {

		}
		else {
			MessageWindow.ShowAlert (msgInfo);
		}
	}

	/// <summary>
	/// 挑战世界首领信息处理
	/// </summary>
	/// <param name="erlArray">Erl array.</param>
	/// <param name="index">Index.</param>
	private void doAttack (ErlArray erlArray, int index) {
		string msgInfo = erlArray.Value [index++].getValueString ();
		if (msgInfo == "ok") {
			WorldBossManagerment.Instance.isAttackBoss = true;
		}
		else {
			MessageWindow.ShowAlert (msgInfo);
		}
	}

	/// <summary>
	/// 刷新挑战冷却时间
	/// </summary>
	/// <param name="erlArray">Erl array.</param>
	/// <param name="index">Index.</param>
	private void doResCD (ErlArray erlArray, int index) {
		string msgInfo = erlArray.Value [index++].getValueString ();
		if (msgInfo == "ok") {

		}
		else {
			MessageWindow.ShowAlert (msgInfo);
		}
	}
}
