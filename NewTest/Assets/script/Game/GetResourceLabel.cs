using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//副本拾取

public class GetResourceLabel : MonoBase
{
	public float deadDelay ;
	public UILabel label;
	public UISprite ico;

	public void init (string  spName, int data)
	{
		ico.name=spName;
		label.text = data.ToString ();
	}
	void moveUpdate (Vector3 data)
	{
		transform.localPosition = data; 
	}
	void Start ()
	{
		iTween.ValueTo (gameObject, iTween.Hash ("onupdate", "moveUpdate", "from",  transform.localPosition, "to",  transform.localPosition+new Vector3(0,50f,0), "oncomplete", "DoOver", "time", deadDelay , "easetype", iTween.EaseType.easeOutCubic));
	}
	void DoOver ()
	{
		EffectManager.Instance.removeEffect (this); 
	}


}
