using UnityEngine;
using System.Collections;



public struct AwardGoodsInfo
{
    public int type;
    public int sid;
    public int num;
}


public class HappySundaySample 
{

    public HappySundaySample()
    {

	}

	public int Sid;
    /// <summary>
    /// 星期几显示
    /// </summary>
   // public int Week;
    /// <summary>
    /// 最大积分
    /// </summary>
	public int MaxScore;
    /// <summary>
    /// 奖励道具
    /// </summary>
	public AwardGoodsInfo[] AwardGoods;
    /// <summary>
    /// 限制在开服多少天后显示
    /// </summary>
    public int OnlineDay;
	public int timeID;


	public void parse (int id, string str)
	{
		this.Sid = id; 
		string[] strArr = str.Split ('|');
        OnlineDay = StringKit.toInt(strArr[1]);
		timeID = StringKit.toInt(strArr[2]);
//        Week = StringKit.toInt(strArr[2]);
//        if (Week == 7) Week = 0; //将周末转换为程序里面的周日0
        MaxScore = StringKit.toInt(strArr[3]);
        string[] goods = strArr[4].Split('#');
        AwardGoods = new AwardGoodsInfo[goods.Length];
        for (int i = 0; i < goods.Length; i++)
        {
            string[] data = goods[i].Split(',');
            if (data.Length < 3) continue;
            AwardGoods[i] = new AwardGoodsInfo() { type = StringKit.toInt(data[0]), sid = StringKit.toInt(data[1]), num = StringKit.toInt(data[2]) };
        }


	}

}
