using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GuildApplyPage : MonoBase {
	public GuildApplyItem [] applys;
	private List<GuildRankInfo> list;
	public void updatePage(List<GuildRankInfo> list)
	{
		this.list = list;
		for (int i = 0; i <applys.Length; ++i) {
			if(i<list.Count)
			{
				applys[i].gameObject.SetActive(true);
				applys[i].updateActive(list[i]);
			}
			else
			{
				applys[i].gameObject.SetActive(false);
			}
		}
	}
}
