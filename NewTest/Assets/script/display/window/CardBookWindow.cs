using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 卡片详细信息UI组件
 * @author 杨小珑
 **/
public class CardBookWindow : WindowBase {

    public const int INTOTEAM = 1;//上阵模式 按钮显示上阵
    public const int CARDCHANGE = 2;//卡牌替换模式 按钮显示替换
    public const int VIEW = 3;//卡牌浏览模式 按钮显示一键换装 
    public const int MINING = 6;//挖矿队伍
    public const int SHOW = 11;//不显示按钮 技能框可以拖动 点击装备可以查看详细信息 但不能操作
    public const int CHATSHOW = 12;//不显示按钮 技能框可以拖动 点击装备可以查看详细信息 但不能操作
    public const int OTHER = 13;//其他 用于控制窗口按钮显示（不显示按钮 并且技能框不能拖动） 
    public const int CONTINUE = 14;//技能框不能拖动 , 显示继续按钮,点继续按钮强化,学习技能等操作,关闭返回主界面,类似于OTHER
    public const int AWARDINTO = 15;
    public const int ONEKEYEQUIP = 1;//一键换装
    public const int RELOADEQUIP = 2;//穿装
    public const int UNDRESS = 3;//脱装
    public const int CLICKCHATSHOW = 5;//聊天界面点击装备
    public const int CARDTRAINING = 4; //卡牌训练

    /** 卡片品质特效点 */
    public GameObject qualityEffectPoint;
    public SampleDynamicContent sampleContent;
    public UISprite jobSprite;
    public GameObject leftArrow;
    public GameObject rightArrow;
    public GameObject[] star;//卡片星级
    public GameObject starContent;//星星容器

    public UITexture CardImage;
    public barCtrl expbar;
    public UILabel nameLabel;
    public UILabel cardLv;
    public UILabel hp;
    public UILabel combat;
    public UILabel mag;
    public UILabel att;
    public UILabel dex;
    public UILabel def;
    public UILabel hpAdd;
    public UILabel magAdd;
    public UILabel attAdd;
    public UILabel dexAdd;
    public UILabel defAdd;
    public UILabel LockedLable;//秘宝未解锁标签
    public UISprite hasMagicCanUseTip;
    /** 进化等级 */
    public UILabel jinhua;
    /** 进化标题 */
    public UISprite jinhuaTitle;
    /** 卡片品质 */
    public UISprite quality;
    /** 顶部背景 */
    public UITexture topBackGround;
    /** 特技品质 */
    public UISprite skillQuality;//史诗、传说、神话
    /** 底部背景 */
    public UITexture downTextures;
    public UILabel[] addLabels;
    public UILabel[] addValueLabels;
    public UILabel[] totleValueLabels;

    /** 技能样板 */
    public GameObject skillObj;
    /** 主动技能容器 */
    public GameObject root_mainSkill;
    /** 被动技容器 */
    public GameObject root_passiveSkill;
    /** 被动技能样板 */
    public GameObject passiveSkillObj;
    /** 被动技容器组 */
    public GameObject root_passiveSkillGroup;

    public ButtonEquipInCardBook[] equipItem;//0武器1胸甲2鞋子3头盔4戒指
    public ButtonBase buttonPowerUp;
    public ButtonBase buttonOther;
    public ButtonBase buttonOneKey;
    public ButtonBase buttonContinue;
    public ButtonBase buttonPicture;
    public ButtonBase buttonChatShow;
    public ButtonBase buttonHelp;
    public ButtonBase buttonBlood;//血脉按钮
    /** 去进化按钮 */
    public ButtonBase buttonGotoEvo;

    public GameObject[] typeProject;//0普通，1祭品
    public UILabel projectDesc;//祭品描述

    public UIDragScrollView drag;
    public ButtonBase selsecStarSoulButton;//上阵星魂按钮（或查看）
    public ButtonBase selsectMagicWeaponButton;//装备秘宝按钮（或查看）
    public ButtonBase selectResonance;//共鸣按钮
    public GameObject selecStarSoulLock;//等级不够显示星魂
    public GameObject selectMagicWeaponLock;//等级不够显示秘宝
    public GameObject selectResonanceLock;//等级不够显示共鸣
    /** 天赋容器 */
    public GameObject talentContent;
    /** 天赋样板 */
    public GameObject talentPrefab;
    /** 天赋背景 */
    public UISprite talentBg;
    /** 天赋技能容器 */
    public UIPanel panlObj;
    /**有秘宝时候的图标 */
    public UITexture magicWeaponIcon;
    public GameObject starsPrefab;
    /**血脉等级 */
    public GameObject bloodPoint;
    public UILabel bloodLv;
    public UITexture[] selectTextures;
    private int posIndex;
    /** 临时缓存的主动技能 */
    private List<ButtonSkill> mainSkillList;
    /** 临时缓存的被动技能 */
    private List<ButtonSkill> attrSkillList;
    /** 临时缓存的天赋 */
    private List<CardAttrTalentItem> talentList;

    private int showType;
    private List<Card> list;
    private CallBack closeCallback;
    private int defaultIndex;
    int storageVersion = -1;
    private int showCombat = 0;
    private CardBaseAttribute attrOld;
    private Card showCard;
    private ServerCardMsg showCardNet;
    private List<Equip> showChatEquips;
    private static string chatPlayerUid;
    int[] addValues;
    int[] oldValues;
    int[] currentValues;

    public override void OnAwake() {
        base.OnAwake();
        UiManager.Instance.cardBookWindow = this;
    }

    public override void DoDisable() {
        iTween.Stop(gameObject, true);
        base.DoDisable();
        ChatManagerment.Instance.chatCard = null;
    }


