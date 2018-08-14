using UnityEngine;
using System.Collections;

public class PracticeFubenTipContent : MonoBase
{
	public GameObject root_newSavePoint;
	public GameObject root_toSavePoint;
	public UILabel label_newPoint;
	public UILabel label_toPoint;

	TweenScale tween_scale;
	TweenAlpha tween_alpha;
	void Awake()
	{
		tween_scale=GetComponent<TweenScale>();
		tween_alpha=GetComponent<TweenAlpha>();
	}

	public void playAni(int point,bool isNew)
	{
		if(point<1)
			return;
		gameObject.SetActive(true);
		root_newSavePoint.gameObject.SetActive(isNew);
		root_toSavePoint.gameObject.SetActive(!isNew);

		if(isNew)
		{
			label_newPoint.text=point.ToString();
		}else
		{
			label_toPoint.text=point.ToString();
		}

		EventDelegate.Add(tween_scale.onFinished,onFinishScale);
		tween_scale.ResetToBeginning();
		tween_scale.enabled=true;
	}
	private void onFinishScale()
	{
		EventDelegate.Remove(tween_scale.onFinished,onFinishScale);

		EventDelegate.Add(tween_alpha.onFinished,onFinishAlpha);
		tween_alpha.ResetToBeginning();
		tween_alpha.enabled=true;
	}
	private void onFinishAlpha()
	{
		EventDelegate.Remove(tween_alpha.onFinished,onFinishAlpha);
		gameObject.SetActive(false);
	}
}

