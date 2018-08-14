using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// 图鉴配置管理器
/// </summary>
public class PictureSampleManager : SampleConfigManager {
	public static PictureSampleManager Instance {
		get { return SingleManager.Instance.getObj ("PictureSampleManager") as PictureSampleManager; }
	}

	public List<PictureSample> pictureList;
	public PictureSampleManager () {
		base.readConfig (ConfigGlobal.CONFIG_PICTURE);
		initPictureList ();

	}

	/// <summary>
	/// 初始化图鉴配置
	/// </summary>
	void initPictureList(){
		pictureList = new List<PictureSample> ();
		foreach (DictionaryEntry item in data) {
			PictureSample pic = new PictureSample();
			pic.parse(StringKit.toInt(item.Key.ToString()),item.Value.ToString());
			pictureList.Add(pic);
		}
	}


}
