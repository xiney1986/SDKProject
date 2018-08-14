using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 超级奖池兑换窗口
 * */
public class SuperDrawShopWindow : WindowBase {
	
	public ContentShopGoods shopContent;//商店
	public UILabel shopCost;
	public GameObject goodsButtonPrefab;
	public CallBack callback;
	Notice notice;
	private int num;
		
	protected override void begin () {
		base.begin ();
		MaskWindow.UnlockUI ();
	}
    protected override void DoEnable() {
        base.DoEnable();
        intoShop();
    }
	/** 初始化窗口 */
	public void initWindow (Notice notice,int num,CallBack callback) {
		intoShop();
		this.notice = notice;
		this.num = num;
		this.callback = callback;
		//updateShop ();
	}
	/** 更新商店 */
	public void updateShop () {

		shopCost.text = SuperDrawManagerment.Instance.getPropSumBySid(SuperDrawManagerment.Instance.propSid).ToString();
		//ArrayList array = ShopManagerment.Instance.getAllSuperDrawGoods ();
		//这里根据活动来区分具体兑换条目
		int[] sids = (notice.getSample().content as SidNoticeContent).sids;
		ArrayList goodlists = ShopManagerment.Instance.getSuperSidGoods (sids[0]);
		goodlists.Sort (new MysticalComp ());
		float y = shopContent.transform.localPosition.y;
		shopContent.Initialize (goodlists, ContentShopGoods.SUPERDRAW_SHOP, shopBack);
		shopContent.reLoad (goodlists.Count);
		StartCoroutine (Utils.DelayRunNextFrame (() => {
			shopContent.jumpToPos (y);
		}));
		shopBack ();
	}

	public override void OnNetResume ()
	{
		base.OnNetResume ();
		intoShop();
	}
	public void shopBack () {
		//shopCost.text = GuildManagerment.Instance.getGuild ().contributioning.ToString ();
	}

	public override void buttonEventBase (GameObject gameObj) {
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
			closeWindow ();
			if(callback!=null)
				callback();
		} 
	}

	private void intoShop () {
		InitShopFPort fport = FPortManager.Instance.getFPort ("InitShopFPort") as InitShopFPort;		
		fport.access (updateShop);
	}

	IEnumerator waitUnlockUI (float time) {
		yield return new WaitForSeconds (time);
		MaskWindow.UnlockUI ();
	}

	private void closeWindow () {
		finishWindow ();
	}
}
