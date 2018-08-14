using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 天梯中邀请好友助战时 好友选择窗口 模仿好友列表
/// </summary>
public class LaddersFriendsWindow : WindowBase
{
	public UILabel labelFriendsAmount;//好友数量
	public UILabel labelMyUid;//我的UID
	public ContentLaddersFriends content;//好友容器	
	public GameObject friendsBarPrefab;
    public int cost = 0;//好友助战消耗钻石
    public int currentBuyTimes = 0;//当前购买的好友助战次数
    public int totalBuyTimes = 0;//总的可以购买的次数
	//断线重连用，是否是购买后断线
	private bool isOnet=false;
	GameObject gameObj;


	List<FriendInviteInfo> invitefriend=new List<FriendInviteInfo>();
	private Friends friendsInfo;//好友信息


	public override void OnStart ()
	{
		friendsBarPrefab.SetActive(false);
	}

	protected override void begin ()
	{
		base.begin ();
		if (friendsInfo == null)
			getFriendsInfo ();
		MaskWindow.UnlockUI ();
	}
	
	//断线重连
	public override void OnNetResume ()
	{
		base.OnNetResume ();
		if (isOnet && gameObj.name =="btn_battle") {
			ladderFriendHelp(gameObj);
		}
	}
    public void setCostInfo(int costRMB ,int current,int total)
    {
        this.cost = costRMB;
        this.currentBuyTimes=current;
        this.totalBuyTimes = total;
    }
	public void initWin ()
	{
		friendsInfo = FriendsManagerment.Instance.getFriends ();
		invitefriend.Clear();
		getFriendHelpInfo();
//		if (invitefriend == null)
//			return;
//		content.reLoad(invitefriend);
//		showUI ();
//		GuideManager.Instance.guideEvent();
	}
	public void getFriendHelpInfo()
	{
		LaddersFriendHelpFPort fport = FPortManager.Instance.getFPort("LaddersFriendHelpFPort") as LaddersFriendHelpFPort;
		fport.access((inviteFriend)=>{
			FriendInfo fi;
			for(int i=0;i<friendsInfo.friends.Length;i++)
			{
				fi=friendsInfo.friends[i];
				FriendInviteInfo inf = new FriendInviteInfo(fi.getUid(),fi.getHeadIco(),fi.getName(),fi.getExp(),fi.getVipExp(),fi.getGuild(),fi.getCombatPower(),fi.getStar(),fi.getIsOnline(),fi.getGiftReceiveStatus(),fi.getGiftSendStatus(),LaddersManagement.Instance.BeInviteMaxNum);
				for(int j=0;j<inviteFriend.Count;j++)
				{
					if(inf.getUid ()== inviteFriend[j].uid){
						if(inviteFriend[j].inviteNum <= LaddersManagement.Instance.BeInviteMaxNum)
							inf.inviteNum -= inviteFriend[j].inviteNum;
					}
				}
				invitefriend.Add(inf);
			}
			if (invitefriend == null)
				return;
			content.reLoad(invitefriend);
			showUI ();
			GuideManager.Instance.guideEvent();
		});
	}
	public void initFriendsInfo ()
	{
		initWin();
	}
	
	public void showUI ()
	{		
		labelFriendsAmount.text = friendsInfo.getAmount () + " / " + friendsInfo.getMaxSize ();
		labelMyUid.text = StringKit.serverIdToFrontId(UserManager.Instance.self.uid);
	}
	
	public void getFriendsInfo ()
	{
		if(FriendsManagerment.Instance.getFriends ()==null)
		{
			FriendsManagerment.Instance.getFriendsInfo (initFriendsInfo);
		}else
		{
			initFriendsInfo();
		}
	}
    public void ladderFriendHelp(GameObject gameObj)
    {
        MaskWindow.LockUI();
        //好友出战
        Ladders_FriendItem tempItem = gameObj.transform.parent.GetComponent<Ladders_FriendItem>();
        if (tempItem == null)
        {
            MaskWindow.UnlockUI();
            return;
        }
        string uid = tempItem.data.getUid();

        PvpInfoManagerment.Instance.sendLaddersFight(uid, (msg) =>
        {
            LaddersManagement.Instance.currentBattleIsFriendHelp = true;
            LaddersManagement.Instance.currentFriendHelpTimes++;
            finishWindow();
            MaskWindow.instance.setServerReportWait(true);
            GameManager.Instance.battleReportCallback = GameManager.Instance.intoBattle;

        });
    }

	/// <summary>
	/// 该面板中的点击事件
	/// </summary>
	/// <param name="gameObj">Game object.</param>
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		
		if (gameObj.name == "close") {
			PvpPlayerWindow.comeFrom=PvpPlayerWindow.FROM_LADDERS;
			finishWindow();
		}else if(gameObj.name == "btn_battle")
		{
            if (cost > 0)
            {
                if (UserManager.Instance.self.getRMB() >= cost)
                {
                    UiManager.Instance.openDialogWindow<MessageWindow>((win) =>
                    {
                        win.dialogCloseUnlockUI = false;
                        win.initWindow(2, Language("s0094"), Language("s0093"), LanguageConfigManager.Instance.getLanguage("s0595", cost.ToString(), currentBuyTimes.ToString(), totalBuyTimes.ToString()), (msg) =>
                        {
                            if (msg.msgEvent == msg_event.dialogOK)
                            {
								isOnet = true;
								this.gameObj = gameObj;
								LaddersFriendHelpBuyFPort fport = FPortManager.Instance.getFPort("LaddersFriendHelpBuyFPort") as LaddersFriendHelpBuyFPort;
                                fport.access(() => {
                                    ladderFriendHelp(gameObj);
                                });
                            }
                            else
                            {
                                MaskWindow.UnlockUI();
                            }
                        });
                    });
                }
                else
                {
                    UiManager.Instance.openDialogWindow<MessageWindow>((win) =>
                    {
                        win.dialogCloseUnlockUI = false;
                        win.initWindow(2, Language("s0324"), Language("s0093"), LanguageConfigManager.Instance.getLanguage("s0158"), (msg) =>
                        {
                            if (msg.msgEvent == msg_event.dialogOK)
                            {
                                UiManager.Instance.openWindow<VipWindow>();
                            }
                            else
                            {
                                MaskWindow.UnlockUI();
                            }
                        });
                    });
                }
            }
            else ladderFriendHelp(gameObj);
			
		}else if(gameObj.name == "icon_player")
		{
			//查看好友信息
			Ladders_FriendItem tempItem=gameObj.transform.parent.GetComponent<Ladders_FriendItem>();
			if(tempItem == null)
			{
				return;
			}
			string uid=tempItem.data.getUid();
			LaddersGetPlayerInfoFPort fport=FPortManager.Instance.getFPort ("LaddersGetPlayerInfoFPort") as LaddersGetPlayerInfoFPort;
			fport.access(uid, 10,M_onGetPlayerInfoCmp);
		}
	}	
	/// <summary>
	/// 当请求玩家信息返回后 打开玩家信息面板
	/// </summary>
	/// <param name="_playerInfo">_player info.</param>
	private void M_onGetPlayerInfoCmp(PvpOppInfo _playerInfo)
	{
		PvpPlayerWindow.comeFrom=PvpPlayerWindow.FROM_LADDERS_FRIEND;
		UiManager.Instance.openWindow<PvpPlayerWindow> (
			(win) => {
			win.teamType = 10;
			win.initInfo (_playerInfo);
		});
	}
}

