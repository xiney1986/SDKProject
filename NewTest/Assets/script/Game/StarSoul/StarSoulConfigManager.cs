using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// 星魂信息相关数据配置管理
/// </summary>
public class StarSoulConfigManager :ConfigManager
{
	/* const */
	/** 猎魂类型 */
	public const int HUNT_MONEY_TYPE = 0, // 普通猎魂
							HUNT_RMB_TYPE=1; // 钻石猎魂

	/* static fields */
	private static StarSoulConfigManager instance;

	/* static methods */
	public static StarSoulConfigManager Instance {
		get {
			if (instance == null)
				instance = new StarSoulConfigManager();
			return instance;
		}
	}

	/* fields */
	/** 猎魂玩家等级 */
	public int huntUserLevel;
	/** 显示钻石猎魂最低vip等级 */
	public int showHuntRmbVipLevel;
	/** 钻石猎魂最低vip等级 */
	public int huntRmbVipLevel;
	/** 单次猎魂游戏币费用 */
	public int[] huntConsumeMoney;
	/** 单次猎魂钻石费用 */
	public int huntConsumeRmb;
	/** 普通一键猎取vip等级 */
	public int oneKeyVipLevel;
	/** 钻石一键猎取vip等级 */
	public int rmbOneKeyVipLevel;
	/**星魂凹槽开放等级配置 */
	public int[] grooveOpneLevel;

	/* methods */
	public StarSoulConfigManager() {
		base.readConfig(ConfigGlobal.CONFIG_STARSOUL_OPERATOR);
	}
	/** 解析 */
	public override void parseConfig(string str) {
		string[] strs=str.Split('|');
		checkLength (strs.Length,9);
		// str[0] 配置文件说明
		// str[1]
		huntUserLevel=StringKit.toInt(strs[1]);
		// str[2]
		showHuntRmbVipLevel=StringKit.toInt(strs[2]);
		// str[3]
		huntRmbVipLevel=StringKit.toInt(strs[3]);
		// str[4] 
		parseHuntConsumeMoney (strs[4]);
		// str[5] 
		huntConsumeRmb=StringKit.toInt(strs[5]);
		// str[6]
		oneKeyVipLevel=StringKit.toInt(strs[6]);
		// str[7]
		rmbOneKeyVipLevel=StringKit.toInt(strs[7]);
		// str[8]
		parseGrooveOpneLevel (strs[8]);
	}
	public void checkLength (int len, int max) { 
		if (len != max)
			throw new Exception (this.GetType () + " ConfigGlobal.CONFIG_STARSOUL_OPERATOR error len=" + len + " max=" + max);
	}
	/** 解析普通裂魂消耗游戏币 */
	private void parseHuntConsumeMoney (string str)
	{
		//表示空
		if (str == Convert.ToString(0))
			return;
		string[] strArr = str.Split ('#');  
		huntConsumeMoney = new int[strArr.Length]; 
		for (int i = 0; i<strArr.Length; i++) {
			huntConsumeMoney [i] = StringKit.toInt(strArr [i]);
		}
	}
	/** 解析装备星魂等级限制 */
	private void parseGrooveOpneLevel (string str) {
		//表示空
		if (str == Convert.ToString(0))
			return;
		string[] strArr = str.Split ('#');  
		grooveOpneLevel = new int[strArr.Length]; 
		for (int i = 0; i<strArr.Length; i++) {
			grooveOpneLevel [i] = StringKit.toInt(strArr [i]);
		}
	}
	/// <summary>
	/// 校验玩家等级是否足够
	/// </summary>
	public bool isEnoughByHuntLevel() {
		return UserManager.Instance.self.getUserLevel ()>=huntUserLevel;
	}
	/// <summary>
	/// 校验游戏币是否足够
	/// </summary>
	/// <param name="huntCount">裂魂次数</param>
	/// <param name="huntStarSoulQuality">裂魂品质</param>
	public bool isEnoughByHuntMoney(int huntCount,int huntStarSoulQuality) {
		int consumeValue = huntConsumeMoney[huntStarSoulQuality] * huntCount;
		return UserManager.Instance.self.getMoney () >= consumeValue;
	}
	/// <summary>
	/// 校验钻石是否足够
	/// </summary>
	/// <param name="huntCount">裂魂次数<</param>
	public bool isEnoughByHuntRMB(int huntCount) {
		int consumeValue = huntConsumeRmb * huntCount;
		return UserManager.Instance.self.getRMB () >= consumeValue;
	}/// <summary>
	/// 是否满足显示钻石猎魂VIP等级
	/// </summary>
	public bool isEnoughByShowHuntVipLevel() {
		return UserManager.Instance.self.getVipLevel () >= getShowHuntRmbVipLevel ();
	}
	/// <summary>
	/// 是否满足钻石猎魂VIP等级
	/// </summary>
	public bool isEnoughByHuntVipLevel() {
		return UserManager.Instance.self.getVipLevel () >= getHuntRmbVipLevel ();
	}
	/// <summary>
	/// 是否满足普通一键猎魂VIP等级
	/// </summary>
	public bool isEnoughByHuntOneKeyVipLevel() {
		return UserManager.Instance.self.getVipLevel () >= getOneKeyVipLevel ();
	}
	/// <summary>
	/// 是否满足钻石一键猎魂VIP等级
	/// </summary>
	public bool isEnoughByHuntRmbOneKeyVipLevel() {
		return UserManager.Instance.self.getVipLevel () >= getRmbOneKeyVipLevel ();
	}
	/// <summary>
	/// 打开炼金面板
	/// </summary>
	/// <param name="msg">Message.</param>
	public void getMoney(MessageHandle msg){
		if (msg.buttonID == MessageHandle.BUTTON_RIGHT) {
			return;
		}
		UiManager.Instance.openWindow<NoticeWindow> ((win) => {
			win.entranceId = NoticeSampleManager.Instance.getNoticeSampleBySid(NoticeType.ALCHEMY_SID).entranceId;
			win.updateSelectButton (NoticeType.ALCHEMY_SID);
		});
	}
	/// <summary>
	/// 猎魂条件
	/// </summary>
	/// <param name="huntType">裂魂类型(0=普通,1=钻石)</param>
	/// <param name="huntCount">裂魂次数</param>
	/// <param name="isAuto">是否一键猎魂(true=是)</param>
	public bool checkHuntCondition(int huntType,int huntCount,bool isAuto) {
		if (huntType == HUNT_MONEY_TYPE) {
			if (!isEnoughByHuntMoney(huntCount,StarSoulManager.Instance.getHuntQuality())) { // 游戏币不足
//				UiManager.Instance.openDialogWindow<MessageLineWindow>((win)=>{
//					win.Initialize (LanguageConfigManager.Instance.getLanguage ("guild_851"));
//				});
				UiManager.Instance.openDialogWindow<MessageWindow>((win)=>{
					win.initWindow (2, LanguageConfigManager.Instance.getLanguage ("go_get_money"), LanguageConfigManager.Instance.getLanguage ("s0094"), LanguageConfigManager.Instance.getLanguage ("StarSoulWindow_Hunt_noMoney"), getMoney);
				});
				return false;
			}
		} else if (huntType == HUNT_RMB_TYPE) {
			if (!isEnoughByHuntRMB(huntCount)) { // 钻石不足
				UiManager.Instance.openDialogWindow<MessageLineWindow>((win)=>{
					win.Initialize (LanguageConfigManager.Instance.getLanguage ("Guild_4"));
				});
				return false;
			}	
		}
		if (isAuto) {
            if (huntType == HUNT_RMB_TYPE) {
				if (!isEnoughByHuntRmbOneKeyVipLevel()) { // 钻石一键猎魂vip等级不足
					UiManager.Instance.openDialogWindow<MessageLineWindow>((win)=>{
						win.Initialize (LanguageConfigManager.Instance.getLanguage ("StarSoulWindow_Hunt_VIPInfo", Convert.ToString(StarSoulConfigManager.Instance.getRmbOneKeyVipLevel())));
					});
					return false;
				}	
			}
		}
		StorageManagerment manager=StorageManagerment.Instance;
		if (manager.isHuntSoulStorageFull(huntCount)) { //裂魂仓库已满
			UiManager.Instance.openDialogWindow<MessageLineWindow>((win)=>{
				win.Initialize (LanguageConfigManager.Instance.getLanguage ("StarSoulWindow_HuntStarSoul_Storage_Full"));
			});
			return false;
		}
		return true;
	}
	/// <summary>
	///解析星魂装备凹槽所需等级数组数据
	/// </summary>
	public int[] getGrooveOpenLevel(string[] strs,int startIndex,int endIndex) {
		int[] grooveOpenLe=new int[6];
		int j=0;
		for(int i=startIndex;i<=endIndex;i++) {
			grooveOpenLe[j]=StringKit.toInt(strs[i]);
			j++;
		}
		return grooveOpenLe;
	}

