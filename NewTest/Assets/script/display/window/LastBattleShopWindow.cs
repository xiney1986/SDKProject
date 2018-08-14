using UnityEngine;
using System.Collections;

public class LastBattleShopWindow : WindowBase
{
	public GameObject goodsButtonPrefab;
	public UILabel lblShopName;
	public UILabel lblJunGong;
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
		ArrayList list = ShopManagerment.Instance.getAllLastBattleGoods ();
		contentGoods.Initialize (list, ContentShopGoods.JUNGONG_SHOP, updateInfo);
		float y = contentGoods.transform.localPosition.y;
		contentGoods.reLoad (list.Count);
		StartCoroutine(Utils.DelayRunNextFrame(()=>{
			contentGoods.jumpToPos(y);
		}));
	}

	public void updateInfo ()
	{
		if (StorageManagerment.Instance.getProp(CommandConfigManager.Instance.lastBattleData.junGongSid) == null)
			lblJunGong.text = "0" + "/" + CommandConfigManager.Instance.lastBattleData.junGongMaxNum;
		else
			lblJunGong.text = StorageManagerment.Instance.getProp(CommandConfigManager.Instance.lastBattleData.junGongSid).getNum() + "/" + CommandConfigManager.Instance.lastBattleData.junGongMaxNum;
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
			finishWindow ();
		}
	}
}
