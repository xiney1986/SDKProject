using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SdkFriendManager  {


	public static SdkFriendManager Instance {
		get{ return SingleManager.Instance.getObj ("SdkFriendManager") as SdkFriendManager;}
	}

	private SdkFriendsInfo[]  sdkFriendsInfos;

	private int selfgetnums;
	private int selfinvitenums;
	public  int MAXGETNUM = 20;
	public  int MAXINVITENUM = 3;
	
	private SdkFriendsInfo currentSdkFriendsInfo;
	private CallBack thisBack;

	public int showGetNum = 0;
	private int initflags = 0;
	private List<string> friendUid = new List<string>();

	private CallBack backCall;


	// 添加（+陌陌好友）记录 防止一次登陆游戏重复添加
	public void addFriendUidRecord(string uid)
	{
		if (!friendUid.Contains (uid)) {
			friendUid.Add (uid);
		}
	}

	
	// 判断已经添加的游戏好友中的uid
	public bool IsContentFriendUid(string uid)
	{
		if (friendUid == null)
			return false;
		return friendUid.Contains (uid);
	}
	
	
	public void getsdkFirendsInfos(CallBack back, int flags)
	{
		if (initflags != flags) 
		{
			getsdkFirendsInfos(back);
			initflags = flags;
		}
	}



	/// 从skd处获取friendsmsg
	public void getsdkFirendsInfos(CallBack back)
	{
	    backCall = back;
	}
	
	///ui界面获取sdk好友列表
	public SdkFriendsInfo[]  SdkFriendsInfos
	{
		get {
			return sdkFriendsInfos;
		}
	}
	
	
	public SdkFriendsInfo[] difServerFriendsInfos
	{
		get {
			if (sdkFriendsInfos == null )
				return null;
			List<SdkFriendsInfo> infos = new List<SdkFriendsInfo> ();
			foreach (SdkFriendsInfo info in sdkFriendsInfos) 
			{
				if (info.ServerFlag != 2)
					infos.Add(info);
			}
			return infos.ToArray ();
		}
	}
	
	public SdkFriendsInfo[] getPlayerInfos()
	{
		if (sdkFriendsInfos == null)
			return null;
		List<SdkFriendsInfo> infos = new List<SdkFriendsInfo> ();
		foreach (SdkFriendsInfo info in sdkFriendsInfos) 
		{
			if (info.ServerFlag != 1)
				infos.Add(info);
		}
		return infos.ToArray ();
	}
	
	public bool isSdkFriend(string uid)
	{

		bool iscontin = false;
		if (sdkFriendsInfos != null) 
		{
			foreach (SdkFriendsInfo info in sdkFriendsInfos)
			{
				if (info.SdkUid == uid)
					iscontin = true;
			}
		}
		
		return iscontin;
	}

	public SdkFriendsInfo getUidPlayerInfos(string uid)
	{
		foreach (SdkFriendsInfo info in sdkFriendsInfos) 
		{
			if (info.Uid == uid)
			{
				return info;
			}
		}
		return 	null;
	}

	public SdkFriendsInfo getSDKUidPlayerInfos(string uid)
	{
		foreach (SdkFriendsInfo info in sdkFriendsInfos) 
		{
			if (info.SdkUid == uid)
			{
				return info;
			}
		}
		return 	null;
	}

	public SdkFriendsInfo getSDKUidPlayerInfos(string uid,List<SdkFriendsInfo> lists)
	{
		foreach (SdkFriendsInfo info in lists) 
		{
			if (info.SdkUid == uid)
			{
				return info;
			}
		}
		return 	null;
	}
	

	///在SdkFriendsInfo 之前调用 获取服务器返回的sdk friends信息
	public void skdbackFriendsInfos(CallBack callback)
	{
		SdkFriendsInfo[] sinfos = getPlayerInfos ();
		SdkFriendFPort fport;
		string uidss = "";
		if (sinfos == null) 
		{
			fport = FPortManager.Instance.getFPort ("SdkFriendFPort") as SdkFriendFPort;
			fport.getsdkFriendsInfoMsg(uidss,callback);
			return;
		};
		string[] uids = new string[sinfos.Length];
		for (int i = 0; i < uids.Length; i++)
		{
			uids[i] += sinfos[i].SdkUid;
		}
		fport = FPortManager.Instance.getFPort ("SdkFriendFPort") as SdkFriendFPort;
		uidss = string.Join(",",uids);
	    fport.getsdkFriendsInfoMsg(uidss,callback);
	}
	
	///  获取服务器返回的sdk friends信息
	public void createsdkFriendsInfos(ErlArray msg)
	{
		sdkFriendsInfos = createsdkFriends(msg);
		if (backCall != null)
			backCall ();
	}
	
	public void addsdkFriend(string uid)
	{
		string reson = LanguageConfigManager.Instance.getLanguage ("sdk_add_friendmsg", UserManager.Instance.self.nickname);
	}
	
	/// 赠送按钮 点击触发 uiback更新
	public void sendPower(SdkFriendsInfo finfo,CallBack uiback)
	{
		SdkFriendFPort fport = FPortManager.Instance.getFPort ("SdkFriendFPort") as SdkFriendFPort;
		if (finfo == null ||(finfo!= null && string.IsNullOrEmpty(finfo.Uid)))
		{
			MaskWindow.UnlockUI();
			return;	
		}

		fport.sendsdkFriendsMsg(finfo.Uid,t => {
			if (t == "ok")
			{
				finfo.SendFlags = 1;
				if (uiback != null)
					uiback();
				ShowMsg (LanguageConfigManager.Instance.getLanguage ("sdk_send_suc"));
			} else {
				ShowMsg (LanguageConfigManager.Instance.getLanguage ("sdk_send_fail"));
			}
		});
	}
	
	/// 加sdk好友 先向服务发送数据取sdkid,现和sdk通信
	public void getsendSdkidFriend(string uid, CallBack callback)
	{
		SdkFriendFPort fport = FPortManager.Instance.getFPort ("SdkFriendFPort") as SdkFriendFPort;
		fport.sendAddsdkFriendsMsg (uid, t => {
			if (t.Length > 3)
			{
				addsdkFriend (t);
			}
			MaskWindow.UnlockUI();
			if (callback != null)	
			{
				callback();
			}
		});
	}
	
	/// 领取按钮点击触发 行动力成功时uiback更新
	public void sendgetPower (SdkFriendsInfo finfo,CallBack uiback)
	{
		
		if (GetNum < MAXGETNUM)
		{
			if ( UserManager.Instance.self.getPvEPointMax() < ( UserManager.Instance.self.getPvEPoint () + 2))
			{
				ShowMsg (LanguageConfigManager.Instance.getLanguage ("sdk_get_error1"));
				return;
			}
		} else {
			
			ShowMsg (LanguageConfigManager.Instance.getLanguage ("sdk_get_error2"));
			return;
		}
	
		SdkFriendFPort fport = FPortManager.Instance.getFPort ("SdkFriendFPort") as SdkFriendFPort;
		fport.getsdkFriendsMsg(finfo.Uid,t => {
			if (t == "ok")
			{
			    GetNum ++;
				showGetNum--;
				showGetNum = showGetNum < 0? 0:showGetNum;
				ShowMsg (LanguageConfigManager.Instance.getLanguage ("sdk_get_suc"));
				UserManager.Instance.self.addPvEPoint(2);
				if (uiback != null)
					uiback();
			} else {
				
				ShowMsg (LanguageConfigManager.Instance.getLanguage ("sdk_get_fail"));
			}
		});
	}

	/// 邀请好友加入该游戏
	public void sendInviteJoin (SdkFriendsInfo finfo,CallBack uiback)
	{
		if (InviteNum >= MAXINVITENUM) {
			ShowMsg(LanguageConfigManager.Instance.getLanguage("sdk_get_error3"));
			return;
		}
		
		SdkFriendFPort fport = FPortManager.Instance.getFPort ("SdkFriendFPort") as SdkFriendFPort;
		fport.sendInviteMsg(finfo.SdkUid, t => {
			sendIviteMsgProcess(t,finfo,uiback);
		});
		
		string invitereson = "Invite you!";
		if (ServerManagerment.Instance.lastServer  == null || UserManager.Instance.self == null) {
			invitereson = LanguageConfigManager.Instance.getLanguage ("sdk_invite_suc");
		} else {
			invitereson = LanguageConfigManager.Instance.getLanguage ("sdk_invite_suc", ServerManagerment.Instance.lastServer.name,
			                                                          StringKit.serverIdToFrontId (UserManager.Instance.self.uid));
		}
	}
    
	
	// 解析邀请信息返回
	public void sendIviteMsgProcess(string msg,SdkFriendsInfo finfo,CallBack uiback)
	{
	
		if (msg == "ok")
		{
			InviteNum++;
			finfo.IsInviate = false;
			if (uiback != null)
				uiback();
			addAward();
		} else {
			ShowMsg (LanguageConfigManager.Instance.getLanguage ("sdk_invite_fail"));
		}
	}
	
	
	private void addAward ()
	{
		AwardManagerment.Instance.addFunc (AwardManagerment.AWARDS_INVITE_SDKAWARD, sendInfoBack);
		
	}


	private void sendInfoBack (Award[] award)
	{
		string title = LanguageConfigManager.Instance.getLanguage ("sdk_inivte_suc");
		PrizeSample[] prizes =    AllAwardViewManagerment.Instance.addAwards (award);
		UiManager.Instance.openDialogWindow<AllAwardViewWindow>(win => {
			win.Initialize (prizes, null, null);
			win.topLabel.text = title;
		});
	}

	
	/// 解析skdfriendinfos的服務器返回消息
	private SdkFriendsInfo[] createsdkFriends(ErlArray msg)
	{
		bool isinviate,issend,isget;
		
		if (msg.Value.Length < 1)
			return sdkFriendsInfos;
		
		ErlArray  friendsInfo;
		SdkFriendsInfo sdkFriendInfo;
		MAXGETNUM = StringKit.toInt (msg.Value [0].getValueString ());
		MAXINVITENUM = StringKit.toInt (msg.Value [1].getValueString ());
		ErlArray sdksids = msg.Value [2] as ErlArray;
		string[] sdkuids = new string[sdksids.Value.Length];

		for (int i = 0; i < sdkuids.Length; i++) {
			sdkuids[i] = sdksids.Value[i].getValueString();
		}

		InviteNum = sdkuids.Length;
		GetNum = 0;
		showGetNum = 0;

		List<SdkFriendsInfo> infoslist = new List<SdkFriendsInfo>();
		foreach (SdkFriendsInfo info in sdkFriendsInfos) {
			infoslist.Add(info);
		}

		foreach (string id in sdkuids) {

			SdkFriendsInfo info = getSDKUidPlayerInfos(id,infoslist);
			if ( info != null)
			{
				info.IsInviate = false;
			}
		}

		ErlArray msgs = msg.Value [3] as ErlArray;




		for (int i = 0; i < msgs.Value.Length; i++)
		{

			friendsInfo = msgs.Value[i] as ErlArray;
			
			sdkFriendInfo = getSDKUidPlayerInfos( friendsInfo.Value[0].getValueString(),infoslist);

			if (sdkFriendInfo == null)
			{
				continue;
			}

			sdkFriendInfo.Uid = friendsInfo.Value[1].getValueString();
			if (sdkFriendInfo.Uid == "0" || string.IsNullOrEmpty(sdkFriendInfo.Uid))
			{
				infoslist.Remove(sdkFriendInfo);
				continue;
			}
			sdkFriendInfo.VipLevel = StringKit.toInt( friendsInfo.Value[2].getValueString());
			sdkFriendInfo.ChangeNum = StringKit.toInt( friendsInfo.Value[3].getValueString());
			sdkFriendInfo.ServerFlag = StringKit.toInt( friendsInfo.Value[4].getValueString()) == 2 ? 3:2;
		    sdkFriendInfo.SendFlags = StringKit.toInt( friendsInfo.Value[5].getValueString());
		    sdkFriendInfo.GetInfo = StringKit.toInt( friendsInfo.Value[6].getValueString());


			if (sdkFriendInfo.GetInfo == 3)
			{
				GetNum++;
			} else if (sdkFriendInfo.GetInfo == 1) {
				showGetNum++;
			}
		
				
		}

		if (UiManager.Instance.getWindow<LoginWindow>() == null)
			MaskWindow.UnlockUI ();
		
		sdkFriendsInfos = infoslist.ToArray ();
		return sdkFriendsInfos;
	}

	/// 获取自己领取体力次数的信息
	public int GetNum
	{
		get {

			return selfgetnums;
		}
		set {
			value = value < 0? 0 : value;
			value = value > MAXGETNUM ? MAXGETNUM:value;
			selfgetnums = value;
		}
	}
	
	
	

	/// 获取自己邀请次数的信息
	public int InviteNum
	{
		get {

			return selfinvitenums;
		}
		set {
			value = value < 0? 0 : value;
			value = value > MAXINVITENUM ? MAXINVITENUM:value;
			selfinvitenums = value;
		}
	}
	
	public InvitePrizeManager Prize;

	private void ShowMsg (string _str)
	{
		UiManager.Instance.createMessageLintWindow (_str);
	}

}

