using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 装备升星配置相关管理
/// </summary>
public class EquipStarConfigManager : SampleConfigManager
{	
	/* static fields */
	private static EquipStarConfigManager instance;
	
	/* static methods */
	public static EquipStarConfigManager Instance {
		get {
			if (instance == null)
				instance = new EquipStarConfigManager();
			return instance;
		}
	}
    #region
    ///* fields */
    ///** 装备升星玩家等级 */
    //private int[] equipStarUserLevel;
    ///** 装备升星玩家VIP等级 */
    //private int[] equipStarUserVipLevel;
    ///** 装备升星装备品质 */
    //private int[] equipStarEquipQuality;
    ///** 装备升星装备等级 */
    //private int[] equipStarEquipLevel;
    ///** 单次升星消耗装备结晶sid */
    //private int[] equipStarConsumeCrystalSids;
    ///** 单次升星消耗装备结晶sid */
    //private int[] equipStarConsumeCrystalValues;
    ///** 单次升星消耗符文石sid */
    //private int[] equipStarConsumeStoneSids;
    ///** 单次升星消耗符文石sid */
    //private int[] equipStarConsumeStoneValues;
    ///** 红装单次升星消耗金币 */
    //private long[] redEquipStarConsumeMoneyValues;
    ///* methods */
    //public EquipStarConfigManager() {
    //    base.readConfig(ConfigGlobal.CONFIG_EQUIPSTAR_OPERATOR);
    //}
    #endregion
    public EquipStarConfigManager() {
        base.readConfig(ConfigGlobal.CONFIG_EQUIPSTAR_OPERATOR);
    }
    public EquipStarSample GetEquipStarSampleBySid(int sid)
    {
        if (!isSampleExist(sid))
            createSample(sid);
        return samples[sid] as EquipStarSample;
    }

