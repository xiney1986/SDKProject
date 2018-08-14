using UnityEngine;
using System.Collections;

public class FriendApplyWindow : WindowBase, IFriendsWindow
{
    public ContentFriends UI_Container;
    public GameObject friendsBarPrefab;


    public GameObject FriendsBarPrefab {
        get {
            return friendsBarPrefab;
        }
    }



    protected override void begin()
    {
        base.begin();
        MaskWindow.UnlockUI();
        reload();
    }


    public override void OnBeginCloseWindow()
    {
        base.OnBeginCloseWindow();
        ((FriendsWindow)fatherWindow).updatepage(0);
        ((FriendsWindow)fatherWindow).showNewApply();
    }

    public void reload()
    {
        UI_Container.reLoad(1);
        if (UI_Container.ic.Length <= 0)
            finishWindow();
    }

    public override void buttonEventBase(GameObject gameObj)
    {
        base.buttonEventBase(gameObj);
		if (gameObj.name == "close") {
			finishWindow ();
		} else if(gameObj.name == "icon") {
			finishWindow ();
		}
    }
}
