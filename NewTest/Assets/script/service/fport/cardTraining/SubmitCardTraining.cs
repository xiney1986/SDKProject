using System;
using UnityEngine;

/**
 * 卡牌训练
 * @author huangzhenghan
 * */
public class SubmitCardTraining : BaseFPort
{
    CallBack<int> callback;


    public void access(CallBack<int> callback, int isRmb, string cardUid, int locationIndex, int timeIndex)
    {
        ////匹配服务器
        if (locationIndex == 0) locationIndex = 3;
        else if (locationIndex == 1) locationIndex = 2;
        else if (locationIndex == 2) locationIndex = 1;
        timeIndex += 1;

        this.callback = callback;
        ErlKVMessage message = new ErlKVMessage(FrontPort.CARDTRAINING_SUBMIT);
        message.addValue("isrmb", new ErlInt(isRmb));
        message.addValue("cardid", new ErlString(cardUid));
        message.addValue("location", new ErlInt(locationIndex));
        message.addValue("timeindex", new ErlInt(timeIndex));
        access(message);
    }

    public override void read(ErlKVMessage message)
    {
        if (message.Value[1] is ErlAtom)
        {
            string str = (message.Value[1] as ErlAtom).getValueString();
            switch (str)
            {
                case "not_exist":
                    MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("s0479"));
                    break;
                case "condition_limit":
                    MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("s0480"));
                    break;
                case "locked":
                    MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("s0481"));
                    break;
                case "rmb_limit":
                    MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("s0482"));
                    break;
                case "cd_limit":
                    MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("s0483"));
                    break;
            }
        }
        else
        {
            int time = StringKit.toInt((message.Value[1] as ErlInt).getValueString()); //StringKit.toInt((message.Value[1] as ErlInt).getValueString());
            callback(time);
        }
        


    }










}