    public override void parseSample(int sid)
    {
        EquipStarSample sample = new EquipStarSample();
        string dataStr = getSampleDataBySid(sid);
        sample.parse(sid, dataStr);
        samples.Add(sid, sample);
    }
    #region
//    /** 解析 */
//    public override void parseConfig(string str) {
//        string[] strs=str.Split('|');
//        checkLength (strs.Length,10);
//        parseUserLevel (strs [1]);
//        parseUserVipLevel (strs [2]);
//        parseEquipQuality (strs [3]);
//        parseEquipLevel (strs [4]);
//        parseConsumeCrystalSid (strs [5]);
//        parseConsumeCrystalValue (strs [6]);
//        parseConsumeStoneSid (strs [7]);
//        parseConsumeStoneValue (strs [8]);
//        parseConsumeMoneyValue(strs[9]);

//    }
//    public void checkLength (int len, int max) { 
//        if (len != max)
//            throw new Exception (this.GetType () + " ConfigGlobal.CONFIG_STARSOUL_OPERATOR error len=" + len + " max=" + max);
//    }
//    /** 解析玩家等级限制 */
//    private void parseUserLevel (string str)
//    {
//        //表示空
//        if (str == Convert.ToString(0))
//            return;
//        string[] strArr = str.Split ('#');  
//        equipStarUserLevel = new int[strArr.Length]; 
//        for (int i = 0; i<strArr.Length; i++) {
//            equipStarUserLevel [i] = StringKit.toInt(strArr [i]);
//        }
//    }
//    /** 解析玩家VIP等级限制 */
//    private void parseUserVipLevel (string str)
//    {
//        //表示空
//        if (str == Convert.ToString(0))
//            return;
//        string[] strArr = str.Split ('#');  
//        equipStarUserVipLevel = new int[strArr.Length]; 
//        for (int i = 0; i<strArr.Length; i++) {
//            equipStarUserVipLevel [i] = StringKit.toInt(strArr [i]);
//        }
//    }

//    /** 解析装备品质限制 */
//    private void parseEquipQuality (string str)
//    {
//        //表示空
//        if (str == Convert.ToString(0))
//            return;
//        string[] strArr = str.Split ('#');  
//        equipStarEquipQuality = new int[strArr.Length]; 
//        for (int i = 0; i<strArr.Length; i++) {
//            equipStarEquipQuality [i] = StringKit.toInt(strArr [i]);
//        }
//    }
//    /** 解析装备等级限制 */
//    private void parseEquipLevel (string str)
//    {
//        //表示空
//        if (str == Convert.ToString(0))
//            return;
//        string[] strArr = str.Split ('#');  
//        equipStarEquipLevel = new int[strArr.Length]; 
//        for (int i = 0; i<strArr.Length; i++) {
//            equipStarEquipLevel [i] = StringKit.toInt(strArr [i]);
//        }
//    }
//    /** 解析消耗结晶sid */
//    private void parseConsumeCrystalSid (string str)
//    {
//        //表示空
//        if (str == Convert.ToString(0))
//            return;
//        string[] strArr = str.Split ('#');  
//        equipStarConsumeCrystalSids = new int[strArr.Length]; 
//        for (int i = 0; i<strArr.Length; i++) {
//            equipStarConsumeCrystalSids [i] = StringKit.toInt(strArr [i]);
//        }
//    }
//    /** 解析消耗结晶value */
//    private void parseConsumeCrystalValue (string str)
//    {
//        //表示空
//        if (str == Convert.ToString(0))
//            return;
//        string[] strArr = str.Split ('#');  
//        equipStarConsumeCrystalValues = new int[strArr.Length]; 
//        for (int i = 0; i<strArr.Length; i++) {
//            equipStarConsumeCrystalValues [i] = StringKit.toInt(strArr [i]);
//        }
//    }
//    /** 解析消耗符文石sid */
//    private void parseConsumeStoneSid (string str)
//    {
//        //表示空
//        if (str == Convert.ToString(0))
//            return;
//        string[] strArr = str.Split ('#');  
//        equipStarConsumeStoneSids = new int[strArr.Length]; 
//        for (int i = 0; i<strArr.Length; i++) {
//            equipStarConsumeStoneSids [i] = StringKit.toInt(strArr [i]);
//        }
//    }
//    /** 解析消耗符文石value */
//    private void parseConsumeStoneValue (string str)
//    {
//        //表示空
//        if (str == Convert.ToString(0))
//            return;
//        string[] strArr = str.Split ('#');  
//        equipStarConsumeStoneValues = new int[strArr.Length]; 
//        for (int i = 0; i<strArr.Length; i++) {
//            equipStarConsumeStoneValues [i] = StringKit.toInt(strArr [i]);
//        }
//    }
//    /** 解析升红后装备升星消耗的金币 */
//    private void parseConsumeMoneyValue(string str)
//    {
//        string[] strArr = str.Split('#');
//        redEquipStarConsumeMoneyValues = new long[strArr.Length];
//        for (int i = 0; i < strArr.Length; i++)
//        {
//            redEquipStarConsumeMoneyValues[i] = StringKit.toLong(strArr[i]);
//        }
//    }
//#endregion

//    #region
//    /// <summary>
//    /// 校验玩家等级是否足够
//    /// </summary>
//    public bool isEnoughByUserLevel(int index) {
//        return UserManager.Instance.self.getUserLevel ()>=equipStarUserLevel[index];
//    }

//    /// <summary>
//    /// 校验玩家VIP等级是否足够
//    /// </summary>
//    public bool isEnoughByUserVipLevel(int index) {
//        return UserManager.Instance.self.getVipLevel ()>=equipStarUserVipLevel[index];
//    }

//    /// <summary>
//    /// 获取玩家等级条件
//    /// </summary>
//    public int getUserLevelCondition(int index) {
//        return equipStarUserLevel[index];
//    }
//    /// <summary>
//    /// 获取玩家VIP等级条件
//    /// </summary>
//    public int getUserVipLevelCondition(int index) {
//        return equipStarUserVipLevel[index];
//    }
//    /// <summary>
//    /// 获取装备品质条件
//    /// </summary>
//    public int getEquipQualityCondition(int index) {
//        return equipStarEquipQuality[index];
//    }
//    /// <summary>
//    /// 获取装备等级条件
//    /// </summary>
//    public int getEquipLevelCondition(int index) {
//        return equipStarEquipLevel[index];
//    }
    #endregion

