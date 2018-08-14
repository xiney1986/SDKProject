using System;
using System.Collections.Generic;
 
/**
 * 召唤兽配置文件
 * @author longlingquan
 * */
public class BeastConfigManager:ConfigManager
{
	//单例
	private static BeastConfigManager instance;
	private List<BeastEvolve> list;
	 
	public static BeastConfigManager Instance {
		get{
			if(instance==null)
				instance=new BeastConfigManager();
			return instance;
		}
	}

	public BeastConfigManager ()
	{ 
		list = new List<BeastEvolve> ();
		base.readConfig (ConfigGlobal.CONFIG_BEAST);
	}
	
	//获得所有召唤兽进化信息
	public List<BeastEvolve> getList ()
	{
		return list;
	}
	
	//解析配置
	public override void parseConfig (string str)
	{ 
		BeastEvolve be = new BeastEvolve (str);
		list.Add (be);
	}
	
} 

