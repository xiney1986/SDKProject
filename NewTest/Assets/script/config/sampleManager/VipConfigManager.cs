using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/**累计登陆配置文件
  *@author 汤琦
  **/
public class VipConfigManager : SampleConfigManager
{
	//单例
	private static VipConfigManager instance;
	private List<Vip> list;

	public VipConfigManager ()
	{
		base.readConfig (ConfigGlobal.CONFIG_VIP);
	}

	public static VipConfigManager Instance {
		get{
			if(instance==null)
				instance=new VipConfigManager();
			return instance;
		}
	}
	//获得所有累计登陆信息
	public Vip[] getVipInfos ()
	{
		return list.ToArray ();
	}
	
	//解析配置
	public override void parseConfig (string str)
	{  
		Vip be = new Vip (str);
		if (list == null)
			list = new List<Vip> ();
		list.Add (be);
	}

}
