using UnityEngine;
using System.Collections;

public class GetMineralReportFport : BaseFPort {

	CallBack callback;
	
	public void access (string server_name,string rob_uid,long time,CallBack callback) {   
		ErlKVMessage message = new ErlKVMessage (FrontPort.MINERAL_GET_REPORT);   
		message.addValue("server_name", new ErlString(server_name));
		message.addValue("rob_uid", new ErlString(rob_uid));
        message.addValue("time",new ErlInt((int)time));
		this.callback = callback;
		access (message);
	}
	
	public override void read (ErlKVMessage message) { 
		string str = (message.getValue ("msg") as ErlAtom).Value;
		if (str == "none") {
			MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage("mining_no_report"));
		}
        if (callback != null) {
            callback();
        }
	}
}
