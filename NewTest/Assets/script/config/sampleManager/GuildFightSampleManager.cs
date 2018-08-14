using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;

/// <summary>
/// 公会战乱七八糟配置
/// </summary>
public class GuildFightSampleManager : SampleConfigManager
{
	/** 祝福的模版SID */
	public const int WISH_SID = 1;
	/** 鼓舞的模版SID */
	public const int INSPIRE_SID = 2;
    /** 复活消耗模版SID */
    public const int REVIVE_COST = 3;
	/** 挑战的模版SID */
	public const int CHALLENGE_SID = 101;
	/** 获取行动值的模版SID */
	public const int GET_POWER = 102;
	/** 评级SID */
	public const int JUDGE_SID = 1001;
	/** 公会战参与条件SID */
	public const int JOIN_CONDITION =10001;
	/** 战斗时间 */
	public const int FIGHT_TIME = 100001;
	/** 公会战开启时间 */
	public const int OPEN_TIME = 100002;
	/** 能激活BUFF的时间 */
	public const int BUFF_TIME = 100003;
	/** 每日任务获得行军值SID */
	public const int TASK_POWER = 1000001;
	/** 每日任务获得行军值上限SID */
	public const int TASK_MAX_POWER = 1000002;

	private  static GuildFightSampleManager _instance;

	private bool isShowGuildFightFlagBool = true;

	public static GuildFightSampleManager Instance ()
	{
		if (_instance == null)
			_instance = new GuildFightSampleManager ();
		return _instance;
	}

	public GuildFightSampleManager ()
	{
		base.readConfig (ConfigGlobal.CONFIG_GUILD_FIGHT);
	}

	public T getSampleBySid<T> (int sid)where T : Sample {
		
		if (!samples.ContainsKey (sid)) {
			T t = Activator.CreateInstance<T> ();
			t.parse(sid,data[sid].ToString());
			samples.Add(sid,t);
		}
		return samples[sid] as T;
	}

	/// <summary>
	/// 增加的行军值是否超过行军值上限
	/// </summary>
	public bool isOverPower ()
	{
		return UserManager.Instance.self.guildFightPower + getTaskPowerBySid (TASK_POWER) > getTaskPowerBySid (TASK_MAX_POWER);
	}

	/// <summary>
	/// 返回配置的指定数值
	/// </summary>
	public int getTaskPowerBySid (int sid)
	{
		if (!isSampleExist (sid)) {
			string dataStr = getSampleDataBySid (sid); 
			string [] strs = dataStr.Split ('|');
			int taskPower = StringKit.toInt(strs [1]);
			samples.Add (sid, taskPower);
		}
		return (int)samples [sid];
	}


	/** 获取公会战开启时间模版 */
	public GuildFightGetPowerSample getGuildFightGetPowerSampleBySid(int sid){
		if (!isSampleExist (sid)) {
			string dataStr = getSampleDataBySid (sid); 
			GuildFightGetPowerSample sample = new GuildFightGetPowerSample ();
			sample.parse (sid, dataStr);
			samples.Add (sid, sample);
		}
		return samples [sid] as GuildFightGetPowerSample;
	}

	/** 是否为开战时间 */
	public bool isFightTime(){
        if (!isGuildFightOpen())
            return false;
		bool isFightTime = false;
		GuildFightTimeSample timeSample = getSampleBySid<GuildFightTimeSample>(FIGHT_TIME);
		int serverTime = changeDateToSecond(ServerTimeKit.getDateTime());
		foreach (GuildFightTime time in timeSample.timeList) {
			int startTime = changeDateToSecond(System.DateTime.Parse(time.start));
			int endTime = changeDateToSecond(System.DateTime.Parse(time.end));
			if(startTime <= serverTime && serverTime <=endTime){
				isFightTime = true;
				break;
			}
		}
		return isFightTime;
	}

