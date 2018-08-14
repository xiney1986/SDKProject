using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// growup奖励窗口
/// </summary>
public class GrowupAwardWindow: WindowBase { 

	/* fields */
	/** 容器 */
	public GrowupAwardContent growupContent;

	/**method */
	protected override void begin () {
		base.begin ();
		if (GrowupAwardMangement.Instance.GetAwardList () == null ) {
			GrowupAwardMangement.Instance.InitAwards (doBegin);
		}
		else {
			doBegin ();
		}
	}
	/** 执行begin */
	public void  doBegin () {
		updateUI (0);
		MaskWindow.UnlockUI ();
	}
	/** 更新UI */
	public void updateUI (int jumpIndex) {
		List<GrowupAwardSample> awards = GrowupAwardMangement.Instance.GetAwardList ();
		growupContent.reLoad (awards.ToArray (),jumpIndex);
	}
	//断线重新连接
	public override void OnNetResume () {
		base.OnNetResume ();
		updateUI (0);
	}
	/// <summary>
	/// 处理button事件
	/// </summary>
	/// <param name="gameObj">Game object.</param>
	public override void buttonEventBase (GameObject gameObj) {
		base.buttonEventBase (gameObj); 
		if (gameObj.name == "close") {
			finishWindow ();
		}
		else {
			if (gameObj.name == "AwardButton") {
				ButtonBase button = gameObj.GetComponent<ButtonBase>();
				string needLevel = button.exFields["needLevel"].ToString();
				int index = (int)button.exFields["index"];
				GrowupAwardMangement manager=GrowupAwardMangement.Instance;
				manager.GetAward (needLevel, () => {
					if (manager.getAwardStatas == "ok") {
						UiManager.Instance.createMessageLintWindow (LanguageConfigManager.Instance.getLanguage ("s0120"));
						manager.RemoveItem (needLevel);
						updateUI (index);
					}
					MaskWindow.UnlockUI ();
				});
			}
		}
	}
}