using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 好友主窗口
 * @authro 陈世惟  
 * */
public class FriendsWindow : WindowBase, IFriendsWindow
{
	public UILabel labelFriendsAmount;//好友数量
	public UILabel labelMyUid;//我的UID
    public UILabel labelGiftReceiveCount;//好友禮物領取次數
	public UILabel labelGiftTtile;
	public UILabel labelFriendChatTips; //好友聊天提示選擇好友
	public UILabel labelFriendChatNoFriendTips; //好友聊天没有好友提示
	public ButtonBase buttonRecommend;//推荐好友
    public ButtonBase buttonApply;
	public ButtonBase buttonRecommendFind;
    public GameObject[] buttonFindType;

	public TapContentBase tapBase;//分页按钮
	public ContentFriends content1;//好友容器
	public ContentFriends content2;//待批准容器
	public ContentFriends content3;//查找容器

	public GameObject friendsBarPrefab;
	public UIInput inputFind;
	public UISprite bgNum;//待申请背景
	public UILabel labelNum;//待申请数目
	private Friends friendsInfo;//好友信息
	private int tapType = 0;//属于哪个标签。0好友信息，1待批准，2查找申请

	private bool mIsFriendChat = false; //是否是好友聊天
    private int mFindType = 1;


    public GameObject FriendsBarPrefab {
        get {
            return friendsBarPrefab;
        }
    }
	/// <summary>
	/// 打开查找好友窗口
	/// </summary>
	public void openFriendApplyWindow()
	{
		UiManager.Instance.openDialogWindow<FriendFindRecommendWindow>((win) => {
			win.SetFatherWindow(this);
		});
	}

	public int getTapType ()
	{
		return tapType;
	}

	protected override void begin ()
	{
		base.begin ();
		friendsInfo = null;

		if (friendsInfo == null)
			getFriendsInfo ();
		else
			loadData ();
        updateFindTypeStatus();
		MaskWindow.UnlockUI ();
	}

	public void updatepage (int  tapIndex)
	{
		tapBase.resetTap ();
		initWin (tapIndex);
		loadData ();
	}

	/// <summary>
	/// 获得玩家附加信息
	/// </summary>
	void getSdkInfo (int type, CallBack<Dictionary<string, PlatFormUserInfo>> cb)
	{
		string strArray = "";
		if (type == 1) {
			//获得好友和待批转好友的sdk信息
			FriendInfo[] infos = FriendsManagerment.Instance.getFriendList ();
			if (infos != null && infos.Length > 0) {

				for (int i=0; i< infos.Length; i++) {
					strArray += infos [i].getUid () + ",";
				}
			}

			infos = FriendsManagerment.Instance.getRequestFriendList ();
			if (infos != null && infos.Length > 0) {
				for (int i=0; i< infos.Length; i++) {
					strArray += infos [i].getUid () + ",";
				}
			}
		} else {

			FriendInfo[] infos = FriendsManagerment.Instance.getRecommendFriends ();
			if (infos != null && infos.Length > 0) {
				for (int i=0; i< infos.Length; i++) {
					strArray += infos [i].getUid () + ",";
				}

			}
		}

		if (strArray == "") {
			cb (null);
			return;
		}
		strArray = strArray.Substring (0, strArray.Length - 1);
		SdkFPort fp = FPortManager.Instance.getFPort<SdkFPort> ();
		fp.getSdkInfo (strArray, cb);

	}



	//断线重连
	public override void OnNetResume ()
	{
		base.OnNetResume ();
		tapBase.resetTap ();
		initWin (tapType);
	}


	public void initWin (int _tap)
	{
		//	friendsInfo=null;
		//极端情况这里会走两次,正常一次,取不到好友begin的时候取再走一次;
		friendsInfo = FriendsManagerment.Instance.getFriends ();
		tapType = _tap;
	}

	public void loadData ()
	{
		//tapInitialize (tapType);
		tapType = mIsFriendChat ? 5 : 0;
		content1.reLoad (tapType);

		friendsInfo = FriendsManagerment.Instance.getFriends ();
		if (friendsInfo != null) {
			showUI ();
			showNewApply ();
		}
	}

    private void updateFindTypeStatus()
    {
        for (int i = 0; i < buttonFindType.Length; i++)
        {
            buttonFindType[i].gameObject.SetActive(i == mFindType);
        }
    }

    public bool checkInput(string str)
    {
        if (str.Replace(" ", "") == "")
            return false;
        if (str == null)
            return false;
        return true;
    }

    private void findFriend()
    {
        if (!checkInput(inputFind.value))
            return;
        if (mFindType == 0)
        {
            if (!StringKit.isNum(inputFind.value))
            {
                UiManager.Instance.createMessageWindowByOneButton(LanguageConfigManager.Instance.getLanguage("FriendErrorId"), null);
                callBackByFind2();
                return;
            }
            string _uid = StringKit.frontIdToServerId(inputFind.value);
            if (_uid == "error")
            {
                UiManager.Instance.createMessageWindowByOneButton(LanguageConfigManager.Instance.getLanguage("FriendErrorId"), null);
                callBackByFind2();
                return;
            }
        }

        FriendsFPort fport = FPortManager.Instance.getFPort("FriendsFPort") as FriendsFPort;
        fport.findFriend(mFindType, inputFind.value, callBackByFind);
    }

	/// <summary>
	/// 初始化好友聊天
	/// </summary>
	public void initFriendChat ()
	{
		mIsFriendChat = true;
		buttonApply.gameObject.SetActive (false);
		buttonRecommendFind.gameObject.SetActive (false);
		labelGiftReceiveCount.gameObject.SetActive (false);
		labelGiftTtile.gameObject.SetActive(false);
		labelFriendChatTips.gameObject.SetActive (friendsInfo.getAmount() > 0);
		labelFriendChatNoFriendTips.gameObject.SetActive (!labelFriendChatTips.gameObject.activeSelf);
	}

