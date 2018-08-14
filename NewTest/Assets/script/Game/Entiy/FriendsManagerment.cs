using System.Linq;
using System.Collections.Generic;

/**
 * 好友管理器
 * @authro 陈世惟  
 * */
public class FriendsManagerment
{

	private FriendInfo[] recommendFriends;//推荐好友
	private Friends friends = null;
	private const int MAXNUM = 30;//单个容器数目
	private const int SHOWMSGLEVEL = 5;//显示提示信息等级


	public static FriendsManagerment Instance {
		get{ return SingleManager.Instance.getObj ("FriendsManagerment") as FriendsManagerment;}
	}

	/// <summary>
	/// 好友满了没,true=满，false=没满
	/// </summary>
	public bool isFull ()
	{
		if (getFriends ().getAmount () >= getFriends ().getMaxSize ()) {
			UiManager.Instance.createMessageWindowByOneButton (LanguageConfigManager.Instance.getLanguage ("FriendAPPLY_size_limit"), null);
			return true;
		} else 
			return false;
	}

	/// <summary>
	/// 获得好友上限
	/// </summary>
	public int getFriendMaxAmount ()
	{
		return getFriends ().getMaxSize ();
	}

	/// <summary>
	/// 获得好友数量
	/// </summary>
	public int getFriendAmount ()
	{
		return getFriends ().getAmount ();
	}

	/// <summary>
	/// 获得待批准好友数量
	/// </summary>
	public int getRequestAmount ()
	{
		return getFriends ().getRequestAmount ();
	}

    /// <summary>
    /// 获得是否有可以领取好友赠送的活力
    /// </summary>
    public bool getCanReceiveGift()
    {
		if (friends.giftReceiveCount >= friends.giftReceiveMax) 
			return false;

        if (UserManager.Instance.self.getPvEPoint() + FriendsGiftButton.PVE_GIFT > UserManager.Instance.self.getPvEPointMax() || friends.friends == null)
            return false;

        for (int i = 0; i < friends.friends.Length; i++)
        {
            if (friends.friends[i].getGiftReceiveStatus() == 1)
            {
                return true;
            }
        }
        return false;
    }

	/// <summary>
	/// 获得推荐好友
	/// </summary>
	public FriendInfo[] getRecommendFriends ()
	{
		if (recommendFriends == null)
			return null;
		FriendInfo[] info = recommendFriends;
		FriendInfo item;
		
		for (int i=0; i<info.Length; i++) {
			for (int j=i+1; j<info.Length; j++) {
				if (info [i].getLevel () < info [j].getLevel ()) {
					item = info [i];
					info [i] = info [j];
					info [j] = item;
				}
			}
		}
		return info;
	}

	/// <summary>
	/// 清除推荐好友列表
	/// </summary>
	public void clearRecommendFriends ()
	{
		recommendFriends = null;
	}

	/// <summary>
	/// 创建推荐好友
	/// </summary>
	public void createRecommendFriends (ErlArray ea)
	{
		clearRecommendFriends ();
		recommendFriends = createFriendInfoByErlArray (ea);
	}

	/// <summary>
	/// 获取最新的好友信息端口
	/// </summary>
	public void getFriendsInfo (CallBack _callback)
	{
		FriendsFPort fport = FPortManager.Instance.getFPort ("FriendsFPort") as FriendsFPort;
		fport.initFriendsInfo (_callback);
	}

	/// <summary>
	///  附加平台信息
	/// </summary>
	public void setSdkInfo (Dictionary<string, PlatFormUserInfo> dic, FriendInfo[] friends)
	{
		if (friends == null || friends.Length < 1)
			return;


		foreach (FriendInfo each in friends) {
			if (dic.ContainsKey (each.getUid ()))
				each.setSdkInfo (dic [each.getUid ()]);
		}
	}


	/// <summary>
	///  附加平台信息
	/// </summary>
	public void setSdkInfo (Dictionary<string, PlatFormUserInfo> dic)
	{
		if (dic == null || dic.Values == null || dic.Values.Count < 1)
			return;
		FriendInfo[] infos = FriendsManagerment.Instance.getFriendList ();
		if (infos != null) {
			foreach (FriendInfo each in infos) {
			
				if (dic.ContainsKey (each.getUid ())) {
					each.setSdkInfo (dic [each.getUid ()]);
				}
			}
		}
		infos = FriendsManagerment.Instance.getRequestFriendList ();
		if (infos != null) {
			foreach (FriendInfo each in infos) {
			
				if (dic.ContainsKey (each.getUid ())) {
					each.setSdkInfo (dic [each.getUid ()]);
				}

			}
		}

		infos = FriendsManagerment.Instance.getRecommendFriends ();
		if (infos != null) {
			foreach (FriendInfo each in infos) {
			
				if (dic.ContainsKey (each.getUid ())) {
					each.setSdkInfo (dic [each.getUid ()]);
				}
			}
		}
	}

