using System;

/**经验值模板 
  *@author longlingquan
  **/
public class EXPSample:Sample
{
	public EXPSample ()
	{
	}

	private long[] exps;
	
	public override void parse (int sid, string str)
	{
		this.sid = sid;
		string[] strArr = str.Split ('|');
		checkLength (strArr.Length, 1);
		 
		//strArr[0] is sid  
		//strArr[1] exps
		parseEXP (strArr [1]);
	}
	
	private void parseEXP (string str)
	{
		string[] strArr = str.Split (',');
		exps = new long[strArr.Length]; 
		for (int i = 0; i<strArr.Length; i++) {
			exps [i] = StringKit.toLong (strArr [i]);
		}
	}
	
	public long[] getExps ()
	{
		return exps;
	}

	public override void copy (object destObj)
	{
		base.copy (destObj);
		EXPSample dest = destObj as EXPSample;
		if (this.exps != null) {
			dest.exps = new long[this.exps.Length];
			for (int i = 0; i < this.exps.Length; i++)
				dest.exps [i] = this.exps [i];
		}
	}
} 

