using UnityEngine;
using System.Collections;

/**
 * 坐骑模板
 **/
public class MountsSample : Sample {

	public MountsSample ()
	{
		
	}

	/** 坐骑名字 */
	public string name = "";
	/** 图标编号 */
	public int imageID = 0;
	/** 3d形象 */
	public string modelID;
	/** 品质编号 */
	public int qualityId = 0;
	/** 最高等级 */
	public int maxLevel = 0;
	/** 移动速度 */
	public int speed = 0;

	/** 初始生命值 */
	public int baseLife = 0;
	/** 初始攻击值 */
	public int baseAttack = 0;
	/** 初始防御值 */
	public int baseDefecse = 0;
	/** 初始魔力值 */
	public int baseMagic = 0;
	/** 初始敏捷值 */
	public int baseAgile = 0;

	/** 生命成长值 */
	public int developLife = 0;
	/** 攻击成长值 */
	public int developAttack = 0;
	/** 防御成长值 */
	public int developDefecse = 0;
	/** 魔力成长值 */
	public int developMagic = 0;
	/** 敏捷成长值 */
	public int developAgile = 0;

	/** 技能 */
	public int[] skills;
	public int sortIndex;
	public bool isShowTime;//是否显示时间

	public override void parse (int sid, string str)
	{
		this.sid = sid;
		string[] strArr = str.Split ('|');
		this.name = strArr [1];
		this.imageID = StringKit.toInt (strArr [2]);
		this.modelID = strArr [3];
		this.qualityId = StringKit.toInt (strArr [4]);
		this.maxLevel = StringKit.toInt (strArr [5]);
		this.speed = StringKit.toInt (strArr [6]);
		
		this.baseLife = StringKit.toInt (strArr [7]);
		this.baseAttack = StringKit.toInt (strArr [8]);
		this.baseDefecse = StringKit.toInt (strArr [9]);
		this.baseMagic = StringKit.toInt (strArr [10]);
		this.baseAgile = StringKit.toInt (strArr [11]);

		this.developLife = StringKit.toInt (strArr [12]);
		this.developAttack = StringKit.toInt (strArr [13]);
		this.developDefecse = StringKit.toInt (strArr [14]);
		this.developMagic = StringKit.toInt (strArr [15]);
		this.developAgile = StringKit.toInt (strArr [16]);
		parseSkills (strArr [17]);
		sortIndex=StringKit.toInt(strArr[18]);
		isShowTime=StringKit.toInt(strArr[19])==1?true:false;
	}

	//解析默认技能配置
	void parseSkills (string str)
	{
		string[] strArr = str.Split ('#');
		
		skills = new int[strArr.Length];
		for (int i = 0; i < strArr.Length; i++) {
			if (strArr [i] == "0") {
				skills = null;
				break;
			}
			skills [i] = StringKit.toInt (strArr [i]);
		}
	}
}
