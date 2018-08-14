using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 聊天主界面
 * @authro 陈世惟  
 * */
public class ChatWindow : WindowBase
{
    public Vector3[] UI_FriendSendBtnPos;
    public GameObject UI_ChatMsgTips;
	public TapContentBase tapBase;//分页按钮
	public GameObject buttonOpenKey;
	public GameObject showButton;
	public GameObject sendButton;
	public UILabel numLabel;
    

	public UITextList textWorldList;
	public UITextList textGuildList;
    public UITextList textFriendList;

    private int mNowChannel = ChatManagerment.CHANNEL_WORLD;
    
    protected override void begin()
    {
        base.begin();
        MaskWindow.UnlockUI();
    }

    public override void OnBeginCloseWindow()
    {
        base.OnBeginCloseWindow();
        if (ChatManagerment.Instance.tipsNewFriendMsg == -1)
            ChatManagerment.Instance.tipsNewFriendMsg = 0;
        if (UiManager.Instance.mainWindow != null)
            UiManager.Instance.mainWindow.UpdateChatMsgTips();
        if (UiManager.Instance.missionMainWindow != null)
            UiManager.Instance.missionMainWindow.UpdateChatMsgTips();
    }

	protected override void DoEnable()
	{
		UiManager.Instance.backGround.switchBackGround("backGround_1");
		if (MissionManager.instance != null)
			MissionManager.instance.hideAll ();
		if (ChatManagerment.Instance.sendType == ChatManagerment.CHANNEL_FRIEND)
			ChatManagerment.Instance.tipsNewFriendMsg = -1;

	}

    public void initChatWindow(int intoTyep)
    {
//		if (ChatManagerment.GuildNum != ChatManagerment.Instance.getAllChatByGuild().Count) {
//			numLabel.gameObject.SetActive (true);//显示
//			numLabel.text = ChatManagerment.Instance.getAllChatByGuild().Count - ChatManagerment.GuildNum + "";
//		} else {
//			numLabel.gameObject.SetActive (false);
//		}
        InitData();
        InitializeTap(intoTyep);
        ChatManagerment.Instance.chatStatus = true;
        ChatManagerment.Instance.setUpdate(OnChat);
        updateChatFriendTips();

    }

    public override void DoDisable()
    {
        base.DoDisable();
        ChatManagerment.Instance.changeChatIco();
        ChatManagerment.Instance.chatStatus = false;
		if (ChatManagerment.Instance.sendType == ChatManagerment.CHANNEL_GUILD) {
			updateGuildNum();
		}

    }
    
    //按钮事件
    public override void buttonEventBase(GameObject gameObj)
    {
        base.buttonEventBase(gameObj);
		
        //发消息
        if (gameObj.name == "buttonSendMsg")
        {
            UiManager.Instance.openDialogWindow<ChatSendMsgWindow>((win) => {
                win.initWindow(mNowChannel);
            });
            if (buttonOpenKey.activeSelf)
                buttonOpenKey.SetActive(false);
        }
        
        if (gameObj.name == "buttonShow")
        {
            buttonKey();
        }
        
        if (gameObj.name == "buttonShowCard")
        {
            showCard(mNowChannel);
            buttonKey();
        }
        
        if (gameObj.name == "buttonShowEquip")
        {
            showEquip(mNowChannel);
            buttonKey();
        }
        
        if (gameObj.name == "close") {
			ChatManagerment.Instance.setUpdate(null);
			ChatManagerment.Instance.chatCard=null;
			finishWindow();
			if (MissionManager.instance != null)
			{
				MissionManager.instance.showAll ();
				MissionManager.instance.setBackGround();
			}
        }
    }
    
    public void buttonKey()
    {
        if (!buttonOpenKey.activeSelf)
        {
            changeScale(buttonOpenKey, true);
        } else
        {
            changeScale(buttonOpenKey, false);
        }
    }

    public void changeScale(GameObject obj, bool isopen)
    {
        if (isopen)
        {
            obj.SetActive(isopen);
            obj.GetComponent<TweenScale>().from = new Vector3(1, 0f, 1);
            obj.GetComponent<TweenScale>().to = new Vector3(1, 1, 1);
            TweenScale.Begin<TweenScale>(obj, 0.1f);
        } else
        {
            obj.GetComponent<TweenScale>().from = new Vector3(1, 1, 1);
            obj.GetComponent<TweenScale>().to = new Vector3(1, 0f, 1);
            TweenScale.Begin<TweenScale>(obj, 0.1f);
			StartCoroutine(objShow(obj));
        }
        MaskWindow.UnlockUI();
    }


    /// <summary>
    /// 更新好友消息提示
    /// </summary>
    private void updateChatFriendTips()
    {
        UI_ChatMsgTips.SetActive(ChatManagerment.Instance.tipsNewFriendMsg > 0 && ChatManagerment.Instance.sendType != ChatManagerment.CHANNEL_FRIEND);
    }
	/// <summary>
	/// 更新公会聊天消息提示
	/// </summary>
	private void updateChatGuildTips()
	{
//		numLabel.gameObject.SetActive(ChatManagerment.Instance.getAllChatByGuild().Count - ChatManagerment.GuildNum > 0 && ChatManagerment.Instance.sendType != ChatManagerment.CHANNEL_GUILD);
	}

