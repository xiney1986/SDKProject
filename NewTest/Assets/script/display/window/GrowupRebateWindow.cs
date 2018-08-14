using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 成长奖励返利窗口
/// </summary>
public class GrowupRebateWindow : WindowBase {

	/** 容器 */
	public GrowupRebateContent growupContent;
	/**method */
	protected override void begin () {
		base.begin ();
		if (GrowupAwardMangement.Instance.GetAllAwardsList () == null) {
			GrowupAwardMangement.Instance.GetAllAwardsList (doBegin);
		}
		else {
			doBegin ();
		}
		MaskWindow.UnlockUI ();
	}
	/** 执行begin */
	public void  doBegin () {
		updateUI ();
		MaskWindow.UnlockUI ();
	}
	public void updateUI () {
		List<GrowupAwardSample> awards = GrowupAwardMangement.Instance.GetAllAwardsList ();
		growupContent.reLoad (awards.ToArray ());
	}
	//断线重新连接
	public override void OnNetResume () {
		base.OnNetResume ();
		updateUI ();
	}
	public override void buttonEventBase (GameObject gameObj) {
		base.buttonEventBase (gameObj); 
		if (gameObj.name == "close") {
			finishWindow ();
		}
	}
}
