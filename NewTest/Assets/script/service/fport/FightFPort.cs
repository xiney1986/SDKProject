using System;
using UnityEngine;
 
public class FightFPort:BaseFPort
{
	public FightFPort ()
	{
		
	}
	
	public void send ( )
	{
		ErlKVMessage message = new ErlKVMessage (FrontPort.FIGHTGM);  
	//	message.addValue ("sid", new ErlInt (fight));
		send (message);		
	}
	 
} 

