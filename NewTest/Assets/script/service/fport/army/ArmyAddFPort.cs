using UnityEngine;
using System;

/**
 * 添加队伍接口
 * @author 张海山
 * */
public class ArmyAddFPort : BaseFPort
{
  
	public void access (Army army)
	{   
		ErlKVMessage message = new ErlKVMessage (FrontPort.ARMY_ADD);   
		message.addValue ("index", new ErlInt (army.formationID));//新队伍需要匹配的阵型
		message.addValue ("beastid", new ErlString (army.beastid));//召唤兽id
		message.addValue ("array_lead", new ErlString (army.getPlayersToString (army.players)));//队伍主力卡片的id
		message.addValue ("array_alternate", new ErlString (army.getPlayersToString (army.alternate)));//队伍替补卡片的id
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{ 
		string str = (message.getValue ("msg") as ErlAtom).Value; 
	}
}
