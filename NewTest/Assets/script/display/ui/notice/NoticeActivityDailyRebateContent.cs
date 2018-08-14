using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NoticeActivityDailyRebateContent : MonoBase
{
	public DailyRebateContent content;
	private Notice notice;
	[HideInInspector]
	public NoticeWindow win;//活动窗口
	
	public void initContent (Notice notice, NoticeWindow win, ArrayList dailyList)
	{
		this.notice = notice;
		this.win = win;
		content.Initialize (win, notice, dailyList);
	}
}
