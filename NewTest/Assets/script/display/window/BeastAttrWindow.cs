using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//召唤兽属性窗口
//李程
public class BeastAttrWindow : WindowBase
{
	public SampleDynamicContent sampleContent;
	Card selectedCard;
	BeastEvolve selectedEvolve;
	public Card oldCard;
	public barCtrl expbar;//等级经验条
	public UILabel level;//等级
	public UITexture mainSkillIcon;//主技能图标
	public UILabel mainSkill;//主技能名称
	public UILabel mainSkillLevel;//主技能等级
	public UILabel mainSkillDescript;//主技能详细属性
	public UITexture featuresIcon;//特性图标
	public UILabel featuresText;//特性名称
	public UILabel featuresDescript;//特性详细属性
	public UILabel  combat;
	public UILabel  hp;
	public UILabel  mag;
	public UILabel  att;
	public UILabel  dex;
	public UILabel  def;
	public UILabel  jinhua;
	public UISprite quality;
	/**召唤条件显示那一部分 */
	public GameObject conditonPoint;//条件信息挂节点
	public GameObject combatPoint;//战斗力挂接点
	public GameObject inofPoint;//属性挂接点
	public UILabel nvshengLabel;//星座名
	public UILabel nvshengDayLabel;//星座日期
	public UISprite nvshengSprite;//星座图标
	public ButtonExchange propButton;
	public Transform beastEffectPoint;
	public flyItemCtrl flyItem;
	public UILabel needMoneyLabel;//需要金币label
	public const int CARDCHANGE = 2;//队伍编辑点召唤兽出现的页面
	public const int STOREVIEW = 3;//召唤兽仓库浏览页面
	public const int TEMPSTORE = 4;//临时仓库
	public const int FUBEN = 5;//副本
	public const int RESONANCE = 6;//原共鸣窗口,现在女神屏风
	public const int FRIEND = 7;//好友
	int showType;
	public ButtonBase otherButton;//0036替换、0047召唤、0092进化、0035上阵
	public ButtonBase beastSummonButton;//进化按钮,召唤兽满级后显示
	public Transform leftPoint;
	public Transform centerPoint;
    public Card evolveCard = null; //进化前的卡片

	public GameObject leftArrow;
	public GameObject rightArrow;
	ExchangeSample sample;
	private int mainSkillLv;//主技能等级
	private long oldExp;//圣器经验
	private int day;//圣器时间 
	private int count;//圣器免费次数

	public AudioSource audioSource;//女神语音
	[HideInInspector]public bool iscacheAudioOK = false;//是否加载好资源
	[HideInInspector]public ButtonBeast ActiveShowItem;

	private bool stopMusic;
	private CallBack callback;
	private int defaultIndex;
	private List<BeastEvolve> beastList;
	private int storageVersion = -1;
	/** 需求功勋数量 */
	private int needPropNum;

	private string titlename;

	public string titleName
	{
		get {
			return titlename;
		}
		set {
			titlename = value;
		}
	}

	public void Initialize ()
	{ 
		Initialize (null, STOREVIEW);
	}
	public void Initialize (List<BeastEvolve> bo,int type,Card chooseItem)
	{ 
		showType = type;
		
		beastList = bo;
		
		bool isHave = false;
		iscacheAudioOK = false;
		
		if(type != FRIEND)
		{
			if (type == CARDCHANGE || type == FUBEN)
				isHave = true;
		}
		
		if (isHave) {
			List<BeastEvolve> list = new List<BeastEvolve> ();
			for (int i = 0; i < beastList.Count; i++) {
				if (beastList [i].isAllExist ())
					list.Add (beastList [i]);
			}
			beastList = list;						
		}
		
		if (chooseItem != null) {
			for (int i = 0; i < beastList.Count; i++) {
				if (beastList [i].getBeast().sid == chooseItem.sid) {
					defaultIndex = i;
					break;
				}
			}
		}
	}
	
	public void Initialize (Card showCard, int type, CallBack closeCallback)
	{
		this.callback = closeCallback;
		Initialize(showCard,showType);
	}
	
