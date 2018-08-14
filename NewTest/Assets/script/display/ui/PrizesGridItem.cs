using UnityEngine;
using System.Collections;

/// <summary>
/// 奖品网格标准显示goods
/// </summary>
public class PrizesGridItem : MonoBase {

	/* gameobject fields */
	/** goods视图预制 */
	public GameObject goodsViewPrefab;
	/** 标题 */
	public UILabel titleLabel;
	/** 网格 */
	public UIGrid content;
	/** 领取奖励按钮 */
	public ButtonBase awardButton;

	/* fields */
	/** 父窗口 */
	WindowBase win;
	/** 标题 */
	string titleText;
	/** 展示的对象 */
	object showObj;
	/** 展示的奖品 */
	PrizeSample[] prizes;
	/** 领取奖励回调 */
	CallBack<PrizesGridItem,object> awardCallBack;

	/* methods */
	/** 初始化 */
	public void init(object showObj,PrizeSample[] prizes,WindowBase win,CallBack<PrizesGridItem,object> callBack) {
		init(showObj,prizes,win,callBack,"");
	}
	/** 初始化 */
	public void init(object showObj,PrizeSample[] prizes,WindowBase win,CallBack<PrizesGridItem,object> callBack,string titleText) {
		this.win = win;
		this.showObj = showObj;
		this.prizes = prizes;
		this.titleText = titleText;
		this.awardCallBack = callBack;
		initButtonInfo ();
		UpdateUI ();
	}
	/** 初始化button信息 */
	public void initButtonInfo() {
		awardButton.fatherWindow = win;
		awardButton.onClickEvent = HandleAwardButton;
	}
	/** 更新ui */
	public void UpdateUI() {
		UpdateTitle ();
		UpdateAwardButton ();
		createGoodsViewByAll ();
	}
	/** 获取奖励 */
	public PrizeSample[] getPrizes(){
		return prizes;

	}
	/** 更新领取奖励 */
	private void UpdateAwardButton() {
		bool isAward = true;
		if (showObj is Vip) { // 先这样写吧,下次想办法把这玩意儿抽出去写
			HandleVipAwardButton();
		} else{
			awardButton.textLabel.text = LanguageConfigManager.Instance.getLanguage ("recharge02");
			awardButton.disableButton (true);
		}
	}
	/** vip情况处理 */
	private void HandleVipAwardButton(){
		Vip vip=showObj as Vip;
		bool isAward = VipManagerment.Instance.alreadyGetAward (vip.vipAwardSid);
		if (isAward) {
			awardButton.textLabel.text = LanguageConfigManager.Instance.getLanguage ("recharge02");
		} else {
			awardButton.textLabel.text = LanguageConfigManager.Instance.getLanguage ("quiz09");
		}
		bool isVipLevel=UserManager.Instance.self.vipLevel < vip.vipLevel;
		if(isVipLevel||isAward){
			awardButton.disableButton (true);
		} else{
			awardButton.disableButton (false);
		}
	}
	/** 更新条目标题 */
	private void UpdateTitle() {
		if (titleLabel != null) {
			titleLabel.text=titleText;
		}
	}
	/** 创建goods视图 */
	private void createGoodsViewByAll() {
		if (content.gameObject.transform.childCount > 0) {
			Utils.RemoveAllChild(content.gameObject.transform.transform);		
		}
		if (prizes != null) {
			for (int i = 0; i < prizes.Length; i++) {
				createGoodsView(prizes[i],i);
			}
		}
		StartCoroutine(Utils.DelayRunNextFrame(()=>{
			content.repositionNow = true;
		}));	
	}
	/// <summary>
	/// 创建goods视图
	/// </summary>
	/// <param name="item">显示条目</param>
	/// <param name="index">Index.</param>
	private void createGoodsView(PrizeSample item,int index) {
		if (item == null)
			return;
		GameObject gameobj = NGUITools.AddChild (content.gameObject, goodsViewPrefab);
		gameobj.transform.localScale = new Vector3 (0.7f,0.7f,1);
		gameobj.name = StringKit.intToFixString (index + 1);
		GoodsView goodsButton = gameobj.GetComponent<GoodsView> ();
		goodsButton.fatherWindow = win;
		goodsButton.onClickCallback = goodsButton.DefaultClickEvent;
		goodsButton.init(item);
	}
	/** 处理领取奖励 */
	private void HandleAwardButton(GameObject gameObj) {
		if(awardCallBack!=null) {
			awardCallBack(this,showObj);
		}
		MaskWindow.UnlockUI ();
	}
}
