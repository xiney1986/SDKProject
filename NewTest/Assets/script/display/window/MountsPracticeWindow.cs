using UnityEngine;
using System.Collections;

/// <summary>
/// 坐骑修炼窗口
/// </summary>
public class MountsPracticeWindow :  WindowBase
{

	/* gameobj fields */
    public UITexture bgg;
	/** 骑术书1-6 */
	public GoodsView[] items = new GoodsView[6];
	/** 经验条 */
	public ExpbarCtrl expbar;
	/** 等级 */
	public UILabel labelLevel;
	/** 强化按钮 */
	public ButtonBase intensifyButton;
	/** 一键强化按钮 */
	public ButtonBase oneKeyIntensifyButton;
	/** 经验标签 */
	public UILabel expLabel;

	/* fields */
	/** 原始经验 */
	private long oldExp;
	/** 新获取经验 */
	private long newExp;
	/** 当前等级 */
	private int nowLv;
	/**骑术共鸣限制条件 */
	public UILabel attrPerLimitLabel;
	/**骑术行动力限制条件 */
	public UILabel attrStoreLimitLabel;
	/**骑术共鸣属性 */
	public UILabel attrPerLabel;
	/**行动力bar */
	/** 存储行动力经验条 */
	public barCtrl pveStoreBar;
	/** 存储行动力值 */
	public UILabel pveStoreValue;
    //**技能按钮*/ 点击弹出说明窗口
    public ButtonBase storePveButton;//存储行动力按钮
    public ButtonBase mountsSkillButton;//坐骑技能按钮
    private Timer timer;

