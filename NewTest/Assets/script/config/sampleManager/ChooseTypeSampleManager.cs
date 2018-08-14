using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChooseTypeSampleManager : SampleConfigManager {

	private static ChooseTypeSampleManager instance;
	private  List<ChooseTypeSample> chooseTyepInfo;//类型(1技能经验，2装备经验，3游戏币，4附加属性)
	/** 技能经验 */
	public const int TYPE_SKILL_EXP = 1;
	/** 装备经验 */
	public const int TYPE_EQUIP_EXP = 2;
	/** 游戏币 */
	public const int TYPE_MONEY_NUM = 3;
	/** 附加属性 */
	public const int TYPE_ADDON_NUM = 4;

	public static ChooseTypeSampleManager Instance {
		get{
			if(instance==null)
				instance=new ChooseTypeSampleManager();
			return instance;
		}
	}
	
	public ChooseTypeSampleManager ()
	{
		base.readConfig (ConfigGlobal.CONFIG_CHOOSETYPE);
	}

	//解析配置
	public override void parseConfig (string str)
	{  
		ChooseTypeSample be = new ChooseTypeSample (str);
		if(chooseTyepInfo == null)
			chooseTyepInfo = new List<ChooseTypeSample>();
		chooseTyepInfo.Add(be);
	}

	/** 获得指定类型的SID组合 */
	public int[] getSidsByType(int type)
	{
		for(int i = 0;i<chooseTyepInfo.Count;i++)
		{
			if(chooseTyepInfo[i].getType() == type)
				return chooseTyepInfo[i].getSids();
		}
		return null;
	}

	/** 这装备是不是有特殊用途 */
	public bool isToEat(Equip equip,int type)
	{
		int[] a = getSidsByType(type);
		
		if(a != null) {
			for(int i = 0;i<a.Length;i++) {
				if(a[i] == equip.sid)
					return true;
			}
		}
		else
			return false;
		return false;
	}

	/** 这卡片是不是有特殊用途 */
	public bool isToEat(Card card) {
		return isToEat (card, ChooseTypeSampleManager.TYPE_MONEY_NUM) || isToEat (card, ChooseTypeSampleManager.TYPE_ADDON_NUM) || isToEat (card, ChooseTypeSampleManager.TYPE_SKILL_EXP);
	}

	/** 这卡片是不是有特殊用途 */
	public bool isToEat(Card card,int type)
	{
		int[] a = getSidsByType(type);
		if(a != null) {
			for(int i = 0;i<a.Length;i++) {
				if(a[i] == card.sid)
					return true;
			}
		}
		else
			return false;
		return false;
	}

	/** 这卡片是不是有特殊用途 */
	public bool isToEat(StarSoul starSoul) {
		return isToEat (starSoul, ChooseTypeSampleManager.TYPE_MONEY_NUM) || isToEat (starSoul, ChooseTypeSampleManager.TYPE_ADDON_NUM) || isToEat (starSoul, ChooseTypeSampleManager.TYPE_SKILL_EXP);
	}

	/** 这卡片是不是有特殊用途 */
	public bool isToEat(StarSoul starSoul,int type) {
		int[] a = getSidsByType(type);
		if(a != null) {
			for(int i = 0;i<a.Length;i++) {
				if(a[i] == starSoul.sid)
					return true;
			}
		}
		else
			return false;
		return false;
	}
}
