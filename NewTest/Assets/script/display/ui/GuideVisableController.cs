using UnityEngine;
using System.Collections;

public class GuideVisableController : MonoBehaviour {

	public int sid;
	public int level;
	public bool useAnimation = true;

	Vector3 pos;
	float time = 0;

	// Use this for initialization
	void Start () {
		if (GameManager.Instance.skipGuide || isCompleted()) {
			Destroy (this);
		}else {
			pos = transform.localPosition;
			transform.localPosition = new Vector3(100000,100000,0);
		}
	}
	
	// Update is called once per frame
	void Update () {

		time += Time.deltaTime;
		if (time < 0.1f)
			return;
		time = 0;

		if (isCompleted()) {
			transform.localPosition = pos;
			if(useAnimation)
			{
				Vector3 scale = transform.localScale;
				transform.localScale = new Vector3(0.1f,0.1f,0.1f);
				TweenScale.Begin(gameObject,0.3f,scale).method = UITweener.Method.EaseOut;
				EffectManager.Instance.CreateEffect(transform,"Effect/UiEffect/feature_open");
			}
			Destroy(this);
		} else {
			if (transform.localPosition.x != 100000) {
				transform.localPosition = new Vector3(100000,100000,0);
			}
		}
	}


	bool isCompleted()
	{
		if (sid > 0 && level == 0)
			return GuideManager.Instance.isOverStep(sid);
		else if (sid == 0 && level > 0)
			return UserManager.Instance.self.getUserLevel () >= level;
		else
			return GuideManager.Instance.isOverStep(sid) && UserManager.Instance.self.getUserLevel () >= level;
	}
}