	public void Initialize (Card chooseItem, int type)
	{
		showType = type;
		UiManager.Instance.backGround.switchBackGround("battleMap_11");
		beastList = BeastEvolveManagerment.Instance.getAllBest ();
		
		bool isHave = false;
		iscacheAudioOK = false;
		
		if(type != FRIEND)
		{
			if (type == CARDCHANGE || type == FUBEN)
				isHave = true;
		}
		
		if (isHave) {
			List<BeastEvolve> list = new List<BeastEvolve> ();
			for (int i = 0; i < beastList.Count; i++) {
				if (beastList [i].isAllExist ())
					list.Add (beastList [i]);
			}
			beastList = list;						
		}

		if (chooseItem != null) {
			for (int i = 0; i < beastList.Count; i++) {
				if (beastList [i].getBeast().sid == chooseItem.sid) {
					defaultIndex = i;
					break;
				}
			}
		}
	}

	protected override void begin ()
	{
		base.begin ();

		cacheAudio(()=>{
			iscacheAudioOK = true;			
			UpdateUI();
			if (GuideManager.Instance.isEqualStep (16004000) || GuideManager.Instance.isEqualStep (17004000)) {
				GuideManager.Instance.guideEvent ();
			}
			MaskWindow.UnlockUI();
		});
	}

	public void UpdateUI() {
		sampleContent.startIndex = defaultIndex;
		if (!isAwakeformHide) {
			sampleContent.maxCount = beastList.Count;
			sampleContent.onLoadFinish = onContentFinish;
			sampleContent.callbackUpdateEach = updatePage;
			sampleContent.onCenterItem = updateActivePage;
			sampleContent.init ();
		} else {
			//hide唤醒
			if (StorageManagerment.Instance.RoleStorageVersion!=storageVersion) {
				//仓库有更改就刷新下容器
				sampleContent.maxCount = beastList.Count;
				sampleContent.init ();
				storageVersion=StorageManagerment.Instance.RoleStorageVersion;
			} 
			//更新当前页,awake会更新三页,所以不用
			ActiveShowItem.updateAll ();
		}
		updateActivePage (sampleContent.getCenterObj ());
	}

	void onContentFinish ()
	{

	}
	
	void updatePage (GameObject obj)
	{
		//更新当前显示的ShowItem;
		ActiveShowItem = sampleContent.getCenterObj ().GetComponent<ButtonBeast> ();
		ButtonBeast bookitem = obj.GetComponent<ButtonBeast> ();
		int index = StringKit.toInt (obj.name) - 1;
		//不够3页.隐藏
		if (beastList == null || index >= beastList.Count || beastList [index] == null) {
			return;
		}
		//防止出现几个相同女神形象
		bookitem.updateBeast(beastList [index]);
	}
	
	void updateActivePage (GameObject obj)
	{
		//更新箭头
		int index = StringKit.toInt (obj.name) - 1;
		if (beastList.Count == 1) {
			leftArrow.gameObject.SetActive (false);
			rightArrow.gameObject.SetActive (false);
		} else if (index == 0) {
			leftArrow.gameObject.SetActive (false);
			rightArrow.gameObject.SetActive (true);
		} else if (index == beastList.Count - 1) {
			leftArrow.gameObject.SetActive (true);
			rightArrow.gameObject.SetActive (false);
		} else {
			leftArrow.gameObject.SetActive (true);
			rightArrow.gameObject.SetActive (true);
		}

		ActiveShowItem = obj.GetComponent<ButtonBeast> ();
		if(ActiveShowItem == null)
			return;
		updateBeast(ActiveShowItem.getBeastEvo());
		defaultIndex = StringKit.toInt (obj.name) - 1;
	}

	//加载女神语音资源
	void cacheAudio(CallBack cb)
	{
		string[] _list = new string[]{
			"audio/audio_401",
			"audio/audio_402",
			"audio/audio_403",
			"audio/audio_404",
			"audio/audio_405",
			"audio/audio_406",
			"audio/audio_407",
			"audio/audio_408",
			"audio/audio_409",
			"audio/audio_410",
			"audio/audio_411",
			"audio/audio_412",
		}; 
		ResourcesManager.Instance.cacheData (_list, (ss)=>{
			cb();
		}, "other");
	}



