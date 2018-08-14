using UnityEngine;
using System.Collections;

public class ChatGetPlayerLevelFPort : BaseFPort
{
    private CallBack<PvpOppInfo> readDataCallback2;
    public PvpOppInfo pvpOppInfo;

    public void access(string _role_uid, int _teamType, CallBack<PvpOppInfo> readDataCallback2, int comefrom)
    {
        this.readDataCallback2 = readDataCallback2;
        ErlKVMessage message = new ErlKVMessage(FrontPort.CHAT_PLAYERINFO);
        message.addValue("role_uid", new ErlString(_role_uid));
        message.addValue("team_type", new ErlInt(_teamType));
        PvpPlayerWindow.comeFrom = comefrom;
        access(message);
    }
    public override void read(ErlKVMessage message)
    {
        ErlArray list = message.getValue("msg") as ErlArray;
        pvpOppInfo = PvpOppInfo.pares(list);
        if (readDataCallback2 != null)
        {
            readDataCallback2(pvpOppInfo);
            readDataCallback2 = null;
        }
    }
}
