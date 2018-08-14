using UnityEngine;
using System.Collections;

/// <summary>
/// 星魂通讯端口
/// </summary>
public class StarSoulFPort : BaseFPort {

	/** 回调函数 */
	private CallBack callback;
	/** 转化经验回调函数 */
	private CallBack<int> convertCallback;

	/// <summary>
	/// 获取星魂信息通讯
	/// </summary>
	public void getStarSoulInfoAccess (CallBack callback) {
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.STARSOUL_INFO);
		access (message);
	}
	/// <summary>
	/// 星魂加锁
	/// </summary>
	/// <param name="lockinfo">锁信息串</param>
	/// <param name="callback">callback</param>
	public void doLockStarSoulAccess (string lockinfo,CallBack callback) {
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.STARSOUL_LOCK);
		message.addValue ("update_locks", new ErlString (lockinfo));
		access (message);
	}
	/// <summary>
	/// 回调读取通讯
	/// </summary>
	/// <param name="message">Message.</param>
	public override void read (ErlKVMessage message) {
		string msgInfo = (message.getValue ("msg") as ErlType).getValueString ();
		if (msgInfo=="info") {
			ErlType erlType = message.getValue ("value") as ErlType;
			if (erlType is ErlArray) {
				StarSoulManager manager=StarSoulManager.Instance;
				ErlArray arr= erlType as ErlArray;
				StarSoulInfo starSoulInfo=manager.createStarSoulInfo(arr);
				manager.setStarSoulInfo(starSoulInfo);
			} else {
				UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
					win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0093"), null, erlType.getValueString (),null);
				});
			}
			if(callback!=null){
				callback();
				callback=null;
			}
		} else if (msgInfo == "lock") {
			ErlType erlType = message.getValue ("value") as ErlType;
			if(erlType.getValueString ()=="ok") {
				StarSoulManager manager=StarSoulManager.Instance;
				manager.updateStarSoulLockState();
				StarSoulManager.Instance.cleanDic();
				StorageManagerment.Instance.starSoulStorageVersion++;
			} else{
				UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
					win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0093"), null, erlType.getValueString (),null);
				});
			}
			if(callback!=null){
				callback();
				callback=null;
			}
		}
	}
}
