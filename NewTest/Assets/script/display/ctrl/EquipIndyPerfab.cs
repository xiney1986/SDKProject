using UnityEngine;
using System.Collections;

//装备共鸣之装备强化精练item
public class EquipIndyPerfab : MonoBase
{
    public UITexture eqIcon;//装备图标
    public UISprite eqbg;//装备背景
    public UILabel eqName;//装备名字
    public barCtrl lvBar;//装备等级条
    //ExpBar.updateValue (UserManager.Instance.self.getLevelExp (), UserManager.Instance.self.getLevelAllExp ());
    public UILabel eqlLabel;//装备的等级
    public ButtonBase clickButton;//点击事件
    public UISprite partt;//部位
    private Equip selectEquip;//选择的装备
    private int showTypeNum;//你从哪里来
    /// <summary>
    /// 初始化
    /// </summary>
    public void init(WindowBase wb,Equip eq,int part,int showType)
    {
        selectEquip = eq;
        clickButton.fatherWindow = wb;
        showTypeNum = showType;
        if (eq == null)
        {

            eqName.text = LanguageConfigManager.Instance.getLanguage("resonanceWindow02");
            eqIcon.gameObject.SetActive(false);
            eqbg.gameObject.SetActive(false);
            lvBar.updateValue(0, 1);
            eqlLabel.text =  "";
            clickButton.disableButton(false);//点击事件和装备强化界面绑定
            clickButton.onClickEvent = intoNoShowInf;
            partt.gameObject.SetActive(true);
            if (part == EquipPartType.WEAPON) partt.spriteName = "icon_back_weapon";
            else if (part == EquipPartType.ARMOUR) partt.spriteName = "icon_back_armor";
            else if (part == EquipPartType.SHOSE) partt.spriteName = "icon_back_shose";
            else if (part == EquipPartType.HELMET) partt.spriteName = "icon_back_hat";
            else if (part == EquipPartType.RING) partt.spriteName = "icon_back_ring";
            if(showTypeNum==CardBookWindow.CLICKCHATSHOW)
            {
                clickButton.disableButton(true);
                this.transform.FindChild("button").gameObject.GetComponent<Collider>().enabled = false;
            }
        }
        else
        {
            partt.gameObject.SetActive(false);
            eqIcon.gameObject.SetActive(true);
            eqbg.gameObject.SetActive(true);
            eqName.text = QualityManagerment.getQualityColor(selectEquip.getQualityId()) + selectEquip.getName();
            ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.ICONIMAGEPATH + selectEquip.getIconId(), eqIcon);
            eqbg.spriteName = QualityManagerment.qualityIDToIconSpriteName(selectEquip.getQualityId());
            lvBar.updateValue(selectEquip.getLevel(), selectEquip.getMaxLevel());
            eqlLabel.text = selectEquip.getLevel() + "/" + selectEquip.getMaxLevel();
            clickButton.disableButton(false);
            if (showTypeNum == CardBookWindow.CLICKCHATSHOW)
            {
                clickButton.disableButton(true);
                this.transform.FindChild("button").gameObject.GetComponent<Collider>().enabled = false;
            }
            else
            clickButton.onClickEvent = intoShowInf;//点击事件和装备强化界面绑定
        }
    }

    public void initRefine(WindowBase wb, Equip eq, int part,int showType)
    {
        selectEquip = eq;
        clickButton.fatherWindow = wb;
        showTypeNum = showType;
        if (eq == null)
        {

            eqName.text = LanguageConfigManager.Instance.getLanguage("resonanceWindow02");
            eqIcon.gameObject.SetActive(false);
            eqbg.gameObject.SetActive(false);
            lvBar.updateValue(0, 1);
            eqlLabel.text = "";
            clickButton.disableButton(false);//点击事件和装备强化界面绑定
            
            partt.gameObject.SetActive(true);
            if (part == EquipPartType.WEAPON) partt.spriteName = "icon_back_weapon";
            else if (part == EquipPartType.ARMOUR) partt.spriteName = "icon_back_armor";
            else if (part == EquipPartType.SHOSE) partt.spriteName = "icon_back_shose";
            else if (part == EquipPartType.HELMET) partt.spriteName = "icon_back_hat";
            else if (part == EquipPartType.RING) partt.spriteName = "icon_back_ring";
            if (showTypeNum == CardBookWindow.CLICKCHATSHOW)
            {
                clickButton.disableButton(true);
                this.transform.FindChild("button").gameObject.GetComponent<Collider>().enabled = false;
            }
            else clickButton.onClickEvent = intoNoShowInf;
        }
        else
        {
            partt.gameObject.SetActive(false);
            eqIcon.gameObject.SetActive(true);
            eqbg.gameObject.SetActive(true);
            eqName.text = QualityManagerment.getQualityColor(selectEquip.getQualityId()) + selectEquip.getName();
            ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.ICONIMAGEPATH + selectEquip.getIconId(), eqIcon);
            eqbg.spriteName = QualityManagerment.qualityIDToIconSpriteName(selectEquip.getQualityId());
            lvBar.updateValue(selectEquip.getrefineLevel(), selectEquip.getRefineMaxLevel());            
            eqlLabel.text = selectEquip.getrefineLevel() + "/" + selectEquip.getRefineMaxLevel();
            clickButton.disableButton(false);
            if (showTypeNum == CardBookWindow.CLICKCHATSHOW)
            {
                clickButton.disableButton(true);
                this.transform.FindChild("button").gameObject.GetComponent<Collider>().enabled = false;
            }
            else
            clickButton.onClickEvent = intoShowRefineInfo;//点击事件和装备强化界面绑定
        }



    }
    /// <summary>
    /// 点击装备强化
    /// </summary>
    /// <param name="obj"></param>
    void intoShowInf(GameObject obj)
    {
        //MaskWindow.UnlockUI();
        if (selectEquip!=null)
        {
            UiManager.Instance.openWindow<IntensifyEquipWindow>((win) =>
            {
                win.Initialize(selectEquip, IntensifyEquipWindow.EQUIPSTORE);
            });
        }
    }
    void intoNoShowInf(GameObject obj)
    {
        UiManager.Instance.openDialogWindow<MessageLineWindow>((winn) =>
        {
            winn.Initialize(LanguageConfigManager.Instance.getLanguage("resonanceWindow14"));
        });
    }
    /// <summary>
    /// 点击装备精炼
    /// </summary>
    /// <param name="obj"></param>
    void intoShowRefineInfo(GameObject obj)
    {
        if (RefineSampleManager.Instance.getRefineSampleBySid(selectEquip.sid) != null)
        {
            UiManager.Instance.openWindow<RefineWindow>((win) =>
            {
                win.initialize(selectEquip);
            });
        }else
        {
            UiManager.Instance.openDialogWindow<MessageLineWindow>((winn) =>
            {
                winn.Initialize(LanguageConfigManager.Instance.getLanguage("resonanceWindow15"));
            });
        }
    }


}
