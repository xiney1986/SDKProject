using UnityEngine;
using System.Collections;

/// <summary>
/// 成长奖励通讯端口
/// </summary>
public class GrowupAwardFPort : BaseFPort {

	/** 回调函数 */
	private CallBack callback;
	/** 转化经验回调函数 */
	private CallBack<int> convertCallback;
	/// <summary>
	/// 领取成长计划奖励
	/// </summary>
	/// <param name="grade">领取的等级</param>
	/// <param name="callback">Callback.</param>
	public void GetGrowupAwardAccess (string grade, CallBack callback) {
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.GrowupAward_GETAWARD);
		message.addValue ("grade", new ErlString (grade));
		access (message);
	}
	/// <summary>
	/// 投资成长计划
	/// </summary>
	/// <param name="lockinfo">投资金额</param>
	/// <param name="callback">callback</param>
	public void Invest (string value, CallBack callback) {
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.GrowupAward_INVEST);
		message.addValue ("value", new ErlString (value));
		access (message);
	}
	/// <summary>
	/// 获取成长计划信息
	/// </summary>
	/// <param name="callback">Callback.</param>
	public void GetGrowupInfo (CallBack callback) {
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.GrowupAward_GETINFO);
		access (message);
	}
	/// <summary>
	/// 回调读取通讯
	/// </summary>
	/// <param name="message">Message.</param>
	public override void read (ErlKVMessage message) {
		ErlArray msgArr = message.getValue ("msg") as ErlArray;
		int index = 0;
		string msgInfo = msgArr.Value [index].getValueString ();
		if (msgInfo == "get_award") {
			string result = msgArr.Value [++index].getValueString ();
			GrowupAwardMangement.Instance.getAwardStatas = result;
			if (callback != null) {
				callback ();
				callback = null;
			}
		}
		else if (msgInfo == "invest") {
			string result = msgArr.Value [++index].getValueString ();
			GrowupAwardMangement.Instance.investStatas = result;
			if (callback != null) {
				callback ();
				callback = null;
			}
		}
		else if (msgInfo == "get_info") {
			int prestoreMoney = StringKit.toInt (msgArr.Value [++index].getValueString ());
			GrowupAwardMangement.Instance.prestoreMoney = prestoreMoney;
			ErlArray tmpArr = msgArr.Value [++index] as ErlArray;
			if (tmpArr != null) {
				GrowupAwardMangement.Instance.tookLevel = new string[tmpArr.Value.Length];
				for (int i = 0; i<tmpArr.Value.Length; i++) {
					GrowupAwardMangement.Instance.tookLevel [i] = tmpArr.Value [i].getValueString ();
				}
			}
			GrowupAwardMangement.Instance.timeID = StringKit.toInt (msgArr.Value [++index].getValueString ());
			if (callback != null) {
				callback ();
				callback = null;
			}
		}
	}
}
