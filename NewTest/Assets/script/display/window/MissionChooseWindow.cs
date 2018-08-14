using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MissionChooseWindow : WindowBase
{
	[HideInInspector]
	public int[] missionList;
	public Transform missionsParentPos;//副本创建父集
	public Transform bgPos;//底层背景图父集
	public Transform bgCenterPos;//中间层背景图父集
	public UIPanel panel;//副本容器
	public GameObject itemPrefab;
	public GameObject newItemRayPrefab;//新副本底纹
	public GameObject starNumObj;//星星兑换红底
	public UILabel awardNumLabel;//奖励可兑换数目
	public UILabel starNumLabel;//星星数量
	public UILabel fubenName;
	public UILabel moneyCount;//玩家金钱
	public UILabel pveValue;//行动力
	public UILabel storeValue;//行动力
	public GameObject[] pvpBars;
	public barCtrl pveBar;
	public barCtrl storeBar;
	public UITexture[] centerBackGround;//中间层背景图

	private List<MissionItem> itemList;
	private MissionItem lastItem;
	private int ChapterSid;
	/** 背景是否可以移动 */
	private bool isBgCanMove = true;
	private int numAwards;

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
			setBgIsCanMove (false);
			finishWindow ();
		} else if (gameObj.name == "luckyButton") {
			setBgIsCanMove (false);
			UiManager.Instance.openDialogWindow<MissionAwardWindow> ((win)=>{
				win.initWin(ChapterSid);
			});
		}
	}

	protected override void DoEnable ()
	{
		base.DoEnable ();
		setBgIsCanMove (false);
		UiManager.Instance.storyMissionWindow = this;
		for (int i = 0; i < missionsParentPos.childCount; i++) {
			Destroy (missionsParentPos.GetChild (i).gameObject);
		}
		refreshData ();
		UiManager.Instance.backGround.switchBackGround ("missionBG",()=>{setBgIsCanMove(true);});
		int imageSid = ChapterSampleManager.Instance.getChapterSampleBySid (ChapterSid).thumbIcon;
		if (imageSid == 0 || imageSid >= 13) {
			ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.MISSIONCHOOSE_PATH + "mission_bg" + UserManager.Instance.self.star,centerBackGround[0]);
			ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.MISSIONCHOOSE_PATH + "mission_bg" + UserManager.Instance.self.star,centerBackGround[1]);
		} else {
			ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.MISSIONCHOOSE_PATH + "mission_bg" + imageSid,centerBackGround[0]);
			ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.MISSIONCHOOSE_PATH + "mission_bg" + imageSid,centerBackGround[1]);
			centerBackGround[0].gameObject.SetActive (true);
			centerBackGround[1].gameObject.SetActive (true);
		}

	}

	public override void DoDisable ()
	{
		base.DoDisable ();
		UiManager.Instance.storyMissionWindow = null;
		UiManager.Instance.backGround.ReturnFromMissionChooseWindow ();
	}

	protected override void begin ()
	{
		base.begin ();
		createItem ();
		if (!isAwakeformHide) {
			jumpToMission();
		} else {
			jumpToMissionFromHide();
		}
		GuideManager.Instance.guideEvent ();
		StartCoroutine (Utils.DelayRun (()=>{
			MaskWindow.UnlockUI();
		},0.5f));
	}

	public void refreshData ()
	{
		ChapterSid = FuBenManagerment.Instance.selectedChapterSid;
		missionList = FuBenManagerment.Instance.getAllMissions (ChapterSid);
		fubenName.text = ChapterSampleManager.Instance.getChapterSampleBySid (ChapterSid).name;
		starNumLabel.text = FuBenManagerment.Instance.getMyMissionStarNum (ChapterSid) + "/" + FuBenManagerment.Instance.getAllMissionStarNum (ChapterSid);
		moneyCount.text = UserManager.Instance.self.getMoney ().ToString ();
		for(int i=0;i<pvpBars.Length;i++){
			if((i+1)<=UserManager.Instance.self.getPvPPoint ())pvpBars[i].SetActive(true);
			else pvpBars[i].SetActive(false);
		}
		updateBar();
		int num = FuBenManagerment.Instance.checkAwardCanGetNum (ChapterSid);
		numAwards = num;
		awardNumLabel.text = num.ToString();
		if (num > 0) {
			starNumObj.SetActive (true);
		} else {
			starNumObj.SetActive (false);
		}
	}
	/**更新坐骑或普通行动力 */
	private void updateBar(){
		pveBar.gameObject.SetActive(true);
	    pveBar.updateValue (UserManager.Instance.self.getPvEPoint (), UserManager.Instance.self.getPvEPointMax ());
		pveValue.text = UserManager.Instance.self.getTotalPvEPoint() + "/" + UserManager.Instance.self.getPvEPointMax ();
	}

	void createItem ()
	{
		if (itemList != null) {
			itemList.Clear ();
			itemList = null;
		}
		lastItem = null;
		itemList = new List<MissionItem> ();
		//解决部分机型高宽不分的BUG
		int width = Screen.width < Screen.height ? Screen.width : Screen.height;
		float y = 0;//每个副本点位之间的高度差
		float x = 0;//每一行X坐标
		float centerX = 0;//屏幕中间值
		GameObject a;
		for (int i = 0; i < missionList.Length; i++) {
			Mission _mission = MissionInfoManager.Instance .getMissionBySid (missionList [i]);
			//根据屏幕宽度来算中间随机的位置
			centerX = (width * UiManager.Instance.screenScaleX) / 2 - (width / 3);
			x = UnityEngine.Random.Range (-1*centerX, centerX);
			a = Instantiate (itemPrefab) as GameObject;
			MissionItem item = a.GetComponent<MissionItem> ();
			item.name = StringKit.intToFixString (i + 1);
			item.initButton (_mission, i + 1);
			item.fatherWindow = this;
			item.transform.parent = missionsParentPos;
			item.transform.localScale = Vector3.one;
			item.transform.localPosition = new Vector3 (x, y, 0);
			y += 300;

			if (FuBenManagerment.Instance.isNewMission (_mission.getChapterType (), _mission.sid) && FuBenManagerment.Instance.isCompleteLastMission (_mission.sid)) {
				lastItem = item;
				NGUITools.AddChild (lastItem.gameObject, newItemRayPrefab);
			}
			itemList.Add (item);
		}

		//这里是连线的
		for (int i = 0; i < itemList.Count; i++) {
			if (i + 1 >= itemList.Count) {
				break;
			}
			itemList[i].line.gameObject.SetActive (true);
			//计算两点之间的长度
			itemList[i].line.height = (int)Vector2.Distance(itemList[i].getPos(),itemList[i + 1].getPos());
			//计算两点之间的角度
			Vector3 targetDir = itemList[i + 1].line.transform.position - itemList[i].line.transform.position;
			Vector3 forward = transform.up;
			float angle = Vector3.Angle(targetDir, forward);
			if(itemList[i + 1].line.transform.position.x - itemList[i].line.transform.position.x > 0) {
				angle = -1 * angle;
			}
			itemList[i].line.transform.eulerAngles = new Vector3(0,0,angle);
		}
	}

	public void jumpToMission()
	{
		if (lastItem != null) {
			missionsParentPos.GetComponent<UICenterOnChild> ().CenterOn (lastItem.transform);
		} else {
			missionsParentPos.GetComponent<UICenterOnChild> ().CenterOn (itemList [itemList.Count - 1].transform);
		}
	}

	public void jumpToMissionFromHide()
	{
		MissionItem jumtoItem = null;
		for (int i = 0; i < itemList.Count; i++) {
			if (i + 1 >= itemList.Count) {
				break;
			}
			if (itemList[i].mission.sid == FuBenManagerment.Instance.selectedMissionSid) {
				int type = itemList[i].mission.getChapterType ();
				int nextSid = itemList[i + 1].mission.sid;
				string nextTypeDesc = itemList[i + 1].mission.getMissionType ();//下个副本状态
				string nowTypeDesc = itemList[i].mission.getMissionType ();//刚才打的副本状态

				if (nextTypeDesc == MissionShowType.NEW || nextTypeDesc == MissionShowType.COUNT_NOT_ENOUGH) {
					jumtoItem = itemList[i + 1];
				} else {
					jumtoItem = itemList[i];
				}
			}
		}
		if(jumtoItem == null && lastItem == null)
			missionsParentPos.GetComponent<UICenterOnChild> ().CenterOn (itemList [itemList.Count - 1].transform);
		else if(jumtoItem == null && lastItem != null)
			missionsParentPos.GetComponent<UICenterOnChild> ().CenterOn (lastItem.transform);
		else
			missionsParentPos.GetComponent<UICenterOnChild> ().CenterOn (jumtoItem.transform);
	}

	void Update ()
	{
		if (isBgCanMove) {
			float a = panel.clipOffset.y;
			//bgPos.localPosition = new Vector3 (0, -a / 10, 0);
			UiManager.Instance.backGround.UpdateMissionChooseBG (a);
			bgCenterPos.localPosition = new Vector3 (0, -a / 5, 0);
		}

	}

	/// <summary>
	/// 设置背景是否可以移动
	/// </summary>
	public void setBgIsCanMove (bool b)
	{
		isBgCanMove = b;
	}

	public MissionItem getLastItem ()
	{
		return lastItem;
	}

	public override void OnNetResume ()
	{
		base.OnNetResume ();
		MissionAwardWindow win = UiManager.Instance.getWindow<MissionAwardWindow>();
		if(win!=null)
		{
			win.OnNetResume();
			if(win.isOnNet&&numAwards>0)
			{
				awardNumLabel.text = (numAwards-1).ToString();
				if (numAwards > 0) {
					starNumObj.SetActive (true);
				} else {
					starNumObj.SetActive (false);
				}
			}
		}
		ChapterSelectWindow win_1 = UiManager.Instance.getWindow<ChapterSelectWindow>();
		if(win!=null)
			win_1.loadDataForOnNet();
	}
}
