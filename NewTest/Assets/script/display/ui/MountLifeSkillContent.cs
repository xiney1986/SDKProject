using UnityEngine;
using System.Collections;

public class MountLifeSkillContent  : MonoBase {
	
	/* gameobj fields */
	/** 修炼按钮 */
	public ButtonBase buttonPower;
	/** 生活技能-存储行动力 */
	public MountPveStoreSkillItem pveStoreSkillItem;
	/** 生活技能-共鸣 */
	public MountCombinSkillItem combinSkillItem;
	/** 骑术经验条 */
	public ExpbarCtrl skillExpBar;
	/** 骑术等级文本 */
	public UILabel skillLevelText;
	/** 经验文本 */
	public UILabel expLabel;
	/** 父窗口 */
	WindowBase fatherWindow;
	
	/* fields */
	
	/* methods */
	void Start () {
		buttonPower.onClickEvent = HandlePowerEvent;
	}
	/***/
	public void init(WindowBase fatherWindow) {
		this.fatherWindow=fatherWindow;
		initButton (fatherWindow);
		UpdateUI();
	}
	/** 初始化button */
	public void initButton(WindowBase fatherWindow) {
		buttonPower.fatherWindow=fatherWindow;
	}
	/** 更新UI */
	public void UpdateUI() {
		updateExpBar();
		pveStoreSkillItem.updateUI();
		combinSkillItem.updateUI();
	}
	/// <summary>
	/// 经验条
	/// </summary>
	private void updateExpBar() {
		MountsManagerment manager=MountsManagerment.Instance;
		expLabel.text=EXPSampleManager.Instance.getExpBarShow (EXPSampleManager.SID_MOUNTS_EXP,manager.getMountsExp ());
		skillLevelText.text= "Lv."+manager.getMountsLevel();
		LevelupInfo levelupInfo=manager.createLevelupInfo();
		skillExpBar.init (levelupInfo);
	}
	/** 处理修炼事件 */
	private void HandlePowerEvent(GameObject gameObj) {
		UiManager.Instance.openWindow<MountsPracticeWindow>();
	}
	/** button点击事件 */
	public void buttonEventBase (GameObject gameObj) {
		MaskWindow.UnlockUI();
	}
}