	public void clearAudio()
	{
		iscacheAudioOK = false;
		stopMusic = true;
		audioSource.volume = 0f;
		audioSource.Stop();
	}

	public void playGoddessSound(BeastEvolve chooseItem)
	{
		if (chooseItem.getBeast().uid != "") {
			int audioId = 401 + BeastEvolveManagerment.Instance.getBeastIndexBySid(chooseItem.getBeast().sid);
			if (audioSource.isPlaying) {
				stopMusic = true;
				StartCoroutine(Utils.DelayRun(()=>{
					PlayGoddessMusic(audioId);
				},1f));
			} else {
				StartCoroutine(Utils.DelayRun(()=>{
					PlayGoddessMusic(audioId);
				},1f));
			}
		} else {
			if(audioSource.isPlaying) {
				stopMusic = true;
			}
		}
	}

	private void PlayGoddessMusic(int id)
	{
		if (ResourcesManager.Instance.allowLoadFromRes) {
			audioSource.clip = Resources.Load("audio/audio_"+id,typeof(AudioClip)) as AudioClip;
			audioSource.Play();
			audioSource.volume = 1f;
		} else {
			ResourcesData data = ResourcesManager.Instance.getResource ("audio/audio_" + id);
			audioSource.clip = data.ResourcesBundle.mainAsset as AudioClip;
			audioSource.Play ();
			audioSource.volume = 1f;
		}
	}

	public void updateBeast (BeastEvolve chooseItem)
	{
		if (chooseItem == null)
			return;

		if(iscacheAudioOK && AudioManager.Instance.IsAudioOpen) {
			playGoddessSound(chooseItem);
		}
		
		updateSelectedCard (chooseItem);
		changeButton ();
		updateCondition(chooseItem);
		level.text = "Lv." + selectedCard .getLevel () + "/" + selectedCard.getMaxLevel ();
		int expSid = selectedCard.getEXPSid();
		long _exp = selectedCard.getEXP();
		long nowExp = EXPSampleManager.Instance.getNowEXPShow(expSid,_exp);
		long maxExp = EXPSampleManager.Instance.getMaxEXPShow(expSid,_exp);

		expbar.updateValue (nowExp,maxExp);

        if (evolveCard != null && evolveCard.getEXP() > 0)
        {
            CardBaseAttribute oldAttr = CardManagerment.Instance.getCardWholeAttr(evolveCard);
            CardBaseAttribute newAttr = CardManagerment.Instance.getCardWholeAttr(selectedCard);
            hp.text = oldAttr.getWholeHp().ToString();
            att.text =oldAttr.getWholeAtt().ToString();
            def.text = oldAttr.getWholeDEF().ToString();
            mag.text =  oldAttr.getWholeMAG().ToString();
            dex.text =  oldAttr.getWholeAGI().ToString();
			jinhua.text=(evolveCard.getQualityId()-1).ToString()+ "/4";
            hp.text += "[64ED6E]" + " + " + (newAttr.getWholeHp() - oldAttr.getWholeHp()).ToString() + "[-]";
			att.text += "[64ED6E]" + " + " + (newAttr.getWholeAtt() - oldAttr.getWholeAtt()).ToString() + "[-]";
			def.text += "[64ED6E]" + " + " + (newAttr.getWholeDEF() - oldAttr.getWholeDEF()).ToString() + "[-]";
			mag.text += "[64ED6E]" + " + " + (newAttr.getWholeMAG() - oldAttr.getWholeMAG()).ToString() + "[-]";
			dex.text += "[64ED6E]" + " + " + (newAttr.getWholeAGI() - oldAttr.getWholeAGI()).ToString() + "[-]";
            evolveCard = null;
        }
        else
        {
            CardBaseAttribute attr = CardManagerment.Instance.getCardWholeAttr(selectedCard);
            hp.text = attr.getWholeHp().ToString();
            att.text = attr.getWholeAtt().ToString();
            def.text =  attr.getWholeDEF().ToString();
            mag.text = attr.getWholeMAG().ToString();
            dex.text =  attr.getWholeAGI().ToString();

			jinhua.text=(selectedCard.getQualityId()-1).ToString()+ "/4";
        }

		titleName = "horStar"+selectedCard.getTitleName(selectedCard.sid);
		setTitle(titleName,selectedCard.getName ());		
		quality.spriteName =QualityManagerment.qualityIDToStringByBG (selectedCard.getQualityId ());
		quality.gameObject.SetActive(true);
		string str =  LanguageConfigManager.Instance.getLanguage("s0371");
		InitSkill ();

	}
	/**显示兑换条件 */
	private void updateCondition(BeastEvolve chooseItem){
		if(selectedCard.uid==""&&!(fatherWindow is TeamEditWindow)){//召唤
			int index=BeastEvolveManagerment.Instance.getBeastIndexBySid(chooseItem.getBeast().sid)+1;
			Horoscopes hores=HoroscopesManager.Instance.getStarByType (index);
			conditonPoint.SetActive(true);
			combatPoint.SetActive(false);
			inofPoint.SetActive(false);
			//nvshengSprite.spriteName= "horStar"+selectedCard.getTitleName(selectedCard.sid);
			nvshengLabel.text=hores.getName();
			nvshengDayLabel.text=hores.getDate();
			nvshengLabel.effectStyle = UILabel.Effect.Outline;
			nvshengLabel.effectColor = new Color32 (0, 1, 0, 255);
			nvshengLabel.color = new Color32 (0, 213, 255, 255);
			sample = selectedEvolve.getExchangeBySids (selectedEvolve.getNextBeast ().sid);
			foreach (ExchangeCondition each in sample.conditions[0]) {
				if (each.costType == PrizeType.PRIZE_MONEY) {
					needMoneyLabel.text = each.num.ToString ();
					if(UserManager.Instance.self.getMoney()<each.num){
						needMoneyLabel.text=Colors.RED+each.num.ToString();
					}
				} else {
					propButton .updateButton (each, ButtonExchange.BEASTSUMMON);
					needPropNum = each.num;
					break;
				} 
			} 


		}else{//进化
			conditonPoint.SetActive(false);
			combatPoint.SetActive(true);
			inofPoint.SetActive(true);

		}
	}

