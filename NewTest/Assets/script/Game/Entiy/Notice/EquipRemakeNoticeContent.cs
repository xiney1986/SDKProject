using System;

/// <summary>
/// 装备改造升级公告配置
/// </summary>
public class EquipRemakeNoticeContent:NoticeContent
{
	/** 源装备sid */
	int sourceEquipSid;
	/** 升级的目标装备sid */
	int targetEquipSid;
	/** 显示公告玩家等级 */
	int showUserLevel;
	/** 显示公告VIP等级 */
	int showVipLevel;
	/** 可用公告VIP等级 */
	int usedVipLevel;
	/** 找回装备功能VIP等级 */
	int usedGetBackVipLevel;
	/** 消耗rmb价格 */
	int consumRmbValue;
	/** 抵扣道具sid */
	int exchangePropSid;
	/** 抵扣道具比例(1个道具抵扣多少rmb) */
	int exchangePropRate;
	
	public EquipRemakeNoticeContent () {
	}
	/** 解析 */
	public override void parse (string str)
	{
		base.parse (str);
		string[] strs = str.Split (',');
		checkLength (strs.Length, 9);
		sourceEquipSid=StringKit.toInt (strs [0]);
		targetEquipSid=StringKit.toInt (strs [1]);
		showUserLevel=StringKit.toInt(strs [2]);
		showVipLevel=StringKit.toInt (strs [3]);
		usedVipLevel=StringKit.toInt (strs [4]);
		usedGetBackVipLevel=StringKit.toInt (strs [5]);
		consumRmbValue=StringKit.toInt (strs [6]);
		exchangePropSid=StringKit.toInt (strs [7]);
		exchangePropRate=StringKit.toInt (strs [8]);
	}
	//长度检测
	public void checkLength (int len, int indexmax) { 
		if (len != indexmax)
			throw new Exception (this.GetType () + " config EquipRemakeNoticeContent error,len=" + len + " indexmax=" + indexmax);
	}

	/// <summary>
	/// 源装备sid
	/// </summary>
	public int getSourceEquipSid() {
		return this.sourceEquipSid;
	}
	/// <summary>
	/// 升级的目标装备sid
	/// </summary>
	public int getTargetEquipSid() {
		return this.targetEquipSid;
	}
	/// <summary>
	/// 显示公告VIP等级
	/// </summary>
	public int getShowVipLevel() {
		return this.showVipLevel;
	}
	/// <summary>
	/// 显示公告玩家等级
	/// </summary>
	public int getShowUserLevel() {
		return this.showUserLevel;
	}
	/// <summary>
	/// 可用公告VIP等级
	/// </summary>
	public int getUsedVipLevel() {
		return this.usedVipLevel;
	}
	/// <summary>
	/// 找回装备功能VIP等级
	/// </summary>
	public int getUsedGetBackVipLevel() {
		return this.usedGetBackVipLevel;
	}
	/// <summary>
	/// 消耗rmb价格
	/// </summary>
	public int getConsumRmbValue() {
		return this.consumRmbValue;
	}
	/// <summary>
	/// 抵扣道具sid
	/// </summary>
	public int getExchangePropSid() {
		return this.exchangePropSid;
	}
	/// <summary>
	/// 抵扣道具比例(1个道具抵扣多少rmb)
	/// </summary>
	public int getExchangePropRate() {
		return this.exchangePropRate;
	}
}
