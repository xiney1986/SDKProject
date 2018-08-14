using UnityEngine;
using System.Collections;
using System;
using System.Reflection;

/// <summary>
/// 星魂强化
/// </summary>
public class MagicWeaponReStrengWindow : WindowBase {

	/**field**/
    public UITexture icon;//秘宝形象
    public UISprite icon_bg;//秘宝背景
    public UISprite jobType;//职业
    public UISprite jobBg;//职业背景
    public UILabel magicName;//秘宝名字
    public UILabel strengLv;//秘宝强化等级
    public UILabel phaseLv; //秘宝进阶等级
    public UISprite[] sttrDec;//秘宝的属性描述
    public UILabel[] sttrVel;//秘宝的属性数值
    public ButtonBase putOnButton;//穿上按钮
    public ButtonBase putDownButton;//脱下按钮
    public ButtonBase strengButton;//强化按钮
    public ButtonBase phaseButton;//进阶按键
    public UITexture[] skillTexture;//技能图标
    public UILabel[] skillDec;//技能描述
    public GameObject[] skillPonts;//放技能的节点
    public UILabel[] addVel;//各属性加成显示
    public GameObject[] sttrPonts;
    public GoodsView goodsView;
    public UILabel needNum;//需要的道具数量
    public GameObject maxObj;
    public GameObject needProObj;
    private MagicWeapon magicWeapon;//传入进来的秘宝
    public GameObject effectPoint;
    public GameObject starPrefab;//星星
    private int type;
	/* methods */
	/** 激活window */
	protected override void begin () {
		base.begin ();
		MaskWindow.UnlockUI ();
	}
    public override void OnNetResume() {
        base.OnNetResume();
        this.magicWeapon=StorageManagerment.Instance.getMagicWeapon(magicWeapon.uid);
        updateUI();
    }