    /// <summary>
    /// 是否为公会战开战以及开战前5分钟
    /// </summary>
    public bool isGuildFightAndFrontFiveMin() {
        if (!isGuildFightOpen())
            return false;
        bool result = false;
        GuildFightTimeSample timeSample = getSampleBySid<GuildFightTimeSample>(FIGHT_TIME);
        int serverTime = changeDateToSecond(ServerTimeKit.getDateTime());
        foreach (GuildFightTime time in timeSample.timeList)
        {
            int startTime = changeDateToSecond(System.DateTime.Parse(time.start));
            int endTime = changeDateToSecond(System.DateTime.Parse(time.end));
            if (startTime - 300 <= serverTime && serverTime <= endTime)
            {
                result = true;
                break;
            }
        }
        return result;
    }

	/// <summary>
	/// 是否为限时公会战阶段提示的时间
	/// </summary>
	public bool isShowTipsTime(){
		if(!isGuildFightOpen())
			return false;
		bool isShowTipsTime = true;
		GuildFightTimeSample timeSample = getSampleBySid<GuildFightTimeSample>(FIGHT_TIME);
		int serverTime = changeDateToSecond(ServerTimeKit.getDateTime());
		foreach (GuildFightTime time in timeSample.timeList) {
			int startTime = changeDateToSecond(System.DateTime.Parse(time.start));
			int endTime = changeDateToSecond(System.DateTime.Parse(time.end));
			if(startTime -300<= serverTime && serverTime <=endTime){
				isShowTipsTime = false;
				break;
			}
		}
		return isShowTipsTime;
	}

	/** 获取该场公会战剩余时间 */
	public int getFightTimeDelay(){
		if (!isFightTime ())
			return 0;
		int result = 0;
		GuildFightTimeSample timeSample = getSampleBySid<GuildFightTimeSample>(FIGHT_TIME);
		int serverTime = changeDateToSecond(ServerTimeKit.getDateTime());
		foreach (GuildFightTime time in timeSample.timeList) {
			int endTime = changeDateToSecond(System.DateTime.Parse(time.end));
			int startTime = changeDateToSecond(System.DateTime.Parse(time.start));
			if(startTime <= serverTime && serverTime <=endTime){
				result = endTime - serverTime;
				break;
			}
		}
		return result;
	}

	/** 是否为可激活BUFF时间 */
	public bool isActivityBuffTime(){
		bool isActivityTime = false;
		GuildFightTimeSample timeSample = getSampleBySid<GuildFightTimeSample>(BUFF_TIME);
		int serverTime = changeDateToSecond(ServerTimeKit.getDateTime());

		foreach (GuildFightTime time in timeSample.timeList) {
			int startTime = changeDateToSecond(System.DateTime.Parse(time.start));
			int endTime = changeDateToSecond(System.DateTime.Parse(time.end));
			if(startTime <= serverTime && serverTime <=endTime){
				isActivityTime = true;
				break;
			}
		}
		return isActivityTime;
	}

	/** 当天的公会战是否已经结束 */
	public bool isGuildFightOver(){
		bool isFightOver = true;
		GuildFightTimeSample timeSample = getSampleBySid<GuildFightTimeSample>(FIGHT_TIME);
		int serverTime = changeDateToSecond(ServerTimeKit.getDateTime());
		foreach (GuildFightTime time in timeSample.timeList) {
			int startTime = changeDateToSecond(System.DateTime.Parse(time.start));
			int endTime = changeDateToSecond(System.DateTime.Parse(time.end));
			if(endTime > serverTime){
				isFightOver = false;
				break;
			}
		}
		return isFightOver;
	}

	/// <summary>
	/// 获取公会战下次开战时间
	/// </summary>
	public int getLastFightOpenTime(){
		/** 如果在开战期间 */
		if (isFightTime())
			return 0;
		int result = 0;
		int serverTime = changeDateToSecond(ServerTimeKit.getDateTime());
		GuildFightTimeSample timeSample = getSampleBySid<GuildFightTimeSample>(FIGHT_TIME);
		foreach (GuildFightTime time in timeSample.timeList) {
			int startTime = changeDateToSecond(System.DateTime.Parse(time.start));
			if(serverTime <= startTime){
				int temp = startTime - serverTime;
				if(result ==0 || temp<result)
					result = temp;
			}
		}
		return result;
	}

