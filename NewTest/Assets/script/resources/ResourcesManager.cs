using UnityEngine;
using System.Collections;
using System.Xml;
using System.IO;
using System.Collections.Generic;

public class ResourcesManager:MonoBase
{
	
	
	public const string MISSIONCHOOSE_PATH = "texture/missionChoose/";
	public const string CARDIMAGEPATH = "texture/card/";
	public const string ICONIMAGEPATH = "texture/icon/icon_";
	public const string MAINCARD_ICONIMAGEPATH = "texture/roleIcon/roleIcon_";
	public const string CHAPTERDESCIMAGEPATH = "texture/chapterImage/";
	public const string CHAPTERICONPATH = "texture/chapterIcon/";
	/** 女神头像(半圆) */
	public const string NVSHENHEADPATH = "texture/beastHead/";
	/** 女神头像(小) */
	public const string GODDESS_HEAD2="texture/beastHead2/";
    public const string GODDESS_EMTY = "texture/constellation/";//女神空的时候显示的
	/** 女神头像(半身) */
	public const string GODDESS_HEAD="texture/beastHead3/";
	public const string SKILLIMAGEPATH = "texture/skill_icon/";
	public const string BUFFIMAGEPATH = "texture/buff_icon/";
	public const string BACKGROUNDPATH = "texture/backGround/";
	public const string GODDESS_BG_PATH = "texture/GoddessAstrolabe/";
	public const string TEXTURE_TEAM_FORMATION_PATH = "texture/formation/";
	public const string STARSOUL_ICONPREFAB_PATH = "starSoul/iconPrefab/starIcon_";
	public const string SHOP_LIST="texture/shopList/shoplist_";
    public const string OTHER_TEXTURE = "texture/other/";
    public const string SIGNIN_TEXTURE = "texture/signInTexture/";
	public const string SHOP_CHUANGLIAN = "UI/shopWindow/texture/red_tap";// shop窗帘//
	public   bool allowLoadFromRes = true;
	static	ResourcesManager _instance;
	//固定资源
	public Dictionary<string ,ResourcesData> baseDataList;
	public Dictionary<string ,ResourcesData> effectDataList;
	
	//用户行为控制
	public Dictionary<string ,ResourcesData> uiDataList;//包含UIeffect
	//动态资源
	public Dictionary<string ,ResourcesData> textureDataList;
	public Dictionary<string ,ResourcesData> otherDataList;
	public Dictionary<string ,ResourcesData> battleEffectDataList;
	public Dictionary<string ,Dictionary<string ,ResourcesData>> allDataList;
	public float AllTotalSize;
	public float baseTotalSize;
	public float uiTotalSize;
	public float effectTotalSize;
	public float  textureTotalSize;
	public float  otherTotalSize;
	public float  battleTotalSize;
	TextureManager singleTextureLoader;
	public float cacheProgress;
	
	//通过路径获得指定资源
	public ResourcesData getResource (string path)
	{
		
		foreach (Dictionary<string ,ResourcesData> each in allDataList.Values) {
			if (each.ContainsKey (path)) {
				
				return each [path];
			}
			
		}
		return null;
	}
	
	public int getResourcesLength ()
	{
		return  baseDataList.Count;
	}
	
	ResourcesData getScreenResource (string path)
	{
		if (ScreenManager.Instance == null) 
			return null;
		if (ResourcesManager.Instance.otherDataList == null)
			return null;
		if (ResourcesManager.Instance.otherDataList.ContainsKey (path))
			return ResourcesManager.Instance.otherDataList [path];
		else
			return null;
		
	}
	public void  LoadAssetBundleTexture (string path, Object updateItem, CallBack<Object> callback)
	{
		if (singleTextureLoader == null)
			singleTextureLoader = gameObject.AddComponent<TextureManager> ();
		
		singleTextureLoader.addTexture (path, updateItem, callback);
	}
	
