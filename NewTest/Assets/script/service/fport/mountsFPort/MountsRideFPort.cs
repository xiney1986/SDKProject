using UnityEngine;
using System.Collections;

/// <summary>
/// 坐骑通讯端口
/// </summary>
public class MountsRideFPort : BaseFPort {
	
	/** 回调函数 */
	private CallBack callback;

	/// <summary>
	/// 坐骑坐乘-更换
	/// </summary>
	/// <param name="mountsUid">坐骑uid</param>
	/// <param name="callback">Callback.</param>
	public void putOnMountsAccess (string mountsUid,CallBack callback) {
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.MOUNTS_PUTON);
		message.addValue ("mounts_uid", new ErlString (mountsUid));
		access (message);
	}
	/// <summary>
	/// 坐骑休息
	/// </summary>
	/// <param name="callback">Callback.</param>
	public void putOffMountsAccess (CallBack callback) {
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.MOUNTS_PUTON);
		message.addValue ("mounts_uid", new ErlString ("0"));
		access (message);
	}
	/// <summary>
	/// 坐骑激活
	/// </summary>
	/// <param name="mountsSid">坐骑sid</param>
	/// <param name="callback">Callback.</param>
	public void activeMountsAccess (int mountsSid,CallBack callback) {
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.MOUNTS_ACTIVE);
		message.addValue ("mounts_sid", new ErlString (mountsSid.ToString()));
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
		if (returnType == "puton") {
			doPutonMounts(erlArray,index);
		} else if (returnType == "putoff") {
			doPutoffStarSoul(erlArray,index);
		} else if (returnType == "active") {
			doActiveStarSoul(erlArray,index);
		}
	}
	/// <summary>
	/// 坐骑坐乘,更换通讯读取处理
	/// </summary>
	private void doPutonMounts(ErlArray erlArray,int index) {
		string msgInfo = erlArray.Value [index++].getValueString ();
		if(msgInfo=="ok") {
			string uid=erlArray.Value [index++].getValueString ();
			MountsManagerment manager=MountsManagerment.Instance;
			Mounts useMounts=manager.getMountsInUse();
			if(useMounts!=null) {
				useMounts.setState(false);
			}
			Mounts mounts=manager.getMountsByUid(uid);
			if(mounts!=null) {
				mounts.setState(true);
			}
		} else {
			UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
				win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0093"), null, msgInfo,null);
			});
		}
		if(callback!=null) {
			callback();
			callback=null;
		}
	}
	/// <summary>
	/// 坐骑休息通讯读取处理
	/// </summary>
	private void doPutoffStarSoul(ErlArray erlArray,int index) {
		string msgInfo = erlArray.Value [index++].getValueString ();
		if(msgInfo=="ok") {
			Mounts useMounts=MountsManagerment.Instance.getMountsInUse();
			if(useMounts!=null) {
				useMounts.setState(false);
			}
		} else {
			UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
				win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0093"), null, msgInfo,null);
			});
		}
		if(callback!=null) {
			callback();
			callback=null;
		}
	}
	/// <summary>
	/// 坐骑激活通讯读取处理
	/// </summary>
	private void doActiveStarSoul(ErlArray erlArray,int index) {
		string msgInfo = erlArray.Value [index++].getValueString ();
		if(msgInfo=="ok") {
		} else {
			UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
				win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0093"), null, msgInfo,null);
			});
		}
		if(callback!=null) {
			callback();
			callback=null;
		}
	}
}
