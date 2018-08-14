using System;
using System.Collections.Generic;
 
/**
 * 讨伐副本配置文件
 * 讨伐副本只有一个章节
 * @author longlingquan
 * */
public class FuBenWarConfigManager:ConfigManager
{
	// 单例
	private static FuBenWarConfigManager instance;

	private int sid;//讨伐副本章节编号
	 
	public static FuBenWarConfigManager Instance {
		get{
			if(instance==null)
				instance=new FuBenWarConfigManager();
			return instance;
		}
	}

	public FuBenWarConfigManager ()
	{  
		base.readConfig (ConfigGlobal.CONFIG_FUBEN_WAR);
	}
	
	//获得讨伐活动章节sid
	public int getSid ()
	{
		return sid;
	}
	
	//解析配置
	public override void parseConfig (string str)
	{   
		sid = StringKit.toInt (str);
	}
} 