using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
 
/**
 * 女神进化管理器
 * @author longlingquan
 * */
public class BeastEvolveManagerment
{
	private List<BeastEvolve> list;
	public int num = 0;//女神数量
	public int evolveNum = 0;//女神进化数量
	public int skilllv = 0;//圣器等级=技能等级
	public long exp;//圣器经验
	public int day;//获取是否同一天,无用属性
	public int count;//已使用免费次数
	public static float NUM_ADD = 2f;//女神数量加成百分比
	public static float EVOLVE_ADD = 2f;//女神进化加成百分比
	private int initCount = 1;//初始免费次数
	public string BeastName ;
	public bool showEffect = false;

	public static BeastEvolveManagerment Instance {
		get{return SingleManager.Instance.getObj("BeastEvolveManagerment") as BeastEvolveManagerment;}
	}

	/// <summary>
	/// 返回圣器经验
	/// </summary>
	public long getHallowExp ()
	{
		return exp;
	}

	/// <summary>
	/// 设置圣器经验
	/// </summary>
	public void setHallowExp (long _exp)
	{
		this.exp = _exp;
	}

	/// <summary>
	/// 获得免费次数
	/// </summary>
	public int getHallowCount ()
	{
		if (UserManager.Instance.self.getVipLevel () <= 0)
			return initCount;
		else {
			int addCount = VipManagerment.Instance.getVipbyLevel (UserManager.Instance.self.getVipLevel ()).privilege.unrealFreeDay;
			return initCount + addCount;
		}
	}

	/// <summary>
	/// 获得剩余免费次数
	/// </summary>
	public int getLaveHallowConut ()
	{
		return getHallowCount () - count;
	}

	/// <summary>
	/// 设置剩余免费次数
	/// </summary>
	public void setHallowCount (int _count)
	{
		this.count = _count;
	}

	/// <summary>
	/// 返回否同一天,无用属性
	/// </summary>
	public int getHallowDay ()
	{
		return day;
	}

	public void setHallowDay (int _day)
	{
		this.day = _day;
	}

	/// <summary>
	/// 返回女神技能等级(圣器等级)
	/// </summary>
	public int getSkillLv ()
	{
		return EXPSampleManager.Instance.getLevel (EXPSampleManager.SID_HALLOW_EXP, exp);
	}

	/// <summary>
	/// 返回女神数量加成百分比
	/// </summary>
	public int getNumAdd ()
	{
		return (int)(num * NUM_ADD);
	}

	/// <summary>
	/// 返回女神进化加成百分比
	/// </summary>
	public int getEvolveNumAdd ()
	{
		return (int)(evolveNum * EVOLVE_ADD);
	}

	/// <summary>
	/// 返回女神共鸣总加成
	/// </summary>
	public int getBestResonance ()
	{
		return getNumAdd () + getEvolveNumAdd ();
	}

	/// <summary>
	/// 返回指定数目的共鸣总加成
	/// </summary>
	public int getBestResonanceByNums (int num, int evoNum)
	{
		return (int)(num * NUM_ADD) + (int)(evoNum * EVOLVE_ADD);
	}

	/// <summary>
	/// 返回女神进化后共鸣总加成
	/// </summary>
	public int getNextEvolveBestResonance ()
	{
		return getNumAdd () + (int)((evolveNum + 1) * EVOLVE_ADD);
	}

	/// <summary>
	/// 获得共鸣属性比例加成
	/// </summary>
	public CardBaseAttribute getBeastResonanceEffectByPer ()
	{
		CardBaseAttribute attr = new CardBaseAttribute ();

		attr.perHp += getBestResonance ();
		attr.perAttack += getBestResonance ();
		attr.perDefecse += getBestResonance ();
		attr.perMagic += getBestResonance ();
		attr.perAgile += getBestResonance ();

		return attr;
	}

	/// <summary>
	/// 获得所有女神
	/// </summary>
	public List<BeastEvolve> getAllBest ()
	{
		return list;
	}

	/// <summary>
	/// 是否已拥有女神
	/// </summary>
	public bool isHaveBeast ()
	{
		List<BeastEvolve> beastList = getAllBest ();
	
		for (int i = 0; i < beastList.Count; i++) {
			if (beastList [i].isAllExist ())
				return true;
		}
		return false;
	}

	public BeastEvolveManagerment ()
	{ 
		list = BeastConfigManager.Instance.getList ();
	}

	/// <summary>
	/// 改变女神数量 进化数量
	/// </summary>
	public void updateNum (int num, int evolve)
	{
		this.num = num;
		this.evolveNum = evolve;
	}

	/// <summary>
	/// 女神进化
	/// </summary>
	public void beastEvolve ()
	{
		evolveNum++;
	}

	/// <summary>
	/// 女神召唤
	/// </summary>
	public void beastSummon ()
	{
		num++;
	}

	/// <summary>
	/// 根据指定的女神SID获得相应的女神序号0-11
	/// </summary>
	public int getBeastIndexBySid(int _sid)
	{
		if(list == null)
			return 0;
		for (int i = 0; i < list.Count; i++) {
			if(list[i].isExist(_sid)) {
				return i;
			}
		}
		return 0;
	}

	/// <summary>
	/// 根据指定的女神图片ID获得相应的女神序号-----此方法返回的下标是从1开始的,小心点用
	/// </summary>
	public int getBeastIndexByImageId(int _id)
	{
		for (int i = 1,j = 2050; i < 13; i++ , j++) {
			if (_id == j) {
				return i;
			}
		}
		return 1;
	}

	/// <summary>
	/// 指定商品SID是否对应已召唤的女神
	/// </summary>
	public bool isSameBeastGoods(int _goodsSid)
	{
		for (int i = 0; i <list.Count; i++) {
			if (list [i].isAllExist()) {
				ExchangeSample sample = list [i].getExchangeBySids(list [i].getFristBeast().sid);
				foreach (ExchangeCondition each in sample.conditions[0]) {
					if (each.costSid == _goodsSid) {
						return true;
					} else {
						continue;
					}
				}
			}
		}
		return false;
	}
	
	//获得指定索引的女神进化信息
	public BeastEvolve getBeastEvolveByIndex (int index)
	{
		return list [index];
	}
	
	//获得指定sid对应的女神进化信息
	public BeastEvolve getBeastEvolveBySid (int sid)
	{
		for (int i = 0; i <list.Count; i++) {
			if (list [i].isExist (sid))
				return list [i];
		}
		return null;
	}
	/// <summary>
	/// 更新现有的女神列表（并且剔除满级的和比玩家等级高的） 
	/// </summary>
	public bool haveCanTranningBeast(){
		ArrayList beastList=StorageManagerment.Instance.getAllBeast();
		if(beastList!=null){
			for (int k = 0; k < beastList.Count; k++) {
				Card ca=beastList[k] as Card;
				if(ca.getLevel()<StorageManagerment.Instance.getRole(UserManager.Instance.self.mainCardUid).getLevel()&&ca.getLevel()<ca.getMaxLevel()){
					return true;
				}
			}
			return false;
		}
		return false;
	}
	
} 

