using UnityEngine;
using System.Collections;

public class UpdateLastBattleService :BaseFPort
{
	public UpdateLastBattleService ()
	{
		
	}

	public override void read (ErlKVMessage message)
	{
		string msg = (message.getValue("type") as ErlType).getValueString();
		if(msg == "refresh_shop")
		{
			LastBattleUpdateFPort init = FPortManager.Instance.getFPort ("LastBattleUpdateFPort") as LastBattleUpdateFPort;
			init.updateAccess(()=>{
				//LastBattleManagement.Instance.isUpdateDonationList = true;
				PlayerPrefs.SetInt(LastBattleManagement.lastbattleDonationKey,1);
			},LastBattleUpdateType.DONATE);
		}
		else if(msg == "boss_open")
		{
			if(UiManager.Instance.getWindow<LastBattleWindow>() != null)
			{
				LastBattleInitFPort init = FPortManager.Instance.getFPort ("LastBattleInitFPort") as LastBattleInitFPort;
				init.lastBattleInitAccess(()=>{
					if(UiManager.Instance.getWindow<LastBattleWindow>().gameObject.activeSelf)
					{
						UiManager.Instance.backGround.switchBackGround("ChouJiang_BeiJing");
						UiManager.Instance.getWindow<LastBattleWindow>().showDetialBtnTips();
						UiManager.Instance.getWindow<LastBattleWindow>().updateBossBattlePanel();
					}
				});
			}
		}
	}
}
