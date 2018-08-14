using UnityEngine;
using System.Collections;

/**
 * 好友基础信息实体类
 * @authro 陈世惟  
 * */
public class Friends {
	//{"empty":21,"msg":[20,50,[[18003,1,莫伟志,60],[9002,1,崔英耀,100000]],[[9003,1,令狐烨烁,100000],[19001,1,侯安怡,0]],0,0,[],0,0]}
	//%%limit_size=限制大小,max_size=最大个数,friend=好友,request=待批准,friend_score=友情点,%%share=分享次数,share_list=自己的分享列表,praise=已赞次数,praised=被赞次数
	public Friends (int _maxSize,FriendInfo[] _friends,FriendInfo[] _request,int _scores,int _share,int _praise,int _praised, int giftReceiveCount, int giftReceiveMax)
	{
        this.giftReceiveCount = giftReceiveCount;
        this.giftReceiveMax = giftReceiveMax;
		this.share = _share;
		this.praise = _praise;
		this.praised = _praised;
		this.maxSize = _maxSize;
		this.scores = _scores;
		this.friends = _friends;
		this.request = _request;

	}

    public int giftReceiveCount = -1;
    public int giftReceiveMax = 0;
	public int share = 0;//分享次数
	public int praise = 0;//点赞次数
	public int praised = 0;//被赞次数
	public int amount = 0;//好友数量
	public int maxSize = 0;//好友上限
	public int scores = 0;//友情点数
	public FriendInfo[] friends;//好友列表
	public FriendInfo[] request;//待批准列表
	public ErlArray shareList;//我的分享列表

	/// <summary>
	/// 获取分享次数
	/// </summary>
	public int getShare()
	{
		return share;
	}

	/// <summary>
	/// 获取点赞次数
	/// </summary>
	public int getPraise()
	{
		return praise;
	}

	/// <summary>
	/// 获取被赞次数
	/// </summary>
	public int getPraised()
	{
		return praised;
	}

	/// <summary>
	/// 获取好友数量
	/// </summary>
	public int getAmount()
	{
		if(friends == null)
			return 0;
		else
			return friends.Length;
	}

	/// <summary>
	/// 获取待批准数量
	/// </summary>
	public int getRequestAmount()
	{
		if(request == null)
			return 0;
		else
			return request.Length;
	}

	/// <summary>
	/// 获取好友上限
	/// </summary>
	public int getMaxSize()
	{
		return maxSize;
	}

	/// <summary>
	/// 获取友情点数
	/// </summary>
	public int getScores()
	{
		return scores;
	}

	/** 更新好友数量上限 */
	public void updateMaxSize(int size)
	{
		maxSize = size;
	}

	/** 增加好友数量上限 */
	public void addMaxSize(int add)
	{
		maxSize += add;
	}
}


/**
 * 好友实体类
 * @authro 陈世惟
 * */
public class FriendInfo
{
	public FriendInfo(string _uid,string _headIco,string _name,long _exp,int _vipExp,string _guildName,int _combat, int _star, bool isOnline, int isReceiveGift, int isSendGift)
	{
		this.uid = _uid;
		this.headIco = _headIco;
		this.name = _name;
		this.exp = _exp;
		this.vipExp = _vipExp;
		this.guild=_guildName.Equals("none")?LanguageConfigManager.Instance.getLanguage("s0484"):_guildName;
		this.combatPower=_combat;
        this.star = _star;
        this.isOnline = isOnline;
        this.receiveGiftStatus = isReceiveGift;
        this.sendGiftStatus = isSendGift;
	}
	/*游戏内好友列表*/
	public FriendInfo(string _uid,string _headIco,string _name,long _exp,int _vipExp,string _guildName,int _combat, int _star, bool isOnline, int isReceiveGift, int isSendGift,string _platUid)
	{
		this.uid = _uid;
		this.headIco = _headIco;
		this.name = _name;
		this.exp = _exp;
		this.vipExp = _vipExp;
		this.guild=_guildName.Equals("none")?LanguageConfigManager.Instance.getLanguage("s0484"):_guildName;
		this.combatPower=_combat;
		this.star = _star;
		this.isOnline = isOnline;
		this.receiveGiftStatus = isReceiveGift;
		this.sendGiftStatus = isSendGift;
		this.platUid=_platUid;
	}
	private string uid = "";//唯一引索
	private string headIco = "";//头像
	private string name = "";//名称
	private long exp = 0;//经验
	private int vipExp = 0;//VIP经验
	private bool apply = false;//0未申请，1已申请
	private string guild="";
	private int combatPower=0;
	private PlatFormUserInfo sdkInfo;
    private int star;
    private bool isOnline;