    protected override void DoEnable() {
        iTween.Stop(gameObject,true);
        UiManager.Instance.backGround.switchBackGround("ChouJiang_BeiJing");
        for (int i = 0; i < selectTextures.Length; i++)
        {
            ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.BACKGROUNDPATH + "selectTextures" + (i>3?"4":i+""),selectTextures[i]);
            
        }
        if (MissionManager.instance != null)//隐藏掉3D场景
            MissionManager.instance.hideAll();
    }
    public void updateEuipEnableStatus() {
        updateEquipStatus();

    }

    public void init(Card card, int type, CallBack closeCallback) {
        this.list = new List<Card>();
        list.Add(card);
        this.defaultIndex = 0;
        showCard = card;
    }

    public void init(List<Card> list, int defaultIndex, int type, CallBack closeCallback) {
        this.list = list;
        this.showType = type;
        this.defaultIndex = defaultIndex;
        this.closeCallback = closeCallback;
        showCard = list[defaultIndex];
    }

    void onContentFinish() {
    }

    void updatePage(GameObject obj) {

        int index = StringKit.toInt(obj.name) - 1;
        //不够3页.隐藏
        if (list == null || index >= list.Count || list[index] == null) {
            return;
        }
        UITexture texContent = obj.transform.GetChild(0).GetComponent<UITexture>();
        ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.CARDIMAGEPATH + list[index].getImageID(), texContent);

    }

    void updateActivePage(GameObject obj) {
        //更新箭头
        int index = StringKit.toInt(obj.name) - 1;
        if (list.Count == 1) {
            leftArrow.gameObject.SetActive(false);
            rightArrow.gameObject.SetActive(false);
        } else if (index == 0) {
            leftArrow.gameObject.SetActive(false);
            rightArrow.gameObject.SetActive(true);
        } else if (index == list.Count - 1) {
            leftArrow.gameObject.SetActive(true);
            rightArrow.gameObject.SetActive(false);
        } else {
            leftArrow.gameObject.SetActive(true);
            rightArrow.gameObject.SetActive(true);
        }

        showCard = list[index];
        updateMagicWeaponButton();//此界面关于秘宝的更新
        updateBloodButton();//更新血脉按键
        if (UserManager.Instance.self.getUserLevel() > 34 && !isSpriteCard(showCard)) {
            selsecStarSoulButton.gameObject.SetActive(!ChooseTypeSampleManager.Instance.isToEat(showCard));
            selecStarSoulLock.gameObject.SetActive(ChooseTypeSampleManager.Instance.isToEat(showCard));
        } else {
            selsecStarSoulButton.gameObject.SetActive(false);
            selecStarSoulLock.gameObject.SetActive(true);
        }
        if (UserManager.Instance.self.getUserLevel() >= 30 && !isSpriteCard(showCard))
        {
            //selectResonance.gameObject.SetActive(true);
            selectResonance.gameObject.SetActive(true);
            selectResonanceLock.gameObject.SetActive(false);
        }
        else
        {
            selectResonance.gameObject.SetActive(false);
            selectResonanceLock.gameObject.SetActive(true);
        }
        CardImage = obj.transform.GetChild(0).GetComponent<UITexture>();
        if (ChatManagerment.Instance.chatCard != null) {
            //存在聊天查看卡片
            InitializeCardInfo(ChatManagerment.Instance.chatCard, getShowType());
        } else {
            CardManagerment.Instance.showChatEquips.Clear();
            if (showCardNet != null)
            {
                updateAllServer();
            }
            else
            {
                InitializeCardInfo(list[index], getShowType());
            }
        }
        defaultIndex = StringKit.toInt(obj.name) - 1;
    }
    void updateBloodButton() {
        if (showType == CardBookWindow.SHOW || showType == CardBookWindow.CHATSHOW || showType == CardBookWindow.OTHER || showType == CardBookWindow.CLICKCHATSHOW) {
            buttonBlood.gameObject.SetActive(false);
            return;
        }
        buttonBlood.gameObject.SetActive(CardSampleManager.Instance.checkBlood(showCard.sid, showCard.uid));
    }
    void updateMagicWeaponButton() {
        Utils.RemoveAllChild(magicWeaponIcon.transform);
        if (showType == CardBookWindow.SHOW)
        {//不显示按钮 技能框可以拖动 点击装备可以查看详细信息 但不能操作
            selsectMagicWeaponButton.gameObject.SetActive(true);//false
            selsectMagicWeaponButton.gameObject.GetComponent<Collider>().enabled = false;
            return;
        }
        if (showType == CardBookWindow.CHATSHOW || showType == CardBookWindow.CLICKCHATSHOW||showType==CardBookWindow.OTHER) {//从聊天界面过来对秘宝的判断
            if (showCard.magicWapon == null)
            {
                selsectMagicWeaponButton.gameObject.SetActive(false);
                selectMagicWeaponLock.gameObject.SetActive(true);
                if (showType == CardBookWindow.CHATSHOW && showCard.magicWeaponUID != null && (UserManager.Instance.self.getUserLevel() >= CommandConfigManager.Instance.getMagicLimitLevel()) || showType == CardBookWindow.OTHER)
                {
                    selsectMagicWeaponButton.gameObject.SetActive(true);
                    selectMagicWeaponLock.gameObject.SetActive(false);
                    selsectMagicWeaponButton.gameObject.GetComponent<Collider>().enabled = false;
                    if (showCard.magicWeaponUID != "0"&&showType != CardBookWindow.OTHER)
                    {
                        MagicWeapon mw = StorageManagerment.Instance.getMagicWeapon(showCard.magicWeaponUID);
                        if(mw != null)
                            ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.ICONIMAGEPATH + mw.getIconId(), magicWeaponIcon);
                        if (starsPrefab != null)
                        {
                            Utils.RemoveAllChild(magicWeaponIcon.transform);
                            if (mw == null) return;
                            MagicWeaponSample sample = MagicWeaponSampleManager.Instance.getMagicWeaponSampleBySid(mw.sid);
                            GameObject stars = NGUITools.AddChild(magicWeaponIcon.gameObject, starsPrefab);
                            ShowStars show = stars.GetComponent<ShowStars>();
                            if (sample != null && sample.starLevel > 0)
                            {
                                show.initStar(sample.starLevel, MagicWeaponManagerment.USEDBYMAGICITEM);
                            }
                        }
                    }
                }

            }
            else
            {
                selsectMagicWeaponButton.gameObject.SetActive(true);
                ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.ICONIMAGEPATH + showCard.magicWapon.getIconId(), magicWeaponIcon);
                if (starsPrefab != null)
                {
                    Utils.RemoveAllChild(magicWeaponIcon.transform);
                    if (showCard.magicWapon == null) return;
                    MagicWeaponSample sample = MagicWeaponSampleManager.Instance.getMagicWeaponSampleBySid(showCard.magicWapon.sid);
                    GameObject stars = NGUITools.AddChild(magicWeaponIcon.gameObject, starsPrefab);
                    ShowStars show = stars.GetComponent<ShowStars>();
                    if (sample != null && sample.starLevel > 0)
                    {
                        show.initStar(sample.starLevel, MagicWeaponManagerment.USEDBYMAGICITEM);
                    }
                }
            }
        } else {
            //判断秘宝是否解锁
            if (UserManager.Instance.self.getUserLevel() < CommandConfigManager.Instance.getMagicLimitLevel()) {
                //selsectMagicWeaponButton.gameObject.collider.enabled = false;
                //LockedLable.gameObject.SetActive(true);
                selectMagicWeaponLock.gameObject.SetActive(true);
                selsectMagicWeaponButton.gameObject.SetActive(false);
            } 
            else 
            {
                selsectMagicWeaponButton.gameObject.collider.enabled = true;
                LockedLable.gameObject.SetActive(false);
                if (showCard.magicWeaponUID != "" && showCard.magicWeaponUID != "0") {
                    hasMagicCanUseTip.gameObject.SetActive(false);
                    MagicWeapon mw = StorageManagerment.Instance.getMagicWeapon(showCard.magicWeaponUID);
                    ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.ICONIMAGEPATH + mw.getIconId(), magicWeaponIcon);
                    if (starsPrefab != null) {
                        Utils.RemoveAllChild(magicWeaponIcon.transform);
                        if (mw == null) return;
                        MagicWeaponSample sample = MagicWeaponSampleManager.Instance.getMagicWeaponSampleBySid(mw.sid);
                        GameObject stars = NGUITools.AddChild(magicWeaponIcon.gameObject, starsPrefab);
                        ShowStars show = stars.GetComponent<ShowStars>();
                        if (sample != null && sample.starLevel > 0) {
                            show.initStar(sample.starLevel, MagicWeaponManagerment.USEDBYMAGICITEM);
                        }
                    }
                } else {
                    magicWeaponIcon.mainTexture = null;
                    if (StorageManagerment.Instance.getAllMagicWeaponByType(showCard.getJob()) == null) {
                        hasMagicCanUseTip.gameObject.SetActive(false);
                    } else {
                        hasMagicCanUseTip.gameObject.SetActive(true);
                    }
                }
            }
            selsectMagicWeaponButton.gameObject.SetActive(!ChooseTypeSampleManager.Instance.isToEat(showCard));
            if (UserManager.Instance.self.getUserLevel() < CommandConfigManager.Instance.getMagicLimitLevel())
            {
                //selsectMagicWeaponButton.gameObject.collider.enabled = false;
                //LockedLable.gameObject.SetActive(true);
                selectMagicWeaponLock.gameObject.SetActive(true);
                selsectMagicWeaponButton.gameObject.SetActive(false);
            }
        }
    }
    public bool isSpriteCard(Card _card) {
        if (ChooseTypeSampleManager.Instance.isToEat(_card, ChooseTypeSampleManager.TYPE_ADDON_NUM)) {
            return true;
        }
        return false;
    }
    protected override void begin() {
        ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.BACKGROUNDPATH + "card_downBackGround", downTextures);
        sampleContent.startIndex = defaultIndex;
        if (!isAwakeformHide) {
            sampleContent.maxCount = list.Count;
            sampleContent.onLoadFinish = onContentFinish;
            sampleContent.callbackUpdateEach = updatePage;
            sampleContent.onCenterItem = updateActivePage;
            sampleContent.init();
        } else {
            //hide唤醒
            if (StorageManagerment.Instance.RoleStorageVersion != storageVersion) {
                //仓库有更改就刷新下容器
                sampleContent.maxCount = list.Count;
                sampleContent.init();
                storageVersion = StorageManagerment.Instance.RoleStorageVersion;
            }
            //更新页,awake会更新三页,所以不用
            if (showCardNet!=null)
            {
                updateAllServer();
            }
            else
            {
                updateAll();
            }
        }
        //	updateActivePage (sampleContent.getCenterObj ());
        MaskWindow.UnlockUI();
    }

    public int getShowType() {
        return showType;
    }

    public List<Card> getList() {
        return list;
    }

    public CallBack getCloseCallback() {
        return closeCallback;
    }

    //显示角色属性变化 用于穿装脱装后
    public void equipNewItem(List<AttrChange> atts, int type) {
        equipNewItem2(atts, type);
        IncreaseManagerment.Instance.getAllEquips();
        updateEuipEnableStatus();
    }

    public void showItemUpdateAll() {
        updateAll();
    }

    public static void Show(List<Card> list, int type, CallBack closeCallback) {
        Show(list, 0, type, closeCallback);
    }

    public static void Show(List<Card> list, Card defaultCard, int type, CallBack closeCallback) {
        if (list == null)
            throw new System.Exception("list is null");
        if (list.Count == 0)
            throw new System.Exception("list length == 0");
        int defaultIndex = -1;
        for (int i = 0; i < list.Count; i++) {
            if (list[i] == defaultCard) {
                defaultIndex = i;
                break;
            }
        }
        if (defaultIndex < 0)
            throw new System.Exception("defaultCard not in list");
        Show(list, defaultIndex, type, closeCallback);
    }

    public static void Show(Card showCard, int type, CallBack closeCallback) {
        if (showCard == null)
            throw new System.Exception("showCard is null");

        List<Card> list = new List<Card>();
        list.Add(showCard);
        Show(list, showCard, type, closeCallback);
    }

    public static void Show(List<Card> list, int defaultIndex, int type, CallBack closeCallback) {
        if (list == null)
            throw new System.Exception("list is null");
        if (list.Count == 0)
            throw new System.Exception("list length == 0");
        if (defaultIndex < 0 || defaultIndex >= list.Count)
            defaultIndex = 0;

        UiManager.Instance.openWindow<CardBookWindow>(
            (win) => {
                win.init(list, defaultIndex, type, closeCallback);
            }
        );

    }

    public static void Show(ServerCardMsg showCard, int type) {        
        if (showCard == null)
            throw new System.Exception("showCard is null");
        CardBookWindow.Show(showCard.card, type, null);
        ChatManagerment.Instance.chatCard = showCard;
    }
    public static void Show(ServerCardMsg showCard, int type, string uid)
    {
        chatPlayerUid = uid;
        if (showCard == null)
            throw new System.Exception("showCard is null");
        CardBookWindow.Show(showCard.card, type, null);
        ChatManagerment.Instance.chatCard = showCard;
    }
    public static void Show() {
        if (UiManager.Instance.cardBookWindow == null) {
            UiManager.Instance.openWindow<CardBookWindow>();
        }
    }

    //获得当前显示卡片
    public Card getShowCard() {

        return showCard;
    }
    // jordenwu::展示卡片质量效果 蓝色 黄色 紫色
    public void UpdateQualityEffect() {
        if (showCard == null) {
            qualityEffectPoint.SetActive(false);
            return;
        }
        bool isShow = showEffectByQuality(showCard.getQualityId());
        if (isShow) {
            qualityEffectPoint.SetActive(true);
        } else {
            if (qualityEffectPoint != null && qualityEffectPoint.transform.childCount > 0)
                Utils.RemoveAllChild(qualityEffectPoint.transform);
            qualityEffectPoint.SetActive(false);
        }
    }
    /** 显示卡片本身品质  */
    public bool showEffectByQuality(int qualityId) {
        if (qualityEffectPoint == null)
            return false;
        if (qualityId < QualityType.GOOD)
            return false;
        if (qualityEffectPoint.transform.childCount > 0)
            Utils.RemoveAllChild(qualityEffectPoint.transform);
        qualityEffectPoint.SetActive(true);
        EffectManager.Instance.CreateEffect(qualityEffectPoint.transform, "Effect/UiEffect/CardQualityEffect" + qualityId);
        return true;
    }

    public override void OnNetResume() {
        base.OnNetResume();
        showItemUpdateAll();
    }

    public void updateEquipStatus() {
        iTween.Stop(gameObject,true);
        foreach (ButtonEquipInCardBook equip in equipItem) {
            if(equip.gameObject.GetComponent<iTween>())DestroyImmediate(equip.gameObject.GetComponent<iTween>());
            iTween.Stop(equip.gameObject, true);
            equip.updateStatus();
        }
    }
    private void initButton() {
        buttonPicture.fatherWindow = this;
        buttonPicture.gameObject.SetActive(PictureManagerment.Instance.mapType.ContainsKey(showCard.getEvolveNextSid()));

        buttonPowerUp.gameObject.SetActive(false);
        buttonOther.gameObject.SetActive(false);
        buttonOneKey.gameObject.SetActive(false);
        if (showType == CardBookWindow.INTOTEAM) {
            buttonOneKey.gameObject.SetActive(true);
            buttonOther.textLabel.text = LanguageConfigManager.Instance.getLanguage("s0035");
            buttonOther.gameObject.SetActive(showCard.uid != UserManager.Instance.self.mainCardUid);
            buttonPowerUp.gameObject.SetActive(true);
        } else if (showType == CardBookWindow.VIEW) {
            buttonOneKey.gameObject.SetActive(true);
            buttonPowerUp.gameObject.SetActive(true);
        } else if (showType == CardBookWindow.CARDCHANGE) {
            buttonOneKey.gameObject.SetActive(true);
            buttonOther.textLabel.text = LanguageConfigManager.Instance.getLanguage("s0036");
            buttonOther.gameObject.SetActive(true);
            buttonPowerUp.gameObject.SetActive(true);
            if (showCard.uid == UserManager.Instance.self.mainCardUid) {
                buttonOther.disableButton(true);
            } else {
                buttonOther.disableButton(false);
            }
        } else if (showType == CardBookWindow.CHATSHOW) {
            buttonChatShow.gameObject.SetActive(true);
        } else if (showType == CardBookWindow.OTHER) {
        } else if (showType == CardBookWindow.MINING) {
            buttonOneKey.gameObject.SetActive(true);
            buttonOther.textLabel.text = LanguageConfigManager.Instance.getLanguage("s0036");
            buttonOther.gameObject.SetActive(true);
            buttonPowerUp.gameObject.SetActive(true);
            if (showCard.uid == UserManager.Instance.self.mainCardUid) {
                buttonOther.disableButton(true);
            } else {
                buttonOther.disableButton(false);
            }
        }
        if (ChooseTypeSampleManager.Instance.isToEat(showCard)) {
            for (int i = 0; i < equipItem.Length; i++) {
                equipItem[i].GetComponent<BoxCollider>().enabled = false;
                equipItem[i].updateAddEquipTipVisible(false);
            }
            buttonOneKey.gameObject.SetActive(false);
            buttonPowerUp.gameObject.SetActive(false);
        } else {
            for (int i = 0; i < equipItem.Length; i++) {
                equipItem[i].GetComponent<BoxCollider>().enabled = true;
            }
        }
        //达到自身等级上限（非主角等级限制）且未进化满10次的卡片： 需进化
        if (!EvolutionManagerment.Instance.isMaxEvoLevel(showCard) && showCard.isMaxLevel() && StorageManagerment.Instance.getRole(showCard.uid) != null) {
            buttonGotoEvo.gameObject.SetActive(true);
        } else {
            buttonGotoEvo.gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// 检查特殊卡片的描述可以状态
    /// </summary>
    /// <returns></returns>
    private bool checkEatState(){
        if (ChooseTypeSampleManager.Instance.isToEat(showCard, ChooseTypeSampleManager.TYPE_SKILL_EXP)) {
            typeProject[0].SetActive(false);
            typeProject[1].SetActive(true);
            projectDesc.text = LanguageConfigManager.Instance.getLanguage("JustToEat01", showCard.getEatenExp().ToString());
            return false;
        } else if (ChooseTypeSampleManager.Instance.isToEat(showCard, ChooseTypeSampleManager.TYPE_ADDON_NUM)) {
            typeProject[0].SetActive(false);
            typeProject[1].SetActive(true);
            projectDesc.text = LanguageConfigManager.Instance.getLanguage("JustToEat04");
            return false;
        } else if (ChooseTypeSampleManager.Instance.isToEat(showCard, ChooseTypeSampleManager.TYPE_MONEY_NUM)) {
            typeProject[0].SetActive(false);
            typeProject[1].SetActive(true);
            projectDesc.text = LanguageConfigManager.Instance.getLanguage("JustToEat03", showCard.getSellPrice().ToString());
            return false;
        } else {
            typeProject[0].SetActive(true);
            typeProject[1].SetActive(false);
        }
        return true;
    }

    //初始化技能
    private void updateSkill() {
        root_passiveSkillGroup.SetActive(false);
        if (!checkEatState())return;
        //获得3类技能
        Skill[] mainSkil = showCard.getSkills();
        Skill[] temattrSkill = showCard.getAttrSkills();
        //这里在被动技能还要加上血脉给的
        Skill[] attrSkill;
        if (CardSampleManager.Instance.getRoleSampleBySid(showCard.sid).bloodPointSid != 0)
        {
            List<Skill> bloodSkills = BloodConfigManager.Instance.isActiveSkillSid(showCard.sid, showCard.cardBloodLevel);
            foreach (Skill skill in bloodSkills) {
            }
            if (bloodSkills != null && bloodSkills.Count > 0)
            {
                Skill[] tem;
                if (temattrSkill != null && temattrSkill.Length > 0)
                    tem = new Skill[bloodSkills.Count + temattrSkill.Length];
                else
                    tem = new Skill[bloodSkills.Count];
                for (int s = 0; s < bloodSkills.Count; s++)
                {
                    tem[s] = bloodSkills[s];
                }
                if (temattrSkill != null && temattrSkill.Length > 0)
                {
                    for (int k = 0; k < temattrSkill.Length; k++)
                    {
                        tem[bloodSkills.Count + k] = temattrSkill[k];
                    }
                }
                attrSkill = tem;
                //如果替换的技能不是在血脉中激活的，就会走下面，干掉被替换的技能
                List<Skill> tempSkill = new List<Skill>();
                for (int i = 0; i < attrSkill.Length; i++) {
                    tempSkill.Add(attrSkill[i]);
                }
                List<Skill> isReplacedSkill = BloodConfigManager.Instance.getIsReplacedSkill(showCard.sid, showCard.cardBloodLevel);
                for (int i = 0; i < isReplacedSkill.Count; i++) {
                    for (int j = 0; j < tempSkill.Count; j++) {
                        if (isReplacedSkill[0].sid == tempSkill[j].sid) {
                            tempSkill.Remove(temattrSkill[j]);
                            break;
                        }
                    }
                }
                attrSkill = tempSkill.ToArray();
            }
            else
            {
                attrSkill = temattrSkill;
            }
        }
        else
        {
            attrSkill = temattrSkill;
        }
        
        Skill[] buffSkill = showCard.getBuffSkills();

        //已开放技能槽位
        //		int[] max = showCard.getSkillMaxSlot ();
        //		int buffSkillSlot = max [0];//开场
        //		int mainSkillSlot = max [1];//主动
        //		int attrSkillSlot = max [2];//被动
        //
        //		int canLearnCount = mainSkillSlot + buffSkillSlot;

        //以上注释是因为前台现在不能学技能了，所以不需要技能槽位判断

        List<Skill> tmpSkills = new List<Skill>();
        if (mainSkil != null) {
            for (int i = 0; i < mainSkil.Length; i++) {
                tmpSkills.Add(mainSkil[i]);
            }
        }
        if (buffSkill != null) {
            for (int i = 0; i < buffSkill.Length; i++) {
                tmpSkills.Add(buffSkill[i]);
            }
        }
        //主动技能显示
        if (tmpSkills != null && tmpSkills.Count > 0) {
            GameObject passiveSkill;
            ButtonSkill skillComponent;

            if (mainSkillList == null) {
                mainSkillList = new List<ButtonSkill>();
            } else {
                for (int i = 0; i < mainSkillList.Count; i++) {
                    mainSkillList[i].gameObject.SetActive(false);
                }
            }

            for (int i = 0, j = 0; i < tmpSkills.Count; i++) {
                if (tmpSkills[i].getShowType() == 2) {
                    continue;
                }
                //如果缓存有，就读缓存，没有就创建出来丢进缓存
                if (j >= mainSkillList.Count) {
                    passiveSkill = NGUITools.AddChild(root_mainSkill.gameObject, skillObj);
                    passiveSkill.SetActive(true);
                    passiveSkill.transform.localScale = Vector3.one;
                    passiveSkill.transform.localPosition = new Vector3(j * 270, 0f, 0f);
                    skillComponent = passiveSkill.GetComponent<ButtonSkill>();
                    skillComponent.fatherWindow = this;
                    mainSkillList.Add(skillComponent);
                }
                mainSkillList[j].gameObject.SetActive(true);
                mainSkillList[j].initSkillData(tmpSkills[i], ButtonSkill.STATE_LEARNED);

                j++;
            }
        }
        

        //被动技能显示
        if (attrSkill != null && attrSkill.Length > 0) 
        {
            bool isShowAttrTitle = false;
            GameObject passiveSkill;
            ButtonSkill skillComponent;

            if (attrSkillList == null) {
                attrSkillList = new List<ButtonSkill>();
            } else {
                for (int i = 0; i < attrSkillList.Count; i++) {
                    attrSkillList[i].gameObject.SetActive(false);
                }
            }

            for (int i = 0, j = 0; i < attrSkill.Length; i++) {
                if (attrSkill[i].getShowType() == 2) {
                    continue;
                }
                //如果缓存有，就读缓存，没有就创建出来丢进缓存
                if (j >= attrSkillList.Count) {
                    passiveSkill = NGUITools.AddChild(root_passiveSkill.gameObject, passiveSkillObj);
                    passiveSkill.SetActive(true);
                    passiveSkill.transform.localScale = Vector3.one;
                    passiveSkill.transform.localPosition = new Vector3(j * 80, 0f, 0f);
                    skillComponent = passiveSkill.GetComponent<ButtonSkill>();
                    attrSkillList.Add(skillComponent);
                }
                attrSkillList[j].gameObject.SetActive(true);
                attrSkillList[j].initSkillData(attrSkill[i], ButtonSkill.STATE_LEARNED);

                j++;
                isShowAttrTitle = true;
            }
            skillQuality.spriteName = "";
            if (CardSampleManager.Instance.getRoleSampleBySid(showCard.sid).qualityId == QualityType.MYTH) {
                skillQuality.spriteName = "icon_mythSkillTitle";//神话级别的卡，显示神话特技标签
            } else if (CardSampleManager.Instance.getRoleSampleBySid(showCard.sid).qualityId == QualityType.LEGEND) {
                skillQuality.spriteName = "chuanqiteji";//
            } else if (CardSampleManager.Instance.getRoleSampleBySid(showCard.sid).qualityId == QualityType.EPIC) {
                skillQuality.spriteName = "shishiteji";//
            }

            root_passiveSkillGroup.SetActive(isShowAttrTitle);
        }
        else 
        {
            root_passiveSkillGroup.SetActive(false);
        }
    }



    //显示角色属性变化 用于穿装脱装后
    public void equipNewItem2(List<AttrChange> atts, int type) {
        //强化装备后步进到关闭窗口
        if (GuideManager.Instance.isEqualStep(123004000)) {
            GuideManager.Instance.doGuide();
            GuideManager.Instance.guideEvent();
        }
        updateEquip();
        updateAttributes();
        UiManager.Instance.openDialogWindow<MessageLineWindow>((win) => {
            if (type == CardBookWindow.ONEKEYEQUIP) {
                win.Initialize(LanguageConfigManager.Instance.getLanguage("s0088"));
            } else if (type == CardBookWindow.RELOADEQUIP) {
                win.Initialize(LanguageConfigManager.Instance.getLanguage("s0087"));
            } else if (type == CardBookWindow.UNDRESS) {
                win.Initialize(LanguageConfigManager.Instance.getLanguage("s0091"));
            }
            for (int i = 0; i < atts.Count; i++) {
                if (atts[i].num > 0) {
                    win.Initialize(LanguageConfigManager.Instance.getLanguage("s0089") + atts[i].typeToString() + atts[i].num);
                } else if (atts[i].num < 0) {
                    win.Initialize(LanguageConfigManager.Instance.getLanguage("s0090") + atts[i].typeToString() + atts[i].num);
                }
            }
        });

        //更新其他卡牌状态
        IncreaseManagerment.Instance.getAllEquips();
        updateEuipEnableStatus();
    }

    void updateEquip() {
        if (CardManagerment.Instance.isMyCard(showCard) && showCard.uid != "-1" && showType != CardBookWindow.CLICKCHATSHOW) {
            showCard = StorageManagerment.Instance.getRole(showCard.uid);
        }

        for (int i = 0; i < equipItem.Length; i++) {
            if (showType == CardBookWindow.SHOW || showType == CardBookWindow.CHATSHOW || showType == CardBookWindow.CLICKCHATSHOW)
                equipItem[i].initInto(EquipAttrWindow.OTHER);
            else
                equipItem[i].initInto(EquipAttrWindow.CARDVIEW);

            equipItem[i].updateEquip(null);
        }

        string[] ids = showCard.getEquips();

        if (ids != null && showType != CardBookWindow.CLICKCHATSHOW) {
            for (int i = 0; i < ids.Length; i++) {
                Equip eq = StorageManagerment.Instance.getEquip(ids[i]);
                if (eq.getPartId() == EquipPartType.WEAPON) {
                    equipItem[0].updateEquip(eq);
                }
                if (eq.getPartId() == EquipPartType.ARMOUR) {
                    equipItem[1].updateEquip(eq);
                }
                if (eq.getPartId() == EquipPartType.SHOSE) {
                    equipItem[2].updateEquip(eq);
                }
                if (eq.getPartId() == EquipPartType.HELMET) {
                    equipItem[3].updateEquip(eq);
                }
                if (eq.getPartId() == EquipPartType.RING) {
                    equipItem[4].updateEquip(eq);
                }
            }
        } else if (showChatEquips != null) {
            //CardManagerment.Instance.showChatEquips = showChatEquips;
            CardManagerment.Instance.showChatEquips.Clear();
            for (int i = 0; i < showChatEquips.Count; i++) {
                Equip tmp = new Equip(showChatEquips[i].uid, showChatEquips[i].sid, showChatEquips[i].exp, showChatEquips[i].state, showChatEquips[i].equpStarState,0);
                CardManagerment.Instance.showChatEquips.Add(tmp);
                if (showChatEquips[i].getPartId() == EquipPartType.WEAPON) {
                    equipItem[0].updateEquip(showChatEquips[i]);
                }
                if (showChatEquips[i].getPartId() == EquipPartType.ARMOUR) {
                    equipItem[1].updateEquip(showChatEquips[i]);
                }
                if (showChatEquips[i].getPartId() == EquipPartType.SHOSE) {
                    equipItem[2].updateEquip(showChatEquips[i]);
                }
                if (showChatEquips[i].getPartId() == EquipPartType.HELMET) {
                    equipItem[3].updateEquip(showChatEquips[i]);
                }
                if (showChatEquips[i].getPartId() == EquipPartType.RING) {
                    equipItem[4].updateEquip(showChatEquips[i]);
                }
            }
        }
    }
    /** 加载主卡突破天赋 */
    void loadMainCardTalent() {
        string[][] strs = SurmountManagerment.Instance.getAddEffectByStringByAll(showCard);
        if (strs == null || strs.Length == 0)
            return;

        CardAttrTalentItem tempTalent;
        if (talentList == null) {
            talentList = new List<CardAttrTalentItem>();
        } else {
            for (int i = 0; i < talentList.Count; i++) {
                talentList[i].gameObject.SetActive(false);
                talentList[i].text1.text = "";
                talentList[i].text2.text = "";
            }
        }
        for (int i = 0; i < strs.Length; i++) {
            //如果缓存有，就读缓存，没有就创建出来丢进缓存
            if (i >= talentList.Count) {
                tempTalent = NGUITools.AddChild(talentContent, talentPrefab).GetComponent<CardAttrTalentItem>();
                tempTalent.transform.localPosition = new Vector3(0, i * -115, 0);
                tempTalent.transform.localScale = Vector3.one;
                talentList.Add(tempTalent);
            }
            talentList[i].gameObject.SetActive(true);
            if (strs[i].Length == 1)
                talentList[i].text2.text = strs[i][0];
            else if (strs[i].Length == 2)
                talentList[i].text2.text = strs[i][0] + strs[i][1];

            if (showCard.getSurLevel() - 1 < i) {
                talentList[i].text1.text = "[C65843]" + string.Format(LanguageConfigManager.Instance.getLanguage("s0446"), i + 1);
            } else {
                talentList[i].text1.text = Colors.CHAT_WORLD + Language("goddess11");
            }
        }
        talentBg.height = 25 + 115 * strs.Length;
    }
    /// <summary>
    /// 加载普通卡进化天赋
    /// </summary>
    void loadTalent() {
        EvolutionSample sample = EvolutionManagerment.Instance.getEvoInfoByType(showCard);
        if (sample == null)
            return;
        string[] strs = sample.getAddTalentString();
        if (strs == null || strs.Length == 0)
            return;
        CardAttrTalentItem tempTalent;
        if (talentList == null) {
            talentList = new List<CardAttrTalentItem>();
        } else {
            for (int i = 0; i < talentList.Count; i++) {
                talentList[i].gameObject.SetActive(false);
                talentList[i].text1.text = "";
                talentList[i].text2.text = "";
            }
        }
        for (int i = 0, j = 0; i < strs.Length; i++) {
            //如果缓存有，就读缓存，没有就创建出来丢进缓存
            if (i >= talentList.Count) {
                tempTalent = NGUITools.AddChild(talentContent, talentPrefab).GetComponent<CardAttrTalentItem>();
                tempTalent.transform.localPosition = new Vector3(0, i * -115, 0);
                tempTalent.transform.localScale = Vector3.one;
                talentList.Add(tempTalent);
            }
            talentList[i].gameObject.SetActive(true);
            talentList[i].text2.text = strs[j];
            if (sample.getTalentNeedTimes()[j] > showCard.getEvoLevel()) {
                talentList[i].text1.text = "[C65843]" + Language("s0386", sample.getTalentNeedTimes()[j]);
            } else {
                talentList[i].text1.text = Colors.CHAT_WORLD + Language("goddess11");
            }
            j++;
        }
        talentBg.height = 25 + 115 * strs.Length;
    }

    private void hideAllEquipButton() {
        foreach (ButtonEquipInCardBook each in equipItem) {
            each.gameObject.SetActive(false);
        }
    }

    public void InitializeCardInfo(Card showCard, int type) {
        showType = type;
        updateAll();
    }

    public void InitializeCardInfo(ServerCardMsg showCard, int type) {
        showType = type;
        //	this.closeCallback = closeCallback;
        //	this.showCard = showCard.card;
        this.showChatEquips = showCard.showEquips;
        this.showCardNet = showCard;
        updateAllServer();
    }

    public void updateAll() {
        //更新标题栏和职业栏
        int jobId = getShowCard().getJob();
        jobSprite.spriteName = CardManagerment.Instance.qualityIconTextToBackGround(jobId) + "s";
        //属性界面“力”系背景（力系和毒系职业用）
        if (jobId == 1 || jobId == 4) {
            ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.BACKGROUNDPATH + "card_topBackGround_1", topBackGround);
        }
            //属性界面“敏”系背景（反和敏职业用）
        else if (jobId == 3 || jobId == 5) {
            ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.BACKGROUNDPATH + "card_topBackGround_2", topBackGround);
        }
            //属性界面“魔”系背景（魔和辅职业用）
        else {
            ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.BACKGROUNDPATH + "card_topBackGround_3", topBackGround);
        }

        UpdateQualityEffect();

        //1.进化后uid不变，形象改变 2.展示一张未获得的卡片 3别人的卡也有uid
        if (!showCard.uid.Equals("") && showType != CardBookWindow.CLICKCHATSHOW && showCard.uid != "-1") {
            Card cardTemp = StorageManagerment.Instance.getRole(showCard.uid);
            if (cardTemp != null)
                showCard = cardTemp;
        }
        nameLabel.text = QualityManagerment.getQualityColor(getShowCard().getQualityId()) + getShowCard().getName();
        //8005000学妹上阵,8008000亚瑟王上阵,12004000强化卡片(祭品)
        //123004000一键穿装,124004000强化装备
        //12004000献祭强化,129005000进化强化,112004000附加强化
        if (GuideManager.Instance.isEqualStep(8005000) || GuideManager.Instance.isEqualStep(8008000) || GuideManager.Instance.isEqualStep(12004000)
            || GuideManager.Instance.isEqualStep(123004000) || GuideManager.Instance.isEqualStep(124004000)
            || GuideManager.Instance.guideSid == 129005000 || GuideManager.Instance.isEqualStep(112004000)) {
            GuideManager.Instance.guideEvent();
            drag.enabled = false;
        }
        GuideManager.Instance.doFriendlyGuideEvent();

        expbar.reset();
        if (showCard == null)
            ResourcesManager.Instance.LoadAssetBundleTexture("texture/card/1", CardImage);
        else
            ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.CARDIMAGEPATH + showCard.getImageID(), CardImage);

        quality.spriteName = QualityManagerment.qualityIDToString(showCard.getQualityId()) + "Bg";
        quality.alpha = 1;
        showStar();


        updateAttributes();
        int maxLv = showCard.getMyMaxLevel();
        int roleCardLv = StorageManagerment.Instance.getRole(UserManager.Instance.self.mainCardUid).getLevel();
        int lv = showCard.getLevel();
        cardLv.text = "Lv." + lv + "/" + maxLv;

        expbar.updateValue(EXPSampleManager.Instance.getNowEXPShow(showCard.getEXPSid(), showCard.getEXP()), EXPSampleManager.Instance.getMaxEXPShow(showCard.getEXPSid(), showCard.getEXP()));
        //显示血脉等级
        bloodPoint.gameObject.SetActive(false);
        if (CardSampleManager.Instance.checkBlood(showCard.sid, showCard.uid)) {
            bloodPoint.gameObject.SetActive(true);
            bloodLv.text = "[FF0000]"+ LanguageConfigManager.Instance.getLanguage("bloodDesc1");
            if (showCard.cardBloodLevel == BloodConfigManager.Instance.getTotalItemNum(showCard.sid) && showCard.getQualityId() == QualityType.MYTH) {
                bloodLv.text = "[FF0000]"+ LanguageConfigManager.Instance.getLanguage("prefabzc31", (BloodConfigManager.Instance.getCurrentBloodMap(showCard.sid, showCard.cardBloodLevel).Length + ""));
            } else if (showCard.cardBloodLevel != 0) {
                int[] colorQuliq = BloodConfigManager.Instance.getCurrentBloodLvColor(showCard.sid, showCard.cardBloodLevel);
                bloodLv.text = QualityManagerment.getQualityColorForlv(colorQuliq[0], colorQuliq[1]);
            } 
        }
        jinhuaTitle.spriteName = showCard.isMainCard() ? "attr_ev" : "attr_ev";
        if (EvolutionManagerment.Instance.getMaxLevel(showCard) == 0) {
            jinhua.text = LanguageConfigManager.Instance.getLanguage("Evo10");
        } else {
            if (showCard.isMainCard())
                jinhua.text = showCard.getSurLevel() + "/" + SurmountManagerment.Instance.getMaxSurLevel(showCard);
            else
                jinhua.text = showCard.getEvoLevel() + "/" + showCard.getMaxEvoLevel();
        }
        updateSkill();
        updateEquip();
        initButton();
        if (showCard.isMainCard())
            //UpdateCardAddEffect ();
            loadMainCardTalent();
        else
            loadTalent();
        //UpdateTalent ();
        //浏览到下一个卡牌的时需要重置容器的位置
        SpringPanel.Begin(panlObj.gameObject, new Vector3(0, 0, 0), 9);
        updateMagicWeaponButton();
        if (UserManager.Instance.self.getUserLevel() >= 30 && !isSpriteCard(showCard))
        {
            if (showType == 1 || showType == 2 || showType == 5 || showType == 3 || showType == 6)
            {
                if (!ChooseTypeSampleManager.Instance.isToEat(showCard))
                {
                    selectResonance.gameObject.SetActive(true);
                }
            }
            else
            {
                selectResonance.gameObject.SetActive(true);
                selectResonance.GetComponent<Collider>().enabled = false;
            }
        }
        else
        {
            selectResonance.gameObject.SetActive(false);
        }
        if (UserManager.Instance.self.getUserLevel() > 34 && !isSpriteCard(showCard))//判断等级和是否有特殊用途
        {
            if (showType == 1 || showType == 2 || showType == 5 || showType == 3 || showType == 6)
            {
                if (!ChooseTypeSampleManager.Instance.isToEat(showCard))
                {
                    selsecStarSoulButton.gameObject.SetActive(true);
                }
            }
            else
            {
                selsecStarSoulButton.gameObject.SetActive(true);
                selsecStarSoulButton.GetComponent<Collider>().enabled = false;
            }
        }
        else
        {
            selsecStarSoulButton.gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// 看人家的卡片数据
    /// </summary>
    public void updateAllServer()
    {
        //更新标题栏和职业栏
        int jobId = getShowCard().getJob();
        jobSprite.spriteName = CardManagerment.Instance.qualityIconTextToBackGround(jobId) + "s";
        //属性界面“力”系背景（力系和毒系职业用）
        if (jobId == 1 || jobId == 4)
        {
            ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.BACKGROUNDPATH + "card_topBackGround_1", topBackGround);
        }
        //属性界面“敏”系背景（反和敏职业用）
        else if (jobId == 3 || jobId == 5)
        {
            ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.BACKGROUNDPATH + "card_topBackGround_2", topBackGround);
        }
        //属性界面“魔”系背景（魔和辅职业用）
        else
        {
            ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.BACKGROUNDPATH + "card_topBackGround_3", topBackGround);
        }

        UpdateQualityEffect();

        //1.进化后uid不变，形象改变 2.展示一张未获得的卡片 3别人的卡也有uid
        if (!showCard.uid.Equals("") && showType != CardBookWindow.CLICKCHATSHOW && showCard.uid != "-1")
        {
            Card cardTemp = StorageManagerment.Instance.getRole(showCard.uid);
            if (cardTemp != null)
                showCard = cardTemp;
        }
        nameLabel.text = QualityManagerment.getQualityColor(getShowCard().getQualityId()) + getShowCard().getName();
        //8005000学妹上阵,8008000亚瑟王上阵,12004000强化卡片(祭品)
        //123004000一键穿装,124004000强化装备
        //12004000献祭强化,129005000进化强化,112004000附加强化
        if (GuideManager.Instance.isEqualStep(8005000) || GuideManager.Instance.isEqualStep(8008000) || GuideManager.Instance.isEqualStep(12004000)
            || GuideManager.Instance.isEqualStep(123004000) || GuideManager.Instance.isEqualStep(124004000)
            || GuideManager.Instance.guideSid == 129005000 || GuideManager.Instance.isEqualStep(112004000))
        {
            GuideManager.Instance.guideEvent();
            drag.enabled = false;
        }
        GuideManager.Instance.doFriendlyGuideEvent();

        expbar.reset();
        if (showCard == null)
            ResourcesManager.Instance.LoadAssetBundleTexture("texture/card/1", CardImage);
        else
            ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.CARDIMAGEPATH + showCard.getImageID(), CardImage);

        quality.spriteName = QualityManagerment.qualityIDToString(showCard.getQualityId()) + "Bg";
        quality.alpha = 1;
        showStar();


        updateAttributes();
        int maxLv = showCard.getMyMaxLevel();
        int roleCardLv = StorageManagerment.Instance.getRole(UserManager.Instance.self.mainCardUid).getLevel();
        int lv = showCard.getLevel();
        cardLv.text = "Lv." + lv + "/" + maxLv;

        expbar.updateValue(EXPSampleManager.Instance.getNowEXPShow(showCard.getEXPSid(), showCard.getEXP()), EXPSampleManager.Instance.getMaxEXPShow(showCard.getEXPSid(), showCard.getEXP()));
        //显示血脉等级
        bloodPoint.gameObject.SetActive(false);
        if (CardSampleManager.Instance.checkBlood(showCard.sid, showCard.uid))
        {
            bloodPoint.gameObject.SetActive(true);
            bloodLv.text = "[FF0000]" + LanguageConfigManager.Instance.getLanguage("bloodDesc1");
            if (showCard.cardBloodLevel == BloodConfigManager.Instance.getTotalItemNum(showCard.sid) && showCard.getQualityId() == QualityType.MYTH)
            {
                bloodLv.text = "[FF0000]" + LanguageConfigManager.Instance.getLanguage("prefabzc31", (BloodConfigManager.Instance.getCurrentBloodMap(showCard.sid, showCard.cardBloodLevel).Length + ""));
            }
            else if (showCard.cardBloodLevel != 0)
            {
                int[] colorQuliq = BloodConfigManager.Instance.getCurrentBloodLvColor(showCard.sid, showCard.cardBloodLevel);
                bloodLv.text = QualityManagerment.getQualityColorForlv(colorQuliq[0], colorQuliq[1]);
            }
        }
        jinhuaTitle.spriteName = showCard.isMainCard() ? "attr_ev" : "attr_ev";
        if (EvolutionManagerment.Instance.getMaxLevel(showCard) == 0)
        {
            jinhua.text = LanguageConfigManager.Instance.getLanguage("Evo10");
        }
        else
        {
            if (showCard.isMainCard())
                jinhua.text = showCard.getSurLevel() + "/" + SurmountManagerment.Instance.getMaxSurLevel(showCard);
            else
                jinhua.text = showCard.getEvoLevel() + "/" + showCard.getMaxEvoLevel();
        }
        updateSkill();
        updateEquip();
        initButton();
        if (showCard.isMainCard())
            //UpdateCardAddEffect ();
            loadMainCardTalent();
        else
            loadTalent();
        //UpdateTalent ();
        //浏览到下一个卡牌的时需要重置容器的位置
        SpringPanel.Begin(panlObj.gameObject, new Vector3(0, 0, 0), 9);
        updateMagicWeaponButtonServer();
        //if (UserManager.Instance.self.getUserLevel() > 30 && !isSpriteCard(showCard))
        //{
        //    if (showType == 1 || showType == 2 || showType == 5 || showType == 3 || showType == 6)
        //    {
        //        if (!ChooseTypeSampleManager.Instance.isToEat(showCard))
        //        {
                    selectResonance.gameObject.SetActive(true);
                    selectResonanceLock.gameObject.SetActive(false);
                //}
            //}
        //    else
        //    {
        //        selectResonance.gameObject.SetActive(true);
        //        selectResonance.GetComponent<Collider>().enabled = false;
        //    }
        //}
        //else
        //{
        //    selectResonance.gameObject.SetActive(false);
        //}
        //if (UserManager.Instance.self.getUserLevel() > 34 && !isSpriteCard(showCard))//判断等级和是否有特殊用途
        //{
        //    if (showType == 1 || showType == 2 || showType == 5 || showType == 3 || showType == 6)
        //    {
        //        if (!ChooseTypeSampleManager.Instance.isToEat(showCard))
        //        {
                    selsecStarSoulButton.gameObject.SetActive(true);
                    selecStarSoulLock.gameObject.SetActive(false);
        //        }
        //    }
        //    else
        //    {
        //        selsecStarSoulButton.gameObject.SetActive(true);
        //        selsecStarSoulButton.GetComponent<Collider>().enabled = false;
        //    }
        //}
        //else
        //{
        //    selsecStarSoulButton.gameObject.SetActive(false);
        //}
    }
    /// <summary>
    /// 查看别人卡数据-神器
    /// </summary>
    void updateMagicWeaponButtonServer()
    {
        Utils.RemoveAllChild(magicWeaponIcon.transform);
        if (showType == CardBookWindow.CLICKCHATSHOW)
        {//从聊天界面过来对秘宝的判断
            if (showCardNet.card.magicWeaponUID != null && showCardNet.card.magicWapon!=null)
            {
                selsectMagicWeaponButton.gameObject.SetActive(true);
                selectMagicWeaponLock.gameObject.SetActive(false);
                //selsectMagicWeaponButton.gameObject.GetComponent<Collider>().enabled = false;
                MagicWeapon mw = StorageManagerment.Instance.getMagicWeapon(showCardNet.card.magicWeaponUID);
                ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.ICONIMAGEPATH + showCardNet.card.magicWapon.getIconId(), magicWeaponIcon);
                if (starsPrefab != null)
                {
                    Utils.RemoveAllChild(magicWeaponIcon.transform);
                    if (showCardNet.card.magicWapon == null) return;
                    MagicWeaponSample sample = MagicWeaponSampleManager.Instance.getMagicWeaponSampleBySid(showCardNet.card.magicWapon.sid);
                    GameObject stars = NGUITools.AddChild(magicWeaponIcon.gameObject, starsPrefab);
                    ShowStars show = stars.GetComponent<ShowStars>();
                    if (sample != null && sample.starLevel > 0)
                    {
                        show.initStar(sample.starLevel, MagicWeaponManagerment.USEDBYMAGICITEM);
                    }
                }
            }
            else
            {
                selsectMagicWeaponButton.gameObject.SetActive(true);
                selsectMagicWeaponButton.gameObject.GetComponent<Collider>().enabled = false;
                selectMagicWeaponLock.gameObject.SetActive(false);

            }
        }
    }

    /// <summary>
    /// 卡片星级星星显示
    /// </summary>
    public void showStar() {
        if (starContent != null) {
            for (int i = 0; i < star.Length; i++) {
                star[i].SetActive(false);
                star[i].gameObject.transform.localPosition = new Vector3(-30 + i * 20, -30, 0);
            }
            starContent.transform.localPosition = new Vector3(0, 0, 0);
            int cardStarLevel = CardSampleManager.Instance.getStarLevel(showCard.sid);
            for (int i = 0; i < cardStarLevel; i++) {
                star[i].SetActive(true);
            }
            if (cardStarLevel == CardSampleManager.ONESTAR) {
                starContent.transform.localPosition = new Vector3(32, 0, 0);
            } else if (cardStarLevel == CardSampleManager.TWOSTAR) {
                starContent.transform.localPosition = new Vector3(21, 0, 0);
            } else if (cardStarLevel == CardSampleManager.THREESTAR) {
                starContent.transform.localPosition = new Vector3(11, 0, 0);
            } else if (cardStarLevel == CardSampleManager.FOURSTAR) {
                starContent.transform.localPosition = new Vector3(0, 0, 0);
                for (int i = 0; i < cardStarLevel; i++) {
                    star[i].gameObject.transform.localPosition = new Vector3(-25 + i*16,-30,0);
                }
            }
        }
    }
    private void updateAttributes() {
        if (showType == CardBookWindow.CLICKCHATSHOW) {
            combat.text = showCard.CardCombat.ToString();
            hp.text = showCard.getHPExp().ToString();
            att.text = showCard.getATTExp().ToString();
            def.text = showCard.getDEFExp().ToString();
            mag.text = showCard.getMAGICExp().ToString();
            dex.text = showCard.getAGILEExp().ToString();
        } else {
            rushCombat();
            CardBaseAttribute attr = CardManagerment.Instance.getCardWholeAttr(showCard);
            hp.text = attr.getWholeHp().ToString();
            att.text = attr.getWholeAtt().ToString();
            def.text = attr.getWholeDEF().ToString();
            mag.text = attr.getWholeMAG().ToString();
            dex.text = attr.getWholeAGI().ToString();

            addValues = new int[5];
            currentValues = new int[5];
            oldValues = new int[5];
            currentValues[0] = attr.getWholeHp();
            currentValues[1] = attr.getWholeAtt();
            currentValues[2] = attr.getWholeDEF();
            currentValues[3] = attr.getWholeMAG();
            currentValues[4] = attr.getWholeAGI();

            SwitchShow();
        }

    }

    private void SwitchShow() {
        if (showType == CardBookWindow.CLICKCHATSHOW) {
            return;
        }
        CardBaseAttribute attr2 = CardManagerment.Instance.getCardAppendEffectNoSuit(showCard);
        if (showAttrTime == true) {

            hpAdd.text = attr2.getWholeHp() == 0 ? "" : (" + " + attr2.getWholeHp().ToString());
            attAdd.text = attr2.getWholeAtt() == 0 ? "" : (" + " + attr2.getWholeAtt().ToString());
            defAdd.text = attr2.getWholeDEF() == 0 ? "" : (" + " + attr2.getWholeDEF().ToString());
            magAdd.text = attr2.getWholeMAG() == 0 ? "" : (" + " + attr2.getWholeMAG().ToString());
            dexAdd.text = attr2.getWholeAGI() == 0 ? "" : (" + " + attr2.getWholeAGI().ToString());
        } else {
            string fj = LanguageConfigManager.Instance.getLanguage("s0141");
            int hpl = CardManagerment.Instance.getCardAttrAppendLevel(showCard, AttributeType.hp);
            hpAdd.text = hpl == 0 ? "" : (fj + "Lv." + hpl);

            int attl = CardManagerment.Instance.getCardAttrAppendLevel(showCard, AttributeType.attack);
            attAdd.text = attl == 0 ? "" : (fj + "Lv." + attl);

            int defl = CardManagerment.Instance.getCardAttrAppendLevel(showCard, AttributeType.defecse);
            defAdd.text = defl == 0 ? "" : (fj + "Lv." + defl);

            int magl = CardManagerment.Instance.getCardAttrAppendLevel(showCard, AttributeType.magic);
            magAdd.text = magl == 0 ? "" : (fj + "Lv." + magl);

            int dexl = CardManagerment.Instance.getCardAttrAppendLevel(showCard, AttributeType.agile);
            dexAdd.text = dexl == 0 ? "" : (fj + "Lv." + dexl);
        }
    }

    int oldCombat = 0;//初始战斗力
    int newCombat = 0;//最新战斗力
    int step;//步进跳跃值
    private bool isRefreshCombat = false;//刷新战斗力开关

    //刷新战斗力
    public void rushCombat() {
        if (showCard == null)
            return;
        newCombat = showCombat > 0 ? showCombat : showCard.getCardCombat();

        isRefreshCombat = true;
        if (newCombat >= oldCombat)
            step = (int)((float)(newCombat - oldCombat) / 20);
        else
            step = (int)((float)(oldCombat - newCombat) / 20);
        if (step < 1)
            step = 1;
    }

    private void refreshCombat() {
        if (oldCombat != newCombat) {
            if (oldCombat < newCombat) {
                oldCombat += step;
                if (oldCombat >= newCombat)
                    oldCombat = newCombat;
            } else if (oldCombat > newCombat) {
                oldCombat -= step;
                if (oldCombat <= newCombat)
                    oldCombat = newCombat;
            }
            combat.text = oldCombat + "";
        } else {
            isRefreshCombat = false;
            combat.text = newCombat + "";
            oldCombat = newCombat;
        }
    }

    float time; //属性加成切换显示的时间
    public const float SWITCHTIME = 3f;
    bool showAttrTime = false;//true就显示附加属性数据，false显示附加等级

    void Update() {

        if (showCard != null) {
            time -= Time.deltaTime;

            if (time <= 0) {
                showAttrTime = !showAttrTime;
                time = SWITCHTIME;
                SwitchShow();
            }
        }

        if (isRefreshCombat) {
            refreshCombat();
        }

        updateTalentArrow();
    }

    IEnumerator StartEffectLabels() {
        yield return new WaitForSeconds(0.4f);
        for (int i = 0; i < addValueLabels.Length; i++) {
            playerEffect(addLabels[i], addValueLabels[i], addValues[i]);
            yield return new WaitForSeconds(0.1f);
            TweenLabelNumber tss = TweenLabelNumber.Begin(totleValueLabels[i].gameObject, 0.3f, currentValues[i]);
            tss.from = oldValues[i];
            yield return new WaitForSeconds(0.1f);
        }
    }

    int nextSetp = 0;

    private void playerEffect(UILabel _labelTitle, UILabel _labelDesc, int _desc) {
        _labelTitle.text = "+";
        TweenScale ts = TweenScale.Begin(_labelTitle.gameObject, 0.3f, Vector3.one);
        ts.method = UITweener.Method.EaseIn;
        ts.from = new Vector3(5, 5, 1);
        _labelDesc.color = new Color(255, 0, 0);
        _labelDesc.text = "";
        TweenScale ts2 = TweenScale.Begin(_labelDesc.gameObject, 0.3f, Vector3.one);
        ts2.method = UITweener.Method.EaseIn;
        ts2.from = new Vector3(5, 5, 1);
        EventDelegate.Add(ts2.onFinished, () => {
            TweenLabelNumber tln = TweenLabelNumber.Begin(_labelDesc.gameObject, 0.1f, _desc);
            tln.from = 0;
            EventDelegate.Add(tln.onFinished, () => {
                GameObject obj = Create3Dobj("Effect/Other/flash").obj;
                obj.transform.parent = _labelDesc.transform;
                obj.transform.localScale = Vector3.one;
                obj.transform.localPosition = new Vector3(0, 0, -600);
                StartCoroutine(Utils.DelayRun(() => {
                    nextSetp++;
                }, 0.1f));
            }, true);
        }, true);
    }

    public override void buttonEventBase(GameObject gameObj) {
        if (gameObj.name == "buttontemp")
        {
            UiManager.Instance.openWindow<ResonanceWindow>((win) =>
            {
                win.init(showCard.uid,showType);
            });
        }
        else  if (gameObj.name == "buttonOther") {
            if (showType == CardBookWindow.CARDCHANGE) {

                if (ArmyManager.Instance.isEditArmyActive()) {
					//公会战队伍单独处理响应弹窗
					if(ArmyManager.Instance.ActiveEditArmy.armyid == ArmyManager.PVP_GUILD)
					{
						UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage("GuildArea_102"));
					}
					else
					{                
						if(ArmyManager.Instance.teamEditInMissonWin)
						{
							UiManager.Instance.openDialogWindow<MessageWindow>((win) => {
								win.initWindow(1, LanguageConfigManager.Instance.getLanguage("teamEdit_err03"), "", LanguageConfigManager.Instance.getLanguage("teamEdit_err02"), null);
							});
						}
						else
						{
							//					TextTipWindow.Show(LanguageConfigManager.Instance.getLanguage("s0408"));
							UiManager.Instance.openDialogWindow<MessageWindow>((win) => {
								win.initWindow(2, LanguageConfigManager.Instance.getLanguage("s0094"), LanguageConfigManager.Instance.getLanguage("teamEdit_err03"),
								               LanguageConfigManager.Instance.getLanguage("teamEdit_err02"), (msgHandle) => {
									if (msgHandle.buttonID == MessageHandle.BUTTON_RIGHT) {
										EventDelegate.Add(OnHide, () => {
											if (fatherWindow is TeamEditWindow) {
												fatherWindow.destoryWindow();
											}
											this.destoryWindow();
										});
										FuBenManagerment.Instance.inToFuben();
									}
								}
								);
							});
						}
					}
                    return;
                }

                //设置当前需要替换的队员
                ArmyManager.Instance.activeInstandCard = showCard;

                (fatherWindow as TeamEditWindow).chooseButton = (fatherWindow as TeamEditWindow).getDstRoleViewWidthCard(showCard);

                //判断是否来自开矿界面

                UiManager.Instance.openWindow<CardChooseWindow>((win) => {
                    win.Initialize(CardChooseWindow.ROLECHOOSE);
                });

            } else if (showType == CardBookWindow.VIEW) {

            } else if (showType == MINING) {
                ArmyManager.Instance.activeInstandCard = showCard;
                UiManager.Instance.openWindow<CardChooseWindow>((win) => {
                    win.Initialize(CardChooseWindow.MINING);
                });
            } else {
                //点上阵处理
                //空位上阵
                if (fatherWindow.GetFatherWindow() is TeamEditWindow) {
                    //jordenwu add
                    //先判断当前编辑队伍 是否含有此张卡
                    Army curEditArmy = ArmyManager.Instance.ActiveEditArmy;
                    if ((curEditArmy != null) && ((curEditArmy.IsHaveSameSIDCard(this.showCard)) || ArmyManager.Instance.IsHaveSameSIDCardInMineralTeam(this.showCard))) {
                        //提示
                        UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage("armyEditeMessage01"));
                        return;
                    }
                    //学妹上阵,亚瑟王上阵,四人阵上阵
                    if (GuideManager.Instance.isEqualStep(8005000) || GuideManager.Instance.isEqualStep(8008000)) {
                        GuideManager.Instance.doGuide();
                    }

                    (fatherWindow.GetFatherWindow() as TeamEditWindow).updateChooseButton(showCard);
                    fatherWindow.destoryWindow();
                }
                    //替换上阵
                else if (fatherWindow.GetFatherWindow() is CardBookWindow) {
                    //
                    //先判断当前编辑队伍 是否含有此张卡
                    Army curEditArmy = ArmyManager.Instance.ActiveEditArmy;
                    if ((curEditArmy != null) && ((curEditArmy.IsHaveSameSIDCard(this.showCard)) || ArmyManager.Instance.IsHaveSameSIDCardInMineralTeam(this.showCard))) {
                        //提示
                        Card instandCard = ArmyManager.Instance.activeInstandCard;
                        //在比较是否与替换的一样
                        if (instandCard.sid == showCard.sid) {
                            ArmyManager.Instance.activeInstandCard = null;
                            (fatherWindow.GetFatherWindow().GetFatherWindow() as TeamEditWindow).updateChooseButton(showCard);
                            GetFatherWindow().GetFatherWindow().destoryWindow();
                            GetFatherWindow().destoryWindow();
                        } else {
                            int[] sids = CardSampleManager.Instance.getRoleSampleBySid(showCard.sid).sameCardSids;
                            for (int i = 0; i < sids.Length; i++)//血脉升级上去的卡片
                              {
                                if (instandCard.sid == sids[i]) {
                                    ArmyManager.Instance.activeInstandCard = null;
                                    (fatherWindow.GetFatherWindow().GetFatherWindow() as TeamEditWindow).updateChooseButton(showCard);
                                    GetFatherWindow().GetFatherWindow().destoryWindow();
                                    GetFatherWindow().destoryWindow();
                                    finishWindow();
                                    return;
                                }
                            }
                            //提示
                            UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage("armyEditeMessage01"));
                            return;
                        }
                        //
                    } else {
                        //
                        (fatherWindow.GetFatherWindow().GetFatherWindow() as TeamEditWindow).updateChooseButton(showCard);
                        GetFatherWindow().GetFatherWindow().destoryWindow();
                        GetFatherWindow().destoryWindow();
                    }
                }
                //
                finishWindow();
            }

        } else if (gameObj.name == "buttonContinue") {

            if (showType == CardBookWindow.CONTINUE) {
                //				window.destoryWindow ();
                //				window.finishWindow ();
                IntensifyCardManager.Instance.setMainCard(showCard);
                if (closeCallback != null)
                    closeCallback();
                closeCallback = null;
            }

        } else if (gameObj.name == "buttonHelp") {
            UiManager.Instance.openDialogWindow<EquipAttrDesWindow>();
        } else if (gameObj.name == "close") {
            if (MissionManager.instance != null) {
                MissionManager.instance.showAll();
                MissionManager.instance.setBackGround();
            }
            //123005000自动穿装备关闭卡片浏览
            if (GuideManager.Instance.isEqualStep(123005000)) {
                ArmyManager.Instance.cleanAllEditArmy();
                IntensifyCardManager.Instance.clearData();
                GuideManager.Instance.doGuide();
                UiManager.Instance.openMainWindow();
                return;
            } else if (showType == CardBookWindow.CONTINUE) {
                UiManager.Instance.openMainWindow();
                return;
            } else if (showType == CardBookWindow.OTHER || showType == CardBookWindow.AWARDINTO) {
                //有其他类型的窗口特殊处理,在这里加相应的分支
                if (GetFatherWindow() is VipWindow) {
                    finishWindow();
                    return;
                } else {
                    finishWindow();
                    if (closeCallback != null)
                        closeCallback();
                    closeCallback = null;
                    return;
                }
            } else if (showType == CardBookWindow.CLICKCHATSHOW) {
                finishWindow();
                /*这里开始是可滑动聊天窗口展示的关闭后处理，暂时不删
                if (window.GetFatherWindow () is PvpPlayerWindow) {
                    window.finishWindow ();
                } else {
                    window.finishWindow ();
                    UiManager.Instance.openDialogWindow<NewChatWindow> ((win) => {
                        win.initChatWindow (ChatManagerment.Instance.sendType - 1);
                    });
                }
                */
                return;
            }
            if (closeCallback != null)
                closeCallback();
            closeCallback = null;
            finishWindow();
        } else if (gameObj.name == "buttonPowerUp") {
            //12004000献祭强化,129005000进化强化,112004000附加强化
            if (GuideManager.Instance.isEqualStep(12004000) || GuideManager.Instance.isEqualStep(129005000) || GuideManager.Instance.isEqualStep(112004000)) {
                GuideManager.Instance.doGuide();
            }
            gotoIntensify();

        } else if (gameObj.name == "buttonBlood") {
            //打开英雄血脉窗口
            SortCondition sc = SortConditionManagerment.Instance.getConditionsByKey(SiftWindowType.SIFT_CARDCHOOSE_WINDOW);
            ArrayList cardList = SortManagerment.Instance.cardSort(StorageManagerment.Instance.getAllRole(), sc);
            List<Card> list = new List<Card>();
            int starIndex = 0;
            for (int i = 0; i < cardList.Count; i++) {
                if(CardSampleManager.Instance.getRoleSampleBySid((cardList[i] as Card).sid).bloodPointSid!=0)
                list.Add(cardList[i] as Card);
                
            }
            for (int j=0;j<list.Count;j++)
            {
                if (list[j] == showCard) {
                    starIndex = j;
                }
            }
            UiManager.Instance.openWindow<BloodEvolutionWindow>((win) => {
                win.init(list, starIndex, closeCallback);
            });

        } else if (gameObj.name == "buttonOneKeyEquips") {
            if (checkEquipPowerValue()) {
                EquipOneKeyFPort fport = FPortManager.Instance.getFPort("EquipOneKeyFPort") as EquipOneKeyFPort;
                fport.access(showCard.uid.ToString(), equipNewItem);
            } else {
                UiManager.Instance.openDialogWindow<MessageWindow>((win) => {
                    win.initWindow(1, LanguageConfigManager.Instance.getLanguage("s0040"), null, LanguageConfigManager.Instance.getLanguage("s0061"), null);
                });
            }
        } else if (gameObj.name == "buttonHeroRoad") {
            FuBenGetCurrentFPort port = FPortManager.Instance.getFPort("FuBenGetCurrentFPort") as FuBenGetCurrentFPort;
            port.getInfo((b) => {
                if (b) {
                    UiManager.Instance.openDialogWindow<MessageWindow>((win) => {
                        win.initWindow(1, LanguageConfigManager.Instance.getLanguage("s0040"), null, LanguageConfigManager.Instance.getLanguage("s0388"), null);
                    });
                } else {
                    UiManager.Instance.openWindow<HeroRoadWindow>((win) => {
                        win.init(showCard, closeCallback, getShowType());
                    });
                }
            });
        } else if (gameObj.name == "buttonPicture") {
            UiManager.Instance.openWindow<CardPictureWindow>((win) => {
                win.init(PictureManagerment.Instance.mapType[showCard.getEvolveNextSid()], 0);
            });
        } else if (gameObj.name == "buttonChatShow") {
            ChatSendMsgFPort fport = FPortManager.Instance.getFPort("ChatSendMsgFPort") as ChatSendMsgFPort;
            if (ChatManagerment.Instance.sendType == ChatManagerment.CHANNEL_FRIEND) {
                fport.access(ChatManagerment.Instance.CurrentFriendInfo.getUid(), UserManager.Instance.self.uid, ChatManagerment.Instance.sendType, null, ChatManagerment.SHOW_CARD, showCard.uid, () => {
                    UiManager.Instance.BackToWindow<ChatWindow>();
                });
            } else {
                fport.access(ChatManagerment.Instance.sendType, null, ChatManagerment.SHOW_CARD, showCard.uid, () => {
                    UiManager.Instance.BackToWindow<ChatWindow>();


                    /*这里开始是可滑动聊天窗口展示的关闭后处理，暂时不删
                    UiManager.Instance.openDialogWindow<NewChatWindow> ((win) => {
                        win.initChatWindow (ChatManagerment.Instance.sendType - 1);
                    });
                    window.GetFatherWindow ().destoryWindow ();
                    window.finishWindow ();
                    */
                });
            }
        } else if (gameObj.name == "buttonGotoEvo") {
            IntensifyCardManager.Instance.setMainCard(showCard);
            IntensifyCardManager.Instance.intoIntensify(IntensifyCardManager.INTENSIFY_CARD_EVOLUTION, null);
        } else if (gameObj.name == "selectStarSoul") {
            UiManager.Instance.openWindow<StarSoulWindow>((win) => {
                win.init(showType, showCard);                
            });
        } else if (gameObj.name == "selectMagicWeapon") {//选择秘宝界面
            //如果来自聊天界面 别人的卡片
            if (showType == CardBookWindow.CHATSHOW || showType == CardBookWindow.CLICKCHATSHOW) {
                UiManager.Instance.openWindow<MagicWeaponStrengWindow>((win) => {
                    win.init(showCard.magicWapon, MagicWeaponType.FORM_OTHER);
                });
            } else {//自己的卡片
                if (showCard.magicWeaponUID == "" || showCard.magicWeaponUID == "0") {
                    if (StorageManagerment.Instance.getAllMagicWeaponByType(showCard.getJob()) == null) {
                        UiManager.Instance.openDialogWindow<MessageLineWindow>((win) => {
                            win.Initialize(LanguageConfigManager.Instance.getLanguage("not_have_suit_put_on"));
                        });
                        return;
                    }
                    MagicWeaponManagerment.Instance.selectCard = null;
                    MagicWeaponManagerment.Instance.selectCard = showCard;
                    UiManager.Instance.openWindow<MagicWeapStoreWindow>((win) => {
                        win.init(showCard, MagicWeaponType.FROM_CARD_BOOK_NOT_M);//装备秘宝模式（需要打开独立的秘宝仓库界面）
                    });
                } else {//装备了秘宝 是要脱掉得节奏
                    MagicWeaponManagerment.Instance.selectCard = null;
                    MagicWeaponManagerment.Instance.selectCard = showCard;
                    MagicWeapon mv = StorageManagerment.Instance.getMagicWeapon(showCard.magicWeaponUID);
                    UiManager.Instance.openWindow<MagicWeaponStrengWindow>((win) => {
                        win.init(mv, MagicWeaponType.PUTON);
                    });
                } 
            }

        }
        else if (gameObj.name == "selectResonance")
        {
            if (showCardNet != null)
            {
                UiManager.Instance.openWindow<ResonanceWindow>((win) =>
                {
                    win.initServer(showCardNet, showType,chatPlayerUid);
                });
            }
            else
            {
                //点击装备共鸣
                UiManager.Instance.openWindow<ResonanceWindow>((win) =>
                {
                    win.init(showCard.uid, showType);
                });
            }
        }
    }

    private void gotoIntensify() {
        IntensifyCardManager.Instance.setMainCard(showCard);
        IntensifyCardManager.Instance.intoIntensify(IntensifyCardManager.INTENSIFY_CARD_SACRIFICE, null);
    }

    //一键换装判断
    private bool checkEquipPowerValue() {
        if (StorageManagerment.Instance.getAllEquip().Count < 1)
            return false;
        for (int i = 0; i < StorageManagerment.Instance.getAllEquip().Count; i++) {
            if (((StorageManagerment.Instance.getAllEquip()[i] as Equip).state & EquipStateType.OCCUPY) != 1) {
                break;
            } else if (i == StorageManagerment.Instance.getAllEquip().Count - 1) {
                return false;
            }
        }
        ArrayList allEquips = StorageManagerment.Instance.getAllEquip();
        ArrayList equips = new ArrayList();
        for (int i = 0; i < allEquips.Count; i++) {
            if (((allEquips[i] as Equip).state & EquipStateType.OCCUPY) != 1) {
                if ((allEquips[i] as Equip).isPutOn(showCard.sid)) {
                    equips.Add(allEquips[i]);
                }
            }
        }

        for (int i = 0; i < equipItem.Length; i++) {
            for (int j = 0; j < equips.Count; j++) {
                if (equipItem[i].equip == null) {
                    if ((equips[j] as Equip).getPartId() == i + 1) {
                        return true;
                    }
                } else {
                    if (equipItem[i].equip.getPartId() == (equips[j] as Equip).getPartId()) {
                        if (equipItem[i].equip.getPower() < (equips[j] as Equip).getPower()) {
                            return true;
                        }
                    }
                }

            }
        }
        return false;
    }
    ///<summary>
    /// 更新天赋提示箭头
    /// </summary>
    private void updateTalentArrow() {
        //		if(posIndex > 4){
        //			if (talentPanel.clipOffset.y < -20)
        //				talentArrowUp.SetActive (true);
        //			else
        //				talentArrowUp.SetActive (false);
        //
        //			if (talentPanel.clipOffset.y + ( posIndex - 4 )* 85 > 20)
        //				talentArrow.SetActive (true);
        //			else
        //				talentArrow.SetActive (false);
        //		}

    }

	public static void setChatPlayerUid(string uid)
	{
		chatPlayerUid = uid;
	}
}