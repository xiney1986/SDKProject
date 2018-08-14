using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 骰子投掷奖励模版管理器
/// </summary>
public class ShakeEblowsRewardSampleManager : SampleConfigManager
{
	private  static readonly int[] normalSids = new int[6] {1001,1002,1003,1004,1005,1006};
	private const int baseReward = 1008;
	private const int fiveDiffSid = 1007;
	private  static ShakeEblowsRewardSampleManager _instance;

	public static ShakeEblowsRewardSampleManager Instance ()
	{
		if (_instance == null)
			_instance = new ShakeEblowsRewardSampleManager ();
		return _instance;
	}

	public ShakeEblowsRewardSampleManager ()
	{
		base.readConfig (ConfigGlobal.CONFIG_GUILDSHAKE);
	}
	public override void parseSample (int sid)
	{
		ShakeEblowsRewardSample sample = new ShakeEblowsRewardSample (); 
		string dataStr = getSampleDataBySid (sid); 
		sample.parse (sid, dataStr); 
		samples.Add (sid, sample);
	}

	//获得模板对象
	public ShakeEblowsRewardSample getRewardSampleBySid (int sid)
	{
		if (!isSampleExist (sid))
			createSample (sid);
		return samples [sid] as ShakeEblowsRewardSample;
	}



	/// <summary>
	/// 通过投掷结果获取奖励列表
	/// </summary>
	public  List<PrizeSample> GetPrizeByResult (GuildLuckyNvShenShakeResult result)
	{
		List<PrizeSample> prizes = new List<PrizeSample> ();
		prizes.AddRange (GetPrizeByResult (baseReward, 1));//获取基础奖励
		if (result.isFiveDifferentResult ()) 
			prizes.AddRange (GetPrizeByResult(fiveDiffSid,1)); //五个完全不同面的奖励
		//正常奖励
		Hashtable resultTable = result.getResultTable ();
		foreach (DictionaryEntry d in resultTable) {
			int sid = int.Parse (d.Key.ToString ());
			int num = int.Parse (d.Value.ToString ());
			List<PrizeSample> temp = GetPrizeByResult (sid, num);
			prizes.AddRange (temp);
		}
		List<PrizeSample> finalPrizes = new List<PrizeSample> ();
		PrizeSample tempPrize;
		/** 合并相同的奖励,剔除空奖励*/
		for (int i =0; i <prizes.Count; ++ i) {
			tempPrize = prizes [i];
			if (tempPrize.getPrizeNumByInt () == 0)		
				continue;			
			for (int j = i+1; j<prizes.Count; ++ j) {
				if (prizes [j].type == tempPrize.type) {
					tempPrize.addNum( prizes [j].getPrizeNumByInt());
					prizes [j].num = "0";
				}
			}
			finalPrizes.Add (tempPrize);
		}
		return finalPrizes;
	}

	/// <summary>
	/// 通过模版名字和档次获取奖励
	/// </summary>
	public  List<PrizeSample> GetPrizeByResult (int sid, int num)
	{
		if (getRewardSampleBySid (sid) == null)
			return null;
		else {
			return getRewardSampleBySid (sid).getParsePrizesByLevel (num);
		}
	}



	/// <summary>
	/// 获取普通的投掷骰子奖励模版
	/// </summary>
	public List<ShakeEblowsRewardSample> GetNormalShakeEblowRewardSamples ()
	{
		List<ShakeEblowsRewardSample> normalSamples = new List<ShakeEblowsRewardSample> ();
		foreach (int sid in normalSids) {
			ShakeEblowsRewardSample temp = getRewardSampleBySid (sid);
			normalSamples.Add (temp);
		}	
		return normalSamples;
	}

	/// <summary>
	/// 获取五个不同骰子的奖励模版
	/// </summary>
	public ShakeEblowsRewardSample GetFiveDiffSample ()
	{
		return getRewardSampleBySid (fiveDiffSid);
	}

	public ShakeEblowsRewardSample GetBaseSample ()
	{
		return getRewardSampleBySid (baseReward);
	}

	/// <summary>
	/// 将名字转化为sid
	/// </summary>
	public static int convertNameToSid (string type)
	{
		int sid = 0;
		switch (type) {
		case "score":
			sid = 1001;
			break;
		case "money":
			sid = 1002;
			break;
		case "rmb":
			sid = 1003;
			break;
		case "contribution":
			sid = 1004;
			break;	
		case "honor":
			sid = 1005;
			break;
		case "none":
			sid = 1006;
			break;		
		}
		return sid;
	}


}
