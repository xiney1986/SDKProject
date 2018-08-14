using UnityEngine;
using System.Collections;

/**
 * 获得公会审批列表接口
 * @author 汤琦
 * */
public class GuildGetApprovalListFPort : BaseFPort
{
	private CallBack callback;
	
	public void access (CallBack callback)
	{   
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.GUILD_GET_APPROVALLIST);
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
			GuildManagerment.Instance.clearGuildApprovalList();
			ErlArray array = type as ErlArray;
			int index = 0,level,vipLevel;
            string uid,name,headIcon;
			ErlArray temp;
			for (int i = 0; i < array.Value.Length; i++) {
				temp = array.Value[i] as ErlArray;
				uid = temp.Value[index++].getValueString();
                name = temp.Value[index++].getValueString();
                level = StringKit.toInt(temp.Value[index++].getValueString());
				vipLevel = StringKit.toInt(temp.Value[index++].getValueString());
                headIcon = temp.Value[index++].getValueString();
				GuildApprovalInfo info = new GuildApprovalInfo(uid,name,level,vipLevel,headIcon);
				GuildManagerment.Instance.createGuildApprovalList(info);
				index = 0;
			}
			if (callback != null)
				callback ();
		}
	}
}
