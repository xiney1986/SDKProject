using UnityEngine;
using System.Collections;

public class SdkFriendButton : ButtonBase
{
	
	public  SdkFriendsItem sdkfriendInfo;
	public  InvitePrizeItem prizeInfo;
	
	// setType == 1 skdfriendButton,2 backView,3 backprizeget,4,inviteprizeget
	public int setType = 0;
	// friendtype = 1;invitbutton,2,givebutton,3,getbutton
	public int friendType = 0;
	
	public void initButton (InvitePrizeItem prizeItem,int type,WindowBase father)
	{
		prizeInfo = prizeItem;
		setType = type;
		fatherWindow = father;
	}
	
	public void initButton(SdkFriendsItem friendInfo,int type,WindowBase father)
	{
		sdkfriendInfo = friendInfo;
		friendType = type;
		fatherWindow = father;
	}
	
	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
		if (sdkfriendInfo != null)
		{
			if (friendType == 1)
			{
                SdkFriendManager.Instance.sendInviteJoin(sdkfriendInfo.sdkfriendsInfo, () =>
                    {
                        sdkfriendInfo.inviteButton.disableButton(true);
					    sdkfriendInfo.inviteLabel.text = LanguageConfigManager.Instance.getLanguage("sdk_friended_invite");
					    var win = fatherWindow as SdkFriendWindow;
					    if (win != null)
					        win.updateShowInfo();
                    });	
			} else if (friendType == 2) {
                SdkFriendManager.Instance.sendPower(sdkfriendInfo.sdkfriendsInfo, () =>
                {
                    sdkfriendInfo.giftbutton.disableButton(true);
					sdkfriendInfo.sendLabel.text = LanguageConfigManager.Instance.getLanguage("sdk_friended_gift");
                });
				
			} else if (friendType == 3) {
                SdkFriendManager.Instance.sendgetPower(sdkfriendInfo.sdkfriendsInfo, () =>
                {
					SdkFriendManager.Instance.skdbackFriendsInfos(()=>{
						sdkfriendInfo.sdkfriendsInfo.GetInfo=3;
						sdkfriendInfo.getButton.disableButton(true);
						sdkfriendInfo.getLabel.text = LanguageConfigManager.Instance.getLanguage("sdk_friended_get");
						if (fatherWindow != null)
						{
							(fatherWindow as SdkFriendWindow).showNumber();
							(fatherWindow as SdkFriendWindow).updateShowInfo();
						}
					});
					//此代码屏蔽，是因为调了初始化接口但只传了一个uid过去，而不是全部的uid，导致初始化不正确
//					SdkFriendFPort ffport = FPortManager.Instance.getFPort ("SdkFriendFPort") as SdkFriendFPort;
//					ffport.getsdkFriendsInfoMsg(sdkfriendInfo.sdkfriendsInfo.Uid,()=>{
//						sdkfriendInfo.sdkfriendsInfo.GetInfo=3;
//						sdkfriendInfo.getButton.disableButton(true);
//						sdkfriendInfo.getLabel.text = LanguageConfigManager.Instance.getLanguage("sdk_friended_get");
//						if (fatherWindow != null)
//						{
//							(fatherWindow as SdkFriendWindow).showNumber();
//							(fatherWindow as SdkFriendWindow).updateShowInfo();
//						}
//					});

                });
			}
		} 
		if (prizeInfo != null)
		{
			if (setType == 2)
			{
                InvitePrizeManager.Instance.clickViewButton();
				
			} else if (setType == 3) {
                InvitePrizeManager.Instance.sendbackGetinvitePrize(prizeInfo.invitePrize, () =>
                {
					prizeInfo.prizeButton[0].setTextNum(prizeInfo.invitePrize.backMoney);
                    prizeInfo.backGetButton.disableButton(true);
					if (fatherWindow != null)
						(fatherWindow as SdkFriendWindow).showNumber();
                });
			} else if (setType == 4) {
                InvitePrizeManager.Instance.sendlevelGetinvitePrize(prizeInfo.invitePrize, () =>
                {
					prizeInfo.initItem(prizeInfo.invitePrize,prizeInfo.thisFather);
					if (fatherWindow != null)
						(fatherWindow as SdkFriendWindow).showNumber();
                });
				
			}

		}
		
	}
}
