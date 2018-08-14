
using System;

/**
 * 简单用户，存储后台用户数据 后需求的数据继续加
 * 配合UserView显示用
 */
public class SimpleUser
{
	//[role_uid,style,name,vip,level,star]]
	public SimpleUser (ErlArray array)
	{
		uid = (array.Value [0] as ErlType).getValueString ();
		style = StringKit.toInt ((array.Value [1] as ErlType).getValueString ());
		name = (array.Value [2] as ErlType).getValueString ();
		vipLevel = StringKit.toInt ((array.Value [3] as ErlType).getValueString ());
		level = StringKit.toInt ((array.Value [4] as ErlType).getValueString ());
		star = StringKit.toInt ((array.Value [5] as ErlType).getValueString ());
	}

	private string uid = string.Empty;
	private int style;//头像
	private string name;//姓名
	private int vipLevel;//vip等级
	private int level;//玩家等级
	private int star;//星座

	public string getUid ()
	{
		return uid;
	}

	public int getStyle ()
	{
		return style;
	}

	public string getName ()
	{
		return name;
	}

	public int getVipLevel ()
	{
		return vipLevel;
	}

	public int getLevel ()
	{
		return level;
	}

	public int getStar ()
	{
		return star;
	}

}

