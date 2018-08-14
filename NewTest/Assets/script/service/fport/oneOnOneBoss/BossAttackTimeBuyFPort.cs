using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//购买恶魔挑战次数
public class BossAttackTimeFPort : BaseFPort {
    private CallBack callback;

    public void access(int count,CallBack callback) {
        this.callback = callback;
        ErlKVMessage message = new ErlKVMessage(FrontPort.BOSSATTACKTIME_BUY);
        message.addValue("count", new ErlInt(count));
        access(message);
    }

    public override void read(ErlKVMessage message) {
        ErlType msgType = message.getValue("msg") as ErlType;
        if (msgType != null && msgType is ErlArray)
        {
            ErlArray buyArray = msgType as ErlArray;
            if (buyArray.Value[0].getValueString() == "ok")
            {
                StorageManagerment.Instance.parseAddStorageProps(buyArray.Value[1] as ErlArray);
                if (callback != null)
                    callback();
            }else
                UiManager.Instance.openDialogWindow<MessageLineWindow>((win) => {
                    win.Initialize(LanguageConfigManager.Instance.getLanguage("OneOnOneBoss_425"));
                });
        } else {
            UiManager.Instance.openDialogWindow<MessageLineWindow>((win) => {
                win.Initialize(LanguageConfigManager.Instance.getLanguage("OneOnOneBoss_425"));
            });
        }
        callback = null;
    }
}
