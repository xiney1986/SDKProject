using System;
using UnityEngine;
using System.Collections;

public class BloodItemSample
{
	/** sid */
	public int sid;
	/** 节点的类型 大中小 */
	public int itemType;
	/** 获得该节点的条件 */
	public PrizeSample[] condition;
    /** 获得改节点的附加条件*/
    public int evoLvCondition;
	/** 获得该节点可以产生的效果 */
    public bloodEffect[] effects;//激活效果
    public const int SMALL_ITEM = 1;
    public const int MADDLE_ITEM = 2;
    public const int BIG_ITEM = 3;

    public BloodItemSample(string str)
	{
		parse (str);
	}

	private void parse (string str)
	{
		string[] strArr = str.Split ('|');
		//strArr[0] 
		sid = StringKit.toInt (strArr [0]);
        itemType = StringKit.toInt(strArr[1]);
		//strArr[3] prizes
	    parseCondtions(strArr[2]);
        parseEffect(strArr[3]);
	    if (strArr.Length == 5)
	        evoLvCondition = StringKit.toInt(strArr[4]);
	}
    private void parseEffect(string str) {
        string[] strEffects = str.Split('#');
        effects=new bloodEffect[strEffects.Length];
        for (int i=0;i<strEffects.Length;i++)
        {
            effects[i]=new bloodEffect(strEffects[i]);
        }
    }

    private void parseCondtions(string str)
	{
		string[] strArr = str.Split ('#');
        condition = new PrizeSample[strArr.Length];
		for (int i = 0; i < strArr.Length; i++) {
            condition[i] = new PrizeSample(strArr[i], ',');
		}
	}	
}
public class bloodEffect {
    public bloodEffect()
    {
        
    }

    public bloodEffect(string str)
    {
        string[] st = str.Split(',');
        int tempType = StringKit.toInt(st[0]);
        type = tempType;
        if (tempType == 1)//基础属性+
        {
            prizeAttr(st[1],StringKit.toInt(st[2]));
        }else if (tempType==2)//基础属性按百分比+
        {
            prizePerAttr(st[1],StringKit.toInt(st[2]));
        }else if (tempType==3)//某一个技能等级+1
        {
            prizeSkill(StringKit.toInt(st[1]), StringKit.toInt(st[2]));
        }else if (tempType==4)
        {
            prizePerSkill(StringKit.toInt(st[1]), StringKit.toInt(st[2]));
        }else if (tempType==5)
        {
            prizeAddSkill(StringKit.toInt(st[1])); 
        }else if (tempType==6)
        {
            prizeChangeSkill(StringKit.toInt(st[1]), StringKit.toInt(st[2]));
        }
    }
    public int type;//效果类型 1：基础属性直接+；2：基础属性按百分比+；3某一个技能等级+ ；4：某一个技能效果增幅加百分比；5:直接加一个技能
    private string attrName;//属性标志
    public int skillSid;//技能Sid
    public int skilltype;//技能类型 1开场 2 天赋 3 特技
    public int hp;
    public int attack;
    public int defec;
    public int magic;
    public int agile;
    public int perhp;
    public int perattack;
    public int perdefec;
    public int permagic;
    public int peragile;
    public int perAllAttr;
    public int skillLv;
    public int perSkillLv;
    public int drSkillSid;//被替换的技能sid
    public string dec;//描述


    void prizeChangeSkill(int oldSid,int newSid)
    {
        drSkillSid = oldSid;
        skillSid = newSid;
        SkillSample oldSk = SkillSampleManager.Instance.getSkillSampleBySid(oldSid);
        SkillSample newSk = SkillSampleManager.Instance.getSkillSampleBySid(newSid);
        if (oldSk != null && newSk != null)
        {
            dec = LanguageConfigManager.Instance.getLanguage("bloodSkillChange", oldSk.name,"\n", newSk.name);
        }
        else
        {
            BuffSample oldbf = BuffSampleManager.Instance.getBuffSampleBySid(oldSid);
            BuffSample newbf = BuffSampleManager.Instance.getBuffSampleBySid(newSid);
            dec = LanguageConfigManager.Instance.getLanguage("bloodSkillChange", oldbf.name,"\n", newbf.name);
        }
    }
    void prizeAddSkill(int sid)
    {
        skillSid = sid;
        SkillSample sk;
        sk = SkillSampleManager.Instance.getSkillSampleBySid(skillSid);
        if (sk != null) {
            dec = LanguageConfigManager.Instance.getLanguage("bolldItemDec4", sk.name);
        } else {
            BuffSample bs = BuffSampleManager.Instance.getBuffSampleBySid(skillSid);
            dec = LanguageConfigManager.Instance.getLanguage("bolldItemDec4", bs.name);
        }
    }

