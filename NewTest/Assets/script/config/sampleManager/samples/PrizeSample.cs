using System;
 
/**
 * 奖励模板对象
 * 不是标准模板 无sid
 * @author longlingquan
 * */
public class PrizeSample:CloneObject
{
	public PrizeSample ()
	{
		
	}

	public PrizeSample (string str, char r)
	{
		parse (str, r);
	}
	
	public PrizeSample (int type, int sid, string num)
	{
		this.type = type;
		this.pSid = sid;
		this.num = num;
	}

	public PrizeSample (int type, int sid, int num)
	{
		this.type = type;
		this.pSid = sid;
		this.num = num.ToString ();
	}
    public PrizeSample(int type, int sid, int num,int index) {
        this.type = type;
        this.pSid = sid;
        this.num = num.ToString();
        this.index = index;
    }

    public PrizeSample(int type, string dec)
    {
        this.type = type;
        this.prizeDec = dec;
    }
	public PrizeSample (int type, int sid, long num)
	{
		this.type = type;
		this.pSid = sid;
		this.num = num.ToString ();
	}

	public int type;//奖励类型prizeType
	public int pSid;//奖励品sid rmb等为0 无sid
	public string num = "1";//数量
	public int time=0;
    public int index;
    public string prizeDec;//奖品的描述
	
	private void parse (string str, char r)
	{
		if (str == 0 + "")
			return;
		string[] strArr = str.Split (r);
		type = typeChange (strArr [0]);
		pSid = StringKit.toInt (strArr [1]);
		num = strArr [2];
		if(type==PrizeType.PRIZE_MOUNT&&strArr.Length>3)time=StringKit.toInt(strArr[3]);
	}

	public void addNum (int add) {
		this.num = (getPrizeNumByInt () + add).ToString();
	}

	/** 获得奖品数量Int */
	public int getPrizeNumByInt () {
		return StringKit.toInt (this.num);
	}
	/** 获得奖品数量Long */
	public long getPrizeNumByLong () {
		return StringKit.toLong (this.num);
	}
	/** 获得奖品数量String */
	public string getPrizeNumByString () {
		return this.num;
	}
	
	private int typeChange (string str)
	{
		//后台都是字符串标识
		//前台配置的是数字,\
		//所以需要转换一道
		switch (str) {
		case "goods":
			return 3;
		case "equipment":
			return 4;
		case "card":
			return 5;
		case "beast":
			return 6;
		case "equip":
			return 4;
		case "prop":
			return 3;
		case "money":
			return 1;
		case "rmb":
			return 2;
		case TempPropType.STARSOUL:
			return PrizeType.PRIZE_STARSOUL;
		case TempPropType.STARSOUL_DEBRIS:
			return PrizeType.PRIZE_STARSOUL_DEBRIS;
		case TempPropType.MOUNT:
			return PrizeType.PRIZE_MOUNT;
            case "artifact":
            return 21;
		default:
			return StringKit.toInt (str);
				
		}
	}

	public string  getPrizeName ()
	{
		if (type == PrizeType.PRIZE_CARD) { 
			return  CardSampleManager.Instance.getRoleSampleBySid (pSid).name; 
		} else if (type == PrizeType.PRIZE_EQUIPMENT) { 
			return   EquipmentSampleManager.Instance.getEquipSampleBySid (pSid).name; 
		} else if (type == PrizeType.PRIZE_PROP) {
			return   PropSampleManager .Instance.getPropSampleBySid (pSid).name;
		} else if (type == PrizeType.PRIZE_MONEY) {
			return  LanguageConfigManager.Instance.getLanguage ("resources_money_name");
		} else if (type == PrizeType.PRIZE_RMB) {
			return  LanguageConfigManager.Instance.getLanguage ("resources_rmb_name");
		} else if (type == PrizeType.PRIZE_MERIT) {
			return  LanguageConfigManager.Instance.getLanguage ("resources_merit_name");
		} else if (type == PrizeType.PRIZE_EXP) {
			return  LanguageConfigManager.Instance.getLanguage ("resources_exp_name");
		} else if (type == PrizeType.PRIZE_HONOR) {
			return  LanguageConfigManager.Instance.getLanguage ("resources_honor_name");
		} else if (type == PrizeType.PRIZE_PRESTIGE) {
			return  LanguageConfigManager.Instance.getLanguage ("resources_prestige_name");
		} else if (type == PrizeType.PRIZE_STARSOUL_DEBRIS) {
			return LanguageConfigManager.Instance.getLanguage ("s0466");
		} else if (type == PrizeType.PRIZE_SHAKE_SCORE) {
			return LanguageConfigManager.Instance.getLanguage ("resources_shake_name");		
		} else if (type == PrizeType.PRIZE_STAR_SCORE) {
			return LanguageConfigManager.Instance.getLanguage ("resources_star_score_name");
		} else if (type == PrizeType.PRIZE_LEDDER_SCORE) {
			return LanguageConfigManager.Instance.getLanguage ("laddermoney");
		} else if (type == PrizeType.PRIZE_CONTRIBUTION) {
			return LanguageConfigManager.Instance.getLanguage ("resources_contribution_name");
		}else if(type==PrizeType.PRIZE_MOUNT){
			return MountsSampleManager.Instance.getMountsSampleBySid(pSid).name;
		}else if(type==PrizeType.PRIZE_MAGIC_WEAPON){
            return MagicWeaponSampleManager.Instance.getMagicWeaponSampleBySid(pSid).name;
        }
		return "";
	}

