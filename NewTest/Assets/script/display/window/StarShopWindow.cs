using UnityEngine;
using System.Collections;

public class StarShopWindow : WindowBase
{
	public GameObject goodsButtonPrefab;
	public UILabel lblShopName;
	// 星屑//
	public UILabel starCount;
	public ContentShopGoods contentGoods;

	protected override void begin ()
	{
		InitShopFPort fport = FPortManager.Instance.getFPort ("InitShopFPort") as InitShopFPort;      
		fport.access (getDataSuccess);
		MaskWindow.UnlockUI ();
	}

	// 断线重连//
	public override void OnNetResume ()
	{
		base.OnNetResume ();
		InitShopFPort fport = FPortManager.Instance.getFPort ("InitShopFPort") as InitShopFPort;      
		fport.access (getDataSuccess);
	}

	void getDataSuccess ()
	{
		updateInfo ();
		reloadShop ();
	}


	public void reloadShop ()
	{
		ArrayList list = ShopManagerment.Instance.getAllStarGoods ();
		contentGoods.Initialize (list, ContentShopGoods.STAR_SHOP, updateInfo);
		float y = contentGoods.transform.localPosition.y;
		contentGoods.reLoad (list.Count);
		StartCoroutine(Utils.DelayRunNextFrame(()=>{
			contentGoods.jumpToPos(y);
		}));
	}

	public void updateInfo ()
	{
		starCount.text = GoddessAstrolabeManagerment.Instance.getStarScore().ToString();
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
			finishWindow ();
		}
	}
}
