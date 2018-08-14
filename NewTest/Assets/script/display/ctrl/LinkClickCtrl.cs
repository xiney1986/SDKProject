using UnityEngine;
using System.Collections;

public class LinkClickCtrl : MonoBase
{
	public WindowBase fatherWindow;
	public EnumLink type;
	void OnClick ()
	{

		Component tempC;
		UILabel label;

		tempC=GetComponent<UITextList>();
		if(tempC!=null)
		{
			label = (tempC as UITextList).textLabel;
		}else
		{
			label = GetComponent<UILabel>();
		}

		if (label != null)
		{
			string url = label.GetUrlAtPosition(UICamera.lastHit.point);
			if (!string.IsNullOrEmpty(url))
			{
				switch(type)
				{
				case EnumLink.laddersRecord:
					M_clickUrl_LaddersRecord(url);
					break;
				}
			}
		}

	}
	private void M_clickUrl_LaddersRecord(string _url)
	{
		if(fatherWindow.GetType()!=typeof(LaddersRecordsWindow))
			return;

		(fatherWindow as LaddersRecordsWindow).M_onClickUrl(_url);
	}
}
public enum EnumLink
{
	laddersRecord
}

