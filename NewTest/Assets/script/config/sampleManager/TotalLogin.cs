using System;
 
/**
 * 累计登陆奖励
 * @author longlingquan
 * */
public class TotalLogin
{
	public TotalLogin(){
	}
	public TotalLogin (string str)
	{
		parse (str);
	}
	public string single="";//周几描述
	public int week;//星期几
	public string showTime;//显示的时间
	public int day;//第几天
	public int holidayAllSid;//节日送活动大Sid;
	public string holidayName;//节日名称
	public string holidayBeginTime;
	public string holidayEndTime;
	public int isAward;//是否领取
	public int isloginn;//是否登陆过
	public int prizeSid;//领奖sid
	public int totalDays;//累计天数
	public int rewardType;//奖励类型	BIGPRIZE = 1;//大奖  SMALLPRIZE = 0;//小奖
	public PrizeSample[] prizes;//奖品


	private void parse (string str)
	{
		string[] strArr = str.Split ('|');
		//strArr[0] 
		this.prizeSid = StringKit.toInt(strArr [0]);
		//strArr[1] totalDays
		this.totalDays = StringKit.toInt(strArr [1]);
		
		//strArr[2] rewardType
		this.rewardType = StringKit.toInt(strArr [2]);
		
		//strArr[3] prizes
		parsePrizes(strArr[3]);
		
	}
	
	private void parsePrizes (string str)
	{
		string[] strArr = str.Split ('#');
		prizes = new PrizeSample[strArr.Length];
		for (int i = 0; i < strArr.Length; i++) {
			prizes[i]=new PrizeSample();
			string[] strs = strArr[i].Split(',');
			prizes[i].type = StringKit.toInt(strs[0]);
			prizes[i].pSid = StringKit.toInt(strs[1]);
			prizes[i].num = strs[2];
		}
	}
} 

