using UnityEngine;
using System.Collections;

/// <summary>
/// 用于购买挑战次数
/// </summary>
public class ArenaChallengePrice 
{
    private int myNum;

    public ArenaChallengePrice(int myNum)
    {
        this.myNum = myNum;
    }

    public string getName()
    {
        return LanguageConfigManager.Instance.getLanguage("Arena37");
    }

    public string getIconPath()
    {
        return constResourcesPath.TIMES_ICONPATH;
    }

    public int getPrice (int num) {
//		竞技场消耗公式
//			购买后的总消耗=INT(0.75*MIN(购买后的次数,49)^2+3*MIN(购买后的次数,49)+300*MAX(购买后的次数-49,0)+3)
//			购买前的总消耗=INT(0.75*MIN(购买前的次数,49)^2+3*MIN(购买前的次数,49)+300*MAX(购买前的次数-49,0)+3)
//			实际消耗=购买后的总消耗-购买前的总消耗
//			初始购买前的次数默认为0
       
        int reBuy = ArenaManager.instance.buyChallengeCount;
        int[] prices = CommandConfigManager.Instance.getPrice();
        int AllPrice =0;
        //int currectNum = 1;
        //int temBuy = 0;
        for (int i = 0; i < num;i++ )
        {
            AllPrice += reBuy < (prices.Length - 1) ? prices[reBuy] : prices[prices.Length - 1];
           
            reBuy++;
        }
        return AllPrice;

        //int dstNum = myNum + num;
        //int money2 = (int)(0.75f * Mathf.Pow (Mathf.Min (dstNum, 49), 2) + 3 * Mathf.Min (dstNum, 49) + 300 * Mathf.Max (dstNum - 49, 0) + 3);
        //int money1 = (int)(0.75f * Mathf.Pow (Mathf.Min (myNum, 49), 2) + 3 * Mathf.Min (myNum, 49) + 300 * Mathf.Max (myNum - 49, 0) + 3);
        //return money2 - money1;
      
	}
}
