using System;
using System.Collections.Generic;

/**
 * 卡牌训练
 * @author huangzhenghan
 * */
public class HappySundayInit : BaseFPort
{
    CallBack<int, List<int>> callback;


    public void access(CallBack<int, List<int>> callback)
    {
        this.callback = callback;
        ErlKVMessage message = new ErlKVMessage(FrontPort.HAPPYSUNDAY_INIT);
        access(message);
    }


    public override void read(ErlKVMessage message)
    {
        ErlArray list = message.getValue("msg") as ErlArray;
        int score = StringKit.toInt(list.Value[0].getValueString());

        ErlArray receiveSidErlArr = list.Value[1] as ErlArray;
        List<int> receiveSid = new List<int>(receiveSidErlArr.Value.Length);
        for (int i = 0; i < receiveSidErlArr.Value.Length; i++)
        {
            receiveSid.Add(StringKit.toInt(receiveSidErlArr.Value[i].getValueString()));
        }
        callback(score, receiveSid);
    }


}

