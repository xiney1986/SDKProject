using UnityEngine;
using System.Collections;

public class ContentShopGoods : dynamicContent {
	private ArrayList goods;
	private int type;
	public const int RMB_SHOP = 1;//RMB商店
	public const int HONOR_SHOP = 2;//荣誉商店
	public const int GUILD_SHOP = 3;//公会商店
	public const int MERIT_SHOP = 4;//功勋商店
	public const int LADDER_SHOP = 7;//天梯争霸
	public const int STARSOUL_DEBRIS_SHOP = 5;//星魂碎片商店
	public const int MYSTICAL_SHOP = 6;//神秘商店
	public const int TEHUI_SHOP=8;//特惠商店
	public const int SUPERDRAW_SHOP = 23;//超级奖池商店
	public const int GODSWAR_SHOP = 24;//超级奖池商店
    public const int HEROSYMBOL_SHOP = 25;//英雄徽记商店
	public const int JUNGONG_SHOP = 26;// 末日决战军功商店//
	public const int STAR_SHOP = 27;// 星屑商店//
	public Card chooseCard;
	private CallBack shopUpdate;
	
	public void Initialize (ArrayList goods, int type, CallBack callback) {
		this.type = type;
		this.goods = goods;
		this.shopUpdate = callback;
	}
	
	public override void updateItem (GameObject item, int index) { 
		ButtonShopGoods button = item.GetComponent<ButtonShopGoods> ();
		button.updateGoods (goods [index] as Goods, shopUpdate, type); 
	}

	public override void initButton (int  i) {
		if (type == RMB_SHOP) {
			if (nodeList [i] == null) {
				nodeList [i] = NGUITools.AddChild (gameObject, (fatherWindow as ShopWindow).goodsButtonPrefab);
			}
		}
		else if (type == GUILD_SHOP) {
			if (nodeList [i] == null) {
				nodeList [i] = NGUITools.AddChild (gameObject, (fatherWindow as GuildShopWindow).goodsButtonPrefab);
			}
		}
		else if (type == MERIT_SHOP) {
			if (nodeList [i] == null) {
				nodeList [i] = NGUITools.AddChild (gameObject, (fatherWindow as MeritShopWindow).goodsButtonPrefab);
			}
		}
		else if (type == STARSOUL_DEBRIS_SHOP) {
			if (nodeList [i] == null) {
				nodeList [i] = NGUITools.AddChild (gameObject, (fatherWindow as StarSoulDebrisShopWindow).goodsButtonPrefab);
			}
		}
		else if (type == MYSTICAL_SHOP) {
			if (nodeList [i] == null) {
				nodeList [i] = NGUITools.AddChild (gameObject, (fatherWindow as ShopWindow).goodsButtonPrefab);
			}
		}
		else if (type == SUPERDRAW_SHOP) {
			if (nodeList [i] == null) {
				nodeList [i] = NGUITools.AddChild (gameObject, (fatherWindow as SuperDrawShopWindow).goodsButtonPrefab);
			}
		}
		else if (type == GODSWAR_SHOP) {
			if (nodeList [i] == null) {
				nodeList [i] = NGUITools.AddChild (gameObject, (fatherWindow as GodsWarShopWindow).goodsButtonPrefab);
			}
        } else if (type == HEROSYMBOL_SHOP) {
            if (nodeList[i] == null) {
                nodeList[i] = NGUITools.AddChild(gameObject, (fatherWindow as HuiJiShopWindow).goodsButtonPrefab);
            }
		}else if (type == JUNGONG_SHOP) {
			if (nodeList[i] == null) {
				nodeList[i] = NGUITools.AddChild(gameObject, (fatherWindow as LastBattleShopWindow).goodsButtonPrefab);
			}
		}
		else if (type == STAR_SHOP) {
			if (nodeList[i] == null) {
				nodeList[i] = NGUITools.AddChild(gameObject, (fatherWindow as StarShopWindow).goodsButtonPrefab);
			}
		}
		nodeList [i].name = StringKit.intToFixString (i + 1);
		ButtonShopGoods button = nodeList [i].GetComponent<ButtonShopGoods> ();
		button.fatherWindow = fatherWindow; 
		button.updateGoods (goods [i] as Goods, shopUpdate, type); 

	}
	void OnDisable () {
		cleanAll ();
	}
}