    void prizePerSkill(int sid, int lv) {
        skillSid = sid;
        perSkillLv = lv;
        SkillSample sk;
        sk = SkillSampleManager.Instance.getSkillSampleBySid(skillSid);
        if (sk != null) {
            dec = LanguageConfigManager.Instance.getLanguage("bolldItemDec3", sk.name, lv.ToString()+"%");
        } else {
            BuffSample bs = BuffSampleManager.Instance.getBuffSampleBySid(skillSid);
            dec = LanguageConfigManager.Instance.getLanguage("bolldItemDec3", bs.name, lv.ToString() + "%");
        }
    }
    void prizeSkill(int sid,int lv)
    {
        skillSid = sid;
        skillLv = lv;
        SkillSample sk;
        sk = SkillSampleManager.Instance.getSkillSampleBySid(skillSid);
        if (sk != null)
        {
            dec = LanguageConfigManager.Instance.getLanguage("bolldItemDec2", sk.name, lv.ToString());
        }
        else
        {
            BuffSample bs = BuffSampleManager.Instance.getBuffSampleBySid(skillSid);
            dec = LanguageConfigManager.Instance.getLanguage("bolldItemDec2", bs.name, lv.ToString());
        }
    }
    void prizeAttr(string st,int val)
    {
        if (st == AttrChangeType.HP)
        {
            hp = val;
            dec = LanguageConfigManager.Instance.getLanguage("bloodItemDec1", LanguageConfigManager.Instance.getLanguage("s0005"),val.ToString());
        }
        else if(st==AttrChangeType.ATTACK)
        {
            attack = val;
            dec = LanguageConfigManager.Instance.getLanguage("bloodItemDec1", LanguageConfigManager.Instance.getLanguage("s0006"), val.ToString());
        }else if (st==AttrChangeType.DEFENSE)
        {
            defec = val;
            dec = LanguageConfigManager.Instance.getLanguage("bloodItemDec1", LanguageConfigManager.Instance.getLanguage("s0007"), val.ToString());
        } else if (st == AttrChangeType.MAGIC) {
            magic = val;
            dec = LanguageConfigManager.Instance.getLanguage("bloodItemDec1", LanguageConfigManager.Instance.getLanguage("s0008"), val.ToString());
        } else if (st == AttrChangeType.AGILE) {
            agile = val;
            dec = LanguageConfigManager.Instance.getLanguage("bloodItemDec1", LanguageConfigManager.Instance.getLanguage("s0009"), val.ToString());
        } else if (st == AttrChangeType.ALLATTR) {
            attack = defec = magic = hp = agile = val;
            dec = LanguageConfigManager.Instance.getLanguage("bloodItemDec1", LanguageConfigManager.Instance.getLanguage("bloodDesc8"), val.ToString());
        }
    }

    void prizePerAttr(string st,int val)
    {
        if (st == AttrChangeType.HP) {
            perhp = val;
            dec = LanguageConfigManager.Instance.getLanguage("bloodItemDec1", LanguageConfigManager.Instance.getLanguage("s0005"), val+"%");
        } else if (st == AttrChangeType.ATTACK) {
            perattack = val;
            dec = LanguageConfigManager.Instance.getLanguage("bloodItemDec1", LanguageConfigManager.Instance.getLanguage("s0006"), val + "%");
        } else if (st == AttrChangeType.DEFENSE) {
            perdefec = val;
            dec = LanguageConfigManager.Instance.getLanguage("bloodItemDec1", LanguageConfigManager.Instance.getLanguage("s0008"), val + "%");
        } else if (st == AttrChangeType.MAGIC) {
            permagic = val;
            dec = LanguageConfigManager.Instance.getLanguage("bloodItemDec1", LanguageConfigManager.Instance.getLanguage("s0009"), val + "%");
        } else if (st == AttrChangeType.AGILE) {
            peragile = val;
            dec = LanguageConfigManager.Instance.getLanguage("bloodItemDec1", LanguageConfigManager.Instance.getLanguage("s0007"), val + "%");
        } else if (st == AttrChangeType.PER_ALLATTR) {
            perhp = perattack = perdefec = peragile = permagic = val;
            perAllAttr = val;
            dec = LanguageConfigManager.Instance.getLanguage("bloodItemDec1", LanguageConfigManager.Instance.getLanguage("bloodDesc8"), val + "%");
        }
    }
}
