using System;
using System.Collections;

/**
 * 奖励缓存管理器
 * @author longlingquan
 * */
public class AwardsCacheManager
{
	public static Hashtable awards = new Hashtable () ;

	public AwardsCacheManager ()
	{
		
	}
	 
	public static void addAwards (string key, Award[] awards)
	{
		AwardCache cache = getAwardCache (key);
		cache.addAwards (awards);
	}
	
	public static void clearAllCache()
	{
		awards = new Hashtable();
	}
	
	public static void setAwards (string key, Award[] awads)
	{
		AwardCache cache = getAwardCache (key);
		cache.setAwards (awads);
	}
	
	public static AwardCache getAwardCache (string key)
	{
		if (!awards.Contains (key)) {
			AwardCache cache = new AwardCache ();
			awards.Add (key, cache);
		}
		return awards [key] as AwardCache;
	}
}  