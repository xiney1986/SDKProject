using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/**血脉单个节点配置配置文件
  *@author 汤琦
  **/
public class BloodItemConfigManager : SampleConfigManager
{
	//单例
	private static BloodItemConfigManager instance;
    private List<BloodItemSample> list;

    public BloodItemConfigManager()
	{
        base.readConfig(ConfigGlobal.CONFIG_BLOODITEM);
	}

    public static BloodItemConfigManager Instance {
		get{
			if(instance==null)
                instance = new BloodItemConfigManager();
			return instance;
		}
	}
	//获得指定Sid的节点信息
    public BloodItemSample getBolldItemSampleBySid(int sid)
    {
        if (list == null || list.Count <= 0) return null;
        for (int i=0;i<list.Count;i++)
        {
            if (list[i].sid==sid)
            {
                return list[i];
            }
        }
        return null;
    }
	
	//解析配置
	public override void parseConfig (string str)
	{
        BloodItemSample be = new BloodItemSample(str);
		if (list == null)
            list = new List<BloodItemSample>();
		list.Add (be);
	}

}
