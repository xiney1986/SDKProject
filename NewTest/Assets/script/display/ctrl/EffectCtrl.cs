using UnityEngine;
using System.Collections;

/** 
  * 特效图形控制器 
  * @author 李程
  * */


public class EffectCtrl : MonoBase
{
	/** 持续的时间 */
	public float life;
	/** 开始时间 */
	public float startlife;
	public Transform point;
	float lifeNow;


	protected CharacterData target;//目标
	protected CharacterData owner;	//发射者
	// EffectCtrl AoeEffect;



	public void  initEffect (Transform _point)
    {
        point = _point;
    }



	void Start ()
	{ 
		lifeNow = life;
	}

	protected void destory ()
	{
		EffectManager.Instance.removeEffect (this);	
 
	}
	public void destoryThis ()
	{
		EffectManager.Instance.removeEffect (this);	
		
	}

	// Update is called once per frame
	void Update ()
	{ 

		if (life == -1)
			return;

		lifeNow -= Time.deltaTime;
		if (lifeNow < 0.0f)
			EffectManager.Instance.removeEffect (this); 
	}
}
