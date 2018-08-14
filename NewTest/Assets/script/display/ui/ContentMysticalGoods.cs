using UnityEngine;
using System.Collections;

public class ContentMysticalGoods : dynamicContent {

	private ArrayList goods;
	private int type;
	public const int RMB_SHOP = 1;//RMB商店
	public const int HONOR_SHOP = 2;//荣誉商店
	public const int GUILD_SHOP = 3;//公会商店
	public const int MERIT_SHOP = 4;//功勋商店
	public const int STARSOUL_DEBRIS_SHOP = 5;//星魂碎片商店
	public const int MYSTICAL_SHOP=6;//神秘商店
	public Card chooseCard;
	private CallBack shopUpdate;
	
	public void Initialize (ArrayList goods, int type, CallBack callback)
	{
		this.type = type;
		this.goods = goods;
		//当玩家小于21级的引导时，主动隐藏掉第一条置顶的购买项目
//		if (GuideManager.Instance.isLessThanStep (132001000)) {
//			goods.RemoveAt (0);
//		}
		this.shopUpdate = callback;
		//	base.Initialize (this.goods.Count); 
	}
	
	public override void updateItem (GameObject item, int index)
	{ 
		ButtonMysticalGoods button = item.GetComponent<ButtonMysticalGoods> ();
		button.updateGoods (goods [index] as Goods, shopUpdate,type); 
	}
	
	public override void initButton (int  i)
	{
		 if (type == MYSTICAL_SHOP)
		{
			if (nodeList [i] == null)
			{
				nodeList [i] = NGUITools.AddChild(gameObject, (fatherWindow as ShopWindow).mysticalButtonPrefab);
			}
		}
		nodeList [i].name = StringKit. intToFixString (i + 1);
		ButtonMysticalGoods button= nodeList [i].GetComponent<ButtonMysticalGoods> ();
		button.gameObject.transform.localScale=new Vector3(0.8f,0.8f,1f);
		button.fatherWindow = fatherWindow; 
		button.updateGoods (goods [i] as Goods, shopUpdate,type); 
		
	}
	void OnDisable()
	{
		cleanAll();
	}

	public override void jumpToPage (int index) {
		base.jumpToPage (index);
	}
}
