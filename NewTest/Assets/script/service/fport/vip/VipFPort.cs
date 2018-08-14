using System;
using UnityEngine;

/**
 * VIP access端口，处理领取和获得信息，这两个通讯返回格式相同所有写在一起
 * @author zhoujie
 * */
public class VipFPort:BaseFPort
{
	private CallBack callback;
	
	public VipFPort ()
	{
	}
	/**
	* 领取特权礼包 access通讯
	* 
	* @return  CallBack<int>返回 0已领取|领取的道具sid
	* */
	public void get_gift (CallBack callback,int awardSid)
	{ 
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.VIP_GET_GIFT);
		message.addValue ("award_sid", new ErlInt (awardSid));
		access (message);
	}
	
	/**
	* 获得已领取特权礼包信息 access通讯
	* 
	* @return CallBack<int>返回 0未领过|已领取特权礼包索引
	* */
	public void get_gift_info (CallBack callback)
	{ 
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.VIP_GET_INFO);
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{
		string msg = (message.getValue ("msg") as ErlType).getValueString ();
		if (msg == "ok") {
			int sid = StringKit.toInt((message.getValue ("sid") as ErlType).getValueString());//领取奖励sid
			VipManagerment.Instance.addAwardSids(sid);
			if (callback != null){
				callback();
				callback = null;
			}
		} else {
			if (callback != null)
				callback = null;
		}
	}

}

