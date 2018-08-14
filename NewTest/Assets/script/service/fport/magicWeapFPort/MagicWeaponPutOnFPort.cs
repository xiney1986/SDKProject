using System;
 
/**
 * 秘宝锻造接口
 * @auhtor longlingquan
 * */
public class MagicWeaponPutOnFPort:BaseFPort
{
	CallBack callback;
	int exchangeID;
	int exchangeNum;

    public MagicWeaponPutOnFPort()
	{
		
	}
	
	public void exchange (string magicWeaponuid,string cardUid,CallBack callback)
	{
		this.callback = callback;
        ErlKVMessage message = new ErlKVMessage(FrontPort.MAGIC_WEAPON_PUTON);
        message.addValue("artifact_uid", new ErlString(magicWeaponuid));//秘宝uid
        message.addValue("card_uid", new ErlString(cardUid));
		access (message);
	
	}
	
	public override void read (ErlKVMessage message)
	{
		ErlType type = message.getValue ("msg") as ErlType;
		if (type.getValueString () == FPortGlobal.SYSTEM_OK) {
			callback ();
        } else {
            MessageWindow.ShowAlert(type.getValueString());
            if (callback != null)
                callback = null;
        }
	}
} 

