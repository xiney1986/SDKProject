using UnityEngine;
using System.Collections.Generic;
using System.Collections;


/// <summary>
/// 英雄血脉窗口
/// </summary>
public class BloodEvolutionWindow : WindowBase {

    /*当前卡片的属性信息、血脉等级信息*/
    public UILabel[] attrLabel;//当前属性值
    public UILabel[] evoAddAttrLabel;//激活血脉节点所附加的属性值
    public UILabel[] addLabel;
    public UILabel bloodLevel;//血脉等级
    public flyItemCtrl flyItem;//技能飞出效果控制器
    /*当前血脉节点激活所需要的条件、增加的属性信息*/
    public UILabel needPropDesc1;
    public UITexture needropTexture1;
    public UITexture needPropTexture2;
    public UILabel needPropDesc2;
    public GameObject needPropTwoPoint;
	public GameObject needPropOnePoint;
	public GameObject[] needProps;
    public Transform propflyPoint;
    public UILabel describe;//当前卡片血脉信息描述
    public GameObject pointContentFather;
    public GameObject pontFatherContentForEffect;
    public Card oldCardold;//突破前的卡片

    public UILabel heroName;//卡片名字
    public UITexture cardIcon;
    public UITexture cardIconForMaxQuality;//最高品质时的icontexture

    /*左右切换卡片的提示按钮*/
    public GameObject buttonLeft;
    public GameObject buttonRight;

