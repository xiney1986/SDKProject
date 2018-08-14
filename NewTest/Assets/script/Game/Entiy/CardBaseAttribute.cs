using System;
 
/**卡片基础属性对象
 * @author longlingquan
 * */
public class CardBaseAttribute
{
	
	//---------------属性值-------------------
	public int attack = 0;
	public int hp = 0;
	public int defecse = 0;
	public int magic = 0;
	public int agile = 0;
	
	//----------------属性百分比-------------
	public float perAttack = 0;
	public float perHp = 0;
	public float perDefecse = 0;
	public float perMagic = 0;
	public float perAgile = 0;

	public CardBaseAttribute ()
	{
		
	}
	
	//合并卡片基础属性对象
	public void mergeCardBaseAttr (CardBaseAttribute attr)
	{
		this.hp += attr.hp;
		this.agile += attr.agile;
		this.attack += attr.attack;
		this.defecse += attr.defecse;
		this.magic += attr.magic;
		
		this.perHp += attr.perHp;
		this.perAttack += attr.perAttack;
		this.perAgile += attr.perAgile;
		this.perDefecse += attr.perDefecse;
		this.perMagic += attr.perMagic; 
	}


	//合并卡片基础属性对象
	public void mergeCardBaseNum (CardBaseAttribute attr)
	{
		this.hp += attr.hp;
		this.agile += attr.agile;
		this.attack += attr.attack;
		this.defecse += attr.defecse;
		this.magic += attr.magic;
	}

	//合并卡片基础属性对象
	public void mergeCardBasePer (CardBaseAttribute attr)
	{
		this.perHp += attr.perHp;
		this.perAttack += attr.perAttack;
		this.perAgile += attr.perAgile;
		this.perDefecse += attr.perDefecse;
		this.perMagic += attr.perMagic; 
	}

	//获得hp总值
	public int getWholeHp ()
	{
		return (int)((hp * (100 + perHp)) / 100);
	}
	
	//获得攻击总值
	public int getWholeAtt ()
	{
		return (int)((attack * (100 + perAttack)) / 100);
	}
	//获得防御总值
	public int getWholeDEF ()
	{
		return (int)((defecse * (100 + perDefecse)) / 100);
	}
	//获得魔力总值
	public int getWholeMAG ()
	{
		return (int)((magic * (100 + perMagic)) / 100);
	}
	//获得敏捷总值
	public int getWholeAGI ()
	{
		return (int)((agile * (100 + perAgile)) / 100);
	}
} 

