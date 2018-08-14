using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// 星魂碎片商店窗口
/// </summary>
public class StarSoulDebrisShopWindow : WindowBase {

	/** 商品条目预制件 */
	public GameObject goodsButtonPrefab;
	/** 星魂碎片 */
	public UILabel statSoulDebris;
	/** 商店容器 */
	public ContentShopGoods contentGoods;
	
	protected override void begin () {
		InitShopFPort fport = FPortManager.Instance.getFPort ("InitShopFPort") as InitShopFPort;      
		fport.access (getDataSuccess);
		MaskWindow.UnlockUI ();
	}
	/**  */
	void getDataSuccess () {
		updateInfo ();
		reloadShop ();
	}

	/// <summary>
	/// 加载商品
	/// </summary>
	public void reloadShop () {

		float y = contentGoods.transform.localPosition.y;
		ArrayList list = ShopManagerment.Instance.getAllStarSoulDebrisGoods ();
		contentGoods.Initialize (list, ContentShopGoods.STARSOUL_DEBRIS_SHOP, updateInfo);
		contentGoods.reLoad (list.Count);
		StartCoroutine(Utils.DelayRunNextFrame(()=>{
			contentGoods.jumpToPos(y);
		}));
	}
	/// <summary>
	/// 更新数据
	/// </summary>
	public void updateInfo () {
		StarSoulManager manager = StarSoulManager.Instance;
		statSoulDebris.text = Convert.ToString (manager.getDebrisNumber());
	}
	/***/
	public override void buttonEventBase (GameObject gameObj) {
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
			finishWindow ();
		}
	}
	public override void OnNetResume ()
	{
		base.OnNetResume ();
		doOnNetResume();
	}

	public void doOnNetResume()
	{
		(FPortManager.Instance.getFPort ("StarSoulFPort") as StarSoulFPort).getStarSoulInfoAccess (
			()=>{this.begin();});
	}
}
