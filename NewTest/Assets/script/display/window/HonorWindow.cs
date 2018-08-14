using UnityEngine;
using System.Collections;

/**
 * 爵位窗口
 * @author 汤琦
 * */
public class HonorWindow : WindowBase
{
	public UILabel honorName;
	public UISprite honorIcon;
	public UILabel currentAttValue;//当前物攻值
	public UILabel nextAttValue;//下一级物攻值
	public UILabel currentDefValue;//当前物防值
	public UILabel nextDefValue;//下一级物防值
	public UILabel currentHpValue;//当前生命值
	public UILabel nextHpValue;//下一级生命值
	public UILabel currentMagValue;//当前法攻
	public UILabel nextMagValue;//下一级法攻
	public UILabel currentMagFValue;//当前法防
	public UILabel nextMagFValue;//下一级法防
	public UILabel currentCombat;//当前全队战斗力
	public UILabel addCombat;//下一级增加的全队战斗力
	public UILabel needHonorValue;//所需荣誉值
	public ButtonBase upButton;
	public UISprite[] stars;
	public EffectCtrl firstEffect;
	public EffectCtrl secondEffect;


	private Knighthood knighthood;
	private Knighthood nextKnigthood;
	private const string STARING = "star";//有星星
	private const string STARED = "star_b";//没有星星
	private string[] honorIconNames = {
		"pingmin",
		"juewei_viscount",
		"juewei_count",
		"juewei_baron",
		"juewei_marquiss",
		"juewei_duke"
	};
	private bool isInit = true;
	private int runOverSum = 10;//为10停止
	protected override void begin ()
	{
		base.begin ();
		GuideManager.Instance.guideEvent ();
		MaskWindow.UnlockUI ();
	}

	protected override void DoUpdate ()
	{
		base.DoUpdate ();

	}

	public void updateInfo ()
	{
		knighthood = KnighthoodConfigManager.Instance.getKnighthoodByGrade (UserManager.Instance.self.honorLevel);
		nextKnigthood = KnighthoodConfigManager.Instance.getNextKnigthoodByGrade (UserManager.Instance.self.honorLevel);
		honorName.text = knighthood.kName;
		int needValue = knighthood.needHonorValue;
		int haveValue = UserManager.Instance.self.honor;
		needHonorValue.text =  haveValue + "/" + needValue;

		if (isInit) {
//			runOverSum = 0;
			numberRun ();
			updateState ();
			honorIcon.spriteName = honorIconNames [knighthood.icon];
//			honorIcon.MakePixelPerfect ();
			updateStar ();
			MaskWindow.UnlockUI();
		} else {
			StartCoroutine (playEffect ());
		}
	}
	private void updateState ()
	{
		if (UserManager.Instance.self.honor - knighthood.needHonorValue >= 0) {
			upButton.disableButton (false);
			if(upButton.transform.childCount <= 2) {
				EffectManager.Instance.CreateEffectCtrlByCache(upButton.transform,"Effect/UiEffect/Breakthrough_effects",(obj,effectCtrl)=>{
					effectCtrl.name = "buttonEffect";
				});
			}
		} else {
			upButton.disableButton (true);
			for (int i = 0; i < upButton.transform.childCount; i++) {
				if (upButton.transform.GetChild(i).name == "buttonEffect") {
					Destroy(upButton.transform.GetChild(i).gameObject);
				}
			}
		}
	}
	
	public void updateStar ()
	{
		int num = knighthood.starValue;
		for (int i = 0; i < stars.Length; i++) {
			if (i < num) {
				stars [i].spriteName = STARING;
			} else {
				stars [i].spriteName = STARED;
			}
		}
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);

