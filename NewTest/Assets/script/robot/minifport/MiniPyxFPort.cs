using System;
 
/**
 * 圣器端口
 * @author zhoujie
 * */
public class MiniPyxFPort:MiniBaseFPort
{
	private CallBack callback;
	
	public MiniPyxFPort ()
	{
		
	}
	//强化圣器
	public void intensifyPyx (int intensify, CallBack callback)
	{  
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.INTENSIFY_PYX);  
		message.addValue ("intensify", new ErlInt (intensify));
		access (message);
	}
	//获得圣器强化信息
	public void pyxInfo (CallBack callback)
	{  
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.PYX_INFO);
		access (message);
	}

	public void parseKVMsg (ErlKVMessage message)
	{
	}

	public override void read (ErlKVMessage message)
	{ 
//		ErlType msg = message.getValue ("msg") as ErlType;
//		if (msg is ErlArray) {
//
//			ErlArray arr = msg as ErlArray;
//			long exp = StringKit.toLong (arr.Value [0].getValueString ());//圣器经验
//			ErlArray arr2 = arr.Value [1] as ErlArray;
//			int day = StringKit.toInt (arr2.Value[0].getValueString());//日期 天
//			int count = StringKit.toInt (arr2.Value[1].getValueString());//已经使用免费强化次数
//			BeastEvolveManagerment.Instance.setHallowExp(exp);
//			BeastEvolveManagerment.Instance.setHallowDay(day);
//			BeastEvolveManagerment.Instance.setHallowCount(count);
//		} else {
//			MonoBase.print (GetType () + "============error:"+msg);
//		}
		parseKVMsg (message);
		if (callback != null)
			callback ();
	}
	
	
}

