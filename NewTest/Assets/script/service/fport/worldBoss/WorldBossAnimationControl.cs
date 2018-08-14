using UnityEngine;
using System.Collections;

public class WorldBossAnimationControl : MonoBehaviour {

	public float rotationSpeed; //转向速度
	public float moveSpeed;//移动速度
	Animator ani; 
	Vector3 moveEnd; //移动目标点
	int activeId;//动作id

	// Use this for initialization
	void Start () {
		ani = GetComponent<Animator> (); //获取动画对象
		RandomActive ();//初始化动作
		SetMoveEndPoint ();//设置移动点
	}

	// Update is called once per frame
	void Update () {
		switch (activeId) {
		case 1:
			RandomMove ();
			break;
		}
	}

	/// <summary>
	/// 移动与转向
	/// </summary>
	void RandomMove () {
		//开始转向目标点
		//计算目标方向
		Vector3 targetDir = (moveEnd - transform.position).normalized;
		transform.localRotation = Quaternion.Slerp (transform.localRotation, Quaternion.LookRotation (targetDir), rotationSpeed * Time.deltaTime);
		if (Vector3.Distance (transform.position, moveEnd) > 0.3f) {
			float speed = moveSpeed * Time.deltaTime;
			transform.Translate (transform.forward * speed, Space.World);
		}
		else {
			SetMoveEndPoint ();
		}
	}

	/// <summary>
	/// 随机获取一个动作 
	/// </summary>
	void RandomActive () {
		int step = Random.Range (0, 100);
		if (step <= 5) {
			activeId = 0;
		}
		else if (step > 5 && step <= 90) {
			activeId = 1;
		}
		else if (step > 90 && step <= 95) {
			activeId = 2;
		}
		else {
			activeId = 3;
		}
		ani.SetInteger ("Active", activeId);
	}
	/// <summary>
	/// 设置目标点
	/// </summary>
	void SetMoveEndPoint () {
		float x = Random.Range (2.8f, 4.2f);
		float z = Random.Range (-0.8f, 1f);
		moveEnd = new Vector3 (x, 0f, z);
	}
}
