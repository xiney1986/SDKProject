using UnityEngine;
using System.Collections;

public class BeastResonanceItem : ButtonBase {

	public UITexture icoHead;
	public UISprite icoBg;
	public UILabel labelName;
	public UILabel labelLevel;
	public BeastEvolve chooseItem;

	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
		UiManager.Instance.openWindow<BeastAttrWindow>((win)=>{
			win.Initialize(chooseItem.getBeast(),BeastAttrWindow.RESONANCE);
			fatherWindow.finishWindow();
		});
	}
}
