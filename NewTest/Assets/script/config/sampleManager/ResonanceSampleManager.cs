using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ResonanceSampleManager : SampleConfigManager
{
    private static ResonanceSampleManager instance;
    private static List<ResonanceSample> list;//所有的模板对象
    private static List<ResonanceSample> eqQHlist;//装备强化模板对象
    private static List<ResonanceSample> eqJLlist;//装备精练模板对象
    private static List<ResonanceSample> starList;//星魂强化模板对象 
    private List<Equip> showChatEquips;//别人卡的装备
    private bool showTypeNum;//星魂用，查看从哪里来
    public ResonanceSampleManager()
	{

        base.readConfig(ConfigGlobal.CONFIG_RESONANCE_EQUIP);
	}
    public static ResonanceSampleManager Instance
    {
        get
        {
            if (instance == null)
                instance = new ResonanceSampleManager();
            return instance;
        }
    }
    /// <summary>
    /// 别人卡的装备
    /// </summary>
    /// <param name="eqq"></param>
    public void showChatEq(List<Equip> eqq)
    {
        this.showChatEquips = eqq;
    }
    public void showNum(bool type)
    {
        this.showTypeNum = type;
    }

    public List<ResonanceSample> getrssList(int type, Card card ,long exp,int ii)
    {
        List<ResonanceSample> rssList=new List<ResonanceSample>();
       // ResonanceSample[] teS = new ResonanceSample[2];
        if (type == 1)
        {
            string[] eqStrings = card.getEquips();
            
            if (eqStrings == null || eqStrings.Length <= 4)//没有产生共鸣
            {
                for (int i = 0; i < eqQHlist.Count; i++) {
                    if (eqQHlist[i].needLv < 0) continue;//理论上不存在
                    if (ii == 0)
                    {
                        rssList.Add(eqQHlist[i]);
                        return rssList;
                    }
                    rssList.Add(eqQHlist[i < eqQHlist.Count - 1 ? i + 1 : eqQHlist.Count - 1]);
                    return rssList;
                }
            }
            int templv = -1;
            for (int j = 0; j < eqStrings.Length; j++) {                
                if (!showTypeNum) {
                    Equip eq = StorageManagerment.Instance.getEquip(eqStrings[j]);
                    if (templv == -1 || templv > eq.getLevel()) templv = eq.getLevel();
                } else {
                    if (templv == -1 || templv > showChatEquips[j].getLevel()) templv = showChatEquips[j].getLevel();                    
                }
            }
            for (int i = 0; i < eqQHlist.Count; i++)
            {
                if (ii == 0) //当前等级
                {
                    if (eqQHlist[i].needLv > templv)
                    {
                        rssList.Add(eqQHlist[i==0?0:i-1]);
                        return rssList;
                    }
                    if (i != eqQHlist.Count - 1 || eqQHlist[i].needLv > templv) continue;
                    rssList.Add(eqQHlist[eqQHlist.Count-1]);
                    return rssList;
                }
                if (eqQHlist[i].needLv > templv) {
                    rssList.Add(eqQHlist[i]);
                    return rssList;
                }
                if (i != eqQHlist.Count - 1 || eqQHlist[i].needLv > templv) continue;
                rssList.Add(eqQHlist[eqQHlist.Count - 1]);
                return rssList;
            }
        }
        if (type == 2)
        {
            string[] eqStrings = card.getEquips();
            if (eqStrings == null || eqStrings.Length <= 4)
            {
                for (int i = 0; i < eqJLlist.Count; i++)
                {
                    if (eqJLlist[i].needLv < 0) continue;
                    if (ii == 0)
                    {
                        rssList.Add(eqJLlist[i]);
                        return rssList;
                    }
                    rssList.Add(eqJLlist[i < eqJLlist.Count - 1 ? i + 1 : eqJLlist.Count - 1]);
                    return rssList;
                }
            }
            int templv = -1;
            for (int j = 0; j < eqStrings.Length; j++)
            {
                if (!showTypeNum)
                {
                    Equip eq = StorageManagerment.Instance.getEquip(eqStrings[j]);
                    if (templv == -1 || templv > eq.getrefineLevel()) templv = eq.getrefineLevel();
                }
                else
                {
                    if (templv == -1 || templv > showChatEquips[j].getrefineLevel()) templv = showChatEquips[j].getrefineLevel();
                }
            }
            for (int i = 0; i < eqJLlist.Count; i++)
            {
                if (ii == 0) //当前等级
                {
                    if (eqJLlist[i].needLv > templv) {
                        rssList.Add(eqJLlist[i == 0 ? 0 : i - 1]);
                        showTypeNum = false;
                        return rssList;
                    }
                    if (i != eqJLlist.Count - 1 || eqJLlist[i].needLv > templv) continue;
                    rssList.Add(eqJLlist[eqJLlist.Count - 1]);
                    return rssList;
                }
                if (eqJLlist[i].needLv > templv) {
                    rssList.Add(eqJLlist[i]);
                    showTypeNum = false;
                    return rssList;
                }
                if (i != eqJLlist.Count - 1 || eqJLlist[i].needLv > templv) continue;
                rssList.Add(eqJLlist[eqJLlist.Count - 1]);
                showTypeNum = false;
                return rssList;
            }
        }
        ArrayList bores = card.getStarSoulBores();
        if (bores == null || bores.Count <= 0)
        {
            for (int i = 0; i < starList.Count; i++)
            {
                if (starList[i].needLv >= 0)
                {
                    if (ii == 0)
                    {
                        rssList.Add(starList[i]);
                        return rssList;
                    }
                    rssList.Add(starList[i < starList.Count - 1 ? i + 1 : starList.Count - 1]);
                    return rssList;
                }
            }
        }
        int kk = 0;
        if (!showTypeNum)
        {
            for (int i = 0; i < 6; i++)//检查几个没有开
            {
                if (!StarSoulManager.Instance.checkBroeOpenLev(card, i + 1)) kk++;
            }
            if (bores.Count + kk < 6)
            {
                if (ii == 0)
                {
                    rssList.Add(starList[0]);
                    return rssList;
                }
                rssList.Add(starList[1]);
                return rssList;
            }
        }
        else
        {
            for (int i = 0; i < 6; i++)
            {
                if (EXPSampleManager.Instance.getLevel(EXPSampleManager.SID_USER_EXP, exp, 0) >= StarSoulConfigManager.Instance.getGrooveOpen()[i]) kk++;
            }
            if (bores.Count + kk < 6)
            {
                if (ii == 0) {
                    rssList.Add(starList[0]);
                    return rssList;
                }
                rssList.Add(starList[1]);
                return rssList;
            }
        }
        int temppp = -1;
        for (int t = 0; t < bores.Count; t++)
        {
            if (showTypeNum)
            {
//                List<Card> teamData = new List<Card>();
//                teamData = StarSoulManager.Instance.getTeamCardData(1, card);
//                int lv = teamData[t].getLevel();
				StarSoulBore starSoulBore = bores[t] as StarSoulBore;
				StarSoul soul = new StarSoul("0", starSoulBore.getSid(), starSoulBore.getExp(), 0);
				int lv = soul.getLevel();
                if (temppp == -1 || temppp > lv) temppp = lv;
            }
            else
            {
                StarSoulBore starSoulBore = bores[t] as StarSoulBore;
                int lv = StorageManagerment.Instance.getStarSoul(starSoulBore.getUid()).getLevel();
                if (temppp == -1 || temppp > lv) temppp = lv;
            }
        }
        for (int i = 0; i < starList.Count  ; i++) {
            if (ii == 0) //当前等级
                {
                    if (starList[i].needLv > temppp) {
                        rssList.Add(starList[i == 0 ? 0 : i - 1]);
                    showTypeNum = false;
                    return rssList;
                }
                    if (i != starList.Count - 1 || starList[i].needLv > temppp) continue;
                    rssList.Add(starList[starList.Count - 1]);
                return rssList;
            }
            if (starList[i].needLv > temppp) {
                rssList.Add(starList[i]);
                showTypeNum = false;
                return rssList;
            }
            if (i != starList.Count - 1 || starList[i].needLv > temppp) continue;
            rssList.Add(starList[starList.Count - 1]);
            showTypeNum = false;
            return rssList;
        }
        return null;

    }
    public string[] caseName(string[] name,string _name)
    {
        switch (_name)
        {
            case AttrChangeType.PER_HP:
                name[0] = LanguageConfigManager.Instance.getLanguage("refine_008");
                break;
            case AttrChangeType.PER_ATTACK:
                name[1] = LanguageConfigManager.Instance.getLanguage("refine_019");
                break;
            case AttrChangeType.PER_DEFENSE:
                name[2] = LanguageConfigManager.Instance.getLanguage("refine_018");
                break;
            case AttrChangeType.PER_MAGIC:
                name[3] = LanguageConfigManager.Instance.getLanguage("refine_010");
                break;
            case AttrChangeType.PER_AGILE:
                name[4] = LanguageConfigManager.Instance.getLanguage("refine_016");
                break;
            case AttrChangeType.HP:
                name[0] = LanguageConfigManager.Instance.getLanguage("refine_008");
                break;
            case AttrChangeType.ATTACK:
                name[1] = LanguageConfigManager.Instance.getLanguage("refine_019");
                break;
            case AttrChangeType.DEFENSE:
                name[2] = LanguageConfigManager.Instance.getLanguage("refine_018");
                break;
            case AttrChangeType.MAGIC:
                name[3] = LanguageConfigManager.Instance.getLanguage("refine_010");
                break;
            case AttrChangeType.AGILE:
                name[4] = LanguageConfigManager.Instance.getLanguage("refine_016");
                break;
        }
        return name;
    }
    public int[] caseType(int[] a, string type, int num)
    {

        switch (type)
        {
            case AttrChangeType.PER_HP:
                a[0] += num;
                break;
            case AttrChangeType.PER_ATTACK:
                a[1] += num;
                break;
            case AttrChangeType.PER_DEFENSE:
                a[2] += num;
                break;
            case AttrChangeType.PER_MAGIC:
                a[3] += num;
                break;
            case AttrChangeType.PER_AGILE:
                a[4] += num;
                break;
            case AttrChangeType.HP:
                a[0] += num;
                break;
            case AttrChangeType.ATTACK:
                a[1] += num;
                break;
            case AttrChangeType.DEFENSE:
                a[2] += num;
                break;
            case AttrChangeType.MAGIC:
                a[3] += num;
                break;
            case AttrChangeType.AGILE:
                a[4] += num;
                break;
        }
        return a;
    }

    public override void parseConfig(string str)
    {
        ResonanceSample be = new ResonanceSample(str);
        if (be.resonanceType == 1)
        {
            if(eqQHlist==null)eqQHlist=new List<ResonanceSample>();
            eqQHlist.Add(be);
        }else if (be.resonanceType==2)
        {
            if(eqJLlist==null)eqJLlist=new List<ResonanceSample>();
            eqJLlist.Add(be);
        }
        else
        {
          if(starList==null)starList=new List<ResonanceSample>(); 
            starList.Add(be);
        }
    }
}
