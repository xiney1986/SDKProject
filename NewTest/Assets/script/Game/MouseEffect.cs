using UnityEngine;
using System.Collections;

/// <summary>
/// 鼠标效果
/// </summary>
public class MouseEffect : MonoBase {

	/* fields */
	/** 触发特效对象 */
	public Transform touchEffect;

	/* methods */
	/** 更新 */
	void Update () {
		if(Input.GetMouseButtonDown(0)) {
			EffectCtrl obj = EffectManager.Instance.CreateEffect (touchEffect,"Effect/UiEffect/touchEffect");
			obj.transform.position = UiManager.getMousePosition();
			obj.transform.parent = touchEffect;
		}
	}
}