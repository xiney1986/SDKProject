using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 限时抽奖广播展示窗口
/// </summary>
public class LuckyRadioWindow : WindowBase {

	/** 文本显示列表 */
	public UITextList textList;
	/** 关闭标签 */
	public UILabel closeLabel;

	protected override void begin () {
		base.begin ();
		MaskWindow.UnlockUI ();
	}
	/** 初始化UI */
	public void initUI (string[] strList) {
		textList.Clear ();
		if (strList != null) {
			for (int i = 0; i < strList.Length; i++) {
				textList.Add(strList[i]);
			}
		}
	}
	/***/
	void Update () {
		if(closeLabel.gameObject.activeSelf) {
			closeLabel.alpha =sin();
		}
	}
	/***/
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") { 
			finishWindow();
		}
	}
}
