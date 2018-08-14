using UnityEngine;
using System.Collections;

/**
 * PVP玩家信息窗口
 * @author 汤琦
 * */
public class PvpPlayerWindow : WindowBase
{
	public barCtrl [] hpViews;
	private PvpOppInfo oppItem;
	private ArenaFinalInfo fin;
//	private RankItem rankItem;
	public ButtonPvpInfo buttonInfo;
	public ButtonBase applyButton;
	public ButtonBase deleteButton;
	public ButtonBase ladders_challenge;
	public ButtonBase ladders_applyHelp;
	public ButtonBase ladders_close;
	public ButtonBase guild_area_challenge;
	public FriendsChatButton fcbutton;
	public UITexture  headIcon;
	public GameObject cardViewPrefab;
	/** 其他 */
	public const int FROM_OTHER = -1;
	/** 聊天 */
	public const int FROM_CHAT = 0;
	/** 排行榜 */
	public const int FROM_RANK = 1;
	/** 副本NPC */
	public const int FROM_MISSION_NPC = 2;
	/** 查看好友 */
	public const int FROM_FRIENDLOOK = 3;
	/** 公会 */
	public const int FROM_UNION = 3;
	/** 竞技场 */
	public const int FROM_ARENA = 4;
	/** 天梯 */
	public const int FROM_LADDERS = 5;
	/** 天梯好友 */
	public const int FROM_LADDERS_FRIEND = 6;
	/** 公会战玩家预览(不能挑战) */
	public const int FROM_GUILD_AREA = 7;
	/** 公会战玩家预览(能挑战) */
	public const int FROM_GUILD_AREA_CHALLENGE = 8;
	public static int comeFrom = FROM_OTHER;
	private const int USER_EXP_SID = 1;//玩家经验索引
	public const int PVE_TEAM=1;//冒险队伍标记
	public const int PVP_TEAM=3;//竞技队伍标记
	public const int PVE_TEAM_TAPE=5;//五人队
	public const int PVP_TEAM_TYPE=10;//十人队伍
	private CallBack callback;
	private CallBack<bool> reLoadFriendWin;
	private string uid;
	private bool isOperateFriend;//是否操作了好友相关
	public int teamType;//队伍类型
	private LaddersPlayerInfo mCurrentLaddersPlayer;

	public LaddersPlayerInfo currentLaddersPlayer {
		set {
			mCurrentLaddersPlayer = value;
		}
	}

	public override void OnStart ()
	{
		base.OnStart ();
	}

	private void setNormalButtonVisible (bool _visible)
	{
		applyButton.gameObject.SetActive (_visible);
		deleteButton.gameObject.SetActive (_visible);
		buttonInfo.root_nomal.SetActive (_visible);
	}

	private void setLaddersOppButtonVisible (bool _visible)
	{
		ladders_applyHelp.gameObject.SetActive (_visible);
		ladders_challenge.gameObject.SetActive (_visible);
	}

	private void setLaddersFriendButtonVisible (bool _visible)
	{
		//ladders_close.gameObject.SetActive(_visible);
	}

