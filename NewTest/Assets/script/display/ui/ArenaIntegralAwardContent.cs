using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArenaIntegralAwardContent : dynamicContent
{
    private List<ArenaAwardSample> awards;
	private UIScrollView scrollView;

    public void init()
    {
        scrollView = GetComponent<UIScrollView>();
        //awards = ArenaIntegralAwardSampleManager.Instance.getAllArenaIntegralSamples();
        //List<ArenaAwardSample> samples 
        awards = ArenaAwardSampleManager.Instance.getSamplesByType(ArenaAwardWindow.TYPE_INTEGRAL);
        if (awards != null)
			base.reLoad(awards.Count,getAwardIndex());
    }

    public override void updateItem(GameObject item, int index)
    {
        ArenaIntegralAwardItem arenaItem = item.GetComponent<ArenaIntegralAwardItem>();
        arenaItem.initialize(awards[index], fatherWindow,this);
    }

    public override void initButton(int i)
    {
        nodeList[i] = NGUITools.AddChild(gameObject, (fatherWindow as ArenaIntegralAwardWindow).arenaIntegralItemPrefab);
        nodeList[i].SetActive(true);
        //nodeList[i].name = StringKit.intToFixString(i + 1);
        ArenaIntegralAwardItem arenaItem = nodeList[i].GetComponent<ArenaIntegralAwardItem>();
        arenaItem.initialize(awards[i], fatherWindow,this);

    }

	public int getAwardIndex()
	{
		for(int i=0;i<awards.Count;i++)
		{
			if((ArenaAwardManager.Instance.awardCanReceive(awards[i]) && ArenaAwardManager.Instance.getArenaAwardInfo(awards[i]) != null && !ArenaAwardManager.Instance.getArenaAwardInfo(awards[i]).received))
				return i;
		}
		return 0;
	}
}
