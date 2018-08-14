using UnityEngine;
using System.Collections;

public class RankFPort : BaseFPort
{
	private CallBack callback;

	public void access (int type, CallBack callback)
	{

		if (RankManagerment.Instance.nextTime.ContainsKey (type) && ServerTimeKit.getSecondTime () < RankManagerment.Instance.nextTime [type]) {
			callback ();
			return;
		}
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.RANK_GET);
		message.addValue ("type", new ErlInt (type));
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{
		ErlArray array = message.getValue ("msg") as ErlArray;
		if (array != null) {
//			RankManagerment.Instance.myRank.Clear();
			ErlType[] types = array.Value;
			for (int i = 0; i < types.Length; i++) {
				ErlArray arlItem = types [i] as ErlArray;
				int type = StringKit.toInt (arlItem.Value [0].getValueString ());
				int nextUpdateTIme = StringKit.toInt (arlItem.Value [1].getValueString ());
				if (RankManagerment.Instance.nextTime.ContainsKey (type)) {
					RankManagerment.Instance.nextTime [type] = nextUpdateTIme;
				} else {
					RankManagerment.Instance.nextTime.Add (type, nextUpdateTIme);
				}
				clear(type);
				ErlArray values = arlItem.Value [2] as ErlArray;
				for (int j = 0; j < values.Value.Length; j++) {
					ErlArray vs = values.Value [j] as ErlArray;
					switch (type) {
					case RankManagerment.TYPE_COMBAT:
						RankItemCombat rankItemCombat = new RankItemCombat ();
						rankItemCombat.uid = vs.Value [0].getValueString ();
						rankItemCombat.name = vs.Value [1].getValueString ();
						rankItemCombat.combat = StringKit.toInt (vs.Value [2].getValueString ());
						rankItemCombat.vipLevel = StringKit.toInt (vs.Value [3].getValueString ());
						RankManagerment.Instance.combatList.Add (rankItemCombat);
						if (rankItemCombat.uid == UserManager.Instance.self.uid) {
							if (RankManagerment.Instance.myRank.ContainsKey (RankManagerment.TYPE_COMBAT))
								RankManagerment.Instance.myRank [RankManagerment.TYPE_COMBAT] = j + 1;
							else
								RankManagerment.Instance.myRank.Add (RankManagerment.TYPE_COMBAT, j + 1);
						}
						break;

					case RankManagerment.TYPE_PVP:
						RankItemPVP rankItemPVP = new RankItemPVP ();
						rankItemPVP.uid = vs.Value [0].getValueString ();
						rankItemPVP.name = vs.Value [1].getValueString ();
						rankItemPVP.win = StringKit.toInt (vs.Value [2].getValueString ());
						rankItemPVP.vipLevel = StringKit.toInt (vs.Value [3].getValueString ());
						RankManagerment.Instance.pvpList.Add (rankItemPVP);
						if (rankItemPVP.uid == UserManager.Instance.self.uid) {
							if (RankManagerment.Instance.myRank.ContainsKey (RankManagerment.TYPE_PVP))
								RankManagerment.Instance.myRank [RankManagerment.TYPE_PVP] = j + 1;
							else
								RankManagerment.Instance.myRank.Add (RankManagerment.TYPE_PVP, j + 1);
						}
						break;

					case RankManagerment.TYPE_MONEY:
						RankItemMoney rankItemMoney = new RankItemMoney ();
						rankItemMoney.uid = vs.Value [0].getValueString ();
						rankItemMoney.name = vs.Value [1].getValueString ();
						rankItemMoney.money = StringKit.toInt (vs.Value [2].getValueString ());
						rankItemMoney.vipLevel = StringKit.toInt (vs.Value [3].getValueString ());
						RankManagerment.Instance.moneyList.Add (rankItemMoney);
						if (rankItemMoney.uid == UserManager.Instance.self.uid) {
							if (RankManagerment.Instance.myRank.ContainsKey (RankManagerment.TYPE_MONEY))
								RankManagerment.Instance.myRank [RankManagerment.TYPE_MONEY] = j + 1;
							else
								RankManagerment.Instance.myRank.Add (RankManagerment.TYPE_MONEY, j + 1);
						}
						break;

                    case RankManagerment.TYPE_BOSSDAMAGE:
                        RankItemTotalDamage rankItemDamage = new RankItemTotalDamage();
                        rankItemDamage.uid = vs.Value[0].getValueString();
                        rankItemDamage.name = vs.Value[1].getValueString();
                        rankItemDamage.damage = vs.Value[2].getValueString();
                        rankItemDamage.vipLevel = StringKit.toInt(vs.Value[3].getValueString());
                        RankManagerment.Instance.totalDamageList.Add(rankItemDamage);
                        if (rankItemDamage.uid == UserManager.Instance.self.uid) {
                            if (RankManagerment.Instance.myRank.ContainsKey(RankManagerment.TYPE_BOSSDAMAGE))
                                RankManagerment.Instance.myRank[RankManagerment.TYPE_BOSSDAMAGE] = j + 1;
                            else
                                RankManagerment.Instance.myRank.Add(RankManagerment.TYPE_BOSSDAMAGE, j + 1);
                        }
                        break;

					case RankManagerment.TYPE_LASTBATTLE:
						LastBattleRank rank = new LastBattleRank();
						rank.uid = vs.Value[0].getValueString();
						rank.serverName = vs.Value[1].getValueString();
						rank.name = vs.Value[2].getValueString();
						rank.vipLV = StringKit.toInt(vs.Value[3].getValueString());
						rank.score = StringKit.toInt(vs.Value[4].getValueString());
						RankManagerment.Instance.lastBattleRankList.Add(rank); 
//						if (rank.uid == UserManager.Instance.self.uid) {
//							if (RankManagerment.Instance.myRank.ContainsKey(RankManagerment.TYPE_LASTBATTLE))
//								RankManagerment.Instance.myRank[RankManagerment.TYPE_LASTBATTLE] = j + 1;
//							else
//								RankManagerment.Instance.myRank.Add(RankManagerment.TYPE_LASTBATTLE, j + 1);
//						}
						break;

					case RankManagerment.TYPE_ROLE:
						RankItemRole rankItemRole = new RankItemRole ();
						rankItemRole.uid = vs.Value [0].getValueString ();
						rankItemRole.name = vs.Value [1].getValueString ();
						rankItemRole.cardUid = vs.Value [2].getValueString ();
						rankItemRole.cardName = vs.Value [3].getValueString ();
						rankItemRole.vipLevel = StringKit.toInt (vs.Value [4].getValueString ());
						RankManagerment.Instance.roleList.Add (rankItemRole);
						if (rankItemRole.uid == UserManager.Instance.self.uid) {
							if (RankManagerment.Instance.myRank.ContainsKey (RankManagerment.TYPE_ROLE)) {
								if (RankManagerment.Instance.myRank [RankManagerment.TYPE_ROLE] > j + 1)
									RankManagerment.Instance.myRank [RankManagerment.TYPE_ROLE] = j + 1;
							} else
								RankManagerment.Instance.myRank.Add (RankManagerment.TYPE_ROLE, j + 1);
						}
						break;

					case RankManagerment.TYPE_GUILD:
						RankItemGuild rankItemGuild = new RankItemGuild ();
						rankItemGuild.gid = vs.Value [0].getValueString ();
						rankItemGuild.name = vs.Value [1].getValueString ();
						rankItemGuild.score = StringKit.toInt (vs.Value [2].getValueString ());
                    //rankItemGuild.vipLevel = StringKit.toInt(vs.Value[3].getValueString());
						RankManagerment.Instance.guildList.Add (rankItemGuild);
						break;

					case RankManagerment.TYPE_ROLE_LV:
						RankItemRoleLv rankItemRoleLv = new RankItemRoleLv ();
						rankItemRoleLv.uid = vs.Value [0].getValueString ();
						rankItemRoleLv.name = vs.Value [1].getValueString ();
						rankItemRoleLv.lv = StringKit.toInt (vs.Value [2].getValueString ());
						rankItemRoleLv.vipLevel = StringKit.toInt (vs.Value [3].getValueString ());
						RankManagerment.Instance.roleLvList.Add (rankItemRoleLv);
						if (rankItemRoleLv.uid == UserManager.Instance.self.uid) {
							if (RankManagerment.Instance.myRank.ContainsKey (RankManagerment.TYPE_ROLE_LV))
								RankManagerment.Instance.myRank [RankManagerment.TYPE_ROLE_LV] = j + 1;
							else
								RankManagerment.Instance.myRank.Add (RankManagerment.TYPE_ROLE_LV, j + 1);
						}
						break;

					case RankManagerment.TYPE_GODDESS:
						RankItemGoddess rankItemGoddess = new RankItemGoddess ();
						rankItemGoddess.uid = vs.Value [0].getValueString ();
						rankItemGoddess.name = vs.Value [1].getValueString ();
						rankItemGoddess.addPer = StringKit.toInt (vs.Value [2].getValueString ());
						rankItemGoddess.vipLevel = StringKit.toInt (vs.Value [3].getValueString ());
						RankManagerment.Instance.goddessList.Add (rankItemGoddess);
						if (rankItemGoddess.uid == UserManager.Instance.self.uid) {
							if (RankManagerment.Instance.myRank.ContainsKey (RankManagerment.TYPE_GODDESS))
								RankManagerment.Instance.myRank [RankManagerment.TYPE_GODDESS] = j + 1;
							else
								RankManagerment.Instance.myRank.Add (RankManagerment.TYPE_GODDESS, j + 1);
						}
						break;
					case RankManagerment.TYPE_LADDER:

						break;

					case RankManagerment.TYPE_GUILD_FIGHT:
						RankItemGuildFight guild = new RankItemGuildFight();
						guild.uid = vs.Value[0].getValueString();
						guild.name = vs.Value[1].getValueString();
						guild.judgeScore = StringKit.toInt(vs.Value[2].getValueString());
						RankManagerment.Instance.guildFightJudgeList.Add(guild);
						if(GuildManagerment.Instance.getGuild()!=null && guild.uid == GuildManagerment.Instance.getGuild().uid)
						{
							if (RankManagerment.Instance.myRank.ContainsKey (RankManagerment.TYPE_GUILD_FIGHT)) {
								if (RankManagerment.Instance.myRank [RankManagerment.TYPE_GUILD_FIGHT] > j + 1)
									RankManagerment.Instance.myRank [RankManagerment.TYPE_GUILD_FIGHT] = j + 1;
							} else
								RankManagerment.Instance.myRank.Add (RankManagerment.TYPE_GUILD_FIGHT, j + 1);
						}
						break;
					}
				
				}
				// 末日决战排行榜解析自己名次//
				if(type == RankManagerment.TYPE_LASTBATTLE)
				{
					LastBattleManagement.Instance.myRank = StringKit.toInt(arlItem.Value [3].getValueString ());
				}
			
			}
			if (callback != null)
				callback ();
		} else {
			MessageWindow.ShowAlert ((message.getValue ("msg") as ErlType).getValueString ());
			if (callback != null)
				callback = null;
		}
	}

