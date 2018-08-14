using UnityEngine;
using System.Collections;

/// <summary>
/// 坐骑修炼通讯端口
/// </summary>
public class MountsPracticeFPort : BaseFPort {

	/** 回调函数 */
	private CallBack callback;
	
	/// <summary>
	/// 修炼
	/// </summary>
	/// <param name="type">修炼类型-1=普通修炼,2=一键修炼</param>
	/// <param name="callback">Callback.</param>
	public void powerAccess (int type,CallBack callback) {
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.MOUNTS_POWER);
		message.addValue ("type", new ErlString (type.ToString()));
		access (message);
	}
	/// <summary>
	/// 回调读取通讯
	/// </summary>
	/// <param name="message">Message.</param>
	public override void read (ErlKVMessage message) {
		ErlArray erlArray = message.getValue ("msg") as ErlArray;
		int index=0;
		string returnType = erlArray.Value [index++].getValueString ();
		if (returnType == "power") {
			doPowerAccess(erlArray,index);
		}
	}
	/// <summary>
	/// 执行修炼通讯读取处理
	/// </summary>
	private void doPowerAccess(ErlArray erlArray,int index) {
		string msgInfo = erlArray.Value [index++].getValueString ();
		if(msgInfo=="ok") {
			long exp= StringKit.toLong (erlArray.Value [index++].getValueString ());
			MountsManagerment.Instance.addMountsExp (exp);
		} else {
			UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
				win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0093"), null, msgInfo,null);
			});
		}
		if(callback!=null){
			callback();
			callback=null;
		}
	}
}