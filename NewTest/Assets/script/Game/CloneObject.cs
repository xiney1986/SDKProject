using System;

/**
 * 克隆基础对象
 * 
 * */
public class CloneObject:ICloneable
{
	public CloneObject ()
	{
	}

	public object Clone()
	{
		object desObj = base.MemberwiseClone ();
		copy(desObj);
		return desObj;
	}

	//重写复杂数据的克隆
	public virtual void copy(object destObj)
	{
	}
}

