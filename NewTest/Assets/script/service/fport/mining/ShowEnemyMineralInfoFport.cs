using UnityEngine;
using System.Collections;

public class ShowEnemyMineralInfoFport : BaseFPort
{

	CallBack callback;
	
	public void access (string node, string rob_uid, string local, CallBack callback)
	{   
		ErlKVMessage message = new ErlKVMessage (FrontPort.MINERAL_SHOW_ENEMY_MINERAL); 
		message.addValue ("node", new ErlString (node));
		message.addValue ("rob_uid", new ErlString (rob_uid));
		message.addValue ("local", new ErlString (local));
		this.callback = callback;
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{ 
		ErlType data = message.getValue ("msg") as ErlType;
		if (data != null) {
			if (!(data is  ErlArray)) {

				string str = (data as ErlAtom).Value;
				if (str == "none") {
					MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("mining_no_enemy_team"));
				} else if (str == "rob_close") {
					MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("mining_rob_close"));
				}
			} else {
				ErlArray arr = data as ErlArray;
				EnemyMineralInfo info = new EnemyMineralInfo ();
				int i = 0;
				info.serverName = arr.Value [i++].getValueString ();
				info.playerName = arr.Value [i++].getValueString ();
				info.HeadIconId = StringKit.toInt (arr.Value [i++].getValueString ());
				info.playerLevel = arr.Value [i++].getValueString ();
				info.localId = StringKit.toInt (arr.Value [i++].getValueString ());
				info.sid = StringKit.toInt (arr.Value [i++].getValueString ());
				info.combat = StringKit.toInt (arr.Value [i++].getValueString ());
				//info.count = StringKit.toInt(arr.Value[i++].getValueString());

				ErlList counts = arr.Value [i++] as ErlList;
				for (int j = 0; j < counts.Value.Length; j++) {
					ErlArray arr_1 = counts.Value [j] as ErlArray;
					if (arr_1.Value [0].getValueString () == "goods") {
						ErlArray goods = arr_1.Value [1] as ErlArray;
						info.count = StringKit.toInt (goods.Value [1].getValueString ());
					} else if (arr_1.Value [0].getValueString () == "money") {
						info.count = StringKit.toInt (arr_1.Value [1].getValueString ());
					}
				}

				ErlArray teamInfo = arr.Value [i++] as ErlArray;
				ErlArray tempArray;
				for (int j=0; j<teamInfo.Value.Length; j++) {
					//地雷，暂时这么处理
					tempArray = teamInfo.Value [j] as ErlArray;
					if (tempArray == null) {
						info.roles [j] = "0";
						info.evoLv [j] = "0";
					} else {
						info.roles [j] = tempArray.Value [0].getValueString ();
						info.evoLv [j] = tempArray.Value [1].getValueString ();
					}
				}
				MiningManagement.Instance.AddEnemyMineral (info);
				if (this.callback != null) {
					callback ();
				}
			}
		}
	}
}
