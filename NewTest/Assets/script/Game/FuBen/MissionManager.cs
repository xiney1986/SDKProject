using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MissionManager : MonoBase, IResCacher {
	public Transform pointObjRoot;
	public Transform npcRoot;
	public MissionCharacterCtrl character;
	public static  MissionManager instance;
	public static  int  weatherID;
	public MissionMapInfo mapInfo;
	public GameObject floor;
	public int itemWindowCount = 2;
	public Award[] missionAward;
	public Camera backGroundCamera;
	public static int userState = 0;
	public const int MISSION_NONE = 0;
	public const int MISSION_NEWIN = 1;//新进入副本,完全没点过前进(自动行走相关)
	public const int MISSION_BATTELRETURN = 2;//战斗回来
	public const int MISSION_GETAWARD = 3;//路上捡取完宝箱
    public const int MISSION_GETPATA = 4;//从爬塔抽奖中回来
	[HideInInspector]
	public bool isTalking = false;//是否正在对话
	private bool isAutoRun = false;//是否自动行走
	public GameObject starPathPrefab;
	List<ResourcesData> cacheList;
	public Transform roleCamearT;
	public MissionRoad missionRoad;//最新的3D路线
	[HideInInspector]
	public int tmpStorageVersion = -1;
	[HideInInspector]
	public int AutoRunIndex = -1; //试炼自动跑,当前在什么地方
	[HideInInspector]
	public bool isLoadFinish; // 是否加载完成
	[HideInInspector]
	public  bool[] mSettings;//自动走的两个选择状态

	/// <summary>
	/// 通关结算前减少系统压力
	/// </summary>
	public void cleanGameObj () {
		if (UiManager.Instance.backGround != null)
			UiManager.Instance.backGround.switchBackGround ("ChouJiang_BeiJing");

		MissionNpcManagerment.Instance.clearNpc ();
		character.removeNameLabel ();

		if (pointObjRoot != null && pointObjRoot.childCount > 0) {
			foreach (Transform each in pointObjRoot) {
				if (each != null)
					Destroy (each.gameObject);
			}
		}
	}

	/// <summary>
	/// 清除场景相关的全部缓存
	/// </summary>
	public void cleanCache () {
		ResourcesManager.Instance.UnloadAssetBundleBlock (cacheList.ToArray (), false, "base");
		cacheList.Clear ();
	}

	/// <summary>
	/// 断线重连
	/// </summary>
	public void OnNetResume () {
		//激活3D场景摄像机
		MissionManager.instance.showAll ();
	}

	/// <summary>
	/// 初始化天气情况
	/// </summary>
	public void initWeather () {

		if (weatherID == 0)
			return;

		string[] weather = new string[6]{
			"",
			"Effect/SceneEffects/sunlight",
			"Effect/SceneEffects/rain",
			"Effect/SceneEffects/rainAndThunder",
			"Effect/SceneEffects/snow",
			"Effect/SceneEffects/miasma"};

		//加载天气特效模型
		if (ResourcesManager.Instance.allowLoadFromRes) {
			passObj obj = Create3Dobj (weather[weatherID]);
			obj.obj.transform.parent = character.transform;
			obj.obj.transform.localScale = Vector3.one;
			obj.obj.transform.localPosition = Vector3.zero;
		}
		else {
			ResourcesManager.Instance.cacheData (weather[weatherID], ( list ) => {
				cacheList.Add (list[0]);
				passObj obj = Create3Dobj (weather[weatherID]);
				obj.obj.transform.parent = character.transform;
				obj.obj.transform.localScale = Vector3.one;
				obj.obj.transform.localPosition = Vector3.zero;
			}, "other");
		}
	}

	/// <summary>
	/// 创建地图
	/// </summary>
	public void createMap () {
		//创建场景模型
		passObj _obj = Create3Dobj (mapInfo.packPath);

		if (_obj.obj == null) {
			Debug.LogError ("map is null!!!");
		}
		missionRoad = _obj.obj.GetComponent<MissionRoad> ();
		UIUtils.M_addChild (floor, _obj.obj);
		mapInfo.resourcesData = _obj.data;
		mapInfo.pointRoot = pointObjRoot;
		mapInfo.drawPoint = pointObjRoot;
		mapInfo.createAllPointInfo ();

		//原来配置的坐标不再需要，但是需要这些点的总的数量 来确定新的路线
		int sid = MissionInfoManager.Instance.mission.sid;
		int pointCount = mapInfo.getAllPointData ().Length;
		//每个关卡都对应一个MissionRoadSample数据实体，这些数据缓存在MissionRoadSampleManager中
		MissionRoadSample roadSample;
		if (MissionRoadSampleManager.Instance.cacheMissionRoadSamples.ContainsKey (sid)) {
			roadSample = MissionRoadSampleManager.Instance.cacheMissionRoadSamples[sid];
		}
		else {
			string roadUrl = getMissionRoadUrl_Temp ();
			roadSample = new MissionRoadSample (sid, roadUrl, "Environment_01", pointCount, "null");
			MissionRoadSampleManager.Instance.cacheMissionRoadSamples.Add (sid, roadSample);
		}
		//如果路线数据时随机的 并且判断路段类型数组是否为空（为空标识还未随机生成数据，不空则表示已经生成随机路线片段）
		if (roadSample.segmentRandom && roadSample.segmentTypes == null) {
			roadSample.M_creatRandom (missionRoad);
		}
		//把数据传入路径控制核心组件，依据传入的数据，构建路径
		missionRoad.M_init (roadSample);

		int playCurrentStep = MissionInfoManager.Instance.mission.getPlayerPointIndex ();
		//初始化路径 玩家的当前所在的点位从0开始 路径中的步点从1开始：第1步，第2步，第3步.......
		missionRoad.M_startStep (++playCurrentStep, onRoadCmp);

		MissionInfoManager.Instance.mission.setComplete (missionComplete);
		MissionInfoManager.Instance.mission.setFightLoss (fightLoss);
		MissionInfoManager.Instance.mission.setPvp (doPvp);
		//存一下临时仓库版本号
		tmpStorageVersion = StorageManagerment.Instance.tmpStorageVersion;
	}

	/// <summary>
	/// 创建地图info对象,并设置得到场景模型路径
	/// </summary>
	public void CreateMapInfo () {
		mapInfo = new MissionMapInfo ();
		mapInfo.packPath = getMissionRoadUrl_Temp ();
	}

	/// <summary>
	/// 得到场景模型路径
	/// </summary>
	private string getMissionRoadUrl_Temp () {
		int mapId = MissionInfoManager.Instance.mission.points[0].getMapId ();
		string url = string.Empty;
		switch (mapId) {
			case 3:
				url = "missionRoad/missionRoad_Grassland";
				break;
			case 4:
				url = "missionRoad/missionRoad_Snow";
				break;
			case 6:
//				url = "missionRoad/missionRoad_StarSky";
				url = "missionRoad/DesertRoad";
				break;
			case 7:
				url = "missionRoad/missionRoad_Volcano";
				break;
			case 11:
				url = "missionRoad/missionRoad_StarSky";
				break;
			case 12 :
				url = "missionRoad/Tunnel";
				break;
			case 13:
			url = "missionRoad/PalaceRoad";
			break;

			default:
				url = "missionRoad/missionRoad_Grassland";
				break;
		}
		return url;
	}

	/// <summary>
	/// 设置场景背景图
	/// </summary>
	public void setBackGround () {
		int mapId = MissionInfoManager.Instance.mission.points[0].getMapId ();
		switch (mapId) {
			case 3:
				UiManager.Instance.backGround.switchBackGround ("grassland");
				break;
			case 4:
				UiManager.Instance.backGround.switchBackGround ("snow");
				break;
			case 6:
			UiManager.Instance.backGround.switchBackGround ("Desertbeijing");
				break;
			case 7:
				UiManager.Instance.backGround.switchBackGround ("volcano");
				break;
			case 11:
				UiManager.Instance.backGround.switchBackGround ("ChouJiang_BeiJing");
				break;
			case 12 :
			UiManager.Instance.backGround.switchBackGround ("ChouJiang_BeiJing");
				break;
			case 13 :
			UiManager.Instance.backGround.switchBackGround ("Palace");
			break;
			default:
				UiManager.Instance.backGround.switchBackGround ("grassland");
				break;
		}
	}

	/// <summary>
	/// 初始化各个属性
	/// </summary>
	void Awake () {
		instance = this;
		character = GameObject.Find ("3Dscreen/root/character").GetComponent<MissionCharacterCtrl> ();
		floor = GameObject.Find ("3Dscreen/root/map/floor");
		pointObjRoot = GameObject.Find ("3Dscreen/root/map/pointObjs").transform;
		npcRoot = GameObject.Find ("3Dscreen/root/npc").transform;

		if (userState == 0)
			userState = MISSION_NEWIN;
		MissionManager.instance.StartCoroutine (MissionManager.instance.cacheData ());
	}

	/// <summary>
	/// 废弃的代码
	/// </summary>
	public void hideAll () {
		backGroundCamera.gameObject.SetActive (false);
	}
	public void showAll () {
		backGroundCamera.gameObject.SetActive (true);
	}

	/// <summary>
	/// 更新事件任务
	/// </summary>
	public void updateEventObj () {
		MissionPoint info = MissionInfoManager.Instance.mission.getPlayerPoint ().PointInfo;
		MissionEventSample m_event = MissionInfoManager.Instance.mission.getPlayerPoint ().getPointEvent ();
		EventObjCtrl eventCtrl = info.eventObj;

		//如果下个事件为空，则隐藏事件图标;
		if (m_event == null) {
			cleanPointEvent (info);
			return;
		}

		//如果有任务，但是事件图标为空，则直接创建新事件图标
		if (eventCtrl == null) {
			DrawEventObj (info, m_event, true);
			return;
		}

		//如果有任务，但是事件图标不为空，则直接删除老的创建新的
		if (m_event != info.parentEvent) {
			info.parentEvent = m_event;
			eventCtrl.destory ();
			DrawEventObj (info, m_event, true);
		}
	}

	/// <summary>
	/// 显示剧情对话框关闭之后回调
	/// </summary>
	private void showTalkWindowCallBack () {
		isTalking = false;
		UiManager.Instance.missionMainWindow.TweenerGroupIn ();
		MissionManager.instance.updateEventObj ();
		if(HeroGuideManager.Instance.checkHaveGuid()){
			HeroGuideSample heroGuideSample= HeroGuideManager.Instance.getCurrectSample(MissionInfoManager.Instance.mission.getPlayerPointIndex ());
			if(heroGuideSample!=null&& heroGuideSample.haveTalk==1&&MissionInfoManager.Instance.mission.getPlayerPointIndex ()==heroGuideSample.pointNum&&heroGuideSample.showFlag==1&&MissionInfoManager.Instance.mission.getPlayerPoint().isComplete()){
				HeroGuideManager.Instance.openNvShenWindow(heroGuideSample,true,1);
			}else if(MissionInfoManager.Instance.mission.getPlayerPoint().isComplete()&&heroGuideSample!=null&&heroGuideSample.haveTalk==1&&MissionInfoManager.Instance.mission.getPlayerPointIndex ()==heroGuideSample.pointNum&&(heroGuideSample.showFlag==2)){

				HeroGuideManager.Instance.openNvShenWindow(heroGuideSample,false,3);
			}
		}
		if (GuideManager.Instance.isMoreThanStep (GuideGlobal.NEWOVERSID)) {
			if (MissionInfoManager.Instance.autoGuaji)StartCoroutine (autoMove (1f));
			return;
		}
		//不是第一次打就不用执行对话处理了
		if (!FuBenManagerment.Instance.isNewMission (ChapterType.STORY, MissionInfoManager.Instance.mission.sid)) {
			if (MissionInfoManager.Instance.autoGuaji)StartCoroutine (autoMove (1f));
			return;
		}

		//这里开启控制新手引导强制指引,针对对话结束后的各种情境做步骤控制
		if (GuideManager.Instance.isLessThanStep (7001000)) {
			GuideManager.Instance.guideEvent ();
		}
		GameManager.Instance.playAnimationType = 0;

		if (MissionInfoManager.Instance.mission.sid == GuideGlobal.FIRST_MISSION_SID & MissionInfoManager.Instance.mission.getPlayerPointIndex () == 3
			&& GuideManager.Instance.guideSid != 6004000 && GuideManager.Instance.guideSid <= GuideGlobal.SPECIALSID5) {
			MaskWindow.LockUI ();
			//第1个副本开启女神解锁
			UiManager.Instance.openDialogWindow<OpenNvShenWindow> (( win ) => {
				win.initWindow (0);
			});
		}
		else if (MissionInfoManager.Instance.mission.sid == GuideGlobal.SECOND_MISSION_SID & MissionInfoManager.Instance.mission.getPlayerPointIndex () == 2
		  && GuideManager.Instance.guideSid <= GuideGlobal.SPECIALSID64) {
			MaskWindow.LockUI ();
			//第2个副本女神解锁1&2
			GuideManager.Instance.doGuide ();
			GuideManager.Instance.guideEvent ();
			UiManager.Instance.openDialogWindow<OpenNvShenWindow> (( win ) => {
				win.initWindow (1);
			});
		}
		else if (MissionInfoManager.Instance.mission.sid == GuideGlobal.THREE_MISSION_SID & MissionInfoManager.Instance.mission.getPlayerPointIndex () == 3
		  && GuideManager.Instance.guideSid <= GuideGlobal.SPECIALSID69) {
			MaskWindow.LockUI ();
			//第3个副本收集到最后一颗碎片,不放女神解锁动画了,直接闪光
			GuideManager.Instance.doGuide ();
			GuideManager.Instance.guideEvent ();
			GameManager.Instance.playAnimationType = 1;
		}
	}

	/// <summary>
	/// 角色向前移动到下个点
	/// </summary>
	public void moveForward () {
		if (isTalking) {
			MaskWindow.UnlockUI ();
			return;
		}
		MaskWindow.LockUI ();
		if (GuideManager.Instance.isLessThanStep (7001000)) {
			if (GuideManager.Instance.isEqualStep (3003000) || GuideManager.Instance.isEqualStep (4003000) || GuideManager.Instance.isEqualStep (4004000)
				|| GuideManager.Instance.isEqualStep (6005000)) {
				if (!isAutoRun)
					GuideManager.Instance.doGuide ();
			}
		}
		//清空记录的自动行走条件状态
		MissionManager.userState = MISSION_NONE;
		isAutoRun = false;
		if (MissionInfoManager.Instance.mission.canGotoNext ()) {
			MissionInfoManager.Instance.mission.sendGo (gotoNextCallback);
		}
		else {
			doEvent ();
		}
	}

	/// <summary>
	/// 执行事件
	/// </summary>
	private void doEvent () {
		MaskWindow.LockUI ();
		MissionEventSample e = MissionInfoManager.Instance.mission.getPlayerPoint ().getPointEvent ();
		if (e == null)
			return;
		//事件消耗检查
        if (!UserManager.Instance.self.costCheck(e.cost, e.costType)) {
            UiManager.Instance.openDialogWindow<PveUseWindow>();
            MissionInfoManager.Instance.autoGuaji = false;
            if (UiManager.Instance.missionMainWindow.guajiPoint.activeInHierarchy) UiManager.Instance.missionMainWindow.guajiPoint.SetActive(false);
            if (UiManager.Instance.missionMainWindow.stopButton.gameObject.activeInHierarchy) UiManager.Instance.missionMainWindow.stopButton.gameObject.SetActive(false);
            return;
        }

		if (e.eventType == MissionEventType.FIGHT || e.eventType == MissionEventType.BOSS) {
			//获得战报后回调
		    GameManager.Instance.isCanBeSecondSkill = e.eventType != MissionEventType.BOSS;
            GameManager.Instance.battleReportCallback = () => {//设置一个得到战报以后的回调 为战斗做准备
                if (MissionInfoManager.Instance.mission != null && MissionInfoManager.Instance.mission.getChapterType() == ChapterType.STORY && GameManager.Instance.isCanBeSecondSkill) {
                    GameManager.Instance.intoBattle();
                    GameManager.Instance.miaoShaCallBack = () => {
                        UiManager.Instance.missionMainWindow.updateUserInfo();
                        MissionInfoManager.Instance.mission.doEvent(doEventCallBack);
                        updatePoints();
                    };
                } else {
                    prepareIntoBattle();
                    GameManager.Instance.intoBattle();
                }

            };
		   if (!MissionInfoManager.Instance.mission.getPlayerPoint ().isDoBattleEvent ()) {//这一步判断这个点的事件是不是战斗事件或boss事件 第一次必然为false 进入下一步
				MissionInfoManager.Instance.mission.getBattleInfo (showBattleInfoWindow);
				return;
			}
		}
        if(e.eventType==MissionEventType.TOW_OVER){//一来就是宝箱结束时间
            ClmbTowerManagerment.Instance.getAwardSuccessCallBack = () => {
                UiManager.Instance.missionMainWindow.updateUserInfo();
                MissionInfoManager.Instance.mission.doEvent(doEventCallBack);
            };
            ClmbTowerManagerment.Instance.getGiveUpCallBack = () => {
                UiManager.Instance.missionMainWindow.updateUserInfo();
                MissionInfoManager.Instance.mission.doEvent(doEventCallBack);
                //AudioManager.Instance.PlayAudio(135);
                //updateEventObj();
                //StartCoroutine(autoMove(1f));
            };
            ClmbTowerManagerment.Instance.boxMissionSid = MissionInfoManager.Instance.mission.sid;
            ClmbTowerManagerment.Instance.intoTpye = 1;
            //检查宝箱状态
            ClmbTowerManagerment.Instance.beginIntoTower();
        } else {
            UiManager.Instance.missionMainWindow.updateUserInfo();
            MissionInfoManager.Instance.mission.doEvent(doEventCallBack);
        }
        
		
	}

	/// <summary>
	/// 进入战斗清理下界面
	/// </summary>
	void prepareIntoBattle () {
		MaskWindow.instance.setServerReportWait (true);
		if (UiManager.Instance.missionMainWindow != null) {
			UiManager.Instance.missionMainWindow.UIEffectRoot.gameObject.SetActive (false);
			UiManager.Instance.missionMainWindow.TweenerGroupOut ();
		}
	}

	/// <summary>
	/// 显示战前队伍信息窗口
	/// </summary>
	private void showBattleInfoWindow () {
		// 做标记,表明下次进副本是从战斗里回去的
		MissionManager.userState = MISSION_BATTELRETURN;

		if (!MissionInfoManager.Instance.mission.isDoBattle ()) {
			MissionInfoManager.Instance.mission.getPlayerPoint ().isBattlePrepared = false;
            if (!MissionInfoManager.Instance.isTowerFuben()) {
                UiManager.Instance.openDialogWindow<MessageWindow>(
                (window) => {
                    window.initWindow(1, LanguageConfigManager.Instance.getLanguage("s0093"), "", LanguageConfigManager.Instance.getLanguage("s0226"), null);
                });
            } else { 
                UiManager.Instance.openDialogWindow<MessageWindow>((win) => {//弹出卡片全部阵亡提示
                    win.initWindow(1, LanguageConfigManager.Instance.getLanguage("ladderButton"), "", LanguageConfigManager.Instance.getLanguage("towerShowWindow06"), (msgHandle) => {
                        UiManager.Instance.missionMainWindow.outTowerFuBen();
                    });
                });
            }			
			return;
		}

		//是否boss战
		if (MissionInfoManager.Instance.mission.getPlayerPoint ().getPointEvent ().eventType == MissionEventType.BOSS)
		{
		    MissionInfoManager.Instance.isBossFight = true;
			UiManager.Instance.missionMainWindow.showMissionBossWarring (MissionInfoManager.Instance.mission.getPlayerPoint ().getPointEvent ().other);
		}
		else {
			prepareBattle (false);
		}
	}
	/// <summary>
	/// 准备战斗
	/// </summary>
	void prepareBattle ( bool isBoss ) {
		//取战斗阵形人数配置,保证不管敌人多少都按这个阵形
		BattleManager.lastMissionEvent = MissionInfoManager.Instance.mission.getPlayerPoint ().getPointEvent ();
		//获得该点的唯一ID,标识战斗用
		BattleManager.battleSid = MissionInfoManager.Instance.mission.getPlayerPointOnlyID ();

		if (MissionInfoManager.Instance.mission.getPlayerPoint ().getPointEvent ().showBattelPrepare == 1) {
			UiManager.Instance.openWindow<BattlePrepareWindowNew> (( win ) => {
				EventDelegate.Add (win.OnStartAnimFinish, () => {
					StartCoroutine (Utils.DelayRun (() => {
						win.autoBegin ();
					}, 1f));
				}, true);
				win.Initialize (MissionInfoManager.Instance.mission.getPlayerPoint ().getPointEvent ().battleType, false, isBoss, doEvent);
			});
		}
		else {
			doEvent ();
		}
	}
	
	/// <summary>
	/// 前往下一个点
	/// </summary>
	private void gotoNextCallback ( bool autoMove ) {
		if (character.state == fuben_characterState.stand) {
			character.changeState (fuben_characterState.move);
			//	mapInfo.getPlayerBeforePointInfo ().pointCtrl.ScaleDown ();

			//如果走下一个点不成功就返回
			//if (mapInfo.moveNextPoint () == false)
			//	return; 

			//	mapInfo.getPlayerPointInfo ().pointCtrl.ScaleUp ();
			character.beginMove (autoMove);
		}
		
		int playCurrentStep = MissionInfoManager.Instance.mission.getPlayerPointIndex ();
		MissionManager.instance.missionRoad.M_goStep (playCurrentStep, adjustRoleCamera);
	}

	/// <summary>
	/// 调整摄像机旋转到角色
	/// </summary>
	private void adjustRoleCamera ( bool immediately ) {
		int playCurrentStep = MissionInfoManager.Instance.mission.getPlayerPointIndex ();
		playCurrentStep++;
		Vector3 rotation = MissionManager.instance.missionRoad.M_calculateSegmentRotatioin (playCurrentStep);
		GameObject targetCamera = MissionManager.instance.roleCamearT.gameObject;
		if (immediately) {
			roleCamearT.transform.rotation = Quaternion.Euler (rotation);
		}
		else {
			iTween.RotateTo (targetCamera, iTween.Hash ("rotation", rotation, "easetype", iTween.EaseType.easeInOutCubic, "time", 1.5f));
		}
	}

	/// <summary>
	/// 调整摄像机旋转到角色_缓动
	/// </summary>
	private void adjustRoleCamera () {
		adjustRoleCamera (false);
	}

	/// <summary>
	/// 自动向前移动
	/// </summary>
	public IEnumerator autoMove ( float time ) {
		yield return new WaitForSeconds (time);
		if (MissionInfoManager.Instance.mission.getPlayerPoint ().canAutoMove ()) {
			isAutoRun = true;
			moveForward ();
		}
		else {
			MaskWindow.UnlockUI ();
		}
	}

	//事件处理显示回调函数 不涉及逻辑处理 
	private void doEventCallBack ( MissionEventSample e ) {
		if (e == null) {
			Debug.LogError ("------------------------->>>>back");
			//所有事件都完成了
			MaskWindow.UnlockUI ();
		}
		else {
			if (MissionManager.instance == null)
				return;
			if (e.cost > 0) {
				EffectManager.Instance.CreateActionCast ((-e.cost).ToString ());
				UserManager.Instance.self.costPoint (e.cost, e.costType);
			}
			int i = e.eventType;
			//这里写针对每个事件的处理显示方法 
			if (i == MissionEventType.SWITCH) {
				doSwitch ();
			}
			else if (i == MissionEventType.PLOT) {  //剧情对话
				//老副本直接跳过剧情
				if (!FuBenManagerment.Instance.isNewMission (ChapterType.STORY, MissionInfoManager.Instance.getMission ().sid)) {
					MissionManager.instance.updateEventObj ();
					StartCoroutine (autoMove (0.2f));
				}
				else {
					isTalking = true;
					UiManager.Instance.missionMainWindow.TweenerGroupOut ();
					if(MissionInfoManager.Instance.autoGuaji){
						UiManager.Instance.openWindow<EmptyWindow> (( win ) => {
							UiManager.Instance.openDialogWindow<TalkWindow> (( window ) => {
								window.Initialize (e.other, showTalkWindowCallBack, null,true);
								window.setIsSkip (MissionInfoManager.Instance.mission.isNew ());
							});
						});
					}else{
						UiManager.Instance.openWindow<EmptyWindow> (( win ) => {
							UiManager.Instance.openDialogWindow<TalkWindow> (( window ) => {
								window.Initialize (e.other, showTalkWindowCallBack, null);
								window.setIsSkip (MissionInfoManager.Instance.mission.isNew ());
							});
						});
					}
				}
			}
			else if (i == MissionEventType.TREASURE || i == MissionEventType.RESOURCES||i==MissionEventType.TOW_OVER) {//或者爬塔副本抽奖完成
                //途中捡到物品
                updateEventObj();
                character.happy();
                userState = MISSION_GETAWARD;//标注一下，是刚捡完宝箱
                //播放捡宝箱音效
                AudioManager.Instance.PlayAudio(135);
                //等扣行动力显示完成
                StartCoroutine(showResDelay(e));
                StartCoroutine(autoMove(1f));
			}else if(i==MissionEventType.TOW_TREASURE){//就进入爬塔开宝箱的界面啦
                ClmbTowerManagerment.Instance.getAwardSuccessCallBack = () => {
                    UiManager.Instance.missionMainWindow.updateUserInfo();
                    MissionInfoManager.Instance.mission.doEvent(doEventCallBack);
                };
                ClmbTowerManagerment.Instance.getGiveUpCallBack = () => {
					if(UiManager.Instance.getWindow<MissionMainWindow>() != null)
                    	UiManager.Instance.missionMainWindow.updateUserInfo();
					if(MissionInfoManager.Instance.mission != null)
                    	MissionInfoManager.Instance.mission.doEvent(doEventCallBack);
					if(UiManager.Instance.getWindow<MissionMainWindow>() != null)
                    	UiManager.Instance.missionMainWindow.updateUserInfo();
                };
                ClmbTowerManagerment.Instance.boxMissionSid = MissionInfoManager.Instance.mission.sid;
                ClmbTowerManagerment.Instance.intoTpye = 1;
                ClmbTowerManagerment.Instance.checkCanIntoTower();
            }
			else if (i == MissionEventType.PVP) { //遇到玩家pk
				updateEventObj ();
			}
			else if (i == MissionEventType.REST && PvpInfoManagerment.Instance.getIsStartTime () == true) { //

			}
			else if (i == MissionEventType.REST && PvpInfoManagerment.Instance.getIsStartTime () == false && MissionInfoManager.Instance.mission.restPointNoPvP == true) {
				//如果pvp不在计时中,.那么需要满足 休息点+没有触发pvp 这个条件
				MaskWindow.UnlockUI ();
			}
		}
	}

	/// <summary>
	/// 显示获得资源
	/// </summary>
	IEnumerator showResDelay ( MissionEventSample e ) {
		yield return new WaitForSeconds (0.5f);
		int i = e.eventType;
		if (i == MissionEventType.RESOURCES) {
			AwardManagerment.Instance.addFunc (AwardManagerment.AWARDS_FUBEN_RES, getResouce);
		}
		else {
			getTreasure (e.other);
			MissionInfoManager.Instance.mission.addTreasures (e.other);
		}
	}

	/// <summary>
	/// 执行跳转事件
	/// </summary>
	private void doSwitch () {
		ScreenManager.Instance.loadScreen (4, missionClean, null);
	}

	/// <summary>
	/// 战斗失败
	/// </summary>
	private void fightLoss () {
		MissionInfoManager.Instance.mission.updateBoss ();
		updateBossHp ();
	}

	/// <summary>
	/// pvp时间限制
	/// </summary>
	private void doPvp ( int isOK ) {
		if (isOK == 1) {
			if (PvpInfoManagerment.Instance.getPvpTime (null) <= 0) {
				PvpInfoManagerment.Instance.initPvpEvent ();//初始化PVP事件
				PvpInfoManagerment.Instance.setPvpType (PvpInfo.TYPE_PVP_FB);
			}
			else {
				MissionInfoManager.Instance.mission.sendGo (gotoNextCallback);
			}
		}
		else if (isOK == 2) {
			UiManager.Instance.openDialogWindow<MessageWindow> (( window ) => {
				window.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0093"), "", LanguageConfigManager.Instance.getLanguage ("s0227"), null);
			});
		}
		else if (isOK == 3) {
			MissionInfoManager.Instance.mission.sendGo (gotoNextCallback);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	void Update () {
		//npc对missionWindow 有依赖,要显示名字在窗口上
		if (UiManager.Instance.missionMainWindow != null && MissionInfoManager.Instance.mission!=null && AutoRunIndex == -1)
			MissionNpcManagerment.Instance.AI ();
	}

	/// <summary>
	/// 更新boss血量
	/// </summary>
	private void updateBossHp () {
		if (MissionInfoManager.Instance.mission.getPlayerPoint ().getPointEvent ().eventType == MissionEventType.BOSS) {
			MissionInfoManager.Instance.mission.updateBoss ();
		}
	}

	/// <summary>
	/// 创建资源特效,在当前人物位置
	/// </summary>
	public void getResouce ( Award[] award ) {
		foreach (Award each in award) {
			if (each.type == AwardManagerment.RES) {
				if (each.moneyGap > 0) {
					EffectManager.Instance.CreateGetResourceLabel (character.transform.position, constResourcesPath.MONEYIMAGE, each.moneyGap);
				}
				if (each.rmbGap > 0) {
					EffectManager.Instance.CreateGetResourceLabel (character.transform.position, constResourcesPath.RMBIMAGE, each.rmbGap);
				}
				if (each.expGap > 0) {
					EffectManager.Instance.CreateGetResourceLabel (character.transform.position, "", each.expGap);
				}
			}
		}
	}

	/// <summary>
	/// 创建宝箱特效,在当前人物位置
	/// </summary>
	public void getTreasure ( int treasureType ) {
		if (treasureType == TreasureType.TREASURE_SILVER) {
			EffectManager.Instance.CreateGetResourceLabel (character.transform.position, constResourcesPath.SLIVERTREASURE, 1);
		}
		if (treasureType == TreasureType.TREASURE_GOLD) {
			EffectManager.Instance.CreateGetResourceLabel (character.transform.position, constResourcesPath.GOLDTREASURE, 1);
		}
	}

	/// <summary>
	/// 清除指定点事件
	/// </summary>
	public void cleanPointEvent ( MissionPoint info ) {
		if (info.eventObj != null) {
			info.eventObj.Hide ();
		}
	}

	/// <summary>
	/// 清除前一个点
	/// </summary>
	private void clearBeforePoint () {
		MissionPoint info = mapInfo.getPlayerBeforePointInfo ();
		if (info == null || info.eventObj == null)
			return;
		cleanPointEvent (info);
	}

	/// <summary>
	/// 自动跑经过一个关卡
	/// </summary>
	private void autoRunItweenOnUpdateNode (Vector3[] arr) {
		if (AutoRunIndex < arr.Length) {
			iTween.MoveTo (character.gameObject, iTween.Hash (
					"position", arr[AutoRunIndex],
					"time", 0.3f,
					"easeType", iTween.EaseType.linear,
					"oncompleteTarget", gameObject,
					"oncomplete", "autoRunItweenOnUpdateNode",
					"oncompleteParams", arr));
			if (AutoRunIndex + 2 < arr.Length) {
				int type = mapInfo.getAllPointData ()[AutoRunIndex + 1].getPointEvent ().eventType;
				mapInfo.getAllPointData ()[AutoRunIndex + 1].getPointEvent ().eventType = TreasureType.TREASURE_SILVER;
				DrawEventObj (mapInfo.pointList[AutoRunIndex + 1], mapInfo.getAllPointData ()[AutoRunIndex + 1].getPointEvent (), true);
				mapInfo.getAllPointData ()[AutoRunIndex + 1].getPointEvent ().eventType = type;
				UiManager.Instance.getWindow<MissionPracticeRunTipsWindow> ().SetCurrentNode(AutoRunIndex);
			}

			//EffectManager.Instance.CreateGetResourceLabel (character.transform.position, constResourcesPath.GOLDTREASURE, 1);
			EventObjCtrl obj = mapInfo.pointList[AutoRunIndex - 1].eventObj;
			if (obj != null) obj.Hide ();

			character.activeAnimCtrl.transform.LookAt (arr[AutoRunIndex]);
			if (Mathf.Abs ((character.activeAnimCtrl.transform.eulerAngles.y - 60) - roleCamearT.transform.localEulerAngles.y) > 10 && roleCamearT.GetComponent<iTween>() == null)
				iTween.RotateTo (roleCamearT.gameObject, iTween.Hash ("y", character.activeAnimCtrl.transform.eulerAngles.y - 60, "time", AutoRunIndex == 0 ? 0 : 1, "easetype", iTween.EaseType.easeInOutQuad));
			AutoRunIndex++;
		}
		else
			autoRunItweenOnComplete ();
	}

	/// <summary>
	/// 开始自动跑
	/// </summary>
	public void AutoRunStart () {
		MaskWindow.LockUI ();
		Vector3[] posArr	= new Vector3[mapInfo.getPlayerPointInfo ().pointIndex + 1];
		for (int i = 0; i < posArr.Length; i++) {
			posArr[i] = MissionManager.instance.missionRoad.M_getPosition (i + 1);
			posArr[i].y = character.transform.position.y;
		}
		AutoRunIndex = 1;
		if(character.activeAnimCtrl!=null)
			character.activeAnimCtrl.animation["run"].speed = 1.8f;
		character.playMove ();
		character.transform.position = posArr[0];
		character.enabled = false;
		UiManager.Instance.missionMainWindow.destoryWindow ();
		AddEffect(character);
		UiManager.Instance.openWindow<MissionPracticeRunTipsWindow> (( win ) => { autoRunItweenOnUpdateNode (posArr); });
	}

	EffectCtrl autoRunEffect;

	public void AddEffect(MissionCharacterCtrl character){
		if(AutoRunIndex != -1){
			autoRunEffect = EffectManager.Instance.CreateEffect (character.animCtrl.gameObject.transform, "Effect/Other/Haste");
			autoRunEffect.transform.localPosition = new Vector3 (0.36f, 0, 0f);
			autoRunEffect.transform.localScale = Vector3.one;
			autoRunEffect.transform.localEulerAngles = new Vector3(0f,-90,0f);
		}
	}

	public void CloseEffect(){
		if(autoRunEffect!=null)
			EffectManager.Instance.removeEffect (autoRunEffect); 
	}

	/// <summary>
	/// 当试炼自动跑到终点
	/// </summary>
	private void autoRunItweenOnComplete () {
		character.enabled = true;
		character.activeAnimCtrl.animation["run"].speed = 1;
		character.changeState (fuben_characterState.stand);
		character.playStand ();
		iTween.RotateTo (character.gameObject, iTween.Hash ("rotation", Vector3.zero, "time", 2, "oncomplete", "moveArriveByPractice", "easetype", iTween.EaseType.easeInOutQuad));
		iTween.RotateTo (roleCamearT.gameObject, iTween.Hash ("y", 0, "time", 1, "easetype", iTween.EaseType.easeInOutQuad));
		AutoRunIndex = -1;
		updateEventObj ();
		CloseEffect();
		UiManager.Instance.openWindow<MissionMainWindow> ((win) => {
			win.practicePointSaveTip.playAni (mapInfo.getPlayerPointInfo ().pointIndex, false);
			win.practiceAwardDisplay.addAllAward (mapInfo.getPlayerPointInfo ().pointIndex);
			if(MissionInfoManager.Instance.autoGuaji){
				win.stopButton.gameObject.SetActive(true);
				StartCoroutine(MissionManager.instance.autoMove(2f));
			}
		});
	}

	/// <summary>
	/// 更新下一个点事件 到达下一个点时触发该方法
	/// </summary>
	public void updateNextPoint ( bool isMove, bool isAutoMove ) {
		clearBeforePoint ();
		//立即执行事件
		if (isMove && MissionInfoManager.Instance.mission.getPlayerPoint ().isNeedDoEventImmediately ()) {
			doEvent ();
		}
		else {
			//如果要自动行走,则不取消遮罩
			if (isAutoMove != true)
				MaskWindow.UnlockUI ();
		}

		MissionPointInfo point = MissionInfoManager.Instance.mission.GetPlayerNextPoint ();	//获取下个节点的逻辑信息
		if (point == null) {
			return;
		}

		MissionPoint info = point.PointInfo;

		if (info != null && info.eventObj != null) {
			Destroy (info.eventObj.gameObject);
		}

		DrawEventObj (info, point.getPointEvent (), false);

		if (isAutoMove)
			StartCoroutine (autoMove (0f));
	}

	//构建角色(棋子)
	public void CreateRole () {
		passObj _obj = Create3Dobj (UserManager.Instance.self.getModelPath ());

		if (_obj.obj == null) {
			Debug.LogError ("role is null!!!");
			return;
		}
		_obj.obj.transform.parent = instance.character.transform;
		_obj.obj.transform.localPosition = new Vector3 (_obj.obj.transform.localPosition.x, 0.03f, _obj.obj.transform.localPosition.z);
		_obj.obj.transform.localScale = Vector3.one;
		_obj.obj.name = "role";

		FuBenCardCtrl cardCtrl = _obj.obj.transform.GetChild (0).gameObject.GetComponent<FuBenCardCtrl> ();
		
		// 第一版暂时写死
		Mounts mounts=MountsManagerment.Instance.getMountsInUse ();

		instance.character.initRoleAniCtrl (cardCtrl, mounts,UserManager.Instance.self.getVipLevel());

		/////新加入试炼的判断,如果是试炼就启动相应的功能,
		if (MissionInfoManager.Instance.mission.getChapterType () == ChapterType.PRACTICE && !BattleGlobal.isBackFromBattle && mapInfo.getPlayerPointInfo ().pointIndex + 1 > 1 && AutoRunIndex != -2) {
			AutoRunIndex = 0;
            character.transform.position = new Vector3(MissionManager.instance.missionRoad.M_getPosition(1).x, character.transform.position.y, MissionManager.instance.missionRoad.M_getPosition(1).z);
           // character.transform.position=new Vector3(mapInfo.getPlayerPointInfo().woldPos.x, instance.character.transform.position.y, mapInfo.getPlayerPointInfo().woldPos.z);
		}
		else {
			AutoRunIndex = -1;
			instance.character.transform.position = new Vector3 (mapInfo.getPlayerPointInfo ().woldPos.x, instance.character.transform.position.y, mapInfo.getPlayerPointInfo ().woldPos.z);
		}

		if (MissionInfoManager.Instance.mission != null) {
			if (MissionInfoManager.Instance.mission.getChapterType () == ChapterType.PRACTICE) {
				int currentStep=mapInfo.getPlayerPointInfo ().pointIndex;

				if (BattleGlobal.isBackFromBattle) {
					//播放修炼奖励飞行动画,只有赢的时候才有
					if (BattleGlobal.isPracticeWarWin) {
						showPracticeAward (currentStep, true);
					
					//判断保存点
					if (FuBenPracticeConfigManager.Instance.checkIndexIsSavePoint (currentStep)) {
						MissionMainWindow mainWin=UiManager.Instance.getWindow<MissionMainWindow> ();
						if (mainWin != null) {
							mainWin.practicePointSaveTip.playAni (currentStep, true);
						}
					}
					}
				}
				else {
					MissionMainWindow mainWin=UiManager.Instance.getWindow<MissionMainWindow> ();
					if (mainWin != null) {
						//mainWin.practicePointSaveTip.playAni (currentStep, false);
					}
					showPracticeAward (currentStep, false);
				}
			}
			BattleGlobal.isPracticeWarWin = false;
			BattleGlobal.isBackFromBattle = false;
		}
	}

	/// <summary>
	/// 显示修炼奖励动效
	/// </summary>
	private void showPracticeAward ( int index, bool hasEffect ) {
		MissionMainWindow mainWin=UiManager.Instance.getWindow<MissionMainWindow> ();
		if (mainWin == null && !mainWin.practiceAwardDisplay.gameObject.activeSelf) {
			return;
		}
		mainWin.practiceAwardDisplay.gameObject.SetActive (true);
		if (hasEffect) {
			//动画之前 先显示原来的奖励
			mainWin.practiceAwardDisplay.updateAward (index - 1);
			PrizeSample prize=FuBenPracticeConfigManager.Instance.getPrizeByIndex (index);
			if (prize != null) {
				mainWin.practiceAwardDisplay.addPrizeAnimation (prize, index);
			}
		}
		else {
			mainWin.practiceAwardDisplay.updateAward (index);
		}
	}

    public void updatePoints()
    {
        //绘制所有点
        foreach (MissionPoint each in mapInfo.pointList) {
            if (!each.pointOnRoad) {
                continue;
            }
            passObj _obj = null;
            if (each == mapInfo.pointList[0]) {
                //_obj = Create3Dobj ("mission/point_start");
            } else if (each == mapInfo.pointList[mapInfo.pointList.Count - 1]) {
                if (pointObjRoot.FindChild("point_end(Clone)") == null)
                    _obj = Create3Dobj("mission/point_end");
            }


            if (_obj == null) {
                continue;
            }
            if (_obj.obj == null) {
                Debug.LogError("point is null!!!");
                continue;
            }
            _obj.obj.transform.parent = pointObjRoot;
            _obj.obj.transform.position = each.woldPos;
            _obj.obj.transform.localScale = Vector3.one;
            //	_obj.obj.name = each.
            each.pointCtrl = _obj.obj.gameObject.GetComponent<MissionPointCtrl>();
        }
    }

    /// <summary>
	/// 绘制点和背景
	/// </summary>
	public void DrawPointObj () {

		//绘制所有点
		foreach (MissionPoint each in mapInfo.pointList) {
			if (!each.pointOnRoad) {
				continue;
			}
			passObj _obj = null;
			if (each == mapInfo.pointList[0]) {
				//_obj = Create3Dobj ("mission/point_start");
			}
			else if (each == mapInfo.pointList[mapInfo.pointList.Count - 1]) {
				_obj = Create3Dobj ("mission/point_end");
			}


			if (_obj == null) {
				continue;
			}
			if (_obj.obj == null) {
				Debug.LogError ("point is null!!!");
				continue;
			}

			_obj.obj.transform.parent = pointObjRoot;
			_obj.obj.transform.position = each.woldPos;
			_obj.obj.transform.localScale = Vector3.one;
			//	_obj.obj.name = each.
			each.pointCtrl = _obj.obj.gameObject.GetComponent<MissionPointCtrl> ();
		}

		//已经绘制过的点
		ArrayList arrl = new ArrayList ();
		//绘制点上的事件背景等
		for (int i=0; i < mapInfo.getAllPointData ().Length; i++) {

			//获得显示对象
			MissionPointCtrl point = null;
			MissionPointInfo each = mapInfo.getAllPointData ()[i];
			foreach (Transform p in pointObjRoot.transform) {
				if (each.getPointLoction ().ToString () == p.name && each.getMapId () == MissionInfoManager.Instance.mission.getPlayerPoint ().getMapId ()) {
					point = p.gameObject.GetComponent<MissionPointCtrl> ();
					break;
				}
			}
			if (point == null)
				continue;

			//绘制背景			
			if (each.getBgId () != 0) {
				MissionPoint info = each.PointInfo;
				if (info.bgObj != null)
					continue;
				//绘制点的背景
				passObj _obj2 = Create3Dobj ("mission/town_" + each.getBgId ());
				_obj2.obj.transform.parent = pointObjRoot;
				_obj2.obj.transform.localPosition = point.gameObject.transform.localPosition;
				_obj2.obj.transform.localScale = Vector3.one;
				info.bgObj = _obj2.obj;
				//each.PointInfo.bgObj = _obj2.obj;
			}
		}

		updateEventObj ();
		updateNextPoint (false, false);

	}

	//正常结算完毕。退出副本 
	public void missionEnd () {
		LoadingWindow.isShowProgress = false;
		missionClean ();
		UiManager.Instance.switchWindow<EmptyWindow> (( win ) => {
			EventDelegate.Add (win.OnStartAnimFinish, () => {
				ScreenManager.Instance.loadScreen (1, cleanCache, GameManager.Instance.outMission);
			});
		});
	}

	/// <summary>
	/// 通关效果相关
	/// </summary>
	public void showCompleteEffectOver () {
		FuBenManagerment.Instance.completeMission (MissionInfoManager.Instance.mission.sid, MissionInfoManager.Instance.mission.getChapterType (), MissionInfoManager.Instance.mission.starLevel);
		ArmyManager.Instance.unActiveArmy ();
		AwardManagerment.Instance.addFunc (HeroRoadManagerment.Instance.currentHeroRoad == null ? AwardManagerment.AWARDS_FUBEN_OVER : AwardManagerment.AWARDS_HERO_ROAD, awardPlayer);
	}

	/// <summary>
	/// 遇到boss战斗之前
	/// </summary>
	public void showBossWarringCompleteEffectOver () {
		prepareBattle (true);
	}

	//完成副本
	public void missionComplete () {
		character.happy ();
		if (UiManager.Instance.missionMainWindow != null) {
			UiManager.Instance.missionMainWindow.levelupRewardButton.gameObject.SetActive (false);
			UiManager.Instance.missionMainWindow.nvshenItem.gameObject.SetActive (false);
			UiManager.Instance.missionMainWindow.fubenName.transform.parent.gameObject.SetActive (false);
			//UiManager.Instance.missionMainWindow.showMissionCompleteEffect ();
			MissionManager.instance.showCompleteEffectOver ();
		}
		else {
			MissionManager.instance.showCompleteEffectOver ();
		}
	}

	/// <summary>
	/// 播放显示奖励
	/// </summary>
	public void awardPlayer ( Award[] awards ) {
		if (awards != null) {
			AwardDisplayCtrl ctrl = MissionManager.instance.gameObject.AddComponent<AwardDisplayCtrl> ();
			ctrl.Initialize (awards, AwardManagerment.FB_END);
		}
	}

	/// <summary>
	/// 清除场景数据
	/// </summary>
	public void missionClean () {
		isLoadFinish = false;
		mapInfo.clean ();
		mapInfo = null;
		instance = null;
		character = null; 
		if (MissionNpcManagerment.Instance == null) {
			MissionNpcManagerment.Instance.removeAllGraphicData ();
		}
	}

	//MissionPointInfo 点的显示对象 
	//MissionEvent 需要绘制的事件 bool
	//now 是否是当前点
	public void DrawEventObj ( MissionPoint each, MissionEventSample _event, bool now ) {
		if (_event == null)
			return;

		if (_event.eventType != MissionEventType.NONE && _event.eventType != MissionEventType.REST) {

			//最新策划不标识剧情
			if (_event.eventType == MissionEventType.PLOT || _event.eventType == MissionEventType.OVER)
				return;

			//绘制点上的第一个事件图标 
            passObj obj;
            if(_event.eventType==11){
                obj = Create3Dobj("mission/event_2");
            } else {
                obj = Create3Dobj("mission/event_" + _event.eventType);
            }
			
			if (obj.obj == null)
				return;

			obj.obj.transform.parent = pointObjRoot;
            if (_event.eventType == MissionEventType.TREASURE||_event.eventType==MissionEventType.TOW_TREASURE) {//如果是宝箱就偏移0.2个单位
                obj.obj.transform.position = each.woldPos+new Vector3(0f,0f,0.2f);
            } else {
                obj.obj.transform.position = each.woldPos;
            }
			

			each.eventObj = obj.obj.GetComponent<EventObjCtrl> ();
			if (_event.eventType == MissionEventType.BOSS) {
				if (now) {
					Boss boss = MissionInfoManager.Instance.mission.getBoss (_event.other);
					if (boss != null && boss.hp != 0) {
						each.eventObj.setBossHP (boss.hp, boss.max);
					}
					else {
						each.eventObj.setBossHP (1, 1);
					}
				}
				else {
					each.eventObj.setBossHP (1, 1);
				}
			}
			else if (_event.eventType == MissionEventType.FIGHT && now == true) {
				//图标上移
				obj.obj.transform.localPosition += new Vector3 (0, 0.7f, 0);
			}
			each.eventObj.Show (_event);
		}
	}

	/// <summary>
	/// 缓存场景资源
	/// </summary>
	public IEnumerator cacheData () {
		CreateMapInfo ();
		string[] _list = new string[]{	
			mapInfo.packPath,	
			"mission/event_1",
			"mission/event_2",
			"mission/event_3",
			"mission/event_4",
			"mission/event_6",
			"mission/event_7",
			"mission/event_8",
			"mission/event_9",
            "mission/event_11",
			"mission/ez",
			"mission/girl",
			"mission/line",
			"mission/mage",
			"mission/maleMage",
			"mission/point",
			"mission/point_end",
			"mission/point_start",
			"mission/swordsman",
			"mission/archer",
			"Effect/UiEffect/luckybox_effect",
			"Effect/Other/Haste",
			};
//		string[] _list2 = new string[_list.Length + MountsResourceManager.Instance.GetPaths().Length];
//		_list.CopyTo(_list2,0);
//		MountsResourceManager.Instance.GetPaths().CopyTo(_list2,_list.Length);
		if (ResourcesManager.Instance.allowLoadFromRes) {
			ResourcesManager.Instance.cacheProgress = 1;
			cacheWindowFinish (new List<ResourcesData> ());
		}
		else {
			ResourcesManager.Instance.cacheData (_list, cacheWindowFinish, "base");
		}
		yield break;
	}

	/// <summary>
	/// 延迟初始化场景
	/// </summary>
	IEnumerator waitMissionWindow () {
		while (UiManager.Instance.missionMainWindow == null) {
			yield return 0.2f;
		}
		initMission ();
	}

	/// <summary>
	/// 缓存完成
	/// </summary>
	public void cacheWindowFinish ( List<ResourcesData> _list ) {
		cacheList = _list;
		setBackGround ();
		StartCoroutine (waitMissionWindow ());
	}

	/// <summary>
	/// 初始化场景
	/// </summary>
	public void initMission () {
		createMap ();
	}

	/// <summary>
	/// 初始化路径完成
	/// </summary>
	private void onRoadCmp () {
		adjustRoleCamera (true);
		DrawPointObj ();
		CreateRole ();
		initWeather ();
		initMissionEvent ();
		getNPC ();
		isLoadFinish = true;
		if (FuBenManagerment.Instance.cacheFinishCallBack != null) {
			FuBenManagerment.Instance.cacheFinishCallBack ();
			FuBenManagerment.Instance.cacheFinishCallBack = null;
		}
		else {
			StartCoroutine (autoMove (0f));
		}
	}

	/// <summary>
	/// 初始化场景事件
	/// </summary>
	void initMissionEvent () {
		if (MissionInfoManager.Instance.mission.getChapterType () == ChapterType.STORY) {
			FubenGetStarFPort userFPort = FPortManager.Instance.getFPort ("FubenGetStarFPort") as FubenGetStarFPort;
			userFPort.getStar (UserManager.Instance.self.updateStarSum, playStarEffect);
		}
		else if (MissionInfoManager.Instance.mission.getChapterType () == ChapterType.HERO_ROAD) {
			//处理英雄之章副本中途退出的情况
			if (HeroRoadManagerment.Instance.currentHeroRoad == null) {
				int cid = MissionInfoManager.Instance.mission.getChapterSid ();
				foreach (HeroRoad road in HeroRoadManagerment.Instance.map.Values) {
					if (road.sample.chapter == cid) {
						HeroRoadManagerment.Instance.currentHeroRoad = road;
						break;
					}
				}
			}
		}
		//只有修炼副本战败会推出
		if (MissionInfoManager.Instance.mission.isExit && MissionInfoManager.Instance.mission.getChapterType () == ChapterType.PRACTICE) {

			UiManager.Instance.missionMainWindow.TweenerGroupOut ();
			UiManager.Instance.openDialogWindow<PracticeAwardWindow> (( window ) => {
				window.init (MissionInfoManager.Instance.mission);
				window.updateAward (MissionAward.Instance.parcticeAwards);
			});

		}
	}

	/// <summary>
	/// 创建npc
	/// </summary>
	void getNPC () {

		if (MissionNpcManagerment.Instance != null)
			MissionNpcManagerment.Instance.cleanData ();

		if (MissionManager.userState == MissionManager.MISSION_NEWIN) {
			FuBenGetPlayersFPort pFPort = FPortManager.Instance.getFPort ("FuBenGetPlayersFPort") as FuBenGetPlayersFPort;
			pFPort.get_players (() => {
				MissionNpcManagerment.Instance.initAI ();
			});
		}
	}

	/// <summary>
	/// 准备播放收集到幸运星
	/// </summary>
	public void playStarEffect () {
		if (GuideManager.Instance.isOverStep (GuideGlobal.NEWOVERSID)) {
			if (UserManager.Instance.self.getBattleStarNum () > 0) {
				int num = UserManager.Instance.self.getBattleStarNum ();
				if (UiManager.Instance.missionMainWindow != null) {
					StartCoroutine (showStar (num));
				}
			}
		}
	}

	/// <summary>
	/// 播放收集到幸运星
	/// </summary>
	private IEnumerator showStar ( int num ) {
		int hit = 1;
		if (FuBenManagerment.Instance.checkStarMultipleTime ())
			hit = FuBenManagerment.Instance.getStarHit ();
		num = num / hit;
		if (num > 10) {
			num = 10;
		}
		yield return new WaitForSeconds (0.7f);
		for (int i = 0; i < num; i++) {
			GameObject obj = Create3Dobj ("Effect/Other/battleStar").obj;
			obj.GetComponent<BattleStar> ().addValue = hit;
			obj.transform.parent = UiManager.Instance.UIEffectRoot.transform;
			obj.transform.localScale = Vector3.one;

			int pathNum = i >= 5 ? (i - 5) : i;

			iTween.MoveTo (obj, iTween.Hash ("path", iTweenPath.GetPath ("path" + pathNum), "oncomplete", "destory", "easetype", iTween.EaseType.easeInOutQuad, "time", 1f));

			StartCoroutine (Utils.DelayRun (() => {
				AudioManager.Instance.PlayAudio (138);
			}, 0.8f));
			yield return new WaitForSeconds (UnityEngine.Random.Range (0.1f, 0.2f));
		}
		if (MissionInfoManager.Instance.mission != null && MissionInfoManager.Instance.mission.getChapterType() == ChapterType.STORY&&GameManager.Instance.isCanBeSecondSkill && UiManager.Instance.missionMainWindow != null)
	    {
            StartCoroutine(Utils.DelayRun(() => {
            UiManager.Instance.missionMainWindow.updateUserInfo();
            }, 1.2f));
	    }
		UserManager.Instance.self.resetCurrentStarNum ();
	}

	/// <summary>
	/// 退出
	/// </summary>
	public void exit () {
		ArmyManager.Instance.clearArmyState ();
		missionEnd ();
	}

	/// <summary>
	/// 播放打开宝箱特效
	/// </summary>
	void playOpenTreasureEffect ( Award award ) {
		if (UiManager.Instance.missionMainWindow.goldBoxCount == 0 && UiManager.Instance.missionMainWindow.sliverBoxCount == 0) {
			getFirstBloodAward ();
			return;
		}
		UiManager.Instance.openWindow<OpenTreasureWindow> (( win ) => {
			win.award = award;
		});
	}

	/// <summary>
	/// 显示宝箱奖励
	/// </summary>
	public void getTreasureAward () {
		AwardDisplayCtrl ctrl = gameObject.GetComponent<AwardDisplayCtrl> ();
		Award award = ctrl.changeActiveAward (AwardManagerment.MNGV);
		//为空下一步拿首通
		playOpenTreasureEffect (award);
	}

	/// <summary>
	/// 
	/// </summary>
	public void getFirstBloodAward () {
		AwardDisplayCtrl ctrl = gameObject.GetComponent<AwardDisplayCtrl> ();
		Award award = ctrl.changeActiveAward (AwardManagerment.FIRST);

		ctrl.openNextWindow ();
	}


	//一下代码保留不要删除，可能会用到
	/**
	private void outMissionMainWindow ()
	{
		MessageWindow.ShowAlert (Language ("reconnection_01"), (msg) => {
			LoadingWindow.isShowProgress = false;
			UiManager.Instance.switchWindow<EmptyWindow> (
				(win) => {
				MissionManager.instance.cleanCache ();
				ScreenManager.Instance.loadScreen (1, MissionManager.instance.missionClean, saveMission);
			}
			);
		});
	}
	
	public void saveMission ()
	{ 
		int type = MissionInfoManager.Instance.mission.getChapterType ();
		MissionInfoManager.Instance.clearMission (); 
		GuideManager.Instance.guideEvent ();
		reLoadMissionMainWindow (type);
	}
	
	private void reLoadMissionMainWindow (int type)
	{
		FuBenInfoFPort port = FPortManager.Instance.getFPort ("FuBenInfoFPort") as FuBenInfoFPort;
		port.info (getFubenCurrentInfo, type);
	}
	
	private void getFubenCurrentInfo ()
	{
		FuBenGetCurrentFPort port = FPortManager.Instance.getFPort ("FuBenGetCurrentFPort") as FuBenGetCurrentFPort;
		port.getInfo (getContinueMission);
	}
	
	private void getContinueMission (bool b)
	{  
		if (b) {
			MissionInfoManager.Instance.mission.updateBoss ();	
			continueMission (); 
		} else { 
			//如若副本已完成，直接停留主界面
			UiManager.Instance.openMainWindow ();
			UiManager.Instance.createMessageLintWindow (Language ("reconnection_02"));
		} 
	}
	
	private void continueMission ()
	{
		FuBenIntoFPort port = FPortManager.Instance.getFPort ("FuBenIntoFPort") as FuBenIntoFPort;
		port.toContinue (continueIntoMission); 
	}
	
	private void continueIntoMission ()
	{ 
		LoadingWindow.isShowProgress = false;
		ScreenManager.Instance.loadScreen (4, null, () => {
			UiManager.Instance.switchWindow<MissionMainWindow> ();
		});
	}
	**/
}

