using UnityEngine;
using System.Collections;

/**
 * 公会捐献接口
 * @author 汤琦
 * */
public class GuildDonationFPort : BaseFPort
{
	private CallBack callback;
	
	public void access (string donation,CallBack callback)
	{   
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.GUILD_DONATION);
		message.addValue ("donation", new ErlString (donation));
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{
		ErlType type = message.getValue ("msg") as ErlType;
		if(type.getValueString() == "non_mem")
		{
			UiManager.Instance.openDialogWindow<MessageWindow>( (win)=>{
				win.initWindow(1,LanguageConfigManager.Instance.getLanguage("s0093"),null,LanguageConfigManager.Instance.getLanguage("Guild_90"),GuildManagerment.Instance.closeAllGuildWindow);
			});
		}
		else if(type is ErlArray)
		{
			ErlArray array = type as ErlArray;
			GuildManagerment.Instance.donationResult(array);
			if(callback != null)
				callback();
		}
		else if(type.getValueString() == "not_consume") // 下次优化，应该提前判断消耗
		{
			UiManager.Instance.openDialogWindow<MessageWindow>( (win)=>{
				win.initWindow(2,LanguageConfigManager.Instance.getLanguage("s0040"),LanguageConfigManager.Instance.getLanguage("s0315"),LanguageConfigManager.Instance.getLanguage("Guild_85"),recharge);
			});
		}
	}

	private void recharge(MessageHandle msg)
	{
		if(msg.buttonID == MessageHandle.BUTTON_LEFT)
			return;
		UiManager.Instance.openWindow<rechargeWindow>();
	}
}
