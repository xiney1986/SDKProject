using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/**
 * 激活码前台奖励信息管理器
 * @authro 陈世惟  
 * */
public class InviteCodeConfigManager : SampleConfigManager {
	
	//单例
	private static InviteCodeConfigManager instance;
	private List<InviteCode> list;
	
	public InviteCodeConfigManager ()
	{
		base.readConfig (ConfigGlobal.CONFIG_INVITE);
	}
	
	public static InviteCodeConfigManager Instance {
		get{
			if(instance==null)
				instance=new InviteCodeConfigManager();
			return instance;
		}
	}
	
	//获得所有激活进度奖励信息
	public List<InviteCode> getInviteCode ()
	{
		return list;
	}
	
	//解析配置
	public override void parseConfig (string str)
	{  
		InviteCode be = new InviteCode (str);
		if (list == null)
			list = new List<InviteCode> ();
		list.Add (be);
	}
}
