using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeroRoad 
{
	public enum State
	{
		COMPLETED,//完成
		ACTIVE,//可进行
		WAIT,//已完成激活次数
	}

	public HeroRoadSample sample;
	public int activeCount; //激活次数
	public int conquestCount; //征服次数

	public bool isDirectFight()
	{
		ChapterSample cs = ChapterSampleManager.Instance.getChapterSampleBySid (sample.chapter);
		int missionSid = cs.missions[Mathf.Min(conquestCount,cs.missions.Length - 1)];
		MissionSample mission = MissionSampleManager.Instance.getMissionSampleBySid (missionSid);
		return StringKit.toInt(mission.other [0]) == 1;
	}

	public MissionSample[] getMissionsByChapter()
	{
		ChapterSample cs = ChapterSampleManager.Instance.getChapterSampleBySid (sample.chapter);
		if (cs.missions == null) return null;
		MissionSample[] missionSamples = new MissionSample[cs.missions.Length];
		for(int i=0;i<missionSamples.Length;i++)
		{
			MissionSample missionSample = MissionSampleManager.Instance.getMissionSampleBySid (cs.missions[i]);
			missionSamples[i]=missionSample;
		}
		return missionSamples;
	}

	//获取某英雄之章的觉醒情况
	//-1无觉醒,0未觉醒,1已觉醒
	public int[] getAwakeInfo()
	{
		
		int[] missions = ChapterSampleManager.Instance.getChapterSampleBySid (sample.chapter).missions;
		int[] result = new int[missions.Length];
		for (int i = 0; i < missions.Length; i++) {
			MissionSample mission = MissionSampleManager.Instance.getMissionSampleBySid (missions[i]);
			if(StringKit.toInt(mission.other[1]) == 0)
				result[i] = -1;
			else if(StringKit.toInt(mission.other[1]) == 1 && conquestCount > i)
				result[i] = 1;
			else
				result[i] = 0;
			
		}
		return result;
	}

	//暂时这么处理，根据进化等级来判断觉醒信息
	public int[] getAwakeInfo(int evoLevel)
	{
		int[] missions = ChapterSampleManager.Instance.getChapterSampleBySid (sample.chapter).missions;
		int[] result = new int[missions.Length];
		for (int i = 0; i < missions.Length; i++) {
			MissionSample mission = MissionSampleManager.Instance.getMissionSampleBySid (missions[i]);
			if(StringKit.toInt(mission.other[1]) == 0)
				result[i] = -1;
			else if(StringKit.toInt(mission.other[1]) == 1 && evoLevel > i)
				result[i] = 1;
			else
				result[i] = 0;
			
		}
		return result;
	}

    public string getAwakeString(int index)
    {
        int[] missions = ChapterSampleManager.Instance.getChapterSampleBySid(sample.chapter).missions;
        MissionSample mission = MissionSampleManager.Instance.getMissionSampleBySid(missions[index]);
        if (mission != null)
            return mission.other[3];
        return null;
    }

	public State getState()
	{
		if (conquestCount < activeCount)
			return State.ACTIVE;
		else if (conquestCount >= sample.getMissionCount ())
			return State.COMPLETED;
		else
			return State.WAIT;
	}
}
