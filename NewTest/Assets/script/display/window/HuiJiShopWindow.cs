using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 恶魔商店
 * */
public class HuiJiShopWindow : WindowBase {
	
	public ContentShopGoods shopContent;//商店
	public UILabel shopCost;
	public GameObject goodsButtonPrefab;
	public CallBack callback;

    protected override void begin() {
        base.begin();
        intoShop();
        MaskWindow.UnlockUI();
    }
    /** 初始化窗口 */
    public void initWindow(CallBack callback) {
        this.callback = callback;
        //updateShop ();
    }
    /** 更新商店 */
    public void updateShop() {

        //shopCost.text = num.ToString();
        int sid = GoodsSampleManager.Instance.getGoodsSampleBySid(GoodsSampleManager.Instance.getAllShopGoods(ShopType.HEROSYMBOL_SHOP)[0]).costToolSid;
        if (StorageManagerment.Instance.getProp(sid) == null)
            shopCost.text = "0" + "/" + CommandConfigManager.Instance.getMaxNum();
        else
            shopCost.text = StorageManagerment.Instance.getProp(sid).getNum() + "/" + CommandConfigManager.Instance.getMaxNum();
        ArrayList goodlists = ShopManagerment.Instance.getAllBossAttackGoods();
        //goodlists.Sort (new MysticalComp ());
        float y = shopContent.transform.localPosition.y;
        shopContent.Initialize(goodlists, ContentShopGoods.HEROSYMBOL_SHOP, shopBack);
        shopContent.reLoad(goodlists.Count);
        StartCoroutine(Utils.DelayRunNextFrame(() => {
            shopContent.jumpToPos(y);
        }));
    }

    public override void OnNetResume() {
        base.OnNetResume();
        intoShop();
    }
    public void shopBack() {
        //shopCost.text = GuildManagerment.Instance.getGuild ().contributioning.ToString ();
    }

    public override void buttonEventBase(GameObject gameObj) {
        base.buttonEventBase(gameObj);
        if (gameObj.name == "close") {
            closeWindow();
            if (callback != null)
                callback();
        }
    }

    private void intoShop() {
        InitShopFPort fport = FPortManager.Instance.getFPort("InitShopFPort") as InitShopFPort;
        fport.access(updateShop);
    }

    IEnumerator waitUnlockUI(float time) {
        yield return new WaitForSeconds(time);
        MaskWindow.UnlockUI();
    }

    private void closeWindow() {
        finishWindow();
    }
}
