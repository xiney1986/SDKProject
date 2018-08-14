using UnityEngine;
using System.Collections;

public class GuildInviteItem : MonoBehaviour 
{
	public UILabel level;
	public UILabel member;
	public UILabel guildName;
	public UILabel declaration;
	public ButtonGuildInviteAccept buttonAccept;
	public ButtonGuildInviteCancel buttonCancel;
	private GuildRankInfo guild;
	
	public void updateActive (GuildRankInfo guild,GuildInviteWindow win)
	{
		this.guild = guild; 
		level.text = LanguageConfigManager.Instance.getLanguage("Guild_108l") + "LV " + guild.level;
		member.text =LanguageConfigManager.Instance.getLanguage("Guild_109l") + guild.membership + "/" + guild.membershipMax;
		guildName.text = guild.name;
		declaration.text = guild.declaration;
		buttonAccept.initInfo(guild,win);
		buttonCancel.initInfo(guild,win);
	}
}
