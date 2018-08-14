using UnityEngine;
using System.Collections;

public class ButtonRuleShow : ButtonBase {

	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
		UiManager.Instance.openDialogWindow<LadderRuleWindow>((win) => {
			win.initWin();
		
		});
	}
}
