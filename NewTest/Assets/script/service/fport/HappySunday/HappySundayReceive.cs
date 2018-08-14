using System;

public class HappySundayReceive : BaseFPort
{
    CallBack callback;


    public void access(int sid, CallBack callback)
    {
        this.callback = callback;
        ErlKVMessage message = new ErlKVMessage(FrontPort.HAPPYSUNDAY_RECEIVE);
        message.addValue("sid", new ErlInt(sid));
		//message.addValue ("activeSid", new ErlInt(NoticeType.HAPPY_SUNDAY));
        access(message);
    }


    public override void read(ErlKVMessage message)
    {
        ErlAtom str = (message.getValue("msg") as ErlAtom);
        if (str == null) return;
        
        if (str.Value == "ok")
        {
            if (callback != null)
                callback();
        }
        else if (str.Value == "aready_award")
        {
            UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage("s0504"));
        }
        else if (str.Value == "error")
        {
            UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage("s0204"));
        }
        else
        {
            MonoBase.print(GetType() + "error:" + str);
        }
    }


}

