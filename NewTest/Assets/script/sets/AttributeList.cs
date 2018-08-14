using UnityEngine;
using System;

/// <summary>
/// 文字属性表
/// </summary>
public class AttributeList
{

	/* static fields */
	/** 空数组 */
	public static readonly string[] NULL = new string[0];
	
	/* fields */
	/** 映射表 */
	string[] array;

	/** 构造一个空文字属性表 */
	public AttributeList()
	{
		array=NULL;
	}
//	/** 用指定的字符数组构造一个文字属性表 */
//	public AttributeList(string[] strs)
//	{
//		this(strs,0,strs.Length);
//	}
	/** 用指定的字符数组构造一个文字属性表 */
	public AttributeList(string[] strs,int offset,int length)
	{
		if(strs==null)
		{
			Debug.LogError(GetType().FullName +" <init>, null strs");
			return;
		}
		if(offset<0||offset>=strs.Length)
		{
			Debug.LogError(GetType().FullName +" <init>, invalid offset:"+offset);
			return;
		}
		if(length<0) length=strs.Length;
		if(length>strs.Length-offset) length=strs.Length-offset;
		int i=length-length%2;
		string[] temp=new string[i];
		if(i>0) Array.Copy(strs,offset,temp,0,i);
		array=temp;
	}

	/* properties */
	/** 属性的数量 */
	public int size()
	{
		return array.Length/2;
	}
	/** 获得全部的属性名称和值 */
	public string[] getArray()
	{
		return array;
	}
	/* methods */
	/** 获得指定属性名称的位置 */
	int indexOf(string[] array,string name)
	{
		int i=array.Length-2;
		if(name!=null)
		{
			for(;i>=0;i-=2)
			{
				if(name.Equals(array[i])) break;
			}
		}
		else
		{
			for(;i>=0;i-=2)
			{
				if(array[i]==null) break;
			}
		}
		return i;
	}
	/** 判断是否包含指定属性 */
	public bool contain(string name)
	{
		string[] array=this.array;
		return indexOf(array,name)>0;
	}
	/** 返回指定属性上的值 */
	public string get(string name)
	{
		string[] array=this.array;
		int i=indexOf(array,name);
		if(i<0) return null;
		return array[i+1];
	}
	/** 返回指定属性上的值 */
	public string get(int i)
	{
		string[] array=this.array;
		if(i<0) return null;
		return array[i*2+1];
	}
	/** 设置指定属性上的值，返回属性上的原值 */
	public string set(string name,string value)
	{
		lock (this) 
		{
			string[] array=this.array;
			int i=indexOf(array,name);
			string old=null;
			if(i<0)
			{
				i=array.Length;
				string[] temp=new string[i+2];
				if(i>0) Array.Copy(array,0,temp,0,i);
				temp[i]=name;
				temp[i+1]=value;
				this.array=temp;
			}
			else
			{
				old=array[i+1];
				array[i+1]=value;
			}
			return old;
		}
	}
	/** 移除指定属性上的值 */
	public string remove(string name)
	{
		lock (this) 
		{
			string[] array=this.array;
			int i=indexOf(array,name);
			if(i<0) return null;
			string value=array[i+1];
			if(array.Length==2)
			{
				this.array=NULL;
				return value;
			}
			string[] temp=new string[array.Length-2];
			if(i>0) Array.Copy(array,0,temp,0,i);
			if(i<temp.Length) Array.Copy(array,i+2,temp,i,temp.Length-i);
			this.array=temp;
			return value;
		}
	}
	/** 清除全部的属性 */
	public void clears()
	{
		array=NULL;
	}
}
