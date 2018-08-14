using UnityEngine;
using System.Collections;

/**
 * 获得公会邀请列表接口
 * @author 汤琦
 * */
public class GuildGetInviteFPort : BaseFPort
{

	private CallBack callback;
	
	public void access (CallBack callback)
	{   
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.GET_INVITEGUILD);
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
			GuildManagerment.Instance.clearGuildInviteList();
			ErlArray array = type as ErlArray;
			for (int i = 0; i < array.Value.Length; i++) {
				ErlArray temp = array.Value[i] as ErlArray;
				if(temp != null)
				{
					int index = 0;
					string uid = temp.Value[index++].getValueString();
					int level = StringKit.toInt(temp.Value[index++].getValueString());
					string name = temp.Value[index++].getValueString();
					string declaration = temp.Value[index++].getValueString();
					int membership = StringKit.toInt(temp.Value[index++].getValueString());
					int membershipMax = StringKit.toInt(temp.Value[index++].getValueString());
					int liveness = StringKit.toInt(temp.Value[index++].getValueString());
					GuildRankInfo info = new GuildRankInfo(uid,level,name,membership,membershipMax,declaration,liveness);
					GuildManagerment.Instance.createGuildInviteList(info);
				}
			}

		}

		if(callback != null)
			callback();
	}
}
