using System;
using UnityEngine;
using System.Collections;

/**
 * 装备图标节点按钮 ,应用于equipStore equipChoose 
 * */
public class MagicWeaponItem : ButtonBase
{ 
	public UILabel buttonName;
	public UILabel state;
	public UILabel mwname;//秘宝的名字
	public UILabel stengLv;//秘宝的强化等级
	public UILabel[] equipAttribute;
	public UILabel[] equipAttributeValue;
	public UITexture itemIcon;
	public UISprite quality;
    public UISprite typeIcon;//神器对应的职业
    public UISprite typeQuality;//职业图标显示颜色
	//public ButtonStoreResult intensifyButton;
    public MagicWeapon magicWeapon;//绑定的秘宝
	public UISprite isNew;
 	public UILabel starLevel;//秘宝的强化等级
	private const string FIRSTEQUIPNAME = "001";//第一个加载的装备的对象名字
    public ButtonBase putonButton;
    public ButtonBase iconButton;//头像事件
    public int intoType = 0;//进入的类型
    public GameObject starPrefab;//神器星星

	public override void DoUpdate ()
	{
		if (starLevel != null && starLevel.gameObject != null && starLevel.gameObject.activeSelf)
			starLevel.alpha = sin ();
	}
    public void UpdateMagicWeapon(MagicWeapon mWeapon, int type)
	{

        magicWeapon = mWeapon;
        intoType = type;
        if (intoType == MagicWeaponType.FROM_CARD_BOOK_NOT_M) {
            //没有秘宝的情况敢进入单独仓库界面 是想上装的节奏
            if (magicWeapon.state == MagicWeaponType.ON_USED) {
                putonButton.gameObject.SetActive(false);
            } else {
                putonButton.gameObject.SetActive(true);
                putonButton.textLabel.text = LanguageConfigManager.Instance.getLanguage("s0013");
                putonButton.onClickEvent = putOnMagicWeapon;
            }

        } else if(intoType==MagicWeaponType.STRENG) {
            putonButton.textLabel.text = LanguageConfigManager.Instance.getLanguage("s0012");
            putonButton.onClickEvent = gotoStrengWindow;//进入强化界面
        }else if(intoType==MagicWeaponType.FROM_CARD_BOOK_HAVE_M){//替换
            putonButton.textLabel.text = LanguageConfigManager.Instance.getLanguage("s0036");
            putonButton.onClickEvent = gotoReplareWindow;
        }
        iconButton.onClickEvent = gotoStrengWindowformIcon;

        //更新秘宝的强化等级
        if (magicWeapon.getPhaseLv() > 0) {//更新秘宝的进阶等级
            starLevel.gameObject.SetActive(true);
            starLevel.text = "+" + magicWeapon.getPhaseLv().ToString();
        } else starLevel.gameObject.SetActive(false);
        //更新秘宝的名字、图片、强化等级、状态、是否是新装备、职业类型
        typeIcon.gameObject.SetActive(true);
        typeQuality.gameObject.SetActive(true);
        if (magicWeapon.getMgType() == JobType.POWER) {//力
            typeIcon.spriteName = "roleType_2";
        } else if (magicWeapon.getMgType() == JobType.MAGIC) {//魔
            typeIcon.spriteName = "roleType_5";
        } else if (magicWeapon.getMgType() == JobType.AGILE) {//敏
            typeIcon.spriteName = "roleType_3";
        } else if (magicWeapon.getMgType() == JobType.POISON) {//毒
            typeIcon.spriteName = "roleType_4";
        } else if (magicWeapon.getMgType() == JobType.COUNTER_ATTACK) {//反 
            typeIcon.spriteName = "roleType_1";
        } else if (magicWeapon.getMgType() == JobType.ASSIST) {//辅
            typeIcon.spriteName = "roleType_6";
        } else {
            typeIcon.gameObject.SetActive(false);
            typeQuality.gameObject.SetActive(false);
        }
        //更新神器职业背景颜色，用以区分品质
        typeQuality.spriteName = QualityManagerment.qualityIconBgToBackGround(magicWeapon.getMagicWeaponQuality());
        mwname.text = QualityManagerment.getQualityColor(magicWeapon.getMagicWeaponQuality()) + magicWeapon.getName();
        ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + magicWeapon.getIconId (), itemIcon);
        stengLv.text = "Lv."+magicWeapon.getStrengLv().ToString();
        if (magicWeapon.state == 1) {
            state.text = LanguageConfigManager.Instance.getLanguage("s0017");
        } else state.text = "";
        if (magicWeapon.getIsNew()) {
            if (isNew != null && !isNew.gameObject.activeSelf)
                isNew.gameObject.SetActive(true);
        } else if (isNew != null && isNew.gameObject.activeSelf) {
            isNew.gameObject.SetActive(false);
        }
        //更新秘宝图片的背景 用品质来区分
        quality.spriteName = QualityManagerment.qualityIDtoMagicWeapon(magicWeapon.getMagicWeaponQuality());
        //更新秘宝的属性
        AttrChange[] attrs = magicWeapon.getAttrChanges();
        for (int j = 0; j < equipAttributeValue.Length; j++) {//所有属性全清零
            equipAttribute[j].text = "";
            equipAttributeValue[j].text = "";
        }
        if (attrs != null && attrs.Length > 0 && attrs[0] != null) {
            for (int i = 0; i < attrs.Length; i++) {
                equipAttribute[i].text = attrs[i].typeToString();
                if (equipAttributeValue[i] != null)
                    equipAttributeValue[i].text = attrs[i].num + "";
            }
        }
        if (magicWeapon != null && starPrefab != null) {
            Utils.RemoveAllChild(itemIcon.transform);
            int level = MagicWeaponSampleManager.Instance.getMagicWeaponSampleBySid(magicWeapon.sid).starLevel;
            if (level > 0) {
                GameObject obj = NGUITools.AddChild(itemIcon.gameObject, starPrefab);
                ShowStars show = obj.GetComponent<ShowStars>();
                show.initStar(level, MagicWeaponManagerment.USEDBUMAGIC_ATTRSHOW);
            }
        }
	}
    private void gotoStrengWindow(GameObject obj) {
        UiManager.Instance.openWindow<MagicWeaponStrengWindow>((win) => {
            win.init(magicWeapon, MagicWeaponType.STRENG);
        });
    }
    private void gotoStrengWindowformIcon(GameObject obj) {
        UiManager.Instance.openWindow<MagicWeaponStrengWindow>((win) => {
            win.init(magicWeapon, MagicWeaponType.STRENG);
        });
    }
    private void putOnMagicWeapon(GameObject obj) {//点击上装 给后台通信啦
        MaskWindow.LockUI();
        Card selectCard = MagicWeaponManagerment.Instance.selectCard;
        MagicWeaponPutOnFPort exf = FPortManager.Instance.getFPort("MagicWeaponPutOnFPort") as MagicWeaponPutOnFPort;
        exf.exchange(magicWeapon.uid, selectCard.uid,complatePutOn);
    }
    void complatePutOn() {
        StorageManagerment.Instance.getRole(MagicWeaponManagerment.Instance.selectCard.uid).magicWeaponUID = magicWeapon.uid;
        magicWeapon.state = 1;
        MagicWeaponManagerment.Instance.selectCard = null;
        UiManager.Instance.openDialogWindow<MessageLineWindow>((win) => {
            win.Initialize(LanguageConfigManager.Instance.getLanguage("puton_magicweapon_success"));
               return;
           });
        fatherWindow.finishWindow();
    }
    void gotoReplareWindow(GameObject obj) {//点击替换 给后台通信啦
        MaskWindow.LockUI();
        Card selectCard = MagicWeaponManagerment.Instance.selectCard;
        MagicWeaponPutOnFPort exf = FPortManager.Instance.getFPort("MagicWeaponPutOnFPort") as MagicWeaponPutOnFPort;
        exf.exchange(magicWeapon.uid, selectCard.uid, complateRele);
    }
    void complateRele(){
        StorageManagerment.Instance.getMagicWeapon(MagicWeaponManagerment.Instance.selectCard.magicWeaponUID).state = 0;
        StorageManagerment.Instance.getRole(MagicWeaponManagerment.Instance.selectCard.uid).magicWeaponUID = magicWeapon.uid;
        magicWeapon.state = 1;
        MagicWeaponManagerment.Instance.selectCard = null;
        UiManager.Instance.openDialogWindow<MessageLineWindow>((win) => {
            win.Initialize(LanguageConfigManager.Instance.getLanguage("replate_magicweapon_success"));
            return;
        });
        UiManager.Instance.BackToWindow<CardBookWindow>();
    }
}
