using System;
 
/**
 * 获得新手任务信息接口
 * @author longlingquan
 * */
public class MiniGuideGetInfoFPort:MiniBaseFPort
{
	public MiniGuideGetInfoFPort ()
	{
		
	}
	
	private CallBack callback;
	
	public void getInfo (CallBack callback)
	{
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.GUIDE_GET);   
		access (message);
	}
	 
	public void parseKVMsg (ErlKVMessage message)
	{
	}

	public override void read (ErlKVMessage message)
	{
//		ErlType type = message.getValue ("msg") as ErlType;
//		ErlArray arr = type as ErlArray;
//		
//		int guideSid = StringKit.toInt (arr.Value [1].getValueString ());
//		ErlArray array = arr.Value [0] as ErlArray;
//		int[] onceGuideList = null;
//		if (array != null && array.Value.Length > 0) {
//			int max = array.Value.Length;
//			onceGuideList = new int[max];
//			for (int i = 0; i < max; i++) {
//				onceGuideList [i] = StringKit.toInt (array.Value [i].getValueString ());
//			}	
//		}
//		GuideManager.Instance.init(guideSid,onceGuideList);
		parseKVMsg (message);
		if (callback != null)
			callback ();
	}
}

