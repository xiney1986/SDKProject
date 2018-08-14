using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
 
//卡片属性中的装备按钮
public class ButtonEquipInCardBook : ButtonBase
{
    public UITexture iconImage;
    public UISprite backGround;
    public UILabel equipLv;
    public Equip equip;
    //* 升星状态显示*/
    public UILabel starLevelState;
    public UILabel renLevel;
    public string orgSpriteName;
    private bool starflag;
    private bool reflag;
    private bool canStar;
    private int intoType = 0;
    public UISprite addTip;
    [HideInInspector] public int partID;

    public Timer timer;

    public override void begin()
    {
        base.begin();

    }

    public void initInto(int type)
    {
        this.intoType = type;
    }

    private void refreshData()
    {
        if (this == null || !gameObject.activeInHierarchy)
        {
            if (timer != null)
            {

                timer.stop();
                timer = null;
            }
            return;
        }
        showStarEffect();

    }

    private void showStarEffect()
    {
        
        if(canStar)return;
        if (starflag&&!reflag)
        {
            canStar = true;
            starLevelState.alpha = 1;
            starLevelState.gameObject.SetActive(true);
        } else if (!starflag && reflag)
        {
            canStar = true;
            renLevel.alpha = 1;
            renLevel.gameObject.SetActive(true);
        } else if (starflag && reflag)
        {
            canStar = true;
            starLevelState.alpha = 0;
            if (!starLevelState.gameObject.activeInHierarchy) starLevelState.gameObject.SetActive(true);
            iTween.ValueTo(gameObject, iTween.Hash("from", 0, "to", 1, "onupdate", "changeAlpha", "oncomplete", "tpOneComplete", "easetype", iTween.EaseType.linear, "time", 1f, "name", gameObject.name + "0"));

        }
        else
        {
            starLevelState.gameObject.SetActive(false);
            starLevelState.gameObject.SetActive(false);
        }
    }

    void changeAlpha(float a)
    {
        starLevelState.alpha = a;
    }
    void changeAlphaa(float a) {
        renLevel.alpha = a;
    }
    void tpOneComplete()
    {
        starLevelState.alpha = 1f;
        iTween.ValueTo(gameObject, iTween.Hash("from", 1, "to", 0, "onupdate", "changeAlpha", "oncomplete", "tpTwoComplete", "delay",4f, "easetype", iTween.EaseType.linear, "time", 1f, "name", gameObject.name+"1"));
    }
    void tpTwoComplete()
    {
        renLevel.gameObject.SetActive(true);
        renLevel.alpha = 0f;
        iTween.ValueTo(gameObject, iTween.Hash("from", 0, "to", 1, "onupdate", "changeAlphaa", "oncomplete", "tpThreeComplete", "delay", 1.5f, "easetype", iTween.EaseType.linear, "time", 1f, "name", gameObject.name + "2"));
    }
    void tpThreeComplete()
    {
        renLevel.alpha = 1f;
        iTween.ValueTo(gameObject, iTween.Hash("from", 1, "to", 0, "onupdate", "changeAlphaa", "oncomplete", "tpForeComplete", "delay", 4f, "easetype", iTween.EaseType.linear, "time", 1f, "name", gameObject.name + "3"));
    }
    void tpForeComplete() {
        canStar = false;
    }
    public void updateEquip(Equip item)
    {
        if (gameObject.GetComponent<iTween>()) DestroyImmediate(gameObject.GetComponent<iTween>());
        iTween.Stop(gameObject, true);
        canStar = false;
        equip = item;
        if (equip != null)
        {
            if (equip.equpStarState > 0)
            {
                starLevelState.text = equip.equpStarState + LanguageConfigManager.Instance.getLanguage("star_star_star");
                starflag = true;
            }
            else
            {
                starflag = false;
                starLevelState.text = "";
                starLevelState.gameObject.SetActive(false);
            }
            if (equip.getrefineLevel() > 0)
            {
                reflag = true;
                renLevel.text = equip.getrefineLevel() + LanguageConfigManager.Instance.getLanguage("refine_024");
            }
            else
            {
                reflag = false;
                renLevel.gameObject.SetActive(false);
                renLevel.text = "";
            }
            iconImage.gameObject.SetActive(true);
            ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.ICONIMAGEPATH + equip.getIconId(),
                iconImage);
            backGround.spriteName = QualityManagerment.qualityIDToIconSpriteName(equip.getQualityId());
            equipLv.text = "Lv"+equip.getLevel();
        }
        else
        {
            reflag = false;
            starflag = false;
            renLevel.gameObject.SetActive(false);
            starLevelState.gameObject.SetActive(false);
            backGround.spriteName = orgSpriteName;
            iconImage.gameObject.SetActive(false);
            starLevelState.text = "";
            equipLv.text = "";
            renLevel.text = "";
        }
        canStar = false;
        if (equip != null && timer == null)
        {
        timer = TimerManager.Instance.getTimer(1000);
        timer.addOnTimer(refreshData);
        timer.start();
        }
        
