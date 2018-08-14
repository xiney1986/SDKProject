using System;

public class ConsumeRebateNoticeContent:NoticeContent
{
	//商品信息
	public NoticeActiveAndSid[] actives;

	public ConsumeRebateNoticeContent ()
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
}

