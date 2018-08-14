using UnityEngine;
using System.Collections.Generic;

/**
 * 公告实体类
 * @author hzh
 * */
public class RechargeNotice:Notice
{
	public RechargeNotice (int sid):base(sid)
	{
		this.sid = sid;
	}

	public override bool isValid ()
	{
		if (isInTimeLimit ()) {
			if (!isTimeLimit ())
				return RechargeManagerment.Instance.getCanUseRecharges ((getSample ().content as SidNoticeContent).sids).Count > 0;
			return true;
		}
		return false;
	}
}

