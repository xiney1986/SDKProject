using System;
 
/**
 * 累积奖励 领奖
 * @author longlingquan
 * */
public class TotalLoginPrizesFPort:BaseFPort
{
	CallBack callback;
	bool isnew=false;

	public TotalLoginPrizesFPort ()
	{
		
	}
	
	public void getPrize (int laid, CallBack callback,bool isNew)
	{
		this.isnew=isNew;
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.TOTAL_LOGIN_GET);  
		message.addValue ("laid", new ErlInt (laid));
		access (message);	
	}
	
	public override void read (ErlKVMessage message)
	{
		ErlType type = message.getValue ("msg") as ErlType;
		string msg = type.getValueString ();
		if(msg!=null)
		{
			int returnValue = int.Parse (msg);
			if(returnValue>0)
			{
				if(!isnew){
						TotalLoginManagerment.Instance.addReceivedAwardId(returnValue);
					}else{
					TotalLoginManagerment.Instance.addNewrAwardId(returnValue);
				}
				if (callback != null)
				{
					callback();
					callback = null;
				}
			}
		}
	}
} 

