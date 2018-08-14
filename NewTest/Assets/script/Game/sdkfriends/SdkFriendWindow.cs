using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * momo好友邀请奖励主窗口
 * @authro 刘正平
 * */
public class SdkFriendWindow : WindowBase {
	public UILabel labelGetCount;//领取次数
	public UILabel labelInviteCount;//邀请次数
	
	public UILabel labelFriendsNumber;//朋友数量
	public UILabel labelAwardsNumber;//奖励数量
	
	public UISprite spriteFriendsNumber;//朋友数量
	public UISprite spriteAwardsNumber;//奖励数量
	
	public TapContentBase tapBase;//分页按钮
	public SdkFriendContent content1;//好友容器
	public SdkPrizeContent content2;//奖励容器
	
	public GameObject friendsListView;
	public GameObject awardsListView;
	
	public GameObject friendsBarPrefab;//好友预制体
	public GameObject awardsBarPrefab;//奖励预制体
	
	private Friends friendsInfo;//好友信息
	//需要自己定义Awards;
	private Friends awardsInfo;//好友信息
	private int tapType = 0;//属于哪个标签。0好友信息，1奖励信息
	public UIToggle toggle;
	
	
	protected override void begin()
	{
		base.begin();
		
		updateInfo ();
		MaskWindow.UnlockUI ();

	}


	public void updateInfo()
	{
		tapBase.changeTapPage(tapBase.tapButtonList[0]);
		showInfo();
		showNumber();
	}
	
	
	public void updateShowInfo()
	{
		int getCount = SdkFriendManager.Instance.GetNum;
		int maxGetCount = SdkFriendManager.Instance.MAXGETNUM;
		labelGetCount.text =(maxGetCount - getCount) + "/" + maxGetCount;
		
		int inviteCount = SdkFriendManager.Instance.InviteNum;
		int maxInviteCount = SdkFriendManager.Instance.MAXINVITENUM;
		labelInviteCount.text =(maxInviteCount - inviteCount) + "/" + maxInviteCount;
	}
	
	private void showInfo()
	{   
		updateShowInfo ();
		
		friendsListView.SetActive(true);
		awardsListView.SetActive(false);
		content1.fatherWindow = this;
		content1.reLoad(1);  
	}
	
	//断线重连
	public override void OnNetResume()
	{
		base.OnNetResume();
		SdkFriendManager.Instance.getsdkFirendsInfos (()=>{
			updateInfo();
		});
		tapBase.resetTap();       
	}
	
	//显示朋友数量，奖励数量小标签
	public void showNumber()
	{
		
		
		if (SdkFriendManager.Instance.showGetNum == 0)
		{
			labelFriendsNumber.gameObject.SetActive(false);
			spriteFriendsNumber.gameObject.SetActive(false);
		} else {
			labelFriendsNumber.gameObject.SetActive(true);
			spriteFriendsNumber.gameObject.SetActive(true);
			labelFriendsNumber.text = SdkFriendManager.Instance.showGetNum.ToString();
		}
		
		if (InvitePrizeManager.Instance.showInviteNum == 0)
		{
			labelAwardsNumber .gameObject.SetActive(false);
			spriteAwardsNumber.gameObject.SetActive(false);
		} else {
			labelAwardsNumber.gameObject.SetActive(true);
			spriteAwardsNumber.gameObject.SetActive(true);
			labelAwardsNumber.text = InvitePrizeManager.Instance.showInviteNum.ToString();
		}
		
	}
	
	
	public override void buttonEventBase (GameObject gameObj)
	{
		if (gameObj.name == "close" ) {
			finishWindow ();
		}
		else if (gameObj.name == "Toggle") {
			
			if (gameObj.GetComponent<UIToggle>().value)
			{
				content1.reLoad(2);
			} else {
				content1.reLoad(1);
			}
		}
	}
	
	//切换页面
	public override void tapButtonEventBase(GameObject gameObj, bool enable)
	{
		base.tapButtonEventBase(gameObj, enable);
		return;
		if (gameObj.name == "tapFriends" && enable == true)
		{
			tapType = 0;
			showInfo();
			if (toggle.value)
			{
				content1.reLoad(2);
			} else {
				content1.reLoad(1);
			}
			awardsListView.SetActive(false);
			friendsListView.SetActive(true);
			
		}
		else if (gameObj.name == "tapAwards" && enable == true)
		{
			friendsListView.SetActive(false);
			awardsListView.SetActive(true);
			tapType = 1;
			InvitePrizeManager.Instance.getserverInvitePrizes(() =>
			                                                  {
				content2.reLoad(tapType);
			});

		}    
		
	}
	protected override void DoEnable()
	{
		UiManager.Instance.backGround.switchToDark();
	}
	
	
	
}
