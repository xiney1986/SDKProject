using UnityEngine;
using System.Collections;

public class BeastSummonWindow : WindowBase
{ 
	public UITexture cardImage;
	public UILabel needMoney;
	public UILabel userMoney;
	public ButtonBase conditionButton;
	public ButtonBase summonButton;
	public ButtonExchange[] buttons;
	public flyItemCtrl[] flyItem;
	ExchangeSample sample;
	BeastEvolve summonCardEvo;
	Card nextBeast;
	public Transform beastEffectPoint;
	public GameObject data;
	[HideInInspector] public Card oldCard;
	[HideInInspector] public Card newCard;
	[HideInInspector] public  long exp;

	public void Initialize (BeastEvolve beastEvo)
	{

		summonCardEvo = beastEvo;
		cardImage.color = new Color(0.1f,0.1f,0.1f,1);
		nextBeast = summonCardEvo.getNextBeast ();
		cardImage.gameObject.SetActive (true);
        ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.CARDIMAGEPATH , nextBeast, cardImage);
		sample = beastEvo.getExchangeBySids (nextBeast.sid);
		int moneyNum = UserManager.Instance.self.getMoney ();
		conditionButton.textLabel.text = beastEvo.checkPremises (summonCardEvo);

		int index = 0;
		
		foreach (ExchangeCondition each in sample.conditions[0]) {
			if (each.costType == PrizeType.PRIZE_MONEY) {
				needMoney.text = each.num.ToString ();
				userMoney.text = (moneyNum < each.num ? Colors.RED : "") + moneyNum;
				continue;
			} else {
				if (index >= buttons.Length) 
					continue;
				buttons [index].updateButton (each, ButtonExchange.BEASTSUMMON); 
				index += 1;
			} 
		}  
		
		//前提条件是否达成
		if (!beastEvo.isCheckAllPremises (summonCardEvo)) { 
			changeButton (false);
			return;
		}  

		//兑换条件是否达成
		if (!ExchangeManagerment.Instance.isCheckConditions (summonCardEvo.getExchangeBySids (nextBeast.sid))) {
			changeButton (false);
			return;
		}  
		 
