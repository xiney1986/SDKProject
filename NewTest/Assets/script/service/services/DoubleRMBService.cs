using UnityEngine;
using System.Collections;


public class DoubleRMBService : BaseFPort
{

    public DoubleRMBService()
    {
    }

    public override void read(ErlKVMessage message)
    {
        //ErlArray list = message.getValue("msg") as ErlArray;
        //int rmb = StringKit.toInt(list.Value[1].getValueString());
        int rmb = StringKit.toInt((message.Value[0] as ErlType).getValueString());
        
        UiManager.Instance.openDialogWindow<MessageLineWindow>((win) => {
            win.Initialize(LanguageConfigManager.Instance.getLanguage("doubleRMB_05") + rmb);
        });

        DoubleRMBManagement.Instance.IsRecharge = true;
        DoubleRMBManagement.Instance.isEnd = true;
        if (UiManager.Instance.mainWindow != null)
            UiManager.Instance.mainWindow.updateDoubleRmb();

    }


}
