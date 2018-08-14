using UnityEngine;
using System.Collections;

public class ButtonExchangeCondition : ButtonBase
{
public object fatherBar;

	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
		
		if(fatherBar.GetType()==typeof(ExchangeBarCtrl))
		{
			UiManager.Instance.openDialogWindow<ConditionsWindow>((win)=>{
				ExchangeBarCtrl bar=fatherBar as ExchangeBarCtrl;
				win.Initialize(	ExchangeSampleManager.Instance.getExchangeSampleBySid(bar.exchange.sid));
			});

		}
		if(fatherBar.GetType()==typeof(NoticeActivityExchangeBarCtrl))
		{
			UiManager.Instance.openDialogWindow<ConditionsWindow>((win)=>{
				NoticeActivityExchangeBarCtrl bar=fatherBar as NoticeActivityExchangeBarCtrl;
				win.Initialize(	ExchangeSampleManager.Instance.getExchangeSampleBySid(bar.exchange.sid));
			});


		}
	}
}
