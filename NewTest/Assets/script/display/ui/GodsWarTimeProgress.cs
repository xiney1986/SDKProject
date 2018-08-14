using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GodsWarTimeProgress : ButtonBase
{
	/** 时间数组 */
	public UILabel[] timelabels;
	/** 比赛日程节点 */
	public UISprite[] nodePoint;
	/** 比赛日程激活线条 */
	public GameObject[] nodeLine;
	/** 节点描述 */
	public UILabel[] lblDes;

	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
		inToFight ();
	}

	/// <summary>
	/// 小组赛时间信息
	/// </summary>
	public void initTime ()
	{
		System.DateTime serverDate = ServerTimeKit.getDateTime ();
		int day = TimeKit.getWeekCHA (serverDate.DayOfWeek);
		setTimeLabel (day);
	}
	/// <summary>
	/// 初始化淘汰赛时间信息
	/// </summary>
	public void initFinal ()
	{
		System.DateTime serverDate = ServerTimeKit.getDateTime ();
		int hour = serverDate.Hour;
		int minute = serverDate.Minute;
		setTimeLabel (hour, minute);
	}

	/// <summary>
	/// 设置时间标签
	/// </summary>
	public void setTimeLabel (int type)
	{
		int num;
		if (type > 4)
			num = 3;
		else 
			num = type - 1;
		for (int i= num; i>=0; i--) {
			timelabels [i].text = getDateTime (ServerTimeKit.getSecondTime () - 86400 * (num - i));
			nodePoint [i].spriteName = "point_green";
			if (i == num) {
				nodePoint [i].spriteName = "point-y";
			}
			if (i - 1 >= 0)
				nodeLine [i - 1].gameObject.SetActive (true);
		}
		for (int i= num+1; i<timelabels.Length; i++) {
			timelabels [i].text = getDateTime (ServerTimeKit.getSecondTime () + 86400 * (i - num));
		}
	}
	/// <summary>
	/// 设置淘汰赛时间标签
	/// </summary>
	public void setTimeLabel (int hour, int minute)
	{
		List<godsWarTime> time = new List<godsWarTime> (); 
		time = GodsWarInfoConfigManager.Instance ().getSampleBySid (6001).times;
	    int currentTime = hour*3600 + minute*60;
	    for (int k=time.Count-1;k>=0;k--)
	    {
            timelabels[k].text = time[k].hour + ":" + (time[k].minute == 0 ? "00" : time[k].minute + "");
	        int tempTime = time[k].hour*3600 + time[k].minute*60;
	        if (k == time.Count - 1)
	        {
	            if (currentTime >= tempTime + 600) nodePoint[k].spriteName = "point_green";
                else if (currentTime >= tempTime) nodePoint[k].spriteName = "point-y";
                continue;
	        }
            int temp=time[k+1].hour*3600 + time[k+1].minute*60;
	        if (currentTime >= temp)
	        {
	            nodePoint[k].spriteName = "point_green";
                nodeLine[k].gameObject.SetActive(true);
	        }
            else if (currentTime >= tempTime) nodePoint[k].spriteName = "point-y";
	    }
        //for (int i = 0; i < time.Count; i++) {
        //    if (hour > time [i].hour || (hour >= time [i].hour && minute >= time [i].minute)) {
        //        for (int j = i+1; j >0; j--) {
        //            nodePoint [j-1].spriteName = "point_green";
        //            //正在进行则显示为黄色
        //            if(((hour >= time [i].hour && minute >= time [i].minute)&&(hour == time [i+1 < time.Count ? (i+1) : i].hour && minute < time [i+1 < time.Count ? (i+1) : i].minute))
        //               ||(hour >= time [i].hour && minute >= time [i].minute)&& hour < time [i+1 < time.Count ? (i+1) : i].hour)
        //                nodePoint[j-1].spriteName = "point-y";
        //            if (j-1 > 0)
        //                nodeLine [j - 2].gameObject.SetActive (true);
        //        }
        //    }
        //    timelabels [i].text = time [i].hour + ":" + (time [i].minute == 0 ? "00" : time [i].minute + "");
        //}
		int pos = 0;
		lblDes [pos++].text = LanguageConfigManager.Instance.getLanguage ("godsWar_112");
		lblDes [pos++].text = LanguageConfigManager.Instance.getLanguage ("godsWar_113");
		lblDes [pos++].text = LanguageConfigManager.Instance.getLanguage ("godsWar_114");
		lblDes [pos++].text = LanguageConfigManager.Instance.getLanguage ("godsWar_115");
	}
	
	/// <summary>
	/// 获得日期
	/// </summary>
	public string getDateTime (int secondTime)
	{
		return TimeKit.dateToFormat (secondTime, LanguageConfigManager.Instance.getLanguage ("notice04"));
	}
	/// <summary>
	/// 日程表
	/// </summary>
	public void inToFight ()
	{
		UiManager.Instance.openDialogWindow<GodsWarProgramWindow> ();
	}

}
