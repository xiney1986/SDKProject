using System;
 
/**
 * 剧情模板
 * @author longlingquan
 * */
public class PlotSample:Sample
{
	public PlotSample ()
	{
	}
	
	public int[] sids;//对白sid有序集合
	
	public override void parse (int sid, string str)
	{ 
		this.sid = sid;
		string[] strArr = str.Split ('|');
		checkLength (strArr.Length, 1);
		parseInfo (strArr [1]);
	}
	
	private void parseInfo (string str)
	{
		string[] strArr = str.Split (',');
		sids = new int[strArr.Length]; 
		for (int i = 0; i<strArr.Length; i++) {
			sids [i] = StringKit.toInt (strArr [i]);
		}
	}

	public override void copy (object destObj)
	{
		base.copy (destObj);
		PlotSample dest = destObj as PlotSample;
		if (this.sids != null) {
			dest.sids = new int[this.sids.Length];
			for (int i = 0; i < this.sids.Length; i++)
				dest.sids [i] = this.sids [i];
		}
	}
}

