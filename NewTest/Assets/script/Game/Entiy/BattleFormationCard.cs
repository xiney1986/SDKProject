using System;
 
/**
 * 卡片站位信息
 * 包含卡片信息 站位信息
 * @author longlingquan
 * */
public class BattleFormationCard
{
	public BattleFormationCard ()
	{
	}
	
	public Card card = null;
	public int loc;//站位 1 - 15
	private int hp = -1;//如果为-1 表示满血
	private int maxHp = -1;
	private int lv = -1;

	public void setLevel (int lv)
	{
		this.lv = lv;
	}

	public int getLevel ()
	{
		if (lv == -1)
			return card.getLevel ();
		return lv;
	}
	
	public void setHp(int hp)
	{
		this.hp = hp;
	}
	
	public void setHpMax(int maxHp)
	{
		this.maxHp = maxHp;
	}

	public int getHp ()
	{
		if (hp == -1)
			return CardManagerment.Instance.getCardWholeAttr (card).hp;
		return hp;
	}
	
	public int getHpMax ()
	{
		if (maxHp == -1)
			return CardManagerment.Instance.getCardWholeAttr (card).hp;
		return maxHp;
	}
} 

