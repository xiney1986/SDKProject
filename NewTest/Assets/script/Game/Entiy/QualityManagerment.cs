using System;
 
/**
 * 品质管理器
 * @author longlingquan 
 * */
public class QualityManagerment
{
	public QualityManagerment ()
	{
	}
	
	/** 获得品质对应的中文描述 包含颜色 */
	public static string getQualityName (int quality)
	{
		if (quality == QualityType.COMMON) {
			return getQualityColor(quality) + LanguageConfigManager.Instance.getLanguage ("s0073");
		} else if (quality == QualityType.EXCELLENT) {
			return getQualityColor(quality) + LanguageConfigManager.Instance.getLanguage ("s0074");
		} else if (quality == QualityType.GOOD) {
			return getQualityColor(quality) + LanguageConfigManager.Instance.getLanguage ("s0075");
		} else if (quality == QualityType.EPIC) {
			return getQualityColor(quality) + LanguageConfigManager.Instance.getLanguage ("s0076");
		} else if (quality == QualityType.LEGEND) {
			return getQualityColor(quality) + LanguageConfigManager.Instance.getLanguage ("s0077");
        } else if (quality == QualityType.MYTH) {
            return getQualityColor(quality) + LanguageConfigManager.Instance.getLanguage("s0077ss");
        }
		return "";
	}

	/** 获得品质对应的中文描述 不包含颜色 */
	public static string getQualityNameByNone (int quality)
	{
		if (quality == QualityType.COMMON) {
			return LanguageConfigManager.Instance.getLanguage ("s0073");
		} else if (quality == QualityType.EXCELLENT) {
			return LanguageConfigManager.Instance.getLanguage ("s0074");
		} else if (quality == QualityType.GOOD) {
			return LanguageConfigManager.Instance.getLanguage ("s0075");
		} else if (quality == QualityType.EPIC) {
			return LanguageConfigManager.Instance.getLanguage ("s0076");
		} else if (quality == QualityType.LEGEND) {
			return LanguageConfigManager.Instance.getLanguage ("s0077");
        } else if (quality == QualityType.MYTH) {
            return LanguageConfigManager.Instance.getLanguage("s0077ss");
        }
		return "";
	}

	/** 获取卡片品质标签 */
	public static  string qualityIDToString (int id)
	{ 
		switch (id) {
		case QualityType.COMMON:
			return "quality_1";
		case QualityType.EXCELLENT:
			return "quality_2";
		case QualityType.GOOD:
			return "quality_3";
		case QualityType.EPIC:
			return "quality_4";
		case QualityType.LEGEND:
			return "quality_5";
        case QualityType.MYTH:
            return "quality_6";
		} 
		return "quality_1";
	}

	/** 获取卡片品质标签 */
	public static  string qualityIDToStringByBG (int id)
	{ 
		switch (id) {
		case QualityType.COMMON:
			return "quality_1Bg";
		case QualityType.EXCELLENT:
			return "quality_2Bg";
		case QualityType.GOOD:
			return "quality_3Bg";
		case QualityType.EPIC:
			return "quality_4Bg";
		case QualityType.LEGEND:
			return "quality_5Bg";
        case QualityType.MYTH:
            return "quality_6Bg";
		} 
		return "quality_1Bg";
	}

	/** 获取物品品质背景 */
	public static  string qualityIDToIconSpriteName (int id)
	{	
		switch (id) {
		case QualityType.COMMON:
			return "qualityIconBack_1";
		case QualityType.EXCELLENT:
			return "qualityIconBack_2";
		case QualityType.GOOD:
			return "qualityIconBack_3";
		case QualityType.EPIC:
			return "qualityIconBack_4";
		case QualityType.LEGEND:
			return "qualityIconBack_5";
        case QualityType.MYTH:
            return "qualityIconBack_6";
		} 
		return "qualityIconBack_1";
	}
    /// <summary>
    /// 神器的配色
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static string qualityIDtoMagicWeapon(int id) {
        switch (id) {
            case QualityType.COMMON:
                return "magic_1";
            case QualityType.EXCELLENT:
                return "magic_1";
            case QualityType.GOOD:
                return "magic_2";
            case QualityType.EPIC:
                return "magic_3";
            case QualityType.LEGEND:
                return "magic_4";
        }
        return "magic_1";
    }

