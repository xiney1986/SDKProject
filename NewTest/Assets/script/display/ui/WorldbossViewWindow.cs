using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 世界Boss展示
/// </summary>
public class WorldbossViewWindow : WindowBase {
	/**克星卡片*/
	public RoleView[] views;
	/**时间*/
	public UILabel timeLabel;
	/**worldboss Icon*/
	public UITexture bossIcon;
	/**克星卡片sid*/
	private int[] sids;
	/// <summary>
	/// 加载克星卡片和worldboss
	/// </summary>
	void Start()
	{
		initCard ();
		initWorldboss (15124);
	}
	/// <summary>
	/// begin
	/// </summary>
	protected override void begin ()
	{
		base.begin ();
//		initCard ();
//		initWorldboss (2010);
		MaskWindow.UnlockUI ();
	}
	/// <summary>
	/// 初始化克星卡片
	/// </summary>
	public void initCard () {
		sids = WorldBossManagerment.Instance.getNemesidCard ();
		for (int i = 0; i < views.Length; i++) {
			if (sids !=null && i < sids.Length) {
				views [i].gameObject.SetActive (true);
				views [i].tempObj = i;
				views [i].init (CardManagerment.Instance.createCard (sids[i]),this,(roleView) => {
					UiManager.Instance.openWindow<CardPictureWindow> ((win) => {
						win.init (PictureManagerment.Instance.mapType [roleView.card.getEvolveNextSid ()], 0);
					});
				});
			}
			else {
				views [i].gameObject.SetActive (false);
			}
		}
	}
	/// <summary>
	/// 初始worldboss
	/// </summary>
	public void initWorldboss (int sid) {
		Card boss = CardManagerment.Instance.createCard (sid);
		ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.CARDIMAGEPATH + boss.getImageID (), bossIcon);
	}
	/// <summary>
	/// 处理按钮事件
	/// </summary>
	public override void buttonEventBase (GameObject gameObj) {
		if (gameObj.name == "awardDesButton") {
			UiManager.Instance.openWindow<WorldbossAwardWindow> ();
		}
		else if (gameObj.name == "challengeButton") {
			UiManager.Instance.openWindow<WorldBossWindow>(win =>{
				win.initialization();
			});
		}
		else if (gameObj.name == "close") {
			finishWindow();
		}
	}
	/// <summary>
	/// 更新时间
	/// </summary>
	void Update () {
		if (WorldBossManagerment.Instance.isInTime ()) {
			timeLabel.text = ServerTimeKit.getDateTime().ToString();
			timeLabel.text = WorldBossManagerment.Instance.getOverTime ().ToString () + "over";
		}
		else {
			timeLabel.text = WorldBossManagerment.Instance.getOverTime ().ToString () + "start";
			//test
			timeLabel.text = ServerTimeKit.getDateTime().ToString();
		}
	}
}
