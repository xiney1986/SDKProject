using System;
using System.Collections.Generic;

/**
 * 副本点数据实体对象
 * @author longlingquan
 * */
using UnityEngine;


public class MissionPointInfo
{
	public int sid = 0;//sid
	private int[] loction ;//位置坐标
	private string[] e_ids ;//事件点sid集合
	private MissionEventSample e;//当前点事件
	public MissionPoint PointInfo;
	public int step = 1;//点事件步骤 起始setp为1 
	public bool isBattlePrepared = false;//战斗前置 判断条件  如果是战斗事件  则需要先执行获取战前信息 当battleEvent=true 才能执行战斗事件 执行完战斗事件后需要重置为false

	/// <summary>
	/// 比较两个MissionPointInfo是否是相同的
	/// </summary>
	public bool  compare (MissionPointInfo data)
	{
		if (data.loction [0] == loction [0] && data.loction [1] == loction [1])
			return true;

		return false;
	}

	public MissionPointInfo (int sid, int[] loction, string[] ids)
	{
		this.sid = sid; 
		this.loction = loction;
		this.e_ids = ids;
	}

	
	//获得当前点全部事件
	public string[] getEventArr ()
	{
		return e_ids;
	}
	/// <summary>
	/// 当前点可否自动行走
	/// </summary>
	public bool canAutoMove ()
	{
		if(MissionInfoManager.Instance.autoGuaji)return true;
		//起始点不自动走
		if (MissionInfoManager.Instance.mission.points [0] == this)
			return false;

		//刚进来也不自动走
		if (MissionManager.userState == MissionManager.MISSION_NEWIN)
			return false;

		//点位完成了并且不是从战斗回来才自动走
		if (isComplete () && MissionManager.userState != MissionManager.MISSION_BATTELRETURN) {

			return true;
		} else {
			//战斗回来,点事件没完成,而且下个事件是对话,那么自动行走
			if (MissionManager.userState == MissionManager.MISSION_BATTELRETURN && getPointEvent () != null && getPointEvent ().eventType == MissionEventType.PLOT) {
				MissionManager.userState =0;
				return true;
			}

			//点事件没完成,而且下个事件是战斗,那么自动行走
			if (MissionManager.userState != MissionManager.MISSION_BATTELRETURN && getPointEvent () != null && getPointEvent ().eventType == MissionEventType.FIGHT) {
				//MissionManager.userState =0;
				return true;
			}

			//捡完宝箱后，如果点事件没完成,而且下个事件是对话,那么自动行走
			 if (MissionManager.userState == MissionManager.MISSION_GETAWARD && getPointEvent () != null && getPointEvent ().eventType == MissionEventType.PLOT) {
				MissionManager.userState =0;
				return true;
			}

		}
		return false;
	}

	public bool isComplete ()
	{
		if (step == 0 || e_ids == null || e_ids.Length < 1)
			return true;
		if (getPointEvent () == null || getPointEvent ().eventType == MissionEventType.NONE)
			return true;
		return false;
	}
	
	//是否是剧情事件
	private bool isPlot (int e_sid)
	{
		MissionEventSample e = MissionEventSampleManager.Instance.getMissionEventSampleBySid (e_sid);
		if (e == null)
			return false;
		if (e.eventType == MissionEventType.PLOT)  
			return true; 
		return false;
	} 
	
	// 是否需要立即执行事件
	public bool isNeedDoEventImmediately ()
	{
		//第一个事件将被立即执行
		if (e_ids != null && e_ids.Length > 0 && step == 1)
			return true;
		return false;
	}
	
	//能否执行战斗事件
	public bool isDoBattleEvent ()
	{
		MissionEventSample e = getPointEvent ();
		if (e.eventType == MissionEventType.FIGHT || e.eventType == MissionEventType.BOSS) {
			return isBattlePrepared;
		} else {
			return true;
		}
	}
	
	//接收后台消息点上没有事件，e_ids=null
	public void e_eds_null ()
	{
		this.e_ids = null;
	}
	
	public void updateStep (int step)
	{ 
		this.step = step;
	}
	
	//战斗失败
	public void fightLoss ()
	{
		step--;
	}
  
	//获得当前点  事件
	public MissionEventSample getPointEvent ()
	{ 
		//步骤如果为0表示 当前点无事件
		if (step == 0)
			return null;
		int index = step - 1;
		if (e_ids == null || index >= e_ids.Length)
			return null;
		int e_sid = StringKit.toInt (e_ids [index]);
		e = MissionEventSampleManager.Instance.getMissionEventSampleBySid (e_sid);
		return e;
	}

  
	//获得下一个事件步骤 如果无 返回0
	public void gotoNextStep ()
	{
		if (e_ids == null || e_ids.Length < 1) {
			step = 0;
			return;
		} 
		step++;
		if (step - 1 > e_ids.Length) {
			step = 0; 
		} 
		//NEW
		//MissionManager.instance.missionRoad.M_goStep(step,onCreatCmp);
	} 
	private void onCreatCmp()
	{
		int playCurrentStep=MissionInfoManager.Instance.mission.getPlayerPointIndex();
		Vector3 rotation=MissionManager.instance.missionRoad.M_calculateSegmentRotatioin(playCurrentStep);
		GameObject targetCamera= MissionManager.instance.roleCamearT.gameObject;
		iTween.RotateTo(targetCamera,rotation, 0.5f);
	}

	//获得当前点位置坐标
	public int[] getPointLoction ()
	{
		return this.loction;
	} 
	
	//获得点所属地图id
	public int getMapId ()
	{ 
		return MissionPointSampleManager.Instance.getMissionPointSampleBySid (sid).mapId; 
	}
	
	//战斗背景
	public int getBattleBg ()
	{
		return MissionPointSampleManager.Instance.getMissionPointSampleBySid (sid).battleBg;
	}
	
	//获得当前点背景信息
	public int getBgId ()
	{
		return MissionPointSampleManager.Instance.getMissionPointSampleBySid (sid).bgId;
	}
	
} 

