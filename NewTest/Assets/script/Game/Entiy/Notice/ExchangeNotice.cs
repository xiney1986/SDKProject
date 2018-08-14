using UnityEngine;
using System.Collections.Generic;

/**
 * 兑换活动公告
 * @author hzh
 * */
public class ExchangeNotice:Notice
{

	public ExchangeNotice (int sid):base(sid)
	{
		this.sid = sid;
	}

	public override bool isValid ()
	{
		if (isInTimeLimit ()) {
			if (!isTimeLimit ())
				return ExchangeManagerment.Instance.getCanUseExchanges ((getSample ().content as SidNoticeContent).sids, getSample ().type, false).Count > 0;
			return true;
		}
		return false;
	}
}

