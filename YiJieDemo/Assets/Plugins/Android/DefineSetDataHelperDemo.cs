using UnityEngine;
using System.Collections;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
using System;
using System.IO;  
using System.Net;
using Common;
using System.Globalization;

public class DefineSetDataHelperDemo : MonoBehaviour
{


	Rect windowRect = new Rect (0, 0, 400, 600);
	string str = "setData";
	
	public Boolean isDebug = false;
	public string keyName = "";
	public GUISkin guiSkin;
	
	GUI.WindowFunction windowFunction;

	void OnGUI ()
	{
		if (guiSkin) {   
			GUI.skin = guiSkin;
		} 
		windowRect = GUI.Window (3, windowRect, DoSetDataWindow, str);
	}
	
		
	public  void DoSetDataWindow (int windowID)
	{		
		GUI.Label (new Rect (20, 30, 100, 40), "setData:");
		keyName = GUI.TextField (new Rect (130, 30, 100, 40), keyName, 15);
		if (GUI.Button (new Rect (10, 200, 100, 50), "OK")) {
				Debug.Log ("------------DoSetDataWindow------------" );
				PlayerPrefs.SetString ("setDataType", "setData");
				PlayerPrefs.SetString ("keyName", keyName);
				PlayerPrefs.SetInt ("login", 1);
				Application.LoadLevel ("MainCamera");
		}
		if (GUI.Button (new Rect (150, 200, 100, 50), "Cancel")) {
			Debug.Log ("------------DoSetDataWindow2------------" );
			PlayerPrefs.SetInt ("login", 1);
			Application.LoadLevel ("MainCamera");
		}
		Debug.Log ("-------------------");
	}

	void Start ()
	{
		windowFunction = DoSetDataWindow;
	}
	void Update ()
	{
	    if(Input.GetKey(KeyCode.Backspace) || Input.GetKey(KeyCode.Escape)){
            Application.LoadLevel ("MainCamera");	
        }
	}
	void Awake ()
	{
	
	}
}
