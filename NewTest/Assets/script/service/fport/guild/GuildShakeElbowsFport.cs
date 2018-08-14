using UnityEngine;
using System.Collections;

public class GuildShakeElbowsFport : BaseFPort {
	/** 是否为重新投掷 */
	private bool isReShake = false;
	private CallBack callBack;
	/// <summary>
	/// 普通投掷
	/// </summary>
	public void beginShakeElbows( CallBack callBack)
	{
		isReShake = false;
		this.callBack = callBack;
		ErlKVMessage msg = new ErlKVMessage (FrontPort.GUILD_SHAKEEBLOWS);
		msg.addValue ("type", new ErlInt (1));
		access (msg);
	}
	/// <summary>
	/// 重投
	/// </summary>
	public void reShakeElbows(string  locks, CallBack callBack)
	{
		isReShake = true;
		this.callBack = callBack;
		ErlKVMessage msg = new ErlKVMessage (FrontPort.GUILD_SHAKEEBLOWS);
		msg.addValue ("type", new ErlInt (2));
		msg.addValue("re_cast",new ErlString(locks));
		access (msg);
	}


	public override void read (ErlKVMessage message)
	{
		ErlType type = message.getValue ("msg") as ErlType;
		if (type is ErlList) {
			ErlList list = type as ErlList;
			string [] results = new string[5];
			for(int i = 0 ;i<results.Length ; ++i){
				results[i] = list.Value [i].getValueString ();
			}
			GuildManagerment.Instance.createGuildShakeResult(results);
			//投掷/重投成功，减去相应次数
			if(!isReShake)
			{
				GuildManagerment.Instance.getGuildLuckyNvShenInfo().shakeCount --;
			}
			else
			{
				GuildManagerment.Instance.getGuildLuckyNvShenInfo().reShakeCount --;
			}
			if (callBack != null){
				callBack ();
				callBack = null;
			}
		} else {

		}

	}
}
