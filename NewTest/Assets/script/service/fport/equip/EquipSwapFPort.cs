using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 交换两张卡牌的装备
 * @author 杨小珑
 * */
public class EquipSwapFPort : BaseFPort
{
	public const int TYPE_EQUIP = 1;//装备
	public const int TYPE_STARSOUL = 2;//星魂
	private CallBack<bool> callback;

	//uid1 -> uid2
	public void access (string uid1,string uid2,string type, CallBack<bool> callback)
	{ 
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.EQUIP_SWAP);
		message.addValue ("card_uid1", new ErlString (uid1));//老的
		message.addValue ("card_uid2", new ErlString (uid2));//新的
		message.addValue ("type", new ErlString (type));//需要交换的类型（装备,星魂,...）
		access (message);
	}

	public override void read (ErlKVMessage message)
	{
		ErlType type = message.getValue ("msg") as ErlType;

		if (callback != null) {
			callback (type.getValueString () == "ok");
		}
	}

}
