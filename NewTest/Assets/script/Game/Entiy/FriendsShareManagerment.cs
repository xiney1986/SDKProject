using System.Linq;
using System.Collections;
using System.Collections.Generic;

/**
 * 分享管理器
 * @authro 陈世惟  
 * */
public class FriendsShareManagerment
{

	/** 获得卡片 */
	public const string TYPE_CARD = "1";
	/** 获得装备 */
	public const string TYPE_EQUIP = "2";
	/** 修炼副本 */
	public const string TYPE_XIULIAN = "3";
	/** 讨伐副本 */
	public const string TYPE_TAOFA = "4";
	/** 剧情副本 */
	public const string TYPE_JUQING = "5";
	/** 副本PVP */
	public const string TYPE_PVP = "6";
	/** 圣器强化 */
	public const string TYPE_SHENGQI = "7";
	/** 星盘 */
	public const string TYPE_XINGPAN = "8";
	/** 卡片进化 */
	public const string TYPE_JINHUA = "9";
	/** 角色等级 */
	public const string TYPE_SHENGJI = "10";
	/** 获得女神 */
	public const string TYPE_NVSHEN = "11";
	/** 主卡突破 */
	public const string TYPE_TUPO = "12";

	public const string TYPE_YXZHIZHANG = "13";
	public const string TYPE_JWTISHENG = "14";
	public const string TYPE_XINGHUN = "15";
	public const string TYPE_ZUOQI = "16";
	public const string TYPE_QISHU = "17";
	public const string TYPE_VIP = "18";
	public const string TYPE_LADDER = "19";
	public const string TYPE_ARENA = "20";
	public const string TYPE_JINGCAI = "21";
	public const string TYPE_BEAST = "22";
    public const string TYPE_MAGICWEAPON = "23";

	public const int PRAISE_NUM = 50;//每日可点赞次数
	public const int MAXNUM = 999;//容器存储数量
	private FriendsShare friendShare = null;

	public static FriendsShareManagerment Instance {
		get{ return SingleManager.Instance.getObj ("FriendsShareManagerment") as FriendsShareManagerment;}
	}

	/// <summary>
	/// 返回所有分享信息
	/// </summary>
	public FriendsShare getFriendsShare ()
	{
		return friendShare;
	}

	/// <summary>
	/// 返回我的分享列表
	/// </summary>
	public ShareInfo[] getShareInfo ()
	{
		if (getFriendsShare () == null)
			return null;
		else if (getFriendsShare ().shareInfo == null)
			return null;
		else
			return getFriendsShare ().shareInfo;
	}

	/// <summary>
	/// 清空我的分享列表
	/// </summary>
	public void setShareInfoNull ()
	{
		getFriendsShare ().shareInfo = null;
	}

	/// <summary>
	/// 给已分享的打标签
	/// </summary>
	public void setShareInfoUse ()
	{
		for (int i = 0; i	< getShareInfo().Length; i++) {
			getShareInfo () [i].setUse (1);
		}
	}

	/// <summary>
	/// 给已点赞的打标签
	/// </summary>
	public void setPraiseInfoUse (int num)
	{
		if (num > getPraiseInfo ().Length)
			num = getPraiseInfo ().Length;
		for (int i = 0; i	< num; i++) {
			getPraiseInfo () [i].setUse (1);
		}
	}

	/// <summary>
	/// 返回我的好友分享列表
	/// </summary>
	public ShareInfo[] getPraiseInfo ()
	{
		if (getFriendsShare () == null)
			return null;
		else if (getFriendsShare ().praiseInfo == null)
			return null;
		else
			return getFriendsShare ().praiseInfo;
	}

	/// <summary>
	/// 返回可点赞次数
	/// </summary>
	public int getPraiseNum ()
	{
		int a = PRAISE_NUM - getFriendsShare ().praiseNum;
		if (a <= 0)
			return 0;
		else 
			return a;
	}

	/// <summary>
	/// 返回点赞次数是否已用完
	/// </summary>
	public bool getPraiseNumIsFull ()
	{
		return getFriendsShare ().praiseNum >= PRAISE_NUM;
	}

	/// <summary>
	/// 减去可点赞次数
	/// </summary>
	public void setPraiseNum (int num)
	{
		getFriendsShare ().praiseNum = getFriendsShare ().praiseNum + num;
	}

	/// <summary>
	/// 减去已点赞的朋友分享容器
	/// </summary>
	public void setPraise (int num)
	{
		if (getFriendsShare ().praiseInfo != null) {
			getFriendsShare ().praiseInfo = null;
		}
	}

	/// <summary>
	/// 创建分享基础信息
	/// </summary>
	public void createFriendsShare (ShareInfo[] _shareInfo, ShareInfo[] _praiseInfo, int _praiseNum)
	{
		friendShare = new FriendsShare (_shareInfo, _praiseInfo, _praiseNum);
	}

