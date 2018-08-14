using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 主卡进化主窗口
 * @author 陈世惟
 * */
public class MainCardSurmountWindow : WindowBase {

	private SurmountSample surInfo;
	private Card mainCardOld;//主卡
	private EvolutionCondition[] surCon;//主卡当前突破需要的条件
	
	public EvolutionConditionButton[] buttons;//突破需求列表
	public UILabel[] effectLabels;//突破需求列表
	public RoleView mainOld;//主角突破前
	public RoleView mainNew;//主角突破后
	public ButtonBase buttonSurmount;//突破按钮
	public UILabel manlvLabel;//满级
	public GameObject showSurTitle;
	
	//以下是显示突破信息用
	public GameObject showObj;//做展示用
	public GameObject showDownCon;//消耗模块
	public UILabel[] oldMsgLabel;//0生命，1攻击，2防御，3魔法，4敏捷，5等级上限，6战斗力
	public UILabel[] newMsgLabel;//0生命，1攻击，2防御，3魔法，4敏捷，5等级上限，6战斗力
	public UILabel[] newAddMsgLabel;
	public GameObject showObjEff;//展示图片特效
	public RoleView effectCard;//主角突破后特效
	public EffectCtrl[] effectSurUp;
	public Animator[] effectSurText;
	public UITexture background;
	public UISprite newQuality;
	public ButtonSkill[] buttonSkills;//0旧主技能，1新主技能
	private GameObject effObj;//按钮特效
	public UILabel combat; //战斗力
	public UILabel addQuality;
	protected override void begin ()
	{
		base.begin ();
		initInfo();
		ResourcesManager.Instance.LoadAssetBundleTexture("texture/backGround/ChouJiang_BeiJing",background);
		background.gameObject.SetActive (false);
		MaskWindow.UnlockUI ();
	}



	//断线重连
	public override void OnNetResume ()
	{
		base.OnNetResume ();
		initInfo();
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		switch(gameObj.name)
		{
		case "close":
			finishWindow();
			break;
			
		case "buttonEvolution":
			if (!isCanEvo()) {
				MaskWindow.UnlockUI();
				return;
			}
			if (effObj != null) {
				effObj.SetActive (false);
			}
			showSurTitle.SetActive(true);
			evolution();
			break;
			
		case "ShowTitle":
			showObjEff.SetActive (false);
			showObj.SetActive (false);
			if (effObj != null) {
				effObj.SetActive (true);
			}
			MaskWindow.UnlockUI();
			break;

		case "ShowSurTitle":
			//废弃
			showObjEff.SetActive (false);
			showObj.SetActive (false);
			if (effObj != null) {
				effObj.SetActive (true);
			}
			MaskWindow.UnlockUI();
			break;
		}
	}
	
	private void back()
	{
		UiManager.Instance.openWindow<MainCardEvolutionWindow> ();
	}
	
	private void initInfo()
	{
		mainCardOld = StorageManagerment.Instance.getRole(UserManager.Instance.self.mainCardUid).Clone() as Card;
		surInfo = SurmountManagerment.Instance.getSurInfoByType(mainCardOld);
		initButtons();
		initEffectStrings();
		initUI();
	}
	
	private void initUI()
	{
		mainOld.gameObject.SetActive(true);
		mainOld.init(mainCardOld,this,null);
		combat.text =mainCardOld.getCardCombat ().ToString();
		if(SurmountManagerment.Instance.isMaxSurLevel(mainCardOld)) {
			if(mainNew.gameObject.activeSelf)
				mainNew.gameObject.SetActive(false);
			mainOld.transform.localPosition = new Vector3(0,mainOld.transform.localPosition.y,0);
			return;
		}

		mainNew.gameObject.SetActive(true);
		Card newcard = CardManagerment.Instance.createCardBySurLevel(mainCardOld,mainCardOld.getSurLevel() + 1);
		mainNew.init(newcard,this,null);
	}

	//获得当前突破需要的条件
	private void initButtons()
	{
		if(SurmountManagerment.Instance.isMaxSurLevel(mainCardOld))
		{
			for (int i = 0; i < buttons.Length; i++) {
				buttons[i].gameObject.SetActive(false);
			}
			manlvLabel.gameObject.SetActive (true);
			buttonSurmount.gameObject.SetActive (false);
			showDownCon.SetActive (false);
			return;
		}
		if(SurmountManagerment.Instance.isCanSurByCon(mainCardOld)) {
			buttonSurmount.gameObject.SetActive(true);
			buttonSurmount.disableButton(false);
			if(effObj == null) {
				EffectManager.Instance.CreateEffectCtrlByCache(buttonSurmount.transform,"Effect/UiEffect/Breakthrough_effects",(_obj,ctrl)=>{
					effObj = _obj.obj;
				});
			}
		}
		else {
			if (effObj != null) {
				Destroy(effObj);
			}
			buttonSurmount.gameObject.SetActive(true);
			buttonSurmount.disableButton(true);
		}
		showDownCon.SetActive (true);
		buttons[0].initButton(1,SurmountManagerment.Instance.getNeedMoney(mainCardOld));
		surCon = SurmountManagerment.Instance.getEvoCon(mainCardOld);
		if(surCon != null) {
			for (int i = 0; i < surCon.Length; i++) {
				if(buttons[i + 1] != null) {
					buttons[i + 1].gameObject.SetActive(true);
					buttons[i + 1].initButton(surCon[i]);
				}
			}
		}
	}