    public Card choosedCard;//当前所选择的卡片
    public GameObject bloodPointPrefab;//血脉节点
    private int defaultIndex;
    private List<Card> list;//
    private int[] currentItemMap;//当前的节点链表
    private int[] totalMap;//所有节点
    public BloodPointSample sample;
    SortCondition sc;
    //===============技能框=====================
    public GameObject skillPoint;
    public UISprite skillQuality;
    public GameObject[] skillChildPoint;
    public UILabel addSkillNameLabel;
    public UITexture addSkillIconTexture;
    public UILabel addInfo;
    public UILabel updateSkillNameOldLabel;
    public UILabel updateSkillNameNewLabel;
    public UITexture updateSkillIconOldTexture;
    public UITexture updateSkillIconNewTexture;
    public UILabel updateInfo;
    private int addSkillSid;//添加的传奇特效Sid
    private int newSkillSid;
    //================品质突破====================
    public UITexture tupaoBackGroundTexture;//品质突破的背景
    public GameObject showObjEff;//展示图片特效
    public RoleView effectCard;//主角突破后特效
    public UISprite newQuality;//新的品质图标
    public EffectCtrl[] effectSurUp;//保存两个品质提升特效
    public Animator[] effectSurText;//显示品质提升几个字的动画
    public UILabel[] oldMsgLabel;//0生命，1攻击，2防御，3魔法，4敏捷，5等级上限，6战斗力
    public UILabel[] newMsgLabel;//0生命，1攻击，2防御，3魔法，4敏捷，5等级上限，6战斗力
    public UILabel[] newAddMsgLabel;
    public GameObject showObj;//做展示用
    public UILabel combat; //战斗力
    public UILabel addQuality;
    public ButtonSkill[] buttonSkills;//0旧主技能，1新主技能
    public GameObject combatPoint;//战斗力挂机点
    public bool isPressed = false;//点击过血脉点
    public int lineType = 0;
    private bool isOnNetResume = false;
    protected override void begin() {
        updatePage(defaultIndex);
        MaskWindow.UnlockUI();
    }
    /// <summary>
    /// 更新箭头并且根据箭头更新其他的东西
    /// </summary>
    void updatePage(int index)
    {
        defaultIndex = index;
        buttonLeft.gameObject.SetActive(index!= 0);
        buttonRight.gameObject.SetActive(index != list.Count - 1);
        choosedCard = list[defaultIndex];//当前选择的卡片
        choosedCard = StorageManagerment.Instance.getRole(choosedCard.uid);
        ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.CARDIMAGEPATH + choosedCard.getImageID(), cardIcon);
        currentItemMap = BloodConfigManager.Instance.getCurrentBloodMap(choosedCard.sid, choosedCard.cardBloodLevel);
        totalMap = BloodConfigManager.Instance.getTotalMap(choosedCard.sid, choosedCard.cardBloodLevel);
        isPressed = false;
        updateAll();
    }
    public override void OnNetResume() {
        base.OnNetResume();
        updatePage(defaultIndex);
        MaskWindow.UnlockUI();
    }
    /// <summary>
    /// 初始化窗口
    /// </summary>
    /// <param name="list"></param>
    /// <param name="defaultIndex"></param>
    /// <param name="callBack"></param>
    public void init(List<Card> list, int index, CallBack callBack) {
        UiManager.Instance.backGround.switchBackGround("battleMap_11");
        this.list = list;
        defaultIndex = index;
    }

    public void updateAll() {
        /*当前等阶血脉节点的所有信息*/
        if (BloodManagement.Instance.prizes != null && BloodManagement.Instance.prizes.Length > 0 && lineType != 0) {
            UiManager.Instance.openDialogWindow<BloodEvoWindow>((win => {
                win.Initialize(BloodManagement.Instance.prizes);
            }));
        }
        sample = BloodConfigManager.Instance.getBloodPointSampleBySid(CardSampleManager.Instance.getBloodSid(choosedCard.sid));
        updateUI(sample);//更新界面上的属性
        updateNeedProp();
        updateBloodPoint();//关于节点的
    }

    void updateNeedProp()
    {
        int currentIndex = BloodConfigManager.Instance.getCurrentBloodMapIndex(choosedCard.sid,
                choosedCard.cardBloodLevel);
        BloodItemSample bis = BloodItemConfigManager.Instance.getBolldItemSampleBySid(currentItemMap[currentIndex]);
        PrizeSample[] ps = bis.condition;
        needPropTwoPoint.SetActive(ps.Length == 2);
        for (int i=0;i<ps.Length;i++)
        {
			needProps[i].GetComponent<BloodEvoPropView>().init(ps[i]);
            if (i == 0)
            {
                ResourcesManager.Instance.LoadAssetBundleTexture(ps[i].getIconPath(), needropTexture1);
                needPropDesc1.text = ps[i].getPrizeName() + ":" + ps[i].getPrizeHadNum() + "";
            }else if (i==1)
            {
                ResourcesManager.Instance.LoadAssetBundleTexture(ps[i].getIconPath(), needPropTexture2);
                needPropDesc2.text = ps[i].getPrizeName() + ":" + ps[i].getPrizeHadNum() + "";
            } else if (choosedCard.getQualityId() == QualityType.MYTH) {
                ResourcesManager.Instance.LoadAssetBundleTexture(ps[ps.Length -1].getIconPath(), needropTexture1);
                needPropDesc1.text = ps[ps.Length - 1].getPrizeName() + ":" + ps[ps.Length - 1].getPrizeHadNum() + "";
            }
        }
    }
    /// <summary>
    /// 更新UI
    /// </summary>
    public void updateUI(BloodPointSample bps)
    {
        cardIcon.gameObject.SetActive(true);
        cardIconForMaxQuality.gameObject.SetActive(false);
        cardIconForMaxQuality.mainTexture = null;
        updateAttrs();//更新卡片基础信息
        updateChangeAttrs(bps);//更新即将加成的属性数值（如果是技能类那么不需要显示）
        int[] tempQuliq = BloodConfigManager.Instance.getMapQualiy(choosedCard.sid,choosedCard.cardBloodLevel);
        //当前血脉最高描述
        describe.text = "";
        if (choosedCard.cardBloodLevel < BloodConfigManager.Instance.getTotalItemNum(choosedCard.sid) && choosedCard.getQualityId() != QualityType.MYTH) { //激活每一阶的最后一个节点一定会提升品质才可以这样判断
            if (tempQuliq[0] == -1) describe.text = "";
            else describe.text = LanguageConfigManager.Instance.getLanguage("prefabzc34", QualityManagerment.getQualityColorForBloodItem(tempQuliq[0], tempQuliq[1]), QualityManagerment.getdecForBloodItem(tempQuliq[0]), QualityManagerment.getColorForBloodItem(tempQuliq[0] -1),QualityManagerment.getColorForBloodItem(tempQuliq[0]));
        }
        if (choosedCard.cardBloodLevel == BloodConfigManager.Instance.getTotalItemNum(choosedCard.sid) && choosedCard.getQualityId() == QualityType.MYTH) {//激活每一阶的最后一个节点一定会提升品质才可以这样判断
            cardIcon.gameObject.SetActive(false);
            cardIcon.mainTexture = null;
            ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.CARDIMAGEPATH + choosedCard.getImageID(), cardIconForMaxQuality);
            cardIconForMaxQuality.gameObject.SetActive(true);
        }
        int[] colorQuliq = BloodConfigManager.Instance.getCurrentBloodLvColor(choosedCard.sid, choosedCard.cardBloodLevel);
        //当前节点的附加属性
        if (choosedCard.cardBloodLevel == BloodConfigManager.Instance.getTotalItemNum(choosedCard.sid) && choosedCard.getQualityId() == QualityType.MYTH) {
            bloodLevel.text = "[FF0000]"+LanguageConfigManager.Instance.getLanguage("prefabzc31", (BloodConfigManager.Instance.getCurrentBloodMap(choosedCard.sid, choosedCard.cardBloodLevel).Length + ""));//bloodLavel UIlaber描述
        }else 
            bloodLevel.text = QualityManagerment.getQualityColorForlv(colorQuliq[0],colorQuliq[1]);//bloodLavel UIlaber描述
    }
    /// <summary>
    /// 更新血脉节点
    /// </summary>
    public void updateBloodPoint() {
        MaskWindow.LockUI();
        for (int i = 0; i < pointContentFather.transform.childCount; i++) {
            DestroyImmediate(pointContentFather.transform.GetChild(i).gameObject);
            i--;
        }
        //if (choosedCard.cardBloodLevel == BloodConfigManager.Instance.getTotalItemNum(choosedCard.sid) || choosedCard.getQualityId() == QualityType.MYTH)
        //    return;
        GameObject a;
        float y = 0;
        int currentIndex = 0;
        if (choosedCard.cardBloodLevel == 0) {
            currentIndex = 0;
        } else
            currentIndex = choosedCard.cardBloodLevel;
        //if (choosedCard.getQualityId() == QualityType.MYTH)
        //    currentIndex = 50;
        for (int k=0;k<totalMap.Length;k++)
        {
            if (isPressed) {
                if (k == currentIndex) {
                    y = (k-1) * 134;
                    break;
                }
            } else {
                if (k == currentIndex) {
                    y = k * 134;
                    break;
                }
            }
        }
        if (choosedCard.cardBloodLevel == totalMap.Length && choosedCard.getQualityId() == QualityType.MYTH) {
            if(isPressed)
            y = (totalMap.Length - 2) * 134;
            else y = (totalMap.Length - 1) * 134;
        }
        pontFatherContentForEffect.transform.localPosition=Vector3.zero;
        pontFatherContentForEffect.GetComponent<UIPanel>().clipOffset = new Vector2(0f, 0f);
        int[] colorList = BloodConfigManager.Instance.getBloodItemColor(choosedCard.sid, choosedCard.cardBloodLevel);
        for (int i = 0; i < totalMap.Length; i++)
        {
            a = Instantiate(bloodPointPrefab) as GameObject;
            BloodPointItem bloodItem = a.GetComponent<BloodPointItem>();
            bloodItem.name = StringKit.intToFixString(i + 1);
            bloodItem.fatherWindow = this;
            bloodItem.transform.parent = pointContentFather.transform;
            bloodItem.transform.localScale = Vector3.one;
            bloodItem.transform.localPosition=new Vector3((i%2)*137,y,0f);
            BloodItemSample bts = BloodItemConfigManager.Instance.getBolldItemSampleBySid(totalMap[i]);
            int color = colorList[i] -2;
            int line = 0;
            int currentIndexx = currentIndex;
            if (i != totalMap.Length - 1)
            {
                if (i % 2 == 1) {
                    line = 1;
                } else {
                    line = 2;
                }
            }
            if (i < currentIndexx) currentIndexx = 2;
            else if (i == currentIndexx)
            {
               // pointContentFather.transform.localPosition=new Vector3(pointContentFather.transform.localPosition.x,i*134,pointContentFather.transform.localPosition.z);
                currentIndexx = 1;
            } else currentIndexx = 0;
            int index = BloodConfigManager.Instance.getCurrentBloodLvColor(choosedCard.sid, i)[1];
            bloodItem.init(bts, color, line, currentIndexx, choosedCard, index, pontFatherContentForEffect.transform);
            y-=134;
        }
        if (isPressed) {
            pontFatherContentForEffect.GetComponent<UIPanel>().clipOffset += new Vector2(0f, -134f);
            TweenPosition tp = TweenPosition.Begin(pontFatherContentForEffect, 0.7f, (pontFatherContentForEffect.transform.localPosition + new Vector3(0, 134, 0)));
            tp.from = new Vector3(0, pontFatherContentForEffect.transform.localPosition.y, 0);
            EventDelegate.Add(tp.onFinished, () => {
                MaskWindow.UnlockUI();
                 },true);
        }
    }
    public override void buttonEventBase(GameObject gameObj) {
        base.buttonEventBase(gameObj);
        if (gameObj.name == "close") {
            finishWindow();
        }
        else if (gameObj.name == "right")
        {
            defaultIndex = defaultIndex + 1;
            updatePage(defaultIndex);
            MaskWindow.UnlockUI();
        }
        else if (gameObj.name == "left") {
            defaultIndex = defaultIndex -1;
            updatePage(defaultIndex);
            MaskWindow.UnlockUI();
        } else if (gameObj.name == "flyInfo")
        {
            skillPoint.SetActive(false);
            updateCardData();
            MaskWindow.UnlockUI();
        } else if (gameObj.name == "ShowTitle")
        {
            tupaoBackGroundTexture.gameObject.SetActive(false);
            showObjEff.SetActive(false);
            combatPoint.SetActive(false);
            showObj.SetActive(false);
            updateCardData();
            MaskWindow.UnlockUI();
        }
    }
    void updateAttrs() {
        for (int i = 0; i < attrLabel.Length; i++) {//清空属性信息
            attrLabel[i].text = "";
            evoAddAttrLabel[i].text = "";
        }
        describe.text = "";
        CardBaseAttribute cardAttr = CardManagerment.Instance.getCardAllWholeAttr(choosedCard);
        this.heroName.text = QualityManagerment.getQualityColor(choosedCard.getQualityId()) + choosedCard.getName();
        //卡片现在的属性
        attrLabel[0].text = cardAttr.getWholeHp() + "";
        attrLabel[1].text = cardAttr.getWholeAtt() + "";
        attrLabel[2].text = cardAttr.getWholeDEF() + "";
        attrLabel[3].text = cardAttr.getWholeMAG() + "";
        attrLabel[4].text = cardAttr.getWholeAGI() + "";
    }
    /// <summary>
    /// 更新可以加的属性值
    /// </summary>
    /// <param name="bps"></param>
    void updateChangeAttrs(BloodPointSample bps) {
        for (int i = 0; i < evoAddAttrLabel.Length; i++) {//不管什么情况都先清零在说
            evoAddAttrLabel[i].text = "";
        }
        if (choosedCard.cardBloodLevel >= bps.maxLv) return;//没有满级才会显示
        int bloodItemSid = BloodConfigManager.Instance.getCurrentItemSid(bps, choosedCard.cardBloodLevel);//拿到具体的节点信息Sid
        BloodItemSample bis = BloodItemConfigManager.Instance.getBolldItemSampleBySid(bloodItemSid);
        if (bis == null) return;
        bloodEffect[] blEffects = bis.effects;
        int hp = 0, attack = 0, defec = 0, magic = 0, agile = 0;//保存具体数值属性
        for (int i = 0; i < blEffects.Length; i++) {
            hp += blEffects[i].hp;
            attack += blEffects[i].attack;
            defec += blEffects[i].defec;
            magic += blEffects[i].magic;
            agile += blEffects[i].agile;
        }
        CardBaseAttribute cardAttr = CardManagerment.Instance.getCardAllWholeAttr(choosedCard);
        for (int i = 0; i < blEffects.Length; i++) {
            if (blEffects[i].perhp == 0) {
                if (hp == 0) evoAddAttrLabel[0].text = "";
                else evoAddAttrLabel[0].text = "+" + hp;
            } else evoAddAttrLabel[0].text = "+" + (cardAttr.getWholeHp() + hp) * (1.0f + (float)blEffects[i].perhp / 100);
            if (blEffects[i].perattack == 0) {
                if (attack == 0) evoAddAttrLabel[1].text = "";
                else evoAddAttrLabel[1].text = "+" + attack;
            } else evoAddAttrLabel[1].text = "+" + (cardAttr.getWholeAtt() + attack) * (1.0f + (float)blEffects[i].perattack / 100);
            if (blEffects[i].perdefec == 0) {
                if (defec == 0) evoAddAttrLabel[2].text = "";
                else evoAddAttrLabel[2].text = "+" + defec;
            } else evoAddAttrLabel[2].text = "+" + (cardAttr.getWholeDEF() + defec) * (1.0f + (float)blEffects[i].perdefec / 100);
            if (blEffects[i].permagic == 0) {
                if (magic == 0) evoAddAttrLabel[3].text = "";
                else evoAddAttrLabel[3].text = "+" + magic;
            } else evoAddAttrLabel[3].text = "+" + (cardAttr.getWholeMAG() + magic) * (1.0f + (float)blEffects[i].permagic / 100);
            if (blEffects[i].peragile == 0) {
                if (agile == 0) evoAddAttrLabel[4].text = "";
                else evoAddAttrLabel[4].text = "+" + agile;
            } else evoAddAttrLabel[4].text = "+" + (cardAttr.getWholeAGI() + agile) * (1.0f + (float)blEffects[i].peragile / 100);
        }
    }
    private void playerEffect(UILabel _labelTitle, UILabel _labelDesc, int _desc,int index) {
        MaskWindow.LockUI();
        _labelTitle.text = "+";
        TweenScale ts = TweenScale.Begin(_labelTitle.gameObject, 0.1f, Vector3.one);
        ts.method = UITweener.Method.EaseIn;
        ts.from = new Vector3(5, 5, 1);
        _labelDesc.text = "";
        TweenScale ts2 = TweenScale.Begin(_labelDesc.gameObject, 0.1f, Vector3.one);
        ts2.method = UITweener.Method.EaseIn;
        ts2.from = new Vector3(5, 5, 1);
        EventDelegate.Add(ts2.onFinished, () => {
            TweenLabelNumber tln = TweenLabelNumber.Begin(_labelDesc.gameObject, 0.1f, _desc);
            tln.from = 0;
            EventDelegate.Add(tln.onFinished, () => {
                GameObject obj = Create3Dobj("Effect/Other/Flash").obj;
                obj.transform.parent = _labelDesc.transform;
                obj.transform.localScale = Vector3.one;
                obj.transform.localPosition = new Vector3(0, 0, -600);
                StartCoroutine(Utils.DelayRun(() =>
                {

                    beginReSuchData(_desc, index, _labelTitle);
                }, 0.1f));
            }, true);
        }, true);
    }
    /// <summary>
    /// 刷新数据
    /// </summary>
    void beginReSuchData(int desc,int index,UILabel titleLabel)
    {
        TweenLabelNumber tll = TweenLabelNumber.Begin(attrLabel[index].gameObject, 1f, desc+StringKit.toInt(attrLabel[index].text));
        tll.from = StringKit.toInt(attrLabel[index].text);
        EventDelegate.Add(tll.onFinished, () =>
        {
            titleLabel.text = "";
            updateCardData();
            //MaskWindow.UnlockUI();
        }, true);
    }

    public void updateCardData()//只适用于add技能
    {

        choosedCard = StorageManagerment.Instance.getRole(choosedCard.uid);
        currentItemMap = BloodConfigManager.Instance.getCurrentBloodMap(choosedCard.sid, choosedCard.cardBloodLevel);
        totalMap = BloodConfigManager.Instance.getTotalMap(choosedCard.sid, choosedCard.cardBloodLevel);
        updateAll();
    }
    /// <summary>
    /// 显示添加传奇特技的信息
    /// </summary>
    public void showTheAddSkillInfo()
    {
        skillPoint.transform.localScale = new Vector3(0.1f,0.1f,0.1f);
        if (choosedCard.getQualityId() == QualityType.EPIC) {
            skillQuality.spriteName = "shishiteji";
            addInfo.text = LanguageConfigManager.Instance.getLanguage("bloodSkillInfo", LanguageConfigManager.Instance.getLanguage("s0076"));
            updateInfo.text = LanguageConfigManager.Instance.getLanguage("bloodSkillInfo1", LanguageConfigManager.Instance.getLanguage("s0076"));
        } else if (choosedCard.getQualityId() == QualityType.LEGEND) {
            skillQuality.spriteName = "chuanqiteji";
            addInfo.text = LanguageConfigManager.Instance.getLanguage("bloodSkillInfo", LanguageConfigManager.Instance.getLanguage("s0077"));
            updateInfo.text = LanguageConfigManager.Instance.getLanguage("bloodSkillInfo1", LanguageConfigManager.Instance.getLanguage("s0077"));
        } else if (choosedCard.getQualityId() == QualityType.MYTH) {
            skillQuality.spriteName = "icon_mythSkillTitle";
            addInfo.text = LanguageConfigManager.Instance.getLanguage("bloodSkillInfo", LanguageConfigManager.Instance.getLanguage("s0077ss"));
            updateInfo.text = LanguageConfigManager.Instance.getLanguage("bloodSkillInfo1", LanguageConfigManager.Instance.getLanguage("s0077ss"));
        }
        skillPoint.SetActive(true);
        skillChildPoint[0].SetActive(true);
        skillChildPoint[1].SetActive(false);
        SkillSample sk = SkillSampleManager.Instance.getSkillSampleBySid(addSkillSid);
        SkillSample newSk = SkillSampleManager.Instance.getSkillSampleBySid(newSkillSid);
        if (sk != null && newSk == null)
        {
            addSkillNameLabel.text = sk.name;
            ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.SKILLIMAGEPATH + sk.iconId, addSkillIconTexture);
        } else if (sk != null && newSk != null) {
            skillChildPoint[0].SetActive(false);
            skillChildPoint[1].SetActive(true);
            updateSkillNameOldLabel.text = sk.name;
            updateSkillNameNewLabel.text = newSk.name;
            ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.SKILLIMAGEPATH + sk.iconId, updateSkillIconOldTexture);
            ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.SKILLIMAGEPATH + newSk.iconId, updateSkillIconNewTexture);
        }
        StartCoroutine(Utils.DelayRun(() => {
            MaskWindow.UnlockUI();
        }, 0.2f));
        iTween.ScaleTo(skillPoint, iTween.Hash("scale", new Vector3(1f, 1f, 1f), "easetype", "easeInQuad", "time", 0.2f));
    }

    void overrr()
    {
        MaskWindow.UnlockUI();
    }
    IEnumerator playSkillEffect(int oldSid,int newSid,Vector3 v3)
    {
        addSkillSid = oldSid;
        newSkillSid = newSid;
        if(newSid != null)
        StartCoroutine(flyItemInit(newSid, v3));
        else StartCoroutine(flyItemInit(oldSid, v3));
        yield return new WaitForSeconds(1f);
    }
    IEnumerator playSkillEffect(int sid,Vector3 v3) {
        addSkillSid = sid;
		newSkillSid = 0;
        StartCoroutine(flyItemInit(sid, v3));
        yield return new WaitForSeconds(1f);
    }
    private GameObject getEffectByQualiy() {
        Card newCard = StorageManagerment.Instance.getRole(choosedCard.uid);
        if (newCard.getQualityId() == 4) {
            return effectSurUp[0].gameObject;
        } else if (newCard.getQualityId() == 5)
            return effectSurUp[1].gameObject;
        else
            return effectSurUp[1].gameObject;
    }
    private Animator getEffectTextByQualiy() {
        Card newCard = StorageManagerment.Instance.getRole(choosedCard.uid);
        if (newCard.getQualityId() == 4)
            return effectSurText[0];
        else if (newCard.getQualityId() == 5)
            return effectSurText[1];
        else
            return effectSurText[1];
    }
    public void bloodSuccessCallBack(BloodItemSample bis,Vector3 v3,int lineType)
    {
        bloodEffect[] blEffects = bis.effects;
        this.lineType = lineType;
        int hp = 0, attack = 0, defec = 0, magic = 0, agile = 0;//保存具体数值属性
        if (lineType == 0)
        {
            if (choosedCard.cardBloodLevel == BloodConfigManager.Instance.getTotalItemNum(choosedCard.sid) && choosedCard.getQualityId() == QualityType.MYTH) {
                for (int i = 0; i < blEffects.Length; i++) {
                    hp += blEffects[i].hp;
                    attack += blEffects[i].attack;
                    defec += blEffects[i].defec;
                    magic += blEffects[i].magic;
                    agile += blEffects[i].agile;
                }
                CardBaseAttribute cardAttr = CardManagerment.Instance.getCardAllWholeAttr(choosedCard);
                for (int i = 0; i < blEffects.Length; i++) {
                    hp += (int)((cardAttr.getWholeHp() + hp) * ((float)blEffects[i].perhp / 100));
                    attack += (int)((cardAttr.getWholeAtt() + attack) * ((float)blEffects[i].perattack / 100));
                    defec += (int)((cardAttr.getWholeDEF() + defec) * ((float)blEffects[i].perdefec / 100));
                    magic += (int)((cardAttr.getWholeMAG() + magic) * ((float)blEffects[i].permagic / 100));
                    agile += (int)((cardAttr.getWholeAGI() + agile) * ((float)blEffects[i].peragile / 100));
                }
                if (hp != 0) playerEffect(addLabel[0], evoAddAttrLabel[0], hp, 0);
                if (attack != 0) playerEffect(addLabel[1], evoAddAttrLabel[1], attack, 1);
                if (defec != 0) playerEffect(addLabel[2], evoAddAttrLabel[2], defec, 2);
                if (magic != 0) playerEffect(addLabel[3], evoAddAttrLabel[3], magic, 3);
                if (agile != 0) playerEffect(addLabel[4], evoAddAttrLabel[4], agile, 4);
                StartCoroutine(Utils.DelayRun(() => {
                    if (BloodManagement.Instance.prizes != null && BloodManagement.Instance.prizes.Length > 0) {
                        UiManager.Instance.openDialogWindow<BloodEvoWindow>((win => {
                            win.Initialize(BloodManagement.Instance.prizes);
                        }));
                    }
                },0.5f));
                return;
            }
            showQualityEvo();
        }
        if (blEffects[0].type == 5||blEffects[0].type==6)
        {
            if (blEffects[0].type == 5)
                StartCoroutine(playSkillEffect(blEffects[0].skillSid,v3));
            else
            StartCoroutine(playSkillEffect(blEffects[0].drSkillSid, blEffects[0].skillSid, v3));
        }
        else
        {
            for (int i = 0; i < blEffects.Length; i++) {
                hp += blEffects[i].hp;
                attack += blEffects[i].attack;
                defec += blEffects[i].defec;
                magic += blEffects[i].magic;
                agile += blEffects[i].agile;
            } 
            CardBaseAttribute cardAttr = CardManagerment.Instance.getCardAllWholeAttr(choosedCard);
            for (int i = 0; i < blEffects.Length; i++) {
                hp += (int)((cardAttr.getWholeHp() + hp) * ((float)blEffects[i].perhp / 100));
                attack += (int)((cardAttr.getWholeAtt() + attack) * ((float)blEffects[i].perattack / 100));
                defec += (int)((cardAttr.getWholeDEF() + defec) * ((float)blEffects[i].perdefec / 100));
                magic += (int)((cardAttr.getWholeMAG() + magic) * ((float)blEffects[i].permagic / 100));
                agile += (int)((cardAttr.getWholeAGI() + agile) * ((float)blEffects[i].peragile / 100));
            }
            if (hp != 0) playerEffect(addLabel[0], evoAddAttrLabel[0], hp, 0);
            if (attack != 0) playerEffect(addLabel[1], evoAddAttrLabel[1], attack, 1);
            if (defec != 0) playerEffect(addLabel[2], evoAddAttrLabel[2], defec, 2);
            if (magic != 0) playerEffect(addLabel[3], evoAddAttrLabel[3], magic, 3);
            if (agile != 0) playerEffect(addLabel[4], evoAddAttrLabel[4], agile, 4);
        }
    }
    IEnumerator flyItemInit(int sid,Vector3 v3) {
        float randomValue = Random.Range(0.1f, 0.4f);
        yield return new WaitForSeconds(randomValue);
        flyItem.gameObject.transform.localPosition = v3+new Vector3(-100f,0f,0f);
        flyItem.gameObject.SetActive(true);
        flyItem.Initialize(sid, this);
    }
    /// <summary>
    /// 品质突破的表现
    /// </summary>
    /// <param name="msg"></param>
    private void showQualityEvo() {
        ResourcesManager.Instance.LoadAssetBundleTexture("texture/backGround/ChouJiang_BeiJing", tupaoBackGroundTexture);
        tupaoBackGroundTexture.gameObject.SetActive(true);
        showObjEff.SetActive(true);
        combatPoint.SetActive(true);
        ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.CARDIMAGEPATH + choosedCard.getImageID(), effectCard.icon);
        newQuality.spriteName = QualityManagerment.qualityIDToString(choosedCard.getQualityId()) + "Bg";
        TweenPosition tp = TweenPosition.Begin(effectCard.gameObject, 0.3f, effectCard.transform.localPosition);
        tp.from = new Vector3(500, effectCard.transform.localPosition.y, 0);
        EventDelegate.Add(tp.onFinished, () => {
            StartCoroutine(Utils.DelayRun(() => {
                //nextSetp++;//开始第二步
                NGUITools.AddChild(effectCard.gameObject, getEffectByQualiy().gameObject);
                StartCoroutine(Utils.DelayRun(() => {
                    //nextSetp++;//开始下一步
                    getEffectTextByQualiy().gameObject.SetActive(true);
                    StartCoroutine(Utils.DelayRun(() => {
                        getEffectTextByQualiy().gameObject.SetActive(false);
                        //nextSetp++;下一步开始
                        newQuality.alpha = 1;
                        TweenScale ts = TweenScale.Begin(newQuality.gameObject, 0.3f, Vector3.one);
                        ts.method = UITweener.Method.EaseIn;
                        ts.from = new Vector3(5, 5, 1);
                        EventDelegate.Add(ts.onFinished, () => {
                            iTween.ShakePosition(transform.parent.gameObject, iTween.Hash("amount", new Vector3(0.03f, 0.03f, 0.03f), "time", 0.4f));
                            iTween.ShakePosition(transform.parent.gameObject, iTween.Hash("amount", new Vector3(0.01f, 0.01f, 0.01f), "time", 0.4f));
                            StartCoroutine(Utils.DelayRun(() => {
                                //nextSetp++;下一步
                                showEvoUI();
                                TweenScale ts1 = TweenScale.Begin(showObj, 0.3f, Vector3.one);
                                ts1.method = UITweener.Method.EaseIn;
                                ts1.from = new Vector3(5, 5, 1);
                                EventDelegate.Add(ts1.onFinished, () => {
                                    iTween.ShakePosition(transform.parent.gameObject, iTween.Hash("amount", new Vector3(0.03f, 0.03f, 0.03f), "time", 0.4f));
                                    iTween.ShakePosition(transform.parent.gameObject, iTween.Hash("amount", new Vector3(0.01f, 0.01f, 0.01f), "time", 0.4f));
                                    StartCoroutine(Utils.DelayRun(() => {
                                        if (BloodManagement.Instance.prizes != null && BloodManagement.Instance.prizes.Length > 0) {
                                            UiManager.Instance.openDialogWindow<BloodEvoWindow>((win => {
                                                win.Initialize(BloodManagement.Instance.prizes);
                                            }));
                                        }
                                        //nextSetp++;
                                        //initInfo();//初始化下一级突破条件以及界面
                                        MaskWindow.UnlockUI();
                                    }, 0.5f));
                                }, true);
                            }, 1.8f));
                        }, true);
                    }, 1.6f));
                }, 1.5f));
            }, 0.2f));
        }, true);
        return;
    }
    private void showEvoUI() {
        showObj.SetActive(true);
        Card newCard = StorageManagerment.Instance.getRole(choosedCard.uid);
        showOldInfo(oldCardold);
        showNewInfo(oldCardold, newCard);
    }

    private void showOldInfo(Card oldCard) {
        CardBaseAttribute attr = CardManagerment.Instance.getCardAllWholeAttr(oldCard);
        oldMsgLabel[0].text = attr.getWholeHp() + "";
        oldMsgLabel[1].text = attr.getWholeAtt() + "";
        oldMsgLabel[2].text = attr.getWholeDEF() + "";
        oldMsgLabel[3].text = attr.getWholeMAG() + "";
        oldMsgLabel[4].text = attr.getWholeAGI() + "";
        oldMsgLabel[5].text = "Lv." + oldCard.getMaxLevel();
        //		oldMsgLabel [6].text = oldCard.getCardCombat ().ToString();

        buttonSkills[0].initSkillData(oldCard.getSkills()[0], ButtonSkill.STATE_LEARNED);
    }

    private void showNewInfo(Card oldCard, Card newCard) {
        CardBaseAttribute attrNew = CardManagerment.Instance.getCardAllWholeAttr(newCard);
        CardBaseAttribute attr = CardManagerment.Instance.getCardAllWholeAttr(oldCard);
        newMsgLabel[0].text = attr.getWholeHp().ToString();
        newMsgLabel[1].text = attr.getWholeAtt().ToString();
        newMsgLabel[2].text = attr.getWholeDEF().ToString();
        newMsgLabel[3].text = attr.getWholeMAG().ToString();
        newMsgLabel[4].text = attr.getWholeAGI().ToString();
        newMsgLabel[5].text = "Lv." + oldCard.getMaxLevel();
        TweenLabelNumber tl = TweenLabelNumber.Begin(combat.gameObject, 0.5f, newCard.getCardCombat());
        //		combat.text =  newCard.getCardCombat ().ToString();
        newAddMsgLabel[0].text = (attrNew.getWholeHp() - attr.getWholeHp()) >= 0 ? " + " + (attrNew.getWholeHp() - attr.getWholeHp()) : (attrNew.getWholeHp() - attr.getWholeHp()) +"";
        newAddMsgLabel[1].text = (attrNew.getWholeAtt() - attr.getWholeAtt()) >= 0 ? " + " + (attrNew.getWholeAtt() - attr.getWholeAtt()) : (attrNew.getWholeAtt() - attr.getWholeAtt())+"";
        newAddMsgLabel[2].text = (attrNew.getWholeDEF() - attr.getWholeDEF()) >= 0 ? " + " + (attrNew.getWholeDEF() - attr.getWholeDEF()) : (attrNew.getWholeDEF() - attr.getWholeDEF()) + "";
        newAddMsgLabel[3].text = (attrNew.getWholeMAG() - attr.getWholeMAG()) >= 0 ? " + " + (attrNew.getWholeMAG() - attr.getWholeMAG()) : (attrNew.getWholeMAG() - attr.getWholeMAG()) + "";
        newAddMsgLabel[4].text = (attrNew.getWholeAGI() - attr.getWholeAGI()) >= 0 ? " + " + (attrNew.getWholeAGI() - attr.getWholeAGI()) : (attrNew.getWholeAGI() - attr.getWholeAGI()) + "";
        newAddMsgLabel[5].text = (newCard.getMaxLevel() - oldCard.getMaxLevel()) >= 0 ? " + " + (newCard.getMaxLevel() - oldCard.getMaxLevel()) : (newCard.getMaxLevel() - oldCard.getMaxLevel()) + "";
        int addQ = newCard.getQualityId() - oldCard.getQualityId();
        if (addQ != 0)
            addQuality.text = LanguageConfigManager.Instance.getLanguage("beastSummonShow11") + "+" + addQ;
        else
            addQuality.text = "";
        buttonSkills[1].initSkillData(newCard.getSkills()[0], ButtonSkill.STATE_LEARNED);
    }
}
