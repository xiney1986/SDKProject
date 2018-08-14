using System;
 
public class ErlObjectString:ErlType
{
	public ErlObjectString (string _value)
	{
		 
	}
	
	public const int TAG = 0x6e;
		
	 
		
	/* methods */
	/** 是否是数据标记 */
	override public bool isTag (int tag)
	{
		return TAG == tag;
	}
		
	/** 将字符串数据结构写入json对象 */
	override public void writeToJson (object key, object jsonObj)
	{
	 
	}
} 

