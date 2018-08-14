using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 随机名配置解析文件
 * @author 汤琦
 * */
public class RandomNameConfigManager : ConfigManager
{
	public string[] prefixNames;//名字前形容词
	public string[] manNames;//男名
	public string[] womanNames;//女名


	//单例
	private static RandomNameConfigManager _Instance; 
  
	public static RandomNameConfigManager Instance {
		get {
			if (_Instance == null) {
				_Instance = new RandomNameConfigManager (); 
			} 
			return _Instance; 
		} 
		set { 
			_Instance = value;
		} 
	}
	
	public RandomNameConfigManager ()
	{   
		base.readConfig (ConfigGlobal.CONFIG_RANDOMNAME);
	}
	
	public override void parseConfig (string str)
	{
		string[] arr = str.Split ('|');
		parsePrifixName(arr[0]);
		parseManName(arr[1]);
		parseWomanName(arr[2]);
	}
	
	private void parsePrifixName(string str)
	{
		string[] strs = str.Split(',');
		prefixNames = new string[strs.Length];
		for (int i = 0; i < prefixNames.Length; i++) {
			prefixNames[i] = strs[i];
		}
	}
	
	private void parseManName(string str)
	{
		string[] strs = str.Split(',');
		manNames = new string[strs.Length];
		for (int i = 0; i < manNames.Length; i++) {
			manNames[i] = strs[i];
		}
	}
	private void parseWomanName(string str)
	{
		string[] strs = str.Split(',');
		womanNames = new string[strs.Length];
		for (int i = 0; i < womanNames.Length; i++) {
			womanNames[i] = strs[i];
		}
	}
}
