using System;
 
/**
 * 累积登陆奖励信息接口
 * @author longlingquan
 * */
public class MiniTotalLoginFPort:MiniBaseFPort
{
	private CallBack callback;
	
	public MiniTotalLoginFPort ()
	{
		
	}
	
	public void access (CallBack callback)
	{   
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.TOTAL_LOGIN_INFO);
		access (message);
	}
	
	public void getinfo ()
	{
		ErlKVMessage message = new ErlKVMessage (FrontPort.TOTAL_LOGIN_INFO);   
		access (message);	
	}

	public void parseKVMsg (ErlKVMessage message)
	{

	}

	public override void read (ErlKVMessage message)
	{
//		ErlType type = message.getValue ("msg") as ErlType;
//		if (type is ErlArray) {
//			ErlArray array = type as ErlArray;
//			int total = StringKit.toInt (array.Value [0].getValueString ());
//			int now = StringKit.toInt (array.Value [1].getValueString ());
//			TotalLoginManagerment.Instance.update (total, now);
//			callback();
//		} else {
//			MonoBase.print (GetType () + "=============" + type.getValueString ());
//		}
		parseKVMsg (message);
		callback ();
	}
} 

