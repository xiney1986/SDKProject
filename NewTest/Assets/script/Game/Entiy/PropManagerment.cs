using System;
 
/**
 * 道具管理器
 * @author longlingquan
 * */
public class PropManagerment
{ 

	/** 道具sid */
	public const int PROP_PRIPLE_DEBRIS_SID = 71010, // 紫色碎片
						PROP_PRIPLE_ORANGE_SID = 71011; // 橙色碎片

	public PropManagerment ()
	{

	}

	public static PropManagerment Instance {
		get{return SingleManager.Instance.getObj("PropManagerment") as PropManagerment;}
	}
	public Prop createProp(){
		return new Prop ();
	}

	public Prop  createProp (int sid)
	{
		return new Prop (sid, 0);
	}

	public Prop createProp (int sid, int num)
	{
		return new Prop (sid, num);
	}
	
	public Prop createProp (ErlArray array)
	{
		int j = 0;
		int sid = StringKit.toInt (array.Value [j++].getValueString ());
		int num = StringKit.toInt (array.Value [j++].getValueString ());
		return new Prop(sid,num);
	}
	
	public void useProp (Prop prop, int num, CallBack<int> back)
	{
		if (prop == null)
			return;
		if (!StorageManagerment.Instance.checkProp (prop.sid, num))
			return;
		UsePropPort upp = FPortManager.Instance.getFPort ("UsePropPort") as UsePropPort;
		upp.access (prop.sid, num, back);
	}
} 