	/** 获取卡片品质背景 */
	public static string qualityIDToBackGround (int id)
	{ 
		switch (id) {
		case QualityType.COMMON:
			return "roleBack_0";
		case QualityType.EXCELLENT:
			return "roleBack_1";
		case QualityType.GOOD:
			return "roleBack_2";
		case QualityType.EPIC:
			return "roleBack_3";
		case QualityType.LEGEND:
			return "roleBack_4";
        case QualityType.MYTH:
            return "roleBack_5";
		} 
		return "roleBack_0";
	}
	/** 获取卡片品质图标背景 */
	public static string qualityIconBgToBackGround (int id)
	{ 
		switch (id) {
		case QualityType.COMMON:
			return "joball_0";
		case QualityType.EXCELLENT:
			return "joball_1";
		case QualityType.GOOD:
			return "jobBall_2";
		case QualityType.EPIC:
			return "jobBall_3";
		case QualityType.LEGEND:
			return "joball_4";
        case QualityType.MYTH:
            return "joball_5";
		} 
		return "roleBack_0";
	}
	/** 获取卡片品质背景边框 */
	public static string qualityBianToBackGround (int id)
	{ 
		switch (id) {
		case QualityType.COMMON:
			return "roleBottom_0";
		case QualityType.EXCELLENT:
			return "roleBottom_1";
		case QualityType.GOOD:
			return "roleBottom_2";
		case QualityType.EPIC:
			return "roleBottom_3";
		case QualityType.LEGEND:
			return "roleBottom_4";
        case QualityType.MYTH:
            return "roleBottom_5";
		} 
		return "roleBottom_0";
	}
	/** 获得品质对应的品质特效路径 包含颜色 */
	public static string getQualityEffectPath (int quality)
	{
		if (quality == QualityType.COMMON) {
			return "Effect/UiEffect/BackgroundWhite";
		} else if (quality == QualityType.EXCELLENT) {
			return "Effect/UiEffect/BackgroundGreen";
		} else if (quality == QualityType.GOOD) {
			return "Effect/UiEffect/BackgroundBlue";
		} else if (quality == QualityType.EPIC) {
			return "Effect/UiEffect/BackgroundPurple";
		} else if (quality == QualityType.LEGEND) {
			return "Effect/UiEffect/BackgroundOrange";
        } else if (quality == QualityType.MYTH) {
            return "Effect/UiEffect/BackgroundOrange";
        }
		return "";
	}
	
	
	/** 获得品质对应的中文描述 包含颜色 */
	public static string getQualityColor (int quality)
	{
		if (quality == QualityType.COMMON) {
			return "[6B6B6B]";
		} else if (quality == QualityType.EXCELLENT) {
			return "[358C35]";
		} else if (quality == QualityType.GOOD) {
			return "[2787BA]";
		} else if (quality == QualityType.EPIC) {
			return "[8840AD]";
		} else if (quality == QualityType.LEGEND) {
			return "[C65D00]";
        } else if (quality == QualityType.MYTH) {
            return "[FF0000]";
        }
		return "";
	}

    public static string getQualityColorForBloodItem(int quality,int lenth)
    {
         if (quality == 0) {//精良
             return "[358C35]" + LanguageConfigManager.Instance.getLanguage("perfabzc29l01",lenth+"")+"[-]";
        } else if (quality == 1) {//优秀
            return "[2787BA]" + LanguageConfigManager.Instance.getLanguage("perfabzc29l02", lenth + "") + "[-]";
        } else if (quality == 2) {//史诗
            return "[8840AD]" + LanguageConfigManager.Instance.getLanguage("prefabzc29", lenth + "") + "[-]";
        } else if (quality == 3) {//传说
            return "[C65D00]" + LanguageConfigManager.Instance.getLanguage("prefabzc30", lenth + "") + "[-]";
        } else if (quality == 4) {//神话
            return "[FF0000]"+LanguageConfigManager.Instance.getLanguage("prefabzc31", lenth + "") + "[-]";
        }
        return "";
    }
    public static string getQualityColorForlv(int quality, int lenth) {
        if (quality == 0) {
            return "[358C35]" + LanguageConfigManager.Instance.getLanguage("perfabzc29l01", lenth+1 + "") + "[-]";
        } else if (quality == 1) {
            return "[2787BA]" + LanguageConfigManager.Instance.getLanguage("perfabzc29l02", lenth + "") + "[-]";
        } else if (quality == 2) {
            return "[8840AD]" + LanguageConfigManager.Instance.getLanguage("prefabzc29", lenth + "") + "[-]";
        } else if (quality == 3) {
            return "[C65D00]" + LanguageConfigManager.Instance.getLanguage("prefabzc30", lenth + "") + "[-]";
        } else if (quality == 4) {
            return "[FF0000]" + LanguageConfigManager.Instance.getLanguage("prefabzc31", lenth + "") + "[-]";
        }
        return "";
    }
    public static string getQualityColorForlv(int quality) {
        if (quality == 0) {
            return "[358C35]" + LanguageConfigManager.Instance.getLanguage("s0074") + "[-]";
        } else if (quality == 1) {
            return "[2787BA]" + LanguageConfigManager.Instance.getLanguage("s0075") + "[-]";
        } else if (quality == 2) {
            return "[8840AD]" + LanguageConfigManager.Instance.getLanguage("s0076") + "[-]";
        } else if (quality == 3) {
            return "[C65D00]" + LanguageConfigManager.Instance.getLanguage("s0077") + "[-]";
        } else if (quality == 4) {
            return "[FF0000]" + LanguageConfigManager.Instance.getLanguage("s0077ss") + "[-]";
        }
        return "";
    }

    public static string getdecForBloodItem(int quality)
    {
        if (quality == 0) {
            return "[2787BA]" + LanguageConfigManager.Instance.getLanguage("prefabzc32l1")+"[-]";
        } else if (quality == 1) {
            return "[8840AD]" + LanguageConfigManager.Instance.getLanguage("prefabzc32l2") + "[-]";
        } else if (quality == 2) {
            return "[C65D00]" + LanguageConfigManager.Instance.getLanguage("prefabzc32") + "[-]";
        } else if (quality == 3) {
            return "[FF0000]" + LanguageConfigManager.Instance.getLanguage("prefabzc33") + "[-]";
        } else if (quality == 4) {
            return "[FF0000]" + LanguageConfigManager.Instance.getLanguage("prefabzc33") + "[-]";
        }
        return "";
    }
    public static string getColorForBloodItem(int quality) {
        if (quality == 0) {
            return "[2787BA]"+LanguageConfigManager.Instance.getLanguage("s0075");
        } else if (quality == 1) {
            return "[8840AD]"+LanguageConfigManager.Instance.getLanguage("s0076");
        } else if (quality == 2) {
            return "[C65D00]"+LanguageConfigManager.Instance.getLanguage("s0077");
        } else if (quality == 3) {
            return "[FF0000]"+LanguageConfigManager.Instance.getLanguage("s0077ss");
        } else if (quality == 4) {
            return "[FF0000]"+LanguageConfigManager.Instance.getLanguage("s0077ss");
        }
        return "";
    }
	
} 

