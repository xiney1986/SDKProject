using UnityEngine;
using System.Collections;

public class HeroRoadIntoFPort : BaseFPort {

	public CallBack<bool> callback;

	public void intoRoad(int type,int arrayid,CallBack<bool> callback)
	{
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.HERO_ROAD_INTO_HR); 
		message.addValue ("type",new ErlString(type.ToString()));
		message.addValue ("arrayid",new ErlInt(arrayid));
		access (message);
	}

	public override void read (ErlKVMessage message)
	{
		string str = (message.getValue ("msg") as ErlAtom).Value;  

		if (callback != null) {
			if(str == "fight")
				callback(true);
			else if(str == "into_fb")
				callback(false);
			else{
				MessageWindow.ShowAlert (str);
				if (callback != null)
					callback = null;
			}
		}
	}
}
