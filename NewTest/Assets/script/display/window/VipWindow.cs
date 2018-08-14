using UnityEngine;
using System.Collections;

public class VipWindow : WindowBase
{
	public UITexture icon;
	public UILabel vipIcon;
	public ButtonBase awardButton;
	public UILabel vipLevelLabel;
	public UILabel nextLevelTip;
	public UILabel vipExpLabel;
	public UISprite sprite_vipIcon;
    //**守护天使提示*/
    public GameObject angelTip;
	public barCtrl expbar;
	public GameObject alreadyGetAward;
	public Vip  userVipInfo;
	public SampleDynamicContent sampleContent;
	public UILabel titleLabel;
	public TapContentBase tapContent;
    public UILabel vipNum;//vip可领取的数量
    public UISprite vipBackground;//vip可领取礼包数量背景
	public GameObject audioToggle;
	private int defaultIndex = -1;
	private Vip[] vips;
	private VipPage ActiveShowItem;
	private bool  isBoxShaking =false;
	private UIPanel panel;
	/** 左箭头 */
	public UISprite leftArrow;
	/** 右箭头 */
	public UISprite rightArrow;
	/** 容器当前位置 */
	float nowX;

	protected override void begin ()
	{
		base.begin ();
		vips = VipManagerment.Instance.getAllVip ();
		if(defaultIndex == -1)
			defaultIndex = UserManager.Instance.self.getVipLevel () - 1 < 0 ? 0 : UserManager.Instance.self.getVipLevel () - 1;
		updateInfo ();
		panel = tapContent.GetComponent<UIPanel> ();
		sampleContent.startIndex = defaultIndex;
		sampleContent.maxCount = vips.Length;
		sampleContent.onLoadFinish = onContentFinish;
		sampleContent.callbackUpdateEach = updatePage;
		sampleContent.onCenterItem = updateActivePage;
		sampleContent.init ();
		updateActivePage (sampleContent.getCenterObj ());
        showVipRewardNum();
		isBoxShaking = true;
		boxShakeTimesCtrl ();
		tapContent.changeTapPage (tapContent.tapButtonList [defaultIndex], true);
        GuideManager.Instance.doFriendlyGuideEvent();
		MaskWindow.UnlockUI ();

	}

	void onContentFinish ()
	{
		
	}
	
	void updatePage (GameObject obj)
	{
		//更新当前显示的ShowItem;
		ActiveShowItem = sampleContent.getCenterObj ().GetComponent<VipPage> ();
		VipPage bookitem = obj.GetComponent<VipPage> ();
		int index = StringKit.toInt (obj.name) - 1;
		if (vips==null || index > vips.Length - 1 || index < 0)
			return;
		//不够3页.隐藏
		if (vips == null) {
			return;
		}
		if (bookitem.getVip () != vips [index]) {
			bookitem.updatePage (vips [index]);
		}
	}
	
	void updateActivePage (GameObject obj)
	{
		//更新箭头
		int index = StringKit.toInt (obj.name) - 1;
		if (vips==null || index > vips.Length - 1 || index < 0)
			return;
		ActiveShowItem = obj.GetComponent<VipPage> ();
		if (ActiveShowItem == null)
			return;
		ActiveShowItem.updatePage (vips [index]);
		updateVipAwardInfo ();
		defaultIndex = StringKit.toInt (obj.name) - 1;
	}
	