		changeButton (true);  
	}
	
	protected override void begin ()
	{
		base.begin ();
		if (GuideManager.Instance.isEqualStep (16005000)) {
			GuideManager.Instance.guideEvent ();
		}
		MaskWindow.UnlockUI ();
	}



	public override void DoDisable ()
	{
		base.DoDisable ();
		IncreaseManagerment.Instance.clearData (IncreaseManagerment.TYPE_BEAST);
	}
	public void changeButton (bool onOff)
	{
		Card beast = summonCardEvo.getBeast ();
		//如果当前召唤兽 uid==0表示需要召唤 否则是进化
		if (beast.uid == "") {
			summonButton.textLabel.text = LanguageConfigManager.Instance.getLanguage ("s0093");//beastSummonW11
		} else {
			summonButton.textLabel.text = LanguageConfigManager.Instance.getLanguage ("s0093");//beastSummonW06
		}

		if (onOff == false) {
			summonButton.disableButton (true);
		} else {
			summonButton.disableButton (false);
		}
	}

	public void clickSummon () {
		Card beast = summonCardEvo.getBeast ();
		if (beast.uid == "") {
			summon ();
		}
		else {
			evolve ();
		}
		summonButton.gameObject.SetActive (false);
	}
	
	public override void buttonEventBase (GameObject gameObj)
	{ 
		base.buttonEventBase (gameObj);

		if (gameObj.name == "close") {
			WindowBase win=UiManager.Instance.getWindow<GoddessWindow>();
			if(win!=null)
			{
				UiManager.Instance.BackToWindow<GoddessWindow>();
			}else
			{
				finishWindow ();
			}
		} else if (gameObj.name == "conditionButton") { 
			if (sample == null){
				MaskWindow.UnlockUI ();
				return;
			}
			UiManager.Instance.openDialogWindow<ConditionsWindow> ((win) => {
				win.Initialize (summonCardEvo);	
			});

		} else if (gameObj.name == "summon") {
			clickSummon ();
		} 
	}
	
	//召唤 召唤兽 传 兑换id
	private void summon ()
	{ 
		BeastSummonFPort fport = FPortManager.Instance.getFPort ("BeastSummonFPort") as BeastSummonFPort;
		fport.access (sample.sid, summonBack);
	}

	IEnumerator flyItemInit(){
		for (int i=0; i< buttons.Length; i++) {
			if (buttons [i].item == null) continue;
			float randomValue=Random.Range (0.1f, 0.4f);
			yield return  new WaitForSeconds(randomValue);
			flyItem [i].gameObject.transform.position = buttons [i].Image.transform.position;
			flyItem [i].gameObject.SetActive (true);
			flyItem [i].Initialize (buttons [i].Image.mainTexture, this);
			buttons [i].cleanData ();
		}
	}
 
	IEnumerator playSummonEffect ()
	{
		StartCoroutine(flyItemInit());
		yield return new WaitForSeconds (1.4f);
		changeLight ();
		EffectManager.Instance.CreateEffectCtrlByCache(beastEffectPoint, "Effect/UiEffect/SummonBeast",(obj,ctrl)=>{
			ctrl.transform.localPosition = new Vector3 (ctrl.transform.localPosition.x, ctrl.transform.localPosition.y - 160, ctrl.transform.localPosition.x);
		});
		yield return new WaitForSeconds (2f);
		if (oldCard != null) { // 进化女神
			EffectManager.Instance.CreateEffectCtrlByCache(beastEffectPoint,"Effect/UiEffect/EvolutionarySuccess",null);
			yield return new WaitForSeconds (1.0f);
			StartCoroutine (Utils.DelayRun (() =>
			{
				hideEvoBeastComponent();
				UiManager.Instance.openDialogWindow<BeastAttrLevelInfo>((win)=>{
					win.Initialize (oldCard,newCard,exp,updateLastWindow); 
				});
			}, 0.5f));
		}
		else {
		//通过sample.sid 在仓库中获得新的召唤兽 
		updateLastWindow ();
		MaskWindow.UnlockUI ();
		}
	}

	/** 隐藏进化女神组件 */
	public void hideEvoBeastComponent()
	{
		data.gameObject.SetActive (false);
		summonButton.gameObject.SetActive (false);
	}

	void changeLight ()
	{
		iTween.ValueTo (gameObject, iTween.Hash ("delay", 0.3f, "from", 0, "to", 1f, "easetype", iTween.EaseType.easeInCubic, "onupdate", "colorChange", "time", 0.4f));
	}
	
	void colorChange (float data)
	{
		cardImage.color = new Color (data, data, data, 1);
	}
	//召唤完成回调方法
	private void summonBack ()
	{
		BeastEvolveManagerment.Instance.beastSummon ();
		
		StartCoroutine (playSummonEffect ());
	}

	public void evolveBack ()
	{
		BeastEvolveManagerment.Instance.beastEvolve ();
		BeastEvolveManagerment.Instance.showEffect = true;
		StartCoroutine (playSummonEffect ());
	}
	
	void updateLastWindow ()
	{
		if (GuideManager.Instance.guideSid == GuideGlobal.MISSION_NVSHEN3) {
			UiManager.Instance.openDialogWindow<OpenNvShenWindow> ((win) => {
				win.initWindow (3);
			});

		} else {
			if (fatherWindow is MainWindow) {
				UiManager.Instance.switchWindow<BeastAttrWindow> ((win) => {
					win.Initialize (summonCardEvo.getBeast (), BeastAttrWindow.STOREVIEW);
				});
			} else {
				finishWindow ();
			}
		}
	}
	
	//进化 召唤兽 传 当前召唤兽uid
	public void evolve ()
	{
		BeastEvolveFPort fport = FPortManager.Instance.getFPort ("BeastEvolveFPort") as BeastEvolveFPort;
		string uid = summonCardEvo.getBeast ().uid;
		fport.access (uid, evolveBack);
	}
}
