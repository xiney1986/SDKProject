using UnityEngine;
using System.Collections;

public class buffTipCtrl : MonoBase
{
	public float deadDelay ;
	public float speed = 1 ;		
	public UILabel label;
	[HideInInspector]
	public Transform parent;
	private Vector3 lastPosition=Vector3.zero;

	public void init ( string _text ,Transform _parent )
	{
		label.text = _text;
		parent=_parent;
	}
	
	// Use this for initialization
	void Start ()
	{ 
		//由于Buff 可能会跟随触发者 移动，所以这里就不做缓动了
		//gameObject.transform.localPosition += new Vector3 (0, 0, 50f);		
		///iTween.ValueTo (gameObject, iTween.Hash ("onupdate", "moveUpdate", "from", gameObject.transform.localPosition, "to", gameObject.transform.localPosition + new Vector3 (0, 30, 0f), "time", 0.1f * speed, "easetype", iTween.EaseType.easeOutSine));
		//iTween.ValueTo (gameObject, iTween.Hash ("delay", 0.1f * speed, "onupdate", "moveUpdate", "from", gameObject.transform.localPosition + new Vector3 (0, 30, 0f), "to", gameObject.transform.localPosition, "oncomplete", "DoOver", "time", 0.6f * speed, "easetype", iTween.EaseType.easeOutElastic));
	}
	
	void moveUpdate (Vector3 Pos)
	{
		//transform.localPosition = Pos; 
	}
	
	// Update is called once per frame
	void Update ()
	{  
		if(parent.localPosition!=lastPosition)
		{
			lastPosition=parent.localPosition;
			Vector3 pos = BattleManager.Instance.BattleCamera.WorldToScreenPoint (lastPosition + new Vector3 (0, 0.2f, 0.1f));
			transform.localPosition = UiManager.Instance.gameCamera.ScreenToWorldPoint (pos);
		}

		deadDelay -= Time.deltaTime;
		if (deadDelay <= 0)
			EffectManager.Instance.removeEffect (this); 
	}
	
}