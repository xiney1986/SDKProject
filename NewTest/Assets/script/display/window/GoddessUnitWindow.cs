using UnityEngine;
using System.Collections;

/// <summary>
/// 女神修炼界面 
/// </summary>
public class GoddessUnitWindow : WindowBase {
	/*filed */
	/** tap容器 */
	public TapContentBase tapContent;
	/** 当前tap下标--1开始 4 8 24小时 */
	int currentTapIndex;
	/**选择的女神 */
	public Card be;
	/**不可用状太 */
	public GameObject closePoint;
	/**女神头像 */
	public UITexture headIcon;
	/**女神头像背景 */
	public GameObject bg;
	/**双倍需要的钻石数量 */
	public UILabel needRMBnum;
	/**增加的经验 */
	public UILabel UI_TipsExpLabel;
	public UILabel oldExpLabel;
	public UILabel oldLvLabel;
	public GameObject expjian;
	public GameObject lvjian;
	public UILabel overLabel;
	public UILabel selectLavel;
	/**双倍复选框 */
	public UIToggle DoubleSelect;
	/**确定按钮 */
	public ButtonBase enterButton;
	/**增加的等级 */
	public UILabel UI_TipsLvLabel;
	/**升级的箭头 */
	public GameObject jiantou;
	/**升级的经验特效 */
	public UILabel UI_ExpValue;
	/**特效移动的位置 */
	public Vector2 UI_ExpValueTweenY;
	/**CD倒计时 */
	public UILabel timeLabel;
	/**可修炼的提示 */
	public UILabel donLabel;
	/**女神选择button */
	public ButtonBase goddessSelectButton;
	public GameObject addFlag;
	/**特效挂接点 */
	public GameObject effectPoint;
	/**是否勾选双倍 默认false */
	private bool isDouble;
	/**奖励的经验 */
	private long mAwardExp;
	/**挂机时长 */
	private int mSelecteTime=0;
	/**是否有经验溢出*/
	private bool mExpOverflow;
	/**能奖励的最大经验 */
	private long mMaxAwardExp;
	/**是否可以升级*/
	private bool mIsUpgrade;
	/** 计时器 */
	private Timer timer;
	/**cd结束的时间 */
	private int cdEndTime;
	private bool tapFalg=false;
	/** begin */
	protected override void begin () {
		base.begin ();
		updateUI();
		timer = TimerManager.Instance.getTimer (UserManager.TIMER_DELAY);
		timer.addOnTimer (refreshData);
		timer.start ();
		MaskWindow.UnlockUI ();

	}
	public void init(int time){
		cdEndTime=time;
	}
	/// <summary>
	/// 刷新时间
	/// </summary>
	void refreshData ()
	{
		if (this == null || !gameObject.activeInHierarchy) {
			if (timer != null) {
				timer.stop ();
				timer = null;
			}
			return;
		}
		updateTime();
		
	}
	/// <summary>
	/// 更新CD倒计时
	/// </summary>
	void updateTime(){
		if(cdEndTime>ServerTimeKit.getSecondTime()){
			timeLabel.gameObject.SetActive(true);
			if(!closePoint.activeSelf)closePoint.SetActive(true);
			if(addFlag.activeInHierarchy)addFlag.SetActive(false);
			if(donLabel.gameObject.activeSelf)donLabel.gameObject.SetActive(false);
			if(!goddessSelectButton.isDisable())goddessSelectButton.disableButton(true);
			timeLabel.text=LanguageConfigManager.Instance.getLanguage("s0376l13",TimeKit.timeTransform(cdEndTime*1000-ServerTimeKit.getSecondTime()*1000));
		}else{
			if(closePoint.activeSelf)closePoint.SetActive(false);
			if(be==null){
				if(!addFlag.activeInHierarchy)addFlag.SetActive(true);
			}else{
				if(addFlag.activeInHierarchy)addFlag.SetActive(false);
			}
			if(!donLabel.gameObject.activeSelf)donLabel.gameObject.SetActive(true);
			timeLabel.gameObject.SetActive(false);
			if(goddessSelectButton.isDisable())goddessSelectButton.disableButton(false);
		}
	}
	/// <summary>
	/// 更新奖励的经验和等级,双倍消耗
	/// </summary>
	public  void updateUI ()
	{
//		if(tapFalg){
//			tapContent.changeTapPage (tapContent.tapButtonList [mSelecteTime]);
//			tapFalg=false;
//		}
		GoddessTrainingSample samplee = GoddessTrainingSampleManager.Instance.getDataBySid (22);
		string[] timeLimitLvv=samplee.TimeLimitLv;
		if(UserManager.Instance.self.getUserLevel()< StringKit.toInt(timeLimitLvv[mSelecteTime])){
			tapContent.changeTapPage (tapContent.tapButtonList [mSelecteTime]);
			return;
		}
		updateTime();
		if(cdEndTime>ServerTimeKit.getSecondTime())goddessSelectButton.disableButton(true);
		else goddessSelectButton.disableButton(false);
		if(be==null){
			lvjian.SetActive(false);
			enterButton.disableButton(true);
			headIcon.gameObject.SetActive(false);
			bg.SetActive(true);
			UI_TipsExpLabel.text="";
			oldExpLabel.text="";
			oldLvLabel.text="";
			expjian.SetActive(false);
			overLabel.text="";
			UI_TipsLvLabel.text="";
			jiantou.SetActive(false);
			selectLavel.text=LanguageConfigManager.Instance.getLanguage("s0376l8");
		}else{
			enterButton.disableButton(false);
			bg.SetActive(false);
			headIcon.gameObject.SetActive(true);
			selectLavel.text=be.getName();
            ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.CARDIMAGEPATH ,be, headIcon);
		}
		GoddessTrainingSample sample = GoddessTrainingSampleManager.Instance.getDataBySid (22);
		string[] timeLimitLv=sample.TimeLimitLv;
		for(int i=0;i<timeLimitLv.Length;i++){//这里是可以作废的 tap按钮上文字变化
			TapButtonBase tap= tapContent.getTapButtonByIndex(i);
			TapButtonBase tapButtonBase=tapContent.getTapButtonByIndex(i);
			if(UserManager.Instance.self.getUserLevel()< StringKit.toInt(timeLimitLv[i])){
				tapButtonBase.doEventButNoActive=true;
				tap.textLabel.text=LanguageConfigManager.Instance.getLanguage("s0043l3",sample.TrainingTime[i]);
				tap.disableButton(false);
			}else{
				tapButtonBase.doEventButNoActive=false;
				tap.textLabel.text=LanguageConfigManager.Instance.getLanguage("s0043l3",sample.TrainingTime[i]);
				tap.disableButton(false);
			}
		}
		needRMBnum.text=LanguageConfigManager.Instance.getLanguage("s037615",sample.TimeRmb [mSelecteTime]);
		if (be == null)
			return;
		Card mCard=be;

