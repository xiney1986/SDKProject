using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 图鉴获取途径模版管理器
/// </summary>
public class PictureTipsSampleConfigManager : SampleConfigManager {
	private static PictureTipsSampleConfigManager instance;
	public static PictureTipsSampleConfigManager Instance
	{
		get{
			if(instance == null)
				instance = new PictureTipsSampleConfigManager();
			return instance;
		}
	}
	public List<PictureTipsSample> allPictureTips;
	public PictureTipsSampleConfigManager()
	{
		base.readConfig (ConfigGlobal.CONFIG_PICTURETIPS);
		initPictureTips ();
	}

	private void initPictureTips(){
		allPictureTips = new List<PictureTipsSample> ();
		foreach (DictionaryEntry item in data) {
			PictureTipsSample picTips = new PictureTipsSample();
			picTips.parse(StringKit.toInt(item.Key.ToString()),item.Value.ToString());
			allPictureTips.Add(picTips);
		}
	}

}
