using UnityEngine;
using System.Collections;

/// <summary>
/// 坐骑信息窗口
/// </summary>
public class MountShowWindow : WindowBase {

	/* gameobj fields */
	/** 坐骑属性点 */
	public GameObject mountsAttr;
	/**坐骑技能点 */
	public ButtonSkill[] buttonSkill;
	/**坐骑骑乘和休息按钮显示label */
	public UILabel buttonLabel;
	public ButtonBase rideButton;
	/** 坐骑 */
	Mounts mounts;
	/** */
	public int stateType=0;

	/* methods */
	/***/
	protected override void DoEnable () {
		base.DoEnable ();
		UiManager.Instance.backGround.switchBackGround("ChouJiang_BeiJing");
	}
	/** 断线重链 */
	public override void OnNetResume () {
		base.OnNetResume ();
		UpdateUI();
	}
	/** 初始化 */
	public void init (int mountSid, int type) {
		mounts = MountsManagerment.Instance.getMountsBySid (mountSid);
		stateType = type;
		if (mounts == null)
			mounts = MountsManagerment.Instance.createMounts (mountSid);
		setTitle (mounts.getName ());
		UpdateUI();
	}
	/** begin */
	protected override void begin () {
		base.begin ();
		MaskWindow.UnlockUI ();
	}
	/** 更新UI */
	public void UpdateUI(){
		updateMountsAttr();
		updatePassiveSkill();
		updateButtonState();
	}
	/**更新button状态 */
	private void updateButtonState(){
		switch(stateType){
		case MountStoreItem.IS_CAN_RIDE:
			buttonLabel.text=LanguageConfigManager.Instance.getLanguage("mount_button1");
			rideButton.gameObject.SetActive(true);
			break;
		case MountStoreItem.IS_CAN_STOP:
			buttonLabel.text=LanguageConfigManager.Instance.getLanguage("mount_stop");
			rideButton.gameObject.SetActive(true);
			break;
		case MountStoreItem.IS_CAN_ACTIVE:
			buttonLabel.text=LanguageConfigManager.Instance.getLanguage("goddess07");
			rideButton.gameObject.SetActive(true);
			break;
		case MountStoreItem.IS_CAN_UNACTIVE:
			buttonLabel.text=LanguageConfigManager.Instance.getLanguage("goddess07");
			rideButton.gameObject.SetActive(false);
			break;
		default:
			break;
		}
	}
	/** 更新属性显示对象 */
	private void updateMountsAttr() {
		MountsAttrItem mountsAttrItem=mountsAttr.GetComponent<MountsAttrItem>();
		mountsAttrItem.init(fatherWindow,mounts);
	}
	/** 更新被动技能 */
	private void updatePassiveSkill() {
		for(int j=0;j<buttonSkill.Length;j++){
			buttonSkill[j].gameObject.SetActive(false);
		}
		if(mounts!=null) {
			Skill[] mainSkills=mounts.getSkills();
			if(mainSkills!=null){
				for(int i=0;i<mainSkills.Length;i++){
					buttonSkill[i].gameObject.SetActive(true);
					buttonSkill[i].initSkillData (mainSkills [i], ButtonSkill.STATE_LEARNED);
				}
			}
		}
	}
	/** 点击事件 */
	public override void buttonEventBase (GameObject gameObj) {
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
			finishWindow ();
		} else if(gameObj.name=="buttonChoo") {
			if(stateType==MountStoreItem.IS_CAN_RIDE){
				// 与服务器通讯--骑乘
				(FPortManager.Instance.getFPort ("MountsRideFPort") as MountsRideFPort).putOnMountsAccess (mounts.uid, ()=>{
					if(fatherWindow is MountsWindow ){
						(fatherWindow as MountsWindow).isAwakeformHide=false;
						(fatherWindow as MountsWindow).init(MountsWindow.TAP_STORE_CONTENT);
					}
					finishWindow();
				});
			}else if(stateType==MountStoreItem.IS_CAN_STOP){
				// 与服务器通讯--休息
				(FPortManager.Instance.getFPort ("MountsRideFPort") as MountsRideFPort).putOffMountsAccess (()=>{
					if(fatherWindow is MountsWindow ){
						(fatherWindow as MountsWindow).isAwakeformHide=false;
						(fatherWindow as MountsWindow).init(MountsWindow.TAP_STORE_CONTENT);
					}
					finishWindow();
				});
			}else if(stateType==MountStoreItem.IS_CAN_ACTIVE){
				// 与服务器通讯--激活
				(FPortManager.Instance.getFPort ("MountsRideFPort") as MountsRideFPort).activeMountsAccess (mounts.sid, ()=>{
					UiManager.Instance.openDialogWindow<MessageLineWindow>((win) =>{
						win.Initialize(LanguageConfigManager.Instance.getLanguage("mount_active_success"));
					});
					if(fatherWindow is MountsWindow ){
						(fatherWindow as MountsWindow).isAwakeformHide=false;
						(fatherWindow as MountsWindow).init(MountsWindow.TAP_STORE_CONTENT);
					}
					finishWindow();
				});
			}
		}
	}
}