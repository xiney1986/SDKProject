using UnityEngine;
using System.Collections;

/// <summary>
/// 坐骑常规相关配置
/// </summary>
public class MountsConfigManager : ConfigManager {

	/* static fields */
	private static MountsConfigManager instance;

	/* static methods */
	public static MountsConfigManager Instance {
		get {
			if (instance == null)
				instance = new MountsConfigManager ();
			return instance;
		}
	}

	/* fields */
	/** 储备行动力恢复速度 */
	private int pveSpeed;
	/** 初始储备行动力上限 */
	private int initPveMax;
	/** 每激活一个骑宠储备行动力上限+X */
	private int addPveConut;
	/** 每激活一个骑宠所有骑宠的属性+X% */
	private int addAttrPer;
	/** 储备行动力开放等级 */
	private int addPveOpenLv;
	/** 共鸣开放等级 */
	private int attrPerOpenLv;
	/** 骑术升级消耗Sid组 */
	private int[] itemSids;

	/* methods */
	public MountsConfigManager () {
		base.readConfig (ConfigGlobal.CONFIG_MOUNTS_CONFIG);
	}
	public override void parseConfig (string str) {
		string[] strs = str.Split ('|');
		// str[0] 配置文件说明
		pveSpeed = StringKit.toInt (strs [1]);
		initPveMax = StringKit.toInt (strs [2]);
		addPveConut = StringKit.toInt (strs [3]);
		addAttrPer = StringKit.toInt (strs [4]);
		addPveOpenLv = StringKit.toInt (strs [5]);
		attrPerOpenLv = StringKit.toInt (strs [6]);
		parseInts (strs [7]);
	}

	private void parseInts (string str)
	{
		string[] strs = str.Split ('#');
		itemSids = new int[strs.Length];
		for (int i = 0; i < strs.Length; i++) {
			itemSids[i] =  StringKit.toInt (strs [i]);
		}
	}
	
	/// <summary>
	/// 获得骑术升级消耗的sid组
	/// </summary>
	public int[] getItemSids ()
	{
		return itemSids;
	}

	/// <summary>
	/// 获得储备行动力恢复速度（毫秒）
	/// </summary>
	public int getPveSpeed () {

		// 测试代码
		return 450* 1000;
		// 测试代码

//		return pveSpeed * 1000;
	}
	/// <summary>
	/// 获得当前拥有骑宠共鸣百分比数值
	/// </summary>
	public float getAttrPerByOwn () {
		if (MountsManagerment.Instance.getMountsLevel () < getAttrPerOpenLv ()) {
			return 1;
		} else {
			return 1 + MountsManagerment.Instance.getAllMountsCount () * addAttrPer * 0.0001f;
		}
	}
	/// <summary>
	/// 获得每个坐骑共鸣提高的百分比数值[描述]
	/// </summary>
	public string getAddAttrPer () {
		return (addAttrPer * 0.0001f) * 100 + "%";
	}
	/// <summary>
	/// 获得当前拥有骑宠共鸣百分比数值[描述]
	/// </summary>
	public string getAttrPerByString () {
		return (MountsManagerment.Instance.getAllMountsCount () * addAttrPer * 0.0001f) * 100 + "%";
	}

	/* properties */
	/// <summary>
	/// 获得储备行动力开放等级
	/// </summary>
	public int getAddPveOpenLv () {
		return addPveOpenLv;
	}
	/// <summary>
	/// 获得共鸣开放等级
	/// </summary>
	public int getAttrPerOpenLv () {
		return attrPerOpenLv;
	}
	/// <summary>
	/// 获取坐骑存储初始上限
	/// </summary>
	public int getInitPveMax() {
		return initPveMax;
	}
	/// <summary>
	/// 每获得一个坐骑上限增加值
	/// </summary>
	/// <returns>The add pve conut.</returns>
	public int getAddPveConut() {
		return addPveConut;
	}
}