	public override void OnNetResume ()
	{
		base.OnNetResume ();
		updateInfo ();
		updateVipAwardInfo ();
	}
	/*
	protected void onChangePageHandler (int _currentPageIndex)
	{
		vips = VipManagerment.Instance.getAllVip ();
		vipTitleLabel.text = "VIP " + vips [_currentPageIndex].vipLevel + LanguageConfigManager.Instance.getLanguage ("s0314");
		showGiftLevel = _currentPageIndex + 1;
		updateVipAwardInfo ();
	}

	void Start ()
	{
		radioScrollBar.onChangePage = onChangePageHandler;
		ResourcesManager.Instance.LoadAssetBundleTexture (UserManager.Instance.self.getIconPath (), icon);
		Vip[] vips = VipManagerment.Instance.getAllVip ();
		for (int i = 0; i < vipPages.Length && i < vips.Length; i++) {
			vipPages [i].vipinfo = vips [i];
			vipPages [i].updatePage ();
		}
		int level = UserManager.Instance.self.getVipLevel ();
		if (level > 0) {
			Vector3 currentVipPosition = vipPages [level - 1].transform.localPosition;
			Vector3 currentViewPosition = transform.localPosition;
			currentViewPosition.x = -currentVipPosition.x;
			SpringPanel.Begin (scrollView.gameObject, currentViewPosition, 1000f);
			sprite_vipIcon.gameObject.SetActive (true);
			sprite_vipIcon.spriteName = "vip" + level.ToString ();
			sprite_vipIcon.MakePixelPerfect ();
		} else {
			onChangePageHandler (0);
			sprite_vipIcon.gameObject.SetActive (false);
		}
	}
	*/
	public void updateInfo ()
	{
		UserManager.Instance.setSelfHeadIcon(icon);

		int level = UserManager.Instance.self.getVipLevel ();
		if (level > 0) {
			sprite_vipIcon.gameObject.SetActive (true);
			sprite_vipIcon.spriteName = "vip" + level;
			sprite_vipIcon.MakePixelPerfect ();
		} else {
			sprite_vipIcon.gameObject.SetActive (false);
		}

		float exp = (float)UserManager.Instance.self.getVipEXP () - (float)UserManager.Instance.self.getVipEXPDown ();
		float expNeed = (float)UserManager.Instance.self.getVipEXPUp () - (float)UserManager.Instance.self.getVipEXPDown ();
		
		if (UserManager.Instance.self.getVipLevel () == 0) {
			userVipInfo = VipManagerment.Instance.getVipbyLevel (1);
			vipLevelLabel.text = LanguageConfigManager.Instance.getLanguage ("s0319");
			nextLevelTip.text = LanguageConfigManager.Instance.getLanguage ("s0313", ((int)(expNeed - exp)).ToString (), (UserManager.Instance.self.getVipLevel () + 1).ToString ());
		} else {
			userVipInfo = VipManagerment.Instance.getVipbyLevel (UserManager.Instance.self.vipLevel);	
			//当前级别:VIP 1
			vipLevelLabel.text = LanguageConfigManager.Instance.getLanguage ("s0317") + ":";//VIP " + userVipInfo.vipLevel.ToString ();
			if (VipManagerment.Instance.getMaxLevel () == UserManager.Instance.self.vipLevel) {
				nextLevelTip.text = LanguageConfigManager.Instance.getLanguage ("s0318");
			} else {
				//s0313|再充值 %1元可成为VIP%2
				nextLevelTip.text = LanguageConfigManager.Instance.getLanguage ("s0313", ((int)(expNeed - exp)).ToString (), (UserManager.Instance.self.getVipLevel () + 1).ToString ());
			}
		}
		
		//可能从隐藏回来调用,先重置
		expbar.reset ();
		
		if (expNeed == 0) {
			//满级
			expbar.updateValue (1, 1);
			vipExpLabel.text = (int)UserManager.Instance.self.getVipEXP () + "/" + (int)UserManager.Instance.self.getVipEXPUp ();
		} else {
			expbar.updateValue (UserManager.Instance.self.getVipEXP (), UserManager.Instance.self.getVipEXPUp ());
			vipExpLabel.text = (int)UserManager.Instance.self.getVipEXP () + "/" + (int)UserManager.Instance.self.getVipEXPUp ();
		}
		
		vipIcon.text = "VIP " + UserManager.Instance.self.vipLevel;
	}


    public void showVipRewardNum()
    {
//		Animator ani = audioToggle.GetComponent<Animator> ();
        if (UserManager.Instance.self.getVipLevel() > 5 && PlayerPrefs.GetString(PlayerPrefsComm.ANGEL_USER_NAME + UserManager.Instance.self.uid) == "not")
            angelTip.SetActive(true);
        else
            angelTip.SetActive(false);
        int count = UserManager.Instance.self.vipLevel - VipManagerment.Instance.getAwardSids().Length;
        if (count > 0)
        {
            vipNum.text = count.ToString();
            vipNum.gameObject.SetActive(true);
            vipBackground.gameObject.SetActive(true);
//			if(ani !=null)
//				ani.enabled = true;
        }
        else
        {
            vipNum.gameObject.SetActive(false);
            vipBackground.gameObject.SetActive(false);
//			if(ani !=null)
//				ani.enabled = false;
        }
    }


