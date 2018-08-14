using UnityEngine;
using System.Collections;

public class PvpComboBarCtrl : MonoBehaviour
{
public	UILabel text; 
	Award[] awards;
	public	WindowBase fatherWindow;
	// Use this for initialization
	public 	void init (string str,Award[] awards)
	{
		iTween.ValueTo (gameObject, iTween.Hash ("from", 800, "to", 0, "easetype", iTween.EaseType.easeInOutCubic, "onupdate", "moveUpdate", "oncomplete", "", "time", 0.3f  ));
		iTween.ValueTo (gameObject, iTween.Hash ("delay", 1.5f  , "from", 0, "to", -800, "easetype", iTween.EaseType.easeInOutCubic, "onupdate", "moveUpdate", "oncomplete", "buffComplete", "time", 0.3f  ));
		text.text=str;
		this.awards=awards;
	}

	void buffComplete ()
	{
		gameObject.SetActive(false);
		PvpInfoManagerment.Instance.setGetPrize();
		AwardDisplayCtrl ctrl = BattleManager.Instance.gameObject.AddComponent<AwardDisplayCtrl> ();
		ctrl.Initialize (awards, AwardManagerment.PVP);
		fatherWindow.finishWindow();
		ctrl.openNextWindow();
	//	UiManager.Instance.destoryWindowByName("effectBlackWindow");
		
	}
	// Update is called once per frame
	void moveUpdate (float data)
	{
		transform.localPosition = new Vector3(data,	transform.localPosition .y,	transform.localPosition .z);
	}
}
