using UnityEngine;
using System.Collections;

public class DivineSendFPort : BaseFPort
{
    CallBack<int, int, int> callback;

    //返回占卜后运势
    public void access(CallBack<int, int, int> callback)
    {
        access(0, callback);
    }
    //返回占卜后运势
    public void access(int type, CallBack<int, int, int> callback)
    {
        //type>0说明是分享并占卜
        this.callback = callback;
        ErlKVMessage message = new ErlKVMessage(FrontPort.DIVINE_SEND);
        message.addValue("type", new ErlInt(type));
        access(message);
    }
    public override void read(ErlKVMessage message)
    {
        ErlArray arr = message.getValue("msg") as ErlArray;
        if (arr != null)
        {
            ErlArray arr2 = arr.Value[0] as ErlArray;
			// 增加之后的运势
            int num = StringKit.toInt(arr2.Value[0].getValueString());
			// 非分享增加的运势
            int num1 = StringKit.toInt(arr2.Value[1].getValueString());
			// 分享增加的运势
            int shareAward = StringKit.toInt(arr2.Value[2].getValueString());
			// 基础奖励(不管任何情况都奖励)
            int awardSid = StringKit.toInt(arr.Value[1].getValueString());
            callback(num1, awardSid, shareAward);
        }
        else if ((message.getValue("msg") as ErlType).getValueString() == "limit")
        {
            UiManager.Instance.getWindow<DivineWindow>().finishWindow();
        }
        else
        {
            MessageWindow.ShowAlert((message.getValue("msg") as ErlType).getValueString());
            if (callback != null)
                callback = null;
        }
    }
}
