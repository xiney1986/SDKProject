using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 装备操作接口
 * @author 汤琦
 * */
public class EquipOperateFPort : BaseFPort
{

	private CallAttr callback;
	
	public EquipOperateFPort()
	{
		
	}
	//脱装备，只需要将equipUid为0就可以了
	public void access (string cardUid,int cardSid,string equipUid,int part,CallAttr back)
	{  
		this.callback = back;
		ErlKVMessage message = new ErlKVMessage (FrontPort.EQUIP_OPERATE); 
		message.addValue ("card_uid", new ErlString (cardUid));
		message.addValue ("card_sid", new ErlInt (cardSid));
		message.addValue ("equip_uid", new ErlString (equipUid));
		message.addValue ("part", new ErlInt (part));
		access (message);
	} 
	
	public override void read (ErlKVMessage message)
	{
		string str = (message.getValue ("msg") as ErlAtom).Value; 
		if (str == FPortGlobal.INTENSIFY_SUCCESS) {
			ErlArray ea = message.getValue ("uid") as ErlArray;
			string cuid = ea.Value [0].getValueString ();
			string euid = ea.Value [1].getValueString ();
			string lasteuid = ea.Value [2].getValueString ();
			Card card = StorageManagerment.Instance.getRole (cuid);
			List<AttrChange> attrs = card.operateEquip (euid, lasteuid);
			if (euid != "0") {
				callback (attrs, CardBookWindow.RELOADEQUIP);
			} else {
				callback (attrs, CardBookWindow.UNDRESS);
			} 
		} else {
			MessageWindow.ShowAlert (str);
			if (callback != null)
				callback = null;
		}
//		else if(str == FPortGlobal.EQUIP_NOHAVE)//脱装备时，装备不存在
//		{
//			
//		}
//		else if (str == FPortGlobal.EQUIP_NOEXIST)//装备不存在
//		{
//			
//		}
//		else if(str == FPortGlobal.INTENSIFY_CARD_NONENTITY)//卡片不存在
//		{
//			
//		}
//		else if(str == FPortGlobal.EQUIP_BEUSED)//装备已使用
//		{
//			
//		}
//		else if (str == FPortGlobal.CARD_SID_NOEQUAL)//卡片sid不相等
//		{
//			
//		}
//		else if(str == FPortGlobal.PART_NOT_SUIT)//部位不一致
//		{
//			
//		}
	}
}
