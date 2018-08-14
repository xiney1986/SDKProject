using UnityEngine;
using System.Collections;
/// <summary>
/// 节日送领取端口
/// </summary>
public class HolidayAwardButtonFPort : BaseFPort {

	private CallBack<int> callback;
	private int sid;
	public HolidayAwardButtonFPort(){
	}
	public void getPrize (int laid,int day,CallBack<int> callback)
	{   
		sid=day;
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.HOLIDAY_AWARD_BUTTON);  
		message.addValue ("sid", new ErlInt (laid));
		message.addValue("day",new ErlInt(day));
		access (message);
	}
	//解析ErlKVMessgae
	public override void read (ErlKVMessage message)
	{
		ErlType type = message.getValue ("msg") as ErlType;
		string msg = type.getValueString ();
		if(msg=="ok"){
			TotalLoginManagerment.Instance.addReceivedAwardHoliday(sid);
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
