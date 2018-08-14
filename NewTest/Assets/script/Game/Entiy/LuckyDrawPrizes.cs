using System;
using System.Collections.Generic;
 
/**
 * 抽奖结果
 * @author longlingquan
 * */
public class LuckyDrawResults
{
	private List<LuckyDrawPrize> prizes;
	private int index = 0;//索引从0开始
	private List<SinglePrize> list;
	private bool isIntoTemp = false;
	public DrawWay drawWay ;
	
	public LuckyDrawResults ()
	{
		prizes = new List<LuckyDrawPrize> ();
	}
	
	/// <summary>
	/// 抽奖 奖品是否有进入临时仓库的
	/// </summary>
	/// <returns><c>true</c>, if temp was ised, <c>false</c> otherwise.</returns>
	public bool isTemp ()
	{
		return isIntoTemp;
	}
	
	//获得下一个奖励
	public LuckyDrawPrize getNextPrize ()
	{
		if (prizes [index].isGetNext ()) {
			return prizes [index];
		} else {
			if (index == prizes.Count - 1)
				return null;
			index++;
			return getNextPrize ();
		}
	}
	 
	public List<SinglePrize> getSinglePrizes ()
	{
		if (list != null)
			return list;
		list = new List<SinglePrize> ();
		for (int i = 0; i < prizes.Count; i++) {
			if (prizes [i].type == LuckyDrawPrize.TYPE_MONEY || prizes [i].type == LuckyDrawPrize.TYPE_RMB) {
				list.Add (evaMoneyOrRMBPrize (prizes [i]));
			} else if (prizes [i].type == LuckyDrawPrize.TYPE_CARD || prizes [i].type == LuckyDrawPrize.TYPE_EQUIP|| prizes [i].type == LuckyDrawPrize.TYPE_TOOL||prizes[i].type==LuckyDrawPrize.TYPE_MAGIC_WEAPON) {
                //if (prizes [i].num > 1) {
                //    for (int j = 0; j < prizes[i].num; j++) {
                //        list.Add (evaOtherPrize (prizes [i]));
                //    }
                //} else {		
					list.Add (evaOtherPrize (prizes [i]));
                //}
			}
		}

		return list;
	}

	public List<SinglePrize> getSinglePrizesByQuality (List<SinglePrize> _Oldlist)
	{
		if(_Oldlist == null) {
			return null;
		}

		for(int i=0;i<_Oldlist.Count;i++)
		{
			for(int j=i+1;j<_Oldlist.Count;j++)
			{
				if(_Oldlist [i].type == LuckyDrawPrize.TYPE_CARD) {
					if(_Oldlist [j].type == LuckyDrawPrize.TYPE_CARD) {
						Card card_i = StorageManagerment.Instance.getRole(list[i].uid);
						if (card_i == null) {
							card_i = CardManagerment.Instance.createCard (list[i].sid);
						}
						Card card_j = StorageManagerment.Instance.getRole(list[j].uid);
						if (card_j == null) {
							card_j = CardManagerment.Instance.createCard (list[j].sid);
						}
						if(card_i != null && card_j != null) {
							if(card_i.getQualityId()<card_j.getQualityId())
							{
								SinglePrize item = _Oldlist[i];
								_Oldlist[i] = _Oldlist[j];
								_Oldlist[j] = item;
							}
						}
					}
				}
			}
		}

		return _Oldlist;

	}
	
	/// <summary>
	/// 检查奖励是否进入到玩家背包<仓库>
	/// </summary>
	/// <returns><c>true</c>, if prize is into package was checked, <c>false</c> otherwise.</returns>
	/// <param name="prize">Prize.</param>
	private bool checkPrizeIsIntoPackage (LuckyDrawPrize prize)
	{
		if (prize.type == "card") {
			Card card = StorageManagerment.Instance.getRole (prize.uid);
			if (card != null)
				return true; 
		} else if (prize.type == "goods") {
			Prop prop = StorageManagerment.Instance.getProp (prize.sid);
			if (prop != null)
				return true; 
		} else if (prize.type == "equipment") {
			Equip equip = StorageManagerment.Instance.getEquip (prize.uid);
			if (equip != null)
				return true; 
		}
		return false;
	}

	private SinglePrize evaOtherPrize (LuckyDrawPrize prize)
	{
		SinglePrize p = new SinglePrize ();
		p.sid = prize.sid;
		p.uid = prize.uid;
		p.num = 1;
		p.sourceType = prize.sourceType;
		p.type = prize.type;
		return p;
	}
	