	protected override void begin ()
	{
		if (isAwakeformHide) {
			if (comeFrom == FROM_LADDERS) {
				FPortManager.Instance.getFPort<LaddersGetInfoFPort> ().apply ((bo)=>{
					if (bo) {
						updateView_ladders ();
					}
				});
			}
			MaskWindow.UnlockUI ();
			return;
		}
		buttonInfo.combatBg.spriteName = "allCombat";
		switch (comeFrom) {
		case FROM_LADDERS:
			buttonInfo.root_ladders.SetActive (true);
			buttonInfo.root_nomal.SetActive (false);
			setNormalButtonVisible (false);
			setLaddersOppButtonVisible (true);
			setLaddersFriendButtonVisible (false);
			break;
		case FROM_LADDERS_FRIEND:
			buttonInfo.root_ladders.SetActive (true);
			buttonInfo.root_nomal.SetActive (false);
			//buttonInfo.label_applyHelpCount.gameObject.SetActive (false);
			setNormalButtonVisible (false);
			setLaddersOppButtonVisible (false);
			setLaddersFriendButtonVisible (true);
			break;
		case FROM_GUILD_AREA:
			guild_area_challenge.gameObject.SetActive(false);
			buttonInfo.root_nomal.SetActive (true);
			buttonInfo.combatBg.gameObject.SetActive(false);
			buttonInfo.combat.gameObject.SetActive(false);
			break;

		case FROM_GUILD_AREA_CHALLENGE:
			guild_area_challenge.gameObject.SetActive(true);
			buttonInfo.root_nomal.SetActive (true);
			buttonInfo.combatBg.gameObject.SetActive(false);
			buttonInfo.combat.gameObject.SetActive(false);
			break;
		default:
			buttonInfo.root_ladders.SetActive (false);
			buttonInfo.root_nomal.SetActive (true);
			setNormalButtonVisible (true);
			setLaddersOppButtonVisible (false);
			setLaddersFriendButtonVisible (false);
			break;
		}		

		base.begin ();
		if (oppItem != null) {
			uid = oppItem.uid;
			FormationSample sample = FormationSampleManager.Instance.getFormationSampleBySid (oppItem.formation);
			buttonInfo.initInfo (oppItem,this);

			if(sample != null)
				loadFormationGB (buttonInfo, sample.getLength (), buttonInfo.gameObject);
			else
				teamType = PVP_TEAM_TYPE;

			CreateFormation (buttonInfo, oppItem);


			if (comeFrom == FROM_LADDERS) {
				updateView_ladders ();
			} else if (comeFrom == FROM_LADDERS_FRIEND) {
				udpateView_ladders_friend ();
			}else if(comeFrom == FROM_GUILD_AREA || comeFrom == FROM_GUILD_AREA_CHALLENGE){
				updateView_guildFight();
			}
			else {
				updateView_normal ();
			}
		}

		MaskWindow.UnlockUI ();
	}


	private bool showMainCombat=true;//当前是否显示主力战斗力，主力和全部战斗力 来回切换标记
	void Update()
	{
		if(Time.frameCount%250==0&&teamType==10)
		{
			showMainCombat=!showMainCombat;
			if(showMainCombat)
			{
				buttonInfo.combat.text=oppItem.allCombat.ToString();
				buttonInfo.combatBg.spriteName = "allCombat";
			}else
			{
				buttonInfo.combat.text=oppItem.combat.ToString();
				buttonInfo.combatBg.spriteName = "mainCombat";
			}
		}
	}
    protected override void DoEnable() {
        //2014.7.2 9:50 modified
        base.DoEnable();
        UiManager.Instance.backGround.switchBackGround("ChouJiang_BeiJing");
        //UiManager.Instance.backGroundWindow.switchToDark();
        if (MissionManager.instance != null)
            MissionManager.instance.hideAll();
    }

	void setSex (string str)
	{
		buttonInfo.sexSprite.gameObject.SetActive (true);
		buttonInfo.sexSprite.spriteName = str;

	}
	void setSexLadders (string str)
	{
		buttonInfo.ladders_sexSprite.gameObject.SetActive (true);
		buttonInfo.ladders_sexSprite.spriteName = str;
		
	}
	private void updateView_normal ()
	{
		buttonInfo.playerName.text = oppItem.name;

		if (oppItem.sdkInfo != null)
			setSex (oppItem.sdkInfo.sex);

		buttonInfo.guildName.text = Language ("pvpPlayerWindow02") +  oppItem.guildName;
		isFriend ();
	}

