using UnityEngine;
using System.Collections;

public class GoddessItem : ButtonBase
{

	public UITexture icoHead;
	[HideInInspector]
	public BeastEvolve chooseItem;

	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
		if(gameObject.name=="summon"){
			UiManager.Instance.openWindow<BeastAttrWindow> ((win) => {
				if (GuideManager.Instance.isEqualStep (16003000)) {
					GuideManager.Instance.doGuide ();
					win.Initialize (chooseItem.getBeast (), BeastAttrWindow.RESONANCE);
				} else {
					win.Initialize (chooseItem.getBeast (), BeastAttrWindow.RESONANCE);
				}
			});
		}else{
			MaskWindow.UnlockUI();
		}				
	}
}
