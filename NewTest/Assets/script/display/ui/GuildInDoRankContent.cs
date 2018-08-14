using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GuildInDoRankContent : MonoBehaviour 
{
	public GuildInRankItem[] ranks;

	public void initContent()
	{
		List<GuildMember> list = GuildManagerment.Instance.getGuildMembersByDonate();

		if(list == null) 
			return;
		if(list.Count > 10)
		{
			for (int i = 0; i < ranks.Length; i++) {
				if(list[i].donated == 0)
					continue;
				ranks[i].names.text = list[i].name;
				ranks[i].values.text = list[i].donated.ToString();
				ranks[i].gameObject.SetActive(true);
			}
		}
		else
		{
			for (int i = 0; i < list.Count; i++) {
				if(list[i].donated == 0)
					continue;
				ranks[i].names.text = list[i].name;
				ranks[i].values.text = list[i].donated.ToString();
				ranks[i].gameObject.SetActive(true);
			}
		}


	}
}
