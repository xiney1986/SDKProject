using System;

/// <summary>
/// 天梯玩家信息封装
/// </summary>
public class LaddersPlayerInfo
{

	public PvpOppInfo playerInfo;

	public int uiIndex=0;
	public int index=0;
	public int belongChestIndex;
	public bool isDefeated;
	public int rank;
	public string uid;
	public int level;
	public int vip;
	public int headIconId;
	public PlatFormUserInfo sdkInfo;

	public string getHeadIconPath ()
	{
		if(sdkInfo==null)
		return UserManager.Instance.getIconPath (headIconId);
		else
			return  sdkInfo.face;
	}
	public LaddersPlayerInfo ()
	{

	}
}


