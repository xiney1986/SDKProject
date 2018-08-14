using System.Collections.Generic;
using UnityEngine;
using System.Collections;

/// <summary>
/// 坐骑常规相关配置
/// </summary>
public class CommandConfigManager : ConfigManager {

	/* static fields */
	private static CommandConfigManager instance;

	/* static methods */
	public static CommandConfigManager Instance {
		get {
			if (instance == null)
				instance = new CommandConfigManager ();
			return instance;
		}
	}
    public string[][] getMagicWeaponStrengProp(){
        return magiceWeaponStrengProp;
    }
    public string[][] getMagicWeaponPhaseProp() {
        return magicWeaponPhaseProp;
    }

	/* fields */
	/** 圣器强化需要的关卡sid */
	private int missionSid;
    private string[] miningSearchConsume;
    private int[] priceBuy;//购买挑战次数的价格
    private string[][] magiceWeaponStrengProp;
    private string[][] magicWeaponPhaseProp;
    private int[] allscrptSid;//所有的秘宝碎片
    private int[] clmbTowerNum;//爬塔副本的最大开宝箱次数和最大可重置次数
    private int[] clmbTowerBuyCost;//爬塔副本购买开宝箱的价格
    private int[] lotteyMissionSid;//通关以后有宝箱奖励的副本章节（爬塔）
    private PrizeSample[][] towePrizes;//爬塔副本通关以后在界面上显示的宝箱奖励
    private int[] combatMagicWeaponSkill;//
    private float numTempForMagic;//神器相关战斗力计算的指数
    private int nvshenMoneySid;//女神币的sid
    private int[] towerPaiNeedRMB;
    public int[] towerBoxAward;//爬塔有箱子奖励的层数
    public string[] towerBoxDec;//对应上面的描述
    public int towerLimitLevel;//爬塔副本开启等级
    public string[] towerPassDec;//爬塔通关的奖励描述
    public int[] vipBoxNum;//vip对应的爬塔开宝箱次数
    public int[] vipBuyBoxNum;//vip对应的爬塔购买宝箱的次数
    public int[] getBoxAwardSid;//对应
    public string[] nvshenShopSId;//爬塔商店对应解锁的missionSid
    public bool isUseBattleBugFix = false;
    private int magicLimitLevel;//神器开放等级
    private int limitLevel;//限时翻翻乐开放等级
    private int[] fujiaoxishu;//附加系数
    public string[] bloodPointInfo;
    public string[] godsWarsRushTimes;
    public int[] bloodEvoCostForGoodQuality;
    public int[] bloodEvoCostForLegendQuality;
    public int[] signInCost;//补签消耗
    private int[] onoOnOneBossData;//单挑boss开放日期
    private int[] oneOnOneBossTimeInfo;//单挑boss开启和关闭时间（一天内）
    private int oneOnOneBossLimitLv = 0;//单挑boss开放等级
    private int timesOfDay = 0;//每天挑战最大次数
    private int oneOnOneSid = 0;//恶魔挑战sid
    private int huijiMoneySid;//徽记积分sid
    private int MaxNum = 0;//徽记积分上限
    private int bossBuyTimeCost;//购买次数单次消耗
    private int bossBuyTimeGetHuiJiNum;//购买次数获得徽记数量
    private int bossBuyTimeOpenLevel;//开放购买次数Vip等级
    private int[] awardsNum;//排名奖励徽记数量
    private int[] rankLimit;//排名
    private int huiJiPrizeNum = 0;//每挑战一次boss获得的徽记积分
    private int[] refineProp;//精炼经验
    private int[] refinePropSid;//精炼道具ID
    public int maxPvePoint;//最大的行动力值
	public int[] doubleEffectSkillSids;//分正反的技能Sid
    public int[] numOfBeast;//解锁神格槽对应的女神数量
    public int levelOfUser;//神格开放等级
    public List<ShenGeExtraEffect> ShenGeExtra;//神格额外效果
    public List<ShenGePower> shenGePowers;//神格威能 
    public int meritLimit;//功勋上限
    public int maxTimesOfJingJi;//竞技场最大购买次数
    public int yaoQingButtonFlag;//邀请按钮开启关闭:0,关；1，开
    public int nvShenTextureType;//0;没穿衣，1;穿衣
	public LastBattleData lastBattleData;// 末日决战相关数据//
    public string shenGeGongMingString;//神格共鸣计算公式
    private int getMoneySpeed;//天国宝藏金币获取速度最大值
	private int taoFaAwardMaxCount;// 讨伐幸运日有效奖励次数上限//
	private float lastBattle_hpDownTime;// 末日决战血条掉落时长//
	private float lastBattle_xDistance;// 末日决战血条掉落x轴偏量//
	private float lastBattle_yDistance;// 末日决战血条掉落y轴偏量//
	private LotteryData lotteryData;// 大乐透相关数据//
    private int[] bossBuyTimesByVip;//恶魔挑战Vip可购买次数
    private long MoneyMaxNum;//金币最大值
	private int kuaFuSoulHuntID;// 跨服猎魂活动id//
	private int benDiSoulHuntID;// 本地猎魂活动id//


