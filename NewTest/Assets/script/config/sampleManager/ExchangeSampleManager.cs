using System;
using System.Collections.Generic;
 
/**
 * 兑换模板管理器
 * */
public class ExchangeSampleManager:SampleConfigManager
{
	//单例
	private static ExchangeSampleManager instance;

	public ExchangeSampleManager ()
	{
		base.readConfig (ConfigGlobal.CONFIG_EXCHANGE);
	}
	
	public static ExchangeSampleManager Instance {
		get {
			if (instance == null) 
				instance = new ExchangeSampleManager ();
			return instance;
		}
	} 
	
	//获得装备模板对象
	public ExchangeSample getExchangeSampleBySid (int sid)
	{
		if (!isSampleExist (sid))
			createSample (sid); 
		return samples [sid] as ExchangeSample;
	}

	 
	
	//解析模板数据
	public override void parseSample (int sid)
	{
		ExchangeSample sample = new ExchangeSample (); 
		string dataStr = getSampleDataBySid (sid); 
		sample.parse (sid, dataStr); 
		samples.Add (sid, sample);
	}
	
	//获得所有兑换sid
	public int[] getAllExchange ()
	{
		List<int> list = new List<int> ();
		foreach (int key in data.Keys) { 
			list.Add (key); 
		}
		list.Sort ();
		return list.ToArray ();
	}

	//需要修改 data samples 如果存在就覆盖，不存在添加
	public void updataSample (int sid, string dataStr)
	{
		if (data.ContainsKey (sid)) { // 存在
			data [sid] = dataStr;
			if (samples [sid] != null)//若样本已经创建，需要修改
				(samples [sid] as ExchangeSample).parse (sid, dataStr);
		} else {
			data.Add (sid, dataStr);
		}
	}
}