	public void createFriendsShareByErlArray (ErlArray array)
	{
		int j = 0;
		ShareInfo[] shareInfo = createShareInfoByErlArray (array.Value [j++] as ErlArray);
		ShareInfo[] praiseInfo = createPraiseInfoByErlArray (array.Value [j++] as ErlArray);
		int num = StringKit.toInt (array.Value [j++].getValueString ());
        ShareDrawManagerment.Instance.isFirstShare = StringKit.toInt(array.Value[j++].getValueString());//今天是否分享过0，没分享1，分享过
        ShareDrawManagerment.Instance.canDrawTimes = StringKit.toInt(array.Value[j++].getValueString());//分享抽奖次数
		createFriendsShare (shareInfo, praiseInfo, num);
	}

	/// <summary>
	/// 创建我的分享列表
	/// </summary>
	public ShareInfo createShareInfo (string _type, ErlType _sid)
	{ 
		return new ShareInfo (_type, _sid);
	}

	/// <summary>
	/// 创建朋友的分享列表
	/// </summary>
	public ShareInfo createPraiseInfo (string _type, ErlType _sid, string _time, string _name,string _vip)
	{ 
		return new ShareInfo (_type, _sid, _time, _name,_vip);
	}
	

	//创建我的分享列表
	public ShareInfo[] createShareInfoByErlArray (ErlArray ea)
	{
		if (ea.Value.Length < 1)
			return null;
		ShareInfo[] shareInfo = new ShareInfo[ea.Value.Length];
		ErlArray info;
		for (int i = 0; i < shareInfo.Length; i++) {
			info = ea.Value [i] as ErlArray;
			shareInfo [i] = createShareInfo (info.Value [0].getValueString (),
			                                 info.Value [1] as ErlType);
		}
		return shareInfo;
	}

	//添加我的分享信息
	public void addShareInfoByErlArray (ErlArray ea)
	{
		if (ea.Value.Length < 1)
			return;

		ShareInfo[] newInfo = createShareInfoByErlArray (ea);
		ShareInfo[] old = getShareInfo ();
		int l = (newInfo.Length + (old == null ? 0 : old.Length));
		int size = l > MAXNUM ? MAXNUM : l;
		ShareInfo[] newInfo1 = new ShareInfo[size];
		
		newInfo.CopyTo (newInfo1, 0);
		if (old != null) {
			if (l > MAXNUM) {
				int size1 = MAXNUM - newInfo.Length;
				System.Array.Copy (old, 0, newInfo1, newInfo.Length, size1);
			} else
				old.CopyTo (newInfo1, newInfo.Length);
		}
		
		getFriendsShare ().shareInfo = newInfo1;

//		ShareInfo[] newInfo = createShareInfoByErlArray(ea);
//		List<ShareInfo> oldInfo = new List<ShareInfo>();
//		if(getShareInfo()!=null)
//			oldInfo = getShareInfo().ToList();
//		
//		for(int i=0;i<newInfo.Length;i++)
//		{
//			if(oldInfo.Count < MAXNUM)
//				oldInfo.Add(newInfo[i]);
//			else
//			{
//				oldInfo.RemoveAt(0);
//				oldInfo.Add(newInfo[i]);
//			}
//		}
//		getFriendsShare().shareInfo = oldInfo.ToArray();
	}

	public ShareInfo[] createPraiseInfoByErlArray (ErlArray ea)
	{
		if (ea.Value.Length < 1)
			return null;
		ShareInfo[] praiseInfo = new ShareInfo[ea.Value.Length];
		ErlArray info;
		for (int i = 0; i < praiseInfo.Length; i++) {
			info = ea.Value [i] as ErlArray;
			praiseInfo [i] = createPraiseInfo (info.Value [0].getValueString (),
			                                   info.Value [1] as ErlType,
			                                   info.Value [2].getValueString (),
			                                   info.Value [3].getValueString (),
			                                   info.Value [4].getValueString());
		}
		return praiseInfo;
	}

	//添加朋友的分享信息 Copy (Array sourceArray, int sourceIndex, Array destinationArray, int destinationIndex, int length)
	public void addPraiseInfoByErlArray (ErlArray ea)
	{
		if (ea.Value.Length < 1)
			return;
		ShareInfo[] newInfo = createPraiseInfoByErlArray (ea);
		ShareInfo[] old = getPraiseInfo ();
		int l = (newInfo.Length + (old == null ? 0 : old.Length));
		int size = l > MAXNUM ? MAXNUM : l;
		ShareInfo[] newInfo1 = new ShareInfo[size];

		newInfo.CopyTo (newInfo1, 0);
		if (old != null) {
			if (l > MAXNUM) {
				int size1 = MAXNUM - newInfo.Length;
				System.Array.Copy (old, 0, newInfo1, newInfo.Length, size1);
			} else
				old.CopyTo (newInfo1, newInfo.Length);
		}
			
		getFriendsShare ().praiseInfo = newInfo1;

//		List<ShareInfo> oldInfo = new List<ShareInfo>();
//		if(getPraiseInfo()!=null)
//			oldInfo = getPraiseInfo().ToList();
//		for(int i=0;i<newInfo.Length;i++)
//		{
//			if(oldInfo.Count < MAXNUM)
//				oldInfo.Add(newInfo[i]);
//			else
//			{
//				oldInfo.RemoveAt(0);
//				oldInfo.Add(newInfo[i]);
//			}
//		}
//		getFriendsShare().praiseInfo = oldInfo.ToArray();
	}
}
