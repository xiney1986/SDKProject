using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 卡片碎片管理器
 * @author 陈世惟
 * */
public class CardScrapManagerment {

	List<Exchange> exchangeList;

	public CardScrapManagerment ()
	{
		
	}
	
	public static CardScrapManagerment Instance {
		get{return SingleManager.Instance.getObj("CardScrapManagerment") as CardScrapManagerment;}
	}

	/// <summary>
	/// 是否有能够兑换的卡片
	/// </summary>
	/// <returns><c>true</c> if this instance can exchange card; otherwise, <c>false</c>.</returns>
	public bool CanExchangeCard(){
		List<Exchange> list =getCardScrapList();
		foreach(var tmp in list){
			int count = ExchangeManagerment.Instance.getCanExchangeNum (tmp);
			if(count >0)
				return true;
		}
		return false;
	}

	//从道具仓库找到碎片列表，从而确定可显示的兑换列表
	public List<Exchange> getCardScrapList()
	{
//		return ExchangeManagerment.Instance.getCanUseExchangesCard (ExchangeType.COMMON);//临时启用
		if (exchangeList == null) {
			exchangeList = ExchangeManagerment.Instance.getCanUseExchangesCardScrap ();
		}
		List<Exchange> newList = new List<Exchange>();
		ArrayList cardScrapList = StorageManagerment.Instance.getAllPropByCardScrap();

		if (cardScrapList != null && cardScrapList.Count > 0) {
			Prop prop;
			for (int i = 0; i < cardScrapList.Count; i++) {
				prop = cardScrapList[i] as Prop;
				for (int j = 0; j < exchangeList.Count; j++) {
					if (isHave(prop.sid,exchangeList[j].getExchangeSample())) {
						newList.Add(exchangeList[j]);
					}
				}
			}
		}
		return ExchangeManagerment.Instance.getSortByCanExchange(newList);
	}

	/// <summary>
	/// 根据碎片SID返回相应卡片
	/// </summary>
	public Card getCardByScrapSid (int propSid)
	{
		if (exchangeList == null) {
			exchangeList = ExchangeManagerment.Instance.getCanUseExchangesCardScrap ();
		}
		for (int i = 0; i < exchangeList.Count; i++) {
			if (isHave(propSid,exchangeList[i].getExchangeSample())) {
				return CardManagerment.Instance.createCard(exchangeList[i].getExchangeSample ().exchangeSid);
			}
		}
		return null;
	}

	public bool isHave(int _sid,ExchangeSample exchange)
	{
		int max = exchange.conditions[0].Length;
		for (int i = 0; i < max; i++) {
			if (exchange.conditions [0][i].costType == PrizeType.PRIZE_PROP) {
				if (_sid == exchange.conditions[0] [i].costSid) {
					return true;
				}
			}
		}
		return false;
	}

	/** 返回数量格式 */
	public string getNumString(ExchangeSample exchange)
	{
		int max = exchange.conditions[0].Length;
		for (int i = 0; i < max; i++) {
			if (exchange.conditions[0] [i].costType == PrizeType.PRIZE_PROP) {
				Prop prop = StorageManagerment.Instance.getProp(exchange.conditions [0][i].costSid);
				if (prop != null) {
					return (prop.getNum() >= exchange.conditions[0] [i].num ? "[418159]": Colors.REDD) + prop.getNum() + "/" + exchange.conditions[0] [i].num;
				} else {
					return Colors.REDD + "0/" + exchange.conditions[0] [i].num;
				}
			}
		}
		return "";
	}
}
