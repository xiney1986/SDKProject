using UnityEngine;
using System.Collections.Generic;

public class HeroRoadManagerment
{

	
	public static HeroRoadManagerment Instance {
		get{return SingleManager.Instance.getObj("HeroRoadManagerment") as HeroRoadManagerment;}
	}


	public Dictionary<int,HeroRoad> map = new Dictionary<int, HeroRoad>();
	public HeroRoad currentHeroRoad; //正在进行的英雄之章

	public void clean()
	{
		currentHeroRoad=null;
	}

	public void loadServerData(CallBack callback)
	{
		HeroRoadFPort port = FPortManager.Instance.getFPort ("HeroRoadFPort") as HeroRoadFPort;
		port.getRoadActivation (callback);

	}

	/**
	//此方法可判断某张卡片是否激活了英雄之章.
	public bool isNewHeroRoad(int type)
	{
		if (map.ContainsKey (type))
			return map [type].conquestCount < map [type].activeCount && map[type].activeCount < map[type].sample.getMissionCount();
		return true;
	}
	*/
	/// <summary>
	/// 是否开启英雄之章
	/// </summary>
	/// <param name="prizes">Prizes</param>
	public bool isOpenHeroRoad(PrizeSample[] prizes){
		if (prizes == null)
			return false;
		PrizeSample prize;
		bool isTempOpen = false;
		bool isOpen=false;
		for (int i = 0; i < prizes.Length; i++) {
			prize = prizes[i];
			isTempOpen=isOpenHeroRoad(prize);
			if(isTempOpen)
				isOpen=true;
		}
		return isOpen;
	}
	/// <summary>
	/// 是否开启英雄之章
	/// </summary>
	/// <param name="prize">prize</param>
	public bool isOpenHeroRoad(PrizeSample prize) {
		if (prize == null)
			return false;
		if (prize.type != PrizeType.PRIZE_CARD)
			return false;
		Card card = CardManagerment.Instance.createCard (prize.pSid);
		if (activeHeroRoadIfNeed (card))
			return true;
		else
			return false;
	}

	/** 这里只用于判断是否激活新英雄之章的判断 */
	public bool isActiveHeroRoad(Card c)
	{
		if (c == null)
			return false;
		int type = c.getEvolveNextSid ();
		if (type > 0) {
			HeroRoad obj = null;
			if(map.ContainsKey(type)){
				obj = map[type];
				if(obj.activeCount < obj.sample.getMissionCount()){
					return true;
				}
			} else{
				return true;
			}
		}
		return false;
	}

	/** 不能随便调用，因为判断成功的同时，就等于激活了新的英雄之章 */
	public bool activeHeroRoadIfNeed(Card c)
	{
	    
		if (c == null)
			return false;
       // Debug.LogError("c.getQualityId()" + c.getQualityId());
        if (c.getQualityId() == 6) return false;
        
		int type = c.getEvolveNextSid ();
		if (type > 0) {
			HeroRoad obj = null;
			if(map.ContainsKey(type)){
				obj = map[type];
				if(obj.activeCount < obj.sample.getMissionCount()){
					obj.activeCount++;
					return true;
				}
			}else{
				obj = new HeroRoad();
				obj.sample = HeroRoadSampleManager.Instance.getSampleBySid(type);
				obj.activeCount = 1;
				map.Add(type,obj);
				return true;
			}
		}
		return false;
	}

	public void conquestHeroRoad(int sid)
	{
		if(map.ContainsKey(sid))
			map [sid].conquestCount++;
	}

	public bool isCompleted(int sid)
	{
		if(map.ContainsKey(sid))
			return map[sid].activeCount == map[sid].conquestCount;
		return false;
	}

	public List<HeroRoad> getHeroRoadListByQuality(int quality,int mustSid)
	{
		List<HeroRoad> list = new List<HeroRoad> ();
		foreach (HeroRoad obj in map.Values) {
			if(obj.sample.quality == quality && ((mustSid > 0 && mustSid == obj.sample.sid) || obj.conquestCount < obj.activeCount)){
				list.Add(obj);
			}
		}
		return list;
	}

	//获取可挑战,用于主页显示
	public int getCanBeChallengingTimes()
	{
		int count = 0;
		foreach (HeroRoad obj in map.Values) {
			count += obj.activeCount - obj.conquestCount;
		}
		return count;
	}

	public int getCanBeChallengingTimesByQuality(int quality)
	{
		int count = 0;
		foreach (HeroRoad obj in map.Values) {
			if(obj.sample.quality == quality)
				count += obj.activeCount - obj.conquestCount;
		}
		return count;
	}

	public bool isHaveBySid(int sid)
	{
		return map.ContainsKey (sid);
	}

	public bool isCurrentDirectFight()
	{
		return currentHeroRoad != null && currentHeroRoad.isDirectFight ();
	}

}
