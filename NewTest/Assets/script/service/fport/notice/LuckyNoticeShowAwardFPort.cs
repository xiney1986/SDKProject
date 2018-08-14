using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 限时卡片,装备显示奖励端口
/// </summary>
public class LuckyNoticeShowAwardFPort : BaseFPort {

	private CallBack callback;
	/** 活动ID */
	private int noticeSid;

	/** 显示奖励领取情况 */
	public void showAwardAccess (int noticeSid,CallBack callback) { 
		this.noticeSid=noticeSid;
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.NOTICE_SHOW_LUCKY_CARD_AWARD);
		message.addValue ("noticeSid", new ErlInt (noticeSid));
		access (message);

	}
	/** 读取数据 */
	public override void read (ErlKVMessage message) {
		ErlType erlType = message.getValue ("msg") as ErlType;
		List<string> receivedAward = new List<string> ();
		if (erlType is ErlArray) {
			ErlArray arr= erlType as ErlArray;
			if (arr.Value.Length>0) {
				for (int m = 0,count=arr.Value.Length; m < count; m++) {
					string awardSid = arr.Value [m].getValueString ();
					receivedAward.Add(awardSid);
				}
			}
		}
		LucklyActivityAwardConfigManager.Instance.updateAwardDate(noticeSid,receivedAward);
		if (callback != null) {
			callback();
		}
	}
}
