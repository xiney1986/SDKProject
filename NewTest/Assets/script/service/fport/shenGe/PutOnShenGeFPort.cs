using UnityEngine;
using System.Collections;
/* 穿戴替换神格接口 */
public class PutOnShenGeFPort : BaseFPort {

    private CallBack callback;

    public void access(int sid,int index, CallBack callback) {
        this.callback = callback;
        ErlKVMessage message = new ErlKVMessage(FrontPort.SHENGE_PUTON);
        message.addValue("sid",new ErlInt(sid));
        message.addValue("local",new ErlInt(index));
        access(message);
    }

    public override void read(ErlKVMessage message) {
        ErlType type = message.getValue("msg") as ErlType;
        if (type == null) return;
        if (type.getValueString() == "ok")
        {
            if (callback != null)
            {
                callback();
                callback = null;
            }
        }
        else
        {
            UiManager.Instance.openDialogWindow<MessageWindow>((win) =>
            {
                win.initWindow(1,LanguageConfigManager.Instance.getLanguage("s0093"),null,type.getValueString(),null);
            });
        }
    }
}
