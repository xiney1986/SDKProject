using UnityEngine;
using System.Collections;
/// <summary>
/// 公会领地挑战
/// </summary>
public class GuildAreaChallengeFPort : BaseFPort
{
	private CallBack<string []> callBack;
	public void access (string serverId, string uid, CallBack<string []> callBack)
	{
		this.callBack = callBack;
		ErlKVMessage msg = new ErlKVMessage (FrontPort.GUILD_AREA_CHALLENGE);
		msg.addValue ("target_sid", new ErlString (serverId));
		msg.addValue ("target_guild_uid", new ErlString (uid));
		access (msg);
	}

	public override void read (ErlKVMessage message)
	{
		base.read (message);
		ErlType type = message.getValue ("msg") as ErlType;
		string cmd = type.getValueString ();
		if ((message.getValue ("msg") as ErlType)!=null &&(message.getValue ("msg") as ErlType).getValueString () == "error") {
			MaskWindow.instance.setServerReportWait(false);
			UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage("GuildArea_54"));
			UiManager.Instance.BackToWindow<GuildMainWindow>();
			callBack = null;
			return;
		}

		if (cmd == "ok") {
			string hurt = (message.getValue("hurt") as ErlType).getValueString();
			string figth_score = (message.getValue("fight_score") as ErlType).getValueString();
			if (callBack != null) {
				callBack (new string [2]{hurt,figth_score});
				callBack = null;
			}

		} else if (cmd == "power_error") {
			UiManager.Instance.createMessageLintWindow (LanguageConfigManager.Instance.getLanguage ("GuildArea_06"));
			MaskWindow.instance.setServerReportWait(false);
			MaskWindow.UnlockUI (true);
		} else if (cmd == "cd_limit") {
			UiManager.Instance.createMessageLintWindow (LanguageConfigManager.Instance.getLanguage ("GuildArea_32"));
			UiManager.Instance.BackToWindow<GuildFightMainWindow> ();
			MaskWindow.instance.setServerReportWait(false);
			MaskWindow.UnlockUI (true);
		} else if (cmd == "fight_time_limit") {
			UiManager.Instance.createMessageLintWindow (LanguageConfigManager.Instance.getLanguage ("GuildArea_39"));
			UiManager.Instance.BackToWindow<GuildFightMainWindow> ();
			MaskWindow.instance.setServerReportWait(false);
			MaskWindow.UnlockUI (true);
		}else if (cmd == "all_dead") {
			UiManager.Instance.createMessageLintWindow (LanguageConfigManager.Instance.getLanguage ("GuildArea_100"));
			UiManager.Instance.BackToWindow<GuildFightMainWindow> ();
			MaskWindow.instance.setServerReportWait(false);
			MaskWindow.UnlockUI (true);
        } else if (cmd == "into_limit")
		{
            UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage("GuildArea_104"));
            UiManager.Instance.BackToWindow<GuildFightMainWindow>();
            MaskWindow.instance.setServerReportWait(false);
            MaskWindow.UnlockUI(true);
        } else if (cmd == "need_revive") {
            UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage("GuildArea_105"));
            UiManager.Instance.BackToWindow<GuildFightMainWindow>();
            MaskWindow.instance.setServerReportWait(false);
            MaskWindow.UnlockUI(true);
        }
	}
}

 