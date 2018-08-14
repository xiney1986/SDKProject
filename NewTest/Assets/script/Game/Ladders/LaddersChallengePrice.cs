using UnityEngine;
using System.Collections;

/// <summary>
/// 用于购买挑战次数
/// </summary>
public class LaddersChallengePrice 
{
	private int[] PriceArr ={200,400,600,800,1000,1200,1400,1600,1800,2000};
	public LaddersChallengePrice()
    {
    }

    public string getName()
    {
        return LanguageConfigManager.Instance.getLanguage("Arena37");
    }

    public string getIconPath()
    {
        return constResourcesPath.TIMES_ICONPATH;
    }

    public int getPrice(int num)
    {
//		if(LaddersManagement.Instance.buyFightCount >=15 && LaddersManagement.Instance.buyFightCount <=30 )
//			return LaddersConfigManager.Instance.config_Const.price_challengeTimes*num*2;
//		else
//			return LaddersConfigManager.Instance.config_Const.price_challengeTimes*num;
//		int temp = 15 - LaddersManagement.Instance.buyFightCount > 0 ? 15 - LaddersManagement.Instance.buyFightCount : 0;
//		int doubleNum = num - temp > 0 ? num - temp : 0;
//		return LaddersConfigManager.Instance.config_Const.price_challengeTimes*(num + doubleNum);

		int sum = 0;
		for (int i = LaddersManagement.Instance.buyFightCount + 1; i <= LaddersManagement.Instance.buyFightCount + num; i++) {
			if(i<7){
				sum += PriceArr[(i-1)/2];
				continue;
			}
			if(i<19){
				sum += PriceArr[(i+2)/3];
				continue;
			}
			if(i<25){
				sum += PriceArr[7];
				continue;
			}
			sum += PriceArr[(i-1)/3];
		}
		return sum;
    }
}
