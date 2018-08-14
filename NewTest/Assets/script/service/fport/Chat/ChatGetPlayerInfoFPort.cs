using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChatGetPlayerInfoFPort : BaseFPort
{
    private CallBack closeWindowCallback;
    private CallBack readDataCallback;
    private CallBack<bool> reLoadCallback;
    private PvpOppInfo pvpOppInfo;

    private int teamType;

    public void access(string _role_uid, int _teamType, CallBack readDataCallback, CallBack closeWindowCallback, int comefrom)
    {
        this.readDataCallback = readDataCallback;
        this.closeWindowCallback = closeWindowCallback;
        this.reLoadCallback = reLoadCallback;
        ErlKVMessage message = new ErlKVMessage(FrontPort.CHAT_PLAYERINFO);
        message.addValue("role_uid", new ErlString(_role_uid));
        message.addValue("team_type", new ErlInt(_teamType));//请求的是队伍的5人阵法 还是10人阵 5 表示5人 10表示10人
        PvpPlayerWindow.comeFrom = comefrom;
        teamType = _teamType;
        access(message);
    }
    public void access(string _role_uid, CallBack readDataCallback, CallBack closeWindowCallback, int comefrom)
    {
        access(_role_uid, 5, readDataCallback, closeWindowCallback, comefrom);
    }

    public void access(string _role_uid, int _teamType, CallBack<bool> reLoadCallback, int comefrom)
    {
        this.reLoadCallback = reLoadCallback;
        ErlKVMessage message = new ErlKVMessage(FrontPort.CHAT_PLAYERINFO);
        message.addValue("role_uid", new ErlString(_role_uid));
        message.addValue("team_type", new ErlInt(_teamType));//请求的是队伍的5人阵法 还是10人阵 5 表示5人 10表示10人
        PvpPlayerWindow.comeFrom = comefrom;
        teamType = _teamType;
        access(message);
    }
    public void access(string _role_uid, int teamNum, int _teamType, CallBack<bool> reLoadCallback, int comefrom)
    {
        this.reLoadCallback = reLoadCallback;
        ErlKVMessage message = new ErlKVMessage(FrontPort.CHAT_PLAYERINFO);
        message.addValue("role_uid", new ErlString(_role_uid));
        message.addValue("array_id", new ErlInt(teamNum));
        message.addValue("team_type", new ErlInt(_teamType));//请求的是队伍的5人阵法 还是10人阵 5 表示5人 10表示10人
        PvpPlayerWindow.comeFrom = comefrom;
        teamType = _teamType;
        access(message);
    }
    public void access(string _role_uid, CallBack<bool> reLoadCallback, int comefrom)
    {
        access(_role_uid, 5, reLoadCallback, comefrom);
    }


    public override void read(ErlKVMessage message)
    {
        ErlArray list = message.getValue("msg") as ErlArray;

        pvpOppInfo = PvpOppInfo.pares(list);

        if (readDataCallback != null)
        {
            readDataCallback();
            readDataCallback = null;
        }
        openWindow(null);


    }

    void openWindow(Dictionary<string, PlatFormUserInfo> dic)
    {
        if (dic != null)
        {
            if (dic.ContainsKey(pvpOppInfo.uid))
                pvpOppInfo.sdkInfo = dic[pvpOppInfo.uid];
        }
        CardBookWindow.setChatPlayerUid(pvpOppInfo.uid);
        UiManager.Instance.openWindow<PvpPlayerWindow>(
            (win) =>
            {
                win.teamType = teamType;
                win.initInfo(pvpOppInfo, reLoadCallback);
            });


    }

}
