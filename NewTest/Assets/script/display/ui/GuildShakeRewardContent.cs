using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 公会投掷奖励容器
/// </summary>
public class GuildShakeRewardContent : MonoBase
{
	/** 特殊描述 */
	public UILabel otherDes;
	public GuildShakeRewardItem[] shakeRewardItems;
	public void Init (List<ShakeEblowsRewardSample> samples)
	{
		//女行不显示奖励列表
		for (int i =0; i<samples.Count-1; ++i) {
			ShakeEblowsRewardSample temp = samples[i];
			shakeRewardItems[i].Init(samples[i]);
		}
	}
}
