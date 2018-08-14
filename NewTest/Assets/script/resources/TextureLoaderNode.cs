using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TextureLoaderNode
{
	public	string  path;
	public long createTime;
	public	ResourcesData data;
	public Texture2D webImage;
	public bool isWebNode;//是否从网络下载

	public bool resReady=false;
	//每个tex节点包含了所有需要使用这个贴图的容器,和对应的加载完成callback;
	public Dictionary<Object,CallBack<Object>> callbackDic;

	public void addCallBack (Object KeyObj, CallBack<Object> cb)
	{
		if (callbackDic == null)
			callbackDic = new Dictionary<Object, CallBack<Object>> ();



		//如果需求这个贴图的容器已经在list中
		if (callbackDic.ContainsKey (KeyObj)) {
			callbackDic[KeyObj]=cb;//更新callback
		} else {
			callbackDic.Add (KeyObj, cb);
		}
			 
	}

	public	void pushTexture (Object obj)
	{
		if (obj == null)
			return;
		if (data != null || webImage != null) {

			//有网络资源用网络资源
			Texture2D tmpTex = null;
			if (webImage != null) {
				tmpTex = webImage;
			} else if (data != null && data.ResourcesBundle != null) {
//				if (TextureManager.instance.debugMode)
//					Debug.Log (data.ResourcesPath);

				if (data.ResourcesBundle.mainAsset is Texture2D)
					tmpTex = (Texture2D)data.ResourcesBundle.mainAsset;
			}

			if (typeof(UITexture) == obj.GetType ()) {
				if ((obj as UITexture).mainTexture == null || (obj as UITexture).mainTexture.GetHashCode () != tmpTex.GetHashCode ()) {
					(obj as UITexture).mainTexture = tmpTex;
				}
				(obj as UITexture).gameObject.SetActive (true);
				callbackAfterLoad (obj, obj);
			} else if (typeof(GameObject) == obj.GetType ()) {
				GameObject gameObj=obj as GameObject;
				if(gameObj.renderer!=null && gameObj.renderer.material!=null) {
					gameObj.renderer.material.mainTexture = tmpTex;
				}
				callbackAfterLoad (obj, obj);
			} else if (typeof(Transform) == obj.GetType ()) {	
				GameObject tmp = GameObject.Instantiate (data.ResourcesBundle.mainAsset) as GameObject;
				tmp.transform.parent = obj as Transform;
				tmp.transform.localScale = Vector3.one;
				tmp.transform.localPosition = Vector3.zero;
				callbackAfterLoad (obj, tmp);
			}	
		} else if (ResourcesManager.Instance.allowLoadFromRes) {

			Texture2D tmpTex = null;
			if (webImage != null)
				tmpTex = webImage;
			else
				tmpTex = Resources.Load (path) as Texture2D;	
			//直接资源读取咯
			if (typeof(UITexture) == obj.GetType ()) {
				if ((obj as UITexture).mainTexture != tmpTex) {
					(obj as UITexture).mainTexture = tmpTex;	
				}
				(obj as UITexture).gameObject.SetActive (true);
				callbackAfterLoad (obj, obj);
			} else if (typeof(GameObject) == obj.GetType ()) {
				GameObject gameObj=obj as GameObject;
				if(gameObj.renderer!=null && gameObj.renderer.material!=null) {
					gameObj.renderer.material.mainTexture = tmpTex;
				}
				callbackAfterLoad (obj, obj);
			} else if (typeof(Transform) == obj.GetType ()) {	


				Object obj2=Resources.Load(path);
				
				//如果路径无，再次尝试
				if (obj2 == null) {
					string _name = PathKit.getFileName (path);
					obj2 = Resources.Load (path + "/" + _name) as GameObject;
				}

				GameObject tmp = GameObject.Instantiate (obj2) as GameObject;
				tmp.transform.parent = obj as Transform;
				tmp.transform.localScale = Vector3.one;
				tmp.transform.localPosition = Vector3.zero;
				callbackAfterLoad (obj, tmp);
			}	
		}
	}

	public void unLoadAssetBundle ()
	{

		ResourcesManager.Instance.UnloadAssetBundleBlock (path, false);


	}

	public	void resourcesLoadTexture (Object obj)
	{
 
		if (obj == null)
			return;
 
		if (typeof(UITexture) == obj.GetType ()) {
			if ((obj as UITexture).mainTexture != (Texture2D)Resources.Load (path)) {
				(obj as UITexture).mainTexture = (Texture2D)Resources.Load (path);				
			}
			(obj as UITexture).gameObject.SetActive (true);
			callbackAfterLoad (obj, obj);
		} else if (typeof(GameObject) == obj.GetType ()) {
			GameObject gameObj=obj as GameObject;
			if(gameObj.renderer!=null && gameObj.renderer.material!=null) {
				gameObj.renderer.material.mainTexture = (Texture2D)Resources.Load (path);
			}
			callbackAfterLoad (obj, obj);
		} else if (typeof(Transform) == obj.GetType ()) {
			Object obj2=Resources.Load(path);
			
			//如果路径无，再次尝试
			if (obj2 == null) {
				string _name = PathKit.getFileName (path);
				obj2 = Resources.Load (path + "/" + _name) as GameObject;
			}


			GameObject tmp = GameObject.Instantiate (obj2 ) as GameObject;
			tmp.transform.parent = obj as Transform;
			tmp.transform.localScale = Vector3.one;
			tmp.transform.localPosition = Vector3.zero;
			callbackAfterLoad (obj, tmp);
		}
	}

	//读取到图片才会回调
	void callbackAfterLoad (Object obj, Object instanceObj)
	{
		if (callbackDic.ContainsKey (obj)) {
			if (callbackDic [obj] != null) {
				callbackDic [obj] (instanceObj);
			}
		}

	}

	public	void downloadFinish (Texture2D tex)
	{
		resReady=true;
		webImage = tex;
		foreach (Object each in callbackDic.Keys) {
			pushTexture (each);
		}
	
		TextureManager.instance.	clear ();
	}

	public	void cacheSingeFinish (List<ResourcesData> _list)
	{
		resReady=true;

		if (_list == null || _list.Count < 1) {
			//找不到res里直接读取
			if (ResourcesManager.Instance.allowLoadFromRes) {
				foreach (Object each in callbackDic.Keys) {

					resourcesLoadTexture (each);

				}
			}
			return;
		}


		if (_list [0].ResourcesBundle == null)
			return;

		data = _list [0];

		foreach (Object each in callbackDic.Keys) {
			pushTexture (each);
		}
	callbackDic.Clear();
	TextureManager.instance.clear ();

	}
	





}