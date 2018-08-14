using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ContentBossView : ContentBase
{
	public List<Mission>  missionList;
	public UISprite rightArrow;
	public UISprite leftArrow;
 
	public override void initAllButton (GameObject each)
	{
		base.initAllButton (each);
	}

	public void jumptToMission (Mission mis)
	{
		if (missionList == null)
			return;
		if (mis == null)
			return;
		for (int i=0; i< missionList.Count; i++) {
			if (missionList [i].sid == mis.sid) {
				jumpToPage (i + 1);
				return;
			}
		}
	}

	public void init (List<Mission>  list)
	{
		if (missionList != null)
			return;		
		missionList = list;
		init (missionList.Count);
	}

	public override void CreateButton (int index, GameObject page, int buttonIndex)
	{
		base.CreateButton (index, page, buttonIndex);
		ButtonBossView button = page.GetComponent<ButtonBossView> ();
		button.LockOnClick = false;
		button.fatherWindow = fatherWindow;	
		button.updateBoss (missionList [index]);
	}

	public override void updateActive (GameObject obj, int pageNUm)
	{
		base.updateActive (obj, pageNUm);
		ButtonBossView button = obj.GetComponent<ButtonBossView> ();
		Mission mis = missionList [pageNUm - 1];
		button.updateBoss (mis);
		setFaterWindowTitle (mis);
		TeamPrepareWindow faWnd = fatherWindow as TeamPrepareWindow;	
		if (faWnd.getMission () != missionList [pageNUm - 1]) {
			faWnd.setMissionByBossView (missionList [pageNUm - 1]);
		}
		if (pageNUm == 1) {
			leftArrow.alpha = 0;
		} else {
			leftArrow.alpha = 1;
		}
		if (pageNUm >= missionList.Count) {
			rightArrow.alpha = 0;
		} else {
			rightArrow.alpha = 1;
		}		
	}
	/// <summary>
	/// 设置标题
	/// </summary>
	public void setFaterWindowTitle(Mission mis)
	{
		if(mis!=null)
		{
			Card boss = CardManagerment.Instance.createCard (mis.getBossSid ());
			if (boss != null) 
				(fatherWindow as TeamPrepareWindow).setTitle (boss.getName ());
		}
	}
}
