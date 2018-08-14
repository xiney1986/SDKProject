using System;
 
/**
 * 爬塔副本地图对象
 * */
public class ClmbTowerMap
{
    public ClmbTowerMap(string str)
	{
		parse(str);
	}
	public int mapId=0;//爬塔副本大地图编号
	public int chapterSids=0;//剧爬塔副本章节sid集合
	
	private void parse(string str)
	{
		string[] strArr = str.Split('|');
		mapId = StringKit.toInt(strArr[0]);
        chapterSids = StringKit.toInt(strArr[1]);
	}
} 

