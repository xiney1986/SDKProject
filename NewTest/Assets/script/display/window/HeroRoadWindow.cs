using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeroRoadWindow : WindowBase {
	
	public UIScrollView scrollView;
	public UIPanel scrollPanel;
	public GameObject scrollContent;
	public GameObject itemPrefab;
	public TapContentBase tapContent;
	public UILabel[] tabNums;
	public UILabel labelTimes;
	private Card fromCard;
	private CallBack closeCallback;
	private int showType;
	private HeroRoadItem needOpenItem;
	private bool firstSelectTab = true;
	private int lastSelectQuality ;
	private float updateTimesDelay;
	public HeroRoadItem selectItem;
	private int  itemIndex = 0;
	public string currentTab;

	protected override void begin ()
	{
		base.begin ();
		StartCoroutine (UpdateTimes());
		ShowNum ();
		HeroRoadManagerment.Instance.clean();


		if (lastSelectQuality > 0) {
			Refresh ();
			MaskWindow.UnlockUI();
			return;
		}

		int checkTabIndex = 0;
		
		if (fromCard != null) {
			checkTabIndex = HeroRoadSampleManager.Instance.getSampleBySid (fromCard.getEvolveNextSid ()).quality - 1;
		} else if (HeroRoadManagerment.Instance.currentHeroRoad != null) {
			checkTabIndex = HeroRoadManagerment.Instance.currentHeroRoad.sample.quality - 1;
		} else {
			checkTabIndex = getCanBeChallengingTap();
		}
		tapContent.resetTap ();
		tapContent.changeTapPage (tapContent.tapButtonList [checkTabIndex]);
		GuideManager.Instance.guideEvent();
		MaskWindow.UnlockUI ();
		if(BattleManager.battleData != null)
		{
			BattleManager.battleData.isHeroRoad = false;
		}
	}

	public override void OnNetResume ()
	{
		base.OnNetResume ();
		ShowNum ();
		Refresh();
		if(BattleManager.battleData != null)
		{
			BattleManager.battleData.isHeroRoad = false;
		}
	}
	public int getCanBeChallengingTap()
	{
		for (int i = 0; i <5; i++) {
			int count = HeroRoadManagerment.Instance.getCanBeChallengingTimesByQuality(i+1);
			if(count > 0)
				return i;
		}
		return 0;
	}

	private void ShowNum()
	{
		for (int i = 0; i <5; i++) {
			int count = HeroRoadManagerment.Instance.getCanBeChallengingTimesByQuality(i+1);
			if(count > 0)
			{
				tabNums[i].transform.parent.gameObject.SetActive(true);
				tabNums[i].text = count.ToString();
			}
			else
			{
				tabNums[i].transform.parent.gameObject.SetActive(false);
			}
		}
	}
	
	public void init(Card fromCard,CallBack closeCallback,int showType)
	{
		this.fromCard = fromCard;
		this.closeCallback = closeCallback;
		this.showType = showType;

	}

	public void Refresh()
	{
		int index = lastSelectQuality - 1;
		lastSelectQuality = 0;
		if (index >= 0) {
			tapContent.resetTap ();
			tapContent.changeTapPage (tapContent.tapButtonList [index]);
		}
	}

	public override void tapButtonEventBase (GameObject gameObj, bool enable)
	{
		if (!enable)
			return;
		
		int quality = StringKit.toInt (gameObj.name);
		if (lastSelectQuality == quality)
			return;
		lastSelectQuality = quality;
		Utils.DestoryChilds (scrollContent, null, null);
		List<HeroRoad> list = HeroRoadManagerment.Instance.getHeroRoadListByQuality (quality, fromCard == null ? 0 : fromCard.getEvolveNextSid ());
		HeroRoadItem lastItem = null;
		for (int i = 0; i < list.Count; i++) {
			GameObject obj = NGUITools.AddChild (scrollContent, itemPrefab);
			obj.SetActive (true);
			obj.transform.localPosition = new Vector3 (0, i * -235, 0);
			obj.name = i.ToString ();
			HeroRoadItem item = obj.GetComponent<HeroRoadItem> ();
			item.init (list [i]);
			if (lastItem != null)
				lastItem.next = item;
			lastItem = item;
			ButtonBase btn = item.buttonFight.GetComponent<ButtonBase> ();
			if (btn.exFields == null) {
				btn.exFields = new Hashtable ();
			}
			
			//			Vector3 localpoint = new Vector3(item.transform.localPosition.x,item.transform.localPosition.y,item.transform.localPosition.z);
			btn.exFields.Add ("index", i);
			btn.exFields.Add ("tabName", gameObj.name);
			if (firstSelectTab && ((fromCard != null && list [i].sample.sid == fromCard.getEvolveNextSid ()) ||
			                       (HeroRoadManagerment.Instance.currentHeroRoad != null && 
			 HeroRoadManagerment.Instance.currentHeroRoad.sample.sid == list [i].sample.sid))) {
				needOpenItem = item;
				firstSelectTab = false;
			}
		}
		scrollView.ResetPosition ();
		
		if (needOpenItem != null) {
			openItemDetail (needOpenItem);
			needOpenItem = null;
		}
		if(currentTab != gameObj.name){
			itemIndex = 0;
		}
		if (itemIndex != 0 && currentTab == gameObj.name) {
			Transform jumpItem = null;
			Transform[] items = new Transform[scrollContent.transform.childCount];
			foreach (Transform tmp in scrollContent.transform) {
				int index = (int)tmp.GetComponent<HeroRoadItem> ().buttonFight.GetComponent<ButtonBase> ().exFields ["index"];
				items [index] = tmp;
			}
			if (items [itemIndex] != null) {
				jumpItem = items [itemIndex - 1];
			}else  {
				jumpItem = items [itemIndex - 2];
			}
			if (jumpItem != null) {
				openItemDetail (jumpItem.localPosition);
			}
		}
	}
	public void openItemDetail(Vector3 point)
	{
		SpringPanel.Begin(scrollView.gameObject, -point, 6f);
		return;
	}

	public void openItemDetail(HeroRoadItem item)
	{
		SpringPanel.Begin(scrollView.gameObject, -item.transform.localPosition, 6f);
		return;
	}



	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
			HeroRoadManagerment.Instance.clean();
				finishWindow();
		}else if(gameObj.name=="button_fight")
		{
			//进入英雄之章不进行是否在副本中检查
			/*
			if(SweepManagement.Instance.hasSweepMission)
			{
				UiManager.Instance.openDialogWindow<MessageWindow>((win)=>{
					win.initWindow(1,LanguageConfigManager.Instance.getLanguage("s0093"),null,LanguageConfigManager.Instance.getLanguage("sweepTip_09"),null);
				});
			}else
			*/
			//{
				HeroRoadItem currentItem=gameObj.transform.parent.gameObject.GetComponent<HeroRoadItem>();
				currentItem.OnBtnFightClick();
				itemIndex = (int)gameObj.GetComponent<ButtonBase>().exFields["index"];
				currentTab = gameObj.GetComponent<ButtonBase>().exFields["tabName"].ToString();
			//}
			GuideManager.Instance.doGuide();
			GuideManager.Instance.guideEvent();
		}
	
	}

	protected override void DoEnable ()
	{
		base.DoEnable ();
		UiManager.Instance.backGround.switchBackGround("ChouJiang_BeiJing");
		Utils.DestoryChilds (scrollContent, null, null);
		//UiManager.Instance.backGroundWindow.switchToDark();
	}

	IEnumerator UpdateTimes()
	{
		int num = UserManager.Instance.self.getChvPoint ();
		int max = UserManager.Instance.self.getChvPointMax ();
		string timeStr = UserManager.Instance.getNextChvTime ();
		string t = "";
		if (string.IsNullOrEmpty (timeStr))
			t = num >= max ? "" : "";
		else
			t = num >= max ? "" : "( " + UserManager.Instance.getNextChvTime ().Substring (3) + " )";

		labelTimes.text = string.Format (LanguageConfigManager.Instance.getLanguage ("s0406"), num, max, t);

		yield return new WaitForSeconds(1f);
		StartCoroutine(UpdateTimes());
	}

	public override void DoDisable ()
	{
		base.DoDisable ();

	}
}
