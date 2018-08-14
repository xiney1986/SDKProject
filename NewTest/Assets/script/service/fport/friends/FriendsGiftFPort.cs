using UnityEngine;
using System.Collections;

public class FriendsGiftFPort : BaseFPort
{

    private CallBack mCallback;


    public void receive(CallBack callback, string uid)
    {
        mCallback = callback;
        ErlKVMessage message = new ErlKVMessage(FrontPort.FRIENDS_GIFT_RECEIVE);
        message.addValue("uid", new ErlString(uid));
        access(message);
    }


    public void send(CallBack callback, string uid)
    {
        mCallback = callback;
        ErlKVMessage message = new ErlKVMessage(FrontPort.FRIENDS_GIFT_SEND);
        message.addValue("uid", new ErlString(uid));
        access(message);
    }



	public override void read (ErlKVMessage message)
	{
        string str = (message.getValue("msg") as ErlAtom).Value;
        if (str == "ok")
        {
            if (mCallback != null)
                mCallback();
        }
        else
        {
            switch (str)
            {
                case "aready_get": UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage("recharge02")); break;
                case "limit_count": UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage("Friends24")); break;
                case "pve_full": UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage("Friends25")); break;
                case "aready_give": UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage("Friends26")); break;
                default : UiManager.Instance.createMessageLintWindow(str); break;
            }
        }
        MaskWindow.UnlockUI();
	}




}
