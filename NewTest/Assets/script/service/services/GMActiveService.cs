using UnityEngine;
using System.Collections;

/**
 * gm修改活动
 * @author huangzhenghan
 * */
public class GMActiveService:BaseFPort
{

	public GMActiveService ()
	{
		
	}

	public override void read (ErlKVMessage message)
	{ 
		ErlArray arr = message.getValue ("msg") as ErlArray;
		if (arr != null)
			NoticeActiveManagerment.Instance.parseGMDetailOneInfo (arr);
	}
}

