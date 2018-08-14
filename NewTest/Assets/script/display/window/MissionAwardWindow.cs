using UnityEngine;
using System.Collections.Generic;

public class MissionAwardWindow : WindowBase {

	private Mission mission;
	private int ChapterSid;
	private ChapterAwardSample[] award;
	private List<MissionAwardItem> items;//所有奖励实体项
	int myStar;
//	public MissionAwardItem[] awardItemList;
	public UILabel starLabel;
	public GameObject awardItemPref;
	public GameObject awardContent;
	/// <summary>
	/// 领取过的奖励（处理断线重连）
	/// </summary>
	MissionAwardItem awardedItem;
	[HideInInspector]
	public bool isOnNet = false;
	protected override void DoEnable ()
	{
		base.DoEnable ();
		if (UiManager.Instance.backGround != null)
			UiManager.Instance.backGround.switchBackGround ("ChouJiang_BeiJing");
	}

	protected override void begin ()
	{
		base.begin ();
		jumpToItem ();
	}

	public void initWin(int _ChapterSid)
	{
		ChapterSid = _ChapterSid;
		showUI();
	}

	void showUI()
	{
		award = ChapterSampleManager.Instance.getChapterSampleBySid (ChapterSid).prizes;
		int[] myAwards = FuBenManagerment.Instance.getAwardSidsByChapterSid(ChapterSid);
		myStar = FuBenManagerment.Instance.getMyMissionStarNum (ChapterSid);
		items = new List<MissionAwardItem>();
		for (int i=0; i<award.Length; i++) {
			bool isGet = false;
			GameObject obj = NGUITools.AddChild(awardContent,awardItemPref);
			obj.SetActive(true);
			obj.name = obj.name + "_" + i;
			if (myAwards != null && myAwards.Length > 0) {
				for (int j = 0; j < myAwards.Length; j++) {
					if(myAwards[j] == award[i].awardSid){
						isGet = true;
						break;
					}
				}
			}
			MissionAwardItem item = obj.GetComponent<MissionAwardItem>();
			item.updateAwardItem(award[i],this,isGet,myStar);
			items.Add (item);
		}
		awardContent.transform.GetComponent<UIGrid> ().repositionNow = true;
		starLabel.text = myStar + "/" + FuBenManagerment.Instance.getAllMissionStarNum (ChapterSid);
	}

	private void jumpToItem ()
	{
		if (items == null || items.Count == 0) {
			MaskWindow.UnlockUI ();
			return;
		}
		MissionAwardItem item = null;
		for (int i = 0; i < items.Count; i++) {
			if (myStar >= items[i].getNeedStar () && !items[i].getIsGet ()) {
				if (item == null) {
					item = items[i];
				} else if (item != null && item.getNeedStar () >= items[i].getNeedStar ()) {
					item = items[i];
				}
			}
		}
		if (item != null) {
			SpringPanel.Begin (awardContent.gameObject, -item.transform.localPosition, 9);
		}
		MaskWindow.UnlockUI ();
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		MaskWindow.UnlockUI();

		if (gameObj.name == "close") { 
			finishWindow();
		} else if (gameObj.name == "awardButton") {
			string[] strs = gameObj.transform.parent.name.Split(new char[]{'_'});
            cas = award[StringKit.toInt(strs[1])];
			awardedItem = gameObj.transform.parent.GetComponent<MissionAwardItem>();
			isOnNet = true;
			getAward(award[StringKit.toInt(strs[1])].awardSid,awardedItem);
			 
		}
	}

    ChapterAwardSample cas;
	void getAward(int awardSid,MissionAwardItem awardItem)
	{
		MissionAwardFPort fport = FPortManager.Instance.getFPort ("MissionAwardFPort") as MissionAwardFPort;
		fport.getMissionAward(ChapterSid,awardSid,() => {
			changeAwardButtonState(awardItem);
		});
	}

	private void changeAwardButtonState(MissionAwardItem awardItem){
		awardItem.receivedSprite.gameObject.SetActive (true);
		awardItem.awardButton.gameObject.SetActive (false);
        UiManager.Instance.createPrizeMessageLintWindow(cas.prizes);
		MissionChooseWindow fw = this.GetFatherWindow() as MissionChooseWindow;
		fw.refreshData();
	}

	public override void OnNetResume ()
	{
		base.OnNetResume ();
		if(isOnNet)
			changeAwardButtonState(awardedItem);
	}
}
