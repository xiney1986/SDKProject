using System;
using System.Collections;
 
/**
 * 装备仓库
 * @author 汤琦
 * */
public class EquipStorage:Storage
{
	public EquipStorage ()
	{
	
	}

	/** 获得指定品质 */
	public ArrayList getAllEquipByQuality (int qualityId)
	{
		ArrayList equips = getStorageProp ();
		ArrayList temp = new ArrayList ();
		for (int i = 0; i < equips.Count; i++) {
			Equip c = equips [i] as Equip;
			if (c.getQualityId () == qualityId) {
				c.index = i;//重置索引
				temp.Add (c);
			}
		}
		return temp;
	}

	/** 获得非祭品装备 */
	public ArrayList getAllEquipByNotToEat ()
	{
		ArrayList equips = getStorageProp ();
		ArrayList temp = new ArrayList ();
		for (int i = 0; i < equips.Count; i++) {
			Equip c = equips [i] as Equip;
			if (!ChooseTypeSampleManager.Instance.isToEat(c,ChooseTypeSampleManager.TYPE_EQUIP_EXP)) {
				c.index = i;//重置索引
				temp.Add (c);
			}
		}
		return temp;
	}

	/** 获得祭品装备 */
	public ArrayList getAllEquipByEat ()
	{
		ArrayList equips = getStorageProp ();
		ArrayList temp = new ArrayList ();
		for (int i = 0; i < equips.Count; i++) {
			Equip c = equips [i] as Equip;
			if (ChooseTypeSampleManager.Instance.isToEat(c,ChooseTypeSampleManager.TYPE_EQUIP_EXP)) {
				c.index = i;//重置索引
				temp.Add (c);
			}
		}
		return temp;
	}

	public ArrayList getAllEquipByEatByQualityId(int id)
	{
		ArrayList equips = getStorageProp ();
		ArrayList temp = new ArrayList ();
		for (int i = 0; i < equips.Count; i++) {
			Equip c = equips [i] as Equip;
			if (ChooseTypeSampleManager.Instance.isToEat(c,ChooseTypeSampleManager.TYPE_EQUIP_EXP) && c.getQualityId() == id) {
				c.index = i;//重置索引
				temp.Add (c);
			}
		}
		return temp;
	}

	public override void parse (ErlArray arr)
	{
		ErlArray ea1 = arr.Value [1] as ErlArray;
		if (ea1.Value.Length <= 0) {
			init (StringKit.toInt (arr.Value [0].getValueString ()), null);
		} else {
			ArrayList al = new ArrayList (); 
			Equip equip;
			for (int i = 0; i < ea1.Value.Length; i++) { 
				equip = EquipManagerment.Instance.createEquip ();
				equip.bytesRead (0, ea1.Value [i] as ErlArray);
				al.Add (equip);
			}  
			init (StringKit.toInt (arr.Value [0].getValueString ()), al);
		}
	}
} 