	/* methods */
	public CommandConfigManager () {
		base.readConfig (ConfigGlobal.CONFIG_COMMAND_CONFIG);
	}
	public override void parseConfig (string str) {
		string[] strs = str.Split ('|');
		// str[0] 配置文件说明
		if(str.StartsWith("1^"))missionSid=StringKit.toInt(strs[1]);
		//if(str.StartsWith("2^"))rushNum=StringKit.toInt(strs[1]);
        if (str.StartsWith("3^")) ParseMininngSearchConsume(strs[1]);
        if (str.StartsWith("4^")) parsePrice(strs[1]);
        if (str.StartsWith("5^")) parsePropForMagicStreng(strs[1]);
        if (str.StartsWith("6^")) prisePropForMagicPhase(strs[1]);
        if (str.StartsWith("7^")) priseScrptSid(strs[1]);
        if (str.StartsWith("8^")) priseTowerNum(strs[1]);
        if (str.StartsWith("9^")) priseTowerCost(strs[1]);
        if (str.StartsWith("10^")) {
            priseTowerLotteyMissionSid(strs[1]);
            priseTowerLotterPrizes(strs[2]);
        }
        if (str.StartsWith("11^")) {
            priseCombatMagic(strs[1]);
            numTempForMagic = float.Parse(strs[2]);
        }
        if (str.StartsWith("12^"))parseNvShenMoney(strs[1]);
        if (str.StartsWith("13^")) parseNeedMoney(strs[1]);
        if (str.StartsWith("14^")) pariseBoxDec(strs[1]);
        if (str.StartsWith("15^")) towerLimitLevel = StringKit.toInt(strs[1]);
        if (str.StartsWith("16^")) parseTowerPassDec(strs[1]);
        if (str.StartsWith("17^")) parseVipBoxNum(strs[1]);
        if (str.StartsWith("18^")) parseVipBuyBoxNum(strs[1]);
        if (str.StartsWith("19^")) parseGetBoxAwardSid(strs[1]);
        if (str.StartsWith("20^")) parseNvShenShop(strs[1]);
        if (str.StartsWith("21^")) isUseBattleBugFix = StringKit.toInt(strs[1]) == 1;
        if (str.StartsWith("22^")) magicLimitLevel = StringKit.toInt(strs[1]);
        if (str.StartsWith("23^")) limitLevel = StringKit.toInt(strs[1]);
	    if (str.StartsWith("24^")) parseFujiao(strs[1]);
	    if (str.StartsWith("26^")) parseGodsTimes(strs[1]);
        if (str.StartsWith("27^")) parseEvoCost(strs[1]);
        if (str.StartsWith("28^")) parseSignCost(strs[1]);
        if (str.StartsWith("29^")) parseOneOnOneBossInfo(strs[1]);
        if (str.StartsWith("30^")) parseBossAward(strs[1]);
        if (str.StartsWith("31^")) parseRefineProp(strs[1]);
        if (str.StartsWith("32^")) maxPvePoint=StringKit.toInt(strs[1]);
		if (str.StartsWith("33^")) lastBattleData = LastBattleManagement.Instance.parseLastBattleData(strs[1]);
		if (str.StartsWith("34^")) parseDoubleSid(strs[1]);
        if (str.StartsWith("35^")) parseShenGe(strs[1]);
	    if (str.StartsWith("36^")) parseShenGeExtraInfo(strs[1]);
	    if (str.StartsWith("37^"))
	    {
	        parseShenGePowerInfo(strs[1]);
            shenGeGongMingString = strs[2];
	    }
	    if (str.StartsWith("38^")) meritLimit = StringKit.toInt(strs[1]);
	    if (str.StartsWith("39^")) maxTimesOfJingJi = StringKit.toInt(strs[1]);
        if (str.StartsWith("40^")) yaoQingButtonFlag = StringKit.toInt(strs[1]);
        if (str.StartsWith("41^")) nvShenTextureType = StringKit.toInt(strs[1]);
        if (str.StartsWith("42^")) getMoneySpeed = StringKit.toInt(strs[1]);
		if (str.StartsWith("43^")) taoFaAwardMaxCount = StringKit.toInt(strs[1]);
		if (str.StartsWith("44^")) lastBattle_hpDownTime = StringKit.toFloat(strs[1]);
		if (str.StartsWith("45^"))
		{
			string[] strArr = strs[1].Split(',');
			lastBattle_xDistance = StringKit.toFloat(strArr[0]);
			lastBattle_yDistance = StringKit.toFloat(strArr[1]);
		}
		if (str.StartsWith("46^")) lotteryData = LotteryManagement.Instance.parseLotteryData(strs[1]);
        if (str.StartsWith("47^")) parseBossBuyTimeByVip(strs[1]);
	    if (str.StartsWith("48")) MoneyMaxNum = StringKit.toLong(strs[1]);
		if (str.StartsWith("49")) kuaFuSoulHuntID = StringKit.toInt(strs[1]);
		if (str.StartsWith("50")) benDiSoulHuntID = StringKit.toInt(strs[1]);
	}

