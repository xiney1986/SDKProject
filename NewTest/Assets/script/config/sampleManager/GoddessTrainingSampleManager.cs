using UnityEngine;
using System.Collections;

public class GoddessTrainingSampleManager : SampleConfigManager {

	public static GoddessTrainingSampleManager Instance
	{
		get
		{
			return SingleManager.Instance.getObj("GoddessTrainingSampleManager") as GoddessTrainingSampleManager;
		}
	}
	
	public GoddessTrainingSampleManager()
	{
		base.readConfig(ConfigGlobal.CONFIG_GODDESSTRAINING);
	}
	
	//获得模板对象
	public GoddessTrainingSample getDataBySid(int sid)
	{
		if (!isSampleExist(sid))
			createSample(sid);
		return samples[sid] as GoddessTrainingSample;
	}
	
	//解析模板数据
	public override void parseSample(int sid)
	{
		GoddessTrainingSample sample = new GoddessTrainingSample();
		string dataStr = getSampleDataBySid(sid);
		sample.parse(sid, dataStr);
		samples.Add(sid, sample);
	}
}
