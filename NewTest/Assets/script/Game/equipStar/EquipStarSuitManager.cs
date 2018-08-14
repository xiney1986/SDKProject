using System;
using System.Collections.Generic;

/**
 * 套装属性管理器
 * */
public class EquipStarSuitManager
{ 
	public const int NOT_SUIT = 0;//套装id为0的装备 为单件
	
	public EquipStarSuitManager ()
	{ 
		
	}
	public static EquipStarSuitManager Instance {
		get{return SingleManager.Instance.getObj("EquipStarSuitManager") as EquipStarSuitManager;}
	} 
	
	//获得装备套装属性属性变化信息
	public EquipStarSuitAttrChange[] getEquipsSuitAttrChanges (string[] equips)
	{
		EquipStarSuitInfo[] infos = getSuitInfos (equips); 
		if (infos == null || infos.Length < 1)
			return null;
		List<EquipStarSuitAttrChange> list = new List<EquipStarSuitAttrChange> ();
		for (int i = 0; i < infos.Length; i++) {
			ListKit.AddRange(list,getSuitAttrChangeBySuitInfo (infos [i]));
		}
		return list.ToArray ();	
	}
	
	//获得套装属性比例加成
	public CardBaseAttribute getSuitBaseAttrByPer (string[] equips)
	{
		CardBaseAttribute attr = new CardBaseAttribute ();
		EquipStarSuitAttrChange[] suitAttrs = getEquipsSuitAttrChanges (equips); 
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
	public CardBaseAttribute getSuitBaseAttrChange (string[] equips)
	{
		CardBaseAttribute attr = new CardBaseAttribute ();
		EquipStarSuitAttrChange[] suitAttrs = getEquipsSuitAttrChanges (equips); 
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
				}
			} 
		}
		return attr;
	}
	
	//获得套装对应属性
	private List<EquipStarSuitAttrChange> getSuitAttrChangeBySuitInfo (EquipStarSuitInfo info)
	{
		EquipStarSuitSample sample = EquipStarSuitSampleManager.Instance.getEquipStarSuitSampleBySid (info.suitSid);
		List<EquipStarSuitAttrChange> list = new List<EquipStarSuitAttrChange> ();
		for (int i = 0; i < sample.infos.Length; i++) {
			if (sample.infos [i].num <= info.ids.Count) {
				list.Add (sample.infos [i]); 
			}
		}
		return list;
	}
	
	//获得套装信息
	public EquipStarSuitInfo[] getSuitInfos (string[] equips)
	{
		if (equips == null || equips.Length < 1)
			return null; 
		List<EquipStarSuitInfo> infos = new List<EquipStarSuitInfo> ();
		for (int i = 0; i < equips.Length; i++) {
			addSuitInfo (infos, equips [i]);
		}
		return infos.ToArray ();
	}
	
	//添加套装信息
	private void addSuitInfo (List<EquipStarSuitInfo> infos, string equip_id)
	{ 
		int suitSid = getEquipSuitSid (equip_id);
		if (suitSid == NOT_SUIT)
			return ;
		if (suitSid == 0)
			return ;
		for (int i = 0; i < infos.Count; i++) {
			if (infos [i].suitSid == getEquipSuitSid (equip_id)) {
				infos [i].ids.Add (equip_id);
				return;
			}
		}
		EquipStarSuitInfo info = new EquipStarSuitInfo (getEquipSuitSid (equip_id)); 
		info.addEquipId (equip_id); 
		infos.Add (info);
	}
	
	
	//获得装备套装sid
	public int getEquipSuitSid (string id)
	{
		return StorageManagerment.Instance.getEquip (id).getSuitSid ();
	}
	
	//获得套装对应信息描述
	public string getSuitDescribe (EquipStarSuitSample suit, int num)
	{
		
		foreach (EquipStarSuitAttrChange info in suit.infos) {
			if (info.num == num) {
				return DescribeManagerment.getDescribe (info.describe, 1, info.effects); 
			}
		}
		return "";
		
	} 
	//获得套装对应信息描述
	public string getSuitDescribe (EquipStarSuitAttrChange suit) {
		return DescribeManagerment.getDescribe (suit.describe, 1, suit.effects); 
	} 
	
} 

//套装信息 套装sid 套装收集数量
public class EquipStarSuitInfo
{
	public int suitSid;//套装sid
	public List<string> ids;
	
	public EquipStarSuitInfo (int suitSid)
	{
		this.suitSid = suitSid;
	}
	
	public void addEquipId (string id)
	{
		if (ids == null) {
			ids = new List<string> ();
		}
		ids.Add (id);
	}
	
}
