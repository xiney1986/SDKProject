using System;
 
/**
 * 副本信息接口
 * 获得指定类型副本信息
 * @author longlingquan
 * */
public class MiniFuBenInfoFPort:MiniBaseFPort
{
	private const string NONE = "none";//没有过往副本记录 
	private const string INFO = "fbinfo";//有过往副本记录
	private const string KEY_INFO = "info";//副本信息 键
	private CallBack callback;

	public MiniFuBenInfoFPort ()
	{
		
	}
	 
	public void info (CallBack callback, int chapterType)
	{
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.FUBEN_INFO);   
		message.addValue ("type", new ErlInt (chapterType));
		access (message);
	}

	public void parseKVMsg (ErlKVMessage message)
	{
	}
	
	public override void read (ErlKVMessage message)
	{
		parseKVMsg (message);
		if (callback != null)
			callback ();
	} 

} 

