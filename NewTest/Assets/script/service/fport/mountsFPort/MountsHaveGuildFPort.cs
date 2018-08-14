using UnityEngine;
using System.Collections;

/// <summary>
/// 公会坐骑信息端口
/// </summary>
public class MountsHaveGuildFPort : BaseFPort {
	public CallBack callback;

	/// <summary>
	/// 公会战斗胜利次数
	/// </summary>
	public void initGuildIinfo (CallBack callback) {
		ErlKVMessage message = new ErlKVMessage (FrontPort.MOUNTS_TIME_INFO);
		this.callback=callback;
		access (message);
	}

	/// <summary>
	/// 回调读取通讯
	/// </summary>
	/// <param name="message">Message.</param>
	public override void read (ErlKVMessage message) {
		string fl= (message.getValue ("msg") as ErlType).getValueString();
		int wunNum,gongxianIndex;
		if(fl=="no_info"){
			wunNum=0;
			gongxianIndex=0;
		}else{
			wunNum =StringKit.toInt((message.getValue ("guild_num") as ErlType).getValueString());
			gongxianIndex =StringKit.toInt((message.getValue ("role_num") as ErlType).getValueString());
		}
		GuildManagerment.Instance.setGongxuanIndex(gongxianIndex);
		GuildManagerment.Instance.setWunNum(wunNum);
		callback();
	}
}
