using System;
 
/**
 * 幸运星活动
 * */
public class FuBenStarMultipleFPort:BaseFPort
{
	private CallBack callback;
	
	public void getStarMultiple (CallBack callback)
	{
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.STAR_MULTIPLE);
		message.addValue ("sid", new ErlInt (7));//这个活动写死
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{ 
		int multiple = StringKit.toInt ((message.getValue ("msg") as ErlType).getValueString ()); //倍数
		int timeId = StringKit.toInt ((message.getValue ("time") as ErlType).getValueString ()); //倍数
		FuBenManagerment.Instance.setStarMultiple (timeId, multiple);
	}
} 

