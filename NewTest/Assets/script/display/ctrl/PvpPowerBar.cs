using UnityEngine;
using System.Collections;

public class PvpPowerBar : barCtrl
{
	public GameObject[] fronts;

	protected override void updateBar ()
	{
		//base.updateBar ();
		if (maxValue == 0)
			return;
		for (int i = 0; i < fronts.Length; i++) {
			fronts[i].SetActive(newValue > i);
		}
	}
}
