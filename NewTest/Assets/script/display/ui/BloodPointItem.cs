using UnityEngine;


/// <summary>
/// 每个节点预制体
/// </summary>
public class BloodPointItem : ButtonBase {

    public GameObject bloodItemBg;//血脉节点背景（有缩放关系）
    public GameObject bgPoint;//挂接子物体
    public UISprite bloodLine1;//左右两条线
    public UISprite bloodLine2;//;左右两条线
    public GameObject openPoint;//已经开放的节点
    public GameObject canOpenPoint;//可以开放的节点
    public GameObject effectPoint;//挂接特效的节点
    public UILabel canOpenAttr;//可激活节点提高的效果
    public UILabel canOpenNeed;//可激活节点的需求
    public UILabel openAttrLabel;//长按的效果
    public UILabel openlvLabel;//长按的血脉等级
    public flyItemCtrl flyItem;//技能飞出效果控制器
    /**三种特效数组 */
    public EffectCtrl[] effect_Small;
    public EffectCtrl[] effect_Medium;
    public EffectCtrl[] effect_Big;
    public EffectCtrl[] openEffect_Small;
    public EffectCtrl[] openEffect_Medium;
    public EffectCtrl[] openEffect_Big;
    public EffectCtrl[] huiEffectCtrls;
    public EffectCtrl[] huiEffectnotGuang;
    public UITexture skillIconTexture;//技能图标
    public UITexture skillMask;//技能遮罩
    public GameObject skillBg;//技能背景图
    private string effects;//激活效果
    private string attrDesc;
    private string levelDesc;
    private BloodItemSample bloodItemSample;//节点模板
    private int lineType;//0就是不显示线条1就是显示左边线条2就是显示右边线条
    private int colorr;
    private int pointType;//0代表不可以点亮，1，可以点亮 点击按键就是升级，2/0，已经点亮 长按按键显示节点属性；3，到头了什么都不显示
    private Card chooseCard;
    private int[] bloodItemMap;
    private int index;
    private Transform faTherTransform;
    /** 长按回调 */
    [HideInInspector]
    public CallBack longPassCallback;
    private Timer timer;
    /// <summary>
    /// 初始化方法
    /// </summary>
    /// <param name="win"></param>
    /// <param name="bts"></param>
    public void init(BloodItemSample bts,int color,int line,int pointTypee,Card card,int indexx,Transform fatherTran)
    {

        faTherTransform = fatherTran;
        chooseCard = card;
        index = indexx;
        bloodItemSample = bts;
        colorr = color;
        bloodItemMap = BloodConfigManager.Instance.getCurrentBloodMap(card.sid, card.cardBloodLevel);
        lineType = line;
        pointType = pointTypee;
        updateItem();
        timer = TimerManager.Instance.getTimer(100);
        timer.addOnTimer(refreshData);
        timer.start();
    }

    void refreshData()
    {
        if (this==null||!gameObject.activeInHierarchy) {
            if (timer != null) {
                timer.stop();
                timer = null;
            }
            return;
        }
        if (faTherTransform != null &&effectPoint.transform.childCount>=1)
        {
           if (effectPoint.gameObject.activeInHierarchy&&(transform.localPosition.y + faTherTransform.localPosition.y >= 370||transform.localPosition.y+faTherTransform.localPosition.y<=-367))
            {
                effectPoint.gameObject.SetActive(false);
            }
           if (!effectPoint.gameObject.activeInHierarchy && (transform.localPosition.y + faTherTransform.localPosition.y <370&&transform.localPosition.y+faTherTransform.localPosition.y>-367))
           {
                effectPoint.gameObject.SetActive(true);
            }
       }
        if (faTherTransform != null && bloodItemBg.transform.childCount >= 1) {
            if (bloodItemBg.gameObject.activeInHierarchy && (transform.localPosition.y + faTherTransform.localPosition.y >= 370 || transform.localPosition.y + faTherTransform.localPosition.y <= -367)) {
                bloodItemBg.SetActive(false);
            }
            if (!bloodItemBg.gameObject.activeInHierarchy && (transform.localPosition.y + faTherTransform.localPosition.y < 370 && transform.localPosition.y + faTherTransform.localPosition.y > -367)) {
                bloodItemBg.SetActive(true);
            }
        }
        if (faTherTransform != null && bgPoint.transform.childCount >= 1) {
            if (bgPoint.gameObject.activeInHierarchy && (transform.localPosition.y + faTherTransform.localPosition.y >= 370 || transform.localPosition.y + faTherTransform.localPosition.y <= -367)) {
                bgPoint.SetActive(false);
            }
            if (!bgPoint.gameObject.activeInHierarchy && (transform.localPosition.y + faTherTransform.localPosition.y < 370 && transform.localPosition.y + faTherTransform.localPosition.y > -367)) {
                bgPoint.SetActive(true);
            }
        }
    }
    /// <summary>
    /// 更新关于这个节点的所有显示信息
    /// </summary>
    void updateItem()
    {
        updateLine();//更新节点
        if (pointType == 2)
            updateEffect(); //更新特效
        else
            updateBgEffect();//更新背景节点特效
        updateCurrentPoint();//更新当前节点信息

    }

