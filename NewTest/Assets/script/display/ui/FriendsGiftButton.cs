using UnityEngine;
using System.Collections;


public enum E_FriendsGiftType
{ 
    Send,
    Receive,
}


public class FriendsGiftButton : ButtonBase
{
    public const int PVE_GIFT = 2;


    public E_FriendsGiftType type;
	public FriendInfo info;
	public GameObject thunder;


    public override void begin()
    {
        base.begin();
        updateBtnStatus();
    }


    public void updateBtnStatus()
    {
        if (type == E_FriendsGiftType.Receive) {
			disableButton (info.getGiftReceiveStatus () != 1);
			if (info.getGiftReceiveStatus () == 3){
				textLabel.text = Language ("recharge02");
				thunder.gameObject.SetActive (false);
			}
			else
				textLabel.text = Language ("Friends19");
		}
		else if (type == E_FriendsGiftType.Send) {
			disableButton (info.getGiftSendStatus () == 1);
			if (info.getGiftSendStatus () == 2)
				textLabel.text = Language ("Friends20");
			else {
				textLabel.text = Language ("Friends34");
				thunder.gameObject.SetActive (false);
			}
		}
	}


	public override void DoClickEvent ()
	{
		base.DoClickEvent ();

        FriendsGiftFPort fport = FPortManager.Instance.getFPort<FriendsGiftFPort>();
        if (type == E_FriendsGiftType.Send)
        {
            fport.send(() => {
                info.setGiftSendStatus(1);
                updateBtnStatus();
                UiManager.Instance.openDialogWindow<MessageLineWindow>((win) => {
					win.Initialize(LanguageConfigManager.Instance.getLanguage("Friends27",info.getName())); 
                });
            }, info.getUid());
        }
        else if (type == E_FriendsGiftType.Receive)
        {
            int totalPveMax = CommonConfigSampleManager.Instance.getSampleBySid<PvePowerMaxSample>(CommonConfigSampleManager.PvePowerMax_SID).pvePowerMax;//总的存储行动力上限，默认为600
            Friends friends = FriendsManagerment.Instance.getFriends();
            if (friends.giftReceiveCount >= friends.giftReceiveMax)
            {
                UiManager.Instance.openDialogWindow<MessageLineWindow>((win) => { win.Initialize(Language("Friends22")); });
                return;
            }
            if (UserManager.Instance.self.getPvEPoint() + PVE_GIFT > UserManager.Instance.self.getPvEPointMax())//totalPveMax)
            {
                UiManager.Instance.openDialogWindow<MessageLineWindow>((win) => { win.Initialize(Language("Friends23")); });
                return;
            }

            fport.receive(() => {
                FriendsManagerment.Instance.GiftReceive();
                (fatherWindow as FriendsWindow).updateGiftCount();
                info.setGiftReceiveStatus(3);
                updateBtnStatus();
                UiManager.Instance.openDialogWindow<MessageLineWindow>((win) => {
                win.Initialize(Language("Friends28")); 
                });
            }, info.getUid());
        }




	}


}
