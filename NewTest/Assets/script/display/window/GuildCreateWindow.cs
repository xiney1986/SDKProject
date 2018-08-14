using UnityEngine;
using System.Collections;

/**
 * 创建公会界面
 * @author 汤琦
 * */
public class GuildCreateWindow : WindowBase
{
	public UIInput guildName;
	public UILabel buttonNameRmb;
	public UILabel buttonNameMoney;
	public UIToggle[] cost;
	private int autoJoin=1;
	public UIToggle storeChoose;

	protected override void begin ()
	{
		base.begin ();
		guildName.defaultText = Language ("Guild_8");
		storeChoose.value=true;
		MaskWindow.UnlockUI ();
	}

	protected override void DoEnable ()
	{
		//base.DoEnable ();
		buttonNameMoney.text = LanguageConfigManager.Instance.getLanguage("Guild_105",GuildManagerment.CREATEGUILDMONEY.ToString());
		buttonNameRmb.text = LanguageConfigManager.Instance.getLanguage("Guild_106",GuildManagerment.CREATEGUILDRMB.ToString());
		//UiManager.Instance.backGroundWindow.switchBackGround("ChouJiang_BeiJing");
	//	UiManager.Instance.backGround.switchToDark();
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);

		if(gameObj.name == "createButton")
		{

			guildName.value = guildName.value.Trim();
			if(guildName.value.Length > 6)
			{
				guildName.value = guildName.value.Substring(0,6);
				UiManager.Instance.openDialogWindow<MessageWindow>((win)=>{
					win.initWindow(1,LanguageConfigManager.Instance.getLanguage("s0093"),null,LanguageConfigManager.Instance.getLanguage("Guild_87"),null);
				});
			} else {
				if(GuildManagerment.Instance.isCreateGuild(getCostType(),guildName.value))
				{
					GuildCreateFPort fport = FPortManager.Instance.getFPort("GuildCreateFPort") as GuildCreateFPort;
					fport.access(guildName.label.text,getCostType(),autoJoin,succeedBack);
				}
			}
		}

		else if(gameObj.name == "close" || gameObj.name == "returnButton")
		{
			finishWindow();
		}
	}

	private void succeedBack(MessageHandle msg)
	{
		GuildGetInfoFPort fport = FPortManager.Instance.getFPort ("GuildGetInfoFPort") as GuildGetInfoFPort;
		fport.access (openGuildWindow);
	}

	private void openGuildWindow ()
	{
		GuildManagerment.Instance.openWindow ();
	}

	private string getCostType()
	{
		for (int i = 0; i < cost.Length; i++) {
			if(cost[i].value)
			{
				return cost[i].name;
			}
		}
		return "";
	}
	bool checkName ()
	{
//		bool isnext = true;
		if(guildName.value.Length > 6)
		{
			guildName.value = guildName.value.Substring(0,6);
			UiManager.Instance.openDialogWindow<MessageWindow>((win)=>{
				win.initWindow(1,LanguageConfigManager.Instance.getLanguage("s0093"),null,LanguageConfigManager.Instance.getLanguage("Guild_87"),null);
			});
			return false;
		} else {
			return true;
		}
	}
	/** 改变Toggle */
	public void ChangeToggleValue() {
		if(storeChoose.value)autoJoin=1;
		else autoJoin=0;
	}
}
