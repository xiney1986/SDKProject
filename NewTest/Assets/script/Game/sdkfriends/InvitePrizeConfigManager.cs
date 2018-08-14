using System;
using System.Collections;
using System.Collections.Generic;

public class InvitePrizeConfigManager : SampleConfigManager
{
	private static InvitePrizeConfigManager instance;
	private List<InvitePrize> list;

	//返利钻石
	public int rebatesMoney;
	//返利信息
	public string rebatesMsg;
	//是否可以领取
	public bool isgetrebateMoney;
	
	public InvitePrizeConfigManager ()
	{
		base.readConfig (ConfigGlobal.CONFIG_INVITEPRIZE);
	}
	
	public static InvitePrizeConfigManager Instance {
		get{
			if(instance==null)
				instance=new InvitePrizeConfigManager();
			return instance;
		}
	}
	//获得等级奖品信息
	public InvitePrize[] getInvitePrizes ()
	{
		return list.ToArray ();
	}
	
	//解析配置
	public override void parseConfig (string str)
	{  
		InvitePrize be = new InvitePrize (str);
		if (list == null)
			list = new List<InvitePrize> ();
		list.Add (be);
	}
	
}