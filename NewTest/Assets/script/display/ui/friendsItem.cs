using UnityEngine;
using System.Collections;

public class friendsItem : ButtonBase {

    public UISprite UI_StarIcon;
    public UILabel UI_StarLabel;
    public FriendsChatButton UI_ChatBtn; //好友聊天按钮

	public UITexture iconHead;//头像
	public UILabel labelName;//名称
	public UILabel labelLevel;//等级
	public UISprite spriteVip;//VIP背景
	public UISprite spriteSex;//性别
    public FriendsGiftButton buttonGiftReceive; //接收好友禮物
    public FriendsGiftButton buttonGiftSend; //發送給好友禮物
	public FriendsYesButton buttonYes;//同意好友申请
	public FriendsNoButton buttonNo;//拒绝好友申请
	public FriendsApplyButton buttonApply;//高富帅我们成为好朋友吧
	public FriendsLookButton HeadLook;//头像查看
	public FriendsLookButton buttonLook;//+陌陌好友
	public UISprite onlineIcon;
	public UILabel onlineLabel;

	public FriendInfo info;
	private int type;//0好友信息，1待批准，2好友申请

	public void initInfo(int _type,FriendInfo _info, bool showOnlineMark)
	{
		type = _type;
		info = _info;
		HeadLook.type = type;

		if(info == null)
			return;
		changeButtons();
		showUI();
		if(onlineIcon!=null)
			onlineIcon.spriteName = "";
		if (showOnlineMark) {
			if(onlineLabel!=null)	
				onlineLabel .text =  !info.getIsOnline ()? Colors.CHAT_GRAY + Language ("Friends31") + "[-]" : Colors.GREEN + Language ("Guild_20") + "[-]";
			if(onlineIcon!=null)	
				onlineIcon.spriteName = !info.getIsOnline () ? "point_outline":"point_online";
		}
	}

	private void showUI()
	{
		HeadLook.fatherWindow = this.fatherWindow;
		HeadLook.info = info;
		labelName.text = info.getName();
		labelLevel.text = "LV." + info.getLevel();

		//设置头像
		if(info.getSdkInfo()!=null && info.getSdkInfo().face.Length>7)
			ResourcesManager.Instance.LoadAssetBundleTexture(info.getSdkInfo().face,iconHead);
		else
		ResourcesManager.Instance.LoadAssetBundleTexture(UserManager.Instance.getIconPath(StringKit.toInt( info.getHeadIco())),iconHead);

		//设置性别
		if(info.getSdkInfo()!=null){
			spriteSex.gameObject.SetActive(true);
			spriteSex.spriteName=info.getSdkInfo().sex;
		}

        UI_StarLabel.text = HoroscopesManager.Instance.getStarByType(info.getStar()).getName();
        UI_StarIcon.spriteName = HoroscopesManager.Instance.getStarByType(info.getStar()).getSpriteName();


		if(info.getVipLevel() >0) {
			spriteVip.gameObject.SetActive (true);
			spriteVip.spriteName = "vip" + info.getVipLevel();
		}
		else {
			spriteVip.gameObject.SetActive (false);
		}
	}

	private void changeButtons()
	{
		switch(type)
		{
		case 0:
          
			break;
		case 1:
			buttonYes.fatherWindow = this.fatherWindow;
			buttonNo.fatherWindow = this.fatherWindow;
			buttonYes.info = info;
			buttonNo.info = info;
            buttonGiftReceive.gameObject.SetActive(false);
            buttonGiftSend.gameObject.SetActive(false);
			buttonLook.gameObject.SetActive(false);
			buttonYes.gameObject.SetActive (true);
			buttonNo.gameObject.SetActive (true);
			buttonApply.gameObject.SetActive (false);
			break;
		case 2:
			buttonApply.fatherWindow = this.fatherWindow;
			buttonApply.info = info;
			if(buttonApply.info.isApply())
			{
				buttonApply.textLabel.text = LanguageConfigManager.Instance.getLanguage("FriendAPPLY_already_apply");
				buttonApply.disableButton(true);
			}
			else
			{
				buttonApply.textLabel.text = LanguageConfigManager.Instance.getLanguage("FriendAPPLY");
				buttonApply.disableButton(false);
			}

            buttonGiftReceive.gameObject.SetActive(false);
			buttonLook.gameObject.SetActive(false);
            buttonGiftSend.gameObject.SetActive(false);
			buttonYes.gameObject.SetActive (false);
			buttonNo.gameObject.SetActive (false);
			if(FriendsManagerment.Instance.isFriend(info.getUid()) || UserManager.Instance.self.uid == info.getUid())
				buttonApply.gameObject.SetActive (false);
			else
				buttonApply.gameObject.SetActive (true);

			break;
		case 3:
            buttonGiftReceive.gameObject.SetActive(false);
			buttonLook.gameObject.SetActive(false);
            buttonGiftSend.gameObject.SetActive(false);
			buttonYes.gameObject.SetActive (false);
			buttonNo.gameObject.SetActive (false);
			buttonApply.gameObject.SetActive (false);
			HeadLook.GetComponent<BoxCollider>().enabled = false;
			break;
        case 5 :
            buttonGiftReceive.gameObject.SetActive(false);
            buttonGiftSend.gameObject.SetActive(false);
			buttonLook.gameObject.SetActive(false);
            buttonYes.gameObject.SetActive (false);
			buttonNo.gameObject.SetActive (false);
			buttonApply.gameObject.SetActive (false);
            UI_ChatBtn.gameObject.SetActive(true);
            UI_ChatBtn.info = info;
            UI_ChatBtn.fatherWindow = fatherWindow;
            HeadLook.enabled = false;
            break;
		}
		MaskWindow.UnlockUI();
	}
}