	/// <summary>
	/// 初始化秘宝强化界面
	/// </summary>
	public void init(MagicWeapon magicWeapon,int type) {
        this.magicWeapon = magicWeapon;
        this.type = type;
		updateUI ();
	}
	/** 更新UI */
    public void updateUI() {
        if (type == MagicWeaponType.STRENGG) {//点击穿戴进入页面
            updateCommonInfo();
        }else if(type==MagicWeaponType.PHASE){
            updateCommonInfo();
        }
    }
    /// <summary>
    /// 更新拥有形象(强化等级，进阶等级，形象，形象背景)
    /// </summary>
    private void updateCommonInfo() {
        if (magicWeapon.getStrengLv() >= magicWeapon.getMaxStrengLv()) {
            strengButton.disableButton(false);
            maxObj.SetActive(true);
            needProObj.SetActive(false);
        } else {
            maxObj.SetActive(false);
            needProObj.SetActive(true);
        }
        if (magicWeapon.getPhaseLv() > 0) {
            phaseLv.gameObject.SetActive(true);
            phaseLv.text = "+" + magicWeapon.getPhaseLv().ToString();
        } else phaseLv.gameObject.SetActive(false);
        magicName.text = QualityManagerment.getQualityColor(magicWeapon.getMagicWeaponQuality()) + magicWeapon.getName();
        ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.ICONIMAGEPATH + magicWeapon.getIconId(), icon);
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
        //职业显示
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
        updateProp();
       
    }
    private void updateProp() {
        if (type == MagicWeaponType.STRENGG) {
            string[][] props = CommandConfigManager.Instance.getMagicWeaponStrengProp();
            int quaIndex = magicWeapon.getMaxPhaseLv();
            string[] selectPropList = props[quaIndex - 1];
            string selsecNum = magicWeapon.getStrengLv() >= (selectPropList.Length) ? selectPropList[selectPropList.Length - 1] : selectPropList[magicWeapon.getStrengLv()];
            Prop p = PropManagerment.Instance.createProp(StringKit.toInt(selsecNum.Split(',')[0]));
            goodsView.init(p);
            goodsView.backGround.gameObject.SetActive(false);
            Prop storeProp = StorageManagerment.Instance.getProp(StringKit.toInt(selsecNum.Split(',')[0]));
            if (storeProp == null) {
                needNum.text = "[FF0000]0/" + StringKit.toInt(selsecNum.Split(',')[1]).ToString();
            }else{
                if(storeProp.getNum()< StringKit.toInt(selsecNum.Split(',')[1]))needNum.text="[FF0000]"+storeProp.getNum() + "/" + StringKit.toInt(selsecNum.Split(',')[1]).ToString();
                else needNum.text = storeProp.getNum() + "/" + StringKit.toInt(selsecNum.Split(',')[1]).ToString();
            }
            
        }
        
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
                    ResourcesManager.Instance.LoadAssetBundleTexture( BuffManagerment.Instance.getSkillIconPath(skillSids[i]),skillTexture[i]);
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
    private void updateAttrChanges() {
        AttrChange[] attrs = magicWeapon.getAttrChanges();
        AttrChange[] nextattrs = magicWeapon.getNextAttrChanges();
        for (int n = 0; n < sttrPonts.Length; n++) {
            sttrPonts[n].SetActive(false);
        }
        int[] index = new int[attrs.Length];
        if (attrs.Length == 1) index[0] = 1;
        else if (attrs.Length == 2) {
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
            for (int m = 0; m < sttrPonts.Length; m++) {
                if (m == index[j]) {
                    sttrPonts[m].SetActive(true);
                    sttrDec[m].spriteName = updateSprieName(attrs[j]);
                    sttrVel[m].text = attrs[j].num.ToString();
                    if (magicWeapon.getStrengLv() >= magicWeapon.getMaxStrengLv()) {
                        addVel[m].text = "";
                    } else {
                        addVel[m].text = "+" + nextattrs[j].num.ToString();
                    }
                    
                }
            }
        }
    }
    private string updateSprieName(AttrChange ac) {
        switch (ac.type) {
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
        for (int i = 0; i < addVel.Length; i++) {
            if (addVel[i] != null && addVel[i].gameObject != null && addVel[i].text != "")addVel[i].alpha = 1;
        }
    }
	/** 点击事件 */
	public override void buttonEventBase (GameObject gameObj) {
		base.buttonEventBase (gameObj);
        if(gameObj.name=="close"){
            finishWindow();
        } else if (gameObj.name == "OKButton" && type== MagicWeaponType.STRENGG) {
            if (magicWeapon.getStrengLv() >= magicWeapon.getMaxStrengLv()) {
                UiManager.Instance.openDialogWindow<MessageLineWindow>((win) => {
                    win.Initialize(LanguageConfigManager.Instance.getLanguage("magicweapLiii08"));
                });
                MaskWindow.UnlockUI();
                return;
            }
            string st=checkCondition();
            if (st == "pass") {
                MagicWeaponStrengFPort exf = FPortManager.Instance.getFPort("MagicWeaponStrengFPort") as MagicWeaponStrengFPort;
                if (effectPoint.transform.childCount == 0) {
                    exf.exchange(magicWeapon.uid, complateStreng);
                }
            } else {
               UiManager.Instance.openDialogWindow<MessageLineWindow>((win)=>{
				win.Initialize (st);
			    });
                return;
                }
            }   
        }
    /// <summary>
    /// 检查神器的强化条件
    /// </summary>
    /// <returns></returns>
    public string checkCondition() {
        string[][] props = CommandConfigManager.Instance.getMagicWeaponStrengProp();
        int quaIndex = magicWeapon.getLvType();
        string[] selectPropList = props[quaIndex-2];
        string selsecNum = magicWeapon.getStrengLv() >= (selectPropList.Length + 1) ? selectPropList[selectPropList.Length - 1] : selectPropList[magicWeapon.getStrengLv()];
        Prop p = PropManagerment.Instance.createProp(StringKit.toInt(selsecNum.Split(',')[0]));
        Prop storeProp = StorageManagerment.Instance.getProp(StringKit.toInt(selsecNum.Split(',')[0]));
        if (storeProp == null) return p.getName() + LanguageConfigManager.Instance.getLanguage("magicweapLiii07");
        if (storeProp.getNum() < StringKit.toInt(selsecNum.Split(',')[1])) return p.getName() + LanguageConfigManager.Instance.getLanguage("magicweapLiii07");
        return "pass"; 
    } 
    /// <summary>
    /// 完成强化回调
    /// </summary>
    void complateStreng() {
        MaskWindow.LockUI();
        UiManager.Instance.openDialogWindow<MessageLineWindow>((win) => {
            win.dialogCloseUnlockUI = false;
            win.Initialize(LanguageConfigManager.Instance.getLanguage("magicWeapStrengSuccess"),false);
        });
        EffectManager.Instance.CreateEffectCtrlByCache(effectPoint.transform, "Effect/UiEffect/Reinforced_SyntheticTwo", null);
        this.magicWeapon = StorageManagerment.Instance.getMagicWeapon(magicWeapon.uid);
        updateUI();
        Invoke("playAnimationOver", 2f);
    }
    public void playAnimationOver() {
        if (effectPoint.transform.childCount > 0) {
            effectPoint.transform.GetChild(0).gameObject.SetActive(false);
            Destroy(effectPoint.transform.GetChild(0).gameObject);
        }
        MaskWindow.UnlockUI();
    }
	/// <summary>
	/// 强化完成回调
	/// </summary>
	


}
