using UnityEngine;
using System.Collections;

/// <summary>
/// 用于购买副本挑战次数
/// </summary>
public class FubenChallengePrice 
{
    private int myNum;
	public FubenChallengePrice(int myNum)
    {
        this.myNum = myNum;
    }
    public int getPrice (int num) {
        string str = CommonConfigSampleManager.Instance.getSampleBySid<FubenBuyChallengeTimesCostSample>(3).getPricesString();
        string[] attr = str.Split(',');
        return StringKit.toInt(attr[this.myNum]);
	}
	public string getPriceString(){
        string str = CommonConfigSampleManager.Instance.getSampleBySid<FubenBuyChallengeTimesCostSample>(3).getPricesString();
        string[] attr = str.Split(',');
		return attr[this.myNum];
	}
}