		if (gameObj.name == "close") {
			finishWindow();
		} else if (gameObj.name == "buttonUpgrade") {
			//是否爵位达到顶值
			if (KnighthoodConfigManager.Instance.isLastKnighthood (UserManager.Instance.self.honorLevel)) {
				UiManager.Instance.openDialogWindow<MessageWindow>((win)=>{
					win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0093"), null, LanguageConfigManager.Instance.getLanguage ("Honor03"), null);
				});
				return;
			}
			if (UserManager.Instance.self.honor - knighthood.needHonorValue >= 0) {
				GuideManager.Instance.doGuide ();
				GuideManager.Instance.guideEvent ();
				MaskWindow.LockUI();
				HonorUpFPort fport = FPortManager.Instance.getFPort ("HonorUpFPort") as HonorUpFPort;
				fport.access (fportBack);
			} else {
				UiManager.Instance.openDialogWindow<MessageWindow>((win)=>{
					win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0093"), null, LanguageConfigManager.Instance.getLanguage ("Honor02"), null);
				});
			}

		}
	}

	//断线重练处理
	public override void OnNetResume ()
	{
		base.OnNetResume ();
		updateInfo ();
	}

	public void fportBack ()
	{
		isInit = false;
		updateInfo ();
	}

	public IEnumerator playEffect ()
	{
		yield return new WaitForSeconds (0.1f);
		int num = knighthood.starValue;
		if (num == 1) {
			changeIcon();
			EffectManager.Instance.CacheEffectCtrl("Effect/UiEffect/equipIntensifyResults",(()=>{
				for (int i = 0; i < stars.Length; i++) {
					if (i == 0) {
						stars [i].spriteName = STARING;
						EffectManager.Instance.CreateEffect(stars [i].transform,"Effect/UiEffect/equipIntensifyResults");
					} else {
						stars [i].spriteName = STARED;
					}
				}
				StartCoroutine (Utils.DelayRun (() => {
					step = 0;
					nextSetp = 1;
					canRefresh = true;
					isInit = true;
				}, 1.2f));
			}));
		} else {
			updateState ();
			EffectManager.Instance.CacheEffectCtrl("Effect/UiEffect/equipIntensifyResults",(()=>{
				for (int i = 0; i < stars.Length; i++) {
					if (i == num - 1) {
						stars [i].spriteName = STARING;
						EffectManager.Instance.CreateEffect(stars [i].transform,"Effect/UiEffect/equipIntensifyResults");
					}
				}
				StartCoroutine (Utils.DelayRun (() => {
					step = 0;
					nextSetp = 1;
					canRefresh = true;
					isInit = true;
				}, 0.5f));
			}));
		}
	}

	void Update ()
	{
		if (canRefresh == true) {

			if (nextKnigthood == null) {
				nextAttValue.text = LanguageConfigManager.Instance.getLanguage ("Honor04");
				nextDefValue.text = LanguageConfigManager.Instance.getLanguage ("Honor04");
				nextHpValue.text = LanguageConfigManager.Instance.getLanguage ("Honor04");
				nextMagValue.text = LanguageConfigManager.Instance.getLanguage ("Honor04");
				nextMagFValue.text = LanguageConfigManager.Instance.getLanguage ("Honor04");
				addCombat.text = LanguageConfigManager.Instance.getLanguage ("Honor04");
				MaskWindow.UnlockUI();
				canRefresh = false;
			}


			if(step == nextSetp){
				return;
			}
			if(step == 0 && nextKnigthood !=null) {
				numEffect(nextHpValue,currentHpValue,knighthood.values [0].currentValue,nextKnigthood.values[0].currentValue);
			}

			if(step == 1  && nextKnigthood !=null) {
				numEffect(nextAttValue,currentAttValue,knighthood.values [1].currentValue,nextKnigthood.values[1].currentValue);
			}

			if(step == 2  && nextKnigthood !=null) {
				numEffect(nextDefValue,currentDefValue,knighthood.values [2].currentValue,nextKnigthood.values[2].currentValue);
			}

			if(step == 3  && nextKnigthood !=null) {
				numEffect(nextMagValue,currentMagValue,knighthood.values [3].currentValue,nextKnigthood.values[3].currentValue);
			}

			if(step == 4  && nextKnigthood !=null) {
				numEffect(nextMagFValue,currentMagFValue,knighthood.values [4].currentValue,nextKnigthood.values[4].currentValue);
			}

			if(step == 5 ){
				numEffect(addCombat,currentCombat,getNowCombat(),getAddCombat());
			}
			if(step == 6) {

				nextHpValue.gameObject.SetActive (true);
				nextAttValue.gameObject.SetActive (true);
				nextDefValue.gameObject.SetActive (true);
				nextMagValue.gameObject.SetActive (true);
				nextMagFValue.gameObject.SetActive (true);
				addCombat.gameObject.SetActive(true);
				if (nextKnigthood == null) {
					nextAttValue.text = LanguageConfigManager.Instance.getLanguage ("Honor04");
					nextDefValue.text = LanguageConfigManager.Instance.getLanguage ("Honor04");
					nextHpValue.text = LanguageConfigManager.Instance.getLanguage ("Honor04");
					nextMagValue.text = LanguageConfigManager.Instance.getLanguage ("Honor04");
					nextMagFValue.text = LanguageConfigManager.Instance.getLanguage ("Honor04");
					addCombat.text = LanguageConfigManager.Instance.getLanguage ("Honor04");
				} else {
					nextHpValue.text = nextKnigthood.values [0].currentValue + "";
					nextAttValue.text = nextKnigthood.values [1].currentValue + "";
					nextDefValue.text = nextKnigthood.values [2].currentValue + "";
					nextMagValue.text = nextKnigthood.values [3].currentValue + "";
					nextMagFValue.text = nextKnigthood.values [4].currentValue + "";
					addCombat.text = "+" + getAddCombat() + "";
				}
				canRefresh = false;
				MaskWindow.UnlockUI();
			}
			step++;
		}

	}

	private void resetInfo ()
	{
		currentAttValue.text = "";
		nextAttValue.text = "";
		currentDefValue.text = "";
		nextDefValue.text = "";
		currentHpValue.text = "";
		nextHpValue.text = "";
		currentMagValue.text = "";
		nextMagValue.text = "";
		currentMagFValue.text = "";
		nextMagFValue.text = "";
		currentCombat.text = "";
		addCombat.text = "";
	}

	int step = 0;
	int nextSetp = 0;
	bool canRefresh = false;

	private void numEffect(UILabel _labelNext,UILabel _labelDesc,int _desc,int _nextDesc)
	{
		GameObject obj = Create3Dobj ("Effect/Other/Flash").obj;
		obj.transform.parent = _labelDesc.transform;
		obj.transform.localScale = Vector3.one;
		obj.transform.localPosition = new Vector3 (0, 0, -600);

		TweenLabelNumber tln = TweenLabelNumber.Begin (_labelNext.gameObject, 0.2f, _nextDesc);
		tln.from = StringKit.toInt (_labelNext.text);
//		EventDelegate.Add (tln.onFinished, () => {
//			_labelNext.gameObject.SetActive (false);
//		},true);
		TweenLabelNumber tln2 = TweenLabelNumber.Begin (_labelDesc.gameObject, 0.2f, _desc);
		tln2.from = StringKit.toInt (_labelDesc.text);
		EventDelegate.Add (tln2.onFinished, () => {
			StartCoroutine (Utils.DelayRun (() => {
				nextSetp++;}, 0.1f));
		},true);
	}
	
	private void numberRun ()
	{
		showNumEffect(currentHpValue,StringKit.toInt (currentHpValue.text),knighthood.values [0].currentValue);
		showNumEffect(currentAttValue,StringKit.toInt (currentAttValue.text),knighthood.values [1].currentValue);
		showNumEffect(currentDefValue,StringKit.toInt (currentDefValue.text),knighthood.values [2].currentValue);
		showNumEffect(currentMagValue,StringKit.toInt (currentMagValue.text),knighthood.values [3].currentValue);
		showNumEffect(currentMagFValue,StringKit.toInt (currentMagFValue.text),knighthood.values [4].currentValue);
		showNumEffect(currentCombat,StringKit.toInt (currentCombat.text),ArmyManager.Instance.getTeamAllCombat(ArmyManager.PVE_TEAMID));
		if (nextKnigthood == null) {
			nextAttValue.text = LanguageConfigManager.Instance.getLanguage ("Honor04");
			nextDefValue.text = LanguageConfigManager.Instance.getLanguage ("Honor04");
			nextHpValue.text = LanguageConfigManager.Instance.getLanguage ("Honor04");
			nextMagValue.text = LanguageConfigManager.Instance.getLanguage ("Honor04");
			nextMagFValue.text = LanguageConfigManager.Instance.getLanguage ("Honor04");
			addCombat.text = LanguageConfigManager.Instance.getLanguage ("Honor04");
		} else {
			showNumEffect(nextHpValue,StringKit.toInt (nextHpValue.text),nextKnigthood.values [0].currentValue);
			showNumEffect(nextAttValue,StringKit.toInt (nextAttValue.text),nextKnigthood.values [1].currentValue);
			showNumEffect(nextDefValue,StringKit.toInt (nextDefValue.text),nextKnigthood.values [2].currentValue);
			showNumEffect(nextMagValue,StringKit.toInt (nextMagValue.text),nextKnigthood.values [3].currentValue);
			showNumEffect(nextMagFValue,StringKit.toInt (nextMagFValue.text),nextKnigthood.values [4].currentValue);
			addCombat.text = "+" + getAddCombat();
		}
	}

	/// <summary>
	/// 获取达到下一个爵位等级，全队增加的战斗力
	/// </summary>
	/// <returns>The add combat.</returns>
	private int getAddCombat(){
		return getNextCombat() - getNowCombat();
	}
	private int getNowCombat(){
		int combatNow = ArmyManager.Instance.getTeamAllCombat(ArmyManager.PVE_TEAMID);
		return combatNow;
	}
	private int getNextCombat(){
		UserManager.Instance.self.honorLevel ++;
		int combatNext = ArmyManager.Instance.getTeamAllCombat(ArmyManager.PVE_TEAMID);
		UserManager.Instance.self.honorLevel --;
		return combatNext;
	}


	private void showNumEffect(UILabel _labelDesc,int temp, int num)
	{
		TweenLabelNumber tln = TweenLabelNumber.Begin (_labelDesc.gameObject, 0.5f, num);
		tln.from = temp;
		EventDelegate.Add (tln.onFinished, () => {

		},true);
	}


	//得到滚动数字
	private string getValue (int temp, int num)
	{
		if (num == 0) {
			return num.ToString ();
		}
		temp += (int)(num * 0.05f);
		if (temp >= num) {
			temp = num;
			runOverSum ++;

		}
		return temp.ToString ();
	}
	//更换图标
	private void changeIcon ()
	{
		NGUITools.AddChild (honorIcon.gameObject, firstEffect.gameObject);

		StartCoroutine (Utils.DelayRun (() => {
			TweenScale ts = TweenScale.Begin (honorIcon.gameObject, 0.1f, Vector3.zero);
			ts.method = UITweener.Method.EaseOut;
			ts.from = new Vector3 (1, 1, 1);
			EventDelegate.Add (ts.onFinished, () => {
				NGUITools.AddChild (honorIcon.gameObject, secondEffect.gameObject);
				honorIcon.spriteName = honorIconNames[knighthood.icon];
				StartCoroutine (Utils.DelayRun (() => {
					TweenScale ts2 = TweenScale.Begin (honorIcon.gameObject, 0.3f, Vector3.one);
					ts2.method = UITweener.Method.EaseIn;
					ts2.from = new Vector3 (5, 5, 1);
					EventDelegate.Add (ts2.onFinished, () => {
						iTween.ShakePosition (this.gameObject, iTween.Hash ("amount", new Vector3 (0.03f, 0.03f, 0.03f), "time", 0.4f));
						iTween.ShakePosition (this.gameObject, iTween.Hash ("amount", new Vector3 (0.01f, 0.01f, 0.01f), "time", 0.4f));

						StartCoroutine (Utils.DelayRun (() => {
							updateState();
						}, 0.2f));

					}, true);
				}, 0.2f));

			}, true);
		}, 0.5f));
	}


}
