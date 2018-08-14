using UnityEngine;
using System.Collections;

public class NoticeActivityShopBarCtrl : MonoBase
{ 
	[HideInInspector]
	public NoticeConsumeRebateContent
		fatherContent;
	public NoticeActiveGoods activeGoods;
	public UILabel conditionDes, costNumLabel, totalCountLabel, maxBuyLabel, goodsName;
	public GameObject goodsShowPos;
	public ButtonBase  buyButton;
	public GameObject goodsViewPref;

	public void updateItem (NoticeActiveGoods goods, NoticeSample sample)
	{
		NoticeActiveServerInfo serverInfo = fatherContent.serverInfo;
		activeGoods = goods;
		//2014.7.17 modified
		//conditionDes.text = Language ("ConsumeRebate_01", activeGoods.getSample().rmbCondition, 
		//                             Mathf.Min (serverInfo.consumeValue, activeGoods.getSample().rmbCondition), activeGoods.getSample().rmbCondition);
		conditionDes.text = Language ("ConsumeRebate_01", activeGoods.getSample().rmbCondition, 
		                              activeGoods.getRoleCountCanBuy (),  activeGoods.getSample().maxBuyCount);
		costNumLabel.text = "X " + activeGoods.getCostPrice ();
		totalCountLabel.text = Language ("ConsumeRebate_04", activeGoods.getServerCountCanBuy ());
		//maxBuyLabel.text = Language ("ConsumeRebate_03", activeGoods.getRoleCountCanBuy (), activeGoods.getSample().maxBuyCount);
		maxBuyLabel.text = Language ("ConsumeRebate_03", activeGoods.getSample().rmbCondition - serverInfo.consumeValue > 0 ? activeGoods.getSample().rmbCondition - serverInfo.consumeValue : 0 );
		if (goodsShowPos.transform.childCount > 0) {
			goodsShowPos.transform.GetChild (0).GetComponent<GoodsView> ().init (activeGoods.getGoodsType (), activeGoods.getGoodsSid (), activeGoods.getGoodsShowNum ());
		} else {
			GoodsView goodsView = (NGUITools.AddChild (goodsShowPos, goodsViewPref) as GameObject).GetComponent<GoodsView> ();
			goodsView.init (activeGoods.getGoodsType (), activeGoods.getGoodsSid (), activeGoods.getGoodsShowNum ());
			goodsView.fatherWindow = fatherContent.win;
		}
		if (activeGoods.getRoleCountCanBuy () < 1 || activeGoods.getServerCountCanBuy () < 1)
			buyButton.disableButton (true);
		else {
			buyButton.disableButton (false);
			buyButton.fatherWindow = fatherContent.win;
			buyButton.onClickEvent = clickBuyButton;
		}
	}

	private void clickBuyButton (GameObject obj)
	{
		ActiveTime activeTime = (fatherContent.notice as ConsumeRebateNotice).activeTime;
		int now = ServerTimeKit.getSecondTime ();
		if (now < activeTime.getDetailStartTime())
			UiManager.Instance.createMessageLintWindow (Language ("s0171"));
		else if (now > activeTime.getDetailEndTime())
			UiManager.Instance.createMessageLintWindow (Language ("ConsumeRebate_07"));
		else if (activeGoods.getRoleCountCanBuy () < 1)
			UiManager.Instance.createMessageLintWindow (Language ("ConsumeRebate_08"));
		else if (activeGoods.getServerCountCanBuy () < 1)
			UiManager.Instance.createMessageLintWindow (Language ("ConsumeRebate_09"));
		else if (activeGoods.getSample().rmbCondition > fatherContent.serverInfo.consumeValue)
			UiManager.Instance.createMessageLintWindow (Language ("ConsumeRebate_10"));
		else
			UiManager.Instance.openDialogWindow<BuyWindow> ((win) => {
				win.init (activeGoods, Mathf.Min (activeGoods.getServerCountCanBuy (), activeGoods.getRoleCountCanBuy (), UserManager.Instance.self.getRMB () / activeGoods.getCostPrice ()),
				          1, activeGoods.getCostType (), buy); 
			});
	}

	public void buy (MessageHandle msg)
	{
		if (msg.msgEvent == msg_event.dialogCancel)
			return;
		BuyGoodsFPort fport = FPortManager.Instance.getFPort ("BuyGoodsFPort") as BuyGoodsFPort;
		fport.buyGoods (activeGoods.sid, msg.msgNum, (sid,num) => {
			UiManager.Instance.createMessageLintWindow (Language ("monthCardBuySuccessTip"));
			fatherContent.refreshContent ();
		});
	}

}
