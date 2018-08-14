using UnityEngine;
using System.Collections;

public class HolidayAwardFPort : BaseFPort {
	private CallBack callback;
	public HolidayAwardFPort(){
	}
	public void access (int sid,CallBack callback)
	{   
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.HOLIDAY_AWARD_LIST);
		message.addValue ("sid", new ErlInt (sid));
		access (message);
	}
	public override void read (ErlKVMessage message)
	{
		parseKVMsg (message);
		callback ();
	}
	//解析ErlKVMessgae
	public bool parseKVMsg (ErlKVMessage message)
	{
		ErlType type = message.getValue ("msg") as ErlType;
		if (type is ErlArray) {
			ErlArray stateArray=type as ErlArray;
			string state =stateArray.Value[0].getValueString();
			if(state=="ok"){
				TotalLoginManagerment.Instance.HolidayState=true;
				TotalLoginManagerment.Instance.updateHolidasyAwardData(stateArray.Value[1] as ErlArray);
			}else if(state=="close"){
				TotalLoginManagerment.Instance.HolidayState=false;
			}
			return true;
		} else {
			MonoBase.print (GetType () + "==" + type.getValueString ());
		}
		return false;
	}
}
