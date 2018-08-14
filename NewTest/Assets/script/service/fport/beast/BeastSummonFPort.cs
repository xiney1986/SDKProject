using System;
 
/**
 * 召唤兽召唤
 * @author longlingquan
 * */
public class BeastSummonFPort:BaseFPort
{
	private CallBack callback;

	public BeastSummonFPort ()
	{
		
	}
	
	public void access (int exchangeId, CallBack callback)
	{  
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.BEAST_SUMMON); 
		message.addValue ("sid", new ErlInt (exchangeId)); 
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{ 
		string info = (message.getValue ("msg") as ErlType).getValueString (); 
		if (info == FPortGlobal.SYSTEM_OK) {
			GuideManager.Instance.doGuide(); 
			callback ();
		} else {
			MonoBase.print (GetType () + "==============================error! info=" + info);
		}
	}
} 

