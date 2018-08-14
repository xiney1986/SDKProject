using UnityEngine;
using System.Collections;

public class ConditionsWindow : WindowBase
{
	public  UILabel[] conditionsLabel;
	ExchangeSample sample;
	public UILabel  CloseDescript;

	protected override void begin ()
	{
		base.begin ();
		MaskWindow.UnlockUI ();
	}

	public void Initialize (ExchangeSample _sample)
	{

		sample = _sample;
		string[] str = ExchangeManagerment.Instance.getAllPremises (sample);
		for (int i=0; i< str.Length; i++) {
			
			if (i >= 5)
				continue;
			if(str[i].StartsWith(Colors.GREEN))str[i]="[3A9663]"+str[i].Substring(Colors.GREEN.Length);
			else str[i]="[C65843]"+str[i];
			conditionsLabel [i].text = str [i]; 
		}		
		 
	}

	public void Initialize (BeastEvolve beastEvo)
	{
		string[][] str = beastEvo.getAllPremises (beastEvo);
		int num=0;
		for (int i=0; i< str.Length; i++) {
			for(int j=0;j<str[i].Length;j++){
				if (i >= 5)
					continue;
				conditionsLabel [num].text = str [i][j]; 
				num++;
			}

		}		
		
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		hideWindow ();
	}

	void Update ()
	{
		CloseDescript.alpha =sin();
	}
	 
}
