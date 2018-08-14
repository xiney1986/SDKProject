using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 公会祭坛实体类
 * @author 汤琦
 * */
public class GuildAltar 
{
	public GuildAltar(int bossSid,long hurtSum,int count,List<GuildAltarRank> list)
	{
		this.bossSid = bossSid;
		this.hurtSum = hurtSum;
		this.count = count;
		this.list = list;
	}
	public int bossSid;//BOSS sid
	public long hurtSum;//公会总伤害
	public int count;//挑战次数
	public List<GuildAltarRank> list;//公会祭坛排名
}

public class GuildAltarRank
{
	public GuildAltarRank(string sid,string playerName,long hurtValue)
	{
		this.sid = sid;
		this.playerName = playerName;
		this.hurtValue = hurtValue;
	}
	public string sid;//sid
	public string playerName;//名称
	public long hurtValue;//伤害值
}
