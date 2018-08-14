using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 窗口连接
/// </summary>
   public class WindowLinkManagerment {
	private int curruntSid;
	/** 窗口连接Sid */
	public static int INTENSIFY_CARD_SID=1001, // 强化卡片界面
					HEROROAD_SID = 1005, // 英雄之章
					GUILD_MAIN_SID=1006, // 公会主界面
					CHAPTERSELECT_SID = 1022, //关卡章节
					WARCHOOSE_SID = 1013, //讨伐副本
					TEAMPREPARE_SID = 1015, //女神试炼
					GUILD_DONATE_SID=1019, // 公会捐献界面
					INTENSIFY_CARD_EVLOUTION=1021,//卡片进化	
					XIDONGGEILI_SID =1012, //行动给力
					WARCHOOSE_TEAM_PRE_SID = 1025,
					EQUIP_UPSTAR_SID = 1027,//装备升星
					TOWER_SID = 1028;//爬塔给力
	
	public static WindowLinkManagerment Instance {
		get { return SingleManager.Instance.getObj ("WindowLinkManagerment") as WindowLinkManagerment; }
	}

	public WindowLinkManagerment () {}



	public void OpenWindow (int sid)
	{
		this.curruntSid = sid;
		// 进入副本相关的连接
		if (sid == CHAPTERSELECT_SID || sid == WARCHOOSE_SID || sid == TEAMPREPARE_SID || sid == XIDONGGEILI_SID) {
			OpenIntoFuben ();
		}
		/** 公会相关 */
		else if (sid == GUILD_MAIN_SID || sid == GUILD_DONATE_SID) {
			if (!GuildManagerment.Instance.isHaveGuild ()) {
				UiManager.Instance.openDialogWindow<MessageLineWindow> (( win ) => {
					win.Initialize (LanguageConfigManager.Instance.getLanguage ("s0573"));
				});
			} else {
				OpenWindow (sid, null);
			}
		}
		/** 卡牌强化 */
		else if (sid == INTENSIFY_CARD_SID) {
			UiManager.Instance.openWindow<IntensifyCardWindow> (( win ) => {
				IntensifyCardManager.Instance.setMainCard (ArmyManager.Instance.getActiveArmy ().getLeastCombatCard ());
				win.initWindow (IntensifyCardManager.INTENSIFY_CARD_SACRIFICE);
			});
		} 
		else if (sid == WARCHOOSE_TEAM_PRE_SID) {
			WindowLinkSample sample = WindowLinkSampleManager.Instance.getDataBySid (sid);
			if(sample.windowArgs.Length <=0)
				return;
			teamType = StringKit.toInt(sample.windowArgs[0]);
			OpenIntoFuben ();
		}
		else if( sid == EQUIP_UPSTAR_SID ){

			UiManager.Instance.openWindow<EquipChooseWindow> (( win ) => {
				win.Initialize(ContentEquipChoose.FROM_TO_UPSTAR);
			});
		}else if(sid == TOWER_SID){
			OpenIntoFuben ();
		}
		/** 其他的走配置获取窗口名，然后创建 */
		else {
			OpenWindow (sid, null);
		}
	}

	/** 进入副本相关的连接 */
	public void OpenIntoFuben() {
		if(FuBenManagerment.Instance.isStoreFull()) 
			return;
		if (SweepManagement.Instance.playerCanSweep) {
			SweepManagement.Instance.initSweepInfo (()=>{
				if (SweepManagement.Instance.hasSweepMission) {
					UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
						win.initWindow (2, LanguageConfigManager.Instance.getLanguage ("s0094"), LanguageConfigManager.Instance.getLanguage ("teamEdit_err03"),
						                LanguageConfigManager.Instance.getLanguage ("winLinkErr01"),(msgHandle) => {
							if (msgHandle.buttonID == MessageHandle.BUTTON_RIGHT) {
								FuBenManagerment.Instance.intoSweepMainWindow();
							}
						}
						);
					});
				} else {
					intoFuben ();
				}
			});
		}
		else {
			intoFuben ();
		}
	}

	/// <summary>
	/// 进入爬塔界面
	/// </summary>
	private void intoTowerFuben() {
		//添加过程记录
		if (FuBenManagerment.Instance.getTowerChapter() == null) return;
		FuBenManagerment.Instance.selectedChapterSid = FuBenManagerment.Instance.getTowerChapter().sid;//爬塔副本章节sid
		FuBenManagerment.Instance.selectedMapSid = 1;
		UiManager.Instance.openWindow<ClmbTowerChooseWindow>();
	}
	private void intoFuben ()
	{
		FuBenGetCurrentFPort port = FPortManager.Instance.getFPort ("FuBenGetCurrentFPort") as FuBenGetCurrentFPort;
		port.getInfo ((bool b)=>{
			if (b) {
				UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
					win.initWindow (2, LanguageConfigManager.Instance.getLanguage ("s0094"), LanguageConfigManager.Instance.getLanguage ("teamEdit_err03"),
					                LanguageConfigManager.Instance.getLanguage ("winLinkErr01"),(msgHandle) => {
						if (msgHandle.buttonID == MessageHandle.BUTTON_RIGHT) {
							UiManager.Instance.clearWindows (UiManager.Instance.getWindow<MainWindow> ());
							FuBenManagerment.Instance.inToFuben ();
						}
					});	});
			} else {
				if(curruntSid == TOWER_SID){
					FuBenInfoFPort _port = FPortManager.Instance.getFPort("FuBenInfoFPort") as FuBenInfoFPort;
					_port.info(intoTowerFuben, ChapterType.TOWER_FUBEN);
				}
				else if(curruntSid == WARCHOOSE_SID)
				{
					FuBenInfoFPort _port = FPortManager.Instance.getFPort("FuBenInfoFPort") as FuBenInfoFPort;
					_port.info(()=>{
						if (FuBenManagerment.Instance.getWarChapter () == null)
							return;	
						
						//添加过程记录
						FuBenManagerment.Instance.selectedChapterSid = FuBenManagerment.Instance.getWarChapter ().sid;
						FuBenManagerment.Instance.selectedMapSid = 1;
						UiManager.Instance.openWindow<WarChooseWindow> ();
					}, ChapterType.WAR);
				}
				else if(curruntSid == TEAMPREPARE_SID)
				{
					FuBenInfoFPort _port = FPortManager.Instance.getFPort("FuBenInfoFPort") as FuBenInfoFPort;
					_port.info(()=>{
						FuBenManagerment.Instance.selectedChapterSid = FuBenManagerment.Instance.getPracticeChapter ().sid;
						FuBenManagerment.Instance.selectedMapSid = 1;
						
						UiManager.Instance.openWindow<TeamPrepareWindow> ((win) => {
							win.Initialize (FuBenManagerment.Instance.selectedChapterSid, TeamPrepareWindow.WIN_PRACTICE_ITEM_TYPE);
						});
					},ChapterType.PRACTICE);
				}
				else 
					initFuBenInfo();
			}
		});
	}

	/// <summary>
	/// 初始化副本信息
	/// </summary>
	private void initFuBenInfo(){
		initUserStar ();
		initStarMultiple ();
		FuBenInfoFPort port = FPortManager.Instance.getFPort ("FuBenInfoFPort") as FuBenInfoFPort;
		port.info (() => {
			port.info ( gotoTeamPrepare,ChapterType.PRACTICE);
		}, ChapterType.WAR);
	}
	/// <summary>
	/// 初始化幸运星活动
	/// </summary>
	private void initStarMultiple () {
		(FPortManager.Instance.getFPort<FuBenStarMultipleFPort> () as FuBenStarMultipleFPort).getStarMultiple (null);
	}
	/// <summary>
	/// 初始化用户的星星数
	/// </summary>
	private void initUserStar () {
		if (UserManager.Instance.self.isUpdateStar ()) {
			FubenGetStarFPort userFPort = FPortManager.Instance.getFPort ("FubenGetStarFPort") as FubenGetStarFPort;
			userFPort.getStar (UserManager.Instance.self.initStarNum, null);
		}
	}




	/** 队伍类型(讨伐，剧情。。。 */
	private int teamType;
	/** 关卡sid */
	private int missionSid;
	public void setTeamPrepareInfo(int missionSid){
		this.missionSid = missionSid;
	}
	/// <summary>
	/// 进入战斗准备界面
	/// </summary>
	private void gotoTeamPrepare( ){
		/** 目前只支持跳转到讨伐，其他的等到有需求再写 */
		if (teamType == TeamPrepareWindow.WIN_BOSS_ITEM_TYPE) {
			Mission mission = null;
			Chapter chapter = FuBenManagerment.Instance.getWarChapter ();
			int [] misList = FuBenManagerment.Instance.getAllShowMissions (chapter.sid);
			List<Mission> missonList = new List<Mission> ();
			foreach (int each in misList) {
				if (FuBenManagerment.Instance.isCompleteLastMission (each))
					missonList.Add (MissionInfoManager.Instance .getMissionBySid (each));
				if (each == missionSid)
					mission = MissionInfoManager.Instance .getMissionBySid (each);
			}
			/** 如果配错，这个地方直接返回 */
			if (mission == null)
				return;
			UiManager.Instance.clearWindows (UiManager.Instance.getWindow<MainWindow> ());			
			UiManager.Instance.openWindow<TeamPrepareWindow> ((win) => {
				win.Initialize (mission, TeamPrepareWindow.WIN_BOSS_ITEM_TYPE, missonList);
			});
		} else {
			FuBenManagerment.Instance.inToFuben();
		}
	}
	
	public void OpenWindow (int sid, CallBack<WindowBase> callback) {
		WindowLinkSample sample = WindowLinkSampleManager.Instance.getDataBySid (sid);
		if (sample == null) {
			return;
		}
		UiManager.Instance.openWindow (sample.windowClassName, ( win ) => {
			openWindowArgs (sample, win);
			if (callback != null)
				callback (win);
		}, sample.isDialog, false, true);
	}


	private void openWindowArgs (WindowLinkSample sample, WindowBase win) {
		if (sample.windowArgs.Length <= 0) return;
		if (win is NoticeWindow) {
			(win as NoticeWindow).entranceId = NoticeSampleManager.Instance.getNoticeSampleBySid(StringKit.toInt (sample.windowArgs [0])).entranceId;
			(win as NoticeWindow).updateSelectButton (StringKit.toInt (sample.windowArgs [0]));
		} else if (win is GuildMainWindow) {
			(win as GuildMainWindow).initWindow ();
		} else if (win is TeamPrepareWindow) {
			FuBenInfoFPort port = FPortManager.Instance.getFPort ("FuBenInfoFPort") as FuBenInfoFPort;
			port.info (() => {
				FuBenManagerment.Instance.selectedChapterSid = FuBenManagerment.Instance.getPracticeChapter ().sid;
				FuBenManagerment.Instance.selectedMapSid = 1;

				(win as TeamPrepareWindow).Initialize (FuBenManagerment.Instance.selectedChapterSid, TeamPrepareWindow.WIN_PRACTICE_ITEM_TYPE);
			}, ChapterType.PRACTICE);
		} else if (win is IntensifyCardWindow) {
			if(sample.sid==INTENSIFY_CARD_EVLOUTION)
				IntensifyCardManager.Instance.setMainCard (ArmyManager.Instance.getActiveArmy().getLeastCombatCardExistMain());
			else 
				IntensifyCardManager.Instance.setMainCard (ArmyManager.Instance.getActiveArmy ().getLeastCombatCard ());
			(win as IntensifyCardWindow).initWindow (StringKit.toInt (sample.windowArgs [0]));
		} else if (win is HonorWindow) {
			(win as HonorWindow).updateInfo ();
		} else if (win is GoddessWindow) {
//			(win as GoddessWindow).initWin ();
		} else if (win is ExChangeWindow) {
			(win as ExChangeWindow).setDefaultIndex (StringKit.toInt (sample.windowArgs [0])); 
		} else if (win is ShopWindow) {
			int type = StringKit.toInt (sample.windowArgs [0]);
			ShopWindow shopWindow = win as ShopWindow;
			/** 神秘商城 */
			if(type == ShopWindow.TAP_MYSTICAL_CONTENT){
				shopWindow.setTitle (LanguageConfigManager.Instance.getLanguage ("shop_mystical"));
				shopWindow.init (ShopWindow.TAP_MYSTICAL_CONTENT);
			}
			/** 钻石商城 */
			else if(type == ShopWindow.TAP_SHOP_CONTENT)
			{
				shopWindow.setTitle (LanguageConfigManager.Instance.getLanguage ("shop02"));
				shopWindow.init (ShopWindow.TAP_SHOP_CONTENT);
			}
		}
		
	}

}
