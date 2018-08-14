using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BackPrizeConfigManager : SampleConfigManager
{
	private static BackPrizeConfigManager instance;
	public List<BackPrize> list;


	public BackPrizeConfigManager ()
	{
		base.readConfig (ConfigGlobal.CONFIG_BACKPRIZE);
	}

	public static BackPrizeConfigManager Instance
	{		
		get{return SingleManager.Instance.getObj("BackPrizeConfigManager") as BackPrizeConfigManager;}
	}


	public override void parseConfig (string str)
	{
		BackPrize bp = new BackPrize (str);
		if (list == null)
			list = new List<BackPrize> ();
		list.Add (bp);
	}
}
