using System;
using System.Collections;
using System.Collections.Generic;
 
/**
 * 秘宝管理器
 * @author liwei
 * */
public class MagicWeaponManagerment
{
	private const int RATIO_HP = 35;//装备能力值系数 HP
	private const int RATIO_ATT = 115;//装备能力值系数 ATT
	private const int RATIO_DEF = 90;//装备能力值系数 DEF
	private const int RATIO_MAG = 115;//装备能力值系数 MIG
	private const int RATIO_AGI = 115;//装备能力值系数 AGI

    public const int USEDBYMAGICITEM = 6;//用于神器显示星星
    public const int USEDBUMAGIC_ATTRSHOW = 7;//用于神器显示星星
    public const int USEDBUMAGIC_AWARD = 8;//用于神器显示星星
	public  Card activeEquipMan;	//当前穿装者
	public Equip  activeInstandEquip;//准备要被替换的装备
	public int  activePartID=-1;//本次替换的部位
    public Card selectCard;//装备秘宝时选择的秘宝 用了以后要清空

    public MagicWeaponManagerment()
	{
		
	}

    public static MagicWeaponManagerment Instance {
        get { return SingleManager.Instance.getObj("MagicWeaponManagerment") as MagicWeaponManagerment; }
	}

	public void setEquipChange(Card card,Equip equip,int partID)
	{
		activeEquipMan=card;
		activeInstandEquip=equip;
		activePartID=partID;
	}
	public void finishEquipChange()
	{
		activeEquipMan=null;
		activeInstandEquip=null;
		activePartID=-1;
	}
	
	public  string stateIDToString (int id)
	{
		
		switch (id) {
		case EquipStateType.LOCKED:
			return "";
		case EquipStateType.OCCUPY:
			return LanguageConfigManager.Instance.getLanguage ("s0017");
		}	 
		return "";
	}

    /// <summary>
    /// 重建一个新的秘宝
    /// </summary>
    /// <returns></returns>
	public MagicWeapon createMagicWeapon (int sid)
	{
        return new MagicWeapon("",sid,0,0,0);
	}
    public MagicWeapon createMagicWeapon() {
        return new MagicWeapon();
    }
	
	//获得一组装备的id
	public string[] getEquipId (ErlArray ea)
	{
		if(ea == null)
			return null;
		if(ea.Value == null)
			return null;
		if (ea.Value.Length < 1)
			return null;
		string[] equips = new string[ea.Value.Length];
		for (int i = 0; i < equips.Length; i++) {
			equips [i] = ea.Value [i].getValueString ();
		}
		return equips;
	}
	
	//获得装备基础属性
	public int getEquipAttribute (int sid, int level, AttributeType attr)
	{
		int baseAttr = EquipmentSampleManager.Instance.getBaseAttribute (sid, attr);
		int developAttr = EquipmentSampleManager.Instance.getLevelUpAttribute (sid, attr); 
		return baseAttr + developAttr * (level - 1);
	} 
	
	//计算装备战斗力
	public int getEquipPower (Equip equip)
	{
		return equip.getHP () * RATIO_HP + equip.getAttack () * RATIO_ATT + equip.getDefecse () * RATIO_DEF + equip.getMagic () * RATIO_MAG + equip.getAgile () * RATIO_AGI;
	}
	
	//判断是否是最强装备(要根据位置判断)
	public bool isStrongestEquip (ArrayList list, Equip equip)
	{
		if (list == null || list.Count < 1)
			return true; 
		
		Condition[] cons = new Condition[1];
		cons [0] = new Condition (SortType.EQUIP_PART);
		cons [0].addCondition (equip.getPartId ());//添加筛选条件 部位
 		
		Condition con = new Condition (SortType.SORT);
		con.addCondition (SortType.SORT_POWERDOWN);		
		SortCondition sc = new SortCondition ();
		sc.sortCondition = con;
		sc.siftConditionArr = cons;
		
		ArrayList arr = SortManagerment.Instance.equipSort (list, sc);
		if (arr == null || arr.Count < 1)
			return true;
		Equip eq = arr [0] as Equip;
		if (eq.getPower () > equip.getPower ())
			return true;
		else
			return false;
	}

    public bool canUse(int cardSid, int equipSid)
    {
        EquipSample sample = EquipmentSampleManager.Instance.getEquipSampleBySid(equipSid);
        if (sample == null)
            return false;
        for (int i = 0; i < sample.exclusive.Length; i++)
        {
            if (sample.exclusive[i] == 0 || sample.exclusive[i] == cardSid)
                return true;
        }
        return false;
    }

}  