using UnityEngine;
using System.Collections;

public class TimeDescript
{
	public int preheatingTime;//预热时间
	public int startTime;//开始时间
	public int durationTime;//持续时间
	public int cycleTime;//循环间隔

	//读取详细数据
	public  TimeDescript (string str, int _time)
	{
		preheatingTime = _time;
		string[] strArr = str.Split (',');
		startTime = StringKit.toInt (strArr [0]);
		durationTime = StringKit.toInt (strArr [1]);
		cycleTime = StringKit.toInt (strArr [2]);
	}
}

public class TimeInfoSample
{
	public TimeDescript[] smallTimeDescript;
	public int sid;
	public int type;//几种时间格式
	public int preheatingTime;//预热时间
	public int[] onLineSecondTime;	// 开服秒数,开服多少天以后过期
	public int[] mainTimeInfo;//周,月循环:[1,2] 绝对时间=0：[开始,持续,循环] 开服时间:[相对于开服的活动开始,持续,循环]
	public int[] mainTimeInfoo;//周,月循环:[1,2] 绝对时间=0：[开始,持续,循环] 开服时间:[相对于开服的活动开始,持续,循环]

	public TimeInfoSample (string str)
	{
		parse (str);
	}

	private void parseMainTimeInfo (string str)
	{
		string[] strArr = str.Split (',');
		mainTimeInfo = new int[strArr.Length];
		for (int i=0; i<strArr.Length; i++) {
			mainTimeInfo [i] = StringKit.toInt (strArr [i]);
		}
	}
	private void parseMainTimeInfoo (string str)
	{
		string[] strArr = str.Split (',');
		mainTimeInfoo = new int[strArr.Length];
		for (int i=0; i<strArr.Length; i++) {
			mainTimeInfoo [i] = StringKit.toInt (strArr [i]);
		}
	}

	private void parse (string str)
	{
		string[] strArr = str.Split ('|');
		sid = StringKit.toInt (strArr [0]);
		//时间类型
		type = StringKit.toInt (strArr [1]);
		//预热时间
		preheatingTime = StringKit.toInt (strArr [2]);
		// 开服时间
		pareseOpenLineTime(strArr [3]);

		//解析主时间信息
		parseMainTimeInfo (strArr [4]);
		parseMainTimeInfoo(strArr [4]);
		//解析具体循环时间
		parseSmall (strArr [5], preheatingTime);
	}
    private void pareseOpenLineTime(string str)
    {
        string[] strArr = str.Split(',');
        if(strArr.Length==1){
            onLineSecondTime = new int[strArr.Length+1];
            for (int i = 0; i < strArr.Length; i++)
            {
                onLineSecondTime[i] = StringKit.toInt(strArr[i]);
            }
            onLineSecondTime[1] = 0;
        }
        else
        {
            onLineSecondTime = new int[strArr.Length];
            for (int i = 0; i < strArr.Length; i++)
            {
                onLineSecondTime[i] = StringKit.toInt(strArr[i]);
            }
        }
        
    }
	private void parseSmall (string str, int _preheatingTime)
	{
		string[] strArr = str.Split ('#');
		if (strArr [0].Split (',').Length != 3)
			return;
		smallTimeDescript = new TimeDescript[strArr.Length];
		for (int i=0; i<strArr.Length; i++) {
			smallTimeDescript [i] = new TimeDescript (strArr [i], _preheatingTime);
		}
	}

	public void updataData(string dataStr)
	{
		parse (dataStr);
	}
}


