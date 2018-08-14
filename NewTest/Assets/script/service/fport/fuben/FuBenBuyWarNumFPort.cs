using System;
 
/**
 * 购买讨伐副本挑战次数
 * @author longlingquan
 * */
public class FuBenBuyWarNumFPort:BaseFPort
{
	private CallBack<int> callback;
	private int buyCount;

	public FuBenBuyWarNumFPort ()
	{
		
	}
	
	//讨伐次数只能一次一次购买
	public void buyNum (CallBack<int> callback,int count)
	{
		this.callback = callback;
		this.buyCount = count;
		ErlKVMessage message = new ErlKVMessage (FrontPort.FUBEN_BUY_WAR_NUM);
		message.addValue ("crusade_type", new ErlInt (501));
		message.addValue ("number", new ErlInt (count));
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{ 
		
		string str = (message.getValue ("msg") as ErlType).getValueString ();
		if (str == FPortGlobal.SYSTEM_OK)
		{
			callback (buyCount);
		} 
		else
		{
			callback (0);
		}
	}
} 