	private SinglePrize evaMoneyOrRMBPrize (LuckyDrawPrize prize)
	{
		SinglePrize p = new SinglePrize ();
		p.sid = prize.sid;
		p.uid = prize.uid;
		p.num = prize.num;
		p.sourceType = prize.sourceType;
		p.type = prize.type;
		return p;
	}
	
	//获得上一个奖励
	public LuckyDrawPrize getLastPrize ()
	{
		if (prizes [index].isGetLast ())
			return prizes [index];
		else {
			if (index == 0)
				return null;
			else {
				index--;
				return getLastPrize ();
			}
		} 
	}
	
	public void parse (ErlArray arr)
	{ 
		string type = (arr.Value [0] as ErlAtom).Value;
		ErlArray erlArr = arr.Value [1] as ErlArray;
		if (erlArr.Value.Length < 1)
			return;
		for (int i = 0; i < erlArr.Value.Length; i++) {
			LuckyDrawPrize prize = new LuckyDrawPrize (type, erlArr.Value [i] as ErlArray);
			if(!isIntoTemp)
			{
				isIntoTemp=!checkPrizeIsIntoPackage(prize);
			}
			prizes.Add (prize);
		}  
	}
    public void setPrizes(LuckyDrawPrize ldp) {
        prizes.Add(ldp);
    }
} 

/**
 * 单个抽奖奖励信息
 * @author longlingquan
 * */
public class LuckyDrawPrize
{ 
	
	public const string PRIZE = "prize";//抽奖奖励
	public const string GIFT = "gift";//抽奖赠品
	
	public const string TYPE_CARD = "card";
	public const string TYPE_EQUIP = "equipment";
	public const string TYPE_TOOL = "goods";
	public const string TYPE_MONEY = "money";
	public const string TYPE_RMB = "rmb";
	public const string TYPE_BEAST = "beast";
    public const string TYPE_MAGIC_WEAPON = "magic_weapon";
	public string sourceType = "";//奖励来源类型
	public string type = "";//奖励类型
	public int sid = 0;//奖励sid 
	public string uid = "";//奖励uid
	public int num = 0;//奖励数量
	public int index = 0;//当前奖励索引 从1开始
	
	public LuckyDrawPrize (string sourceType, ErlArray arr)
	{
		this.sourceType = sourceType;
		parse (arr);
	}
    public LuckyDrawPrize() {

    }
	
	//解析单个奖励信息
	private void parse (ErlArray arr)
	{
		if (arr == null) {
			return;
		}
		type = (arr.Value [0]).getValueString (); 
		if (type == TYPE_MONEY || type == TYPE_RMB) {
			num = StringKit.toInt (arr.Value [1].getValueString ());
		}else if(type == TYPE_CARD){
			Card card=CardManagerment.Instance.createCard();
			card.bytesRead(0,arr.Value [1] as ErlArray);
			StorageManagerment.Instance.addCardProp(card);
			sid=card.sid;
			uid=card.uid;
			num=1;
		}
		else if(type == TYPE_BEAST){
			Card card=CardManagerment.Instance.createCard();
			card.bytesRead(0,arr.Value [1] as ErlArray);
			StorageManagerment.Instance.addBeastProp(card);
			sid=card.sid;
			uid=card.uid;
			num=1;
		}
		else if(type == TYPE_EQUIP){
			Equip equip=EquipManagerment.Instance.createEquip();
			equip.bytesRead(0,arr.Value [1] as ErlArray);
			StorageManagerment.Instance.addEquipProp(equip);
			sid=equip.sid;
			uid=equip.uid;
			num=1;
		}
		else if(type == TYPE_TOOL){
			Prop prop=PropManagerment.Instance.createProp();
			prop.bytesRead(0,arr.Value [1] as ErlArray);
			StorageManagerment.Instance.addGoodsProp(prop);
			sid = prop.sid;
			num = prop.getNum();
		} 
	}
	
	//是否能够获得下一个
	public bool isGetNext ()
	{
		//数值类奖励只能取一次
		if (type == TYPE_MONEY || type == TYPE_RMB) {
			if (index == 1)
				return false;
		}
		if (index == num)
			return false;
		else {
			index++;
			return true;
		}
	}
	 
	//是否能够获得前一个
	public bool isGetLast ()
	{
		if (type == TYPE_MONEY || type == TYPE_RMB) {
			if (index == 1) {
				index--;
				return false;
			}
		}
		if (index < 1)
			return false;
		else if (index == 1) {
			index--;
			return false;
		} else {
			index--;
			return true;
		}
	}
}