    private void parseBossBuyTimeByVip(string str)
    {
        string[] strArr = str.Split(',');
        bossBuyTimesByVip = new int[strArr.Length];
        for (int i = 0; i < strArr.Length; i++)
        {
            bossBuyTimesByVip[i] = StringKit.toInt(strArr[i]);
        }
    }

    private void parseShenGePowerInfo(string str) {
        shenGePowers = new List<ShenGePower>();
        string[] sttr = str.Split('#');
        for (int i = 0; i < sttr.Length; i++)
        {
            string tmp = sttr[i];
            string[] strrStrings = tmp.Split('&');
            ShenGePower shenGePower = new ShenGePower();
            shenGePower.level = StringKit.toInt(strrStrings[0]);
            string tempStr = strrStrings[1];
            string[] str2 = tempStr.Split('*');
            List<AttrInfo> newAttrInfos = new List<AttrInfo>();
            for (int k = 0; k < str2.Length; k++)
            {
                string tempp = str2[k];
                string[] str3 = tempp.Split(',');
                AttrInfo attrInfo = new AttrInfo();
                attrInfo.type = StringKit.toInt(str3[0]);
                attrInfo.value = StringKit.toInt(str3[1]);
                newAttrInfos.Add(attrInfo);
            }
            shenGePower.AttrInfos = newAttrInfos;
            shenGePowers.Add(shenGePower);
        }
    }

    private void parseShenGeExtraInfo(string str)
    {
        ShenGeExtra = new List<ShenGeExtraEffect>();
        string[] sttr = str.Split('#');
        for (int i = 0; i < sttr.Length; i++) {
            string tmp = sttr[i];
            string[] strrStrings = tmp.Split('&');
            ShenGeExtraEffect shenGeExtra = new ShenGeExtraEffect();
            shenGeExtra.level = StringKit.toInt(strrStrings[0]);
            string tempStr = strrStrings[1];
            string[] str2 = tempStr.Split('*');
            List<AttrInfo> newAttrInfos = new List<AttrInfo>();
            for (int k = 0; k < str2.Length; k++) {
                string tempp = str2[k];
                string[] str3 = tempp.Split(',');
                AttrInfo attrInfo = new AttrInfo();
                attrInfo.type = StringKit.toInt(str3[0]);
                attrInfo.value = StringKit.toInt(str3[1]);
                newAttrInfos.Add(attrInfo);
            }
            shenGeExtra.AttrInfos = newAttrInfos;
            ShenGeExtra.Add(shenGeExtra);
        }
    }

