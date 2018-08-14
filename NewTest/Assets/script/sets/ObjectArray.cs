/**
 * Coypright 2013 by 刘耀鑫<xiney@youkia.com>.
 */

using System;
/**
 * @author 刘耀鑫
 */

public class ObjectArray:ICloneable,Container,Selectable
{

	/* static fields */
	/** 空数组 */
	public static readonly object[] NULL={};

	/* static fields */
	/** 移除数组中指定位置的对象，返回新数组 */
	public static object[] remove(object[] array,int i)
	{
		if(array.Length<=1) return NULL;
		object[] temp=new object[array.Length-1];
		if(i>0) Array.Copy(array,0,temp,0,i);
		if(i<temp.Length) Array.Copy(array,i+1,temp,i,temp.Length-i);
		return temp;
	}

	/* fields */
	/** 数组 */
	private object[] array;
	/** 对象比较器 */
	private Comparator comparator;
	/** 降序 */
	private bool descending;

	/* constructors */
	/** 构造一个列表 */
	public ObjectArray():this(NULL)
	{
	}
	/** 用指定的对象数组构造一个列表 */
	public ObjectArray(object[] array)
	{
		this.array=array;
	}
	/* properties */
	/** 获得数量 */
	public int size()
	{
		return array.Length;
	}
	/** 判断容器是否为空 */
	public bool isEmpty()
	{
		return array.Length<=0;
	}
	/** 判断容器是否已满 */
	public bool isFull()
	{
		return false;
	}
	/** 获得数组 */
	public object[] getArray()
	{
		return array;
	}
	/** 获得对象比较器 */
	public Comparator getComparator()
	{
		return comparator;
	}
	/** 设置对象比较器 */
	public void setComparator(Comparator comparator)
	{
		this.comparator=comparator;
	}
	/** 获得对象比较器 */
	public bool isDescending()
	{
		return descending;
	}
	/** 设置对象比较器 */
	public void setDescending(bool b)
	{
		descending=b;
	}
	/* methods */
	/** 判断是否包含指定的对象 */
	public bool contain(object obj)
	{
		object[] array=this.array;
		if(obj!=null)
		{
			for(int i=array.Length-1;i>=0;i--)
			{
				if(obj.Equals(array[i])) return true;
			}
		}
		else
		{
			for(int i=array.Length-1;i>=0;i--)
			{
				if(array[i]==null) return true;
			}
		}
		return false;
	}
	/** 添加指定的对象 */
	public bool add(object obj)
	{
		object[] array=this.array;
		int i=array.Length;
		object[] temp=new object[i+1];
		if(i>0) Array.Copy(array,0,temp,0,i);
		temp[i]=obj;
		if(comparator!=null) SetKit.sort(temp,comparator,descending);
		this.array=temp;
		return true;
	}
	/** 添加指定的对象数组 */
	public void add(object[] objs)
	{
		if(objs!=null&&objs.Length>0) add(objs,0,objs.Length);
	}
	/** 添加指定的对象数组 */
	public void add(object[] objs,int index,int length)
	{
		if(objs==null||index<0||length<=0||objs.Length<index+length) return;
		object[] array=this.array;
		int i=array.Length;
		object[] temp=new object[i+length];
		if(i>0) Array.Copy(array,0,temp,0,i);
		Array.Copy(objs,index,temp,i,length);
		if(comparator!=null) SetKit.sort(temp,comparator,descending);
		this.array=temp;
	}
	/** 检索容器中的对象 */
	public object get()
	{
		object[] array=this.array;
		return array[array.Length-1];
	}
	/** 获得指定对象的位置 */
	int indexOf(object[] array,object obj)
	{
		int i=array.Length-1;
		if(obj!=null)
		{
			for(;i>=0&&!obj.Equals(array[i]);i--)
				;
		}
		else
		{
			for(;i>=0&&array[i]!=null;i--)
				;
		}
		return i;
	}
	/** 移除指定的对象 */
	public bool remove(object obj)
	{
		object[] array=this.array;
		int i=indexOf(array,obj);
		if(i<0) return false;
		this.array=remove(array,i);
		return true;
	}
	/** 移除对象 */
	public object remove()
	{
		object[] array=this.array;
		int i=array.Length-1;
		object obj=array[i];
		this.array=remove(array,i);
		return obj;
	}
	/** 排序 */
	public void sort()
	{
		sort(comparator,descending);
	}
	/** 排序 */
	public void sort(Comparator comparator,bool descending)
	{
		if(comparator==null) return;
		object[] array=this.array;
		object[] temp=new object[array.Length];
		Array.Copy(array,0,temp,0,array.Length);
		SetKit.sort(temp,comparator,descending);
		this.array=temp;
	}
	/** 选择方法，用指定的选择器对象选出表中的元素，返回值参考常量定义 */
	public int select(Selector selector)
	{
		object[] array=this.array;
		object[] temp=null;
		int n=array.Length;
		int i=0,j=n;
		int t;
		int r=SelectorKit.FALSE;
		for(;i<n;i++)
		{
			t=selector.select(array[i]);
			if(t==SelectorKit.FALSE) continue;
			if(t==SelectorKit.TRUE)
			{
				if(temp==null)
				{
					temp=new object[array.Length];
					Array.Copy(array,0,temp,0,array.Length);
				}
				temp[i]=temp;
				j--;
				r=t;
				continue;
			}
			if(t==SelectorKit.TRUE_BREAK)
			{
				if(temp==null)
				{
					temp=new object[array.Length];
					Array.Copy(array,0,temp,0,array.Length);
				}
				temp[i]=temp;
				j--;
			}
			r=t;
			break;
		}
		if(temp==null) return r;
		if(j<=0)
		{
			this.array=NULL;
			return r;
		}
		object[] tmp=new object[j];
		for(i=0,j=0;i<n;i++)
		{
			if(temp[i]!=temp) tmp[j++]=temp[i];
		}
		this.array=tmp;
		return r;
	}
	/** 以对象数组的方式得到列表中的元素 */
	public object[] toArray()
	{
		object[] array=this.array;
		object[] temp=new object[array.Length];
		Array.Copy(array,0,temp,0,array.Length);
		return temp;
	}
	/** 将列表中的元素拷贝到指定的数组 */
	public object[] toArray(object[] objs)
	{
		object[] array=this.array;
		int len=(objs.Length>array.Length)?array.Length:objs.Length;
		Array.Copy(array,0,objs,0,len);
		return objs;
	}
	/** 清除列表中的所有元素 */
	public void clear()
	{
		array=NULL;
	}
	/* common methods */
	public object Clone()
	{
		return base.MemberwiseClone();
	}
	public override string ToString()
	{
		return base.ToString()+"[size="+array.Length
			+(comparator!=null?" descending="+descending:"")+"]";
	}

}
