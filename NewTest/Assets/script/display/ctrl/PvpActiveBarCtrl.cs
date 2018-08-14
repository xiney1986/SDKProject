using UnityEngine;
using System.Collections;

public class PvpActiveBarCtrl : MonoBehaviour
{
	// Use this for initialization
	public 	void init ()
	{
 
		iTween.ValueTo (gameObject, iTween.Hash ("from", 800, "to", 0, "easetype", iTween.EaseType.easeInOutCubic, "onupdate", "moveUpdate", "oncomplete", "", "time", 0.3f  ));
		iTween.ValueTo (gameObject, iTween.Hash ("delay", 1f  , "from", 0, "to", -800, "easetype", iTween.EaseType.easeInOutCubic, "onupdate", "moveUpdate", "oncomplete", "complete", "time", 0.3f  ));
	 
	}

	void complete ()
	{
		PvpInfoManagerment.Instance.openPvpWindow();
		gameObject.SetActive (false);
		UiManager.Instance.cancelMask();
		MissionInfoManager.Instance.mission.restPointNoPvP =true;
		 //PvpInfoManagerment.Instance.openPvpWindow();

	}
	// Update is called once per frame
	void moveUpdate (float data)
	{
		transform.localPosition = new Vector3 (data, transform.localPosition .y, transform.localPosition .z);
	}
}
