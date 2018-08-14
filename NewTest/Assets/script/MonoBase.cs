using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/** 
  * 基类 
  * @author 李程
  * */
public	class passObj
{
	public	ResourcesData data;
	public	GameObject obj;
}

public class MonoBase : MonoBehaviour
{
#if UNITY_EDITOR
	//编辑器模式下使用MonoBehaviour 默认。
	//带flag则使用NGUI DEBUG 打印输出到屏幕.
	public  static void	print(object message,bool flag)
	{

		
		if(Debug.isDebugBuild==true){
		
			NGUIDebug.Log(message.ToString());
			
		}
	}
#else
	//log only
	
	public  static void	print (object message)
	{
		
//	if(message==null) Android_SDK_Manager.ErrorCode+= " null     "; else 	Android_SDK_Manager.ErrorCode+=message.ToString()+"      ";
		
		if (Debug.isDebugBuild == true) {
			
			if (message == null) {
				Debug.Log ("null");
				return;
			} else
				Debug.Log (message.ToString ());
			
		}
		

		
		//we do nothing when debug=false;;
		
		
	}
	
	
	// DebugBuild模式下输出到手机屏幕
	
	public  static void	print (object message, bool ins)
	{
//	if(message==null) Android_SDK_Manager.ErrorCode+= " null     "; else 	Android_SDK_Manager.ErrorCode+=message.ToString()+"      ";
		
		if (Debug.isDebugBuild == true) {
			if (message == null) {
				NGUIDebug.Log ("null");
				return;
			} else
				
				NGUIDebug.Log (message.ToString ());
			
		}
	}
	
#endif
	public static passObj CreateNGUIObj (string name)
	{
		passObj Obj = Create3Dobj (name);
 
		if (Obj.obj != null) {
			Obj.obj.name = name;
			//	lastObj.layer = 12;
			if (UiManager.Instance.UIScaleRoot == null)
				return Obj;
				
			if (Obj.obj.transform.parent == null) {
				Obj.obj.transform.parent = UiManager.Instance.UIScaleRoot.transform;
				Obj.obj.transform.localScale = Vector3.one;
			}
		}
		
		return Obj;
	}

	public static float sin ()
	{
		return 0.5f * Mathf.Sin (Time.time * 4) + 0.5f;
	}

	public static passObj Create3Dobj (string path)
	{
		return 	Create3Dobj (path, null);
	}

	//指定读取path包内的name文件
	public static passObj Create3Dobj (string path, string fileName)
	{

		passObj lastObj = new passObj ();
		UnityEngine.Object _tmp = null;

		//开发时直接读取Resource资源
		if (ResourcesManager.Instance.allowLoadFromRes) {

			if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor) {
				UnityEngine.Object obj = Resources.Load (path);
				if (obj == null) {
					string _name = PathKit.getFileName (path);

					if (string.IsNullOrEmpty (fileName))
						obj = Resources.Load (path + "/" + _name);
					else {
						print (path + "/" + fileName);
						obj = Resources.Load (path + "/" + fileName);
					}
				}
				if (obj != null) {
					lastObj.obj = MonoBehaviour.Instantiate (obj) as GameObject;
					return lastObj;
				}
			}
		
		}

		//从资源库里获取
		ResourcesData res = ResourcesManager.Instance.getResource (path);
		if (res == null) {
			if (ResourcesManager.Instance.allowLoadFromRes) {
				_tmp = Resources.Load (path);
				//如果路径无，再次尝试
				if (_tmp == null) {
					string _name = PathKit.getFileName (path);
					_tmp = Resources.Load (path + "/" + _name);
				}
			}
		} else {
			lastObj.data = res;
			if (string.IsNullOrEmpty (fileName))
				_tmp = res.ResourcesBundle.mainAsset;
			else {
				_tmp = res.ResourcesBundle.Load (fileName);
			}
//			lastObj.data.MemoryData=_tmp;
		}
		if (_tmp == null) 
			return lastObj;
		lastObj.obj = MonoBehaviour.Instantiate (_tmp) as GameObject;
		return lastObj;
		
	}

	public string Language (string _name, params object[] _parameters)
	{
		return LanguageConfigManager.Instance.parseLanguage (_name, _parameters);
	}
 
	
}
