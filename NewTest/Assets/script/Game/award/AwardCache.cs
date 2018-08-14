using System;
using System.Collections.Generic;
 
/**
 * 奖励基础缓存结构
 * @author longlingquan
 * */
public class AwardCache
{ 
	private CallBackAwards func;//奖励发送方法 
	private List<Award> awards;//奖励信息
	
	public AwardCache ()
	{
		 
	}
	
	//添加方法
	public void addFunc (CallBackAwards func)
	{
		this.func = func;  
		sendAward ();
	}
	
	//设置奖励 
	public void setAwards (Award[] arr)
	{ 
		awards = new List<Award> ();
		awards.InsertRange (0, arr);
		sendAward (); 
	}
	
	//添加奖励
	public void addAwards (Award[] arr)
	{
		if (awards == null)
			awards = new List<Award> (); 
		awards.InsertRange (awards.Count - 1, arr);
	}
	 
	public void clear ()
	{
		this.func = null;
		this.awards = null;
	}
	
	//获得奖励 不需要回调函数 用于显示连续的同类型奖励
	//不需要再获得奖励后清空
	public Award getAward ()
	{
		if (awards.Count < 1)
			return null;
		Award award = awards [0];
		awards.RemoveAt (0);
		return award;
	}
	/** 获取所有缓存奖励 */
	public Award[] getAwards() {
		if (awards == null || awards .Count < 1)
			return null; 
		return awards.ToArray ();
	}
	//发送奖励方法guild_war
	//需要在获得奖励后清空
	public void sendAward ()
	{
		
		
		if (func == null || awards == null || awards .Count < 1)
			return; 

		func (awards.ToArray());  
		clear ();

		/*
		if(func==null)
			return;
		if (awards == null || awards.Count < 1){
			
			func (null);  
			clear ();
			return;
		}else{
			func (awards.ToArray());  
			clear ();
		}
		*/

	}
}  