	/** 获取公会战第一次开打时间 */
	public string getGuildFightOpenTime(){
		int result = 0;
		GuildFightTimeSample timeSample = getSampleBySid<GuildFightTimeSample>(FIGHT_TIME);
		foreach (GuildFightTime time in timeSample.timeList) {
			int startTime = changeDateToSecond(System.DateTime.Parse(time.start));
			if(result ==0 || startTime <result){
				result = startTime;
			}	
		}
		System.DateTime date = System.DateTime.Parse("00:00:00");
		date = date.AddSeconds (result);
		return date.ToString ("HH:mm:ss");

	}

	/** 获取最近的公会战开打时间 */
	public string getLastFightOpenTimeString(){
		int secondes = getLastFightOpenTime ();
		System.DateTime date = System.DateTime.Parse("00:00:00");
		date = date.AddSeconds (secondes);
		return date.ToString ("HH:mm:ss");
	}

	public int changeDateToSecond(System.DateTime date){
		return  date.Hour * 3600 + date.Minute * 60 + date.Second;
	}


	/** 根据评分获取评价等级 */
	public string getJudgeString(int judgeScore){
		return  getSampleBySid<GuildFightJudgeSample>(JUDGE_SID).getJudgeString(judgeScore);
	}
    /// <summary>
    /// 获得最大评分
    /// </summary>
    /// <returns></returns>
    public int getMaxScore()
    {
        return getSampleBySid<GuildFightJudgeSample>(JUDGE_SID).getMaxJudgeScore();
    }
    public int getNextScore(int score)
    {
        return getSampleBySid<GuildFightJudgeSample>(JUDGE_SID).getNextJudgeScore(score);
    }

	/** 公会战是否在今天开启 */
	public bool isGuildFightOpen(){
		GuildFightOpenTimeSample sample = getSampleBySid<GuildFightOpenTimeSample>(OPEN_TIME);
		return sample.isOpenTime ();
	}

	/** 是否显示公会战标签 */
	public bool isShowGuildFightFlag(){
		if (isGuildFightOpen () == false)
			return false;
		if (!isShowGuildFightFlagBool)
			return 	false;
		int serverTime = changeDateToSecond(ServerTimeKit.getDateTime());
		GuildFightTimeSample timeSample = getSampleBySid<GuildFightTimeSample>(FIGHT_TIME);
		foreach (GuildFightTime time in timeSample.timeList) {
			int startTime = changeDateToSecond(System.DateTime.Parse(time.start));
			int endTime = changeDateToSecond(System.DateTime.Parse(time.end));
			if(startTime-300<=serverTime && serverTime <= endTime)
				return true;
		}
		return false;
	}
	/** 保存是否显示公会战标签 */
	public void saveGuildFigthFlag(){
		if (isShowGuildFightFlag() && isShowGuildFightFlagBool)
			isShowGuildFightFlagBool = false;
	}

	/** 获取领取的行动值 */
	public int getPowerNum(){
		GuildFightGetPowerSample sample = getGuildFightGetPowerSampleBySid (GET_POWER);
		return sample.getPower; 
	}


	public string getTips(){
		if (isGuildFightOver ())
			return LanguageConfigManager.Instance.getLanguage ("GuildArea_61");
		string result = "";
		int serverTime = changeDateToSecond(ServerTimeKit.getDateTime());
		GuildFightTimeSample timeSample = getSampleBySid<GuildFightTimeSample>(FIGHT_TIME);
		int i = 0;
		int start = 0;
		int end = 0;
		foreach (GuildFightTime time in timeSample.timeList) {
			end =  changeStringToSconed(time.start) -300;
			if(start < serverTime && serverTime < end )
				break;
			i++;
			start = changeStringToSconed(time.end);
		}

		if (i == 0) {
			result = LanguageConfigManager.Instance.getLanguage("GuildArea_58");
		} else if (i == 1) {
			result = LanguageConfigManager.Instance.getLanguage("GuildArea_59");
		} else if (i == 2) {
			result = LanguageConfigManager.Instance.getLanguage("GuildArea_60");
		}
		return result;
 
	}
	public int changeStringToSconed(string str){
		return changeDateToSecond(System.DateTime.Parse(str));
	}


}

