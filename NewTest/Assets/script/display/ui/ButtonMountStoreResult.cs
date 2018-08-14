using UnityEngine;
using System.Collections;

/// <summary>
/// 坐骑仓库功能按键 
/// </summary>
public class ButtonMountStoreResult : ButtonBase {

	/* gameobj filed */
	/** 状态类型 */
	private Bgeneralscrollview.ButtonStateType stateType;
	/** button显示标签 */
	public UILabel buttonLabel;

	/* filed */
	/** 当前选择的坐骑 */
	Mounts mounts;

	/* methods */
	/// <summary>
	/// 更新button
	/// </summary>
	/// <param name="mounts">当前选择的坐骑</param>
	/// <param name="type">button状态类型</param>
	public void UpdateResultButton (Mounts mounts,Bgeneralscrollview.ButtonStateType type) {
		this.mounts=mounts;
		this.stateType=type;
		UpdateUI ();
	}
	/** 更新UI */
	public void UpdateUI() {
		UpdateButtonShow ();
	}
	/** 更新button显示 */
	public void UpdateButtonShow() {
		switch (stateType) {
		case Bgeneralscrollview.ButtonStateType.ride:
			ShowRideState();
			break;
		case Bgeneralscrollview.ButtonStateType.acitve:
			ShowActiveState();
			break;
		case Bgeneralscrollview.ButtonStateType.stop:
			ShowStopState();
			break;
		default:				
			break;
		}
	}
	/** 骑乘状态 */
	private void ShowRideState(){
		buttonLabel.text=LanguageConfigManager.Instance.getLanguage("mount_button1");
	}
	/** 显示激活状态 */
	private void ShowActiveState(){
		buttonLabel.text=LanguageConfigManager.Instance.getLanguage("goddess07");
	}
	/** 显示休息状态 */
	private void ShowStopState() {
		buttonLabel.text=LanguageConfigManager.Instance.getLanguage("mount_stop");
	}
	/***/
	public override void DoClickEvent () {
		base.DoClickEvent ();
		if(stateType==Bgeneralscrollview.ButtonStateType.ride) {
			// 与服务器通讯--骑乘
			(FPortManager.Instance.getFPort ("MountsRideFPort") as MountsRideFPort).putOnMountsAccess (mounts.uid, ()=>{
				if(fatherWindow is MountsWindow) {
					//激活坐骑
					if (GuideManager.Instance.isEqualStep (134006000)) {
						GuideManager.Instance.doGuide ();
						GuideManager.Instance.guideEvent ();
					}
					MountsWindow win=fatherWindow as MountsWindow;
					win.changeTapPage(MountsWindow.TAP_ATTR_CONTENT);
				}
				MaskWindow.UnlockUI();
			});
		} else if(stateType==Bgeneralscrollview.ButtonStateType.acitve) {
			// 与服务器通讯--激活
			(FPortManager.Instance.getFPort ("MountsRideFPort") as MountsRideFPort).activeMountsAccess (mounts.sid, ()=>{
				UiManager.Instance.openDialogWindow<MessageLineWindow>((win) =>{
					win.Initialize(LanguageConfigManager.Instance.getLanguage("mount_active_success"));
				});
				if(fatherWindow is MountsWindow) {
					//激活坐骑
					if (GuideManager.Instance.isEqualStep (134005000)) {
						GuideManager.Instance.doGuide ();
					}
					MountsWindow win=fatherWindow as MountsWindow;
					win.UpdateContent();
				}
			});
		} else if(stateType==Bgeneralscrollview.ButtonStateType.stop) {
			// 与服务器通讯--休息
			(FPortManager.Instance.getFPort ("MountsRideFPort") as MountsRideFPort).putOffMountsAccess (()=>{
				if(fatherWindow is MountsWindow) {
					MountsWindow win=fatherWindow as MountsWindow;
					win.changeTapPage(MountsWindow.TAP_ATTR_CONTENT);
				}
				MaskWindow.UnlockUI();
			});
		}
	}
}
