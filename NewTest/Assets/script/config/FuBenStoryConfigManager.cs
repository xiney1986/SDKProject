using System;
using System.Collections.Generic;
 
/**
 * 剧情副本配置文件
 * @author longlingquan
 * */
public class FuBenStoryConfigManager:ConfigManager
{
	// 单例
	private static FuBenStoryConfigManager instance;
	private List<FuBenStoryMap> maps;
	 
	public static FuBenStoryConfigManager Instance {
		get{
			if(instance==null)
				instance=new FuBenStoryConfigManager();
			return instance;
		}
	}

	public FuBenStoryConfigManager ()
	{  
		base.readConfig (ConfigGlobal.CONFIG_FUBEN_STORY);
	}
	
	//获得所有的剧情副本地图信息
	public FuBenStoryMap[] getAllStorys ()
	{
		return maps.ToArray ();
	}
	
	//获得指定id的章节地图
	public FuBenStoryMap getFuBenStoryMap (int id)
	{
		int max = maps.Count;
		for (int i = 0; i < max; i++) {
			if (maps [i].mapId == id)
				return maps [i];
		}
		return null;
	}
	
	//解析配置
	public override void parseConfig (string str)
	{  
		if (maps == null)
			maps = new List<FuBenStoryMap> ();
		FuBenStoryMap map = new FuBenStoryMap (str);
		maps.Add (map); 
	}
} 