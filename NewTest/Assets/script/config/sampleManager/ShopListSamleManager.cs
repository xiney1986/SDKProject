using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// 商店列表管理器 
/// </summary>
public class ShopListSamleManager :SampleConfigManager  {

	//单例
	private static ShopListSamleManager instance;
	private List<ShopListSample> list;
	public ShopListSamleManager ()
	{
		base.readConfig (ConfigGlobal.CONFIG_SHOP_LIST_CONFIG);
	}
	public static ShopListSamleManager Instance {
		get{
			if(instance==null)
				instance=new ShopListSamleManager();
			return instance;
		}
	}
	//获得模板对象
	public ShopListSample getShopListSampleBySid (int sid)
	{ 
		if (!isSampleExist (sid))
			createSample (sid); 
		return samples [sid] as ShopListSample;
	}
	public override void parseConfig (string str)
	{
		ShopListSample sample = new ShopListSample (str); 
		if (list == null)
			list = new List<ShopListSample> ();
		list.Add (sample);
	}
	public List<ShopListSample> getAllShop(){
		return list;
	}
	public int getMyoPenLv(){
		for(int i=0;i<list.Count;i++){
			if(list[i].shopLag=="shengmi")return list[i].activeLv;
		}
		return 6;
	}
}
