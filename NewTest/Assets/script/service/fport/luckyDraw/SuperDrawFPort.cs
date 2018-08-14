using System;
 
public class SuperDrawFPort:BaseFPort
{
	private CallBack<int,int,int> callback;

	public SuperDrawFPort ()
	{
		
	}

	public void access (int sid, CallBack<int,int,int> callback)
	{
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.DRAW_SUPERDRAW); 
		message.addValue ("sid", new ErlInt (sid));//抽奖条目id
		access (message);
	}

	public override void read (ErlKVMessage message)
	{
		ErlType type = message.getValue ("msg") as ErlType;
		if (type is ErlArray) {
		
			ErlArray arr = type as ErlArray;
			int index = 0;
			int checkedPoint = StringKit.toInt(arr.Value[index++].getValueString());
			int prizeSid     = StringKit.toInt(arr.Value[index++].getValueString());
			int prizeNum     = StringKit.toInt(arr.Value[index++].getValueString());

			if (callback != null)
				callback (checkedPoint,prizeSid,prizeNum);
		} else {
			MessageWindow.ShowAlert((message.getValue("msg") as ErlString).getValueString());
			MaskWindow.UnlockUI ();
			if (callback != null)
				callback = null;
		}
	} 
} 

