using System;
using UnityEngine;
using System.Collections.Generic;

/**
 * 单列管理，创建单列都可以走这个----适用于数据会被清理或者释放的单例类
 * @author zhoujie
 * */
public class SingleManager
{
	//单例
	private static SingleManager _Instance;
	//存储所有单列对象
	private static Dictionary<string,object> singleObj;
	//私有构造函数
	private SingleManager ()
	{
	}
	//获得单列管理器对象
	public static SingleManager Instance {
		get {
			if (_Instance == null) {
				singleObj = new Dictionary<string,object> ();
				_Instance = new SingleManager ();
				return _Instance;
			} else
				return _Instance;
		}
		set { 
			_Instance = value;
		}
	}
	/// <summary>
	/// 校验指定类名的类对象
	/// </summary>
	public bool checkObj (string key)
	{
		return singleObj.ContainsKey (key);
	}
	/// <summary>
	/// 获得指定类名的类对象
	/// </summary>
	public object getObj (string key)
	{
		if (!singleObj.ContainsKey (key))
			singleObj.Add (key, DomainAccess.getObject (key));
		return singleObj [key];
	}
	//清理所有
	public void clean ()
	{
		_Instance = null;
		GC.Collect ();
	}
	//清理指定
	public void clean (string key)
	{
		singleObj.Remove (key);
	}
}

