using UnityEngine;
using System.Collections;

/**
 * 女神星盘服务
 * */
public class GoddessAstrolabeService : BaseFPort {

	public GoddessAstrolabeService ()
	{
		
	}
	
	public override void read (ErlKVMessage message)
	{
		GoddessAstrolabeManagerment.Instance.updateInfoByServer(message);
	}
}
