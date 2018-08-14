using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 淘汰赛(决赛)地图3D
/// </summary>
public class ArenaFinalMap : MonoBase {
    public Camera camera;
    public GameObject root;
    public GameObject roleRoot;
    public GameObject lineRoot;
    public GameObject pointRoot;
    public GameObject linePrefab;
    public GameObject linePrefab2;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    public void addPoint(GameObject pos,bool end,CallBack<GameObject> callBack)
    {
		passObj _obj=Create3Dobj("mission/point_start");
		_obj.obj.transform.localScale = Vector3.one;
		_obj.obj.transform.parent = pointRoot.transform;

//		EffectManager.Instance.CreateEffectCtrlByCache(pointRoot.transform,"mission/point",(_obj,ctrl)=>{
//			_obj.obj.transform.localScale = Vector3.one;
//			_obj.obj.transform.parent = pointRoot.transform;
//			if (!end)
//			{
//				_obj.obj.transform.FindChild("ice_point").gameObject.SetActive(false);
//				_obj.obj.transform.FindChild("point_model").localScale = new Vector3(2, 2, 2);
//				_obj.obj.transform.FindChild("point_model").gameObject.SetActive(true);
//			}
			Vector3 v = UiManager.Instance.gameCamera.WorldToViewportPoint(pos.transform.position);
			v = camera.ViewportToWorldPoint(new Vector3(v.x,v.y,20));
			_obj.obj.transform.position = v;
			if(callBack!=null) {
				callBack(_obj.obj);
			}
    }

    public GameObject addLine(GameObject role, Vector3 src,Vector3 dst,bool small,bool active)
    {
        src = UiManager.Instance.gameCamera.WorldToViewportPoint(src);
        src = camera.ViewportToWorldPoint(new Vector3(src.x,src.y,20));
        
        dst = UiManager.Instance.gameCamera.WorldToViewportPoint(dst);
        dst = camera.ViewportToWorldPoint(new Vector3(dst.x,dst.y,20));

        GameObject obj = NGUITools.AddChild(role, active? linePrefab : linePrefab2);
        obj.transform.localScale = Vector3.one;
        obj.transform.position = src;
        obj.transform.localScale = new Vector3 (small ? 0.01f : 0.02f, 0.1f, Vector3.Distance (src, dst)* 1.02f);
        obj.transform.LookAt (dst, Vector3.up);
        return obj;

    }

    public GameObject addRole(int icon,GameObject pos)
    {
        passObj _obj = Create3Dobj (UserManager.Instance.getModelPath(icon)); 
        
        if (_obj.obj == null) {
            Debug.LogError ("role is null!!!");
			return null;
        } 
        _obj.obj.transform.parent = roleRoot.transform;
        _obj.obj.transform.localScale = Vector3.one;
        _obj.obj.transform.GetChild(0).localScale = Vector3.one;

        Vector3 v = UiManager.Instance.gameCamera.WorldToViewportPoint(pos.transform.position + new Vector3(0,0.04f,0));
        v = camera.ViewportToWorldPoint(new Vector3(v.x,v.y,10));
        _obj.obj.transform.position = v;
        return _obj.obj;
    }


    public void clear()
    {
        Utils.DestoryChilds(lineRoot);
        Utils.DestoryChilds(roleRoot);
        Utils.DestoryChilds(pointRoot);
    }
}
