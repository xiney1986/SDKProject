using UnityEngine;
using System.Collections;

public class PlatFormUserInfo
{
	
	public string  nickname;
	public string  path;
	public string  userId;
	public string   face;
	public string   sex;
	public string   time;
	public string   inviteuser;
	public string   callback;
	public string   sig;
	public string   sig2;
	public string   vip;
	public string   OS;
	public string   version;
	public string 	isFangChenMi;
	public string 	pushtoken;
	public string gameIP;
	public int gamePort;
	public int serverid;
	public string constellation;
	public void coverGameSex(string str)
	{
		#if UNITY_IPHONE
		if(str=="0")
			sex="male";
		else if(str=="1")
			sex="female";
		#endif
		
		#if UNITY_ANDROID
		if(str=="M")
			sex="male";
		else
			sex="female";
		#endif
	}

	public void coverGameUrl (string url)
	{
		string[] tmp = StringKit.stringToStringList (url, new char[]{':'});
		gameIP = tmp [0];
		gamePort = StringKit.toInt (tmp [1]);
	}
	
	
	//通过返回的url得到值
	public 	PlatFormUserInfo (string url)
	{	
		path = url;
		return;

//		System.Uri uri = new System.Uri (url);
//		string queryString = uri.Query;
//		System.Collections.Specialized.NameValueCollection col = UrlKit.GetQueryString (queryString);
//
//		if (col ["nickname"] != null)
//			nickname = col ["nickname"].ToString ();
//
//		if (col ["userid"] != null)
//			userid = col ["userid"].ToString ();
//
//		if (col ["face"] != null)
//			face = col ["face"].ToString ();
//
//		if (col ["time"] != null)
//			time = col ["time"].ToString ();
//
////		if(col ["inviteuser"]!=null)
////		inviteuser = col ["inviteuser"].ToString ();
//
//		if (col ["callback"] != null)
//			callback = col ["callback"].ToString ();
//
//		if (col ["sig"] != null)
//			sig = col ["sig"].ToString ();
//
////		if(col ["sig2"]!=null)
////		sig2 = col ["sig2"].ToString ();
//
////		if(col ["vip"]!=null)
////		vip = col ["vip"].ToString ();
//
//		if (col ["isfangchenmi"] != null)
//			isFangChenMi = col ["isfangchenmi"].ToString ();
//
////		if(col ["OS"]!=null)
////		OS = col ["OS"].ToString ();
//
////		if(col ["version"]!=null)
////		version = col ["version"].ToString ();
////		pushtoken = col ["pushtoken"].ToString ();
	}
	
}
