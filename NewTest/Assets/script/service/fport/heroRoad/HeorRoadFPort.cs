using UnityEngine;
using System.Collections;

public class HeroRoadFPort : BaseFPort
{
	private CallBack callback;

	public void getRoadActivation (CallBack callback)
	{
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.HERO_ROAD_GET_HR_ACTIVATION); 
		access (message);
	}

//	public void getRoadAwaken()
//	{
//		ErlKVMessage message = new ErlKVMessage (FrontPort.HERO_ROAD_GET_HR_AWAKEN); 
//		access (message);
//	}

	public override void read (ErlKVMessage message)
	{
		parseKVMsg (message);
		if (callback != null) {
			callback ();
		}
	}
	//解析ErlKVMessgae
	public void parseKVMsg (ErlKVMessage message)
	{
		HeroRoadManagerment.Instance.map.Clear ();
		ErlArray array = message.getValue ("msg") as ErlArray;
		if (array != null) {
			ErlType[] values = array.Value;
			ErlArray a,b;
			int sid,active,conquest;
			HeroRoad obj;
			for (int i = 0; i < values.Length; i++) {
				a = values [i] as ErlArray;
				sid = StringKit.toInt (a.Value [0].getValueString ());
				b = a.Value [1] as ErlArray;
				active = StringKit.toInt (b.Value [0].getValueString ());
				conquest = StringKit.toInt (b.Value [1].getValueString ());
				obj = new HeroRoad ();
				obj.sample = HeroRoadSampleManager.Instance.getSampleBySid (sid);
				obj.activeCount = active;
				obj.conquestCount = conquest;
				HeroRoadManagerment.Instance.map.Add (sid, obj);
			}
		}
	}
}
