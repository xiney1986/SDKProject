using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/**成长奖励配置文件
  *@author 黄兴财
  **/
public class GrowupSampleConfigManager : SampleConfigManager
{
	//模板集合
	new List<GrowupAwardSample> samples = new List<GrowupAwardSample>(); 
	static GrowupSampleConfigManager instance;
	public GrowupSampleConfigManager ()
	{
		base.readConfig (ConfigGlobal.CONFIG_GROWUP);
	}
	public static GrowupSampleConfigManager Instance {
		get{
			if(instance==null)
				instance=new GrowupSampleConfigManager();
			return instance;
		}
	}
	
	//对配置文件进行分块处理 根据sid
	public override void parseConfig (string str)
	{
		base.parseConfig (str); 
		GrowupAwardSample gas = new GrowupAwardSample();
		gas.parse(0,str);
		samples.Add(gas);
	}
	
	//返回奖品信息
	public List<GrowupAwardSample> GetPrizeSamples(){
		if(samples != null){
			return samples;
		}
		return null;
	}
	
	public GrowupAwardSample[] GetPrizes(){
		if(samples != null){
			return samples.ToArray();
		}
		return null;
	}
	//清除数据
	public new void clear() {
		if(samples!=null)
			samples.Clear ();
	}

}
