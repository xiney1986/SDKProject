using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GodsWarRankContent :dynamicContent 
{
	List<GodsWarRankUserInfo> infos;
	private UIScrollView scrollView;
	private int big_id;
	public WindowBase win;

	public void initContent(int big_id,WindowBase win)
	{
		this.big_id=big_id;
		this.win = win;
		this.fatherWindow = win;
		scrollView = GetComponent<UIScrollView>();
		Utils.RemoveAllChild(gameObject.transform);
		FPortManager.Instance.getFPort<GodsWarGetRankInfoFport>().access(initData,big_id);
	}
	public void initData()
	{
		setInfoByBidId();
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
		nodeList[i] = NGUITools.AddChild(gameObject, (fatherWindow as GodsWarSuportRankWindow).rankitemPrefab);
		nodeList[i].SetActive(true);
		GodsWarRankItemView arenaItem = nodeList[i].GetComponent<GodsWarRankItemView>();
		arenaItem.init(infos[i], i);
		
	}

	public void setInfoByBidId()
	{
		switch (big_id) {
		case 110:
			infos = GodsWarManagerment.Instance.usersRankList_bronze;
			break;
		case 111:
			infos = GodsWarManagerment.Instance.usersRankList_silver;
			break;
		case 112:
			infos = GodsWarManagerment.Instance.usersRankList_gold;
			break;
		default:
			break;
		}
	}
}