	IEnumerator objShow(GameObject obj)
	{
		yield return new WaitForSeconds(0.32f);
		obj.SetActive(false);
	}

    //切换页面
    public override void tapButtonEventBase(GameObject gameObj, bool enable)
    {
        base.tapButtonEventBase(gameObj, enable);
        
        if (gameObj.name == "buttonWorld" && enable == true)
        {
            ChatManagerment.Instance.sendType = ChatManagerment.CHANNEL_WORLD;
			textWorldList.scrollBar.gameObject.SetActive (true);
			textGuildList.scrollBar.gameObject.SetActive (false);
            textFriendList.scrollBar.gameObject.SetActive(false);
			updateGuildNum();
        }
        if (gameObj.name == "buttonGuild" && enable == true)
        {
            ChatManagerment.Instance.sendType = ChatManagerment.CHANNEL_GUILD;
			textWorldList.scrollBar.gameObject.SetActive (false);
			textGuildList.scrollBar.gameObject.SetActive (true);
            textFriendList.scrollBar.gameObject.SetActive(false);
			updateGuildNum();
			numLabel.gameObject.SetActive (false);
        }
        if (gameObj.name == "buttonFriend" && enable == true)
        {
            ChatManagerment.Instance.sendType = ChatManagerment.CHANNEL_FRIEND;
            textWorldList.scrollBar.gameObject.SetActive(false);
            textGuildList.scrollBar.gameObject.SetActive(false);
            textFriendList.scrollBar.gameObject.SetActive(true);

            numLabel.gameObject.SetActive(false);
            ChatManagerment.Instance.tipsNewFriendMsg = -1;
        }
        sendButton.transform.localPosition = ChatManagerment.Instance.sendType == ChatManagerment.CHANNEL_FRIEND ? UI_FriendSendBtnPos[1] : UI_FriendSendBtnPos[0];
        showButton.SetActive(ChatManagerment.Instance.sendType != ChatManagerment.CHANNEL_FRIEND);
        mNowChannel = ChatManagerment.Instance.sendType;
        if (ChatManagerment.Instance.tipsNewFriendMsg == -1 && mNowChannel != ChatManagerment.CHANNEL_FRIEND)
            ChatManagerment.Instance.tipsNewFriendMsg = 0;
        if (buttonOpenKey.activeSelf)
            buttonOpenKey.SetActive(false);
        updateChatFriendTips();
    }
    
    //展示装备
    public void showEquip(int _chatChannelType)
    {
        UiManager.Instance.openWindow<EquipChooseWindow>((win) => {
            win.Initialize(EquipChooseWindow.FROM_CHAT);
        });
    }
    
    //展示卡片
    public void showCard(int _chatChannelType)
    {
        UiManager.Instance.openWindow<CardChooseWindow>((win) => {
            win.chatChannelType = _chatChannelType;
            win.Initialize(CardChooseWindow.CHATSHOW);
        });
    }

    
    //标签切换方法
    public void InitializeTap(int index)
    {
        tapBase.changeTapPage(tapBase.tapButtonList [index]);
    }

    private void InitData()
    {
        List<Chat> list = ChatManagerment.Instance.getAllChat();

        if (list != null)
        {
            foreach (Chat c in list)
            {
                OnChat(ChatManagerment.Instance.initString(c), c.channelType);
            }
        }
//        List<string> cacheList = ChatManagerment.Instance.getAllCacheFriendChat();
//        if (cacheList != null)
//        {
//            for (int i = 0; i < cacheList.Count; i++)
//            {
//                if (cacheList[i].Length <= 0) continue;
//                OnChat(cacheList[i], ChatManagerment.CHANNEL_FRIEND);
//            }
//        }
//
//		List<string> cacheGuildChats = ChatManagerment.Instance.getAllCacheGuildChats();
//		if (cacheGuildChats != null)
//		{
//			for (int i = 0; i < cacheGuildChats.Count; i++)
//			{
//				if (cacheGuildChats[i].Length <= 0) continue;
//				OnChat(cacheGuildChats[i], ChatManagerment.CHANNEL_GUILD);
//			}
//		}

    }
	public void updateGuildNum(){
		ChatManagerment.GuildNum=ChatManagerment.Instance.getAllChatByGuild().Count;
	}
    public void OnChat(string str, int channelType)
    {
        str = str + "\r\n[666666]--------------------------------------[-]";//每条聊天自动换行加分隔符
        if (channelType == ChatManagerment.CHANNEL_WORLD || channelType == ChatManagerment.CHANNEL_SYSTEM || channelType == ChatManagerment.CHANNEL_RADIO)
        {
			textWorldList.Add(str);
        }
		if (channelType == ChatManagerment.CHANNEL_GUILD)
        {
			textGuildList.Add(str);
        }
        if (channelType == ChatManagerment.CHANNEL_FRIEND)
        {
            textFriendList.Add(str);
        }
        updateChatFriendTips();
		updateChatGuildTips();
    }

	public override void OnNetResume () {
		base.OnNetResume ();
		textWorldList.Clear();
		textGuildList.Clear();
		textFriendList.Clear();
		initChatWindow(0);
	}

	public void clear()
	{
		textWorldList.Clear();
		textGuildList.Clear();
		textFriendList.Clear();
	}



}
