using UnityEngine;
using System.Collections;

/// <summary>
/// 传承端口
/// </summary>
public class InheritFPort : BaseFPort {

	private CallBack callback;

	/// <summary>
	/// 传承端口
	/// </summary>
	/// <param name="mainUid">继承数据的卡片 uid.</param>
	/// <param name="foodUid">被清数据的原始卡片 uid.</param>
	/// <param name="change">是否交换装备.</param>
	/// <param name="evo">是否传承进化等级.</param>
	/// <param name="callback">Callback.</param>
	public void inherit (string mainUid, string foodUid, int change, int evo, CallBack callback)
	{
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.INHERIT);
		message.addValue ("main_uid", new ErlString (mainUid));
		message.addValue ("food_uid", new ErlString (foodUid));
		message.addValue ("type", new ErlInt (change));//1打勾,0不打勾
		message.addValue ("evolution", new ErlInt (evo));//1打勾,0不打勾
		access (message);
	}

	public override void read (ErlKVMessage message)
	{
		base.read (message);

		ErlType str = message.getValue ("msg") as ErlType;

		if(str.getValueString() == "ok")//传承成功，后台推送卡片信息
		{
			if(callback!=null)
				callback();
		}
	}
}
