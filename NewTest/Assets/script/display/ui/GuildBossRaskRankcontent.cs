using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GuildBossRaskRankcontent : MonoBehaviour
{

	//private GuildBossRankItem[] ranks;
	public  GameObject content;
	public  GameObject itemPrefab;
	
	public void initContent ()
	{
		GuildAltar guildAltar = GuildManagerment.Instance.getGuildAltar ();
		List<GuildAltarRank> list = guildAltar.list;
		if (list == null)
			return; 
		Utils.RemoveAllChild (content.transform);
		int rank = 0;
		for(int i=0;i<list.Count;i++){
			if (list[i].hurtValue> 0) {
				rank++;
				if(rank<=5)
				{
					GuildBossRankItem item = NGUITools.AddChild (content.gameObject, itemPrefab).GetComponent<GuildBossRankItem> ();
					item.transform.localPosition = new Vector3(0,-60*i,0);
					item.init (list[i], rank);
				}
			}
		}
	}
}
