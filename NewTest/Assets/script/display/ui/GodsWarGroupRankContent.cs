using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GodsWarGroupRankContent :dynamicContent 
{
	List<GodsWarRankUserInfo> infos;
	private UIScrollView scrollView;

	public void initContent()
	{
		scrollView = GetComponent<UIScrollView>();
		Utils.RemoveAllChild(gameObject.transform);
		FPortManager.Instance.getFPort<GodsWarGetGroupRankInfoFport>().access(initData);
	}
	public void initData()
	{
		infos = GodsWarManagerment.Instance.myGroupRanklist;
		if(infos!=null)
		{
			base.reLoad(infos.Count);
		}
	}

	public override void updateItem(GameObject item, int index)
	{
		GodsWarRankItemView arenaItem = item.GetComponent<GodsWarRankItemView>();
		arenaItem.init(infos[index],index);
	}
	
	public override void initButton(int i)
	{
		nodeList[i] = NGUITools.AddChild(gameObject, (fatherWindow as GodsWarGroupRankWindow).rankitemPrefab);
		nodeList[i].SetActive(true);
		GodsWarRankItemView arenaItem = nodeList[i].GetComponent<GodsWarRankItemView>();
		arenaItem.init(infos[i], i);
		
	}
}
