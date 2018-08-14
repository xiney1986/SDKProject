using UnityEngine;
using System.Collections;

/// <summary>
/// 星魂拾取选择窗口
/// </summary>
public class StarSoulGetChooseWindow : WindowBase {
	
	/** 品质选择框 */
	public UIToggle[] neverChooses;

	/***/
	public void init(){
		UpdateUI ();
	}
	protected override void begin () {
		base.begin ();
		MaskWindow.UnlockUI ();
	}
	/** 更新UI */
	public void UpdateUI(){
		int quality=PlayerPrefs.GetInt (UserManager.Instance.self.uid + PlayerPrefsComm.STARSOUL_CHOOSE_QUALITY);
		if (quality==QualityType.LEGEND)
			neverChooses[0].value=true;
		else if (quality==QualityType.EPIC)
			neverChooses[1].value=true;
		else if (quality==QualityType.GOOD)
			neverChooses[2].value=true;
		else if (quality==QualityType.EXCELLENT)
			neverChooses[3].value=true;
		else
			neverChooses[4].value=true;
	}
	/***/
	public override void buttonEventBase (GameObject gameObj) {
		base.buttonEventBase (gameObj);
		if (gameObj.name == "confirm") {
			int chooseIndex=QualityType.COMMON;
			if (neverChooses [0].value)
				chooseIndex = QualityType.LEGEND;
			else if (neverChooses [1].value)
				chooseIndex = QualityType.EPIC;
			else if (neverChooses [2].value)
				chooseIndex = QualityType.GOOD;
			else if (neverChooses [3].value)
				chooseIndex = QualityType.EXCELLENT;
			else if (neverChooses [4].value)
				chooseIndex = QualityType.COMMON;
			PlayerPrefs.SetInt (UserManager.Instance.self.uid + PlayerPrefsComm.STARSOUL_CHOOSE_QUALITY, chooseIndex);
			PlayerPrefs.Save();
		}
		finishWindow ();
	}
}