		int vipLv = UserManager.Instance.self.vipLevel;
		int lv = mCard.getLevel ();
		mAwardExp = StringKit.toInt (sample.AwardExp [UserManager.Instance.self.getUserLevel () - 1]);
		int trainingTime = StringKit.toInt (sample.TrainingTime [mSelecteTime]);
		mAwardExp *= trainingTime;
		if (isDouble)
			mAwardExp *= 2;
		long exp = mCard.getEXP ();
		float vipAwardExp = vipLv > 0 ? VipManagerment.Instance.getVipbyLevel(vipLv).privilege.expAdd * 0.0001f + 1 : 1;
		mAwardExp = (long)(vipAwardExp * mAwardExp);
		
		//最大能获取的经验值
		long AwardMaxExp = mAwardExp;
		mExpOverflow = false;
		
		long tmpExp = mCard.checkExp (mAwardExp);
		if (tmpExp != -1) {
			AwardMaxExp = tmpExp;
			mExpOverflow = true;
		}
		
		//        mExpOverflow = AwardMaxExp < awardExp + exp;
		mMaxAwardExp = AwardMaxExp;
		mIsUpgrade = mMaxAwardExp >= mCard.getNeedExp ();
		if(mIsUpgrade){
			jiantou.SetActive(true);
		}else{
			jiantou.SetActive(false);
		}
		//UI_TipsExpLabel.text = "+" + mAwardExp;
		oldExpLabel.text=exp.ToString();
		UI_TipsExpLabel.text = (exp+mAwardExp).ToString();
		if (vipLv > 0)
		{
			if (!mExpOverflow)
				//"+" + ((long)(mAwardExp / vipAwardExp))
				UI_TipsExpLabel.text = (exp + ((long)(mAwardExp / vipAwardExp))).ToString();
			UI_TipsExpLabel.text += " x" + (vipAwardExp) + "(VIP" + vipLv.ToString() + ")";
		}
		
		int nextLv = 0;
		
		if (tmpExp != -1) {
			nextLv = EXPSampleManager.Instance.getLevel (mCard.getEXPSid (), tmpExp);
		} else {
			nextLv = EXPSampleManager.Instance.getLevel (mCard.getEXPSid (), mMaxAwardExp + exp);
		}
		//UI_TipsLvLabel.text = string.Format ("LV{0}             LV{1}", lv, nextLv);
		UI_TipsLvLabel.text = "Lv."+nextLv.ToString();
		oldLvLabel.text="Lv."+lv.ToString();
		if(mIsUpgrade){
			lvjian.SetActive(true);
			lvjian.transform.localPosition=UI_TipsLvLabel.transform.localPosition+new Vector3(UI_TipsLvLabel.width+10f,0f,0f);
		}else{
			lvjian.SetActive(false);
		}
		if(mAwardExp>0){
			expjian.SetActive(true);
			expjian.transform.localPosition=UI_TipsExpLabel.transform.localPosition+new Vector3(UI_TipsExpLabel.width+10f,0f,0f);
		}else{
			expjian.SetActive(false);
		}
		if (mExpOverflow) {
			overLabel.text="[ff0000]"+Language ("cardtraining_expOverflow", (exp + mAwardExp) - AwardMaxExp)+"[-]";
//			if(expjian.activeInHierarchy){
//				overLabel.transform.localPosition=expjian.transform.localPosition+new Vector3(36f,0f,0f);
//			}else{
//				overLabel.transform.localPosition=UI_TipsExpLabel.transform.localPosition+new Vector3(UI_TipsExpLabel.width+10f,0f,0f);
//			}
//			UI_TipsExpLabel.text = "[ff0000]" + UI_TipsExpLabel.text + Language ("cardtraining_expOverflow", (exp + mAwardExp) - AwardMaxExp) + "[-]";
//			UI_TipsLvLabel.text = "[ff0000]" + UI_TipsLvLabel.text + "[-]";
			//UI_TipsExpLabel.text = "[ff0000]" + UI_TipsExpLabel.text + Language ("cardtraining_expOverflow", (exp + mAwardExp) - AwardMaxExp) + "[-]";
			//UI_TipsLvLabel.text = "[ff0000]" + UI_TipsLvLabel.text + "[-]";
		}else{
			overLabel.text="";
		}

