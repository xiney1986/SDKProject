using UnityEngine;
using System.Collections;

public class msgLineCtrl : MonoBehaviour {
	 
	public UILabel msgText;
	public UISprite backGround;
	public int targetY;
	MessageLineWindow fatherWindow;
	// Use this for initialization
	float life = 1.5f;

	public void Initialize (MessageLineWindow father, int y, string text) {
		targetY = y;
		fatherWindow = father;
		msgText.text = text;
	}

	void Update () {
		life -= Time.deltaTime;
		if (life <= 0)
			over ();
	}

	void Start () {
		
		iTween.ValueTo (gameObject, iTween.Hash ("from", 0, "to", 1, "onupdate", "changeAlpha", "oncomplete", "", "easetype", iTween.EaseType.linear, "time", 0.05f));
		iTween.ValueTo (gameObject, iTween.Hash ("from", new Vector3 (0, -600, 0), "to", new Vector3 (0, targetY, 0), "onupdate", "changePos", "oncomplete", "", "", iTween.EaseType.easeInOutExpo, "time", 0.1f));		
		iTween.ValueTo (gameObject, iTween.Hash ("delay", 1.5f, "from", 1, "to", 0, "onupdate", "changeAlpha", "easetype", iTween.EaseType.linear, "time", 0.1f));	
		iTween.ValueTo (gameObject, iTween.Hash ("from", 1, "to", 0, "onupdate", "", "oncomplete", "readNext", "easetype", iTween.EaseType.linear, "time", 0.1f));	
		
	}
	
	void readNext () {
		fatherWindow.readOne ();	
	}

	void over () {
		Destroy (gameObject);
		fatherWindow.finishWindow ();
	}

	void 	changePos (Vector3 pos) {
		transform.localPosition = pos;
	}
	
	void 	changeAlpha (float alpha) {
		msgText.alpha = alpha;
		backGround.alpha = alpha;
		
	}
}
