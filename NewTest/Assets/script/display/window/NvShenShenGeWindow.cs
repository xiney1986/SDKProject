using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;

public class NvShenShenGeWindow : WindowBase
{
    public GameObject[] shengePoint;
    public UILabel[] attrsLabels;//属性基础值
    public UILabel[] attrsPerLabels;//属性百分比
    public UILabel combatLabel;//战斗力
    public UILabel descLabel;//神格额外效果描述
    public UILabel shenGeEffectLabel;//神格威能描述
    public ButtonBase moreButton;//查看详情按钮
    public ButtonBase closeButton;//关闭详情按钮
    public GameObject moreDescPanel;//神格所有额外效果描述面板
    public UILabel AllExtraEffectLabel;//所有等级的额外效果描述
    public GameObject shengePrefab;
    public UITexture bg1;
    public UITexture bg2;
    public UITexture bg3;
    private ButtonShenGe[] shenGes;//神格
    private int[] attrs = new int[5];


    protected override void DoEnable()
    {
        base.DoEnable();
        UiManager.Instance.backGround.switchBackGround("ChouJiang_BeiJing");
        ResourcesManager.Instance.LoadAssetBundleTexture("texture/shenGe/bg_middle_texture", bg1);
        ResourcesManager.Instance.LoadAssetBundleTexture("texture/shenGe/quan_texture", bg2);
        ResourcesManager.Instance.LoadAssetBundleTexture("texture/shenGe/bg_Word_texture", bg3);
        moreDescPanel.SetActive(false);
    }
    public void init()
    {
        //取后台传来的神格镶嵌信息
        GetShenGeInfoFPort fport = FPortManager.Instance.getFPort("GetShenGeInfoFPort") as GetShenGeInfoFPort;
        fport.access(updateUI);
       // updateUI();
    }
    
    //断线重连
    public override void OnNetResume()
    {
        base.OnNetResume();
        GetShenGeInfoFPort fport = FPortManager.Instance.getFPort("GetShenGeInfoFPort") as GetShenGeInfoFPort;
        fport.access(updateUI);
        if(UiManager.Instance.getWindow<ShenGeGroupWindow>() != null)
            UiManager.Instance.getWindow<ShenGeGroupWindow>().finishWindow();
    }

