using UnityEngine;
using System.Collections.Generic;

public class FriendFindRecommendWindow : WindowBase, IFriendsWindow
{
	/**緩存, 為了在好友查找推薦點擊好友詳細信息返回后還原*/
	public static FriendInfo[] CACHE_LIST;

    public ContentFriends UI_Container;
    public GameObject friendsBarPrefab;
    public UIInput inputFind;
    public ButtonBase buttonRecommend;
    public GameObject[] buttonFindType;

    private int mFindType = 1;



    public GameObject FriendsBarPrefab {
        get {
            return friendsBarPrefab;
        }
    }

    public override void OnStart()
    {
        base.OnStart();
        FriendsManagerment.Instance.clearRecommendFriends();
        inputFind.value = "";
        inputFind.label.text = LanguageConfigManager.Instance.getLanguage("s0383");
        isShowRecommendButton(true);
        updateFindTypeStatus();
    }

    protected override void begin()
    {
        base.begin();
        MaskWindow.UnlockUI();
		if (CACHE_LIST != null) {
			isShowRecommendButton (false);
			UI_Container.reLoad (2, CACHE_LIST);
		}

    }


    private void updateFindTypeStatus()
    {
        for (int i = 0; i < buttonFindType.Length; i++)
        {
            buttonFindType[i].gameObject.SetActive(i == mFindType);
        }
    }


    private void findFriend()
    {
        if (!checkInput(inputFind.value))
            return;

		string uid = inputFind.value;
        //Debug.LogError("inputFind.value" + inputFind.value.ToString());
        if (mFindType == 0)
        {
            if (!StringKit.isNum(inputFind.value))
            {
                UiManager.Instance.createMessageWindowByOneButton(LanguageConfigManager.Instance.getLanguage("FriendErrorId"), null);
                callBackByFind2();
                return;
            }
            uid = StringKit.frontIdToServerId(inputFind.value);
            if (uid == "error")
            {
                UiManager.Instance.createMessageWindowByOneButton(LanguageConfigManager.Instance.getLanguage("FriendErrorId"), null);
                callBackByFind2();
                return;
            }
        }

        FriendsFPort fport = FPortManager.Instance.getFPort("FriendsFPort") as FriendsFPort;
        //Debug.LogError("uid===="+uid.ToString());
        fport.findFriend(mFindType, uid, callBackByFind);
    }


    public bool checkInput(string str)
    {
        if (str.Replace(" ", "") == "")
            return false;
        if (str == null)
            return false;
        return true;
    }

    private void callBackByFind2()
    {
        MaskWindow.UnlockUI();
        isShowRecommendButton(true);
    }


    //要不显示下推荐好友？
    public void isShowRecommendButton(bool bo)
    {
        if (bo)
        {
            buttonRecommend.gameObject.SetActive(true);
            UI_Container.gameObject.SetActive(false);
        }
        else
        {
            buttonRecommend.gameObject.SetActive(false);
            UI_Container.gameObject.SetActive(true);
        }
    }

    //推荐好友回调
    public void callBackByFind()
    {
        FriendInfo[] info = FriendsManagerment.Instance.getRecommendFriends();

        if (info != null)
        {
            //获得查找的好友的sdk信息
            SdkFPort fp = FPortManager.Instance.getFPort<SdkFPort>();
            fp.getSdkInfo(info[0].getUid(), (dic) =>
            {
                FriendsManagerment.Instance.setSdkInfo(dic, info);
                isShowRecommendButton(false);
                UI_Container.reLoad(2);
				CACHE_LIST = UI_Container.ic;
                MaskWindow.UnlockUI();
            });


        }
        else
        {
            UiManager.Instance.createMessageWindowByOneButton(LanguageConfigManager.Instance.getLanguage("Friend_noRecommend"), null);
            isShowRecommendButton(true);
        }
    }

    public override void buttonEventBase(GameObject gameObj)
    {
        base.buttonEventBase(gameObj);

        switch (gameObj.name)
        {
            case "close":
				CACHE_LIST = null;
                finishWindow();
				if(fatherWindow is FriendsWindow){
				(fatherWindow as FriendsWindow).loadData();
			}
                break;
            case "buttonRecommend":
                FriendsManagerment.Instance.clearRecommendFriends ();
		        FriendsFPort fport = FPortManager.Instance.getFPort ("FriendsFPort") as FriendsFPort;
		        fport.recommendFriend (callBackByFind);
                break;
            case "button_id" :
                mFindType = 1;
                updateFindTypeStatus();
                break ;
            case "button_name" :
                mFindType = 0;
                updateFindTypeStatus();
                break;
            case "buttonFind" :
                if (inputFind.value.Replace (" ", "") == "" || inputFind.value == null) {
				    UiManager.Instance.createMessageWindowByOneButton (LanguageConfigManager.Instance.getLanguage ("Friend_InputNull"), null);
				    return;
			    }
                FriendsManagerment.Instance.clearRecommendFriends();
                findFriend();
                break;
        }
    }


	public void UpdateContent(){
		if (CACHE_LIST != null) {
			isShowRecommendButton (false);
			float y = UI_Container.transform.localPosition.y;
			UI_Container.reLoad (2, CACHE_LIST);
			StartCoroutine(Utils.DelayRunNextFrame(()=>{
				UI_Container.jumpToPos(y);
			}));

		}
	}

}

