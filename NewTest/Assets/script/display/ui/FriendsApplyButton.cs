using UnityEngine;
using System.Collections;

public class FriendsApplyButton : ButtonBase {

	public FriendInfo info;
	private string uid;
	//申请好友
	public void applyFriend()
	{
		if(FriendsManagerment.Instance.isFull())
			return;
		FriendsFPort fport = FPortManager.Instance.getFPort ("FriendsFPort") as FriendsFPort;
		uid = info.getUid();
		fport.applyFriend(info.getUid(),applyOk);
	}

	public void applyOk()
	{
	
		if (!uid.Equals(info.getUid()))
			return ;
		info.setApply(true);
		if(this.textLabel == null || this.gameObject == null)
			return;
		this.textLabel.text = LanguageConfigManager.Instance.getLanguage("FriendAPPLY_already_apply");
		this.disableButton(true);
	}

	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
		applyFriend();
	}
}
