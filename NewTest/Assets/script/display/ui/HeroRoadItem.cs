using UnityEngine;
using System.Collections;

public class HeroRoadItem : MonoBase
{
	public UILabel descLabel;
	public HeroRoadItem next;
	public HeroRoadWindow window;
	public UILabel lblName;
	public GameObject buttonFight;
	public HeroRoad heroRoad;
	public RoleView roleView;
	public GoodsView[] goodsViews;
	GoodsView wakeObject;
	public MissionSample mission;

	public void init (HeroRoad heroRoad)
	{
		this.heroRoad = heroRoad;
		ChapterSample cs = ChapterSampleManager.Instance.getChapterSampleBySid (heroRoad.sample.chapter);
		int missionSid = cs.missions [Mathf.Min(heroRoad.conquestCount,cs.missions.Length-1)];
		mission = MissionSampleManager.Instance.getMissionSampleBySid (missionSid);
		lblName.text = string.Format(LanguageConfigManager.Instance.getLanguage("s0389"),heroRoad.activeCount,heroRoad.sample.getMissionCount (),Mathf.Min(heroRoad.conquestCount+1,heroRoad.sample.getMissionCount ()));
		roleView.LockOnClick = false;

		int bossID=mission.bossSid;
		CardSample cardSample=CardSampleManager.Instance.getRoleSampleBySid (bossID);
		cardSample.level=StringKit.toInt(mission.other[2]);
		roleView.init (cardSample,window,null);


		HeroRoad.State state = heroRoad.getState ();
		if (state == HeroRoad.State.COMPLETED) {
			buttonFight.GetComponent<BoxCollider> ().enabled = false;
			buttonFight.transform.FindChild ("Label").GetComponent<UILabel> ().color = Color.gray;
		} else if (state == HeroRoad.State.WAIT) {

			//TODO

		}

		UpdateAward ();
	}

	void UpdateAward ()
	{
		int wakeIndex = 0;
		PrizeSample[] ps = MissionSampleManager.Instance .getMissionSampleBySid (mission.sid).prizes;
		for (int i = 0; ps != null && i < ps.Length && i < goodsViews.Length - 1; i++) {
			wakeIndex = i + 1;
			goodsViews [i].gameObject.SetActive (true);
			goodsViews[i].init(ps [i]);
			goodsViews[i].fatherWindow = window;
		}

//		if (mission.other.Length > 1 && StringKit.toInt(mission.other [1]) == 1) {
//			wakeObject = goodsViews [wakeIndex];
//			wakeObject.gameObject.SetActive (true);
//			wakeObject.init("texture/icon/icon_86",0);
//		}
	}

	void OnBtnDetailClick ()
	{
		window.openItemDetail (this);
	}

	public void OnBtnFightClick ()
	{
		getFubenCurrentInfo();
	}
	private void initUserStar ()
	{
		if (UserManager.Instance.self.isUpdateStar ()) {
			FubenGetStarFPort userFPort = FPortManager.Instance.getFPort ("FubenGetStarFPort") as FubenGetStarFPort;
			userFPort.getStar (UserManager.Instance.self.initStarNum, null);//初始化用户的星星数
		}
	}

	//获得保存的关卡信息
	private void getFubenCurrentInfo ()
	{
		//非必要条件
		initUserStar ();

		FuBenGetCurrentFPort port = FPortManager.Instance.getFPort ("FuBenGetCurrentFPort") as FuBenGetCurrentFPort;
		port.getInfo (getContinueMission);
	}
	
	//处理存档的关卡信息 
	private void getContinueMission (bool b)
	{
			//不在副本中才检测消耗
			if(!UserManager.Instance.self.costCheck(1,MissionEventCostType.COST_CHV))
			{
				UiManager.Instance.openDialogWindow<MessageWindow>((win)=>{
					win.initWindow(2,LanguageConfigManager.Instance.getLanguage ("recharge01"),LanguageConfigManager.Instance.getLanguage("s0093"),
					               LanguageConfigManager.Instance.getLanguage("s0407"),(msgHandle)=>{
						if(msgHandle.buttonID == MessageHandle.BUTTON_LEFT){
							UiManager.Instance.openWindow<VipWindow>();
						}
					});
				});
				return;
			}          
            continueFight();
	}

