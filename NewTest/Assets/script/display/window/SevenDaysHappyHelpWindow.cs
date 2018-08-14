using UnityEngine;
using System.Collections.Generic;

public class SevenDaysHappyHelpWindow : WindowBase
{
	public RoleView card1;
	public RoleView card2;
	public RoleView card3;

	public UILabel cardName1;
	public UILabel cardName2;
	public UILabel cardName3;

	protected override void begin ()
	{
		base.begin ();
		MaskWindow.UnlockUI();
	}

	void closeWin(GameObject obj)
	{
		finishWindow ();
	}

	public void initWin(Card[] cards)
	{
		card1.init(cards[0],this,clickCard);
		card2.init(cards[1],this,clickCard);
		card3.init(cards[2],this,clickCard);

		cardName1.text = card1.card.getName();
		cardName2.text = card2.card.getName();
		cardName3.text = card3.card.getName();
	}

	public void clickCard(RoleView card)
	{
		OnItemClick(card.card);
	}

	public void OnItemClick (Card item) {
		MaskWindow.LockUI ();
		UiManager.Instance.openWindow<CardPictureWindow> (
			(window) => {
			int type = PictureManagerment.Instance.mapCard [item];
			window.init (PictureManagerment.Instance.mapType [type], 0);
			SevenDaysHappyManagement.Instance.setIsSevendayshappyHelpWin(true);
			SevenDaysHappyManagement.Instance.setHelpObj(this.gameObject);
			gameObject.SetActive(false);}
		);
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);

		if(gameObj.name == "button_1")
		{
			finishWindow ();
		}
	}

}
