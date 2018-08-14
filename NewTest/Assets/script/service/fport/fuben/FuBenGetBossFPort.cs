using System;
 
public class FuBenGetBossFPort:BaseFPort
{
	public FuBenGetBossFPort ()
	{
	}
	
	public void getBoss (int m_id)
	{ 
		ErlKVMessage message = new ErlKVMessage (FrontPort.FUBEN_GET_BOSS); 
		message.addValue ("fbid", new ErlInt (m_id));
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{
		ErlType type = message.getValue ("msg") as ErlType;
		if (type is ErlArray) { 
			MissionInfoManager.Instance.mission.setBoss (type as ErlArray);
		} else if (type is ErlString) {
			//boss满血
		}
	}
} 

