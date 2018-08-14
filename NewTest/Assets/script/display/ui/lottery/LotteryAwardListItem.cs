using UnityEngine;
using System.Collections;

public class LotteryAwardListItem : MonoBehaviour
{
	AwardLottery item;
	public UILabel time;
	public UILabel name;
	public UILabel num;
	public UILabel money;

	public void updateItem(AwardLottery item)
	{
		this.item = item;
		time.text = item.time;
		name.text = item.serName + item.playerName;
		num.text = item.awardNum;
		money.text = item.money;
		gameObject.SetActive(true);
	}
}
