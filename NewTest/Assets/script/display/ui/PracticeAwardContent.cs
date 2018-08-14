using UnityEngine;
using System.Collections.Generic;

public class PracticeAwardContent:MonoBase
{
	public GoodsView[] goodsList;
	public UITexture awardIcon;
	private Vector3 srcPosition;

	void Awake()
	{
		awardIcon.gameObject.SetActive(false);
	}


	public void addAllAward (int index) {
		List<PrizeSample> totalPrize=FuBenPracticeConfigManager.Instance.getTotalPrizeByIndex (index);
		for (int i=0; i < totalPrize.Count; i++) {
			if (i < goodsList.Length) {
				goodsList[i].init (totalPrize[i]);
			}
		}
		srcPosition = awardIcon.transform.localPosition;
	}


	public void updateAward(int _index)
	{
		//if(!gameObject.activeSelf)
		//	return;
		List<PrizeSample> totalPrize=FuBenPracticeConfigManager.Instance.getTotalPrizeByIndex(_index);
		for(int i=0;i<totalPrize.Count;i++)
		{
			if(i<goodsList.Length)
			{
				goodsList[i].init(totalPrize[i]);
			}
		}
		srcPosition=awardIcon.transform.localPosition;
	}

	public Vector3 getPrizeDisplayPosition(PrizeSample prize)
	{
		PrizeSample p;
		for(int i=0;i<goodsList.Length;i++)
		{
			p=goodsList[i].prize;
			if(p!=null&&p.pSid==prize.pSid)
			{
				return goodsList[i].transform.localPosition;
			}
		}
		return Vector3.zero;
	}
	
	private int toIndex;
	private PrizeSample tempPrize;
	public void addPrizeAnimation(PrizeSample _prize,int _toIndex)
	{
		tempPrize=_prize;
		toIndex=_toIndex;

		awardIcon.gameObject.SetActive(true);
		ResourcesManager.Instance.LoadAssetBundleTexture (_prize.getIconPath(), awardIcon);

		TweenScale tweenScale=awardIcon.GetComponent<TweenScale>();
		tweenScale.ResetToBeginning();
		EventDelegate.Add(tweenScale.onFinished,onScaleFinish);
		StartCoroutine(Utils.DelayRun(()=>{
			tweenScale.enabled=true;
		},0.5f));

	}
	private void onScaleFinish()
	{
		TweenScale tweenScale=awardIcon.GetComponent<TweenScale>();
		EventDelegate.Remove(tweenScale.onFinished,onScaleFinish);

		TweenPosition tweenPosition=awardIcon.GetComponent<TweenPosition>();
		tweenPosition.ResetToBeginning();
		EventDelegate.Add(tweenPosition.onFinished,onMoveFinish);
		tweenPosition.from=srcPosition;
		tweenPosition.to=getPrizeDisplayPosition(tempPrize);
		tweenPosition.enabled=true;
	}
	private void onMoveFinish()
	{
		TweenPosition tweenPosition=awardIcon.GetComponent<TweenPosition>();
		EventDelegate.Remove(tweenPosition.onFinished,onMoveFinish);

		awardIcon.transform.localPosition=srcPosition;
		awardIcon.gameObject.SetActive(false);
		updateAward(toIndex);
	}
}


