using System;
using System.Collections.Generic;

/**
  *引导配置解析文件
  *@author 汤琦
  **/
public class GuideConfigManager:ConfigManager
{
	//单例
	private static GuideConfigManager instance;
	public List<int> guideSids;//步骤sid集合 
	public List<int> gotoSids;//跳转sid集合 如果不需要跳转 则为0  需要 则为跳转后的步骤编号
	public List<int> plotSids;//剧情sid
	public List<GuideCondition[]> conditions;//条件
	public List<string> infos;//提示框内容
	
	public static GuideConfigManager Instance {
		get {
			if (instance == null)
				instance = new GuideConfigManager ();
			return instance;
		}
	}
	
	public GuideConfigManager ()
	{   
		base.readConfig (ConfigGlobal.CONFIG_GUIDE);
	}
	
	public override void parseConfig (string str)
	{
		string[] arr = str.Split ('|'); 
		int max = arr.Length;
		if (guideSids == null)
			guideSids = new List<int> ();
		if (gotoSids == null)
			gotoSids = new List<int> ();
		if (plotSids == null)
			plotSids = new List<int> ();
		if (infos == null)
			infos = new List<string> (); 
		guideSids.Add (StringKit.toInt (arr [0]));
		gotoSids.Add (StringKit.toInt (arr [1]));
		plotSids.Add (StringKit.toInt (arr [2]));
		parseCondition (arr [3]);
		infos.Add (arr [4]); 
	}

	private void parseCondition (string str)
	{
		if (conditions == null)
			conditions = new List<GuideCondition[]> ();
		string[] strs = str.Split ('#');
		if (strs.Length <= 1 && strs [0] == "0") {
			conditions.Add (null); 
			return;
		}
		GuideCondition[] temp = new GuideCondition[strs.Length];
		for (int i = 0; i < strs.Length; i++) {
			string[] strss = strs [i].Split (',');
			int num = strss.Length == 3 ? StringKit.toInt (strss[2]) : 0;
			temp [i] = new GuideCondition (strss [0], strss [1], num);
		}
		conditions.Add (temp);
	}

	/// <summary>
	/// 获取所有新手步骤
	/// </summary>
	/// <returns>The guide sids.</returns>
	public List<int> getGuideSids ()
	{
		return guideSids;
	}

	/// <summary>
	/// 获取所有新手条件
	/// </summary>
	/// <returns>The guide conditions.</returns>
	public List<GuideCondition[]> getGuideConditions ()
	{
		return conditions;
	}

	/** 根据指引Sid获得对应索引 */
	public int getIndexByGuideSid (int guideSid)
	{
		for (int i = 0; i < guideSids.Count; i++) {
			if (guideSids [i] == guideSid) {
				return i;
			}
		}
		return 0;
	}

	/** 获得跳转Sid */
	public int getGotoSid (int guideSid)
	{
		if (guideSid == 0)
			return gotoSids [0];
		for (int i = 0; i < guideSids.Count; i++) {
			if (guideSids [i] == guideSid) {
				return gotoSids [i];
			}
		}
		return guideSid;
	}

	/** 根据指引sid，获得相应条件 */
	public GuideCondition[] getConditionByGuideSid (int guideIndex)
	{
		return conditions [guideIndex];
	}
}

/** 指引条件 */
public class GuideCondition
{
	public GuideCondition (string conditionType, string conditionValue, int conditionNum)
	{
		this.conditionType = conditionType;
		this.conditionValue = conditionValue;
		this.conditionNum = conditionNum;
	}

	/** 条件类型 */
	public string conditionType;
	/** 需求内容 */
	public string conditionValue;
	/** 需求数量 */
	public int conditionNum;
}

/** 指引条件类型 */
public class GuideConditionType
{
	/** 等级 */
	public const string LEVEL = "lv";
	/** 所在窗口名 */
	public const string WINDOWBASE = "wb";
	/** 卡片Sid */
	public const string CARDSID = "cardSid";
	/** 装备Sid */
	public const string EQUIPSID = "equipSid";
	/** 道具Sid */
	public const string PROPSID = "propSid";
	/** 副本Sid */
	public const string MISSIONSID = "missionSid";
	/** 指引结束 */
	public const string OVER = "over";
	/** 上阵 */
	public const string BATTLE = "battle";
	/** 宝箱 */
	public const string BOX = "box";
}