/// <summary>
/// 好友奖励面板管理器
/// </summary>
public class InvitePrizeManager
{
	public static InvitePrizeManager Instance {
		get{ return SingleManager.Instance.getObj ("InvitePrizeManager") as InvitePrizeManager;}
	}
	
	private InvitePrize[] invitePrizes;
	
	private int backMoney;//获取好友返利的钱
	private string bMsg;// 返利说明
	private bool isGetBM;//是否可以领取返利
	
	private List<string> getintList = new List<string>();
	private List<string> getedList ;
	private bool initFlags = false;
	private string viewMsg;
	public int showInviteNum  = 0;
	private int initflags = 0;
	private CallBack callback;
	
	
	/// 获取返利钻石数
	public int BackMoney
	{
		get {
			return backMoney;
		}
	}
	
	/// 获取返利说明
	public string BackMsg
	{
		get {
			if (bMsg == null)
				bMsg = LanguageConfigManager.Instance.getLanguage ("sdk_prize_backmsg");
			return bMsg;
		}
	}
	
	/// 获取返利是否可以领取
	public bool IsGetBM
	{
		get {
			return isGetBM;
		}
	}
	
	
	/// Gets the invite pirzes.
	public InvitePrize[] InvitePrizes
	{
		get {
			return invitePrizes;
		}
	}
	
