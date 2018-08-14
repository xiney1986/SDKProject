using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 公会管理器
 * @author 汤琦
 * */
public class GuildManagerment {
	public CallBack<GuildMsg> updateMsg;//更新公会动向信息
	private Guild guild;//公会
	/** 大厅 */
	public const string HALL = "1";
	/** 商店 */
	public const string SHOP = "3";
	/** 祭坛 */
	public const string ALTAR = "4";
	/** 学院 */
	public const string COLLEGE = "2";
	/** 领地 */
	public const string AREA = "5";
	/** 加入公会等级限制 */
	public const int JOINGUILDLEVEL = 18;
	/** 允许一次性申请加入公会的次数 */
	public const int JOINGUILDCOUNT = 10;
	/** 允许创建公会等级限制 */
	public const int CREATEGUILDLEVEL = 18;
	/** 退出公会人数限制 */
	private const int EXITGUILDMEMBER = 1;
	/** 创建公会人民币花费标准 */
	public const int CREATEGUILDRMB = 20000;
	/** 创建公会游戏币花费标准 */
	public const int CREATEGUILDMONEY = 5000000;
	/** 公会等级上限 */
	private const int GUILDLEVELMAX = 15;
	private const string COSTMONEY = "d_money";
	private const string COSTRMB = "d_rmb";
	private const int MSGMAXCOUNT = 10;//公会动向最多条数
	//公会捐献类型
	public const string DONATIONMONEY = "d1";
	public const string DONATIONGONE = "d2";
	public const string DONATIONGTWO = "d3";
	public const string DONATIONGTHREE = "d4";
	//公会技能类型
	public const string SKILLTYPE_ROLEEXP = "roleexp";
	public const string SKILLTYPE_BEASTEXP = "beastexp";
	public const string SKILLTYPE_CARDEXP = "cardexp";
	//公会幸运女神骰子数量
	public const int EBLOWS_MAXNUM = 5;
	/** 幸运骰子开放等级 */
	public const int LUCK_GODDESS = 1;
	private int applyCount = 0;//申请公会次数
	private List<GuildRankInfo> guildList;//公会集
	private List<GuildRankInfo> guildInviteList;//公会邀请集
	private List<GuildMember> guildMembers;//公会成员集
	private List<GuildApprovalInfo> guildApprovalList;//公会审批列表
	private List<string> ids;//被申请过的公会id集
	private List<GuildBuild> builds = new List<GuildBuild> (){new GuildBuild("1",0),new GuildBuild("3",0),new GuildBuild("4",0),new GuildBuild("2",0),new GuildBuild("5",0)};//公会建筑
	private List<GuildSkill> guildSkills;//公会技能集
	private List<GuildRastInfo> guildRastInfos;//公会成员伤害集 
	private GuildAltar guildAltar;//公会祭坛
	private GuildLuckyNvShenInfo guildShakeElvowsInfo; //公会幸运女神信息
	private GuildLuckyNvShenShakeResult guildShakeResult;//投掷骰子结果数组
	public bool isGuildBattle = false;//是否工会Boss战
	private int[] awardContributions;//公会幸运女神排名奖励贡献
	public GuildFightInfo guildFightInfo;//公会战信息
    public int selfScore; //公会战自己公会积分
	public int autoJoin;//临时保存自动勾选
	private int wunNum=0;//本周公会战连胜次数
	private int gongxuanIndex=0;//贡献名次
	/**是否是复活回来*/
	public bool isReviveBack = false;

	public static GuildManagerment Instance {
		get{ return SingleManager.Instance.getObj ("GuildManagerment") as GuildManagerment;}
		
	}

	public void clearGuildRastInfo () {
		if (guildRastInfos != null) {
			guildRastInfos.Clear ();
		}
	}

	/// <summary>
	/// 更新公会信息，有回调就回调
	/// </summary>
	public void updateGuildInfo (CallBack _callback) {
		GuildGetInfoFPort fport = FPortManager.Instance.getFPort ("GuildGetInfoFPort") as GuildGetInfoFPort;
		fport.access (() => {
			if (_callback != null) {
				_callback ();
				_callback = null;
			}
		});
	}

	public void createGuildRastInfo (GuildRastInfo rask) {
		if (guildRastInfos == null)
			guildRastInfos = new List<GuildRastInfo> ();
		guildRastInfos.Add (rask);
	}

	public GuildLuckyNvShenInfo getGuildLuckyNvShenInfo () {
		return guildShakeElvowsInfo;
	}
	public void createGuildLuckyNvShenInfo (int selfIntegral, int guildIntegral, int topIntegral, int shakeCount, int reShakeCount) {
		guildShakeElvowsInfo = new GuildLuckyNvShenInfo (selfIntegral, guildIntegral, topIntegral, shakeCount, reShakeCount);
	}

	public GuildLuckyNvShenShakeResult  getGuildShakeResult () {
		return guildShakeResult;
	}
	public void  clearGuildShakeResult () {
		guildShakeResult = null;
	}

	public void createGuildShakeResult (string[] shakeResult) {
		guildShakeResult = new GuildLuckyNvShenShakeResult (shakeResult);
	}

	public List<GuildRastInfo> getGuildRastInfo () {
		return guildRastInfos;
	}


	#region 公会祭坛
	public void createGuildAltar (int bossSid, long hurtSum, int count, List<GuildAltarRank> list) {
		guildAltar = new GuildAltar (bossSid, hurtSum, count, list);
	}

	public GuildAltar getGuildAltar () {
		return guildAltar;
	}

	public void updateBossSid (int sid) {
		guildAltar.bossSid = sid;
	}

	public string getGuildBossBless () {
		return GuildBossSampleManager.Instance.getGuildBossSampleBySid (guildAltar.bossSid).blessing;
	}

