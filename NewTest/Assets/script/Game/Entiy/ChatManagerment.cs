using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

/**
 * 聊天管理器
 * @authro 陈世惟  
 * */
public class ChatManagerment
{

    public const string CHAT_CACHE_DIR = "ChatFriendCache_{0}.txt";
	public const string GUILD_CACHE_DIR = "ChatGuildCache_{0}.txt";

	public CallBack<string, int> updateMsg;
	public const string SHOW_CARD = "card", SHOW_Equip = "equip";//展示类型。0=无内容。1=普通聊天，2=展示装备，3=展示卡片。
	public const int MSGTYPE_SAY = 1, MSGTYPE_EQUIP = 2, MSGTYPE_CARD = 3;//判断聊天类型。0=无内容。1=普通聊天，2=展示装备，3=展示卡片。
    public const int CHANNEL_WORLD = 1, CHANNEL_GUILD = 2, CHANNEL_FRIEND = 3, CHANNEL_SYSTEM = 4, CHANNEL_RADIO = 5;
	public int sendType = 1;//记录上一次发送频道
    public int tipsNewFriendMsg = 0; //提示新的好友消息,0不需提示, >1需要提示, -1当前界面正在打开的
    public FriendInfo CurrentFriendInfo;
	private List<Chat> chats = new List<Chat> ();
	private List<string>guildChats = new List<string>();
    private List<string> mCacheFriendChats = new List<string>();
	private List<ErlArray> mCacheFriendErlArr = new List<ErlArray> ();

	private List<string> mCacheGuildChats = new List<string>();
	private List<ErlArray> mCacheGuildErlArr = new List<ErlArray>();

	private bool isOpenChat = false;//是否打开聊天室
	private int num = 0;//关闭聊天室后收到的消息数目
	private int ALLMAXNUM = 100;//所有频道的聊天容器
	private int MAXNUM = 50;//单个频道聊天容器
	public static int GuildNum;
	string left = LanguageConfigManager.Instance.getLanguage("chat_l");
	string right = LanguageConfigManager.Instance.getLanguage("chat_r");

	private Queue<string> filterWorldRecord = new Queue<string>();
	private Queue<string> filterFriendRecord = new Queue<string>();

	private string worldMsg = null;
	private string friendMsg = null;

	//聊天展示-做好清理工作
	public ServerCardMsg chatCard;

	public static ChatManagerment Instance {
		get{ return SingleManager.Instance.getObj ("ChatManagerment") as ChatManagerment;}
	}


    private void SaveFriendChatToLocal()
    {
        string cacheStr = "";
		for (int i = 0; i < mCacheFriendChats.Count && i < mCacheFriendErlArr.Count; i++)
        {
            if (mCacheFriendChats[i].Length <= 0) continue;
            cacheStr += mCacheFriendChats[i] + "\t" + ErlArray.erlArrToStr(mCacheFriendErlArr[i]);
            if (i != mCacheFriendChats.Count - 1)
                cacheStr += "\n";
        }

        string path = PathKit.RESROOT + string.Format(CHAT_CACHE_DIR, UserManager.Instance.self.uid);
        FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
        StreamWriter sw = new StreamWriter(fs);
        sw.Flush();
        sw.BaseStream.Seek(0, SeekOrigin.Begin);
        sw.Write(cacheStr);
        sw.Close(); 
    }

