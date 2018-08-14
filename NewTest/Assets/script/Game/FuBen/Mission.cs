using System;
using System.Collections;
using System.Collections.Generic;
 
/**
 * 副本关卡实体对象
 * @author longlingquan
 * */
public class Mission
{
	public Mission (int sid)
	{
		this.sid = sid;
		createPoints ();
	} 
	
	public delegate void MissionEventCallBack (MissionEventSample e);

	public int sid = 0;//关卡sid
	public int count = -1;//副本挑战次数 -1表示无次数限制
	public MissionPointInfo[] points;//副本点集合
	public int allCostPve = 0;//副本pve总消耗 
	public bool isExit = false;
	private int p_step = 0;//当前点做的步数index, 0表示起点  起点不能配置事件  
	private int ActivEventState = 0;//0未知状态,信任前台数据, 1后台说可走 -1后台说不能走
	public bool restPointNoPvP = true;
	private CallBack<bool> gotoCallback;
	private MissionEventCallBack doEventCallback;
	private CallBack complete;
	private CallBack callbackLoss;
	private CallBack<int> doPvp;
	private Boss[] bosses;
	private int[] treasures = new int[2];//[0] gold [1]silver 宝箱数量 	
	public BattleFormationCard[] mine;//战前我方卡片信息
	public BattleFormationCard[] enemy;//战前敌方卡片信息
	public bool isPvPTimeLimit = false;//pvp是否处于事件限制中
	public int deadNum = 0; //副本结束死亡人数，用于通关评价
	public int starSum = 0;
	public bool isFirstComplete = false;//是否是第一次通关
	public int chapterID;
	public int mapID;
	public int starLevel; //当前进行的副本难度
	public int currentPracticePoint; // 副本结束的当前挑战记录
	public int historyPracticeHightPoint; // 副本结束的历史最高挑战记录


	//获得宝箱数量
	public int getTreasureNum (int type)
	{
		if (type == TreasureType.TREASURE_GOLD) {
			return treasures [0];
		} else if (type == TreasureType.TREASURE_SILVER) {
			return treasures [1];
		}
		return 0;
	}
 
	//获得副本自己卡片计血类型
	public int getMissionBloodType ()
	{
		return MissionSampleManager.Instance.getMissionSampleBySid (sid).bloodType;
	}
	
	//能否进行战斗 对当前卡片血量进行判断
	public bool isDoBattle ()
	{
		//1 代表不计血直接返回true 这里这个记血类型是从
		if (getMissionBloodType () == 1)
			return true;
		int battleType = getPlayerPoint ().getPointEvent ().battleType;
		//5人战 主力没血则不能战斗
		if (battleType == BattleType.BATTLE_FIVE) {
			for (int i = 0; i < 5; i++) {
				if (mine [i] != null) {
					if (mine [i].getHp () == -1 || mine [i].getHp () > 0)
						return true;
				}
			}		
		} else {
			for (int i = 0; i < 10; i++) {
				if (mine [i] != null) {
					if (mine [i].getHp () == -1 || mine [i].getHp () > 0)
						return true;
				}
			}
		}
		return false;
	}
	
	//初始化宝箱
	public void initTreasures (int gold, int silver)
	{
		treasures [0] = gold;
		treasures [1] = silver;
	}
	
	//添加宝箱
	public void addTreasures (int type)
	{
		if (type == TreasureType.TREASURE_GOLD) {
			treasures [0]++;
		} else if (type == TreasureType.TREASURE_SILVER) {
			treasures [1]++;
		}
	}
	
	//设置boss  
	public void setBoss (ErlArray arr)
	{ 
		if (arr == null || arr.Value.Length < 1) {
			bosses = null;
			return;
		}
		bosses = new Boss[arr.Value.Length];
		for (int i = 0; i < bosses.Length; i++) {
			int sid = StringKit.toInt ((arr.Value [i] as ErlArray).Value [0].getValueString ());
			int hp = StringKit.toInt ((arr.Value [i] as ErlArray).Value [1].getValueString ());
			int max = StringKit.toInt ((arr.Value [i] as ErlArray).Value [2].getValueString ());		
			bosses [i] = new Boss ();
			bosses [i].setHp (sid, hp, max); 
		} 
	}
	 