	// 点击查看按钮
    public void clickViewButton ()
	{
		//  如果没有初始过，读取本地文件
		if (getedList == null && !initFlags)
		{
			getedList = new List<string>();
			string[] msgs = readStoreageTxt();
			if (msgs == null || (msgs != null && msgs.Length == 0))
			{
				MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("skd_prize_viewmsgError"));
				return;
			}
			processMsg(msgs,getedList);
			initFlags = true;
		}

		if (getedList == null || getintList == null)
		{
			MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("skd_prize_viewmsgError"));
			return;
		}
		viewMsg  = string.Join("\n",processMsg(getedList,getintList).ToArray());
		if (!string.IsNullOrEmpty(viewMsg))
		{
		   MessageWindow.ShowAlert (viewMsg);
		} else {
		   MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("skd_prize_viewmsgError"));
		}
	}
	
	public void getserverInvitePrizes(int flags)
	{
		if (initflags != flags) 
		{
			initflags = flags;
			getserverInvitePrizes();
		}
	}
	
	/// 从服务获取奖励的所有信息
	public void getserverInvitePrizes()
	{
		invitePrizes = getInvitePirzes ();

		string uids = "";
		foreach (InvitePrize p in invitePrizes)
		{
			uids += ","+p.prizeSid;
		}
		SdkFriendFPort fport = FPortManager.Instance.getFPort ("SdkFriendFPort") as SdkFriendFPort;
		fport.getserverPrizesMsg(uids, (t)=>{resolveServerMoneyMsg(t);});

		
	}

	public void getserverInvitePrizes(CallBack back)
	{
		if (back != null)
			callback = back;	
		getserverInvitePrizes ();
	}
	
	/// 向后台服务器发送返利领取消息
	public void sendbackGetinvitePrize(InvitePrize iprize,CallBack uiback)
	{
		SdkFriendFPort fport = FPortManager.Instance.getFPort ("SdkFriendFPort") as SdkFriendFPort;
		fport.sendgetbackPrizesMsg(iprize.friendsLevel.ToString(),(t) => { resolveBackPrizeMsg(t,uiback,iprize); });
	}

	/// 向后台服务器发送等级领取消息
	public void sendlevelGetinvitePrize(InvitePrize iprize,CallBack uiback)
	{
		if (iprize.currentGetNum <= 0) 
		{
			ShowMsg(LanguageConfigManager.Instance.getLanguage("sdk_prize_getError0"));
			return;
		}
		SdkFriendFPort fport = FPortManager.Instance.getFPort ("SdkFriendFPort") as SdkFriendFPort;
		fport.sendgetlevelPrizesMsg(iprize.friendsLevel.ToString(),(t) => { resovleLevelPrizeMsg(t,uiback,iprize); });
	}
	
	///获取邀请奖励面板的信息
	public InvitePrize[] getInvitePirzes()
	{
		return InvitePrizeConfigManager.Instance.getInvitePrizes();
	}
	
	
	/// 解析服务返回奖励
	public void resolveServerMoneyMsg(ErlArray msg)
	{

		invitePrizes[0].backDesc = BackMsg;
		ErlArray ary = msg.Value [0] as ErlArray;
		string lists = "";
		showInviteNum = 0;
		for (int i = 0; i < ary.Value.Length; i++) 
		{
			ErlArray ary_c = ary.Value[i] as ErlArray;
			lists = LanguageConfigManager.Instance.getLanguage("sdk_prize_back_moneymsg1",
			                                                   ary_c.Value[1].getValueString(),
			                                                   ary_c.Value[2].getValueString(),
			                                                   ary_c.Value[0].getValueString(),
			                                                   ary_c.Value[3].getValueString(),
			                                                   ary_c.Value[4].getValueString());
			                                                   
			                                                   
			processMsg(getintList,lists);
		}
		invitePrizes[0].backMoney = StringKit.toInt( msg.Value[1].getValueString());
		invitePrizes[0].isBackGet = invitePrizes[0].backMoney > 0 ? true:false;
		if (invitePrizes [0].isBackGet) {
			showInviteNum++;
		}
		SdkFriendFPort fport = FPortManager.Instance.getFPort ("SdkFriendFPort") as SdkFriendFPort;
		fport.getserverLevelPrizesMsg("uid", (t)=>{resolveServerLevelMsg(t);});
	}

	public void resolveServerLevelMsg(ErlArray msg)
	{
		if (msg.Value.Length < 1)
			return;
		ErlArray info;
		for (int i = 0; i < msg.Value.Length; i++)
		{
			info = msg.Value[i] as ErlArray;
			invitePrizes[i+1].friendsLevel = StringKit.toInt(info.Value[0].getValueString());
			invitePrizes[i+1].currentGetNum = (invitePrizes[i+1].GetMax - StringKit.toInt(info.Value[1].getValueString()));
			invitePrizes[i+1].getNums =  StringKit.toInt(info.Value[2].getValueString());
			invitePrizes[i+1].isGetLevelPrize = invitePrizes[i+1].getNums > 0? true:false;
			if (invitePrizes[i+1].isGetLevelPrize)
			{
				showInviteNum++;
			}
		}

		if (callback != null)
			callback ();
	}

	///点击领取返利后解析
	public void resolveBackPrizeMsg(ErlArray msg,CallBack callback,InvitePrize info)
	{
		string lists = "";
		string[] listss = new string[msg.Value.Length];
		showInviteNum--;
		showInviteNum = showInviteNum < 0 ? 0 : showInviteNum;
		for (int i = 0; i < msg.Value.Length; i++) 
		{
			ErlArray ary_c = msg.Value[i] as ErlArray;
			lists = LanguageConfigManager.Instance.getLanguage("sdk_prize_back_moneymsg2",
			                                                   ary_c.Value[1].getValueString(),
			                                                   ary_c.Value[2].getValueString(),
			                                                   ary_c.Value[0].getValueString(),
			                                                   ary_c.Value[3].getValueString(),
			                                                   ary_c.Value[4].getValueString());
			
			
			listss[i] = lists;
		}
		info.backMoney = 0;
		info.isBackGet = false;
		processMsg(listss,(msgs)=>{
			processMsg(msgs);
		});
		
		ShowMsg (LanguageConfigManager.Instance.getLanguage ("sdk_get_suc"));
		
		if (callback != null)
			callback();
	}
	
	/// 点击领取等级后解析
	public void resovleLevelPrizeMsg(string msg,CallBack callback,InvitePrize info)
	{
		if (msg == "ok") {
			info.isGetLevelPrize = false;
			info.currentGetNum--;
			info.currentGetNum = info.currentGetNum < 0 ? 0 : info.currentGetNum;
			info.getNums = 0;
			showInviteNum--;
			showInviteNum = showInviteNum < 0 ? 0 : showInviteNum;
			
			ShowMsg (LanguageConfigManager.Instance.getLanguage ("sdk_get_suc"));
			if (callback != null)
				callback ();
		} else {
			ShowMsg (LanguageConfigManager.Instance.getLanguage ("sdk_get_fail"));
		}
	}
	
	
	// 处理领取返利后的信息
	private void processMsg(string msg,List<string> storageList)
	{
		string[] msgs = msg.Split(',');
		foreach (string m in msgs)
		{
			storageList.Add(m);
		}
	}


	private void processMsg(List<string> storageList,string msg) 
	{
		storageList.Add (msg);
	}


	
	private void processMsg(string[] msgs,List<string> storageList)
	{
		foreach (string m in msgs)
		{
			storageList.Add(m);
		}
	}
	
	private List<string> processMsg(List<string> storageListA,List<string> storageListB)
	{
		List<string> storageall = new List<string>();
		
		foreach (string m in storageListA)
		{
			if (!string.IsNullOrEmpty(m))
			   storageall.Add(m);
		}
		foreach (string m in storageListB)
		{
	       if (!string.IsNullOrEmpty(m))
			   storageall.Add(m);
		}
		
		if (storageall.Count > 10)
		{
			int length = storageall.Count -10;
			while (length > 0)
			{
				storageall.RemoveAt(0);
				length--;
			}
		}
		
		return storageall;
	}
	
	
	
	private void processMsg (string[] msgs,CallBack<string[]> callback)
	{
		if (callback != null)
			callback(msgs);
	}
	
	// 处理服务返回的消息
	private void processMsg(string[] msgs)
	{
		processMsg(msgs,getedList);
	    getintList.Clear();
		storeageTxt(string.Join(",",msgs));
	}
	
	private void storeageTxt(string msg)
	{
		ReadFile.CreateFile(Application.persistentDataPath,UserManager.Instance.self.uid+".txt",msg);
	}
	
	
	private string[] readStoreageTxt()
	{
		
		return ReadFile.readMsg(Application.persistentDataPath,UserManager.Instance.self.uid+".txt");
	}
	
	
	private void showBackMsg(List<string> nogetMsg,List<string> yesgetMsg)
	{
		
	}
	
	
	private void ShowMsg (string _str)
	{
		UiManager.Instance.createMessageLintWindow (_str);
	}
	
}