	private void updateView_ladders ()
	{
		buttonInfo.label_ladders_playerName.text = Language ("pvpPlayerWindow01") + oppItem.name;

		if (oppItem.sdkInfo != null)
			setSexLadders (oppItem.sdkInfo.sex);


		buttonInfo.label_ladders_rank.text = Language ("laddersTip_13", mCurrentLaddersPlayer.rank.ToString ());
		//buttonInfo.label_ladders_playerScore.text=Language("laddersTip_13",mCurrentLaddersPlayer.rank.ToString());
		ladders_applyHelp.textLabel.text = Language ("laddersName_03")+ "(" +(LaddersManagement.Instance.TotalApplyFriendHelpTimes - LaddersManagement.Instance.currentFriendHelpTimes) + "/" + LaddersManagement.Instance.TotalApplyFriendHelpTimes + ")";
        if (!LaddersManagement.Instance.FriendCanHelp)
        {
			//防止购买成功后没有进行战斗的情况（最大免费 + 已经购买 - 已经战斗的次数>0）
			if(LaddersManagement.Instance.TotalApplyFriendHelpTimes+LaddersManagement.Instance.mBuyFriendFightCount-LaddersManagement.Instance.currentFriendHelpTimes > 0){
				GameObject rmbObj = ladders_applyHelp.transform.FindChild("rmb").gameObject;
				rmbObj.SetActive(false);
				ladders_applyHelp.textLabel.text = Language ("laddersName_03")+ "(" +(LaddersManagement.Instance.TotalApplyFriendHelpTimes+LaddersManagement.Instance.mBuyFriendFightCount-LaddersManagement.Instance.currentFriendHelpTimes) + "/" + LaddersManagement.Instance.TotalApplyFriendHelpTimes + ")";
			}else{
				GameObject rmbObj = ladders_applyHelp.transform.FindChild("rmb").gameObject;
				rmbObj.SetActive(true);
				int costTimes = LaddersManagement.Instance.currentFriendHelpTimes - LaddersManagement.Instance.TotalApplyFriendHelpTimes + 1;
				int cost = CommonConfigSampleManager.Instance.getSampleBySid<LadderFriendsHelpCostSample>(CommonConfigSampleManager.LadderFriendsHelp_SID).getCostByCount(costTimes-1);
				ladders_applyHelp.textLabel.text = LanguageConfigManager.Instance.getLanguage("laddersName_04", cost.ToString());
			}  
        }
	}

	private void updateView_guildFight(){
		buttonInfo.playerName.text = oppItem.name;
		buttonInfo.guildName.text = Language ("pvpPlayerWindow02") +  oppItem.guildName;
		if (oppItem.sdkInfo != null)
			setSex (oppItem.sdkInfo.sex);
	}

	private void udpateView_ladders_friend ()
	{
		buttonInfo.label_ladders_playerName.text = oppItem.name;
		if (oppItem.sdkInfo != null)
			setSex (oppItem.sdkInfo.sex);

	}

	public void initInfo (PvpOppInfo opp)
	{
		this.oppItem = opp;
	}

	public void initInfo (PvpOppInfo opp, CallBack callback)
	{
		this.oppItem = opp;
		this.callback = callback;
	}


	public void initInfo (PvpOppInfo opp, CallBack<bool> reLoadFriendWin)
	{
		this.oppItem = opp;
		this.reLoadFriendWin = reLoadFriendWin;
	}

