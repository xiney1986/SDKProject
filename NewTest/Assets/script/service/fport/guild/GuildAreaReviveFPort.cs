using UnityEngine;
using System.Collections;

/// <summary>
/// 复活端口
/// </summary>
public class GuildAreaReviveFPort : BaseFPort
{
    private CallBack callBack;
    public void access(CallBack  callBack) {
        this.callBack = callBack;
        ErlKVMessage msg = new ErlKVMessage(FrontPort.GUILDWAR_REVIVE);
        access(msg);
    }
    public override void read(ErlKVMessage message)
    {
        /** 复活成功 */
        if ((message.getValue("msg") as ErlType).getValueString() == "ok") {
            if (callBack != null)
            {
                callBack();
                callBack = null;
            }
        }
    }
}
