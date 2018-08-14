using UnityEngine;
using System.Collections;

/// <summary>
/// 坐骑属性容器
/// </summary>
public class MountsAttrContent : MonoBase {
	
	/* gameobj fields */
	/** 属性预制件 */
	public GameObject mountsAttrPrefab;
	/** 修炼按钮 */
	public ButtonBase buttonPower;
	/** 休息按钮 */
	public ButtonBase buttonUnsnatch;
	/** 更换按钮 */
	public ButtonBase buttonReplace;
	/** 选择按钮 */
	public ButtonBase buttonChoose;
	/** 骑术经验条 */
	public ExpbarCtrl skillExpBar;
	/** 骑术等级文本 */
	public UILabel skillLevelText;
	/** 经验文本 */
	public UILabel expLabel;
	/** 坐骑属性点 */
	public GameObject mountsAttrPoint;
	/** 技能组 */
	public GameObject skillGroups;
	public ButtonSkill[] skills;
	/** 父窗口 */
	WindowBase fatherWindow;

	/* fields */
	/** 坐骑 */
	Mounts mounts;

	/* methods */
	void Start () {
		buttonPower.onClickEvent = HandlePowerEvent;
		buttonUnsnatch.onClickEvent = HandleUnsnatchNEvent;
		buttonReplace.onClickEvent = HandleReplaceEvent;
		buttonChoose.onClickEvent = HandleChooseEvent;
		//执行坐骑功能说明
		if (GuideManager.Instance.isEqualStep (134003000)) {
			GuideManager.Instance.doGuide ();
			GuideManager.Instance.guideEvent ();
		}
	}
	/***/
	public void init(WindowBase fatherWindow) {
		this.fatherWindow=fatherWindow;
		this.mounts=MountsManagerment.Instance.getMountsInUse();
		initButton (fatherWindow);
		UpdateUI();
	}
	/** 初始化button */
	public void initButton(WindowBase fatherWindow) {
		buttonPower.fatherWindow=fatherWindow;
		buttonUnsnatch.fatherWindow=fatherWindow;
		buttonReplace.fatherWindow=fatherWindow;
		buttonChoose.fatherWindow = fatherWindow;
	}
	/** 更新UI */
	public void UpdateUI() {
		updateExpBar();
		updateMountsAttr();
		updateButton();
		updateSkill();
	}
	/// <summary>
	/// 经验条
	/// </summary>
	private void updateExpBar() {
		MountsManagerment manager=MountsManagerment.Instance;
		expLabel.text= EXPSampleManager.Instance.getExpBarShow (EXPSampleManager.SID_MOUNTS_EXP,manager.getMountsExp ());
		LevelupInfo levelupInfo=manager.createLevelupInfo();
		skillLevelText.text= "Lv."+MountsManagerment.Instance.getMountsLevel();
		skillExpBar.init (levelupInfo);
	}
	/** 更新技能 */
	private void updateSkill() {
		if(mounts==null) {
			skillGroups.SetActive(false);
		} else {
			updatePassiveSkill();
		}
	}
	/** 更新被动技能 */
	private void updatePassiveSkill() {
		skillGroups.SetActive(true);
		for(int i=0;i<skills.Length;i++){
			skills[i].gameObject.SetActive(false);
		}
		Skill[] mountSkills=mounts.getSkills();
		for(int j=0;j<mountSkills.Length;j++){
			skills[j].gameObject.SetActive(true);
			skills[j].initSkillData(mountSkills [j], ButtonSkill.STATE_CANLEARN);
			skills[j].fatherWindow=fatherWindow as MountsWindow;
		}
	}
	/** 更新属性显示对象 */
	private void updateMountsAttr() {
		if(mountsAttrPoint.transform.childCount>0)
			Utils.RemoveAllChild(mountsAttrPoint.transform);
		GameObject mountsAttrObj = NGUITools.AddChild (mountsAttrPoint, mountsAttrPrefab);
		MountsAttrItem mountsAttrItem=mountsAttrObj.GetComponent<MountsAttrItem>();
		mountsAttrItem.init(fatherWindow,mounts);
	}
	/** 更新button */
	private void updateButton() {
		if(mounts==null) {
			buttonUnsnatch.gameObject.SetActive(false);
			buttonReplace.gameObject.SetActive(false);
			buttonChoose.gameObject.SetActive(true);
		} else {
			buttonUnsnatch.gameObject.SetActive(true);
			buttonReplace.gameObject.SetActive(true);
			buttonChoose.gameObject.SetActive(false);
		}
	}
	/** 处理修炼事件 */
	private void HandlePowerEvent(GameObject gameObj) {
        MaskWindow.LockUI();
		UiManager.Instance.openWindow<MountsPracticeWindow>();
	}
	/** 处理休息事件 */
	private void HandleUnsnatchNEvent(GameObject gameObj) {
		// 与服务器通讯
		(FPortManager.Instance.getFPort ("MountsRideFPort") as MountsRideFPort).putOffMountsAccess (()=>{
			init(fatherWindow);
		});
		
		Mounts temp=MountsManagerment.Instance.getMountsInUse();
		if(temp!=null)
			temp.setState(false);
		init(fatherWindow);
	}
	/** 处理更换事件 */
	private void HandleReplaceEvent(GameObject gameObj) {
		if(fatherWindow is MountsWindow) {
			MountsWindow mountsWin=fatherWindow as MountsWindow;
			mountsWin.setButtonState(MountStoreContent.UN_SHOW_RIDE_MOUNT);
			mountsWin.changeTapPage(MountsWindow.TAP_STORE_CONTENT);
		}
	}
	/** 处理选择事件 */
	private void HandleChooseEvent(GameObject gameObj) {
		if(fatherWindow is MountsWindow) {
			//选择坐骑
			if (GuideManager.Instance.isEqualStep (134004000)) {
				GuideManager.Instance.doGuide ();
			}
			MountsWindow mountsWin=fatherWindow as MountsWindow;
			mountsWin.changeTapPage(MountsWindow.TAP_STORE_CONTENT);
		}
	}
	/** button点击事件 */
	public void buttonEventBase (GameObject gameObj) {
		MaskWindow.UnlockUI();
	}
}
