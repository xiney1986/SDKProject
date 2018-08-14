using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 精灵翻翻乐获取信息端口
/// </summary>
public class NoticeGetHappyTurnSpriteFPort : BaseFPort
{
	private CallBack callBack;
	private int sid;

	public void access (int sid,CallBack callBack)
	{
		this.callBack = callBack;
		this.sid = sid;
		ErlKVMessage msg = new ErlKVMessage (FrontPort.NOTICE_GET_TURN_SPRITE);
		msg.addValue("sid",new ErlInt(sid));
		access (msg);
	}

	public void parseKVMsg (ErlKVMessage message)
	{
		ErlList list = message.getValue ("msg") as ErlList;
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
				data.num_max = StringKit.toInt (itemArray.Value [1].getValueString ());
				break;
			case "last_time":
				data.lastTime = StringKit.toInt (itemArray.Value [1].getValueString ());
				break;
			case "award":
				ErlArray awardArray = itemArray.Value [1] as ErlArray;
				for (int j=0; j<awardArray.Value.Length; j++) {
					ErlArray tempArray = awardArray.Value [j] as ErlArray;
					TurnSpriteAward award = new TurnSpriteAward ();
					award.num = StringKit.toInt (tempArray.Value [0].getValueString ());
					award.index = StringKit.toInt (tempArray.Value [1].getValueString ());
					data.awardList.Add (award);
				}
				break;
			case "pool":
				ErlArray poolArray = itemArray.Value [1] as ErlArray;
				for (int k=0; k<poolArray.Value.Length; k++) {
					ErlArray rewardArray = poolArray.Value [k] as ErlArray;
					TurnSpriteReward reward = new TurnSpriteReward ();
					int offset = 0;
					reward.num = StringKit.toInt (rewardArray.Value [offset++].getValueString ());
					reward.type = rewardArray.Value[offset++].getValueString ();
					reward.sid = StringKit.toInt (rewardArray.Value [offset++].getValueString ());
					data.rewardList.Add (reward);
				}
				break;
			}
		}
		if(sid == 0)
			NoticeManagerment.Instance.turnSpriteData = data;
		else
			NoticeManagerment.Instance.xs_turnSpriteData = data;
	}

	public override void read (ErlKVMessage message)
	{
		base.read (message);
		parseKVMsg (message);
		if (callBack != null) {
			callBack ();
			callBack = null;
		}
	}
}

public class TurnSpriteData
{
	/** 重新发牌的CD */
	public int cd;
	/** 重新发牌的次数 */
	public int num;
	/** 重新发牌的最大次数 */
	public int num_max;
	public int lastTime;
	/** 已经翻过的牌 */
	public List<TurnSpriteAward> awardList = new List<TurnSpriteAward> ();
	/** 奖励 */
	public List<TurnSpriteReward> rewardList = new List<TurnSpriteReward> ();
    /**爬塔翻过的牌子 */
    public List<TurnSpriteReward> towerRewardList = new List<TurnSpriteReward>();
    public List<TurnSpriteReward> towerNotTurnRewardList = new List<TurnSpriteReward>();
}

public class TurnSpriteAward
{
	public	int num;
	public	int index;
}

public class TurnSpriteReward
{
	/** 奖励卡牌的数量 */
	public int num;
	/** 奖励的卡牌的SID */
	public int sid;
	/** 奖励的类型 */
	public string type;
    /**0代表没有被干开，其他的就是 你懂的。。。。index */
    public int index=0;//默认都是初
}