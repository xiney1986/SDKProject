using System;
 
public class ErlPingPort:PortHandler
{
	override public void erlReceive(Connect connect,ErlKVMessage message)
	{
		long time=TimeKit.getMillisTime();
		connect.ping=time-connect.PingTime;
		connect.PingTime=0;
	}
} 

