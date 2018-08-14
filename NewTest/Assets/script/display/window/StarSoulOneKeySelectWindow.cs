using UnityEngine;
using System.Collections;

/// <summary>
/// 星魂一键选择选择窗口
/// </summary>
public class StarSoulOneKeySelectWindow : WindowBase {
	
	/** 品质选择框 */
	public UIToggle[] neverChooses;
	public UIToggle notShowThisLogin;
	public CallBack callback;

	/***/
	public void init(){
		updateUI();
	}
	protected override void begin () {
		base.begin ();
		MaskWindow.UnlockUI ();
	}
	void updateUI(){
		int chooseIndex=GameManager.Instance.starSoulOneKeySelectValue;
		if(chooseIndex == QualityType.COMMON)neverChooses[4].value=true;
		else if(chooseIndex == QualityType.EXCELLENT)neverChooses[3].value=true;
		else if(chooseIndex == QualityType.GOOD)neverChooses[2].value=true;
		else if(chooseIndex == QualityType.EPIC)neverChooses[1].value=true;
		else if(chooseIndex == QualityType.LEGEND)neverChooses[0].value=true;
	}
	/***/
	public override void buttonEventBase (GameObject gameObj) {
		base.buttonEventBase (gameObj);
		if (gameObj.name == "confirm") {
			int chooseIndex=GameManager.Instance.starSoulOneKeySelectValue;
			if (neverChooses [4].value)
				chooseIndex = QualityType.COMMON;
			else if (neverChooses [3].value)
				chooseIndex = QualityType.EXCELLENT;
			else if (neverChooses [2].value)
				chooseIndex = QualityType.GOOD;
			else if (neverChooses [1].value)
				chooseIndex = QualityType.EPIC;
            else if (neverChooses[0].value)
                chooseIndex = QualityType.LEGEND;
			if(notShowThisLogin.value)GameManager.Instance.isShowStarSoulOneKeySelect=false;
			GameManager.Instance.starSoulOneKeySelectValue=chooseIndex;
			StarSoulManager.Instance.clearChangeExpStateDic();
			callback();

		}
		finishWindow ();
	}
}
