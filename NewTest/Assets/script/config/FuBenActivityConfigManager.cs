using System;
using System.Collections.Generic;
 
/**
 * 活动副本配置文件
 * @author longlingquan
 * */
public class FuBenActivityConfigManager:ConfigManager
{
	//单例
	private static FuBenActivityConfigManager instance;
	private  int[] sids;
	 
	public static FuBenActivityConfigManager Instance {
		get{
			if(instance==null)
				instance=new FuBenActivityConfigManager();
			return instance;
		}
	}

	public FuBenActivityConfigManager ()
	{  
		base.readConfig (ConfigGlobal.CONFIG_FUBEN_ACTIVITY);
	}
	
	//获得所有限时活动章节sid
	public int[] getSids ()
	{
		return sids;
	}
	
	//解析配置
	public override void parseConfig (string str)
	{  
		string[] strArr = str.Split ('|');
		int max = strArr.Length;
		sids = new int[max];
		for (int i = 0; i < max; i++) {
			sids [i] = StringKit.toInt (strArr [i]);	
		} 
	}
} 