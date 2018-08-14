using UnityEngine;
using System.Collections;

public class FriendsShareShowButton : ButtonBase {

	public int showType;
	public ShareInfo info;

	public override void begin ()
	{
		base.begin ();
	}

	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
//		(fatherWindow as FriendsShareWindow).hideWindow();
		string sidTwo = info.sid.getValueString();
		switch(info.type)
		{
		case FriendsShareManagerment.TYPE_CARD:
			Card card = CardManagerment.Instance.createCard(StringKit.toInt(sidTwo));
			if (card != null) {
				CardBookWindow.Show(card,CardBookWindow.OTHER,back);
			} else {
				MaskWindow.UnlockUI ();
			}
			break;

		case FriendsShareManagerment.TYPE_JINHUA:
			ServerCardMsg cardServer = CardManagerment.Instance.createCardByChatServer(info.sid as ErlArray);
			if (cardServer != null) {
				CardBookWindow.Show(cardServer,CardBookWindow.CLICKCHATSHOW);
			} else {
				MaskWindow.UnlockUI ();
			}
			break;

		case FriendsShareManagerment.TYPE_EQUIP:
			Equip equip = EquipManagerment.Instance.createEquip(StringKit.toInt(sidTwo));
			if (equip != null) {
				UiManager.Instance.openWindow <EquipAttrWindow>((win)=>{
					win.Initialize (equip, EquipAttrWindow.OTHER,back);
				});
			} else {
				MaskWindow.UnlockUI ();
			}
			break;
        case FriendsShareManagerment.TYPE_MAGICWEAPON:
            MagicWeapon mw = MagicWeaponManagerment.Instance.createMagicWeapon(StringKit.toInt(sidTwo));
            if (mw != null) {
                UiManager.Instance.openWindow<MagicWeaponStrengWindow>((win) => {
                    win.init(mw,MagicWeaponType.FORM_OTHER);
                });
            } else {
                MaskWindow.UnlockUI();
            }
            break;
		}

	}

	private void back()
	{
//		UiManager.Instance.openWindow<FriendsShareWindow>();
	}
}
