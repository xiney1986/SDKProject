using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 卡片超进化
/// </summary>
public class IntensifyCardSuperEvoContent : MonoBase
{
    /** 主卡 */
    public SacrificeShowerCtrl mainRole;
    /** 超进化有多张食物 */
    public SacrificeShowerCtrl[] foods;
    /** 选择主卡按钮 */
    public ButtonBase buttonMain;
    /** 选择副卡按钮 */
    public ButtonBase[] buttonFoods;
    /** 战斗力 */
    public UILabel combatLabel;
    /** 战斗力增加值 */
    public UILabel combatAddLabel;
    /** 天赋觉醒说明 */
    public UILabel talentLabel;
    private IntensifyCardWindow win;
    private Card mainCard;
    /** 进化到下一等级需要的副卡数量 */
    private int needFoodNum;
    /** 最后点击的副卡的下标 */
    private int lastClickIndex = 0;
    /** 临时存放的进化前卡片 */
    private Card tempCard;
    private Card oldCard;
    private Card newCard;
    private Prop foodProp;
    /** 警告提示 */
    private string errorString;

    public string getErrorMsg() {
        return errorString;
    }
    public void updateCtrl(IntensifyCardWindow _win, bool isChooseFood)
    {
        this.win = _win;
        clearLabelMsg();
        //主卡不为空
        if (!IntensifyCardManager.Instance.isHaveMainCard())
        {
            mainCard = IntensifyCardManager.Instance.getMainCard();
            if (!mainCard.isInSuperEvo())
            {
                clearAllData();
                /** 最大进化等级 */
                if (mainCard.isMaxEvoLevel()) {
                    UiManager.Instance.createMessageLintWindow(Language("Intensify30"));
                }
                /** 没达到超进化等级 */
                else
                {
                    UiManager.Instance.createMessageLintWindow(Language("Intensify29"));
                }
                win.costSum.text = "0";
                return;
            }
            buttonMain.gameObject.SetActive(false);
            mainRole.updateShower(IntensifyCardManager.Instance.getMainCard());
 
            /** 根据主卡进化到下一等级需要进化的次数，开放对应的副卡选择按钮 */
            needFoodNum = mainCard.getEvotimesToNextLevel();
            updateFoods(needFoodNum);
            //主卡不为空就自动上副卡(优先卡片再万能卡)
            if (isChooseFood)
            {
                /** 有选择副卡 */
                if (IntensifyCardManager.Instance.getHaveGetSuperExoFood())
                {
                    Card tmpFoodCard = IntensifyCardManager.Instance.getNewFoodCard();
                    //先检测有没有副卡，有副卡的时候匹配主卡，符合就放上去
                    if (tmpFoodCard != null && tmpFoodCard.getEvolveNextSid() == mainCard.getEvolveNextSid())
                    {
                        foods[lastClickIndex].updateShower(tmpFoodCard);
                        buttonFoods[lastClickIndex].gameObject.SetActive(false);
                    }

                }
                    /** 如果一张卡或道具都没选,自动帮选,优先同名卡,其次万能卡 */
                else if (IntensifyCardManager.Instance.getFoodPropNum() == 0 && IntensifyCardManager.Instance.getFoodCard().Count == 0)
                {
                    List<Card> foodEvoCards = new List<Card>();
                    foodEvoCards.AddRange(EvolutionManagerment.Instance.getFoodCardsForSuperEvo(IntensifyCardManager.Instance.getMainCard()));
                    Prop foodEvoProp = EvolutionManagerment.Instance.getCardByQuilty(IntensifyCardManager.Instance.getMainCard());
                    int usePropNum = 0;//自动使用的万能卡数量
                    for (int i = 0; i < needFoodNum; i++)
                    {
                        if (i < foodEvoCards.Count)
                        {
                            IntensifyCardManager.Instance.setFoodCard(foodEvoCards[i]);
                            foods[i].updateShower(foodEvoCards[i]);
                            buttonFoods[i].gameObject.SetActive(false);
                        } else if (foodEvoProp != null && usePropNum < foodEvoProp.getNum())
                        {
                            IntensifyCardManager.Instance.setFoodProp(foodEvoProp);
                            foods[i].updateShowerByProp(foodEvoProp);
                            buttonFoods[i].gameObject.SetActive(false);
                            usePropNum++;
                        }
                    }
                }                
            }
            IntensifyCardManager.Instance.resetSuperExoChoosedNum();
            updateDesLabel(mainCard);

        } else
        {
            buttonMain.gameObject.SetActive(true);
            clearAllData();
        }
    }
    /// <summary>
    /// 更新战斗力，天赋等描述
    /// </summary>
    private void updateDesLabel(Card mainCard)
    {
        /** 战斗力 */
        combatLabel.text = mainCard.getCardCombat().ToString();
        Card newCard = mainCard.Clone() as Card;
        newCard.uid = "-1";
        /** 更新进化等级 */
        newCard.updateEvoLevel(newCard.getEvoTimes() + newCard.getEvotimesToNextLevel());  
        /** 计算副卡附加经验 */
        int[] inheritAddonExp = new int[5];
        long inheritExp = 0;
        long inheritSkillExp = 0;
        foreach (Card each in IntensifyCardManager.Instance.getFoodCard()) {
            inheritExp += each.getEXP();
            inheritSkillExp += each.getSkillsExp();
            inheritAddonExp[0] += each.getHPExp();
            inheritAddonExp[1] += each.getATTExp();
            inheritAddonExp[2] += each.getDEFExp();
            inheritAddonExp[3] += each.getMAGICExp();
            inheritAddonExp[4] += each.getAGILEExp();
        }

        newCard.updateHPExp(newCard.getHPExp() + inheritAddonExp[0]);
        newCard.updateATTExp(newCard.getATTExp() + inheritAddonExp[1]);
        newCard.updateDEFExp(newCard.getDEFExp() + inheritAddonExp[2]);
        newCard.updateMAGICExp(newCard.getMAGICExp() + inheritAddonExp[3]);
        newCard.updateAGILEExp(newCard.getAGILEExp() + inheritAddonExp[4]);
        /** 检查继承的经验是否受到主角等级限制 */
        long tmpExp = newCard.checkExp(inheritExp);
        if (tmpExp != -1)
        {
            if (errorString != "")
            {
                errorString += "\n" + LanguageConfigManager.Instance.getLanguage("inherit_err0");
            } else
            {
                errorString += LanguageConfigManager.Instance.getLanguage("inherit_err0");
            }
            newCard.updateExp(tmpExp);
        } else
            newCard.updateExp(newCard.getEXP() + inheritExp);

        /** 继承级技能经验 */
        if (newCard.isSkillExpUpFull((int)inheritSkillExp))
        {
            if (errorString != "")
            {
                errorString += "\n" + LanguageConfigManager.Instance.getLanguage("inherit_err1");
            } else
            {
                errorString += LanguageConfigManager.Instance.getLanguage("inherit_err1");
            }
        }
        if (newCard.getSkills() != null)
        {
            newCard.getSkills()[0].updateExp(newCard.getSkills()[0].getEXP() + inheritSkillExp);
		}
        if (newCard.getBuffSkills() != null)
        {
            newCard.getBuffSkills()[0].updateExp(newCard.getBuffSkills()[0].getEXP() + inheritSkillExp);
		}
        if (newCard.getAttrSkills() != null)
        {
            newCard.getAttrSkills()[0].updateExp(newCard.getAttrSkills()[0].getEXP() + inheritSkillExp);
		}
        combatAddLabel.text = "+" + (newCard.getCardCombat() - mainCard.getCardCombat()).ToString();

        /** 天赋 */
        if (EvolutionManagerment.Instance.isOpenTalentByThisEvoLv(newCard, mainCard.getEvoLevel()))
        {
            string talentStr = EvolutionManagerment.Instance.getOpenTalentString(newCard, mainCard.getEvoLevel());
            talentLabel.text = LanguageConfigManager.Instance.getLanguage("Evo17", ":\n", talentStr);
            talentLabel.gameObject.SetActive(true);
        }
    }

