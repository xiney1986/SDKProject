using System;
using System.Collections.Generic;
 
/**
 * 副本章节信息管理器 目前只对爬塔有效果 
 * @author longlingquan
 * */
public class ChapterInfoManager
{
	//副本进度关卡sid 副本进度为单线 自增 仅对剧情副本有效
	public Chapter chapter;//当前副本信息
	public  bool[] mSettings={false,false};//自动走的两个选择状态
	public bool autoGuaji=false;//自动挂机

    public ChapterInfoManager()
	{

	}

    public static ChapterInfoManager Instance {
        get { return SingleManager.Instance.getObj("ChapterInfoManager") as ChapterInfoManager; }
	}

	//得到章节图标编号
	public int getChapterIconIdBySid (int sid)
	{
		return ChapterSampleManager.Instance.getChapterSampleBySid (sid).thumbIcon;
	}
	
	//是否是新副本 老副本不需要播放剧情
    //public bool isNewMission () 
    //{
    //    return FuBenManagerment.Instance.isNewMission (mission.getChapterType (), mission.sid); 
    //}
	public bool getPKstate(){
		return autoGuaji&&mSettings[0];
	}
	public bool getBossState(){
		return autoGuaji&&mSettings[1];
	}
	public void cleaChapter ()
	{
		chapter = null;  
	}

	public void saveChapter (int sid,int starLevel)
	{
		chapter = new Chapter (sid);
        //chapter.starLevel = starLevel;
	}
	
	//判断mission是否为空
	public bool isChapterNull ()
	{
        if (chapter == null)
			return true;
		else
			return false;
	} 
	  
	//获得指定副本 暂用
    public Chapter getChapterBysid(int sid)
	{
		return new Chapter (sid);
	}
	
	//获得当前副本信息
	public Chapter getMission ()
	{
        return chapter;
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

}  