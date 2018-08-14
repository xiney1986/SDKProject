using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 获取公会战信息接口 
/// </summary>
public class GuildGetFightInfoFport : BaseFPort
{
	private CallBack callBack;
	public void access (CallBack callBack)
	{
		this.callBack = callBack;
		ErlKVMessage msg = new ErlKVMessage (FrontPort.GUILD_GET_FIGHTINFO);
		access (msg);
	}

	public override void read (ErlKVMessage message)
	{
		base.read (message);
		if ((message.getValue ("msg") as ErlType)!=null &&(message.getValue ("msg") as ErlType).getValueString () == "error") {
			UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage("GuildArea_54"));
			UiManager.Instance.BackToWindow<GuildMainWindow>();
			callBack = null;
			return;
		}

		if (message.getValue ("state").ToString () != null) {
			List<string> messageList = new List<string>();
			ErlType stateType = message.getValue ("state") as ErlType;
			ErlArray powerType = message.getValue("power") as ErlArray;
			int guildPowerNow = StringKit.toInt(powerType.Value[0].getValueString());
			int guildPowerMax = StringKit.toInt(powerType.Value[1].getValueString());
			int openTime = 0;
			int weekActivi= 0;
			int selfWarNum = 0;
            int selfCurBlood = 0;
            int selfMaxBlood = 0;
			bool get_power = false;
            bool isDead = false;
			int guildFightState = StringKit.toInt (stateType.getValueString ());
			List<GuildAreaPreInfo> areaList = new List<GuildAreaPreInfo> ();
			/** 公会战未开启 */
			if (guildFightState == GuildFightMainWindow.NOTOPEN) {
				/** 公会战开启时间 */
				ErlType openTimeType = message.getValue("open_time") as ErlType;
				openTime = StringKit.toInt(openTimeType.getValueString());
				/** 公会本周活跃度 */
				ErlType weekActiviType = message.getValue("week_active") as ErlType;
				weekActivi =StringKit.toInt( weekActiviType.getValueString());
			}
			else if(guildFightState == GuildFightMainWindow.NOTJOIN){
				/** 公会本周活跃度 */
				ErlType weekActiviType = message.getValue("week_active") as ErlType;
				weekActivi =StringKit.toInt( weekActiviType.getValueString());
			}
			/** 公会战分组中 */
			else if (guildFightState ==  GuildFightMainWindow.GROUPING) {
				//DO NOTHING
			}
			/** 公会战期间 */
			else if (guildFightState == GuildFightMainWindow.FIGHTING) {
				get_power = (message.getValue("get_power") as ErlType).getValueString() != "1";  
                isDead = (message.getValue("revive") as ErlType).getValueString() == "0";
				ErlType type = message.getValue ("msg") as ErlType;
				if (type is ErlArray) {
					ErlArray areas = type as ErlArray;				
					for (int i= 0; i<areas.Value.Length; ++i) {
						ErlArray areaArray = areas.Value [i] as ErlArray;
						int offset = 0;
						/** 服务器名 */
						string server = areaArray.Value [offset++].getValueString ();
						/** uid */
						string uid = areaArray.Value [offset++].getValueString ();
						/** 名称 */
						string name = areaArray.Value [offset++].getValueString ();
						/** 战争点 */
						int warNum = StringKit.toInt (areaArray.Value [offset++].getValueString ());
						/** 评分 */
						int judge = StringKit.toInt (areaArray.Value[offset++].getValueString());
						/** 状态 */
						int state =	StringKit.toInt(areaArray.Value[offset++].getValueString());
						int time =	StringKit.toInt(areaArray.Value[offset++].getValueString());
						int defense  =	StringKit.toInt(areaArray.Value[offset++].getValueString());
						int attack  =	StringKit.toInt(areaArray.Value[offset++].getValueString());
						/** 自己公会，就去接收消息 */
						if(uid == GuildManagerment.Instance.getGuild().uid){
							ErlArray messageArrays = areaArray.Value[offset++] as ErlArray;
							for(int j =0 ;j<messageArrays.Value.Length;++j){
								messageList.Add(messageArrays.Value[j].getValueString());
							}
							selfWarNum = StringKit.toInt(areaArray.Value[offset++].getValueString());
                            selfCurBlood = StringKit.toInt(areaArray.Value[offset++].getValueString());
                            selfMaxBlood = StringKit.toInt(areaArray.Value[offset++].getValueString());
						}
						GuildAreaPreInfo info = new GuildAreaPreInfo (uid, server, name, warNum, judge, state,time,defense,attack);
						areaList.Add (info);
					}
				}
			}
			GuildManagerment.Instance.guildFightInfo = new GuildFightInfo (guildFightState, areaList,get_power,messageList,openTime,weekActivi,selfWarNum,isDead,selfCurBlood,selfMaxBlood);
			if (callBack != null)
				callBack ();
		}
	}
}