    /// <summary>
    /// 更新界面显示
    /// </summary>
    public void updateUI()
    {
        List<ShenGeCaoInfo> infos = ShenGeManager.Instance.getAllEquipedShenGeSid();
        shenGes = new ButtonShenGe[shengePoint.Length];
		for (int i = 0; i < shengePoint.Length; i++)//清空
		{
            Utils.RemoveAllChild(shengePoint[i].transform);
		}
        for (int i = 0; i < shengePoint.Length; i++)//初始化
        {
            GameObject obj = NGUITools.AddChild(shengePoint[i], shengePrefab);
            obj.GetComponent<ButtonShenGe>().init(null, (i+1));
            obj.GetComponent<ButtonShenGe>().fatherWindow = this;
            shenGes[i] = obj.GetComponent<ButtonShenGe>();
        }
        for (int i = 0; i < attrs.Length; i++)
        {
            attrs[i] = 0;
        }
        for (int k = 0; k < infos.Count; k++)//计算各类型神格所附加的影响值总和
        {
            int index = infos[k].index;
            Prop tmpProp = PropManagerment.Instance.createProp(infos[k].sid);
            PropSample sample = PropSampleManager.Instance.getPropSampleBySid(infos[k].sid);
            if (sample != null)
            {
                shenGes[index -1].init(tmpProp, index);
                if (tmpProp != null)
                {
                    switch (tmpProp.getType())
                    {
                        case PropType.PROP_SHENGE_HP:
                            attrs[0] += tmpProp.getEffectValue();
                            break;
                        case PropType.PROP_SHENGE_DEF:
                            attrs[1] += tmpProp.getEffectValue();
                            break;
                        case PropType.PROP_SHENGE_AGI:
                            attrs[2] += tmpProp.getEffectValue();
                            break;
                        case PropType.PROP_SHENGE_ATT:
                            attrs[3] += tmpProp.getEffectValue();
                            break;
                        case PropType.PROP_SHENGE_MAG:
                            attrs[4] += tmpProp.getEffectValue();
                            break;
                    }
                }
            }
        }
        for (int i = 0; i < attrsLabels.Length; i++)//赋值神格属性值
        {
            attrsLabels[i].text = "";
            if (attrs[i] != 0)
                attrsLabels[i].text = "+"+ attrs[i];
        }

        //==========神格威能=========================
        shenGeEffectLabel.text = LanguageConfigManager.Instance.getLanguage("NvShenShenGe_019");
        ShenGePower tmpPower = ShenGeManager.Instance.CalculateShenGePower();
        string str = "";
        if (tmpPower != null) {
            for (int i = 0; i < tmpPower.AttrInfos.Count; i++) {
                if (tmpPower.AttrInfos[i].type == ShenGeManager.ADDATTACK)
                    str += LanguageConfigManager.Instance.getLanguage("NvShenShenGe_014",
                        tmpPower.AttrInfos[i].value + "% ");
                else if (tmpPower.AttrInfos[i].type == ShenGeManager.REDUECEDAMAGE)
                    str += LanguageConfigManager.Instance.getLanguage("NvShenShenGe_015",
                        tmpPower.AttrInfos[i].value + "% ");
            }
            shenGeEffectLabel.text = LanguageConfigManager.Instance.getLanguage("NvShenShenGe_013", tmpPower.level + "", str);
        }
        //==========神格额外属性=========================
        string str1 = "";
                str1 += LanguageConfigManager.Instance.getLanguage("NvShenShenGe_014",
                    "0% ");
                str1 += LanguageConfigManager.Instance.getLanguage("NvShenShenGe_015",
                    "0% ");
        descLabel.text = LanguageConfigManager.Instance.getLanguage("NvShenShenGe_033", "0", str1);
        ShenGeExtraEffect extraValue = ShenGeManager.Instance.CalculateExtrEffectValue();
        //for (int i = 0; i < attrsPerLabels.Length; i++) {
        //    attrsPerLabels[i].text = "";
        //    if (value != "")
        //        attrsPerLabels[i].text = "[FF0000]+" + value;
        //}
        string strs = "";
        if (extraValue != null)
        {
            strs = "";
            for (int i = 0; i < extraValue.AttrInfos.Count; i++) {
                if (extraValue.AttrInfos[i].type == ShenGeManager.ADDATTACK)
                    strs += LanguageConfigManager.Instance.getLanguage("NvShenShenGe_014",
                        extraValue.AttrInfos[i].value + "% ");
                else if (extraValue.AttrInfos[i].type == ShenGeManager.REDUECEDAMAGE)
                    strs += LanguageConfigManager.Instance.getLanguage("NvShenShenGe_015",
                        extraValue.AttrInfos[i].value + "% ");
            }
            descLabel.text = LanguageConfigManager.Instance.getLanguage("NvShenShenGe_033", extraValue.level.ToString(), strs);
        }
        //所有的神格额外效果
        string effectsStr = "";
        string strss = "";
        List<ShenGeExtraEffect> allExtraEffectList = CommandConfigManager.Instance.GetShenGeExtraEffectsList();
        for (int i = 0; i < allExtraEffectList.Count; i++)
        {
            strss = "";
            if (allExtraEffectList[i] != null)
            {
                for (int k = 0; k < allExtraEffectList[i].AttrInfos.Count; k++)
                {
                    if (allExtraEffectList[i].AttrInfos[k].type == ShenGeManager.ADDATTACK)
                        strss += LanguageConfigManager.Instance.getLanguage("NvShenShenGe_014",
                            allExtraEffectList[i].AttrInfos[k].value + "% ");
                    else if (allExtraEffectList[i].AttrInfos[k].type == ShenGeManager.REDUECEDAMAGE)
                        strss += LanguageConfigManager.Instance.getLanguage("NvShenShenGe_015",
                            allExtraEffectList[i].AttrInfos[k].value + "% ");
                }
            }
            effectsStr += LanguageConfigManager.Instance.getLanguage("NvShenShenGe_012", allExtraEffectList[i].level.ToString(),allExtraEffectList[i].level.ToString(), strss);
        }
        AllExtraEffectLabel.text = LanguageConfigManager.Instance.getLanguage("NvShenShenGe_034",CommandConfigManager.Instance.shenGeGongMingString) + LanguageConfigManager.Instance.getLanguage("NvShenShenGe_035") + effectsStr;
        //==========冒险队伍战斗力=========================
        combatLabel.text = ArmyManager.Instance.DeepClone(ArmyManager.Instance.getArmy(1)).getAllCombat().ToString();

        MaskWindow.UnlockUI();
        GuideManager.Instance.doFriendlyGuideEvent();
    }


    public override void buttonEventBase(GameObject gameObj)
    {
        base.buttonEventBase(gameObj);
        switch (gameObj.name)
        {
            case "closeButton":
                moreDescPanel.SetActive(false);
                MaskWindow.UnlockUI();
                break;
            case "moreButton":
                moreDescPanel.SetActive(true);
                MaskWindow.UnlockUI();
                break;
            case "close":
                finishWindow();
                break;
        }
    }
}
