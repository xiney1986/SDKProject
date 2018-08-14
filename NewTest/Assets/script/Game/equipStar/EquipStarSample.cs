using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 装备升星消耗信息模板
/// </summary>
public class EquipStarSample :Sample
{	

    public EquipStarSample ()
    {
    }

    /* fields */
	/** 装备升星玩家等级 */
	public int[] equipStarUserLevel;
	/** 装备升星玩家VIP等级 */
    public int[] equipStarUserVipLevel;
	/** 装备升星装备品质 */
    public int[] equipStarEquipQuality;
	/** 装备升星装备等级 */
    public int[] equipStarEquipLevel;
	/** 单次升星消耗装备结晶sid */
    public int[] equipStarConsumeCrystalSids;
	/** 单次升星消耗装备结晶sid */
    public int[] equipStarConsumeCrystalValues;
	/** 单次升星消耗符文石sid */
    public int[] equipStarConsumeStoneSids;
	/** 单次升星消耗符文石sid */
    public int[] equipStarConsumeStoneValues;
    /** 红装单次升星消耗金币 */
    public string[] redEquipStarConsumeMoneyValues;
	/** 解析 */
	public override void parse( int sid,string str)
	{

	    this.sid = sid;
		string[] strs=str.Split('|');
		//checkLength (strs.Length,8);

		parseUserLevel (strs [1]);
		parseUserVipLevel (strs [2]);
		parseEquipQuality (strs [3]);
		parseEquipLevel (strs [4]);
		parseConsumeCrystalSid (strs [5]);
		parseConsumeCrystalValue (strs [6]);
		parseConsumeStoneSid (strs [7]);
		parseConsumeStoneValue (strs [8]);
        if(strs.Length > 9)
	        parseConsumeMoneyValue(strs[9]);

	}
	/** 解析玩家等级限制 */
	private void parseUserLevel (string str)
	{
		//表示空
		if (str == Convert.ToString(0))
			return;
		string[] strArr = str.Split ('#');  
		equipStarUserLevel = new int[strArr.Length]; 
		for (int i = 0; i<strArr.Length; i++) {
			equipStarUserLevel [i] = StringKit.toInt(strArr [i]);
		}
	}
	/** 解析玩家VIP等级限制 */
	private void parseUserVipLevel (string str)
	{
		//表示空
		if (str == Convert.ToString(0))
			return;
		string[] strArr = str.Split ('#');  
		equipStarUserVipLevel = new int[strArr.Length]; 
		for (int i = 0; i<strArr.Length; i++) {
			equipStarUserVipLevel [i] = StringKit.toInt(strArr [i]);
		}
	}

	/** 解析装备品质限制 */
	private void parseEquipQuality (string str)
	{
		//表示空
		if (str == Convert.ToString(0))
			return;
		string[] strArr = str.Split ('#');  
		equipStarEquipQuality = new int[strArr.Length]; 
		for (int i = 0; i<strArr.Length; i++) {
			equipStarEquipQuality [i] = StringKit.toInt(strArr [i]);
		}
	}
	/** 解析装备等级限制 */
	private void parseEquipLevel (string str)
	{
		//表示空
		if (str == Convert.ToString(0))
			return;
		string[] strArr = str.Split ('#');  
		equipStarEquipLevel = new int[strArr.Length]; 
		for (int i = 0; i<strArr.Length; i++) {
			equipStarEquipLevel [i] = StringKit.toInt(strArr [i]);
		}
	}
	/** 解析消耗结晶sid */
	private void parseConsumeCrystalSid (string str)
	{
		//表示空
		if (str == Convert.ToString(0))
			return;
		string[] strArr = str.Split ('#');  
		equipStarConsumeCrystalSids = new int[strArr.Length]; 
		for (int i = 0; i<strArr.Length; i++) {
			equipStarConsumeCrystalSids [i] = StringKit.toInt(strArr [i]);
		}
	}
	/** 解析消耗结晶value */
	private void parseConsumeCrystalValue (string str)
	{
		//表示空
		if (str == Convert.ToString(0))
			return;
		string[] strArr = str.Split ('#');  
		equipStarConsumeCrystalValues = new int[strArr.Length]; 
		for (int i = 0; i<strArr.Length; i++) {
			equipStarConsumeCrystalValues [i] = StringKit.toInt(strArr [i]);
		}
	}
	/** 解析消耗符文石sid */
	private void parseConsumeStoneSid (string str)
	{
		//表示空
		if (str == Convert.ToString(0))
			return;
		string[] strArr = str.Split ('#');  
		equipStarConsumeStoneSids = new int[strArr.Length]; 
		for (int i = 0; i<strArr.Length; i++) {
			equipStarConsumeStoneSids [i] = StringKit.toInt(strArr [i]);
		}
	}
	/** 解析消耗符文石value */
	private void parseConsumeStoneValue (string str)
	{
		//表示空
		if (str == Convert.ToString(0))
			return;
		string[] strArr = str.Split ('#');  
		equipStarConsumeStoneValues = new int[strArr.Length]; 
		for (int i = 0; i<strArr.Length; i++) {
			equipStarConsumeStoneValues [i] = StringKit.toInt(strArr [i]);
		}
	}
    /** 解析升红后装备升星消耗的金币 */
    private void parseConsumeMoneyValue(string str)
    {
        string[] strArr = str.Split('#');
        redEquipStarConsumeMoneyValues = new string[strArr.Length];
        for (int i = 0; i < strArr.Length; i++)
        {
            redEquipStarConsumeMoneyValues[i] = strArr[i];
        }
    }

    public override void copy(object destObj) {
        base.copy(destObj);
        EquipStarSample dest = destObj as EquipStarSample;
        if (this.redEquipStarConsumeMoneyValues != null)
        {
            dest.redEquipStarConsumeMoneyValues = new string[redEquipStarConsumeMoneyValues.Length];
            for (int i = 0; i < redEquipStarConsumeMoneyValues.Length; i++)
            {
                dest.redEquipStarConsumeMoneyValues[i] = this.redEquipStarConsumeMoneyValues[i];
            }
        }
    }
}
