using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 限时卡片,装备显示奖励端口
/// </summary>
public class LuckyXianshiShowAwardFPort : BaseFPort {

	private CallBack callback;
	/** 活动ID */
	private int noticeSid;

	/** 显示奖励领取情况 */
	public void showAwardAccess (int noticeSid,CallBack callback) { 
		this.noticeSid=noticeSid;
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.NOTICE_LUCKY_XIANSHI_AWARD_SHOW);
		message.addValue ("sid", new ErlInt (noticeSid));
		access (message);

	}
	/** 读取数据 */
	public override void read (ErlKVMessage message) {
		ErlType erlType = message.getValue ("msg") as ErlType;
		List<int> receivedAward = new List<int> ();
		if (erlType is ErlArray) {
			ErlArray arr= erlType as ErlArray;
			if (arr.Value.Length>0) {
				for (int m = 0,count=arr.Value.Length; m < count; m++) {
					int awardSid = StringKit.toInt(arr.Value [m].getValueString ());
					receivedAward.Add(awardSid);
				}
			}
		}
		LucklyActivityAwardConfigManager.Instance.updateAwardDateByIntegral(noticeSid,receivedAward);
		if (callback != null) {
			callback();
		}
	}
}
