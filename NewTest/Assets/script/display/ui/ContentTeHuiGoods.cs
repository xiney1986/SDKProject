using UnityEngine;
using System.Collections;

/// <summary>
/// 特惠商店动态容器
/// </summary>
public class ContentTeHuiGoods : dynamicContent {

	private ArrayList goods;
	private int type;
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
		ButtonTeHuiGoods button = item.GetComponent<ButtonTeHuiGoods> ();
		button.updateGoods (goods [index] as Goods, shopUpdate,type); 
	}
	
	public override void initButton (int  i)
	{
		if (type == ContentShopGoods.TEHUI_SHOP)
		{
			if (nodeList [i] == null)
			{
				nodeList [i] = NGUITools.AddChild(gameObject, (fatherWindow as ShopWindow).tehuiButtonPerfab);
			}
		}
		nodeList [i].name = StringKit. intToFixString (i + 1);
		ButtonTeHuiGoods button= nodeList [i].GetComponent<ButtonTeHuiGoods> ();
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
