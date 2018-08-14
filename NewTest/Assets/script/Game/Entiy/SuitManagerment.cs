using System;
using System.Collections.Generic;
 
/**
 * 套装属性管理器
 * */
public class SuitManagerment
{ 
	public const int NOT_SUIT = 0;//套装id为0的装备 为单件

	public SuitManagerment ()
	{ 

	}
	 
	public static SuitManagerment Instance {
		get{return SingleManager.Instance.getObj("SuitManagerment") as SuitManagerment;}
	} 
	
	//获得装备套装属性属性变化信息
	public SuitAttrChange[] getEquipsSuitAttrChanges (string[] equips,int starLevel)
	{
		SuitInfo[] infos = getSuitInfos (equips,starLevel); 
		if (infos == null || infos.Length < 1)
			return null;
		List<SuitAttrChange> list = new List<SuitAttrChange> ();
		for (int i = 0; i < infos.Length; i++) {
			ListKit.AddRange(list,getSuitAttrChangeBySuitInfo (infos [i]));
		}
		return list.ToArray ();	
	}

	//获得套装属性比例加成
	public CardBaseAttribute getSuitBaseAttrByPer (string[] equips, int starLevel)
	{
		CardBaseAttribute attr = new CardBaseAttribute ();
		SuitAttrChange[] suitAttrs = getEquipsSuitAttrChanges (equips,starLevel); 
		if (suitAttrs == null || suitAttrs.Length < 1)
			return attr; 
		
		for (int i = 0; i < suitAttrs.Length; i++) { 
			AttrChangeSample[] effects = suitAttrs [i].effects;
			if (effects == null || effects.Length < 1)
				return attr;
			//套装属性不受等级影响 取第一级
			for (int j = 0; j < effects.Length; j++) {

				if (effects [j].getAttrType () == AttrChangeType.PER_HP) {
					attr.perHp += effects [j].getAttrValue (1);
				} else if (effects [j].getAttrType () == AttrChangeType.PER_ATTACK) {
					attr.perAttack += effects [j].getAttrValue (1);
				} else if (effects [j].getAttrType () == AttrChangeType.PER_DEFENSE) {
					attr.perDefecse += effects [j].getAttrValue (1);
				} else if (effects [j].getAttrType () == AttrChangeType.PER_MAGIC) {
					attr.perMagic += effects [j].getAttrValue (1);
				} else if (effects [j].getAttrType () == AttrChangeType.PER_AGILE) {
					attr.perAgile += effects [j].getAttrValue (1);
				}
			} 
		}
		return attr;
	}
	
	
	//获得套装基础属性影响值
	public CardBaseAttribute getSuitBaseAttrChange (string[] equips, int starLevel)
	{
		CardBaseAttribute attr = new CardBaseAttribute ();
		SuitAttrChange[] suitAttrs = getEquipsSuitAttrChanges (equips,starLevel); 
		if (suitAttrs == null || suitAttrs.Length < 1)
			return attr; 
		
		for (int i = 0; i < suitAttrs.Length; i++) { 
			AttrChangeSample[] effects = suitAttrs [i].effects;
			if (effects == null || effects.Length < 1)
				return attr;
			//套装属性不受等级影响 取第一级
			for (int j = 0; j < effects.Length; j++) {
				if (effects [j].getAttrType () == AttrChangeType.HP) {
					attr.hp += effects [j].getAttrValue (1); 
				} else if (effects [j].getAttrType () == AttrChangeType.ATTACK) {
					attr.attack += effects [j].getAttrValue (1);
				} else if (effects [j].getAttrType () == AttrChangeType.DEFENSE) {
					attr.defecse += effects [j].getAttrValue (1);
				} else if (effects [j].getAttrType () == AttrChangeType.MAGIC) {
					attr.magic += effects [j].getAttrValue (1);
				} else if (effects [j].getAttrType () == AttrChangeType.AGILE) {
					attr.agile += effects [j].getAttrValue (1);
//				} else if (effects [j].getAttrType () == AttrChangeType.PER_HP) {
//					attr.perHp += effects [j].getAttrValue (1);
//				} else if (effects [j].getAttrType () == AttrChangeType.PER_ATTACK) {
//					attr.perAttack += effects [j].getAttrValue (1);
//				} else if (effects [j].getAttrType () == AttrChangeType.PER_DEFENSE) {
//					attr.perDefecse += effects [j].getAttrValue (1);
//				} else if (effects [j].getAttrType () == AttrChangeType.PER_MAGIC) {
//					attr.perMagic += effects [j].getAttrValue (1);
//				} else if (effects [j].getAttrType () == AttrChangeType.PER_AGILE) {
//					attr.perAgile += effects [j].getAttrValue (1);
				}
			} 
		}
		return attr;
	}
	
