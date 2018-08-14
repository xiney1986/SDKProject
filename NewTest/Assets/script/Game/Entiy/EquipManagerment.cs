using System;
using System.Collections;
using System.Collections.Generic;
 
/**
 * 装备管理器
 * @author longlingquan
 * */
public class EquipManagerment
{
	private const int RATIO_HP = 35;//装备能力值系数 HP
	private const int RATIO_ATT = 115;//装备能力值系数 ATT
	private const int RATIO_DEF = 90;//装备能力值系数 DEF
	private const int RATIO_MAG = 115;//装备能力值系数 MIG
	private const int RATIO_AGI = 115;//装备能力值系数 AGI


	public  Card activeEquipMan;	//当前穿装者
	public Equip  activeInstandEquip;//准备要被替换的装备
	public int  activePartID=-1;//本次替换的部位

	public EquipManagerment ()
	{
		
	}

	public static EquipManagerment Instance {
		get{return SingleManager.Instance.getObj("EquipManagerment") as EquipManagerment;}
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
	
	public  int stateIDToString (int id)
	{
		
		switch (id) {
		case EquipStateType.LOCKED:
                return EquipStateType.LOCKED;
		case EquipStateType.OCCUPY:
                return EquipStateType.OCCUPY;
		}	 
		return EquipStateType.LOCKED;
	}


	public Equip createEquip ()
	{
		return new Equip ();
	}
	public Equip createEquip ( int sid)
	{
		return new Equip ("", sid, 0, 0, 0,0);
	}
	
	public Equip createEquip (string id, int sid, long exp, int state,long reexp)
	{
		return new Equip (id, sid, exp, state, 0,reexp);
	}
	
	public Equip createEquip (ErlArray array)
	{
		int j = 0;
        int arrayLength = array.Value.Length;
		string uid = array.Value [j++].getValueString ();
		int sid = StringKit.toInt (array.Value [j++].getValueString ());
		long exp = StringKit.toLong (array.Value [j++].getValueString ());
		int state = StringKit.toInt (array.Value [j++].getValueString ());
        //特殊处理，聊天分享中存在装备，后台没给升星等级，按0级处理。
        int starLevel = (j == arrayLength ? 0 : StringKit.toInt(array.Value[j++].getValueString()));//StringKit.toInt (array.Value [j++].getValueString ());
        long reexp = StringKit.toLong(array.Value[j++].getValueString());
		return new Equip (uid, sid, exp, state, starLevel,reexp);
	}
	
	//获得一组装备，聊天用,返回[string[],List<Equip>]
	public object[] createEquipByChat (ErlArray array)
	{
		if(array == null)
			return null;
		object[] objs=new object[2];
		List<Equip> eq = new List<Equip>();
		int length=array.Value.Length;
		string[] strs=new string[length];
		Equip equip;
		for(int i=0;i<length;i++)
		{
			equip=createEquip(array.Value[i] as ErlArray);
			strs[i]=equip.uid;
			eq.Add(equip);
		}
		objs[0]=strs;
		objs[1]=eq;
		return objs;
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

    public int getEquipBasePower(Equip equip)
    {
        int hp = getEquipAttribute(equip.sid, equip.getLevel(), AttributeType.hp);
        int mag = getEquipAttribute(equip.sid, equip.getLevel(), AttributeType.magic);
        int agi = getEquipAttribute(equip.sid, equip.getLevel(), AttributeType.agile);
        int def = getEquipAttribute(equip.sid, equip.getLevel(), AttributeType.defecse);
        int att = getEquipAttribute(equip.sid, equip.getLevel(), AttributeType.attack);
        return hp * RATIO_HP + att * RATIO_ATT + def * RATIO_DEF + mag * RATIO_MAG + agi * RATIO_AGI;
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

	public bool canUse(int cardSid,int equipSid)
	{
		EquipSample sample = EquipmentSampleManager.Instance.getEquipSampleBySid (equipSid);
		if(sample == null)
			return false;
		for(int i = 0; i < sample.exclusive.Length; i++)
		{
			if(sample.exclusive[i] == 0 || sample.exclusive[i] == cardSid)
				return true;
		}
		return false;
	}
	
}  