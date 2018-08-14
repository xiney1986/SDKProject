using UnityEngine;
using System.Collections;

public class NvShenBlessBuffCtrl : MonoBehaviour
{
	public UILabel nvShenBless;

	public 	void init (string str)
	{
		nvShenBless.text = string.Format(LanguageConfigManager.Instance.getLanguage("LastBattle_NvShenAddLV2"),str);
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
