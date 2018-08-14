using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 公会成员分页
 * @author 汤琦
 * */
public class GuildMemberContent : dynamicContent
{
	List<GuildMember> members;
	public UILabel memberSum;
	public ButtonBase inviteButton;
	public ButtonBase approveButton;
	public UILabel count;
	public UISprite countBg;

	public void reLoad()
	{
		if(GuildManagerment.Instance.getGuild().job == GuildJobType.JOB_PRESIDENT || GuildManagerment.Instance.getGuild().job == GuildJobType.JOB_VICE_PRESIDENT)
		{
			if(GuildManagerment.Instance.getGuildApprovalList() != null && GuildManagerment.Instance.getGuildApprovalList().Count > 0)
			{
				count.text = GuildManagerment.Instance.getGuildApprovalList().Count.ToString();
				count.gameObject.SetActive(true);
				countBg.gameObject.SetActive(true);
			}
			else
			{
				count.text = "";
				count.gameObject.SetActive(false);
				countBg.gameObject.SetActive(false);
			}
		}
		if(GuildManagerment.Instance.getGuild().job == GuildJobType.JOB_PRESIDENT || GuildManagerment.Instance.getGuild().job == GuildJobType.JOB_VICE_PRESIDENT)
		{
			inviteButton.gameObject.SetActive(true);
			approveButton.gameObject.SetActive(true);
			count.gameObject.SetActive(true);

		}
		else
		{
			inviteButton.gameObject.SetActive(false);
			approveButton.gameObject.SetActive(false);
			count.gameObject.SetActive(false);
		}
		members = GuildManagerment.Instance.getGuildMembersByJob();
		memberSum.text = GuildManagerment.Instance.getGuild().membership + "/" + GuildManagerment.Instance.getGuild().membershipMax;
		base.reLoad(members.Count);
	}
	
	public override void updateItem (GameObject item, int index)
	{
		GuildMemberItem member = item.GetComponent<GuildMemberItem> ();
		member.initInfo (members [index] as GuildMember,fatherWindow as GuildMemberWindow,GuildManagerment.Instance.getGuild()); 
	}
	
	public override void initButton (int  i)
	{
		if (nodeList [i] == null){
			nodeList [i] = NGUITools.AddChild (gameObject, (fatherWindow as GuildMemberWindow).guildMemberItem);
		}
	//	nodeList [i].name = StringKit. intToFixString (i + 1);
		GuildMemberItem member = nodeList [i].GetComponent<GuildMemberItem> ();
		member.initInfo (members [i] as GuildMember,fatherWindow as GuildMemberWindow,GuildManagerment.Instance.getGuild());
	}

	void OnDisable()
	{
		cleanAll();
	}
}
