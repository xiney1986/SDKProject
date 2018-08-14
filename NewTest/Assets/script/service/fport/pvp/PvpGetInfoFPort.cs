using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 获得PVP信息接口
 * @author 汤琦
 * */
public class PvpGetInfoFPort : BaseFPort
{
	 
	private CallBack<bool> callbackBool;
	private CallBack callbackVoid;

	public void access (CallBack<bool> callbackBool)
	{   
		this.callbackBool = callbackBool;
		ErlKVMessage message = new ErlKVMessage (FrontPort.PVPGET_INFO);
		access (message);
	}

	public void access (CallBack callbackVoid)
	{   
		this.callbackVoid = callbackVoid;
		ErlKVMessage message = new ErlKVMessage (FrontPort.PVPGET_INFO);
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{
		bool hasPvp = parseKVMsg (message);

		if (callbackBool != null) {
			callbackBool (hasPvp);
			callbackBool = null;
		}

		if (callbackVoid != null) {
			callbackVoid ();
			callbackVoid = null;
		}
	}
	//解析ErlKVMessgae
	public bool parseKVMsg (ErlKVMessage message)
	{
		string str = (message.getValue ("msg") as ErlType).getValueString ();
		bool hasPvp = false;
		if (str == "ok") {
			hasPvp = true;
			ErlType type = message.getValue ("info") as ErlType;
			if (type is ErlArray) {
				ErlArray array = type as ErlArray;
				//pvp信息过期时间戳
				int time = StringKit.toInt (array.Value [0].getValueString ());
				string rule = array.Value [1].getValueString ();
				int round = StringKit.toInt (array.Value [2].getValueString ());
				ErlArray arrays = array.Value [3] as ErlArray;
				int max = arrays.Value.Length;
				List<PvpOppInfo> oppList = new List<PvpOppInfo> ();
				for (int i = 0; i < max; i++) {
					ErlArray list = arrays.Value [i] as ErlArray;
					oppList.Add (PvpOppInfo.pares (list));
				}
				PvpInfoManagerment.Instance.createPvpInfo (time, rule, round, oppList.ToArray ());
			}
		} else if (str == "no_pvp") {
			PvpInfoManagerment.Instance.clearDate ();
		}
		return hasPvp;
	}
	
}
