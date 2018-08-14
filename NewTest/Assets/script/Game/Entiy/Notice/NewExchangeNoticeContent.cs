using System;

public class NewExchangeNoticeContent:NoticeContent
{
	//商品信息
	public NoticeActiveAndSid[] actives;

	public NewExchangeNoticeContent ()
	{
	}

	//54,105001,105002#62,105001,105002
	public override void parse (string str)
	{
		base.parse (str);
		string[] strs = str.Split ('#');
		actives = new NoticeActiveAndSid[strs.Length];
		for (int i=0; i<strs.Length; i++) 
			actives [i] = new NoticeActiveAndSid (strs [i]);
	}

	public override string ToString()
	{
		string str = "\"";
		for (int i = 0; i < actives.Length; i++)
		{
			str += actives[i];
			if (i < actives.Length - 1)
			{
				str += "\n";
			}
		}
		str += "\"";
		return str;
	}
}

public class NoticeActiveAndSid
{
	public int activeID;
	public int[] exchangeSids;

	public NoticeActiveAndSid (string str)
	{
		string[] strs = str.Split (',');
		activeID = StringKit.toInt (strs [0]);
		exchangeSids = new int[strs.Length - 1];
		for (int i = 1; i < strs.Length; i++)
			exchangeSids [i - 1] = StringKit.toInt (strs [i]);
	}

	public override string ToString()
	{
		string str = "";
		for (int i = 0; i < exchangeSids.Length; i++)
		{
			ExchangeSample sample = ExchangeSampleManager.Instance.getExchangeSampleBySid(exchangeSids[i]);
			str += sample;
			if (i < exchangeSids.Length - 1)
			{
				str += "\n";
			}
		}
		return str;
	}
}