	void updateVipAwardInfo ()
	{
        //awardButton.transform.localPosition = new Vector3(awardButton.transform.localPosition.x, awardButton.transform.localPosition.y - 11, awardButton.transform.localPosition.z);
		Vip vip = ActiveShowItem.getVip();
		awardButton.textLabel.text = vip.vipAwardName;
        awardButton.textLabel.gameObject.SetActive(false);
		alreadyGetAward.SetActive (false);
		titleLabel.text = "VIP " + vip.vipLevel + LanguageConfigManager.Instance.getLanguage("s0314");
	}
	
//	void getInfoCallBack (int data)
//	{
//		//展示下个礼包
//		showGiftLevel = data > 1 ? data : 1;
//		if (showGiftLevel >= VipManagerment.Instance.getMaxLevel ())
//			showGiftLevel = VipManagerment.Instance.getMaxLevel ();
//		
//		updateVipAwardInfo ();
//		
//	}
	
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj); 
		if (gameObj.name == "close") {
			finishWindow ();
		}
		if (gameObj.name == "awardButton") {
			UiManager.Instance.openWindow<VipAwardViewWindow>();
		}
		if (gameObj.name == "buttonRecharge") {  
			UiManager.Instance.switchWindow<rechargeWindow> ();
		}
		if (gameObj.name == "angelButton") {
			UiManager.Instance.openDialogWindow<DefendingAngelWindow>((win)=>{
				win.init();
			});
		}
	}
	
	bool checkOpenHeroRoad (PrizeSample[] prizes)
	{
		PrizeSample prize;
		for (int i = 0; i < prizes.Length; i++) {
			prize = prizes [i];
			if (prize == null || prize.type != PrizeType.PRIZE_CARD)
				continue;
			Card card = CardManagerment.Instance.createCard (prize.pSid);
			if (HeroRoadManagerment.Instance.activeHeroRoadIfNeed (card))
				return true;
		}
		return false;
	}
	
	//展示界面点击领取
	private void getVipAward ()
	{
		//vip等级允许领取
        //if(true) 当从vipAwardWindow领取时
		if (UserManager.Instance.self.vipLevel >= ActiveShowItem.getVip().vipLevel) {
			VipFPort port = FPortManager.Instance.getFPort ("VipFPort") as VipFPort;
			port.get_gift (() => {
				UiManager.Instance.openDialogWindow<MessageLineWindow> ((win) => {
					win.Initialize (LanguageConfigManager.Instance.getLanguage ("s0120"));
					bool isOpenHeroRoad = checkOpenHeroRoad (VipManagerment.Instance.getVipbyLevel (ActiveShowItem.getVip().vipLevel).prizes);
					if (isOpenHeroRoad)
						win.Initialize (LanguageConfigManager.Instance.getLanguage ("HeroRoad_open"));
				});
				updateVipAwardInfo ();
			}, VipManagerment.Instance.getVipbyLevel (ActiveShowItem.getVip().vipLevel).vipAwardSid);
		} else {
			UiManager.Instance.createMessageLintWindow (LanguageConfigManager.Instance.getLanguage ("s0316"));
		}
	}
	
	void rechargeCallback ()
	{
		UiManager.Instance.openWindow<VipWindow> ((win) => {
			win.updateInfo ();
		});
	}

	public void InitWin(int pageIndex){
		defaultIndex = pageIndex;
	}
	///<summary>
	/// box抖动次数控制
	/// </summary>
	private void boxShakeTimesCtrl(){
		int count = UserManager.Instance.self.vipLevel - VipManagerment.Instance.getAwardSids().Length;
		if(UserManager.Instance.self.vipLevel < 12)
			count = 1;
		Animator ani = audioToggle.GetComponent<Animator> ();
		if (isBoxShaking && count > 0) {
			if (ani != null)
				ani.enabled = true;
			StartCoroutine (Utils.DelayRun (() => {
				isBoxShaking = false;		
				if (ani != null)
					ani.enabled = false;
			}, 12.0f));
		}
		else {
			if (ani != null)
				ani.enabled = false;
		}
	}

	public override void tapButtonEventBase (GameObject gameObj, bool enable)
	{
		if (!enable)
			return;
		
		MaskWindow.LockUI ();
		TapButtonBase[] buttons = tapContent.tapButtonList;
		
		//大于5个才进行居中
		if (buttons.Length > 5) {
			//选中的居中,排除头尾首2个
			if (gameObj != buttons [0].gameObject && gameObj != buttons [1].gameObject &&
			    gameObj != buttons [buttons.Length - 1].gameObject && gameObj != buttons [buttons.Length - 2].gameObject) {
				SpringPanel.Begin (tapContent.gameObject, new Vector3 (-gameObj.transform.localPosition.x, tapContent.transform.localPosition.y, tapContent.transform.localPosition.z), 9);
			}
			else if (gameObj == buttons [buttons.Length - 1].gameObject || gameObj == buttons [buttons.Length - 2].gameObject) {
				GameObject tempObj = buttons [buttons.Length - 3].gameObject;
				SpringPanel.Begin (tapContent.gameObject, new Vector3 (-tempObj.transform.localPosition.x, tapContent.transform.localPosition.y, tapContent.transform.localPosition.z), 9);
			}
			else if (gameObj == buttons [0].gameObject || gameObj == buttons [1].gameObject) {
				GameObject tempObj = buttons [2].gameObject;
				SpringPanel.Begin (tapContent.gameObject, new Vector3 (-tempObj.transform.localPosition.x, tapContent.transform.localPosition.y, tapContent.transform.localPosition.z), 9);
			}
		}

		for(int i = 0;i<buttons.Length;i++){
			if(gameObj == buttons[i].gameObject){
//				sampleContent.startIndex = i;
//				sampleContent.init();
				sampleContent.jumpTo(i);
				break;
			}
		}

		updateArrow ();
		
		MaskWindow.UnlockUI();


	}

	void updateArrow ()
	{
		if (panel != null) {
			if (panel.clipOffset.x >= 330) {
				leftArrow.gameObject.SetActive (true);
				rightArrow.gameObject.SetActive (false);
			}
			else if (panel.clipOffset.x <= 0) {
				leftArrow.gameObject.SetActive (false);
				rightArrow.gameObject.SetActive (true);
			}
			else {
				leftArrow.gameObject.SetActive (true);
				rightArrow.gameObject.SetActive (true);
			}
			nowX = panel.clipOffset.x;
		}
	}
	void Update ()
	{
		if (panel != null && nowX != panel.clipOffset.x) {
			updateArrow ();
		}
	}
//	void Update(){
//		boxShakeTimesCtrl ();
//	}
}
