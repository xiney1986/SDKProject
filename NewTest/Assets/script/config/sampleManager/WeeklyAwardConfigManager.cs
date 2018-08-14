using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// 周末送前台配置解析文件
/// </summary>
public class WeeklyAwardConfigManager : SampleConfigManager {
	//单例
	private static WeeklyAwardConfigManager instance;
	private List<TotalLogin> list;
	private List<TotalLogin> listt;
	private List<TotalLogin> listholiday;
	private List<TotalLogin> listtholiday;
	private string[] timeflag;
	public WeeklyAwardConfigManager ()
	{
		base.readConfig (ConfigGlobal.CONFIG_WEEKLYAWARD);
	}
	public static WeeklyAwardConfigManager Instance {
		get{
			if(instance==null)
				instance=new WeeklyAwardConfigManager();
			return instance;
		}
	}
	//获得节日的奖励信息
	public TotalLogin[] getHolidayAward ()
	{
		return listholiday.ToArray ();
	}
	//获得本周的奖励信息
	public TotalLogin[] getWeeklyAward ()
	{

		return list.ToArray ();
	}
	//解析配置
	public override void parseConfig (string str)
	{  
		String[] strArr=str.Split('|');
		//看是不是时间戳
		if(strArr[0].Split(',').Length>1){//是时间戳
			timeflag=str.Split('|');
		}else{
			if(StringKit.toInt(strArr[1])==0){
				parseWeekly(str);
			}else if(StringKit.toInt(strArr[1])==1){
				parseHoliday(str);
			}
		}
	}
	private void parseHoliday(string str){
		TotalLogin bb =new TotalLogin();
		String[] strArr=str.Split('|');
		bb.prizeSid=StringKit.toInt(strArr[0]);
		bb.day=StringKit.toInt(strArr[3]);
		bb.holidayAllSid=StringKit.toInt(strArr[2]);
		bb.holidayBeginTime=getTimee(bb.holidayAllSid.ToString(),1);
		bb.holidayEndTime=getTimee(bb.holidayAllSid.ToString(),2);
		bb.holidayName=getTimee (bb.holidayAllSid.ToString(),0);
		bb.rewardType=StringKit.toInt(strArr[4]);
		bb.prizes=parsePrizes(strArr[5]);
		if(listholiday==null){
			listholiday=new List<TotalLogin>();
		}
		listholiday.Add(bb);
	}
	private string getTimee(string sid,int index){
		for(int i=0;i<timeflag.Length;i++){
			if(timeflag[i].Split(',')[0]==sid)return (timeflag[i].Split(','))[index+1];
		}
		return "";
	}
	private void parseWeekly(string str){
		TotalLogin bb =new TotalLogin();
		String[] strArr=str.Split('|');
		bb.prizeSid=StringKit.toInt(strArr[0]);
		bb.week=StringKit.toInt(strArr[3]);
		bb.single=strArr[4];
		bb.rewardType=StringKit.toInt(strArr[5]);
		DateTime dt=TimeKit.getDateTimeMillis(ServerTimeKit.getMillisTime());
		int x=(dt.DayOfYear / 7+1 )%2;
		if(x==0){
			bb.prizes=parsePrizes(strArr[6]);
		}else{
			bb.prizes=parsePrizes(strArr[7]);
		}
		if (list == null)
			list = new List<TotalLogin> ();
		list.Add (bb);

	}
	private PrizeSample[] parsePrizes (string str)
	{
		string[] strArr = str.Split ('#');
		PrizeSample[] prizes = new PrizeSample[strArr.Length];
		for (int i = 0; i < strArr.Length; i++) {
			prizes[i]=new PrizeSample();
			string[] strs = strArr[i].Split(',');
			prizes[i].type = StringKit.toInt(strs[0]);
			prizes[i].pSid = StringKit.toInt(strs[1]);
			prizes[i].num = strs[2];
		}
		return prizes;
	}
	public string[] getTimee(){
		return timeflag;
	}

}
