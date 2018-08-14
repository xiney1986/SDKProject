using System;
 
/**
 * 关卡点配置
 * @author longlingquan
 * */
public class MissionPointSample:Sample
{
	public MissionPointSample ()
	{
	}
 
	public int mapId = 0;//所属地图编号
	public int bgId = 0;//背景图标
	public int battleBg = 0;//战斗背景图编号
	
	override public void parse (int sid, string str)
	{
		this.sid = sid;
		string[] strArr = str.Split ('|');
		checkLength (strArr.Length, 3);
		
		//strArr[0] is sid   
		//strArr[1] mapId
		this.mapId = StringKit.toInt (strArr [1]);
		//strArr[2] bgId
		this.bgId = StringKit.toInt (strArr [2]); 
		//strArr[3] battleBg
		this.battleBg = StringKit.toInt (strArr [3]);
	}

	public override void copy (object destObj)
	{
		base.copy (destObj);
	}
} 

