using UnityEngine;
using System.Collections;

public class MiniRankFPort : MiniBaseFPort 
{
	private CallBack callback;

	public void access (CallBack callback)
	{   
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.RANK_GET);
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{
//		RankManagerment.Instance.combatList.Clear ();
//		RankManagerment.Instance.guildList.Clear ();
//		RankManagerment.Instance.moneyList.Clear ();
//		RankManagerment.Instance.pvpList.Clear ();
//		RankManagerment.Instance.roleList.Clear ();
//
//		long minNextUpdateTime = long.MaxValue;
//		ErlArray array = message.getValue ("msg") as ErlArray;
//		ErlType[] types = array.Value;
//		for (int i = 0; i < types.Length; i++) {
//			ErlArray arlItem = types[i] as ErlArray;
//			int type = StringKit.toInt(arlItem.Value[0].getValueString());
//			long nextUpdateTIme = StringKit.toInt(arlItem.Value[1].getValueString());
//			minNextUpdateTime = nextUpdateTIme < minNextUpdateTime ? nextUpdateTIme : minNextUpdateTime;
//			ErlArray values = arlItem.Value[2] as ErlArray;
//			for(int j = 0; j < values.Value.Length; j++)
//			{
//				ErlArray vs = values.Value[j] as ErlArray;
//				switch(type)
//				{
//				case RankManagerment.TYPE_COMBAT:
//					RankItemCombat rankItemCombat = new RankItemCombat();
//					rankItemCombat.uid = vs.Value[0].getValueString();
//					rankItemCombat.name = vs.Value[1].getValueString();
//					rankItemCombat.combat = StringKit.toInt(vs.Value[2].getValueString());
//					RankManagerment.Instance.combatList.Add(rankItemCombat);
//					if(rankItemCombat.uid == UserManager.Instance.self.uid)
//						RankManagerment.Instance.myRank[0] = j + 1;
//					break;
//
//				case RankManagerment.TYPE_PVP:
//					RankItemPVP rankItemPVP = new RankItemPVP();
//					rankItemPVP.uid = vs.Value[0].getValueString();
//					rankItemPVP.name = vs.Value[1].getValueString();
//					rankItemPVP.win = StringKit.toInt(vs.Value[2].getValueString());
//					RankManagerment.Instance.pvpList.Add(rankItemPVP);
//                    if(rankItemPVP.uid == UserManager.Instance.self.uid)
//                        RankManagerment.Instance.myRank[1] = j + 1;
//					break;
//
//				case RankManagerment.TYPE_MONEY:
//					RankItemMoney rankItemMoney = new RankItemMoney();
//					rankItemMoney.uid = vs.Value[0].getValueString();
//					rankItemMoney.name = vs.Value[1].getValueString();
//					rankItemMoney.money = StringKit.toInt(vs.Value[2].getValueString());
//					RankManagerment.Instance.moneyList.Add(rankItemMoney);
//                    if(rankItemMoney.uid == UserManager.Instance.self.uid)
//                        RankManagerment.Instance.myRank[2] = j + 1;
//					break;
//
//				case RankManagerment.TYPE_ROLE:
//					RankItemRole rankItemRole = new RankItemRole();
//					rankItemRole.uid = vs.Value[0].getValueString();
//					rankItemRole.name = vs.Value[1].getValueString();
//					rankItemRole.cardUid = vs.Value[2].getValueString();
//					rankItemRole.cardName = vs.Value[3].getValueString();
//					RankManagerment.Instance.roleList.Add(rankItemRole);
//                    if(rankItemRole.uid == UserManager.Instance.self.uid)
//                        RankManagerment.Instance.myRank[3] = j + 1;
//					break;
//
//				case RankManagerment.TYPE_GUILD:
//					RankItemGuild rankItemGuild = new RankItemGuild();
//					rankItemGuild.gid = vs.Value[0].getValueString();
//					rankItemGuild.name = vs.Value[1].getValueString();
//					rankItemGuild.score = StringKit.toInt(vs.Value[2].getValueString());
//					RankManagerment.Instance.guildList.Add(rankItemGuild);
//					break;
//				}
//				
//			}
//			
//		}
//		
//		RankManagerment.Instance.nextUpdateTime = minNextUpdateTime;
		if (callback != null)
			callback ();
	}
}
