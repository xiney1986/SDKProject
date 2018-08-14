using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/**
 * 女神主窗口
 * @author 陈世惟
 * */
public class GoddessWindow: WindowBase {

	public UILabel countdownTimeLabel;
	public UILabel sumResonLabel;
	public UILabel evoResonLabel;
	public UILabel  combat;
    public UILabel openLvelLabel;//神格开启等级
	public ButtonBase rightButton;
    public GameObject middleButton;//神格按钮
	public UILabel doneLabel;
	public GoddessContentItem[] goddessItems;
	public GameObject tipLevel;
    public GameObject liftButton;
    public GameObject tipsGameObject;
	/**cd结束时间 */
	private int cdTime;
	 
	/** 定时器 */
	private Timer timer;

	public GameObject effectFly;
	public GameObject effect;

	public Transform effectEndPoint;
	protected override void begin () {
		base.begin ();
        if (UserManager.Instance.self.getUserLevel() <= 9) liftButton.SetActive(false);
        else liftButton.SetActive(true);
		if(isAwakeformHide){
			GoddessTrainingInit fport = FPortManager.Instance.getFPort("GoddessTrainingInit") as GoddessTrainingInit;
			fport.access(onReceiveInit);
		}
		updateTimer ();
		Initialize();
		if(BeastEvolveManagerment.Instance.showEffect){
			StartCoroutine(playSummonEffect());
		}else{
			info ();
			MaskWindow.UnlockUI ();
		}

	}
	public override void OnAwake () {
		base.OnAwake ();
		GoddessTrainingInit fport = FPortManager.Instance.getFPort("GoddessTrainingInit") as GoddessTrainingInit;
		fport.access(onReceiveInit);
	}
	protected override void DoEnable () {

	}
	
