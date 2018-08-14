using System;
using System.Collections.Generic;
 
/**
 * 副本关卡信息管理器 
 * @author longlingquan
 * */
public class MissionInfoManager
{
	//副本进度关卡sid 副本进度为单线 自增 仅对剧情副本有效
	public Mission mission;//当前副本信息
	public  bool[] mSettings={false,false};//自动走的两个选择状态
	public bool autoGuaji=false;//自动挂机
    public bool isBossFight = false;

	public MissionInfoManager ()
	{

	}
	
	public static MissionInfoManager Instance {
		get{return SingleManager.Instance.getObj("MissionInfoManager") as MissionInfoManager;}
	}

	//得到章节图标编号
	public int getChapterIconIdBySid (int sid)
	{
		return ChapterSampleManager.Instance.getChapterSampleBySid (sid).thumbIcon;
	}
	
	//是否是新副本 老副本不需要播放剧情
	public bool isNewMission () 
	{
		return FuBenManagerment.Instance.isNewMission (mission.getChapterType (), mission.sid); 
	}
	public bool getPKstate(){
		return autoGuaji&&mSettings[0];
	}
	public bool getBossState(){
		return autoGuaji&&mSettings[1];
	}
	public void clearMission ()
	{
		mission = null;  
	}

	public void saveMission (int sid,int starLevel)
	{
		mission = new Mission (sid);
		mission.starLevel = starLevel;
	}
	
	//判断mission是否为空
	public bool isMissionNull ()
	{
		if (mission == null)
			return true;
		else
			return false;
	} 
	  
	//获得指定副本 暂用
	public Mission getMissionBySid (int sid)
	{
		return new Mission (sid);
	}
	
	//获得当前副本信息
	public Mission getMission ()
	{
		return mission;
	}
 
	/// <summary>
	/// 获得副本详细名字 格式： 天秤座 第1节【困难】
	/// </summary>
	/// <returns>The mission detail name.</returns>
	/// <param name="_sid">_sid.</param>
	public string getMissionDetailName(int sid,int difficulty)
	{
		Mission mission=getMissionBySid(sid);
		if(mission.getChapterType()!=ChapterType.STORY)
		{
			return mission.getMissionName();
		}

		string name=mission.getMissionName();
		char[] chars=name.ToCharArray();
		int length=chars.Length;
		bool lastIsNumber=char.IsNumber(chars[length-1]);
		bool lastTwoIsNumber=char.IsNumber(chars[length-2]);

		string numStr=string.Empty;
		string nameStr=string.Empty;
		string diffStr=string.Empty;

		length=name.Length;
		if(lastTwoIsNumber)
		{
			nameStr=name.Substring(0,length-2);
			numStr=name.Substring(length-2,2);
		}else if(lastIsNumber)
		{
			nameStr=name.Substring(0,length-1);
			numStr=name.Substring(length-1,1);
		}
		switch(difficulty)
		{
			case 1:
				diffStr=LanguageConfigManager.Instance.getLanguage("s0450");
				break;
			case 2:
				diffStr=LanguageConfigManager.Instance.getLanguage("s0451");
				break;
			case 3:
				diffStr=LanguageConfigManager.Instance.getLanguage("s0452");
				break;
			default:
				diffStr=LanguageConfigManager.Instance.getLanguage("s0450");
				break;
		}
		diffStr=diffStr.Substring(0,2);
		string missionName=LanguageConfigManager.Instance.getLanguage("sweepMissionName",nameStr,numStr,diffStr);
		return missionName;
	}
	/// <summary>
	/// 获得副本详细名字 格式： 天秤座 第1节【困难】 为数组
	/// </summary>
	/// <returns>The mission detail name.</returns>
	/// <param name="_sid">_sid.</param>
	public string[] getMissionDetailNameforFuben(int sid,int difficulty)
	{
		Mission mission=getMissionBySid(sid);
		string[] missionName;
		if(mission.getChapterType()!=ChapterType.STORY)
		{
			missionName=new string[1];
			missionName[0]=mission.getMissionName();
			return missionName;
		}
		
		string name=mission.getMissionName();
		char[] chars=name.ToCharArray();
		int length=chars.Length;
		bool lastIsNumber=char.IsNumber(chars[length-1]);
		bool lastTwoIsNumber=char.IsNumber(chars[length-2]);
		
		string numStr=string.Empty;
		string nameStr=string.Empty;
		string diffStr=string.Empty;
		
		length=name.Length;
		if(lastTwoIsNumber)
		{
			nameStr=name.Substring(0,length-2);
			numStr=name.Substring(length-2,2);
		}else if(lastIsNumber)
		{
			nameStr=name.Substring(0,length-1);
			numStr=name.Substring(length-1,1);
		}
		switch(difficulty)
		{
		case 1:
			diffStr=LanguageConfigManager.Instance.getLanguage("s0450");
			break;
		case 2:
			diffStr=LanguageConfigManager.Instance.getLanguage("s0451");
			break;
		case 3:
			diffStr=LanguageConfigManager.Instance.getLanguage("s0452");
			break;
		default:
			diffStr=LanguageConfigManager.Instance.getLanguage("s0450");
			break;
		}
		diffStr=diffStr.Substring(0,2);
		missionName=new string[3];
		missionName[0]=nameStr;
		missionName[1]=LanguageConfigManager.Instance.getLanguage("sweepMissionName1",numStr);
		missionName[2]=LanguageConfigManager.Instance.getLanguage("sweepMissionName2",diffStr);
		return missionName;
	}


	//是否是最新章节
	public bool isLatest (int sid)
	{
		int[] missions = ChapterSampleManager.Instance.getChapterSampleBySid (sid).missions;
		if (missions [0] <= sid && missions [1] >= sid) {
			return true;
		} else {
			return false;
		}
	}
    public bool isTowerFuben() {
        if (mission == null) return false;
        int chapterSid=MissionSampleManager.Instance.getMissionSampleBySid(mission.sid).chapterSid;
        return ChapterSampleManager.Instance.getChapterSampleBySid(chapterSid).type == ChapterType.TOWER_FUBEN;
    }

}  