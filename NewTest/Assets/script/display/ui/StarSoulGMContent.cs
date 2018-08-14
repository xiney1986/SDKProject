using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StarSoulGMContent : MonoBase
{
    private Card selectCard;//选择的卡片
    private WindowBase fatherWindow;//父类
    public GameObject zhuangbenqianghGameObject;//装备强化信息点
    public GameObject eqPerfbatPoint;//装备预制体挂机点
    public EquipIndyPerfab equipIndyPerfab;
    public SoulInfoButtonItem soulInfoPerfab;
    public SoulIndyPerfab soulIndyPerfab;
    public UILabel lvInfoTile;//星魂强化等级
    public UILabel oldLvInfo;//装备强化等级
    public UILabel oldLvInfoDec;//装备强化等级描述
    public UILabel newLvInfo;//装备强化等级
    public UILabel newLvInfoDec;//装备强化等级描述
    public GameObject[] oldAttrPoints;//老的属性点
    public GameObject[] newAttrPoints;//新德属性点
    public UISprite[] oldAttrSprites;//老的属性描述
    public UISprite[] newattrSprites;//新德属性描述
    public UILabel[] oldAttrVal;//老的属性值
    public UILabel[] newAttrVal;//新的属性值
    public UILabel[] newAddAttrVal;//新增加的属性值
    private int currentStarBroeIndex;//在星魂槽的位置
    private int showTypeNum;
    public ServerCardMsg cardMsg;//别人的卡
    private string chatPlayerUid;//别人的UID-获取等级
    private PvpOppInfo pvpOppInfo;
    public UILabel[] secOldAttrVal;//老的属性值
    public UILabel[] secNewAttrVal;//新的属性值
    public UILabel[] secOldAttrName;//老的名称
    public UILabel[] secNewAttrName;//新的名称
    public UILabel[] secOldPerAttrVal;//老的百分比加成
    public UILabel[] secNewPerAttrVal;//新的百分比加成


    public void init(Card card, WindowBase win,int showType)
    {
        selectCard = card;
        fatherWindow = win;
        showTypeNum = showType;
        if (card == null) return;
        updateEqItem();
        updatePlayerInfo(null);

    }
    public void initServer(ServerCardMsg card, WindowBase win, int showType,string _uid)
    {
        selectCard = card.card;
        fatherWindow = win;
        showTypeNum = showType;
        chatPlayerUid = _uid;
        updateEqItem();

        ChatGetPlayerLevelFPort fport = FPortManager.Instance.getFPort("ChatGetPlayerLevelFPort") as ChatGetPlayerLevelFPort;
        fport.access(_uid, 5, updatePlayerInfo, PvpPlayerWindow.FROM_CHAT);
    }
    /// <summary>
    ///  更新星魂显示节点
    /// </summary>
    private void updateEqItem()
    {
        //获取星魂槽上的所有星魂
        
        foreach (Transform each in eqPerfbatPoint.transform)
        {
            Destroy(each.gameObject);
        }
        ResonanceSampleManager.Instance.showNum(showTypeNum == CardBookWindow.CLICKCHATSHOW);
        ArrayList arrayList = selectCard.getStarSoulArrayList();
        for (int i=0;i<6;i++)
        {
            GameObject item = NGUITools.AddChild(eqPerfbatPoint, soulIndyPerfab.gameObject);
            item.transform.localPosition = new Vector3(i % 2 == 0 ? -128f : 140f, i < 2 ? 0 : (i < 4 ? -98f : -196f), 0);
            SoulIndyPerfab sif = item.GetComponent<SoulIndyPerfab>();
            if (arrayList == null || arrayList.Count <= 0)
            {
                sif.init(fatherWindow, null, selectCard, showTypeNum, i);
            }
            else
            {
                bool falg=false;
                for (int j=0;j<arrayList.Count;j++)
                {
                    StarSoulBore ssb = (StarSoulBore)arrayList[j];
                    if (ssb.getIndex() == i + 1)
                    {
                        falg = true;
                        if (showTypeNum == CardBookWindow.CLICKCHATSHOW)
                            sif.init(fatherWindow, new StarSoul("0", ssb.getSid(), ssb.getExp(), 0), selectCard,
                                showTypeNum, i + 1);
                        else sif.init(fatherWindow, StorageManagerment.Instance.getStarSoul(ssb.getUid()), selectCard,
                                showTypeNum, i + 1);
                    }
                    if (!falg) sif.init(fatherWindow, null, selectCard, showTypeNum, i);                    
                }
            }

        }
    }
    private void updatePlayerInfo(PvpOppInfo pvpPlayer)
    {
        pvpOppInfo = pvpPlayer;
                              
        for (int i = 0; i < 5; i++)
        {
            secOldAttrVal[i].gameObject.SetActive(false);
            secOldAttrName[i].gameObject.SetActive(false);
            secNewAttrVal[i].gameObject.SetActive(false);
            secNewAttrName[i].gameObject.SetActive(false);
        }
        int[] a = new int[5];
        int[] b = new int[5];
        int[] aa = new int[5];
        int[] bb = new int[5];
        string[] gongmingName = new string[5];
        string[] gongmingName2 = new string[5];
        int oldResonancelv = 0;
        int oldNeedlv = 0;
        int newResonancelv = 0;
        int newNeedlv = 0;
        //下一等级
        for (int l = 0; l < 2; l++)
        {
            if (showTypeNum == CardBookWindow.CLICKCHATSHOW)
            {
                ResonanceSampleManager.Instance.showNum(showTypeNum == CardBookWindow.CLICKCHATSHOW);
            }
            List<ResonanceSample> rsss = ResonanceSampleManager.Instance.getrssList(3, selectCard, pvpOppInfo == null ? 0 : pvpOppInfo.exp, l);
        for (int i = 0; i < rsss.Count; i++)
        {
            ResonanceSample newRsS = rsss[i];
            if (l == 0)
            {
                oldResonancelv = newRsS.resonanceLv;
                oldNeedlv = newRsS.needLv;
            }
            else
            {
                newResonancelv = newRsS.resonanceLv;
                newNeedlv = newRsS.needLv;
            }
            ResonanceInfo newRiI = newRsS.resonanceAttr;
            for (int j = 0; j < newRiI.items.Count; j++)
            {
                AttrChangeSample acs = newRiI.items[j];
                if (acs.getAttrType() == AttrChangeType.PER_AGILE || acs.getAttrType() == AttrChangeType.PER_ATTACK || acs.getAttrType() == AttrChangeType.PER_DEFENSE || acs.getAttrType() == AttrChangeType.PER_HP || acs.getAttrType() == AttrChangeType.PER_MAGIC)
                {
                    if (l == 0)
                    {
                        b = ResonanceSampleManager.Instance.caseType(b, acs.getAttrType(), acs.getAttrValue(0));
                    }
                    else
                        bb = ResonanceSampleManager.Instance.caseType(bb, acs.getAttrType(), acs.getAttrValue(0));
                }
                else
                {
                    if (l == 0)
                    {
                        a = ResonanceSampleManager.Instance.caseType(a, acs.getAttrType(), acs.getAttrValue(0));
                    }
                    else
                        aa = ResonanceSampleManager.Instance.caseType(aa, acs.getAttrType(), acs.getAttrValue(0));
                }
                gongmingName = ResonanceSampleManager.Instance.caseName(gongmingName, acs.getAttrType());
                gongmingName2 = ResonanceSampleManager.Instance.caseName(gongmingName2, acs.getAttrType());
            }
        }
    }
        for (int i = 0; i < 5; i++)
        {
            secNewAttrVal[i].gameObject.SetActive(true);
            secNewAttrName[i].gameObject.SetActive(true);
            secNewAttrVal[i].text = aa[i].ToString();
            secNewAttrName[i].text = gongmingName2[i];
            secOldAttrName[i].gameObject.SetActive(true);
            secOldAttrVal[i].gameObject.SetActive(true);
            secOldAttrName[i].text = gongmingName[i];
            secOldAttrVal[i].text = a[i].ToString();
            if (b[i] != 0 || b[i] == 0)//特例
            {
                secOldPerAttrVal[i].gameObject.SetActive(true);
                secOldPerAttrVal[i].text = "+" + b[i] + "%";
            }
            if (aa[i] == 0)
            {
                secNewAttrVal[i].gameObject.SetActive(false);
            }
            if(a[i]==0)
            {
                secOldAttrVal[i].gameObject.SetActive(false);//特例
            }
            if (bb[i] != 0) {
                secNewPerAttrVal[i].gameObject.SetActive(true);
                secNewPerAttrVal[i].text = "+" + bb[i]+ "%";
            }
        }
        lvInfoTile.text = LanguageConfigManager.Instance.getLanguage("resonanceWindow09", oldResonancelv + "");
        oldLvInfo.text = LanguageConfigManager.Instance.getLanguage("resonanceWindow09", oldResonancelv + "");
        oldLvInfoDec.text = LanguageConfigManager.Instance.getLanguage("resonanceWindow10", oldNeedlv + "");
        newLvInfo.text = LanguageConfigManager.Instance.getLanguage("resonanceWindow09", newResonancelv + "");
        if (oldResonancelv == newResonancelv)
            newLvInfo.text = LanguageConfigManager.Instance.getLanguage("resonancewindow17");
        newLvInfoDec.text = LanguageConfigManager.Instance.getLanguage("resonanceWindow10", newNeedlv + "");
    }
}
