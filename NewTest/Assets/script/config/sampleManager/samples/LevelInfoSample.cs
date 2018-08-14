using System;

/**经验值模板 
  *@author longlingquan
  **/
public class LevelInfoSample:Sample
{
	public LevelInfoSample ()
	{
	}
	
	private string[] infos;
	
	public override void parse (int sid, string str)
	{
		this.sid = sid;
		string[] strArr = str.Split ('|');
		checkLength (strArr.Length, 2);

		infos = strArr [2].Split (',');
	}
	
	public int getInfoByLevel (int level)
	{
		return int.Parse (infos [level - 1]);
	}

	public override void copy (object destObj)
	{
		base.copy (destObj);
		LevelInfoSample dest = destObj as LevelInfoSample;
		if (this.infos != null) {
			dest.infos = new string[this.infos.Length];
			for (int i = 0; i < this.infos.Length; i++)
				dest.infos [i] = this.infos [i];
		}
	}
} 

