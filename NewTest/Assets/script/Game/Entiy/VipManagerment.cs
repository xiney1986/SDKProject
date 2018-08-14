using UnityEngine;
using System.Collections;

public class VipManagerment
{
	//已经领取的奖励sid
	int[] awardSids;

	public static VipManagerment Instance {
		get{return SingleManager.Instance.getObj("VipManagerment") as VipManagerment;}
	}

	//true表示已领取
	public bool alreadyGetAward(int awardSid){
		for (int i=0; i<awardSids.Length; i++) {
			if(awardSids[i] == awardSid)
				return true;
		}
		return false;
	}
	
	public int getMaxLevel ()
	{
		return  VipConfigManager.Instance.getVipInfos ().Length;
	}

	public int[] getAwardSids()
	{
		return awardSids;
	}

	public void setAwardSids(int[] sids){
		awardSids = sids;
	}

	public void addAwardSids(int sid){
		if (awardSids == null || awardSids.Length < 1)
			awardSids = new int[]{sid};
		else {
			int[] temp = new int[awardSids.Length + 1];
			System.Array.Copy(awardSids,0,temp,0,awardSids.Length);
			temp[temp.Length-1] = sid;
			awardSids = temp;
		}
	}

	public Vip[] getAllVip ()
	{
		return VipConfigManager.Instance.getVipInfos ();
	}

	public Vip getVipbyLevel (int lv)
	{
		if (lv < 1)
			return null;
		
		return VipConfigManager.Instance.getVipInfos () [lv - 1];
	}
	
	public string vipLevelToSpriteName (int level)
	{
		switch (level) {
		case 0:
			return "vip_0";
		case 1:
			return "vip_1";
		case 2:
			return "vip_2";
		case 3:
			return "vip_3";
		case 4:
			return "vip_4";
		case 5:
			return "vip_5";
		}
		
		return "vip_-1";
	}

	//统一处理vip升级特权情况
	public void updateLevel (int oldLevel, int newLevel)
	{
		vipPrivilege _vipPrivilege1 = getVipbyLevel (newLevel).privilege;
		Vip _vip2 = getVipbyLevel (oldLevel);
		vipPrivilege _vipPrivilege2 = _vip2 == null ? null : _vip2.privilege;
		//增加讨伐次数
//		if (FuBenManagerment.Instance.getWarInfos () != null)
//			FuBenManagerment.Instance.getWarChapter ().addNum (_vipPrivilege2 == null ? _vipPrivilege1.bossCountAdd : _vipPrivilege1.bossCountAdd - _vipPrivilege2.bossCountAdd);
		StorageManagerment.Instance.updateRoleStorageMaxSpace (_vipPrivilege2 == null ? _vipPrivilege1.cardStoreAdd : _vipPrivilege1.cardStoreAdd - _vipPrivilege2.cardStoreAdd);
		StorageManagerment.Instance.updateEquipStorageMaxSpace (_vipPrivilege2 == null ? _vipPrivilege1.equipStoreAdd : _vipPrivilege1.equipStoreAdd - _vipPrivilege2.equipStoreAdd);
		UserManager.Instance.self.updatePvEPointMax (_vipPrivilege2 == null ? _vipPrivilege1.pveAdd : _vipPrivilege1.pveAdd - _vipPrivilege2.pveAdd);
		FriendsManagerment.Instance.getFriends().addMaxSize(_vipPrivilege2 == null ? _vipPrivilege1.friendAdd : _vipPrivilege1.friendAdd - _vipPrivilege2.friendAdd);
	}

	
}
