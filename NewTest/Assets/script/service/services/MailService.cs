using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 邮件服务
 * */
public class MailService : BaseFPort
{
	public MailService ()
	{
		
	}
	
	public override void read (ErlKVMessage message)
	{
		if (message.getValue ("add_mail") != null) {
			ErlType mailType = message.getValue ("add_mail") as ErlType;
			ErlList conList = mailType as ErlList;
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
			GameObject mainWin = GameObject.Find ("MainWindow");
			GameObject win = GameObject.Find ("MailWindow");
			if (mainWin != null) {
//				UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage("newMail01"));
				(mainWin.GetComponent<MainWindow> ()).showMailNum ();
			} else if (win != null) {
				(win.GetComponent<MailWindow> ()).Initialize (0);
				(win.GetComponent<MailWindow> ()).mailContent.reLoad (MailManagerment.Instance.getSortAllMail ());
				(win.GetComponent<MailWindow> ()).changeButton (true);
			}

		}
		if (message.getValue ("del_mail") != null) {
			ErlType erlType = message.getValue ("del_mail") as ErlType;
			ErlList array = erlType as ErlList;
			for (int i = 0; i < array.Value.Length; i++) {
				MailManagerment.Instance.addNeedDelUids ((array.Value [i] as ErlString).getValueString ());
			}
			MailManagerment.Instance.runDeleteUids ();
			GameObject mainWin = GameObject.Find ("MainWindow");
			if (mainWin != null)
				(mainWin.GetComponent<MainWindow> ()).showMailNum ();
		}
	}
}