    private void LoadFriendChatFromLocal()
    {
        try
        {
            string path = PathKit.RESROOT + string.Format(CHAT_CACHE_DIR, UserManager.Instance.self.uid);
            if (!File.Exists(path)) return;
            FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Read);
            byte[] bytes = new byte[fs.Length];
            fs.Read(bytes, 0, (int)fs.Length);
            fs.Close();
			mCacheFriendChats = new List<string> ();
			mCacheFriendErlArr = new List<ErlArray> ();
			string[] arr = Encoding.UTF8.GetString (bytes).Split ('\n');
			for (int i = 0; i < arr.Length; i++) {
				string[] item = arr[i].Split ('\t');
				mCacheFriendChats.Add (item[0]);
				mCacheFriendErlArr.Add (ErlArray.strToErlArr(item[1]));
			}   
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }
    }



	private void SaveGuildChatToLocal()
	{
		string cacheStr = "";
		for (int i = 0; i < mCacheGuildChats.Count && i < mCacheGuildErlArr.Count; i++)
		{
			if (mCacheGuildChats[i].Length <= 0) continue;
			cacheStr += mCacheGuildChats[i] + "\t" + ErlArray.erlArrToStr(mCacheGuildErlArr[i]);
			if (i != mCacheGuildChats.Count - 1)
				cacheStr += "\n";
		}
		
		string path = PathKit.RESROOT + string.Format(GUILD_CACHE_DIR, UserManager.Instance.self.uid);
		FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
		StreamWriter sw = new StreamWriter(fs);
		sw.Flush();
		sw.BaseStream.Seek(0, SeekOrigin.Begin);
		sw.Write(cacheStr);
		sw.Close(); 
	}


	private void LoadGuildChatFromLocal()
	{
		try
		{
			string path = PathKit.RESROOT + string.Format(GUILD_CACHE_DIR, UserManager.Instance.self.uid);
			if (!File.Exists(path)) return;
			FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Read);
			byte[] bytes = new byte[fs.Length];
			fs.Read(bytes, 0, (int)fs.Length);
			fs.Close();
			mCacheGuildChats = new List<string> ();
			mCacheGuildErlArr = new List<ErlArray> ();
			string[] arr = Encoding.UTF8.GetString (bytes).Split ('\n');
			for (int i = 0; i < arr.Length; i++) {
				string[] item = arr[i].Split ('\t');
				mCacheGuildChats.Add (item[0]);
				mCacheGuildErlArr.Add (ErlArray.strToErlArr(item[1]));
			}   
		}
		catch (System.Exception e)
		{
			Debug.Log(e.Message);
		}
	}




	public void setUpdate (CallBack<string, int> aaa)
	{
		updateMsg = aaa;
	}
	
	/// <summary>
	/// 创建聊天内容
	/// </summary>
	/// <param name='uid'>
	/// Uid玩家Uid.
	/// </param>
	/// <param name='name'>
	/// Name玩家名称.
	/// </param>
	/// <param name='vip'>
	/// vip玩家VIP经验.
	/// </param> 
	/// <param name='type'>
	/// Type聊天类型，世界或公会.
	/// </param>
	/// <param name='sender'>
	/// Sender发送者，玩家或者GM.
	/// </param>
	/// <param name='stime'>
	/// Stime时间.
	/// </param>
	/// <param name='isShow'>
	/// isShow内容类型1=普通聊天，2=展示装备，3=展示卡片.
	/// </param>
	/// <param name='content'>
	/// Content内容.
	/// </param>
	/// <param name='goods'>
	/// Equip装备
	/// </param>
	/// /// <param name='card'>
	/// Card卡片
	/// </param>
	/// 
	/// 	private List<string> mCacheGuildChats = new List<string>();
	///private List<ErlArray> mCacheGuildErlArr = new List<ErlArray>();
