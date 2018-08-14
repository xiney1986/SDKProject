using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 主卡进化主窗口
 * @author 陈世惟
 * */
public class MainCardEvolutionWindow : WindowBase {

	private EvolutionSample evoInfo;
	private Card mainCard;//主卡
	private EvolutionCondition[] evoCon;//主卡当前修身需要的条件
	private CardBaseAttribute attrOldEff;
	private CardBaseAttribute attrNewEff;
	private bool fristIn = true;

	public EvolutionConditionButton[] buttons;//培养需求列表
	public EvolutionConditionButton[] oldConbuttons;//上次培养需求
	public UILabel evoLvLabel;//培养等级
	public UILabel surLvLabel;//突破说明
	public UITexture mainIco;//主角图标
	public ButtonBase surmountButton;//突破按钮
	public ButtonBase evolutionButton;//培养按钮
	/** 一键培养最低 */
	public ButtonBase evolutionButtonOne;
	/** 一键培养最低等级 */
	public int oneKeyMinLv;
	/** 一键培养最低等级描述 */
	public UILabel oneKeyMinLvLabel;
	public UILabel manlvLabel;//满级!
	public UILabel needTitleLabel;//培养需求标题
	public UILabel moneyOwnedLabel;//玩家持有金币标题
	public UILabel[] attrNow;//当前属性
	public UILabel[] addEvo;//增加属性
	public UILabel[] addEvoTitle;//增加属性标题
	public EvolutionStarView Star1;
	public EvolutionStarView Star2;
	public EvolutionStarView Star3;
	public EffectCtrl effectButton;

	public Transform leftP;
	public Transform centerP;
	public Transform rightP;

	//以下是显示培养信息用
	public GameObject showObj;//做展示用
	public UILabel[] oldMsgLabel;//0生命，1攻击，2防御，3魔法，4敏捷
	public UILabel[] newMsgLabel;//0生命，1攻击，2防御，3魔法，4敏捷
	public GameObject costObj;//消费条目
	public UILabel showTitleLabel;//tips

	protected override void begin ()
	{
		base.begin ();
		fristIn = true;
		initInfo();
		initAttr();
		initStar();
		fristIn = false;
		GuideManager.Instance.guideEvent();
		GuideManager.Instance.doFriendlyGuideEvent ();
		MaskWindow.UnlockUI ();
	}

