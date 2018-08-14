using UnityEngine;
using System;

/**
 * 报名
 * @author yxl
 * */
public class ArenaApplyFPort : BaseFPort
{

  
	public void access ()
	{   
		ErlKVMessage message = new ErlKVMessage (FrontPort.ARENA_APPLY);
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{ 
		//海选报名前台不在乎结果
	}
}
