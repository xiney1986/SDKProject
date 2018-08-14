using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MassGetPlayerInfoFPort : BaseFPort {
	
    private CallBack<PvpOppInfo> callback;
	

    public void access (int index,CallBack<PvpOppInfo> callback)
	{
        this.callback = callback;
        ErlKVMessage message = new ErlKVMessage (FrontPort.ARENA_GET_MASS_PLAYER_INFO);
        message.addValue ("index", new ErlString (index.ToString()));
		access (message);
	}
	public void access (int index,int teamNum,int teamType,CallBack<PvpOppInfo> callback)
	{
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.ARENA_GET_MASS_PLAYER_INFO);
		message.addValue ("index", new ErlString (index.ToString()));
		message.addValue("array_id",new ErlInt(teamNum));
		message.addValue("team_type",new ErlInt(10));
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{
		ErlArray list = message.getValue ("msg") as ErlArray;
        int arenaTeam = StringKit.toInt(list.Value[0].getValueString());
        int arenaIntegral = StringKit.toInt(list.Value[1].getValueString());
        int arenaRank = StringKit.toInt(list.Value[2].getValueString());

        list = list.Value [3] as ErlArray;
		PvpOppInfo pvpOppInfo = PvpOppInfo.pares(list);
        pvpOppInfo.arenaIntegral = arenaIntegral;
        pvpOppInfo.arenaRank = arenaRank;
        pvpOppInfo.arenaTeam = arenaTeam;
        if (callback != null)
        {
            callback(pvpOppInfo);
        }
	}
}