/** BUFF配置模版 */
public class GuildBuffSample :Sample
{
	/** 名称 */
	private string name;
	/** 行动值消耗 */
	private int expend;
	/** 效果值 */
	private List<int> effect = new List<int>();
	/** BUFF效果描述 */
	private string des;
	/** 激活描述 */
	private string useDes;
	/** 奖励描述 */
	private List<string> rewarDes = new List<string>();
	
	public override void parse (int sid, string str)
	{
		string [] strs = str.Split ('|');
		name = strs [1];
		expend = StringKit.toInt (strs [2]);
		parseEffect (strs [3]);
		des = strs [4];
		useDes = strs [5];
		parseRewardDes (strs[6]);
	}

	private void parseRewardDes(string str){
		string [] strs = str.Split ('#');
		foreach (string s in strs) {
			rewarDes.Add(s);
		}
	}
	private void parseEffect (string str)
	{
		if (string.IsNullOrEmpty (str))
			return;
		string [] strs = str.Split ('#');
		effect = new List<int> ();
		foreach (string s in strs) {
			effect.Add (StringKit.toInt (s));
		}
	}
	
	/** 获取BUFF描述 */
	public string getBuffDes (int num)
	{
		if (num == 0)
			return "";
		string result = des;
		for (int i =0; i <effect.Count; i++) {
			result = result.Replace ("%" + (i + 1), (num * effect [i]).ToString ());
		}
		return result;
	}
    /** 获取BUFF描述 */
    public string getGuildBuffDes(int num) {
        string result = des;
        for (int i = 0; i < effect.Count; i++) {
            result = result.Replace("%" + (i + 1), (num * effect[i]).ToString());
        }
        return result;
    }
	
	/** 获取激活BUFF描述 */
	public string getUseBuffDes (int num)
	{
		string result = useDes.Replace ("%1", expend.ToString ());
		result = result.Replace ("%2", getBuffDes (num));
		result = result.Replace ("~", "\n");
		return result;
	}

	public List<string> getRewardDes(){
		return rewarDes;
	}

    public string getRewardDesString()
    {
        string str = null;
        foreach (string s in rewarDes)
        {
            str += s;
        }
        return str;
    }

	public int getExpends ()
	{
		return expend;
	}

	public string getName ()
	{
		return name;
	}
    public List<int> getEffect()
    {
        return effect;
    }
}

/** 挑战配置模版 */
public class GuildFightChallengeSample :Sample
{
	/** 名字 */
	private string name;
	/** 消耗 */	
	private int expend;
	/** 描述 */
	private string des;

	public override void parse (int sid, string str)
	{
		string [] strs = str.Split ('|');
		name = strs [1];
		expend = StringKit.toInt (strs [2]);
		des = strs [3];
	}

	/** 行动值消耗提示*/
	public string getExpendsDes ()
	{
        return des;
	}

	/** 获取消耗 */
	public int getExpends(){
		return expend;
	}
}

/** 评价配置模版 */
public class GuildFightJudgeSample : Sample
{
	/** 名字 */
	public string name;
	public List<GuildJudge> judges = new List<GuildJudge> ();
	public override void parse (int sid, string str)
	{
		string [] strs = str.Split ('|');
		name = strs [1];
		parseJudge (strs [2]);
	}

