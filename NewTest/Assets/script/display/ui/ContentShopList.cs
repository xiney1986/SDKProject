using UnityEngine;
using System.Collections;

/// <summary>
/// 商店列表容器
/// </summary>
public class ContentShopList : dynamicContent {
	/*filed */
	/**商店种类列表 */
	ShopListSample[] sample;
	/**预制体 */
	public GameObject luckyDrawBarPrefab;
	public void	reLoad ()
	{ 
		sample =ShopListSamleManager.Instance.getAllShop().ToArray();
		base.reLoad (sample.Length);
	}
	public override void  updateItem (GameObject item, int index)
	{
		ButtonShopListPerfab button = item.GetComponent<ButtonShopListPerfab> ();				 
		button.updateShopList (sample [index]);
		
	}
	
	public override void initButton (int  i)
	{
		if (nodeList [i] == null){
			nodeList [i] = NGUITools.AddChild (gameObject, luckyDrawBarPrefab);
		}
		ButtonShopListPerfab button = nodeList [i].GetComponent<ButtonShopListPerfab> ();
		button.fatherWindow = fatherWindow;
		button.updateShopList (sample [i]);
	}

	public override void jumpToPage (int index) {
		base.jumpToPage (index);
		if (GuideManager.Instance.isEqualStep (132003000)) {
			GuideManager.Instance.guideEvent ();
		}
	}
}
