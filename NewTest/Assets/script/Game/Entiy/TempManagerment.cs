using System;

/**
 * 零时道具管理器
 * @author zhoujie
 * */
public class TempManagerment
{
	public static string goods = "goods"; // 普通道具
	public static string equipment = "equipment"; // 装备
	public static string card = "card"; // 卡片
	public static string beast = "beast"; // 召唤兽
	
	public TempManagerment ()
	{

	}

	public static TempManagerment Instance {
		get{return SingleManager.Instance.getObj("TempManagerment") as TempManagerment;}
	}
	//创建临时道具 后台索引是从1开始
	public TempProp createTempProp ()
	{
		return new TempProp ();
	}
	
}

