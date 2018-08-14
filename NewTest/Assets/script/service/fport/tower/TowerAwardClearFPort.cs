using UnityEngine;
using System.Collections;

/// <summary>
/// 爬塔奖励清理通讯端口
/// </summary>
public class TowerAwardClearFPort : BaseFPort {
	
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
        ErlKVMessage message = new ErlKVMessage(FrontPort.TOWER_CLEAR);
        int sid=missionSid==0?MissionInfoManager.Instance.mission.sid:missionSid;
        message.addValue("tower_sid", new ErlInt(sid));
        message.addValue("index", new ErlInt(index));
		access (message);
	}
    public void accessGiveUp(CallBack callback, int towerSid) {
        this.callback = callback;
        ErlKVMessage message = new ErlKVMessage(FrontPort.TOWER_CLEAR);
        message.addValue("tower_sid", new ErlInt(towerSid));
        message.addValue("index", new ErlInt(0));
        message.addValue("type", new ErlInt(ClmbTowerManagerment.Instance.intoTpye));
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
         if(type.getValueString()=="ok"){//这里标示放弃副本成功了
            if (callback != null) {
                callback();
                callback = null;
            }
        } else if (type.getValueString() == "not_open") {
            UiManager.Instance.openDialogWindow<MessageLineWindow>((win) => {
                callback = null;
                win.Initialize(LanguageConfigManager.Instance.getLanguage("towerShowWindow31"));
            });
        } else if (type.getValueString() == "index_error") {
            UiManager.Instance.openDialogWindow<MessageLineWindow>((win) => {
                callback = null;
                win.Initialize(LanguageConfigManager.Instance.getLanguage("towerShowWindow32"));
            });
        }
    }


}