using System;
 
/**
 * 获得新手任务信息接口
 * @author longlingquan
 * */
public class GuideGetInfoFPort:BaseFPort
{
	public GuideGetInfoFPort ()
	{
		
	}
	
	private CallBack callback;
	
	public void getInfo (CallBack callback)
	{
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.GUIDE_GET);   
		access (message);
	}
	 
	public override void read (ErlKVMessage message)
	{
		parseKVMsg (message);
		callback ();
	}
	//解析ErlKVMessgae
	public void parseKVMsg (ErlKVMessage message)
	{
		ErlType type = message.getValue ("msg") as ErlType;
		ErlArray arr = type as ErlArray;
		
		int guideSid = StringKit.toInt (arr.Value [1].getValueString ());
		ErlArray array = arr.Value [0] as ErlArray;
		int[] onceGuideList = null;
		if (array != null && array.Value.Length > 0) {
			int max = array.Value.Length;
			onceGuideList = new int[max];
			for (int i = 0; i < max; i++) {
				onceGuideList [i] = StringKit.toInt (array.Value [i].getValueString ());
			}
		}
		//0是新玩家,1是老玩家
		bool isNew = StringKit.toInt (arr.Value [2].getValueString ()) == 0 ? true : false;
		GuideManager.Instance.init (guideSid, onceGuideList, isNew);
	}
}

