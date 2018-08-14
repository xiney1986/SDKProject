using UnityEngine;
using System.Collections;

public class ButtonGuildApply : ButtonBase
{
	private GuildRankInfo guild;

	public void initInfo(GuildRankInfo guild)
	{
		this.guild = guild;
		if(GuildManagerment.Instance.getIds() != null && GuildManagerment.Instance.getIds().Contains(guild.uid)) 
		{
			textLabel.text = LanguageConfigManager.Instance.getLanguage("Guild_11");
		}
		else
		{
			textLabel.text = LanguageConfigManager.Instance.getLanguage("Guild_10");;
		}

	}

	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
		if(GuildManagerment.Instance.getIds() != null && GuildManagerment.Instance.getIds().Contains(guild.uid)) 
		{ 
			GuildCancelJoinFPort fport = FPortManager.Instance.getFPort("GuildCancelJoinFPort") as GuildCancelJoinFPort;
			fport.access(guild.uid,guildCancelJoinBack);

		}
		else
		{
			if(GuildManagerment.Instance.isJoinGuild())
			{
				GuildJoinFPort fport = FPortManager.Instance.getFPort("GuildJoinFPort") as GuildJoinFPort;
				fport.access(guild.uid,guildJoinBack,fatherWindow);

			}
		}
	}

	private void guildCancelJoinBack()
	{
		GuildManagerment.Instance.removeIds (guild.uid);
		guild.isApply = false;
		textLabel.text = LanguageConfigManager.Instance.getLanguage("Guild_10");
		TextTipWindow.Show (LanguageConfigManager.Instance.getLanguage("Guild_11"));

	}

	private void guildJoinBack(int i)
	{
		GuildManagerment.Instance.addIds (guild.uid);
		guild.isApply = true;
		textLabel.text = LanguageConfigManager.Instance.getLanguage("Guild_11");
		TextTipWindow.Show(LanguageConfigManager.Instance.getLanguage("FriendAPPLY_ok"));
		if(i==1){
			fatherWindow.finishWindow();
		}
	}

}
