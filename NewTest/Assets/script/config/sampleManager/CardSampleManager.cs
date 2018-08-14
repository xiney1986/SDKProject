using System;
 
public class CardSampleManager:SampleConfigManager
{ 
	private const int APPEND_ATTACK = 100;
	private const int APPEND_HP = 300;
	private const int APPEND_DEFECSE = 100;
	private const int APPEND_MAGIC = 100;
	private const int APPEND_AGILE = 100;
    public const int ONESTAR = 1;
    public const int TWOSTAR = 2;
    public const int THREESTAR = 3;
    public const int FOURSTAR = 4;
    public const int USEDBYCARDITEM = 1;
    public const int USEDBYCARD = 2;
    public const int USEDINBATTLEPREPARE = 3;
    public const int USEDBYINTENSIFY = 4;
    public const int USEDBYSHOW = 5;
	public CardSampleManager ()
	{
		if (_singleton) {
			throw new Exception ("This is singleton!");
		}
		base.readConfig (ConfigGlobal.CONFIG_ROLE); 
	}
	
	//单例
	private static CardSampleManager _Instance;
	private static bool _singleton = true;
	
	public static CardSampleManager Instance {
		get { 
			if (_Instance == null) {
				_singleton = false;
				_Instance = new CardSampleManager ();
				_singleton = true;
				return _Instance;
			} else
				return _Instance;
		}
		set { 
			_Instance = value;
		}
	}
	 
	//获得角色模板对象
	public CardSample getRoleSampleBySid (int sid)
	{ 
		if (!isSampleExist (sid))
			createSample (sid); 
		return samples [sid] as CardSample;
	}  
	 
	 
	//获得角色模板基础属性值
	public int getBaseAttribute (int sid, AttributeType attr)
	{
		if (attr == AttributeType.attack) {
			return getRoleSampleBySid (sid).baseAttack;
		} else if (attr == AttributeType.hp) {
			return getRoleSampleBySid (sid).baseLife;
		} else if (attr == AttributeType.defecse) {
			return getRoleSampleBySid (sid).baseDefecse;
		} else if (attr == AttributeType.magic) {
			return getRoleSampleBySid (sid).baseMagic;
		} else if (attr == AttributeType.agile) {
			return getRoleSampleBySid (sid).baseAgile;
		} else {
			throw new Exception ("getBaseAttribute role attribute error! attr = " + attr);
		} 
	}

	// 获得星座女神的标题标识
	public string getGoddessNameSprite(int sid)
	{
		int index = (sid - 1800)%12;
		if (index == 0)
		{
			return "12";
		} else {
			return index.ToString();
		}

	}
	
	//获得角色属性等级成长值
	public int getLevelUpAttribute (int sid, AttributeType attr)
	{
		if (attr == AttributeType.attack) {
			return getRoleSampleBySid (sid).developAttack;
		} else if (attr == AttributeType.hp) {
			return getRoleSampleBySid (sid).developLife;
		} else if (attr == AttributeType.defecse) {
			return getRoleSampleBySid (sid).developDefecse;
		} else if (attr == AttributeType.magic) {
			return getRoleSampleBySid (sid).developMagic;
		} else if (attr == AttributeType.agile) {
			return getRoleSampleBySid (sid).developAgile;
		} else {
			throw new Exception ("getLevelUpAttribute role attribute error! attr = " + attr);
		}
	}
    /// <summary>
    /// 检测卡片是否拥有血脉
    /// </summary>
    /// <param name="sid"></param>
    /// <returns></returns>
    public bool checkBlood(int sid,string uid) {
        if (getRoleSampleBySid(sid) != null && getRoleSampleBySid(sid).bloodPointSid != 0) {
            return true;
        }
        return false;
    }
    /// <summary>
    /// 获取卡片的星级
    /// </summary>
    /// <param name="sid"></param>
    /// <returns></returns>
    public int getStarLevel(int sid) {
        if (getRoleSampleBySid(sid) != null) {
            return getRoleSampleBySid(sid).sFlagLevel;
        }
        return 0;
    }
   
	//获得角色属性附加成长值
	public int getAppendUpAttribute (AttributeType attr)
	{
	    int[] fu = CommandConfigManager.Instance.getFujiaoSuxi();
		if (attr == AttributeType.attack) {
			return fu[0];
		} else if (attr == AttributeType.hp) {
			return  fu[1];
		} else if (attr == AttributeType.defecse) {
			return fu[2];
		} else if (attr == AttributeType.magic) {
			return fu[3];
		} else if (attr == AttributeType.agile) {
			return fu[4];
		} else {
			throw new Exception ("getAppendUpAttribute role attribute error! attr = " + attr);
		}
	} 
	 
	//解析模板数据
	public override void parseSample (int sid)
	{
		CardSample sample = new CardSample (); 
		string dataStr = getSampleDataBySid (sid); 
		sample.parse (sid, dataStr); 
		samples.Add (sid, sample);
	}
    /// <summary>
    /// 取得血脉节点sid
    /// </summary>
    /// <param name="sid"></param>
    /// <returns></returns>
    public int getBloodSid(int sid) {
        if (getRoleSampleBySid(sid) != null) {
            return getRoleSampleBySid(sid).bloodPointSid;
        }
        return 0;
    }
    /// <summary>
    /// 通过指导的进阶等级拿条件配置
    /// </summary>
    /// <param name="lv"></param>
    /// <returns></returns>
    //public PrizeSample[] getConditionByLv(int sid, int lv) {
    //    BloodPointSample bps = BloodConfigManager.Instance.getBloodPointSampleBySid(getBloodSid(sid));
    //    if (bps == null) return null;
    //    return bps.conditions[lv>=bps.conditions.Length?bps.conditions.Length-1:lv];
    //}
}

//基础属性枚举
public enum AttributeType
{
	attack,//攻击力
	hp,//生命值
	defecse,//防御力
	defense,
	magic,//魔力
	agile,//敏捷值
}