    /// <summary>
	/// 获取装备结晶消耗条件
	/// </summary>
	public int[] getCrystalCondition( int sid,int index)
    {
        EquipStarSample sample = GetEquipStarSampleBySid(sid);
        if (sample == null)
            return null;
        if (index == sample.equipStarConsumeCrystalSids.Length) {
            return new int[] { sample.equipStarConsumeCrystalSids[sample.equipStarConsumeCrystalSids.Length - 1], 0 };
		}
        return new int[] { sample.equipStarConsumeCrystalSids[index], sample.equipStarConsumeCrystalValues[index] };
	}

	/// <summary>
	/// 获取符文石消耗条件
	/// </summary>
	public int[] getStoneCondition(int sid,int index) {
        EquipStarSample sample = GetEquipStarSampleBySid(sid);
        if (sample == null)
            return null;
        if (index == sample.equipStarConsumeStoneSids.Length) {
            return new int[] { sample.equipStarConsumeStoneSids[sample.equipStarConsumeStoneSids.Length - 1], 0 };
		}
        return new int[] { sample.equipStarConsumeStoneSids[index], sample.equipStarConsumeStoneValues[index] };
	}

    /// <summary>
    /// 获取红装升星的金币消耗条件
    /// </summary>
    /// <param name="sid"></param>
    /// <returns></returns>
    public PrizeSample[] getMoneyCostBySid(int sid)
    {
        EquipStarSample sample = GetEquipStarSampleBySid(sid);
        if (sample == null)
            return null;
        if (sample.redEquipStarConsumeMoneyValues == null)
            return null;
        PrizeSample[] prizes= new PrizeSample[sample.redEquipStarConsumeMoneyValues.Length];
        for (int i = 0; i < sample.redEquipStarConsumeMoneyValues.Length; i++)
        {
            string str = sample.redEquipStarConsumeMoneyValues[i];
            string[] strs = str.Split(',');
            PrizeSample prize = new PrizeSample();
            if (strs.Length == 1)
            {
                prize.type = PrizeType.PRIZE_MONEY;
                prize.pSid = 0;
                prize.num = strs[0];
            }
            else
            {
                prize.type = StringKit.toInt(strs[0]);
                prize.pSid = StringKit.toInt(strs[1]);
                prize.num = strs[2];
            }
            prizes[i] = prize;
        }
        return prizes;
    }
	///<summary>
	/// 获取从1星指定星级的装备结晶消耗
	/// </summary>
	public List<PrizeSample> getCrystalConsume(int sid,int level){
        EquipStarSample sample = GetEquipStarSampleBySid(sid);
        if (sample == null)
            return null;
		List<PrizeSample> pList = new List<PrizeSample> ();
		for (int i=0; i<level; i++) {
            PrizeSample tmp = new PrizeSample(PrizeType.PRIZE_PROP, sample.equipStarConsumeCrystalSids[i], sample.equipStarConsumeCrystalValues[i]);
			pList.Add(tmp);
		}
		return pList;
	}
	///<summary>
	/// 获取从1星指定星级的符文石消耗
	/// </summary>
	public List<PrizeSample> getStoneConsume(int sid,int level){
        EquipStarSample sample = GetEquipStarSampleBySid(sid);
        if (sample == null)
            return null;
		List<PrizeSample> pList = new List<PrizeSample> ();
		for (int i=0; i<level; i++) {
            PrizeSample tmp = new PrizeSample(PrizeType.PRIZE_PROP, sample.equipStarConsumeStoneSids[i], sample.equipStarConsumeStoneValues[i]);
			pList.Add(tmp);
		}
		return pList;
	}

    public List<PrizeSample> getMoneyConsume(int sid, int level)
    {
        EquipStarSample sample = GetEquipStarSampleBySid(sid);
        if (sample == null)
            return null;
        List<PrizeSample> pList = new List<PrizeSample>();
        for (int i = 0; i < level; i++) {
            PrizeSample tmp = new PrizeSample(PrizeType.PRIZE_MONEY, 0, sample.redEquipStarConsumeMoneyValues[i]);
            pList.Add(tmp);
        }
        return pList;
    }
}
