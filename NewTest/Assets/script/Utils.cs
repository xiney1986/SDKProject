using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;



/// <summary>
/// 工具集合
/// </summary>
public class Utils
{
#if UNITY_EDITOR
	[UnityEditor.MenuItem("Utils/Clear Player Prefs")]
	static void ClearPlayerPrefs()
	{
		PlayerPrefs.DeleteAll();
	}
#endif
	
	public static IEnumerator DelayRun(CallBack callback,float time)
	{
		yield return new WaitForSeconds(time);
		callback();
	}

	public static IEnumerator DelayRunFrame(CallBack callback,int frame)
	{
		yield return frame;
		callback();
	}

    public static IEnumerator DelayRunNextFrame(CallBack callback)
    {
        yield return 1;
        callback();
    }
	
	public static IEnumerator WaitCompleted(CallBack callback,BCallBack isCompleted)
	{
		while(!isCompleted())
		{
			yield return 1;
		}
		callback();
	}

	public static byte[] readFile(string path)
	{
		try
		{
			FileStream fs = File.OpenRead(path);
			byte[] data = new byte[fs.Length];
			fs.Read(data,0,data.Length);
			fs.Close();
			return data;
		}catch(System.Exception ex)
		{
			Debug.LogException(ex);
			return null;
		}
	}

	public static void DestoryChilds(GameObject obj)
	{
		DestoryChilds (obj, null);
	}

	public static void DestoryChilds(GameObject obj,params GameObject[] withOut)
	{
		int count = obj.transform.childCount;
		for (int i = 0; i < count; i++) {

			if(obj.transform.GetChild(i)==null)
				continue;

			GameObject child = obj.transform.GetChild(i).gameObject;
			for(int j = 0; withOut != null && j < withOut.Length; j++){
				if(child == withOut[j])
					continue;
			}
			GameObject.Destroy(child);

		}
	}

	public static string ArrayToString(int[] array)
	{
		if(array == null)
			return "null";
		string result = "[";
		for (int i = 0; i < array.Length; i++) {
			result += array[i].ToString();
			if(i < array.Length - 1)
				result += ",";
		}
		result += "]";
		return result;
	}

	public static List<T> ArrayList2List<T>(ArrayList arrayList)
	{
		if (arrayList == null)
			return null;
		List<T> list = new List<T> ();
		foreach (object obj in arrayList) {
			list.Add((T)obj);
		}
		return list;
	}

	public static int GetCharacterCount(string str)
	{
		if(string.IsNullOrEmpty(str))
			return 0;
		char[] cs = str.ToCharArray();
		int len = 0;
		for(int i = 0; i < cs.Length; i++)
		{
			if(cs[i] >= 19968 && cs[i] <= 40623)
				len += 2;
			else
				len += 1;
		}
		return len;
	}

    public static void SetLayer(GameObject obj,int layer)
    {
        obj.layer = layer;
        int count = obj.transform.childCount;
        for (int i = 0; i < count; i++)
        {
            SetLayer(obj.transform.GetChild(i).gameObject,layer);
        }
    }
	public static void RemoveAllChild(Transform parent)
	{
		Transform[] childs=parent.GetComponentsInChildren<Transform>();
		foreach(Transform item in childs)
		{
			if(item!=parent)
			{
				MonoBehaviour.Destroy(item.gameObject);
			}
		}
	}
	/// <summary>
	/// 获得指定容器中所有子节点的材质渲染队列值
	/// </summary>
	/// <param name="renderQueue">渲染队列值</param>
	public static void SetMaterialRenderQueueByAll (GameObject obj, int renderQueue,Color color) {
		if (obj == null) return;

		Renderer[] srs = obj.GetComponentsInChildren<Renderer> ();
		if (srs == null)
			return;
		Renderer sr;
		for (int i = 0; i < srs.Length; ++i) {
			sr = srs [i];
			if (sr == null || sr.material == null)
				continue;
			sr.material.renderQueue = renderQueue;
			sr.material.color = color;
		}
	}
	/// <summary>
	/// 获得父容器中指定名字的子节点
	/// </summary>
	/// <param name="nodeName">节点名</param>
	public static GameObject getNodeObjByName(GameObject parent,string nodeName)
	{
		if (parent == null)
			return null;
		Transform trans=parent.transform;
		int childCount = trans.transform.childCount;
		GameObject temp=null;
		for (int i = 0; i < childCount; ++i) {
			temp=trans.GetChild (i).gameObject;
			if(temp.name==nodeName){
				return temp;
			}
		}
		return null;
	}
	/// <summary>
	/// 校验字符串中的非法字符(主要以sql为主)
	/// </summary>
	public static bool EncodeToValid(string str)
	{
		if (Sharp.SHARP1.encode (str, new CharBuffer()))
			return true;
		return false;
	}
	/// <summary>
	/// 替换字符串中的非法字符(主要以sql为主)
	/// </summary>
	public static void EncodeToValid(string str,CharBuffer charBuffer)
	{
		Sharp.SHARP1.encode(str, charBuffer);
	}
}