	public void  LoadAssetBundleTexture (string path, Object updateItem)
	{
		if (singleTextureLoader == null)
			singleTextureLoader = gameObject.AddComponent<TextureManager> ();
		
		singleTextureLoader.addTexture (path, updateItem, null);
	}
	public void  LoadAssetBundleTexture (string path,Card obj, Object updateItem)
	{
		if(obj.getQualityId()>1){
			    ResourcesManager.Instance.LoadAssetBundleTexture (path + obj.getImageID ()+"c", updateItem);
		}else{
            if (CommandConfigManager.Instance.getNvShenClothType() == 0)
			    ResourcesManager.Instance.LoadAssetBundleTexture (path + obj.getImageID ()+"c", updateItem);
            else ResourcesManager.Instance.LoadAssetBundleTexture(path + obj.getImageID(), updateItem);
		}
	}
	//读取一系列AssetBundle镜像,这会在内存划分一块区域存放;
	IEnumerator  LoadAssetBundleImage (List<string> paths, CallBack<List<ResourcesData>> Callback, string key)
	{

		WWW loader = null;
		bool canCall = false;//判断是否最后一个
		List<ResourcesData> _list = new List<ResourcesData> ();
		
		for (int i=0; i<paths.Count; i++) {
			
			//如果是最后一个
			if (i == paths.Count - 1) {
				canCall = true;
			}
			
			//缓存进度
			cacheProgress = ((float)i + 1f) / (float)paths.Count - 0.1f;
			if (GameManager.Instance.guide != null) {
				GameManager.Instance.guide.progress.text = ((int)((i) / (float)paths.Count * 100)).ToString () + "%";
			}
			
			string each = paths [i];
			if (string.IsNullOrEmpty (each))
				continue;
			string _path = PathKit.GetURLPath (each, true, true);
			
			ResourcesData _tmp = new ResourcesData ();	
			if (File.Exists (PathKit.GetURLPath (each, false, true)) && GameManager.Instance. ignoreUpdate==false) {
				_tmp.ResType = ResourcesData_type.AssetBundleRes;
			} else {
				
				//外部更新文件夹没有的话,尝试读取内部资源
				if (!GameManager.Instance.allowLoadFromRes) {
		
					_path = PathKit.GetStreamingAssetsPath (each);

				} else {
					_path = "";
				}
				
			}
			if (string.IsNullOrEmpty (_path) == false) {
				loader = new WWW (_path);
				yield return loader;
				if (loader.assetBundle == null) {
//					Debug.LogError (_path + "   downLoad assetBundle is null ");
					
				} else {
					
					_tmp.ResourcesBundle = loader.assetBundle;
					_tmp.ResourcesPath = _path;
					_tmp.ResourcesName = each;		
					_tmp.size = loader.bytes.LongLength;
					loader.Dispose ();
					
					if (allDataList.ContainsKey (key))
						allDataList [key].Add (_tmp.ResourcesName, _tmp);

					changeSize (key, _tmp.size);
					_list.Add (_tmp);
					
				}
			} else {
//				Debug.LogError (each + "   is empty  path");
			}
			
			if (Callback != null && canCall == true) {
				//缓冲完成后回调.
				cacheProgress = 1;
				Callback (_list);		
				Callback = null;
			}
		}
	}
	
	public void cacheData (string path, CallBack<List<ResourcesData>> Callback, string  key)
	{
		string[] _tmp = new string[]{
			path,
		};
		
		cacheData (_tmp, Callback, key);
		
	}
	
	public void cacheData (string[] paths, CallBack<List<ResourcesData>> Callback, string  key)
	{
		//	Debug.LogWarning("cacheData !!!!!!!!!!!===================  "+key);
		
		List<string> _noCacheItemList = new List<string> ();//需要缓存的资源队列
		List<ResourcesData> _CacheItemList = new List<ResourcesData> ();//已经缓存的东西
		
		foreach (string  str in paths) {
			bool CacheThis = true;
			
			foreach (Dictionary<string ,ResourcesData> each in allDataList.Values) {
				//这里必须遍历所有缓存池,因为同一个资源可能放在不同的池,比如坐骑,副本中放base池,窗口加载放other池
				if (each.ContainsKey (str)) {
					_CacheItemList.Add (each [str]);
					CacheThis = false;
					//有对应资源,不用缓存了
					break;
				} 
				
			}

			//找完了都没，那就新加
			if (CacheThis)
				_noCacheItemList.Add (str);
		}
		
		if (_noCacheItemList.Count == 0) {
			ResourcesManager.Instance.cacheProgress = 1;
			Callback (_CacheItemList);
		} else
			//	Debug.LogWarning("StartCoroutine LoadAssetBundleImage================== " + key);
			StartCoroutine (LoadAssetBundleImage (_noCacheItemList, Callback, key));
		
	}
	
	public void moveAssetBundle ()
	{
		
		TextAsset ta = Resources.Load ("MoveList") as TextAsset;
		string[] _list = ta.text.Split (new string[]{"\r\n"}, System.StringSplitOptions.RemoveEmptyEntries);
		
		
		string rootDir = PathKit.GetURLPath ("", false, false);
		
		if (!Directory.Exists (rootDir)) {
			MonoBase.print (rootDir + " not exists!");
			if (Application.platform == RuntimePlatform.WindowsPlayer) {
				Directory.CreateDirectory (rootDir);
			} else
				return;
		}
		
		
		//存在不再重复拷贝
		if (File.Exists (PathKit.GetURLPath ("view/roleView", false, true))) {
			GameManager.Instance.PrepareResourcesOK ();
			return;
		}
		
		//如果存在就清空先
		string[] _dirList = Directory.GetDirectories (rootDir);
		if (_dirList != null)
			foreach (string each in _dirList)
				Directory.Delete (each, true);
		
		//Directory.CreateDirectory (rootDir+"/Config");
		GameManager.Instance.StartCoroutine (beginMoveAssetBundle (_list));
		
	}
	