	//获得当前突破后的效果
	private void initEffectStrings()
	{
		for (int i = 0; i < effectLabels.Length; i++) {
			effectLabels[i].gameObject.SetActive(false);
		}
		if(SurmountManagerment.Instance.isMaxSurLevel(mainCardOld))
			return;
		string[] strs = SurmountManagerment.Instance.getNextLevelAddEffectByString(mainCardOld);

		if(strs == null)
			return;

		for (int i = 0; i < strs.Length; i++) {
			effectLabels[i].gameObject.SetActive(true);
			effectLabels[i].text = strs[i];
		}
	}
	
	/** 主卡进化端口 */
	private void evolution()
	{
		EvolutionFPort fport = FPortManager.Instance.getFPort ("EvolutionFPort") as EvolutionFPort;
		fport.surmountMainCard(mainCardOld,surmountOver);
	}
	
	private void surmountOver()
	{
		step = 0;
		nextSetp = 1;
		canRefresh = true;
	}

	int step = 0;
	int nextSetp = 0;
	bool canRefresh = false;

	private EffectCtrl getEffectByQualiy()
	{
		Card newCard = StorageManagerment.Instance.getRole (mainCardOld.uid);
		if(newCard.getQualityId() == 4)
			return effectSurUp[0];
		else if(newCard.getQualityId() == 5)
			return effectSurUp[1];
		else
			return effectSurUp[1];
	}

	private Animator getEffectTextByQualiy()
	{
		Card newCard = StorageManagerment.Instance.getRole (mainCardOld.uid);
		if(newCard.getQualityId() == 4)
			return effectSurText[0];
		else if(newCard.getQualityId() == 5)
			return effectSurText[1];
		else
			return effectSurText[1];
	}

