﻿using UnityEngine;
using System.Collections;

public class MiniHeroRoadFPort : MiniBaseFPort
{
	private CallBack callback;

	public void getRoadActivation (CallBack callback)
	{
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.HERO_ROAD_GET_HR_ACTIVATION); 
		access (message);
	}

	public void getRoadAwaken ()
	{
		ErlKVMessage message = new ErlKVMessage (FrontPort.HERO_ROAD_GET_HR_AWAKEN); 
		access (message);
	}

	public void parseKVMsg (ErlKVMessage message)
	{
	}

	public override void read (ErlKVMessage message)
	{
//		ErlArray array = message.getValue ("msg") as ErlArray;
		parseKVMsg (message);
		if (callback != null) {
			callback ();
		}
	}

}
