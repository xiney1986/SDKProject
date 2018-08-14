using UnityEngine;
using System.Collections;

/**
 * 获得所有邮件接口
 * @author 汤琦
 * */
public class MailGetFPort : BaseFPort
{ 
	private CallBack callback;
	
	public void access (CallBack callback)
	{   
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.MAIL_GET);
		access (message);
	}

	public override void read (ErlKVMessage message)
	{
		parseKVMsg (message);
		if (callback != null) {
			callback ();
		}
	}
	//解析ErlKVMessgae
	public void parseKVMsg (ErlKVMessage message)
	{
		ErlArray list = message.getValue ("msg") as ErlArray; 
		if (list == null)
			return;
		MailManagerment.Instance.clearMail ();//先清理邮箱
		for (int i=0; i<list.Value.Length; i++) {
			ErlList conList = list.Value [i]as ErlList;
			int index = 0;
			string uid = "0";
			int src = 0;
			int type = 0;
			int stime = 0;
			int etime = 0;
			string theme;
			string content;
			Annex[] annex = null;
			int state = 0;
			uid = ((conList.Value [index] as ErlArray).Value [1]).getValueString ();
			src = StringKit.toInt (((conList.Value [++index] as ErlArray).Value [1]).getValueString ());
			type = StringKit.toInt (((conList.Value [++index] as ErlArray).Value [1]).getValueString ());
			stime = StringKit.toInt (((conList.Value [++index] as ErlArray).Value [1]).getValueString ());
			etime = StringKit.toInt (((conList.Value [++index] as ErlArray).Value [1]).getValueString ());
			theme = ((conList.Value [++index] as ErlArray).Value [1]).getValueString ();
			content = ((conList.Value [++index] as ErlArray).Value [1]).getValueString ();
			ErlList listc = (conList.Value [++index] as ErlArray).Value [1] as ErlList;
			annex = MailManagerment.Instance.parseAnnex(listc);
			state = StringKit.toInt (((conList.Value [++index] as ErlArray).Value [1]).getValueString ());
			MailManagerment.Instance.createMail (uid, src, type, stime, etime, theme, content, annex, state);
		}
	}
}
