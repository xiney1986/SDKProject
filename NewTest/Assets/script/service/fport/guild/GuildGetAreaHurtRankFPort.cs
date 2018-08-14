using UnityEngine;
using System.Collections;

/// <summary>
/// 获取公会战贡献排行榜
/// </summary>
public class GuildGetAreaHurtRankFPort : BaseFPort {
	private CallBack callBack;
	public void access(CallBack callBack){
		this.callBack = callBack;
		ErlKVMessage msg = new ErlKVMessage (FrontPort.GUILD_AREA_HURT_RANK);
		access (msg);
	}

	public override void read (ErlKVMessage message)
	{
		base.read (message);
		ErlType type = message.getValue ("msg") as ErlType;
		if (type is ErlArray) {
			ErlArray array = type as ErlArray;
			RankManagerment.Instance.guildAreaHurtList.Clear();
			for(int i =0 ;i<array.Value.Length;++i){
				ErlArray temp = array.Value[i] as ErlArray;
				int index =0;
				string uid = temp.Value[index++].getValueString();
				string name  = temp.Value[index++].getValueString();
				int vipLevel = StringKit.toInt(temp.Value[index++].getValueString());
				string warNum = temp.Value[index++].getValueString();
				string hurtNum = temp.Value[index++].getValueString();
				GuildAreaHurtRankItem item = new GuildAreaHurtRankItem(uid,name,warNum,hurtNum);
				RankManagerment.Instance.guildAreaHurtList.Add(item);
			}

			if(callBack != null)
				callBack();
		}
	}
}
