using UnityEngine;
using System.Collections;

/**
 * PVP战斗接口
 * @author 汤琦
 * */
public class PvpFightFPort : BaseFPort
{ 
	private CallBack callback;
	
	//type 1 普通攻击 2全力攻击	uid为对手
	public void access (int type, string uid, int pvpType, CallBack callback)
	{   
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.PVP_FIGHT);
		message.addValue ("array", new ErlInt (3));
		message.addValue ("type", new ErlInt (type));
		message.addValue ("pvp_type", new ErlInt (pvpType));

		if (PvpInfoManagerment.Instance.getPvpInfo ().rule == "match") {
			//普通赛可以选人
			message.addValue ("enemy", new ErlString (uid));
		}

		access (message);
	}

	public override void read (ErlKVMessage message)
	{
		ErlType str = message.getValue ("msg") as ErlType;
		if (str.getValueString () == "ok") {
			if (callback != null)
				callback ();
		} else {
			if (str.getValueString () == "over_time") {
				PvpInfoManagerment.Instance.clearDate ();
			}else if(str.getValueString () == "pvp_error"){
				MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("pvpuse00"));
			}else{
				MessageWindow.ShowAlert (str.getValueString ());
			}
			if (callback != null)
				callback = null;
		}
	}
}
