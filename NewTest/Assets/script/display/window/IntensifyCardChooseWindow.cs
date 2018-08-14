using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 卡片强化选择窗口
 * @author 汤琦
 **/
public class IntensifyCardChooseWindow : WindowBase
{
    public ContentIntensifyCardChoose content;
    /**没有可用卡片的显示节点 */
    public GameObject noCardPoint;
    /**去炼金 女神摇一摇的button显示文字*/
    public UILabel noCardLabel;
    /**去炼金 女神摇一摇的文字描述*/
    public UILabel noCardDescLabel;
    /** 没有进化卡片提示 */
    public UILabel noEvoCardLabel;
    //private SacrificeWindow win;
    private ArrayList allList;
    private ArrayList showList;
    private ArrayList hideList;
    private ArrayList currentList;//筛选容器
    private int intoType = 0;
    private int selectType = 0;//IntensifyCardManager.FOODCARDSELECT

    private int currentStorageVersion = -999;
    public SortCondition sc;

    public int getSelectType()
    {
        return selectType;
    }
    protected override void DoEnable()
    {
        base.DoEnable();

        UiManager.Instance.backGround.switchBackGround("ChouJiang_BeiJing");
    }
    //置顶未上阵角色
	ArrayList topTeamRole(ArrayList list)
    {
        for (int i = 0; i < list.Count; i++) {//上阵卡排在最后
            if (i >= list.Count - 1) return list;
            if (!ArmyManager.Instance.getTeamCardUidList().Contains((list[i] as Card).uid)) continue;
            //是上阵卡片
            object card = list[i];
            for (int k = i + 1; k < list.Count; k++) {
                if (ArmyManager.Instance.getTeamCardUidList().Contains((list[k] as Card).uid)) continue;
                object tmp = list[k];
                list[i] = tmp;
                list[k] = card;
                break;
            }
        }
		return list;
    }
    /** 移除无法献祭卡 */
    public ArrayList removeFullskillCard(ArrayList list)
    {
        ArrayList siftSkillsFullCard = new ArrayList();
        for (int i = 0; i < list.Count; i++)
        {
            if (!(list[i] as Card).isCanSacrific())
            {
                siftSkillsFullCard.Add(list[i]);
            }
        }
        return siftSkillsFullCard;
    }

