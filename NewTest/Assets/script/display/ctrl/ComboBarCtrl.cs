using UnityEngine;
using System.Collections;

public class ComboBarCtrl : MonoBase
{
	public UILabel numLabel;
	public int state = -2; //-2:开始状态 -1进场中 0 normal  1出场中 
	public float outTime = 2f;
	public UILabel hertNum;//伤害总值

	public 	void init (int i,int hertNu,bool bo)
	{
		gameObject.SetActive(true);
		hertNum.text=(-hertNu).ToString();
		if(bo){
			numLabel.transform.localScale = new Vector3 (8, 8, 8);
			numLabel.text = i.ToString ();
		}
		if (state == -2) {
			state = -1;
			iTween.ValueTo (gameObject, iTween.Hash ("from", 800, "to", 0, "easetype", iTween.EaseType.easeInOutCubic, "onupdate", "moveUpdate", "oncomplete", "incomplete", "time", 0.1f));
			return;
		} else if (state == 0) {
			outTime = 2f;
			numLabel.transform.localScale = new Vector3 (8, 8, 8);
			numLabel.text = i.ToString ();
			iTween.ScaleTo (numLabel.gameObject, iTween.Hash ("scale", new Vector3 (2.5f, 2.5f, 2.5f), "easetype", iTween.EaseType.easeInCubic, "time", 0.1f));
			iTween.ValueTo (numLabel.gameObject, iTween.Hash ("from", 0.2f, "to", 1, "easetype", iTween.EaseType.easeInCubic, "onupdate", "alphaUpdate", "time", 0.1f));
		}
	}
	public 	void init (int i,int hertNu)
	{
		gameObject.SetActive(true);
		hertNum.text=(-hertNu).ToString();
		if (state == -2) {
			state = -1;
			iTween.ValueTo (gameObject, iTween.Hash ("from", 800, "to", 0, "easetype", iTween.EaseType.easeInOutCubic, "onupdate", "moveUpdate", "oncomplete", "incomplete", "time", 0.1f));
			return;
		} else if (state == 0) {
			outTime = 2f;
			numLabel.transform.localScale = new Vector3 (8, 8, 8);
			numLabel.text = i.ToString ();
			iTween.ScaleTo (numLabel.gameObject, iTween.Hash ("scale", new Vector3 (2.5f, 2.5f, 2.5f), "easetype", iTween.EaseType.easeInCubic, "time", 0.1f));
			iTween.ValueTo (numLabel.gameObject, iTween.Hash ("from", 0.2f, "to", 1, "easetype", iTween.EaseType.easeInCubic, "onupdate", "alphaUpdate", "time", 0.1f));
		}
	}

	void moveUpdate (float data)
	{
		transform.localPosition = new Vector3 (data, transform.localPosition .y, transform.localPosition .z);
	}
	void alphaUpdate (float data)
	{
		numLabel.alpha =data; 
	}
	void incomplete ()
	{
		state = 0;
	}

	void outcomplete ()
	{
		//终结清理
		state = -2;
		outTime = 2f;
		numLabel.text ="1";
		gameObject.SetActive(false);
	}

	// Update is called once per frame
	void  Update ()
	{
		//进出场中就return
		if (state != 0)
			return;

		outTime -= Time.deltaTime;

		if (outTime <= 0) {
			iTween.ValueTo (gameObject, iTween.Hash ("from", 0, "to", 800, "easetype", iTween.EaseType.easeInOutCubic, "onupdate", "moveUpdate", "oncomplete", "outcomplete", "time", 0.1f ));
			state = 1;
		}
	}
}
