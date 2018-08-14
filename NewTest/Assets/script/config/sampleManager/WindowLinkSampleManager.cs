using UnityEngine;
using System.Collections;

/// <summary>
/// 窗口线路连接模板管理器
/// </summary>
public class WindowLinkSampleManager : SampleConfigManager {


	public static WindowLinkSampleManager Instance {
		get {
			return SingleManager.Instance.getObj ("WindowLinkSampleManager") as WindowLinkSampleManager;
		}
	}

	public WindowLinkSampleManager () {
		base.readConfig (ConfigGlobal.CONFIG_WINDOW_LINK);
	}

	public WindowLinkSample getDataBySid (int sid) {
		if (!isSampleExist (sid))
			createSample (sid);
		return samples [sid] as WindowLinkSample;
	}

	public override void parseSample (int sid) {
		WindowLinkSample sample = new WindowLinkSample ();
		string dataStr = getSampleDataBySid (sid);
		sample.parse (sid, dataStr);
		samples.Add (sid, sample);
	}
}
