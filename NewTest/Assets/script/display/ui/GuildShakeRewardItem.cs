using UnityEngine;
using System.Collections;
/// <summary>
/// 公会投掷奖励条目
/// </summary>
public class GuildShakeRewardItem : MonoBase
{
	public  GuildShakeRewardView[] views;
	public void Init (ShakeEblowsRewardSample sample)
	{
		PrizeSample [][] prizes = sample.getAllParsePrizes ();
		for (int i = 0; i<prizes.Length; ++i) {
			views [i].Init (prizes [i][0]);
		}
	}
}
