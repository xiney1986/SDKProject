using UnityEngine;
using System.Collections;

/// <summary>
/// 宣传栏信息获取端口
/// </summary>
public class BulletinFPort : BaseFPort {

	/** 回调函数 */
	private CallBack callback;

	public void getBulletin (CallBack callback) {
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.BULLETIN);
		access (message);
	}

	public override void read (ErlKVMessage message) {
		base.read (message);
		BulletinManager.Instance.creatButtletinList (message);
		if (callback != null) {
			callback ();
			callback = null;
		}
	}
}
