using UnityEngine;
using System.Collections.Generic;

public class PracticeAutoRunAwardContent : MonoBase {
	public GoodsView[] goodsList;
	public UITexture awardIcon;
	private Vector3 srcPosition;

	public static int[] prizeSampleList;

	void Awake () {
		awardIcon.gameObject.SetActive (false);
	}
	public void updateAward ( int _index ) {
		List<PrizeSample> totalPrize=FuBenPracticeConfigManager.Instance.getTotalPrizeByIndex (_index);
		for (int i=0; i < totalPrize.Count; i++) {
			if (i < goodsList.Length) {
				goodsList[i].init (totalPrize[i]);
			}
		}
		srcPosition = awardIcon.transform.localPosition;
	}

	public Vector3 getPrizeDisplayPosition ( PrizeSample prize ) {
		PrizeSample p;
		for (int i=0; i < goodsList.Length; i++) {
			p = goodsList[i].prize;
			if (p != null && p.pSid == prize.pSid) {
				return goodsList[i].transform.localPosition;
			}
		}
		return Vector3.zero;
	}


	public void addPrizeAnimation ( PrizeSample _prize, int _toIndex ) {
		UITexture awardIcon = (Instantiate (this.awardIcon) as UITexture);
		awardIcon.transform.parent = this.awardIcon.transform.parent;
		awardIcon.transform.position = this.awardIcon.transform.position;
		awardIcon.gameObject.SetActive (true);
		ResourcesManager.Instance.LoadAssetBundleTexture (_prize.getIconPath (), awardIcon,(obj)=>{
			TweenScale tweenScale = awardIcon.GetComponent<TweenScale> ();
			tweenScale.ResetToBeginning ();
			EventDelegate.Add (tweenScale.onFinished, () => { 
				onScaleFinish (awardIcon.gameObject, _prize, _toIndex); 
			});
			tweenScale.Play(true);
		});
	}
	private void onScaleFinish ( GameObject awardIcon, PrizeSample _prize, int _toIndex ) {
		TweenPosition tweenPosition=awardIcon.GetComponent<TweenPosition> ();
		tweenPosition.ResetToBeginning ();
		EventDelegate.Add (tweenPosition.onFinished, () => { onMoveFinish (awardIcon, _prize, _toIndex); });
		tweenPosition.from = srcPosition;
		tweenPosition.to = getPrizeDisplayPosition (_prize);
		tweenPosition.Play(true);
	}
	private void onMoveFinish ( GameObject awardIcon, PrizeSample _prize, int _toIndex ) {
		Destroy (awardIcon);
		updateAward (_toIndex);
	}
}