    protected override void begin()
    {
        base.begin();
        noCardPoint.SetActive(false);

        if (!isAwakeformHide || currentStorageVersion != StorageManagerment.Instance.RoleStorageVersion)
        {
            currentStorageVersion = StorageManagerment.Instance.RoleStorageVersion;
            allList = StorageManagerment.Instance.getAllRole();
            //设置默认排序
            sc = SortConditionManagerment.Instance.initDefaultSort(SiftWindowType.SIFT_CARDCHOOSE_WINDOW);
            allList = SortManagerment.Instance.cardSort(allList, sc);
            allList = topTeamRole(allList);
            //归类:全部(allList),非上阵卡(showList),上阵卡(hideList),
            recalculateAllCard(allList);
            //选择主卡 or 被传承的卡片
            if (selectType == IntensifyCardManager.MAINCARDSELECT)
            {
                ArrayList mainSelect = StorageManagerment.Instance.getAllRoleByNotToEat();
                //进化
                if (intoType == IntensifyCardManager.INTENSIFY_CARD_EVOLUTION)
                {
                    currentList = chooseEvoMainCard(mainSelect);
					currentList = topTeamRole(currentList);
                    content.Initialize(currentList, selectType, intoType);

                }
                //献祭
                else if (intoType == IntensifyCardManager.INTENSIFY_CARD_SACRIFICE)
                {
                    currentList = removeFullskillCard(mainSelect);
                    currentList = filterFood(mainSelect);
					currentList = topTeamRole(currentList);
                    content.Initialize(currentList, selectType, intoType);
                }
                //传承
                else if (intoType == IntensifyCardManager.INTENSIFY_CARD_INHERIT)
                {
                    currentList = RemoveSpriteCard(mainSelect);
                    currentList = removeMainCard(currentList);
					currentList = topTeamRole(currentList);
                    content.Initialize(currentList, selectType, intoType);
                } 
                //超进化
                else if (intoType == IntensifyCardManager.INTENSIFY_CARD_SUPRE_EVO)
                {
                    currentList = removeMainCard(mainSelect);
                    currentList = getCanSuperEvoCard(currentList);
					currentList = topTeamRole(currentList);
                    content.Initialize(currentList, selectType, intoType);
                } else
                {
                    currentList = filterFood(mainSelect);
					currentList = topTeamRole(currentList);
                    content.Initialize(currentList, selectType, intoType);
                }

            }
                //选择副卡 or 获得传承属性的卡片
            else if (selectType == IntensifyCardManager.FOODCARDSELECT)
            {
                ArrayList list = null;
                //进化
                if (intoType == IntensifyCardManager.INTENSIFY_CARD_EVOLUTION)
                {
                    list = chooseEvoFoodCard(showList);
                }
                    //超进化
                else if (intoType == IntensifyCardManager.INTENSIFY_CARD_SUPRE_EVO)
                {
                    list = chooseEvoFoodCard(showList);
                }
                    //附加
                else if (intoType == IntensifyCardManager.INTENSIFY_CARD_ADDON)
                {
                    list = IntensifyCardManager.Instance.RemoveWasteCard(showList);
                }
                    //传承
                else if (intoType == IntensifyCardManager.INTENSIFY_CARD_INHERIT)
                {
                    list = RemoveNotHaveExpCard(showList);
                    list = removeMainCard(list);   
                }

                //献祭
                else
                {
                    list = removeMainCard(showList);
                    list = RemoveSpriteCard(list);
                    list = RemoveHaveExpCard(list);//优化为显示所有的卡YXZH-7732、、(YXZH -8065 要求剔除有附加，进化，等级经验的卡)
                }
                //currentList是最后过滤完所有条件后的list
                currentList = filterMain(list);
				currentList = topTeamRole(currentList);
                //（进化/超进化）选卡发现没有卡片的话需要提示
                if (intoType == IntensifyCardManager.INTENSIFY_CARD_EVOLUTION || intoType == IntensifyCardManager.INTENSIFY_CARD_SUPRE_EVO)
                {
                    if (list.Count == 0)
                    {
                        noEvoCardLabel.gameObject.SetActive(true);
                    } else
                    {
                        noEvoCardLabel.gameObject.SetActive(false);
                    }
                }
                content.Initialize(currentList, selectType, intoType);

            }
        } else if (SortManagerment.Instance.isIntensifyCardChooseModifyed)
        {
            SortManagerment.Instance.isIntensifyCardChooseModifyed = false;

            updateContent();
        }
        if (intoType == IntensifyCardManager.INTENSIFY_CARD_ADDON && currentList.Count < 1)
        {
            noCardPoint.SetActive(true);
            noCardDescLabel.text = LanguageConfigManager.Instance.getLanguage("go_to_happy1");
            noCardLabel.text = LanguageConfigManager.Instance.getLanguage("go_to_happy");
        } else if (intoType == IntensifyCardManager.INTENSIFY_CARD_SACRIFICE && currentList.Count < 1)
        {
            noCardPoint.SetActive(true);
            noCardDescLabel.text = LanguageConfigManager.Instance.getLanguage("intensifyCardWindow_buttonSelect2");
            noCardLabel.text = LanguageConfigManager.Instance.getLanguage("call_card");
        }
        MaskWindow.UnlockUI();
    }

    /** 移除没有附加，进化，等级经验，进化等级,以及所有特殊用途的卡 */
    public ArrayList RemoveNotHaveExpCard(ArrayList _list)
    {
        ArrayList newlist = new ArrayList();
        foreach (Card tempCard in _list)
        {
            if (tempCard == null)
                continue;
            if (ChooseTypeSampleManager.Instance.isToEat(tempCard))
            {
                continue;
            }
            if (tempCard.getEXP() == 0 && tempCard.getHPExp() == 0 && tempCard.getATTExp() == 0 && tempCard.getDEFExp() == 0 &&
                tempCard.getMAGICExp() == 0 && tempCard.getAGILEExp() == 0 && tempCard.getEvoLevel() == 0 && tempCard.getSkillsExp() == 0)
            {
                continue;
            }

            newlist.Add(tempCard);
        }
        return newlist;
    }