	/// <summary>
	/// 获得好友基础信息
	/// </summary>
	public Friends getFriends ()
	{
		return friends;
	}

	/// <summary>
	/// 获得好友列表
	/// </summary>
	public FriendInfo[] getFriendList ()
	{
		if (getFriends ().friends == null)
			return null;
		FriendInfo[] info = getFriends ().friends;
		FriendInfo item;

		for (int i=0; i<info.Length; i++) {
			for (int j=i+1; j<info.Length; j++) {
				if (info [i].getLevel () < info [j].getLevel ()) {
					item = info [i];
					info [i] = info [j];
					info [j] = item;
				}
			}
		}
		return info;
	}

	/// <summary>
	/// 根据uid获取好友
	/// </summary>
	public FriendInfo getFriendByUid (string uid)
	{
		FriendInfo[] infos = getFriends ().friends;
		if (infos == null)
			return null;
		for (int i = 0; i < infos.Length; i++) {
			if (infos [i].getUid () == uid)
				return infos [i];
		}
		return null;
	}

	/// <summary>
	/// 获得待批准好友列表
	/// </summary>
	public FriendInfo[] getRequestFriendList ()
	{
		if (getFriends ().request == null)
			return null;
		FriendInfo[] info = getFriends ().request;
		
		for (int i=0; i<info.Length; i++) {
			for (int j=i+1; j<info.Length; j++) {
				if (info [i].getLevel () < info [j].getLevel ()) {
					FriendInfo item = info [i];
					info [i] = info [j];
					info [j] = item;
				}
			}
		}
		return info;
	}

	/// <summary>
	/// 是否是好友
	/// </summary>
	public bool isFriend (string _uid)
	{
		if (getFriends () == null)
			return false;
		if (getFriends ().friends == null)
			return false;
		for (int i=0; i<getFriends().friends.Length; i++) {
			if (_uid == getFriends ().friends [i].getUid ())
				return true;
		}
		return false;
	}

    public void updateFriend(ErlArray arr)
    {
        FriendInfo[] newInfo = createFriendInfoByErlArray(arr);
        for (int i = 0; i < friends.friends.Length; i++)
        {
            if (friends.friends[i].getUid() == newInfo[0].getUid())
            {
                FriendInfo old = friends.friends[i];
                friends.friends[i] = newInfo[0];

                if (old.getGiftReceiveStatus() != 1 && newInfo[0].getGiftReceiveStatus() == 1)
                {
                    if (UiManager.Instance.mainWindow != null)
                        UiManager.Instance.mainWindow.showFriendNum();
                }
                
                break;
            }
        }
        

    }

	/// <summary>
	/// 添加好友（用于主动）
	/// </summary>
	public void addFriend (FriendInfo _info)
	{
		List<FriendInfo> info = new List<FriendInfo> ();
		if (friends.friends != null)
			info = friends.friends.ToList ();
		info.Add (_info);
		friends.friends = info.ToArray ();
	}

	/// <summary>
	/// 添加好友（用于被动，服务器后台推送）
	/// </summary>
	public void addFriend (ErlArray ea, bool isApply) {
		FriendInfo[] newInfo = createFriendInfoByErlArray (ea);
		List<FriendInfo> oldInfo = new List<FriendInfo> ();
		if (friends.friends != null)
			oldInfo = friends.friends.ToList ();
		
		for (int i=0; i<newInfo.Length; i++) {
			oldInfo.Add (newInfo [i]);
			refuseFriendApply (newInfo [i].getUid ());
			if(isApply)
				UiManager.Instance.createMessageLintWindow (LanguageConfigManager.Instance.getLanguage ("FriendAddNewMsg", newInfo [i].getName ()));
			else
				UiManager.Instance.createMessageLintWindow (LanguageConfigManager.Instance.getLanguage ("FriendAddNewMsg2", newInfo [i].getName ()));
		}
		friends.friends = oldInfo.ToArray ();
		if (UiManager.Instance.getWindow<FriendFindRecommendWindow> () != null && FriendFindRecommendWindow.CACHE_LIST != null) {
			FriendInfo [] tmpFriends = new FriendInfo[FriendFindRecommendWindow.CACHE_LIST.Length -1];
			int step =0;
			for(int i=0;i<tmpFriends.Length;i++){
				if(FriendsManagerment.Instance.isFriend( FriendFindRecommendWindow.CACHE_LIST[i].getUid())){
					step++;
				}
				if(i+step <= FriendFindRecommendWindow.CACHE_LIST.Length){
					tmpFriends[i] = FriendFindRecommendWindow.CACHE_LIST[i+step];
				}
			}
			FriendFindRecommendWindow.CACHE_LIST = tmpFriends;
			UiManager.Instance.getWindow<FriendFindRecommendWindow> ().UpdateContent();
		};
	}

