using UnityEngine;
using System.Collections;

public class GetStarInfoFPort : BaseFPort
{
	CallBack readBack;

	public void access (CallBack cb)
	{
		readBack = cb;
		ErlKVMessage message = new ErlKVMessage (FrontPort.STAR_GET_PRAY2);
		access (message);
	}
	//\"msg\":[109,[[5,11423],[8,11423],[10,11423],[11,11425]],0,36000,79200]}
	// 109=dy [[星座id,奖励sid],...],0=cd,36000=start,79200=end 
	public override void read (ErlKVMessage message)
	{
		parseKVMsg (message);
		if (readBack != null)
			readBack ();
	} 
	//解析ErlKVMessgae
	public void parseKVMsg (ErlKVMessage message)
	{
		ErlArray array = message.getValue ("msg") as ErlArray;
		HoroscopesManager.Instance.initData (array);
	}
}
 