	public Boss getBoss (int b_sid)
	{ 
		if (bosses == null)
			return null;
		for (int i = 0; i < bosses.Length; i++) {
			if (b_sid == bosses [i].sid)
				return bosses [i];
		}
		return null;
	}
	
	public int getChapterSid ()
	{ 
		return MissionSampleManager.Instance.getMissionSampleBySid (sid).chapterSid;
	} 
	
	//获得战斗失败
	public void fightLoss (bool b)
	{ 
		getPlayerPoint ().fightLoss ();  
		if (callbackLoss != null)
			callbackLoss ();
		isExit = b;
	}

	/** 更新副本结束的历史挑战记录 */
	public void updatePracticeRecode(int currentPracticePoint,int historyPracticeHightPoint){
		this.currentPracticePoint = currentPracticePoint;
		this.historyPracticeHightPoint = historyPracticeHightPoint;
	}
	
	//更新boss血量
	public void updateBoss ()
	{ 
		FuBenGetBossFPort port = FPortManager.Instance.getFPort ("FuBenGetBossFPort") as FuBenGetBossFPort;
		port.getBoss (sid);
	}

	public int getBossSid ()
	{ 
		return MissionSampleManager.Instance .getMissionSampleBySid (sid).bossSid;
	}

	public int getRequirLevel ()
	{ 
		return MissionSampleManager.Instance .getMissionSampleBySid (sid).level;
	}	
	//关卡名字
	public string getMissionName ()
	{
		return MissionSampleManager.Instance .getMissionSampleBySid (sid).name;
	}
 	
	public PrizeSample[] getFirstPrizes ()
	{
		return MissionSampleManager.Instance .getMissionSampleBySid (sid).firstPrizes;
	}

	public PrizeSample[] getPrizes ()
	{
		return MissionSampleManager.Instance .getMissionSampleBySid (sid).prizes;
	}

	/** 根据难度取最大挑战次数（1普通，2困难，3噩梦） */
	public int getMaxNum(int _star)
	{
		return MissionSampleManager.Instance .getMissionSampleBySid (sid).num[_star - 1];
	}
	
	public void setFightLoss (CallBack callback)
	{
		callbackLoss = callback;
	}
	
	public void setComplete (CallBack callback)
	{
		this.complete = callback;	
	}
	
	public void setPvp (CallBack<int> callback)
	{
		this.doPvp = callback;
	}
	
	public void doPvP (int isOK)
	{
		if (doPvp != null) {
			doPvp (isOK);
		}
	}
	
	//更新关卡进度
	public void updateMission (int p_step, int e_step)
	{
		if (p_step != 0)
			this.p_step = p_step; 
		points [p_step].updateStep (e_step); 
	}
	  
	//获得初始地图id
	public int getInitMapId ()
	{
		return points [p_step].getMapId ();
	}
	
	//更新关卡节点 完成当前点所有事件
	public void updateMission (int  comp_p)
	{
		this.ActivEventState = comp_p;
	}
	 
	//副本完成
	public void completeMission ()
	{
		complete (); 
	}
	  
	//是否能够前往下一个点 false表示还有事件未完成
	public bool canGotoNext ()
	{


		if (ActivEventState == 1)
			return true;
		else if (ActivEventState == 0) {
			return points [p_step].isComplete ();
		} else
			return false;
	}
	 
	//前进
	public void sendGo (CallBack<bool> callback)
	{
		if (callback != null)
			gotoCallback = callback;
		FuBenGotoFPort port = FPortManager.Instance.getFPort ("FuBenGotoFPort") as FuBenGotoFPort;
		port.gotoNext (goToNextCallBack); 
	} 
	
