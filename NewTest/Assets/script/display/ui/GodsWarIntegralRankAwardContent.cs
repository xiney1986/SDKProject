using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GodsWarIntegralRankAwardContent : dynamicContent
{
    private List<GodsWarPrizeSample> awards;
	private UIScrollView scrollView;
	public WindowBase win;
	public void init (WindowBase win)
	{
		this.win = win;
		scrollView=GetComponent<UIScrollView>();
        int team = StringKit.toInt(GodsWarManagerment.Instance.self.bigTeam);
        if(team==110)
		awards = GodsWarPrizeSampleManager.Instance.getIntegralRank();
        else if (team == 111) awards = GodsWarPrizeSampleManager.Instance.getIntegralRankYin();
        else if (team == 112) awards = GodsWarPrizeSampleManager.Instance.getIntegralRankJin();
		if(awards != null)
			reLoad(awards.Count);
	}

    public override void updateItem(GameObject item, int index)
    {
		GodsWarRankAwardItem items = item.GetComponent<GodsWarRankAwardItem>();
        items.init(awards[index],index, win);
    }

    public override void initButton(int i)
    {
		nodeList[i] = NGUITools.AddChild(gameObject, (fatherWindow as GodsWarIntegralRankAwardWindow).itemPrefab);
        nodeList[i].SetActive(true);
		GodsWarRankAwardItem item = nodeList[i].GetComponent<GodsWarRankAwardItem>();
		item.init(awards[i],i, win);

    }
}
