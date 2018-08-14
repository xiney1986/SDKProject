using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 获得PVP信息接口
 * @author 汤琦
 * */
public class MiniPvpGetInfoFPort : MiniBaseFPort
{
	 
	private CallBack callback;

	public void access (CallBack callback)
	{   
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.PVPGET_INFO);
		access (message);
	}

	public void parseKVMsg (ErlKVMessage message)
	{
	}

	public override void read (ErlKVMessage message)
	{
//		ErlType str = message.getValue ("msg") as ErlType;
//		if(str.getValueString() == "ok")
//		{
//			ErlType type = message.getValue ("info") as ErlType;
//			if(type is ErlArray)
//			{
//				ErlArray array = type as ErlArray;
//				int time = StringKit.toInt(array.Value[0].getValueString());
//				string rule = array.Value[1].getValueString();
//				int round = StringKit.toInt(array.Value[2].getValueString());
//				ErlArray arrays = array.Value[3] as ErlArray;
//				int max = arrays.Value.Length;
//				int index = 0;
//				List<PvpOppInfo> oppList = new List<PvpOppInfo>();
//				for (int i = 0; i < max; i++) {
//					ErlArray list = arrays.Value[i] as ErlArray;
//					string uid = list.Value[index++].getValueString();
//					string name = list.Value[index++].getValueString();
//                    string guildName = list.Value[index++].getValueString();
//					int headIcon = StringKit.toInt(list.Value[index++].getValueString());
//					long exp = StringKit.toLong(list.Value[index++].getValueString());
//					int state = StringKit.toInt(list.Value[index++].getValueString());
//					ErlArray lists = list.Value[index++] as ErlArray;
//					int combat = StringKit.toInt(list.Value[index++].getValueString());
//					index = 0;
//					int formation = StringKit.toInt(lists.Value[index++].getValueString());
//					ErlArray bArray =  lists.Value[index++] as ErlArray;
//					int beastSid = 0;
//					int beastExp = 0;
//					string beastUid = "";
//					if(bArray.Value.Length != 0)
//					{
//						beastSid = StringKit.toInt(bArray.Value[0].getValueString());
//						beastExp = StringKit.toInt(bArray.Value[1].getValueString());
//						beastUid = bArray.Value[2].getValueString();
//					}
//					ErlArray tArray = lists.Value[index++] as ErlArray;
//					index = 0;
//					List<PvpOppTeam> ptList = new List<PvpOppTeam>();
//					for (int j = 0; j < tArray.Value.Length; j++) {
//						ErlArray ea = tArray.Value[j] as ErlArray;
//						if(ea.Value.Length != 0)
//						{
//							int teamOppSid = StringKit.toInt(ea.Value[0].getValueString());
//							int teamOppExp = StringKit.toInt(ea.Value[1].getValueString());
//							string teamOppUid = ea.Value[2].getValueString();
//							int evoLevel = StringKit.toInt(ea.Value[3].getValueString());
//							int surLevel = StringKit.toInt(ea.Value[4].getValueString());
//							int teamOppIndex = j;
//							PvpOppTeam pt = new PvpOppTeam(teamOppSid,teamOppExp,teamOppIndex,teamOppUid,evoLevel,surLevel);
//							ptList.Add(pt);
//						}
//					}
//                    oppList.Add(new PvpOppInfo(uid, name, guildName, headIcon, exp, state, formation, beastSid, beastExp, beastUid, ptList.ToArray(), combat));
//				}
//				PvpInfoManagerment.Instance.createPvpInfo(time,rule,round,oppList.ToArray());
//			}
		parseKVMsg (message);
		if (callback != null) {
			callback ();
		}
		
	}
	
}