	private void clear(int type){
		switch (type) {
		case RankManagerment.TYPE_COMBAT:
			RankManagerment.Instance.combatList.Clear ();
			break;
			
		case RankManagerment.TYPE_PVP:
			RankManagerment.Instance.pvpList.Clear ();
			break;
			
		case RankManagerment.TYPE_MONEY:
			RankManagerment.Instance.moneyList.Clear ();
			break;
			
		case RankManagerment.TYPE_ROLE:
			RankManagerment.Instance.roleList.Clear ();
			break;
			
		case RankManagerment.TYPE_GUILD:
			RankManagerment.Instance.guildList.Clear ();
			break;
			
		case RankManagerment.TYPE_ROLE_LV:
			RankManagerment.Instance.roleLvList.Clear ();
			break;

		case RankManagerment.TYPE_GODDESS:
			RankManagerment.Instance.goddessList.Clear ();
			break;

		case RankManagerment.TYPE_GUILD_FIGHT:
			RankManagerment.Instance.guildFightJudgeList.Clear();
			break;
        case RankManagerment.TYPE_BOSSDAMAGE:
            RankManagerment.Instance.totalDamageList.Clear();
            break;
		case RankManagerment.TYPE_LASTBATTLE:
			RankManagerment.Instance.lastBattleRankList.Clear();
			break;
		}
	}
}
