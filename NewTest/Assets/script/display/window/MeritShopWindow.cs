using UnityEngine;
using System.Collections;

/// <summary>
/// 功勋商店窗口
/// yxl
/// </summary>
public class MeritShopWindow : WindowBase
{
	public GameObject goodsButtonPrefab;
	public UILabel lblShopName;
	public UILabel lblMerit;
	public ContentShopGoods contentGoods;

	protected override void begin ()
	{
		InitShopFPort fport = FPortManager.Instance.getFPort ("InitShopFPort") as InitShopFPort;      
		fport.access (getDataSuccess);
		MaskWindow.UnlockUI ();
	}

	void getDataSuccess ()
	{
		updateInfo ();
		reloadShop ();
	}


	public void reloadShop ()
	{
		ArrayList list = ShopManagerment.Instance.getAllMeritGoods ();
		contentGoods.Initialize (list, ContentShopGoods.MERIT_SHOP, updateInfo);
		float y = contentGoods.transform.localPosition.y;
		contentGoods.reLoad (list.Count);
		StartCoroutine(Utils.DelayRunNextFrame(()=>{
			contentGoods.jumpToPos(y);
		}));
	}

	public void updateInfo ()
	{
		lblMerit.text = UserManager.Instance.self.merit + "";
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
			finishWindow ();
		}
	}
}
