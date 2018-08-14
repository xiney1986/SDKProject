using UnityEngine;
using System.Collections;

public class flyEquipCtrl : MonoBehaviour
{

	IntensifyEquipWindow fatherWindow;
	public UITexture itemImage;

	public void Initialize (Texture image, IntensifyEquipWindow fatherWindow)
	{
		itemImage.mainTexture = image;
 
		this.fatherWindow = fatherWindow;
		transform.localScale = Vector3.one;
		iTween.MoveTo (gameObject, iTween.Hash ("position", transform.position + new Vector3 (0, 0.1f, 0), "easetype", iTween.EaseType.easeInOutCubic, "time", 1f));	
		iTween.ScaleTo (gameObject, iTween.Hash ("scale", new Vector3 (1.4f, 1.4f, 1.4f), "easetype", iTween.EaseType.easeInOutCubic, "time", 1f));	
		
		Vector3 pos = fatherWindow.main.transform.position;
		
		iTween.MoveTo (gameObject, iTween.Hash ("delay", 1.1f, "position", pos, "easetype", "easeInQuad", "time", 0.2f));
		iTween.ScaleTo (gameObject, iTween.Hash ("delay", 1.1f, "scale", new Vector3 (0.2f, 0.2f, 0.2f), "easetype", "easeInQuad", "oncomplete", "over", "time", 0.2f));	

	}
	
	void over ()
	{
		gameObject.SetActive (false);
	}

}
