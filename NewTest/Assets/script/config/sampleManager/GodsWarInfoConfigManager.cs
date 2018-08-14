using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 诸神战基础信息模版管理器
/// </summary>
public class GodsWarInfoConfigManager : SampleConfigManager
{
	public static GodsWarInfoConfigManager instance;
	public static GodsWarInfoConfigManager Instance ()
	{
		if (instance == null)
			instance = new GodsWarInfoConfigManager ();
		return instance;
	}

	public GodsWarInfoConfigManager(){
		base.readConfig (ConfigGlobal.CONFIG_GODSWAR_INFO);
	}

	public override void parseSample (int sid)
	{
		GodsWarInfoSample sample = new GodsWarInfoSample (); 
		string dataStr = getSampleDataBySid (sid); 
		sample.parse (sid, dataStr); 
		samples.Add (sid, sample);
	}

	public GodsWarInfoSample getSampleBySid(int sid)
	{
		if (!isSampleExist (sid))
			createSample (sid);
		return samples [sid] as GodsWarInfoSample;
	}


}
public class GodsWarInfoSample : Sample
{
	public int[] num;
	public string des;
	public List<godsWarTime> times = new List<godsWarTime>();
	
	public override void parse (int sid, string str)
	{
		string []strs = str.Split ('|');
	    string[] st = strs[1].Split(',');
	    num=new int[st.Length];
        for (int i=0;i<st.Length;i++)
        {
            num[i] = StringKit.toInt(st[i]);
        }
		//num = StringKit.toInt (strs [1]);
		des = strs [2];
		if(strs.Length>3)
			parseTime(strs[3]);
	}
	public void parseTime(string str)
	{
		string[] arr = str.Split('#');
		for (int i = 0; i < arr.Length; i++) {
			godsWarTime t = new godsWarTime();
			t = parseSigletime(arr[i]);
			times.Add(t);
		}

	}
	private godsWarTime parseSigletime(string st)
	{
		string[] arr = st.Split(':');
		return new godsWarTime(StringKit.toInt(arr[0]),StringKit.toInt(arr[1]));
	}
}
public class godsWarTime
{
	public int hour;
	public int minute;

	public godsWarTime(){}
	public godsWarTime(int hour,int minute){
		this.hour = hour;
		this.minute = minute;
	}
}

