using UnityEngine;
using System.Collections;


/** 
  * 伤害字体显示控制器
  * @author 李程
  * */ 
public class SkillNameCtrl : MonoBase
{

	CharacterCtrl character;
	public float duration = 0.3f;
	public float deadDelay ;	
	
	//public float duration;
	public UILabel label;
	public UISprite ico;
	public GUI_anim_class AnimClass;
	public Vector2 scaleFromTo = new Vector2 (0.2f, 1f);
	
	public 	void init (CharacterCtrl target, string str)
	{
		character = target;
		label.text = str;
		ico = null;
	}
	
	// Use this for initialization
	void Start ()
	{
		//Vector3 offset=new Vector3(0,fatherOrg.OrgCtrl.bulletHeightOffset,0); 
		switch (AnimClass) {
		case GUI_anim_class.ScaleOutBack:	
			
			iTween.ValueTo (gameObject, iTween.Hash ("delay", 0.2f  , "onupdate", "moveUpdate", "from", new Vector3 (scaleFromTo.x, scaleFromTo.x, scaleFromTo.x), "to", new Vector3 (scaleFromTo.y, scaleFromTo.y, scaleFromTo.y), "oncomplete", "DoOver", "time", duration  , "easetype", iTween.EaseType.easeOutBack));
			break; 
		} 
	}
	
	void moveUpdate (Vector3 scleP)
	{
		transform.localScale = scleP;
		
	}

	void DoOver ()
	{
 
	}
	// Update is called once per frame
	void Update ()
	{ 
		//	transform.localPosition+=new Vector3(0,0,100*Time.deltaTime);
		deadDelay -= Time.deltaTime ;
		if (deadDelay <= 0)
			EffectManager.Instance.removeEffect (this); 
	} 
}
