using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;


/**分享抽奖配置模板管理器
  *负责分享抽奖信息的初始化 
  *@author 
  **/
public class ShareDrawSampleManager : SampleConfigManager
{
	//单例
	private static ShareDrawSampleManager instance;

    public ShareDrawSampleManager()
	{

        base.readConfig(ConfigGlobal.CONFIG_SHAREDRAW);
	}

    public static ShareDrawSampleManager Instance {
		get{
			if(instance==null)
                instance = new ShareDrawSampleManager();
			return instance;
		}
	}
	
	//解析模板数据
	public override void parseSample (int sid)
	{
        ShareDrawSample sample = new ShareDrawSample(); 
		string dataStr = getSampleDataBySid (sid); 
		sample.parse (sid, dataStr); 
		samples.Add (sid, sample);
	}
	
	//获得分享抽奖模板对象
    public ShareDrawSample getShareDrawSampleBySid(int sid)
	{ 
		if (!isSampleExist (sid))
			createSample (sid);
        return samples[sid] as ShareDrawSample;
	} 
}
