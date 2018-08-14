using UnityEngine;
using System.Collections;

public class SdkOneKeyShareFport : BaseFPort
{
    public const int TYPE_SEND_SHARE_SUCCESS = 1;//分享成功
    public const int TYPE_GET_SHARE_COUNT = 2;//分享次数
    public const int TYPE_SHARE_DRAW = 1;//分享抽奖传给后台的类型
    private int sendType;
    int type = 0;

    CallBack<int> callback;
    CallBack callBack;
    public void getSdkOneKeyShareCount(CallBack<int> callback)
    {
        sendType = TYPE_GET_SHARE_COUNT;
        this.callback = callback;
        ErlKVMessage msg = new ErlKVMessage(FrontPort.SDK_SHARE_GET_SHARE_COUNT);
        access(msg);
    }

    public void sendShareSuccess()
    {
        sendType = TYPE_SEND_SHARE_SUCCESS;
        ErlKVMessage msg = new ErlKVMessage(FrontPort.SDK_SHARE_SEND_SHARE_SUCCESS);
        access(msg);
    }
    public void sendShareSuccess(CallBack callback) {
        this.callBack = callback;
        sendType = TYPE_SEND_SHARE_SUCCESS;
        ErlKVMessage msg = new ErlKVMessage(FrontPort.SDK_SHARE_SEND_SHARE_SUCCESS);
        access(msg);
    }
    public void sendDrawShareSuccess(int type,CallBack callBack) {
        sendType = TYPE_SEND_SHARE_SUCCESS;
        this.callBack = callBack;
        this.type = type;
        ErlKVMessage msg = new ErlKVMessage(FrontPort.SDK_SHARE_SEND_SHARE_SUCCESS);
        msg.addValue("type", new ErlInt(type));
        access(msg);
    }

    public override void read(ErlKVMessage message)
    {
        if (sendType == TYPE_GET_SHARE_COUNT)
        {
            int count = 10;
            ErlType msg = message.getValue("msg") as ErlType;
            //if (msg is ErlArray)
            //{
                //ErlArray array = msg as ErlArray;
                //count = StringKit.toInt(array.Value[0].getValueString());
                count = StringKit.toInt(msg.getValueString());
                //string state = array.Value[0].getValueString();
                //if (state == "0k")
                //{
                //    count = StringKit.toInt(array.Value[1].getValueString());
                //}
                if (this.callback != null)
                {
                    this.callback(count);
                }
                callback = null;
           // }
        }else if(sendType == TYPE_SEND_SHARE_SUCCESS){
            if (type == TYPE_SHARE_DRAW) {
                if (ShareDrawManagerment.Instance.isFirstShare == 0) {
                    ShareDrawManagerment.Instance.canDrawTimes = ShareDrawManagerment.Instance.canDrawTimes + 3 > 12 ? 12 : ShareDrawManagerment.Instance.canDrawTimes + 3;
                    ShareDrawManagerment.Instance.isFirstShare = 1;
                }
            }
            if (callBack != null) {
                callBack();
                callBack = null;
            }
        }
       
    }
}
