using UnityEngine;
using System.Collections;

public class SdkFriendsItem : MonoBehaviour {
	
	public UITexture iconHead;//头像
	public UILabel labelName;//名称
	public UILabel sdkLevel;//等级
	public UISprite spriteVip;//vip等级
	public UILabel changeNum;//玩家游戏等级
	public UILabel serverLevel;//服务器显示
	public UISprite spriteSex;//显示性别
	public SdkFriendButton giftbutton;
	public SdkFriendButton getButton;
	public SdkFriendButton inviteButton;

	public UILabel sendLabel;
	public UILabel getLabel;
	public UILabel inviteLabel;
	
	public WindowBase fatherWindow;
	public SdkFriendsInfo sdkfriendsInfo;
	
	public void initItem(SdkFriendsInfo info,WindowBase fatherwindow)
	{
		this.sdkfriendsInfo = info;
		this.fatherWindow = fatherwindow;
		string nameinfo = info.Name;
		if (sdkfriendsInfo.ServerFlag == 1 || sdkfriendsInfo.ServerFlag == 0)
		{
			if(info.HeadIcon!=null && info.HeadIcon.Length > 7)
				ResourcesManager.Instance.LoadAssetBundleTexture(info.HeadIcon,iconHead);
			spriteSex.gameObject.SetActive(true);
			spriteSex.spriteName= info.Sex;
			labelName.text = nameinfo;
			if (info.MomoLevel > 0)
			{
				sdkLevel.text = "VIP";
			} else {
				sdkLevel.text = LanguageConfigManager.Instance.getLanguage("s0073");
			}
			serverLevel.gameObject.SetActive(false);
			setButtonShow(sdkfriendsInfo.ServerFlag,info,fatherWindow);
			
		} else if (sdkfriendsInfo.ServerFlag == 2) {
			serverLevel.gameObject.SetActive(true);
			serverLevel.depth = 100;
			if(info.HeadIcon!=null && info.HeadIcon.Length > 7)
				ResourcesManager.Instance.LoadAssetBundleTexture(info.HeadIcon,iconHead);
			serverLevel.text = LanguageConfigManager.Instance.getLanguage("sdk_friend_difs");
			labelName.text = nameinfo;
			spriteSex.spriteName = info.Sex.ToString();
			
			if(info.VipLevel >0) {
				spriteVip.gameObject.SetActive (true);
				spriteVip.spriteName = "vip" + info.VipLevel;
			} else {
				spriteVip.gameObject.SetActive (false);
			}
			setButtonShow(2,info,fatherWindow);
		} else if (sdkfriendsInfo.ServerFlag == 3) {
			serverLevel.gameObject.SetActive(true);
			serverLevel.depth = 100;
			if(info.HeadIcon!=null && info.HeadIcon.Length > 7)
				ResourcesManager.Instance.LoadAssetBundleTexture(info.HeadIcon,iconHead);
			serverLevel.text = LanguageConfigManager.Instance.getLanguage("sdk_friend_sames");
			labelName.text = nameinfo;
			spriteSex.spriteName = info.Sex.ToString();
			
			if(info.VipLevel >0) {
				spriteVip.gameObject.SetActive (true);
				spriteVip.spriteName = "vip" + info.VipLevel;
			} else {
				spriteVip.gameObject.SetActive (false);
			}
			changeNum.gameObject.SetActive(true);
			changeNum.text = LanguageConfigManager.Instance.getLanguage("sdk_friend_changenum",info.ChangeNum.ToString());
			setButtonShow(3,info,fatherWindow);
			
		}
	}
	
	
	void setButtonShow(int flags,SdkFriendsInfo info,WindowBase fatherwindow)
	{
		switch(flags)
		{
		case 0:
			giftbutton.gameObject.SetActive(false);
			getButton.gameObject.SetActive(false);
			inviteButton.gameObject.SetActive(false);
			break;
		case 1:
			giftbutton.gameObject.SetActive(false);
			getButton.gameObject.SetActive(false);
			inviteButton.gameObject.SetActive(true);
			if (info.IsInviate)
			{
				inviteButton.disableButton(false);
				inviteLabel.text = LanguageConfigManager.Instance.getLanguage("sdk_friend_invite");
			} else {
				inviteButton.disableButton(true);
				inviteLabel.text = LanguageConfigManager.Instance.getLanguage("sdk_friended_invite");
			}
			inviteButton.initButton(this,1,fatherwindow); 
			inviteButton.fatherWindow = fatherwindow;
			break;
		case 2:
			giftbutton.gameObject.SetActive(false);
			getButton.gameObject.SetActive(false);
			inviteButton.gameObject.SetActive(false);
			break;
		case 3:
			giftbutton.gameObject.SetActive(true);
			getButton.gameObject.SetActive(true);
			if (info.SendFlags == 1)
			{
				giftbutton.disableButton(true);
				sendLabel.text = LanguageConfigManager.Instance.getLanguage("sdk_friended_gift");
			} else if (info.SendFlags == 2) {
				giftbutton.disableButton(false);
				sendLabel.text = LanguageConfigManager.Instance.getLanguage("sdk_friend_gift");
			}
			if (info.GetInfo == 3)
			{
				getButton.disableButton(true);
				getLabel.text = LanguageConfigManager.Instance.getLanguage("sdk_friended_get");
			} else if (info.GetInfo == 1) {
				getButton.disableButton(false);
				getLabel.text = LanguageConfigManager.Instance.getLanguage("sdk_friend_get");
			} else {
				getButton.gameObject.SetActive(false);
			}
			giftbutton.initButton(this,2,fatherwindow); 
			getButton.initButton(this,3,fatherwindow); 
			inviteButton.gameObject.SetActive(false);
			break;
		}
	}
	
	
	
}
