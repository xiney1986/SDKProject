using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 首冲过得列表
/// </summary>
public class RmbFristFPort : BaseFPort {

	/** 回调函数 */
	/** 转化经验回调函数 */
	private CallBack<List<string>> callback;

	/// <summary>
	/// 获取星魂信息通讯
	/// </summary>
    public void access(CallBack<List<string>> callbackk) {
		this.callback = callbackk;
        ErlKVMessage message = new ErlKVMessage(FrontPort.RMB_FIRST);
		access (message);
	}
	/// <summary>
	/// 星魂加锁
	/// </summary>
	/// <param name="lockinfo">锁信息串</param>
	/// <param name="callback">callback</param>
	/// <summary>
	/// 回调读取通讯
	/// </summary>
	/// <param name="message">Message.</param>
    public override void read(ErlKVMessage message) {
        ErlType type = message.getValue("msg") as ErlType;
        List<string> goodids = null;
        if (type is ErlArray) {
            ErlArray arr = type as ErlArray;
            if(arr!=null&&arr.Value.Length>0){
                for (int i = 0; i < arr.Value.Length;i++ ) {
                    if (goodids == null) goodids = new List<string>();
                    goodids.Add(arr.Value[i].getValueString());
                }
            }
            if (callback != null) {
                callback(goodids);
                callback = null;
            }
        } else {
            UiManager.Instance.openDialogWindow<MessageWindow>((win) => {
                win.initWindow(1, LanguageConfigManager.Instance.getLanguage("s0093"), null, type.getValueString(), null);
            });
        }

    }
}
