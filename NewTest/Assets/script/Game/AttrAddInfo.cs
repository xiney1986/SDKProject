using UnityEngine;
using System.Collections;

/// <summary>
/// 类说明：附加属性信息类
/// </summary>
public class AttrAddInfo
{

    /* fields */
	/** 附加前的技能等级 */
	int oldSkillGrade;
	/**装备之前的等级 */
	int oldEquipGrade;
	/**装备属性增加值 */
	int equipAddGrade;
	/**之后的装备增加等级 */
	int equipGrade;
	/** 增加的技能等级 */
	int skillGrade;
    /** 附加前的生命等级 */
    int oldHpGrade;
    /** 附加前的攻击等级 */
    int oldAttGrade;
    /** 附加前的防御等级 */
    int oldDefGrade;
    /** 附加前的魔法等级 */
    int oldMagGrade;
    /** 附加前的敏捷等级 */
    int oldDexGrade;
    /** 增加的附加生命等级 */
    int hpGrade;
    /** 增加的附加攻击等级 */
    int attGrade;
    /** 增加的附加防御等级 */
    int defGrade;
    /** 增加的附加魔法等级 */
    int magGrade;
    /** 增加的附加敏捷等级 */
    int dexGrade;
    /** 生命附加经验 */
    int hpExp;
    /** 攻击附加经验 */
    int attExp = 0;
    /** 防御附加经验 */
    int defExp = 0;
    /** 魔法附加经验 */
    int magExp = 0;
    /** 敏捷附加经验 */
    int dexExp = 0;
	/** 原始生命经验 */
	public int oldHpExp=0;
	/** 原始攻击经验 */
	public int oldAttExp = 0;
	/** 原始防御经验 */
	public int oldDefExp = 0;
	/** 原始魔法经验 */
	public int oldMagExp = 0;
	/** 原始敏捷经验 */
	public int oldDexExp = 0;

    /* methods */
    /// <summary>
    /// 清理数据
    /// </summary>
    public void clear()
    {
		oldSkillGrade=0;
		equipAddGrade=0;
		oldEquipGrade=0;
		equipGrade=0;
		skillGrade=0;
        oldHpGrade = 0;
        oldAttGrade = 0;
        oldDefGrade = 0;
        oldMagGrade = 0;
        oldDexGrade = 0;
        hpGrade = 0;
        attGrade = 0;
        defGrade = 0;
        magGrade = 0;
        dexGrade = 0;
        hpExp = 0;
        attExp = 0;
        defExp = 0;
        magExp = 0;
        dexExp = 0;
		oldHpExp=0;
		oldAttExp = 0;
		oldDefExp = 0;
		oldMagExp = 0;
		oldDexExp = 0;
    }

    /* properties */
	public int EquipAddGrade
	{
		get{return equipAddGrade;}
		set{equipAddGrade=value;}
	}
	public int OldEquipGrade 
	{
		get{return oldEquipGrade;}
		set{oldEquipGrade=value;}
	}
	public int EquipGrad
	{
		get{return equipGrade;}
		set{equipGrade=value;}
	}
    public int HpGrade
    {
        get { return hpGrade; }
        set { hpGrade = value; }
    }
    public int AttGrade
    {
        get { return attGrade; }
        set { attGrade = value; }
    }
    public int DefGrade
    {
        get { return defGrade; }
        set { defGrade = value; }
    }
    public int MagGrade
    {
        get { return magGrade; }
        set { magGrade = value; }
    }
    public int DexGrade
    {
        get { return dexGrade; }
        set { dexGrade = value; }
    }
    public int OldHpGrade
    {
        get { return oldHpGrade; }
        set { oldHpGrade = value; }
    }
	public int OldSkillGrade
	{
		get{return oldSkillGrade;}
		set{oldSkillGrade=value;}
	}
	public int SkillGrade
	{
		get{return skillGrade;}
		set{skillGrade=value;}
	}
    public int OldAttGrade
    {
        get { return oldAttGrade; }
        set { oldAttGrade = value; }
    }
    public int OldDefGrade
    {
        get { return oldDefGrade; }
        set { oldDefGrade = value; }
    }
    public int OldMagGrade
    {
        get { return oldMagGrade; }
        set { oldMagGrade = value; }
    }
    public int OldDexGrade
    {
        get { return oldDexGrade; }
        set { oldDexGrade = value; }
    }
    public int HpExp
    {
        get { return hpExp; }
        set { hpExp = value; }
    }
    public int AttExp
    {
        get { return attExp; }
        set { attExp = value; }
    }
    public int DefExp
    {
        get { return defExp; }
        set { defExp = value; }
    }
    public int MagExp
    {
        get { return magExp; }
        set { magExp = value; }
    }
    public int DexExp
    {
        get { return dexExp; }
        set { dexExp = value; }
    }
}
