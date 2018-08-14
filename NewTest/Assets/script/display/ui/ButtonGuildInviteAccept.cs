using UnityEngine;
using System.Collections;

/**
 * 接受申请加入公会按钮
 * @author 汤琦
 * */
public class ButtonGuildInviteAccept : ButtonBase
{
	private GuildRankInfo guild;
	private GuildInviteWindow win;
	
	public void initInfo(GuildRankInfo guild,GuildInviteWindow win)
	{
		this.guild = guild;
		this.win = win;
		fatherWindow = win;
	}

	public override void DoClickEvent ()
	{
		base.DoClickEvent (); 
		GuildInviteAcceptFPort fport = FPortManager.Instance.getFPort("GuildInviteAcceptFPort") as GuildInviteAcceptFPort;
		fport.access(guild.uid,updateContent);
		MaskWindow.UnlockUI();
	}


	private void updateContent()
	{
		win.updateContent();
	}
}
