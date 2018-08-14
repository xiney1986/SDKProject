using System;
 
/**
 * 进入副本接口
 * @author longlingquan
 * */
public class FuBenIntoFPort:BaseFPort
{
	private const string INTO = "into_fb";//成功进入副本
	private const string CONTINUE = "continue_fb";//继续副本
	private const string ALREADY = "already_in_fb";//已经在副本中
	private const string KEY = "currentfb";//继续副本 键
	private const string LIMIT = "conditions_do_not_meet";
	private const string NO_FB = "no_fb";
	private int intoSid;
	private int missionLevel;
	private CallBack<int,int> callback;
	private CallBack continueCallback;
	private CallBack callback_Practice;
	private CallBack<bool> callbackHasFB;
	private bool isPractice = false;
	
	public FuBenIntoFPort ()
	{
		
	}

	/// <summary>
	/// 进入副本--默认难度为普通=1
	/// <param name="sid">关卡sid</param>
	/// <param name="missionLevel">关卡难度</param>
	/// <param name="arrayId">队伍id</param>
	/// <param name="callback"></param>
	/// </summary>
	public void intoFuben (int sid, int missionLevel, int arrayId, CallBack<int,int> callback)
	{
		this.intoSid = sid;
		this.missionLevel = missionLevel;
		this.callback = callback;

		ErlKVMessage message = new ErlKVMessage (FrontPort.FUBEN_INTO);
		message.addValue ("fbid", new ErlInt (sid));//fuben sid
		message.addValue ("arrayid", new ErlInt (arrayId));//fuben sid
		message.addValue ("fb_lv", new ErlInt (missionLevel));//missionLevel
        MonoBase.print("intot fuben cmd=" + message.Cmd + ",jsonString=" + message.toJsonString());  
		access (message);
	}
	/// <summary>
	/// 进入修炼副本 修炼副本返回直接是继续，因为副本有保存点
	/// </summary>
	/// <param name="sid">Sid.</param>
	/// <param name="missionLevel">Mission level.</param>
	/// <param name="arrayId">Array identifier.</param>
	/// <param name="callback">Callback.</param>
	public void intoPracticeFuben (int sid, int missionLevel, int arrayId, CallBack callback)
	{
		this.intoSid = sid;
		this.missionLevel = missionLevel;
		this.continueCallback = callback;
		
		ErlKVMessage message = new ErlKVMessage (FrontPort.FUBEN_INTO);
		message.addValue ("fbid", new ErlInt (sid));//fuben sid
		message.addValue ("arrayid", new ErlInt (arrayId));//fuben sid
		message.addValue ("fb_lv", new ErlInt (missionLevel));//missionLevel
		MonoBase.print ("intot fuben cmd=" + message.Cmd + ",jsonString=" + message.toJsonString ());  
		access (message);
	}
    public void intoTowerFuben(int sid, int missionLevel, int arrayId, CallBack<int, int> callBack) {
        this.intoSid = sid;
        this.missionLevel = missionLevel;
        this.callback = callBack;

        ErlKVMessage message = new ErlKVMessage(FrontPort.FUBEN_INTO);
        message.addValue("fbid", new ErlInt(sid));//fuben sid
        message.addValue("arrayid", new ErlInt(arrayId));//fuben sid
        message.addValue("fb_lv", new ErlInt(missionLevel));//missionLevel
        MonoBase.print("intot fuben cmd=" + message.Cmd + ",jsonString=" + message.toJsonString());
        access(message);
    }
	/// <summary>
	/// 进入副本--默认难度为普通=1
	/// <param name="sid">关卡sid</param>
	/// <param name="arrayId">队伍id</param>
	/// <param name="callback"></param>
	/// </summary>
	public void intoFuben (int sid, int arrayId, CallBack<int,int> callback)
	{ 
		intoFuben (sid, 1, arrayId, callback);
	}
	
