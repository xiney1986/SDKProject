using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MiningSampleManager : SampleConfigManager {

	//模板集合
	new List<MiningSample> samples = new List<MiningSample>(); 
	static MiningSampleManager instance;
	public MiningSampleManager ()
	{
		base.readConfig (ConfigGlobal.CONFIG_MINING);
	}
	public static MiningSampleManager Instance {
		get{
			if(instance==null)
				instance=new MiningSampleManager();
			return instance;
		}
	}
	
	//对配置文件进行分块处理 根据sid
	public override void parseConfig (string str)
	{
		base.parseConfig (str); 
		MiningSample ms = new MiningSample();
		ms.parse(0,str);
		samples.Add(ms);
	}
	
	//返回信息
	public List<MiningSample> GetMiningSample(){
		if(samples != null){
			return samples;
		}
		return null;
	}
	
	public MiningSample[] GetSample(){
		if(samples != null){
			return samples.ToArray();
		}
		return null;
	}

	public MiningSample GetMiningSample(MiningTypePage type ,MiningSize size){
		List<MiningSample> list = new List<MiningSample>();
		foreach(MiningSample tmp in samples ){
			if(tmp.type == (int)type && tmp.size == (int)size){
				return tmp;
			}
		}
		return null;
	}

	public MiningSample GetMiningSampleBySid(int sid){
		foreach(MiningSample tmp in samples ){
			if(tmp.sid == sid){
				return tmp;
			}
		}
		return null;
	}


	//清除数据
	public new void clear() {
		if(samples!=null)
			samples.Clear ();
	}
}
