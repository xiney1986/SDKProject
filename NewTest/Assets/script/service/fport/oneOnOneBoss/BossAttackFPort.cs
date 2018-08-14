using UnityEngine;
using System.Collections;

public class BossAttackFPort : BaseFPort {

    private CallBack callback;

    public void access(int sid, string cardUid,string beastUid ,CallBack callback) {
        this.callback = callback;
        ErlKVMessage message = new ErlKVMessage(FrontPort.ONEONEONE_FIGHT);
        message.addValue("sid", new ErlInt(sid));
        message.addValue("card_uid", new ErlString(cardUid));
        message.addValue("beast_uid", new ErlString(beastUid));
        access(message);
    }

    public override void read(ErlKVMessage message) {
        ErlType type = message.getValue("msg") as ErlType;
        if (type == null) return;
        if (type.getValueString() == "ok") {
            if (callback != null) {
                callback();
                callback = null;
            }
        } else {
            UiManager.Instance.openDialogWindow<MessageLineWindow>((win) => {
				win.Initialize(LanguageConfigManager.Instance.getLanguage("OneOnOneBoss_" + type.getValueString()));
            });
        }
    }
}
