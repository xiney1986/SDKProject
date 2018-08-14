using UnityEngine;
using System.Collections;

public class maxPowerBuffCtrl : MonoBehaviour
{
	// Use this for initialization
	public 	void init ()
	{
		iTween.ValueTo (gameObject, iTween.Hash ("from", 800, "to", 0, "easetype", iTween.EaseType.easeInOutCubic, "onupdate", "moveUpdate", "oncomplete", "", "time", 0.3f  ));
		iTween.ValueTo (gameObject, iTween.Hash ("delay", 1.5f  , "from", 0, "to", -800, "easetype", iTween.EaseType.easeInOutCubic, "onupdate", "moveUpdate", "oncomplete", "buffComplete", "time", 0.3f  ));
	 
	}

	void buffComplete ()
	{
		gameObject.SetActive(false);
	}
	// Update is called once per frame
	void moveUpdate (float data)
	{
		transform.localPosition = new Vector3(data,	transform.localPosition .y,	transform.localPosition .z);
	}
}