	IEnumerator beginMoveAssetBundle (string[] _list)
	{
		WWW loader;
		for (int i=0; i<_list.Length; i++) {
			string each = _list [i];
			loader = new WWW (PathKit.GetStreamingAssetsPath (each));
			yield return loader;
			
			PathKit.CreateDirIfNotExists (PathKit.GetURLPath (each, false, true));
			FileStream fs = File.OpenWrite (PathKit.GetURLPath (each, false, true));
			
			//	new FileStream(PathKit.GetURLPath(each,false),FileMode.OpenOrCreate ,FileAccess.ReadWrite);
			fs.Write (loader.bytes, 0, loader.bytes.Length);
			fs.Close ();
			fs.Dispose ();

//			MonoBase.print (PathKit.GetURLPath (each, false, true) + "     write ok!!!");
			
			if (i == _list.Length - 1)
				GameManager.Instance.PrepareResourcesOK ();
			
			if (GameManager.Instance.guide != null)
				GameManager.Instance.guide.progress.text = (int)((float)(i + 1) / (float)_list.Length * 100) + "%";
			
		}
	}
	
	//清理内存
	public void clean ()
	{
		Resources.UnloadUnusedAssets ();
		System.GC.Collect ();
	}

	//卸载窗口资源块
	public void UnloadAssetUIBundleBlock (string path)
	{
		foreach (Dictionary<string ,ResourcesData> each in allDataList.Values) {
			if (each.ContainsKey (path)) {
				float size = each [path].size;
				each [path].ResourcesBundle.Unload (true);
				each.Remove (path);
				foreach (string  each2 in allDataList.Keys)
				if (allDataList [each2] == each) {
					changeSize (each2, -size);
					break;
				}
			}
		}
	}
	
	//卸载指定镜像
	public void UnloadAssetBundleBlock (string path, bool forceRemove)
	{
		foreach (Dictionary<string ,ResourcesData> each in allDataList.Values) {
			if (each.ContainsKey (path)) {
				float size = each [path].size;
				each [path].ResourcesBundle.Unload (forceRemove);
				each.Remove (path);
				foreach (string  each2 in allDataList.Keys)
					if (allDataList [each2] == each) {
						changeSize (each2, -size);
						break;
					}
			}
			
		}
	}
	
	//卸载指定镜像
	public void UnloadAssetBundleBlock (ResourcesData path, bool forceRemove, string key)
	{
		if (allDataList.ContainsKey (key)) {
			Dictionary<string,ResourcesData> dic = allDataList [key];
			if (dic.ContainsKey (path.ResourcesName)) {
				float size = dic [path.ResourcesName].size;
				dic [path.ResourcesName].ResourcesBundle.Unload (forceRemove);
				dic.Remove (path.ResourcesName);
				changeSize (key, -size);		
			}
			clean ();	
		}	
	}
	
	//卸载指定镜像
	public void UnloadAssetBundleBlock (ResourcesData[] paths, bool forceRemove, string key)
	{
		if (allDataList.ContainsKey (key)) {
			Dictionary<string,ResourcesData> dic = allDataList [key];
			foreach (ResourcesData path in paths) {		
				if (dic.ContainsKey (path.ResourcesName)) {
					float size = dic [path.ResourcesName].size;
					dic [path.ResourcesName].ResourcesBundle.Unload (forceRemove);
					dic.Remove (path.ResourcesName);
					changeSize (key, -size);				
				}		
			}	
			clean ();	
		}
	}
	
	//卸载镜像块
	public void UnloadAssetBundleList (string key, bool forceRemove)
	{
		if (allDataList.ContainsKey (key)) {	
			Dictionary<string ,ResourcesData> list = allDataList [key];
			if (key == "texture" && singleTextureLoader != null) {
				singleTextureLoader.clear ();
			}
			foreach (ResourcesData each in allDataList [key].Values) {
				float size = each.size;
				each.ResourcesBundle.Unload (forceRemove);
				changeSize (key, -size);
			}
			allDataList [key].Clear ();
			clean ();	
		}
	}
	
