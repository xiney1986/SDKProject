using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GuildHurtValuePrizeItem : MonoBase
{
	public UILabel hurtValue;
	public UILabel liveness;
	public UILabel prizeNum;

	public void init(GuildBossPrizeSample prizeSample,long bossCurrentHurt)
	{
		if(prizeSample.hurt == bossCurrentHurt)
		{
			hurtValue.text = "[00FF00]"+prizeSample.hurt+"[-]";
			liveness.text = "[00FF00]"+"+"+prizeSample.liveness+"[-]";
			prizeNum.text = "[00FF00]"+"x"+prizeSample.prizeSum+"[-]";
		}
		else
		{
			hurtValue.text = prizeSample.hurt.ToString();
			liveness.text = "+"+prizeSample.liveness;
			prizeNum.text = "x"+prizeSample.prizeSum;
		}

	}
}