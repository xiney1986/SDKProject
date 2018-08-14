using UnityEngine;
using System.Collections;

/// <summary>
/// 公告配置接口
/// </summary>
public class NoticeConfigFPort : BaseFPort {
	
	private CallBack<string> callback;
	
	public void access (CallBack<string> callback) {   
		ErlKVMessage message = new ErlKVMessage (FrontPort.ACTIVE_LIST);
		access (message);
	}
	public override void read (ErlKVMessage message) {
		string str = (message.getValue ("front") as ErlType).getValueString();
		if(string.IsNullOrEmpty(str)) {
			callback(str);
		}
	}
	//解析ErlKVMessgae
	public bool parseKVMsg (ErlKVMessage message) {
		string content = (message.getValue ("front") as ErlType).getValueString();
		NoticeSampleManager.Instance.loadNoticeSample (content);
		return true;
	}
}
