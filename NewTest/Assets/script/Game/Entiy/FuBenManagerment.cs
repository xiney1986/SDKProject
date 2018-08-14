using UnityEngine;
using System;
using System.Collections.Generic;

/**
 * 副本管理器
 * @author longlingquan
 * */
public class FuBenManagerment
{
	//通关奖品
	private List<ChapterAwardServerSample> awardsList;
	//剧情副本完成情况 [sid,step]
	private int[][] storyInfos;
	//讨伐副本完成情况
	private int[] warInfos; 
	//修炼副本完成情况
	private int[] practiceInfos;
    //爬塔副本完成情况[mission]
    private int[] towerInfos;
    private int[] refistTowerMissionSid;//那些已经领取过免费开宝箱的副本Sid
	//讨伐章节
	private Chapter warChapter;
	//活动副本章节集合
	private ActivityChapter[] chapters;
	//修炼副本章节
	private Chapter practiceChapter;
    //爬塔副本章节
    private Chapter towerChapter;
	//修炼副本倒计时
	public int practiceDueTime;
	//幸运星信息 开始时间 结束时间 倍数
	private ActiveTime starTime;
	//幸运星信息 倍数
	private int starHit;

	/** 缓存MissionManager窗口完成后回调 */
	public CallBack cacheFinishCallBack;

	//进副本前的最后ID,通关后会有新ID,给显示层做地球转动的数据基础
	int  oldLastStoryChapterID = 0;
	//进入爬塔的最后id 一层通过以后会有新的ID
    int oldLastTowerChapterID = 0;
	//选择副本的流程变量
	public int selectedMapSid;
	public int selectedChapterSid;
	public int selectedMissionSid;
	/** 是否挑战讨伐BOSS */
	public bool isWarAttackBoss = false;
	/** 是否抢矿 */
	public bool isMineralAttack = false;
	/** 是否小组赛战斗 */
	public bool isGodsWarGroup = false;
	/** 是否淘汰赛战斗 */
	public bool isGodsWarFinal = false;
	/** 是否挑战讨伐BOSS成功 */
	public bool isWarAttackBossWin = true;
    /** 是否进行活动副本 */
    public bool isWarActiveFuben = false;
    /** 是否挑战活动副本成功 */
    public bool isWarActiveWin = true;

	/** 挑战讨伐BOSS成功后奖励 */
	public Award[] warWinAward;
    /** 挑战活动副本成功奖励 */
    public Award[] ActiveWinAward;
	/** 临时仓库版本号 */
	public int tmpStorageVersion = -1;
	
	public static FuBenManagerment Instance {
		get{ return SingleManager.Instance.getObj ("FuBenManagerment") as FuBenManagerment;}
	}

	/// <summary>
	/// 仓库是否已满
	/// </summary>
	public bool isStoreFull () {
		bool isFull = false;
		string strErr = "";
		if ((StorageManagerment.Instance.getAllRole ().Count + 1) > StorageManagerment.Instance.getRoleStorageMaxSpace ()) {
			isFull = true;
			strErr = LanguageConfigManager.Instance.getLanguage ("storeFull_card");
		}
		else if ((StorageManagerment.Instance.getAllEquip ().Count + 1) > StorageManagerment.Instance.getEquipStorageMaxSpace ()) {
			isFull = true;
			strErr = LanguageConfigManager.Instance.getLanguage ("storeFull_equip");
		}
		else if ((StorageManagerment.Instance.getAllTemp ().Count + 100) > StorageManagerment.Instance.getTempStorageMaxSpace ()) {
			isFull = true;
			strErr = LanguageConfigManager.Instance.getLanguage ("storeFull_temp");
		}
		else if ((StorageManagerment.Instance.getAllStarSoul ().Count + 1) > StorageManagerment.Instance.getStarSoulStorageMaxSpace ()) {
			isFull = true;
			strErr = LanguageConfigManager.Instance.getLanguage ("storeFull_starSoul");
		}
		if (isFull) {
            GuideManager.Instance.jumpGuideSid();
			MessageWindow.ShowAlert (strErr + LanguageConfigManager.Instance.getLanguage ("storeFull_msg_01"));
			return true;
		}
		else {
			return false;
		}
	}

	/* 进副本 */
	/// <summary>
	/// 正常进副本流程
	/// </summary>
	public void inToFuben () {
		//先判断是否仓库是否已满
		if (isStoreFull ()) {
			return;
		}
		//再看看是否在扫荡中
		if (SweepManagement.Instance.playerCanSweep) {
			SweepManagement.Instance.initSweepInfo (initTopFuben);
		}
		else {
			initTopFuben ();//回调顺序加载主线，讨伐，修炼数据
		}
	}

