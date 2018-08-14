using UnityEngine;
using System.Collections;

public class BoxShakeCtrl : MonoBase {
	/** 持续的时间 */
	public float life;
	/** 开始时间 */
	public float startlife;
	public Transform point;
	float lifeNow;
	protected CharacterData target;//目标
	protected CharacterData owner;	//发射者
	public void  initEffect (Transform _point) {
		point = _point;
	}
	void Start () { 
		lifeNow = life;
	}
	
	protected void destory () {
		EffectManager.Instance.removeEffect (this);	
		
	}
	void Update () { 

		if (life == -1)
			return;
		lifeNow -= Time.deltaTime;
		if (lifeNow < 0.0f)
			EffectManager.Instance.removeEffect (this); 
	}
}
