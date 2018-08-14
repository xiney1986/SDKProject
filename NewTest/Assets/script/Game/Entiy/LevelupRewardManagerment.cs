using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/**
 * 公告管理器
 * 所有公告信息
 * @author 汤琦
 * */
public class LevelupRewardManagerment
{
	public static LevelupRewardManagerment Instance {
		get{ return SingleManager.Instance.getObj ("LevelupRewardManagerment") as LevelupRewardManagerment;}
	}
	//升级奖励是否可领取
	public bool receiveEnabled=false;
	//上次领奖的sid
	public int lastRewardSid=0;
}