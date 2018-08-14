using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 已有公会的主界面
 * @author 汤琦
 * */
public class GuildShopWindow : WindowBase {
	
	public ContentShopGoods shopContent;//商店
	public UILabel shopCost;
	public GameObject goodsButtonPrefab;
	protected override void DoEnable () {
		if (MissionManager.instance != null)
			MissionManager.instance.hideAll ();
	}
	
	protected override void begin () {
		base.begin ();
		openGuildShop ();
		MaskWindow.UnlockUI ();
	}
	/** 初始化窗口 */
	public void initWindow () {
		updateShop ();
	}
	/** 更新商店 */
	public void updateShop () {
		ArrayList array = ShopManagerment.Instance.getAllGuildGoods ();
		float y = shopContent.transform.localPosition.y;
		shopContent.Initialize (array, ContentShopGoods.GUILD_SHOP, shopBack);
		shopContent.reLoad (array.Count);
		StartCoroutine (Utils.DelayRunNextFrame (() => {
			shopContent.jumpToPos (y);
		}));
		shopBack ();
	}
	/***/
	public void shopBack () {
		shopCost.text = GuildManagerment.Instance.getGuild ().contributioning.ToString ();
	}
	/***/
	public override void buttonEventBase (GameObject gameObj) {
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
			closeWindow ();
		} 
	}
	/***/
	IEnumerator waitUnlockUI (float time) {
		yield return new WaitForSeconds (time);
		MaskWindow.UnlockUI ();
	}
	/***/
	private void exitBack (MessageHandle msg) {
		if (msg.buttonID == MessageHandle.BUTTON_LEFT)
			return;
		GuildExitFPort fport = FPortManager.Instance.getFPort ("GuildExitFPort") as GuildExitFPort;
		fport.access (closeWindow);
	}
	/****/
	private void closeWindow () {
		finishWindow ();
	}
	private void intoShop () {
		InitShopFPort fport = FPortManager.Instance.getFPort ("InitShopFPort") as InitShopFPort;		
		fport.access (updateShop);
	}
	/***/
	private void openGuildShop () {
		GuildBuildLevelGetFPort fport = FPortManager.Instance.getFPort ("GuildBuildLevelGetFPort") as GuildBuildLevelGetFPort;
		fport.access (intoShop);
	}
	/***/
	public override void DoDisable () {
		base.DoDisable ();
		GuildManagerment.Instance.clearUpdateMsg ();
		if (MissionManager.instance != null) {
			MissionManager.instance.showAll ();
			MissionManager.instance.setBackGround ();
		}
	}
	public override void OnNetResume () {
		base.OnNetResume ();
		initWindow ();
	}
}
