using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ButtonTeamPlayerViewInMission : ButtonBase
{
	public UILabel evoLevel;//进化等级
	public UITexture cardImage;
	public BattleFormationCard card;//关联的card对象
	public UISprite qualityBg; //品质背景
	public UISprite qualitytopbian;
	public UISprite qualitytopIcon;
	public UISprite qualitybian;
	public UILabel level;//等级不解释
	public HpbarCtrl hpBar;
	public bool IsAlternate;	//是否替补
	public BattleFormationCard brotherCard;//关联卡片,正式卡关联他的替补卡,替补卡关联着他的正式卡
	public int showType;//显示模式
	public const int TEAMVIEWINMISSION = 0;//更换替补的窗口下用
	public const int BATTLEPREPARE = 1;//战前准备窗口下用
	public const int PVPINTO = 2;
	public bool ignoreClick = false;//编辑器中设定:敌人true不能点,我方false能点
	public bool isBoss = false;//是否为boss
	int index;
    BattlePrepareWindowNew win;
	//BattlePrepareWindow win;
	List<int> idss;

	public override void DoLongPass ()
	{
		if (showType == PVPINTO || card == null || GuideManager.Instance.isLessThanStep (GuideGlobal.NEWOVERSID) || isBoss) {
			MaskWindow.UnlockUI ();
			return;
		}

		if (CardManagerment.Instance.isMyCard (card.card)) {
			CardBookWindow.Show (card.card, CardBookWindow.OTHER, null);
		}
	}

	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
		if (ignoreClick)
			return;
		if (card == null && brotherCard == null)
			return;
		//新手引导不能点
		if (GuideManager.Instance.isLessThanStep (GuideGlobal.NEWOVERSID)) {
			MaskWindow.UnlockUI ();
			return;
		}
		if (isBoss) {
			MaskWindow.UnlockUI ();
			return;
		}

		UiManager.Instance.openWindow<ChangeAlternateWindow> (
			(win) => {
			win.SetFatherWindow (fatherWindow);
			if (!IsAlternate)
				win.Initialize (card, brotherCard, index,idss);
			else
				win.Initialize (brotherCard, card, index,idss);

		});
	}

	public void cleanButton ()
	{
		gameObject.SetActive (true);
		level.gameObject.SetActive (false);
		hpBar.gameObject.SetActive (false);
		cardImage.gameObject.SetActive (false);
		qualitytopbian.gameObject.SetActive(false);
		qualitytopIcon.gameObject.SetActive(false);
		level.text = "";
		card = null;
	}

	public void updateButton (BattleFormationCard newCard, BattleFormationCard brother, int type, int teamIndex,bool isAlter)
	{
//		if (newCard == null)
//			return;
		IsAlternate=isAlter;
		index = teamIndex;
		showType = type;
		card = newCard;
        win = fatherWindow as BattlePrepareWindowNew;
		//win = fatherWindow as BattlePrepareWindow;
		idss = win.idss;
		brotherCard = brother;
        if (newCard != null) {
            showStar(card.card);
        }
		if(newCard==null){
			if(qualitytopbian!=null)qualitytopbian.gameObject.SetActive(false);
			if(qualitytopIcon!=null)qualitytopIcon.gameObject.SetActive(false);

		}else{
			if(qualitytopbian!=null)qualitytopbian.gameObject.SetActive(true);
			if(qualitytopIcon!=null)qualitytopIcon.gameObject.SetActive(true);
		}
		if (newCard != null && qualityBg != null) {
			qualityBg.spriteName = QualityManagerment.qualityIDToBackGround (card.card.getQualityId ());
		} else {
			if (qualityBg != null) {
				qualityBg.spriteName = QualityManagerment.qualityIDToBackGround (1);
			}
		}
		if(newCard!=null&&qualitybian!=null){
			qualitybian.spriteName=QualityManagerment.qualityBianToBackGround(card.card.getQualityId());
		}else{
			if(qualitybian!=null){
				qualitybian.spriteName=QualityManagerment.qualityBianToBackGround(1);
			}
		}
		if (evoLevel != null)
			evoLevel.gameObject.SetActive (false);

		if (newCard == null) {
			cleanButton ();
			return;
		} else {

			if (newCard.card == null) {
				cleanButton ();
				return;
			}		
	
			//todo hpbar
			hpBar.gameObject.SetActive (true);
			hpBar.updateValue (newCard.getHp (), newCard.getHpMax ());
			gameObject.SetActive (true);
			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.CARDIMAGEPATH + card.card.getImageID (), cardImage);
			if (this.spriteBg != null)
				this.spriteBg.spriteName = QualityManagerment.qualityIDToBackGround (card.card.getQualityId ());
			if (!isBoss) {
				level.gameObject.SetActive (true);
				level.text = "Lv." + card.getLevel ();
			} else {
				//如果是BOSS，就把等级当成血量去显示
				level.text = card.getHp () + "/" + card.getHpMax ();
			}
			if(newCard!=null&&qualitytopbian!=null){
				qualitytopbian.spriteName=QualityManagerment.qualityIconBgToBackGround(card.card.getQualityId());
			}
			if(newCard!=null&&qualitytopIcon!=null){
				qualitytopIcon.spriteName=CardManagerment.Instance.qualityIconTextToBackGround(card.card.getJob());
			}
			updateEvoLevel ();
		}
	}
    /// <summary>
    /// 显示星星
    /// </summary>
    /// <param name="card"></param>
    void showStar(Card card) {
        if (win.starPrefab != null && this.gameObject.transform.FindChild("starContent(Clone)") == null && CardSampleManager.Instance.getStarLevel(card.sid) > 0) {
            GameObject star = NGUITools.AddChild(this.gameObject, win.starPrefab);
            ShowStars show = star.GetComponent<ShowStars>();
            show.initStar(CardSampleManager.Instance.getStarLevel(card.sid), CardSampleManager.USEDINBATTLEPREPARE);
        }
    }
	public void updateEvoLevel ()
	{
		if (evoLevel != null && card != null && card.card != null) {
			if (card.card.getEvoLevel () > 0) {
				if(card.card.isMainCard()){
					if(card.card.getSurLevel() > 0){
						evoLevel.gameObject.SetActive(true);
						evoLevel.text = "[FF0000]+" + card.card.getSurLevel().ToString();
					}
					else
						evoLevel.gameObject.SetActive(false);
				}
				else{
				evoLevel.gameObject.SetActive (true);
				evoLevel.text = "[FF0000]+" + card.card.getEvoLevel ();
				}
			} else
				evoLevel.gameObject.SetActive (false);
		}
	}

	public override void DoUpdate ()
	{
		if (evoLevel != null && evoLevel.gameObject != null && evoLevel.gameObject.activeSelf)
			evoLevel.alpha = sin ();
	}
}
