using UnityEngine;
using System.Collections;

public class ContentNvShenShopGoods : dynamicContent {
	private ArrayList goods;
	private int type;
	public Card chooseCard;
	private CallBack shopUpdate;
	
	public void Initialize (ArrayList goods, CallBack callback) {
		this.goods = goods;
		this.shopUpdate = callback;
	}
	
	public override void updateItem (GameObject item, int index) {
        ButtonNvShenShopGoods button = item.GetComponent<ButtonNvShenShopGoods>();
		button.updateGoods (goods [index] as Goods, shopUpdate); 
	}

	public override void initButton (int  i) 
    {
        if (nodeList[i] == null) {
            //注意fatherWindow一定要有值
            nodeList[i] = NGUITools.AddChild(this.gameObject, (fatherWindow as NvshenShopWindow).nvShenButtonPrefab);  
        }
		nodeList [i].name = StringKit.intToFixString (i + 1);
        ButtonNvShenShopGoods button = nodeList[i].GetComponent<ButtonNvShenShopGoods>();
		button.fatherWindow = fatherWindow;
		button.updateGoods (goods [i] as Goods, shopUpdate); 

	}
	void OnDisable () {
		cleanAll ();
	}
}