    private void parseShenGe(string str)
    {
        string[] st = str.Split('#');
        levelOfUser = StringKit.toInt(st[1]);
        string[] sttr = st[0].Split(',');
        numOfBeast = new int[sttr.Length];
        for (int i = 0; i < sttr.Length; i++)
        {
            numOfBeast[i] = StringKit.toInt(sttr[i]);
        }
    }

    private void parseDoubleSid(string str)
	{
		string[] st = str.Split(',');
		doubleEffectSkillSids=new int[st.Length];
		for (int i=0;i<st.Length;i++)
		{
			doubleEffectSkillSids[i] = StringKit.toInt(st[i]);
		}
	}
    private void parseRefineProp(string str)
    {
        string[] st = str.Split(',');
        refineProp = new int[st.Length / 2];
        refinePropSid = new int[st.Length / 2];
        int j = 0;
        int k = 0;
        for (int i = 0; i < st.Length; i++)
        {
            if (i % 2 == 1) refineProp[j++] = StringKit.toInt(st[i]);
            else refinePropSid[k++] = StringKit.toInt(st[i]);
        }

    }
    private void parseOneOnOneBossInfo(string str) {
        string[] strs = str.Split('#');
        onoOnOneBossData = new int[strs[0].Length];
        oneOnOneBossTimeInfo = new int[strs[1].Length];
        string str1 = strs[0];
        string str2 = strs[1];
        string[] strs1 = str1.Split(',');
        string[] strs2 = str2.Split(',');
        for (int i = 0; i < strs1.Length; i++) {
            onoOnOneBossData[i] = StringKit.toInt(strs1[i]);
        }
        for (int i = 0; i < strs2.Length; i++) {
            oneOnOneBossTimeInfo[i] = StringKit.toInt(strs2[i]);
        }
        oneOnOneBossLimitLv = StringKit.toInt(strs[2]);
        timesOfDay = StringKit.toInt(strs[3]);
		oneOnOneSid = StringKit.toInt (strs[4]);
        huijiMoneySid = StringKit.toInt(strs[5]);
        huiJiPrizeNum = StringKit.toInt(strs[6]);
        bossBuyTimeCost = StringKit.toInt(strs[7]);
        bossBuyTimeGetHuiJiNum = StringKit.toInt(strs[8]);
        bossBuyTimeOpenLevel = StringKit.toInt(strs[9]);

    }
    private void parseBossAward(string str) {
        string[] strss = str.Split('#');
        MaxNum = StringKit.toInt(strss[0]);
        string strs = strss[1];
        string[] strsss = strs.Split(',');
        awardsNum = new int[strsss.Length];
        for (int i = 0; i < strsss.Length; i++) {
            awardsNum[i] = StringKit.toInt(strsss[i]);
        }
        string strs1 = strss[2];
        string[] strsss1 = strs1.Split(',');
        rankLimit = new int[strsss1.Length];
        for (int i = 0; i < strsss1.Length; i++) {
            rankLimit[i] = StringKit.toInt(strsss1[i]);
        }
    }
    private void parseSignCost(string str) {
        string[] st = str.Split(',');
        signInCost = new int[st.Length];
        for (int i = 0; i < st.Length; i++) {
            signInCost[i] = StringKit.toInt(st[i]);
        }
    }

