using UnityEngine;
using System.Collections;

public class ChatShowButton : ButtonBase {

	public Chat chat;
	public string uid;
	public ServerCardMsg card;
	public Equip equip;
	public int showType;//0=头像，1=装备，2=卡片,999=什么都不做
	
	public override void DoClickEvent ()
	{
		base.DoClickEvent ();

		if (showType == 0)
		{
			getPlayerInfoFPort(uid);
		} else if (showType == 1) {
			UiManager.Instance.openWindow <EquipAttrWindow>((win)=>{
				win.Initialize (equip, EquipAttrWindow.OTHER, null);
			});
		} else if (showType == 2) {
            CardBookWindow.Show(card,CardBookWindow.CLICKCHATSHOW);
            
		}
		MaskWindow.UnlockUI();
	}
	
	private void getPlayerInfoFPort(string _uid)
	{
		ChatGetPlayerInfoFPort fport = FPortManager.Instance.getFPort("ChatGetPlayerInfoFPort") as ChatGetPlayerInfoFPort;
		fport.access(uid,null,null,PvpPlayerWindow.FROM_OTHER);
	}
	

}
