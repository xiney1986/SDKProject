using UnityEngine;
using System.Collections;

/// <summary>
/// 精灵动画控制器
/// </summary>
public class SpriteAniCtrl : MonoBase {

	/** 精灵行为常量 */
	const int ACTION_IDLE=0, // 休闲行为
				ACTION_TALK=1, // 走动行为
				ACTION_ATTACK=2; // 攻击行为

	/* fields */
	/** 移动速度 */
	public float moveSpeed=0.5f;
	/** 动画 */
	Animator anim;
	/** 行为值 */
	int actionValue;

	/* methods */
	/** Start */
	void Awake () {
		anim = GetComponent<Animator> (); //获取动画对象
		actionValue = ACTION_IDLE;
	}
	/** 走动动画 */
	public void playWalk () {
		anim.SetInteger ("Action", ACTION_TALK);
	}
	/** 空闲动画 */
	public void playIdle () {
		anim.SetInteger ("Action", ACTION_IDLE);
	}
	/** 攻击动画 */
	public void playAttack () {
		anim.SetInteger ("Action", ACTION_ATTACK);
	}
}
