using UnityEngine;
using System.Collections;

/// <summary>
/// 限时抽奖活动
/// </summary>
public class LuckyDrawNotice : Notice
{

	public ActiveTime activeTime;
	public LuckyDrawNotice(int sid) : base(sid)
	{
		this.sid = sid;
	}

	public override bool isValid()
	{
		activeTime = ActiveTime.getActiveTimeByID(getSample().timeID);
		if (activeTime.getIsFinish())
			return false;
		if (UserManager.Instance.self.getUserLevel() < getSample().levelLimit)
			return false;
		return ServerTimeKit.getSecondTime() >= activeTime.getPreShowTime();
	}

	public override string ToString()
	{
		RankAward[] awards1 = LucklyActivityAwardConfigManager.Instance.getAward(getSample().sid);
		RankAward[] awards2 = LucklyActivityAwardConfigManager.Instance.getSource(getSample().sid);
		string info1 = "\"";
		string info2 = "\"";
		for (int i = 0; i < awards1.Length; i++)
		{
			info1 += awards1[i];
			if (i < awards1.Length - 1)
			{
				info1 += "\n";
			}
		}
		info1 += "\"";
		for (int i = 0; i < awards2.Length; i++)
		{
			info2 += awards2[i];
			if (i < awards2.Length - 1)
			{
				info2 += "\n";
			}
		}
		info2 += "\"";
		return base.ToString() + "\t" + info1 + "\t" + info2;
	}
}