	/* properties */
	/// <summary>
	/// 获取显示砖石猎魂VIP等级
	/// </summary>
	/// <returns>The hunt rmb vip level.</returns>
	public int getShowHuntRmbVipLevel() {
		return showHuntRmbVipLevel;
	}
	/// <summary>
	/// 获取砖石猎魂VIP等级
	/// </summary>
	/// <returns>The hunt rmb vip level.</returns>
	public int getHuntRmbVipLevel() {
		return huntRmbVipLevel;
	}
	/// <summary>
	/// 获取普通一键猎魂VIP等级
	/// </summary>
	/// <returns>The one key vip level.</returns>
	public int getOneKeyVipLevel() {
		return oneKeyVipLevel;
	}
	/// <summary>
	/// 获取钻石一键猎魂VIP等级
	/// </summary>
	/// <returns>The one key vip level.</returns>
	public int getRmbOneKeyVipLevel() {
		return rmbOneKeyVipLevel;
	}
	/// <summary>
	/// 获得单次裂魂消耗的游戏币
	/// </summary>
	public int getHuntConsumeMoney (int huntStarSoulQuality) {
		return huntConsumeMoney[huntStarSoulQuality];
	}
	/// <summary>
	/// 获得单次裂魂消耗的砖石费用
	/// </summary>
	public int getHuntConsumeRmb () {
		return huntConsumeRmb;
	}
	/// <summary>
	/// 得到配置的星魂装备凹槽所需等级数组
	/// </summary>
	/// <returns>The groove open.</returns>
	public int[] getGrooveOpen() {
		return grooveOpneLevel;
	}

}
