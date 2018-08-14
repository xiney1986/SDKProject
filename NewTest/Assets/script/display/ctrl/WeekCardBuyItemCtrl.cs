using UnityEngine;
using System.Collections;

public class WeekCardBuyItemCtrl : MonoBehaviour
{
	public UILabel lab_rmbCount;
	public UILabel lab_timeInfo;
	public ButtonBase btn_buy;
	private WeekCardSample sample;

	public void init(WeekCardSample _sample,WindowBase _parent)
	{
		sample = _sample;
		if(sample != null)
		{
			lab_timeInfo.text=LanguageConfigManager.Instance.getLanguage(sample.des);
			lab_rmbCount.text=sample.costDiamond.ToString();
			btn_buy.fatherWindow=_parent;
			btn_buy.gameObject.name=sample.id.ToString();
		}
	}
}