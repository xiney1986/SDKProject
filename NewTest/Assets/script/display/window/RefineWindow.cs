using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RefineWindow : WindowBase
{
    public Equip upequip;//强化的装备
    private Equip oldupequip;//老的装备-克隆
    public UITexture equipbg;//装备图片
    public UISprite[] upTexture;//提升箭头
    public int equipIconID;//武器ICONID
    public UILabel[] shuxing1;//属性提升LAable
    public UILabel[] shuxing2;
    public UILabel[] shuxing3;//属性提升多少
    public ExpbarCtrl expbar;//经验条
    public UILabel expLabel;//经验
    public UILabel equipLv;//等级
    public UILabel[] rockNum;//精炼石
    LevelupInfo levelInfo;//经验条的等级信息
    private int[] rockEXP;//精炼石提供的经验
    public ResolveEffect resolveEffect;//道具数量
    public GameObject[] objPos;//精炼石的位置
    public GameObject[] objPostion;//飘文字的坐标
    //用奖励模板对象
    public List<PrizeSample> prizeList = new List<PrizeSample>();
    int[] num1;//道具的数量
    public GameObject ctrl;//动画预制件
    public long propEXP;//经验值
    public GameObject ctrl1;//添加预制件的根
    public GameObject postion;//动画起始坐标
    public GameObject upanimation;//升级动画预制件
    private int clickNum;//点击的按钮
    public GameObject bg;//升级动画根
    public GameObject button1;//四个按钮
    public GameObject button2;
    public GameObject button3;
    public GameObject button4;
    private float start_Time;//记录开始时间
    private bool timebool;
    public float timeJ=0.4f;
    private GameObject timeObj;
    private int useNum = 0;//使用的道具数量
    public UISprite[] sprites;//属性的图片1
    public UISprite[] sprites2;//属性的图片2
    public UISprite[] rootsprites;//属性图片的显示1
    public UISprite[] rootsprites2;//属性图片的显示1
    private int[] refinePropSid;//获得精炼道具的SID
    public UILabel noRefine;//0级时显示
    public UILabel maxRefine;//满级时显示
    public ButtonBase guideButton;


    protected override void begin()
    {
        base.begin();
        //updateContent();
        updateExpBar();
        UIEventListener.Get(button1).onPress = longPress;
        UIEventListener.Get(button2).onPress = longPress;
        UIEventListener.Get(button3).onPress = longPress;
        UIEventListener.Get(button4).onPress = longPress;
        if (GuideManager.Instance.getOnTypp() == 30)
        {
            guideButton.gameObject.GetComponent<BoxCollider>().enabled=true;
            button1.GetComponent<BoxCollider>().enabled=false;
            button2.GetComponent<BoxCollider>().enabled = false;
            button3.GetComponent<BoxCollider>().enabled = false;
            button4.GetComponent<BoxCollider>().enabled = false;
            GuideManager.Instance.doFriendlyGuideEvent();
        }
        else
        {
            guideButton.gameObject.GetComponent<BoxCollider>().enabled=false;
        }
        MaskWindow.UnlockUI();
    }
    void Update()
    {
        if(timebool)
        {
            if(Time.time-start_Time>=timeJ)
            {
                doLongClickPress(timeObj);
				timebool = false;
				start_Time = 0;   
            }
        }
    }
    public void longPress(GameObject obj,bool isPress)
    {
        GuideManager.Instance.doFriendlyGuideEvent();
        if(isPress)
        {
            timeObj = obj;
            start_Time = Time.time;
            timebool = true;
        }
        else
        {
            timebool = false;
            if (Time.time - start_Time < timeJ)
            {
                buttonEvent(timeObj);
            }
        }
    }
    /// <summary>
    /// 长按执行
    /// </summary>
    /// <param name="gameObj"></param>
    public void doLongClickPress(GameObject gameObj)
    {
		MaskWindow.LockUI();
        //升一级需要的经验
        float needEXP = upequip.getRefineEXPUp() - upequip.getrefineEXP();
        //道具提供的最大经验
        string uid = upequip.uid;
        int indexx = StringKit.toInt(gameObj.name);
        float storeExp = num1[indexx - 1] * rockEXP[indexx - 1];
        propEXP = rockEXP[indexx - 1];
        int sid = prizeList[indexx - 1].pSid;
        int useId = indexx - 1;
        int propNum = num1[indexx - 1];
        clickNum = indexx - 1;
        if (storeExp >= needEXP)
        {
            useNum = calculate(needEXP, rockEXP[indexx - 1]);
        }
        if(useNum!=0&&storeExp>=needEXP)
        {
            
            clickUp(gameObj, uid, sid, useNum, useId);
        }
        else 
        {    
            clickUp(gameObj, uid, sid, propNum, useId);
        }
    }
    /// <summary>
    /// 计算数量
    /// </summary>
    /// <param name="needExp"></param>
    /// <param name="rockEXP"></param>
    /// <returns></returns>
    private int calculate(float needExp, float rockEXP)
    {
        float num = needExp / rockEXP;
        return Mathf.CeilToInt(num);
    }
    public void updateContent()
    {
        upequip = StorageManagerment.Instance.getEquip(upequip.uid);
        equipIconID = EquipmentSampleManager.Instance.getEquipSampleBySid(upequip.sid).iconId;
        ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.ICONIMAGEPATH + equipIconID, equipbg);
        int maxLevel = upequip.getRefineMaxLevel();
        int nowLevel = upequip.getrefineLevel();
        int[] a = new int[3];
        string[] b = new string[3];
        int[] aa = new int[3];
        string[] bb = new string[3];
        if (nowLevel == 0)
        {
            noRefine.gameObject.SetActive(true);
        }
        else
        {
            noRefine.gameObject.SetActive(false);
            RefinelvInfo newrfinfo = RefineSampleManager.Instance.getRefineSampleBySid(upequip.sid).refinelvAttr[nowLevel];
            for (int j = 0; j < newrfinfo.items.Count; j++) {
                AttrRefineChangeSample acs = newrfinfo.items[j];
                for (int k = 0; k < 3; k++) {
                    if (b[k] == null) {
                        b[k] = acs.getAttrType();
                        a[k] += acs.getAttrRefineValue(0);
                        break;
                    }
                    if (b[k] != acs.getAttrType()) continue;
                    a[k] += acs.getAttrRefineValue(0);
                    break;
                }
            }
            for (int j = 0; j < 3; j++)
            {
                if (b[j] == null) continue;
                shuxing1[j].text = a[j].ToString();
                rootsprites[j].gameObject.SetActive(true);
                sprites[j].GetComponent<UISprite>().spriteName = "attr_" + getAttrTypePer(b[j]);
            }
        }
        if (nowLevel == maxLevel)
        {
            shuxing2[0].gameObject.SetActive(false);
            //shuxing2[1].text = LanguageConfigManager.Instance.getLanguage("refine_023");
            maxRefine.gameObject.SetActive(true);
            shuxing2[2].gameObject.SetActive(false);
            rootsprites2[0].gameObject.SetActive(false);
            rootsprites2[1].gameObject.SetActive(false);
            rootsprites2[2].gameObject.SetActive(false);
            upTexture[0].gameObject.SetActive(false);
            upTexture[1].gameObject.SetActive(false);
            upTexture[2].gameObject.SetActive(false);
            shuxing3[0].gameObject.SetActive(false);
            shuxing3[1].gameObject.SetActive(false);
            shuxing3[2].gameObject.SetActive(false);
        }
        else
        {

            int nextlevel = upequip.getrefineLevel() + 1;
            RefinelvInfo nextrfinfo = RefineSampleManager.Instance.getRefineSampleBySid(upequip.sid).refinelvAttr[nextlevel];
            for (int j = 0; j < nextrfinfo.items.Count; j++) {
                AttrRefineChangeSample acs = nextrfinfo.items[j];
                for (int k = 0; k < 3; k++) {
                    if (bb[k] == null) {
                        bb[k] = acs.getAttrType();
                        aa[k] += acs.getAttrRefineValue(0);
                        break;
                    }
                    if (bb[k] != acs.getAttrType()) continue;
                    aa[k] += acs.getAttrRefineValue(0);
                    break;
                }
            }
            for (int j = 0; j < 3; j++)
            {
                if (bb[j] == null) continue;
                int upnum = 0;
                bool falg = false;
                for (int n=0;n<3;n++)
                {
                    if(b[n]==null)continue;
                    if (b[n] == bb[j])
                    {
                        upnum = aa[j] - a[n];
                        falg = true;
                        break;
                    }
                }
                if (!falg)
                {
                    upnum = aa[j];
                }
               // int upnum = aa[j] - a[j];
                shuxing2[j].text = (aa[j]-upnum).ToString(); //+" " + "[358C35]" + "+" + " " + upnum.ToString() + "[-]";
                shuxing3[j].text = " " + "[358C35]" + "+" + " " + upnum + "[-]";
                sprites2[j].GetComponent<UISprite>().spriteName = "attr_" + getAttrTypePer(bb[j]);
                rootsprites2[j].gameObject.SetActive(true);
                upTexture[j].gameObject.SetActive(false);
                if (upnum!=0)
                {
                    upTexture[j].gameObject.SetActive(true);
                }
            }

        }
        equipLv.text = upequip.getrefineLevel() + " " + LanguageConfigManager.Instance.getLanguage("refine_024");
        expLabel.text = EXPSampleManager.Instance.getExpBarShow(upequip.getRefineExpSid(), upequip.getrefineEXP());
        rockEXP = CommandConfigManager.Instance.getRefinePropEXP();
        refinePropSid = CommandConfigManager.Instance.getRefinePropSid();
        setPrizeShow();
        updateInfo();
    }

    public string getAttrTypePer(string getType)
    {
        if (getType == AttrChangeType.HP)
        {
            return AttrChangeType.HP;
        }
        if (getType == AttrChangeType.ATTACK)
        {
            return AttrChangeType.ATTACK;
        }
        if (getType == AttrChangeType.DEFENSE)
        {
            return AttrChangeType.DEFENSE;
        }
        if (getType == AttrChangeType.AGILE)
        {
            return AttrChangeType.AGILE;
        }
        if (getType == AttrChangeType.MAGIC)
        {
            return AttrChangeType.MAGIC;
        }
        if (getType == AttrChangeType.PER_AGILE)
        {
            return AttrChangeType.AGILE;
        }
        if (getType == AttrChangeType.PER_ATTACK)
        {
            return AttrChangeType.ATTACK;
        }
        if (getType == AttrChangeType.PER_DEFENSE)
        {
            return AttrChangeType.DEFENSE;
        }
        if (getType == AttrChangeType.PER_HP)
        {
            return AttrChangeType.HP;
        }
        if(getType == AttrChangeType.PER_MAGIC)
        {
            return AttrChangeType.MAGIC;
        }
        if (getType==AttrChangeType.DESC1)
        {
            return AttrChangeType.DESC1;
        }
        if(getType==AttrChangeType.DESC2)
        {
            return AttrChangeType.DESC2;
        }
        if(getType==AttrChangeType.DESC3)
        {
            return AttrChangeType.DESC3;
        }
        return AttrChangeType.DESC4;
    }

    public void initialize (Equip chooseItem)
    {
        upequip = chooseItem;
    }
    /// <summary>
    /// 开启精炼升级动画
    /// </summary>
    /// <param name="levelupinfo"></param>
    private void upAnim(LevelupInfo levelupinfo)
    {
        if (levelupinfo.newLevel > levelupinfo.oldLevel)
        {
            NGUITools.AddChild(bg, upanimation);
        }
        updateContent();
        oldupequip = null;
    }
    /// <summary>
    /// 经验条涨
    /// </summary>
    private void updateExpBar()
    {
        LevelupInfo levelupinfo = createLevelupInfo(oldupequip ?? upequip, upequip);
        expLabel.text = EXPSampleManager.Instance.getExpBarShow(upequip.getRefineExpSid(), upequip.getrefineEXP());
        expbar.init(levelupinfo);
        upAnim(levelupinfo);
    }
    private LevelupInfo createLevelupInfo(Equip oldupequip,Equip newupequip)
    {
        LevelupInfo levelupinfo = new LevelupInfo
        {
            newExp = newupequip.getrefineEXP(),
            newExpDown =
                EXPSampleManager.Instance.getRefineEXPDown(newupequip.getRefineExpSid(), newupequip.getrefineLevel()),
            newExpUp =
                EXPSampleManager.Instance.getRefineEXPUp(newupequip.getRefineExpSid(), newupequip.getrefineLevel()),
            newLevel = newupequip.getrefineLevel(),
            oldExp = oldupequip.getrefineEXP(),
            oldExpDown =
                EXPSampleManager.Instance.getRefineEXPDown(oldupequip.getRefineExpSid(), oldupequip.getrefineLevel()),
            oldExpUp =
                EXPSampleManager.Instance.getRefineEXPUp(oldupequip.getRefineExpSid(), oldupequip.getrefineLevel()),
            oldLevel = oldupequip.getrefineLevel()
        };
        return levelupinfo;
    }
    /// <summary>
    /// 点击事件
    /// </summary>
    /// <param name="gameObj"></param>
    public void buttonEvent(GameObject gameObj)
    {
		MaskWindow.LockUI();
        int indexx = StringKit.toInt(gameObj.name);
        propEXP = rockEXP[indexx - 1];
        string uid = upequip.uid;
        int sid = prizeList[indexx - 1].pSid;
        
        clickUp(gameObj, uid, sid, 1, indexx - 1);
        clickNum = indexx - 1;
    }
    /// <summary>
    /// 提示可以长按使用道具
    /// </summary>
    private void openMessage()
    {
        UiManager.Instance.openDialogWindow<MessageLineWindow>(winn => winn.Initialize(LanguageConfigManager.Instance.getLanguage("refine_028")));
    }
    /// <summary>
    /// CLOSE
    /// </summary>
    /// <param name="gameObj"></param>
    public override void buttonEventBase(GameObject gameObj)
    {
        base.buttonEventBase(gameObj);
        if (gameObj.name == "close")
        {
            finishWindow();
        } else if (gameObj.name == "guideButton")
        {
            guideButton.GetComponent<BoxCollider>().enabled = false;
            button1.GetComponent<BoxCollider>().enabled = true;
            button2.GetComponent<BoxCollider>().enabled = true;
            button3.GetComponent<BoxCollider>().enabled = true;
            button4.GetComponent<BoxCollider>().enabled = true;
            GuideManager.Instance.doFriendlyGuideEvent();
        }
//        else if (Time.time - start_Time >= timeJ)
//        {
//            MaskWindow.UnlockUI();
//        }
           

    }
    /// <summary>
    /// 精炼
    /// </summary>
    /// <param name="gameObj"></param>
    /// <param name="uid"></param>
    /// <param name="sid"></param>
    /// <param name="num"></param>
    /// <param name="numId"></param>
    private void clickUp(GameObject gameObj, string uid, int sid, int num,int numId)
    {
        int maxLevel = upequip.getRefineMaxLevel();
        int nowLevel = upequip.getrefineLevel();
        if (num1[numId] == 0)
        {
            doStrengRefineFinshed("goods_error");
        }
        else if (nowLevel == maxLevel)
        {
            doStrengRefineFinshed("maxlevel");
        }
        else
        {

            useNum = num;
            oldupequip = (Equip)upequip.Clone();
            (FPortManager.Instance.getFPort("EquipRefineFPort") as EquipRefineFPort).access(uid, sid, num, doStrengRefineFinshed);
        }
    }

    private void setPrizeShow()
    {
        prizeList.Clear();
        for(int i=0;i<refinePropSid.Length;i++)
        {
            prizeList.Add(new PrizeSample(PrizeType.PRIZE_PROP, refinePropSid[i], 0));
        }
    }
    private void updateInfo()
    {
        num1 = new int[prizeList.Count];
        for (int i = 0; i < prizeList.Count; i++)
        {
            ResourcesManager.Instance.LoadAssetBundleTexture(prizeList[i].getIconPath(), resolveEffect.icon[i]);
            int sid = prizeList[i].pSid;
            Prop tmp = StorageManagerment.Instance.getProp(sid);
            resolveEffect.iconText[i].text ="x"+(tmp == null ? 0 : tmp.getNum());
            num1[i] = tmp == null ? 0 : tmp.getNum();
            if (num1[i] == 0)
            {
                objPos[i].transform.FindChild("iconbg2").gameObject.SetActive(true);
            }
        }
    }
    private void doStrengRefineFinshed(string type)
    {
        if (type == "ok")
        {
            StartCoroutine(beginFlyNum(clickNum));
        }
        else
        {
            string des="";
            switch (type)
            {
                case "equip_error":
                    des = "refine_025";
                    break;
                case "goods_error":
                    des = "refine_026";
                    break;
                case "maxlevel":
                    des = "refine_029";
                    break;
                case "condition_limit":
                    des = "refine_027";
                    break;
            }
            UiManager.Instance.openDialogWindow<MessageLineWindow>(winn => winn.Initialize(LanguageConfigManager.Instance.getLanguage(des)));
			MaskWindow.UnlockUI();
        }
    }
    private IEnumerator beginFlyNum(int num)
    {
        
        SpriteNumCtrl snc = NGUITools.AddChild(ctrl1, ctrl).GetComponent<SpriteNumCtrl>();
        snc.gameObject.transform.localPosition = objPostion[num].gameObject.transform.localPosition;
        StartCoroutine(snc.flyNum(new Vector3(postion.transform.localPosition.x, postion.transform.localPosition.y, 0f), propEXP,useNum));

		MaskWindow.UnlockUI();
        yield return new WaitForSeconds(0.3f);
        updateExpBar();
        yield return new WaitForSeconds(0.8f);
        openMessage();
        
    }

}
