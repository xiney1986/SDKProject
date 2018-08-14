using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 *预告配置解析文件
 *@author 汤琦
 **/
public class ForeShowConfigManager : ConfigManager
{
	public List<int> levels;//等级
	public List<string> prompt;//提示玩家下一阶段开放的功能
	public List<string> names;//提示玩家即将开放功能的名称
	public List<string> contents;//提示框内容
	private int _index;
	
	public int index
	{
		get { return this._index; }
		set { this._index = value; }
	}
	//单例
	private static ForeShowConfigManager _Instance; 
	public static ForeShowConfigManager Instance {
		get {
			if (_Instance == null) {
				_Instance = new ForeShowConfigManager (); 
			} 
			return _Instance; 
		} 
		set { 
			_Instance = value;
		} 
	}
	public ForeShowConfigManager ()
	{   
		base.readConfig (ConfigGlobal.CONFIG_FORESHOW);
	}
	
	public override void parseConfig (string str)
	{
		string[] arr = str.Split ('|'); 
		int max = arr.Length;
		if (levels == null)
			levels = new List<int> ();
		if (prompt == null)
			prompt = new List<string> ();
		if (names == null)
			names = new List<string> ();
		if (contents == null)
			contents = new List<string> (); 
		levels.Add (StringKit.toInt (arr [0]));
		prompt.Add (arr [1]);
		names.Add (arr [2]);
		contents.Add (arr [3]); 
	}
	
	public string getContents()
	{
		return contents[_index];
	}
}
