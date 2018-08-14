using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 晶炼消息窗口
/// </summary>
public class ResolveMessageWindow : MessageWindow {

	/* 分解奖励图标 */
	public UITexture[] resolveTexture;
	/* 分解奖励数量 */
	public UILabel[] numLabel;

	public void initWindow (int buttonNum, string button1Name, string button2Name, string content, CallBackMsg call, List<PrizeSample> pList) {
//		if (arrValues != null) {
//			for (int i=0; i<arrValues.Length; i++) {
//				numLabel [i].text = arrValues [i].ToString ();
//			}				
//		}
//		if (resolveTexture != null) {
//			for (int i = 0; i < textureNames.Length; i++) {
//				ResourcesManager.Instance.LoadAssetBundleTexture (textureNames [i], resolveTexture [i]);
//			}
//		}
		if (pList != null && pList.Count > 0) {
			for(int i=0 ;i< pList.Count;i++){
				numLabel [i].text = pList[i].num;
				ResourcesManager.Instance.LoadAssetBundleTexture (pList [i].getIconPath(), resolveTexture [i]);
			}
		}
		initWindow (buttonNum, button1Name, button2Name, content, call);
	}
}
