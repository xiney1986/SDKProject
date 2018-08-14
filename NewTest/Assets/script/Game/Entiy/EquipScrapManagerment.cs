using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 装备碎片管理器
 * @author 陈世惟
 * */
public class EquipScrapManagerment {

	List<Exchange> exchangeList;

	public EquipScrapManagerment ()
	{
		
	}
	
	public static EquipScrapManagerment Instance {
		get{return SingleManager.Instance.getObj("EquipScrapManagerment") as EquipScrapManagerment;}
	}
    /// <summary>
    /// 是否有能够兑换的卡片
    /// </summary>
    /// <returns><c>true</c> if this instance can exchange card; otherwise, <c>false</c>.</returns>
    public bool CanExchangeEq() {
        List<Exchange> list = getEquipScrapList();
        foreach (var tmp in list) {
            int count = ExchangeManagerment.Instance.getCanExchangeNum(tmp);
            if (count > 0)
                return true;
        }
        return false;
    }
	//从道具仓库找到碎片列表，从而确定可显示的兑换列表
	public List<Exchange> getEquipScrapList()
	{
//		return ExchangeManagerment.Instance.getCanUseExchangesEquip (ExchangeType.COMMON);//临时启用
		if (exchangeList == null) {
			exchangeList = ExchangeManagerment.Instance.getCanUseExchangesEquipScrap ();
		}
		List<Exchange> newList = new List<Exchange>();
		ArrayList equipScrapList = StorageManagerment.Instance.getAllPropByEquipScrap();
		
		if (equipScrapList != null && equipScrapList.Count > 0) {
			Prop prop;
			for (int i = 0; i < equipScrapList.Count; i++) {
				prop = equipScrapList[i] as Prop;
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
	/// 根据碎片SID返回相应装备
	/// </summary>
	public Equip getEquipByScrapSid (int propSid)
	{

		if (exchangeList == null) {
			exchangeList = ExchangeManagerment.Instance.getCanUseExchangesEquipScrap ();
		}
		for (int i = 0; i < exchangeList.Count; i++) {
			if (isHave(propSid,exchangeList[i].getExchangeSample())) {
				return EquipManagerment.Instance.createEquip(exchangeList[i].getExchangeSample ().exchangeSid);
			}
		}
		return null;
	}
	
	public bool isHave(int _sid,ExchangeSample exchange)
	{
		int max = exchange.conditions[0].Length;
		for (int i = 0; i < max; i++) {
			if (exchange.conditions[0] [i].costType == PrizeType.PRIZE_PROP) {
				if (_sid == exchange.conditions[0] [i].costSid) {
					return true;
				}
			}
		}
		return false;
	}

	public string getNumString(ExchangeSample exchange)
	{
		int max = exchange.conditions[0].Length;
		for (int i = 0; i < max; i++) {
			if (exchange.conditions [0][i].costType == PrizeType.PRIZE_PROP) {
				Prop prop = StorageManagerment.Instance.getProp(exchange.conditions[0] [i].costSid);
				if (prop != null) {
					return (prop.getNum() >= exchange.conditions[0] [i].num ? Colors.DEEP_GRENN : Colors.RED) + prop.getNum() + "/" + exchange.conditions[0] [i].num;
				} else {
					return Colors.RED + "0/" + exchange.conditions[0] [i].num;
				}
			}
		}
		return "";
	}
}
