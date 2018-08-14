using System;
 
/**
 * 剧情副本地图对象
 * */
public class FuBenStoryMap
{
	public FuBenStoryMap (string str)
	{
		parse(str);
	}
	public int mapId=0;//剧情副本大地图编号
	public int[] chapterSids=null;//剧情副本章节sid集合
	
	private void parse(string str)
	{
		string[] strArr = str.Split('|');
		mapId = StringKit.toInt(strArr[0]);
		string[] strArr2 = (strArr[1]).Split(',');
		int max = strArr2.Length;
		chapterSids = new int[max];
		for (int i = 0; i < max; i++) {
			chapterSids[i] =StringKit.toInt(strArr2[i]);
		}
	}
} 

