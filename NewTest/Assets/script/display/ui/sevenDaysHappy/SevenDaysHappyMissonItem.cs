using UnityEngine;
using System.Collections;

public class SevenDaysHappyMissonItem : MonoBehaviour
{

	public GoodsView[] goods;
	public SevenDaysHappyAwardReceve buttonAward;// 领奖按钮//
	SevenDaysHappyMisson misson;
	public UILabel conditionDes;// 条件描述//
	

	public void updateItem(SevenDaysHappyMisson misson,WindowBase fatherWin,SevenDaysHappyMissonContent missonContent,SevenDaysHappyContent content,SevenDaysHappyDetailBtn detailBtn)
	{
		this.misson = misson;
		buttonAward.GetComponent<SevenDaysHappyAwardReceve>().misson = misson;
		buttonAward.GetComponent<SevenDaysHappyAwardReceve>().fatherWindow = fatherWin;
		buttonAward.GetComponent<SevenDaysHappyAwardReceve>().missonContent = missonContent;
		buttonAward.GetComponent<SevenDaysHappyAwardReceve>().content = content;
		buttonAward.GetComponent<SevenDaysHappyAwardReceve>().detailBtn = detailBtn;


		for(int i=0;i<misson.prizes.Length;i++)
		{
			goods[i].gameObject.SetActive(true);
			goods[i].init(misson.prizes[i]);
			goods[i].fatherWindow = fatherWin;
		}

		showConditionDes();

		if(misson.missonState == SevenDaysHappyMissonState.Recevied)// 已领取//
		{
			buttonAward.disableButton(true);
			buttonAward.textLabel.text = LanguageConfigManager.Instance.getLanguage("recharge02");
		}
		else if(misson.missonState == SevenDaysHappyMissonState.Completed)// 已完成可领取//
		{
			buttonAward.disableButton(false);
			buttonAward.textLabel.text = LanguageConfigManager.Instance.getLanguage("GuildLuckyNvShen_20");
		}
		else if(misson.missonState == SevenDaysHappyMissonState.Doing)// 进行中未达成//
		{
			if(misson.missonType == SevenDaysHappyMissonType.Recharge)// 充值类型//
			{
				buttonAward.disableButton(false);
				buttonAward.textLabel.text = LanguageConfigManager.Instance.getLanguage("s0315");
			}
			else
			{
				buttonAward.disableButton(true);
				buttonAward.textLabel.text = LanguageConfigManager.Instance.getLanguage("GuildLuckyNvShen_20");
			}

		}


	}

	public void showConditionDes()
	{
		conditionDes.text = misson.conditionDes;
		if(misson.missonType == SevenDaysHappyMissonType.Recharge || misson.missonType == SevenDaysHappyMissonType.CompleteByCount)// 充值类任务或者按数量完成的任务//
		{
			if(misson.missonProgress != null)
			{
				for(int i=0;i<misson.missonProgress.Length;i++)
				{
					// 钻石转换成元//
					if(misson.missonType == SevenDaysHappyMissonType.Recharge)
					{
						conditionDes.text = conditionDes.text + "(" + misson.missonProgress[i] / 10 + "/" + misson.conditions[i] + ")";
					}
					else
					{
						conditionDes.text = conditionDes.text + "(" + misson.missonProgress[i] + "/" + misson.conditions[i] + ")";
					}
				}
			}
		}
		else if(misson.missonType == SevenDaysHappyMissonType.CompleteNotByCount)
		{
			conditionDes.text = conditionDes.text + "(" + misson.missonProgress[0] + "/" + "1)";
		}
	}
	
}