/// 
	public void createChat (string uid, string name, int vip, int type, int sender, int stime, int isShow, int job, string content, ErlArray goods, string friendTargetUid, string friendTargetName, int friendReceiveVip)
	{
        Chat chat = new Chat(uid, name, vip, type, sender, stime, isShow, job, content, goods, friendTargetUid, friendTargetName, friendReceiveVip);
		
        string msg = "";
//        if (chat.channelType == CHANNEL_FRIEND)
//        {
//            if (tipsNewFriendMsg != -1)
//                tipsNewFriendMsg++;
//            if (mCacheFriendChats.Count >= 20)
//                mCacheFriendChats.RemoveAt(0);
//			mCacheFriendErlArr.Add (goods);
//            msg = ChatManagerment.Instance.initString(chat);
//            mCacheFriendChats.Add(msg);
//            SaveFriendChatToLocal();
//        }
//		else if(chat.channelType == CHANNEL_GUILD){
//			if (mCacheGuildChats.Count >= 20)
//				mCacheGuildChats.RemoveAt(0);
//			mCacheGuildErlArr.Add (goods);
//			msg = ChatManagerment.Instance.initString(chat);
//			mCacheGuildChats.Add(msg);
//			SaveGuildChatToLocal();
//		}
//        else
//        {
            if (chats.Count >= ALLMAXNUM)
                chats.RemoveAt(0);
           	chats.Add(chat);
			if(UserManager.Instance.self == null)
			return;
            msg = ChatManagerment.Instance.initString(chat);
//        }


        if (UiManager.Instance.mainWindow != null)
            UiManager.Instance.mainWindow.UpdateChatMsgTips();
        if (UiManager.Instance.missionMainWindow != null)
            UiManager.Instance.missionMainWindow.UpdateChatMsgTips();
		if (UiManager.Instance.guildFightMainWindow != null)
			UiManager.Instance.guildFightMainWindow.UpdateChatMsgTips ();
		if (updateMsg != null) {
			if(chat.uid == UserManager.Instance.self.uid){
                AddFilterList(msg, type);
			}
			updateMsg (msg, chat.channelType);
		}

	}
	
	/// <summary>
	/// 得到所有聊天记录
	/// </summary>
	public List<Chat> getAllChat ()
	{
		return chats; 
	}

	public List<string> getAllCacheGuildChats(){

		if (mCacheGuildChats.Count <= 0)
			LoadGuildChatFromLocal();
		return mCacheGuildChats;
	}
    public List<string> getAllCacheFriendChat()
    {
        if (mCacheFriendChats.Count <= 0)
            LoadFriendChatFromLocal();
        return mCacheFriendChats;
    }

	/// <summary> 
	/// 得到指定的聊天信息
	/// </summary>
	public ErlArray getChatByUid (string _index, string channelType)
	{
		if (StringKit.toInt (channelType) == CHANNEL_FRIEND) {
			int indexItem = StringKit.toInt (_index);
//			if (mCacheFriendErlArr == null || mCacheFriendErlArr.Count == 0 || mCacheFriendErlArr.Count <= indexItem) 
//				return null;
			if (indexItem != -1&& chats[indexItem] != null){ //&& mCacheFriendErlArr[indexItem] != null) {
				return chats[indexItem].goods;//mCacheFriendErlArr[indexItem];
			}
		}
		else {
			if (chats == null || chats.Count == 0) {
				return null;
			}
			int indexItem = StringKit.toInt (_index);
			if (indexItem != -1 && chats[indexItem] != null) {
				return chats[indexItem].goods;
			}
		}
		return null; 
	}
    /// <summary>
    /// 获取玩家UID
    /// </summary>
    /// <param name="_index"></param>
    /// <param name="channelType"></param>
    /// <returns></returns>
    public string getChatByUidName(string _index, string channelType)
    {
        if (StringKit.toInt(channelType) == CHANNEL_FRIEND)
        {
            int indexItem = StringKit.toInt(_index);
            //			if (mCacheFriendErlArr == null || mCacheFriendErlArr.Count == 0 || mCacheFriendErlArr.Count <= indexItem) 
            //				return null;
            if (indexItem != -1 && chats[indexItem] != null)
            { //&& mCacheFriendErlArr[indexItem] != null) {
                return chats[indexItem].uid;//mCacheFriendErlArr[indexItem];
            }
        }
        else
        {
            if (chats == null || chats.Count == 0)
            {
                return null;
            }
            int indexItem = StringKit.toInt(_index);
            if (indexItem != -1 && chats[indexItem] != null)
            {
                return chats[indexItem].uid;
            }
        }
        return null;
    }

	public int getIndexByChat (Chat _chat)
	{
		if (_chat.channelType != CHANNEL_FRIEND && (chats == null || chats.Count == 0)) {
			return -1;
		}
		return chats.IndexOf (_chat);//_chat.channelType == CHANNEL_FRIEND ? mCacheFriendErlArr.IndexOf(_chat.goods) : chats.IndexOf (_chat);
	}
	
	/// <summary>
	/// 得到世界聊天记录
	/// </summary>
	public List<Chat> getAllChatByWorld ()
	{
		List<Chat> newchats = new List<Chat> ();
		for (int i=0; i<chats.Count; i++) {
			if (chats [i].channelType != CHANNEL_GUILD) {
				if (newchats.Count < MAXNUM)
					newchats.Add (chats [i]);
				else {
					newchats.RemoveAt (0);
					newchats.Add (chats [i]);
				}
			}
		}
		return newchats;
	}
	
	/// <summary>
	/// 得到公会聊天记录
	/// </summary>
	public List<Chat> getAllChatByGuild ()
	{
		List<Chat> newchats = new List<Chat> ();
		for (int i=0; i<chats.Count; i++) {
			if (chats [i].channelType == CHANNEL_GUILD) {
				if (newchats.Count < MAXNUM)
					newchats.Add (chats [i]);
				else {
					newchats.RemoveAt (0);
					newchats.Add (chats [i]);
				}
			}
		}
		return newchats;
	}

    /// <summary>
    /// 得到好友聊天记录
    /// </summary>
    public List<Chat> getAllChatByFriend()
    {
        List<Chat> newchats = new List<Chat>();
        for (int i = 0; i < chats.Count; i++)
        {
            if (chats[i].channelType == CHANNEL_FRIEND)
            {
                if (newchats.Count < MAXNUM)
                    newchats.Add(chats[i]);
                else
                {
                    newchats.RemoveAt(0);
                    newchats.Add(chats[i]);
                }
            }
        }
        return newchats;
    }

	/// <summary>
	/// 清除聊天记录
	/// </summary>
	public void clearChat ()
	{
		chats.Clear ();
	}
	
	/// <summary>
	/// 判断是否为系统消息，true=是，false=否
	/// </summary>
	public bool isSystemMsg (Chat chat)
	{
		if (chat.sender == 2)
			return true;
		else if (chat.sender == 1)
			return false;
		else 
			return false;
	}
	
	/// <summary>
	/// 判断聊天类型。0=无内容。1=普通聊天，2=展示装备，3=展示卡片。
	/// </summary>
	public int getMsgType (Chat chat)
	{
		if (chat.isShow == MSGTYPE_SAY)
			return 1;
		else if (chat.isShow == MSGTYPE_EQUIP)
			return 2;
		else if (chat.isShow == MSGTYPE_CARD)
			return 3;
		else
			return 0;
	}
	
	//聊天按钮图标变化
	public int IsHaveNewHaveMsg ()
	{
        if (tipsNewFriendMsg > 0) return 2;
        if (num > 0) return 1;
		return 0;
	}
	
	public int changeChatIco ()
	{
		if (chatStatus) {
			num = 0;
			return 0;
		} else {
			num++;
            if (UiManager.Instance.mainWindow != null)
                UiManager.Instance.mainWindow.UpdateChatMsgTips();
			else if(UiManager.Instance.guildFightMainWindow != null)
				UiManager.Instance.guildFightMainWindow.UpdateChatMsgTips();
			return num;
		}


	}
	
	//设置聊天窗口是否打开
	public bool chatStatus {
		get {
			return isOpenChat;
		}
		set {
			isOpenChat = value;
		}
	}

	/// <summary>
	/// 初始化聊天信息展示
	/// </summary>
	public string initString (Chat _chat)
	{
		string uid = _chat.uid;
		string time = getChatTime (_chat);
		string type = getChatType (_chat);
		string sname = "[url=name|" + _chat.channelType + "|" + _chat.uid + "]"+Colors.CHAT_USER+ _chat.name + "[-][/url]";
		string job = _chat.channelType != CHANNEL_GUILD ? "" : left + getJob (_chat.job) + right;
		string vip = "";
		string content = _chat.content;
        ///vip判断
		if (EXPSampleManager.Instance.getLevel (3, _chat.vip) != 0)
			vip = Colors.CHAT_VIP + "<" + "Vip" + EXPSampleManager.Instance.getLevel (3, _chat.vip) + ">" + "[-]";
        ///判断是否好友聊天
        if (_chat.channelType == CHANNEL_FRIEND)
        {
            if (_chat.friendReceiveUid == UserManager.Instance.self.uid)
            {
                vip += LanguageConfigManager.Instance.getLanguage("chat12");
            }
            else
            {
				sname = LanguageConfigManager.Instance.getLanguage ("chat11") + "[url=name|" + _chat.channelType + "|" + _chat.friendReceiveUid + "]" +Colors.CHAT_USER+ _chat.friendReceiveName + "[-][/url]";
                if (EXPSampleManager.Instance.getLevel(3, _chat.friendReceiveVip) != 0)
                    vip = Colors.CHAT_VIP + "<" + "Vip" + EXPSampleManager.Instance.getLevel(3, _chat.friendReceiveVip) + ">" + "[-]";
                vip += LanguageConfigManager.Instance.getLanguage("chat13");
            }
        }
		///
		if (getMsgType (_chat) == MSGTYPE_SAY) {
			if (!isSystemMsg (_chat))
				return type + sname + job + vip + ":" + content + time;
			else {
				return type + content + time;
			}
		} else {	
			if (getMsgType (_chat) == MSGTYPE_EQUIP) {
				string str = changeEquipMsgToUrl (_chat);
				return type + sname + job + vip + ":" + LanguageConfigManager.Instance.getLanguage ("s0305") + str + time;
			} else if (getMsgType (_chat) == MSGTYPE_CARD) {
				string str = changeCardMsgToUrl (_chat);
				return type + sname + job + vip + ":" + LanguageConfigManager.Instance.getLanguage ("s0304") + str + time;
			}
		}

		return "bug";

	}

	public string getChatTime (Chat _chat)
	{
		DateTime dt = TimeKit.getDateTime (_chat.stime);
		string hourInfo = dt.Hour < 10 ? "0" + dt.Hour : dt.Hour.ToString ();
		string minInfo = dt.Minute < 10 ? "0" + dt.Minute : dt.Minute.ToString ();
		switch (_chat.channelType) {
		case CHANNEL_WORLD:
			return   left + hourInfo + ":" + minInfo + right;
		case CHANNEL_GUILD:
			return  left + hourInfo + ":" + minInfo + right;
        case CHANNEL_FRIEND :
			return left + hourInfo + ":" + minInfo + right;
		case CHANNEL_SYSTEM:
			return Colors.CHAT_SYSTEM + left + hourInfo + ":" + minInfo + right;
		case CHANNEL_RADIO:
			return left + hourInfo + ":" + minInfo + right;
		default:
			return "";
		}
	}

	public string getChatType (Chat _chat)
	{
		switch (_chat.channelType) {
		case CHANNEL_WORLD:
			return Colors.CHAT_WORLD + left + LanguageConfigManager.Instance.getLanguage ("s0302") +  right + "[-]" + Colors.CHAT_CONTENT;
		case CHANNEL_GUILD:
			return Colors.CHAT_UNION + left + LanguageConfigManager.Instance.getLanguage ("s0303") + right+ "[-]" + Colors.CHAT_CONTENT;
		case CHANNEL_FRIEND :
			return Colors.CHAT_FRIEND + left + LanguageConfigManager.Instance.getLanguage("s0491") + right+"[-]" + Colors.CHAT_CONTENT;
		case CHANNEL_SYSTEM:
			return Colors.CHAT_SYSTEM + left + LanguageConfigManager.Instance.getLanguage ("s0306") + right;
		case CHANNEL_RADIO:
			return Colors.CHAT_RADIO + left + LanguageConfigManager.Instance.getLanguage ("chat08") + right +"[-]" + Colors.CHAT_CONTENT;
		default:
			return "";
		}
	}

	//职位
	public string getJob (int _type)
	{
		return GuildJobType.getJobName (_type);
	}

	//转成超链接
	public string changeEquipMsgToUrl (Chat _chat)
	{
		Equip equip = EquipManagerment.Instance.createEquip (_chat.goods);
		return "[url=equip|" + _chat.channelType + "|" + getIndexByChat (_chat) + "]" + QualityManagerment.getQualityColor (equip.getQualityId ()) + left + equip.getName () + right + "[-][/url]";
		//[url=http://www.tasharen.com/forum/index.php?topic=7013.0][u]this link[/u][/url]
	}

	//转成超链接
	public string changeCardMsgToUrl (Chat _chat)
	{
//		Debug.LogError ("change index==" + getIndexByChat(_chat));
		ServerCardMsg card = CardManagerment.Instance.createCardByChatServer (_chat.goods);
		string name;
		if (card.card.isMainCard ()) {
			name = card.card.getName () + (card.card.getSurLevel () > 0 ? ("+" + card.card.getSurLevel ()) : "");
		} else {
			name = card.card.getName () + (card.card.getEvoLevel () > 0 ? ("+" + card.card.getEvoLevel ()) : "");
		}
		return "[url=card|" + _chat.channelType + "|" + getIndexByChat (_chat) + "][u]" + QualityManagerment.getQualityColor (card.card.getQualityId ()) + left + name + right + "[-][/u][/url]";
	}

	public bool Filter(string msg,int sendType){
		if(sendType == ChatManagerment.CHANNEL_FRIEND){
			return Filter(msg,filterFriendRecord,ref friendMsg,sendType);
		}else if(sendType == ChatManagerment.CHANNEL_WORLD)
			return Filter (msg,filterWorldRecord,ref worldMsg,sendType);
        return false;
	}
    //验证输入的信息是否重复
	private bool Filter(string msg,Queue<string> que, ref string cache,int sendType){
        if (currentMsg == null) {
            currentMsg = msg;
			return false;
		}
		//刷新过滤队列
		updateFilter(que);
		//如果过滤队列中含有信息，则返回true;
		foreach(string tmp in que){
			string[] record = tmp.Split('&');
			if(record[0] == msg){
				updateMsg(record[2],sendType);
				return true;
			}
		}
        cache = msg;
		return false;
	}

    string currentMsg;


    public void AddFilterList(string selfLastMsg, int sendType) {
        if (sendType == ChatManagerment.CHANNEL_FRIEND) {
             AddFilterList(selfLastMsg, filterFriendRecord, ref friendMsg, sendType);
        }else if(sendType == ChatManagerment.CHANNEL_WORLD)
         AddFilterList(selfLastMsg, filterWorldRecord, ref worldMsg, sendType);
    }

    //AddFilterList与Filter在一个发送逻辑中是配合使用的AddfilterList负责在数据返回的时候将接受到的数据放入队列

    private void AddFilterList(string selfLastMsg, Queue<string> que,ref string cache, int sendType ) {
        //如果msg和上次说的话一样则添加到过滤队列
        if (cache == null)
            return;
        if (cache == currentMsg) {
            currentMsg += "&" + TimeKit.getMillisTime() + "&" + selfLastMsg;
            if (que.Count == 10) { //如果过滤队列达到10条，则移除第一条
                que.Dequeue();
            }
            que.Enqueue(currentMsg);
        } else {
            currentMsg = cache;
        }
    }

	private void updateFilter(Queue<string> filter){

		for(int i=0;i<filter.Count;i++){
			string[] tmp = filter.Peek().Split('&');
			long filterTime = Convert.ToInt64(tmp[1]);
			if(TimeKit.timeSecond(TimeKit.getMillisTime()) - TimeKit.timeSecond(filterTime) >300){ //超过5分钟，移除
				filter.Dequeue();
			}
		}
	}

}
