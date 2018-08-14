using UnityEngine;
using System.Collections;


public class WindowLinkSample {

	public WindowLinkSample () {

	}

	public int sid;
	public string windowClassName = "";
	public string[] windowArgs;
	public bool isDialog;


	public void parse ( int id, string str ) {
		this.sid = id;
		string[] strArr = str.Split ('|');
		string[] window = strArr[1].Split(',');
		windowClassName = window[0];
		if (window.Length > 1)
			windowArgs = window[1].Split('#');
		else
			windowArgs = new string[0];
		isDialog = strArr[2] == "1";
	}




}
