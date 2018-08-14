using UnityEngine;
using System.Collections;

public class MissionCompleteBarCtrl : MonoBehaviour
{
	

	// Use this for initialization
	public 	void init ()
	{ 

		iTween.ValueTo (gameObject, iTween.Hash ("from", 800, "to", 0, "easetype", iTween.EaseType.easeInOutCubic, "onupdate", "", "oncomplete", "complete", "time", 3f));
	//	iTween.ValueTo (gameObject, iTween.Hash ("delay", 1f  , "from", 0, "to", -800, "easetype", iTween.EaseType.easeInOutCubic, "onupdate", "moveUpdate", "oncomplete", "complete", "time", 0.3f  ));
	 
	}

	void complete ()
	{
		MissionManager.instance.showCompleteEffectOver();
		gameObject.SetActive (false);
		UiManager.Instance.cancelMask();
	}

}
