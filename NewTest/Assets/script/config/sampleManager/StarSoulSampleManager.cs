using UnityEngine;
using System.Collections;

/// <summary>
/// 星魂模板管理器
/// </summary>
public class StarSoulSampleManager : SampleConfigManager
{

	/* static fields */
	//单例
	private static StarSoulSampleManager instance;

	/* static methods */
	public static StarSoulSampleManager Instance {
		get{
			if(instance==null)
				instance=new StarSoulSampleManager();
			return instance;
		}
	} 

	/* methods */
	public StarSoulSampleManager () {
		base.readConfig (ConfigGlobal.CONFIG_STARSOUL);
	}
	//获得星魂模板对象
	public StarSoulSample getStarSoulSampleBySid (int sid) {
		if (!isSampleExist (sid))
			createSample (sid); 
		return samples [sid] as StarSoulSample;
	} 
	//解析模板数据
	public override void parseSample (int sid) {
		StarSoulSample sample = new StarSoulSample (); 
		string dataStr = getSampleDataBySid (sid); 
		sample.parse (sid, dataStr); 
		samples.Add (sid, sample);
	}
}
