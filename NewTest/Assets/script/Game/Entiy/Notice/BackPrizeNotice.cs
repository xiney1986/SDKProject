using UnityEngine;
using System.Collections;

public class BackPrizeNotice : Notice
{
	public ActiveTime activeTime;

	public BackPrizeNotice (int sid): base(sid)
	{
		this.sid = sid;
	}

	public override bool isValid () 
	{
		return BackPrizeLoginInfo.Instance.backPrizeIsActive;
	}
}
