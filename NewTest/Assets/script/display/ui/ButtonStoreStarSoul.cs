using UnityEngine;
using System.Collections;

/// <summary>
/// 星魂仓库按钮
/// </summary>
public class ButtonStoreStarSoul : ButtonBase {

	/* enum */
	/** 属性窗口类型 */
	public enum ButtonStateType {
		Power, // 强化
		Replace, // 替换
		PutOn, // 穿上
		Lock, // 锁
		CHANGE, // 转化经验
	}

	/* fields */
	/** 预制件 */
	public GameObject goodsViewPrefab;
	/** 星魂 */
	public StarSoul starSoul;
	/** 星魂等级标签 */
	public UILabel starSoulLev;
	/** 当前星魂状态 */
	public UILabel state;
	/** 星魂仓库功能按钮  */
	public ButtonStoreSoulResult storeButton;
	/** 星魂仓库功能复选框  */
	public UIToggle storeChoose;
	/** 星魂视图点 */
	public GameObject goodsViewPoint;
	/**星魂属性描述 */
	public UILabel infoLabel;
    public UILabel infoLabel1;
	/** 状态 */
	ButtonStateType stateType;
	private bool isSelect=false;
 

	/* methods */
	/// <summary>
	/// 更新星魂仓库列表.
	/// </summary>
	public void UpdateSoul(StarSoul starSoul,ButtonStateType type,bool isAutoSelect) {
		this.stateType = type;
		this.starSoul=starSoul;
		isSelect=isAutoSelect;
		storeButton.setFatherWindow(fatherWindow);
		UpdateIU ();
	}
	/** 更新button */
	public void UpdateButton() {
		if (stateType == ButtonStateType.Lock) 
        {
			storeButton.gameObject.SetActive(false);
			storeChoose.gameObject.SetActive(true);
			StarSoulManager manager=StarSoulManager.Instance;
			bool isExist=manager.checkLockState(starSoul.uid);
			if(isExist) {
				storeChoose.value=manager.getLockState(starSoul.uid);
			} else {
				storeChoose.value=starSoul.checkState(EquipStateType.LOCKED);
			}
		} 
        else if(stateType == ButtonStateType.CHANGE) 
        { 
			if(starSoul.checkState(EquipStateType.OCCUPY)||starSoul.checkState(EquipStateType.LOCKED)) { // 装备中或者锁定中转化经验不显示勾选框
				storeButton.gameObject.SetActive(false);
				storeChoose.gameObject.SetActive(false);
				storeChoose.value=false;
			} else {
				StarSoulManager manager=StarSoulManager.Instance;
				storeButton.gameObject.SetActive(false);
				storeChoose.gameObject.SetActive(true);
				if(isSelect){
					if(starSoul.getQualityId()<=GameManager.Instance.starSoulOneKeySelectValue){
						StarSoulManager.Instance.setChangeExpState(starSoul.uid,true);
					}
				}
                if (starSoul.getStarSoulType() == 0) {
                    StarSoulManager.Instance.setChangeExpState(starSoul.uid, true);
                }
				storeChoose.value=manager.getChangeExpState(starSoul.uid);
			}
		} 
        else 
        {
			storeButton.gameObject.SetActive(true);
			storeChoose.gameObject.SetActive(false);
			storeButton.UpdateSoul(starSoul,stateType);
		}
	}
	/** 更新UI */
	public void UpdateIU() {
		UpdateButton ();
		GameObject obj;
		if (goodsViewPoint.transform.childCount > 0) {
			obj = goodsViewPoint.transform.GetChild(0).gameObject;
		}
		else {
			obj = NGUITools.AddChild(goodsViewPoint,goodsViewPrefab);
			obj.transform.localScale=new Vector3(0.85f,0.85f,1);
		}
		GoodsView gv = obj.GetComponent<GoodsView>();
		gv.setFatherWindow (fatherWindow);
		gv.init(starSoul);
		//starSoulLev.text = QualityManagerment.getQualityColor(starSoul.getQualityId()) + starSoul.getName () + "  Lv." + starSoul.getLevel ();
        //清空属性内容
        infoLabel.text = "";
        infoLabel1.text = "";
        starSoulLev.text = starSoul.getName() + "  Lv." + starSoul.getLevel();
        string[] str = starSoul.getDescribe().Split('#');
        if (str.Length > 1) {
            infoLabel.text = "[A65644]" + str[0].Replace("+", "[3A9663]+");
            infoLabel1.text = "[A65644]" + str[1].Replace("+", "[3A9663]+");
        } else {
            infoLabel1.text = "[A65644]" + str[0].Replace("+", "[3A9663]+");
        }
		bool isPutOn=starSoul.checkState (EquipStateType.OCCUPY);
		if(isPutOn)
			state.text = LanguageConfigManager.Instance.getLanguage ("s0017");
		else{
			state.text = "";
		}
	}
	/** 改变Toggle */
	public void ChangeToggleValue() {
		if (stateType == ButtonStateType.Lock) {
			StarSoulManager.Instance.setLockState(starSoul.uid,storeChoose.value);
		} else if(stateType == ButtonStateType.CHANGE) {
			StarSoulManager.Instance.setChangeExpState(starSoul.uid,storeChoose.value);
		}
	}
}