	//	UI_RmbLabel.text = string.Format (LanguageConfigManager.Instance.getLanguage ("CardTraining_07"), sample.TimeRmb [mSelecteTime]);
	}
	/** button点击事件 */
	public override void buttonEventBase (GameObject gameObj) {
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
			finishWindow ();
		} else if(gameObj.name=="goddessButton"){
			if(cdEndTime>ServerTimeKit.getSecondTime()){
				UiManager.Instance.openDialogWindow<MessageWindow>((window)=>{
					window.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0093"), null, LanguageConfigManager.Instance.getLanguage ("s0111l1"), null);
				});
				return;
			}
			UiManager.Instance.openDialogWindow<GoddessSelectWindow>((win)=>{
				win.init(this);
			});
		}else if(gameObj.name=="enterButton"){
			GoddessTrainingSubmit fport = FPortManager.Instance.getFPort ("GoddessTrainingSubmit") as GoddessTrainingSubmit;
			fport.access (onReceiveInit, isDouble ? 1 : 0, be.uid, 1, mSelecteTime);
		}
	}
	/// <summary>
	/// 挂机完成后回调
	/// </summary>
	private void onReceiveInit (int time)
	{
		CardTrainingWindow window = UiManager.Instance.getWindow<CardTrainingWindow> ();
		cdEndTime=time;
		//window.getCardItem(mLocationIndex).SetCD(time);
		showEffect(mIsUpgrade, mMaxAwardExp);
//		CardTrainingManagerment.Instance.UpdateTime (mLocationIndex, time);
	}
	/** tap点击事件 */
	public override void tapButtonEventBase (GameObject gameObj,bool enable) {
		if (!enable)
			return;
		base.tapButtonEventBase (gameObj,enable);
		GoddessTrainingSample sample = GoddessTrainingSampleManager.Instance.getDataBySid (22);
		string[] timeLimitLv=sample.TimeLimitLv;
		if(UserManager.Instance.self.getUserLevel()< StringKit.toInt(timeLimitLv[int.Parse (gameObj.name)-1])){
			UiManager.Instance.openDialogWindow<MessageLineWindow> ((win) => {
				win.Initialize (LanguageConfigManager.Instance.getLanguage ("s0043l4", sample.TrainingTime[int.Parse (gameObj.name)-1],timeLimitLv[int.Parse (gameObj.name)-1]));
			});
			updateUI ();
			return;
		}
		int tapIndex=int.Parse (gameObj.name)-1 ;
		mSelecteTime=tapIndex;
		updateUI ();

	}
	/** 改变Toggle */
	public void ChangeToggleValue() {
		isDouble=DoubleSelect.value;
		updateUI();
	}
	/// <summary>
	/// 显示特效,+经验, 如果升级有升级特效
	/// </summary>
	public void showEffect(bool isUpgrade, long awardExp)
	{
		UI_ExpValue.text = "+" + awardExp.ToString();
		
		UI_ExpValue.transform.localPosition = new Vector3(0, UI_ExpValueTweenY.x, 0);
		iTween.MoveTo(UI_ExpValue.gameObject, iTween.Hash("position", new Vector3(0, UI_ExpValueTweenY.y, 0), "time", 1f, "islocal", true, "oncomplete", "showEffectComplete", "oncompleteparams", isUpgrade, "oncompletetarget", gameObject));
	}
	private void showEffectComplete(bool isUpgrade)
	{
		UI_ExpValue.text = "";
		StartCoroutine(Utils.DelayRun(() =>
		                              {
			SetData(null);
		}, 2.5f));
		
		if (isUpgrade)
		{
			EffectManager.Instance.CreateEffectCtrlByCache(effectPoint.transform, "Effect/UiEffect/levelupEffect", (obj, ctrl) => { });
		}
	}
	/// <summary>
	/// 设置数据
	/// </summary>
	/// <param name="card"></param>
	public void SetData(Card be)
	{
		this.be = be;
		mAwardExp=0;
		mExpOverflow=false;
		mIsUpgrade=false;
		jiantou.SetActive(false);
		UI_TipsExpLabel.text="";
		UI_TipsLvLabel.text="";
		updateUI();
		MaskWindow.UnlockUI();
	}
}
