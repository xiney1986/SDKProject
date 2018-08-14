using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class GetShenGeInfoFPort : BaseFPort {

    private CallBack callback;

    public void access(CallBack callback) {
        this.callback = callback;
        ErlKVMessage message = new ErlKVMessage(FrontPort.GET_SHENGE_INFO);
        access(message);
    }

    public override void read(ErlKVMessage message) {
        ErlType type = message.getValue("msg") as ErlType;
        ShenGeManager.Instance.sidList = new List<ShenGeCaoInfo>();
        if (type == null)
        {
            if (callback != null) {
                callback();
                callback = null;
            } else {
                MaskWindow.UnlockUI();
            }
            return;
        }
        if (type is ErlArray) {
            ErlArray erlArray = message.getValue("msg") as ErlArray;
            if (erlArray.Value.Length == 0)
            {
                if (callback != null) {
                    callback();
                    callback = null;
                } else {
                    MaskWindow.UnlockUI();
                }
                return;
            }
            for (int i = 0; i < erlArray.Value.Length; i++)
            {
                ErlArray array = erlArray.Value[i] as ErlArray;
                ShenGeCaoInfo info = new ShenGeCaoInfo();
                info.index = StringKit.toInt(array.Value[0].getValueString());
                info.sid = StringKit.toInt(array.Value[1].getValueString());
                ShenGeManager.Instance.sidList.Add(info);
            }
        }
        if (callback != null)
        {
            callback();
            callback = null;
        }
        else
        {
            MaskWindow.UnlockUI();
        }
    }
}