	//断线重连
	public override void OnNetResume ()
	{
		base.OnNetResume ();
		fristIn = true;
		initInfo();
		initAttr();
		initStar();
		fristIn = false;
	}


	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);

		switch(gameObj.name)
		{
		case "close":
			GuideManager.Instance.doGuide();
			GuideManager.Instance.guideEvent();
			//UiManager.Instance.switchWindow<MainWindow>();
			finishWindow();
			break;
		case "continue":
			if(showObj.activeSelf)
				showObj.SetActive(false);
			break;
			
		case "buttonEvolution":
			GuideManager.Instance.doGuide(); 
			GuideManager.Instance.guideEvent();
			GuideManager.Instance.doFriendlyGuideEvent ();
			if (!EvolutionManagerment.Instance.isCanEvoByString(mainCard)){
				return;
			}
			evolution(0);
			break;
		case "buttonEvolutionOne":
			GuideManager.Instance.doFriendlyGuideEvent ();
			if (!EvolutionManagerment.Instance.isCanEvoByString(mainCard)){
				return;
			}
			evolution(1);
			break;
		case "card_image":
			fristIn = true;
			CardBookWindow.Show(mainCard,CardBookWindow.OTHER,null);
			break;

		case "buttonSurmount":
			UiManager.Instance.openWindow<MainCardSurmountWindow>();
			break;

		case "Show":
			GuideManager.Instance.doGuide(); 
			GuideManager.Instance.guideEvent();
			costObj.SetActive (false);
			showObj.SetActive (false);
			MaskWindow.UnlockUI();
			break;

		case "Center":
			if(EvolutionManagerment.Instance.isMaxEvoLevel(mainCard)){
				MaskWindow.UnlockUI();
				return;
			}
			showNextEvoUI();
			MaskWindow.UnlockUI();
			break;
		}
	}

	private void initInfo()
	{
		mainCard = StorageManagerment.Instance.getRole(UserManager.Instance.self.mainCardUid);
		evoInfo = EvolutionManagerment.Instance.getEvoInfoByType(mainCard);
		initButtons();
		initUI();
		MaskWindow.UnlockUI();
	}

	/// <summary>
	/// 是否显示一键培养，是否能点击一键培养
	/// </summary>
	private void initOneKeyButton (bool _show, bool _canClick)
	{
		//显示
		if (_show) {
			evolutionButtonOne.gameObject.SetActive (true);
			if (_canClick) {
				evolutionButtonOne.disableButton (UserManager.Instance.self.getUserLevel () < oneKeyMinLv);
			} else {
				evolutionButtonOne.disableButton (true);
			}
			oneKeyMinLvLabel.gameObject.SetActive (UserManager.Instance.self.getUserLevel () < oneKeyMinLv);
			oneKeyMinLvLabel.text = Language("s0043l2",oneKeyMinLv.ToString ());
		}
		else {
			oneKeyMinLvLabel.gameObject.SetActive (false);
			evolutionButtonOne.gameObject.SetActive (false);
		}
	}

	private void initUI()
	{
		if(showObj.activeSelf)
			showObj.SetActive(false);
		if(SurmountManagerment.Instance.isCanSurLevel(mainCard)) {
			initOneKeyButton(false,true);
			surmountButton.gameObject.SetActive(true);
			surmountButton.disableButton (false);
			if(surmountButton.transform.childCount <= 2) {
				EffectManager.Instance.CreateEffectCtrlByCache(surmountButton.transform,"Effect/UiEffect/Breakthrough_effects",null);
			}
		}
		else {
			surmountButton.gameObject.SetActive(false);
		}

		evoLvLabel.text = mainCard.getEvoLevel() + "/" + EvolutionManagerment.Instance.getMaxLevel(mainCard);

		//这里显示突破预告
		int jindu = SurmountManagerment.Instance.getCardLevel(mainCard);
		string str;
		if (SurmountManagerment.Instance.isNextSurChangeQuitly(mainCard)) {
			str = LanguageConfigManager.Instance.getLanguage("EvoTitle05",jindu.ToString());
		} else {
			str = LanguageConfigManager.Instance.getLanguage("EvoTitle06",jindu.ToString());
		}
		surLvLabel.text = SurmountManagerment.Instance.isMaxSurLevel(mainCard) ? "" : str;
		ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.CARDIMAGEPATH + mainCard.getImageID(), mainIco);
	}

	//那些星球
	private void initStar()
	{
		if(EvolutionManagerment.Instance.isMaxEvoLevel(mainCard)) {
			Star1.gameObject.SetActive (false);
			Star2.gameObject.SetActive (false);
			Star3.gameObject.SetActive (false);
			return;
		}
		int surlv = SurmountManagerment.Instance.getQuitlyLevel(mainCard);// mainCard.getSurLevel();
		int evolv = mainCard.getEvoLevel();
		int shownum = evolv%5;//点亮多少星星
		int sum = evolv/5;//多少个轮回
		int picnum = sum%3;//决定使用第几张图

		switch(picnum)
		{
		case 0:
			if(shownum == 0 && fristIn == false) {
				Star1.gameObject.SetActive (false);
				Star2.gameObject.SetActive (false);
				Star3.gameObject.SetActive (true);
				Star3.initStar(2,surlv,5);
				
				StartCoroutine (Utils.DelayRun (() => {

					TweenScale ts = TweenScale.Begin(Star3.gameObject,0.3f,Vector3.zero);
					EventDelegate.Add (ts.onFinished, () => {
						Star1.gameObject.SetActive (true);
						Star2.gameObject.SetActive (false);
						Star3.gameObject.SetActive (false);

						TweenScale ts2 = TweenScale.Begin(Star1.gameObject,0.3f,Vector3.one);
						ts2.method = UITweener.Method.EaseIn;
						ts2.from = new Vector3 (5, 5, 1);

						EventDelegate.Add (ts2.onFinished, () => {
							iTween.ShakePosition (Star1.gameObject, iTween.Hash ("amount", new Vector3 (0.03f, 0.03f, 0.03f), "time", 0.4f));
							iTween.ShakePosition (Star1.gameObject, iTween.Hash ("amount", new Vector3 (0.01f, 0.01f, 0.01f), "time", 0.4f));
							Star1.initStar(0,surlv,shownum);
						},true);
					},true);
				}, 2f));
			}
			else {
				Star1.gameObject.SetActive (true);
				Star2.gameObject.SetActive (false);
				Star3.gameObject.SetActive (false);
				Star1.initStar(0,surlv,shownum);
			}
			break;

		case 1:
			if(shownum == 0 && fristIn == false) {
				Star1.gameObject.SetActive (true);
				Star2.gameObject.SetActive (false);
				Star3.gameObject.SetActive (false);
				Star1.initStar(0,surlv,5);

				StartCoroutine (Utils.DelayRun (() => {
					
					TweenScale ts = TweenScale.Begin(Star1.gameObject,0.3f,Vector3.zero);
					EventDelegate.Add (ts.onFinished, () => {
						Star1.gameObject.SetActive (false);
						Star2.gameObject.SetActive (true);
						Star3.gameObject.SetActive (false);
						
						TweenScale ts2 = TweenScale.Begin(Star2.gameObject,0.3f,Vector3.one);
						ts2.method = UITweener.Method.EaseIn;
						ts2.from = new Vector3 (5, 5, 1);
						
						EventDelegate.Add (ts2.onFinished, () => {
							iTween.ShakePosition (Star2.gameObject, iTween.Hash ("amount", new Vector3 (0.03f, 0.03f, 0.03f), "time", 0.4f));
							iTween.ShakePosition (Star2.gameObject, iTween.Hash ("amount", new Vector3 (0.01f, 0.01f, 0.01f), "time", 0.4f));
							Star2.initStar(1,surlv,shownum);
						},true);
					},true);
				}, 2f));
			}
			else {
				Star1.gameObject.SetActive (false);
				Star2.gameObject.SetActive (true);
				Star3.gameObject.SetActive (false);
				Star2.initStar(1,surlv,shownum);
			}
			break;

		case 2:
			if(shownum == 0 && fristIn == false) {
				Star1.gameObject.SetActive (false);
				Star2.gameObject.SetActive (true);
				Star3.gameObject.SetActive (false);
				Star2.initStar(1,surlv,5);
				
				StartCoroutine (Utils.DelayRun (() => {
					
					TweenScale ts = TweenScale.Begin(Star2.gameObject,0.3f,Vector3.zero);
					EventDelegate.Add (ts.onFinished, () => {
						Star1.gameObject.SetActive (false);
						Star2.gameObject.SetActive (false);
						Star3.gameObject.SetActive (true);
						
						TweenScale ts2 = TweenScale.Begin(Star3.gameObject,0.3f,Vector3.one);
						ts2.method = UITweener.Method.EaseIn;
						ts2.from = new Vector3 (5, 5, 1);
						
						EventDelegate.Add (ts2.onFinished, () => {
							iTween.ShakePosition (Star3.gameObject, iTween.Hash ("amount", new Vector3 (0.03f, 0.03f, 0.03f), "time", 0.4f));
							iTween.ShakePosition (Star3.gameObject, iTween.Hash ("amount", new Vector3 (0.01f, 0.01f, 0.01f), "time", 0.4f));
							Star3.initStar(2,surlv,shownum);
						},true);
					},true);
				}, 2f));
			}
			else {
				Star1.gameObject.SetActive (false);
				Star2.gameObject.SetActive (false);
				Star3.gameObject.SetActive (true);
				Star3.initStar(2,surlv,shownum);
			}
			break;
		}
	}

	private void initAttr()
	{
		CardBaseAttribute attr = CardManagerment.Instance.getCardAllWholeAttr (mainCard);
		attrNow[0].text = attr.getWholeHp () + "";
		attrNow[1].text = attr.getWholeAtt () + "";
		attrNow[2].text = attr.getWholeDEF () + "";
		attrNow[3].text = attr.getWholeMAG () + "";
		attrNow[4].text = attr.getWholeAGI () + "";
	}

	//获得当前修身需要的条件
	private void initButtons()
	{
		if(EvolutionManagerment.Instance.isMaxEvoLevel(mainCard))
		{
			for (int i = 0; i < buttons.Length; i++) {
				buttons[i].gameObject.SetActive(false);
			}
			evolutionButton.disableButton (true);
			initOneKeyButton (true,false);
			needTitleLabel.gameObject.SetActive(false);
			moneyOwnedLabel.gameObject.SetActive(false);
			manlvLabel.gameObject.SetActive (true);
			return;
		}
		evolutionButton.disableButton (false);
		initOneKeyButton (true,true);
		buttons[0].initButton(1,EvolutionManagerment.Instance.getNeedMoney(mainCard));
		evoCon = EvolutionManagerment.Instance.getEvoCon(mainCard);
		needTitleLabel.gameObject.SetActive(true);
		moneyOwnedLabel.gameObject.SetActive (true);

		if(evoCon != null) {
			buttons[1].gameObject.SetActive(true);
			buttons[1].initButton(evoCon[0]);
		}
	}

	/** 主卡进化端口 */
	private void evolution(int type)
	{
		EvolutionFPort fport = FPortManager.Instance.getFPort ("EvolutionFPort") as EvolutionFPort;
		fport.evolutionMainCard(type,mainCard,evolutionOver);
	}

	private void evolutionOver()
	{
		fristIn = false;
		Card oldCard = mainCard.Clone () as Card;
		oldCard.updateEvoLevel (mainCard.getEvoLevel () - 1);
		attrOldEff = CardManagerment.Instance.getCardAllWholeAttr (oldCard);
		attrNewEff = CardManagerment.Instance.getCardAllWholeAttr (mainCard);
		_oldAttr = new int[5]{attrOldEff.getWholeHp(),attrOldEff.getWholeAtt(),attrOldEff.getWholeDEF(),attrOldEff.getWholeMAG(),attrOldEff.getWholeAGI()};
		_newAttr = new int[5]{attrNewEff.getWholeHp(),attrNewEff.getWholeAtt(),attrNewEff.getWholeDEF(),attrNewEff.getWholeMAG(),attrNewEff.getWholeAGI()};
		_addAttr = new int[5]{(attrNewEff.getWholeHp() - attrOldEff.getWholeHp()),(attrNewEff.getWholeAtt() - attrOldEff.getWholeAtt()),(attrNewEff.getWholeDEF() - attrOldEff.getWholeDEF())
			,(attrNewEff.getWholeMAG() - attrOldEff.getWholeMAG()),(attrNewEff.getWholeAGI() - attrOldEff.getWholeAGI())};
		_step = new int[5]{setStep(attrOldEff.getWholeHp(),attrNewEff.getWholeHp()),setStep(attrOldEff.getWholeAtt(),attrNewEff.getWholeAtt()),setStep(attrOldEff.getWholeDEF(),attrNewEff.getWholeDEF()),
			setStep(attrOldEff.getWholeMAG(),attrNewEff.getWholeMAG()),setStep(attrOldEff.getWholeAGI(),attrNewEff.getWholeAGI())};

		initStar();
		step = 0;
		nextSetp = 1;
		canRefresh = true;
	}

	public int setStep (int newCombat, int oldCombat)
	{
		int a = 1;
		if (newCombat >= oldCombat)
			a = (int)((float)(newCombat - oldCombat) / 10);
		else
			a = (int)((float)(oldCombat - newCombat) / 10);
		if (a < 1)
			return 1;
		else
			return a;
	}

	int[] _oldAttr;
	int[] _newAttr;
	int[] _openRefresh;
	int[] _step;
	int[] _addAttr;
	int step = 0;
	int nextSetp = 0;
	bool canRefresh = false;

	//刷新属性值
	public void refreshAttr (int i)
	{
		if (_oldAttr[i] != _newAttr[i]) {

			if (_oldAttr[i] < _newAttr[i]) {
				_oldAttr[i] += _step[i];
				if (_oldAttr[i] >= _newAttr[i])
					_oldAttr[i] = _newAttr[i];
			} else if (_oldAttr[i] > _newAttr[i]) {
				_oldAttr[i] -= _step[i];
				if (_oldAttr[i] <= _newAttr[i])
					_oldAttr[i] = _newAttr[i];
			}
			attrNow[i].text = _oldAttr[i] + "";
		} else {
			_openRefresh[i] = 1;
			attrNow[i].text = _newAttr[i] + "";
		}
	}

	//可以关闭刷新没
	private bool isCloseRefresh ()
	{
		int a = 0;
		if(_openRefresh == null || _openRefresh.Length <= 0) {
			return true;
		}
		for(int i = 0;i < _openRefresh.Length;i++) {
			if(_openRefresh[i] != 0)
				a++;
		}
		if(a >= _openRefresh.Length)
			return true;
		else
			return false;
	}

	private void playerEffect(UILabel _labelTitle,UILabel _labelDesc,int _desc)
	{
		_labelTitle.text = "+";
		TweenScale ts = TweenScale.Begin(_labelTitle.gameObject,0.1f,Vector3.one);
		ts.method = UITweener.Method.EaseIn;
		ts.from = new Vector3 (5, 5, 1);
		_labelDesc.text = "";
		TweenScale ts2 = TweenScale.Begin(_labelDesc.gameObject,0.1f,Vector3.one);
		ts2.method = UITweener.Method.EaseIn;
		ts2.from = new Vector3 (5, 5, 1);
		EventDelegate.Add(ts2.onFinished, ()=> {
			TweenLabelNumber tln = TweenLabelNumber.Begin (_labelDesc.gameObject, 0.1f, _desc);
			tln.from = 0;
			EventDelegate.Add (tln.onFinished, () => {
				GameObject obj = Create3Dobj ("Effect/Other/Flash").obj;
				obj.transform.parent = _labelDesc.transform;
				obj.transform.localScale = Vector3.one;
				obj.transform.localPosition = new Vector3 (0, 0, -600);
				StartCoroutine (Utils.DelayRun (() => {
					nextSetp++;}, 0.1f));
			},true);
		},true);
	}

	void Update ()
	{
		if (!isCloseRefresh()) {
			refreshAttr(0);
			refreshAttr(1);
			refreshAttr(2);
			refreshAttr(3);
			refreshAttr(4);
		}

		if (canRefresh == true) {

			if(step == nextSetp)
				return;

			if(step == 0) {
				playerEffect(addEvoTitle[0],addEvo[0],_addAttr[0]);
			}

			else if(step == 1) {
				playerEffect(addEvoTitle[1],addEvo[1],_addAttr[1]);
			}

			else if(step == 2) {
				playerEffect(addEvoTitle[2],addEvo[2],_addAttr[2]);
			}

			else if(step == 3) {
				playerEffect(addEvoTitle[3],addEvo[3],_addAttr[3]);
			}

			else if(step == 4) {
				playerEffect(addEvoTitle[4],addEvo[4],_addAttr[4]);
			}

			else if(step == 5) {
				_openRefresh = new int[5]{0,0,0,0,0};
				for(int i = 0;i<addEvo.Length;i++) {
					TweenScale.Begin(addEvo[i].gameObject,0.5f,Vector3.zero);
					TweenScale.Begin(addEvoTitle[i].gameObject,0.5f,Vector3.zero);
				}
				StartCoroutine (Utils.DelayRun (() => {
					nextSetp++;}, 0.5f));
			}

			else if(step == 6) {
				initInfo();
				StartCoroutine (Utils.DelayRun (() => {
					nextSetp++;}, 0.2f));
				canRefresh = false;
				MaskWindow.UnlockUI();
			}

			step++;
		}
	}

	/** 进化前预览展示 */
	private void showNextEvoUI()
	{
		showTitleLabel.text = LanguageConfigManager.Instance.getLanguage("resolve06");
		showObj.SetActive (true);
		costObj.SetActive (false);
		Card newdCard = mainCard.Clone () as Card;
		newdCard.updateEvoLevel (mainCard.getEvoLevel () + 1);
		showOldInfo(mainCard);
		showNewInfo(mainCard,newdCard);
	}

	//0生命，1攻击，2防御，3魔法，4敏捷，5等级上限，6战力
	private void showOldInfo(Card oldCard)
	{
		CardBaseAttribute attr = CardManagerment.Instance.getCardAllWholeAttr (oldCard);
		
		oldMsgLabel[0].text = attr.getWholeHp () + "";
		oldMsgLabel[1].text = attr.getWholeAtt () + "";
		oldMsgLabel[2].text = attr.getWholeDEF () + "";
		oldMsgLabel[3].text = attr.getWholeMAG () + "";
		oldMsgLabel[4].text = attr.getWholeAGI () + "";
	}

	//0生命，1攻击，2防御，3魔法，4敏捷，5等级上限，6战力
	private void showNewInfo(Card oldCard,Card newCard)
	{
		CardBaseAttribute attrOld = CardManagerment.Instance.getCardAllWholeAttr (oldCard);
		CardBaseAttribute attrNew = CardManagerment.Instance.getCardAllWholeAttr (newCard);
		
		newMsgLabel[0].text = attrOld.getWholeHp () + " + " + Colors.GREEN + (attrNew.getWholeHp () - attrOld.getWholeHp ());
		newMsgLabel[1].text = attrOld.getWholeAtt () + " + " + Colors.GREEN + (attrNew.getWholeAtt () - attrOld.getWholeAtt ());
		newMsgLabel[2].text = attrOld.getWholeDEF () + " + " + Colors.GREEN + (attrNew.getWholeDEF () - attrOld.getWholeDEF ());
		newMsgLabel[3].text = attrOld.getWholeMAG () + " + " + Colors.GREEN + (attrNew.getWholeMAG () - attrOld.getWholeMAG ());
		newMsgLabel[4].text = attrOld.getWholeAGI () + " + " + Colors.GREEN + (attrNew.getWholeAGI () - attrOld.getWholeAGI ());
	}
}
