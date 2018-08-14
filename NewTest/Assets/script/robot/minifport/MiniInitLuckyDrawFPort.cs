using System;
 
/**
 * 初始化抽奖信息接口
 * @author longlingquan
 * */
public class MiniInitLuckyDrawFPort:MiniBaseFPort
{
	private CallBack callback;
	
	public MiniInitLuckyDrawFPort ()
	{
	}
 
	public void init (CallBack callback)
	{
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.LUCKY_GET_INFO);   
		access (message);
	}

	public void parseKVMsg (ErlKVMessage message)
	{
	}

	public override void read (ErlKVMessage message)
	{
//		ErlArray array = message.getValue ("msg") as ErlArray;
//		if (array == null)
//			return ;
//		LuckyDrawManagerment.Instance.updateAllLuckyDraw (array);
		parseKVMsg (message);
		if (callback != null)
			callback ();
	}
} 