	//前进回调函数
	private void goToNextCallBack (int sid)
	{ 
		//还有事件未完成
		
		if (sid == FuBenGotoFPort.NOT_GO) {
			//这里说明数据不对称了,需要和后台重新同步
			ActivEventState = -1;//后台断定不能走,请doEvent
			MissionManager.instance.moveForward();
		} else { 
			p_step++;
			ActivEventState = 0;

			if (sid == FuBenGotoFPort.NONE) {
				//一个休息点,没pvp事件
				restPointNoPvP = true;
				MissionInfoManager.Instance.mission.getPlayerPoint ().e_eds_null ();
				gotoCallback (true); 
				return;

			} 



			if (sid != FuBenGotoFPort.NONE) {
				restPointNoPvP = false;
			}

			//图形表现上开始前往下一个点:  missionManager.gotoNextCallback
			gotoCallback (false); 
		}
	}  
	
	//执行事件
	public void doEvent (MissionEventCallBack callback){ 
		if (callback != null)
			doEventCallback = callback; 
		 
		FuBenDoEventFPort doPort = FPortManager.Instance.getFPort ("FuBenDoEventFPort") as FuBenDoEventFPort;
		doPort.doEvent (doEvent);
	}
	
	//获得战前信息
	public void getBattleInfo (CallBack callback)
	{
		getPlayerPoint ().isBattlePrepared = true;
		FuBenGetEventInfoFPort port = FPortManager.Instance.getFPort ("FuBenGetEventInfoFPort") as FuBenGetEventInfoFPort;
		port.getInfo (callback);
	}
	
	//是否是新副本
	public bool isNew ()
	{
		if (getMissionType () == MissionShowType.NEW)
			return true;
		return false;
	} 
	  	
	//执行事件回调函数	
	public void doEvent (bool b)
	{
        if (b) {//execute_event
			MissionEventSample e = getEvent (); 
			points [p_step].gotoNextStep (); 
			doEventCallback (e); 
			doEvent (e);  
		} else {
			MissionManager.instance.moveForward ();
		}
	}
	
	//执行事件逻辑
	private void doEvent (MissionEventSample e)
	{
		if (e == null)
			return;
		//跳转点
		if (e.eventType == MissionEventType.SWITCH) {
			p_step++;
		}
		//战斗完成后讲 战斗事件 前置判定条件改为false
		if (e.eventType == MissionEventType.FIGHT || e.eventType == MissionEventType.BOSS) {
			getPlayerPoint ().isBattlePrepared = false;
        }
	} 
	  /// <summary>
	/// 获得该点的唯一ID,标识战斗用
	/// </summary>
	public long getPlayerPointOnlyID(){

	return 	StringKit.toLong( sid+"000"+getPlayerPointIndex());
	}
	//获得当前点索引
	public int getPlayerPointIndex ()
	{
		return p_step;
	} 
	
	//获得当前点
	public MissionPointInfo  getPlayerPoint ()
	{ 
		if (points == null || p_step >= points.Length)
			return null;
		return points [p_step];
	} 
	
	//获得当前事件(即将执行)
	private MissionEventSample getEvent ()
	{
		return getPlayerPoint ().getPointEvent ();
	}
	
	private int getNPCPlotSid ()
	{
		return 0;
	}
	
	public int getChapterType ()
	{
		int cSid = MissionSampleManager.Instance.getMissionSampleBySid (sid).chapterSid;
		int type = ChapterSampleManager.Instance.getChapterSampleBySid (cSid).type;
		return type;
	}
	
	//构造副本点信息
	private void createPoints ()
	{
		if(sid==-1)
			return;
		if (this.getChapterType () == ChapterType.WAR) {
			return;
		}
		string[] infos = MissionSampleManager.Instance.getMissionSampleBySid (sid).points;
		points = new MissionPointInfo[infos.Length];   
		for (int i=0; i<infos.Length; i++) {
			int[] p_loc = new int[2];
			string[] strarr = (infos [i]).Split (',');
			int p_sid = StringKit.toInt (strarr [0]);
			p_loc [0] = StringKit.toInt (strarr [1]);
			p_loc [1] = StringKit.toInt (strarr [2]);
			string[] p_ids = new string[strarr.Length - 3];
			Array.Copy (strarr, 3, p_ids, 0, p_ids.Length); 
			points [i] = new MissionPointInfo (p_sid, p_loc, p_ids);
			countMissionEventCost (points [i]);
		}
	} 
	
