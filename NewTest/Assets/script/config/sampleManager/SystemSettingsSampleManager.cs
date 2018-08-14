using UnityEngine;
using System.Collections;

/// <summary>
/// 系统设置模板管理器
/// </summary>
public class SystemSettingsSampleManager : SampleConfigManager {

	public static SystemSettingsSampleManager Instance {
		get {
			return SingleManager.Instance.getObj ("SystemSettingsSampleManager") as SystemSettingsSampleManager;
		}
	}

	private bool[] mSettings;

	public SystemSettingsSampleManager () {
		base.readConfig (ConfigGlobal.CONFIG_SYS_SET);
	}

	public bool[] getDefualtSettings () {
		if (mSettings == null) {
			string[] strs = data [0].ToString ().Split (',');
			mSettings = System.Array.ConvertAll<string, bool> (strs, ( str ) => {
				return str == "1";
			});
		}
		return mSettings;
	}
}