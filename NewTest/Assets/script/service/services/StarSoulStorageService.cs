using UnityEngine;
using System.Collections;

/// <summary>
/// 星魂仓库服务
/// </summary>
public class StarSoulStorageUpdateService : BaseFPort {
	
	public StarSoulStorageUpdateService () {
		
	}
	
	public override void read (ErlKVMessage message){
		ErlArray starsouls = message.getValue ("storage") as ErlArray;
		if (starsouls != null) {
			StarSoulStorage starSoulStorage = new StarSoulStorage ();
			starSoulStorage.parse (starsouls);
		}
	}
}