	//计算副本pve 总消耗
	private void countMissionEventCost (MissionPointInfo point)
	{
		string[] e_ids = point.getEventArr ();
		int max = e_ids.Length;
		for (int i = 0; i < max; i++) {
			MissionEventSample sample = MissionEventSampleManager.Instance.getMissionEventSampleBySid (StringKit.toInt (e_ids [i]));
			if (sample.costType == MissionEventCostType.COST_PVE) {
				allCostPve += sample.cost;
			}
		}
	}
	
	//获得关卡当前状态 面板中的显示方式
	public string getMissionType ()
	{
		if (count == 0)
			return MissionShowType.COUNT_NOT_ENOUGH;
		if (!FuBenManagerment.Instance.isNewMission (getChapterType (), sid)) {
			return MissionShowType.COMPLET;
		} else {  
			if (UserManager.Instance.self.getUserLevel () < MissionSampleManager.Instance.getMissionSampleBySid (sid).level) {
				return MissionShowType.LEVEL_LOW;
			} 
			if (!FuBenManagerment.Instance.isCompleteLastMission (sid)) {
				return MissionShowType.NOT_COMPLETE_LAST_MISSION;
			} 
			return MissionShowType.NEW; 
		} 
	}
	/// <summary>
	///获得玩家的下个逻辑点数据
	/// </summary>
	public MissionPointInfo GetPlayerNextPoint ()
	{
		if (p_step + 1 >= points.Length)
			return null; 
		return points [p_step + 1]; 
	}
    //得到副本一共是多少个点
    public int getMaxPoint() {
        return points.Length;
    }
    //得到完成了多少个点位数
    public int getComplatePont() {
       return p_step;
    }
    ////得到宝箱点位
    ////_event.eventType == MissionEventType.TREASURE
    public List<int> getTreasurePoint() {
        List<int> trasureList = null;
        if (points == null || points.Length < 1) return null;
        for (int i = 0; i < points.Length; i++) {
            MissionEventSample e = points[i].getPointEvent();
            //e = MissionEventSampleManager.Instance.getMissionEventSampleBySid(e_sid);
            if (e.eventType == MissionEventType.TREASURE) {
                if (trasureList == null) trasureList = new List<int>();
                trasureList.Add(i);
            }
        }
        return trasureList;
    }
	
	/// <summary>
	/// 得到给定逻辑点的下个逻辑点
	/// </summary>
	public MissionPointInfo getNextPoint (MissionPointInfo  data)
	{ 
		for (int i=0; i<points.Length; i++) {
			if (data.compare (points [i])) {
				if (i + 1 >= points.Length)
					return null;
				
				return points [i + 1];
				
			}
		}
		return null;
		
	}

	/// <summary>
	/// 得到玩家当前点的前两个点的图形点位数据(用于NPC生成)
	/// </summary>
	public MissionPointInfo getNpcStartPoint ()
	{ 
		//如果前面没有点了,那么返回第一个点
		if (p_step - 2 <= 0)
			return points [0]; 
		
		return points [p_step - 2];
	}
	/// <summary>
	/// 得到所有逻辑点位
	/// </summary>
	public MissionPointInfo[] getAllPoint ()
	{
		return points;
	}
	/// <summary>
	/// 通过index得到点位
	/// </summary>
	public MissionPointInfo getPointInfoByIndex (int index)
	{
		if (index >= points.Length)
			return null;
		return points [index];
	}

	/// <summary>
	/// 得到玩家当前点的后2个点的图形点位数据(用于NPC销毁)
	/// </summary>
	public MissionPointInfo getNpcEndPoint ()
	{ 
		//如果前面没有点了,那么返回第一个点
		if (p_step + 2 >= points.Length)
			return points [points.Length - 1];
		
		return points [p_step + 2];
	}

	public string[] getOther ()
	{
		return MissionSampleManager.Instance.getMissionSampleBySid (sid).other;
	}
}

public class Boss
{
	public int sid = 0;
	public int hp = 0;
	public int max = 0;
	
	public void setHp (int sid, int hp, int max)
	{
		this.sid = sid;
		this.hp = hp;
		this.max = max; 
	} 
	
}

