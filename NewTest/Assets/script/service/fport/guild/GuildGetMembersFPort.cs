using UnityEngine;
using System.Collections;

/**
 * 获得公会成员集接口
 * @author 汤琦
 * */
public class GuildGetMembersFPort : BaseFPort
{
	private CallBack callback;
	
	public void access (CallBack callback)
	{   
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.GUILD_GET_MEMBERS);
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
		}
		else if(type is ErlArray)
		{
			GuildManagerment.Instance.clearGuildMembers();
			ErlArray array = type as ErlArray;
			for (int i = 0; i < array.Value.Length; i++) {
				ErlArray temps = array.Value[i] as ErlArray;
				int index = 0;
				string uid = temps.Value[index++].getValueString();
				int icon = StringKit.toInt(temps.Value[index++].getValueString());
				int vipLv = StringKit.toInt(temps.Value[index++].getValueString());
				string name = temps.Value[index++].getValueString();
				int level = StringKit.toInt(temps.Value[index++].getValueString());
				int job = StringKit.toInt(temps.Value[index++].getValueString());
				int contributioning = StringKit.toInt(temps.Value[index++].getValueString());
				int contributioned = StringKit.toInt(temps.Value[index++].getValueString());
				int login = StringKit.toInt(temps.Value[index++].getValueString());
				int logout = StringKit.toInt(temps.Value[index++].getValueString());
				int donating = StringKit.toInt(temps.Value[index++].getValueString());
				int donated = StringKit.toInt(temps.Value[index++].getValueString());

				GuildMember member = new GuildMember(uid,icon,vipLv,name,level,job,contributioning,contributioned,login,logout,donating,donated);
				GuildManagerment.Instance.createGuildMembers(member);
			}
			if(callback != null)
				callback();
		}

	}
}
