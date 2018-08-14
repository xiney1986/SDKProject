using System;
 
/**
 * 秘宝锻造接口
 * @auhtor longlingquan
 * */
public class MagicWeaponPhaseFPort:BaseFPort
{
	CallBack<string> callback;
	int exchangeID;
	int exchangeNum;

    public MagicWeaponPhaseFPort()
	{
		
	}
	
	public void exchange (string uid,CallBack<string> callback)
	{
		this.callback = callback;
        ErlKVMessage message = new ErlKVMessage(FrontPort.MAGIC_WEAPON_PHASE);
        message.addValue("artifact_uid", new ErlString(uid));//商品sid
		access (message);
	
	}
	
	public override void read (ErlKVMessage message)
	{
		ErlType type = message.getValue ("msg") as ErlType;
		if (type.getValueString () == FPortGlobal.SYSTEM_OK) {
			callback ("ok");
        } else if (type.getValueString() == "evo_failed") {
            callback("evo_failed");
        }else{
            MessageWindow.ShowAlert(type.getValueString());
            if (callback != null)
                callback = null;
        }
            
        
	}
} 

