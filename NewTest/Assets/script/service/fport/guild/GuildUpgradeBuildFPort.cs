using UnityEngine;
using System.Collections;

/**
 * 升级公会建筑接口
 * @author 汤琦
 * */
public class GuildUpgradeBuildFPort : BaseFPort
{
	private CallBack<GuildBuildSample> callback;
	private CallBack call;
	private string sid;
	private int cost;
	
	public void access (string sid,int cost,CallBack call)
	{   
		this.cost = cost;
		this.sid = sid;
		this.call = call;
		ErlKVMessage message = new ErlKVMessage (FrontPort.GUILD_UPGRADE_BUILD);
		message.addValue ("build", new ErlString (sid));
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{
		ErlType type = message.getValue ("msg") as ErlType;
		if(type.getValueString() == "non_mem")
		{
			UiManager.Instance.openDialogWindow<MessageWindow>((win)=>{
				win.initWindow(1,LanguageConfigManager.Instance.getLanguage("s0093"),null,LanguageConfigManager.Instance.getLanguage("Guild_90"),GuildManagerment.Instance.closeAllGuildWindow);
			});
		} else if (type.getValueString() == "condition_limit"){
			MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("guildmanager4"));
		}
		else if(type is ErlArray)
		{
			ErlArray array = type as ErlArray;
			GuildManagerment.Instance.updateBuild(array.Value[0].getValueString(),StringKit.toInt(array.Value[1].getValueString()));
			GuildManagerment.Instance.updateLiveness(cost);
			UiManager.Instance.createMessageLintWindow (LanguageConfigManager.Instance.getLanguage("Guild_48",GuildBuildSampleManager.Instance.getGuildBuildSampleBySid(StringKit.toInt(sid)).buildName));
		}

		if(call != null)
		{
			call();
			call = null;
		}
	}
}
