using UnityEngine;
using System.Collections;

/**
 * 出售内容按钮
 * @author 汤琦
 * */
public class ButtonSellCard : ButtonBase
{
	public UITexture cardImage;
	public Card card;//关联的card对象
	public UISprite quality;//品质图标
	public UISprite sign;//风格图标 
	public UILabel level;//等级不解释
	public UISprite selectPic;//选中标志
	public bool isChoose;//是否选中;
	public SellWindow win;
	 	public override void DoLongPass ()
	{
		if (card == null)
			return;
		fatherWindow.finishWindow ();
		UiManager.Instance.openWindow<CardBookWindow>((win)=>{
			win.init (card, CardBookWindow.SHOW, openFatherWindow);
		});
	}

	void openFatherWindow ()
	{
		UiManager.Instance.openWindow<SellWindow> ();
	//	win.Initialize (false);
	}
	
	public  void initialize (Card _card)
	{
		win = fatherWindow as SellWindow;
		updateButton (_card);
	}
	public  void changeBlack ()
	{
		cardImage.color = Color.gray;
	}

	public  void changeLight ()
	{
		cardImage.color = Color.white;
	}
	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
		if (isChoose == false) {
			win.onSelectCard(card);
			selectOn ();
		} else {
			win.offSelectCard(card);
			selectOff ();
		}
		win.changeButton();
	}
	void selectOn ()
	{
		isChoose = true;
		setShower();
	}
	void selectOff ()
	{
		isChoose = false;
		cleanShower();
	}
	void setShower ()
	{
		selectPic.gameObject.SetActive(true);
	}
	void cleanShower ()
	{
		selectPic.gameObject.SetActive(false);
	}
	void cleanButton ()
	{
		cardImage.alpha = 0;
		cardImage.mainTexture = null;
		level.text = "";
		quality.alpha = 0;
		card = null;
		sign.alpha = 0;
		selectPic.gameObject.SetActive(false);
	
	}
	void resetCard()
	{
		cardImage.alpha=1;
		cardImage.color=Color.white;
	}
	public void updateButton (Card newCard)
	{
		if (newCard == null) {
			cleanButton ();
			return;
		} else {
			card = newCard;
			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.CARDIMAGEPATH + card.getImageID (), cardImage);
			quality.spriteName = QualityManagerment.qualityIDToString (card.getQualityId ());
			level.text = card.getLevel ().ToString();
			sign.spriteName = CardManagerment.Instance.jobIDToString (card.getJob ());
			resetCard();
			if(!win.isSelect(card))
			{
				isChoose = false;
				selectPic.gameObject.SetActive(false);
			}
			else
			{
				isChoose = true;
				selectPic.gameObject.SetActive(true);
			}
			
		}
	}
	
	
}
