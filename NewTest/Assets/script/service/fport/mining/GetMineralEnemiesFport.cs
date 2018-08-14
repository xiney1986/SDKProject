using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GetMineralEnemiesFport : BaseFPort {

	CallBack callback;
	
	public void access (CallBack callback) {   
		ErlKVMessage message = new ErlKVMessage (FrontPort.MINERAL_GET_ENEMIES);   
		this.callback = callback;
		access (message);
	}
	
	public override void read (ErlKVMessage message) { 
		ErlType data = message.getValue ("msg") as ErlType;
		if (data != null) {
			if (!(data is  ErlArray)) {
                string str = (data as ErlAtom).Value;
                if (str == "timeout") {
                    MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("mining_timeout"));
                }

			}
			else {
                
                ErlArray arr = data as ErlArray;
                MiningManagement.Instance.NewEnemyNum = StringKit.toInt(arr.Value[0].getValueString());
                MiningManagement.Instance.ClearEnemyInfoList();
                ErlArray peies = arr.Value[1] as ErlArray;
                for (int i = 0; i < peies.Value.Length; i++) {
                    ErlArray info = peies.Value[i] as ErlArray;
					int k = 0;
					PillageEnemyInfo pei = new PillageEnemyInfo();
					pei.node = info.Value[k++].getValueString();
					pei.RoleUid =  info.Value[k++].getValueString();
					pei.HeadIconId = StringKit.toInt( info.Value[k++].getValueString());
					pei.playerName =  info.Value[k++].getValueString();
					pei.serverName =  info.Value[k++].getValueString();
					pei.time =StringKit.toLong( info.Value[k++].getValueString());
					pei.sid = StringKit.toInt( info.Value[k++].getValueString());
					pei.count = StringKit.toInt( info.Value[k++].getValueString());
					ErlArray minerals = info.Value[k++] as ErlArray;
					for(int j=0;j< minerals.Value.Length; j++){
						ErlArray mineral = minerals.Value[j] as ErlArray;
						pei.minerals.Add(StringKit.toInt(mineral.Value[0].getValueString()),StringKit.toInt(mineral.Value[1].getValueString()));
					}

					MiningManagement.Instance.AddEnemyInfoList(pei);
				}
			}
		}
		if (this.callback != null) {
			callback ();
		}
	}
}
