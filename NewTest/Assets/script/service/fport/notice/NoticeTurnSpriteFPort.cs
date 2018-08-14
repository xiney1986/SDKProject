using UnityEngine;
using System.Collections;
/// <summary>
/// 精灵翻翻乐翻牌端口
/// </summary>
public class NoticeTurnSpriteFPort : BaseFPort {
	private CallBack<TurnSpriteReward> callBack;
	public void access(int sid,CallBack<TurnSpriteReward> callBack,int index){
		this.callBack = callBack;
		ErlKVMessage msg = new ErlKVMessage (FrontPort.NOTICE_TURN_SPRITE);
		msg.addValue ("index", new ErlInt (index));
		msg.addValue("sid",new ErlInt(sid));
		access (msg);
	}

	public override void read (ErlKVMessage message)
	{
		base.read (message);
		ErlType type = message.getValue ("msg") as ErlType;
		if (type.getValueString () == "ok") {
			TurnSpriteReward reward = new TurnSpriteReward();
			ErlArray array = message.getValue("info") as ErlArray;
			reward.sid = StringKit.toInt(array.Value[2].getValueString());
			reward.type = array.Value[1].getValueString();
			reward.num = StringKit.toInt(array.Value[0].getValueString());
			if(callBack != null){
				callBack (reward);
				callBack = null;
			}
		}
	}
}
