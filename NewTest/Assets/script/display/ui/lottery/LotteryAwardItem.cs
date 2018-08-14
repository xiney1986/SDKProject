using UnityEngine;
using System.Collections;

public class LotteryAwardItem : MonoBehaviour
{
	public Transform goodsTmp;// goodsview模板大小//
	public GoodsView goods;
	public UIGrid goodsInfoGrid;// 展示奖品grid//
	public UILabel titleLabel;// 奖品名称//
	private LotterySelectPrizeSample item;
	public ButtonBase btnRecive;// 领取按钮//

	public void updateItem(LotterySelectPrizeSample item,WindowBase farWin)
	{
		this.item = item;
		btnRecive.fatherWindow = farWin;
		btnRecive.onClickEvent = reciveClick;
		// 领取按钮状态//
		if(item.state == LotterySelectPrizeState.Recived)
		{
			btnRecive.disableButton(true);
			btnRecive.textLabel.text = LanguageConfigManager.Instance.getLanguage("recharge02");
		}
		else if(item.state == LotterySelectPrizeState.CantRecive)
		{
			btnRecive.disableButton(true);
			btnRecive.textLabel.text = LanguageConfigManager.Instance.getLanguage("quiz09");
		}
		else if(item.state == LotterySelectPrizeState.CanRecive)
		{
			btnRecive.disableButton(false);
			btnRecive.textLabel.text = LanguageConfigManager.Instance.getLanguage("quiz09");
		}
		titleLabel.text = item.name;
		GameObject goodObj;
		for(int i=0;i<item.prizes.Length;i++)
		{
			goodObj = GameObject.Instantiate(goods.gameObject) as GameObject;
			goodObj.transform.parent = goodsInfoGrid.gameObject.transform;
			goodObj.transform.localPosition = Vector3.zero;
			goodObj.transform.localScale = goodsTmp.localScale;
			goodObj.GetComponent<GoodsView>().init (item.prizes[i]);
			goodObj.GetComponent<GoodsView>().fatherWindow = farWin;
		}
		goodsInfoGrid.repositionNow = true;
	}

	void reciveClick(GameObject obj)
	{
		LotteryAwardFPort fPort = FPortManager.Instance.getFPort("LotteryAwardFPort") as LotteryAwardFPort;
		fPort.lotteryAwardFPorttAccess(CommandConfigManager.Instance.getLotteryData().sid,item.id,updateRecive);
	}
	void updateRecive()
	{
		if(LotteryManagement.Instance.selectedAwardCount >= 1)
		{
			LotteryManagement.Instance.selectedAwardCount--;
		}
		LotterySelectPrizeConfigManager.Instance.getPrize(item.id).state = LotterySelectPrizeState.Recived;
		btnRecive.disableButton(true);
		btnRecive.textLabel.text = LanguageConfigManager.Instance.getLanguage("recharge02");
		UiManager.Instance.createPrizeMessageLintWindow(item.prizes);
		// 英雄之章//
		for(int i=0;i<item.prizes.Length;i++)
		{
			if(item.prizes[i].type == PrizeType.PRIZE_CARD)
			{
				Card card = CardManagerment.Instance.createCard(item.prizes[i].pSid);
				if(card != null)
				{
					if (HeroRoadManagerment.Instance.activeHeroRoadIfNeed(card)) {
						StartCoroutine(Utils.DelayRun(() => {
							UiManager.Instance.openDialogWindow<TextTipWindow>((win) => {
								win.init(LanguageConfigManager.Instance.getLanguage("s0418"), 0.8f);
							});
						},0.7f));
					}
				}
			}
		}
	}
}
