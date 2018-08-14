using UnityEngine;
using System.Collections;

/// <summary>
/// 装备提升活动通讯端口
/// </summary>
public class NoticeEquipRemakeFPort : BaseFPort {

	/** 回调函数 */
	private CallBack callback;

	/// <summary>
	/// 找回装备
	/// </summary>
	public void getBackEquip (int sid,CallBack callback) {   
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.NOTICE_EQUIP_REMAKE_GETBACK);
		message.addValue ("sid", new ErlInt (sid));
		access (message);
	}
	/// <summary>
	/// 提升装备
	/// </summary>
	public void remakeEquip (int sid,int propNum,CallBack callback) {   
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.NOTICE_EQUIP_REMAKE_GETAWARD);
		message.addValue ("sid", new ErlInt (sid));
		message.addValue ("num", new ErlInt (propNum));
		access (message);
	}
	public override void read (ErlKVMessage message) {
		ErlArray erlArray = message.getValue ("msg") as ErlArray;
		int index=0;
		string returnType = erlArray.Value [index++].getValueString ();
		if (returnType == "promote_get_back") {
			doGetBackEquip(erlArray,index);
		} else if (returnType == "promote_get_award") {
			doRemakeEquip(erlArray,index);
		}
	}
	/// <summary>
	/// 找回装备数据处理
	/// </summary>
	private void doGetBackEquip (ErlArray erlArray, int index) {
		string msgInfo = erlArray.Value [index++].getValueString ();
		if (msgInfo == "ok") {
			if (callback != null) {
				callback ();
				callback = null;
			}
		}
		else if (msgInfo == "notUsedGetBack") {
			MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("RemakeBuyWindow_notUsedGetBack"));
		}
		else {
			MessageWindow.ShowAlert (msgInfo);
		}
	}
	/// <summary>
	/// 提升装备数据处理
	/// </summary>
	private void doRemakeEquip (ErlArray erlArray,int index) {
		string msgInfo = erlArray.Value [index++].getValueString ();
		if(msgInfo=="ok") {
		 	int noticeSid=StringKit.toInt(erlArray.Value [index++].getValueString ());
			NoticeActiveManagerment manager=NoticeActiveManagerment.Instance;
			DoubleRMBInfo info= manager.getActiveInfoBySid(noticeSid) as DoubleRMBInfo;
			if(info!=null) {
				info.state=true;
			} else {
				manager.putActiveInfo(noticeSid, new DoubleRMBInfo (noticeSid, true));
			}
			if(callback!=null) {
				callback();
				callback=null;
			}
		} else {
			MessageWindow.ShowAlert (msgInfo);
		}
	}
}