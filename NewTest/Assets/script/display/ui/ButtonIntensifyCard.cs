using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ButtonIntensifyCard : ButtonBase
{
	
	public UITexture cardImage;
	public Card card;//关联的card对象
	public UISprite quality;//品质图标 
	public UISprite bian;
	public UISprite qualitytext;
	public UILabel level;//等级不解释
	public UISprite selectPic;//选中标志
	public UISprite state;//选中标志
	public UISprite inTeamSprite;//战斗中图标
	public UILabel evoLevel;
//	SacrificeShowerCtrl shower;
	public bool isChoose;//是否选中;
	public IntensifyCardChooseWindow win;
    public GameObject starsPrefab;//星星
//	private int clickCount = 0;
	private int selectType = 0;
	private int intoType = 0;

	public override void DoUpdate ()
	{
		if (evoLevel.gameObject.activeSelf == true) {
			evoLevel.alpha = sin ();
		}
	}
	public override void DoLongPass ()
	{
		if(!GuideManager.Instance.isGuideComplete())
			return;
		if (card == null)
			return;

		UiManager.Instance.openWindow<CardBookWindow> ((win)=>{
			IntensifyCardChooseWindow icwin = fatherWindow as IntensifyCardChooseWindow;
			ArrayList arrayList = icwin.content.getRoleList ();
			List<Card> list = new List<Card> (arrayList.Count);
			int index = 0;
			for(int i = 0; i < arrayList.Count; i++){
				if(arrayList[i] == card)
					index = i;
				list.Add((Card)arrayList[i]);
			}
			win.init (list,index,CardBookWindow.SHOW,openFatherWindow);
		});
	}

	private void openFatherWindow ()
	{
		fatherWindow.restoreWindow();
	}

	public  void changeBlack ()
	{
		cardImage.color = Color.gray;
	}

	public  void changeLight ()
	{
		cardImage.color = Color.white;
	}
	
	//初始化
	public void Initialize (Card _card ,int selectType,int intoType)
	{
		this.intoType = intoType;
		this.selectType = selectType;
		win = fatherWindow as IntensifyCardChooseWindow;
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
		//12006000、12007000选择献祭卡片1和2
		if (GuideManager.Instance.isEqualStep(12006000) || GuideManager.Instance.isEqualStep(12007000)) {
			GuideManager.Instance.doGuide();  
			GuideManager.Instance.guideEvent();
			StartCoroutine (Utils.DelayRun (()=>{
				MaskWindow.UnlockUI();
			},0.5f));
		} else {
			MaskWindow.UnlockUI();
		}

	}



	void selectOn ()
	{
		if (IntensifyCardManager.Instance.isHaveMainCard() && selectType == IntensifyCardManager.MAINCARDSELECT) {
			//进化者为空就选为进化者
			IntensifyCardManager.Instance.setMainCard(card);
			setShower (true);
			isChoose = true;
			return;
		} 
		
//		//主卡不能成为祭品啊~~~
//		if (ArmyManager.Instance.getAllArmyAlternateIds ().Contains (card.uid))
//			return;
		if(intoType == IntensifyCardManager.INTENSIFY_CARD_ADDON || intoType == IntensifyCardManager.INTENSIFY_CARD_SACRIFICE)
		{
			if (!IntensifyCardManager.Instance.isFoodFull() && selectType == IntensifyCardManager.FOODCARDSELECT) {
				if(isCanSelect())
				{   IntensifyCardManager.Instance.setFoodCard(card);
					setShower (true);
					isChoose = true;
				}		
			}
			return;
		}
        if (intoType == IntensifyCardManager.INTENSIFY_CARD_SUPRE_EVO && selectType == IntensifyCardManager.FOODCARDSELECT)
        {
            if (IntensifyCardManager.Instance.getIsCanGetSuperExoFood())
            {
				if(isCanSelect())
				{   IntensifyCardManager.Instance.setFoodCard(card);
					setShower (true);
					isChoose = true;
				}
            }
            return;
        }

		//进化者有了,并且祭品没满人，就选为祭品		
		if (!IntensifyCardManager.Instance.isEvoFoodFull() && selectType == IntensifyCardManager.FOODCARDSELECT) {
			if(isCanSelect())
			{   IntensifyCardManager.Instance.setFoodCard(card);
				setShower (true);
				isChoose = true;
			}	
		}
	}
		
	void selectOff ()
	{
		isChoose = false;
		if (IntensifyCardManager.Instance.compareMainCard(card) && selectType == IntensifyCardManager.MAINCARDSELECT) {
			IntensifyCardManager.Instance.removeMainCard();
		}
		//下面是祭品
		else if (IntensifyCardManager.Instance.isInFood(card) && selectType == IntensifyCardManager.FOODCARDSELECT)
		{
			IntensifyCardManager.Instance.removeFoodCard(card);
		}
		cleanShower ();
	}

	void cleanShower ()
	{
		selectPic.gameObject.SetActive (false);
	}
	public bool isCanSelect()
	{
		if(card.state==4&&intoType!=IntensifyCardManager.INTENSIFY_CARD_INHERIT)
		{
			MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("tips_001"));
			return false;
		}			
		else if(card.state==1&&intoType!=IntensifyCardManager.INTENSIFY_CARD_INHERIT)
		{
			MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("tips_002"));
			return false;
		}
		else if(card.state==5&&intoType!=IntensifyCardManager.INTENSIFY_CARD_INHERIT)
		{
			MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("tips_003"));
			return false;
		}
			return true;
	}
	void setShower (bool onOff )
	{
		if (!onOff) {
			selectPic.gameObject.SetActive (false);
		} else {
			selectPic.gameObject.SetActive (true);
		}
	}

	void cleanBtton ()
	{
		cardImage.gameObject.SetActive(false);
		cardImage.mainTexture = null;
		level.text = "";
		quality.gameObject.SetActive(false);
		qualitytext.gameObject.SetActive(false);
		inTeamSprite.gameObject.SetActive (false);
		bian.gameObject.SetActive(false);
		spriteBg.spriteName = "roleBack_0";
		card = null;
		//shower = null;
		selectPic.gameObject.SetActive (false);	
	}

	void resetCard ()
	{
		cardImage.gameObject.SetActive(false);
		cardImage.color = Color.white;
		selectPic.gameObject.SetActive (false);
		isChoose = false;
	}

	void showEvoLevel(){
		if (card != null) {
			if (card.getEvoLevel () > 0) {
				if(card.isMainCard()){
					if(card.getSurLevel() > 0){
					evoLevel.gameObject.SetActive (true);
						evoLevel.text = "[FF0000]+" + card.getSurLevel();
					}
					else
						evoLevel.gameObject.SetActive(false);
				}
				else{
					evoLevel.gameObject.SetActive (true);
					evoLevel.text = "[FF0000]+" + card.getEvoLevel ();
				}
			} else
				evoLevel.gameObject.SetActive (false);
		}
	}
    /// <summary>
    /// 显示卡片的星级
    /// </summary>
    public void showStar( Card card) {
        if (starsPrefab != null) {
            if (this.gameObject.transform.FindChild("starContent(Clone)") != null) {
                DestroyImmediate(this.gameObject.transform.FindChild("starContent(Clone)").gameObject);
            }
            if (card != null && this.gameObject.transform.FindChild("starContent(Clone)") == null && CardSampleManager.Instance.getStarLevel(card.sid) > 0) {
                GameObject star = NGUITools.AddChild(this.gameObject, starsPrefab);
                ShowStars show = star.GetComponent<ShowStars>();
                show.initStar(CardSampleManager.Instance.getStarLevel(card.sid), CardSampleManager.USEDBYINTENSIFY);
            } 
        }
    }
	public void updateButton (Card newCard)
	{
		if (newCard == null) {
			cleanBtton ();
			return;
		} else {
			resetCard ();
			card = newCard;
            showStar(card);
			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.CARDIMAGEPATH + card.getImageID () , cardImage);
			quality.gameObject.SetActive(true);
			quality.spriteName = QualityManagerment.qualityIconBgToBackGround (card.getQualityId ());
			spriteBg.spriteName = QualityManagerment.qualityIDToBackGround (card.getQualityId ());
			qualitytext.gameObject.SetActive(true);
			qualitytext.spriteName=CardManagerment.Instance.qualityIconTextToBackGround(card.getJob());
			inTeamSprite.gameObject.SetActive (card.isInTeam ());
			bian.gameObject.SetActive(true);
			bian.spriteName=QualityManagerment.qualityBianToBackGround(card.getQualityId());
			level.text ="Lv."+ card.getLevel ();
			showEvoLevel();

			//标识是否出战
			if (state != null) {
				if (ArmyManager.Instance.getAllArmyPlayersIds ().Contains (card.uid) || ArmyManager.Instance.getAllArmyAlternateIds ().Contains (card.uid)) {
					state.gameObject.SetActive (true);
				} else {
					state.gameObject.SetActive (false);
				}
			}

			//如果是主卡，打标识
			if (IntensifyCardManager.Instance.compareMainCard(card) && (fatherWindow as IntensifyCardChooseWindow).getSelectType()==IntensifyCardManager.MAINCARDSELECT) {
				setShower (true);
				isChoose = true;
				return;
			}
			if (IntensifyCardManager.Instance.isInFood(card) && (fatherWindow as IntensifyCardChooseWindow).getSelectType()==IntensifyCardManager.FOODCARDSELECT)
			{
				if(isCanSelect())
				{
					setShower (true);
					isChoose = true;
				}
				return;
			}
		}
	}
	
 
}