    /** 移除有附加，进化，等级经验的卡 */
    public ArrayList RemoveHaveExpCard(ArrayList _list)
    {
        ArrayList newlist = new ArrayList();
        foreach (Card tempCard in _list)
        {
            if (tempCard == null)
                continue;

            if (tempCard.getEXP() > 0 || tempCard.getHPExp() > 0 || tempCard.getATTExp() > 0 || tempCard.getDEFExp() > 0 ||
                tempCard.getMAGICExp() > 0 || tempCard.getAGILEExp() > 0 || tempCard.getEvoLevel() > 0)
            {
                continue;
            }

            newlist.Add(tempCard);
        }
        return newlist;
    }

    /** 移除满级卡 */
    public ArrayList RemoveMaxLvCard(ArrayList _list)
    {
        ArrayList newlist = new ArrayList();
        foreach (Card each in _list)
        {
            if (each == null)
                continue;

            if (each.isMaxLevel())
                continue;

            newlist.Add(each);
        }
        return newlist;
    }

    /** 移除金钱卡和附加卡 */
    public ArrayList RemoveSpriteCard(ArrayList list)
    {
        ArrayList newlist = new ArrayList();
        foreach (Card each in list)
        {
            if (each == null)
                continue;

            if (ChooseTypeSampleManager.Instance.isToEat(each, ChooseTypeSampleManager.TYPE_MONEY_NUM) ||
                ChooseTypeSampleManager.Instance.isToEat(each, ChooseTypeSampleManager.TYPE_ADDON_NUM))
                continue;

            newlist.Add(each);
        }
        return newlist;
    }

    /** 过滤主卡 */
    private ArrayList filterMain(ArrayList list)
    {
        ArrayList temp = new ArrayList();
        for (int i = 0; i < list.Count; i++)
        {
            temp.Add(list[i]);
        }
        for (int i = 0; i < list.Count; i++)
        {
            Card card = list[i] as Card;
            if (IntensifyCardManager.Instance.compareMainCard(card))
            {
                temp.Remove(card);
                break;
            }
        }
        return temp;
    }

    /** 过滤副卡 */
    private ArrayList filterFood(ArrayList list)
    {
        ArrayList temp = new ArrayList();
        for (int i = 0; i < list.Count; i++)
        {
            temp.Add(list[i]);
        }
        for (int i = 0; i < list.Count; i++)
        {
            Card card = list[i] as Card;
            if (IntensifyCardManager.Instance.isInFood(card))
            {
                temp.Remove(card);
            }
        }
        currentList = temp;
        return temp;
    }

    public void initWindow(int intoType, int selectType)
    {
        this.intoType = intoType;
        this.selectType = selectType;

    }
    /*
    public void reLoadMatchingCard ()
    {
        ArrayList list = removeMainCard (showList);
        IntensifyCardManager.Instance.topIntensifyCard (list);
        content.reLoad (list,selectType);
        currentList = list;
        //当没有可吃卡时，显示提示
    }
*/


    public override void buttonEventBase(GameObject gameObj)
    {
        base.buttonEventBase(gameObj);
        if (gameObj.name == "buttonConfirm" || gameObj.name == "close")
        {
            GuideManager.Instance.doGuide();
            finishWindow();
        } else if (gameObj.name == "buttonSift")
        {
            UiManager.Instance.openWindow<SiftCardWindow>((win) =>
            {
                win.Initialize(null, SiftCardWindow.INTENSIFYCARD, SiftWindowType.SIFT_CARDCHOOSE_WINDOW);
            });
        } else if (gameObj.name == "buttoncall")
        {
            if (intoType == IntensifyCardManager.INTENSIFY_CARD_ADDON)
            {
                UiManager.Instance.openWindow<NoticeWindow>((winnn) =>
                {
                    winnn.entranceId = NoticeSampleManager.Instance.getNoticeSampleBySid(NoticeType.GODDNESS_SHAKE_SID).entranceId;
                    winnn.updateSelectButton(NoticeType.GODDNESS_SHAKE_SID);
                });
            } else if (intoType == IntensifyCardManager.INTENSIFY_CARD_SACRIFICE)
            {
                UiManager.Instance.openWindow<LuckyDrawWindow>();
            }

        }
    }

