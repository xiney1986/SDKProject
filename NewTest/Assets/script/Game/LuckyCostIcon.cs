using UnityEngine;
using System.Collections;

/**
 * 抽奖花费图标类
 * @author 汤琦
 * */
public class LuckyCostIcon 
{
	//设置花费图标静态方法
	public static void setToolCostIconName(DrawWay way,UISprite icon)
	{
		switch(way.getCostType())
		{
		case PrizeType.PRIZE_MONEY:
			icon.spriteName = "title_money";
			break;
		case PrizeType.PRIZE_RMB:
			icon.spriteName = "rmb";
			break;
		case PrizeType.PRIZE_PROP:
			switch(PropSampleManager.Instance.getPropSampleBySid(way.getCostToolSid()).iconId)
			{
			case 1:
				icon.spriteName = "goldCoin";
				break;
			case 2:
				icon.spriteName = "ptCoin";
				break;
			case 3:
				icon.spriteName = "activityCoin";
				break;
			case 4:
				icon.spriteName = "ptCoin";
				break;
			case 5:
				icon.spriteName = "sliverCoin";
				break;
			}
			break;
		}
	}
	
}