	private void parseJudge (string str)
	{
		string [] strs = str.Split ('#');
		foreach (string s in strs) {
			GuildJudge judge = new GuildJudge ();
			string [] ss = s.Split (',');
			judge.name = ss [0];
			judge.down = StringKit.toInt (ss [1]);
			judge.up = StringKit.toInt (ss [2]);
			judges.Add (judge);
		}
	}

	public string getJudgeString (int judgeScore)
	{
		string judgeString = "";
		foreach (GuildJudge judge in judges) {
			if (judgeScore >= judge.down && judgeScore <= judge.up) {
				judgeString = judge.name;
				break;
			}
		}
		return judgeString;
	}
    public int getMaxJudgeScore()
    {
        int result = 0;
        foreach (GuildJudge judge in judges)
        {
            if (judge.up > result)
            {
                result = judge.up;
            }
        }
        return result;
    }
    public int getNextJudgeScore(int scores)
    {
        int result = getMaxJudgeScore();
        foreach (GuildJudge judge in judges)
        {
            if (judge.down > scores && judge.down < result)
            {
                result = judge.down;
            }
        }
        return result;
    }
}
public class GuildJudge
{
	/** 名字 */
	public string name;
	/** 下限 */
	public int down;
	/** 上限 */
	public int up;
}

/** 公会战参与条件模版 */
public class GuildFightJoinConditionSample :Sample{
	/** 名字 */
	public string name;
	/** 参与条件 */
	public int condition;
	/** 描述 */
	public string des;
	public override void parse (int sid, string str)
	{
		string [] strs = str.Split ('|');
		name = strs [1];
		condition = StringKit.toInt(strs [2]);
		des = strs [3];
	}
}

/** 公会战战斗时间配置 */
public class GuildFightTimeSample : Sample{
	/** 名字 */
	public string name;
	/** 各时间段 */
	public List<GuildFightTime> timeList = new List<GuildFightTime>();
	public override void parse (int sid, string str)
	{
		string [] strs = str.Split ('|');
		name = strs [1];
		parseTime (strs [2]);
	}

	private void parseTime(string str ){
		string [] strs = str.Split ('#');
		foreach (string s in strs) {
			GuildFightTime time = new GuildFightTime();
			string [] strArr = s.Split('^');
			time.start = strArr[0];
			time.end = strArr[1];
			timeList.Add(time);
		}
	}
}

public class GuildFightTime{ 
	/** 开始时间 */
	public string start;
	/** 结束时间 */
	public string end;
}

public class GuildFightOpenTimeSample :Sample{
	/** 名称 */
	public string name;
	/** 开启时间列表 */
	public List<int> openTimeList = new List<int>();
	public override void parse (int sid, string str)
	{
		base.parse (sid, str);
		string [] strs = str.Split ('|');
		name = strs [1];
		parseTimeList (strs [2]);
	}

	void parseTimeList(string str){
		string [] strs = str.Split ('#');
		foreach (string s in strs) {
			int openTime = StringKit.toInt(s);
			openTimeList.Add(openTime);
		}
	}
	/** 是否为公会战期间 */
	public bool isOpenTime(){
		System.DateTime serverDate = ServerTimeKit.getDateTime ();
		foreach (int day in openTimeList) {
			if(day == TimeKit.getWeekCHA(serverDate.DayOfWeek) ){
				return true;
			}
		}
		return false;
	}

}


public class GuildFightGetPowerSample :Sample{
	/** 名字 */
	public string name;
	/** 获取的行动值 */
	public int getPower;

	public override void parse (int sid, string str)
	{
		base.parse (sid, str);
		string [] strs = str.Split ('|');
		name = strs [1];
		getPower = StringKit.toInt (strs [2]);

	}
}

public class GuildFightReviveSample : Sample {
    /** 姓名 */
    public string name;
    /** 消耗 */
    public int cost;
    /** 描述 */
    public string expendDes;

    public override void parse(int sid, string str)
    {
        base.parse(sid, str);
        string[] strs = str.Split('|');
        name = strs[1];
        cost = StringKit.toInt(strs[2]);
        expendDes = strs[3];             
    }
}

