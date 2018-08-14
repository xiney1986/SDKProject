using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 公会邀请列表容器
 * @author 汤琦
 * */
public class GuildInviteContent : dynamicContent
{

	private List<GuildRankInfo> guildList;
	public GameObject inviteItem;
	
	public void	reLoad ()
	{
		guildList = GuildManagerment.Instance.getGuildInviteList();
		
		if (guildList == null)
			return; 
		 base.reLoad (guildList.Count);
	}
	public override void  updateItem (GameObject item, int index)
	{
		GuildInviteItem button = item.GetComponent<GuildInviteItem> ();				 
		button.updateActive (guildList [index],fatherWindow as GuildInviteWindow);
		
	}
	public override void initButton (int  i)
	{
		if (nodeList [i] == null)
			nodeList [i] = NGUITools.AddChild(gameObject,inviteItem);
		
//		nodeList [i].transform.parent = transform;
//		nodeList [i].transform.localPosition = Vector3 .zero;
//		nodeList [i].transform.localScale = Vector3 .one; 
		nodeList [i].name = StringKit. intToFixString (i + 1);
		GuildInviteItem button = nodeList [i].GetComponent<GuildInviteItem> ();
		button.updateActive (guildList [i],fatherWindow as GuildInviteWindow);
	}
}
