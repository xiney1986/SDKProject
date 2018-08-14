using System;
 
/**
 * 道具模板
 * @author longlingquan
 * */
public class PropSample:Sample
{
	public PropSample ()
	{
		
	}

	public string name = "";//道具名称  
	public string describe = "";//道具描述
	public int iconId = 0;// 道具iconid
	public int qualityId = 0;//道具品质
	public int useLv = 0;//使用等级限制
	public bool isSell = false;//是否可以出售,0代表不可以出售,1代表可以出售
	public int price = 0;//出售价格
	public int type = 0;//道具类型
	public int effectValue = 0;//附加影响值
	public int siftType = 0;//筛选类型
	public int maxUseCount; //每日最大使用次数限制
	public PrizeSample[] needProps;//需要的道具Sid
	public PrizeSample[] prizes;//奖品(展示用)
    public int nextLevelSid;//神格对应的下一级sid(只能用于神格)
    public int expValue;//神格带有的经验
    public int level;//神格等级
	public int orderId = 0;
	 
	override public void parse (int sid, string str)
	{
		this.sid = sid;
		string[] strArr = str.Split ('|');
		//checkLength (strArr.Length, 12);
		
		//strArr[0] is sid  
		//strArr[1] name
		this.name = strArr [1];
		//strArr[2] describe
		this.iconId = StringKit.toInt (strArr [2]);
		//strArr[3] iconId
		this.describe = strArr [3];
		//strArr[4] qualityId
		this.qualityId = StringKit.toInt (strArr [4]);
		//strArr[5] useLv
		this.useLv = StringKit.toInt (strArr [5]);
		//strArr[6] isSell
		this.isSell = parseBool (StringKit.toInt (strArr [6]));
		//strArr[7] price
		this.price = StringKit.toInt (strArr [7]);
		//strArr[8] type
		this.type = StringKit.toInt (strArr [8]);
		//strArr[9] effectValue
		string[] desSplit=strArr [9].Split('^');
		if(desSplit.Length>1){
			this.effectValue = 0;
			needProps=parsePrizes(desSplit[0]);
			prizes=parsePrizes(desSplit[1]);
		}else{
			this.effectValue = StringKit.toInt (strArr [9]);
		}
		//strArr[10] siftType
		this.siftType = StringKit.toInt (strArr [10]);
        this.maxUseCount = StringKit.toInt(strArr[11]);
        this.orderId = StringKit.toInt(strArr[12]);
	    if (strArr.Length > 13)
	    {
	        parseShenGeInfo(strArr[13]);
	    }
	}
	private PrizeSample[] parsePrizes (string str)
	{
        if (str=="0") return null;
		string[] strArr = str.Split ('#');
		PrizeSample[] prizes = new PrizeSample[strArr.Length];
		for (int i = 0; i < strArr.Length; i++) {
			prizes[i]=new PrizeSample();
			string[] strs = strArr[i].Split(',');
			prizes[i].type = StringKit.toInt(strs[0]);
			prizes[i].pSid = StringKit.toInt(strs[1]);
			prizes[i].num = strs[2];
		}
		return prizes;
	}

    private void parseShenGeInfo(string str)
    {
        string[] strings = str.Split(',');
        level = StringKit.toInt(strings[0]);
        expValue = StringKit.toInt(strings[1]);
        nextLevelSid = StringKit.toInt(strings[2]);
    }

    private bool parseBool (int count)
	{
		bool isSell = false;
		if (count == 0)
			isSell = false;
		else if (count == 1)
			isSell = true;
		return isSell;
	}

	public override void copy (object destObj)
	{
		base.copy (destObj);
	}
} 

