using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DailyRebateContent : dynamicContent
{
	public GameObject dailyItemPrefab;
	public GameObject arrowObj;
	ArrayList dailyList;
	NoticeSample noticeSample;
	NoticeWindow Noticewin;
	public void Initialize (NoticeWindow win, Notice notice, ArrayList _list)
	{
		noticeSample = notice.getSample ();
		Noticewin = win;
		dailyList = _list;
		if (dailyList.Count > 3)
			arrowObj.SetActive (true);
		else
			arrowObj.SetActive (false);
		if(noticeSample != null && noticeSample.type == NoticeType.DAILY_REBATE)
		{
			base.reLoad (dailyList.Count,getDailyRebateAwardIndex());
		}
		else 
		{
			base.reLoad (dailyList.Count); 
		}
		//base.reLoad (dailyList.Count); 
	}

	public void m_reLoad()
	{
		if(noticeSample != null && noticeSample.type == NoticeType.DAILY_REBATE)
		{
			base.reLoad (dailyList.Count,getDailyRebateAwardIndex());
		}
	}

	public override void updateItem (GameObject item, int index)
	{
		DailyRebateItem dailyItem = item.GetComponent<DailyRebateItem> ();
		//dailyItem.initialize (dailyList [index] as Task,Noticewin,this);
		dailyItem.updateTask (dailyList [index] as Task);
	}
	
	public override void initButton (int  i)
	{
		if (nodeList [i] == null) {
			nodeList [i] = NGUITools.AddChild (gameObject, dailyItemPrefab);

			DailyRebateItem dailyItem = nodeList[i].GetComponent<DailyRebateItem>();
			dailyItem.initialize(dailyList [i] as Task,Noticewin,this);
		}
	}
	
	public void updateAllItems()
	{
		for (int i=0; i<nodeList.Count; i++)
			updateItem (nodeList [i], i);
	}
	public void updateDailyRebateContent()
	{
		dailyList = TaskManagerment.Instance.getDailyRebateTask ();
		base.reLoad (dailyList.Count); 
		if (dailyList.Count > 3)
			arrowObj.SetActive (true);
		else
			arrowObj.SetActive (false);
	}
	void Start()
	{
		if (dailyList != null)
			updateAllItems ();
	}
	public int getDailyRebateAwardIndex()
	{
		if(dailyList != null)
		{
			Task task;
			TaskSample mSample;
			for(int i=0;i<dailyList.Count;i++)
			{
				task = dailyList [i] as Task;
				mSample = TaskSampleManager.Instance.getTaskSampleBySid (task.sid);
				if(task.index <= mSample.condition.conditions.Length / 2 && TaskManagerment.Instance.isComplete (task))
				{
					return i;
				}
			}
		}
		return 0;
	}
}
