using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LastBattleRankContent :dynamicContent 
{
	private UIScrollView scrollView;
	List<LastBattleRank> rank;

	public void initContent()
	{
		scrollView = GetComponent<UIScrollView>();
		Utils.RemoveAllChild(gameObject.transform);
		//LastBattleManagement.Instance.setRankList();
		initData();
	}
	public void initData()
	{
		//rank = LastBattleManagement.Instance.rank;
		rank = RankManagerment.Instance.lastBattleRankList;
		if(rank != null)
		{
			base.reLoad(rank.Count);
		}
	}

	public override void updateItem(GameObject item, int index)
	{
		LastBattleRankItemView itemView = item.GetComponent<LastBattleRankItemView>();
		itemView.init(rank[index],index);
	}
	
	public override void initButton(int i)
	{
		nodeList[i] = NGUITools.AddChild(gameObject, (fatherWindow as LastBattleRankWindow).rankitemPrefab);
		nodeList[i].SetActive(true);
		LastBattleRankItemView itemView = nodeList[i].GetComponent<LastBattleRankItemView>();
		itemView.init(rank[i],i);	
	}
}
