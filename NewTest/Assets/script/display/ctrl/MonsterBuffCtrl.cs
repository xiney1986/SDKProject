using UnityEngine;
using System.Collections;

public class MonsterBuffCtrl : MonoBehaviour
{

	public UITexture IconPicture;
	public UILabel  buffTitle;


	// Use this for initialization
	public 	void init (string buffName, string PicName,bool isPlayer)
	{
		
		ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.GODDESS_HEAD + PicName + "_bighead",IconPicture);
		IconPicture.gameObject.SetActive(true);
		buffTitle.text=buffName;
		int from = 800;
		if (isPlayer) {
			from = -800;
		}
	 
		iTween.ValueTo (gameObject, iTween.Hash ("from", from, "to", 0, "easetype", iTween.EaseType.easeInOutCubic, "onupdate", "moveUpdate", "oncomplete", "", "time", 0.3f  ));
		iTween.ValueTo (gameObject, iTween.Hash ("delay", 2f  , "from", 0, "to", -from, "easetype", iTween.EaseType.easeInOutCubic, "onupdate", "moveUpdate", "oncomplete", "buffComplete", "time", 0.3f  ));
	 
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
