using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// 秘宝对应属性配置模板管理器
/// </summary>
public class MagicWeaponAttributSampleManager : SampleConfigManager {

	//单例
	private static MagicWeaponAttributSampleManager instance;
    private List<MagicWeaponAttributSample> list;
	public bool doBegin;
	public float oldNumm;
    public MagicWeaponAttributSampleManager()
	{
        base.readConfig(ConfigGlobal.CONFIG_MAGIC_ATTRIBUT);
	}
    public static MagicWeaponAttributSampleManager Instance {
		get{
			if(instance==null)
                instance = new MagicWeaponAttributSampleManager();
			return instance;
		}
	}
	//获得模板对象
    public MagicWeaponAttributSample getMwAttrSampleBySid(int sid)
	{ 
		if (!isSampleExist (sid))
			createSample (sid);
        return samples[sid] as MagicWeaponAttributSample;
	}
    //解析模板数据
    public override void parseSample(int sid) {
        MagicWeaponAttributSample sample = new MagicWeaponAttributSample();
        string dataStr = getSampleDataBySid(sid);
        sample.parse(sid, dataStr);
        samples.Add(sid, sample);
    }
}
