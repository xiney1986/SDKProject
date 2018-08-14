using UnityEngine;
using System.Collections;

/// <summary>
/// 自动移动设置界面 
/// </summary>
public class AutoMoveSetWindow : WindowBase {
	private bool[] mSettings;//两个设置的状态 勾选为true 不勾选是false
	public Transform switchContainer;//放选择框的组件
	private SysSet[] sysSets;//两个选择框
	private const int BUTTONLENGTH=2;//几个选择框
	protected override void begin () {
		base.begin ();
		MaskWindow.UnlockUI ();
	}
	public override void buttonEventBase ( GameObject gameObj ) {
		base.buttonEventBase (gameObj);
		if(gameObj.name=="close"){
			if(MissionInfoManager.Instance.mission!=null&&MissionInfoManager.Instance.mission.sid==41005&&FuBenManagerment.Instance.isNewMission (ChapterType.STORY,41005)&&GuideManager.Instance.loadTimes(51007985)<1){
				GuideManager.Instance.saveTimes(51007985);
			}
			finishWindow ();
		}else if(gameObj.name=="enter"){
				if(MissionInfoManager.Instance.mission!=null&&MissionInfoManager.Instance.mission.sid==41005&&FuBenManagerment.Instance.isNewMission (ChapterType.STORY,41005)&&GuideManager.Instance.loadTimes(51007985)<1){
				GuideManager.Instance.saveTimes(51007985);
			}
			MissionInfoManager.Instance.mSettings[0]=mSettings[0];
			MissionInfoManager.Instance.mSettings[1]=mSettings[1];
			finishWindow ();
			if (MissionManager.instance.AutoRunIndex != -1) {
				MissionInfoManager.Instance.autoGuaji=true;
				MissionManager.instance.AutoRunStart ();
				return;
			}
			if(!MissionInfoManager.Instance.autoGuaji){
				MissionInfoManager.Instance.autoGuaji=true;
				UiManager.Instance.missionMainWindow.stopButton.gameObject.SetActive(true);
				StartCoroutine(MissionManager.instance.autoMove(0f));
			}
		}else if(gameObj.name=="pkstate"){//PK情况
			if(mSettings[0])setSetting("pkPen",0);
			else setSetting("pkPen",1);
			updateUI(true);
		}else if(gameObj.name=="falsestate"){//失败的情况
			if(mSettings[1])setSetting("flasePen",0);
			else setSetting("flasePen",1);
			updateUI(true);
		}
		MaskWindow.UnlockUI();
	}
	public override void OnStart () {
		base.OnStart ();
		mSettings = new bool[BUTTONLENGTH];
		sysSets = switchContainer.GetComponentsInChildren<SysSet> ();
		getSetting();
		updateUI (true);
	}
	private void updateUI ( bool isUpdateSettingsItem ) {
		for (int i = 0; i < sysSets.Length; i++) {
			sysSets[i].choose.gameObject.SetActive (mSettings[i]);
		}
	}
	/// <summary>
	/// 设置或获取两个按钮的状态值，最初就是默认没有选择
	/// </summary>
	private void getSetting(){
		mSettings[0]=PlayerPrefs.GetInt(UserManager.Instance.self.uid+"pkPen",0)==1;
		mSettings[1]=PlayerPrefs.GetInt(UserManager.Instance.self.uid+"flasePen",0)==1;
	}
	/// <summary>
	/// 设置或获取两个按钮的状态值，最初就是默认没有选择
	/// </summary>
	private void setSetting(string str,int i){
		PlayerPrefs.SetInt(UserManager.Instance.self.uid+str,i);
		getSetting();
	}
}