	//继续关卡
	private void continueMission ()
	{
		FuBenIntoFPort port = FPortManager.Instance.getFPort ("FuBenIntoFPort") as FuBenIntoFPort;
		port.toContinue (continueIntoMission); 
	}

	//切屏
	private void continueIntoMission ()
	{ 
		UiManager.Instance.switchWindow<EmptyWindow>();
	

		EventDelegate.Add(window.OnHide,()=>{

			ScreenManager.Instance.loadScreen (4, null, ()=>{
				window.OnHide.Clear();
				UiManager.Instance.switchWindow<MissionMainWindow>();});
		});

	}



	//获得指定副本数据
	private void initFubenByType (int type)
	{
		if (type != ChapterType.STORY) {
			FuBenInfoFPort port = FPortManager.Instance.getFPort ("FuBenInfoFPort") as FuBenInfoFPort;
			port.info (continueMission, type);
		} else {
			continueMission ();
		}
	}

	void continueFight (){

		HeroRoadManagerment.Instance.currentHeroRoad = heroRoad;
//		UiManager.Instance.openWindow<TeamPrepareWindow> ((win) => {
//			win.Initialize (mission.sid,TeamPrepareWindow.WIN_ROAD_ITEM_TYPE,this);
//		});

		int cSid = mission.chapterSid;
		int type = ChapterSampleManager.Instance.getChapterSampleBySid (cSid).type;
		if (type != ChapterType.WAR && type != ChapterType.PRACTICE && type != ChapterType.HERO_ROAD) {
			if (UserManager.Instance.self.getPvEPoint () < 1) {
				UiManager.Instance.openDialogWindow<PveUseWindow> ();
				return; 
			} 
		}
		
		int teamId=ArmyManager.PVE_TEAMID;;
		int currentCombat=0;
		if(mission.teamType==TeamType.All)
		{
			currentCombat=ArmyManager.Instance.getTeamAllCombat(teamId);
		}else if(mission.teamType==TeamType.Main)
		{
			currentCombat=ArmyManager.Instance.getTeamCombat(teamId);
		}
		int requestCombat=mission.getRecommendCombat();
		if (currentCombat < requestCombat) {
			UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
				win.dialogCloseUnlockUI=false;
				string tip=(mission.teamType==TeamType.Main)?Language("combatTip_01",requestCombat.ToString()):Language("combatTip_02",requestCombat.ToString());
				win.initWindow (2, Language("s0094"), Language ("s0093"), tip, msgBack);
			});
			return;
		}
		intoFubenBack (1);

	}

	void msgBack (MessageHandle msg)
	{
		if (msg.buttonID == MessageHandle.BUTTON_RIGHT) {
			intoFubenBack (msg.msgNum);
		}else{
			MaskWindow.UnlockUI();
		}
	}

	void intoFubenBack (int missionLevel)
	{
		//判断玩家是否有足够的存储空间
		if (FuBenManagerment.Instance.isStoreFull ()) {
			return;
		}
		UiManager.Instance.openWindow<EmptyWindow> ((win) => {
			EventDelegate.Add (win.OnStartAnimFinish, () => {
				//如果是英雄之章
				HeroRoadIntoFPort port = FPortManager.Instance.getFPort ("HeroRoadIntoFPort") as HeroRoadIntoFPort;
				//发阵形
				port.intoRoad (HeroRoadManagerment.Instance.currentHeroRoad.sample.sid, ArmyManager.Instance.activeID, (isFight) => {
					UserManager.Instance.self.costPoint (1, MissionEventCostType.COST_CHV);
					if (!isFight) {
						//进副本保存队伍
						FuBenIntoFPort.intoMission (mission.sid, missionLevel, -1);
					} else {
						MaskWindow.instance.setServerReportWait (true);
						GameManager.Instance.battleReportCallback = GameManager.Instance.intoBattle;
						//直接战斗等后台推战报
					}
				});
			});
		});
		
	}

	void OnAwardButtnClick (GameObject obj)
	{
		if (obj == wakeObject.gameObject) {
			ChapterSample cs = ChapterSampleManager.Instance.getChapterSampleBySid (heroRoad.sample.chapter);
			MissionSample ms = MissionSampleManager.Instance.getMissionSampleBySid (cs.missions [Mathf.Min(heroRoad.conquestCount,cs.missions.Length - 1)]);
			UiManager.Instance.openDialogWindow<MessageWindow>((window)=>{
				window.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0093"), "", ms.other[3], null);
			});
		}
	}

}
