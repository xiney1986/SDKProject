using UnityEngine;
using System.Collections;

public class GetMineralsFport :BaseFPort{

	CallBack callback;
	
	public void access (CallBack callback) {   
		ErlKVMessage message = new ErlKVMessage (FrontPort.GET_MINERALS);   
		this.callback = callback;
		access (message);
	}
	
	public override void read (ErlKVMessage message) { 
		ErlType data = message.getValue ("msg") as ErlType;
		if (data != null && data is ErlArray) {
			ErlArray arr = data as ErlArray;
            MiningManagement.Instance.SearchTimes =StringKit.toInt(arr.Value[0].getValueString());
            ErlArray arr_mineral = arr.Value[1] as ErlArray;
            for (int i = 0; i < arr_mineral.Value.Length; i++) {
                ErlArray mineral = arr_mineral.Value[i] as ErlArray;
				MineralInfo info = new MineralInfo ();
				info.localId = StringKit.toInt (mineral.Value [0].getValueString ());
				info.sid = StringKit.toInt (mineral.Value [1].getValueString ());
				info.armyId = StringKit.toInt (mineral.Value [2].getValueString ());
				info.startTime = StringKit.toLong (mineral.Value [3].getValueString ());
				info.balanceTime = StringKit.toLong (mineral.Value [4].getValueString ());
				info.balanceCount = StringKit.toInt (mineral.Value [5].getValueString ());
                MiningManagement.Instance.minerals[info.localId] = null;
				MiningManagement.Instance.AddMineral (info);
			}
			if (this.callback != null) {
				callback ();
			}
		}
	}
}

