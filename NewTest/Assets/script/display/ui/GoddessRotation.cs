using UnityEngine;
using System.Collections;

public class GoddessRotation : MonoBehaviour {

	public bool isRotate = true;
	void Update(){
		if(isRotate)
		transform.Rotate(Vector3.forward, Time.deltaTime*8f, Space.World);
	}
}
