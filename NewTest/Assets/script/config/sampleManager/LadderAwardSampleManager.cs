using System;
using System.Collections;
using System.Collections.Generic;

public class LadderAwardSampleManager : SampleConfigManager {

	private static LadderAwardSampleManager instance;
	private List<LadderHegeMoney> list;
	
	//返利钻石
	public int rebatesMoney;
	//返利信息
	public string rebatesMsg;
	//是否可以领取
	public bool isgetrebateMoney;
	
	public LadderAwardSampleManager ()
	{ 
		base.readConfig (ConfigGlobal.CONFIG_LADDERGEHEMONEY);
	}
	
	public static LadderAwardSampleManager Instance {
		get{
			if(instance==null)
				instance=new LadderAwardSampleManager();
			return instance;
		}
	}
	//获得等级奖品信息
	public LadderHegeMoney[] getLadderHegeMoneys (int noticesid)
	{
		List<LadderHegeMoney> listsids = new List<LadderHegeMoney> ();
	    
		foreach (LadderHegeMoney ladder in list) {

			if (ladder.ladernoticeSid == noticesid)

				listsids.Add(ladder);
		}

		return listsids.ToArray ();
	}
	
	//解析配置
	public override void parseConfig (string str)
	{  
		LadderHegeMoney be = new LadderHegeMoney (str);
		if (list == null)
			list = new List<LadderHegeMoney> ();
		list.Add (be);
	}
}
