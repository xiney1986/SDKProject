using UnityEngine;
using System.Collections;

public class ButtonSkillLearnCard : ButtonBase
{
	
	public UITexture cardImage;
	public Card card;//关联的card对象
	public UISprite quality;//品质图标
	public UISprite sign;//风格图标 
	public UILabel level;//等级不解释
	public UISprite selectPic;//选中标志
	public UISprite state;//选中标志	
	public UILabel attrAll;
	private SacrificeShowerCtrl shower;
	public bool isChoose;//是否选中;
	public LearningSkillWindow win;
	public const string MAINSELECT = "mainchoose";
	public const string CASTSELECT = "castChoose";
		void calculateAllAttr ()
	{
		int count = 0;
		//计算附加等级
		count += card.getHPGrade();
		count += card.getATTGrade();
		count += card.getDEFGrade();
		count += card.getMAGICGrade();
		count += card.getAGILEGrade();
		
		if (count > 0) {
			attrAll.gameObject.SetActive (true);
			attrAll.text = "+" + count;
		} else {
			attrAll.gameObject.SetActive (false);
		}
		
	}
		public override void DoUpdate ()
	{
		if (attrAll.gameObject.activeSelf == true) {
			attrAll.alpha =sin ();
		}
	}
		public override void DoLongPass ()
	{
		if (card == null)
			return;
//		fatherWindow.finishWindow ();
		UiManager.Instance.openWindow<CardBookWindow>((win)=>{
			win.init (card, CardBookWindow.SHOW, null);
		});
	}

	void openFatherWindow ()
	{
		UiManager.Instance.openWindow<LearningSkillWindow>((win)=>{
			win.Initialize (false);
		});
	}
	public  void changeBlack ()
	{
		cardImage.color = Color.gray;
		//	selectPic.color=Color.gray;
		//	sign.color=Color.gray;
		
	}

	public  void changeLight ()
	{
		cardImage.color = Color.white;
		//	selectPic.color=Color.gray;
		//	sign.color=Color.gray;
		
	}
	
	// 等你初始化诗句
	public  void Initialize (Card _card)
	{
		win = fatherWindow as LearningSkillWindow;
		updateButton (_card);

	}
	
	public override void DoClickEvent ()
	{
		
		base.DoClickEvent ();
		if (isChoose == false) {
			selectOn ();
		} else {
			selectOff ();
		}
		
		win.changeButton ();
		 
	}
	
	void selectOn ()
	{


		if (win.hasMainRole () == false) {
			//进化者为空就选为进化者
			setShower (win.mainRole, MAINSELECT);
			win.food.cleanData ();
			isChoose = true;
			win.reLoadMatchingCard ();
			return;
		} 
		
		//主卡不能成为祭品啊~~~
		if (UserManager.Instance.self.mainCardUid == card.uid) {
			return;
		}
		


		//进化者有了,并且祭品没满人，就选为祭品		
		if (win.hasMainRole () == true && win.isCasterEmpty () == true) {
			setShower (win.food, CASTSELECT);
			isChoose = true;	
		}
 
	}
		
	void selectOff ()
	{
		isChoose = false;
		if (card != null && win.mainRole .card != null && win.mainRole.card.uid == card.uid) {
			win.mainRole.cleanData ();
			win.showALLCard ();
			cleanShower ();
			
			//如果所有东西都没放，那么隐藏所有祭品底座
			if (win.isCasterEmpty ()) {
				win.food.cleanAll ();
			}
			return;
		}

		//下面是祭品
	
		if (win.food.card.uid == card.uid) {
			win.food.cleanData ();
		}	

		cleanShower ();
		
						
		if (win.isCasterEmpty ()) {
			//如果一个祭品都没放,禁止按钮
			//	win.enableButton ();
			if (win.mainRole .card == null) {
				//如果一个祭品都没放，并且没主角。那么隐藏所有祭品底座
				win.hideAll ();
			}
		}
		
	}

	void cleanShower ()
	{
		shower = null;
		selectPic.alpha = 0;
	}
	
	void setShower (SacrificeShowerCtrl _shower, string spName)
	{

		shower = _shower;
		if (_shower != null)
			_shower.updateShower (card);
		if (spName == "") {
			selectPic.alpha = 0;
		} else {
			selectPic.alpha = 1;
			selectPic.spriteName = spName;
		}
	}

	void cleanBtton ()
	{
		cardImage.alpha = 0;
		cardImage.mainTexture = null;
		level.text = "";
		quality.alpha = 0;
		card = null;
		sign.alpha = 0;
		shower = null;
		selectPic.alpha = 0;
		
	}

	void resetCard ()
	{
		cardImage.alpha = 1;
		cardImage.color = Color.white;
		selectPic.alpha = 0;
		isChoose = false;
		
	}

	public void updateButton (Card newCard)
	{
		
		
		if (newCard == null) {
			cleanBtton ();
			return;
		} else {

			card = newCard;
			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.CARDIMAGEPATH + card.getImageID (), cardImage);
			quality.spriteName =QualityManagerment.qualityIDToString (card.getQualityId ());
			level.text = card.getLevel () + "";
			sign.spriteName = CardManagerment.Instance.jobIDToString (card.getJob ());
			resetCard ();
			 calculateAllAttr();
			
			if (state != null) {
				if (ArmyManager.Instance.getAllArmyPlayersIds ().Contains (card.uid) || ArmyManager.Instance.getAllArmyAlternateIds().Contains (card.uid)) {
					state.gameObject.SetActive (true);
					state.spriteName = "inTeam";
					state.width = 33;
					state.height = 53;
				}else{
					state.gameObject.SetActive (false);
				}
			}
			if (card != null && win.mainRole .card != null && card.uid == win.mainRole.card.uid) {
				setShower (win.mainRole, MAINSELECT);
				win.food.cleanData ();
				isChoose = true;
				return;
			}
			
			if (win.food.card != null && win.food.card.uid == card.uid) {
				setShower (win.food, CASTSELECT);
				isChoose = true;
				return;
			}

		}
	}
	
 
}
