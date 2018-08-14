using System;
 
/**
 * 获得部分角色信息
 * @author zhoujie
 * */
public class FuBenGetPlayersFPort:BaseFPort
{
	private CallBack callback;

	public FuBenGetPlayersFPort ()
	{
		
	}

	public void get_players (CallBack callback)
	{
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.FUBEN_GET_PLAYERS);
		access (message);
	}

	public override void read (ErlKVMessage message)
	{ 
		ErlArray arr = message.getValue ("msg") as ErlArray;
		int j;
		MissionNpcManagerment.Instance.clearNpc ();
		for (int i=arr.Value.Length-1; i >= 0; i--) {
			ErlArray ea = arr.Value [i] as ErlArray;
			NpcData npcData = new NpcData ();
			j = 0;
			npcData.uid = ea.Value [j++].getValueString ();
			npcData.name = ea.Value [j++].getValueString ();
			npcData.level = StringKit.toInt (ea.Value [j++].getValueString ());
			npcData.vipLevel = StringKit.toInt (ea.Value [j++].getValueString ());
			npcData.style = ea.Value [j++].getValueString ();
			ErlArray titleArray = ea.Value [j++] as ErlArray;
			npcData.mountsSid = StringKit.toInt (ea.Value [j++].getValueString ());
			ErlArray titleItem;
			for (int k=0,length=titleArray.Value.Length; k<length; k++) {
				titleItem = titleArray.Value [k] as ErlArray;
				string name = titleItem.Value [0].getValueString ();
				switch (name) {
				case "prestige":
					npcData.titleSid = StringKit.toInt (titleItem.Value [1].getValueString ());
					break;
				case "ladder_rank":
					npcData.medalSid = StringKit.toInt (titleItem.Value [1].getValueString ());
					break;
				}
			}

			MissionNpcManagerment.Instance.addNpc (npcData);
		}
		if (MissionNpcManagerment.Instance.npcList == null || MissionNpcManagerment.Instance.npcList.Count == 0)
			return;
		else
			callback ();

	}
} 

