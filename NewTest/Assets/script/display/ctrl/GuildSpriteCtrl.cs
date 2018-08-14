using UnityEngine;
using System.Collections;

/// <summary>
/// 公会精灵控制器
/// </summary>
public class GuildSpriteCtrl : MonoBase {

	/** 精灵动画控制器 */
	public SpriteAniCtrl animCtrl;
	/** 攻击过的次数 */
	public UILabel attackNum;
	/** 目标点 */
	Vector3 endPoint;
	/** 移动速度 */
	float moveSpeed=3.5f;

	void Awake () {
		animCtrl = transform.GetChild (0).GetComponent<SpriteAniCtrl> ();
	}
	public void init(Vector3 pos,Vector3 endPoint){
		setPos (pos);
		setEndPoint (endPoint);
	}
	public void init(Vector3 pos,Vector3 endPoint,float moveSpeed){
		init (pos,endPoint);
		this.moveSpeed = moveSpeed;
	}
	/// <summary>
	/// 设置位置
	/// </summary>
	public void setPos (Vector3 pos) {
		transform.localPosition = new Vector3 (pos.x, pos.y, pos.z);
	}
	/// <summary>
	/// 设置目标点
	/// </summary>
	public void setEndPoint (Vector3 endPoint) {
		this.endPoint = endPoint;
	}
	/** 移动开始 */
	public void moveStart () {
		animCtrl.playWalk ();
		iTween.MoveTo (gameObject, iTween.Hash ("position", endPoint, "oncomplete", "moveOver", "easetype", "easeOutQuad", "time", moveSpeed,"islocal",true));
	}
	/** 移动结束 */
	public void moveOver () {
		animCtrl.playAttack ();
	}
	public void playIdle () {
		animCtrl.playIdle ();
	}

	public void setAttackNum(int attackNum){
		this.attackNum.text = attackNum.ToString ();
	}

	public void trunLeft(){
		this.transform.Rotate (Vector3.up, 180, Space.Self);
		attackNum.transform.Rotate (Vector3.up, 180, Space.Self);
	}
}
