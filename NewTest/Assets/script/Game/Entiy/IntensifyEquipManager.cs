using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IntensifyEquipManager
{

	private Equip mainEquip;//主卡
	private List<Equip> foodEquip;//食物卡

	public static bool IsOpenOneKeyWnd = true; //是否打开一键选择窗口
	public static int Choose = QualityType.EXCELLENT; //一键选择的品质


	public static IntensifyEquipManager Instance {
		get{return SingleManager.Instance.getObj("IntensifyEquipManager") as IntensifyEquipManager;}
	}

	//设置主卡
	public void setMainEquip (Equip eq)
	{
		mainEquip = eq;
	}
	
	public bool isHaveMainEquip ()
	{
		return mainEquip != null;
	}
	
	public Equip getMainEquip ()
	{
		return mainEquip;
	}

	//移除主卡
	public void removeMainEquip ()
	{
		mainEquip = null;
	}

	//移除一个食物卡
	public void removeFoodEquip (Equip eq)
	{
		if (foodEquip != null)
			foodEquip.Remove (eq);
	}

	//清空食物卡
	public void clearFoodEquip ()
	{
		foodEquip = null;
	}

	//设置食物卡
	public void setFoodEquip (Equip eq)
	{
		if (foodEquip == null)
			foodEquip = new List<Equip> ();
		foodEquip.Add (eq);
	}
	
	public List<Equip> getFoodEquip ()
	{
		return foodEquip == null ? new List<Equip> () : foodEquip ;
	}

	//是否有食物卡
	public bool isHaveFood ()
	{
		return foodEquip == null || foodEquip.Count == 0;
	}

	public bool isFoodFull ()
	{
		if (isHaveFood ())
			return false;
		return foodEquip.Count >= 8;
	}

	public bool isInFood (Equip eq)
	{
		if (foodEquip == null)
			return false;
		for (int i = 0; i < foodEquip.Count; i++) {
			if (foodEquip [i].uid == eq.uid) {
				return true;
			}
		}
		return false;
	}

	public bool compareMainEquip (Equip eq)
	{
		return eq != null && mainEquip != null && eq.uid == mainEquip.uid;
	}

	//装备中
	private bool isInUse (Equip eq)
	{
		if (eq.getState () == EquipStateType.OCCUPY) {
			return true;
		} else {
			return false;
		}
	}

	//升级过
	private bool isHaveIntensify (Equip eq)
	{
		if (eq.getEXP () > 0) {
			return true;
		} else {
			return false;
		}
	}

	//祭品
	public bool isEat (Equip eq)
	{
		if (ChooseTypeSampleManager.Instance.isToEat (eq, ChooseTypeSampleManager.TYPE_EQUIP_EXP))
			return true;
		else
			return false;
	}

	//一键选择普通装备
	public List<Equip> getOneKeySacrifice ()
	{
		List<Equip> tempList = new List<Equip> ();

		int num = 0;
		if (foodEquip == null) {
			num = 8;
		} else {
			num = 8 - foodEquip.Count;
		}

		//获取普通装备集
		ArrayList common = StorageManagerment.Instance.getAllEquipByCommon();
		if (common != null) {
			for (int i = 0; i < common.Count; i++) {
				Equip tempEq = common [i] as Equip;
				
				if (compareMainEquip (tempEq) || isInFood (tempEq) || isInUse (tempEq) || isHaveIntensify (tempEq) || isEat(tempEq))
					continue;
				if (num == 0) {
					return tempList;
				}
				tempList.Add (tempEq);
				num --;
			}
		}
		//获取优秀装备集
		ArrayList excellent = StorageManagerment.Instance.getAllEquipByExcellent ();
		if (excellent != null) {
			for (int i = 0; i < excellent.Count; i++) {
				Equip tempEq = excellent [i] as Equip;
				if (compareMainEquip (tempEq) || isInFood (tempEq) || isInUse (tempEq) || isHaveIntensify (tempEq) || isEat(tempEq))
					continue;
				if (num == 0) {
					return tempList;
				}
				tempList.Add (tempEq);
				num --;
			}
		}
		//获取精良装备集
		ArrayList good = StorageManagerment.Instance.getAllEquipByGood ();
		if (good != null) {
			for (int i = 0; i < good.Count; i++) {
				Equip tempEq = good [i] as Equip;
				if (compareMainEquip (tempEq) || isInFood (tempEq) || isInUse (tempEq) || isHaveIntensify (tempEq) || isEat(tempEq))
					continue;
				if (num == 0) {
					return tempList;
				}
				tempList.Add (tempEq);
				num --;
			}
		}
		//获取史诗装备集
		ArrayList epic = StorageManagerment.Instance.getAllEquipByEpic ();
		if (epic != null) {
			for (int i = 0; i < epic.Count; i++) {
				Equip tempEq = epic [i] as Equip;
				//Epic 品质要排除掉精灵卡
				if (compareMainEquip (tempEq) || isInFood (tempEq) || isInUse (tempEq) || isHaveIntensify (tempEq) || isEat(tempEq))
					continue;
				if (num == 0) {
					return tempList;
				}
				tempList.Add (tempEq);
				num --;
			}
		}
		//获取传说装备集
		ArrayList legend = StorageManagerment.Instance.getAllEquipByLegend ();
		if (legend != null) {
			for (int i = 0; i < legend.Count; i++) {
				Equip tempEq = legend [i] as Equip;
				if (compareMainEquip (tempEq) || isInFood (tempEq) || isInUse (tempEq) || isHaveIntensify (tempEq) || isEat(tempEq))
					continue;
				if (num == 0) {
					return tempList;
				}
				tempList.Add (tempEq);
				num --;
			}
		}

		return tempList;
	}

	//一键选择祭品装备
	public List<Equip> getOneKeySacrificeEat ()
	{
		List<Equip> tempList = new List<Equip> ();
		
		int num = 0;
		if (foodEquip == null) {
			num = 8;
		} else {
			num = 8 - foodEquip.Count;
		}
		
		//获取普通装备集
		ArrayList common = StorageManagerment.Instance.getAllEquipByCommon();
		if (common != null) {
			for (int i = 0; i < common.Count; i++) {
				Equip tempEq = common [i] as Equip;
				
				if (compareMainEquip (tempEq) || isInFood (tempEq) || isInUse (tempEq) || isHaveIntensify (tempEq) || !isEat(tempEq))
					continue;
				if (num == 0) {
					return tempList;
				}
				tempList.Add (tempEq);
				num --;
			}
		}
		//获取优秀装备集
		ArrayList excellent = StorageManagerment.Instance.getAllEquipByExcellent ();
		if (excellent != null) {
			for (int i = 0; i < excellent.Count; i++) {
				Equip tempEq = excellent [i] as Equip;
				if (compareMainEquip (tempEq) || isInFood (tempEq) || isInUse (tempEq) || isHaveIntensify (tempEq) || !isEat(tempEq))
					continue;
				if (num == 0) {
					return tempList;
				}
				tempList.Add (tempEq);
				num --;
			}
		}
		//获取精良装备集
		ArrayList good = StorageManagerment.Instance.getAllEquipByGood ();
		if (good != null) {
			for (int i = 0; i < good.Count; i++) {
				Equip tempEq = good [i] as Equip;
				if (compareMainEquip (tempEq) || isInFood (tempEq) || isInUse (tempEq) || isHaveIntensify (tempEq) || !isEat(tempEq))
					continue;
				if (num == 0) {
					return tempList;
				}
				tempList.Add (tempEq);
				num --;
			}
		}
		//获取史诗装备集
		ArrayList epic = StorageManagerment.Instance.getAllEquipByEpic ();
		if (epic != null) {
			for (int i = 0; i < epic.Count; i++) {
				Equip tempEq = epic [i] as Equip;
				//Epic 品质要排除掉精灵卡
				if (compareMainEquip (tempEq) || isInFood (tempEq) || isInUse (tempEq) || isHaveIntensify (tempEq) || !isEat(tempEq))
					continue;
				if (num == 0) {
					return tempList;
				}
				tempList.Add (tempEq);
				num --;
			}
		}
		//获取传说装备集
		ArrayList legend = StorageManagerment.Instance.getAllEquipByLegend ();
		if (legend != null) {
			for (int i = 0; i < legend.Count; i++) {
				Equip tempEq = legend [i] as Equip;
				if (compareMainEquip (tempEq) || isInFood (tempEq) || isInUse (tempEq) || isHaveIntensify (tempEq) || !isEat(tempEq))
					continue;
				if (num == 0) {
					return tempList;
				}
				tempList.Add (tempEq);
				num --;
			}
		}
		
		return tempList;
	}

}
