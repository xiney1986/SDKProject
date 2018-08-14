using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class NewTotalLoginConfigManager : SampleConfigManager {

	//单例
	private static NewTotalLoginConfigManager instance;
	private List<TotalLogin> list;
	
	public NewTotalLoginConfigManager ()
	{
		base.readConfig (ConfigGlobal.CONFIG_NEWTOTALLOGIN);
	}
	
	public static NewTotalLoginConfigManager Instance {
//		get{
//			if(instance==null)
//				instance=new NewTotalLoginConfigManager();
//			return instance;
//		}

		get{return SingleManager.Instance.getObj("NewTotalLoginConfigManager") as NewTotalLoginConfigManager;}
	}
	//获得所有累计登陆信息
	public TotalLogin[] getNewTotalLogins ()
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
