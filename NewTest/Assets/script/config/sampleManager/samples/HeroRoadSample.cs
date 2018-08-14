using System;

/**经验值模板 
  *@author longlingquan
  **/
public class HeroRoadSample:Sample
{
	public int chapter; //章节ID
	public int quality; //品质

	public override void parse (int sid, string str)
	{
		this.sid = sid;
		string[] strArr = str.Split ('|');
		checkLength (strArr.Length, 2);
		
		chapter = StringKit.toInt (strArr [1]);
		quality = StringKit.toInt (strArr [2]);
	}
	
	public int getMissionCount ()
	{
		ChapterSample obj = ChapterSampleManager.Instance.getChapterSampleBySid (chapter);
		return obj.missions.Length;
	}

	public override void copy (object destObj)
	{
		base.copy (destObj);
	}
} 

