using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 星魂仓库容器
/// </summary>
public class StarSoulStoreContent : MonoBase {

	/* fields */
	/** 容器数量条目 */
	public GameObject contentNumItem;
	/** 容器数量标签 */
	public UILabel contentNumLabel;
	/** 勾选描述条目 */
	public GameObject chooseDescItem;
	/** 勾选描述 */
	public UILabel chooseDescLabel;
	/** 星魂显示节点容器 */
	public StarSoulItemContent content;
	/** 加锁保护按钮 */
	public ButtonBase buttonLocked;
	/** 确定加锁保护按钮 */
	public ButtonBase buttonConfirmLocked;
	/** 转化经验按钮 */
	public ButtonBase buttonExChangeExp;
	/** 返回按钮 */
	public ButtonBase buttonCancel;
	/** 确定转化经验阿牛按钮 */
	public ButtonBase buttonConfirmExchangeExp;
	/**一键转化按钮 */
	public ButtonBase buttonOneKeyExp;
	/** 排序条件 */
	public SortCondition sc ;
	/** 星魂仓库版本号 */
	int storageVersion = -1;
	/** 星魂仓库中获取出的星魂列表 */
	ArrayList starSoulList;
	/** 进入状态类型 */
	ButtonStoreStarSoul.ButtonStateType stateType;
	/** 首先剔除的状态 */
	private int moveTpye;
	/* methods */
	/** 初始化 */
	public void init(WindowBase fatherWindow,ButtonStoreStarSoul.ButtonStateType stateType) {
		this.stateType=stateType;
		content.fatherWindow = fatherWindow;
		initButton (fatherWindow);
		UpdateUI ();
        resetButton();
        
	}
   
	/** 初始化button */
	public void initButton(WindowBase fatherWindow) {
		if(buttonLocked!=null)buttonLocked.fatherWindow = fatherWindow;
		if(buttonExChangeExp!=null)buttonExChangeExp.fatherWindow = fatherWindow;
		if(buttonConfirmLocked!=null)buttonConfirmLocked.fatherWindow = fatherWindow;
		buttonConfirmExchangeExp.fatherWindow = fatherWindow;
		buttonCancel.fatherWindow = fatherWindow;
        if(buttonOneKeyExp!=null)buttonOneKeyExp.fatherWindow=fatherWindow;
	}
	/// <summary>
	/// 更新星魂仓库内容
	/// </summary>
	public void updateContent() {
		if (sc == null) { // 默认
			sc = SortConditionManagerment.Instance.getConditionsByKey (SiftWindowType.SIFT_STARSOULSTORE_WINDOW);
			moveTpye= EquipStateType.NO_REMOVE;
		}else{
			moveTpye= EquipStateType.OCCUPY;
		}
		starSoulList = StorageManagerment.Instance.getAllStarSoul ();
		starSoulList = SortManagerment.Instance.starSoulSort (starSoulList, sc,EquipStateType.INIT, moveTpye);
		starSoulList = SortManagerment.Instance.starSoulSplit(starSoulList,new int[4]{SortType.SPLIT_EQUIP_NEW,SortType.SPLIT_FREE_STATE,SortType.SPLIT_USING_STATE,SortType.SPLIT_EATEN});
		content.cleanAll ();
		sc=null;
		content.reLoad (starSoulList,stateType);
		contentNumLabel.text = starSoulList.Count + "/" + StorageManagerment.Instance.getStarSoulStorageMaxSpace ();
	}
	/** 更新UI */
	public void UpdateUI() {
		// 筛选改变刷新
		if (StorageManagerment.Instance.starSoulStorageVersion != storageVersion) {
			storageVersion = StorageManagerment.Instance.starSoulStorageVersion;
			updateContent ();
			//resetButton();
		} else {
            updateContent();
			content.updateVisibleItem();
		}
	}
	/** 更新Button */
	public void resetButton() {
		chooseDescItem.SetActive(false);
		contentNumItem.SetActive(true);
        if (buttonExChangeExp != null)buttonExChangeExp.gameObject.SetActive(true);
        if (buttonLocked != null)buttonLocked.gameObject.SetActive(true);
		buttonConfirmExchangeExp.gameObject.SetActive(false);
        if (buttonOneKeyExp!=null)buttonOneKeyExp.gameObject.SetActive(false);
		if(buttonConfirmLocked!=null)buttonConfirmLocked.gameObject.SetActive(false);
		buttonCancel.gameObject.SetActive(false);

	}
    /// <summary>
    /// 星魂魂库转化经验界面上的  返回 意见选择 确认  按钮更新操作
    /// </summary>
    public void updateButton()
    {
        StarSoulManager.Instance.clearChangeExpStateDic();
        contentNumItem.SetActive(false);
        chooseDescItem.SetActive(true);
        if (buttonExChangeExp != null) buttonExChangeExp.gameObject.SetActive(false);
        if (buttonLocked != null) buttonLocked.gameObject.SetActive(false);
        buttonConfirmExchangeExp.gameObject.SetActive(true);
        buttonOneKeyExp.gameObject.SetActive(true);
        buttonCancel.gameObject.SetActive(true);
        if (buttonConfirmLocked != null) buttonConfirmLocked.gameObject.SetActive(false);
    }


