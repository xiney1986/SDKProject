using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 主卡进化主窗口
 * @author 陈世惟
 * */
public class MainCardSurmountShowWindow : WindowBase {

	private SurmountSample surInfo;
	private Card mainCardOld;//主卡
	private EvolutionCondition[] surCon;//主卡当前突破需要的条件
	public UILabel manlvLabel;//满级
	public GameObject showSurTitle;
	
	//以下是显示突破信息用
	public GameObject showObj;//做展示用
	public UILabel[] oldMsgLabel;//0生命，1攻击，2防御，3魔法，4敏捷，5等级上限，6战斗力
	public UILabel[] newMsgLabel;//0生命，1攻击，2防御，3魔法，4敏捷，5等级上限，6战斗力
	public UILabel[] newAddMsgLabel;
	public GameObject showObjEff;//展示图片特效
	public RoleView effectCard;//主角突破后特效
	public EffectCtrl[] effectSurUp;
	public Animator[] effectSurText;
	public UITexture background;
	public UITexture background1;
	public UISprite newQuality;
	public ButtonSkill[] buttonSkills;//0旧主技能，1新主技能
	public UILabel addQuality;
	public bool isShoww=false;
	private int quNum=4;
	protected override void begin ()
	{
		base.begin ();
		if(fatherWindow is MissionMainWindow){
			(fatherWindow as MissionMainWindow).moveButton.gameObject.SetActive(false);
			(fatherWindow as MissionMainWindow).redion.SetActive(false);

		}
		initInfo();
		ResourcesManager.Instance.LoadAssetBundleTexture("texture/backGround/ChouJiang_BeiJing",background);
		background.gameObject.SetActive (false);
		background1.gameObject.SetActive (true);
		showSurTitle.SetActive(true);
		surmountOver();
	}
	public void init(int index){
		quNum=index;
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
		case "ShowTitle":
			showObjEff.SetActive (false);
			showObj.SetActive (false);
			MaskWindow.UnlockUI();
			break;

		case "ShowSurTitle":
			//废弃
			if(fatherWindow is MissionMainWindow){
				(fatherWindow as MissionMainWindow).moveButton.gameObject.SetActive(true);
				(fatherWindow as MissionMainWindow).redion.SetActive(true);
				
			}
			finishWindow();
			break;
		}
	}
	
	private void initInfo()
	{
		mainCardOld = StorageManagerment.Instance.getRole(UserManager.Instance.self.mainCardUid).Clone() as Card;
		surInfo = SurmountManagerment.Instance.getSurInfoByType(mainCardOld);
		//initButtons();
		//initEffectStrings();
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
		if(quNum == 4)
			return effectSurUp[0];
		else if(quNum == 5)
			return effectSurUp[1];
		else
			return effectSurUp[1];
	}

	private Animator getEffectTextByQualiy()
	{
		if(quNum == 4)
			return effectSurText[0];
		else if(quNum == 5)
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
				newQuality.spriteName = QualityManagerment.qualityIDToString (quNum) + "Bg";
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
//					string[] strs = SurmountManagerment.Instance.getAddEffectByString(mainCard);
//					
//					if(strs == null)
//						return;
//					
//					for (int i = 0; i < strs.Length; i++) {
//						UiManager.Instance.createMessageLintWindow(strs[i]);
//					}
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
		newAddMsgLabel[0].text =  " + ?";
		newAddMsgLabel[1].text =  " + ?";
		newAddMsgLabel[2].text =  " + ?";
		newAddMsgLabel[3].text =  " + ?";
		newAddMsgLabel[4].text =  " + ?";
		newAddMsgLabel[5].text =  " + ?" ;
		int addQ = newCard.getQualityId() - oldCard.getQualityId();
//		if(addQ != 0)
		addQuality.text = LanguageConfigManager.Instance.getLanguage("beastSummonShow11") +"+ ?";
		buttonSkills[1].initSkillData (newCard.getSkills ()[0], ButtonSkill.STATE_LEARNED);
	}
}
