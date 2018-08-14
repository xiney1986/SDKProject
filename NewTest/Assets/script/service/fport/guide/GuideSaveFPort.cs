using System;
 
/**
 * 新手任务保存数据接口
 * @author longlingquan
 * 
 * type 1两种都保存,2只保存步骤(int),3只保存一次性引导
 * */
public class GuideSaveFPort:BaseFPort
{
	public GuideSaveFPort ()
	{ 

	}
	
	public void saveStep (int type, int step)
	{
		ErlKVMessage message = new ErlKVMessage (FrontPort.GUIDE_SET);  
		message.addValue ("type", new ErlInt (type));//新手任务类型
		message.addValue ("step", new ErlInt (step));//保存的步骤
		send (message);
	}
} 

