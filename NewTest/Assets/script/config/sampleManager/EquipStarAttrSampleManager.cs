using UnityEngine;
using System.Collections;

/// <summary>
/// 装备升星模板管理器
/// </summary>
public class EquipStarAttrSampleManager : SampleConfigManager
{
	
	/* static fields */
	//单例
	private static EquipStarAttrSampleManager instance;
	
	/* static methods */
	public static EquipStarAttrSampleManager Instance {
		get{
			if(instance==null)
				instance=new EquipStarAttrSampleManager();
			return instance;
		}
	} 
	
	/* methods */
	public EquipStarAttrSampleManager () {
		base.readConfig (ConfigGlobal.CONFIG_EQUIPSTARATTR);
	}
	//获得升星模板对象
	public EquipStarAttrSample getEquipStarAttrSampleBySid (int sid) {
		if (!isSampleExist (sid))
			createSample (sid);
		return samples [sid] as EquipStarAttrSample;
	} 
	//解析模板数据
	public override void parseSample (int sid) {
		EquipStarAttrSample sample = new EquipStarAttrSample (); 
		string dataStr = getSampleDataBySid (sid); 
		sample.parse (sid, dataStr); 
		samples.Add (sid, sample);
	}
}
