using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArenaManager 
{
	/* const */
	/** 开服竞技等待阶段 */
    public const int STATE_WAIT = 0;
	/** 海选 */
    public const int STATE_MASS = 1;
	/** 64进32 */
    public const int STATE_64_32 = 2;
	/** 32进16 */
    public const int STATE_32_16 = 3;
	/** 16进8 */
    public const int STATE_16_8 = 4;
	/** 8进4 */
    public const int STATE_8_4 = 5;
	/** 4进2 */
    public const int STATE_4_2 = 6;
	/** 冠军 */
    public const int STATE_CHAMPION = 7;
	/** 休赛 */
    public const int STATE_RESET = 8;
	/** 海选休赛 */
    public const int STATE_MASS_RESET = 9;
	/** 上次看的标签 */
	public int tapIndex = 0;

	/* static methods */
	public static ArenaManager instance {
		get {
			return SingleManager.Instance.getObj ("ArenaManager") as ArenaManager;
		}
	}
	static void addArenaFinalInfo (List<ArenaFinalInfo> infoList, ArenaFinalInfo[] infos) {
		foreach (ArenaFinalInfo each in infos) {
			
			infoList.Add (each);
		}
	}
	/** 计算竞技场可竞猜数 */
	public static int computeGuessNumber () {
		if (ArenaManager.instance.finalInfoList == null)
			return 0;
		List<ArenaFinalInfo>[] allInfoList = new List<ArenaFinalInfo>[5];
		for (int i = 0; i < 5; i++) {
			List<ArenaFinalInfo> infoList = new List<ArenaFinalInfo> ();
			ArenaFinalInfo[][] infoListArr;
			ArenaFinalInfo[] infos;
			if (i <= 3) {
				infoListArr = new ArenaFinalInfo[5][];
				infos = ArenaManager.instance.getFinalInfo (i * 16, 16);
				if (infos != null)
					addArenaFinalInfo (infoList, infos);
				infos = ArenaManager.instance.getFinalInfo (i * 8 + 64, 8);
				if (infos != null)
					addArenaFinalInfo (infoList, infos);
				infos = ArenaManager.instance.getFinalInfo (i * 4 + 96, 4);
				if (infos != null)
					addArenaFinalInfo (infoList, infos);
				infos = ArenaManager.instance.getFinalInfo (i * 2 + 112, 2);
				if (infos != null)
					addArenaFinalInfo (infoList, infos);
				infos = ArenaManager.instance.getFinalInfo (i + 120, 1);
				if (infos != null)
					addArenaFinalInfo (infoList, infos);
			}
			else {
				infoListArr = new ArenaFinalInfo[3][];
				infos = ArenaManager.instance.getFinalInfo (120, 4);
				if (infos != null)
					addArenaFinalInfo (infoList, infos);
				infos = ArenaManager.instance.getFinalInfo (124, 2);
				if (infos != null)
					addArenaFinalInfo (infoList, infos);
				infos = ArenaManager.instance.getFinalInfo (126, 1);
				if (infos != null)
					addArenaFinalInfo (infoList, infos);
			}
			allInfoList [i] = infoList;
		}
		int now = ServerTimeKit.getSecondTime ();
		int count = 0;
		for (int i = 0; i < 5; i++) {
			List<ArenaFinalInfo> infoList = allInfoList [i];
			for (int j = 0; j < infoList.Count - 1; j+= 2) {
				ArenaFinalInfo info = infoList [j].hasUser () ? infoList [j] : infoList [j + 1];
				if (info.guessStartTime + 60 <= now && now <= info.guessEndTime) {
					if (!(infoList [j].guessed || infoList [j + 1].guessed)) {
						count++;
					}
				}
			}
		}
		return count;
	}

	/** 我的竞技信息 */
    public ArenaUserInfo self;
	/** 上次刷新对手时间 */
    public int lastUpdateEnemyTime;
	/** 已挑战次数 */
    public int challengeCount;
	/** 最大可挑战次数 */
    public int maxChallengeCount;
	/** 挑战时间(天)1-366 */
    public int challengeTime;
	/// <summary>
	/// 购买的挑战次数
	/// </summary>
    public int buyChallengeCount;
	/** 购买挑战时间 */
    public int buyChallengeTime;
	/** 挑战累计使用时间 */
    public int challengeUseTime;
	/** 挑战上次更新时间 */
    public int challengeLastUpdateTime;
	/** 是否为红CD,红cd即不可挑战 */
    public int redChallengeCd;
	/** 对手列表 */
    public List<ArenaUserInfo> enemyList;
	/** 海选战斗类型,参考BattleType */
    public int massBattleType;

	/** 比赛状态,0开服竞技等待阶段,1海选,2(64-32),3(32-16),4(16-8),5(8-4),6(4-2),7决赛,8休赛,9海选休赛  */
    public int state;
	/** 状态结束时间 */
    public int stateEndTime;
	/** 决赛点位列表 */
    public List<ArenaFinalInfo> finalInfoList;
	/** 当前决赛场次 */
    public int finalRound;
	/** 下一场次倒计时 */
    public int finalCD=0;
	/** 下一场次延时时间 */
    public int finalDelay;
	/** 我的积分奖励 */
    public int finalMyIntergal;
	/** 当前正在挑战的对手uid */
    public string currentMassEnemyUid;

    /// <summary>
    /// 获取当前决赛进度
    /// </summary>
    public int getFinalCurrentProgress()
    {
        return 0;
    }

	/// <summary>
	/// 获取赛程描述
	/// </summary>
	public Dictionary<string,string> getFinalPreduceDes(){
		string str = LanguageConfigManager.Instance.getLanguage ("Arena72");
		string [] des = str.Split ('%');
		List<int> timeList = new List<int> ();
		for (int i =0; i <  finalInfoList.Count-1; i++) {
			if(timeList.Contains(finalInfoList[i].startTime))
				continue;
			timeList.Add(finalInfoList[i].startTime);
		}
		Dictionary<string,string> arenaTimesDic = new Dictionary<string, string> ();
		for (int i =0; i<des.Length; i++) {
			des[i] = "[91400C]" + des[i] +"[-]" ;
			arenaTimesDic.Add(des[i],getFinalPreduceTimeDes(timeList[i]));
		}
		return  arenaTimesDic;
	}



	private string getFinalPreduceTimeDes(int time){
		System.DateTime startDate = TimeKit.getDateTime (time);
		System.DateTime serverDate = ServerTimeKit.getDateTime ();
		//已结束
		if (time <ServerTimeKit.getSecondTime()) {
			return "[666666]" + LanguageConfigManager.Instance.getLanguage ("Arena73") + "[-]";
		}
		/**  今天未开始 */
		else if (serverDate.DayOfYear == startDate.DayOfYear) {
			return "[00BB00]" + LanguageConfigManager.Instance.getLanguage("Arena74", startDate.Hour.ToString()) +"[-]";
		}
		/** 今后未开始 */
		else {
			return "[BD3232]" +  LanguageConfigManager.Instance.getLanguage("Arena75",startDate.Month.ToString(),startDate.Day.ToString(),startDate.Hour.ToString()) + "[-]";
		}
	
	}

    public string getTeamNameById(int id)
    {
        if (id == 1)
            return "A";
        else if (id == 2)
            return "B";
        else if(id == 3)
            return "C";
        else
            return "D";
    }

    public void setEnemyList(List<ArenaUserInfo> list)
    {
        if (list == null)
            list = new List<ArenaUserInfo>();
        if (list.Count < 5)
        {
            int count = ArenaRobotUserSampleManager.Instance.data.Count;
            for(int i = list.Count; i < 5; i++)
            {
                ArenaUserInfo user = new ArenaUserInfo();
                int id = Random.Range(1,count+1);
                ArenaRobotUserSample sample = ArenaRobotUserSampleManager.Instance.getSampleBySid(id);
                user.headIcon = sample.headIcon;
                user.name = sample.name;
                user.level = sample.level;
                user.massPosition = i + 1;
                user.npc = true;
                list.Add(user);
            }
        }
        enemyList = list;
    }

    public List<ArenaUserInfo> getEnemyList()
    {
        return enemyList;
    }

    public ArenaFinalInfo[] getFinalInfo(int star, int len)
    {
        ArenaFinalInfo[] infos = new ArenaFinalInfo[len];
        for (int i = 0; i < len; i++)
        {
            infos[i] = finalInfoList[star + i];
        }
        return infos;
    }

    /// <summary>
    /// 获取当前可用挑战次数
    /// </summary>
    public int getChallengeCount()
    {
        return buyChallengeCount + maxChallengeCount - challengeCount;
    }

    /// <summary>
    /// 获取挑战CD倒计时(秒)
    /// </summary>
    public int getChallengeCD()
    {
        return challengeUseTime + challengeLastUpdateTime - ServerTimeKit.getSecondTime();
    }

    /// <summary>
    /// 判断当前状态是否正确,是否过期
    /// </summary>
    public bool isStateCorrect()
    {
        return ServerTimeKit.getSecondTime() < this.stateEndTime;
    }

    public int getMyIntergal()
    {
        if (self != null)
            return self.integral;
        else
            return finalMyIntergal;
    }

	public int getMaxCanBuyCount()
	{
		int addNum = 0;
		int vipLevel = UserManager.Instance.self.vipLevel;
		Vip vip = VipManagerment.Instance.getVipbyLevel (vipLevel);
		if (vip != null) {
			addNum = vip.privilege.areaCountBuyAdd;
		}
		return ArenaInfoConfigManager.Instance().getSampleBySid(1001).num;
	}
}
