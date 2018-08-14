using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ResolveOnekeyWindow : WindowBase
{
	public UIToggle[] Quality_Box;
	public UIToggle neverChoose;
	public static bool IsOpenOneKeyWnd = true; //是否打开一键选择窗口
	public static int qualityChoose = QualityType.EXCELLENT; //一键选择的品质
	protected override void begin ()
	{
		base.begin ();
		MaskWindow.UnlockUI ();
	}
	
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "confirm") {
			if(fatherWindow != null && fatherWindow.GetType() == typeof(ResolveWindow)) {
				ResolveWindow resolveWnd = fatherWindow as ResolveWindow;
				if (Quality_Box [0].value)
					qualityChoose = QualityType.GOOD;
				else if (Quality_Box [1].value)
					qualityChoose = QualityType.EPIC;
				else if (Quality_Box [2].value)
					qualityChoose = QualityType.LEGEND;

				if (neverChoose.value)
					IsOpenOneKeyWnd = false;
			}
		}
		finishWindow ();
	}
}