	void Update ()
	{
		if (canRefresh == true) {
			
			if(step == nextSetp)
				return;
			if(step == 0) {
				background.gameObject.SetActive(true);
				showObjEff.SetActive (true);
				effectCard.init(mainCardOld,this,null);
				Card newCard = StorageManagerment.Instance.getRole (mainCardOld.uid);
				newQuality.spriteName = QualityManagerment.qualityIDToString (newCard.getQualityId ()) + "Bg";
				ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.CARDIMAGEPATH + newCard.getImageID (), effectCard.icon);

				TweenPosition tp = TweenPosition.Begin (effectCard.gameObject, 0.3f, effectCard.transform.localPosition);
				tp.from = new Vector3 (500, effectCard.transform.localPosition.y, 0);
				EventDelegate.Add (tp.onFinished, () => {
					StartCoroutine (Utils.DelayRun (() => {
						nextSetp++;}, 0.2f));
				},true);
			}

			else if(step == 1) {
				NGUITools.AddChild (effectCard.gameObject, getEffectByQualiy().gameObject);
				StartCoroutine (Utils.DelayRun (() => {
					nextSetp++;}, 1.5f));
			}

			else if(step == 2) {
				getEffectTextByQualiy().gameObject.SetActive (true);
				StartCoroutine (Utils.DelayRun (() => {
					getEffectTextByQualiy().gameObject.SetActive (false);
					nextSetp++;
				}, 1.6f));
			}

			else if(step == 3) {
				newQuality.alpha = 1;
				TweenScale ts = TweenScale.Begin(newQuality.gameObject,0.3f,Vector3.one);
				ts.method = UITweener.Method.EaseIn;
				ts.from = new Vector3 (5, 5, 1);

				EventDelegate.Add(ts.onFinished, ()=> {
					iTween.ShakePosition (transform.parent.gameObject, iTween.Hash ("amount", new Vector3 (0.03f, 0.03f, 0.03f), "time", 0.4f));
					iTween.ShakePosition (transform.parent.gameObject, iTween.Hash ("amount", new Vector3 (0.01f, 0.01f, 0.01f), "time", 0.4f));
					StartCoroutine (Utils.DelayRun (() => {
						nextSetp++;}, 1.8f));
				},true);
			}

			else if(step == 4) {
				showEvoUI();

				TweenScale ts = TweenScale.Begin(showObj,0.3f,Vector3.one);
				ts.method = UITweener.Method.EaseIn;
				ts.from = new Vector3 (5, 5, 1);

				EventDelegate.Add(ts.onFinished, ()=> {
					iTween.ShakePosition (transform.parent.gameObject, iTween.Hash ("amount", new Vector3 (0.03f, 0.03f, 0.03f), "time", 0.4f));
					iTween.ShakePosition (transform.parent.gameObject, iTween.Hash ("amount", new Vector3 (0.01f, 0.01f, 0.01f), "time", 0.4f));
					StartCoroutine (Utils.DelayRun (() => {
						nextSetp++;
						initInfo();//初始化下一级突破条件以及界面
						MaskWindow.UnlockUI();}, 0.5f));
				},true);
			}

			step++;
		}
	}
	
	private void showEvoUI()
	{
		showObj.SetActive(true);
		Card oldCard = mainCardOld.Clone() as Card;
		Card newCard = StorageManagerment.Instance.getRole (mainCardOld.uid);
		showOldInfo(oldCard);
		showNewInfo(oldCard,newCard);
	}

	private void showOldInfo(Card oldCard)
	{
		CardBaseAttribute attr = CardManagerment.Instance.getCardAllWholeAttr (oldCard);
		
		oldMsgLabel[0].text = attr.getWholeHp () + "";
		oldMsgLabel[1].text = attr.getWholeAtt () + "";
		oldMsgLabel[2].text = attr.getWholeDEF () + "";
		oldMsgLabel[3].text = attr.getWholeMAG () + "";
		oldMsgLabel[4].text = attr.getWholeAGI () + "";
		oldMsgLabel[5].text = "Lv." + oldCard.getMaxLevel();
//		oldMsgLabel [6].text = oldCard.getCardCombat ().ToString();

		buttonSkills[0].initSkillData (oldCard.getSkills ()[0], ButtonSkill.STATE_LEARNED);
	}

	private void showNewInfo(Card oldCard,Card newCard)
	{
		CardBaseAttribute attrNew = CardManagerment.Instance.getCardAllWholeAttr (newCard);
		CardBaseAttribute attr = CardManagerment.Instance.getCardAllWholeAttr (oldCard);
		
		newMsgLabel[0].text = attr.getWholeHp().ToString();
		newMsgLabel[1].text = attr.getWholeAtt().ToString();
		newMsgLabel[2].text = attr.getWholeDEF().ToString();
		newMsgLabel[3].text = attr.getWholeMAG().ToString();
		newMsgLabel[4].text = attr.getWholeAGI().ToString();
		newMsgLabel[5].text = "Lv." + oldCard.getMaxLevel();
		TweenLabelNumber tl = TweenLabelNumber.Begin(combat.gameObject,0.5f,newCard.getCardCombat ());
//		combat.text =  newCard.getCardCombat ().ToString();
		newAddMsgLabel[0].text =  " + " + (attrNew.getWholeHp() - attr.getWholeHp());
		newAddMsgLabel[1].text =  " + " + (attrNew.getWholeAtt() - attr.getWholeAtt());
		newAddMsgLabel[2].text =  " + " + (attrNew.getWholeDEF() - attr.getWholeDEF());
		newAddMsgLabel[3].text =  " + " + (attrNew.getWholeMAG() - attr.getWholeMAG());
		newAddMsgLabel[4].text =  " + " + (attrNew.getWholeAGI() - attr.getWholeAGI());
		newAddMsgLabel[5].text =  " + " + (newCard.getMaxLevel() - oldCard.getMaxLevel());
		int addQ = newCard.getQualityId() - oldCard.getQualityId();
		if(addQ != 0)
			addQuality.text = LanguageConfigManager.Instance.getLanguage("beastSummonShow11") +"+"+  addQ;
		else
			addQuality.text = "";
		buttonSkills[1].initSkillData (newCard.getSkills ()[0], ButtonSkill.STATE_LEARNED);
	}

	/** 主卡能不能突破 */
	private bool isCanEvo()
	{
		int evoLevel = SurmountManagerment.Instance.getCardLevel(mainCardOld);
		
		if(surInfo == null)
			return false;
		//突破等级已满
		if(SurmountManagerment.Instance.isMaxSurLevel(mainCardOld)) {
			UiManager.Instance.createMessageWindowByOneButton(LanguageConfigManager.Instance.getLanguage("Sur02"),null);
			return false;
		}
		//进化等级不足
		if(!SurmountManagerment.Instance.isCanSurLevel(mainCardOld)) {
			UiManager.Instance.createMessageWindowByOneButton(LanguageConfigManager.Instance.getLanguage("Sur03",evoLevel.ToString(),mainCardOld.getEvoLevel().ToString()),null);
			return false;
		}
		//突破金钱不足
		if(SurmountManagerment.Instance.getNeedMoney(mainCardOld) > UserManager.Instance.self.getMoney()) {
			UiManager.Instance.createMessageWindowByOneButton(LanguageConfigManager.Instance.getLanguage("Sur05"),null);
			return false;
		}
		if(surCon != null) {
			for (int i = 0; i < surCon.Length; i++) {
				if(!buttons[i].isEnough())
				{
					UiManager.Instance.createMessageWindowByOneButton(LanguageConfigManager.Instance.getLanguage("Sur06",buttons[i].getName()),null);
					return false;
				}
			}
		}
		
		return true;
	}
}
