using System;

public class LaddersAttributeEffect
{
	public LaddersAttributeEffect ()
	{
	}
	public int DueTime=-1;//到期时间
	//是否已经到期
	public bool TimeUp
	{
		get
		{
			if(DueTime<0)
			{
				return true;
			}else if(DueTime==0)
			{
				return false;
			}else
			{
				return DueTime<ServerTimeKit.getCurrentSecond();
			}
		}
	}

	private CardBaseAttribute allAddEffectInteger = null;//所有卡片属性加成
	private CardBaseAttribute allAddEffectNumber = null;//所有卡片属性百分比加成


	public CardBaseAttribute getAttrByAllInteger ()
	{
		return allAddEffectInteger == null|| TimeUp ? new CardBaseAttribute () : allAddEffectInteger;
	}

	public CardBaseAttribute getAttrByAllNumber ()
	{
		return allAddEffectNumber == null || TimeUp ? new CardBaseAttribute () : allAddEffectNumber;
	}


	//处理后台初始化
	public void initInfoByServer (ErlArray _attrArray)
	{
		CardBaseAttribute[] getAttr = getAttrByErlArray (_attrArray);
		allAddEffectInteger = getAttr [0];
		allAddEffectNumber = getAttr [1];
	}
	//后台推送更新
	public void updateInfoByServer (ErlArray _attrArray)
	{
		CardBaseAttribute[] getAttr = getAttrByErlArray (_attrArray);
		allAddEffectInteger = getAttr [0];
		allAddEffectNumber = getAttr [1];

	}	
	private CardBaseAttribute[] getAttrByErlArray (ErlArray _attr)
	{
		CardBaseAttribute attrArrayInteger = new CardBaseAttribute ();
		CardBaseAttribute attrArrayNumber = new CardBaseAttribute ();
		ErlArray arry;
		string type;
		string name;
		int attrNum;
		for (int i=0; i <_attr.Value.Length; i++) { 
			arry = _attr.Value [i] as ErlArray;
			type = ((arry.Value [0] as ErlArray).Value [0] as ErlType).getValueString ();
			name = ((arry.Value [0] as ErlArray).Value [1] as ErlType).getValueString ();
			attrNum = StringKit.toInt ((arry.Value [1] as ErlType).getValueString ());
			
			if (type == "integer") {
				if (name == AttrChangeType.HP) {
					attrArrayInteger.hp += attrNum; 
				} else if (name == AttrChangeType.ATTACK) {
					attrArrayInteger.attack += attrNum;
				} else if (name == AttrChangeType.DEFENSE) {
					attrArrayInteger.defecse += attrNum;
				} else if (name == AttrChangeType.MAGIC) {
					attrArrayInteger.magic += attrNum;
				} else if (name == AttrChangeType.AGILE) {
					attrArrayInteger.agile += attrNum;
				}
			} else if (type == "number") {
				if (name == AttrChangeType.HP) {
					attrArrayNumber.perHp += attrNum;
				} else if (name == AttrChangeType.ATTACK) {
					attrArrayNumber.perAttack += attrNum;
				} else if (name == AttrChangeType.DEFENSE) {
					attrArrayNumber.perDefecse += attrNum;
				} else if (name == AttrChangeType.MAGIC) {
					attrArrayNumber.perMagic += attrNum;
				} else if (name == AttrChangeType.AGILE) {
					attrArrayNumber.perAgile += attrNum;
				}
			}
		}		
		return new CardBaseAttribute[2]{attrArrayInteger,attrArrayNumber};
	}
}


