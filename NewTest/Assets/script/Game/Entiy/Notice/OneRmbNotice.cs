using UnityEngine;
using System.Collections;

/**
 * 首冲公告
 * @author hzh
 * */
public class OneRmbNotice:Notice
{
	public OneRmbNotice (int sid):base(sid)
	{
		this.sid = sid;
	}

	public override bool isValid ()
	{
		if (RechargeManagerment.Instance.canFirst)
			return true;
		return !(RechargeManagerment.Instance.getOneRmb () != null && RechargeManagerment.Instance.getOneRmb ().count > 0);
	}
}

