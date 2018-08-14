using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 屏蔽字配置解析文件
 * @author 汤琦
 * */
public class ShieldConfigManager : ConfigManager
{
	public string[] shields;//屏蔽字集
	public string[] shields2;//屏蔽字集
	
	//单例
	private static ShieldConfigManager _Instance; 
  
	public static ShieldConfigManager Instance {
		get {
			if (_Instance == null) {
				_Instance = new ShieldConfigManager (); 
			} 
			return _Instance; 
		} 
		set { 
			_Instance = value;
		} 
	}
	
	public ShieldConfigManager ()
	{   
		base.readConfig (ConfigGlobal.CONFIG_SHIELD);
	}
	
	public override void parseConfig (string str)
	{
		shields = str.Split (',');
		List<string> tmp = new List<string>(shields);
		tmp.RemoveAll((s)=>{return s=="!";});
		tmp.RemoveAll((s)=>{return s=="@";});
		tmp.RemoveAll((s)=>{return s==",";});
		tmp.RemoveAll((s)=>{return s=="#";});
		shields2 = tmp.ToArray();
		MonoBase.print(shields.Length);
	}
}
