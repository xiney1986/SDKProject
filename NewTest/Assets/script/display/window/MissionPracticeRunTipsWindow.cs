using UnityEngine;
using System.Collections.Generic;


/// <summary>
/// 
/// </summary>
public class MissionPracticeRunTipsWindow : WindowBase {

	public PracticeAutoRunAwardContent UI_Award;
	public UILabel UI_CurrentNode;


	/// <summary>
	/// 断线重连
	/// </summary>
	public override void OnNetResume () {
		base.OnNetResume ();


	}

	/// <summary>
	/// 初始化
	/// </summary>
	public void init () {

	}

	/// <summary>
	/// begin
	/// </summary>
	protected override void begin () {
		base.begin ();


		MaskWindow.UnlockUI ();
	}

	public void SetCurrentNode (int index) {
		UI_Award.addPrizeAnimation (FuBenPracticeConfigManager.Instance.getPrizeByIndex (index), index);
		UI_CurrentNode.text = Language ("missionPracticeRunTips_2", index.ToString());
	}
}

