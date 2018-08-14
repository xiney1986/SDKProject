using UnityEngine;
using System.Collections;

/**
 * 获取后台邀请数量，进度，是否领奖等
 * @author 陈世惟
 * */
public class InviteCodeInfoFPort : BaseFPort {

	private CallBack callback;

	public void access (CallBack call)
	{
		this.callback = call;
		ErlKVMessage message = new ErlKVMessage (FrontPort.INVITECODE_GET_INVITEINFO);
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{
		ErlArray array = message.getValue ("msg") as ErlArray;
		
		if (array == null)
			return ;
		
		InviteCodeManagerment.Instance.initInviteCodeInfo(array);
		if (callback != null)
			callback ();
		
	}
}