	public void changeButton ()
	{
		if (selectedCard.uid == "") {
			//尚未拥有就召唤
			changeBeastSummonButton (true);
			beastSummonButton.gameObject.name = "summonButton";
			beastSummonButton.textLabel.text = LanguageConfigManager.Instance.getLanguage ("beastSummonW11");
				//前提条件是否达成
			if(selectedEvolve.isCheckAllPremises (selectedEvolve)&&ExchangeManagerment.Instance.isCheckConditions (selectedEvolve.getExchangeBySids (selectedEvolve.getNextBeast ().sid))			   ) { 
				beastSummonButton.disableButton(false);
			}else{
				beastSummonButton.disableButton(true);
			}
		}
		else {
			//是否还能继续进化
			if (selectedEvolve != null && selectedEvolve.isEndBeast ()) {
				changeBeastSummonButton(false);
			} else {
				changeBeastSummonButton(true);
				beastSummonButton.gameObject.name = "evolutionButton";
				beastSummonButton.textLabel.text = LanguageConfigManager.Instance.getLanguage ("beastSummonW06");
				beastSummonButton.disableButton(false);
			}
		}

		if (fatherWindow is TeamEditWindow || fatherWindow is CardSelectWindow) {
			changeOtherButton (true);

			if (oldCard != null && oldCard.uid != selectedCard.uid && selectedCard.uid != "") {
				//如果是替换模式，并且召唤兽不是选中的那只，那么显示替换按钮出来
				otherButton.textLabel.text = LanguageConfigManager.Instance.getLanguage ("beastSummonW13");
				otherButton.disableButton(ArmyManager.Instance.isEditArmyActive());
			} else if (oldCard != null && oldCard.uid == selectedCard.uid && selectedCard.uid != "") {
				changeOtherButton (false);
			} else {
				//如果是上阵模式，并且召唤兽是选中的那只，那么显示替换按钮出来
				otherButton.textLabel.text = LanguageConfigManager.Instance.getLanguage ("beastSummonW12");
			}
		} else {
			changeOtherButton (false);
		}
	}

