using UnityEngine;
using System.Collections;

public class ArenaRoles : MonoBase {
    public Camera camera;
    public GameObject root;

    [HideInInspector]public bool destroyed;

    GameObject[] locations;
    int[] headIconId;
    public FuBenCardCtrl[] models;
    float animDelay;

   
	public void clear()
	{

	}
    public void init(int[] headIconId,GameObject[] locations)
    {
        this.headIconId = headIconId;
        this.locations = locations;
		UIUtils.M_removeAllChildren(root.transform);

        models = new FuBenCardCtrl[headIconId.Length];
        for(int i = 0; i < headIconId.Length; i++)
        {
            int head = headIconId[i];
            bool dead = head >= 10000;
            if(dead)
                head /= 10000;
            passObj _obj = Create3Dobj (UserManager.Instance.getModelPath(head)); 
            
            if (_obj.obj == null) {
                Debug.LogError ("role is null!!!");
				return;
            } 
            _obj.obj.transform.parent = root.transform;
            _obj.obj.transform.localScale = Vector3.one;
            _obj.obj.transform.GetChild(0).localScale = Vector3.one;
            _obj.obj.name = i.ToString();


            Vector3 v = UiManager.Instance.gameCamera.WorldToScreenPoint(locations[i].transform.position);
            v.z = 10;
            v = camera.ScreenToWorldPoint(v);
            _obj.obj.transform.position = v + new Vector3(0,0.08f,0);

            models[i] = _obj.obj.transform.GetChild(0).GetComponent<FuBenCardCtrl>();


//            _obj = Create3Dobj ("mission/point");
//            _obj.obj.transform.localScale = Vector3.one;
//            _obj.obj.transform.parent = root.transform;
//            _obj.obj.transform.FindChild("ice_point").gameObject.SetActive(false);
//            _obj.obj.transform.FindChild("point_model").localScale = new Vector3(2,2,2);
//            _obj.obj.transform.FindChild("point_model").gameObject.SetActive(true);
//            v = UiManager.Instance.gameCamera.WorldToViewportPoint(locations[i].transform.position);
//            v = camera.ViewportToWorldPoint(new Vector3(v.x,v.y,20));
//            _obj.obj.transform.position = v;

            
            if(dead)
            {
                models[i].playFail();
            }
        }
    }

    void Update()
    {

    }

    void OnDestroy()
    {
        destroyed = true;
    }
}
