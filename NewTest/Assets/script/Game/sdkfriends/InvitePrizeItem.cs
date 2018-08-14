using UnityEngine;
using System.Collections;

public class InvitePrizeItem : MonoBehaviour {

	public ButtonTotalLoginAward[] prizeButton;
	public SdkFriendButton backGetButton;
	public SdkFriendButton backViewButton;
	public SdkFriendButton getButton;
	public UILabel backMoney;
	public UILabel backDesc;
	public UILabel  getNums;
	public GameObject getNumsObj;
	public UILabel itemDesc;
	public WindowBase thisFather;
    public InvitePrize invitePrize;
	
	public void initItem(InvitePrize info,WindowBase fatherWindow)
	{
        invitePrize = info;
		thisFather = fatherWindow;
		if (info.prizeSid == "100001")
		{
			itemDesc.text = LanguageConfigManager.Instance.getLanguage("sdk_prizeitem_des0");
			prizeButton[0].updateButton(info.prizes[0],thisFather);
			prizeButton[0].setTextNum(info.backMoney);
			backDesc.text = info.backDesc;
			backViewButton.initButton(this,2,fatherWindow);
			backGetButton.initButton(this,3,fatherWindow);
			setShow(true,info);
		} else {
			itemDesc.text = LanguageConfigManager.Instance.getLanguage("sdk_prizeitem_des1",
			                                                           info.friendsLevel.ToString(),
			                                                           info.currentGetNum.ToString(),
			                                                           info.GetMax.ToString());
			prizeButton[0].updateButton(info.prizes[0],thisFather);
			prizeButton[1].updateButton(info.prizes[1],thisFather);
			getNums.text = LanguageConfigManager.Instance.getLanguage("sdk_prizeitem_getnum",info.getNums.ToString());
			getButton.initButton(this,4,fatherWindow);
			setShow(false,info);
		}
	}
	
	void setShow(bool isShow,InvitePrize info)
	{
		if (isShow)
		{
			prizeButton[1].gameObject.SetActive(false);
			getNumsObj.SetActive(false);
			getButton.gameObject.SetActive(false);
			backViewButton.gameObject.SetActive(true);
			backGetButton.gameObject.SetActive(true);
			if (info.isBackGet)
			{
				backGetButton.disableButton(false);
			} else {
				backGetButton.disableButton(true);
			}
			backDesc.gameObject.SetActive(true);
			
		} else {
			prizeButton[1].gameObject.SetActive(true);
			getNumsObj.SetActive(true);
			getButton.gameObject.SetActive(true);
			if (info.isGetLevelPrize)
			{
				getButton.disableButton(false);
			} else {
				getButton.disableButton(true);
			}
			backViewButton.gameObject.SetActive(false);
			backGetButton.gameObject.SetActive(false);
			backDesc.gameObject.SetActive(false);
			
		}
	}
}
