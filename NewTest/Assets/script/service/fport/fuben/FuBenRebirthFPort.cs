using System;
 
/**
 * 副本复活卡片接口
 * @author longlingquan
 * */
public class FuBenRebirthFPort:BaseFPort
{
	public FuBenRebirthFPort ()
	{
		
	}

	private CallBackStrtArray callback;
	private string[] ids;
	
	public void rebirth (string[] ids, CallBackStrtArray callback)
	{
		this.callback = callback;
		this.ids = ids;
		
		ErlKVMessage message = new ErlKVMessage (FrontPort.FUBEN_REBIRTH);  
		message.addValue ("card_uid", new ErlString (getStringByArray (ids)));
		access (message);
	}
	
	private string getStringByArray (string[] ids)
	{
		string str = "";
		for (int i = 0; i < ids.Length; i++) {
			if (i == 0) {
				str += ids [i];
			} else {
				str += "," + ids [i];
			}
		}
		return str;
	}
	
	public override void read (ErlKVMessage message)
	{
		string str = (message.getValue ("msg") as ErlType).getValueString ();
		if (str == FPortGlobal.SYSTEM_OK) {
			callback (ids);
		} else {
			MessageWindow.ShowAlert (str);
			if (callback != null)
				callback = null;
		}
	}
	 
} 

