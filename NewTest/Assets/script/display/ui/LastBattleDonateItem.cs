using UnityEngine;
using System.Collections;

public class LastBattleDonateItem : MonoBehaviour
{
	public LastBattleDonationWindow fartherWin;
	LastBattleDonationSample itemData;
	public UILabel juGongValue;// 显示完成奖励军功奖励个数//
	public UILabel nvShenBlessValue;// 女神赐福等级//
	public UILabel processValue;// 世界进度//
	public ButtonBase donateBtn;// 捐献按钮//
	public UILabel donateBtnLabel;// 捐献按钮文字//
	public UILabel goodsTittle;// 捐献物资的tittle//
	public UILabel scoreValue;// 排名得分//
	string[] strArr = new string[2];

	public GoodsView goodsTmp;
	public Transform goodsPos;

	public void initItem(LastBattleDonationSample _itemData)
	{
		itemData = _itemData;
		if(itemData != null)
		{
			juGongValue.text = "X" + itemData.junGong.ToString();
			nvShenBlessValue.text = "lv+" + itemData.nvShenBlessLV.ToString();
			processValue.text = "+" + itemData.process.ToString();
			scoreValue.text = "+" + itemData.scores.ToString();
			updateDonateButtonState(itemData);
			setGoodsTittle(itemData);

			GameObject obj = GameObject.Instantiate(goodsTmp.gameObject) as GameObject;
			GoodsView view = obj.GetComponent<GoodsView>();
			obj.transform.parent = this.transform;
			obj.transform.localPosition = goodsPos.localPosition;
			obj.transform.localScale = Vector3.one;
			view.init(itemData.donation);
			view.rightBottomText.gameObject.SetActive(false);
			view.fatherWindow = fartherWin;
		}
	}
	// 更新捐献按钮显示状态//
	public void updateDonateButtonState(LastBattleDonationSample itemData)
	{
		UIEventListener.Get(donateBtn.gameObject).onClick = clickDonateBtn;
		// 未捐献//
		if(itemData.state == LastBattleDonationState.NO_DONATE)
		{
			// 物资够捐献//
			if(LastBattleManagement.Instance.isEnoughDonation(itemData))
			{
				donateBtn.disableButton(false);
			}
			else
			{
				donateBtn.disableButton(true);
			}
			donateBtnLabel.text = LanguageConfigManager.Instance.getLanguage("LastBattle_Donate");
		}
		// 已捐献//
		else if(itemData.state == LastBattleDonationState.YES_DONATE)
		{
			donateBtn.disableButton(true);
			donateBtnLabel.text = LanguageConfigManager.Instance.getLanguage("LastBattle_Donated");
		}
	}

	void setGoodsTittle(LastBattleDonationSample itemData)
	{
		strArr[0] = LastBattleManagement.Instance.getDonationName(itemData);
		strArr[1] = itemData.donation.num;
		goodsTittle.text = string.Format(LanguageConfigManager.Instance.getLanguage("LastBattle_DonateName"),strArr);
	}
	void clickDonateBtn(GameObject obj)
	{
		fartherWin.initGoodsInfoPanel(itemData,this);
	}
	public LastBattleDonationSample getItemData()
	{
		return itemData;
	}
}
