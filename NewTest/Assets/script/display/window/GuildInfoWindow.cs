using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GuildInfoWindow : WindowBase
{
	public GuildInfoContent content;
	public UILabel labelName;
	public UILabel labelPresident;
	public UILabel labelLive;
	public UILabel labelCount;
	public ButtonGuildApply buttonApply;

	private Guild guild;
	private List<GuildMember> memebers;

	public void init(Guild guild,List<GuildMember> memebers)
	{
		this.guild = guild;
		this.memebers = memebers;

		labelName.text = guild.name;
		labelPresident.text = guild.presidentName;
		labelLive.text = guild.livenessing + "/" + guild.livenessed;
		labelCount.text = LanguageConfigManager.Instance.getLanguage ("s0412") + " : " + guild.membership + "/" + guild.membershipMax;

		if (UserManager.Instance.self.getUserLevel () < 21 || GuildManagerment.Instance.getGuild() != null) {
			buttonApply.gameObject.SetActive(false);
		} else {
			GuildRankInfo guildRankInfo = GuildManagerment.Instance.getGuildByUid (guild.uid);
			if (guildRankInfo == null) {
				guildRankInfo = new GuildRankInfo (guild.uid, guild.level, guild.name, guild.membership, guild.membershipMax, guild.declaration, guild.livenessing);
				GuildManagerment.Instance.createGuildList (guildRankInfo);
			} 
			buttonApply.initInfo (guildRankInfo);
		}
	}


	protected override void begin ()
	{
		if(!isAwakeformHide)
			content.init (memebers,guild);
		MaskWindow.UnlockUI ();
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
            finishWindow();
		}
	}

}
