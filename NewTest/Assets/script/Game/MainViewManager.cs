using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainViewManager : MonoBase
{

	// Use this for initialization
	public static  MainViewManager Instance;
	
	void Awake ()
	{
		Instance = this;
		//StartCoroutine (cacheWindow ());
		cacheWindowFinish(null);
	}
	
	public IEnumerator cacheWindow ()
	{
	
		/*
		string[] _list = new string[]{
			"Effect/Other/Gem/GreenGem/GreenGem",
			"Effect/Other/Gem/blueGem/blueGem",
			"Effect/Other/Gem/VioletGem/VioletGem",
			"Effect/Other/Gem/yellowGem/yellowGem",
			"Effect/Other/Gem/GemShengji/GemShengji",
		};


		TextAsset ta = Resources.Load("MoveList",typeof(TextAsset)) as TextAsset;
		string[] lines = ta.text.Split(new string[]{"\r\n"},System.StringSplitOptions.RemoveEmptyEntries);
		List<string> list = new List<string>();
		for(int i = 0; i < lines.Length; i++)
		{
			string str = lines[i];
			
			if(str.StartsWith("UI/loadingWindow") || str.StartsWith("UI/messageWindow"))
			{
				continue;
			}
			
			if(str.StartsWith("UI/") || str.StartsWith("Effect/UiEffect") || str.StartsWith("TeamPrepare/player/"))
			{
				list.Add(str);
			}
		}
		list.AddRange(_list);

		if(ResourcesManager.Instance.allowLoadFromRes)
			cacheWindowFinish(new List<ResourcesData>());
	//	else
		//	ResourcesManager.Instance.cacheData (list.ToArray(), cacheWindowFinish, false);
 */

		yield break;
	}
	

	

	void cacheWindowFinish (List<ResourcesData> _list)
	{
		ResourcesManager.Instance.cacheProgress = 1;
	}
	

}
