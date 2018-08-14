using UnityEngine;
using System.Collections;
/// <summary>
/// 周末送领取端口
/// </summary>
public class WeeklyAwardButtonFPort : BaseFPort {

	private CallBack<int> callback;
	private int sid;
	public WeeklyAwardButtonFPort(){
	}
	public void getPrize (int laid,CallBack<int> callback)
	{   
		sid=laid;
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.WEEKLY_AWARD_BUTTON);  
		message.addValue ("sid", new ErlInt (laid));
		access (message);
	}
	public override void read (ErlKVMessage message)
	{
		ErlType type = message.getValue ("msg") as ErlType;
		string msg = type.getValueString ();
		if(msg=="ok"){
			TotalLoginManagerment.Instance.addReceivedAwardWeek(sid);
			if (callback != null)
			{
				callback(1);
				callback = null;
			}
		}else if(msg=="aready_award"){
			if (callback != null)
			{
				callback(2);
				callback = null;
			}
		}else{
			if (callback != null)
			{
				callback(3);
				callback = null;
			}
		}
	}	
	
}
