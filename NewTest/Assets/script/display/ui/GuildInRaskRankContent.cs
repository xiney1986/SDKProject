using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GuildInRaskRankContent : MonoBehaviour 
{

	public GuildInRankItem[] ranks;
	
	public void initContent()
	{
		List<GuildRastInfo> list = GuildManagerment.Instance.getGuildRastInfo();
		if(list == null)
			return; 
		if(list.Count > 10)
		{
			for (int i = 0; i < ranks.Length; i++) {
				if(list[i].rast == 0)
					continue;
				ranks[i].names.text = list[i].playerName;
				ranks[i].values.text = list[i].rast.ToString();
				ranks[i].gameObject.SetActive(true);
			}
		}
		else
		{
			for (int i = 0; i < list.Count; i++) {
				if(list[i].rast == 0)
					continue;
				ranks[i].names.text = list[i].playerName;
				ranks[i].values.text = list[i].rast.ToString();
				ranks[i].gameObject.SetActive(true);
			}
		}

	}
}
