using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/**
 * 装备兑换分页
 * @author 汤琦
 **/
public class ContentEquipExchange : dynamicContent 
{
	List<Exchange> exchangeList;
	private int exchangeType;
	
	public void setExchangeType (int type)
	{
		this.exchangeType = type;
	}

	public void Initialize ()
	{
		exchangeList = ExchangeManagerment.Instance.getCanUseExchangesEquip (exchangeType);
		if (exchangeList == null)
			return;
		reLoad (exchangeList.Count);
	}
	
	public void reLoad ()
	{
		exchangeList = ExchangeManagerment.Instance.getCanUseExchangesEquip (exchangeType);
		base.reLoad (exchangeList.Count);
	}
	
	public override void updateItem (GameObject item, int index)
	{
		ExchangeBarCtrl ctrl = item.GetComponent<ExchangeBarCtrl> ();
		ctrl.updateItem (exchangeList [index]);
		
	}
	
	public override void initButton (int  i)
	{
		if (nodeList [i] == null){
			nodeList [i] = NGUITools.AddChild (gameObject, (fatherWindow as ExChangeWindow).exChangeBarPrefab);
		}
		ExchangeBarCtrl ctrl = nodeList [i].GetComponent<ExchangeBarCtrl> ();
		ctrl.fatherWindow = fatherWindow as ExChangeWindow;

	}
}
