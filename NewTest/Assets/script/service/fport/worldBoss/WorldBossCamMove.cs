using UnityEngine;
using System.Collections;

public class WorldBossCamMove : MonoBehaviour {

	public WorldBossWindow fatherWindow;

	private Vector3 Torque;

	private bool isDrag = false;

	private Rigidbody rigidObj;

	//拖动事件

	void OnDrag(Vector2 delta){
		fatherWindow.isMove = true;

		Torque = new Vector3(-delta.x * 1f,0,0);
		fatherWindow.gaCamera.transform.position -= new Vector3(delta.x * 0.004f,0,0);
	}

	//按下事件

	void OnPress(bool isDown){
		isDrag = isDown;
	}

	public bool getOnPress()
	{
		return isDrag;
	}

	void Start(){
		//得到刚体
		rigidObj = fatherWindow.gaCamera.GetComponent<Rigidbody>();

	}

	//物理计算
	void FixedUpdate(){
		if( rigidObj !=null && isDrag  && fatherWindow.isMove )
			rigidObj.AddForce(Torque);
	}
}
