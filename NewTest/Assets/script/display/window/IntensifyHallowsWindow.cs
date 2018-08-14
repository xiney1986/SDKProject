using UnityEngine;
using System.Collections;

/**
 * 圣器强化主界面
 * @authro 陈世惟  
 * */
public class IntensifyHallowsWindow : WindowBase
{

	public const int TYPE_NEED = 1, TYPE_NONEED = 2;
	public int inSideType = 1;//进入类型
	public ExpbarCtrl expbar;//经验条
	public UILabel labelLevel;//等级
	public UILabel labelExp;//经验数值
	public UILabel labelCount;//免费次数
	public UILabel labelMsg2;
	public UILabel[] hallowsNum = new UILabel[3];//物品数量
	//public UITexture[] hallowsTexture = new UITexture[3];//图标
	public ButtonHallows[] hallowsButtons = new ButtonHallows[3];//圣器按钮
	public UISprite levelUpSign;//升级效果图标
	public Transform Hallows;//圣器位置
	public GameObject ani;
	public UILabel combatLabel;   //战斗力
	public GameObject intensifyButton; //强化按钮
	public GameObject oneKeyIntensifyButton; //一键强化按钮
	public GameObject freeIntensifyButton;//免费强化按钮
    public UITexture bgTexture;



	private int laveCount;//总共免费次数
	private int count;//使用了的免费次数
	private int addCount;//VIP额外次数
	private long oldExp;//原始经验
	private long newExp;//新获取经验
	private Prop[] prop = new Prop[3];//三种圣石
	private int maxLv;
	private int nowLv;
	private LevelupInfo lvinfo;
	private GameObject _tmp;
	private CallBack callback;
	public GameObject[] HallowsEffect;
	public GameObject[] HallowsLevelUpEffect;
	private int activeHallowEffect = -1;


