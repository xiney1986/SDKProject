using UnityEngine;
using System;
using System.Collections.Generic;

/**
 * 副本管理器
 * @author longlingquan
 * */
public class AttackBossOneOnOneManager
{
    public int bossSid;//boss sid
    public List<int> damageList;//伤害列表
    public int canChallengeTimes = 0;//可挑战次数
    public int buyTimes = 0;//已购买次数
    public string totalDamage = "";//总伤害
    public List<string> usedCardUid = new List<string>();//已经挑战过的卡片uid
    public Card selectedCard = null;//选择的卡片
    public Card choosedBeast = null;//选择的女神
    public List<FightInfo> fightInfo;//战报信息
    public long damageTemp;//进战斗前的伤害
	public long damageValue;// 战报发过来的伤害值//
	
	public static AttackBossOneOnOneManager Instance {
		get{ return SingleManager.Instance.getObj ("AttackBossOneOnOneManager") as AttackBossOneOnOneManager;}
	}

	/// <summary>
	/// 仓库是否已满
	/// </summary>
	public bool isStoreFull () {
		bool isFull = false;
		string strErr = "";
		if ((StorageManagerment.Instance.getAllRole ().Count + 1) > StorageManagerment.Instance.getRoleStorageMaxSpace ()) {
			isFull = true;
			strErr = LanguageConfigManager.Instance.getLanguage ("storeFull_card");
		}
		else if ((StorageManagerment.Instance.getAllEquip ().Count + 1) > StorageManagerment.Instance.getEquipStorageMaxSpace ()) {
			isFull = true;
			strErr = LanguageConfigManager.Instance.getLanguage ("storeFull_equip");
		}
		else if ((StorageManagerment.Instance.getAllTemp ().Count + 100) > StorageManagerment.Instance.getTempStorageMaxSpace ()) {
			isFull = true;
			strErr = LanguageConfigManager.Instance.getLanguage ("storeFull_temp");
		}
		else if ((StorageManagerment.Instance.getAllStarSoul ().Count + 1) > StorageManagerment.Instance.getStarSoulStorageMaxSpace ()) {
			isFull = true;
			strErr = LanguageConfigManager.Instance.getLanguage ("storeFull_starSoul");
		}
		if (isFull) {
            GuideManager.Instance.jumpGuideSid();
			MessageWindow.ShowAlert (strErr + LanguageConfigManager.Instance.getLanguage ("storeFull_msg_01"));
			return true;
		}
		else {
			return false;
		}
	}
    public int getBossIcon(int sid) {
        BossInfoSample sample = BossInfoSampleManager.Instance.getBossInfoSampleBySid(sid);
        return sample.imageID;
    }
    public int getBossIcon() {
        BossInfoSample sample = BossInfoSampleManager.Instance.getBossInfoSampleBySid(bossSid);
        return sample.imageID;
    }
    public string getBossName(int sid) {
        BossInfoSample sample = BossInfoSampleManager.Instance.getBossInfoSampleBySid(sid);
        return sample.name;
    }
    public string getBossName() {
        BossInfoSample sample = BossInfoSampleManager.Instance.getBossInfoSampleBySid(bossSid);
        return sample.name;
    }
    public string getWeakNess() {
        BossInfoSample sample = BossInfoSampleManager.Instance.getBossInfoSampleBySid(bossSid);
        return sample.weakDesc;
    }
    public string getTotalDamage() {
        return totalDamage;
    }
    public List<string> getUsedCardList() {
        return usedCardUid;
    }
    public List<FightInfo> getFightInfo() {
        return fightInfo;
    }
    public bool bossFightIsOpen() {
        DateTime dt = TimeKit.getDateTimeMillis(ServerTimeKit.getMillisTime());//获取服务器时间
        int dayOfWeek = TimeKit.getWeekCHA(dt.DayOfWeek);
        int nowOfDay = ServerTimeKit.getCurrentSecond();
        int[] timeInfo = CommandConfigManager.Instance.getOneOnOneBossTimeInfo();//开放时间
        int[] data = CommandConfigManager.Instance.getOneOnOneBossData();//开放日期
        for (int i = 0; i < data.Length; i++) {
            if (dayOfWeek == data[i] && (nowOfDay >= timeInfo[0] && nowOfDay <= timeInfo[1])) {
                return true;
            }
        }
        return false;
    }
	
}
public class FightInfo {
    public int cardSid = 0;
    public int beastSid = 0;
    public int bossSid = 0;
    public string damage = "";
}
