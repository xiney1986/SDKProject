using UnityEngine;
using System.Collections;

public class LastBattleUpdateFPort : BaseFPort
{
	private CallBack callBack;

	public void updateAccess(CallBack _callBack,int type)
	{
		this.callBack = _callBack;
		
		ErlKVMessage message = new ErlKVMessage (FrontPort.LASTBATTLEUPDATE);
		message.addValue("type",new ErlInt(type));
		access (message);
	}

	public override void read (ErlKVMessage message)
	{
		ErlType msg = message.getValue ("msg") as ErlType;
		if(msg is ErlAtom)
		{
			MessageWindow.ShowAlert (msg.getValueString ());
		}
		else
		{
			ErlArray info = msg as ErlArray;
			int type = StringKit.toInt( info.Value[0].getValueString());
			if(type == 1)
			{
				LastBattleManagement.Instance.newProcess = StringKit.toInt( info.Value[1].getValueString());
			}
			else if(type == 2)
			{
				LastBattleManagement.Instance.donationNextUpdateTime = StringKit.toInt( info.Value[1].getValueString());
				// 捐献列表//
				ErlArray donations = info.Value[2] as ErlArray;
				(FPortManager.Instance.getFPort("LastBattleInitFPort") as LastBattleInitFPort).updateDonationList(donations);
			}
			else if(type == 3)
			{
				LastBattleManagement.Instance.bossID = StringKit.toInt( info.Value[1].getValueString());
				LastBattleManagement.Instance.currentBossHP = StringKit.toLong( info.Value[2].getValueString());
				LastBattleManagement.Instance.bossTotalHP = StringKit.toLong( info.Value[3].getValueString());
			}
			if(callBack != null)
			{
				callBack();
			}
		}
	}
}
public class LastBattleUpdateType
{
	public const int PREPARE = 1;
	public const int DONATE = 2;
	public const int BOSS = 3;
}
