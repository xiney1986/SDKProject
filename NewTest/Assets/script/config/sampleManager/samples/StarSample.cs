using UnityEngine;
using System.Collections;

public class StarSample : Sample
{
	public StarSample ()
	{

	}

	public string name;
	public int iconId;
	public string languageId;

	public override void parse (int sid, string str)
	{
		this.sid = sid;
		string[] strArr = str.Split ('|');
		parseInfo (strArr);
	}

	private void parseInfo (string[] str)
	{
		iconId = StringKit.toInt (str [1]);
		name = str [2];
		languageId = str [3];
	}

	public int getIconId ()
	{
		return iconId;
	}

	public string getName ()
	{
		return name;
	}

	public string getLanguageId ()
	{
		return languageId;
	}

	public override void copy (object destObj)
	{
		base.copy (destObj);
	}
}