	/* methods */
	protected override void begin () {
		base.begin ();
        ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.OTHER_TEXTURE + "bgCircle", bgg);
		updateUI ();
		MaskWindow.UnlockUI ();
	}
	/** 更新UI */
	public void updateUI () {
        timer = TimerManager.Instance.getTimer(UserManager.TIMER_DELAY);
        timer.addOnTimer(refreshData);
        timer.start();
		UpdateCommon();
		UpdateExpbar ();
		UpdateSkill();
	}
    void refreshData() {
        //若主界面看不到，不做如下的更新
        if (this == null || !gameObject.activeInHierarchy) {
            if (timer != null) {
                timer.stop();
                timer = null;
            }
            return;
        }
        if (pveStoreBar.gameObject.activeInHierarchy) {
            pveStoreBar.updateValue (UserManager.Instance.self.getStorePvEPoint(),UserManager.Instance.self.getStorePvEPointMax());
			pveStoreValue.text = UserManager.Instance.self.getStorePvEPoint() + "/" + UserManager.Instance.self.getStorePvEPointMax()+"("+LanguageConfigManager.Instance.getLanguage("s0572")+")";
        }

    }
	/** 更新除经验条以外的数据 */
	private void UpdateCommon() {
		initProp ();
		MountsManagerment manager = MountsManagerment.Instance;
		oldExp = manager.getMountsExp ();
		nowLv = expToLevel (oldExp);
		labelLevel.text = "Lv." + nowLv + "/" + EXPSampleManager.Instance.getMaxLevel (EXPSampleManager.SID_MOUNTS_EXP);
		expLabel.text = EXPSampleManager.Instance.getExpBarShow (EXPSampleManager.SID_MOUNTS_EXP, manager.getMountsExp ());
	}
	/** 更新经验条 */
	private void UpdateExpbar () {
		expbar.init (initExp (oldExp, oldExp));
	}
	/** 初始化物品 */
	private void initProp () {
		resetItems ();
		Prop tmpProp = null;
		StorageManagerment instance = StorageManagerment.Instance;
		MountsConfigManager config = MountsConfigManager.Instance;
		int[] itemSids = config.getItemSids ();
		for (int i = 0; i < itemSids.Length; i++) {
			items [i].gameObject.SetActive (true);
			items [i].fatherWindow=this;
			tmpProp = instance.getProp (itemSids [i]);
			if (tmpProp != null) {
				items [i].init (tmpProp, tmpProp.getNum ());
			} else {
				tmpProp = new Prop (itemSids [i], 0);
				items [i].init (tmpProp);
			}
			items [i].rightBottomText.gameObject.SetActive (true);
		}
	}
	/** 重置道具图标显示 */
	void resetItems () {
		for (int i = 0; i < items.Length; i++) {
			items [i].gameObject.SetActive (false);
		}
	}
	/** 校验道具 */
	bool checkProp () {
		Prop tmpProp = null;
		StorageManagerment instance = StorageManagerment.Instance;
		MountsConfigManager config = MountsConfigManager.Instance;
		int[] itemSids = config.getItemSids ();
		for (int i = 0; i < itemSids.Length; i++) {
			tmpProp = instance.getProp (itemSids [i]);			
			if (tmpProp != null && tmpProp.getNum () > 0&&tmpProp.getUseLv()<=UserManager.Instance.self.getUserLevel()) {
				return true;
			}
		}
		return false;
	}
	/** 校验满级 */
	bool checkIsMaxLevel () {
		int maxLv = EXPSampleManager.Instance.getMaxLevel (EXPSampleManager.SID_MOUNTS_EXP);
		if (nowLv >= maxLv)
			return true;
		else
			return false;
	}
	/** 强化数据后调用 */
	void updateInfo () {
		newExp = MountsManagerment.Instance.getMountsExp ();
		expbar.init (initExp (oldExp, newExp));
		expbar.setLevelUpCallBack (showLevelupEffect);
		expbar.endCall=()=>{
			UpdateCommon ();
			MaskWindow.UnlockUI();
		};
	}
	/** 经验条回调 */
	void showLevelupEffect (int now) {
		nowLv += 1;
		labelLevel.text = "Lv." + nowLv + "/" + EXPSampleManager.Instance.getMaxLevel (EXPSampleManager.SID_MOUNTS_EXP);
		UpdateSkill();
	}
	/** 初始化经验条信息 */
	LevelupInfo initExp (long _oldExp, long _newExp) {
		LevelupInfo lvinfo = new LevelupInfo ();
		lvinfo.newExp = _newExp;
		lvinfo.newExpDown = MountsManagerment.Instance.getEXPDown (expToLevel (_newExp));
		lvinfo.newExpUp = MountsManagerment.Instance.getEXPUp (expToLevel (_newExp));
		lvinfo.newLevel = expToLevel (_newExp);
		lvinfo.oldExp = _oldExp;
		lvinfo.oldExpDown = MountsManagerment.Instance.getEXPDown (expToLevel (_oldExp));
		lvinfo.oldExpUp = MountsManagerment.Instance.getEXPUp (expToLevel (_oldExp));
		lvinfo.oldLevel = expToLevel (_oldExp);
		lvinfo.orgData = null;
		return lvinfo;
	}
	/** 经验对应的等级 */
	int expToLevel (long _exp) {
		return EXPSampleManager.Instance.getLevel (EXPSampleManager.SID_MOUNTS_EXP, _exp);
	}
	/** 去商店 */
	void gotoShop (MessageHandle msg) {
		if (msg.buttonID == MessageHandle.BUTTON_RIGHT) {
			UiManager.Instance.openWindow<ShopWindow> ((ShopWindow win) => {
				win.setCallBack (null);
			});
		}
	}
	/**更新技能 */
	void UpdateSkill(){
		MountsConfigManager config=MountsConfigManager.Instance;
		MountsManagerment manager= MountsManagerment.Instance;
		if(manager.getMountsLevel()<config.getAttrPerOpenLv()) {
			attrPerLimitLabel.gameObject.SetActive(true);
			attrPerLabel.gameObject.SetActive(true);
			attrPerLimitLabel.text=LanguageConfigManager.Instance.getLanguage("mount_open_lifeskill_desc",config.getAttrPerOpenLv().ToString());
		} else {
			attrPerLimitLabel.gameObject.SetActive(false);
			attrPerLabel.gameObject.SetActive(true);
			attrPerLabel.text=LanguageConfigManager.Instance.getLanguage("mount_addAttrValue",config.getAttrPerByString());
		}
		if(manager.getMountsLevel()<config.getAddPveOpenLv()){
			attrStoreLimitLabel.gameObject.SetActive(true);
			attrStoreLimitLabel.text=LanguageConfigManager.Instance.getLanguage("mount_open_lifeskill_desc",config.getAddPveOpenLv().ToString());
			pveStoreBar.gameObject.SetActive(false);
		}else{
			attrStoreLimitLabel.gameObject.SetActive(false);
			pveStoreBar.gameObject.SetActive(true);
			pveStoreBar.updateValue (UserManager.Instance.self.getStorePvEPoint(),UserManager.Instance.self.getStorePvEPointMax());
			pveStoreValue.text = UserManager.Instance.self.getStorePvEPoint() + "/" + UserManager.Instance.self.getStorePvEPointMax()+"("+LanguageConfigManager.Instance.getLanguage("s0572")+")";
		}
	}
	/** 点击事件 */
	public override void buttonEventBase (GameObject gameObj) {
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
			finishWindow ();
		} else if (gameObj.name == "intensifyButton") {
			if (checkIsMaxLevel ()) {
				MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("mounttp_maxLv"));
				return;
			}
			else if (!checkProp ()) {
				UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
					win.initWindow (2, LanguageConfigManager.Instance.getLanguage ("s0040"), LanguageConfigManager.Instance.getLanguage ("s0014"),
					                LanguageConfigManager.Instance.getLanguage ("mounttp_noProp"), gotoShop);
				});
				return;
			}
			// 与服务器通讯
			(FPortManager.Instance.getFPort ("MountsPracticeFPort") as MountsPracticeFPort).powerAccess (1, ()=>{
				updateInfo();
			});
		} else if (gameObj.name == "oneKeyIntensifyButton") {
			if (checkIsMaxLevel ()) {
				MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("mounttp_maxLv"));
				return;
			}
			else if (!checkProp ()) {
				UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
					win.initWindow (2, LanguageConfigManager.Instance.getLanguage ("s0040"), LanguageConfigManager.Instance.getLanguage ("s0014"),
					                LanguageConfigManager.Instance.getLanguage ("mounttp_noProp"), gotoShop);
				});
				return;
			}
			// 与服务器通讯
			(FPortManager.Instance.getFPort ("MountsPracticeFPort") as MountsPracticeFPort).powerAccess (2, ()=>{
				updateInfo();
			});
        }
        else if (gameObj.name == "store") {
            UiManager.Instance.openDialogWindow<SkillInfoWindow>((win) => {
                string name = LanguageConfigManager.Instance.getLanguage("pve_store_power");
                string des = LanguageConfigManager.Instance.getLanguage("pve_link_l") + LanguageConfigManager.Instance.getLanguage("mount_pve_store_skillDesc");
                string iconPath = ResourcesManager.SKILLIMAGEPATH + "storeActive";
                win.Initialize(name,des,iconPath);
            });
        }
        else if (gameObj.name == "gongming") {
            UiManager.Instance.openDialogWindow<SkillInfoWindow>((win) =>
            {
                string name = LanguageConfigManager.Instance.getLanguage("mount_combin_skillName");
                string des = LanguageConfigManager.Instance.getLanguage("mount_combine_des");
                string iconPath = ResourcesManager.SKILLIMAGEPATH + "gongmin";
                win.Initialize(name, des, iconPath);
            });
        }
	}
}