using UnityEngine;
using System.Collections;

public class WorldBossCtrl : MonoBase {

	public Camera camera;
	public Vector3 spawnPos;
	public GameObject root;

	public void Init(){
		passObj _obj = Create3Dobj (UserManager.Instance.self.getModelPath ()); 
		if (_obj.obj == null) {
			Debug.LogError ("role is null!!!");
			return;
		} 
		_obj.obj.transform.parent = root.transform;
		_obj.obj.transform.localScale = Vector3.one;
		_obj.obj.transform.GetChild(0).localScale = Vector3.one;
		_obj.obj.name = UserManager.Instance.self.guildName;
	}

}
