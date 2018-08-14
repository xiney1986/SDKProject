using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OneKeyWindow : WindowBase
{
	public UIToggle[] Quality_Box;
	public UIToggle neverChoose;
	public GameObject spiritCardBox;// 全部精灵卡选项//
	public GameObject sacrificeEquipBox;// 全部祭品装备选项//

	protected override void begin ()
	{
		base.begin ();
		MaskWindow.UnlockUI ();
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "confirm") {

			if(fatherWindow != null && fatherWindow.GetType() == typeof(IntensifyCardWindow)) {
				if (Quality_Box [0].value)
					IntensifyCardManager.Choose = QualityType.EXCELLENT;
				else if (Quality_Box [1].value)
					IntensifyCardManager.Choose = QualityType.GOOD;
				else if (Quality_Box [2].value)
					IntensifyCardManager.Choose = QualityType.EPIC;
				else if (Quality_Box [3].value)
					IntensifyCardManager.Choose = QualityType.LEGEND;
				else if(Quality_Box [4].value)
					IntensifyCardManager.Choose = QualityType.MYTH;

				if (neverChoose.value)
					IntensifyCardManager.IsOpenOneKeyWnd = false;
				IntensifyCardWindow intensifyWnd = fatherWindow as IntensifyCardWindow;
				intensifyWnd.sacrificeContent.OneKeyChoose ();
			}
			if(fatherWindow != null && fatherWindow.GetType() == typeof(IntensifyEquipWindow)) {
				if (Quality_Box [0].value)
					IntensifyEquipManager.Choose = QualityType.EXCELLENT;
				else if (Quality_Box [1].value)
					IntensifyEquipManager.Choose = QualityType.GOOD;
				else if (Quality_Box [2].value)
					IntensifyEquipManager.Choose = QualityType.EPIC;
				else if (Quality_Box [3].value)
					IntensifyEquipManager.Choose = QualityType.LEGEND;
				else if(Quality_Box [4].value)
					IntensifyEquipManager.Choose = QualityType.MYTH;
				if (neverChoose.value)
					IntensifyEquipManager.IsOpenOneKeyWnd = false;
				IntensifyEquipWindow equipWin = fatherWindow as IntensifyEquipWindow;
				equipWin.OneKeyChoose();
			}
			// 出售界面//
			if(fatherWindow != null && fatherWindow.GetType() == typeof(SellWindow))
			{
				SellWindow sellWin = fatherWindow as SellWindow;
				if (Quality_Box [0].value)
					sellWin.qualityType = QualityType.EXCELLENT;
				else if (Quality_Box [1].value)
					sellWin.qualityType = QualityType.GOOD;
				else if (Quality_Box [2].value)
					sellWin.qualityType = QualityType.EPIC;
				else if (Quality_Box [3].value)
					sellWin.qualityType = QualityType.LEGEND;
				else if(Quality_Box [4].value)
					sellWin.qualityType = QualityType.MYTH;
				else if(Quality_Box [5].value)
					sellWin.qualityType = QualityType.SPIRITCARD;
				else if(Quality_Box [6].value)
					sellWin.qualityType = QualityType.SACRIFICE;

				sellWin.oneKeyChoose();
			}
		}
		finishWindow ();
	}
}
