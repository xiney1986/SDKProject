using UnityEngine;
using System.Collections;

/**
 * 拒绝申请加入公会按钮
 * @author 汤琦
 * */
public class ButtonGuildInviteCancel : ButtonBase
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
		GuildInviteRejectFPort fport = FPortManager.Instance.getFPort("GuildInviteRejectFPort") as GuildInviteRejectFPort;
		fport.access(guild.uid,updateContent);
		MaskWindow.UnlockUI();
	}

	private void updateContent()
	{
		win.updateContent();
	}
}