	public void showUI ()
	{
		labelFriendsAmount.text = friendsInfo.getAmount () + " / " + friendsInfo.getMaxSize ();
		labelMyUid.text = StringKit.serverIdToFrontId (UserManager.Instance.self.uid);
	}

	//显示待处理玩家申请小标签
	public void showNewApply ()
	{
		if (friendsInfo.request == null || friendsInfo.request.Length <= 0 || mIsFriendChat) {
			labelNum.gameObject.SetActive (false);
			bgNum.gameObject.SetActive (false);
            buttonApply.disableButton(true);
		} else {
			labelNum.gameObject.SetActive (true);
			bgNum.gameObject.SetActive (true);
			labelNum.text = friendsInfo.request.Length + "";
            buttonApply.disableButton(false);
		}
	}

	//初始化好友信息
	public void getFriendsInfo ()
	{
		FriendsManagerment.Instance.getFriendsInfo (() => {
			//获得好友和待批准好友
			getSdkInfo (1, (dic) => {
				FriendsManagerment.Instance.setSdkInfo (dic);
				loadData ();
			});
            updateGiftCount();
		});
	}


    public void updateGiftCount()
    {
		int max	= FriendsManagerment.Instance.getFriends().giftReceiveMax;
        labelGiftReceiveCount.text =max - FriendsManagerment.Instance.getFriends().giftReceiveCount+"/"+ max;
    }

	//显示查找页面
	public void showFindUI ()
	{
		FriendsManagerment.Instance.clearRecommendFriends ();
		inputFind.value = "";
		inputFind.label.text = LanguageConfigManager.Instance.getLanguage ("s0383");
		isShowRecommendButton (true);
	}

	//要不显示下推荐好友？
	public void isShowRecommendButton (bool bo)
	{
		if (bo) {
			buttonRecommend.gameObject.SetActive (true);
			content3.gameObject.SetActive (false);
		} else {
			buttonRecommend.gameObject.SetActive (false);
			content3.gameObject.SetActive (true);
		}
	}

	private void callBackByFind2 ()
	{
        MaskWindow.UnlockUI();
		isShowRecommendButton (true);
	}

	//获取推荐好友端口
	public void getRecommendFrind ()
	{
		FriendsFPort fport = FPortManager.Instance.getFPort ("FriendsFPort") as FriendsFPort;
		fport.recommendFriend (callBackByFind);
	}

	//推荐好友的sdk信息



	//推荐好友回调
	public void callBackByFind ()
	{
		FriendInfo[] info = FriendsManagerment.Instance.getRecommendFriends ();

		if (info != null) {
			//获得查找的好友的sdk信息
			SdkFPort fp = FPortManager.Instance.getFPort<SdkFPort> ();
			fp.getSdkInfo (info [0].getUid (), (dic) => {
				FriendsManagerment.Instance.setSdkInfo (dic, info);
				isShowRecommendButton (false);
				content3.reLoad (tapType);
				MaskWindow.UnlockUI ();
			});

	
		} else {
			UiManager.Instance.createMessageWindowByOneButton (LanguageConfigManager.Instance.getLanguage ("Friend_noRecommend"), null);
			isShowRecommendButton (true);
		}
	}


	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);

		if (gameObj.name == "close") {
			finishWindow ();
		} else if (gameObj.name == "buttonFind") {
			if (inputFind.value.Replace (" ", "") == "" || inputFind.value == null) {
				UiManager.Instance.createMessageWindowByOneButton (LanguageConfigManager.Instance.getLanguage ("Friend_InputNull"), null);
				return;
			}
            FriendsManagerment.Instance.clearRecommendFriends();
            //UiManager.Instance.openDialogWindow<FriendFindWindow>((win) => {
            //    win.initWin(inputFind.value, callBackByFind, callBackByFind2);
            //});
            findFriend();
		} else if (gameObj.name == "buttonRecommend") {
			FriendsManagerment.Instance.clearRecommendFriends ();
			getRecommendFrind ();
		}
        else if (gameObj.name == buttonFindType[0].name)
        {
            mFindType = 1;
            updateFindTypeStatus();
        }
        else if (gameObj.name == buttonFindType[1].name)
        {
            mFindType = 0;
            updateFindTypeStatus();
        }
        else if (gameObj.name == "btnApply")
        {
            UiManager.Instance.openDialogWindow<FriendApplyWindow>((win) => {
                win.SetFatherWindow(this);
            });
        }
		else if (gameObj.name == "btnRecommendFind")
        {
            UiManager.Instance.openDialogWindow<FriendFindRecommendWindow>((win) => {
                win.SetFatherWindow(this);
            });
        }
        

	}

	//标签切换方法
	public void tapInitialize (int index)
	{
		tapBase.changeTapPage (tapBase.tapButtonList[index]);
	}

	//切换页面
	public override void tapButtonEventBase (GameObject gameObj, bool enable)
	{
		base.tapButtonEventBase (gameObj, enable);
		
		if (gameObj.name == "tapFriends" && enable == true) {
			tapType = mIsFriendChat ? 5 : 0;
			content1.reLoad (tapType);
		} else if (gameObj.name == "tapApply" && enable == true) {
			tapType = 1;
			content2.reLoad (tapType);
		} else if (gameObj.name == "tapFind" && enable == true) {
			tapType = 2;
			showFindUI ();
		}
	}

	public void clickFriends ()
	{
//		tapType = 0;
		switch (tapType) {
		case 0:
			content1.reLoad (tapType);
			break;
		case 1:
			content2.reLoad (tapType);
			break;
		case 2:
			content3.reLoad (tapType);
			break;
		}
	}

}