    public ArrayList getCanSuperEvoCard(ArrayList list)
    {
        ArrayList newList = new ArrayList();
        foreach (Card each in list)
        {
            /** 只要能够超进化的卡 */
            if (each.isInSuperEvo())
            {
                newList.Add(each);
            }
        }
        return newList;
    }

    /** 过滤主角卡(非选中的主卡) */
    public ArrayList removeMainCard(ArrayList list)
    {
        //如果选中的进化卡不是主卡，那么在准备显示的list中移除主卡;
        ArrayList newlist = new ArrayList();
        foreach (Card each in list)
        {
            if (each.uid != UserManager.Instance.self.mainCardUid)
            {
                newlist.Add(each);
            }
        }
        return newlist;
    }

    /// <summary>
    /// 选择可以普通进化的主卡数据 
    /// </summary>
    public ArrayList chooseEvoMainCard(ArrayList list)
    {
        ArrayList newlist = new ArrayList();
        foreach (Card each in list)
        {
            /** 剔除处于超进化阶段, 主角卡和已经进化到最大等级的卡片*/
            if (!each.isInSuperEvo() && each.uid != UserManager.Instance.self.mainCardUid & !EvolutionManagerment.Instance.isMaxEvoLevel(each))
                newlist.Add(each);
        }
        return newlist;
    }


    /// <summary>
    /// （进化/超进化）选择副卡
    /// </summary>
    public ArrayList chooseEvoFoodCard(ArrayList list)
    {
        ArrayList newList = new ArrayList();
        foreach (Card each in list)
        {
            /** 过滤主角卡 */
            if (each.uid == UserManager.Instance.self.mainCardUid)
                continue;
            /** 过滤不同主卡类型的卡 */
            if (each.getEvolveNextSid() != IntensifyCardManager.Instance.getMainCardType())
                continue;
            /** 过滤具有进化等级的卡 */
            if (each.getEvoLevel() != 0)
                continue;
            /** 过滤已经选为副卡的卡 */
            if (IntensifyCardManager.Instance.isInFood(each))
                continue;
            newList.Add(each);
        }
        return newList;
    }

    //重新计算所有的卡
    public void recalculateAllCard(ArrayList list)
    {
        showList = new ArrayList();
        hideList = new ArrayList();
        foreach (Card each in list)
        {
            showList.Add(each);
        }
        if (intoType == IntensifyCardManager.INTENSIFY_CARD_INHERIT)
        {
            return;
        }
        //筛选出临时队伍中的 fix bug 1880
//        for (int i = 0; i < showList.Count; i++)
//        {
//
//            if (ArmyManager.Instance.getAllEditArmyCards() != null && ArmyManager.Instance.getAllEditArmyCards().Count != 0)
//            {
//                List<Card> teamEditUids = ArmyManager.Instance.getAllEditArmyCards();
//                for (int n = 0; n < teamEditUids.Count; n++)
//                {
//                    if ((showList[i] as Card).uid == teamEditUids[n].uid)
//                    {
//                        hideList.Add(showList[i]);
//                        showList.Remove(showList[i]);
//                        i--;
//                        break;
//                    }
//                }
//            }
//        }
        //剔除队伍中的
//        for (int i = 0; i < showList.Count; i++)
//        {
//            ArrayList teamUids = ArmyManager.Instance.getTeamCardUidList();
//            for (int n = 0; n < teamUids.Count; n++)
//            {
//                if ((showList[i] as Card).uid == (teamUids[n] as string))
//                {
//                    hideList.Add(showList[i]);
//                    showList.Remove(showList[i]);
//                    i--;
//                    break;
//                }
//            }
//        }
		//优化为显示所有的卡YXZH-7732
    }

    public void updateContent()
    {
        SortCondition sc = SortConditionManagerment.Instance.getConditionsByKey(SiftWindowType.SIFT_CARDCHOOSE_WINDOW);
        ArrayList list = SortManagerment.Instance.cardSort(currentList, sc);
		list = topTeamRole(list);
        content.reLoad(list, selectType);
    }
}
