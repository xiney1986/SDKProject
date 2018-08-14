using UnityEngine;
using System.Collections;

/**
 * 公告获取活动条目时间
 * @author huangzhenghan
 * */
public class NoticeGetTimeFPort : BaseFPort
{

	private CallBack callback;
	
	public void access (CallBack callback)
	{   
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.NOTICE_TIME);
		access (message);
	}
	//	"exchange\":[[107018,1397534400,1397541600]] "affiche"
	public override void read (ErlKVMessage message)
	{
		parseKVMsg (message);
		if (callback != null)
			callback ();
	}
	//解析ErlKVMessgae
	public void parseKVMsg (ErlKVMessage message)
	{
		ErlArray exchangeArray = message.getValue ("exchange") as ErlArray;
		ErlArray afficheArray = message.getValue ("affiche") as ErlArray;
		int sid, start, end;
		ErlArray values;
		for (int i=0; i<exchangeArray.Value.Length; i++) {
			values = exchangeArray.Value [i] as ErlArray;if (values.Value.Length == 3) {//未生效，开始结束时间
				sid = StringKit.toInt ((values.Value [0] as ErlType).getValueString ());
				start = StringKit.toInt ((values.Value [1] as ErlType).getValueString ());
				end = StringKit.toInt ((values.Value [2] as ErlType).getValueString ());
				NoticeManagerment.Instance.addExchangeTime (sid, new int[]{start,end});
			}
		}
		for (int i=0; i<afficheArray.Value.Length; i++) {
			values = afficheArray.Value [i] as ErlArray;if (values.Value.Length == 3) {//未生效，开始结束时间
				sid = StringKit.toInt ((values.Value [0] as ErlType).getValueString ());
				start = StringKit.toInt ((values.Value [1] as ErlType).getValueString ());
				end = StringKit.toInt ((values.Value [2] as ErlType).getValueString ());
				NoticeManagerment.Instance.addRechargeTime (sid, new int[]{start,end});
			}
		}
	}
}
