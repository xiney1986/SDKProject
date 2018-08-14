using UnityEngine;
using System.Collections;

public class MiniGetStarInfoFPort : MiniBaseFPort
{
	CallBack readBack;

	public void access (CallBack cb)
	{
		readBack = cb;
		ErlKVMessage message = new ErlKVMessage (FrontPort.STAR_GET);
		access (message);
	}

	public void parseKVMsg (ErlKVMessage message)
	{
	}

	public override void read (ErlKVMessage message)
	{
//        ErlList e = message.getValue("msg") as ErlList;
//        HoroscopesManager.Instance.setPrayTime(StringKit.toInt(((e.Value.GetValue(0) as ErlArray).Value.GetValue(1) as ErlType).getValueString()));
//        ErlArray e2 = (e.Value.GetValue(1) as ErlArray).Value.GetValue(1) as ErlArray;
//        HoroscopesManager.Instance.setBeginTime(StringKit.toInt((e2.Value.GetValue(0) as ErlType).getValueString()));
//        HoroscopesManager.Instance.setEndTime(StringKit.toInt((e2.Value.GetValue(1) as ErlType).getValueString()));
		parseKVMsg (message);
		if (readBack != null)
			readBack ();
	} 
}
 