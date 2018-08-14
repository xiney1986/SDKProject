using System;
 
/**
 * 兑换信息接口
 * @author longlingquan
 * */
public class MiniExchangeInfoFPort:MiniBaseFPort
{
	CallBack callBack;

	public MiniExchangeInfoFPort ()
	{
	}
	
	public void initInfo (CallBack callBack)
	{
		this.callBack = callBack;
		ErlKVMessage message = new ErlKVMessage (FrontPort.EXCHANGE_INFO);   
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
//			int max = array.Value.Length;
//			for (int i = 0; i < max; i++) {
//				ErlArray array2 = array.Value [i] as ErlArray;
//				int sid = StringKit.toInt (array2.Value [0].getValueString ());
//				int num = StringKit.toInt (array2.Value [1].getValueString ());
//				ExchangeManagerment.Instance.updateExchange (sid, num);
//			}
//			ExchangeManagerment.Instance.countCompleteNum();
//			callBack();
//		} else {
//			MonoBase.print (GetType () + "=====================" + type.getValueString ());
//		}
		parseKVMsg (message);
		if (callBack != null)
			callBack ();
	}
} 

