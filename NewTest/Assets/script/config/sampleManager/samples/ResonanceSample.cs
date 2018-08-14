using System.Security;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 强化共鸣模板类
/// </summary>
public class ResonanceSample:Sample
{
    public ResonanceSample()
    {

    }
    public int resonanceLv;//强化或精练的阶段
    public int resonanceType;//强化或精练类型 1是装备强化 2是装备精练 3是星魂强化
    public int needLv;//需要的强化或精练等级
    public ResonanceInfo resonanceAttr;//每个阶段能得到的收益 属性
    public int sid;




    public ResonanceSample(string str)
	{
		parse (str);
	}


    //public override void parse(int sid, string str)
    private void parse(string str)
    {
        base.parse(sid,str);
        string[] strArr = str.Split('|');
        this.sid = StringKit.toInt(strArr[0]);
        resonanceLv = StringKit.toInt(strArr[2]);
        resonanceType = StringKit.toInt(strArr[1]);
        needLv = StringKit.toInt(strArr[3]);
        parseStr(strArr[4]);

    }
    private void parseStr(string str)
    {
        string[] strArr = str.Split('$');
        for (int i = 0; i < strArr.Length;i++ )
        {
            resonanceAttr = new ResonanceInfo(strArr[i]);
        }
    }
    public override void copy(object destObj)
    {
        base.copy(destObj);
        ResonanceSample dest = destObj as ResonanceSample;
        if (this.resonanceAttr != null)
        {
            dest.resonanceAttr = new ResonanceInfo();
            dest.resonanceAttr = this.resonanceAttr;              
        }
    }

}
/// <summary>
/// 类，精炼每一个阶段的属性
/// </summary>
public class ResonanceInfo{
   public  List<AttrChangeSample> items=new List<AttrChangeSample>();
   public List<string> dec = new List<string>();
  public ResonanceInfo(){

    }
  public ResonanceInfo(string str)
  {
      parseEffects(str);
  }
  private void parseEffects(string str)
  {
      //表示空
      if (str == Convert.ToString(0))
          return;
      string[] strArr = str.Split('#');
     // effects = new AttrChangeSample[strArr.Length];
      for (int i = 0; i < strArr.Length; i++)
      {
          AttrChangeSample attr = new AttrChangeSample();
          attr.parse(strArr[i]);
          if (attr.getAttrType() == AttrChangeType.HP)
          {
              dec.Add(LanguageConfigManager.Instance.getLanguage("refine_008"));
          }
          else if (attr.getAttrType() == AttrChangeType.ATTACK)
          {
              dec.Add(LanguageConfigManager.Instance.getLanguage("refine_019"));
          }
          else if (attr.getAttrType()==AttrChangeType.DEFENSE)
          {
              dec.Add(LanguageConfigManager.Instance.getLanguage("refine_018"));
          }
          else if(attr.getAttrType()==AttrChangeType.AGILE)
          {
              dec.Add(LanguageConfigManager.Instance.getLanguage("refine_016"));
          }
          else if (attr.getAttrType() == AttrChangeType.MAGIC)
          {
              dec.Add(LanguageConfigManager.Instance.getLanguage("refine_010"));
          }
          else if (attr.getAttrType() == AttrChangeType.PER_AGILE)
          {
              dec.Add(LanguageConfigManager.Instance.getLanguage("refine_016"));
          }
          else if (attr.getAttrType() == AttrChangeType.PER_ATTACK)
          {
              dec.Add(LanguageConfigManager.Instance.getLanguage("refine_019"));
          }
          else if (attr.getAttrType() == AttrChangeType.PER_DEFENSE)
          {
              dec.Add(LanguageConfigManager.Instance.getLanguage("refine_018"));
          }
          else if (attr.getAttrType() == AttrChangeType.PER_HP)
          {
              dec.Add(LanguageConfigManager.Instance.getLanguage("refine_008"));
          }
          else if (attr.getAttrType() == AttrChangeType.PER_MAGIC)
          {
              dec.Add(LanguageConfigManager.Instance.getLanguage("refine_010"));
          }
          items.Add(attr);
      }
  }
}

