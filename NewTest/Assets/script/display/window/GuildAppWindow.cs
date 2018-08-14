using UnityEngine;
using System.Collections;

/**
 * 公会审批窗口
 * @author 汤琦
 * */
public class GuildAppWindow : WindowBase {
	/* const */
	/** 容器下标常量 */
	const int TAP_APP_CONTENT = 0; // 申请列表
	/* fields */
	/** 当前tap下标--0开始 */
	int currentTapIndex = 0;
	/** tap容器 */
	public TapContentBase tapContent;
	public GuildAppContent content;
	public GameObject guildAppItem;
	/**自动加入复选框 */
	public UIToggle storeChoose;

	protected override void begin () {
		base.begin ();
		if (!isAwakeformHide) {
			tapContent.changeTapPage (tapContent.tapButtonList [currentTapIndex]);
		}
		else {
			UpdateContent ();
		}
		MaskWindow.UnlockUI ();
	}
	/** 激活 */
	protected override void DoEnable () {
		base.DoEnable ();
		GuildMainWindow gmw = UiManager.Instance.getWindow<GuildMainWindow> ();
		if (gmw != null) {
			UiManager.Instance.backGround.switchSynToDynamicBackground (gmw.launcherPanel, "gangBG", BackGroundCtrl.gangSize);
		}
	}
	public override void OnNetResume () {
		base.OnNetResume ();
		UpdateContent ();
	}
	/** 更新节点容器 */
	public void UpdateContent () {
		if (currentTapIndex == TAP_APP_CONTENT) {
			updateUIToggle();
			content.reLoad ();
		} 
	}
	void updateUIToggle(){
		storeChoose.value=GuildManagerment.Instance.getGuild().autoJoin==1;
		if(storeChoose.value){
			GuildManagerment.Instance.autoJoin=1;
		}else GuildManagerment.Instance.autoJoin=0;
	}
	public void initWindow () {
		this.currentTapIndex = 0;
	}
	public override void buttonEventBase (GameObject gameObj) {
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
			if (fatherWindow is GuildMemberWindow) {
				GuildMemberWindow win = fatherWindow as GuildMemberWindow;
				win.updateMember ();
			}
			GuildManagerment.Instance.updateAutoJoin();
			finishWindow ();
		}
	}
	/** 初始化容器 */
	private void initContent (int tapIndex) {
		switch (tapIndex) {
		case TAP_APP_CONTENT:
			updateUIToggle();
			content.reLoad ();
			break;
		}
	}
	/** tap 事件 */
	public override void tapButtonEventBase (GameObject gameObj, bool enable) {
		base.tapButtonEventBase (gameObj, enable);
		if (!enable)
			return;
		int tapIndex = int.Parse (gameObj.name) - 1;
		initContent (tapIndex);
		this.currentTapIndex = tapIndex;
	}
	/** 改变Tap */
	public void changeTapPage (int tapIndex) {
		if (tapIndex > tapContent.tapButtonList.Length - 1)
			return;
		tapContent.resetTap ();
		tapContent.changeTapPage (tapContent.tapButtonList [tapIndex]);
	}
	/** 改变Toggle */
	public void ChangeToggleValue() {
		GuildManagerment.Instance.setAutoJoin(storeChoose.value);
	}
}
