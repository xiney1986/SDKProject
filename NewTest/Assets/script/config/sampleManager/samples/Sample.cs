using System;
 
public class Sample:CloneObject
{
	public Sample ()
	{
	}
	
	public int sid = 0;//sid

	public virtual void parse (int sid, string str)
	{
		
	}
	
	//长度检测
	public void checkLength (int len, int indexmax)
	{ 
		if (len != indexmax + 1)
			throw new Exception (this.GetType () + " config error sid=" + sid + " len!=indexmax+1,len=" + len + " indexmax=" + indexmax);
	}

	public override void copy (object destObj)
	{
		base.copy (destObj);
	}
}

