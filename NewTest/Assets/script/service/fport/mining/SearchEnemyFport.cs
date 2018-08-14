using UnityEngine;
using System.Collections;

public class SearchEnemyFport : BaseFPort {

	CallBack<bool> callback;
	
	public void access (CallBack<bool> callback) {   
		ErlKVMessage message = new ErlKVMessage (FrontPort.MINERAL_SEARCH_ENEMY);  
		this.callback = callback;
		access (message);
	}

	public override void read (ErlKVMessage message) { 
		ErlType data = message.getValue ("msg") as ErlType;
		if (data != null) {
			if (!(data is  ErlArray)) {
                string str = (data as ErlAtom).Value;
                if (str == "center_logout")
                {
                    MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("mining_no_enemy_team"));
                }
                else if (str == "search_consume_limit")
                {
                    MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("search_consume_limit"));
                }
                else if (str == "none")
                {
                    MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("mining_no_enemy_team"));
                }
                else if (str == "timeout")
                {
                    MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("mining_timeout"));
                } else if (str == "rob_close") {
                    MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("mining_rob_close"));
                }

                if (this.callback != null) {
                    callback(false);
                }
			}
			else {
				ErlArray arr = data as ErlArray;
				EnemyMineralInfo info =new EnemyMineralInfo();
				int i=0;
				info.serverName = arr.Value[i++].getValueString();
				info.playerName = arr.Value[i++].getValueString();
				info.HeadIconId = StringKit.toInt(arr.Value[i++].getValueString());
				info.playerLevel = arr.Value[i++].getValueString();
				info.localId =StringKit.toInt(arr.Value[i++].getValueString());
				info.sid = StringKit.toInt(arr.Value[i++].getValueString());
				info.combat = StringKit.toInt(arr.Value[i++].getValueString());
				info.count = StringKit.toInt(arr.Value[i++].getValueString());
				ErlArray teamInfo = arr.Value[i++] as ErlArray;
				for(int j=0; j<teamInfo.Value.Length ;j++){
                    ErlArray role = teamInfo.Value[j] as ErlArray;
                    info.roles[j] = "0";
                    info.evoLv[j] = "0";
                    if (role != null) {
                        info.roles[j] = role.Value[0].getValueString();
                        info.evoLv[j] = role.Value[1].getValueString();
                    }
				}
				MiningManagement.Instance.AddEnemyMineral(info);
                MiningManagement.Instance.SearchTimes++; //ËÑË÷´ÎÊý¼Ó1
				if (this.callback != null) {
					callback (true);
				}
			}
		}
	}
}
