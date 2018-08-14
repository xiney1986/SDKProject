using UnityEngine;
using System;

/**
 * 修改队伍接口
 * @author 张海山
 * */
public class ArmyUpdateFPort : BaseFPort
{
  
	private CallBack callback;
	
	public void access (Army[] armys, CallBack callback)
	{
		this.callback = callback; 
		ErlKVMessage message = new ErlKVMessage (FrontPort.ARMY_UPDATE);  
		message.addValue ("army", new ErlString (createInfos (armys)));//需要更的队伍号
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{ 
		string str = (message.getValue ("msg") as ErlAtom).Value;  
		if (str == FPortGlobal.ARMY_SUCCESS) {
			if (callback != null) {
				callback ();
				callback = null;
			}
		} else {
			MessageWindow.ShowAlert (str);
			if (callback != null)
				callback = null;
		}
	}
	
	private string createInfos (Army[] armys)
	{
		string str = "";
		int max = armys.Length;
		for (int i = 0; i < max; i++) {
			str += createInfo (armys [i]);
			if (i != max - 1)
				str += ';';
		} 
		return str;
	}
	
	//1|1|1|1,2,3,4,5|6,7,8,9,10
	private string createInfo (Army army)
	{
		string str = army.armyid + "" + '|';
		str += army.formationID + "" + '|';
		str += army.beastid + "" + '|';
		str += army.getPlayersToString (army.players) + '|';
		str += army.getPlayersToString (army.alternate);
		return str;
	} 
 
	
}
