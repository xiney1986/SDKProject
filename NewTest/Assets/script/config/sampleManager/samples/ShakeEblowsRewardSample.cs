using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 公会摇骰子奖励模版
/// </summary>
public class ShakeEblowsRewardSample : Sample
{

	/* fields */
	/** 奖励名 */
	string name;
	/** 奖品[档次][奖品集]*/
	PrizeSample[][] prizes;
	/** 奖励描述 */
	string prizesDesc;

	/* methods */

	public override void parse (int sid, string str)
	{
		string [] strs = str.Split ('|');
		checkLength (strs.Length, 3);
		name = strs [1];
		parsePrizes (strs [2]);
		prizesDesc = strs [3];
	}

	/** 解析奖品集 */
	public void parsePrizes (string strArr)
	{
		string[] strs = strArr.Split ('#');
		string str;
		string[] subStrs;
		prizes = new PrizeSample[strs.Length][];
		for (int i=0; i<strs.Length; i++) {
			str = strs [i];
			subStrs = str.Split ('^');
			prizes [i] = new PrizeSample[subStrs.Length];
			for (int j=0; j<subStrs.Length; j++) {
				prizes [i] [j] = parsePrize (subStrs [j]);
			}
		}
	}

	/** 解析奖品 */
	PrizeSample parsePrize (string str)
	{
		string[] strs = str.Split (',');
		PrizeSample sample = new PrizeSample ();
		sample.type = StringKit.toInt (strs [0]);
		sample.pSid = StringKit.toInt (strs [1]);
		sample.num = strs [2];
		return sample;
	}

	/// <summary>
	/// 获取指定档次的奖励集
	/// </summary>
	/// <returns>The parse prizes by level.</returns>
	/// <param name="level">Level.</param>
	public List<PrizeSample> getParsePrizesByLevel (int level)
	{
		if (level > prizes.Length)
			return null;
		List<PrizeSample> list = new List<PrizeSample> ();
		foreach (PrizeSample p in prizes [level-1]) {
			list.Add(p.Clone() as PrizeSample);

		}

		return list;
	}
	/// <summary>
	/// 获取所有档次的奖励集
	/// </summary>
	public PrizeSample[][] getAllParsePrizes ()
	{
		return prizes;
	}
	/// <summary>
	/// 获取奖励描述
	/// </summary>
	public string getPrizesDesc ()
	{
		return prizesDesc;
	}

	/// <summary>
	/// 获取名字
	/// </summary>
	public string getName(){
		return name;
	}
}