	public void initCallBack (CallBack _callback)
	{
		this.callback = _callback;
	}


	
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		string btnName = gameObj.name;
		if (btnName == "applyFriendButton") {
			//申请好友
			applyFriend ();
		} else if (btnName == "deleteFriendButton") {
			//删除好友
			UiManager.Instance.createMessageWindowByTwoButton (LanguageConfigManager.Instance.getLanguage ("s0379"), delFriend);
		} else if (btnName == "btn_challenge") {
			//挑战
			challengeLaddersPlayer ();
		} else if (btnName == "btn_seekFriendHelp") {
			//请求好友帮助
			FriendInfo[] friends = FriendsManagerment.Instance.getFriendList ();
			if (friends == null || friends.Length == 0) {
				MessageWindow.ShowAlert (Language ("laddersTip_16"));
			} else {
				if (LaddersManagement.Instance.FriendCanHelp) {
					UiManager.Instance.openWindow<LaddersFriendsWindow> ();
				} else {
                    Vip vip = VipManagerment.Instance.getVipbyLevel(UserManager.Instance.self.getVipLevel());
                    if (vip != null)
                    {
                        if (vip.privilege.ladderHelpTimes + LaddersManagement.Instance.TotalApplyFriendHelpTimes > LaddersManagement.Instance.currentFriendHelpTimes)
                        {
                            UiManager.Instance.openWindow<LaddersFriendsWindow>((win) =>
                            {
                                int costTimes = LaddersManagement.Instance.currentFriendHelpTimes - LaddersManagement.Instance.TotalApplyFriendHelpTimes + 1;
                                int cost = CommonConfigSampleManager.Instance.getSampleBySid<LadderFriendsHelpCostSample>(CommonConfigSampleManager.LadderFriendsHelp_SID).getCostByCount(costTimes-1);
								//防止购买成功后没有进行战斗的情况（最大免费 + 已经购买 - 已经战斗的次数>0）
								if(LaddersManagement.Instance.TotalApplyFriendHelpTimes+LaddersManagement.Instance.mBuyFriendFightCount-LaddersManagement.Instance.currentFriendHelpTimes > 0)cost=0;
								win.setCostInfo(cost, vip.privilege.ladderHelpTimes-(costTimes - 1), vip.privilege.ladderHelpTimes);
                            });
                        }else{
                            UiManager.Instance.openDialogWindow<MessageWindow>((win) =>
                            {
                                win.initWindow(2, LanguageConfigManager.Instance.getLanguage("recharge01"), LanguageConfigManager.Instance.getLanguage("s0093"),
                                           LanguageConfigManager.Instance.getLanguage("s015313"), (msgHandle) =>
                                           {
                                               if (msgHandle.buttonID == MessageHandle.BUTTON_LEFT)
                                               {
                                                   UiManager.Instance.openWindow<VipWindow>();
                                               }
                                           });
                            });
                        }
                    }
                    else{
                        UiManager.Instance.openDialogWindow<MessageWindow>((win) =>
                        {
                            win.initWindow(2, LanguageConfigManager.Instance.getLanguage("recharge01"), LanguageConfigManager.Instance.getLanguage("s0093"),
                                       LanguageConfigManager.Instance.getLanguage("s015314"), (msgHandle) =>
                                       {
                                           if (msgHandle.buttonID == MessageHandle.BUTTON_LEFT)
                                           {
                                               UiManager.Instance.openWindow<VipWindow>();
                                           }
                                       });
                        });
                    }
				}
			}
		} else if (btnName == "GuildAreaChallenge") {
			if(fatherWindow is GuildAreaWindow){
				GuildAreaWindow win = fatherWindow as GuildAreaWindow;
				win.challenge();				
			}
		}
		else if (btnName == "close" || btnName == "btn_close") {
			//清理查看数据
            if (MissionManager.instance != null) {
                MissionManager.instance.showAll();
                MissionManager.instance.setBackGround();
            }
			ChatManagerment.Instance.chatCard = null;
			if (callback != null) {
				callback ();
				callback = null;
			}
			if (fatherWindow is FriendsWindow) {
				finishWindow ();
				if (FriendFindRecommendWindow.CACHE_LIST != null)
					UiManager.Instance.openDialogWindow<FriendFindRecommendWindow> ();
//				if (isOperateFriend) {
//					EventDelegate.Add (OnHide, () => {
//						if (reLoadFriendWin != null) {
//							reLoadFriendWin (isOperateFriend);
//							reLoadFriendWin = null;
//						}
//					});
//				}
			} else if (fatherWindow is GuildMemberWindow) {
				finishWindow ();
				GuildMemberWindow win = fatherWindow as GuildMemberWindow;
				win.updateMemberInfo ();
			} else if (comeFrom == PvpPlayerWindow.FROM_CHAT) {
				finishWindow ();
				/*这里开始是可滑动聊天窗口展示的关闭后处理，暂时不删
				UiManager.Instance.openDialogWindow<NewChatWindow> ((win) => {
					win.initChatWindow (ChatManagerment.Instance.sendType - 1);
				});
				*/
			} else {
				finishWindow ();
			}
			comeFrom = FROM_OTHER;
		}
	}
	/// <summary>
	/// 天梯挑战玩家
	/// </summary>
	private void challengeLaddersPlayer ()
	{
		LaddersManagement.Instance.currentBattleIsFriendHelp = false;
		string uid = UserManager.Instance.self.uid;
		MaskWindow.instance.setServerReportWait(true);
		GameManager.Instance.battleReportCallback=GameManager.Instance.intoBattle;
		PvpInfoManagerment.Instance.sendLaddersFight (uid);
	}

	//申请好友
	public void applyFriend ()
	{
		if (FriendsManagerment.Instance.isFull ())
			return;
		FriendsFPort fport = FPortManager.Instance.getFPort ("FriendsFPort") as FriendsFPort;
		fport.applyFriend (uid, applyOk);
	}

	//申请后回调
	public void applyOk ()
	{
		applyButton.textLabel.text = LanguageConfigManager.Instance.getLanguage ("FriendAPPLY_already_apply");
		applyButton.disableButton (true);
		isOperateFriend = true;
	}

	//确定后发送删除好友请求
	public void delFriend (MessageHandle msg)
	{
		if (msg.buttonID == MessageHandle.BUTTON_LEFT)
			return;
		FriendsFPort fport = FPortManager.Instance.getFPort ("FriendsFPort") as FriendsFPort;
		fport.deleteFriend (uid, delGoback);
	}

	//判断是不是好友时要做的
	public void isFriend ()
	{
		if (oppItem != null) {
			if (UserManager.Instance.self.uid == oppItem.uid) {
				applyButton.gameObject.SetActive (false);
				deleteButton.gameObject.SetActive (false);
				fcbutton.gameObject.SetActive(false);
				return;
			}
			if (FriendsManagerment.Instance.isFriend (oppItem.uid)) {
				applyButton.gameObject.SetActive (false);
				deleteButton.gameObject.SetActive (true);
				fcbutton.gameObject.SetActive(true);
				FriendInfo fi = FriendsManagerment.Instance.getFriendByUid(oppItem.uid);
				fcbutton.info = fi;
			} else {
				applyButton.gameObject.SetActive (true);
				deleteButton.gameObject.SetActive (false);
				fcbutton.gameObject.SetActive(false);
			}
		}
	}

	//删除成功后回调到好友列表
	public void delGoback ()
	{
		FriendsManagerment.Instance.deleteFriend (uid);
		applyButton.gameObject.SetActive (true);
		deleteButton.gameObject.SetActive (false);
		fcbutton.gameObject.SetActive(false);
		isOperateFriend = true;
	}
	
	public override void DoDisable ()
	{ 
		base.DoDisable ();

	}
	
	//加载阵型对象
	private void loadFormationGB (ButtonPvpInfo button, int formationLength, GameObject root)
	{
		//如果是10人的队伍 则不需要加载阵型对象 10人的阵型单独有root
		if(teamType==10)
		{
			return;
		}
		passObj go = FormationManagerment.Instance.getPlayerInfoFormationObj (formationLength);
		go.obj.transform.parent = root.transform;
		go.obj .transform.localPosition = Vector3.zero;
		go.obj .transform.localScale = Vector3.one;
		
		if (go.obj != null) {
			button.formationRoot = go.obj;
			go.obj.transform.localPosition = new Vector3 (0, 230, 0);
		}



		
	}


	void CreateFormation (ButtonPvpInfo button, PvpOppInfo info)
	{ 
		button.tenFormationRoot.SetActive(teamType==10);		
		for (int i = 0; i < info.opps.Length; i++) {
			RoleView cardView = NGUITools.AddChild (teamType==10?button.tenFormationRoot:button.formationRoot, cardViewPrefab).GetComponent<RoleView> ();
			cardView.transform.localScale = new Vector3 (0.65f, 0.65f, 0.65f);
			CardSample cs = CardSampleManager.Instance.getRoleSampleBySid (info.opps [i].sid);
			Card newcard = CardManagerment.Instance.createCard (cs.sid, info.opps [i].evoLevel, info.opps [i].surLevel);
			newcard.uid = info.opps [i].uid;
			newcard.setLevel (EXPSampleManager.Instance.getLevel (cs.levelId, info.opps [i].exp, 0));
			cardView.init (newcard, this, cardClickEvent);

			//找到对应的阵形点位
			Transform formationPoint = null;
			if(teamType==10)
			{
				formationPoint = button.tenFormationRoot.transform.FindChild((info.opps[i].index+1).ToString());
			}else
			{
				formationPoint = button.formationRoot.transform .FindChild (FormationManagerment.Instance.getLoctionByIndex (info.formation, info.opps [i].index).ToString ());
			}

			cardView.transform.position = formationPoint.position;
			/** 从公会战来,显示血条 */
			if(comeFrom == FROM_GUILD_AREA || comeFrom == FROM_GUILD_AREA_CHALLENGE){
				int index = info.opps[i].index;
				hpViews[index].gameObject.SetActive(true);
				hpViews[index].updateValue(info.opps[i].hpNow,info.opps[i].hpMax );
				cardView.GetComponent<BoxCollider>().enabled = false;
			}
		}
	}

	public  void cardClickEvent (RoleView view)
	{
		//新手引导不能点
		if (GuideManager.Instance.isLessThanStep (GuideGlobal.NEWOVERSID)) {
			MaskWindow.UnlockUI ();
			return;
		}
		GetPlayerCardInfoFPort fport = FPortManager.Instance.getFPort ("GetPlayerCardInfoFPort") as GetPlayerCardInfoFPort;
		fport.getCard (uid, view.card.uid, null);
	}
 

}
