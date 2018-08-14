using UnityEngine;
using System;

public class SampleUser:CloneObject
{
	
	private const int USER_EXP_SID = 1;//玩家经验索引
	private const int VIP_EXP_SID = 3;//玩家vip经验索引
	
	public string nickname = "";//玩家名称
	public int style = 0;//头像	
	private long exp = 0;//当前经验 
	
	public long vipExp = 0;//vip经验值
	public int vipLevel = 0;//vip等级 
	
	private int level = 1;//玩家等级 
	
	public SampleUser ()
	{
		
	}

	public SampleUser (string nickname, int style, long exp, long vipExp)
	{
		this.nickname = nickname;
		this.style = style;
		this.exp = exp;
		this.vipExp = vipExp;
	}	
	
	//获得玩家等级 
	public int getUserLevel ()
	{ 
		return level;
	}
 


	//获得玩家vip等级 
	public int getVipLevel ()
	{ 
		return vipLevel;
	}
	
	public long getEXP ()
	{
		return exp;
	}
	
	public long getVipEXP ()
	{
		return vipExp;
	}	
	//获得当前等级经验值 
	public long getLevelExp ()
	{
		return exp - getEXPDown ();
	}
	
	//获得当前等级经验值
	public long getLevelAllExp ()
	{
		return getEXPUp () - getEXPDown ();
	}
	
	private void updateLevel ()
	{
		level = EXPSampleManager.Instance.getLevel (USER_EXP_SID, exp, level); 
	}
	
	private void updateVipLevel ()
	{
		vipLevel = EXPSampleManager.Instance.getLevel (VIP_EXP_SID, vipExp, vipLevel); 
	}
	
	//改变经验值
	public void updateExp (long exp)
	{ 
		this.exp = exp;
		updateLevel ();
	}
	
	//改变vip经验值
	public void updateVipExp (long vipExp)
	{ 
		this.vipExp = vipExp;
		updateVipLevel ();
	}
	
	//获得当前等级经验下限
	public long getEXPDown ()
	{
		if (level == 0)
			getUserLevel ();
		return EXPSampleManager.Instance.getEXPDown (USER_EXP_SID, level);
	}
	//获得当前vip等级经验下限
	public long getVipEXPDown ()
	{
		if (vipLevel == 0)
			getVipLevel ();
		return EXPSampleManager.Instance.getEXPDown (VIP_EXP_SID, vipLevel);
	}
	//获得当前vip等级经验值上限
	public long getVipEXPUp ()
	{
		if (vipLevel == 0)
			getVipLevel ();
		return EXPSampleManager.Instance.getEXPUp (VIP_EXP_SID, vipLevel);
	}
	/// <summary>
	/// 得到指定vip等级上限
	/// </summary>
	/// <returns>The vip EXP up.</returns>
	/// <param name="lv">Lv.</param>
	public long getVipEXPUp(int lv){
		return EXPSampleManager.Instance.getEXPUp (VIP_EXP_SID, lv);
	}
	/// <summary>
	/// 得到指定Vip等级下限
	/// </summary>
	/// <returns>The vip EXP down.</returns>
	/// <param name="lv">Lv.</param>
	public long getVipEXPDown(int lv){
		return EXPSampleManager.Instance.getEXPDown (VIP_EXP_SID, lv);
	}
	//获得当前等级经验值上限
	public long getEXPUp ()
	{
		if (level == 0)
			getUserLevel ();
		return EXPSampleManager.Instance.getEXPUp (USER_EXP_SID, level);
	}

	public override void copy (object destObj)
	{
		base.copy (destObj);
	}
}
