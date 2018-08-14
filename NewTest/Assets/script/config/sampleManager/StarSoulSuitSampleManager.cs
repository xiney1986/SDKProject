using UnityEngine;
using System.Collections;

/// <summary>
/// 刻印套装模板管理器
/// </summary>
public class StarSoulSuitSampleManager : SampleConfigManager {

	/* static fields */
	//单例
	private static StarSoulSuitSampleManager instance;
	
	/* static methods */
	public static StarSoulSuitSampleManager Instance {
		get{
			if(instance==null)
				instance=new StarSoulSuitSampleManager();
			return instance;
		}
	} 
	
	/* methods */
	public StarSoulSuitSampleManager () {
		base.readConfig (ConfigGlobal.CONFIG_STARSOUL_SUIT);
	}
	//获得星魂刻印模板对象
	public StarSoulSuitSample getStarSoulSuitSampleBySid (int sid) {
		if (!isSampleExist (sid))
			createSample (sid);
		return samples [sid] as StarSoulSuitSample;
	} 
	//解析模板数据
	public override void parseSample (int sid) {
		StarSoulSuitSample sample = new StarSoulSuitSample (); 
		string dataStr = getSampleDataBySid (sid); 
		sample.parse (sid, dataStr); 
		samples.Add (sid, sample);
	}
}