	/** 进入挂机界面 */
	public void intoSweepMainWindow() {
		if (SweepManagement.Instance.state == 0) {
			UiManager.Instance.openWindow<SweepMainWindow> ((win) => {
				win.M_init (SweepManagement.Instance.sweepMissionSid, SweepManagement.Instance.sweepMissionLevel);
			});
		}
		else if (SweepManagement.Instance.state == 1) { 
			UiManager.Instance.openWindow<SweepAwardWindow> ((win)=>{
				win.setDondShow(true);
			});
		}
	}
	
	private void initTopFuben () {
		//如果当前有挂机的副本 则进入
		if (SweepManagement.Instance.hasSweepMission) {
			intoSweepMainWindow();
		} else {
			initUserStar ();
			initStarMultiple ();
			FuBenInfoFPort port = FPortManager.Instance.getFPort ("FuBenInfoFPort") as FuBenInfoFPort;
			port.info (() => {
				port.info (getFubenCurrentInfo, ChapterType.PRACTICE);
			}, ChapterType.WAR);
		}
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
	/// <summary>
	/// 获得保存的关卡信息,判断玩家是否在副本中
	/// </summary>
	private void getFubenCurrentInfo () {
		FuBenGetCurrentFPort port = FPortManager.Instance.getFPort ("FuBenGetCurrentFPort") as FuBenGetCurrentFPort;
		port.getInfo (getContinueMission);
	}
	/// <summary>
	/// 处理存档的关卡信息
	/// </summary>
	private void getContinueMission (bool b) {  
		if (b) {
			//继续游戏
			MissionInfoManager.Instance.mission.updateBoss ();	
			initFubenByType (MissionInfoManager.Instance.mission.getChapterType ()); 
		}
		else { 
			intoStoryFuBen (); 
		} 
	}
	/// <summary>
	/// 获得指定副本数据
	/// </summary>
	private void initFubenByType (int type) {
		FuBenInfoFPort port = FPortManager.Instance.getFPort ("FuBenInfoFPort") as FuBenInfoFPort;
		port.info (continueMission, type);
	}
	/// <summary>
	/// 继续关卡
	/// </summary>
	private void continueMission () {
		FuBenIntoFPort port = FPortManager.Instance.getFPort ("FuBenIntoFPort") as FuBenIntoFPort;
		port.toContinue (continueIntoMission); 
	}
	/// <summary>
	/// 进入剧情副本地图
	/// </summary>
	private void intoStoryFuBen () {
		UiManager.Instance.openWindow<ChapterSelectWindow> ((win)=>{
			win.showZhuXianContent();
		});
	}
	/// <summary>
	/// 继续关卡
	/// </summary>
	private void continueIntoMission () {
		UiManager.Instance.openWindow<EmptyWindow> ((win) => {
			ScreenManager.Instance.loadScreen (4, OnLoadingShow, () => {
				MissionManager.instance.AutoRunIndex = -2;
				UiManager.Instance.switchWindow<MissionMainWindow> ();});
		});
	}
	private void OnLoadingShow () { 

	}

	/// <summary>
	/// 取得所有领过的通关奖品
	/// </summary>
	public void getChapterAwardSeverSampleBySid (List<ChapterAwardServerSample> list)
	{
		awardsList = list;
	}

	/// <summary>
	/// 增加领过的通关奖品
	/// </summary>
	public void addChapterAwardSeverSample (ChapterAwardServerSample newItem)
	{
		if (awardsList == null || awardsList.Count == 0) {
			awardsList = new List<ChapterAwardServerSample> ();
			awardsList.Add (newItem);
			return;
		}
		foreach (ChapterAwardServerSample item in awardsList) {
			if (item.chapterSid == newItem.chapterSid) {
				int[] sss = new int[item.awardSids.Length + 1];
				item.awardSids.CopyTo (sss, 0);
				newItem.awardSids.CopyTo (sss, item.awardSids.Length);
				item.awardSids = sss;
				return;
			}
		}
		awardsList.Add (newItem);
	}

	/// <summary>
	/// 取得指定关卡领过的通关奖品
	/// </summary>
	public int[] getAwardSidsByChapterSid (int _chapterSid)
	{
		if (awardsList == null || awardsList.Count == 0) {
			return null;
		}
		foreach (ChapterAwardServerSample item in awardsList) {
			if (item.chapterSid == _chapterSid) {
				return item.awardSids;
			}
		}
		return null;
	}

	private ArenaActivityInfo[] tempArenaActivities;

	public ArenaActivityInfo[] getArenaActivityArray ()
	{
		if (tempArenaActivities == null) {
			ArenaActivityInfo a = new ArenaActivityInfo (EnumArena.arena, 20);
			ArenaActivityInfo b = new ArenaActivityInfo (EnumArena.ladders, LaddersConfigManager.Instance.config_Const.open_minLevel);
			ArenaActivityInfo c = new ArenaActivityInfo (EnumArena.mineral, 30);
			tempArenaActivities = new ArenaActivityInfo[]{b,a,c};
		}
		return tempArenaActivities;
	}

	//获取天梯赛信息
	public ArenaActivityInfo getLadderActivityArray ()
	{
		ArenaActivityInfo[] areslists = getArenaActivityArray ();
		ArenaActivityInfo info = null;
		for (int i = 0; i < areslists.Length; i++) {
			if (areslists[i].type == EnumArena.ladders)
				return areslists[i];
		}

		return null;
	}

	/// <summary>
	/// 检查关卡有几个奖励可以领
	/// </summary>
	public int checkAwardCanGetNum (int _chapterSid)
	{
		ChapterAwardSample[] award = ChapterSampleManager.Instance.getChapterSampleBySid (_chapterSid).prizes;
		if (award == null || award.Length == 0) {
			return 0;
		}
		int num = 0;

		foreach (ChapterAwardSample sample in award) {
			if (getMyMissionStarNum (_chapterSid) >= sample.needStarNum) {
				num++;
			}
		}
		if (awardsList == null || awardsList.Count == 0) {
			return num;
		}
		foreach (ChapterAwardServerSample item in awardsList) {
			if (item.chapterSid == _chapterSid) {
				return num - item.awardSids.Length;
			}
		}
		return num;
	}

	/// <summary>
	/// 取得指定副本行动力需求
	/// </summary>
	public int getPveCostMissionSid (int _missionSid)
	{
		return MissionSampleManager.Instance.getMissionSampleBySid (_missionSid).pveCost;
	}

	/// <summary>
	/// 取得指定副本的通关数目1-3,0为没打
	/// </summary>
	public int getMyStarNumByMissionSid (int _missionSid)
	{
		for (int i = 0; i < storyInfos.Length; i++) {
			if (storyInfos [i] [0] == _missionSid) {
				return storyInfos [i] [1];
			}
		}
		return 0;
	}

	/// <summary>
	/// 取得指定副本噩梦难度的可打次数
	/// </summary>
	public int getTimesByMissionSid (int _missionSid)
	{
		int MAXTIMES = MissionSampleManager.Instance.getMissionSampleBySid (_missionSid).num [2];
		for (int i = 0; i < storyInfos.Length; i++) {
			if (storyInfos [i] [0] == _missionSid) {
				return (MAXTIMES - storyInfos [i] [2]) < 0 ? 0 : (MAXTIMES - storyInfos [i] [2]);
			}
		}
		return MAXTIMES;
	}
	/// <summary>
	/// 重置指定副本噩梦难度的可打次数
	/// </summary>
	public void resetTimesByMissionSid (int _missionSid)
	{
		int MAXTIMES = MissionSampleManager.Instance.getMissionSampleBySid (_missionSid).num [2];
		for (int i = 0; i < storyInfos.Length; i++) {
			if (storyInfos [i] [0] == _missionSid) {
				storyInfos [i] [2] = 0;
			}
		}
	}
	/// <summary>
	/// 取得指定副本的可通关数目
	/// </summary>
	public int getStarNumByMissionSid (int _missionSid)
	{
		return MissionSampleManager.Instance.getMissionSampleBySid (_missionSid).maxStar;
	}

	/// <summary>
	/// 取得指定章节的星星总数
	/// </summary>
	public int getAllMissionStarNum (int _chapterSid)
	{
		int num = 0;
		int[] missionList = getAllMissions (_chapterSid);
		for (int i = 0; i < missionList.Length; i++) {
			num += MissionSampleManager.Instance.getMissionSampleBySid (missionList [i]).maxStar;
		}
		return num;
	}

	/// <summary>
	/// 取得指定章节已获得星星总数
	/// </summary>
	public int getMyMissionStarNum (int _chapterSid)
	{
		int num = 0;
		int[] missionList = getAllShowMissions (_chapterSid);
		for (int i = 0; i < storyInfos.Length; i++) {
			for (int j = 0; j < missionList.Length; j++) {
				if (storyInfos [i] [0] == missionList [j]) {
					num += storyInfos [i] [1];
				}
			}
		}
		return num;
	}
	
	public void cleanUIProcessData ()
	{ 
		selectedChapterSid = 0;
		selectedMissionSid = 0;
		selectedMapSid = 0;
	}
	
	public FuBenManagerment ()
	{ 
		initActivityChapters ();
	}
	
	//最后开放的章节ID
	public int  getOldLastStoryChapterID ()
	{
		return oldLastStoryChapterID;
	}
	
	public void cleanAll ()
	{
		practiceInfos = null;
		warInfos = null;
		storyInfos = null;
        towerInfos = null;
	}
	
	public Chapter getPracticeChapter ()
	{
		return practiceChapter;
	}
	
	public Chapter getWarChapter ()
	{
		return warChapter;
	}
    public Chapter getTowerChapter() {
        return towerChapter;
    }
	
	/// <summary>
	/// 获得对应的活动副本
	/// </summary>
	public ActivityChapter getActivityChapterBySid (int sid)
	{
		foreach (ActivityChapter each in chapters) {
			if (each.sid == sid) {
				return each;
			}
		}
		return null;
	}
    /// <summary>
    /// 这个章节是否通过
    /// </summary>
    /// <param name="sid"></param>
    /// <returns></returns>
    public bool isPassThisChapter(int sid) {
        if (towerInfos == null || towerInfos.Length <= 0) return false;
        for(int i=0;i<towerInfos.Length;i++){
            if(towerInfos[i]==sid)return true;
        }
        return false;
    }
    public bool isHavePassPrize(int sid) {
         if(towerInfos==null||towerInfos.Length<=0)return false;
        for(int i=0;i<towerInfos.Length;i++){
            if(towerInfos[i]==sid)TowerAwardSampleManager.Instance.getTowerAwardSampleBySid(sid).havePrize();
        }
        return false;
    }
    /// <summary>
    /// 返回最后一个通关的章节的下一个
    /// </summary>
    /// <returns></returns>
    public int getPassChapter() {//配置表里的爬塔章节必须按顺序排列
        int[] missions = ChapterSampleManager.Instance.getChapterSampleBySid(ClmbTowerConfigManager.Instance.getClmbTowerMap(1).chapterSids).missions;
        if (towerInfos == null || towerInfos.Length <= 0)return missions[0];
        return missions[towerInfos.Length>=missions.Length?missions.Length-1:towerInfos.Length];
    }
    public bool istheLashMission() {
        int[] missions = ChapterSampleManager.Instance.getChapterSampleBySid(ClmbTowerConfigManager.Instance.getClmbTowerMap(1).chapterSids).missions;
        if (towerInfos == null || towerInfos.Length <= 0) return false;
        return towerInfos.Length == missions.Length;
    }
    public bool canBeAttack() {
        if (towerInfos == null || towerInfos.Length <= 0) return true;
        int[] missions = ChapterSampleManager.Instance.getChapterSampleBySid(ClmbTowerConfigManager.Instance.getClmbTowerMap(1).chapterSids).missions;
        return towerInfos.Length < missions.Length+1;
    }
    public bool isCanShow(int sid) {
        string[] st = CommandConfigManager.Instance.getNvShenShopSid();
        int flagSid = 0;
        for (int i = 0; i < st.Length;i++ ) {
            string[] kk=st[i].Split('#');
            int tempMissionSid = StringKit.toInt(kk[0]);
            int tempGoodSid = StringKit.toInt(kk[1]);
            if (sid == tempGoodSid) {
                if (towerInfos == null || towerInfos.Length <= 0) return false;
                for (int j = 0; j < towerInfos.Length;j++ ) {
                    if (tempMissionSid == towerInfos[j]) return true;
                }
                return false;
            }
        }
        return true;
    }
    public bool isCanOpenBox(int index) {
        if (towerInfos == null || towerInfos.Length <= 0) return false;
        for (int i = 0; i < towerInfos.Length;i++ ) {
            if (towerInfos[i] == index) return true;
        }
        return false;
    }
	/// <summary>
	/// 判断一个关卡的前置关卡是否完成  限时活动不受限制(包括爬塔)
	/// </summary>
	public bool isCompleteLastMission (int sid)
	{
		MissionSample sample = MissionSampleManager.Instance.getMissionSampleBySid (sid);
		ChapterSample chapter = ChapterSampleManager.Instance.getChapterSampleBySid (sample.chapterSid);
		if (chapter.type == ChapterType.ACTIVITY_CARD_EXP || chapter.type == ChapterType.ACTIVITY_EQUIP_EXP || chapter.type == ChapterType.ACTIVITY_MONEY || chapter.type == ChapterType.ACTIVITY_SKILL_EXP)
			return true;
		int [] sids = chapter.missions;
		int index = 0;
		int max = sids.Length;
		for (int i = 0; i < max; i++) {
			if (sid == sids [i]) {
				index = i;
				break;
			}
		}
		if (index == 0)
			return true;
		if (chapter.type == ChapterType.STORY) {
			return isIntInArray (storyInfos, sids [index - 1]);
		} else if (chapter.type == ChapterType.WAR) {
			return isIntInArray (warInfos, sids [index - 1]);
		} else if (chapter.type == ChapterType.PRACTICE) {
			return isIntInArray (practiceInfos, sids [index - 1]);
        } else if (chapter.type == ChapterType.TOWER_FUBEN) {
            return isIntInArray(practiceInfos, sids[index - 1]);
        }
		return false;
	}
	
	public ActivityChapter getActivityChapterByType (int type)
	{
		foreach (ActivityChapter each in chapters) {
			if (each.getChapterType () == type) {
				return each;
			}
		}
		return null;
	}
	
	
	//根据时间排序
	public ActivityChapter[] getSortByTimeChapter ()
	{
		ActivityChapter[] fixlist = getActivityChapters ();
		List<ActivityChapter> dyList = new List<ActivityChapter> ();
		for (int j=0; j<fixlist.Length; j++) {
			dyList.Add (fixlist [j]);
		}
        
		
		
		//开启的活动放前面
		int index = dyList.Count;
		
		for (int j=0; j<dyList.Count; j++) {
			index -= 1;
			if (dyList [index].isOpen () == true) {
				ActivityChapter tmp = dyList [index];
				dyList.RemoveAt (index);
				dyList.Insert (0, tmp);
				
				index += 1;
				
				
			}
		}
		
		return dyList.ToArray ();
	}
	
	//当前是否有活动开启
	public bool HasActivityOpen ()
	{
		for (int i = 0; i < chapters.Length; i++) {
			if (chapters [i].isOpen ()) {
				return true;
			}
		}
		return false;
	}
	/// <summary>
	/// 获取所有活动副本
	/// </summary>
	public ActivityChapter[] getActivityChapters ()
	{
		return chapters;
	}
	
	private void initActivityChapters ()
	{
		int[] sids = FuBenActivityConfigManager.Instance.getSids ();
		int max = sids.Length;
		chapters = new  ActivityChapter[max];
		for (int i = 0; i < max; i++) {
			chapters [i] = new ActivityChapter (sids [i]);
		}
	}
	
	/// <summary>
	/// 更新活动副本信息
	/// </summary>
	public void updateActivityChapters (int type, int num, int maxnum, int timeId,int buyNum)
	{
 
		int max2 = chapters.Length;
 
		int _type = type;
		for (int j = 0; j < max2; j++) { 

			if (chapters [j].getChapterType () == type) {
				chapters [j].update (num);
				chapters [j].updateMaxNum (maxnum);
				chapters [j].initTime (timeId);
                chapters[j].setReBuyNum(buyNum);
			}

		}
 
	}
	
	/** 进入副本 扣除次数 */
	public void intoMission (int sid, int missonLevel)
	{ 
		MissionSample sample = MissionSampleManager.Instance.getMissionSampleBySid (sid);
		int cSid = sample.chapterSid;
		int type = ChapterSampleManager.Instance.getChapterSampleBySid (cSid).type;
		if (type == ChapterType.WAR) {
			warChapter.costNum ();
		} else if (type == ChapterType.PRACTICE) {
			//新手指引不扣消耗,需要注意的是,砍了新手步骤或者调整了步骤顺序需要重新指向新的步骤
			if (GuideManager.Instance.isEqualStep (118001000) && PlayerPrefs.GetInt (UserManager.Instance.self.uid + "_PRACTICE_GUIDE", 0) == 0) {
				PlayerPrefs.SetInt (UserManager.Instance.self.uid + "_PRACTICE_GUIDE", 1);
				PlayerPrefs.Save ();
			} else {
				practiceChapter.costNum ();
				PlayerPrefs.SetInt (UserManager.Instance.self.uid + "_PRACTICE_GUIDE", 1);
				PlayerPrefs.Save ();
			}
		} else if (type == ChapterType.ACTIVITY_CARD_EXP || type == ChapterType.ACTIVITY_EQUIP_EXP || type == ChapterType.ACTIVITY_SKILL_EXP || type == ChapterType.ACTIVITY_MONEY) {
			costFubenActivityNum (type);
		} else if (type == ChapterType.STORY) {
			if (missonLevel == 3) {
				for (int i =0; i<storyInfos.Length; i++) {
					if (storyInfos [i] [0] == sid && missonLevel == 3) {
						storyInfos [i] [2]++;
						return;
					}
				}
			}
		}
	}

	/** 挂机扣除相应的次数 */
	public void sweepMission (int sid, int missonLevel, int times)
	{
		MissionSample sample = MissionSampleManager.Instance.getMissionSampleBySid (sid);
		int cSid = sample.chapterSid;
		int type = ChapterSampleManager.Instance.getChapterSampleBySid (cSid).type;

		if (type == ChapterType.STORY) {
			if (missonLevel == 3) {
				for (int i =0; i<storyInfos.Length; i++) {
					if (storyInfos [i] [0] == sid && missonLevel == 3) {
						storyInfos [i] [2] += times;
						return;
					}
				}
			}
		}
	}
	
	private void costFubenActivityNum (int type)
	{
		int max = chapters.Length;
		for (int i = 0; i < max; i++) {
			if (type == chapters [i].getChapterType ()) {
				chapters [i].costNum ();
			}
		}
	}
	
	//进入限时活动副本
	private void intoActivityMission (int type)
	{
		int max = chapters.Length;
		for (int i = 0; i < max; i++) {
			if (chapters [i].getChapterType () == type) {
				chapters [i].costNum ();
			}
		}
	} 
	
	//关卡完成 条件完成关卡sid
	public void completeMission (int sid, int type, int star)
	{
		if (type == ChapterType.STORY) {
			if (!isIntInArray (storyInfos, sid)) { 
				//记录副本章节
				FuBenManagerment.Instance.oldLastStoryChapterID = MissionSampleManager.Instance.getMissionSampleBySid (sid).chapterSid;
				List<int[]> list = new List<int[]> ();
				if (storyInfos != null) {
					foreach (int[] each in storyInfos) {
						list.Add (each);
					}
				}

				list.Add (new int[]{sid,1,0});
				storyInfos = list.ToArray ();
				//如果到这里说明这个副本是第一次通关哈
				MissionInfoManager.Instance.mission.isFirstComplete = true;
			} else
				updateStory (sid, star);
		} else if (type == ChapterType.WAR) {
			if (!isIntInArray (warInfos, sid)) {
				if (warInfos == null)
					warInfos = new int[] {sid};
				else {
					int[] newWar = new int[warInfos.Length + 1];
					Array.Copy (warInfos, newWar, warInfos.Length);
					newWar [newWar.Length - 1] = sid;
					warInfos = newWar;
				}
			}
		} else if (type == ChapterType.PRACTICE) {
			if (!isIntInArray (practiceInfos, sid)) {
				List<int> list = new List<int> ();
				if (practiceInfos != null) {
					foreach (int each in practiceInfos) {

						list.Add (each);
					}
				}

				list.Add (sid);
				practiceInfos = list.ToArray ();
			}
		}
	}
	
	public void updateStory (int[][] storySids)
	{
		storyInfos = storySids;
	}

	//确保关卡已存在打过
	public void updateStory (int sid, int star)
	{
		for (int i =0; i<storyInfos.Length; i++) {
			if (storyInfos [i] [0] == sid && star > storyInfos [i] [1]) {
				storyInfos [i] [1] = star;
				return;
			}
		}
	}
	
	public void updateWar (int[] warSids, int num, int buyed)
	{
		warInfos = warSids;
		warChapter = new Chapter (FuBenWarConfigManager.Instance.getSid ());
		warChapter.update (num, buyed);
	}
	
	public void updatePractice (int[] praSids, int num)
	{
		practiceInfos = praSids;
		practiceChapter = new Chapter (FuBenPracticeConfigManager.Instance.getSid ());
		practiceChapter.update (num);
	}
    //更新爬塔数据
    public void updateTower(int[] towSids, int num,int[] reFirstSid,int renum,int loNum,int loBuyNum) {//通关的SID，今天挑战的次数,重置次数
        towerInfos = towSids;
        refistTowerMissionSid = reFirstSid;
        towerChapter = new Chapter(ClmbTowerConfigManager.Instance.getClmbTowerMap(1).chapterSids);//爬塔的章节
        towerChapter.updateTower(num, renum, loNum,loBuyNum);
    }
    /// <summary>
    /// 是不是第一次开启宝箱 免费得
    /// </summary>
    /// <returns></returns>
    public bool isFistIntoAward(int sid){
        if (refistTowerMissionSid != null && refistTowerMissionSid.Length > 0) {
            for (int i = 0; i < refistTowerMissionSid.Length;i++ ) {
                if (refistTowerMissionSid[i] == sid) return false;
            }
        }
        return true;
    }
	public void updatePracticeNum(int num)
	{
		if(practiceChapter!=null)
		{
			practiceChapter.update (num);
		}
	}
	

 
	//是否是新副本
	public bool isNewMission (int chapterType, int sid)
	{
		if (chapterType == ChapterType.STORY) {
			return !isIntInArray (storyInfos, sid);
		} else if (chapterType == ChapterType.WAR) {
			return !isIntInArray (warInfos, sid);
		}
		return false; //默认可跳过，非新副本
	}
	
	//获得最后一个通关的剧情副本sid
	public int getLastStoryMissionSid ()
	{ 
		if (storyInfos == null || storyInfos.Length < 1)
			return -1; 
		return getMaxInArray (storyInfos);
	}
	
	//获得一个数组中最大的数字
	private int getMaxInArray (int[][] arr)
	{
		int max = arr.Length;
		int temp = arr [0] [0];
		if (arr.Length < 2)
			return temp;
		for (int i = 1; i < max; i++) {
			if (temp < arr [i] [0]) {
				temp = arr [i] [0];
			}
		}
		return temp;
	}
	
	//判断一个int 是否在一个数组当中
	private bool isIntInArray (int[] arr, int a)
	{
		if (arr == null || arr.Length < 1)
			return false;
		int max = arr.Length;
		for (int i = 0; i < max; i++) {
			if (arr [i] == a)
				return true;
		}
		return false;
	}
	
	private bool isIntInArray (int[][] arr, int a)
	{
		if (arr == null || arr.Length < 1)
			return false;
		int max = arr.Length;
		for (int i = 0; i < max; i++) {
			if (arr [i] [0] == a)
				return true;
		}
		return false;
	} 
	
	//获得指定剧情章节sid 索引从1开始
	public static int getStoryChapterByIndex (int mapId, int index)
	{
		FuBenStoryMap map = FuBenStoryConfigManager.Instance.getFuBenStoryMap (mapId);
		if (map == null) {
			return -1;
		}
		return map.chapterSids [index - 1]; 
	}
	
	public static int getStoryChapterIndexBySid (int mapId, int sid)
	{
		FuBenStoryMap map = FuBenStoryConfigManager.Instance.getFuBenStoryMap (mapId);
		if (map == null) {
			return -1;
		}
		for (int i=0; i<map.chapterSids.Length; i++) {
			if (map.chapterSids [i] == sid)
				return i;
			
			
		}
		
		return -1;
	}
	
	//获得指定剧情地图对应索引的章节sid 索引从1开始
	public static int[] getAllShowStoryChapter (int mapId)
	{
		FuBenStoryMap map = FuBenStoryConfigManager.Instance.getFuBenStoryMap (mapId);
		if (map == null) {
			return null;
		}
		
		//默认总显示第一张
		List<int> list = new List<int> ();
		list.Add (map.chapterSids [0]);
		
		int tmplastID = map.chapterSids [0];
		for (int i = 0; i <  map.chapterSids.Length; i++) {
			if (FuBenManagerment.Instance.isStoryChapterShow (map.chapterSids [i]) == true) {
				list.Add (map.chapterSids [i]);
				tmplastID = map.chapterSids [i];
			}
		}
		
		
		//更新最后章
		if (tmplastID > FuBenManagerment.Instance.oldLastStoryChapterID) {
			
			FuBenManagerment.Instance.oldLastStoryChapterID = tmplastID;
			
		}
		
		
		
		return list.ToArray ();
	}

    //获得指定爬塔地图对应索引的章节sid 索引从1开始
    public  int[] getAllShowTowerChapter(int mapId) {
        ClmbTowerMap map = ClmbTowerConfigManager.Instance.getClmbTowerMap(mapId);//得到指定大地图的爬塔章节sid[,,,,,]
        if (map == null) {
            return null;
        }
        //应该显示所有的章节
        int tmplastID = map.chapterSids;
        int[] missions = ChapterSampleManager.Instance.getChapterSampleBySid(tmplastID).missions;
        FuBenManagerment.Instance.oldLastTowerChapterID = missions[0];//不考虑后面的 直接弄成第一章
        return missions;
    }
   
	
	//通过mission id反推出是哪个章节
	public int[] getStoryChapterSidByMissionSid (int missionSid)
	{
		
		//返回 index0:地图id,index1:章节id
		
		FuBenStoryMap[] maps = FuBenStoryConfigManager.Instance.getAllStorys ();
		
		//遍历底图
		foreach (FuBenStoryMap each in maps) {
			
			int[] chapterIDs = getAllShowStoryChapter (each.mapId);
			//遍历地图中的所有章节
			foreach (int  eachID in chapterIDs) {
				
				ChapterSample sample = ChapterSampleManager.Instance.getChapterSampleBySid (eachID);
				
				//遍历章节中的所有mission
				foreach (int m_id  in 	sample.missions) {
					
					if (missionSid == m_id) {
						int[] data = new int[2]{each.mapId, eachID};
						return data;
					}
				}
			}
		}
		return null;
	}
	
	
	//判断剧情章节是否显示
	public bool isStoryChapterShow (int sid)
	{ 
		if (storyInfos == null)
			return false;
		ChapterSample chapter = ChapterSampleManager.Instance.getChapterSampleBySid (sid);
		if (chapter.type != ChapterType.STORY)
			return false;
		int firstSid = chapter.missions [0];
		int max = storyInfos.Length;
		
		//+1保证有下一个关卡可打
		for (int i = 0; i < max; i++) {
			//剧情章节关卡sid为自增
			if (storyInfos [i] [0] + 1 == firstSid)
				return true;
		}
		
		return false;
	}
	
	//根据章节SID取得所有关卡
	public int[] getAllShowMissions (int sid)
	{ 
		ChapterSample chapter = ChapterSampleManager.Instance.getChapterSampleBySid (sid);
		
		//限时活动副本(4种) 讨伐副本 显示所有关卡
		if (chapter.type == ChapterType.ACTIVITY_CARD_EXP || chapter.type == ChapterType.WAR || chapter.type == ChapterType.PRACTICE || chapter.type == ChapterType.ACTIVITY_EQUIP_EXP || chapter.type == ChapterType.ACTIVITY_MONEY || chapter.type == ChapterType.ACTIVITY_SKILL_EXP)
			return chapter.missions;
		List<int> list = new List<int> (); 
		
		for (int i=0; i<chapter.missions.Length; i++) {
			list.Add (chapter.missions [i]); 
			if (chapter.missions [i] > getLastStoryMissionSid ())
				break; 
		}
		return list.ToArray ();
	}

	//根据章节SID取得所有关卡
	public int[] getAllMissions (int sid)
	{
		ChapterSample chapter = ChapterSampleManager.Instance.getChapterSampleBySid (sid);
		return chapter.missions;
	}
	
	public int[][] getStoryInfos ()
	{
		return storyInfos;
	}
	
	public int[] getWarInfos ()
	{
		return warInfos;
	}
    public int[] getTowerInfos() {
        return towerInfos;
    }
	
	public int[] getPracticeInfos ()
	{
		return practiceInfos;
	}

	public long[] getStarMultipleTimes ()
	{
		if (starTime == null)
			return null;
		return new long[] {
			starTime.getDetailStartTime (),
			starTime.getDetailEndTime ()
		};
	}

	public void setStarMultiple (int timeId, int hit)
	{
		TimeInfoSample tsample = TimeConfigManager.Instance.getTimeInfoSampleBySid (timeId);
		if (starTime == null) {
			starTime = ActiveTime.getActiveTimeByType (tsample);
			//starTime.initTime (ServerTimeKit.getSecondTime ());
		}
		this.starHit = hit;

	}

	public int getStarHit ()
	{
		return starHit;
	}
	// true有效时间内
	public bool checkStarMultipleTime ()
	{
		if (starTime == null)
			return false;
		int now = ServerTimeKit.getSecondTime ();
		long[] times = getStarMultipleTimes ();
		return times [0] <= now && now <= times [1];
	}
} 
