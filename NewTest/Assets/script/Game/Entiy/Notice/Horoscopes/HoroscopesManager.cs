using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HoroscopesManager
{

	public static HoroscopesManager Instance {
		get{ return SingleManager.Instance.getObj ("HoroscopesManager") as HoroscopesManager;}
	}

	//免费摇一摇
	private int day;//上次获取数据的时间
	private int[] luckyStarAndSid;//[star,sid,star,sid...]
	private int prayTime; //cd累计时间
	private int beginTime; //祈祷开始时间
	private int endTime;// 祈祷结束时间
	private HoroscopesCacheAward[] cacheAwards;//缓存摇一摇奖励，显示用


	public HoroscopesManager ()
	{
	}

	//[109,[[5,11423],[8,11423],[10,11423],[11,11425]],0,36000,79200]}
	// 109=dy [[星座id,奖励sid],...],0=cd,36000=start,79200=end 
	public void initData (ErlArray array)
	{
		day = StringKit.toInt ((array.Value [0] as ErlType).getValueString ());
		ErlArray array1 = array.Value [1] as ErlArray;
		luckyStarAndSid = new int[array1.Value.Length * 2];
		for (int i = 0; i < array1.Value.Length; i++) {
			luckyStarAndSid [i * 2] = StringKit.toInt ((((array1.Value [i] as ErlArray).Value [0]) as ErlType).getValueString ());
			luckyStarAndSid [i * 2 + 1] = StringKit.toInt (((array1.Value [i] as ErlArray).Value [1] as ErlType).getValueString ());
		}
		prayTime = StringKit.toInt ((array.Value [2] as ErlType).getValueString ());
		beginTime = StringKit.toInt ((array.Value [3] as ErlType).getValueString ());
		endTime = StringKit.toInt ((array.Value [4] as ErlType).getValueString ());
	}

	//[[[1,[281479271677953,6,巨石的华莱土,0,17,9],11426,11426,2]],7200]
	//[[[2,10,11427,11423,2]],7200]
	//[摇结果信息,CD] 
	//摇结果信息=[一次,...]
	//一次=[1=玩家|2=女神,[role_uid,style,name,vip,level,star]]=摇到得玩家|star=女神,普通sid,幸运sid,倍数
	public void cacheResult (ErlArray array, ErlType cd)
	{
		cacheAwards = null;
		cacheAwards = new HoroscopesCacheAward[array.Value.Length];
		for (int i=0; i<cacheAwards.Length; i++)
			cacheAwards [i] = new HoroscopesCacheAward (array.Value [i] as ErlArray);
		if (cd != null)
			prayTime = StringKit.toInt (cd.getValueString ());
	}

	public HoroscopesCacheAward[] getCacheAwards ()
	{
		return cacheAwards;
	}

	public void clearCacheAwards ()
	{
		;
	}

	public int[] getLuckyStarAndSid ()
	{
		return luckyStarAndSid;
	}

	public int getPrayTime ()
	{
		return prayTime;
	}

	public void setPrayTime (int time)
	{
		prayTime = time;
	}

	public void setBeginTime (int time)
	{
		beginTime = time;
	}

	public int getBeginTime ()
	{
		return beginTime;
	}

	public void setEndTime (int time)
	{
		endTime = time;
	}

	public int getEndTime ()
	{
		return endTime;
	}

	//根据星座类型得到星座信息
	public Horoscopes getStarByType (int type)
	{
		int iconId = StarSampleManager.Instance.getIconId (type);
		string name = StarSampleManager.Instance.getName (type);
		string language = LanguageConfigManager.Instance.getLanguage (StarSampleManager.Instance.getLanguageId (type));
		string date = LanguageConfigManager.Instance.getLanguage (StarSampleManager.Instance.getLanguageId (type) + "date");

		string skill = LanguageConfigManager.Instance.getLanguage (StarSampleManager.Instance.getLanguageId (type) + "Sdescribe");
		string  passive= LanguageConfigManager.Instance.getLanguage (StarSampleManager.Instance.getLanguageId (type) + "Pdescribe");

		string spriteName = "horStar" + type;
		Horoscopes star = new Horoscopes (type, language, name, iconId, date, spriteName,skill,passive);
		return star;
	}
}


