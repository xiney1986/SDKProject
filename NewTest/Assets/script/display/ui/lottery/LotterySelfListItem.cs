using UnityEngine;
using System.Collections;

public class LotterySelfListItem : MonoBehaviour
{
	public UILabel time;
	public UILabel num;
	public UILabel state;
	Lottery item2;

	public void updateSelfItem(Lottery item)
	{
		item2 = item;
		time.text = item.time;
		num.text = item.num;
		if(item.state == LotteryState.NotOpenAward)
		{
			time.text = "[6e473d]" + item.time;
			num.text = "[6e473d]" + item.num;
			state.text =  LanguageConfigManager.Instance.getLanguage("lottery_notOpenAward");
		}
		else 
		{
			time.text = "[808080]" + item.time;
			num.text = "[808080]" + item.num;
			if(item.state == LotteryState.NoAward)
			{
				state.text = LanguageConfigManager.Instance.getLanguage("lottery_noAward");
			}
			else if(item.state == LotteryState.SpecialAward)
			{
				state.text = LanguageConfigManager.Instance.getLanguage("lottery_specialAward");
			}
			else if(item.state == LotteryState.FirstAward)
			{
				state.text = LanguageConfigManager.Instance.getLanguage("lottery_firstAward");
			}
			else if(item.state == LotteryState.SecondAward)
			{
				state.text = LanguageConfigManager.Instance.getLanguage("lottery_secondAward");
			}
			else if(item.state == LotteryState.ThirdAward)
			{
				state.text = LanguageConfigManager.Instance.getLanguage("lottery_thirdAward");
			}
		}
		gameObject.SetActive(true);
	}
}