    /** 选择万能卡 */
    public void chooseFoodProp(Prop prop)
    {
        IntensifyCardManager.Instance.setFoodProp(prop);
        foods[lastClickIndex].updateShowerByProp(prop);
        buttonFoods[lastClickIndex].gameObject.SetActive(false);
    }

    /** 根据进化到下一等级需要的进化次数开启副卡选择按钮 */
    private void updateFoods(int num)
    {
        for (int i = 0; i < buttonFoods.Length; i++)
        {
            /** 选择槽开启 */
            if (i < num)
            {
                Card card = foods[i].getCard();
                Prop prop = foods[i].getProp();
                if (!IntensifyCardManager.Instance.isInFood(card))
                    card = null;
                if (!IntensifyCardManager.Instance.isInFood(prop))
                    prop = null;

                /** 如果什么都没选 */
                if (card == null && prop == null)
                {
                    buttonFoods[i].gameObject.SetActive(true);
                    buttonFoods[i].onClickEvent = buttonFoodClick;
                    foods[i].gameObject.SetActive(false);
                    foods[i].cleanAll();

                }
                    /** 如果选择了卡或者万能卡 */
                else
                {                    
                    foods[i].gameObject.SetActive(true);
                    buttonFoods[i].gameObject.SetActive(false);
                }
            }
                /** 选择槽未开启 */
            else
            {
                buttonFoods[i].gameObject.SetActive(false);
                foods[i].cleanAll();
                foods[i].gameObject.SetActive(false);
            }
        }
    }

