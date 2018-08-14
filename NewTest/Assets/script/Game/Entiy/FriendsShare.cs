using UnityEngine;
using System.Collections;

/**
 * 分享实体总类
 * @authro 陈世惟  
 * */
public class FriendsShare {

	public FriendsShare (ShareInfo[] _shareInfo,ShareInfo[] _praiseInfo,int _praiseNum) {
		shareInfo = _shareInfo;
		praiseInfo = _praiseInfo;
		praiseNum = _praiseNum;
	}

	public ShareInfo[] shareInfo = null;
	public ShareInfo[] praiseInfo = null;
	public int praiseNum;//点赞次数
}


/**
 * 分享信息实体类
 * @authro 陈世惟  
 * */
public class ShareInfo {

	public ShareInfo (string _type,ErlType _sid) {
		this.type = _type;
		this.sid = _sid;
	}

	public ShareInfo (string _type,ErlType _sid,string _time,string _name,string _vip) {
		this.type = _type;
		this.sid = _sid;
		this.time = _time;
		this.name = _name;
		this.vip =_vip;
	}

	public string type;//1=卡片，2=装备，3=修炼副本，4=讨伐副本,5=剧情，6副本PVP，7圣器强化，8星盘，9进化，10升级，11获得女神，12主角突破
	public ErlType sid;//分享的内容
	public string time;//分享时间（好友分享信息专用）
	public string name;//名字（好友分享信息专用）
	public int isUse;//是否点赞或已分享
	public string vip;//vip等级

	public void setUse(int _isUse)
	{
		isUse = _isUse;
	}
}
