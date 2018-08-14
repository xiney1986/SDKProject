using UnityEngine;
using System.Collections;
using System;
using System.Reflection;

/// <summary>
/// 星魂强化
/// </summary>
public class MagicWeaponStrengWindow : WindowBase {

	/**field**/
    public UITexture icon;//秘宝形象
    public UISprite jobType;//职业类型
    public UISprite jobBg;//职业背景
    public UISprite icon_bg;//秘宝背景
    public UILabel magicName;//秘宝名字
    public UILabel strengLv;//秘宝强化等级
    public UILabel phaseLv; //秘宝进阶等级
    public UISprite[] sttrDec;//秘宝的属性描述
    public UILabel[] sttrVel;//秘宝的属性数值
    public ButtonBase putOnButton;//穿上按钮
    public ButtonBase putDownButton;//脱下按钮
    public ButtonBase strengButton;//强化按钮
    public ButtonBase phaseButton;//进阶按键
    public ButtonBase getWayButton;//获取途径按键
    public UITexture[] skillTexture;//技能图标
    public UILabel[] skillDec;//技能描述
    public GameObject[] skillPonts;//放技能的节点
    public GameObject[] sttrPonts;
    public UILabel[] addVel;//各属性加成显示
    public ButtonBase replaseButton;//替换按键
    public UILabel partNum;//碎片部件
    private MagicWeapon magicWeapon;//传入进来的秘宝
    public GameObject starPrefab;//星星

    private int type;
	/* methods */
	/** 激活window */
	protected override void begin () {
        base.begin();
        if(isAwakeformHide){
            if (type == MagicWeaponType.PUTON&&MagicWeaponManagerment.Instance.selectCard != null) {
                this.magicWeapon = StorageManagerment.Instance.getMagicWeapon(MagicWeaponManagerment.Instance.selectCard.magicWeaponUID);
                updateUI();
            } else if (type == MagicWeaponType.STRENG) {//从强化界面回来
                this.magicWeapon = StorageManagerment.Instance.getMagicWeapon(magicWeapon.uid);
                updateUI();
            } 
        }
        MaskWindow.UnlockUI();
	}

