using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FestivalWishManagerment {

	public List<FestivalWish> festivalWishs = new List<FestivalWish>();
	public List<int> sid = new List<int>();
	public string sids;

	public FestivalWishManagerment ()
	{
		initList ();
	}
	//单例
	public static FestivalWishManagerment Instance {

		get{ return SingleManager.Instance.getObj ("FestivalWishManagerment") as FestivalWishManagerment;}
	}

	//初始化条目集合sid
	private void initList ()
	{
//		sid = FestivalWishSampleManager.Instance.getAllSampleSid();
//		if(sid.Count>0)
//		{
//			foreach(int s in sid)
//				sids += s+",";
//		}
	}
	public List<FestivalWish> getAllFestivalWish()
	{
		return festivalWishs;
	}
	//通信
	public void getAllFestivalWishInfo(CallBack callback,string sids)
	{
		FestivalWishFPort fport = FPortManager.Instance.getFPort<FestivalWishFPort> ();
		fport.access (sids,callback);
	}
}

