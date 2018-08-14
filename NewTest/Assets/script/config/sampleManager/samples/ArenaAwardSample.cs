using System;

/**
 * 竞技场奖励
 *@author yxl
 **/
public class ArenaAwardSample:Sample
{
	/** 决赛 */
	public const int TYPE_FINAL = 1;
	/** 竞猜 */
	public const int TYPE_GUESS = 2;
	/** 积分 */
	public const int TYPE_INTEGRAL = 3;
	/** 占卜 */
	public const int TYPE_DIVINE = 4;
	/** 世界首领 */
	public const int TYPE_WORLDBOSS = 1;
	/** type=1时奖励条件常量 */
	public const int CONDITION_TYPE = 7; // 冠军奖励条件


	public ArenaAwardSample ()
	{
	}
	
	public int type; //1决赛奖励,2竞猜奖励,3积分奖励
	public string name = "";//
	public int condition; //决赛:赛事id, 竞猜:赛事id ,积分:领取所需积分
	public PrizeSample[] prizes;
	public string prizeDescription; //奖励描述
    public int getType;//领取积分奖励的方式（普通，双倍，3倍领取）|0,1,2
    public int costType;//双倍、3倍领取消耗类型
    public int needMoney;//领取消费（双倍，3倍要消费其他货币（钻石））
    public int meritAwardNum;//功勋奖励数量
	
	override public void parse (int sid, string str)
	{
		this.sid = sid;
		string[] strArr = str.Split ('|');
		//checkLength (strArr.Length, 7);
        
		this.type = StringKit.toInt (strArr [1]);
		this.name = strArr [2];
		this.condition = StringKit.toInt (strArr [3]);
		string[] strs = strArr [4].Split ('#');
		prizes = new PrizeSample[strs.Length];
		for (int i = 0; i < prizes.Length; i++) {
			prizes [i] = parsePrize (strs [i]);
		}
	    if (strArr.Length > 6)
	    {
	        getType = StringKit.toInt(strArr[5]);
	        parseCostInfo(strArr[6]);
	    }
	    else {
            this.prizeDescription = strArr[5];
	    }
	}

    private void parseCostInfo(string str)
    {
        string[] strs = str.Split(',');
        costType = StringKit.toInt(strs[0]);
        needMoney = StringKit.toInt(strs[1]);
    }

    PrizeSample parsePrize (string str)
	{
		string[] strs = str.Split (',');
		PrizeSample sample = new PrizeSample ();
		sample.type = StringKit.toInt (strs [0]);
		sample.pSid = StringKit.toInt (strs [1]);
		sample.num = strs [2];
        if (sample.type == PrizeType.PRIZE_MERIT)
        {
            meritAwardNum = StringKit.toInt(sample.num);
        }
        return sample;
	}

	public override void copy (object destObj)
	{
		base.copy (destObj);
		ArenaAwardSample dest = destObj as ArenaAwardSample;
		if (this.prizes != null) {
			dest.prizes = new PrizeSample[this.prizes.Length];
			for (int i = 0; i < dest.prizes.Length; i++)
				dest.prizes [i] = this.prizes [i].Clone () as PrizeSample;
		}
	}
} 

