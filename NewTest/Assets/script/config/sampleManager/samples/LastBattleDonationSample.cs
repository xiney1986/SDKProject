using UnityEngine;
using System.Collections;

public class LastBattleDonationSample
{
	public int id;// 条目id//
	public PrizeSample donation;//奖品
	public int scores;// 所得排名得分，进度数值 //
	public int junGong;// 所得军功个数//
	public int nvShenBlessLV;// 女神鼓舞等级//
	public int index;// 捐献列表上允许有相同条目id的条目，唯一index来区分相同条目id的捐献，通信也是靠index//
	public int state;// 该条目捐献状态//
	public int process;// 所得世界进度//
	public int donationType;// 捐献类型//

	public LastBattleDonationSample(int id,PrizeSample donation,int scores,int junGong,int nvShenBlessLV,int index,int state,int process,int donationType)
	{
		this.id = id;
		this.donation = donation;
		this.scores = scores;
		this.junGong = junGong;
		this.nvShenBlessLV = nvShenBlessLV;
		this.index = index;
		this.state = state;
		this.process = process;
		this.donationType = donationType;
	}
	public LastBattleDonationSample()
	{

	}
}

public class LastBattleDonationState
{
	public const int NO_DONATE = 0;// 未捐献//
	public const int YES_DONATE = 1;// 已捐献//


}

