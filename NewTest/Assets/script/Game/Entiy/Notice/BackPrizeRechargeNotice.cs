using UnityEngine;
using System.Collections;

public class BackPrizeRechargeNotice : Notice
{

	public BackPrizeRechargeNotice (int sid): base(sid)
	{
		this.sid = sid;
	}


	public override bool isValid () 
	{
		return BackPrizeRechargeInfo.Instance.isActive;
	}
}
