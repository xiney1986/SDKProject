using System;
using System.Collections.Generic;
 
/**商品模板管理器
  *@author longlingquan
  **/
public class GoodsSampleManager:SampleConfigManager
{
	//单例
	private static GoodsSampleManager instance;

	public GoodsSampleManager ()
	{
		base.readConfig (ConfigGlobal.CONFIG_GOODS);
	}
	
	public static GoodsSampleManager Instance {
		get {
			if (instance == null)
				instance = new GoodsSampleManager ();
			return instance;
		}
	} 
	
	//获得装备模板对象
	public GoodsSample getGoodsSampleBySid (int sid)
	{
		if (!isSampleExist (sid))
			createSample (sid); 
		return samples [sid] as GoodsSample;
	}   
	
	//解析模板数据
	public override void parseSample (int sid)
	{
		GoodsSample sample = new GoodsSample (); 
		string dataStr = getSampleDataBySid (sid); 
		sample.parse (sid, dataStr);
		samples.Add (sid, sample);
	}
	
	//得到指定商店所有商品sid
	public int[] getAllShopGoods (int shopType)
	{ 
		List<int> list = new List<int> (); 
		foreach (int key in data.Keys) { 
			if (getGoodsSampleBySid (key).shopType == shopType)
				list.Add ((int)key); 
		}
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

