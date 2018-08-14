using UnityEngine;
using System.Collections;

public class NewChapterWindow : WindowBase {

	public UILabel titleLabel;
	public UILabel descLabel;
	private CallBack cb;

	protected override void begin ()
	{
		base.begin ();
	}

	public void initWin(string title,string desc,CallBack _cb)
	{
		writeString(title,desc);
		cb = _cb;
	}

	void writeString (string title,string desc)
	{
		titleLabel.text = title;
		TweenAlpha lname = TweenAlpha.Begin (titleLabel.gameObject, 1f, 1);
		lname.from = 0;
		EventDelegate.Add (lname.onFinished, () => {
			descLabel.text = desc;
			TweenAlpha lname2 = TweenAlpha.Begin (descLabel.gameObject, 1f, 1);
			lname2.from = 0;
			EventDelegate.Add (lname2.onFinished, () => {
				StartCoroutine (Utils.DelayRun (() => {
					finishWindow();
					GuideManager.Instance.doGuide ();
					if (cb != null) {
						cb();
					}
//					GuideManager.Instance.guideEvent ();
				}, 0.2f));

			},true);
		},true);
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		
//		if (gameObj.name == "screenButton") {
//			finishWindow();
//			GuideManager.Instance.doGuide ();
//			GuideManager.Instance.guideEvent ();
//		}
	}
}
