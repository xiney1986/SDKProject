using System;

public class TimeNoticeContent:NoticeContent
{
	//时间列表列表 [st,et,st,et]
	public int[] time;

	public TimeNoticeContent ()
	{
	}

	public override void parse (string str)
	{
		base.parse (str);
		string[] strs = str.Split (',');
		time = new int[strs.Length];
		for (int i = 0; i < strs.Length; i++) {
			time [i] = StringKit.toInt (strs [i]);
		}
	}
}

