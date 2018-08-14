using System;
 
/**
 * 初始化抽奖信息接口
 * @author longlingquan
 * */
public class InitLuckyDrawFPort:BaseFPort
{
	private CallBack callback;
	
	public InitLuckyDrawFPort ()
	{
	}
 
	public void init (CallBack callback)
	{
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.LUCKY_GET_INFO);   
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{
		parseKVMsg (message);
		if (callback != null)
			callback ();
	}
	//解析ErlKVMessgae
	public void parseKVMsg (ErlKVMessage message)
	{
		ErlArray array = message.getValue ("msg") as ErlArray;
		if (array == null)
			return;
		LuckyDrawManagerment.Instance.updateAllLuckyDraw (array);
	}
} 

