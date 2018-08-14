using UnityEngine;
using System.Collections;

public class ContentLadderHegoMoney : dynamicContent {

	private ArrayList goods;
	private int type;

	public Card chooseCard;
	private CallBack shopUpdate;
	private NoticeLadderHegeMoneyContent heContent;
	
	public void Initialize (ArrayList goods, int type, CallBack callback,WindowBase father,NoticeLadderHegeMoneyContent notice)
	{
		this.type = type;
		this.goods = goods;
		this.fatherWindow = father;
		this.heContent = notice;
		this.shopUpdate = callback;
	}
	
	public override void updateItem (GameObject item, int index)
	{ 
		ButtonShopGoods button = item.GetComponent<ButtonShopGoods> ();
		button.updateGoods (goods [index] as Goods, shopUpdate,type); 
	}
	
	public override void initButton (int  i)
	{

		if (nodeList [i] == null)
		{
			nodeList [i] = NGUITools.AddChild(gameObject, heContent.laddderHePrefab);
		}

		nodeList [i].name = StringKit. intToFixString (i + 1);
		ButtonShopGoods button = nodeList [i].GetComponent<ButtonShopGoods> ();
		button.fatherWindow = fatherWindow; 
		button.updateGoods (goods [i] as Goods, shopUpdate,type); 
		
	}

	void OnDisable()
	{
		cleanAll();
	}
}