	public string  getPrizeDes ()
	{
		if (type == PrizeType.PRIZE_CARD) { 
			return  ""; 
		} else if (type == PrizeType.PRIZE_EQUIPMENT) { 
			return   EquipmentSampleManager.Instance.getEquipSampleBySid (pSid).desc; 
		} else if (type == PrizeType.PRIZE_PROP) {
			return   PropSampleManager .Instance.getPropSampleBySid (pSid).describe;
		} else if (type == PrizeType.PRIZE_MONEY) {
			return  LanguageConfigManager.Instance.getLanguage ("resources_money_des");
		} else if (type == PrizeType.PRIZE_RMB) {
			return  LanguageConfigManager.Instance.getLanguage ("resources_rmb_des");
		} else if (type == PrizeType.PRIZE_MERIT) {
			return  LanguageConfigManager.Instance.getLanguage ("resources_merit_des");
		} else if (type == PrizeType.PRIZE_EXP) {
			return  LanguageConfigManager.Instance.getLanguage ("resources_exp_des");
		} else if (type == PrizeType.PRIZE_HONOR) {
			return  LanguageConfigManager.Instance.getLanguage ("resources_honor_des");
		} else if (type == PrizeType.PRIZE_PRESTIGE){
			return LanguageConfigManager.Instance.getLanguage("resources_prestige_des");
		} else if (type == PrizeType.PRIZE_STARSOUL_DEBRIS){
			return LanguageConfigManager.Instance.getLanguage("s0466");
		}else if (type == PrizeType.PRIZE_SHAKE_SCORE) {
			return LanguageConfigManager.Instance.getLanguage ("resources_shake_des");
		}
		else if (type == PrizeType.PRIZE_STAR_SCORE) {
			return LanguageConfigManager.Instance.getLanguage ("resources_star_score_des");
		} else if (type == PrizeType.PRIZE_LEDDER_SCORE) {
			return LanguageConfigManager.Instance.getLanguage ("laddermoney");
		}
		else if (type == PrizeType.PRIZE_CONTRIBUTION) {
			return LanguageConfigManager.Instance.getLanguage ("resources_contribution_des");
		}else if(type==PrizeType.PRIZE_MAGIC_WEAPON){
            return MagicWeaponSampleManager.Instance.getMagicWeaponSampleBySid(pSid).desc;
        }
		return "";
	}
    public int getPrizeHadNum() {
        if (type == PrizeType.PRIZE_CARD) {
            return 0;
        } else if (type == PrizeType.PRIZE_EQUIPMENT) {
            return 0;
        } else if (type == PrizeType.PRIZE_PROP)
        {
            Prop ppp = StorageManagerment.Instance.getProp(pSid);
            if (ppp == null) return 0;
            return ppp.getNum();
        } else if (type == PrizeType.PRIZE_MONEY) {
            return UserManager.Instance.self.getMoney();
        } else if (type == PrizeType.PRIZE_RMB) {
            return UserManager.Instance.self.getRMB();
        } else if (type == PrizeType.PRIZE_MERIT) {
            return -1;
        } else if (type == PrizeType.PRIZE_EXP) {
            return -1;
        } else if (type == PrizeType.PRIZE_HONOR) {
            return -1;
        } else if (type == PrizeType.PRIZE_PRESTIGE) {
            return -1;
        } else if (type == PrizeType.PRIZE_STARSOUL_DEBRIS) {
            return -1;
        } else if (type == PrizeType.PRIZE_SHAKE_SCORE) {
            return -1;
        } else if (type == PrizeType.PRIZE_STAR_SCORE) {
            return -1;
        } else if (type == PrizeType.PRIZE_LEDDER_SCORE) {
            return -1;
        } else if (type == PrizeType.PRIZE_CONTRIBUTION) {
            return -1;
        } else if (type == PrizeType.PRIZE_MAGIC_WEAPON) {
            return 0;
        }
        return 0;
    }
	public string getIconPath ()
	{
		if (type == PrizeType.PRIZE_CARD || type == PrizeType.PRIZE_BEAST) { 
			return ResourcesManager.ICONIMAGEPATH + CardSampleManager.Instance.getRoleSampleBySid (pSid).iconID;//这里用卡片头像，不再用全身像
		} else if (type == PrizeType.PRIZE_EQUIPMENT) { 
			return  ResourcesManager.ICONIMAGEPATH + EquipmentSampleManager.Instance.getEquipSampleBySid (pSid).iconId;  
		} else if (type == PrizeType.PRIZE_PROP) {
			return   ResourcesManager.ICONIMAGEPATH + PropSampleManager.Instance.getPropSampleBySid (pSid).iconId;
		} else if (type == PrizeType.PRIZE_MONEY) {
			return constResourcesPath.MONEY_ICONPATH;
		} else if (type == PrizeType.PRIZE_RMB) {
			return  constResourcesPath.RMB_ICONPATH;
		} else if (type == PrizeType.PRIZE_MERIT) {
			return  constResourcesPath.MERIT_ICONPATH;
		} else if (type == PrizeType.PRIZE_HONOR) {
			return  constResourcesPath.HONOR_ICONPATH;
		} else if (type == PrizeType.PRIZE_EXP) {
			return  constResourcesPath.EXP_ICONPATH;
		} else if (type == PrizeType.PRIZE_PRESTIGE) {
			return constResourcesPath.PRESTIGE_ICONPATH;
		} else if (type == PrizeType.PRIZE_STARSOUL_DEBRIS) {
			return constResourcesPath.STARSOUL_DEBRIS_ICONPATH;
		} else if (type == PrizeType.PRIZE_SHAKE_SCORE) {
			return constResourcesPath.SHAKE_SCORE;
		} else if (type == PrizeType.PRIZE_LEDDER_SCORE) {
			return constResourcesPath.LADDER_ICONPATH;
		} else if (type == PrizeType.PRIZE_STAR_SCORE) {
			return constResourcesPath.STAR_SCORE;
		} else if (type == PrizeType.PRIZE_CONTRIBUTION) {
			return constResourcesPath.CONTRIBUTION;
		}else if(type==PrizeType.PRIZE_MOUNT){
			return ResourcesManager.ICONIMAGEPATH + MountsSampleManager.Instance.getMountsSampleBySid(pSid).imageID;
		}else if(type==PrizeType.PRIZE_MAGIC_WEAPON){
            return ResourcesManager.ICONIMAGEPATH + MagicWeaponSampleManager.Instance.getMagicWeaponSampleBySid(pSid).iconId;
        }
		return constResourcesPath.MONEY_ICONPATH;
	}

