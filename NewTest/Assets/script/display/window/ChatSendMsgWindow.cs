using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/**
 * 聊天消息发送窗口
 * @authro 陈世惟  
 * */
using System;


public class ChatSendMsgWindow : WindowBase {
    public const int CHAT_MAX_RECENT = 5;

    public Vector3 BtnSendPos;
    public GameObject UI_BtnSend;
    public GameObject UI_BtnShow;
    public GameObject UI_BtnShowMenu;
    public UISprite UI_Bg;
    public int UI_DefualtY;
    public Vector3 UI_DefulatClosePos;
	public Vector3 msgLabelPos;
    public Transform UI_CloseBtn;
    public UIPopupList UI_RecentList;
    public GameObject UI_ChatFriend;
    public UIInput msgInput;
    
	
    private int sendType = 1;
    private Dictionary<string, FriendInfo> mRecentDic;



    public override void OnStart()
    {
        base.OnStart();
        msgInput.isSelected = false;
    }
    
	protected override void begin ()
	{
		base.begin ();
		MaskWindow.UnlockUI ();
	}


    /// <summary>
    /// 更新最近聊天好友
    /// </summary>
    private void updateRecentFriends()
    {
        ChatManagerment.Instance.CurrentFriendInfo = null;
        UI_RecentList.value = "";
        UI_RecentList.items.Clear();
        foreach (KeyValuePair<string, FriendInfo> item in mRecentDic)
        {
            UI_RecentList.items.Add(item.Key);
        }
        if (UI_RecentList.items.Count > 0)
        {
            ChatManagerment.Instance.CurrentFriendInfo = mRecentDic[UI_RecentList.items[0]];
            UI_RecentList.value = UI_RecentList.items[0];
        }
    }

	
	public void initWindow(int _sendType)
	{
		sendType = _sendType;

        if (sendType != ChatManagerment.CHANNEL_FRIEND)
        {
            UI_BtnShow.SetActive(false);
            UI_BtnSend.transform.localPosition = BtnSendPos;
            UI_ChatFriend.SetActive(false);
//            UI_Bg.height = UI_DefualtY;
			msgInput.transform.localPosition = msgLabelPos;
			UI_BtnSend.transform.localScale = new Vector3(1.3f,1.3f,1f);
            UI_CloseBtn.localPosition = UI_DefulatClosePos;
        }
        else
        {
            initRecentFriend();
        }
	}


    /// <summary>
    /// 设置发送的好友对象值,仅仅是PopupList value
    /// </summary>
    public void setFriendValue(FriendInfo friendInfo)
    {
        ChatManagerment.Instance.CurrentFriendInfo = friendInfo;
        UI_RecentList.value = ChatManagerment.Instance.CurrentFriendInfo.getName();
        saveCacheRecent();
        initRecentFriend();
    }

	
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if(gameObj.name == "buttonSend")
		{
			sendMsgFPort();
		}
        else if (gameObj.name == "buttonShow")
        {
            buttonKey();
        }
        else if (gameObj.name == "buttonClose")
        {
            finishWindow();
        }
        else if (gameObj.name == "Btn_SelecteFriendBtn")
        {
			FriendsWindow friendsWin=UiManager.Instance.getWindow<FriendsWindow>();
			if(friendsWin!=null)
				friendsWin.destoryWindow();
            finishWindow();
			EventDelegate.Add(OnHide,()=>{
				UiManager.Instance.openWindow<FriendsWindow>((win) => {
					win.initWin(0);
					win.initFriendChat ();
				});
			});
        }
        else if (gameObj.name == "buttonShowCard")
        {
            if (ChatManagerment.Instance.CurrentFriendInfo == null)
            {
                UiManager.Instance.openDialogWindow<MessageLineWindow>((win) =>
                {
                    win.Initialize(LanguageConfigManager.Instance.getLanguage("s0485"));
                });
                return;
            }
            UiManager.Instance.openWindow<CardChooseWindow>((win) =>
            {
                win.chatChannelType = ChatManagerment.CHANNEL_FRIEND;
                win.Initialize(CardChooseWindow.CHATSHOW);
            });
            //buttonKey();
            finishWindow();
            saveCacheRecent();
        }

