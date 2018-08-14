using UnityEngine;
using System.Collections.Generic;

/**
 * 女神星盘星云集合管理器
 * @author 陈世惟
 * */
public class GoddessAstrolabeStarArrayManagerment : SampleConfigManager {

	//单例
	private static GoddessAstrolabeStarArrayManagerment instance;
//	private List<GoddessAstrolabeStarArray> info;//前台配置

	public static GoddessAstrolabeStarArrayManagerment Instance {
		get{
			if(instance==null)
				instance=new GoddessAstrolabeStarArrayManagerment();
			return instance;
		}
	}
	
	public GoddessAstrolabeStarArrayManagerment ()
	{
		base.readConfig (ConfigGlobal.CONFIG_GODDESSASTROLABE_STARARRAY);
	}
	
//	//解析配置
//	public override void parseConfig (string str)
//	{  
//		GoddessAstrolabeStarArray be = new GoddessAstrolabeStarArray (str);
//		if (info == null)
//			info = new List<GoddessAstrolabeStarArray> ();
//		info.Add (be);
//	}
	
//	//取得前台配置
//	public List<GoddessAstrolabeStarArray> getInfo ()
//	{
//		return info;
//	}

	//获得模板对象
	public GoddessAstrolabeStarArray getNebulaBySid (int sid)
	{
		if (!isSampleExist (sid))
			createSample (sid);
		return samples [sid] as GoddessAstrolabeStarArray;
	}
	
	//解析模板数据
	public override void parseSample (int sid)
	{
		GoddessAstrolabeStarArray sample = new GoddessAstrolabeStarArray (); 
		string dataStr = getSampleDataBySid (sid); 
		sample.parse (sid, dataStr); 
		samples.Add (sid, sample);
	}
}
