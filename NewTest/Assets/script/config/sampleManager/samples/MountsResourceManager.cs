using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 坐骑资源路径管理
/// </summary>
public class MountsResourceManager : ConfigManager {

	/* static fields */
	private static MountsResourceManager instance;

	/* static methods */
	public static MountsResourceManager Instance {
		get {
			if (instance == null)
				instance = new MountsResourceManager ();
			return instance;
		}
	}

	/* fields */
	private List<string> paths;

	/* methods */
	public MountsResourceManager () {
		base.readConfig (ConfigGlobal.CONFIG_MOUNTSRESOURCE);
	}
	
	public override void parseConfig (string str) {
		if(paths == null){
			paths = new List<string>();
		}
		paths.Add (str);
	}
	/** 获取坐骑资源路径 */
	public string[] GetPaths(){
		return paths.ToArray();
	}
}
