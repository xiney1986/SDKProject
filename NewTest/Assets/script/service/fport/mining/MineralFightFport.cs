using UnityEngine;
using System.Collections;

public class MineralFightFport : BaseFPort {

	CallBack<bool> callback;
	public void access (int fight_type,int war_type, CallBack<bool> call) {   
		ErlKVMessage message = new ErlKVMessage (FrontPort.MINERAL_FIGHT);

		message.addValue("fight_type",new ErlInt(fight_type));
		message.addValue("war_type",new ErlInt(war_type));
		
		this.callback = call;
		access (message);
	}

	public override void read (ErlKVMessage message)
	{ 
		string str = (message.getValue ("msg") as ErlAtom).Value;
		
		if (str == "ok") {
			FuBenManagerment.Instance.isMineralAttack = true;
			if (callback != null) {
				callback (true);
			}
            return;
		}
        else if (str == "timeout")
        {
            MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("mining_timeout"));
		}
        else if (str == "pvp_error")
        {
            MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("pvpuse00"));
        }
        else if (str == "info_error")
        {
            MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("mining_info_error"));
        }
        else if (str == "info_unvolid")
        {
            MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("mining_info_unvolid"));
        } else if (str == "none") {
            MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("mining_info_error"));
        } else if (str == "fighted") {
            MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("mining_info_unvolid"));
        }

        if (callback != null) {
            callback(false);
        }
	}
}