    private void parseEvoCost(string str) {
        string[] st = str.Split('#');
        string strs = st[0];
        string[] st1 = strs.Split(',');
        string strss = st[1];
        string[] st2 = strss.Split(',');
        bloodEvoCostForGoodQuality = new int[st1.Length];
        bloodEvoCostForLegendQuality = new int[st2.Length];
        for (int i = 0; i < st1.Length; i++) {
            bloodEvoCostForGoodQuality[i] = StringKit.toInt(st1[i]);
        }
        for (int k = 0; k < st2.Length; k++) {
            bloodEvoCostForLegendQuality[k] = StringKit.toInt(st2[k]);
        }
    }
    private void parseGodsTimes(string str)
    {
        string[] st = str.Split(',');
        godsWarsRushTimes=new string[st.Length];
        godsWarsRushTimes = st;
    }
    private void parseFujiao(string str) {
        string[] st = str.Split(',');
        fujiaoxishu = new int[st.Length];
        for (int i = 0; i < st.Length; i++) {
            fujiaoxishu[i] = StringKit.toInt(st[i]);
        }
    }
    private void parseNvShenShop(string str) {
        string[] st = str.Split(',');
        nvshenShopSId = new string[st.Length];
        for (int i = 0; i < st.Length;i++ ) {
            nvshenShopSId[i] = st[i];
        }
    }
    private void parseGetBoxAwardSid(string str) {
        string[] st = str.Split(',');
        getBoxAwardSid = new int[st.Length];
        for (int i = 0; i < st.Length; i++) {
            getBoxAwardSid[i] = StringKit.toInt(st[i]);
        }
    }
    private void parseVipBuyBoxNum(string str) {
        string[] st = str.Split(',');
        vipBuyBoxNum = new int[st.Length];
        for (int i = 0; i < st.Length; i++) {
            vipBuyBoxNum[i] = StringKit.toInt(st[i]);
        }
    }
    private void parseVipBoxNum(string str) {
        string[] st = str.Split(',');
        vipBoxNum = new int[st.Length];
        for (int i = 0; i < st.Length;i++ ) {
            vipBoxNum[i] = StringKit.toInt(st[i]);
        }
    }
    private void parseTowerPassDec(string str) {
        string[] st = str.Split(',');
        towerPassDec = new string[st.Length];
        for (int i = 0; i < st.Length;i++) {
            towerPassDec[i] = st[i];
        }
    }
    private void pariseBoxDec(string str) {
        string[] st = str.Split('#');
        string[] numm = st[0].Split(',');
        string[] dec = st[1].Split(',');
        towerBoxAward = new int[numm.Length];
        for (int i = 0; i < numm.Length; i++) {
            towerBoxAward[i] = StringKit.toInt(numm[i]);
        }
        towerBoxDec = new string[dec.Length];
        for (int j = 0; j < dec.Length;j++ ) {
            towerBoxDec[j] = dec[j];
        }

    }
    private void parseNeedMoney(string st) {
        string[] ss = st.Split(',');
        towerPaiNeedRMB = new int[ss.Length];
        for (int i = 0; i < ss.Length;i++ ) {
            towerPaiNeedRMB[i] = StringKit.toInt(ss[i]);
        }
    }
    
    private void priseCombatMagic(string str){
        string[] stt=str.Split(',');
        combatMagicWeaponSkill=new int[stt.Length];
        for(int i=0;i<stt.Length;i++){
            combatMagicWeaponSkill[i]=StringKit.toInt(stt[i]);
        }
    }
    private void parseNvShenMoney(string str) {
        nvshenMoneySid = StringKit.toInt(str);
    }
    private void priseTowerLotterPrizes(string str) {
        string[] sts = str.Split('*');
        towePrizes=new PrizeSample[sts.Length][];
        for (int i = 0; i < sts.Length;i++ ) {
            string[] st = sts[i].Split('#');
            towePrizes[i] = new PrizeSample[st.Length];
            for (int j = 0; j < st.Length;j++ ) {
                towePrizes[i][j] =  new PrizeSample(st[j], ',');
            }
        }
    }
    private void priseTowerLotteyMissionSid(string str) {
        string[] st = str.Split(',');
        lotteyMissionSid = new int[st.Length];
        for (int i = 0; i < st.Length;i++ ) {
            lotteyMissionSid[i] = StringKit.toInt(st[i]);
        }

    }
    private void priseTowerCost(string str) {
        string[] st = str.Split(',');
        clmbTowerBuyCost = new int[st.Length];
        for (int i = 0; i < st.Length; i++) {
            clmbTowerBuyCost[i] = StringKit.toInt(st[i]);
        }
    }
    private void priseTowerNum(string str) {
        string[] st = str.Split(',');
        clmbTowerNum = new int[st.Length];
        for (int i = 0; i < st.Length;i++ ) {
            clmbTowerNum[i] = StringKit.toInt(st[i]);
        }
    }
    //allscrptSid
    private void priseScrptSid(string str) {
        string[] st = str.Split(',');
        allscrptSid = new int[st.Length];
        for (int i = 0; i < st.Length;i++ ) {
            allscrptSid[i] = StringKit.toInt(st[i]);
        }
    }
    private void parsePropForMagicStreng(string str) {
        string[] st = str.Split('#');
        magiceWeaponStrengProp = new string[st.Length][];
        for (int i = 0; i < st.Length;i++) {
            string[] stt=st[i].Split('$');
            magiceWeaponStrengProp[i] = new string[stt.Length];
            for (int j = 0; j < stt.Length;j++ ) {
                magiceWeaponStrengProp[i][j] = stt[j];
            }
        }
    }
    private void prisePropForMagicPhase(string str) {
        string[] st = str.Split('#');
        magicWeaponPhaseProp = new string[st.Length][];
        for (int i = 0; i < st.Length; i++) {
            string[] stt = st[i].Split('$');
            magicWeaponPhaseProp[i] = new string[stt.Length];
            for (int j = 0; j < stt.Length; j++) {
                magicWeaponPhaseProp[i][j] = stt[j];
            }
        }
    }
	/* properties */
	/// <summary>
	/// 获得储备行动力开放等级
	/// </summary>
	public int getPassMissionSid () {
		return missionSid;
	}
    public int getTowerLimitLevel() {
        return this.towerLimitLevel;
    }
    private void parsePrice(string str)
    {
        string[] strs = str.Split('#');
        priceBuy = new int[strs.Length];
        for (int i = 0; i < strs.Length;i++ )
        {
            priceBuy[i] = StringKit.toInt(strs[i]);
        }
    }
    public int[] getPrice()
    {
        return priceBuy;
    }

