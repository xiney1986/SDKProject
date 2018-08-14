using System;
 
/**
 * 秘宝强化接口
 * @auhtor longlingquan
 * */
public class MagicWeaponStrengFPort:BaseFPort
{
	CallBack callback;
	int exchangeID;
	int exchangeNum;

    public MagicWeaponStrengFPort()
	{
		
	}
	
	public void exchange (string uid,CallBack callback)
	{
		this.callback = callback;
        ErlKVMessage message = new ErlKVMessage(FrontPort.MAGIC_WEAPON_STRENG);
        message.addValue("artifact_uid", new ErlString(uid));//商品sid
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

