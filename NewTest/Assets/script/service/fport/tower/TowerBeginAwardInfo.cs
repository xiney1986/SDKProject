using UnityEngine;
using System.Collections;

/// <summary>
/// 爬塔奖励通讯端口
/// </summary>
public class TowerBeginAwardInfo : BaseFPort {
	
	/** 回调函数 */
	private CallBack<int> callback;

	/// <summary>
	/// 爬塔奖励初始
	/// </summary>
	/// <param name="uid">Uid</param>
	/// <param name="exp">强化所用的经验值</param>
	/// <param name="callback">Callback.</param>
    public void access(CallBack<int> callback) {
		this.callback = callback;
        ErlKVMessage message = new ErlKVMessage(FrontPort.TOWER_AWARD_BEGIN);
		access (message);
	}


	/// <summary>
	/// 回调读取通讯
	/// </summary>
	/// <param name="message">Message.</param>
	public override void read (ErlKVMessage message) {
        base.read(message);
        parseKVMsg(message);
	}
    public void parseKVMsg(ErlKVMessage message) {
        ErlType type = message.getValue("msg") as ErlType;
        if (type is ErlAtom) {
            ClmbTowerManagerment.Instance.canPassBox = true;
            callback(3);
        }
        if (type is ErlArray) {
            ErlArray teamp = type as ErlArray;
            if (teamp.Value.Length <= 0) {
                ClmbTowerManagerment.Instance.turnSpriteData = null;
                callback(0);
                return;
            }
            int oldSid = StringKit.toInt(teamp.Value[0].getValueString());
            ClmbTowerManagerment.Instance.missionSid = oldSid;
            ErlArray itemArray = teamp.Value[1] as ErlArray;
            TurnSpriteData data = new TurnSpriteData();
            if (itemArray.Value.Length <= 0) {
                ClmbTowerManagerment.Instance.turnSpriteData = null;
                callback(0);
            } else {
                for (int i = 0; i < itemArray.Value.Length; i++) {
                    ErlArray rewardArray = itemArray.Value[i] as ErlArray;
                    TurnSpriteReward reward = new TurnSpriteReward();
                    int offset = 0;
                    reward.sid = StringKit.toInt(rewardArray.Value[offset++].getValueString());
                    reward.type = rewardArray.Value[offset++].getValueString();
                    reward.num = StringKit.toInt(rewardArray.Value[offset++].getValueString());
                    reward.index = StringKit.toInt(rewardArray.Value[offset++].getValueString());
                    data.rewardList.Add(reward);
                    if (reward.index != 0) {
                        data.towerRewardList.Add(reward);
                    } 
                    ClmbTowerManagerment.Instance.turnSpriteData = data;
                }
                callback(1);
            }
            

        }
    }


}