	//获得套装对应属性
	private List<SuitAttrChange> getSuitAttrChangeBySuitInfo (SuitInfo info)
	{
		if (info == null) {
			return null;
		}
		SuitSample sample = SuitSampleManager.Instance.getSuitSampleBySid (info.suitSid);
		if (sample == null) {
			return null;
		}
		List<SuitAttrChange> list = new List<SuitAttrChange> ();
		for (int i = 0; i < sample.infos.Length; i++) {
			if (sample.infos [i].num <= info.ids.Count) {
				list.Add (sample.infos [i]); 
			}
		}
		return list;
	}
	
	//获得套装信息
	public SuitInfo[] getSuitInfos (string[] equips, int starLevel)
	{
		if (equips == null || equips.Length < 1)
			return null; 
		List<SuitInfo> infos = new List<SuitInfo> ();
		for (int i = 0; i < equips.Length; i++) {
			addSuitInfo (infos, equips [i],starLevel);
		}
		return infos.ToArray ();
	}
	
	//添加套装信息
	private void addSuitInfo (List<SuitInfo> infos, string equip_id, int starLevel)
	{ 
		int suitSid = getEquipSuitSid (equip_id);
		if (suitSid == NOT_SUIT)
			return;
		/* fix bug 2014.10.23
		 * 重新匹配一次装备星级，因为之前是5件一起算，如果是混穿，会导致低级品质套装也会算上星级
		 * 如果发现品质不是橙色以上，都直接重新把星级重置为0
		 */
		starLevel = getEquipQualityId(equip_id) >= QualityType.LEGEND ? starLevel : 0;
		for (int i = 0; i < infos.Count; i++) {
			if (infos [i].suitSid == getSuitSidByStarLv (getEquipSuitSid (equip_id),starLevel)) {
				infos [i].ids.Add (equip_id);
				return;
			}
		}
		SuitInfo info = new SuitInfo (getEquipSuitSid (equip_id),starLevel); 
		info.addEquipId (equip_id); 
		infos.Add (info);
	}

	/// <summary>
	/// 根据星级获得对应星级套装Sid
	/// </summary>
	private int getSuitSidByStarLv (int suitSid, int equipStarLevel)
	{
		if (equipStarLevel > 0) {
			return suitSid * 100 + equipStarLevel;
		}
		else {
			return suitSid;
		}
	}
	
	//获得装备套装sid
	public int getEquipSuitSid (string id)
	{
		return StorageManagerment.Instance.getEquip (id).getSuitSid ();
	}

	public int getEquipQualityId (string id)
	{
		return StorageManagerment.Instance.getEquip (id).getQualityId ();
	}
	
	//获得套装对应信息描述
	public string getSuitDescribe (SuitSample suit, int num)
	{
		
		foreach (SuitAttrChange info in suit.infos) {
			if (info.num == num) {
				return DescribeManagerment.getDescribe (info.describe, 1, info.effects); 
			}
		}
		return "";
		
	} 
	//获得套装对应信息描述
	public string getSuitDescribe (SuitAttrChange suit) {
		return DescribeManagerment.getDescribe (suit.describe, 1, suit.effects); 
	} 
	 
} 

//套装信息 套装sid 套装收集数量
public class SuitInfo
{
	public int suitSid;//套装sid
	public List<string> ids;

	public SuitInfo (int suitSid, int equipStarLevel)
	{
		if (equipStarLevel > 0) {
			this.suitSid = suitSid * 100 + equipStarLevel;
		}
		else {
			this.suitSid = suitSid;
		}
	}
	
	public void addEquipId (string id)
	{
		if (ids == null) {
			ids = new List<string> ();
		}
		ids.Add (id);
	}
	
}
