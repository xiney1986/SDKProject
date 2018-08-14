using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 秘宝碎片管理器
 * @author liwei
 * */
public class MagicWeaponScrapManagerment {

	List<Exchange> exchangeList;

	public MagicWeaponScrapManagerment ()
	{
		
	}
	
	public static MagicWeaponScrapManagerment Instance {
        get { return SingleManager.Instance.getObj("MagicWeaponScrapManagerment") as MagicWeaponScrapManagerment; }
	}

	//从道具仓库找到碎片列表，从而确定可显示的兑换列表
	public List<Exchange> getMagicWeaponScrapList()
	{
		if (exchangeList == null) {
            exchangeList = ExchangeManagerment.Instance.getCanUseExchangesWeaponScrap();
		}
		List<Exchange> newList = new List<Exchange>();
        ArrayList magicWeaponScrapList = StorageManagerment.Instance.getAllPropByMagicScrap();

        if (magicWeaponScrapList != null && magicWeaponScrapList.Count > 0) {
			Prop prop;
            for (int i = 0; i < magicWeaponScrapList.Count; i++) {
                prop = magicWeaponScrapList[i] as Prop;
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
    public MagicWeapon getMagicWeaponByScrapSid(int propSid)
	{
		if (exchangeList == null) {
            exchangeList = ExchangeManagerment.Instance.getCanUseExchangesWeaponScrap();
		}
		for (int i = 0; i < exchangeList.Count; i++) {
			if (isHave(propSid,exchangeList[i].getExchangeSample())) {
                return MagicWeaponManagerment.Instance.createMagicWeapon(exchangeList[i].getExchangeSample().exchangeSid);
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
					return (prop.getNum() >= exchange.conditions[0] [i].num ? Colors.GRENN : Colors.RED) + prop.getNum() + "/" + exchange.conditions[0] [i].num;
				} else {
					return Colors.RED + "0/" + exchange.conditions[0] [i].num;
				}
			}
		}
		return "";
	}
    public Prop getNeedProp(ExchangeSample exchange) {
        int max = exchange.conditions[0].Length;
        for(int i=0;i<max;i++){
            if (exchange.conditions [0][i].costType == PrizeType.PRIZE_PROP) {
				return StorageManagerment.Instance.getProp(exchange.conditions[0] [i].costSid);
            }
        }
        return null;
    }

	public int canGetMagicWeaponCount(List<Exchange> _scrapList)
	{
		int _count = 0;
		ExchangeSample es = null;
		int scrapCount = 0;
		int needCount = 0;
		int max = 0;
		Prop prop = null;
		for (int i = 0; i < _scrapList.Count; i++)
		{
			es = _scrapList[i].getExchangeSample();
			max = es.conditions[0].Length;
			for (int j = 0; j < max; j++) {
				if (es.conditions [0][j].costType == PrizeType.PRIZE_PROP) {
					prop = StorageManagerment.Instance.getProp(es.conditions[0] [j].costSid);
					if(prop.getNum() >= es.conditions[0] [j].num)
					{
						_count += prop.getNum()/es.conditions[0] [j].num;
					}
				}
			}
		}
		
		return _count;
	}
}