	/** button点击事件 */
	public void buttonEventBase (GameObject gameObj) {
		if (gameObj.name == "buttonLocked") {
			StarSoulManager.Instance.clearLockStateDic();
			contentNumItem.SetActive(false);
			chooseDescItem.SetActive(true);
			if(buttonExChangeExp!=null)buttonExChangeExp.gameObject.SetActive(false);
			if(buttonLocked!=null)buttonLocked.gameObject.SetActive(false);
			buttonConfirmExchangeExp.gameObject.SetActive(false);
			buttonOneKeyExp.gameObject.SetActive(false);
			buttonCancel.gameObject.SetActive(true);
			if(buttonConfirmLocked!=null)buttonConfirmLocked.gameObject.SetActive(true);
			chooseDescLabel.text=LanguageConfigManager.Instance.getLanguage("StarSoulWindow_storeContent_lockDesc");
			content.setIntoType(ButtonStoreStarSoul.ButtonStateType.Lock);
			content.updateVisibleItem();
			MaskWindow.UnlockUI ();
		} else if (gameObj.name == "buttonExChangeExp") {
			StarSoulManager.Instance.clearChangeExpStateDic();
			contentNumItem.SetActive(false);
			chooseDescItem.SetActive(true);
			if(buttonExChangeExp!=null)buttonExChangeExp.gameObject.SetActive(false);
			if(buttonLocked!=null)buttonLocked.gameObject.SetActive(false);
			buttonConfirmExchangeExp.gameObject.SetActive(true);
			buttonOneKeyExp.gameObject.SetActive(true);
			buttonCancel.gameObject.SetActive(true);
			if(buttonConfirmLocked!=null)buttonConfirmLocked.gameObject.SetActive(false);
			chooseDescLabel.text=LanguageConfigManager.Instance.getLanguage("StarSoulWindow_storeContent_changeDesc");
            sc = SortConditionManagerment.Instance.getConditionsByKey(SiftWindowType.SIFT_STARSOULSTORE_WINDOW);
            moveTpye = EquipStateType.OCCUPY;
            content.setIntoType(ButtonStoreStarSoul.ButtonStateType.CHANGE);
            stateType = ButtonStoreStarSoul.ButtonStateType.CHANGE;
            updateContent();
			MaskWindow.UnlockUI ();
		} else if (gameObj.name == "buttonConfirmLocked") {
			DoLocked();
		} else if (gameObj.name == "buttonConfirmExchangeExp") {
            DoExchangeExp();
			
		} else if(gameObj.name == "buttonCancel") {
			StarSoulManager.Instance.cleanDic();
			content.setIntoType(ButtonStoreStarSoul.ButtonStateType.Power);
            stateType = ButtonStoreStarSoul.ButtonStateType.Power;
			resetButton();
            updateContent();
            //UpdateUI();
			MaskWindow.UnlockUI ();
		}else if(gameObj.name=="buttonOneKeyExp"){
			if(GameManager.Instance.isShowStarSoulOneKeySelect){
				UiManager.Instance.openDialogWindow<StarSoulOneKeySelectWindow>((win)=>{
					win.callback=oneKeySelectCallback;
					win.init();
				});
			}else{
				oneKeySelectCallback();
				MaskWindow.UnlockUI ();
			}
		}
		else{
			MaskWindow.UnlockUI ();
		}
	}