    void updateBgEffect()
    {
        bloodEffect bloodEffects = bloodItemSample.effects[0];//第一个属性
        if (bloodEffects.type == 5 || bloodEffects.type == 6) //只有增加新技能才显示技能图标
        {
            skillMask.gameObject.SetActive(false);
            bgPoint.SetActive(false);
            bloodItemBg.SetActive(false);
            SkillSample newSk = SkillSampleManager.Instance.getSkillSampleBySid(bloodEffects.skillSid);
            SkillSample oldSk = SkillSampleManager.Instance.getSkillSampleBySid(bloodEffects.drSkillSid);
            if (bloodEffects.type == 5) {
                if (newSk != null)
                    ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.SKILLIMAGEPATH + newSk.iconId, skillIconTexture);
            } else {
                if (oldSk != null) {
                    if (newSk != null)
                        ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.SKILLIMAGEPATH + newSk.iconId, skillIconTexture);
                    else ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.SKILLIMAGEPATH + oldSk.iconId, skillIconTexture);//如果实在没有图标就只有显示特效了哇
                    //skillBg.SetActive(true);
                }
            }
            if (pointType != 2) {
                skillMask.gameObject.SetActive(true);
            }
        }
        else
        {
            //skillBg.SetActive(false);
            if (pointType == 1) {
                NGUITools.AddChild(bgPoint,
                        huiEffectCtrls[bloodItemSample.itemType - 1].gameObject);
                if (bloodItemSample.itemType == 1) {
                    skillBg.SetActive(false);
                } else if (bloodItemSample.itemType == 3) {
                    skillBg.SetActive(true);
                }
                bloodItemBg.gameObject.SetActive(false);

            } else if (pointType != 2) {
                NGUITools.AddChild(bloodItemBg,
                        huiEffectnotGuang[bloodItemSample.itemType - 1].gameObject);
                if (bloodItemSample.itemType == 1) {
                    skillBg.SetActive(false);
                } else if (bloodItemSample.itemType == 3) {
                    skillBg.SetActive(true);
                }
                bloodItemBg.gameObject.SetActive(true);
            }
        }
        
    }
    void updateCurrentPoint()
    {
        bloodEffect[] effects = bloodItemSample.effects;
        string decString = "";
        for (int i=0;i<effects.Length;i++)
        {
            decString += effects[i].dec;
        }
        if (pointType==1)
        {
            int currentIndex = BloodConfigManager.Instance.getCurrentBloodMapIndex(chooseCard.sid,
                chooseCard.cardBloodLevel);
            int mapLength = BloodConfigManager.Instance.getCurrentBloodMap(chooseCard.sid,chooseCard.cardBloodLevel).Length;
            int[] colorQuliq = BloodConfigManager.Instance.getCurrentBloodLvColor(chooseCard.sid,chooseCard.cardBloodLevel);
            if (currentIndex == mapLength - 1) {
                if(chooseCard.getQualityId() != QualityType.MYTH)
                decString += "\n" + LanguageConfigManager.Instance.getLanguage("bloodItemDec6", QualityManagerment.getQualityColorForlv(colorQuliq[0] + 1));
            }
            canOpenPoint.SetActive(true);
            openPoint.SetActive(false);
            canOpenAttr.text = decString;
            string needDec = "";
            PrizeSample[] ps = bloodItemSample.condition;
            for (int j=0;j<ps.Length;j++) {
                if (StringKit.toInt(ps[j].num) > ps[j].getPrizeHadNum())
                    needDec += "[FF0000]" + ps[j].getPrizeName() + "*" + ps[j].num + "[-]";
                else
                    needDec += ps[j].getPrizeName() + "*" + ps[j].num;
                if (j < ps.Length - 1) {
                    needDec += "\n";
                }
            }
            string evoDec = "";
            if(bloodItemSample.evoLvCondition != 0)
            evoDec = LanguageConfigManager.Instance.getLanguage("bloodEvo", bloodItemSample.evoLvCondition + "");
            if (bloodItemSample.evoLvCondition > chooseCard.getEvoLevel()) {//进化等级不够（只显示进化等级这个条件）
                canOpenNeed.text = "[FF0000]" + evoDec;
                return;
            }
            canOpenNeed.text = needDec;//进化等级够了(只显示资源消耗这个条件)
            return;
        }
        canOpenPoint.SetActive(false); 
        openAttrLabel.text = decString;
        openlvLabel.text = QualityManagerment.getQualityColorForBloodItem(colorr, index+1);
        openPoint.SetActive(false);
    }
    /// <summary>
    /// 更新节点的特效 只有属性节点才有特效 技能节点没有特效 如果又有技能 又提升属性 那么按配置表的第一个效果来
    /// </summary>
    void updateEffect()
    {
        bloodEffect bloodEffects = bloodItemSample.effects[0];//第一个属性
        if (bloodEffects.type==5||bloodEffects.type==6)//只有增加新技能才显示技能图标
        {
            skillMask.gameObject.SetActive(false);
            bloodItemBg.gameObject.SetActive(true);
            SkillSample newSk = SkillSampleManager.Instance.getSkillSampleBySid(bloodEffects.skillSid);
            SkillSample oldSk = SkillSampleManager.Instance.getSkillSampleBySid(bloodEffects.drSkillSid);
            if (bloodEffects.type == 5) {
                if (newSk != null)
                    ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.SKILLIMAGEPATH + newSk.iconId, skillIconTexture);
            } else {
                if (oldSk != null) {
                    if (newSk != null) ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.SKILLIMAGEPATH + newSk.iconId, skillIconTexture);
                    else ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.SKILLIMAGEPATH + oldSk.iconId, skillIconTexture);//如果实在没有图标就只有显示特效了哇
                    //skillBg.SetActive(true);
                    return;
                }
            }
            if (pointType != 2) {
                skillMask.gameObject.SetActive(true);
            }
        }
        //skillBg.SetActive(false);
        bloodItemBg.gameObject.SetActive(false);
        updateCrlcirEffect();
    }
    void updateCrlcirEffect()
    {
        if (bloodItemSample.itemType == BloodItemSample.SMALL_ITEM)
        {
            if (colorr - 1 >= 0) {
                NGUITools.AddChild(effectPoint,
                        effect_Small[(colorr - 1) >= effect_Small.Length - 1 ? effect_Small.Length - 1 : (colorr - 1)].gameObject);
                NGUITools.AddChild(effectPoint,
                        openEffect_Small[(colorr - 1) >= effect_Small.Length - 1 ? effect_Small.Length - 1 : (colorr - 1)].gameObject);
            } else {
                NGUITools.AddChild(effectPoint,
                                effect_Small[0].gameObject);
                NGUITools.AddChild(effectPoint,
                        openEffect_Small[0].gameObject);
            }
        }  
        else if (bloodItemSample.itemType == BloodItemSample.MADDLE_ITEM) {
            if (colorr - 1 >= 0) {
                NGUITools.AddChild(effectPoint,
                        effect_Medium[(colorr - 1) >= effect_Medium.Length - 1 ? effect_Medium.Length - 1 : (colorr - 1)].gameObject);
                NGUITools.AddChild(effectPoint,
                        openEffect_Medium[(colorr - 1) >= effect_Medium.Length - 1 ? effect_Medium.Length - 1 : (colorr - 1)].gameObject);
            } else {
                NGUITools.AddChild(effectPoint,
                                    effect_Medium[0].gameObject);
                NGUITools.AddChild(effectPoint,
                        openEffect_Medium[0].gameObject);
            }
        }else if (bloodItemSample.itemType == BloodItemSample.BIG_ITEM)
        {
            if (colorr -1 >= 0) {
                NGUITools.AddChild(effectPoint,
                        effect_Big[(colorr - 1) >= effect_Big.Length - 1 ? effect_Big.Length - 1 : colorr - 1].gameObject);
                NGUITools.AddChild(effectPoint,
                        openEffect_Big[(colorr - 1) >= effect_Big.Length - 1 ? effect_Big.Length - 1 : colorr - 1].gameObject);
            } else {
                NGUITools.AddChild(effectPoint,
                            effect_Big[0].gameObject);
                NGUITools.AddChild(effectPoint,
                        openEffect_Big[0].gameObject);
            }
            
        }
            
    }
    public override void DoClickEvent()
    {
        if (pointType == 1)
        {
            PrizeSample[] ps = bloodItemSample.condition;
            if (chooseCard.getEvoLevel() < bloodItemSample.evoLvCondition)
            {
                UiManager.Instance.openDialogWindow<MessageLineWindow>((win) => {
                    win.Initialize(LanguageConfigManager.Instance.getLanguage("bloldEvoError"));
                });
                return;
            }
            for (int i=0;i<ps.Length;i++)
            {
                if (ps[i].getPrizeHadNum() < StringKit.toInt(ps[i].num))
                {
                    UiManager.Instance.openDialogWindow<MessageLineWindow>((win) => {
                    win.Initialize(LanguageConfigManager.Instance.getLanguage("bloodItemDec5", ps[i].getPrizeName()));
                     });
                    return;
                }  
            }
            BloodEvolutionWindow beWin = fatherWindow as BloodEvolutionWindow;
            if (beWin != null) {
                beWin.oldCardold = chooseCard.Clone() as Card;
            }
            if (lineType == 0) {
                if (chooseCard.getQualityId() != QualityType.MYTH) {
                    UiManager.Instance.openDialogWindow<BigItemMessageWindow>((win) => {
                        win.initWindow(chooseCard.Clone() as Card, (msg) => {
                            if (msg.msgEvent == msg_event.dialogOK) {
                                BloodLineFPort fportt = FPortManager.Instance.getFPort<BloodLineFPort>();
                                fportt.access(chooseCard.uid, updateSuccess);
                            } else {
                                MaskWindow.UnlockUI();
                            }
                        });
                    });
                    return;
                }
            }
            BloodLineFPort fport = FPortManager.Instance.getFPort<BloodLineFPort>();
            fport.access(chooseCard.uid,updateSuccess);
            //MaskWindow.UnlockUI();
        }
        else
        {
            isBloodItem = true;
            MaskWindow.UnlockUI();
        }
    }

    void updateSuccess()
    {
        //UiManager.Instance.openDialogWindow<MessageLineWindow>((win) =>
        //{
        //    win.Initialize("success!!!!!!!!!");
        //});
        if (bloodItemSample.effects[0].type != 5)
        {
            updateEffect();
        }
        else
        {
            skillMask.gameObject.SetActive(false);
        }
        updateLine();//主界面的属性变化
        if (canOpenPoint != null) {
            TweenAlpha ta = TweenAlpha.Begin(canOpenPoint, 1, 1);
            ta.to = 0f;
            EventDelegate.Add(ta.onFinished, () => {
                canOpenPoint.SetActive(false);
                Utils.DelayRun(() => { MaskWindow.UnlockUI(); },0.8f);
            }, true);
        }
        if (fatherWindow != null)
        {
            BloodEvolutionWindow beWin = fatherWindow as BloodEvolutionWindow;
            if (beWin != null) {
                beWin.isPressed = true;
                beWin.bloodSuccessCallBack(bloodItemSample, new Vector3(transform.localPosition.x, transform.localPosition.y + faTherTransform.localPosition.y, transform.localPosition.z), lineType); 
            }
        }

    }
    public override void DoLongPass() {
        if (pointType == 1) {
            MaskWindow.UnlockUI();
        } else openPoint.SetActive(true);
    }
    public override void LongPassFinish() {
        openPoint.SetActive(false);
        isBloodItem = false;
        MaskWindow.UnlockUI();
    }
    void updateLine() {
        if (lineType == 0) {
            bloodLine1.gameObject.SetActive(false);
            bloodLine2.gameObject.SetActive(false);
        } else if (lineType == 1) {
            bloodLine1.gameObject.SetActive(true);//左边线亮
            if (canOpenPoint.gameObject.activeInHierarchy) canOpenPoint.transform.localPosition = new Vector3(259f, 0f, 0f);
            bloodLine2.gameObject.SetActive(false);
            if (pointType==2) bloodLine1.color = getColor(colorr);
        } else {
            bloodLine1.gameObject.SetActive(false);
            bloodLine2.gameObject.SetActive(true);//右边线亮
            if(canOpenPoint.gameObject.activeInHierarchy)canOpenPoint.transform.localPosition=new Vector3(0f,0f,0f);
            if(pointType==2)bloodLine2.color = getColor(colorr);
        }
    }
    private Color getColor(int lv) {
        switch (lv) {
            case 0:
                return Colors.EVOLUTIONCOLOR_GTEEN;
            case 1:
                return Colors.EVOLUTIONCOLOR_BLUE;
            case 2:
                return Colors.EVOLUTIONCOLOR_PURPLE;
            case 3:
                return Colors.EVOLUTIONCOLOR_YELLOW;
            default:
                return Colors.EVOLUTIONCOLOR_RED;
        }
    }
    

}
