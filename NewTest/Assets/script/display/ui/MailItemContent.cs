using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MailItemContent : dynamicContent
{
	List<Mail> mails;
	public Timer timer;//计时器
	
	public void reLoad(List<Mail> _mails)
	{ 
		mails = _mails;
		timer = TimerManager.Instance.getTimer (UserManager.TIMER_DELAY);
		timer.addOnTimer (updateTime);
		timer.start (); 
		float y = transform.localPosition.y;
		base.reLoad(mails.Count);
		StartCoroutine(Utils.DelayRunNextFrame(()=>{
			base.jumpToPos(y);
		}));
	}
	
	public override void updateItem (GameObject item, int index)
	{
	//	base.updateItem (item, index);
		
		ButtonMailItem button = item.GetComponent<ButtonMailItem> ();
		if(index > mails.Count - 1)
			return;
		button.initialize (mails [index]); 
	}

	public override void initButton (int  i)
	{
		if (nodeList [i] == null) {
			nodeList [i] = NGUITools.AddChild (gameObject, (fatherWindow as MailWindow).mailItemPrefab);
		}
		
		nodeList [i].name = StringKit. intToFixString (i + 1);
		ButtonMailItem button = nodeList [i].GetComponent<ButtonMailItem> ();
		button.fatherWindow = fatherWindow; 
		button.initialize(mails[i]);
	}
	//更新所有button邮件时间
	public void updateTime()
	{
		for(int i=0;i<nodeList.Count;i++)
		{
			if (nodeList [i] == null)continue;
			ButtonMailItem button = nodeList [i].GetComponent<ButtonMailItem> ();
			button.updateTime();
		}
	}
		
}
