using UnityEngine;
using System.Collections;

public class WorldBossIntegrate : MonoBase {
	public Transform[] spawnArea;//角色模型出生区域

	public Transform[]  paths;//跑动路径

	public GameObject root;//角色模型根节点

	public Camera camera;

	[HideInInspector]
	public  bool destroyed = false;
	void Start(){
		passObj _obj = Create3Dobj (UserManager.Instance.self.getModelPath ()); 
		_obj.obj.transform.parent = root.transform;
		_obj.obj.transform.localScale = Vector3.one;
		_obj.obj.transform.GetChild(0).localScale = Vector3.one;
		float x = Random.Range(spawnArea[0].position.x,spawnArea[1].position.x);
		float z = Random.Range(spawnArea[0].position.z,spawnArea[1].position.z);
		_obj.obj.transform.position = new Vector3(x,0f,z);

		camera.transform.position = new Vector3(_obj.obj.transform.position.x-2f,camera.transform.position.y,camera.transform.position.z);
		camera.transform.LookAt(_obj.obj.transform.position);
	}

	void OnDestroy (){
		destroyed = true;
	}



}
