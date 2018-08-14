using UnityEngine;
using System.Collections;
using System.Collections.Generic;
 
// 伤害字体显示器
//@author 李程
 
public class HitTextCtrl : MonoBase
{

	CharacterCtrl character;
	Transform hitPointTrans;
	Vector3 offset;
	public float deadDelay ;
	public float speed = 1 ;
	//public float duration;
	public Transform itemTrans;
	public UILabel label;
	public UISprite ico;

	public void init (CharacterCtrl target, int  damage, int DamageType,Vector3 offsetVector)
	{
		this.offset=offsetVector;
		character = target;
		hitPointTrans=character.hitPoint.transform;
		label.text = damage.ToString ();
		
		if (damage > 0) {
			label.color = Color.green;	
			return;
		} 
		int num = 0;
		switch (DamageType) {
		case BuffDamageType.poison: 
			num = label.text.Length;
			label.gameObject.transform.localPosition = new Vector3 (label.gameObject.transform.localPosition.x - (num - 1) * 7, label.gameObject.transform.localPosition.y, label.gameObject.transform.localPosition.z);

			ico.gameObject.SetActive (true);
			ico.width = 61;
			ico.height = 41;
			ico.spriteName = "poison_text";
			break;
		case BuffDamageType.beIntervene: 
						//援护
			num = label.text.Length;
			label.gameObject.transform.localPosition = new Vector3 (label.gameObject.transform.localPosition.x - (num - 1) * 7, label.gameObject.transform.localPosition.y, label.gameObject.transform.localPosition.z);
			ico.width = 88;
			ico.height = 41;
			ico.gameObject.SetActive (true);
			ico.spriteName = "intervene";
			break;

		/* 优化1492 反击出现在人物启动时
				case BuffDamageType.beRebound: 
						num = label.text.Length;
						label.gameObject.transform.localPosition = new Vector3 (label.gameObject.transform.localPosition.x - (num - 1) * 10, label.gameObject.transform.localPosition.y, label.gameObject.transform.localPosition.z);
						ico.width = 88;
						ico.height = 41;
						ico.gameObject.SetActive (true);
						ico.spriteName = "rebound";
						break;
						*/
		} 
       
	}
	 
	void Start ()
	{ 
		itemTrans.localPosition += new Vector3 (0, 0, 50f);
		iTween.ValueTo (gameObject, iTween.Hash ("onupdate", "moveUpdate", "from", itemTrans.localPosition, "to", itemTrans.localPosition + new Vector3 (0, 30, 0f), "time", 0.1f * speed, "easetype", iTween.EaseType.easeOutSine));
		iTween.ValueTo (gameObject, iTween.Hash ("delay", 0.1f * speed, "onupdate", "moveUpdate", "from", itemTrans.localPosition + new Vector3 (0, 30, 0f), "to", itemTrans.localPosition, "oncomplete", "DoOver", "time", 0.6f * speed, "easetype", iTween.EaseType.easeOutElastic));
	}
	
	void moveUpdate (Vector3 Pos)
	{
		itemTrans.localPosition = Pos; 
//		// 伤害跟随卡片移动
		Vector3 pos = BattleManager.Instance.BattleCamera.WorldToScreenPoint (hitPointTrans.position + offset);
		transform.position = UiManager.Instance.gameCamera.ScreenToWorldPoint (pos);
	}

	// Update is called once per frame
	void Update ()
	{
		deadDelay -= Time.deltaTime;
		if (deadDelay <= 0)
			EffectManager.Instance.removeEffect (this); 
	}
}
