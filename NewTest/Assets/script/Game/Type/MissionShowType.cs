using System;
 
/**
 * 副本关卡显示类型
 * */
public class MissionShowType
{
	public MissionShowType ()
	{
	}

	public const string HIDDEN = "hidden";//隐藏
	public const string LEVEL_LOW = "missionOpenlimit";//等级不够
	public const string NEW = "missionNew";//未通关
	public const string COMPLET = "missionPass";//通关
	public const string COUNT_NOT_ENOUGH = "missionOpenlimit";//挑战次数不够
	public const string NOT_COMPLETE_LAST_MISSION="notCompleteLastMission";//未通关上一个副本
} 

