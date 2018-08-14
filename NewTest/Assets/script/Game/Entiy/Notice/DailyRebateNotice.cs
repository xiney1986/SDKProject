using UnityEngine;
using System.Collections.Generic;

public class DailyRebateNotice : Notice
{
	public DailyRebateNotice (int sid):base(sid)
	{
		this.sid = sid;
	}
	public override bool isValid (){
		if (UserManager.Instance.self.getUserLevel () >= 10)
			return true;
		else
			return false;
	}
}

