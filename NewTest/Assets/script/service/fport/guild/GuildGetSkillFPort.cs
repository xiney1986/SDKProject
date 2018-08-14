using UnityEngine;
using System.Collections;

/**
 * 学习公会技能接口
 * @author 汤琦
 * */
public class GuildGetSkillFPort : BaseFPort
{
	private CallBack callback;
	
	public void access (CallBack callback)
	{   
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.GUILD_GETSKILL);
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{
		ErlType type = message.getValue ("msg") as ErlType;
		ErlArray array = type as ErlArray;
		if(array.Value.Length > 0)
		{
			//断线重连后先清理一次公会技能，不然会反复ADD，增加战斗力 fix YXZH-3962
			GuildManagerment.Instance.clearGuildSkillList ();
			for (int i = 0; i < array.Value.Length; i++) {
				ErlArray temp = array.Value[i] as ErlArray;
				GuildSkill skill = new GuildSkill(temp.Value[0].getValueString(),StringKit.toInt(temp.Value[1].getValueString()));
				GuildManagerment.Instance.createGuildSkill(skill);
			}
		}
		if(callback != null)
			callback();
	}
}
