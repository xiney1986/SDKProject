using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GodsWarIntegralAwardContent : dynamicContent
{
    private List<GodsWarPrizeSample> awards;
	private UIScrollView scrollView;
	public WindowBase win;
	CallBack callback;
	public void init (WindowBase win,CallBack callback)
	{
		this.win = win;
		this.callback = callback;
		scrollView=GetComponent<UIScrollView>();
		awards = GodsWarPrizeSampleManager.Instance.getIntegralPrize();
		if(awards != null)
			base.reLoad(awards.Count);
	}

    public override void updateItem(GameObject item, int index)
    {
		GodsWarIntegralAwardItem arenaItem = item.GetComponent<GodsWarIntegralAwardItem>();
        arenaItem.initItem(awards[index], win,callback);
    }

    public override void initButton(int i)
    {
        nodeList[i] = NGUITools.AddChild(gameObject, (fatherWindow as GodsWarIntegralAwardWindow).godsWarIntegralItemPrefab);
        nodeList[i].SetActive(true);
		GodsWarIntegralAwardItem arenaItem = nodeList[i].GetComponent<GodsWarIntegralAwardItem>();
        arenaItem.initItem(awards[i], win,callback);

    }
}
