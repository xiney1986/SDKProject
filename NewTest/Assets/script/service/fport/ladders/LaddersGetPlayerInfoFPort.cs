using System.Collections.Generic;
using System;

/// <summary>
/// 天梯玩家数据的请求
/// </summary>
public class LaddersGetPlayerInfoFPort : BaseFPort
{

    private CallBack<PvpOppInfo> readDataCallback;
    PvpOppInfo pvpOppInfo;
    public void access(string _role_uid, int teamType, CallBack<PvpOppInfo> _readDataCallback)
    {
        readDataCallback = _readDataCallback;
        ErlKVMessage message = new ErlKVMessage(FrontPort.CHAT_PLAYERINFO);
        message.addValue("role_uid", new ErlString(_role_uid));
        message.addValue("array_id", new ErlInt(3));
        message.addValue("team_type", new ErlInt(teamType));
        access(message);
    }

    public override void read(ErlKVMessage message)
    {
        ErlArray list = message.getValue("msg") as ErlArray;
        pvpOppInfo = PvpOppInfo.pares(list);
        CardBookWindow.setChatPlayerUid(pvpOppInfo.uid);
        getSdkInfoBack(null);



    }

    void getSdkInfoBack(Dictionary<string, PlatFormUserInfo> dic)
    {
        if (dic != null && dic.Count > 0)
            pvpOppInfo.sdkInfo = dic[pvpOppInfo.uid];

        if (readDataCallback != null)
        {
            readDataCallback(pvpOppInfo);
            readDataCallback = null;
        }

    }
}



