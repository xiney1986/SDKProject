using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuffManager
{ 
	List<BuffDataBase> BuffDataBaseList;

	public static BuffManager Instance {
		get{return SingleManager.Instance.getObj("BuffManager") as BuffManager;}
	}
	
	public void init ()
	{ 
		 
	}
	
	public  BuffDataBase getBuffDataBase (int index)
	{
		foreach (BuffDataBase each in BuffDataBaseList)
			if (each.buffSid == index) {
				//	ResourcesManager .Instance.cacheResource (each.damageEffect);	
				//	ResourcesManager .Instance.cacheResource(each.DisplayResPath);	
				return each;
			}
		return null;
	}
	 
	//实例一个buffdata，管理基于battleInfo
	public BuffData CreateBuffData (int orgBuffID, int damage, string dmgEffect)
	{
		BuffData _new = new BuffData (orgBuffID, damage);
//		_new.database = getBuffDataBase (orgBuffID); 
		_new.damageEffect = dmgEffect;
		if (dmgEffect == "" || dmgEffect == null)
			MonoBase.print ("@:" + orgBuffID);
		if (_new == null) 
			MonoBase.print (orgBuffID);
		return _new;	
	}
	
	void 	loadAllBuffDataBase ()
	{
		// form IO 
		//临时
		
		/**	BuffDataBase bf = new BuffDataBase (); 
		bf.buffSid = 1;	
		bf.buffName = "just damage"; 
		bf.isDurationBuff = false;		
		//	bf.damageEffect = "Effect/normalHit/normalHit";
		bf.buffType = buff_type.damage;
		AddBuffDataBase (bf);
		
		bf = new BuffDataBase (); 
		bf.buffSid = 2;	
		bf.buffName = " power ++++";
		bf.damagePerTurn = 0;
		bf.attr_attack = 0;
		bf.attr_defend = 0;
		bf.attr_magic = 0;
		bf.attr_dex = 0; 
		bf.BuffDisplayType = buff_icon_type.None;
		bf.isDurationBuff = false;
		bf.buffType = buff_type.power;
		AddBuffDataBase (bf);
		
		//麻痹
		bf = new BuffDataBase (); 
		bf.buffSid = 1107;	
		bf.buffName = "mabi";
		bf.damagePerTurn = 0;
		bf.attr_attack = 0;
		bf.attr_defend = 0;
		bf.attr_magic = 0;
		bf.attr_dex = 0; 
		bf.BuffDisplayType = buff_icon_type.BodyEffect;
		bf.isDurationBuff = true;
		bf.buffType = buff_type.frozen;
		bf.DisplayResPath = "Effect/Buff/vertigo/vertigo";
		AddBuffDataBase (bf);
		
		//麻痹
		bf = new BuffDataBase (); 
		bf.buffSid = 1214;	
		bf.buffName = "mabi";
		bf.damagePerTurn = 0;
		bf.attr_attack = 0;
		bf.attr_defend = 0;
		bf.attr_magic = 0;
		bf.attr_dex = 0; 
		bf.BuffDisplayType = buff_icon_type.BodyEffect;
		bf.isDurationBuff = true;
		bf.buffType = buff_type.frozen;
		bf.DisplayResPath = "Effect/Buff/vertigo/vertigo";
		AddBuffDataBase (bf);
		
		
		//沉默
		bf = new BuffDataBase (); 
		bf.buffSid = 1220;	
		bf.buffName = "mabi";
		bf.damagePerTurn = 0;
		bf.attr_attack = 0;
		bf.attr_defend = 0;
		bf.attr_magic = 0;
		bf.attr_dex = 0; 
		bf.BuffDisplayType = buff_icon_type.BodyEffect;
		bf.isDurationBuff = true;
		bf.buffType = buff_type.silence;
		bf.DisplayResPath = "Effect/Buff/silence/silence";
		AddBuffDataBase (bf);	
		
		//麻痹
		bf = new BuffDataBase (); 
		bf.buffSid = 1204;	
		bf.buffName = "mabi";
		bf.damagePerTurn = 0;
		bf.attr_attack = 0;
		bf.attr_defend = 0;
		bf.attr_magic = 0;
		bf.attr_dex = 0; 
		bf.BuffDisplayType = buff_icon_type.BodyEffect;
		bf.isDurationBuff = true;
		bf.buffType = buff_type.frozen;
		bf.DisplayResPath = "Effect/Buff/vertigo/vertigo";
		AddBuffDataBase (bf);	
		
		//麻痹
		bf = new BuffDataBase ();
		
		bf.buffSid = 1207;	
		bf.buffName = "att_up";
		bf.damagePerTurn = 0;
		bf.attr_attack = 50;
		bf.attr_defend = 0;
		bf.attr_magic = 0;
		bf.attr_dex = 0;
		
		bf.BuffDisplayType = buff_icon_type.Small_Icon;
		bf.isDurationBuff = true;
		bf.buffType = buff_type.attr_change;
		bf.DisplayResPath = "Effect/Buff/vertigo/vertigo";
		AddBuffDataBase (bf);	
		
		
		
		//提升魔力
		bf = new BuffDataBase ();
		
		bf.buffSid = 1225;	
		bf.buffName = "moli ++";
		bf.damagePerTurn = 0;
		bf.attr_attack = 0;
		bf.attr_defend = 0;
		bf.attr_magic = 50;
		bf.attr_dex = 0;
		
		bf.BuffDisplayType = buff_icon_type.Small_Icon;
		bf.isDurationBuff = true;
		bf.buffType = buff_type.attr_change;
		bf.DisplayResPath = "";
		AddBuffDataBase (bf);	
		
		
		//1209
		bf = new BuffDataBase ();
		
		bf.buffSid = 1209;	
		bf.buffName = "def down";
		bf.damagePerTurn = 0;
		bf.attr_attack = 0;
		bf.attr_defend = -50;
		bf.attr_magic = 0;
		bf.attr_dex = 0;
		
		bf.BuffDisplayType = buff_icon_type.Small_Icon;
		bf.isDurationBuff = true;
		bf.buffType = buff_type.attr_change;
		bf.DisplayResPath = "";
		AddBuffDataBase (bf);		
	
		//1234
		bf = new BuffDataBase ();
		
		bf.buffSid = 1234;	
		bf.buffName = "att up";
		bf.damagePerTurn = 0;
		bf.attr_attack = 80;
		bf.attr_defend = 0;
		bf.attr_magic = 0;
		bf.attr_dex = 0;
		
		bf.BuffDisplayType = buff_icon_type.Small_Icon;
		bf.isDurationBuff = true;
		bf.buffType = buff_type.attr_change;
		bf.DisplayResPath = "";
		bf.DurationdamageEffect = "Effect/Buff/attackUp/attackUp";
		AddBuffDataBase (bf);	
		
		
		bf = new BuffDataBase ();
		
		bf.buffSid = 3;	
		bf.buffName = " popo";
		bf.damagePerTurn = -50;
		bf.duration = 5;
		bf.attr_attack = 0;
		bf.attr_defend = 0;
		bf.attr_magic = 0;
		bf.attr_dex = 0;
		
		bf.BuffDisplayType = buff_icon_type.BodyEffect;
		bf.isDurationBuff = true;
		bf.buffType = buff_type.durationDamage;
		bf.DisplayResPath = "";
		AddBuffDataBase (bf);
		
		bf = new BuffDataBase ();
		
		bf.buffSid = 4;	
		bf.buffName = " m +++hp";
		bf.damagePerTurn = 0;
		bf.duration = 0;
		
		bf.attr_attack = 0;
		bf.attr_defend = 0;
		bf.attr_magic = 0;
		bf.attr_dex = 0;
		
		bf.BuffDisplayType = buff_icon_type.None;
		bf.isDurationBuff = false;
		bf.buffType = buff_type.damage;
		bf.DisplayResPath = "";
		//	bf.damageEffect = "Effect/normalHit/normalHit";
		AddBuffDataBase (bf);
		
		
		bf = new BuffDataBase ();
		
		bf.buffSid = 5;	
		bf.buffName = "just damage";
		
		bf.isDurationBuff = false;		
		//	bf.damageEffect = "Effect/effect_HuoQiu/effect_HuoQiu";
		bf.buffType = buff_type.damage;
		AddBuffDataBase (bf); */
	}
	
	BuffDataBase CreateBuffDataBaseByID (int index)
	{
		BuffDataBase _data = new BuffDataBase ();
		return _data;
	}
	
	public void  clean ()
	{
		BuffDataBaseList.Clear ();
	}
	
	public void AddBuffDataBase (BuffDataBase _data)
	{
		if (BuffDataBaseList == null)
			BuffDataBaseList = new List<BuffDataBase> ();
		BuffDataBaseList.Add (_data);
	}
	
	public void RemoveBuffDataBase (BuffDataBase _data)
	{
		if (BuffDataBaseList == null)
			return; 
		BuffDataBaseList.Remove (_data); 
	}
	
	public int Length ()
	{ 
		return BuffDataBaseList.Count;
	} 
}
