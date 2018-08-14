using UnityEngine;
using System.Collections;

public class StartMiningFPort : BaseFPort {

	CallBack callback;

	public void access (int sid, int armyId, int local, CallBack callback) {   
		ErlKVMessage message = new ErlKVMessage (FrontPort.OPEN_MINERAL);   
		message.addValue ("sid", new ErlInt (sid));
		message.addValue ("array_id", new ErlInt (armyId));
		message.addValue ("local", new ErlInt (local));
		this.callback = callback;
		access (message);
	}


	public void clear(){
		ErlKVMessage message = new ErlKVMessage (FrontPort.CLEARDATA);   
		access (message);
	}
	public override void read (ErlKVMessage message) { 
		ErlType data = message.getValue ("msg") as ErlType;
		if (data != null) {
            if (!(data is ErlArray))
            {
                string str = (data as ErlAtom).Value;
                if (str == "limit") {
                    MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("mining_limit"));
                }else if(str == "sid_error"){
                    MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("mining_sid_error"));
                }
                else if (str == "no_array")
                {
                    MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("mining_no_array"));
                }
                else if (str == "array_busy")
                {
                    MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("mining_array_busy"));
                }
                else if (str == "local_exist")
                {
                    MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("mining_local_exist"));
                }


			}
			else {
				ErlArray arr = data as ErlArray;
				MineralInfo info = new MineralInfo ();
				info.localId = StringKit.toInt (arr.Value [0].getValueString ());
				info.sid = StringKit.toInt (arr.Value [1].getValueString ());
				info.armyId = StringKit.toInt (arr.Value [2].getValueString ());
				info.startTime = StringKit.toLong (arr.Value [3].getValueString ());
				info.balanceTime = StringKit.toLong (arr.Value [4].getValueString ());
				info.balanceCount = StringKit.toInt (arr.Value [5].getValueString ());
				MiningManagement.Instance.AddMineral (info);
				if (this.callback != null) {
					callback ();
				}
			}
		}
	}
}
