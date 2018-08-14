using System;
 
/**
 * 召唤兽数量 进化信息接口(共鸣之力)
 * @author longlingquan
 * */
public class BeastAddInfoFPort:BaseFPort
{
	private CallBack callback;
	
	public BeastAddInfoFPort ()
	{
		
	}

	public void getInfo (CallBack callback)
	{  
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.BEAST_ADD);  
		access (message);
	}

	public override void read (ErlKVMessage message)
	{ 
		if (parseKVMsg (message))
			callback ();
	}
	//解析ErlKVMessgae
	public bool parseKVMsg (ErlKVMessage message)
	{
		ErlArray arr = message.getValue ("msg") as ErlArray;
		if (arr != null) {
			int num = StringKit.toInt (arr.Value [0].getValueString ());//当前拥有召唤兽数量
			int evolveNum = StringKit.toInt (arr.Value [1].getValueString ());//当前召唤兽进化总次数	 
			BeastEvolveManagerment.Instance.updateNum (num, evolveNum);
			return true;
		} else {
			MonoBase.print (GetType () + "=error");
		}
		return false;
	}
	
}

