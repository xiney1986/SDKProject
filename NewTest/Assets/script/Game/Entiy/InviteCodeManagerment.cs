using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 邀请激活码管理器
 * @authro 陈世惟  
 * */
public class InviteCodeManagerment {

	public string inviteNum;//被邀请数量
	public int inviteType;//激活奖励：0未领取，1已领取
	
	public static InviteCodeManagerment Instance {
		get{return SingleManager.Instance.getObj("InviteCodeManagerment") as InviteCodeManagerment;}
	}
	
	//获得所有前台奖励信息
	public List<InviteCode> getAllInviteCode()
	{
		return InviteCodeConfigManager.Instance.getInviteCode();
	}
	
	//根据标签页获取相应的数据，用于邀请码与互粉
	public List<InviteCode> getAllInviteCodeByTapType(int _type)
	{
		List<InviteCode> ic = InviteCodeConfigManager.Instance.getInviteCode();
		
		List<InviteCode> newic = new List<InviteCode>();
		InviteCode icItem = null;
		
		for(int j=0;j<ic.Count;j++)
		{
			if(StringKit.toInt(ic[j].tapType) == _type)
				icItem = ic[j];
			newic.Add(icItem);
		}
		return newic;
	}
	/// <summary>
	/// 判断是否有邀请奖励可领取
	/// </summary>
	/// <returns><c>true</c>, if invitation award was hased, <c>false</c> otherwise.</returns>
	public bool hasInvitationAward()
	{
		List<InviteCode> invitationList = getAllInviteCodeByTapType(1);
		for (int i = 0; i < invitationList.Count; i++) {
			if (invitationList [i].awardType == 0 && invitationList[i].jindu >= StringKit.toInt(invitationList[i].inviteNeedNum)) {
				return true;
			}
		}
		return false;
	}

	//初始化前后台邀请奖励数据，进行整合
	public void initInviteCodeInfo(ErlArray erlarr)
	{
		List<InviteCode> ic = InviteCodeConfigManager.Instance.getInviteCode();
		inviteNum = erlarr.Value[0].getValueString();
		ErlArray lists = erlarr.Value[1] as ErlArray;
		inviteType = StringKit.toInt(erlarr.Value[2].getValueString());
		int sid;
		
		for(int i=0;i<lists.Value.Length;i++)
		{
			sid = StringKit.toInt((lists.Value[i] as ErlArray).Value[0].getValueString());
			for(int j=0;j<ic.Count;j++)
			{
				if(StringKit.toInt(ic[j].uid) == sid)
				{
					ic[j].jindu = StringKit.toInt((lists.Value[i] as ErlArray).Value[1].getValueString());
					ic[j].awardType = StringKit.toInt((lists.Value[i] as ErlArray).Value[2].getValueString());
				}
			}
		}
	}
}