	public int getQuality ()
	{
		if (type == PrizeType.PRIZE_CARD || type == PrizeType.PRIZE_BEAST) { 
			return CardSampleManager.Instance.getRoleSampleBySid (pSid).qualityId;
		} else if (type == PrizeType.PRIZE_EQUIPMENT) { 
			return  EquipmentSampleManager.Instance.getEquipSampleBySid (pSid).qualityId;
		} else if (type == PrizeType.PRIZE_PROP) {
			return  PropSampleManager.Instance.getPropSampleBySid (pSid).qualityId;
		} else if (type == PrizeType.PRIZE_MONEY) {
			return  5;
		} else if (type == PrizeType.PRIZE_RMB) {
			return 5;
		} else if(type == PrizeType.PRIZE_PRESTIGE){
			return 5;
		} else if(type == PrizeType.PRIZE_MERIT){
			return 5;
		}else if(type==PrizeType.PRIZE_MOUNT){
			return MountsSampleManager.Instance.getMountsSampleBySid(pSid).qualityId;
		}else if(type==PrizeType.PRIZE_MAGIC_WEAPON){
            return MagicWeaponSampleManager.Instance.getMagicWeaponSampleBySid(pSid).qualityId;
        }
		return 1;
	}

	public int getQuality1()
	{
		if (type == PrizeType.PRIZE_CARD || type == PrizeType.PRIZE_BEAST)
		{
			return CardSampleManager.Instance.getRoleSampleBySid(pSid).qualityId;
		}
		else if (type == PrizeType.PRIZE_EQUIPMENT)
		{
			return EquipmentSampleManager.Instance.getEquipSampleBySid(pSid).qualityId;
		}
		else if (type == PrizeType.PRIZE_PROP)
		{
			return PropSampleManager.Instance.getPropSampleBySid(pSid).qualityId;
		}
		else if (type == PrizeType.PRIZE_MOUNT)
		{
			return MountsSampleManager.Instance.getMountsSampleBySid(pSid).qualityId;
		}
		else if (type == PrizeType.PRIZE_MAGIC_WEAPON)
		{
			return MagicWeaponSampleManager.Instance.getMagicWeaponSampleBySid(pSid).qualityId;
		}
		return 1;
	}

	public static string GetQualityName(int quality)
	{
		string str = "";
		switch (quality)
		{
			case QualityType.EXCELLENT:
				str = "绿色";
				break;
			case QualityType.GOOD:
				str = "蓝色";
				break;
			case QualityType.EPIC:
				str = "紫色";
				break;
			case QualityType.LEGEND:
				str = "金色";
				break;
			case QualityType.lENGDS:
				str = "红色";
				break;
		}
		return str;
	}

	public override void copy(object destObj)
	{
		base.copy(destObj);
	}

	public override string ToString()
	{
		return GetQualityName(getQuality1()) + getPrizeName() + "x" + num;
	}
} 

