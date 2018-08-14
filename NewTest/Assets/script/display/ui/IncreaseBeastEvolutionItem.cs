using UnityEngine;
using System.Collections;

public class IncreaseBeastEvolutionItem : ButtonBase {

	public BeastEvolve beast;

	public void initButton(BeastEvolve _beast)
	{
		beast = _beast;
	}

	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
		if(beast != null)
		{
			fatherWindow.finishWindow();
			EventDelegate.Add(fatherWindow.OnHide,()=>{
				UiManager.Instance.openWindow<BeastSummonWindow>((win)=>{
					win.Initialize (beast);
					win.oldCard=beast.getBeast().Clone() as Card;
					if(!beast.isEndBeast()) {
						win.newCard = beast.getNextBeast();
						win.newCard.updateExp(win.oldCard.getEXP());
					}
					else
						win.newCard = win.oldCard;
					win.exp = BeastEvolveManagerment.Instance.getHallowExp();
				});
			});
		}
	}
}
