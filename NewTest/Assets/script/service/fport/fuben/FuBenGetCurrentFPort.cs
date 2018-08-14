using System;
 
/**
 * 获取当前副本信息(进行到一半的副本)
 * @author longlingquan
 * */
public class FuBenGetCurrentFPort:BaseFPort
{
	public CallBack<bool> callback;
	bool isSaveMission;

	public FuBenGetCurrentFPort ()
	{
		
	}

	public void getInfo (CallBack<bool> callback,bool isSaveMission)
	{
		this.callback = callback;
		this.isSaveMission = isSaveMission;
		ErlKVMessage message = new ErlKVMessage (FrontPort.FUBEN_GET_CURRENT); 
		access (message);
	}
	
	public void getInfo (CallBack<bool> callback)
	{
		this.callback = callback;
		this.isSaveMission = true;
		ErlKVMessage message = new ErlKVMessage (FrontPort.FUBEN_GET_CURRENT); 
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{
		ErlType type = message.getValue ("sid") as ErlType;
		ErlType level = message.getValue ("lv") as ErlType;
		int sid = StringKit.toInt (type.getValueString ());
		if (sid == 0) {
			callback (false);
			return;
		} 
		int starLevel = StringKit.toInt (level.getValueString ());
		if (isSaveMission) {
			MissionInfoManager.Instance.saveMission (sid,starLevel);
		}
		callback (true);
	}
} 

