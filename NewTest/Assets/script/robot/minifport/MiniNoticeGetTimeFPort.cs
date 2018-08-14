using UnityEngine;
using System.Collections;

/**
 * 公告获取活动条目时间
 * @author huangzhenghan
 * */
public class MiniNoticeGetTimeFPort : MiniBaseFPort
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
	}
}