	/// <summary>
	/// 初始化秘宝强化界面
	/// </summary>
	public void init(MagicWeapon magicWeapon,int type) {
        this.magicWeapon = magicWeapon;
        this.type = type;
        updateUI();
	}
    public void init(MagicWeapon magicWeapon,ExchangeSample sample,int type){//最特别的 是从碎片兑换窗口过来的显示
        this.magicWeapon = magicWeapon;
        this.type = type;
        putDownButton.gameObject.SetActive(false);
        putOnButton.gameObject.SetActive(false);
        strengButton.gameObject.SetActive(false);
        phaseButton.gameObject.SetActive(false);
        replaseButton.gameObject.SetActive(false);
        partNum.gameObject.SetActive(true);
        partNum.text = LanguageConfigManager.Instance.getLanguage("magicweapLiii01", EquipScrapManagerment.Instance.getNumString(sample));
        getWayButton.gameObject.SetActive(true);//获取途径按键可用
        updateCommonInfo();
    }
    protected override void DoEnable() {
        UiManager.Instance.backGround.switchBackGround("ChouJiang_BeiJing");
    }
	/** 更新UI */
    public void updateUI() {
        if (type == MagicWeaponType.PUTON) {//点击穿戴进入页面
            putDownButton.gameObject.SetActive(true);
            putOnButton.gameObject.SetActive(false);
            replaseButton.gameObject.SetActive(true);
            updateCommonInfo();
        }else if(type==MagicWeaponType.STRENG){
            putDownButton.gameObject.SetActive(false);
            putOnButton.gameObject.SetActive(false);
            replaseButton.gameObject.SetActive(false);
            updateCommonInfo();
        } else if (type == MagicWeaponType.FORM_OTHER) {//什么事情也做不了就只能看的节奏
            putDownButton.gameObject.SetActive(false);
            putOnButton.gameObject.SetActive(false);
            strengButton.gameObject.SetActive(false);
            phaseButton.gameObject.SetActive(false);
            replaseButton.gameObject.SetActive(false);
            updateCommonInfo();
        }
    }
    /// <summary>
    /// 更新拥有形象(强化等级，进阶等级，形象，形象背景)
    /// </summary>
    private void updateCommonInfo() {
        if (magicWeapon.getPhaseLv() > 0) {
            phaseLv.gameObject.SetActive(true);
            phaseLv.text = "+" + magicWeapon.getPhaseLv().ToString();
        } else phaseLv.gameObject.SetActive(false);
        magicName.text = QualityManagerment.getQualityColor(magicWeapon.getMagicWeaponQuality()) + magicWeapon.getName();
        ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.ICONIMAGEPATH + magicWeapon.getIconId(),icon);
        strengLv.text = "Lv." + magicWeapon.getStrengLv().ToString() + "/" + magicWeapon.getMaxStrengLv().ToString();
        icon_bg.spriteName = QualityManagerment.qualityIDtoMagicWeapon(magicWeapon.getMagicWeaponQuality());
        if (starPrefab != null) {
            Utils.RemoveAllChild(icon.transform);
            int starLevel = MagicWeaponSampleManager.Instance.getMagicWeaponSampleBySid(magicWeapon.sid).starLevel;
            if (starLevel > 0) {
                ShowStars show = NGUITools.AddChild(icon.gameObject, starPrefab).GetComponent<ShowStars>();
                show.initStar(starLevel, MagicWeaponManagerment.USEDBUMAGIC_ATTRSHOW);
            }
        }
        jobType.gameObject.SetActive(true);
        jobBg.gameObject.SetActive(true);
        if (magicWeapon.getMgType() == JobType.POWER) {//力
            jobType.spriteName = "roleType_2s";
        } else if (magicWeapon.getMgType() == JobType.MAGIC) {//魔
            jobType.spriteName = "roleType_5s";
        } else if (magicWeapon.getMgType() == JobType.AGILE) {//敏
            jobType.spriteName = "roleType_3s";
        } else if (magicWeapon.getMgType() == JobType.POISON) {//毒
            jobType.spriteName = "roleType_4s";
        } else if (magicWeapon.getMgType() == JobType.COUNTER_ATTACK) {//反 
            jobType.spriteName = "roleType_1s";
        } else if (magicWeapon.getMgType() == JobType.ASSIST) {//辅
            jobType.spriteName = "roleType_6s";
        } else {
            jobType.gameObject.SetActive(false);
            jobBg.gameObject.SetActive(false);
        }
        updateAttrChanges();
        updateSkills();
    }
    /// <summary>
    /// 更新技能图标和描述
    /// </summary>
    private void updateSkills() {
        int[] skillSids = magicWeapon.skillSids;
        for (int j = 0; j < skillPonts.Length;j++ ) {
            skillPonts[j].SetActive(false);
        }
        for (int i = 0; i < skillSids.Length;i++ ) {
            SkillSample sk = SkillSampleManager.Instance.getSkillSampleBySid(skillSids[i]);
            if (sk != null) {
                skillPonts[i].SetActive(true);
                ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.SKILLIMAGEPATH+sk.iconId, skillTexture[i]);
                int lv=magicWeapon.getPhaseLv();
                if(lv>i)skillDec[i].text = "[3A9663]"+sk.describe;
                else if (lv == i) skillDec[i].text = "[724C41]" + sk.describe + "[-]" + "[FF0000]" + LanguageConfigManager.Instance.getLanguage("equipNamelv01");
                else skillDec[i].text = "[724C41]" + sk.describe +"[-]"+"[FF0000]"+ LanguageConfigManager.Instance.getLanguage("equipNamelv02");

            } else {
                BuffSample bs = BuffSampleManager.Instance.getBuffSampleBySid(skillSids[i]);
                if (bs != null) {
                    skillPonts[i].SetActive(true);
                    ResourcesManager.Instance.LoadAssetBundleTexture(BuffManagerment.Instance.getSkillIconPath(skillSids[i]), skillTexture[i]);
                    int lv = magicWeapon.getPhaseLv();
                    if (lv > i) skillDec[i].text = "[3A9663]" + bs.name;
                    else if (lv == i) skillDec[i].text = "[724C41]" + bs.name + "[-]" + "[FF0000]" + LanguageConfigManager.Instance.getLanguage("equipNamelv01");
                    else skillDec[i].text = "[724C41]" + bs.name + "[-]" + "[FF0000]" + LanguageConfigManager.Instance.getLanguage("equipNamelv02");
                }
            }
        }
    }
    /// <summary>
    /// 更新属性
    /// </summary>
    private void updateAttrChanges(){
        AttrChange[] attrs = magicWeapon.getAttrChanges();
        for (int n = 0; n < sttrPonts.Length; n++) {
            sttrPonts[n].SetActive(false);
        }
        int[] index = new int[attrs.Length];
        if (attrs.Length == 1) index[0] = 1;
        else  if (attrs.Length == 2) {
            index[0] = 1;
            index[1] = 2;
        } else if (attrs.Length == 3) {
            index[0] = 0;
            index[1] = 1;
            index[2] = 2;
        } else if (attrs.Length == 4) {
            index[0] = 0;
            index[1] = 1;
            index[2] = 2;
            index[3] = 3;
        }
        for (int j = 0; j < index.Length; j++) {
            for (int m = 0; m < sttrPonts.Length;m++ ) {
                if (m == index[j]) {
                    sttrPonts[m].SetActive(true);
                    sttrDec[m].spriteName = updateSprieName(attrs[j]);
                    sttrVel[m].text = attrs[j].num.ToString();
                }
            }
        }
    }
    private string updateSprieName(AttrChange ac) {
        switch (ac.type){
        case AttrChangeType.HP:
			return "attr_hp";
			
		case AttrChangeType.ATTACK:
			return "attr_attack";

		case AttrChangeType.DEFENSE:
			return "attr_defense";


		case AttrChangeType.MAGIC:
			return "attr_magic";

		case AttrChangeType.AGILE:
			return "attr_agile";
		
		}
        return "";
    }
    //一直更新进阶状态
    void Update() {
        if (phaseLv != null && phaseLv.gameObject != null && phaseLv.gameObject.activeSelf)
            phaseLv.alpha = sin();
    }
	/// <summary>
	/// 经验条飞
	/// </summary>

	/// <summary>
	/// 更新星魂形象
	/// </summary>


	/** 点击事件 */
	public override void buttonEventBase (GameObject gameObj) {
		base.buttonEventBase (gameObj);
        if(gameObj.name=="close")
        finishWindow();
        else if (gameObj.name == "StrengButton") {
            //if (magicWeapon.getPhaseLv() >= magicWeapon.getMaxPhaseLv()) {
            //    UiManager.Instance.openDialogWindow<MessageLineWindow>((win) => {
            //        win.Initialize(LanguageConfigManager.Instance.getLanguage("magicweapLiii08"));
            //    });
            //    MaskWindow.UnlockUI();
            //    return;
            //}
            UiManager.Instance.openWindow<MagicWeaponReStrengWindow>((win) => {
                win.init(magicWeapon, MagicWeaponType.STRENGG);
            });
        } else if (gameObj.name == "phaseButton") {
            //if(magicWeapon.getPhaseLv()>=magicWeapon.getMaxPhaseLv()){
            //    UiManager.Instance.openDialogWindow<MessageLineWindow>((win) => {
            //        win.Initialize(LanguageConfigManager.Instance.getLanguage("magicweapLiii09"));
            //    });
            //    MaskWindow.UnlockUI();
            //    return;
            //}
            UiManager.Instance.openWindow<MagicWeaponPhaseWindow>((win) => {
                win.init(magicWeapon, MagicWeaponType.PHASE);
            });
        } else if (gameObj.name == "ReplaseButton") {//没有合适的就不能替换了
            Card tempCard = MagicWeaponManagerment.Instance.selectCard;
            if (StorageManagerment.Instance.getAllMagicWeaponByType(tempCard.getJob(), tempCard.magicWeaponUID) == null) {
                UiManager.Instance.openDialogWindow<MessageLineWindow>((win) => {
                    win.Initialize(LanguageConfigManager.Instance.getLanguage("not_have_suit_put_on"));
                });
                MaskWindow.UnlockUI();
                return;
            }
            UiManager.Instance.openWindow<MagicWeapStoreWindow>((win) => {
                win.init(tempCard, MagicWeaponType.FROM_CARD_BOOK_HAVE_M);//装备秘宝模式（需要打开独立的秘宝仓库界面）
            });
        }else if(gameObj.name=="PutDownButton"){
            Card tempCard = MagicWeaponManagerment.Instance.selectCard;
            MagicWeaponPutOnFPort exf = FPortManager.Instance.getFPort("MagicWeaponPutOnFPort") as MagicWeaponPutOnFPort;
            exf.exchange("0", tempCard.uid, complatePutDown);
        } else if (gameObj.name == "getInfo") {//获取途径 指向爬塔
            //进入爬塔界面
            FuBenGetCurrentFPort port = FPortManager.Instance.getFPort("FuBenGetCurrentFPort") as FuBenGetCurrentFPort;
            port.getInfo(getContinueMission);
            
        }

	}
    void getContinueMission(bool b) {
        if (b) {
            UiManager.Instance.openDialogWindow<MessageLineWindow>((win) => {
                win.Initialize(LanguageConfigManager.Instance.getLanguage("towerShowWindow44"));
            });
        } else {
            FuBenInfoFPort port = FPortManager.Instance.getFPort("FuBenInfoFPort") as FuBenInfoFPort;
            port.info(intoTowerFuben, ChapterType.TOWER_FUBEN);
        }
    }
    void complatePutDown() {
        StorageManagerment.Instance.getRole(MagicWeaponManagerment.Instance.selectCard.uid).magicWeaponUID = "";
        magicWeapon.state = 0;
        finishWindow();
    }
    /// <summary>
    /// 进入爬塔界面
    /// </summary>
    private void intoTowerFuben() {
        //添加过程记录
        if (FuBenManagerment.Instance.getTowerChapter() == null) return;
        FuBenManagerment.Instance.selectedChapterSid = FuBenManagerment.Instance.getTowerChapter().sid;//爬塔副本章节sid
        FuBenManagerment.Instance.selectedMapSid = 1;
        UiManager.Instance.openWindow<ClmbTowerChooseWindow>();
    }
	


}
