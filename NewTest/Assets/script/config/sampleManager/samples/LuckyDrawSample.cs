using System;

/**
 * 抽奖配置模板
 * @author longlingquan
 * */
public class LuckyDrawSample:Sample
{
	public LuckyDrawSample ()
	{
	}
	
	public string name = "";//抽奖名称
	public int iconId = 0;//抽奖广告图标
	public string describe = "";//抽奖描述
	public string idsType = "";//1card 2equip 3猎魂 4炼金
	public int[] ids = null;//抽奖2级界面滚动图标集合
	public bool isReset = false;//是否重置 重置周期
	public int freeNum = 0;//免费次数
	public int drawNum = 0;//限制次数(抽奖次数)
	public int startTime = 0;//开始时间抽奖次数(单位秒)
	public int endTime = 0;//结束时间(单位秒)
	public DrawWaySample[] ways = null;//抽奖方式
	public int luckyIndex = 0; // 排序
	public int drawType = 0;//抽奖类型 1为主界面抽奖 2为星星抽奖
	public string[] luckyPoints; //必出某个品质的卡 [是否需要显示,品质,倍数,品质,倍数,...]
	public int noticeSid; // 相关公告sid

	public override void copy (object destObj)
	{
		base.copy (destObj);
		LuckyDrawSample dest = destObj as LuckyDrawSample;
		if (this.ids != null) {
			dest.ids = new int[this.ids.Length];
			for (int i = 0; i < this.ids.Length; i++)
				dest.ids [i] = this.ids [i];
		}
		if (this.ways != null) {
			dest.ways = new DrawWaySample[this.ways.Length];
			for (int i = 0; i < this.ways.Length; i++)
				dest.ways [i] = this.ways [i].Clone () as DrawWaySample;
		}
		if (this.luckyPoints != null) {
			dest.luckyPoints = new string[this.luckyPoints.Length];
			for (int i = 0; i < this.luckyPoints.Length; i++)
				dest.luckyPoints [i] = this.luckyPoints [i];
		}
	}

	public override void parse (int sid, string str)
	{
		this.sid = sid; 
		string[] strArr = str.Split ('|');
		checkLength (strArr.Length, 14);
		
		//strArr[0] is sid  
		//strArr[1] name
		this.name = strArr [1];
		//strArr[2] iconId
		this.iconId = StringKit.toInt (strArr [2]); 
		//strArr[3] describe
		this.describe = strArr [3]; 
		//strArr[4] ids
		parseIds (strArr [4]); 
		//strArr[5] isReset 
		parseIsReset (strArr [5]); 
		//strArr[6] freeNum
		this.freeNum = StringKit.toInt (strArr [6]);
		//strArr[7] drawNum
		this.drawNum = StringKit.toInt (strArr [7]);
		//strArr[8] startTime
		this.startTime = StringKit.toInt (strArr [8]); 
		//strArr[9] endTime
		this.endTime = StringKit.toInt (strArr [9]); 
		//strArr[10] luckyIndex
		this.luckyIndex = StringKit.toInt (strArr [10]);
		//strArr[11] ways
		parseDrawWays (strArr [11]); 
		//strArr[12] drawType
		this.drawType = StringKit.toInt (strArr [12]);
		//strArr[13] noticeSid
		noticeSid = StringKit.toInt (strArr [13]);
		//strArr[14] luckyPoints
		parseLuckyPoints (strArr [14]);
	}
	
	private void parseIsReset (string str)
	{
		if (str == "1")
			isReset = true;
	}
	
	private void parseIds (string str)
	{
		string[] strArr = str.Split ('#');
		ids = new int[strArr.Length - 1];
		idsType = strArr [0];
		for (int i = 1; i < strArr.Length; i++) {
			ids [i - 1] = StringKit.toInt (strArr [i]);
		}
	}
	
	//解析抽奖方式
	private void parseDrawWays (string str)
	{
		string[] strArr = str.Split ('#');
		ways = new DrawWaySample[strArr.Length];
		for (int i = 0; i < strArr.Length; i++) {
			ways [i] = new DrawWaySample (strArr [i]);
		}
	}

	private void parseLuckyPoints (string str)
	{
		luckyPoints = str.Split ('#');
	}
} 

/**
 * 抽奖方式 不是标准模板 无sid
 * */
public class DrawWaySample:CloneObject
{
	public DrawWaySample (string str)
	{
		parse (str);
	}
	
	public int drawTypeId = 0;//抽奖方式id 与后台抽奖方式对应 
	public int drawTimes = 0;//抽奖数量
	public int costType = 0;//消耗类型
	public int costToolSid = 0;//消耗道具id 
	public int formulaId = 0;//公式id
	public int[] factors = null;//公式系数 
	
	private void parse (string str)
	{
		string[] strArr = str.Split ('*');
		//抽奖方式
		drawTypeId = StringKit.toInt (strArr [0]);
		//抽奖数量
		drawTimes = StringKit.toInt (strArr [1]);
		//消耗类型
		costType = StringKit.toInt (strArr [2]);
		//消耗道具id
		costToolSid = StringKit.toInt (strArr [3]); 
		//公式id
		formulaId = StringKit.toInt (strArr [4]);
		//公式系数
		parseFactors (strArr [5]); 
	}
	
	//解析公式系数
	private void parseFactors (string str)
	{
		string[] strArr = str.Split ('$');
		factors = new int[strArr.Length];
		for (int i = 0; i < strArr.Length; i++) {
			factors [i] = StringKit.toInt (strArr [i]);
		}
	}

	public override void copy (object destObj)
	{
		base.copy (destObj);
		DrawWaySample dest = destObj as DrawWaySample;
		if (this.factors != null) {
			dest.factors = new int[this.factors.Length];
			for (int i = 0; i < this.factors.Length; i++)
				dest.factors [i] = this.factors [i];
		}
	}
}

