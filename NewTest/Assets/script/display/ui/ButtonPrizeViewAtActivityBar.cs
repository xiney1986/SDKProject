using UnityEngine;
using System.Collections;

public class ButtonPrizeViewAtActivityBar : ButtonBase
{
	public 	Mission mission;
	
	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
		UiManager.Instance.openDialogWindow<AllAwardViewWindow> (
			(awin) => {
			awin.Initialize (mission.getPrizes ());
		}
		);
	}
}
