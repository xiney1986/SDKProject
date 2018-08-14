using UnityEngine;
using System.Collections;

/**
 * 获得PVP信息排名接口
 * @author 汤琦
 * */
public class PvpRankInfoFPort : BaseFPort
{ 
	private CallBack callback;

	public void access (CallBack callback)
	{   
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.PVP_RANKINFO);
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{
		ErlType str = message.getValue ("msg") as ErlType;
		ErlArray array = str as ErlArray;
		int win = StringKit.toInt (array.Value [0].getValueString ());
		int rank = StringKit.toInt (array.Value [1].getValueString ());
		PvpInfoManagerment.Instance.createPvpRankInfo (win, rank);
		if (callback != null)
			callback ();
	}
}