	/// <summary>
	/// 删除好友（用于主动）
	/// </summary>
	public void deleteFriend (string _uid)
	{
		List<FriendInfo> info = new List<FriendInfo> ();
		if (friends.friends != null) {
			info = friends.friends.ToList ();
			for (int i = 0; i < info.Count; i++) {
				if (info [i].getUid () == _uid)
					info.RemoveAt (i);
			}
			friends.friends = info.ToArray ();
		}
	}

	/// <summary>
	/// 删除好友（用于被动，服务器后台推送）
	/// </summary>
	public void deleteFriend (ErlArray ea)
	{
		FriendInfo[] delInfo = createFriendInfoByErlArray (ea);
		List<FriendInfo> oldInfo = new List<FriendInfo> ();
		if (friends.friends != null) {
			oldInfo = friends.friends.ToList ();

			for (int i = 0; i < delInfo.Length; i++) {
				for (int j = 0; j < oldInfo.Count; j++) {
					if (delInfo [i].getUid () == oldInfo [j].getUid ()) {
						oldInfo.RemoveAt (j);
						UiManager.Instance.createMessageLintWindow (LanguageConfigManager.Instance.getLanguage ("FriendDelNewMsg", delInfo [i].getName ()));
					}
				}
			}

			friends.friends = oldInfo.ToArray ();
		}
	}

	/// <summary>
	/// 从待批准列表删除某一个人，用于拒绝好友申请
	/// </summary>
	public void refuseFriendApply (string _uid)
	{
		List<FriendInfo> info = new List<FriendInfo> ();
		if (friends.request != null) {
			info = friends.request.ToList ();
			for (int i = 0; i < info.Count; i++) {
				if (info [i].getUid () == _uid)
					info.Remove (info [i]);
			}
			friends.request = info.ToArray ();
		}
	}

	/// <summary>
	/// 添加待批准好友申请（用于被动，服务器后台推送）
	/// </summary>
	public void addRefuseFriend (ErlArray ea)
	{
		FriendInfo[] newInfo = createFriendInfoByErlArray (ea);
		List<FriendInfo> oldInfo = new List<FriendInfo> ();
		if (friends.request != null)
			oldInfo = friends.request.ToList ();

		for (int i=0; i<newInfo.Length; i++) {
			if (oldInfo.Count < MAXNUM) {
				oldInfo.Add (newInfo [i]);
			} else {
				oldInfo.RemoveAt (0);
				oldInfo.Add (newInfo [i]);
			}
			if(UserManager.Instance.self.getUserLevel() > SHOWMSGLEVEL) //Jira YXZH-4331
			UiManager.Instance.createMessageLintWindow (LanguageConfigManager.Instance.getLanguage ("FriendNewMsg", newInfo [i].getName ()));
		}
		friends.request = oldInfo.ToArray ();
	}

    public void GiftReceive()
    {
        friends.giftReceiveCount++;
    }

	/// <summary>
	/// 创建好友信息
	/// </summary>
    public void createFriends(int _maxSize, FriendInfo[] _friends, FriendInfo[] _request, int _scores, int _share, int _praise, int _praised, int giftReceiveCount, int giftReceiveMax)
	{
		friends = new Friends (_maxSize, _friends, _request, _scores, _share, _praise, _praised, giftReceiveCount, giftReceiveMax);
	}

	//{"empty":21,"msg":[20,50,[[18003,1,莫伟志,60],[9002,1,崔英耀,100000]],[[9003,1,令狐烨烁,100000],[19001,1,侯安怡,0]],0,0,[],0,0]}
	//%%limit_size=限制大小,max_size=最大个数,friend=好友,request=待批准,friend_score=友情点,%%share=分享次数,share_list=自己的分享列表,praise=已赞次数,praised=被赞次数
	public void createFriendsByErlArray (ErlArray array)
	{
		int j = 0;
		int limit_size = StringKit.toInt (array.Value [j++].getValueString ());
		FriendInfo[] friend = _createFriendInfoByErlArray (array.Value [j++] as ErlArray);
		FriendInfo[] request = createFriendInfoByErlArray (array.Value [j++] as ErlArray);
		int friend_score = StringKit.toInt (array.Value [j++].getValueString ());
		int share = StringKit.toInt (array.Value [j++].getValueString ());
		ErlArray share_list = array.Value [j++] as ErlArray;
		int praise = StringKit.toInt (array.Value [j++].getValueString ());
		int praised = StringKit.toInt (array.Value [j++].getValueString ());
        int giftReceiveCount = StringKit.toInt(array.Value[j++].getValueString());
        int giftReceiveMax = StringKit.toInt(array.Value[j++].getValueString());
		
		createFriends (limit_size, friend, request, friend_score, share, praise, praised, giftReceiveCount, giftReceiveMax);
	}