public class SdkFriendsInfo
{
	private string sdkuid;
	private string uid;
	private string headIco ;
	private string name ;
	private string sexMan;
	private int momoLevel;

    

	private int serverFlags ;//1：不在游戏2：异服3：同服
	private int vipLevel;//vip等级
	private int combatPower ;//可变玩家消息
	private int isSend;//是否赠送
	private int isGet;//是否领取
	private bool isInvate;//是否邀请
	private bool isAdd;
	private PlatFormUserInfo playInfo;

	public bool IsAdd
	{
		get {
			return isAdd;
		}
		set {
			isAdd = value;
		}
	}

	public PlatFormUserInfo SdkInfo
	{
		get {
			return playInfo;
		}
	}
	
	public string SdkUid
	{
		get {
			return sdkuid;
		}
		set {
			sdkuid = value;
		}
	}
	
	public string Uid
	{
		get {
			if (string.IsNullOrEmpty(uid))
			{
				uid = "000000000";
			}
			return uid;
		}
		set {
			uid = value;
		}
	}

	public string HeadIcon
	{
		get {
			return headIco;
		}
		set {
			headIco = value;
		}
	}

	public string Name 
	{
		get {
			return name;	
		}
		set {
			name = value;
		}
	}

	public string Sex
	{
		get {
			return sexMan;
		}
		set {
			sexMan = value;
		}
	}

