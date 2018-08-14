using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GuildInfoContent : dynamicContent
{
	public GameObject itemPrefab;

	List<GuildMember> list;
	Guild guild;

	public void init(List<GuildMember> list,Guild guild)
	{
		this.list = list;
		this.guild = guild;
		 base.reLoad(list.Count);
	}

	public override void updateItem (GameObject item, int index)
	{
		GuildMemberItem sc = item.GetComponent<GuildMemberItem> ();
		sc.initInfo (list [index], fatherWindow,guild);
	}

	public override void initButton (int i)
	{
		nodeList [i] = NGUITools.AddChild (gameObject, itemPrefab);
		nodeList [i].name = StringKit. intToFixString (i + 1);
		nodeList [i].SetActive (true);

		GuildMemberItem sc = nodeList [i].GetComponent<GuildMemberItem> ();
		sc.initInfo (list [i], fatherWindow,guild);
	}
}