	public string getGuildWeakness () {
		return GuildBossSampleManager.Instance.getGuildBossSampleBySid (guildAltar.bossSid).weakness;
	}

	public int getGuildBossIcon () {
		return GuildBossSampleManager.Instance.getGuildBossSampleBySid (guildAltar.bossSid).icon;
	}

	public string getGuildBossName () {
		return GuildBossSampleManager.Instance.getGuildBossSampleBySid (guildAltar.bossSid).bossName;
	}

	public List<GuildAltarRank> filterRank () {
		for (int i = 0; i < guildAltar.list.Count - 1; i++) {
			for (int j = 0; j < guildAltar.list.Count - 1 - i; j++) {
				if (guildAltar.list [j].hurtValue < guildAltar.list [j + 1].hurtValue) {
					GuildAltarRank temp = guildAltar.list [j];
					guildAltar.list [j] = guildAltar.list [j + 1];
					guildAltar.list [j + 1] = temp;
				}
			}
		}
		List<GuildAltarRank> result = new List<GuildAltarRank> ();
		for (int i = 0; i < guildAltar.list.Count; i++) {
			result.Add (guildAltar.list [i]);
			if (i == 4) {
				break;
			}
		}
		return result;
	}

	public long getMyHurt () {
		if (guildAltar == null)
			return 0;
		for (int i = 0; i < guildAltar.list.Count; i++) {
			if (guildAltar.list [i].sid == UserManager.Instance.self.uid) {
				return guildAltar.list [i].hurtValue;
			}
		}
		return 0;
	}
	#endregion
	#region 公会商店
	//判断物品是否开放
	public bool isOpenGood (Goods good) {
		GuildBuildSample buildSample = GuildBuildSampleManager.Instance.getGuildBuildSampleBySid (StringKit.toInt (SHOP));
		List<GuildGood> list = buildSample.goods;
		for (int i = 0; i < list.Count; i++) {
			if (list [i].sid == good.sid && getBuildLevel (SHOP) >= list [i].level) {
				return true;
			}
		}
		return false;
	}
	//获得限制等级
	public int getOpenLevel (Goods good) {
		GuildBuildSample buildSample = GuildBuildSampleManager.Instance.getGuildBuildSampleBySid (StringKit.toInt (SHOP));
		List<GuildGood> list = buildSample.goods;
		for (int i = 0; i < list.Count; i++) {
			if (getBuildLevel (SHOP) < list [i].level && list [i].sid == good.sid) {
				return list [i].level;
			}
		}
		return 0;
	}
	#endregion
	#region 公会学院
	public void createGuildSkill (GuildSkill skill) {
		if (guildSkills == null)
			guildSkills = new List<GuildSkill> ();
		guildSkills.Add (skill);
	}

	public void updateGuildSkill (int skillLevel, string skillSid) {
		if (guildSkills == null) {
			guildSkills = new List<GuildSkill> ();
			GuildSkill skill = new GuildSkill (skillSid, skillLevel);
			guildSkills.Add (skill);
		}
		else {
			for (int i = 0; i < guildSkills.Count; i++) {
				if (guildSkills [i].sid == skillSid) {
					guildSkills [i].level = skillLevel;
					break;
				}
				else if (i == guildSkills.Count - 1) {
					GuildSkill skill = new GuildSkill (skillSid, skillLevel);
					guildSkills.Add (skill);
				}
			}
		}
	}

	public List<GuildSkill> getGuildSkill () {
		return guildSkills;
	}

	public void clearGuildSkillList () {
		guildSkills = null;
	}