        else if (gameObj.name == "buttonShowEquip")
        {
            if (ChatManagerment.Instance.CurrentFriendInfo == null)
            {
                UiManager.Instance.openDialogWindow<MessageLineWindow>((win) =>
                {
                    win.Initialize(LanguageConfigManager.Instance.getLanguage("s0485"));
                });
                return;
            }
            UiManager.Instance.openWindow<EquipChooseWindow>((win) => {
                win.Initialize(EquipChooseWindow.FROM_CHAT_FRIEND);
            });
            //buttonKey();
            finishWindow();
            saveCacheRecent();
        }

	}


    public void buttonKey()
    {
        if (!UI_BtnShowMenu.activeSelf)
        {
            UiManager.Instance.getWindow<ChatWindow>().changeScale(UI_BtnShowMenu, true);
        }
        else
        {
            UiManager.Instance.getWindow<ChatWindow>().changeScale(UI_BtnShowMenu, false);
        }
    }
	
	//发送消息调用
	private void sendMsgFPort()
	{
		string str = msgInput.label.text;
		if(str.Replace(" ","") == ""){
			MaskWindow.UnlockUI();
			return;
		}
			
		if(msgInput.label.text == null){
			MaskWindow.UnlockUI();
			return;
		}
        ChatSendMsgFPort fport = FPortManager.Instance.getFPort("ChatSendMsgFPort") as ChatSendMsgFPort;
        if (sendType == ChatManagerment.CHANNEL_FRIEND)
        {
            if (ChatManagerment.Instance.CurrentFriendInfo == null)
            {
                UiManager.Instance.openDialogWindow<MessageLineWindow>((win) =>
                {
                    win.Initialize(LanguageConfigManager.Instance.getLanguage("s0485"));
                });
            }
            else
            {
                saveCacheRecent();
                fport.access(ChatManagerment.Instance.CurrentFriendInfo.getUid(), UserManager.Instance.self.uid, sendType, msgInput.label.text, null, null, onReceiveSendMsg);

            }
        }
		else{
			if(ChatManagerment.Instance.Filter(msgInput.label.text,sendType)){
				onReceiveSendMsg();
			}else{
				fport.access(sendType, msgInput.label.text, null, null, onReceiveSendMsg);
			}
				
		}
           
	}

    private void onReceiveSendMsg()
    {
        finishWindow();
    }
	
	
    /// <summary>
    /// 从最近聊天好友选择某个好友
    /// </summary>
    private void onSelecteRecentFriend(GameObject go, bool selected)
	{
        if (selected || mRecentDic.Count <= 0) return;
        if (mRecentDic.ContainsKey(UI_RecentList.value))
            ChatManagerment.Instance.CurrentFriendInfo = mRecentDic[UI_RecentList.value];
	}



    /// <summary>
    /// 好友聊天,初始化最近记录
    /// </summary>
    private void initRecentFriend()
    {
		string[] friendUidArr = PlayerPrefs.GetString(UserManager.Instance.self.uid + PlayerPrefsComm.CHAT_RECENT_FRIENDS, "").Split('|');
        mRecentDic = new Dictionary<string, FriendInfo>();
        for (int i = 0; i < friendUidArr.Length; i++)
        {
            FriendInfo info = FriendsManagerment.Instance.getFriendByUid(friendUidArr[i]);
            if (info != null)
                mRecentDic.Add(info.getName(), info);
        }
        updateRecentFriends();
        UIEventListener.Get(UI_RecentList.gameObject).onSelect = onSelecteRecentFriend;
    }


    /// <summary>
    /// 保存最近聊天的好友
    /// </summary>
    private void saveCacheRecent()
    {

        if (ChatManagerment.Instance.CurrentFriendInfo == null) return;

		if (mRecentDic.ContainsKey(ChatManagerment.Instance.CurrentFriendInfo.getName())) {
			mRecentDic.Remove(ChatManagerment.Instance.CurrentFriendInfo.getName());
		}
        string cacheStr = ChatManagerment.Instance.CurrentFriendInfo.getUid();
        int i = 0;
        foreach (KeyValuePair<string, FriendInfo> item in mRecentDic)
        {
            if (i >= CHAT_MAX_RECENT - 1)
                break;
            if (item.Value == ChatManagerment.Instance.CurrentFriendInfo) continue;
            else i++;
            cacheStr += "|" + item.Value.getUid();
        }
		PlayerPrefs.SetString(UserManager.Instance.self.uid + PlayerPrefsComm.CHAT_RECENT_FRIENDS, cacheStr);
    }






}
