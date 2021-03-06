using UnityEngine;
using System.Collections;
using System.Collections.Generic;
 
// 反击字体显示器
public class ReboundLabelCtrl : MonoBase
{
	public float deadDelay ;
	 float speed = 0.5f ;		
	//public float duration;
	public UILabel label;
	public UISprite ico;
 
	void Start ()
	{ 
		gameObject.transform.localPosition += new Vector3 (0, 50f, 50f);
		iTween.ValueTo (gameObject, iTween.Hash ("onupdate", "moveUpdate", "from", gameObject.transform.localPosition, "to", gameObject.transform.localPosition + new Vector3 (0, 30, 0f), "time", 0.1f * speed, "easetype", iTween.EaseType.easeOutSine));
		iTween.ValueTo (gameObject, iTween.Hash ("delay", 0.1f * speed, "onupdate", "moveUpdate", "from", gameObject.transform.localPosition + new Vector3 (0, 30, 0f), "to", gameObject.transform.localPosition, "oncomplete", "DoOver", "time", 0.6f * speed, "easetype", iTween.EaseType.easeOutElastic));
	}
	
	void moveUpdate (Vector3 Pos)
	{
		transform.localPosition = Pos; 
	}

	// Update is called once per frame
	void Update ()
	{  
		deadDelay -= Time.deltaTime;
		if (deadDelay <= 0)
			EffectManager.Instance.removeEffect (this); 
	}

}
