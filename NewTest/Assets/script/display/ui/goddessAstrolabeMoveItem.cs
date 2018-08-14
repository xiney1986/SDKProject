using UnityEngine;
using System.Collections;

public class goddessAstrolabeMoveItem : MonoBehaviour {

	public GoddessAstrolabeWindow fawin;

	private Rigidbody obj;
	private Vector3 Torque;
	private bool isOnDrag = false;

	void OnDrag (Vector2 delta)
	{
		fawin.isCanMove = true;

		if(fawin.lookType == 1) {
			Torque = new Vector3(-delta.x * 1f,-delta.y * 1f,0);
			fawin.gaCamera.transform.position -= new Vector3(delta.x * 0.004f,delta.y * 0.004f,0);
		}
		else {
			Torque = new Vector3(-delta.x * 1.5f,-delta.y * 1.5f,0);
			fawin.gaCamera.transform.position -= new Vector3(delta.x * 0.007f,delta.y * 0.007f,0);
		}
	}

	void OnPress (bool isDown)
	{
		isOnDrag = isDown;
	}
	
	void Start()
	{
		obj = fawin.gaCamera.GetComponent<Rigidbody>();
	}
	
	void FixedUpdate() {
		if(isOnDrag && obj!=null && fawin.isCanMove) {
			obj.AddForce (Torque);
		}
	}
	
	public bool getOnPress()
	{
		return isOnDrag;
	}

}
