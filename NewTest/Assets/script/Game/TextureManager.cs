using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TextureManager : MonoBase
{
	public static Dictionary<string, TextureLoaderNode> cacheDictionary;
	public static List<TextureLoaderNode> nodeList;
	public static TextureManager instance;
	public bool debugMode;
	
	public const int CACHE_SIZE=80;//缓冲区大小
	public const int MIN_DEADTIME=1000 * 10;//新缓冲出来的数据的保护时间毫秒
	
	void Awake ()
	{
		instance = this;
	}
	
	public void  addTexture (string cacheTex, Object obj, CallBack<Object> callback)
	{
		if (string.IsNullOrEmpty (cacheTex))
			return;
		if (cacheDictionary == null) 
			cacheDictionary = new Dictionary<string, TextureLoaderNode> ();
		if (nodeList == null) 
			nodeList = new List<TextureLoaderNode> ();
		
		if (cacheDictionary.ContainsKey (cacheTex)) {
			//如果有相同资源，下载好的推送，没下好就什么都不做
			
			cacheDictionary [cacheTex].addCallBack (obj, callback);
			
			if (cacheDictionary [cacheTex].resReady) {
				cacheDictionary [cacheTex].pushTexture (obj);
				//后期推送,推一个消一个
				cacheDictionary [cacheTex].callbackDic.Remove (obj);
			}
			
			//推到队列最后
			cacheDictionary [cacheTex].createTime = ServerTimeKit.getMillisTime ();//更新时间
			nodeList.Remove (cacheDictionary [cacheTex]);
			nodeList.Add (cacheDictionary [cacheTex]);
			
			return;
		}
		//走到这里说明队列里没有
		TextureLoaderNode newNode = new TextureLoaderNode ();
		newNode.path = cacheTex;
		newNode.createTime = ServerTimeKit.getMillisTime ();
		newNode.addCallBack (obj, callback);
		cacheDictionary.Add (cacheTex, newNode);
		nodeList.Add (newNode);
		
		//http://img.immomo.com/album/C5/B2/C5B2E711-5A20-6009-1C74-293F61C0779E_S.jpg
		if (cacheTex.Length >= 7 && cacheTex.Substring (0, 7) == "http://") {
			newNode.isWebNode = true;
			//网路下载走这里
			StartCoroutine (loadImageFromWeb (cacheTex, newNode));
		} else {
			newNode.isWebNode = false;
			ResourcesManager.Instance.cacheData (cacheTex, newNode.cacheSingeFinish, "texture");
		}
	}
	
	IEnumerator loadImageFromWeb (string url, TextureLoaderNode node)
	{
		
		Debug.LogWarning ("start downlaod:" + url);
		
		WWW www = new WWW (url);
		
		yield return www;
		
		if (www.texture != null) {
			
			node.downloadFinish (www.texture);
			
			
		}else{
			
			Debug.LogWarning (" download fail  :" );
			Debug.LogWarning (" www bytesDownloaded  :" + www.bytesDownloaded);
			Debug.LogWarning (" www progress  :" + www.progress);
			Debug.LogWarning (" www error  :" + www.error);
		}
		
		www.Dispose ();
	}
	
	public void  clear ()
	{
		int clearNum = 0;
		//大于50个就清理一部分
		if (cacheDictionary.Values.Count > CACHE_SIZE) {
			clearNum = cacheDictionary.Values.Count - (int)(CACHE_SIZE*0.6f);
		} else {
			return;
		}
		
		if (debugMode)
			Debug.LogWarning ("need clear: :" + clearNum);
		
		GameManager.Instance.StartCoroutine(smoothDel());
	}
	IEnumerator smoothDel(){
		for (int t=0; t<nodeList.Count; t++) {
			//找出最早的
			if (checkNode (nodeList [t])) {
				
				if (debugMode)
					Debug.LogWarning (" unLoadAssetBundle: :" + nodeList [t].path);
				
				nodeList [t].callbackDic.Clear ();
				nodeList [t].unLoadAssetBundle ();
				cacheDictionary.Remove (nodeList [t].path);
				nodeList.Remove (nodeList [t]);
				t--;
				
			}
			yield return 0;
		}
		Resources.UnloadUnusedAssets ();
	}
	
	//返回true可删除
	bool checkNode (TextureLoaderNode node)
	{
		//20-秒保护时间内不删除
		if (node.createTime + MIN_DEADTIME <= ServerTimeKit.getMillisTime ()) {
			return false;
		} else if (node.callbackDic.Count == 0) {
			return true;
		} else {
			return false;
		}
	}
	
}
