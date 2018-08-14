using UnityEngine;
using System.Collections;
/// <summary>
/// 重新发牌接口
/// </summary>
public class NoticePullSpriteFPort : BaseFPort {
	public CallBack callBack;
	private int sid;

	public void access(int sid,CallBack callBack){
		this.sid = sid;
		this.callBack = callBack;
		ErlKVMessage msg = new ErlKVMessage (FrontPort.NOTICE_PULL_SPRITE);
		msg.addValue("sid",new ErlInt(sid));
		access (msg);
	}

	public override void read (ErlKVMessage message)
	{
		base.read (message);
		ErlType type = message.getValue ("msg") as ErlType;
		if (type.getValueString () == "ok") {
			ErlList list = message.getValue("info") as ErlList;
			ErlArray itemArray;
			string key;
			TurnSpriteData data = new TurnSpriteData ();
			for (int i=0; i<list.Value.Length; i++) {
				itemArray = list.Value [i] as ErlArray;
				key = itemArray.Value [0].getValueString ();			
				switch (key) {
				case "cd":
					data.cd = StringKit.toInt (itemArray.Value [1].getValueString ());
					break;
				case "num":
					data.num = StringKit.toInt (itemArray.Value [1].getValueString ());
					break;
				case "max_num":
					data.num_max = StringKit.toInt(itemArray.Value[1].getValueString());
					break;
				case "last_time":
					data.lastTime = StringKit.toInt (itemArray.Value [1].getValueString ());
					break;
				case "pool":
					ErlArray poolArray = itemArray.Value[1] as ErlArray;
					for(int k=0;k<poolArray.Value.Length;k++){
						ErlArray rewardArray = poolArray.Value[k] as ErlArray;
						TurnSpriteReward reward = new TurnSpriteReward();
						int offset =0;
						reward.num = StringKit.toInt(rewardArray.Value[offset++].getValueString());
						reward.type = rewardArray.Value[offset++].getValueString();
						reward.sid = StringKit.toInt(rewardArray.Value[offset++].getValueString());
						data.rewardList.Add(reward);
					}
					break;
				}
			}
			if(sid==0)
				NoticeManagerment.Instance.turnSpriteData = data;
			else	
				NoticeManagerment.Instance.xs_turnSpriteData = data;
			if(callBack != null){
				callBack();
				callBack = null;
			}

		}

	} 

}
