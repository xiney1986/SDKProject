using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/**累计登陆配置文件
  *@author 汤琦
  **/
public class TotalLoginConfigManager : SampleConfigManager
{
	//单例
	private static TotalLoginConfigManager instance;
	private List<TotalLogin> list;

	public TotalLoginConfigManager ()
	{
		base.readConfig (ConfigGlobal.CONFIG_TOTALLOGIN);
	}

	public static TotalLoginConfigManager Instance {
		get{
			if(instance==null)
				instance=new TotalLoginConfigManager();
			return instance;
		}
	}
	//获得所有累计登陆信息
	public TotalLogin[] getTotalLogins ()
	{
		return list.ToArray ();
	}
	
	//解析配置
	public override void parseConfig (string str)
	{  
		TotalLogin be = new TotalLogin (str);
		if (list == null)
			list = new List<TotalLogin> ();
		list.Add (be);
	}

}
