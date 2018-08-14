using UnityEngine;
using System.Collections;

/// <summary>
/// 商店点击按钮
/// </summary>
public class ButtonShopListPerfab : ButtonBase {

	public GameObject tipNumObj;
	public UILabel timeLabel;//时间
	public UITexture bgIcon;//广告图
	public UILabel openLv;
	public ShopListSample sample;
	public GameObject mysticalShopFlag;
	/** 计时器 */
	private Timer timer;
	
	public void updateShopList (ShopListSample sample)
	{

		this.sample = sample; 
		ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.SHOP_LIST + sample.iconId, bgIcon);
		if(UserManager.Instance.self.getUserLevel()<sample.activeLv){
			openLv.gameObject.SetActive(true);
			openLv.text=LanguageConfigManager.Instance.getLanguage("shop07l",sample.activeLv.ToString());
			 this.GetComponent<BoxCollider>().enabled=false;
			bgIcon.color=new Color32(128,128,128,255);
		}else{
			this.GetComponent<BoxCollider>().enabled=true;
			openLv.gameObject.SetActive(false);
			bgIcon.color=new Color32(255,255,255,255);
		}
		updateTime();
        if(sample.shopLag=="shengmi"){
            updateMysticalShow();
        }
        else if (sample.shopLag == "tihui")
        {
            updateTeHuishow();
        }
		timer = TimerManager.Instance.getTimer (UserManager.TIMER_DELAY);
		timer.addOnTimer (refreshData);
		timer.start ();
	}
	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
		if(sample.shopLag=="gongxun"){
			UiManager.Instance.openWindow<MeritShopWindow> ();
		}else if(sample.shopLag=="zuanshi"){
			UiManager.Instance.openWindow<ShopWindow> ((win)=>{
				win.setTitle(sample.name);
				win.init(ShopWindow.TAP_SHOP_CONTENT);
			});
		}else if(sample.shopLag=="shengmi"){
			MysticalShopConfigManager.Instance.saveShowFlagTime("main");
			UiManager.Instance.openWindow<ShopWindow> ((win)=>{
				win.setTitle(sample.name);
				win.init(ShopWindow.TAP_MYSTICAL_CONTENT);
			});
		}else if(sample.shopLag=="tihui"){
			GoodsBuyCountManager.Instance.saveShowFlagTime("showte");
			UiManager.Instance.openWindow<ShopWindow> ((win)=>{
				win.setTitle(sample.name);
				win.init(ShopWindow.TAP_TEHUI_CONTEXT);
			});
		}
	}
	//根据类型获得图标名
	public string getIconName (int type)
	{
		string name = string.Empty;
		switch (type) {
		case PrizeType.PRIZE_RMB:
			name = "";
			break;
		case PrizeType.PRIZE_MONEY:
			name = "";
			break;
		case PrizeType.PRIZE_PROP:
			name = "";
			break;
		}
		return name;
	}
	void refreshData ()
	{
		if (this == null || !gameObject.activeInHierarchy) {
			if (timer != null) {
				timer.stop ();
				timer = null;
			}
			return;
		}
        if (sample.shopLag == "shengmi")
        {
            updateMysticalShow();
            updateTime();
        }
        else if (sample.shopLag == "tihui")
        {
            updateTeHuishow();
        }
		
	}
	void updateTeHuishow(){
		if(UserManager.Instance.self.getUserLevel()>=sample.activeLv&&sample.timeFlag==2&&GoodsBuyCountManager.Instance.isCanShowFlag("showte")){
			mysticalShopFlag.SetActive(true);
		}else mysticalShopFlag.SetActive(false);
	}
	void updateMysticalShow(){
		if(UserManager.Instance.self.getUserLevel()>=sample.activeLv&&sample.timeFlag==1&&MysticalShopConfigManager.Instance.isCanShowFlag("main"))
		mysticalShopFlag.SetActive(true);
		else mysticalShopFlag.SetActive(false);
	}
	void updateTime(){
		if(UserManager.Instance.self.getUserLevel()>=sample.activeLv&&sample.timeFlag==1){
			timeLabel.gameObject.SetActive(true);
			timeLabel.text=LanguageConfigManager.Instance.getLanguage("shop08l",TimeKit.timeTransform(MysticalShopConfigManager.Instance.getNextFlushTime()));
		}else{
			timeLabel.gameObject.SetActive(false);
		}
	}
}
