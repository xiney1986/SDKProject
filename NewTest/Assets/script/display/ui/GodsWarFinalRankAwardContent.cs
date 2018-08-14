using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GodsWarFinalRankAwardContent : dynamicContent
{
    private List<GodsWarPrizeSample> awards;
	private UIScrollView scrollView;
	public WindowBase win;
	private int big_id;
	public void init (int big_id,WindowBase win)
	{
		this.win = win;
		this.big_id = big_id;
		this.fatherWindow = win;
		scrollView=GetComponent<UIScrollView>();
		Utils.RemoveAllChild(gameObject.transform);
		setAwardsList();
		if(awards != null)
			base.reLoad(awards.Count);
	}

    public override void updateItem(GameObject item, int index)
    {
		GodsWarRankAwardItem items = item.GetComponent<GodsWarRankAwardItem>();
        items.init(awards[index],index, win);
    }

    public override void initButton(int i)
    {
		nodeList[i] = NGUITools.AddChild(gameObject, (fatherWindow as GodsWarFinalRankWindow).rankitemPrefab);
        nodeList[i].SetActive(true);
		GodsWarRankAwardItem item = nodeList[i].GetComponent<GodsWarRankAwardItem>();
		item.init(awards[i],i, win);

    }
	/// <summary>
	/// 设置奖励列表
	/// </summary>
	public void setAwardsList()
	{
		switch (big_id) {
		case GodsWarManagerment.TYPE_BRONZE:
			awards = GodsWarPrizeSampleManager.Instance.getFinalBronzePrize();
			break;
		case GodsWarManagerment.TYPE_SILVER:
			awards = GodsWarPrizeSampleManager.Instance.getFinalSilverPrize();
			break;
		case GodsWarManagerment.TYPE_GOLD:
			awards = GodsWarPrizeSampleManager.Instance.getFinalGoldPrize();
			break;
		default:
			break;
		}
	}
}
