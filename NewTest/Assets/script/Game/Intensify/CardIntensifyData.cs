using System.Collections.Generic;
using System;
	
/**
 * 卡片进化技能和卡片学习技能格式
 * @author 汤琦
 * */

public class CardIntensifyData : IntensifyData
{
	private List<string> forms = new List<string> ();
	 
	public void addFood (string userUid, int abilityUid)
	{
		string str = userUid + "," + abilityUid.ToString ();
		forms.Add (str);
	}
	
	public override string ToFooding ()
	{
		string form = "";
		for (int i = 0; i < forms.Count; i++) {
			if (i == 0) {
				form += forms [i];
			} else {
				form += ";" + forms [i];
			}
		}
		return form;
	}
}