	public void updateSelectedCard (BeastEvolve evo)
	{
		selectedEvolve = evo;
		selectedCard = selectedEvolve.getBeast (); 
	}
	
	public void updateSelectedCard (Card card)
	{
		selectedCard = card;
	}
	
	void InitSkill ()
	{ 
		oldExp = BeastEvolveManagerment.Instance.getHallowExp();
		day = BeastEvolveManagerment.Instance.getHallowDay();
		count = BeastEvolveManagerment.Instance.getHallowCount();
		mainSkillLv = BeastEvolveManagerment.Instance.getSkillLv();

		Skill[] mSkill = selectedCard .getSkills ();
		if (mSkill == null || mSkill [0] == null) {
			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.SKILLIMAGEPATH + "Skill_Null", mainSkillIcon);
			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.SKILLIMAGEPATH + "Skill_Null", featuresIcon);
			return;
		}
		if(mainSkillLv == 0)
			mainSkillLv = 1;
		if(mainSkillLv >= mSkill [0].getMaxLevel())
			mainSkillLv = mSkill [0].getMaxLevel();
		ResourcesManager.Instance.LoadAssetBundleTexture (mSkill [0].getIcon (), mainSkillIcon);
		ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.SKILLIMAGEPATH + "Skill_Passive", featuresIcon);
		mainSkillLevel.text = "Lv." + mainSkillLv;
		mainSkillDescript.text = mSkill [0].getDescribeByLv(mainSkillLv);
		mainSkill.text = (selectedCard.getQualityId () == 1 ? "[FFFFFF]" : QualityManagerment.getQualityColor(selectedCard.getQualityId ())) + mSkill [0].getName();

		featuresText.text = (selectedCard.getQualityId () == 1 ? "[FFFFFF]" : QualityManagerment.getQualityColor(selectedCard.getQualityId())) + selectedCard .getFeatures () [0];
		featuresDescript.text = selectedCard.getFeatures () [1];
		
		rushCombat();
	}

	int oldCombat = 0;//初始战斗力
	int newCombat = 0;//最新战斗力
	int step;//步进跳跃值
	private bool isRefreshCombat = false;//刷新战斗力开关
	
	//刷新战斗力
	public void rushCombat()
	{
		newCombat = CombatManager.Instance.getBeastEvolveCombat(selectedCard);
		isRefreshCombat = true;
		if(newCombat >= oldCombat)
			step =(int)((float)(newCombat - oldCombat)/20);
		else
			step =(int)((float)(oldCombat - newCombat)/20);
		if(step<1)step=1;
	}
	
	private void refreshCombat()
	{
		if(oldCombat != newCombat){
			if(oldCombat < newCombat) {
				oldCombat += step;
				if (oldCombat >= newCombat)
					oldCombat = newCombat;
			}
			else if(oldCombat > newCombat) {
				oldCombat -= step;
				if (oldCombat <= newCombat)
					oldCombat = newCombat;
			}
			combat.text = " [FFFFFF]" + oldCombat;
		}
		else {
			isRefreshCombat = false;
			combat.text = " [FFFFFF]" + newCombat;
			oldCombat = newCombat;
		}
	}
	
	void Update ()
	{
		if(isRefreshCombat){
			refreshCombat();
		}

		if(stopMusic)
		{
			audioSource.volume -= 0.02f;
			if(audioSource.volume <= 0)
			{
				stopMusic = false;
				audioSource.volume = 0f;
			}
		}
	}
	
	public override void DoDisable ()
	{
		base.DoDisable ();
	}
	
	//改变其他按钮状态
	public void changeOtherButton (bool onOff)
	{
		otherButton.gameObject.SetActive (onOff);
		if(onOff) {
			beastSummonButton.transform.localPosition = leftPoint.localPosition;
		} else {
            if (beastSummonButton.gameObject.name == "evolutionButton")
                beastSummonButton.transform.localPosition = leftPoint.localPosition;
            else
                beastSummonButton.transform.localPosition = centerPoint.localPosition;
		}
	}
	
	//改变进化按钮状态,卡片未召唤和满级才关闭
	public void changeBeastSummonButton (bool onOff)
	{
		beastSummonButton.gameObject.SetActive (onOff);
	}
	
	public int  getShowType ()
	{
		return showType;
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);

		if (gameObj.name == "evolutionButton") {
			clearAudio();
			UiManager.Instance.openWindow<BeastSummonShowWindow>((win)=>{
				win.Initialize(selectedEvolve,oldExp);
			});
		}
		else if (gameObj.name == "summonButton") {//召唤
			if (needPropNum > UserManager.Instance.self.merit) {
				UiManager.Instance.createMessageLintWindow (Language ("beastAttrError_01"));
				MaskWindow.UnlockUI ();
				return;
			}
			clearAudio();
			if (GuideManager.Instance.isEqualStep (16004000)) {
				GuideManager.Instance.doGuide();  
			}
			beastSummonButton.disableButton (true);
			BeastSummonFPort fport = FPortManager.Instance.getFPort ("BeastSummonFPort") as BeastSummonFPort;
			fport.access (sample.sid, summonBack);
		}
		else if (gameObj.name == "otherButton") {
			clearAudio();
			if (GuideManager.Instance.isEqualStep (17004000)) {
				GuideManager.Instance.doGuide();  
			}
			if(fatherWindow is TeamEditWindow) {
				(fatherWindow as TeamEditWindow).updateBeastButton (selectedCard);
			}
            if (fatherWindow is CardSelectWindow) {
                AttackBossOneOnOneManager.Instance.choosedBeast = selectedCard;
                (fatherWindow as CardSelectWindow).updateUI(selectedCard);
            }
			finishWindow();
		}
		else if (gameObj.name == "close") {
			clearAudio();
			finishWindow();
			EventDelegate.Add (OnHide, () => {
				if(fatherWindow is TeamEditWindow) {
					(fatherWindow as TeamEditWindow).reLoadTeam();
				}
			});
		}else if (gameObj.name == "conditionButton") { 
			if (sample == null){
				MaskWindow.UnlockUI ();
				return;
			}
			UiManager.Instance.openDialogWindow<ConditionsWindow> ((win) => {
				win.Initialize (selectedEvolve);	
			});
			
		}
	}
	/** 召唤完成回调方法 */
	private void summonBack ()
	{
		BeastEvolveManagerment.Instance.beastSummon ();
		BeastEvolveManagerment.Instance.showEffect = true;
		StartCoroutine (playSummonEffect ());
	}
	/**播放特效 */
	IEnumerator playSummonEffect ()
	{
		StartCoroutine(flyItemInit());
		yield return new WaitForSeconds (1.4f);
		changeLight ();
		EffectManager.Instance.CreateEffectCtrlByCache(beastEffectPoint, "Effect/UiEffect/SummonBeast",(obj,ctrl)=>{
			ctrl.transform.localPosition = new Vector3 (ctrl.transform.localPosition.x, ctrl.transform.localPosition.y, ctrl.transform.localPosition.x);
		});
		yield return new WaitForSeconds (2f);
		//通过sample.sid 在仓库中获得新的召唤兽 
			updateLastWindow ();
			MaskWindow.UnlockUI ();
	}
	/**打开女神特效界面 */
	void updateLastWindow ()
	{
		if (GuideManager.Instance.guideSid == GuideGlobal.MISSION_NVSHEN3) {
			UiManager.Instance.openDialogWindow<OpenNvShenWindow> ((win) => {
				win.initWindowsWrite (3);
			});
			
		} else {
			UiManager.Instance.openDialogWindow<OpenNvShenWindow> ((win) => {
				win.initWindowWirte (selectedEvolve,3);
			});
		}
	}
	IEnumerator flyItemInit(){
		float randomValue=Random.Range (0.1f, 0.4f);
		yield return  new WaitForSeconds(randomValue);
		flyItem.gameObject.transform.position = propButton.Image.transform.position;
		flyItem.gameObject.SetActive (true);
		flyItem.Initialize (propButton.Image.mainTexture, this);
		propButton.cleanData ();
	}
	void changeLight ()
	{
		iTween.ValueTo (gameObject, iTween.Hash ("delay", 0.3f, "from", 0, "to", 1f, "easetype", iTween.EaseType.easeInCubic, "onupdate", "colorChange", "time", 0.4f));
	}

}