    public string[] GetMiningSearchConsume() {
        return miningSearchConsume;
    }

    public void ParseMininngSearchConsume(string str) {
        miningSearchConsume = str.Split('#');
    }
    public int[] getAllScrptSid() {
        return allscrptSid;
    }
    public int[] getTowerMaxNum() {
        return clmbTowerNum;
    }
    public int[] getTowerBuyCost() {
        return clmbTowerBuyCost; ;
    }
    public int[] getLotteyMissionSid() {
        return lotteyMissionSid;
    }
    public PrizeSample[][] getPrizeShow() {
        return towePrizes;
    }
    public PrizeSample[] getMissionPrizeBySid(int sid) {
        for (int i = 0; i < lotteyMissionSid.Length;i++ ) {
            if (lotteyMissionSid[i] == sid) {
                return towePrizes[i];
            }
        }
         return null;
    }
    public int[] combatMagicWeapon(){
        return combatMagicWeaponSkill;
    }
    public float combatMagicNum() {
        return numTempForMagic;
    }
    public int getnvshenMoneySid() {
        return nvshenMoneySid;
    }
    public int[] getTowerMoney() {
        return towerPaiNeedRMB;
    }
    /// <summary>
    /// 得到爬塔通关的描述
    /// </summary>
    /// <returns></returns>
    public string[] getTowerPassDec() {
        return towerPassDec;
    }
    public int[] getVipBoxNum() {
        return vipBoxNum;
    }
    public int[] getVipBuyBoxNum() {
        return vipBuyBoxNum;
    }
    public int[] getBoxAwardS() {
        return getBoxAwardSid;
    }
    public string[] getNvShenShopSid() {
        return nvshenShopSId;
    }
    public bool getOpenBattleFix() {
        return isUseBattleBugFix;
    }
    public int getMagicLimitLevel() {
        return magicLimitLevel;
    }
    /// <summary>
    /// 获取限时翻翻乐的开放等级
    /// </summary>
    /// <returns></returns>
    public int getLimitLevel() {
        return limitLevel;
    }

