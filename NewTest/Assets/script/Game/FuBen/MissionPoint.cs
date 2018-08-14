using UnityEngine;
using System.Collections;

/// <summary>
/// 点位的显示数据类
/// </summary>
public class MissionPoint
{
	//注意:请勿用图形点位反推出数据点,一个图像点可能多个数据点在用,无法反推
	public Vector3 woldPos
	{
		get
		{
			if(_woldPos==Vector3.zero)
			{
				_woldPos=MissionManager.instance.missionRoad.M_getPosition(pointIndex+1);
			}
			return _woldPos;
		}set
		{
		}
	}
	//当前的点位 在当前路线中是否存在，主要用于判断npc是否已经超出路线，如果是 则移除npc.(因为路线不会一次性构建完)
	public bool pointOnRoad
	{
		get
		{
			if(MissionManager.instance==null)
				return false;
			if(MissionManager.instance.missionRoad==null)
				return false;
			int roadStartStep=MissionManager.instance.missionRoad.startStep;
			int currentMaxStep=MissionManager.instance.missionRoad.currentLastStep;
			return (pointIndex+1>=roadStartStep)&&(pointIndex+1<=currentMaxStep);
		}

	}
	private Vector3 _woldPos=Vector3.zero; //点的世界坐标
	public Vector3 localPosition; //点的本地坐标
	public int[]  loc;//通过这个来和pointinfo做比较,是否在同一个点上
	public MissionPointCtrl pointCtrl;//点位显示物体
	public GameObject bgObj;//点位上的背景物体
	public MissionEventSample parentEvent;
	public EventObjCtrl eventObj;//点位上的事件图标 
	public int pointIndex;//点位在整个路径上的索引
}
