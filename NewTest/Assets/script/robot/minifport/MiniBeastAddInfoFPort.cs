using System;
 
/**
 * 召唤兽数量 进化信息接口(共鸣之力)
 * @author longlingquan
 * */
public class MiniBeastAddInfoFPort:MiniBaseFPort
{
	private CallBack callback;
	
	public MiniBeastAddInfoFPort ()
	{
		
	}

	public void getInfo (CallBack callback)
	{  
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.BEAST_ADD);  
		access (message);
	}

	public void parseKVMsg (ErlKVMessage message)
	{
	}

	public override void read (ErlKVMessage message)
	{ 
//		ErlArray arr = message.getValue ("msg") as ErlArray;
//		if (arr != null) {
//			int num = StringKit.toInt (arr.Value [0].getValueString ());//当前拥有召唤兽数量
//			int evolveNum = StringKit.toInt (arr.Value [1].getValueString ());//当前召唤兽进化总次数	 
//			BeastEvolveManagerment.Instance.updateNum (num, evolveNum);
//			callback();
//		} else {
//			MonoBase.print (GetType () + "============error");
//		}
		parseKVMsg (message);
		if (callback != null)
			callback ();
	}
	
	
}

