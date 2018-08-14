using UnityEngine;
using System.Collections;

public class ShopWindow : WindowBase
{
	/* const */
	/** 容器下标常量 */
	public const int TAP_SHOP_CONTENT=0, // 钻石商城
	TAP_MYSTICAL_CONTENT=1, // 神秘商城
	TAP_TEHUI_CONTEXT=2;//特惠商店
	/* fields */
	/** 预制件容器数组-钻石商城,神秘商城*/
	public GameObject[] contentPrefabs;
	/**预制件挂接点 */
	public GameObject[] contentPoints;
	/** 当前tap下标--0开始 */
	int currentTapIndex=0;
	public Goods goods;
	public GameObject  goodsButtonPrefab;
	public GameObject mysticalButtonPrefab;
	public GameObject tehuiButtonPerfab;
	private CallBack callback;
	public UILabel titleText;
	
	
	protected override void begin ()
	{
		base.begin ();
		if(!isAwakeformHide) {
			initContent(currentTapIndex);
		} else{
			UpdateContent();
		}
		MaskWindow.UnlockUI ();
	}
	public override void OnNetResume ()
	{
		base.OnNetResume ();
        initContent(currentTapIndex);
	}
	public void init(int index){
		this.currentTapIndex = index;			
	}
	public void setCallBack (CallBack callback)
	{
		this.callback = callback;
	}
	/** 更新节点容器 */
	public void UpdateContent() {
		GameObject content = getContent (currentTapIndex);
		if(currentTapIndex==TAP_SHOP_CONTENT){
			titleText.text=LanguageConfigManager.Instance.getLanguage("shop02");
			ContentShopPoint csp=content.GetComponent<ContentShopPoint>();
			csp.updateUI();
		}else if(currentTapIndex==TAP_MYSTICAL_CONTENT){
			titleText.text=LanguageConfigManager.Instance.getLanguage("shop_mystical");
			ContentMysticalShop cms = content.GetComponent<ContentMysticalShop> ();
			cms.updateUI();
		}else if(currentTapIndex==TAP_TEHUI_CONTEXT){
			titleText.text=LanguageConfigManager.Instance.getLanguage("shop_tehui");
			ContentTeHuiShop cmss = content.GetComponent<ContentTeHuiShop> ();
			cmss.updateUI();
		}
		
	}
	/// <summary>
	/// 初始化容器
	/// </summary>
	/// <param name="tapIndex">Tap index.</param>
	public void initContent(int tapIndex){
		resetContentsActive();
		GameObject content = getContent (tapIndex);
		switch (tapIndex) {
		case TAP_SHOP_CONTENT:
			titleText.text=LanguageConfigManager.Instance.getLanguage("shop02");
			ContentShopPoint csp = content.GetComponent<ContentShopPoint> ();
			csp.init(this);
			break;
		case TAP_MYSTICAL_CONTENT:
			titleText.text=LanguageConfigManager.Instance.getLanguage("shop_mystical");
			ContentMysticalShop cms = content.GetComponent<ContentMysticalShop> ();
			cms.init(this);
			break;
		case TAP_TEHUI_CONTEXT:
			titleText.text=LanguageConfigManager.Instance.getLanguage("shop_tehui");
			ContentTeHuiShop cmss = content.GetComponent<ContentTeHuiShop> ();
			cmss.init(this);
			break;
		}
        if (GuideManager.Instance.isEqualStep(132003000))
        {
            GuideManager.Instance.doGuide();
            GuideManager.Instance.guideEvent();
        }
	}
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj); 
		if (gameObj.name == "close") {
			if (callback != null) {
				callback ();
				callback = null;
				return;
			}
			finishWindow ();
		}else{
			GameObject content = getContent (currentTapIndex);
			if (currentTapIndex == TAP_SHOP_CONTENT) {
				ContentShopPoint sshc = content.GetComponent<ContentShopPoint> ();
				sshc.buttonEventBase(gameObj);
			} else if (currentTapIndex == TAP_MYSTICAL_CONTENT) {
				ContentMysticalShop ssec = content.GetComponent<ContentMysticalShop> ();
				ssec.buttonEventBase(gameObj);
			}
		}
	}
	

	/// <summary>
	/// 获取指定下标的容器
	/// </summary>
	/// <param name="contentPoint">容器点</param>
	/// <param name="tapIndex">下标</param>
	private GameObject getContent(int tapIndex) {
		GameObject contentPoint = contentPoints [tapIndex];
		contentPoint.SetActive (true);
		GameObject content;
		if (contentPoint.transform.childCount > 0) {
			Transform childContent=contentPoint.transform.GetChild (0);
			content = childContent.gameObject;
		} else {
			content = NGUITools.AddChild (contentPoint, contentPrefabs[tapIndex]);
		}
		return content;
	}
	/** 重置容器激活状态 */
	private void resetContentsActive() {
		foreach (GameObject item in contentPoints) {
			item.SetActive(false);
		}
	}
}
