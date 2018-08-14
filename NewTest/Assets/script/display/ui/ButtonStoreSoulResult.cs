using UnityEngine;
using System.Collections;

/// <summary>
/// 星魂的容器条目按钮
/// 主要负责星魂强化,穿戴等功能
/// </summary>
public class ButtonStoreSoulResult : ButtonBase {

	/* fields */
	/**强化的星魂 */
	public StarSoul starSoul;
	/**临时保存的星魂飘属性描述 */
	public string tempStarSoul;
	/** 状态类型 */
	private ButtonStoreStarSoul.ButtonStateType stateType;
	/** 回调 */
	private CallBack callBack;

	/* methods */
	/// <summary>
	/// 更新星魂数据
	/// </summary>
	/// <param name="starSoul">星魂</param>
	/// <param name="stateType">显示状态类型</param>
	public void UpdateSoul (StarSoul starSoul,ButtonStoreStarSoul.ButtonStateType stateType) {
		this.starSoul = starSoul;
		this.stateType = stateType;
		UpdateUI ();
	}
	/** 更新UI */
	public void UpdateUI() {
		UpdateButtonShow ();
	}
	/** 更新button显示 */
	public void UpdateButtonShow() {
		switch (stateType) {
			case ButtonStoreStarSoul.ButtonStateType.Power:
				ShowPowerState();
				break;
			case ButtonStoreStarSoul.ButtonStateType.Replace:
				ShowReplaceState();
				break;
			case ButtonStoreStarSoul.ButtonStateType.PutOn:
				ShowPutOnState();
				break;
		default:				
				break;
		}
	}
	/** 显示强化状态 */
	private void ShowPowerState() {
		if (starSoul.isMaxLevel ()||starSoul.getStarSoulType()==0) { // 满级以及经验类星魂不显示强化按钮
			gameObject.SetActive (false);	
		} else {
			gameObject.SetActive (true);
			textLabel.text = LanguageConfigManager.Instance.getLanguage ("s0012");
		}
	}
	/** 显示替换状态 */
	private void ShowReplaceState() {
		if (starSoul.getStarSoulType()==0) { // 经验类星魂不显示替换按钮
			gameObject.SetActive (false);	
		} else {
			gameObject.SetActive (true);
			textLabel.text = LanguageConfigManager.Instance.getLanguage ("s0036");
		}
	}
	/** 显示穿星魂状态 */
	private void ShowPutOnState() {
		if (starSoul.getStarSoulType()==0) { //经验类星魂不显示装备按钮
			gameObject.SetActive (false);	
		} else {
			gameObject.SetActive (true);
			textLabel.text = LanguageConfigManager.Instance.getLanguage ("sell01");
			textLabel.gameObject.SetActive (true);
		}
	}
	/***/
	public override void DoClickEvent () {
		base.DoClickEvent ();
		if (stateType == ButtonStoreStarSoul.ButtonStateType.Power) { // 强化
			UiManager.Instance.openWindow<StarSoulStoreStrengWindow>((win)=>{
				win.init (starSoul);
			});
		} else if (stateType == ButtonStoreStarSoul.ButtonStateType.Replace) { // 替换
			StarSoulManager manager=StarSoulManager.Instance;
			Card activeCard=manager.getActiveCard();
			int activeBoreIndex=manager.getActiveBoreIndex();
			tempStarSoul=manager.getActiveStarSoulDese();
			if(activeCard==null||activeBoreIndex==-1) {
				MaskWindow.UnlockUI();
				return;
			}
			StarSoulBore starBore=activeCard.getStarSoulBoreByIndex(activeBoreIndex);
			StarSoul ss=null;
			if(starBore!=null){
				ss=StorageManagerment.Instance.getStarSoul(starBore.getUid());
			}

			StarSoulManager.Instance.setState(1);
			StarSoulManager.Instance.setActiveSoulStarInfo(activeCard.uid,starSoul.uid,activeBoreIndex);
			// 与服务器通讯
			(FPortManager.Instance.getFPort ("StarSoulEquipFPort") as StarSoulEquipFPort).putOnEquipStarSoulAccess (activeCard.uid,starSoul.uid,activeBoreIndex,()=>{
				putOnFinished();
			});
		} else if (stateType == ButtonStoreStarSoul.ButtonStateType.PutOn) { // 穿戴
			StarSoulManager manager=StarSoulManager.Instance;
			Card activeCard=manager.getActiveCard();
			int activeBoreIndex=manager.getActiveBoreIndex();
			tempStarSoul=manager.getActiveStarSoulDese();
			if(activeCard==null||activeBoreIndex==-1) {
				MaskWindow.UnlockUI();
				return;
			}
			StarSoulManager.Instance.setState(2);
			StarSoulManager.Instance.setActiveSoulStarInfo(activeCard.uid,starSoul.uid,activeBoreIndex);
			// 与服务器通讯
			(FPortManager.Instance.getFPort ("StarSoulEquipFPort") as StarSoulEquipFPort).putOnEquipStarSoulAccess (activeCard.uid,starSoul.uid,activeBoreIndex,()=>{
				putOnFinished();
			});
		}
	}
	/// <summary>
	/// 穿星魂完成后回调
	/// </summary>
	public void putOnFinished () {
		if (fatherWindow is StarSoulStoreAloneWindow) {
			StarSoulStoreAloneWindow win=fatherWindow as StarSoulStoreAloneWindow;
			win.finishWindow();
			EventDelegate.Add(win.OnHide,()=>{
				beginShowArr();
			});
		} else {
			MaskWindow.UnlockUI();
		}
	}
	/// <summary>
	/// 检查可以换指定的星魂不
	/// </summary>
	public bool checkPuton(Card card,StarSoul ss) {
		if(!starSoul.checkStarSoulCanbePut(card,ss)) {
			UiManager.Instance.openDialogWindow<MessageLineWindow> ((win) => {
				win.Initialize (LanguageConfigManager.Instance.getLanguage 
				                ("StarSoulStrengWindow_DecforUnPutOnn"),false);
			});
			return false;
		}
		return true;
	}
	/// <summary>
	/// 显示属性变化的飘字.
	/// </summary>
	public void beginShowArr() {
        string descAttr = "";
        string[] describeAttr = StarSoulManager.Instance.getStarSoulDese(starSoul).Split('#');
        if (describeAttr.Length > 1) {
            descAttr = describeAttr[0] + describeAttr[1];
        } else {
            descAttr = describeAttr[0];
        }
		if(tempStarSoul!=null) {
            if (tempStarSoul.Split('#').Length > 1) {
                tempStarSoul = tempStarSoul.Split('#')[0] + tempStarSoul.Split('#')[1];
            } else {
                tempStarSoul = tempStarSoul.Split('#')[0];
            }
			UiManager.Instance.openDialogWindow<MessageLineWindow> ((win) => {
				win.Initialize (LanguageConfigManager.Instance.getLanguage 
				                ("StarSoulStrengWindow_LOSE",tempStarSoul),false);
				win.Initialize (LanguageConfigManager.Instance.getLanguage
                                ("StarSoulStrengWindow_ADD") + descAttr, false);
			});
		}else{
			UiManager.Instance.openDialogWindow<MessageLineWindow> ((win) => {
				win.Initialize (LanguageConfigManager.Instance.getLanguage
                                ("StarSoulStrengWindow_ADD") + descAttr, false);
			});
		}
	}
}
