using UnityEngine;
using System.Collections;

/// <summary>
/// 全局整理属性表
/// </summary>
public class IncAttributeGlobal
{

	/* static fields */
	private static IncAttributeGlobal instance;

	/* static methods */
	public static IncAttributeGlobal Instance {
		get 
		{ 
			if (instance == null) 
			{
				instance=new IncAttributeGlobal();
				instance.attributes=new AttributeList();
			}
			return instance;
		}
	}

	/* fields */
	/** 属性表 */
	AttributeList attributes;

	/* properties */
	public void setAttribute(string key,int value)
	{
		if (attributes == null) return;
		attributes.set(key,value.ToString());
	}
	public void setAttribute(string key,string value)
	{
		if (attributes == null) return;
		attributes.set(key,value);
	}
	public string getAttribute(string key)
	{
		if(attributes==null) return null;
		return attributes.get(key);
	}
	public int getIntAttribute(string key)
	{
		if(attributes==null) return 0;
		if (attributes.get (key) == null) return 0;
		string value = attributes.get (key);
		int intValue;
		if(!int.TryParse(value,out intValue)) return 0;
		return intValue;
	}
	public string removeAttribute(string key)
	{
		if(attributes==null) return null;
		return attributes.remove(key);
	}

}