	public override void buttonEventBase (GameObject gameObj) {
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
            GuideManager.Instance.doGuide();
			finishWindow ();
		}
		else if(gameObj.name=="RightButton"){
			UiManager.Instance.openWindow<GoddessUnitWindow> ((win)=>{
				win.init(cdTime);
			});
		}
		else if(gameObj.name=="LeftButton"){
			UiManager.Instance.openWindow<IntensifyHallowsWindow> ();
        } else if (gameObj.name == "MiddleButton"){
            //需要检测玩家等级
            if (UserManager.Instance.self.getUserLevel() >= CommandConfigManager.Instance.getLimitOfShenGeLevel())
            {
                UiManager.Instance.openWindow<NvShenShenGeWindow>((win) =>
                {
                    win.init();
                });
                return;
            }
            UiManager.Instance.openDialogWindow<MessageLineWindow>((win) =>
            {
                win.Initialize(LanguageConfigManager.Instance.getLanguage("NvShenShenGe_003", CommandConfigManager.Instance.levelOfUser +""));
            });
        }
	}
	/// <summary>
	/// 拿后台女神修炼CD时间
	/// </summary>
	/// <param name="time">Time.</param>
	void onReceiveInit(int time){
		cdTime=time;
		refreshAwardTime();
	}
	protected override void DoUpdate () {

	}

	List<BeastEvolve> beastList;

	public void Initialize () {
		beastList = BeastEvolveManagerment.Instance.getAllBest ();
		//init item
		
		foreach(GoddessContentItem tmp in goddessItems){
			BeastEvolve be = null;
			 be = beastList.Find((beast)=>{
				if(beast.getBeast().getImageID() == StringKit.toInt(tmp.gameObject.name)){
					return true;
				}else{
					return false;
				}
			});
			if(be != null){
				//获取头像图标
				tmp.fatherWindow = this;
				tmp.beast = be.getBeast();
				if(be.isAllExist()){
					ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.GODDESS_HEAD2 + be.getBeast().getImageID()+"_h", tmp.headIcon);
					tmp.headIcon.gameObject.SetActive(true);
					tmp.emptyIcon.gameObject.SetActive(false);
					tmp.evolutionIcon.gameObject.SetActive(true);
					tmp.level.gameObject.SetActive(true);
					tmp.evolutionIcon.spriteName = QualityManagerment.qualityIDToString(be.getBeast().getQualityId());
					tmp.evolutionTimes.text = be.getBeast().getQualityId().ToString();
					tmp.level.text ="Lv."+ be.getBeast().getLevel() +"/"+be.getBeast().getMaxLevel();
					//获取品质图标
					tmp.qualityIcon.spriteName = QualityManagerment.qualityIDToIconSpriteName(be.getBeast().getQualityId());
				}
			}
		}
		rushCombat ();
        tipsGameObject.SetActive(checkShowTips());
		if (GuideManager.Instance.isEqualStep (16003000)) {
			GuideManager.Instance.guideEvent ();
		}
		//尝试执行友善指引
		GuideManager.Instance.doFriendlyGuideEvent ();
	}
	public void info () {
		int sum = BeastEvolveManagerment.Instance.getNumAdd ();
		int evo = BeastEvolveManagerment.Instance.getEvolveNumAdd ();
		sumResonLabel.text = LanguageConfigManager.Instance.getLanguage ("GoddessWin04", BeastEvolveManagerment.Instance.num.ToString (), sum.ToString ());
		evoResonLabel.text = LanguageConfigManager.Instance.getLanguage ("GoddessWin05", BeastEvolveManagerment.Instance.evolveNum.ToString (), evo.ToString ());
	}

	/** 更新计时器 */
	private void updateTimer () {
		if (timer == null) {
			timer = TimerManager.Instance.getTimer (UserManager.TIMER_DELAY);
			timer.addOnTimer (refreshAwardTime);
			timer.start (true);
		}
		else {
			refreshAwardTime ();
		}
	}
	/**刷新时间*/
	private void refreshAwardTime () {
		long remainCloseTime = cdTime - ServerTimeKit.getSecondTime ();
		if (remainCloseTime >= 0) {
			countdownTimeLabel.gameObject.SetActive (true);
			countdownTimeLabel.text = TimeKit.timeTransform (remainCloseTime*1000);
			 rightButton.disableButton(false);
			if(doneLabel.gameObject.activeSelf)doneLabel.gameObject.SetActive(false);

		}
		else {
			GoddessTrainingSample sample = GoddessTrainingSampleManager.Instance.getDataBySid (22);
			if(UserManager.Instance.self.getUserLevel()>=sample.lvCondition){
				rightButton.disableButton(false);
				if(!doneLabel.gameObject.activeSelf)doneLabel.gameObject.SetActive(true);
				doneLabel.text=LanguageConfigManager.Instance.getLanguage("s0043l1");
			}else{
				rightButton.disableButton(true);
				doneLabel.gameObject.SetActive(false);
//				doneLabel.text=LanguageConfigManager.Instance.getLanguage("s0043l0",sample.lvCondition.ToString());
			}
			countdownTimeLabel.gameObject.SetActive (false);
		}
		checkLevelCondition ();
	    checkShenGeLevelCondition();
	}
	/** 消耗 */
	public override void DoDisable () {
		base.DoDisable ();
		clear ();
	}
	/** 清理 */
	private void clear () {
		if (timer != null)
			timer.stop ();
		timer = null;
	}


	int oldCombat = 0;//初始战斗力
	int newCombat = 0;//最新战斗力
	int step;//步进跳跃值

	//刷新战斗力
	public void rushCombat () {
		//获取战斗力
		int Combat = 0;
		int pveCombat = ArmyManager.Instance.getTeamAllCombat(ArmyManager.PVE_TEAMID);
		int pvpCombat = ArmyManager.Instance.getTeamAllCombat(ArmyManager.PVP_TEAMID);
		Combat = pveCombat >= pvpCombat? pveCombat:pvpCombat;
		newCombat = Combat;
		isRefreshCombat = true;
		if (newCombat >= oldCombat)
			step = (int)((float)(newCombat - oldCombat) / 20);
		else
			step = (int)((float)(oldCombat - newCombat) / 20);
		if (step < 1)
			step = 1;
	}
	
	private void refreshCombat () {
		if (oldCombat != newCombat) {
			if (oldCombat < newCombat) {
				oldCombat += step;
				if (oldCombat >= newCombat)
					oldCombat = newCombat;
			}
			else if (oldCombat > newCombat) {
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

	bool isRefreshCombat;

	void Update () {
		if (isRefreshCombat) {
			refreshCombat ();
		}
	}

	public GoddessContentItem getMyItem ()
	{
		int index = UserManager.Instance.self.star - 1;
		index = Mathf.Clamp (index,0,goddessItems.Length - 1);
		return goddessItems[index];
	}

	/**播放特效 */
	IEnumerator playSummonEffect ()
	{
		//找到起始点
		foreach(var tmp in goddessItems){
			if( tmp.beast.getName() == BeastEvolveManagerment.Instance.BeastName){
				effectFly.transform.position = tmp.gameObject.transform.position;
				break;	
			}
		}
		Transform[] paths = new Transform[2];
		paths[0] = effectFly.transform;
		paths[1] = effectEndPoint;
		effectFly.SetActive(true);
		iTween.MoveTo (effectFly, iTween.Hash ("position", effectFly.transform.position ,"path",paths, "easetype", "easeOutQuad", "time",1.5f));
		yield return new WaitForSeconds (1.5f);
		effectFly.SetActive(false);
		AudioManager.Instance.PlayAudio (312);
		effect.SetActive(true);
		
		yield return new WaitForSeconds (1.5f);
		effect.SetActive(false);
		//通过sample.sid 在仓库中获得新的召唤兽 
		info();
		BeastEvolveManagerment.Instance.showEffect  = false;
		MaskWindow.UnlockUI ();
	}

	public override void OnNetResume () {
		base.OnNetResume ();
		begin();
	}

	///<summary>
	/// 判断是否达到开放等级
	/// </summary>
	private void checkLevelCondition(){
		if (UserManager.Instance.self.getUserLevel () < 22) {
			rightButton.disableButton(true);
			tipLevel.SetActive(true);
		}

	}

    private bool checkShowTips()
    {
        List<ShenGeCaoInfo> infos = ShenGeManager.Instance.getAllEquipedShenGeSid();
        for (int k = 0; k < infos.Count; k++) //计算各类型神格所附加的影响值总和
        {
            Prop tmpProp = PropManagerment.Instance.createProp(infos[k].sid);
            PropSample sample = PropSampleManager.Instance.getPropSampleBySid(infos[k].sid);
            if (sample != null) {
                if (ShenGeManager.Instance.checkCanGroup(tmpProp, ShenGeManager.SHENGEWINDOW)) {
                    return true;
                }
            }
        }
        return false;
    }

    private void checkShenGeLevelCondition()
    {
        if (!GuideManager.Instance.isGuideComplete())
        {
            middleButton.SetActive(false);
            return;
        }
        middleButton.SetActive(true);
        if (UserManager.Instance.self.getUserLevel() < CommandConfigManager.Instance.getLimitOfShenGeLevel())
        {
            openLvelLabel.text = LanguageConfigManager.Instance.getLanguage("NvShenShenGe_004",
                                     CommandConfigManager.Instance.getLimitOfShenGeLevel().ToString());
            openLvelLabel.gameObject.SetActive(true);
        }
        else
            openLvelLabel.text = "";
    }
}
