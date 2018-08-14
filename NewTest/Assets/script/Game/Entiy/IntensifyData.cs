using System;

/**
 * 食物基类
 * @author 汤琦
 * */
public class IntensifyData 
{
	public virtual string ToFooding()
	{
		throw new Exception( GetType() + " Need to subclass cover");
	}
}
