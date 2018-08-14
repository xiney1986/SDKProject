using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InviteCode{
	
	public string uid;//奖励ID
	public int  jindu;//进度
	public int awardType;//奖励状态值，0未领取，1已领取
	public string inviteNeedNum;//邀请需求数量
	public string tapType;//标签
	public string needLevel;//满足等级
	public List<PrizeSample> prizes;//奖励数组
	
	public InviteCode (string str)
	{
		parse (str);
	}
	
	private void parse (string str)
	{
		string[] strArr = str.Split ('|');
		uid = strArr [0];
		inviteNeedNum = strArr [1];
		tapType = (strArr [2]);
		needLevel = (strArr [3]);
		parsePrizes (strArr [4]);
	}
	
	private void parsePrizes (string str)
	{
		prizes = new List<PrizeSample>();
		string[] strs = str.Split('#');
		for (int i = 0; i < strs.Length; i++) {			
			PrizeSample prize = new PrizeSample(strs[i],',');
			prizes.Add(prize);
		}
	}
}