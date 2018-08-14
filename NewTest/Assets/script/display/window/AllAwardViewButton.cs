using UnityEngine;
using System.Collections;

public class AllAwardViewButton : ButtonBase
{
	/** 奖励名字 */
	public UILabel prizeName;
	public RoleView roleButton;
	public GoodsView goodsButton;
	public	PrizeSample prize;
	private CallBack backClose;//需要隐藏的主窗口调用
	private CallBack backOpen;//需要重新打开的主窗口调用
	
	public void initPrize (PrizeSample _prize, WindowBase win, CallBack _back, CallBack _back2)
	{
		fatherWindow = win;
		this.backClose = _back;
		this.backOpen = _back2;
		setCreatButton (_prize);
	}
	
	public override void DoClickEvent ()
	{
		if (prize != null) {
			clickButton (prize);
		} else {
			MaskWindow.UnlockUI();
		}
	}
	
	public void referesh()
	{
		if (fatherWindow is AllAwardViewWindow)
		{
			AllAwardViewWindow win = fatherWindow as AllAwardViewWindow;
			win.refereshWin();
		}
	}
	
	//设置创建按钮信息
	private void setCreatButton (PrizeSample _prize)
	{
		if (_prize == null) {
			return;
		} else {
			prize = _prize;
			roleButton.gameObject.SetActive (false);
			goodsButton.gameObject.SetActive (false);
			goodsButton.fatherWindow = fatherWindow;

			switch (prize.type) {
			case PrizeType.PRIZE_BEAST:
				roleButton.gameObject.SetActive (true);
				Card beast = CardManagerment.Instance.createCard (prize.pSid);
				roleButton.init(beast,fatherWindow,roleButton.DefaultClickEvent);
				break;
			case PrizeType.PRIZE_CARD:
				goodsButton.gameObject.SetActive (true);
				Card card = CardManagerment.Instance.createCard (prize.pSid);
				goodsButton.onClickCallback = ()=>{CardBookWindow.Show (card, CardBookWindow.SHOW, null);fatherWindow.finishWindow();};
				goodsButton.init(card);
				break;
			case PrizeType.PRIZE_EQUIPMENT:
				goodsButton.gameObject.SetActive (true);
				Equip equip = EquipManagerment.Instance.createEquip (prize.pSid);
				goodsButton.init(equip,prize.getPrizeNumByInt ());
				break;
			case PrizeType.PRIZE_MONEY:
				goodsButton.gameObject.SetActive (true);
				goodsButton.init(prize);
				break;
			case PrizeType.PRIZE_PROP:
				goodsButton.gameObject.SetActive (true);
				Prop prop = PropManagerment.Instance.createProp (prize.pSid);
				goodsButton.init(prop,prize.getPrizeNumByInt ());
				break;
			case PrizeType.PRIZE_RMB:
				goodsButton.gameObject.SetActive (true);
				goodsButton.init(prize);
				break;
			}

			if (prizeName != null) prizeName.text = QualityManagerment.getQualityColor (prize.getQuality ()) + prize.getPrizeName ();
		}
	}
	
	//创建可以点击的按钮
	private void clickButton (PrizeSample prize)
	{
	//	referesh();
		switch (prize.type) {
		case PrizeType.PRIZE_BEAST:
			if (backClose != null)
				backClose ();
			Card beast = CardManagerment.Instance.createCard (prize.pSid);
			fatherWindow.finishWindow();
			EventDelegate.Add(fatherWindow.OnHide,()=>{	
				CardBookWindow.Show (beast, CardBookWindow.OTHER, backOpen);});
			break;
		case PrizeType.PRIZE_CARD:
			if (backClose != null)
				backClose ();
			Card card = CardManagerment.Instance.createCard (prize.pSid);
			fatherWindow.finishWindow();
			EventDelegate.Add(fatherWindow.OnHide,()=>{	
				CardBookWindow.Show (card, CardBookWindow.OTHER, backOpen);});
			break;
		case PrizeType.PRIZE_EQUIPMENT:
			if (backClose != null)
				backClose ();
			Equip equip = EquipManagerment.Instance.createEquip (prize.pSid);
			fatherWindow.finishWindow();
			EventDelegate.Add(fatherWindow.OnHide,()=>{	
				UiManager.Instance.openWindow <EquipAttrWindow>((win)=>{
					win.Initialize (equip, EquipAttrWindow.OTHER, backOpen);
				});});
			break;
		case PrizeType.PRIZE_MONEY:
			//暂时处理，有可能游戏币也显示详情
			MaskWindow.UnlockUI();
			break;
		case PrizeType.PRIZE_PROP:
			Prop prop = PropManagerment.Instance.createProp (prize.pSid);
			UiManager.Instance.openDialogWindow<PropAttrWindow>((win)=>{
				win.Initialize (prop);
			});
			break;
		case PrizeType.PRIZE_RMB:
			//暂时处理，有可能软妹币也显示详情
			MaskWindow.UnlockUI();
			break;
		}
	}
}