using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GodsWarMySuportContent :dynamicContent 
{
	List<GodsWarMySuportInfo> infos;
	private UIScrollView scrollView;
	public WindowBase win;
	CallBack callback;

	public void initContent(WindowBase win,CallBack callback)
	{
		this.win = win;
		this.fatherWindow = win;
		this.callback = callback;
		scrollView = GetComponent<UIScrollView>();
//		FPortManager.Instance.getFPort<GodsWarGetMySuportFport>().access(initData);
		initData();
	}
	public void initData()
	{
		infos = GodsWarManagerment.Instance.mySuportInfo;
		if(infos!=null)
		{
			base.reLoad(infos.Count);
		}
		if(callback!=null)
			callback();
	}

	public override void updateItem(GameObject item, int index)
	{
		GodsWarMySuportItem items = item.GetComponent<GodsWarMySuportItem>();
		items.initItem(infos[index]);
	}
	
	public override void initButton(int i)
	{
		nodeList[i] = NGUITools.AddChild(gameObject, (fatherWindow as GodsWarMySuportWindow).itemPrefab);
		nodeList[i].SetActive(true);
		GodsWarMySuportItem item = nodeList[i].GetComponent<GodsWarMySuportItem>();
		item.initItem(infos[i]);
		
	}
}
