using UnityEngine;
using System.Collections;

/// <summary>
/// 爬塔奖励通讯端口
/// </summary>
public class TowerGetAwardInfo : BaseFPort {
	
	/** 回调函数 */
    private CallBack callback;
    private int index;

	/// <summary>
	/// 爬塔奖励初始
	/// </summary>
	/// <param name="uid">Uid</param>
	/// <param name="exp">强化所用的经验值</param>
	/// <param name="callback">Callback.</param>
    public void access(CallBack callback) {
		this.callback = callback;
        ErlKVMessage message = new ErlKVMessage(FrontPort.TOWER_AWARD_GET);
		access (message);
	}


	/// <summary>
	/// 回调读取通讯
	/// </summary>
	/// <param name="message">Message.</param>
	public override void read (ErlKVMessage message) {
        base.read(message);
        ErlType type = message.getValue("msg") as ErlType;
        if (type.getValueString() == "ok") {
            callback();
            }   
	    }


}