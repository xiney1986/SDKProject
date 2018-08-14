using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// 解锁特殊英雄管理器
/// </summary>
public class GoodsBuyCountManager : SampleConfigManager {

	//单例
	private static GoodsBuyCountManager instance;
	private List<GoodsBuyCountSample> list;
	public bool doBegin;
	public float oldNumm;
	public GoodsBuyCountManager ()
	{
		base.readConfig (ConfigGlobal.CONFIG_GOODS_MAX_COUNT);
	}
	public static GoodsBuyCountManager Instance {
		get{
			if(instance==null)
				instance=new GoodsBuyCountManager();
			return instance;
		}
	}
	//获得模板对象
	public GoodsBuyCountSample getGoodsBuyCountSampleBySid (int sid)
	{ 
		if (!isSampleExist (sid))
			createSample (sid); 
		return samples [sid] as GoodsBuyCountSample;
	}
	public override void parseConfig (string str)
	{
		GoodsBuyCountSample sample = new GoodsBuyCountSample (str); 
		if (list == null)
			list = new List<GoodsBuyCountSample> ();
		list.Add (sample);
	}
	/// <summary>
	/// 得到指定商品的可购买最大数量
	/// </summary>
	public int getMaxNum(int sid){
		for(int i=0;i<list.Count;i++){
			if(list[i].goodsSid==sid){
				int vipLv=UserManager.Instance.self.getVipLevel();
				return list[i].vipMaxCount[vipLv];
			}
		}
		return 0;
	}
    public GoodsBuyCountSample getSampleByGoodsSid(int sid)
    {
        for (int i = 0; i < list.Count;i++ )
        {
            if (list[i].goodsSid == sid) return list[i];
        }
        return null;
    }
	/// <summary>
	/// 是否显示特惠商店的刷新标示
	/// </summary>
	public bool isCanShowFlag(string type){
		string falg=PlayerPrefs.GetString (UserManager.Instance.self.uid + "tehui"+type , "null");
		if(ShopListSamleManager.Instance.getMyoPenLv()>UserManager.Instance.self.getUserLevel()){
			return false;
		}
		if(falg=="null")return true;
		if(ServerTimeKit.getDateTime().DayOfYear.ToString()!=falg)return true;
		return false;
	}
	/// <summary>
	/// 保存下次刷新时间
	/// </summary>
	public void saveShowFlagTime(string type){
		PlayerPrefs.SetString(UserManager.Instance.self.uid + "tehui"+type ,ServerTimeKit.getDateTime().DayOfYear.ToString());
	}
}
