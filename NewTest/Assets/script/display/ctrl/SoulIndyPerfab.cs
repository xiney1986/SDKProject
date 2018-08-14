using UnityEngine;
using System.Collections;
//装备共鸣之星魂item
public class SoulIndyPerfab : MonoBase
{
    public UITexture eqIcon;//星魂图标
    public UISprite eqbg;//星魂背景
    public UILabel eqName;//星魂名字
    public barCtrl lvBar;//星魂等级条
    //ExpBar.updateValue (UserManager.Instance.self.getLevelExp (), UserManager.Instance.self.getLevelAllExp ());
    public UILabel eqlLabel;//星魂的等级
    public ButtonBase clickButton;//点击事件
    public UISprite partt;//部位
    private Equip selectEquip;//选择的装备
    public GameObject viewPerfabe;//星魂的预制件
    public GameObject viewPoint;//预制件放的位置
    public UILabel unOpenLabel;
    public UISprite lvClose;
    public UISprite lvOpen;
    private StarSoul starSoul;
    /** 当前激活的卡片 */
    private Card currentcard;
    private int currentStarBroeIndex;
    private int flagChange;//是不是可以点按钮 只有自己的可以点
    private int showTypeNum;
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="wb"></param>
    /// <param name="ss"></param>
    public void init(WindowBase wb, StarSoul ss, Card card, int flag, int currentStarBroe)
    {
        currentcard = card;
        showTypeNum = flag;
        starSoul = ss;
        clickButton.fatherWindow = wb;
        flagChange = 12;
        this.currentStarBroeIndex = currentStarBroe;
        if (ss == null)
        {
            eqName.text = LanguageConfigManager.Instance.getLanguage("resonanceWindow02");
            //eqbg.gameObject.SetActive(false);
            lvBar.updateValue(0, 1);
            eqlLabel.text = "";
            clickButton.disableButton(true);
            partt.gameObject.SetActive(true);
            if (!StarSoulManager.Instance.checkBroeOpenLev(currentcard, currentStarBroeIndex + 1) && showTypeNum != CardBookWindow.CLICKCHATSHOW)
            {
                lvOpen.gameObject.SetActive(false);
                lvClose.gameObject.SetActive(true);
                unOpenLabel.gameObject.SetActive(true);
                unOpenLabel.text = LanguageConfigManager.Instance.getLanguage("warchoose04", StarSoulConfigManager.Instance.getGrooveOpen()[currentStarBroeIndex].ToString());
                
            }
            if (showTypeNum == CardBookWindow.CLICKCHATSHOW)
            {
                lvOpen.gameObject.SetActive(true);
                lvClose.gameObject.SetActive(false);
                unOpenLabel.gameObject.SetActive(false);
                clickButton.disableButton(true);
                this.transform.FindChild("button").gameObject.GetComponent<Collider>().enabled = false;
            }
            else clickButton.onClickEvent = noStarSoulClick;
        }
            else
            {
                eqName.text = starSoul.getName();
                //eqbg.gameObject.SetActive(true);
                partt.gameObject.SetActive(false);
                lvBar.updateValue(starSoul.getLevel(), starSoul.getMaxLevel());
                eqlLabel.text = starSoul.getLevel() + "/" + starSoul.getMaxLevel();
                clickButton.disableButton(true);
                lvOpen.gameObject.SetActive(false);
                GameObject obj;
                if (viewPoint.transform.childCount > 0)
                    obj = viewPoint.transform.GetChild(0).gameObject;
                else
                    obj = NGUITools.AddChild(viewPoint, viewPerfabe);
                GoodsView gv = obj.GetComponent<GoodsView>();

                gv.setFatherWindow(clickButton.fatherWindow);
                //gv.onClickCallback = grooveButtonClickHanderr;//点击出问题
                
                gv.init(starSoul, GoodsView.BOTTOM_TEXT_NAME_LV);
                gv.transform.FindChild("rightBottomText").gameObject.SetActive(false);
                gv.transform.localScale = new Vector3(0.8f, 0.8f, 1);
                //this.transform.FindChild("button").gameObject.GetComponent<Collider>().enabled = false;
                if (showTypeNum == CardBookWindow.CLICKCHATSHOW)
                {
                    clickButton.disableButton(true);
                    this.transform.FindChild("button").gameObject.GetComponent<Collider>().enabled = false;
                }
                else clickButton.onClickEvent = grooveButtonClickHanderr;
            
            }

        }
    
    
    /// <summary>
    /// 点击事件
    /// </summary>

    void grooveButtonClickHanderr(GameObject obj)
    {
        if (starSoul != null)
        {
            if (StarSoulManager.Instance.getStarSoulInfo() == null)
            {
                // 与服务器通讯
                (FPortManager.Instance.getFPort("StarSoulFPort") as StarSoulFPort).getStarSoulInfoAccess(doBegin);
            }
            else
            {
                doBegin();
            }
        }
        //MaskWindow.UnlockUI();
    }
    private void doBegin()
    {
        UiManager.Instance.openWindow<StarSoulStoreStrengWindow>((win) =>
        {
            win.init(starSoul);
        });
    }
    private void noStarSoulClick(GameObject obj)
    {
        //MaskWindow.UnlockUI();
        if (starSoul == null && flagChange != CardBookWindow.CLICKCHATSHOW && StarSoulManager.Instance.checkBroeOpenLev(currentcard, currentStarBroeIndex+1))
        {   //没有星魂用于装配
            UiManager.Instance.openWindow<StarSoulStoreAloneWindow>((win) =>
            {
                StarSoulManager manager = StarSoulManager.Instance;
                manager.setActiveInfo(currentcard, currentStarBroeIndex+1);
                win.init(currentcard, starSoul, ButtonStoreStarSoul.ButtonStateType.PutOn);
            });
        }
        else
        {
            UiManager.Instance.openDialogWindow<MessageLineWindow>((winn) =>
            {
                winn.Initialize(LanguageConfigManager.Instance.getLanguage("resonancewindow16", StarSoulConfigManager.Instance.getGrooveOpen()[currentStarBroeIndex].ToString()));
            });
        }
    }
}
