using UnityEngine;
using System.Collections;

public class WhiteScreenWindow : WindowBase
{
    public UITexture tex;

	protected override void begin ()
	{
		base.begin ();
		//MaskWindow.UnlockUI ();
		TweenAlpha   ta=  TweenAlpha.Begin(tex.gameObject, 2.1f, 1);
		EventDelegate.Add(ta.onFinished, () => {
			finishWindow();
		}, true);
	}
}