	//继续副本
	public void toContinue (CallBack callback)
	{ 
		this.continueCallback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.FUBEN_CONTINUE);  
		access (message);
	}

	//继续副本
	public void toContinue (CallBack<bool> callback)
	{ 
		this.callbackHasFB = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.FUBEN_CONTINUE);  
		access (message);
	}
	 
	public override void read (ErlKVMessage message)
	{
		string str = (message.getValue ("msg") as ErlAtom).Value;
		if (str == INTO) {
			int[] weather = MissionSampleManager.Instance.getMissionSampleBySid (intoSid).weather;
			MissionManager.weatherID = weather [UnityEngine.Random.Range (0, weather.Length - 1)];
			callback (intoSid, missionLevel); 
			intoSid = 0;
			//锁定
			ArmyManager.Instance.lockArmyID (1);
			if (GuideManager.Instance.isEqualStep (9006000) || GuideManager.Instance.isEqualStep (13005000) || GuideManager.Instance.isEqualStep (20005000)) {
				GuideManager.Instance.doGuide ();
				GuideManager.Instance.closeGuideMask ();
			}

		} else if (str == CONTINUE) { 
			ErlArray arr = message.getValue (KEY) as ErlArray;
			int p_index = StringKit.toInt (arr.Value [0].getValueString ());//继续副本 点 索引 0表示起点
			int step = StringKit.toInt (arr.Value [1].getValueString ());//点上事件索引  0表示此点上无事件或者事件都完成
			MissionInfoManager.Instance.mission.updateMission (p_index, step);
			ErlArray treasures = arr.Value [2] as ErlArray;
			int embattle = StringKit.toInt (arr.Value [3].getValueString ());
			ArmyManager.Instance.lockArmyID (embattle);
			MissionInfoManager.Instance.mission.initTreasures (StringKit.toInt (treasures.Value [0].getValueString ()),
			StringKit.toInt (treasures.Value [1].getValueString ()));

			if (continueCallback != null) {
				continueCallback (); 
				continueCallback = null;
			}
		} else if (str == ALREADY) {
			intoMission (intoSid, missionLevel);
			if (callback != null)
				callback = null;
			return;

		} else if (str == LIMIT) {
			UnityEngine.Debug.LogWarning ("mission LIMIT");
			MaskWindow.UnlockUI (true);
		} else if (str == NO_FB) {
			callbackHasFB (false);
		} else {
			MessageWindow.ShowAlert (str);
			if (callback != null)
				callback = null;
		}
	}

	/// <summary>
	/// 用PVE队伍进入指定副本
	/// </summary>
	/// <param name="sid">副本Sid.</param>
	/// <param name="missonLevel">副本难度.</param>
	public static void intoMission (int sid, int missonLevel)
	{
		intoMission (sid, missonLevel, ArmyManager.PVE_TEAMID);
	}
    /// <summary>
    /// 用PVP队伍进入爬塔副本
    /// </summary>
    /// <param name="sid"></param>
    /// <param name="missonLevel"></param>
    public static void intoTowerMission(int sid, int missonLevel) {
        intoMission(sid, missonLevel, ArmyManager.PVE_TEAMID);
    }
	/// <summary>
	/// 进入指定副本
	/// </summary>
	/// <param name="sid">副本Sid.</param>
	/// <param name="missonLevel">副本难度.</param>
	/// <param name="lockArmyId">指定队伍. -1为直接战斗</param>
	public static void intoMission (int sid, int missonLevel, int lockArmyId)
	{ 
		MissionInfoManager.Instance.saveMission (sid, missonLevel);
		FuBenManagerment.Instance.intoMission (sid, missonLevel);
		ArmyManager.Instance.lockArmyID (lockArmyId);
		if (GuideManager.Instance.guideSid <= GuideGlobal.SPECIALSID1)
			UiManager.Instance.switchWindow<EmptyWindow> ((win) => {
				ScreenManager.Instance.loadScreen (4, null, () => {
					UiManager.Instance.switchWindow<MissionMainWindow> ((win2) => {
						GuideManager.Instance.guideEvent ();
					});});
			});
		else {


			int type = -1;
			if (MissionInfoManager.Instance.mission != null) {
				type = MissionInfoManager.Instance.mission.getChapterType ();
			}
			//限时活动副本回来的时候可能已经过期,所以进入前移除这个窗口
			if (type == ChapterType.ACTIVITY_CARD_EXP || type == ChapterType.ACTIVITY_EQUIP_EXP
				|| type == ChapterType.ACTIVITY_MONEY || type == ChapterType.ACTIVITY_SKILL_EXP) {
				UiManager.Instance.destoryWindowByName ("ActivityChooseWindow");
			}
 
			UiManager.Instance.switchWindow<EmptyWindow> ((win) => {
				ScreenManager.Instance.loadScreen (4, null, () => {
					UiManager.Instance.switchWindow<MissionMainWindow> ();});
			});
		}
	}
}

