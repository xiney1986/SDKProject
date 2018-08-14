using System;

public class SidNoticeContent:NoticeContent
{
	//sid列表
	public int[] sids;

	public SidNoticeContent ()
	{
	}

	public SidNoticeContent(NoticeSample noticeSample)
	{
		mNotice = noticeSample;
	}

	public override void parse (string str)
	{
		base.parse (str);
		string[] strs = str.Split (',');
		sids = new int[strs.Length];
		for (int i = 0; i < strs.Length; i++) {
			sids [i] = StringKit.toInt (strs [i]);
		}
	}

	public override string ToString()
	{
		string str = "";
		for (int i = 0; i < sids.Length; i++)
		{
			RechargeSample recharge = RechargeSampleManager.Instance.getRechargeSampleBySid(sids[i]);
			if (recharge != null)
			{
				str += recharge.ToString(mNotice);
			}
			else
			{
				ExchangeSample exchange = ExchangeSampleManager.Instance.getExchangeSampleBySid(sids[i]);
				str += exchange;
			}
			if (i < sids.Length - 1)
			{
				str += "\n";
			}
		}
		//Console.WriteLine(str);
		return "\""+ str+"\"";
	}
}