	protected override void DoEnable () {
        ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.BACKGROUNDPATH + "hallows_dizuo",bgTexture);
		UiManager.Instance.backGround.switchBackGround("intensifyHallowsBg");
		base.DoEnable ();
	}
	protected override void begin ()
	{
		base.begin ();
	
		if (inSideType == TYPE_NEED)
			getOldInfo ();
		else
			initInfo ();
		GuideManager.Instance.guideEvent ();
		GuideManager.Instance.doFriendlyGuideEvent ();
		MaskWindow.UnlockUI ();
	}

	public void setCallBack (CallBack callback)
	{
		this.callback = callback;
	}
	
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		
		levelUpSign.alpha = 0;
		
		if (gameObj.name == "close") {
			finishWindow ();
//			UiManager.Instance.openMainWindow();
		} else if (gameObj.name == "intensifyButton") {
			if (!checkIsMaxLevel ()) {
				MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("s0348"));
//				UiManager.Instance.createMessageWindowByOneButton(LanguageConfigManager.Instance.getLanguage("s0348"),null);
				return;
			}
			if (!testHallowsEnough ()) {
				UiManager.Instance.openDialogWindow<MessageWindow> ((MessageWindow win) => {
					win.initWindow (2, LanguageConfigManager.Instance.getLanguage ("s0040"), LanguageConfigManager.Instance.getLanguage ("s0014"), LanguageConfigManager.Instance.getLanguage ("s0349"), gotoShop);
				});
				return;
			}
			GuideManager.Instance.doGuide (); 
			GuideManager.Instance.closeGuideMask ();
			updateInfo (1);
			return;
		} else if (gameObj.name == "oneKeyIntensifyButton") {

			GuideManager.Instance.doFriendlyGuideEvent ();
			if (!checkIsMaxLevel ()) {
				MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("s0348"));
//				UiManager.Instance.createMessageWindowByOneButton(LanguageConfigManager.Instance.getLanguage("s0348"),null);
				return;
			}
			if (!testHallowsEnough ()) {
				UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
					win.initWindow (2, LanguageConfigManager.Instance.getLanguage ("s0040"), LanguageConfigManager.Instance.getLanguage ("s0014"), LanguageConfigManager.Instance.getLanguage ("s0349"), gotoShop);
				});
				return;
			}
			updateInfo (2);
			return;
		} else if(gameObj.name == "freeIntensifyButton"){
			if (!checkIsMaxLevel ()) {
				MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("s0348"));
				//				UiManager.Instance.createMessageWindowByOneButton(LanguageConfigManager.Instance.getLanguage("s0348"),null);
				return;
			}

			count = BeastEvolveManagerment.Instance.getLaveHallowConut ();
			updateInfo(3);

		}
	}
	
	//获取初始数据
	public void getOldInfo ()
	{
		UiManager.Instance.applyMask ();
		PyxFPort fport = FPortManager.Instance.getFPort ("PyxFPort") as PyxFPort;
		fport.pyxInfo (initInfo);
	}

	//获取初始数据后调用
	public void initInfo ()
	{
		UiManager.Instance.cancelMask ();
		oldExp = BeastEvolveManagerment.Instance.getHallowExp ();

		initExp ();
		expbar.init (lvinfo);

		labelExp.text = EXPSampleManager.Instance.getExpBarShow (EXPSampleManager.SID_HALLOW_EXP, oldExp);	
		nowLv = expToLevel (oldExp);
		labelLevel.text = "Lv." + nowLv;
		updatesUI ();
	}
	
	//强化端口,1普通强化,2一键强化,3一键用完免费强化次数
	public void updateInfo (int _type)
	{
		//分开算，如果狂点强化，重置经验条和等级显示
		//initNewExp ();
		//expbar.init (lvinfo);
		nowLv = expToLevel (oldExp);
		labelLevel.text = "Lv." + nowLv;
		PyxFPort fport = FPortManager.Instance.getFPort ("PyxFPort") as PyxFPort;
		fport.intensifyPyx (_type, updateInfo);

	}
	
	//获取强化数据后调用
	public void updateInfo ()
	{
		newExp = BeastEvolveManagerment.Instance.getHallowExp ();
		showLevelupEffect ();
		initNewExp ();
		expbar.init (lvinfo);
		expbar.setLevelUpCallBack (showLevelupSign);
		labelExp.text = EXPSampleManager.Instance.getExpBarShow (EXPSampleManager.SID_HALLOW_EXP, newExp);
		nowLv = expToLevel (oldExp);
	
		if(freeIntensifyButton.activeSelf)
		UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage("hallows06",count.ToString(),(newExp- oldExp).ToString()));
		if (newExp != 0)
			oldExp = newExp;
		updatesUI ();
	}

	private void showLevelupEffect ()
	{
		if (expToLevel (newExp) > nowLv) {
			EffectCtrl levelEffect = null;

			int tmpIndex=-1;

			if (nowLv >= 0 && nowLv <= 30)
				tmpIndex=0;
			else if (nowLv > 30 && nowLv <= 60)
				tmpIndex=1;
			else if (nowLv > 60 && nowLv <= 80)
				tmpIndex=2;
			else if (nowLv > 80 && nowLv <= 125)
				tmpIndex=3;

			levelEffect=NGUITools.AddChild (Hallows.gameObject, HallowsLevelUpEffect [tmpIndex]).GetComponent<EffectCtrl>();


			levelEffect.transform.GetChild (0).particleSystem.Play ();
			//等级提升特效
			EffectCtrl levelEffect1 = null;
			levelEffect1 = EffectManager.Instance.CreateEffect (labelLevel.transform, "Effect/Other/Flash");
			levelEffect1.transform.GetChild (0).particleSystem.Play ();
			//防止狂点
			StartCoroutine (Utils.DelayRun (() => {
				MaskWindow.UnlockUI ();
			}, 2f)); 
		} else
			MaskWindow.UnlockUI ();
	}
	
	//经验条满后调用
	public void showLevelupSign (int now)
	{
		nowLv += 1;
		labelLevel.text = "Lv." + nowLv;
	}
	
	public void updatesUI ()
	{
		laveCount = BeastEvolveManagerment.Instance.getHallowCount ();

		//升级更新特效
		showHallows (expToLevel (oldExp));

		int times = BeastEvolveManagerment.Instance.getLaveHallowConut ();
		if (times <= 0)
			times = 0;

		if(times != 0){
			freeIntensifyButton.SetActive(true);
			intensifyButton.SetActive(false);
			oneKeyIntensifyButton.SetActive(false);
		}else{
			freeIntensifyButton.SetActive(false);
			intensifyButton.SetActive(true);
			oneKeyIntensifyButton.SetActive(true);
		}

		if (UserManager.Instance.self.getVipLevel () == 0) {
			string[] tmp = LanguageConfigManager.Instance.getLanguage ("s0351", times + "/" + laveCount).Split('\n');

			labelCount.text = tmp[0];
			labelMsg2.text = tmp[1];
		} else {
			string vip = "VIP" + UserManager.Instance.self.getVipLevel ();
			addCount = VipManagerment.Instance.getVipbyLevel (UserManager.Instance.self.getVipLevel ()).privilege.unrealFreeDay;

			string[] tmp =  LanguageConfigManager.Instance.getLanguage ("s0350", times + "/" + laveCount, vip, addCount.ToString ()).Split('\n');
			labelCount.text =tmp[0];
			labelMsg2.text = tmp[1];
		}
		/*
		ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + "36", hallowsTexture [0]);
		ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + "37", hallowsTexture [1]);
		ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + "38", hallowsTexture [2]);
		*/
		ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + "36", hallowsButtons [0].propIcon);
		ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + "37", hallowsButtons [1].propIcon);
		ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + "38", hallowsButtons [2].propIcon);
		
		hallowsButtons [0].propId = 71041;
		hallowsButtons [1].propId = 71042;
		hallowsButtons [2].propId = 71043;

		prop [0] = StorageManagerment.Instance.getProp (71041);
		prop [1] = StorageManagerment.Instance.getProp (71042);
		prop [2] = StorageManagerment.Instance.getProp (71043);


		if (prop [0] != null)
			hallowsNum [0].text = prop [0].getNum () + "";//低级圣石数量
		else
			hallowsNum [0].text = "0";
		if (prop [1] != null)
			hallowsNum [1].text = prop [1].getNum () + "";//中级圣石数量
		else
			hallowsNum [1].text = "0";
		if (prop [2] != null)
			hallowsNum [2].text = prop [2].getNum () + "";//高级圣石数量
		else
			hallowsNum [2].text = "0";
		
	}
	
	private void showHallows (int _lv){
		int tmpIndex = -1;
 
		if (_lv >= 0 && _lv <= 30)
			tmpIndex = 0;
		else if (_lv > 30 && _lv <= 60)
			tmpIndex = 1;
		else if (_lv > 60 && _lv <= 80)
			tmpIndex = 2;
		else if (_lv > 80 && _lv <= 125)
			tmpIndex = 3;

		if (tmpIndex == activeHallowEffect)
			return;
		else
			activeHallowEffect=tmpIndex;


		Destroy (_tmp);
		_tmp = NGUITools.AddChild (Hallows.gameObject, HallowsEffect [activeHallowEffect]);
		_tmp.transform.parent = Hallows;
		_tmp.transform.localPosition = Vector3.zero;
		_tmp.transform.localScale = Vector3.one;
	}
	
	//经验换算等级
	private int expToLevel (long _exp)
	{
		return EXPSampleManager.Instance.getLevel (EXPSampleManager.SID_HALLOW_EXP, _exp);
	}
	
	//新获取经验条
	private void initNewExp ()
	{
		lvinfo = new LevelupInfo ();
		lvinfo.newExp = newExp;
		lvinfo.newExpDown = EXPSampleManager.Instance.getEXPDown (EXPSampleManager.SID_HALLOW_EXP, expToLevel (newExp));
		lvinfo.newExpUp = EXPSampleManager.Instance.getEXPUp (EXPSampleManager.SID_HALLOW_EXP, expToLevel (newExp));
		lvinfo.newLevel = expToLevel (newExp);
		lvinfo.oldExp = oldExp;
		lvinfo.oldExpDown = EXPSampleManager.Instance.getEXPDown (EXPSampleManager.SID_HALLOW_EXP, expToLevel (oldExp));
		lvinfo.oldExpUp = EXPSampleManager.Instance.getEXPUp (EXPSampleManager.SID_HALLOW_EXP, expToLevel (oldExp));
		lvinfo.oldLevel = expToLevel (oldExp);
		lvinfo.orgData = null;
	}
	
	//初始化经验条
	private void initExp ()
	{
		lvinfo = new LevelupInfo ();
		lvinfo.newExp = oldExp;
		lvinfo.newExpDown = EXPSampleManager.Instance.getEXPDown (EXPSampleManager.SID_HALLOW_EXP, expToLevel (oldExp));
		lvinfo.newExpUp = EXPSampleManager.Instance.getEXPUp (EXPSampleManager.SID_HALLOW_EXP, expToLevel (oldExp));
		lvinfo.newLevel = expToLevel (oldExp);
		lvinfo.oldExp = oldExp;
		lvinfo.oldExpDown = EXPSampleManager.Instance.getEXPDown (EXPSampleManager.SID_HALLOW_EXP, expToLevel (oldExp));
		lvinfo.oldExpUp = EXPSampleManager.Instance.getEXPUp (EXPSampleManager.SID_HALLOW_EXP, expToLevel (oldExp));
		lvinfo.oldLevel = expToLevel (oldExp);
		lvinfo.orgData = null;
	}
	
	//判断圣石是否充足，不足弹提示去商店
	private bool testHallowsEnough ()
	{
		if (prop [0] == null && prop [1] == null && prop [2] == null) {
			if (BeastEvolveManagerment.Instance.getLaveHallowConut () <= 0)
				return false;
			else
				return true;
		} else
			return true;
	}
	
	//去商店
	private void gotoShop (MessageHandle msg)
	{
		if (msg.buttonID == MessageHandle.BUTTON_RIGHT) {
			UiManager.Instance.openWindow<ShopWindow> ((ShopWindow win) => {
				win.setCallBack (null);
			});
		}
	}

	//圣器满级后弹提示
	public bool checkIsMaxLevel ()
	{
		maxLv = EXPSampleManager.Instance.getMaxLevel (EXPSampleManager.SID_HALLOW_EXP);
		if (nowLv >= maxLv)
			return false;
		else
			return true;
	}
	//start
	// 战斗力
	int oldCombat = 0;//初始战斗力
	int newCombat = 0;//最新战斗力
	int step;//步进跳跃值
	private bool isRefreshCombat = false;//刷新战斗力开关
	
	//刷新战斗力
	public void rushCombat ()
	{
		//newCombat = ArmyManager.Instance.getTeamCombat (ArmyManager.ARENATEAMID);
		newCombat = ArmyManager.Instance.getTeamCombat (ArmyManager.Instance.getLastId());

		isRefreshCombat = true;
		if (newCombat >= oldCombat)
			step = (int)((float)(newCombat - oldCombat) / 5);
		else
			step = (int)((float)(oldCombat - newCombat) / 5);
		if (step < 1)
			step = 1;
	}
	
	private void refreshCombat ()
	{
		if (oldCombat != newCombat) {
			if (oldCombat < newCombat) {
				oldCombat += step;
				if (oldCombat >= newCombat)
					oldCombat = newCombat;
			} else if (oldCombat > newCombat) {
				oldCombat -= step;
				if (oldCombat <= newCombat)
					oldCombat = newCombat;
			}
			combatLabel.text = "" + oldCombat;
		} else {
			isRefreshCombat = false;
			combatLabel.text = "" + newCombat;
			oldCombat = newCombat;
		}
	}
	
	void Update ()
	{
		rushCombat();
		if (isRefreshCombat)
			refreshCombat ();
	}
	
	//endl combat
}
