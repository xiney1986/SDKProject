using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class TaskManagerment 
{
	private List<Task> array;
	//领取行军时是否提示
	public bool isTips = false;
	
	public TaskManagerment ()
	{ 

	}
	
	public static TaskManagerment Instance {
		get{return SingleManager.Instance.getObj("TaskManagerment") as TaskManagerment;}
	} 
	
	//初始化所有任务信息
	public void createAllTask ()
	{
		int[] sids = TaskSampleManager.Instance.getAllTask();
		array = new List<Task>();
		for (int i = 0; i < sids.Length; i++) {
			array.Add(new Task (sids [i]));	
		}
	}
	
	//更新任务信息
	public void updateAllTask (ErlArray erlarr)
	{
		if (array == null || array.Count < 1)
			createAllTask ();
		for (int i = 0; i < erlarr.Value.Length; i++) {
			updateTask (erlarr.Value [i] as ErlArray);
		}
	}
	
	//更新任务信息
	private void updateTask (ErlArray erlarr)
	{ 
		if (array == null || array.Count < 1)
			throw new Exception (GetType () + "updateLuckyDraw error! array is null");
		if (erlarr == null || erlarr.Value.Length < 1)
			return;
		for (int i=0; i<array.Count; i++) {
			int sid = array [i].sid;
			int _sid = StringKit.toInt (erlarr.Value [0].getValueString ());
			if (sid == _sid) {
				if(StringKit.toInt((erlarr.Value [3] as ErlType).getValueString())==1)
				{
					array.Remove(array[i]);
					return;
				}
				int index = StringKit.toInt (erlarr.Value [1].getValueString ());
				array [i].updateProgress (index);
				ErlArray arr = erlarr.Value [2] as ErlArray;
				if (arr.Value.Length != 0) {
					array [i].updateConditionKey(StringKit.toLong(arr.Value [0].getValueString()));
				}
				return;
			}
		}
	}
	
	public void completeTask(int _sid)
	{
		for (int i=0; i<array.Count; i++) {
			int sid = array [i].sid;
			if (sid == _sid) {
				if(array[i].index+1>TaskSampleManager.Instance.getTaskSampleBySid(sid).condition.conditions.Length)
				{
					array.Remove(array[i]);
					return;
				}
				array [i].updateProgress (array[i].index+1);
				return;
			}
		}
	}
	
	//获得可领取奖励的任务数量
	public int getCompleteTaskNum()
	{
		int num = 0;
		if(array == null)
		{
			return num;
		}
		TaskSample ts;
		TaskSampleManager tmpManager = TaskSampleManager.Instance;
		for (int i = 0; i < array.Count; i++) {
			if(TaskSampleManager.Instance.getTaskSampleBySid(array[i].sid).showLv <= UserManager.Instance.self.getUserLevel())
			{
				ts = tmpManager.getTaskSampleBySid(array[i].sid);
				if(isComplete(array[i]) && ts.condition.key != TaskConditionType.DAILYREBATE)
				{
					num ++;
				}
			}
		}
		return num;
	}
	
	public string setCompleteProShow(Task task)
	{
		TaskSample ts = TaskSampleManager.Instance.getTaskSampleBySid(task.sid);
		if (ts.condition.key == TaskConditionType.LEVEL) {
			return UserManager.Instance.self.getUserLevel () + "/" + ts.condition.conditions [task.index - 1];
		}
		else if (ts.condition.key == TaskConditionType.MAINCARDLV) {
			return StorageManagerment.Instance.getRole (UserManager.Instance.self.mainCardUid).getLevel () + "/" + ts.condition.conditions [task.index - 1];
		}
		else if (ts.condition.key == TaskConditionType.BEASTLV) {
			int temp = 0;
			ArrayList list = StorageManagerment.Instance.getAllBeast ();
			for (int k = 0; k < list.Count; k++) {
				if ((list [k] as Card).getLevel () > temp) {
					temp = (list [k] as Card).getLevel ();
				}
			}
			return temp + "/" + ts.condition.conditions [task.index - 1];
		}
		else if (ts.condition.key == TaskConditionType.CARDSKILLNUM) {
			int temp = -1;
			ArrayList list = StorageManagerment.Instance.getAllRole ();
			Card card;
			for (int k = 0; k < list.Count; k++) {
				card = list [k] as Card;
				if (card.getSkillNum () > temp) {
					temp = card.getSkillNum ();
				}
			}
			return temp + "/" + ts.condition.conditions [task.index - 1];
		}
		else if (ts.condition.key == TaskConditionType.PURPLEBEAST) {
			return task.condition + "/" + ts.condition.conditions [task.index - 1];
		}
		else if (ts.condition.key == TaskConditionType.NUMOFFRIENDS) {
			return FriendsManagerment.Instance.getFriendAmount () + "/" + ts.condition.conditions [task.index - 1]; 
		}
		else if (ts.condition.key == TaskConditionType.ORANGEBEAST) {
			return task.condition + "/" + ts.condition.conditions [task.index - 1];
		}
		else if (ts.condition.key == TaskConditionType.CUMLOADING) {
			return TotalLoginManagerment.Instance.getTotalDay () + "/" + ts.condition.conditions [task.index - 1];
		}
		else if (ts.condition.key == TaskConditionType.GRADEVIP) {
			return UserManager.Instance.self.vipLevel + "/" + ts.condition.conditions [task.index - 1];
		}
		else if (ts.condition.key == TaskConditionType.DRAMAFBPRO) {
			ChapterSample chapter = ChapterSampleManager.Instance.getChapterSampleBySid (MissionSampleManager.Instance.getMissionSampleBySid (ts.condition.conditions [task.index - 1]).chapterSid);
			if (chapter != null) {
                int num = 0;
                int[][] storyInfos = FuBenManagerment.Instance.getStoryInfos();
                int[] missionList = chapter.missions;
                for (int i = 0; i < storyInfos.Length; i++) {
                    for (int j = 0; j < missionList.Length; j++) {
                        if (storyInfos[i][0] == missionList[j]) {
                            num++;
                        }
                    }
                }
                return num + "/" + chapter.missions.Length;
                //return FuBenManagerment.Instance.getMyMissionStarNum(chapter.sid) + "/" + FuBenManagerment.Instance.getAllMissionStarNum(chapter.sid);
			}
			return "";
		}
		else if (ts.condition.key == TaskConditionType.CRUSADEFBPRO) {//主线任务

			return  "";
		}
		else if (ts.condition.key == TaskConditionType.DAILYREBATE) {
			if (task.index <= ts.condition.conditions.Length / 2)
				return 	"(" + task.condition + "/" + ts.condition.conditions [(task.index - 1) * 2] + ")";
			return "(" + task.condition + "/" + ts.condition.conditions [ts.condition.conditions.Length - 2] + ")";
		}
		else if (ts.condition.key == TaskConditionType.STARSHINE) {
			return GoddessAstrolabeManagerment.Instance.getAllStartNum() + "/" + ts.condition.conditions[task.index-1];
		}
		else
		{
			return task.condition + "/" + ts.condition.conditions[task.index-1];
		}
	}
	
	public bool isComplete(Task task)
	{
		TaskSample ts = TaskSampleManager.Instance.getTaskSampleBySid(task.sid);
		if (ts.condition.key == TaskConditionType.LEVEL) {
			if (ts.condition.conditions [task.index - 1] <= UserManager.Instance.self.getUserLevel ()) {
				return true;
			}
		}
		else if (ts.condition.key == TaskConditionType.NUMOFFRIENDS) {
			if (FriendsManagerment.Instance.getFriendAmount () >= ts.condition.conditions [task.index - 1]) {
				return true;
			}
		}
//		else if(ts.condition.key == TaskConditionType.TOWER){
//			if(ClmbTowerManagerment.Instance)
//		}
		else if (ts.condition.key == TaskConditionType.CUMLOADING) {
			if (TotalLoginManagerment.Instance.getTotalDay () >= ts.condition.conditions [task.index - 1]) {
				return true;
			}
		}
		else if (ts.condition.key == TaskConditionType.MAINCARDLV) {
			if (ts.condition.conditions [task.index - 1] <= StorageManagerment.Instance.getRole (UserManager.Instance.self.mainCardUid).getLevel ()) {
				return true;
			}
		}
		else if (ts.condition.key == TaskConditionType.ORANGEBEAST) {
			if (task.condition >= ts.condition.conditions [task.index - 1]) {
				return true;
			}
		}
		else if (ts.condition.key == TaskConditionType.PURPLEBEAST) {
			if (task.condition >= ts.condition.conditions [task.index - 1]) {
				return true;
			}
		}
		else if (ts.condition.key == TaskConditionType.BEASTLV) {
			ArrayList list = StorageManagerment.Instance.getAllBeast ();
			for (int k = 0; k < list.Count; k++) {
				if ((list [k] as Card).getLevel () >= ts.condition.conditions [task.index - 1]) {
					return true;
				}
			}
		}
		else if (ts.condition.key == TaskConditionType.CARDSKILLNUM) {
			ArrayList list = StorageManagerment.Instance.getAllRole ();
			for (int k = 0; k < list.Count; k++) {
				if ((list [k] as Card).getSkills () != null) {
					int skillNum = 0;
					int attrSkillNum = 0;
					int buffSkillNum = 0;
					if ((list [k] as Card).getSkills () != null) {
						skillNum = (StorageManagerment.Instance.getAllRole () [k] as Card).getSkills ().Length;
					}
					if ((list [k] as Card).getAttrSkills () != null) {
						attrSkillNum = (StorageManagerment.Instance.getAllRole () [k] as Card).getAttrSkills ().Length;
					}
					if ((list [k] as Card).getBuffSkills () != null) {
						buffSkillNum = (StorageManagerment.Instance.getAllRole () [k] as Card).getBuffSkills ().Length;
					}
					if ((skillNum + attrSkillNum + buffSkillNum) >= ts.condition.conditions [task.index - 1]) {
						return true;
					}
				}
			}
		}
		else if (ts.condition.key == TaskConditionType.GRADEVIP) {
			if (UserManager.Instance.self.vipLevel >= ts.condition.conditions [task.index - 1]) {
				return true;
			}
		}
		else if (ts.condition.key == TaskConditionType.DRAMAFBPRO) {
			int[][] storeInfo = FuBenManagerment.Instance.getStoryInfos ();
			if (storeInfo != null && storeInfo.Length > 0) {
				for (int i=0; i<storeInfo.Length; i++) {
					if (storeInfo [i] [0] >= ts.condition.conditions [task.index - 1]) {
						return true;
					}
				}
				return false;
				//return storeInfo [storeInfo.Length - 1] [0] >= ts.condition.conditions [task.index - 1];
			}
		}
		else if (ts.condition.key == TaskConditionType.CRUSADEFBPRO) {
			int[] warInfo = FuBenManagerment.Instance.getWarInfos ();
			if (warInfo != null && warInfo.Length > 0) {
				for (int i=0; i<warInfo.Length; i++) {
					if (warInfo [i] >= ts.condition.conditions [task.index - 1]) {
						return true;
					}
				}
				return false;
				//return warInfo [warInfo.Length - 1] >= ts.condition.conditions [task.index - 1];
			}
		}
		else if (ts.condition.key == TaskConditionType.DAILYREBATE) {
			if (task.index <= ts.condition.conditions.Length / 2) {
				if (task.condition >= ts.condition.conditions [(task.index - 1) * 2] && UserManager.Instance.self.getVipLevel () >= ts.condition.conditions [(task.index - 1) * 2 + 1])
					return true;
				else
					return false;
			}
			//else
			//	return true;
		}
		else if (ts.condition.key == TaskConditionType.STARSHINE) {
			if(task.index == 0)
				return false;
			if(GoddessAstrolabeManagerment.Instance.getAllStartNum() >= ts.condition.conditions[task.index -1])
				return true;
			else
				return false;
		}
		else 
		{
			if(task.index==0)
				return false;

			if((ts.condition.conditions[task.index-1] <= task.condition))
			{
				return true;
			}
		}
		
		return false;
	}
	
	
	//获得任务信息
	public List<Task> getTaskArr ()
	{ 
		return array;
	}

	/// <summary>
	/// 获得每日任务未完成的数量
	/// </summary>
	public int getEveryDayTaskNoCompleteCount () {
		Task[] tasks = getEveryDayTask (UserManager.Instance.self.getUserLevel ());
		int count = 0;
		for (int i = 0; i < tasks.Length; i++) {
			if (!isComplete (tasks[i])) 
				count++;
		}
		return count;
	}

	//获得每日任务完成的数量
	public int getEveryDayTaskedNum()
	{
		int num = 0;
		Task[] tasks = getEveryDayTask(UserManager.Instance.self.getUserLevel());
		for (int i = 0; i < tasks.Length; i++) {
			if(isComplete(tasks[i]))
			{
				num ++;
			}
		}
		return num;
	}
	//获得每日任务（符合玩家等级条件）
	public Task[] getEveryDayTask(int level)
	{
		if(array == null)
		{
			return null;
		}
		List<Task> list = new List<Task>();
		for (int i = 0; i < array.Count; i++) {
			TaskSample sample = TaskSampleManager.Instance.getTaskSampleBySid(array[i].sid);
			if(sample.taskType == 1 && sample.showLv <= level)
			{
				list.Add(array[i]);
			}
		}
		List<Task> completeList = new List<Task>();
		int count = list.Count;
		List<Task> temp = new List<Task>();
		for (int i = 0; i < count; i++) {
			if(isComplete(list[i]))
			{
				completeList.Add(list[i]);
			}
			else
			{
				temp.Add(list[i]);
			}
		}
		ListKit.AddRange(completeList,temp);
		return completeList.ToArray();
	}
	//获得每日任务完成的数量
	public int getMainLineTaskedNum()
	{
		int num = 0;
		for (int i = 0; i < getMainLineTask(UserManager.Instance.self.getUserLevel()).Length; i++) {
			if(isComplete(getMainLineTask(UserManager.Instance.self.getUserLevel())[i]))
			{
				num ++;
			}
		}
		return num;
	}
	//获得主线任务（符合玩家等级条件）
	public Task[] getMainLineTask(int level)
	{
		if(array == null)
		{
			return null;
		}
		List<Task> list = new List<Task>();
		for (int i = 0; i < array.Count; i++) {
			TaskSample sample = TaskSampleManager.Instance.getTaskSampleBySid(array[i].sid);
		
			if(sample.taskType == 0)
			{
				if(sample.showLv <= level)
				{
					list.Add(array[i]);
				}
			}
		}
		List<Task> completeList = new List<Task>();
		int count = list.Count;
		List<Task> temp = new List<Task>();
		List<Task> maintakss = new List<Task>();
		for (int i = 0; i < count; i++) {
			if(list[i].sid==104003){
				maintakss.Add(list[i]);
			}
		}
		for (int i = 0; i < count; i++) {
			if(list[i].sid!=104003){
				if(isComplete(list[i]))
				{
					completeList.Add(list[i]);
				}
				else
				{
					temp.Add(list[i]);
				}
			}
		}
 		ListKit.AddRange(maintakss,completeList);
		ListKit.AddRange(maintakss,temp);
		return maintakss.ToArray();
	}
	
	/// <summary>
	/// 获得每日返利任务
	/// </summary>
	public ArrayList getDailyRebateTask()
	{
		if(array == null)
		{
			return null;
		}
		ArrayList list = new ArrayList();
		for (int i = 0; i < array.Count; i++) {
			TaskSample sample = TaskSampleManager.Instance.getTaskSampleBySid(array[i].sid);
			if(sample.taskType == 3 && UserManager.Instance.self.getUserLevel() >= sample.showLv)
			{
				list.Add(array[i]);
			}
		}
		return list;
	}
}
