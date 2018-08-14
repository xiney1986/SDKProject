using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PrestigeSample : Sample {
	public 	string prestigeName;
	public 	string iconID;
	public  List<PrestigeEffectValue> prestigeEffect;
 
	public   PrestigeSample(string  str)
	{
		parse(str);
	}

	private void parse (string str)
	{
		string[] strArr = str.Split ('|');
		sid = StringKit.toInt(strArr [0]);
		iconID=strArr [1];
		prestigeName =strArr [2];

		prestigeEffect=new List<PrestigeEffectValue>();
		parsePrestigeEffect (strArr [3]);
	}

	void parsePrestigeEffect(string str){

		string[] strArr = str.Split (',');
		PrestigeEffectValue va=new PrestigeEffectValue();
		va.descript =strArr [0];
		va.currentValue = StringKit.toInt (strArr [1]);
		prestigeEffect.Add(va);
	}


}

public class PrestigeEffectValue{
	public string descript;
	public int currentValue;
}