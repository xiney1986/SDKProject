using UnityEngine;
using System.Collections;

public class MonthCardBuyItemCtrl : MonoBehaviour
{
	public UILabel lab_rmbCount;
	public UILabel lab_timeInfo;
	public ButtonBase btn_buy;

	public void init(int _index,string _timeInfo,int _count,WindowBase _parent,int _sid)
	{
		lab_timeInfo.text=LanguageConfigManager.Instance.getLanguage(_timeInfo);
		lab_rmbCount.text=_count.ToString();
		btn_buy.fatherWindow=_parent;
		btn_buy.gameObject.name=_sid.ToString();
	}
}