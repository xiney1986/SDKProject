using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 共鸣--装备强化容器
/// </summary>
public class Equiqianghuacontent :MonoBase
{
    private Card selectCard;//选择的卡片
    private WindowBase fatherWindow ;//父类
    public GameObject zhuangbenqianghGameObject;//装备强化信息点
    public GameObject eqPerfbatPoint;//装备预制体挂机点
    public EquipIndyPerfab equipIndyPerfab;
    public UILabel lvInfoTile;//强化等级标题
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
    private int showTypeNum;//你从哪里来
    public ServerCardMsg cardMsg;//别人的卡
    private List<Equip> showChatEquips;//别人卡的装备
    public UILabel[] secOldAttrVal;//老的属性值
    public UILabel[] secNewAttrVal;//新的属性值
    public UILabel[] secOldAttrName;//老的名称
    public UILabel[] secNewAttrName;//新的名称
    public UILabel[] secOldPerAttrVal;//老的百分比加成
    public UILabel[] secNewPerAttrVal;//新的百分比加成
    public void init(Card card,WindowBase win,int showType)
    {
        selectCard = card;
        fatherWindow = win;
        showTypeNum = showType;
        if (card == null) return;
        updateEqItem();
        updateInfo();      
    }
    public void initServer(ServerCardMsg card,WindowBase win,int showType)
    {
        selectCard = card.card;
        cardMsg = card;
        fatherWindow = win;
        showTypeNum = showType;
        showChatEquips=card.showEquips;
        updateEqItem();
        updateInfo();
    }
    /// <summary>
    ///  更新装备显示节点
    /// </summary>
    private void updateEqItem()
    {
        
        foreach (Transform each in eqPerfbatPoint.transform) {
            Destroy(each.gameObject);
        }
        List<Equip> tempList;
        if (showTypeNum == CardBookWindow.CLICKCHATSHOW) tempList = cardMsg.showEquips; //别人的装备
        else tempList = selectCard.getEquipList();//自己的装备
        for (int i = 0; i < 5; i++) {
            GameObject item = NGUITools.AddChild(eqPerfbatPoint, equipIndyPerfab.gameObject);
            item.transform.localPosition = new Vector3(i % 2 == 0 ? -128f : 140f, i < 2 ? 0 : (i < 4 ? -98f : -196f), 0);
            EquipIndyPerfab eif = item.GetComponent<EquipIndyPerfab>();
            if (tempList == null || tempList.Count <= 0) {
                eif.init(fatherWindow, null, i + 1, showTypeNum);
            } else {
                bool flag = false;
                for (int k = 0; k < tempList.Count; k++) {
                    int tmp = tempList[k].getPartId();
                    if (tmp == i + 1) {
                        flag = true;
                        eif.init(fatherWindow, tempList[k], 0, showTypeNum);
                        break;
                    }
                }
                if (!flag) eif.init(fatherWindow, null, i + 1, showTypeNum);
            }
        }
    }
    /// <summary>
    /// 更新属性信息
    /// </summary>
    private void updateInfo()
    {
//        if (showTypeNum == CardBookWindow.CLICKCHATSHOW)
//        {
//            ResonanceSampleManager.Instance.showChatEq(showChatEquips);
//            ResonanceSampleManager.Instance.showNum(true); 
//        }
        for (int i=0;i<5;i++)
        {
            secOldAttrVal[i].gameObject.SetActive(false);
            secOldAttrName[i].gameObject.SetActive(false);
            secNewAttrVal[i].gameObject.SetActive(false);
            secNewAttrName[i].gameObject.SetActive(false);
        }
        int[] a=new int[5];
        int[] b = new int[5];
        int[] aa = new int[5];
        int[] bb = new int[5];
        string[] gongmingName = new string[5];
        string[] gongmingName2 = new string[5];
        int oldResonancelv=0;
        int oldNeedlv=0;
        int newResonancelv=0;
        int newNeedlv=0;
        for (int l = 0; l < 2;l++ )
        {
			if (showTypeNum == CardBookWindow.CLICKCHATSHOW)
			{
				ResonanceSampleManager.Instance.showChatEq(showChatEquips);
				ResonanceSampleManager.Instance.showNum(true); 
			}
            List<ResonanceSample> rsss = ResonanceSampleManager.Instance.getrssList(1, selectCard, 0,l);
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
                        {
                            bb =ResonanceSampleManager.Instance.caseType(bb, acs.getAttrType(), acs.getAttrValue(0));
                        }
                    }
                    else
                    {
                        if (l == 0)
                        {
                            a = ResonanceSampleManager.Instance.caseType(a, acs.getAttrType(), acs.getAttrValue(0));
                        }
                        else
                        {
                            aa = ResonanceSampleManager.Instance.caseType(aa, acs.getAttrType(), acs.getAttrValue(0));
                        }
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
            if (b[i] != 0)
            {
                secOldPerAttrVal[i].gameObject.SetActive(true);
                secOldPerAttrVal[i].text = "+" + b[i].ToString() + "%";
            }
            if (aa[i] == 0)
            {
                secNewAttrVal[i].gameObject.SetActive(false);
            }
            if (bb[i] != 0)
            {
                secNewPerAttrVal[i].gameObject.SetActive(true);
                secNewPerAttrVal[i].text = "+" + bb[i].ToString() + "%";
            }
        }
        oldLvInfo.text = LanguageConfigManager.Instance.getLanguage("resonanceWindow03", oldResonancelv + "");
        lvInfoTile.text = LanguageConfigManager.Instance.getLanguage("resonanceWindow03", oldResonancelv + "");
        oldLvInfoDec.text = LanguageConfigManager.Instance.getLanguage("resonanceWindow04", oldNeedlv + "");
        newLvInfo.text = LanguageConfigManager.Instance.getLanguage("resonanceWindow03", newResonancelv + "");
        if (oldResonancelv == newResonancelv)
            newLvInfo.text = LanguageConfigManager.Instance.getLanguage("resonancewindow17");
        newLvInfoDec.text = LanguageConfigManager.Instance.getLanguage("resonanceWindow04", newNeedlv + "");        
    }    
}