    public int[] getFujiaoSuxi()
    {
        return fujiaoxishu;
    }
    public string[] getBloodPointInfo() {
        return bloodPointInfo;
    }
    public int[] getEvoCostByQuality(int qualityId) {
        if (qualityId == QualityType.EPIC) {
            return bloodEvoCostForGoodQuality;
        } else if (qualityId == QualityType.LEGEND) {
            return bloodEvoCostForLegendQuality;
        }
        return null;
    }
    public int[] getSignInCost() {
        return signInCost;
    }
    /// <summary>
    /// 恶魔挑战开放时间（7：30-11:30）
    /// </summary>
    /// <returns></returns>
    public int[] getOneOnOneBossData() {
        return onoOnOneBossData;
    }
    /// <summary>
    /// 恶魔挑战开放日期（星期1,2.....）
    /// </summary>
    /// <returns></returns>
    public int[] getOneOnOneBossTimeInfo() {
        return oneOnOneBossTimeInfo;
    }
    /// <summary>
    /// 恶魔挑战开放等级
    /// </summary>
    /// <returns></returns>
    public int getOneOnOneBossLimitLv() {
        return oneOnOneBossLimitLv;
    }
    /// <summary>
    /// 取得恶魔挑战最大次数
    /// </summary>
    /// <returns></returns>
    public int getTimesOfDay() {
        return timesOfDay;
    }
	/// <summary>
	/// 获得恶魔挑战sid
	/// </summary>
	/// <returns>The boss fight sid.</returns>
	public int getBossFightSid(){
		return oneOnOneSid;
	}
    /// <summary>
    /// 取得徽记积分上限
    /// </summary>
    /// <returns></returns>
    public int getMaxNum() {
        return MaxNum;
    }
    /// <summary>
    /// 取得排名对应的徽记积分奖励个数
    /// </summary>
    /// <returns></returns>
    public int[] getAwardNum() {
        return awardsNum;
    }
    /// <summary>
    /// 取得排名区间
    /// </summary>
    /// <returns></returns>
    public int[] getRankLimit() {
        return rankLimit;
    }
    /// <summary>
    /// 取得徽记积分sid
    /// </summary>
    /// <returns></returns>
    public int getHuiJiMoneySid() {
        return huijiMoneySid;
    }

    /// <summary>
    /// 获得精炼道具的经验
    /// </summary>
    /// <returns></returns>
    public int[] getRefinePropEXP()
    {
        return refineProp;
    }
    /// <summary>
    /// 获得精炼道具的SID
    /// </summary>
    /// <returns></returns>
    public int[] getRefinePropSid()
    {
        return refinePropSid;
    }
    /// <summary>
    /// 获得每次挑战boss获得的徽记积分数
    /// </summary>
    /// <returns></returns>
    public int getHuiJiPrizeNum() {
        return huiJiPrizeNum;
    }
    public int getLimitOfShenGeLevel() {
        return levelOfUser;
    }

    public int[] getNumOfBeast() {
        return numOfBeast;
    }
    ///获取神格额外效果
    public List<ShenGeExtraEffect> GetShenGeExtraEffectsList()
    {
        return ShenGeExtra;
    }
    ///获取神格威能效果
    public List<ShenGePower> GetsheGePowersList()
    {
        return shenGePowers;
    }

    public int getLimitOfMerit()
    {
        return meritLimit;
    }

    public int getMaxTimesOfJingJi()
    {
        return maxTimesOfJingJi;
    }

    public int getNvShenClothType()
    {
        return nvShenTextureType;
    }

    public int getMoneySpeedOfArean()
    {
        return getMoneySpeed;
    }

	public int getTaoFaMaxCount()
	{
		return taoFaAwardMaxCount;
	}

	public float getLastBattleHpDownTime()
	{
		return lastBattle_hpDownTime;
	}

	public float getLastBattleXDistance()
	{
		return lastBattle_xDistance;
	}

	public float getLastBattleYDistance()
	{
		return lastBattle_yDistance;
	}

	public LotteryData getLotteryData()
	{
		return lotteryData;
	}
    /// <summary>
    /// 购买次数单次消耗
    /// </summary>
    /// <returns></returns>
    public int getBuyTimeCost() {
        return bossBuyTimeCost;
    }

    /// <summary>
    /// Vip等级对应的可购买次数
    /// </summary>
    /// <returns></returns>
    public int[] getBossBuyTimesByVip() {
        return bossBuyTimesByVip;
    }

    /// <summary>
    /// 购买次数获得徽记数量
    /// </summary>
    /// <returns></returns>
    public int getBuyTimeGetHuiJi() {
        return bossBuyTimeGetHuiJiNum;
    }

    /// <summary>
    /// 开放购买次数功能Vip等级
    /// </summary>
    /// <returns></returns>
    public int getOpenBuyTimeVipLevel() {
        return bossBuyTimeOpenLevel;
    }

    public long getMaxMoneyNum()
    {
        return MoneyMaxNum;
    }

	public int getKuaFuSoulHunt()
	{
		return kuaFuSoulHuntID;
	}
	public int getBenDiSoulHunt()
	{
		return benDiSoulHuntID;
	}
}