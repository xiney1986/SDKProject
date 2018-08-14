/**
 * Coyyright 2001 by seasky <www.seasky.cn>.
 */
using System;
/**
 * 类说明：基于数组的队列
 * 
 * @version 1.0
 * @author zminleo <zmin@seasky.cn>
 */
public class ArrayQueue:Container
{
	

	/* fields */
	/** 队列的对象数组 */
	object[] array;
	/** 队列的头 */
	int head;
	/** 队列的尾 */
	int tail;
	/** 队列的长度 */
	int length;

	/* constructors */
	/** 按指定的大小构造一个队列 */
	public ArrayQueue(int capacity)
	{
		if(capacity<1)
			throw new ArgumentException("ArrayQueue <init>, invalid capacity:"+capacity);
		array=new object[capacity];
		head=0;
		tail=0;
		length=0;
	}
	/** 获得队列的长度 */
	public int size()
	{
		return length;
	}
	/** 获得队列的容积 */
	public int capacity()
	{
		return array.Length;
	}
	/** 判断队列是否为空 */
	public bool isEmpty()
	{
		return length<=0;
	}
	/** 判断队列是否已满 */
	public bool isFull()
	{
		return length>=array.Length;
	}
	/** 得到队列的对象数组 */
	public object[] getArray()
	{
		return array;
	}
	/* methods */
	/** 判断对象是否在容器中 */
	public bool contain(object obj)
	{
		if(obj!=null)
		{
			for(int i=head,n=tail>head?tail:array.Length;i<n;i++)
			{
				if(obj.Equals(array[i])) return true;
			}
			for(int i=0,n=tail>head?0:tail;i<n;i++)
			{
				if(obj.Equals(array[i])) return true;
			}
		}
		else
		{
			for(int i=head,n=tail>head?tail:array.Length;i<n;i++)
			{
				if(array[i]==null) return true;
			}
			for(int i=0,n=tail>head?0:tail;i<n;i++)
			{
				if(array[i]==null) return true;
			}
		}
		return false;
	}
	/** 将对象放入到队列中 */
	public bool add(object obj)
	{
		if(length>=array.Length) return false;
		if(length<=0)
		{
			tail=0;
			head=0;
		}
		else
		{
			tail++;
			if(tail>=array.Length) tail=0;
		}
		array[tail]=obj;
		length++;
		return true;
	}
	/** 检索队列中的第一个对象 */
	public object get()
	{
		return array[head];
	}
	/** 从队列中弹出第一个的对象 */
	public object remove()
	{
		object obj=array[head];
		array[head]=null;
		length--;
		if(length>0)
		{
			head++;
			if(head>=array.Length) head=0;
		}
		return obj;
	}
	/** 清除队列 */
	public void clear()
	{
		for(int i=head,n=tail>head?tail:array.Length;i<n;i++)
			array[i]=null;
		for(int i=0,n=tail>head?0:tail;i<n;i++)
			array[i]=null;
		tail=0;
		head=0;
		length=0;
	}
	public override string ToString()
	{
		return base.ToString()+"[length="+length+", capacity="+array.Length+"]";
	}


}