	public int MomoLevel
	{
		get {
			return 	momoLevel;
		}
		set {
			momoLevel = value;
		}
	}

	public int ServerFlag
	{
		get {
			return serverFlags;
		}
		set {
			serverFlags = value;
		}
	}

	// 1：已赠送 2：未赠送 0：补位
	public int SendFlags
	{
		get {
			return isSend;
		}
		set {
			isSend = value;
		}
	}

	//1:可领取2:没有领取内容3:已领取
	public int GetInfo {
		get {
			return isGet;
		}
		set {
			isGet = value;
		}
	}

	public int VipLevel 
	{
		get {
			return vipLevel;
		} set {
			vipLevel = value;
		}
	}

	public bool IsInviate
	{
		get {
			return isInvate;
		}
		set {
			isInvate = value;
		}
	}

	public int ChangeNum
	{
		get {
			return combatPower;
		}
		set {
			combatPower = value;
		}
	}
	              

    public SdkFriendsInfo ()
	{
		
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="SdkFriendsInfo"/> class.
	/// </summary>
	/// <param name="serverflags">Serverflags = 1 friend isn't game</param>
	public SdkFriendsInfo (string id, int serverflags,string headicon,string Name,string sexman,int momolevel,bool isinvate)
	{
		this.uid = id;
		this.serverFlags = serverflags;
		this.headIco = headicon;
		this.name = Name;
		this.sexMan = sexman;
		this.momoLevel = momolevel;
		this.isInvate = isinvate;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="SdkFriendsInfo"/> class.
	/// </summary>
	/// <param name="serverflags">Serverflags = 2 friends defferent server</param>
	public SdkFriendsInfo (string id, int serverflags,string headicon,string Name,string sexman,int momolevel,int fight,bool isv)
	{
		this.uid = id;
		this.serverFlags = serverflags;
		this.headIco = headicon;
		this.name = Name;
		this.sexMan = sexman;
		this.isInvate = isv;
		this.momoLevel = momolevel;
		this.combatPower = fight;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="SdkFriendsInfo"/> class.
	/// </summary>
	/// <param name="serverflags">Serverflags = 3 friends same server</param>
	public SdkFriendsInfo (string id, int serverflags,string headicon,string Name,string sexman,int momolevel,int viplevel,int fight,int issend,int isget)
	{
		this.uid = id;
		this.serverFlags = serverflags;
		this.headIco = headicon;
		this.name = Name;
		this.sexMan = sexman;
		this.momoLevel = momolevel;
		this.vipLevel = viplevel;
		this.combatPower = fight;
		this.isSend = issend;
		this.isGet = isget;
	}



}
