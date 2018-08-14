using UnityEngine;
using System.Collections;

public class mapCtrl : MonoBehaviour
{
	
	public ButtonBase[] checkPointList;
	public WindowBase fatherWindow;
	// Use this for initialization
	Vector3 offset;
	bool lookAting = false;
	float time = 0.5f;
	SpringPosition spr;
	int maxIndex = 0;
	private CallBack moveOver;
	private int mapId = 0;
		
	void Start ()
	{
		foreach (ButtonBase each in checkPointList) {
			each.setFatherWindow (fatherWindow);
		}	 
	}
	
	void OnEnable ()
	{	
		 
	}
	
	private void onMove ()
	{
		time = 0.5f;
		offset = new Vector3 (-checkPointList [maxIndex].transform.localPosition.x, -checkPointList [maxIndex].transform.localPosition.y, transform.localPosition.z);
		lookAting = true;
		gameObject.GetComponent<UIDragObject> ().enabled = false;
		spr = gameObject.GetComponent<SpringPosition> ();
		if (spr != null)
			Destroy (spr);
	}
	
	public void setIsShows (int mapid, bool[] isShows, CallBack callback)
	{
		this.mapId = mapid;
		this.moveOver = callback;
		int max = checkPointList.Length;
		for (int i = 0; i < max; i++) {  
			int id = StringKit.toInt (checkPointList [i].name.Substring (8));	 
			int sid = FuBenManagerment.getStoryChapterByIndex (mapId, i + 1); 
			checkPointList [i].textLabel.text = ChapterSampleManager.Instance.getChapterSampleBySid (sid).name;
			checkPointList [i].gameObject.SetActive (isShows [i]);
			if (isShows [i])
				maxIndex = i;
		}
		onMove ();
	}

	void move (Vector3 _data)
	{
		transform.localPosition = _data;
	}

	void Update ()
	{
		if (lookAting) {
			transform.localPosition = Vector3.Lerp (transform.localPosition, offset, Time.deltaTime * 13);
//			print (	transform.localPosition);
			time -= Time.deltaTime;
			
			if (time <= 0) {
				transform.localPosition = offset;
				gameObject.GetComponent<UIDragObject> ().enabled = true;
				lookAting = false;
				moveOver ();
			}
		}
	}
 
}