	public GuildSkill getGuildSkillBySid (string sid) {
		if (guildSkills == null)
			return null;
		for (int i = 0; i < guildSkills.Count; i++) {
			if (guildSkills [i].sid == sid) {
				return guildSkills [i];
			}
		}
		return null;
	}
	//学习技能和技能升级
	public void learnGuildSkill (string uid) {
		GuildSkillSample sample = GuildSkillSampleManager.Instance.getGuildSkillSampleBySid (StringKit.toInt (uid));
		if (guildSkills == null)
			guildSkills = new List<GuildSkill> ();
		if (guildSkills.Count < 1) {
			guildSkills.Add (new GuildSkill (uid, 1));
			guild.contributioning -= sample.costs [0];
			UiManager.Instance.openDialogWindow<MessageLineWindow> ((win) => {
				win.Initialize (LanguageConfigManager.Instance.getLanguage ("Guild_53", sample.skillName, "1"));
			});
		}
		else {
			for (int i = 0; i < guildSkills.Count; i++) {
				if (guildSkills [i].sid == uid) {
					guild.contributioning -= sample.costs [guildSkills [i].level - 1];
					UiManager.Instance.openDialogWindow<MessageLineWindow> ((win) => {
						win.Initialize (LanguageConfigManager.Instance.getLanguage ("Guild_53", GuildSkillSampleManager.Instance.getGuildSkillSampleBySid (StringKit.toInt (uid)).skillName, guildSkills [i].level.ToString ()));
					});
					return;
				}
			}
			guildSkills.Add (new GuildSkill (uid, 1));
			guild.contributioning -= sample.costs [0];
			UiManager.Instance.openDialogWindow<MessageLineWindow> ((win) => {
				win.Initialize (LanguageConfigManager.Instance.getLanguage ("Guild_53", sample.skillName, "1"));
			});
		}
	}
	//是否可以学习或升级技能
	public bool isUpLearnGuildSkill (string uid) {
		GuildSkillSample sample = GuildSkillSampleManager.Instance.getGuildSkillSampleBySid (StringKit.toInt (uid));
		if (getBuildLevel (GuildManagerment.COLLEGE) < sample.openLevel) {
			UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
				win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0093"), null, LanguageConfigManager.Instance.getLanguage ("Guild_55"), null);
			});
			return false;
		}
		GuildSkill skill = getGuildSkillBySid (uid);
		if (skill != null && skill.level >= getBuildLevel (GuildManagerment.COLLEGE)) {
			UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
				win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0093"), null, LanguageConfigManager.Instance.getLanguage ("Guild_72"), null);
			});
			return false;
		}
		int cost = 0;
		if (skill != null) {
			cost = sample.costs [skill.level];
		}
		else {
			cost = sample.costs [0];
		}
		if (guild.contributioning >= cost) {
			return true;
		}
		else {
			UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
				win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0093"), null, LanguageConfigManager.Instance.getLanguage ("Guild_54"), null);
			});
			return false;
		}
	}
	#endregion

	//设置更新公会动向的方法
	public void setUpdate (CallBack<GuildMsg> updateMsg) {
		this.updateMsg = updateMsg;
	}

	public void createMsg (string content) {
		if (guild == null)
			return;
		GuildMsg msg = new GuildMsg (content);

		if (guild.msgs.Count <= MSGMAXCOUNT)
			guild.msgs.Add (msg);
		else {
			guild.msgs.RemoveAt (0);
			guild.msgs.Add (msg);
		}
		
		if (updateMsg != null) {
			updateMsg (msg);
		}
	}

	public void clearUpdateMsg () {
		updateMsg = null;
	}
	#region 公会建筑
	//初始化公会建筑
	public void updateBuild (string id, int level) {
		for (int i = 0; i < builds.Count; i++) {
			if (builds [i].id == id) {
				builds [i].level = level;
				break;
			}
		}
	}
	//获得公会建筑等级
	public int getBuildLevel (string id) {
		for (int i = 0; i < builds.Count; i++) {
			if (builds [i].id == id) {
				return builds [i].level;
			}
		}
		return 0;
	}
	//获得公会建筑
	public GuildBuild getGuildBuild (string id) {
		for (int i = 0; i < builds.Count; i++) {
			if (builds [i].id == id) {
				return builds [i];
			}
		}
		return null;
	}
	/// <summary>
	/// 建筑是否可以升级
	/// </summary>
	/// <returns><c>true</c>, if up guild build was ised, <c>false</c> otherwise.</returns>
	/// <param name="id">Identifier.</param>
	/// <param name="level">Level.</param>
	public bool isUpGuildBuild (int id, int level) {
		GuildBuildSample sample = GuildBuildSampleManager.Instance.getGuildBuildSampleBySid (id);
		if (level >= sample.levelMax) {
			UiManager.Instance.createMessageLintWindow (LanguageConfigManager.Instance.getLanguage ("Guild_47"));
			return false;
		}

		if (guild.livenessing >= sample.costs [level] && getGuildBuild (HALL).level >= sample.hallLevel [level] && level < sample.levelMax) {
			return true;
		}
		else {
			if (guild.livenessing < sample.costs [level]) {
				UiManager.Instance.createMessageLintWindow (LanguageConfigManager.Instance.getLanguage ("Guild_45"));
			}
			else if (getGuildBuild (HALL).level < sample.hallLevel [level]) {
				UiManager.Instance.createMessageLintWindow (LanguageConfigManager.Instance.getLanguage ("Guild_46"));
			}
			return false;
		}
	}

	public bool isUpGuildBuildState (int id, int level) {
		GuildBuildSample sample = GuildBuildSampleManager.Instance.getGuildBuildSampleBySid (id);
		if (level >= sample.levelMax) {
			return false;
		}
		if (guild.livenessing >= sample.costs [level] && getGuildBuild (HALL).level >= sample.hallLevel [level] && level < sample.levelMax) {
			return true;
		}
		else {
			return false;
		}
	}
	//获得建筑的需求描述
	public string getBuildNeedsDesc (int sid) {
		GuildBuildSample buildSample = GuildBuildSampleManager.Instance.getGuildBuildSampleBySid (sid);
		if (getBuildLevel (sid.ToString ()) < buildSample.levelMax) {
			if (sid == StringKit.toInt (HALL)) {
				return LanguageConfigManager.Instance.getLanguage ("Guild_51", buildSample.costs [getBuildLevel (sid.ToString ())].ToString ());
			}
			else {
				return LanguageConfigManager.Instance.getLanguage ("Guild_50", buildSample.costs [getBuildLevel (sid.ToString ())].ToString (), buildSample.hallLevel [getBuildLevel (sid.ToString ())].ToString ());
			}
		}
		else {
			return LanguageConfigManager.Instance.getLanguage ("Guild_47");
		}
	}

	/// <summary>
	/// 由建筑Sid获得对应等级建筑图标
	/// </summary>
	public string getBuildIcon (int sid) {
		GuildBuildSample buildSample = GuildBuildSampleManager.Instance.getGuildBuildSampleBySid (sid);
		int buildLv = getBuildLevel (sid.ToString ());
		string spriteName = "";
		switch (buildSample.sid) {
		case 1:
			spriteName = "hall_";
			break;
		case 2:
			spriteName = "college_";
			break;
		case 3:
			spriteName = "shop_";
			break;
		case 4:
			spriteName = "altar_";
			break;
		case 5:
			spriteName = "area_";
			break;
		default:
			spriteName = "";
			break;
		}
		
		if (buildLv > 0) {
			return spriteName + buildSample.getIconByLv (buildLv);
		}
		else {
			return spriteName + 1;
		}
	}
	
	#endregion
	#region 创建公会,清除公会信息,获得公会信息
	//是否可以创建公会
	public bool isCreateGuild (string str, string nameLabel) {
		bool costIsOk = costFull (str);
		if ((guild == null && UserManager.Instance.self.getUserLevel () >= CREATEGUILDLEVEL) && costIsOk && nameLabel != "" && nameLabel.Length <= 8) {
			return true;
		}
		else {
			string temp = "";
			bool hasAlert;

			if (guild != null) {
				temp = LanguageConfigManager.Instance.getLanguage ("Guild_1", CREATEGUILDLEVEL.ToString ());
			}
			else if (nameLabel == "" || nameLabel.Replace (" ", "") == "") {
				temp = LanguageConfigManager.Instance.getLanguage ("Guild_8", CREATEGUILDLEVEL.ToString ());
			}
			else if (str == "") {
				temp = LanguageConfigManager.Instance.getLanguage ("Guild_7", CREATEGUILDLEVEL.ToString ());
			}
			else if (str == COSTMONEY) {
				temp = LanguageConfigManager.Instance.getLanguage ("Guild_3", CREATEGUILDLEVEL.ToString ());
			}
			else if (str == COSTRMB) {
				temp = LanguageConfigManager.Instance.getLanguage ("Guild_4", CREATEGUILDLEVEL.ToString ());
			}
			if (str == COSTRMB && !costIsOk)
				MessageWindow.ShowRecharge (LanguageConfigManager.Instance.getLanguage ("Guild_4", CREATEGUILDLEVEL.ToString ()));
			else
				UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
					win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0093"), null, temp, null);
				});
			return false;
		}
	}

	/** 根据入会时间判断是否能够参加公会战 */
	public bool isCanJoinGuildFight(){
		if (guild == null)
			return false;
		System.DateTime joinTime = TimeKit.getDateTime (guild.joinTime);
		System.DateTime serverTime = TimeKit.getDateTime (ServerTimeKit.getSecondTime ());
		if (joinTime.Year!= serverTime.Year || joinTime.DayOfYear!= serverTime.DayOfYear)
			return true;
		else 
			return false;
	}

	private bool costFull (string str) {
		switch (str) {
		case COSTMONEY:
			return UserManager.Instance.self.getMoney () >= CREATEGUILDMONEY;
		case COSTRMB:
			return UserManager.Instance.self.getRMB () >= CREATEGUILDRMB;
		default:
			return false;
		}
	}
	//创建公会
	public void createGuild (string uid, string name, int level, int membership, int membershipMax, int livenessing, int livenessed, string declaration, string notice, string presidentName, int job, int contributioning, int contributioned, List<GuildMsg> msgs, int todayDonateTimes,bool isCanRename,int autoJoin,int joinTime,int firstAward) {
		if (notice == "[]")
			notice = "";
		if (declaration == "[]")
			declaration = "";
		msgs.Reverse ();
		guild = new Guild (uid, name, level, membership, membershipMax, livenessing, livenessed, declaration, notice, presidentName, job, contributioning, contributioned, msgs, todayDonateTimes,isCanRename,autoJoin,joinTime,firstAward); 
	}
	//清除公会信息
	public void clearGuildList () {
		guildList = null;
	}

	//更新自己的职位
	public void updateJob (int jobType) {
		guild.job = jobType;
	}
	//获得公会信息
	public Guild getGuild () {
		return guild;
	}

	public void setGuild (Guild g) {
		guild = g;
	}

	//获得公会职务
	public int getGuildJob () {
		return guild.job;
	}
	//创建公会列表
	public void createGuildList (GuildRankInfo temp) {
		if (guildList == null)
			guildList = new List<GuildRankInfo> ();
		guildList.Add (temp);
	}
	//更新申请按钮状态
	public void updateApplyState (GuildRankInfo info, bool isApply) {
		for (int i = 0; i < guildList.Count; i++) {
			if (guildList [i].name == info.name) {
				guildList [i].isApply = isApply;
			}
		}
	}

	public GuildRankInfo getGuildByUid (string uid) {
		for (int i = 0; guildList != null && i < guildList.Count; i++) {
			if (guildList [i].uid == uid)
				return guildList [i];
		}
		return null;
	}
	/// <summary>
	/// 根据公会名或者公会uid模糊查找对应公会列表
	/// </summary>
	/// <param name="guildName"></param>
	/// <returns></returns>
	public List<GuildRankInfo> findGuildRankInfoByName (string guildName) {
		List<GuildRankInfo> sortList = getGuildList ();
		// 这里调用按照规则排序的列表
		if (sortList == null || sortList.Count == 0)
			return null;
		List<GuildRankInfo> findList = new List<GuildRankInfo> ();
		GuildRankInfo guildRankInfo;
		for (int i = 0; i < sortList.Count; i++) {
			guildRankInfo = sortList [i];
			if (guildRankInfo.name == null || guildRankInfo.uid == null)
				continue;
			if (guildRankInfo.name.IndexOf (guildName) >= 0) {
				findList.Add (guildRankInfo);
			}
		}
		return findList;
	}

	//获取公会列表，把申请过的,且活跃度高的公会排在前面
	public List<GuildRankInfo> getGuildList () {
		if (guildList == null) 
			guildList = new List<GuildRankInfo> ();
		if (ids == null)
			ids = new List<string> ();
		foreach (GuildRankInfo info in guildList) {
			if(ids.Contains(info.uid)){
				info.isApply = true;
			}
		}
		return guildList;
	}

	//获得申请的公会uid集
	public List<string> getIds () {
		return ids;
	}
	//添加uid到申请的公会uid集
	public void addIds (string id) {
		if (ids == null)
			ids = new List<string> ();
		ids.Add (id);
	}

	public void clearIds () {
		ids = null;

		for (int i = 0; guildList != null && i < guildList.Count; i++) {
			guildList [i].isApply = false;
		}
	}

	//移除申请的公会uid集里的uid
	public void removeIds (string id) {
		ids.Remove (id);
		for (int i = 0; i < guildList.Count; i++) {
			if (guildList [i].uid == id) {
				guildList [i].isApply = false;
			}
		}
	}
	//更新公会贡献值
	public void updateContrition (int value) {
		guild.contributioning = value;
		GuildMainWindow gmw = UiManager.Instance.getWindow<GuildMainWindow> ();
		if (gmw != null)
			gmw.UpdateGuildContent ();
	}
	//更新活跃度
	public void updateLiveness (int value) {
		guild.livenessing -= value;
	}
	//判断是否有副会长
	public bool isHaveVicePresident () {
		for (int i = 0; i < guildMembers.Count; i++) {
			if (guildMembers [i].job == GuildJobType.JOB_VICE_PRESIDENT)
				return true;
		}
		return false;
	}
	//是否是副会长
	public bool isVicePresident (string uid) {
		for (int i = 0; i < guildMembers.Count; i++) {
			if (guildMembers [i].uid == uid && guildMembers [i].job == GuildJobType.JOB_VICE_PRESIDENT)
				return true;
		}
		return false;
	}
	//是否是会长
	public bool isPresident (string uid) {
		for (int i = 0; i < guildMembers.Count; i++) {
			if (guildMembers [i].uid == uid && guildMembers [i].job == GuildJobType.JOB_PRESIDENT)
				return true;
		}
		return false;
	}
	#endregion
	#region 邀请
	//创建公会邀请列表
	public void createGuildInviteList (GuildRankInfo temp) {
		if (guildInviteList == null)
			guildInviteList = new List<GuildRankInfo> ();
		guildInviteList.Add (temp);
	}
	//清除公会邀请列表
	public void clearGuildInviteList () {
		guildInviteList = null;
	}
	//获取公会邀请列表,按活跃度排序,只取前10个
	public List<GuildRankInfo> getGuildInviteList () {
		if (guildInviteList == null)
			return null;
		if (guildInviteList.Count == 0)
			return guildInviteList;
		for (int i = 0; i < guildInviteList.Count - 1; i++) {
			for (int j = 0; j < guildInviteList.Count - 1 - i; j++) {
				if (guildInviteList [j].liveness < guildInviteList [j + 1].liveness) {
					GuildRankInfo temp = guildInviteList [j];
					guildInviteList [j] = guildInviteList [j + 1];
					guildInviteList [j + 1] = temp;
				}
			}
		}
		List<GuildRankInfo> tempList = new List<GuildRankInfo> ();
		for (int i = 0; i < guildInviteList.Count; i++) {
			tempList.Add (guildInviteList [i]);
			if (i == 9)
				break;
		}
		return tempList;
	}
	//按uid从邀请列表中移除某个项
	public void removeGuildInviteByUid (string uid) {
		for (int i = 0; i < guildInviteList.Count; i++) {
			if (guildInviteList [i].uid == uid) {
				guildInviteList.Remove (guildInviteList [i]);
				break;
			}
		}
	}
	//是否可以主动邀请会员加入公会
	public bool isInvite () {
		if (guild.membership >= guild.membershipMax) {
			return false;
		}
		else {
			return true;
		}
	}
	#endregion
	#region 成员

	public void clearGuildMembers () {
		guildMembers = null;
	}
	//创建公会成员列表
	public void createGuildMembers (GuildMember temp) {
		if (guildMembers == null)
			guildMembers = new List<GuildMember> ();
		guildMembers.Add (temp);
	}
	//获得公会成员列表，按捐献值高低排序
	public List<GuildMember> getGuildMembersByDonate () {
		for (int i = 0; i < guildMembers.Count - 1; i++) {
			for (int j = 0; j < guildMembers.Count - 1 - i; j++) {
				if (guildMembers [j].donated < guildMembers [j + 1].donated) {
					GuildMember temp = guildMembers [j];
					guildMembers [j] = guildMembers [j + 1];
					guildMembers [j + 1] = temp;
				}
			}
		}
		return guildMembers;
	}
	
	//获得公会成员列表，按贡献值高低排序
	public List<GuildMember> getGuildMembersByContr () {
		for (int i = 0; i < guildMembers.Count - 1; i++) {
			for (int j = 0; j < guildMembers.Count - 1 - i; j++) {
				if (guildMembers [j].contributioned < guildMembers [j + 1].contributioned) {
					GuildMember temp = guildMembers [j];
					guildMembers [j] = guildMembers [j + 1];
					guildMembers [j + 1] = temp;
				}
			}
		}
		return guildMembers;
	}
	//获得公会成员列表，按职务高低排序
	public List<GuildMember> getGuildMembersByJob () {
		List<GuildMember> officerList = new List<GuildMember> ();
		List<GuildMember> commonList = new List<GuildMember> ();
		List<GuildMember> listPre = new List<GuildMember> ();
		List<GuildMember> listVicePre = new List<GuildMember> ();
		for (int i = 0; i < guildMembers.Count; i++) {
			if (guildMembers [i].job == GuildJobType.JOB_PRESIDENT) {
				listPre.Add (guildMembers [i]);
			}
			else if (guildMembers [i].job == GuildJobType.JOB_VICE_PRESIDENT) {
				listVicePre.Add (guildMembers [i]);
			}
			else if (guildMembers [i].job == GuildJobType.JOB_OFFICER) {
				officerList.Add (guildMembers [i]);
			}
			else if (guildMembers [i].job == GuildJobType.JOB_COMMON) {
				commonList.Add (guildMembers [i]);
			}
		}
		for (int i = 0; i < commonList.Count - 1; i++) {
			for (int j = 0; j < commonList.Count - i - 1; j++) {
				if (commonList [j].level < commonList [j + 1].level) {
					GuildMember temp = commonList [j];
					commonList [j] = commonList [j + 1];
					commonList [j + 1] = temp;
				}
			}
		}
		ListKit.AddRange (listPre, listVicePre);
		ListKit.AddRange (listPre, officerList);
		ListKit.AddRange (listPre, commonList);
		return listPre;
	}
	//更新某个成员的职务
	public void updateJobByUid (string uid, int jobType) {
		for (int i = 0; i < guildMembers.Count; i++) {
			if (guildMembers [i].uid == uid) {
				guildMembers [i].job = jobType;
			}
		}
	}
	//获得会长
	public GuildMember getPresident () {
		for (int i = 0; i < guildMembers.Count; i++) {
			if (guildMembers [i].job == GuildJobType.JOB_PRESIDENT) {
				return guildMembers [i];
			}
		}
		return null;
	}
	//移除某个会员
	public void removeMember (string uid) {
		for (int i = 0; i < guildMembers.Count; i++) {
			if (guildMembers [i].uid == uid) {
				guildMembers.Remove (guildMembers [i]);
				break;
			}
		}
	}

	#endregion
	#region 申请
	//更新申请次数
	public void updateApplyCount (int num) {
		applyCount += num;
	}
	//获得申请次数
	public int getApplyCount () {
		return applyCount;
	}
	#endregion
	#region 退会
	//是否可以退会，判断是公会的最后一个成员
	public void exitGuild (CallBackMsg msg) {
		if (guild.membership == EXITGUILDMEMBER) {
			//当公会最后一个人退会时给提示
			UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
				win.initWindow (2, LanguageConfigManager.Instance.getLanguage ("s0094"), LanguageConfigManager.Instance.getLanguage ("s0093"), null, msg);
			});
		}
		else {
			//给提示，是否确定退会
			UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
				win.initWindow (2, LanguageConfigManager.Instance.getLanguage ("s0094"), LanguageConfigManager.Instance.getLanguage ("s0093"), null, msg);
			});
		}
	}
	#endregion
	#region 踢人
	public void clearMem (CallBackMsg msg) {
		//踢人时给提示
		UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
			win.initWindow (2, LanguageConfigManager.Instance.getLanguage ("s0094"), LanguageConfigManager.Instance.getLanguage ("s0093"), null, msg);
		});
	}
	#endregion
	#region 入会
	//是否可以加入公会
	public bool isJoinGuild () {
		if (UserManager.Instance.self.getUserLevel () < JOINGUILDLEVEL) {
			UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
				win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0093"), null, LanguageConfigManager.Instance.getLanguage ("Guild_12", GuildManagerment.JOINGUILDLEVEL.ToString ()), null);
			});
			return false;
		}
		else if (ids != null && ids.Count >= JOINGUILDCOUNT) {
			UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
				win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0093"), null, LanguageConfigManager.Instance.getLanguage ("Guild_17", GuildManagerment.JOINGUILDCOUNT.ToString ()), null);
			});
			return false;
		}
		return true;
	}
	#endregion
	#region 贡献
	public void donationResult (ErlArray array) {
		int index = 0;
		int liveness = StringKit.toInt (array.Value [index++].getValueString ());
		int contribution = StringKit.toInt (array.Value [index++].getValueString ());
		guild.livenessing += liveness;
		guild.livenessed += liveness;
		guild.contributioning += contribution;
		guild.contributioned += contribution;
		UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
			win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0093"), null, LanguageConfigManager.Instance.getLanguage ("Guild_27", contribution.ToString (), liveness.ToString ()), null);
		});
	}
	#endregion
	#region 审批
	//创建公会审批列表
	public void createGuildApprovalList (GuildApprovalInfo temp) {
		if (guildApprovalList == null)
			guildApprovalList = new List<GuildApprovalInfo> ();
		guildApprovalList.Add (temp);
	}
	//清空公会审批列表
	public void clearGuildApprovalList () {
		guildApprovalList = null;
	}
	//获得公会审批列表
	public List<GuildApprovalInfo> getGuildApprovalList () {
		return guildApprovalList;
	}
	//移除公会审批一项
	public GuildApprovalInfo removeGuildApproval (string uid) {
		GuildApprovalInfo guildApprovalInfo = null;
		for (int i = 0; i < guildApprovalList.Count; i++) {
			guildApprovalInfo = guildApprovalList [i];
			if (guildApprovalInfo.uid == uid) {
				guildApprovalList.Remove (guildApprovalInfo);
				return guildApprovalInfo;
			}
		}
		return null;
	}
	//根据uid获取公会审批列表玩家名
	public string getGuildApprovalName (string uid) {
		GuildApprovalInfo guildApprovalInfo;
		for (int i = 0; i < guildApprovalList.Count; i++) {
			guildApprovalInfo = guildApprovalList [i];
			if (guildApprovalInfo.uid == uid) {
				return guildApprovalInfo.name;
			}
		}
		return null;
	}
	#endregion
	//判断公会等级是否满足要求
	public bool isLevelAdequate (int level) {
		return guild.level >= level;
	}
	//判断公会等级是否达到上限
	public bool isGuildLevelMax () {
		return guild.level >= GUILDLEVELMAX;
	}
	//获得公会宣言
	public string getDeclaration () {
		return guild.declaration;
	}
	//更新公会宣言
	public void updateDeclaration (string str) {
		if (str == "[]")
			str = "";
		guild.declaration = str;
	}
	//获得公会公告
	public string getNotice () {
		return guild.notice;
	}
	//更新公会公告
	public void updateNotice (string str) {
		if (str == "[]")
			str = "";
		guild.notice = str;
	}

	public bool isHaveGuild () {
		return guild != null;
	}

	//根据是否有公会弹相应窗口
	public void openWindow () {
		if (guild == null) {
			UiManager.Instance.openDialogWindow<GuildMainNoJoinWindow> ();
		}
		else {
			UiManager.Instance.openWindow<GuildMainWindow> ((win) => {
				win.initWindow ();
			});
		} 
	}

	//转换时间格式
	public string timeTransform (int time, int outTime) {
		//如果在线，就直接显示在线
		if (time >= outTime) {
			return LanguageConfigManager.Instance.getLanguage ("Guild_20");
		}
		outTime = ServerTimeKit.getSecondTime () - outTime;
		int hours = (int)(outTime / 3600);
		int minutes = (int)(outTime % 3600 / 60);
		int seconds = (int)(outTime % 3600 % 60);
		if (hours >= 24) {
			int days = hours / 24;
			if (days < 7) {
				return LanguageConfigManager.Instance.getLanguage ("Guild_23", days.ToString ());
			}
			else if (days / 30 < 1) {
				return LanguageConfigManager.Instance.getLanguage ("Guild_24", (days / 7).ToString ());
			}
			else if (days < 360) {
				return LanguageConfigManager.Instance.getLanguage ("Guild_25", (days / 30).ToString ());
			}
			else {
				return LanguageConfigManager.Instance.getLanguage ("Guild_25_year", (days / 365).ToString ());
			}
		}
		else if (hours < 24 && hours >= 1) {
			return LanguageConfigManager.Instance.getLanguage ("Guild_22", hours.ToString ());
		}
		else if (hours < 1 && minutes >= 1) {
			return LanguageConfigManager.Instance.getLanguage ("Guild_26", minutes.ToString ());
		}
		else {
			if (seconds < 0)
				seconds = 0;
			return LanguageConfigManager.Instance.getLanguage ("Guild_21", seconds.ToString ());
		}
	}

	//弹劾逻辑，根据职位，是否可以弹劾
	public bool isImpeach () {
		GuildMember president = getPresident ();
		if (president.lastLogin > president.lastLogout)
			return false;
		int outTime = president.lastLogout;
		outTime = ServerTimeKit.getSecondTime () - outTime;
		int hours = (int)(outTime / 3600);
		int days = hours / 24;
		if (days >= 7 && days < 14 && guild.job == GuildJobType.JOB_VICE_PRESIDENT) {
			return true;
		}
		else if (days >= 14 && days < 21 && (guild.job == GuildJobType.JOB_VICE_PRESIDENT || guild.job == GuildJobType.JOB_OFFICER)) {
			return true;
		}
		else if (days >= 21) {
			return true;
		}
		else {
			return false;
		}
	}

	//获得公会内部贡献排名名,10个
	public string[] getConRankNames () {
		List<string> list = new List<string> ();
		for (int i = 0; i < 10; i++) {
			list.Add (getGuildMembersByContr () [i].name);
		}
		return list.ToArray ();
	}
	//获得公会内部贡献排名值
	public string[] getConRankValues () {
		List<string> list = new List<string> ();
		for (int i = 0; i < 10; i++) {
			list.Add (getGuildMembersByContr () [i].contributioned.ToString ());
		}
		return list.ToArray ();
	}
	//获得公会内部捐献排名名，10个
	public string[] getDoRankNames () {
		List<string> list = new List<string> ();
		for (int i = 0; i < 10; i++) {
			list.Add (getGuildMembersByDonate () [i].name);
		}
		return list.ToArray ();
	}
	//获得公会内部捐献排名值，10个
	public string[] getDoRankValues () {
		List<string> list = new List<string> ();
		for (int i = 0; i < 10; i++) {
			list.Add (getGuildMembersByDonate () [i].name);
		}
		return list.ToArray ();
	}
	/** 获得公会技能对战斗中卡片经验加成 */
	public int getSkillAddExpPorCardPve () {
		return getSkillAddExpPorPve (SKILLTYPE_CARDEXP);
	}
	/** 获得公会技能对战斗中玩家经验加成 */
	public int getSkillAddExpPorRolePve () {
		return getSkillAddExpPorPve (SKILLTYPE_ROLEEXP);
	}
	/** 获得公会技能对战斗中幻兽经验加成 */
	public int getSkillAddExpPorBeastPve () {
		return getSkillAddExpPorPve (SKILLTYPE_BEASTEXP);
	}
	/** 获得公会技能的相关经验加成 */
	private int getSkillAddExpPorPve (string typeStr) {
		int exp = 0;
		if (getGuildSkill () == null)
			return 0;
		for (int i = 0; i < getGuildSkill().Count; i++) {
			GuildSkillSample sample = GuildSkillSampleManager.Instance.getGuildSkillSampleBySid (StringKit.toInt (getGuildSkill () [i].sid));
			for (int j = 0; j < sample.attr.Length; j++) {
				if (sample.attr [j].getAttrType () == typeStr) {
					exp += sample.attr [0].getAttrValue (getGuildSkill () [i].level);
				}
			}
		}

		return exp;
	}
	/** 获得公会技能对卡片的属性加成 */
	public CardBaseAttribute getSkillEffect () {
		if (getGuildSkill () == null)
			return null;
		CardBaseAttribute attr = new CardBaseAttribute ();
		GuildSkillSample sample;
		for (int i = 0; i < getGuildSkill().Count; i++) {
			sample = GuildSkillSampleManager.Instance.getGuildSkillSampleBySid (StringKit.toInt (getGuildSkill () [i].sid));
			for (int j = 0; j < sample.attr.Length; j++) {
				if (sample.attr [j].getAttrType () == AttrChangeType.HP) {
					attr.hp += sample.attr [0].getAttrValue (getGuildSkill () [i].level);
				}
				else if (sample.attr [j].getAttrType () == AttrChangeType.ATTACK) {
					attr.attack += sample.attr [0].getAttrValue (getGuildSkill () [i].level);
				}
				else if (sample.attr [j].getAttrType () == AttrChangeType.DEFENSE) {
					attr.defecse += sample.attr [0].getAttrValue (getGuildSkill () [i].level);
				}
				else if (sample.attr [j].getAttrType () == AttrChangeType.MAGIC) {
					attr.magic += sample.attr [0].getAttrValue (getGuildSkill () [i].level);
				}
				else if (sample.attr [j].getAttrType () == AttrChangeType.AGILE) {
					attr.agile += sample.attr [0].getAttrValue (getGuildSkill () [i].level);
				} 
			}
		}
		return attr;
	}
	/* 返回指定类型技能的等级*/
	public int getSkillLevel (string skillType) {
		if (getGuildSkill () == null)
			return 0;
		GuildSkillSample sample;
		for (int i = 0; i < getGuildSkill().Count; i++) {
			sample = GuildSkillSampleManager.Instance.getGuildSkillSampleBySid (StringKit.toInt (getGuildSkill () [i].sid));
			for (int j = 0; j < sample.attr.Length; j++) {
				if (sample.attr [j].getAttrType () == skillType)
					return getGuildSkill () [i].level;
			}
		}
		return 0;
	}
	public void closeAllGuildWindow (MessageHandle msg) {
		UiManager.Instance.openMainWindow ();
	}
	private IEnumerator delayOpenWindow () {
		yield return new WaitForSeconds (0.5f);
		UiManager.Instance.openMainWindow ();
	}
	//被踢清理数据
	public void clearMember () {
		updateMsg = null;
		guild = null;
		applyCount = 0;
		guildList = null;
		guildInviteList = null;
		guildMembers = null;
		ids = null;
		builds = new List<GuildBuild> (){new GuildBuild("1",0),new GuildBuild("3",0),new GuildBuild("4",0),new GuildBuild("2",0),new GuildBuild("5",0)};
        if (guildApprovalList!=null) guildApprovalList.Clear();
		isGuildBattle = false;
		if (UiManager.Instance.CurrentWindow.name.StartsWith ("Guild"))
			UiManager.Instance.openMainWindow ();
	}
	public void clearAllDate () {
        
		updateMsg = null;
		guild = null;
		applyCount = 0;
		guildList = null;
		guildInviteList = null;
		guildMembers = null;
		ids = null;
        if (guildApprovalList != null) guildApprovalList.Clear();
		builds = new List<GuildBuild> (){new GuildBuild("1",0),new GuildBuild("3",0),new GuildBuild("4",0),new GuildBuild("2",0),new GuildBuild("5",0)};
		isGuildBattle = false;
	}
	public void ApplyMessageLint (string msg, string guildName) {
		if (msg.Equals ("ok")) {
			UiManager.Instance.createMessageLintWindow (LanguageConfigManager.Instance.getLanguage ("s0577", guildName));
		}
	}

	
	private void initContribution(){
		string awardStr = LanguageConfigManager.Instance.getLanguage ("GuildLuckyNvShen_26");
		string [] awardStrs = awardStr.Split (',');
		awardContributions = new int[awardStrs.Length];
		for (int i =0; i <awardStrs.Length; i++) {
			awardContributions[i] = StringKit.toInt(awardStrs[i]);
		}
	}
	public int getContribution(int rank){
		if (awardContributions == null)
			initContribution ();
		int contribution = 0;
		if (rank >= awardContributions.Length) {
			contribution = awardContributions [awardContributions.Length - 1];
		} else {
			contribution = awardContributions[rank-1];
		}
		return contribution;
	}
	public void setAutoJoin(bool isAuto){
		if(isAuto)autoJoin=1;
		else autoJoin=0;
	}
	public void updateAutoJoin(){
		if(autoJoin!=guild.autoJoin){
			GuildAutoJoinFPort fport = FPortManager.Instance.getFPort ("GuildAutoJoinFPort") as GuildAutoJoinFPort;
			fport.access (autoJoin);
		}
	}
	public void  setGongxuanIndex(int index){
		gongxuanIndex=index;
	}
	public int getGongxuanIndex(){
		return gongxuanIndex;
	}
	public void setWunNum(int num){
		wunNum=num;
	}
	public int getWunNum(){
		return wunNum;
	}

	public  void SendRivive (CallBack callback)
	{
		/** 公会战复活消耗模版 */
	    GuildFightReviveSample reviveSample = GuildFightSampleManager.Instance ().getSampleBySid<GuildFightReviveSample> (GuildFightSampleManager.REVIVE_COST);

		/** 行动力不足 */
		if (UserManager.Instance.self.guildFightPower < reviveSample.cost) {
			goToGetPower ();
			return;
		}
		MessageWindow.ShowConfirm (LanguageConfigManager.Instance.getLanguage ("GuildArea_90"), (msg) => {
			if (msg.msgEvent == msg_event.dialogOK) {
				GuildAreaReviveFPort port = FPortManager.Instance.getFPort<GuildAreaReviveFPort> ();
				port.access (() =>{
					UserManager.Instance.self.guildFightPower -= reviveSample.cost;
					EffectManager.Instance.CreateActionCast (reviveSample.expendDes, ActionCastCtrl.GUILD_FIGHT_TYPE);
					GuildManagerment.Instance.guildFightInfo.isDead = false;
					UiManager.Instance.createMessageLintWindow (LanguageConfigManager.Instance.getLanguage("GuildArea_66"));
					ArmyManager.Instance.setArmyState(ArmyManager.PVP_GUILD,0);
					isReviveBack = true;
					if(callback!=null)
						callback();
				});
			}
		});
	}
	/** 前往任务获取行动力 */
	private void goToGetPower ()
	{
		UiManager.Instance.openDialogWindow<MessageWindow> ((win) =>
		                                                    {
			win.dialogCloseUnlockUI = false;
			win.initWindow (2,LanguageConfigManager.Instance.getLanguage("s0094"), LanguageConfigManager.Instance.getLanguage("GuildArea_50"), LanguageConfigManager.Instance.getLanguage("GuildArea_06"), (msg) =>
			                {
				if (msg.msgEvent == msg_event.dialogOK) {
					UiManager.Instance.openWindow<TaskWindow> ((tWin) =>{
						tWin.initTap (0);
					});
				} else {
					MaskWindow.UnlockUI ();
				}
			});
		});
		
	}
}
