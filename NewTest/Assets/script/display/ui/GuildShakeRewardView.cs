using UnityEngine;
using System.Collections;

public class GuildShakeRewardView : MonoBase {
	/** 数量 */
	public UILabel num;
	/** 图标 */
	public UITexture icon;
	public void Init(PrizeSample prize)
	{
		if (prize == null || prize.getPrizeNumByInt () == 0) {
			num.text = "[6E473D]" + Language("GuildLuckyNvShen_16");
		} else {
//			if(prize.type == PrizeType.PRIZE_SHAKE_SCORE || prize.type == PrizeType.PRIZE_CONTRIBUTION ){
			if(prize.type == PrizeType.PRIZE_SHAKE_SCORE ){
				num.transform.localPosition = new Vector3(10,-2,0);
				num.text = prize.num.ToString()+prize.getPrizeName();
				icon.gameObject.SetActive(false);
			}
			else
			{
				num.text = prize.num.ToString();
				icon.gameObject.SetActive(true);
				ResourcesManager.Instance.LoadAssetBundleTexture (prize.getIconPath (), icon);
			}

		}
	}
		
}
