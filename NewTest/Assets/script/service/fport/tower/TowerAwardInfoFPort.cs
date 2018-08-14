using UnityEngine;
using System.Collections;

/// <summary>
/// 爬塔奖励通讯端口
/// </summary>
public class TowerAwardInfoFPort : BaseFPort {
	
	/** 回调函数 */
	private CallBack callback;
    private int index;

	/// <summary>
	/// 爬塔奖励初始
	/// </summary>
	/// <param name="uid">Uid</param>
	/// <param name="exp">强化所用的经验值</param>
	/// <param name="callback">Callback.</param>
    public void access(CallBack callback,int index,int missionSid) {
        this.index = index;
		this.callback = callback;
        ErlKVMessage message = new ErlKVMessage(FrontPort.TOWER_AWARD_INFO);
        int sid=missionSid==0?MissionInfoManager.Instance.mission.sid:missionSid;
        message.addValue("tower_sid", new ErlInt(sid));
        message.addValue("index", new ErlInt(index));
		access (message);
	}
    public void accessGiveUp(CallBack callback, int towerSid) {
        this.callback = callback;
        ErlKVMessage message = new ErlKVMessage(FrontPort.TOWER_AWARD_INFO);
        message.addValue("tower_sid", new ErlInt(towerSid));
        message.addValue("index", new ErlInt(0));
        access(message);
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
        if (type is ErlArray) {
            ErlArray itemArray = type as ErlArray;
            TurnSpriteData data = new TurnSpriteData();
            for (int i = 0; i < itemArray.Value.Length; i++) {
                    ErlArray rewardArray = itemArray.Value[i] as ErlArray;
                    TurnSpriteReward reward = new TurnSpriteReward();
                    int offset = 0;
                    reward.sid = StringKit.toInt(rewardArray.Value[offset++].getValueString());
                    reward.type = rewardArray.Value[offset++].getValueString();
                    reward.num = StringKit.toInt(rewardArray.Value[offset++].getValueString());
                    reward.index = StringKit.toInt(rewardArray.Value[offset++].getValueString());
                    data.rewardList.Add(reward);
                    ClmbTowerManagerment.Instance.turnSpriteData = data;
            }
            if (callback != null) {
                callback();
                callback = null;
            }
        }else if(type.getValueString()=="ok"){//这里标示放弃副本成功了
            if (callback != null) {
                callback();
                callback = null;
            }
        } else if (type.getValueString() == "limit_time") {
            UiManager.Instance.openDialogWindow<MessageLineWindow>((win) => {
                callback = null;
                win.Initialize(LanguageConfigManager.Instance.getLanguage("towerShowWindow33"));
            });
        } else if (type.getValueString() == "fb_limit") {
            UiManager.Instance.openDialogWindow<MessageLineWindow>((win) => {
                callback = null;
                win.Initialize(LanguageConfigManager.Instance.getLanguage("towerShowWindow34"));
            });
        } else if (type.getValueString() == "already_open") {
            UiManager.Instance.openDialogWindow<MessageLineWindow>((win) => {
                callback = null;
                win.Initialize(LanguageConfigManager.Instance.getLanguage("towerShowWindow35"));
            });
        }
    }


}