    /** 记录上次点击的副卡位置 */
    private void buttonFoodClick(GameObject go)
    {
        /** 点击选卡之后,重置下 */
        IntensifyCardManager.Instance.resetSuperExoChoosedNum();
        for (int i = 0; i < buttonFoods.Length; i++)
        {
            if (buttonFoods[i].gameObject == go)
                lastClickIndex = i;
        }
    }


    /** 清空界面,管理器,主,副卡数据 */
    private void clearAllData()
    {
        lastClickIndex = 0;
        buttonMain.gameObject.SetActive(true);
        mainRole.gameObject.SetActive(false);
        foreach (SacrificeShowerCtrl food in foods)
        {
            food.cleanAll();
            food.gameObject.SetActive(false);
        }

        foreach (ButtonBase bt in buttonFoods)
        {
            bt.gameObject.SetActive(false);
        }
        IntensifyCardManager.Instance.clearData();
    }

    /// <summary>
    /// 清除界面上的文字描述
    /// </summary>
    private void clearLabelMsg()
    {
        talentLabel.text = "";
        talentLabel.gameObject.SetActive(false);
        combatLabel.text = "";
        combatAddLabel.text = "";
        errorString = "";
    }

    /// <summary>
    /// 强化入口
    /// </summary>
    /// <param name="intoType">窗口类型.</param>
    /// <param name="cost">消耗.</param>
    /// <param name="evoType">进化类型1普通卡,2万能卡.</param>
    public void intensify()
    {

        sendEvoData();
    }

    /// <summary>
    /// 进化端口
    /// </summary>
    private void sendEvoData()
    {
        tempCard = mainRole.getCard().Clone() as Card;
        EvolutionFPort fport = FPortManager.Instance.getFPort("EvolutionFPort") as EvolutionFPort;
        fport.evolutionCard(mainRole.getCard(), IntensifyCardManager.Instance.getFoodCard(), playEffect);
    }

    /// <summary>
    /// 进化结束，开始播动画
    /// </summary>
    private void playEffect()
    {
        newCard = StorageManagerment.Instance.getRole(mainRole.getCardUid());
        UiManager.Instance.openWindow<EmptyWindow>((win) =>
        {
            UiManager.Instance.switchWindow<EffectBlackWindow>((win222) =>
            {
                if (IntensifyCardManager.Instance.getFoodCard().Count != 0)
                {
                    oldCard = tempCard;
                    win222.playEvoEffect(oldCard, IntensifyCardManager.Instance.getFoodCard()[0], showEvolutionWindow);
                } else
                {
                    oldCard = tempCard;
                    foodProp = EvolutionManagerment.Instance.getCardByQuilty(mainRole.getCard()) != null
                        ? PropManagerment.Instance.createProp(
                            EvolutionManagerment.Instance.getCardByQuilty(mainRole.getCard()).sid)
                        : PropManagerment.Instance.createProp(
                            EvolutionManagerment.Instance.getCardByQualityNotNull(mainRole.getCard()).sid);
                    win222.playEvoEffect(oldCard, foodProp.getIconId(), showEvolutionWindow);
                }
            });
        });
    }

    /// <summary>
    /// 动画结束，展示窗口
    /// </summary>
    private void showEvolutionWindow()
    {
        IntensifyCardManager.Instance.clearData();
        if (EvolutionManagerment.Instance.isCanEvo(newCard) == false)
        {
            IntensifyCardManager.Instance.updateEvolution(IntensifyCardManager.INTENSIFY_CARD_SUPRE_EVO, win.getCallBack());
        } else
        {
            IntensifyCardManager.Instance.updateEvolution(IntensifyCardManager.INTENSIFY_CARD_SUPRE_EVO, newCard, win.getCallBack());
        }
    }
}
