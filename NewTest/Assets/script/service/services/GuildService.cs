using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 公会信息更新服务
 * @author 汤琦
 * */
public class GuildService : BaseFPort
{
	public GuildService ()
	{
		
	}
	
	public override void read (ErlKVMessage message)
	{  
		if (message.getValue ("msg") != null) {
			ErlType msg = message.getValue ("msg") as ErlType;
			GuildManagerment.Instance.createMsg (msg.getValueString ());
		} else if (message.getValue ("m_challenge_boss") != null) {
			ErlType msg = message.getValue ("m_challenge_boss") as ErlType;
			ErlArray array = msg as ErlArray;
			if (array != null && array.Value.Length > 0 && GuildManagerment.Instance.getGuildAltar () != null) {
				GuildManagerment.Instance.getGuildAltar ().hurtSum = StringKit.toInt (array.Value [0].getValueString ());
				ErlArray temp = array.Value [1] as ErlArray;
				List<GuildAltarRank> ranks = new List<GuildAltarRank> ();
				for (int i = 0; i < temp.Value.Length; i++) {
					ErlArray temps = temp.Value [i] as ErlArray;
					string sid = temps.Value [0].getValueString ();
					string playerName = temps.Value [1].getValueString ();
					long hurtValue = long.Parse (temps.Value [2].getValueString ());
					GuildAltarRank rankTemp = new GuildAltarRank (sid, playerName, hurtValue);
					ranks.Add (rankTemp);
				}
				GuildManagerment.Instance.getGuildAltar ().list = ranks;
			}
		} else if (message.getValue ("guild_skill") != null) {
			ErlType msg = message.getValue ("guild_skill") as ErlType;
			if (msg is ErlArray) {
				ErlArray temp = msg as ErlArray;
				for (int i = 0; i < temp.Value.Length; i++) {
					ErlArray array = temp.Value [i] as ErlArray;
					GuildManagerment.Instance.updateGuildSkill (StringKit.toInt (array.Value [1].getValueString ()), array.Value [0].getValueString ());
				}
			}
		} else if (message.getValue ("clean_mem") != null) { //自己被踢
			MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("Guild_90"));
			GuildManagerment.Instance.clearMember ();
		} else if (message.getValue ("update_boss_id") != null) {
			if (GuildManagerment.Instance.getGuildAltar () != null) {
				ErlType msg = message.getValue ("update_boss_id") as ErlType;
				GuildManagerment.Instance.updateBossSid (StringKit.toInt (msg.getValueString ()));
			}
		}
		else if (message.getValue ("position_change") != null) {
			if (GuildManagerment.Instance.getGuild () != null) {
				ErlType msg = message.getValue ("position_change") as ErlType;
				GuildManagerment.Instance.updateJob (StringKit.toInt (msg.getValueString ()));
			}
		}
		else if (message.getValue ("apply") != null) {
			ErlType msg = message.getValue ("apply") as ErlType;
			ErlType guildName = message.getValue("name") as ErlType;
			GuildManagerment.Instance.ApplyMessageLint(msg.getValueString(),guildName.getValueString());
		}
	}
}
