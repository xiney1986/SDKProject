using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NoticeItemContent : dynamicContent
{ 
	Notice[] notices;
	 
	public void Initialize (Notice[] _notices)
	{
		notices = _notices;
		base.reLoad (notices.Length); 
	}
	public void reLoad(Notice[] _notices)
	{
		notices = _notices;
		base.reLoad(notices.Length);
	}
	
	public override void updateItem (GameObject item, int index)
	{
		base.updateItem (item, index);
		
		ButtonNoticeItem button = item.GetComponent<ButtonNoticeItem> ();
		button.updateNotice (notices [index]); 
	}

	public override void initButton (int  i)
	{
		NoticeSample sample = NoticeSampleManager.Instance.getNoticeSampleBySid(notices[i].sid);
		if(sample.type == NoticeType.STICKNOTICE)
		{
//			if (nodeList [i] == null){
//				nodeList [i] = NGUITools.AddChild (gameObject, (fatherWindow as NoticeWindow).noticeTopItemPrefab);
//			}
		}
		else
		{
//			if (nodeList [i] == null){
//				nodeList [i] = NGUITools.AddChild (gameObject, (fatherWindow as NoticeWindow).noticeCommonItemPrefab);
//			}
		}
 
		nodeList [i].name = StringKit. intToFixString (i + 1);
		ButtonNoticeItem button = nodeList [i].GetComponent<ButtonNoticeItem> ();
		button.fatherWindow = fatherWindow; 
		button.Initialize(notices[i]);
	}
}
