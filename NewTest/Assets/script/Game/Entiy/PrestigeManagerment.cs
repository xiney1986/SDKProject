using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PrestigeManagerment
{
	//对应的exp索引
	public const int EXPSID=1;
	public static PrestigeManagerment Instance {
		get{ return SingleManager.Instance.getObj ("PrestigeManagerment") as PrestigeManagerment;}
	}

	List<Prestige> userPrestigeList;


	/// <summary>
	/// 通过等级获得对应称号
	/// </summary>
	public Prestige createPrestigeByLevel (long exp)
	{
		Prestige p = new Prestige (exp);
		return p;
	}

	//前台显示所有称号时用
	public  List<Prestige> getUserPrestige ()
	{
		return userPrestigeList;
	}

	//后台数据传输过来
	public  void  setUserPrestige (List<Prestige> list)
	{
		userPrestigeList = list;
	}

}