        updateStatus();
       // showStarEffect();
    }

    public void updateAddEquipTipVisible(bool value)
    {
        addTip.gameObject.SetActive(value);
    }

    public void updateStatus()
    {
        
        //已穿装，不考虑提示
        if (equip != null)
        {
            addTip.gameObject.SetActive(false);
        }
        else
        {
            if (GuideManager.Instance.isLessThanStep(123002000))
            {
                addTip.gameObject.SetActive(false); //show tip must :level>7
                return;
            }
            if (intoType == EquipAttrWindow.OTHER || intoType == 0)
            {
                addTip.gameObject.SetActive(false);
                return;
            }
            int showType = (fatherWindow as CardBookWindow).getShowType();
            if (showType == CardBookWindow.OTHER)
            {
                addTip.gameObject.SetActive(false);
                return;
            }


            string n = gameObject.name;
            partID = StringKit.toInt(n.Substring(n.Length - 1, 1));
            Card curCard = (fatherWindow as CardBookWindow).getShowCard();
            Equip canUseEquip = IncreaseManagerment.Instance.checkHasFreeEquipCanUseByCar(curCard, partID);
            
            if (canUseEquip != null)
            {
                addTip.gameObject.SetActive(true);
//				Debug.Log("has can use equip: id="+canUseEquip.sid+" name="+canUseEquip.getName());
            }
            else
            {
                addTip.gameObject.SetActive(false);
            }
        }
    }

    private bool isMyEquip = true;

    public override void DoClickEvent()
    {
        base.DoClickEvent();
        starLevelState.gameObject.SetActive(false);
        if (GuideManager.Instance.isLessThanStep(123002000))
        {
            MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("GuideError_00"));
            return;
        }
        //点击强化装备
        if (GuideManager.Instance.isEqualStep(124004000))
        {
            GuideManager.Instance.doGuide();
        }
        int showType = (fatherWindow as CardBookWindow).getShowType();
        if (showType == CardBookWindow.SHOW || showType == CardBookWindow.OTHER)
        {
            MaskWindow.UnlockUI();
            return;
        }



        int partId = StringKit.toInt(this.name.Substring(5));
        if (equip == null)
        {
            //如果来自聊天窗口 则不显示
            if (showType == CardBookWindow.CHATSHOW || showType == CardBookWindow.CLICKCHATSHOW)
            {
                MaskWindow.UnlockUI();
                return;
            }
            EquipManagerment.Instance.setEquipChange((fatherWindow as CardBookWindow).getShowCard(), null, partId);
            UiManager.Instance.openWindow<EquipChooseWindow>((window) =>
            {
                window.Initialize(
                    EquipChooseWindow.FROM_CARDATTR);
            });
        }
        else
        {

            isMyEquip = StorageManagerment.Instance.getEquip(equip.uid) == null ? false : true;
            EquipManagerment.Instance.setEquipChange((fatherWindow as CardBookWindow).getShowCard(), equip, partId);
            UiManager.Instance.openWindow<EquipAttrWindow>((window) =>
            {
                window.Initialize((fatherWindow as CardBookWindow).getShowCard(), equip,
                    (showType == CardBookWindow.CHATSHOW || showType == CardBookWindow.CLICKCHATSHOW)
                        ? EquipAttrWindow.OTHER
                        : EquipAttrWindow.CARDVIEW, showCardAttrWindow);
                window.setOperation(isMyEquip);
            });
        }
//		cardView.window.finishWindow ();
    }

    private void showCardAttrWindow()
    {
        (fatherWindow as CardBookWindow).showItemUpdateAll();
    }

    private int toInt(string str)
    {
        return int.Parse(str.Substring(str.Length - 1, 1));
    }
}