	private string platUid;
    ////////////////////好友贈送
    /// <summary>
    // 可领取 1 ,无领取 2 ,已领取 3
    /// </summary>
    private int receiveGiftStatus;
    /// <summary>
    /// 已赠送 1 ,未赠送 2
    /// </summary>
    private int sendGiftStatus;
    

	/// <summary>
	/// 唯一引索
	/// </summary>
	public string getUid()
	{
		return uid;
	}
	public string getplatUid()
	{
		return platUid;
	}
	/// <summary>
	/// 平台信息
	/// </summary>
	public PlatFormUserInfo getSdkInfo()
	{
		return sdkInfo;
	}


	public  void setSdkInfo(PlatFormUserInfo info)
	{
		sdkInfo=info;
	}

	/// <summary>
	/// 头像
	/// </summary>
	public string getHeadIco()
	{
		return headIco;
	}

	/// <summary>
	/// 名称
	/// </summary>
	public string getName()
	{
		return name;
	}

	/// <summary>
	/// 经验
	/// </summary>
	public long getExp()
	{
		return exp;
	}
	/// <summary>
	/// 经验
	/// </summary>
	public int getVipExp()
	{
		return vipExp;
	}
	/// <summary>
	/// 获取等级
	/// </summary>
	public int getLevel()
	{
		return EXPSampleManager.Instance.getLevel (EXPSampleManager.SID_USER_EXP,exp);
	}

	/// <summary>
	/// 获取VIP等级
	/// </summary>
	public int getVipLevel()
	{
		return EXPSampleManager.Instance.getLevel (EXPSampleManager.SID_VIP_EXP,vipExp);
	}

	/// <summary>
	/// 申请状态
	/// </summary>
	public bool isApply()
	{
		return apply;
	}

	/// <summary>
	/// 临时设置申请状态
	/// </summary>
	public void setApply(bool bo)
	{
		apply = bo;
	}

	/// <summary>
	/// 公会名称
	/// </summary>
	/// <returns>The guild.</returns>
	public string getGuild()
	{
		return guild;
	}

	/// <summary>
	/// 战斗力
	/// </summary>
	/// <returns>The combat power.</returns>
	public int getCombatPower()
	{
		return combatPower;
	}


    public int getStar()
    {
        return star;
    }

    public bool getIsOnline()
    {
        return isOnline;
    }


    public int getGiftReceiveStatus()
    {
        return receiveGiftStatus;
    }

    public void setGiftReceiveStatus(int v)
    {
        receiveGiftStatus = v;
    }

    public int getGiftSendStatus()
    {
        return sendGiftStatus;
    }

    public void setGiftSendStatus(int v)
    {
        sendGiftStatus = v;
    }



}
public class FriendInviteInfo:FriendInfo
{
	public int inviteNum;
	public FriendInviteInfo(string _uid,string _headIco,string _name,long _exp,int _vipExp,string _guildName,int _combat, int _star, bool isOnline, int isReceiveGift, int isSendGift,int inviteNum):base( _uid, _headIco, _name, _exp, _vipExp, _guildName, _combat,  _star,  isOnline,  isReceiveGift,  isSendGift)
	{
		this.inviteNum=inviteNum;
	}
}
