using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RankWindow : WindowBase
{
	public TapContentBase tapContent;
	public RankLadderContent content_ladder;
	public RankContent content;
	/** 横拖碰撞体，暂时没用到 */
	public UIDragScrollView dragView;
	/** 我的排名 */
	public UILabel lblMyRank;
	/** 更新提示 */
	public UILabel lab_intro;
	/** 暂无数据 */
	public UILabel label_tip;
	/** 左箭头 */
	public UISprite leftArrow;
	/** 右箭头 */
	public UISprite rightArrow;
	/** 当前选中标签 */
	[HideInInspector]public int selectTabType = RankManagerment.TYPE_COMBAT;
	int myRank;
	int tapIndex;
	bool dragSwitch;
	UIPanel panel;
	/** 容器当前位置 */
	float nowX;

	protected override void DoEnable()
	{
		UiManager.Instance.backGround.switchBackGround("ChouJiang_BeiJing");
		if (MissionManager.instance != null)
			MissionManager.instance.hideAll ();
	}

	protected override void begin ()
	{
		if (!isAwakeformHide) {
			tapContent.changeTapPage (tapContent.tapButtonList [selectTabType - 1], true);
		}
		panel = tapContent.GetComponent<UIPanel> ();
		updateArrow ();
		MaskWindow.UnlockUI ();
	}


	private void initTap(int _type,CallBack _callback)
	{
		selectTabType = _type;

		if (_type == RankManagerment.TYPE_LADDER) {
			//dragView.scrollView=content_ladder.GetComponent<UIScrollView>();
			content.gameObject.SetActive (false);
			content_ladder.gameObject.SetActive (true);
		}
		else {
			label_tip.gameObject.SetActive (false);
			//dragView.scrollView=content.GetComponent<UIScrollView>();
			content.gameObject.SetActive (true);
			content_ladder.gameObject.SetActive (false);
			content_ladder.cleanAll ();
		}

		MaskWindow.LockUI ();
		RankManagerment.Instance.loadData (selectTabType, () => {
			_callback ();
		});
	}

	public override void tapButtonEventBase (GameObject gameObj, bool enable)
	{
		if (!enable)
			return;

		MaskWindow.LockUI ();
		TapButtonBase[] buttons = tapContent.tapButtonList;

		//大于5个才进行居中
		if (buttons.Length > 5) {
			//选中的居中,排除头尾首2个
			if (gameObj != buttons [0].gameObject && gameObj != buttons [1].gameObject &&
			    gameObj != buttons [buttons.Length - 1].gameObject && gameObj != buttons [buttons.Length - 2].gameObject) {
				SpringPanel.Begin (tapContent.gameObject, new Vector3 (-gameObj.transform.localPosition.x, tapContent.transform.localPosition.y, tapContent.transform.localPosition.z), 9);
			}
			else if (gameObj == buttons [buttons.Length - 1].gameObject || gameObj == buttons [buttons.Length - 2].gameObject) {
				GameObject tempObj = buttons [buttons.Length - 3].gameObject;
				SpringPanel.Begin (tapContent.gameObject, new Vector3 (-tempObj.transform.localPosition.x, tapContent.transform.localPosition.y, tapContent.transform.localPosition.z), 9);
			}
			else if (gameObj == buttons [0].gameObject || gameObj == buttons [1].gameObject) {
				GameObject tempObj = buttons [2].gameObject;
				SpringPanel.Begin (tapContent.gameObject, new Vector3 (-tempObj.transform.localPosition.x, tapContent.transform.localPosition.y, tapContent.transform.localPosition.z), 9);
			}
		}

		updateArrow ();

		if (gameObj == tapContent.tapButtonList [0].gameObject) {
			initTap (RankManagerment.TYPE_COMBAT, () => {

				myRank = RankManagerment.Instance.getMyRank (RankManagerment.TYPE_COMBAT);
				content.init (RankManagerment.TYPE_COMBAT, RankManagerment.Instance.combatList, this);
				showMyRank (string.Empty);
				lab_intro.gameObject.SetActive (true);				
				tapIndex = 0;
			});
		} else if (gameObj == tapContent.tapButtonList [1].gameObject) {
			initTap (RankManagerment.TYPE_PVP, () => {

				myRank = RankManagerment.Instance.getMyRank (RankManagerment.TYPE_PVP);
				content.init (RankManagerment.TYPE_PVP, RankManagerment.Instance.pvpList, this);
				showMyRank (string.Empty);
				lab_intro.gameObject.SetActive (true);			
				tapIndex = 1;
			});
		} else if (gameObj == tapContent.tapButtonList [2].gameObject) {
			initTap (RankManagerment.TYPE_ROLE, () => {

				myRank = RankManagerment.Instance.getMyRank (RankManagerment.TYPE_ROLE);
				content.init (RankManagerment.TYPE_ROLE, RankManagerment.Instance.roleList, this);
				showMyRank (string.Empty);
				lab_intro.gameObject.SetActive (true);			
				tapIndex = 2;
			});
		} else if (gameObj == tapContent.tapButtonList [3].gameObject) {
			initTap (RankManagerment.TYPE_MONEY, () => {

				myRank = RankManagerment.Instance.getMyRank (RankManagerment.TYPE_MONEY);
				content.init (RankManagerment.TYPE_MONEY, RankManagerment.Instance.moneyList, this);
				showMyRank (string.Empty);
				lab_intro.gameObject.SetActive (true);
				tapIndex = 3;
			});
		} else if (gameObj == tapContent.tapButtonList [5].gameObject) {
			initTap (RankManagerment.TYPE_ROLE_LV, () => {
				myRank = RankManagerment.Instance.getMyRank (RankManagerment.TYPE_ROLE_LV);
				content.init (RankManagerment.TYPE_ROLE_LV, RankManagerment.Instance.roleLvList, this);
				showMyRank (string.Empty);
				lab_intro.gameObject.SetActive (true);
				tapIndex = 5;
			});
		} else if (gameObj == tapContent.tapButtonList [6].gameObject) {
			initTap (RankManagerment.TYPE_GODDESS, () => {
				myRank = RankManagerment.Instance.getMyRank (RankManagerment.TYPE_GODDESS);
				content.init (RankManagerment.TYPE_GODDESS, RankManagerment.Instance.goddessList, this);
				showMyRank (string.Empty);
				lab_intro.gameObject.SetActive (true);	
				tapIndex = 6;
			});
		} else if (gameObj == tapContent.tapButtonList [7].gameObject) {
			initTap (RankManagerment.TYPE_LADDER, () => {

				myRank = RankManagerment.Instance.getMyRank (RankManagerment.TYPE_LADDER);
				content_ladder.fatherWindow = this;
				content_ladder.reLoad (RankManagerment.Instance.ladderList);

				if (RankManagerment.Instance.ladderList.Count > 0) {
					label_tip.gameObject.SetActive (false);
				} else {
					label_tip.gameObject.SetActive (true);
				}
				showMyRank (string.Empty);
				lab_intro.gameObject.SetActive (true);

				tapIndex = 7;
				MaskWindow.UnlockUI ();
			});
		} else if (gameObj == tapContent.tapButtonList [8].gameObject) {
			initTap (RankManagerment.TYPE_GUILD_FIGHT, () => {
				myRank = RankManagerment.Instance.getMyRank (RankManagerment.TYPE_GUILD_FIGHT);
				content.init (RankManagerment.TYPE_GUILD_FIGHT, RankManagerment.Instance.guildFightJudgeList, this);
				showMyRank (string.Empty);
				lab_intro.gameObject.SetActive (true);	
				tapIndex = 8;
			});
		}

		//公会特殊
		if (gameObj == tapContent.tapButtonList [4].gameObject) {
			//显示我的排名
			string str = "";
			initTap(RankManagerment.TYPE_GUILD,()=>{
				if (GuildManagerment.Instance.getGuild () == null) {
					myRank = 0;
					str = LanguageConfigManager.Instance.getLanguage ("s0417");
				} else {
					myRank = RankManagerment.Instance.getMyGuildRank ();
					if (myRank == 0)
						str = string.Format (LanguageConfigManager.Instance.getLanguage ("s0415"), 100);
					else
						str = string.Format (LanguageConfigManager.Instance.getLanguage ("s0416"), myRank);
				}
				content.init (RankManagerment.TYPE_GUILD, RankManagerment.Instance.guildList,this);
				showMyRank(str);
				tapIndex = 4;
			});
		}
	}

	private void showMyRank(string rank)
	{
		if (myRank == 0)
			rank = LanguageConfigManager.Instance.getLanguage ("rankWindow01");
		else {
			if(selectTabType == RankManagerment.TYPE_GUILD_FIGHT)
				rank = LanguageConfigManager.Instance.getLanguage ("GuildArea_48", myRank.ToString());
			else
				rank = string.Format (LanguageConfigManager.Instance.getLanguage ("s0414"), myRank);

		}
		lblMyRank.text = rank;

		if (selectTabType == RankManagerment.TYPE_LADDER) {
			lab_intro.text = Language ("rankWindow_intro04");
		} else if (selectTabType == RankManagerment.TYPE_GUILD_FIGHT) {
			lab_intro.text = Language("rankWindow_intro01");
		}
		else {
			lab_intro.text = Language ("rankWindow_intro01");
		}
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
			finishWindow ();
			ChatManagerment.Instance.chatCard = null;
			if (fatherWindow is MissionMainWindow) {
				EventDelegate.Add (OnHide, () => {
					PvpInfoManagerment.Instance.setPvpType(PvpInfo.TYPE_PVP_FB);
					UiManager.Instance.openDialogWindow<PvpInfoWindow> ();
				});

			}
			if (MissionManager.instance != null)
			{
				MissionManager.instance.showAll ();
				MissionManager.instance.setBackGround();
			}
		}else if(gameObj.name == "root_head")
		{
			PvpOppInfo playerData=gameObj.transform.parent.GetComponent<RankLadderItemView>().data;
			M_onGetPlayerInfoCmp(playerData);
			/*
			string uid=gameObj.transform.parent.GetComponent<RankLadderItemView>().data.uid;
			LaddersGetPlayerInfoFPort fport=FPortManager.Instance.getFPort ("LaddersGetPlayerInfoFPort") as LaddersGetPlayerInfoFPort;
			fport.access(uid,M_onGetPlayerInfoCmp);
			*/
		}
	}
	private void M_onGetPlayerInfoCmp(PvpOppInfo _playerInfo)
	{
		PvpPlayerWindow.comeFrom=PvpPlayerWindow.FROM_RANK;
		UiManager.Instance.openWindow<PvpPlayerWindow> (
			(win) => {
			win.teamType =10;
			win.initInfo (_playerInfo);
		});
	}

	/// <summary>
	/// 获取当前显示的排行榜中我的排名
	/// </summary>
	public int getMyRankWithShow ()
	{
		return myRank;
	}
	/// <summary>
	/// 更新箭头状态
	/// </summary>
	void updateArrow ()
	{
		if (panel != null) {
			if (panel.clipOffset.x >= 330) {
				leftArrow.gameObject.SetActive (true);
				rightArrow.gameObject.SetActive (false);
			}
			else if (panel.clipOffset.x <= 0) {
				leftArrow.gameObject.SetActive (false);
				rightArrow.gameObject.SetActive (true);
			}
			else {
				leftArrow.gameObject.SetActive (true);
				rightArrow.gameObject.SetActive (true);
			}
			nowX = panel.clipOffset.x;
		}
	}

	void Update ()
	{
		if (panel != null && nowX != panel.clipOffset.x) {
			updateArrow ();
		}
	}

	//横向拖动,不方便，暂时斩了
//    public void OnDrag (Vector2 delta)
//    {
//        if (dragSwitch)
//            return;
//        int toIndex = tapIndex;
//        if (delta.x < -80)
//            toIndex++;
//        else if (delta.x > 80)
//            toIndex--;
//        if (toIndex != tapIndex && toIndex >= 0 && toIndex < tapContent.tapButtonList.Length)
//        {
//            tapContent.changeTapPage(tapContent.tapButtonList[toIndex],true);
//
//            dragSwitch = true;
//            StartCoroutine(Utils.DelayRun(()=>{
//                dragSwitch = false;
//            },0.5f));
//        }
//    }


}
