using UnityEngine;
using System.Collections;
using System;
using System.Reflection;

/// <summary>
/// 神器锻造
/// </summary>
public class MagicWeaponPhaseWindow : WindowBase {

	/**field**/
    public UITexture icon;//秘宝形象
    public UISprite icon_bg;//秘宝背景
    public UISprite jobType;//职业
    public UISprite jobBg;//职业背景
    public UILabel magicName;//秘宝名字
    public UILabel phaseLv; //秘宝进阶等级
    public UITexture skillTexture;//技能图标
    public UILabel skillDec;//技能描述
    public GameObject[] skillPonts;//放技能的节点
    public GoodsView goodsView;
    public UILabel needNum;//需要的道具数量
    private MagicWeapon magicWeapon;//传入进来的秘宝
    public barCtrl towerBar;
    public UILabel towerValue;
    public UILabel phaseNum;
    public UILabel phaseNum1;
    public UILabel phaseNeedStengLv;//锻造需求的强化等级
    public ButtonBase phaseButton;
    public GameObject animationPoint;
    /** 骑术等级文本 */
    public UILabel skillLevelText;
    public GameObject conditionPoint;
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
        this.magicWeapon = StorageManagerment.Instance.getMagicWeapon(magicWeapon.uid);
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
        if (magicWeapon.getPhaseLv() > 0) {
            phaseLv.gameObject.SetActive(true);
            phaseLv.text = "+" + magicWeapon.getPhaseLv().ToString();
        } else phaseLv.gameObject.SetActive(false);
        towerBar.updateValue((float)magicWeapon.getLuckNumber(), (float)100);
        towerValue.text = magicWeapon.getLuckNumber()+ "%";
        magicName.text = QualityManagerment.getQualityColor(magicWeapon.getMagicWeaponQuality()) + magicWeapon.getName();
        phaseNum.text = magicWeapon.getPhaseLv() + "/" + magicWeapon.getMaxPhaseLv();
        phaseButton.disableButton(false);
        if(magicWeapon.getPhaseLv()==magicWeapon.getMaxPhaseLv()){
            phaseNum.text = LanguageConfigManager.Instance.getLanguage("magicweapLiii03");
            //phaseButton.disableButton(true);
        }
        updateSuccess();
        ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.ICONIMAGEPATH + magicWeapon.getIconId(),icon);
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
        updateSkills();
        updateProp();
    }
    private void updateSuccess() {
        int succindex = magicWeapon.getSuccess();
        if(succindex>=100){
            phaseNum1.text = LanguageConfigManager.Instance.getLanguage("magicweapSuccess5");
        }else if(succindex>=80){
             phaseNum1.text = LanguageConfigManager.Instance.getLanguage("magicweapSuccess4");
        } else if (succindex >= 60) {
            phaseNum1.text = LanguageConfigManager.Instance.getLanguage("magicweapSuccess3");
        } else if (succindex >= 40) {
            phaseNum1.text = LanguageConfigManager.Instance.getLanguage("magicweapSuccess2");
        } else if (succindex >= 20) {
            phaseNum1.text = LanguageConfigManager.Instance.getLanguage("magicweapSuccess1");
        } else {
            phaseNum1.text = LanguageConfigManager.Instance.getLanguage("magicweapSuccess0");
        }
        if(magicWeapon.getPhaseLv()>=magicWeapon.getMaxPhaseLv()){
            phaseNum1.text = "100%";
        }
    }
    private void updateProp() {
        if (magicWeapon.getPhaseLv() == magicWeapon.getMaxPhaseLv()) conditionPoint.SetActive(false);
        else conditionPoint.SetActive(true);
        if (type == MagicWeaponType.PHASE) {
            string[][] props = CommandConfigManager.Instance.getMagicWeaponPhaseProp();
            int quaIndex=magicWeapon.getLvType();
            string[] selectPropList=props[quaIndex-2];
            string selsecNum=magicWeapon.getPhaseLv()>=(selectPropList.Length)?selectPropList[selectPropList.Length-1]:selectPropList[magicWeapon.getPhaseLv()];
            Prop p = PropManagerment.Instance.createProp(StringKit.toInt(selsecNum.Split(',')[0]));
            goodsView.init(p);
            goodsView.backGround.gameObject.SetActive(false);
            Prop storeProp = StorageManagerment.Instance.getProp(StringKit.toInt(selsecNum.Split(',')[0]));
            int needStringLv = magicWeapon.getNeedStrengLv(magicWeapon.getPhaseLv()>=magicWeapon.getMaxPhaseLv()?magicWeapon.getPhaseLv()-1:magicWeapon.getPhaseLv());
            if (magicWeapon.getStrengLv() < needStringLv) {
                phaseNeedStengLv.text = "[FF0000]"+magicWeapon.getStrengLv().ToString()+"/"+needStringLv.ToString();
            } else {
                phaseNeedStengLv.text = "[3A9663]" + magicWeapon.getStrengLv().ToString() + "/" + needStringLv.ToString();
            }
            if (storeProp == null) needNum.text = "[FF0000]0/" + StringKit.toInt(selsecNum.Split(',')[1]).ToString();
            else {
                if (storeProp.getNum() < StringKit.toInt(selsecNum.Split(',')[1])) {
                    needNum.text = "[FF0000]"+storeProp.getNum() + "/" + StringKit.toInt(selsecNum.Split(',')[1]).ToString();
                } else {
                    needNum.text = "[3A9663]" + storeProp.getNum() + "/" + StringKit.toInt(selsecNum.Split(',')[1]).ToString();
                }
               
            }
        }
        
    }
    /// <summary>
    /// 更新技能图标和描述
    /// </summary>
    private void updateSkills() {
int[] skillSids = magicWeapon.skillSids;
        int lv = magicWeapon.getPhaseLv();
        SkillSample sk = SkillSampleManager.Instance.getSkillSampleBySid(skillSids[lv >= magicWeapon.getMaxPhaseLv() ? magicWeapon.getMaxPhaseLv()-1 : lv]);
        if (sk != null) {
            skillDec.text = "[724C41]" + sk.describe;
            ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.SKILLIMAGEPATH + sk.iconId, skillTexture);
        } 
        if (sk == null) {
            BuffSample bs = BuffSampleManager.Instance.getBuffSampleBySid(skillSids[lv >= magicWeapon.getMaxPhaseLv() ? magicWeapon.getMaxPhaseLv() - 1 : lv]);
           if (bs != null) {
               skillDec.text = "[724C41]" + bs.name;
               //ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.SKILLIMAGEPATH + bs.iconId, skillTexture);
				ResourcesManager.Instance.LoadAssetBundleTexture(BuffManagerment.Instance.getSkillIconPath(bs.sid), skillTexture);
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
	/** 点击事件 */
	public override void buttonEventBase (GameObject gameObj) {
		base.buttonEventBase (gameObj);
        if(gameObj.name=="close"){
            finishWindow();
         
        } else if (gameObj.name == "OKButton" && type == MagicWeaponType.PHASE) {
            if (magicWeapon.getPhaseLv() >= magicWeapon.getMaxPhaseLv()) {
                UiManager.Instance.openDialogWindow<MessageLineWindow>((win) => {
                    win.Initialize(LanguageConfigManager.Instance.getLanguage("magicweapLiii09"));
                });
                MaskWindow.UnlockUI();
                return;
            }
            string st=checkCondtion();
            if(st!="pass"){
                UiManager.Instance.openDialogWindow<MessageLineWindow>((win) => {
                    win.Initialize(st);
                });
                return;
            } else {
                MagicWeaponPhaseFPort exf = FPortManager.Instance.getFPort("MagicWeaponPhaseFPort") as MagicWeaponPhaseFPort;
                exf.exchange(magicWeapon.uid, complatephase);
            }
            
        }

	}
    /// <summary>
    /// 检查条件
    /// </summary>
    /// <returns></returns>
    private string checkCondtion() {
        string[][] props = CommandConfigManager.Instance.getMagicWeaponPhaseProp();
        int quaIndex = magicWeapon.getLvType();
        string[] selectPropList = props[quaIndex -2];
        string selsecNum = magicWeapon.getPhaseLv() >= (selectPropList.Length) ? selectPropList[selectPropList.Length - 1] : selectPropList[magicWeapon.getPhaseLv()];
        Prop p = PropManagerment.Instance.createProp(StringKit.toInt(selsecNum.Split(',')[0]));
        Prop storeProp = StorageManagerment.Instance.getProp(StringKit.toInt(selsecNum.Split(',')[0]));
        if (storeProp == null) return p.getName() + LanguageConfigManager.Instance.getLanguage("magicweapliii06");
        if (storeProp.getNum() < StringKit.toInt(selsecNum.Split(',')[1])) return p.getName() + LanguageConfigManager.Instance.getLanguage("magicweapliii06");
        int needStringLv = magicWeapon.getNeedStrengLv(magicWeapon.getPhaseLv());
        if (magicWeapon.getStrengLv() < needStringLv) {
            return LanguageConfigManager.Instance.getLanguage("magicweapLiii05", needStringLv.ToString());
        }
        return "pass";
    }
    public void complatephase(string str) {
        if(str=="ok"){
            if (animationPoint.transform.childCount == 0) {
                passObj _obj = MonoBase.Create3Dobj("Effect/UiEffect/battleAnim");
                _obj.obj.transform.parent = animationPoint.transform;
                _obj.obj.transform.localPosition = Vector3.zero;
                _obj.obj.transform.localScale = Vector3.one;
                BattleAnimCtrl battleAnimCtrl = _obj.obj.GetComponent<BattleAnimCtrl>();
                battleAnimCtrl.succese.transform.localPosition = Vector3.zero;
                battleAnimCtrl.succese.SetActive(true);
            } else {
                BattleAnimCtrl battleAnimCtrl = animationPoint.transform.GetChild(0).GetComponent<BattleAnimCtrl>();
                battleAnimCtrl.succese.transform.localPosition = Vector3.zero;
                battleAnimCtrl.succese.SetActive(true);
            }
            this.magicWeapon = StorageManagerment.Instance.getMagicWeapon(magicWeapon.uid);
            
            updateUI();
            Invoke("playAnimationOver", 2f);
        } else if (str == "evo_failed") {
            if (animationPoint.transform.childCount == 0) {
                passObj _obj = MonoBase.Create3Dobj("Effect/UiEffect/battleAnim");
                _obj.obj.transform.parent = animationPoint.transform;
                _obj.obj.transform.localPosition = Vector3.zero;
                _obj.obj.transform.localScale = Vector3.one;
                BattleAnimCtrl battleAnimCtrl = _obj.obj.GetComponent<BattleAnimCtrl>();
                battleAnimCtrl.fail.transform.localPosition = Vector3.zero;
                battleAnimCtrl.fail.SetActive(true);
            } else {
                BattleAnimCtrl battleAnimCtrl = animationPoint.transform.GetChild(0).GetComponent<BattleAnimCtrl>();
                battleAnimCtrl.fail.transform.localPosition = Vector3.zero;
                battleAnimCtrl.fail.SetActive(true);
            }
            magicWeapon.addSuccess += 10;
            
            updateUI();
            Invoke("playAnimationOver", 2f);

        }
    }
    public void playAnimationOver() {
        if (animationPoint.transform.GetChild(0).FindChild("root").FindChild("fail").gameObject.activeSelf ||
            animationPoint.transform.GetChild(0).FindChild("root").FindChild("succese").gameObject.activeSelf) {
            animationPoint.transform.GetChild(0).FindChild("root").FindChild("fail").gameObject.SetActive(false);
            animationPoint.transform.GetChild(0).FindChild("root").FindChild("succese").gameObject.SetActive(false);
        }
        MaskWindow.UnlockUI();
    }
    //一直更新进阶状态
    void Update() {
        if (phaseLv != null && phaseLv.gameObject != null && phaseLv.gameObject.activeSelf)
            phaseLv.alpha = sin();
    }
	


}
