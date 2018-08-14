using UnityEngine;
using System.Collections;

public class SevenDaysHappyTouchTaskFPort : BaseFPort
{

	public void SevenDaysHappyTouchTaskAccess()
	{
		ErlKVMessage message = new ErlKVMessage (FrontPort.SEVENDAYSHAPPY_TOUCHTASK);
		access (message);
	}

	public override void read (ErlKVMessage message)
	{

	}
}