	void oneKeySelectCallback(){
        //循环魂库里面所有的星魂，添加到转化经验列表里面
        for (int i = 0; i < starSoulList.Count; i++)
        {
            // 装备中或者锁定中转化经验不显示勾选框
            if ((starSoulList[i] as StarSoul).checkState(EquipStateType.OCCUPY) || (starSoulList[i] as StarSoul).checkState(EquipStateType.LOCKED))
                continue;
            else
            {
                if ((starSoulList[i] as StarSoul).getQualityId() <= GameManager.Instance.starSoulOneKeySelectValue)
                    StarSoulManager.Instance.setChangeExpState((starSoulList[i] as StarSoul).uid, true);
            }
        }
		content.reLoad (starSoulList);
	}



	/** 执行加锁保护 */
	private void DoLocked() {
		string str=StarSoulManager.Instance.getLockStateString ();
		if(string.IsNullOrEmpty(str)) {
			MaskWindow.UnlockUI ();
			return;
		}
		// 与服务器通讯
		(FPortManager.Instance.getFPort ("StarSoulFPort") as StarSoulFPort).doLockStarSoulAccess (str,()=>{
			content.setIntoType(ButtonStoreStarSoul.ButtonStateType.Power);
			resetButton();
			UpdateUI();
			UiManager.Instance.openDialogWindow<MessageLineWindow>((win)=>{
				win.Initialize (LanguageConfigManager.Instance.getLanguage ("StarSoulWindow_storeContent_lockMessage"));
			});
		});
	}
	/** 执行经验转化 */
	private void  DoExchangeExp() {
		StarSoulManager manager = StarSoulManager.Instance;
		string str=StarSoulManager.Instance.getExchangeExpStateString ();
		if(string.IsNullOrEmpty(str)) {
			MaskWindow.UnlockUI ();
			return;
		}
		bool isState=manager.isStarSoulStateByChangeDic (EquipStateType.LOCKED);
		if (isState) {
			UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
				win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0094"), null, LanguageConfigManager.Instance.getLanguage ("StarSoulWindow_storeContent_changeState"),null);
			});
			return;
		}
		bool isQuality=manager.isStarSoulQualityByChangeDic (QualityType.EPIC);
		if (isQuality) {
			UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
				win.initWindow (2, LanguageConfigManager.Instance.getLanguage ("s0094"), LanguageConfigManager.Instance.getLanguage ("s0093"), LanguageConfigManager.Instance.getLanguage ("StarSoulWindow_storeContent_changeQuality"), doExchangeQualityBack);
			});
			return;
		}
		MessageHandle msg = new MessageHandle ();
		msg.buttonID = MessageHandle.BUTTON_RIGHT;
		// 与服务器通讯
		doExchangeQualityBack (msg);
	}
	/** 执行经验转化品质提示回调 */
	private void doExchangeQualityBack(MessageHandle msg) {
		if (msg.buttonID == MessageHandle.BUTTON_RIGHT) {
			string str=StarSoulManager.Instance.getExchangeExpStateString ();
			(FPortManager.Instance.getFPort ("StarSoulExchangeFPort") as StarSoulExchangeFPort).exchangeAccess (str,(eatenExp,exchangeStarSouls)=>{
				content.setIntoType(ButtonStoreStarSoul.ButtonStateType.Power);
				//resetButton ();
                updateButton();
				UpdateUI();
				if(eatenExp>0) {
					UiManager.Instance.openDialogWindow<MessageLineWindow>((win)=>{
						win.Initialize (LanguageConfigManager.Instance.getLanguage ("StarSoulWindow_StarSoul_EatenExp", Convert.ToString(eatenExp)));
					});
				}
			});
		}
	}

	/* properties */
	/** 设置进入状态类型 */
	public void setStateType(ButtonStoreStarSoul.ButtonStateType stateType){
		this.stateType=stateType;
	}
}
