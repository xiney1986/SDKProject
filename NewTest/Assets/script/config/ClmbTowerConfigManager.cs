using System;
using System.Collections.Generic;
 
/**
 * 爬塔副本配置文件
 * @author longlingquan
 * */
public class ClmbTowerConfigManager:ConfigManager
{
	// 单例
	private static ClmbTowerConfigManager instance;
	private List<ClmbTowerMap> maps;
    private int towerChapter;
	 
	public static ClmbTowerConfigManager Instance {
		get{
			if(instance==null)
				instance=new ClmbTowerConfigManager();
			return instance;
		}
	}

    public ClmbTowerConfigManager()
	{
        base.readConfig(ConfigGlobal.CONFIG_CLMB_TOWER);
	}
	
	//获得所有的爬塔副本地图信息
    public ClmbTowerMap[] getAllStorys()
	{
		return maps.ToArray ();
	}
	
	//获得指定id的章节地图
    public ClmbTowerMap getClmbTowerMap(int id)
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
            maps = new List<ClmbTowerMap>();
        ClmbTowerMap map = new ClmbTowerMap(str);
		maps.Add (map); 
	}
} 