	/// <summary>
	/// 创建好友或待批准列表
	/// </summary>
	public FriendInfo createFriendInfo (string _uid, string _headIco, string _name, int _exp, int _vipExp, string _guildName, int _combat, int _star, bool isOnline, int isReceiveGift, int isSendGift)
	{ 
		return new FriendInfo (_uid, _headIco, _name, _exp, _vipExp, _guildName, _combat, _star, isOnline, isReceiveGift, isSendGift);
	}
	public FriendInfo createFriendInfo (string _uid, string _headIco, string _name, int _exp, int _vipExp, string _guildName, int _combat, int _star, bool isOnline, int isReceiveGift, int isSendGift,string platUid)
	{ 
		return new FriendInfo (_uid, _headIco, _name, _exp, _vipExp, _guildName, _combat, _star, isOnline, isReceiveGift, isSendGift,platUid);
	}
	public FriendInfo[] createFriendInfoByErlArray (ErlArray ea)
	{
		if (ea.Value.Length < 1)
			return null;
		FriendInfo[] friends = new FriendInfo[ea.Value.Length];
		ErlArray friendInfo;
		for (int i = 0; i < friends.Length; i++) {
			friendInfo = ea.Value [i] as ErlArray;
			friends [i] = createFriendInfo (friendInfo.Value [0].getValueString (),
			                          		friendInfo.Value [1].getValueString (),
			                            	friendInfo.Value [2].getValueString (),
			                            	StringKit.toInt (friendInfo.Value [3].getValueString ()),
			                                StringKit.toInt (friendInfo.Value [4].getValueString ()),
			                                friendInfo.Value [5].getValueString (),
			                                StringKit.toInt (friendInfo.Value [6].getValueString ()),
                                            StringKit.toInt(friendInfo.Value[7].getValueString()), 
                                            StringKit.toInt(friendInfo.Value[8].getValueString()) == 1, 
                                            StringKit.toInt(friendInfo.Value[9].getValueString()),
                                            StringKit.toInt(friendInfo.Value[10].getValueString()));
		}
		return friends;
	}

	//新添平台Uid
	public FriendInfo[] _createFriendInfoByErlArray (ErlArray ea)
	{
		if (ea.Value.Length < 1)
			return null;
		FriendInfo[] friends = new FriendInfo[ea.Value.Length];
		ErlArray friendInfo;
		for (int i = 0; i < friends.Length; i++) {
			friendInfo = ea.Value [i] as ErlArray;
			friends [i] = createFriendInfo (friendInfo.Value [0].getValueString (),
			                                friendInfo.Value [1].getValueString (),
			                                friendInfo.Value [2].getValueString (),
			                                StringKit.toInt (friendInfo.Value [3].getValueString ()),
			                                StringKit.toInt (friendInfo.Value [4].getValueString ()),
			                                friendInfo.Value [5].getValueString (),
			                                StringKit.toInt (friendInfo.Value [6].getValueString ()),
			                                StringKit.toInt(friendInfo.Value[7].getValueString()), 
			                                StringKit.toInt(friendInfo.Value[8].getValueString()) == 1, 
			                                StringKit.toInt(friendInfo.Value[9].getValueString()),
			                                StringKit.toInt(friendInfo.Value[10].getValueString()),
			                                friendInfo.Value[11].getValueString());
		}
		return friends;
	}
	public FriendInfo[] sortArray (FriendInfo[] oldInfo)
	{
		FriendInfo[] newInfo = new FriendInfo[oldInfo.Length];

		for (int i = 0; i < oldInfo.Length - 1; i++) {
			for (int j = i; j < oldInfo.Length; j++) {
				if (oldInfo [i].getExp () < oldInfo [j].getExp ()) {
					oldInfo [i] = oldInfo [j];
					newInfo [j] = oldInfo [i];
				}
			}
		}
		return newInfo;
	}

}
