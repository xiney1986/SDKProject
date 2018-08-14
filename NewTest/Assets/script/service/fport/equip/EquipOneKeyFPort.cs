using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 一键换装接口
 * @author 汤琦
 * */
public class EquipOneKeyFPort : BaseFPort
{
	 
	private CallAttr callback;
	
	public EquipOneKeyFPort ()
	{
		
	}
	//cardUid格式，字符串以逗号分隔
	public void access (string cardUids, CallAttr back)
	{ 
		this.callback = back;
		ErlKVMessage message = new ErlKVMessage (FrontPort.EQUIP_ONEKEY);
		message.addValue ("card_uids", new ErlString (cardUids));
		access (message);
	}

	public override void read (ErlKVMessage message)
	{
		ErlArray arr = message.getValue ("msg") as ErlArray; 
		ErlArray arrs = arr.Value [0] as ErlArray;
		string uid = (arrs.Value [0]).getValueString ();
		ErlArray equipArr = arrs.Value [1] as ErlArray;
		string[] equips = getIntList (equipArr); 
		Card card = StorageManagerment.Instance.getRole (uid);
		List<AttrChange> attrs = card.oneKeyEquip (equips);
		callback (attrs, CardBookWindow.ONEKEYEQUIP);
	}

	private string[] getIntList (ErlArray arr)
	{
		string[] c = new string[arr.Value.Length];
		for (int i = 0; i < c.Length; i++) {
			c [i] = arr.Value [i].getValueString ();
		}
		return c;
	}

}
