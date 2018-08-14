using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class RefineSample:Sample
{
    public RefineSample()
    {

    }
    public int equipRefineLevelId;//装备精炼等级ID
    public int equipRefineMaxLevelId;//装备精炼最高等级
    public List<RefinelvInfo> refinelvAttr;//类型
    public string stri;
    public int sid;




    public RefineSample(string str)
	{
		parse (str);
	}


    //public override void parse(int sid, string str)
    private void parse(string str)
    {
        base.parse(sid,str);
        string[] strArr = str.Split('|');
        this.sid = StringKit.toInt(strArr[0]);
        this.equipRefineLevelId = StringKit.toInt(strArr[1]);
        this.equipRefineMaxLevelId = StringKit.toInt(strArr[2]);
        parseStr(strArr[3]);

    }
    private void parseStr(string str)
    {
        refinelvAttr=new List<RefinelvInfo>();
        string[] strArr = str.Split('$');
        for (int i = 0; i < strArr.Length;i++ )
        {
            RefinelvInfo rli = new RefinelvInfo(strArr[i]);
            refinelvAttr.Add(rli);
        }
    }
    private void parseCombat()
    {

    }
    public override void copy(object destObj)
    {
        base.copy(destObj);
        RefineSample dest = destObj as RefineSample;
        if (this.refinelvAttr != null)
        {
            dest.refinelvAttr = new List<RefinelvInfo>();
            for (int i = 0; i < this.refinelvAttr.Count; i++)
                dest.refinelvAttr[i] = this.refinelvAttr[i];
        }
    }

}
/// <summary>
/// 类，精炼每一个阶段的属性
/// </summary>
public class RefinelvInfo{
    public List<AttrRefineChangeSample> items = new List<AttrRefineChangeSample>();
   public List<string> dec = new List<string>();
  public RefinelvInfo(){

    }
  public RefinelvInfo(string str)
  {
      parseEffects(str);
  }
  private void parseEffects(string str)
  {
      //表示空
      if (str == Convert.ToString(0))
          return;
      string[] strArr = str.Split('#');
          //effects = new AttrChangeSample[strArr.Length];
      for (int i = 0; i < strArr.Length; i++)
      {
          AttrRefineChangeSample attr = new AttrRefineChangeSample();
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
          else if (attr.getAttrType()==AttrChangeType.DESC1)
          {
              dec.Add(LanguageConfigManager.Instance.getLanguage("refine_013"));
          }
          else if (attr.getAttrType() == AttrChangeType.DESC2)
          {
              dec.Add(LanguageConfigManager.Instance.getLanguage("refine_021"));
          }
          else if (attr.getAttrType() == AttrChangeType.DESC3)
          {
              dec.Add(LanguageConfigManager.Instance.getLanguage("refine_014"));
          }
          else if (attr.getAttrType() == AttrChangeType.DESC4)
          {
              dec.Add(LanguageConfigManager.Instance.getLanguage("refine_021"));
          }
          items.Add(attr);
      }
  
  }
}