	void changeSize (string key, float size)
	{
		AllTotalSize += size;
		switch (key) {
		case "base":
			baseTotalSize += size;
			break;
		case "ui":
			uiTotalSize += size;
			break;
		case "effect":
			effectTotalSize += size;
			break;
		case "other":
			otherTotalSize += size;
			break;
		case "battleEffect":
			battleTotalSize += size;
			break;
		case "texture":
			textureTotalSize += size;
			break;
		}
	}

	public void releaseTextureResources ()
	{
		if (singleTextureLoader != null) {
			singleTextureLoader.clear ();
		}
		if (ResourcesManager.Instance.textureDataList != null) {
			foreach (ResourcesData each in ResourcesManager.Instance.textureDataList.Values) {
				each.ResourcesBundle.Unload (false);
				
			}
			ResourcesManager.Instance.textureDataList.Clear ();
			textureTotalSize = 0;
		}

		TextureManager.cacheDictionary.Clear ();
		TextureManager.nodeList.Clear ();
		Resources.UnloadUnusedAssets ();
	}
	
	public void releaseScreenResources ()
	{
		releaseTextureResources ();
		// otherDataList
		if (ResourcesManager.Instance.otherDataList != null) {
			foreach (ResourcesData each in ResourcesManager.Instance.otherDataList.Values) {
				//Debug.LogWarning ("releaseScreenResources " + each.ResourcesName);
				each.ResourcesBundle.Unload (false);
				
			}
			ResourcesManager.Instance.otherDataList.Clear ();
			otherTotalSize = 0;
		}
		
		
		//battleEffect  
		if (ResourcesManager.Instance.battleEffectDataList != null) {
			foreach (ResourcesData each in ResourcesManager.Instance.battleEffectDataList.Values) {
				//	Debug.LogWarning ("releaseScreenResources " + each.ResourcesName);
				each.ResourcesBundle.Unload (false);
				
			}
			ResourcesManager.Instance.battleEffectDataList.Clear ();
			battleTotalSize = 0;
		}
		
		//gc
		clean ();
	}
	
	public void releaseResources (List<ResourcesData> _list)
	{
		
		if (_list == null)
			return;
		foreach (ResourcesData each in _list) {
			//Debug.LogWarning ("releaseScreenResources " + each.ResourcesName);
			each.ResourcesBundle.Unload (true);
			
		}
		_list.Clear ();
		clean ();
	}
	
	public static ResourcesManager Instance {
		get {
			if (_instance == null) {
				GameObject tmp = GameObject.Find ("GameManager");

				if (tmp == null)
					return null;

				_instance = tmp.AddComponent<ResourcesManager> ();
				_instance.allowLoadFromRes = GameManager.Instance.allowLoadFromRes;
			}
			return _instance;
		}
		
		set {
			_instance = value;
		}
	}
	
	public ResourcesManager ()
	{
	}
	
	public void init ()
	{
		Debug.Log ("res init");
		baseDataList = new Dictionary<string,ResourcesData> ();
		textureDataList = new Dictionary<string,ResourcesData> ();
		effectDataList = new Dictionary<string,ResourcesData> ();
		uiDataList = new Dictionary<string,ResourcesData> ();
		otherDataList = new Dictionary<string,ResourcesData> ();
		battleEffectDataList = new Dictionary<string,ResourcesData> ();
		
		allDataList = new Dictionary<string, Dictionary<string, ResourcesData>> ();
		
		allDataList.Add ("base", baseDataList);
		allDataList.Add ("texture", textureDataList);
		allDataList.Add ("effect", effectDataList);
		
		allDataList.Add ("ui", uiDataList);
		allDataList.Add ("other", otherDataList);
		allDataList.Add ("battleEffect", battleEffectDataList);
	}
	
	public  XmlDocument LoadXmlFileFormString (string Text)
	{
		
		if (Text == "")
			return null;
		
		XmlDocument myDoc;
		myDoc = new XmlDocument ();
		
		myDoc.LoadXml (Text);
		
		return myDoc;
	}

	public  XmlDocument LoadXmlFileFormResources (string fileName)
	{
		
		TextAsset xmlFile;
		if (fileName == "")
			return null;
		xmlFile = (TextAsset)Resources.Load (fileName, typeof(TextAsset));
		XmlDocument myDoc;
		myDoc = new XmlDocument ();
		
		myDoc.LoadXml (xmlFile.text);
		
		return myDoc;
		
		
	}
	
	public  XmlDocument LoadXmlFileFormPath (string  file)
	{
		XmlDocument myDoc;
		myDoc = new XmlDocument ();
		myDoc.Load (file);
		return myDoc;
		
		
	}
	
}
