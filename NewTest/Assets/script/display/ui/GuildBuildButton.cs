using UnityEngine;
using System.Collections;

public class GuildBuildButton : ButtonBase
{
	private int buildLevel;
	private string buildSid;
	private string buildName;
	public CallBack<GuildBuildSample> callback;

	public void initInfo(int buildLevel,string buildSid,string buildName)
	{
		this.buildLevel = buildLevel;
		this.buildSid = buildSid;
		this.buildName = buildName;
	}

	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
//		if(GuildManagerment.Instance.isUpGuildBuild(buildSid,buildLevel))
//		{
//			UiManager.Instance.openDialogWindow<MessageWindow>((win)=>{
//				win.dialogCloseUnlockUI=false;
//				if(GuildManagerment.Instance.getBuildLevel(buildSid) <= 0)
//				{
//					win.initWindow(2,LanguageConfigManager.Instance.getLanguage("s0094"),LanguageConfigManager.Instance.getLanguage("s0093"),LanguageConfigManager.Instance.getLanguage("Guild_77",buildName),building);
//				}
//				else
//				{
//					win.initWindow(2,LanguageConfigManager.Instance.getLanguage("s0094"),LanguageConfigManager.Instance.getLanguage("s0093"),LanguageConfigManager.Instance.getLanguage("Guild_76",buildName),building);
//				}
//			});
//		}
	}

	private void building(MessageHandle msg)
	{
//		if(msg.buttonID == MessageHandle.BUTTON_LEFT){
//			MaskWindow.UnlockUI();
//			return;
//		}
//		GuildBuildSample sample = GuildBuildSampleManager.Instance.getGuildBuildSampleBySid(StringKit.toInt(buildSid));
//		if(textLabel.text == LanguageConfigManager.Instance.getLanguage("Guild_34"))
//		{
//			GuildUpgradeBuildFPort fport = FPortManager.Instance.getFPort("GuildUpgradeBuildFPort") as GuildUpgradeBuildFPort;
//			fport.access(buildSid,sample.costs[buildLevel],callback,(fatherWindow as GuildBuildWindow).updateLive);
//		}
//		else
//		{
//			GuildCreateBuildFPort fport = FPortManager.Instance.getFPort("GuildCreateBuildFPort") as GuildCreateBuildFPort;
//			fport.access(buildSid,sample.costs[buildLevel],callback,(fatherWindow as GuildBuildWindow).updateLive);
//		}
//		GuildManagerment.Instance.guildMain.isChange = true;
	}
}
