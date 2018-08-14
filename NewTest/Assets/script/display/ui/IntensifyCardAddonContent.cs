using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IntensifyCardAddonContent : MonoBase
{ 
	public SacrificeRotCtrl sacrificeRotCtrl;//旋转控制器，容器都在上面哦
	public ButtonBase buttonMain;
	public UILabel title;
	public UILabel combatValue;
	public UILabel hpLabel; //生命值
	public UILabel hpAddValue; //附加生命值
	public UILabel hpLvLabel; //生命等级标签
	public UILabel hpLvValue; //生命等级

	public UILabel attLabel; //攻击值
	public UILabel attAddValue; //附加攻击值
	public UILabel attLvLabel; //攻击等级标签
	public UILabel attLvValue; //攻击等级

	public UILabel defLabel; //防御值
	public UILabel defAddValue; //附加防御值
	public UILabel defLvLabel; //防御等级标签
	public UILabel defLvValue; //防御等级

	public UILabel magLabel; //魔法值
	public UILabel magAddValue; //附加魔法值
	public UILabel magLvLabel; //魔法等级标签
	public UILabel magLvValue; //魔法等级

	public UILabel dexLabel; //敏捷值
	public UILabel dexAddValue; //附加敏捷值
	public UILabel dexLvLabel; //敏捷等级标签
	public UILabel dexLvValue; //敏捷等级

	/** 附加属性信息对象 */
	AttrAddInfo attrAddInfo = new AttrAddInfo ();
	private int oldCombat = 0;//初始战斗力
	private int newCombat = 0;//最新战斗力
	private int step;//步进跳跃值
	private float time; //属性加成切换显示的时间
	public const float SWITCHTIME = 3f;
	private IntensifyCardWindow win;
	/** 属性经验条 */
	public IncbarCtrl hpIncbarCtrl;
	public IncbarCtrl attIncbarCtrl;
	public IncbarCtrl defIncbarCtrl;
	public IncbarCtrl magIncbarCtrl;
	public IncbarCtrl dexIncbarCtrl;
	/** 附加属性信息时间 */
	float nextTime;
	/** 交替次数 */
	int nextCount;
	/** 是否有已强化满级的 */
	string isHaveMaxLv = null;
	/** 精灵按钮 */
	public ButtonSpriteAdd[] buttonSprites;
	private string[] iconArr = new string[] {
		"21010000",
		"48010000",
		"9010000",
		"30010000",
		"40010000"
	};
	private int[] spriteSidArr = new int []{11423,11424,11425,11426,11427};
	
	void Update ()
	{
		if (sacrificeRotCtrl.mainShower.card != null) {
			time -= Time.deltaTime;
			if (time <= 0) {
				time = SWITCHTIME;
				updateInfo ();
			}
		}
		if (CombatManager.Instance.getIsRefreshCombat ()) {
			combatValue.text = CombatManager.Instance.getRefreshCombat ();
		}
		float alphaValue = sin ();
		updateAttrInfoAlpha (alphaValue);
	}
	//刷新附加精灵图
	public void updateButtonSprites ()
	{
		for (int i=0; i<buttonSprites.Length; i++) {

			buttonSprites [i].initUI (iconArr [i], spriteSidArr [i]);
			buttonSprites [i].onClickEvent = buttonbaseEvent;
			updateCount (buttonSprites [i]);
			
		}
	}

	public void buttonbaseEvent (GameObject gameObj)
	{
		int count = 1;

		ButtonSpriteAdd bsa = gameObj.GetComponent<ButtonSpriteAdd> ();
		if (bsa.getCount () < count) {
			showMessageLineWindow ();
			return;
		}
		playAimi (bsa, () => {
			updateCount (bsa);
		});
		MaskWindow.UnlockUI ();
	}


	/// <summary>
	/// 更新附加精灵数量
	/// </summary>
	private void updateCount (ButtonSpriteAdd bsa)
	{
		int useCount = IntensifyCardManager.Instance.getFoodCardNum (bsa.spriteSid);
		bsa.setCount (bsa.spriteList.Count - useCount);
		bsa.updateCount ();			
	}

	/// <summary>
	/// Shows the message line window.
	/// </summary>
	private void showMessageLineWindow ()
	{
        UiManager.Instance.openDialogWindow<MessageWindow>((MessageWindow win) =>
        {
            win.dialogCloseUnlockUI = false;
            string msg = LanguageConfigManager.Instance.getLanguage("messageLine_01");
            win.initWindow(2, LanguageConfigManager.Instance.getLanguage("s0094"), LanguageConfigManager.Instance.getLanguage("s0093"), msg, (eventMsg) => {
                if (eventMsg.buttonID == MessageHandle.BUTTON_RIGHT) {
                    
                    UiManager.Instance.openWindow<NoticeWindow>((winnn) => {
                        winnn.entranceId = NoticeSampleManager.Instance.getNoticeSampleBySid(NoticeType.GODDNESS_SHAKE_SID).entranceId;
                        winnn.updateSelectButton(NoticeType.GODDNESS_SHAKE_SID);
                    });
                } else {
                    MaskWindow.UnlockUI();
                }
            });
        });
	}
	/// <summary>
	/// 选择精灵动画
	/// </summary>
	private void playAimi (ButtonSpriteAdd bsa, CallBack callback)
	{
		ArrayList storgeSpriteList;
		storgeSpriteList = bsa.spriteList;
		Card card;
		for (int i=0; i<storgeSpriteList.Count; i++) {
			card = storgeSpriteList [i] as Card;
			if (sacrificeRotCtrl.isOneOfTheCaster (card))
				continue;
			SacrificeShowerCtrl ctrl = sacrificeRotCtrl.selectOneEmptyCastShower ();
		
			if (ctrl == null)
				break;
			if (!sacrificeRotCtrl.isCasterFull ()) {
				ctrl.updateShower (card);
				IntensifyCardManager.Instance.setFoodCard (card);
				if (callback != null) {
					callback ();
				}
				win.updateInfo ();
				break;
			}
		}
	}

	private void updateAttrInfoAlpha (float alphaValue)
	{
		// hp
		hpAddValue.alpha = alphaValue;
		hpIncbarCtrl.incBar.SliderBar.alpha = alphaValue;
		// att
		attAddValue.alpha = alphaValue;
		attIncbarCtrl.incBar.SliderBar.alpha = alphaValue;
		// def
		defAddValue.alpha = alphaValue;
		defIncbarCtrl.incBar.SliderBar.alpha = alphaValue;
		// mag
		magAddValue.alpha = alphaValue;
		magIncbarCtrl.incBar.SliderBar.alpha = alphaValue;
		// dex
		dexAddValue.alpha = alphaValue;
		dexIncbarCtrl.incBar.SliderBar.alpha = alphaValue;
		if (attrAddInfo.HpGrade > 0)
			hpLvValue.alpha = alphaValue;
		else
			hpLvValue.alpha = 1;
		if (attrAddInfo.AttGrade > 0)
			attLvValue.alpha = alphaValue;
		else
			attLvValue.alpha = 1;
		if (attrAddInfo.DefGrade > 0)
			defLvValue.alpha = alphaValue;
		else
			defLvValue.alpha = 1;
		if (attrAddInfo.MagGrade > 0)
			magLvValue.alpha = alphaValue;
		else
			magLvValue.alpha = 1;
		if (attrAddInfo.DexGrade > 0)
			dexLvValue.alpha = alphaValue;
		else
			dexLvValue.alpha = 1;
	}
	
	public void initInfo (IntensifyCardWindow win)
	{
		this.win = win;
		//初始化附加属性精灵
		updateButtonSprites ();
	}

	public void updateCtrl ()
	{
		if (!IntensifyCardManager.Instance.isHaveMainCard ()) {
			buttonMain.gameObject.SetActive (false);
			sacrificeRotCtrl.mainShower.updateShower (IntensifyCardManager.Instance.getMainCard ());
		} else {
			buttonMain.gameObject.SetActive (true);
			sacrificeRotCtrl.mainShower.cleanData ();
		}
		List<Card> list = IntensifyCardManager.Instance.getFoodCard ();
		sacrificeRotCtrl.refreshShowerCtrl (list);
		if (!IntensifyCardManager.Instance.isHaveFood ()) {
			if (list == null)
				return;
			for (int i = 0; i < list.Count; i++) {
				if (sacrificeRotCtrl.isOneOfTheCaster (list [i]))
					continue;
				sacrificeRotCtrl.selectOneEmptyCastShower ().updateShower (list [i]);
			}
		}
	}

	private void gotoHappy (MessageHandle msg)
	{
		if (msg.buttonID == MessageHandle.BUTTON_LEFT) {
			UiManager.Instance.openWindow<NoticeWindow> ((winnn) => {
				winnn.entranceId = NoticeSampleManager.Instance.getNoticeSampleBySid (NoticeType.GODDNESS_SHAKE_SID).entranceId;
				winnn.updateSelectButton (NoticeType.GODDNESS_SHAKE_SID);
			});
		} else {
			MaskWindow.UnlockUI ();
		}
	}
	/** 刷新控制台 */
	private void refreshSacrificeRotCtrl ()
	{

	}

	public void oneKey ()
	{
		List<Card> list = IntensifyCardManager.Instance.getOneKeyAddon ();
		if (list == null || list.Count == 0) {
			//UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage("Intensify6"));
			UiManager.Instance.openDialogWindow<MessageWindow> ((winn) => {
				winn.dialogCloseUnlockUI = false;
				winn.initWindow (2, LanguageConfigManager.Instance.getLanguage ("go_to_happy"), LanguageConfigManager.Instance.getLanguage ("s0094"), LanguageConfigManager.Instance.getLanguage ("go_to_happy1"), gotoHappy);
			});
			return;
		}
		for (int i = 0; i < list.Count; i++) {
			if (sacrificeRotCtrl.isOneOfTheCaster (list [i]))
				continue;
			SacrificeShowerCtrl ctrl = sacrificeRotCtrl.selectOneEmptyCastShower ();

			if (ctrl == null)
				break;

			ctrl.updateShower (list [i]);
			IntensifyCardManager.Instance.setFoodCard (list [i]);
		}
		win.updateInfo ();
	}

	public void clickUpdateCtrl ()
	{
		if (!IntensifyCardManager.Instance.isHaveMainCard ()) {
			buttonMain.gameObject.SetActive (false);
		} else {
			buttonMain.gameObject.SetActive (true);
		}
	}
	
	//刷新战斗力
	public void rushCombat ()
	{
		if (sacrificeRotCtrl.mainShower.card != null)
			newCombat = CombatManager.Instance.getCardCombat (sacrificeRotCtrl.mainShower.card);
		else {
			combatValue.text = "0";
			oldCombat = 0;
			return;
		}
		CombatManager.Instance.setCombatStep (newCombat, oldCombat);
	}

	public void intensify ()
	{
		Card tmpMainCard = IntensifyCardManager.Instance.getMainCard ();
		AttrAddInfo tempAttrAddinfo = new AttrAddInfo ();
		tempAttrAddinfo.clear ();
		List<Card> tmpFoodCards = IntensifyCardManager.Instance.getFoodCard ();
		if (tmpFoodCards != null) {
			foreach (Card item in tmpFoodCards) {
				//计算附加经验
				tempAttrAddinfo.HpExp += item.getHPExp ();
				tempAttrAddinfo.AttExp += item.getATTExp ();
				tempAttrAddinfo.DefExp += item.getDEFExp ();
				tempAttrAddinfo.MagExp += item.getMAGICExp ();
				tempAttrAddinfo.DexExp += item.getAGILEExp ();
			}
		}
		if (tmpMainCard != null) {
			isHaveMaxLv = null;
			int maxlv = EXPSampleManager.Instance.getMaxLevel (EXPSampleManager.SID_USER_ATTR_ADD_EXP) - 1;
			if (tempAttrAddinfo.HpExp > 0 && tmpMainCard.getHPExp () >= EXPSampleManager.Instance.getMaxExp (EXPSampleManager.SID_USER_ATTR_ADD_EXP)) {
				isHaveMaxLv = Language ("Intensify_Addon_Err01", maxlv.ToString ());
			} else if (tempAttrAddinfo.AttExp > 0 && tmpMainCard.getATTExp () >= EXPSampleManager.Instance.getMaxExp (EXPSampleManager.SID_USER_ATTR_ADD_EXP)) {
				isHaveMaxLv = Language ("Intensify_Addon_Err02", maxlv.ToString ());
			} else if (tempAttrAddinfo.DefExp > 0 && tmpMainCard.getDEFExp () >= EXPSampleManager.Instance.getMaxExp (EXPSampleManager.SID_USER_ATTR_ADD_EXP)) {
				isHaveMaxLv = Language ("Intensify_Addon_Err03", maxlv.ToString ());
			} else if (tempAttrAddinfo.MagExp > 0 && tmpMainCard.getMAGICExp () >= EXPSampleManager.Instance.getMaxExp (EXPSampleManager.SID_USER_ATTR_ADD_EXP)) {
				isHaveMaxLv = Language ("Intensify_Addon_Err04", maxlv.ToString ());
			} else if (tempAttrAddinfo.DexExp > 0 && tmpMainCard.getAGILEExp () >= EXPSampleManager.Instance.getMaxExp (EXPSampleManager.SID_USER_ATTR_ADD_EXP)) {
				isHaveMaxLv = Language ("Intensify_Addon_Err05", maxlv.ToString ());
			}
		} else {
			MaskWindow.UnlockUI ();
			return;
		}
		CallBack cb = () => {
			IntensifyCardManager.Instance.addOnRestorePrize = null;
			win.intensifyButton.disableButton (true);
			win.selectFoodButton.disableButton (true);
			win.oneKeyButton.disableButton (true);
			UiManager.Instance.applyMask ();
			AttributeFPort aas = FPortManager.Instance.getFPort ("AttributeFPort") as AttributeFPort;
			aas.access (sacrificeRotCtrl.mainShower.card.uid, sacrificeRotCtrl.createFoodList (), addonFinish);//(第一个参数,主卡uid,第二个参数,食物卡uid,第三个参数回调)
		};

		if (!string.IsNullOrEmpty (isHaveMaxLv)) {
			UiManager.Instance.createMessageWindowByTwoButton (isHaveMaxLv, (MessageHandle msg) => {
				if (msg.buttonID == MessageHandle.BUTTON_LEFT) {
					MaskWindow.UnlockUI ();
					return;
				}
				cb ();
			});
		} else {
			cb ();
		}
	}

	void addonFinish (string i)
	{
		computeRestoreCardPrize ();
		StartCoroutine (showEffect (i));
	}

	void computeRestoreCardPrize ()
	{
		List<Card> foodCards = IntensifyCardManager.Instance.getFoodCard ();
		if (foodCards == null)
			return;
		List<PrizeSample> prizeSamples = new List<PrizeSample> ();
		List<PrizeSample> prizes;
		foreach (Card card in foodCards) {
			prizes = card.computeRestoreCardPrize ();
			if (prizes.Count > 0)
				ListKit.AddRange (prizeSamples, prizes);
		}
		if (prizeSamples.Count > 0)
			IntensifyCardManager.Instance.addOnRestorePrize = prizeSamples;
	}

	IEnumerator showEffect (string index)
	{
		UiManager.Instance.applyMask ();	
		int count = 0;
		foreach (SacrificeShowerCtrl each in sacrificeRotCtrl.castShowers) {
			if (each.card != null) {
				yield return new WaitForSeconds (Random.Range (0.04f, 0.2f));
				each.cleanData ();
				count += 1;
				EffectManager.Instance.CreateEffectCtrlByCache (transform, "Effect/UiEffect/Reinforced_SyntheticONE", (obj,ctrl) => {
					ctrl.transform.position = each.transform.position;
				});
			}
		}
		yield return new WaitForSeconds (0.2f);
		EffectManager.Instance.CreateEffectCtrlByCache (transform, "Effect/UiEffect/Reinforced_SyntheticTwo", (obj,bigCtrl) => {
			bigCtrl.transform.position = sacrificeRotCtrl.mainShower.transform.position;
		});	
		for (int i=0; i<count; i++) {
			yield return new WaitForSeconds (Random.Range (0.04f, 0.1f));
			EffectManager.Instance.CreateEffectCtrlByCache (transform, "Effect/UiEffect/Reinforced_SyntheticThree", (obj,ctrl) => {
				ctrl.transform.position = new Vector3 (sacrificeRotCtrl.mainShower.transform.position.x + Random.Range (-1f, 1f), sacrificeRotCtrl.mainShower.transform.position.y + 1f, 0);
				iTween.MoveTo (ctrl.gameObject, iTween.Hash ("position", sacrificeRotCtrl.mainShower.transform.position, "easetype", iTween.EaseType.easeOutCubic, "time", 0.3f));
			});	
		}
		yield return new WaitForSeconds (2f);
		UiManager.Instance.cancelMask ();

		sacrificeRotCtrl.mainShower.card = StorageManagerment.Instance.getRole (sacrificeRotCtrl.mainShower.card.uid);
		UiManager.Instance.openWindow<CardAttrLevelUpWindow> ((cardWin) => {
			cardWin.Initialize (sacrificeRotCtrl.mainShower.card, attrAddInfo, showAddOnView); 
		});

		IntensifyCardManager.Instance.clearData ();
		sacrificeRotCtrl.hideCastShowerbase ();
	}

	private void showAddOnView ()
	{
		if (win != null) {
			win.updateInfo ();
		}
	}

	public void clearShowInfo ()
	{
		hpLabel.text = "";
		hpAddValue.text = "";
		hpLvLabel.text = "";
		hpLvValue.text = "";	
		attLabel.text = "";
		attAddValue.text = "";
		attLvLabel.text = "";
		attLvValue.text = "";
		defLabel.text = "";
		defAddValue.text = "";
		defLvLabel.text = "";
		defLvValue.text = "";
		magLabel.text = "";
		magAddValue.text = "";
		magLvLabel.text = "";
		magLvValue.text = "";
		dexLabel.text = "";
		dexAddValue.text = "";
		dexLvLabel.text = "";
		dexLvValue.text = "";
		updateAttrInfoAlpha (1);
		isHaveMaxLv = null;
	}

	public void updateInfo ()
	{
		clearShowInfo ();
		title.text = LanguageConfigManager.Instance.getLanguage ("s0139") + ":";
		// hp
		int oldHpAttr = CardManagerment.Instance.getCardAppendAttr (attrAddInfo.OldHpGrade, AttributeType.hp);
		int hpAttr = CardManagerment.Instance.getCardAppendAttr (attrAddInfo.HpGrade, AttributeType.hp);
		hpLabel.text = oldHpAttr.ToString ();
		if (hpAttr > 0)
			hpAddValue.text = " + " + hpAttr;
		hpLvLabel.text = "Lv+";
		hpLvValue.text = attrAddInfo.HpGrade.ToString ();
		// att
		int oldAttAttr = CardManagerment.Instance.getCardAppendAttr (attrAddInfo.OldAttGrade, AttributeType.attack);
		int attAttr = CardManagerment.Instance.getCardAppendAttr (attrAddInfo.AttGrade, AttributeType.attack);
		attLabel.text = oldAttAttr.ToString ();
		if (attAttr > 0)
			attAddValue.text = " + " + attAttr;
		attLvLabel.text = "Lv+";
		attLvValue.text = attrAddInfo.AttGrade.ToString ();
		// def
		int oldDefAttr = CardManagerment.Instance.getCardAppendAttr (attrAddInfo.OldDefGrade, AttributeType.defecse);
		int defAttr = CardManagerment.Instance.getCardAppendAttr (attrAddInfo.DefGrade, AttributeType.defecse);
		defLabel.text = oldDefAttr.ToString ();
		if (defAttr > 0)
			defAddValue.text = " + " + defAttr;
		defLvLabel.text = "Lv+";
		defLvValue.text = attrAddInfo.DefGrade.ToString ();
		// mag
		int oldMagAttr = CardManagerment.Instance.getCardAppendAttr (attrAddInfo.OldMagGrade, AttributeType.magic);
		int magAttr = CardManagerment.Instance.getCardAppendAttr (attrAddInfo.MagGrade, AttributeType.magic);
		magLabel.text = oldMagAttr.ToString ();
		if (magAttr > 0)
			magAddValue.text = " + " + magAttr;
		magLvLabel.text = "Lv+";
		magLvValue.text = attrAddInfo.MagGrade.ToString ();
		// dex
		int oldDexAttr = CardManagerment.Instance.getCardAppendAttr (attrAddInfo.OldDexGrade, AttributeType.agile);
		int dexAttr = CardManagerment.Instance.getCardAppendAttr (attrAddInfo.DexGrade, AttributeType.agile);
		dexLabel.text = oldDexAttr.ToString ();
		if (dexAttr > 0)
			dexAddValue.text = " + " + dexAttr;
		dexLvLabel.text = "Lv+";
		dexLvValue.text = attrAddInfo.DexGrade.ToString ();
	}
	
	public void recalculateAddonNumber ()
	{
		attrAddInfo.clear ();
		List<Card> cards = IntensifyCardManager.Instance.getFoodCard ();
		if (cards != null) {
			foreach (Card item in cards) {
				//计算附加经验
				attrAddInfo.HpExp += item.getHPExp ();
				attrAddInfo.AttExp += item.getATTExp ();
				attrAddInfo.DefExp += item.getDEFExp ();
				attrAddInfo.MagExp += item.getMAGICExp ();
				attrAddInfo.DexExp += item.getAGILEExp ();
			}
		}

		//附加属性统计
		Card mainCard = IntensifyCardManager.Instance.getMainCard ();
		if (mainCard != null) {
			// 附加前的等级
			attrAddInfo.OldHpGrade = mainCard.getHPGrade ();
			attrAddInfo.OldAttGrade = mainCard.getATTGrade ();
			attrAddInfo.OldDefGrade = mainCard.getDEFGrade ();
			attrAddInfo.OldMagGrade = mainCard.getMAGICGrade ();
			attrAddInfo.OldDexGrade = mainCard.getAGILEGrade ();
			attrAddInfo.oldHpExp = mainCard.getHPExp ();
			attrAddInfo.oldAttExp = mainCard.getATTExp ();
			attrAddInfo.oldDefExp = mainCard.getDEFExp ();
			attrAddInfo.oldMagExp = mainCard.getMAGICExp ();
			attrAddInfo.oldDexExp = mainCard.getAGILEExp ();

			// 附加后的等级
			int tmpHpGrade = Card.getAttrAddGrade (attrAddInfo.HpExp + mainCard.getHPExp ());
			int tmpAttGrade = Card.getAttrAddGrade (attrAddInfo.AttExp + mainCard.getATTExp ());
			int tmpDefGrade = Card.getAttrAddGrade (attrAddInfo.DefExp + mainCard.getDEFExp ());
			int tmpMagGrade = Card.getAttrAddGrade (attrAddInfo.MagExp + mainCard.getMAGICExp ());
			int tmpDexGrade = Card.getAttrAddGrade (attrAddInfo.DexExp + mainCard.getAGILEExp ());
			// 增加的附加等级
			attrAddInfo.HpGrade = tmpHpGrade - attrAddInfo.OldHpGrade;
			attrAddInfo.AttGrade = tmpAttGrade - attrAddInfo.OldAttGrade;
			attrAddInfo.DefGrade = tmpDefGrade - attrAddInfo.OldDefGrade;
			attrAddInfo.MagGrade = tmpMagGrade - attrAddInfo.OldMagGrade;
			attrAddInfo.DexGrade = tmpDexGrade - attrAddInfo.OldDexGrade;
			setIncBarActive (true);
			// 更新滚动条
			UpdateHpIncbarCtrl ();
			UpdateAttIncbarCtrl ();
			UpdateDefIncbarCtrl ();
			UpdateMagIncbarCtrl ();
			UpdateDexIncbarCtrl ();
		} else {
			setIncBarActive (false);
		}
		updateInfo ();
	}

	void setIncBarActive (bool isActive)
	{
		hpIncbarCtrl.gameObject.SetActive (isActive);
		attIncbarCtrl.gameObject.SetActive (isActive);
		defIncbarCtrl.gameObject.SetActive (isActive);
		magIncbarCtrl.gameObject.SetActive (isActive);
		dexIncbarCtrl.gameObject.SetActive (isActive);
	}

	/** 更新血量经验条 */
	void UpdateHpIncbarCtrl ()
	{
		int hpUpExp = (int)EXPSampleManager.Instance.getMaxEXPShow (EXPSampleManager.SID_USER_ATTR_ADD_EXP, attrAddInfo.oldHpExp);
		int hpDownExp = (int)EXPSampleManager.Instance.getNowEXPShow (EXPSampleManager.SID_USER_ATTR_ADD_EXP, attrAddInfo.oldHpExp);
		hpIncbarCtrl.updateValue (hpDownExp, attrAddInfo.HpExp, hpUpExp);

	}
	/** 更新攻击经验条 */
	void UpdateAttIncbarCtrl ()
	{
		int attUpExp = (int)EXPSampleManager.Instance.getMaxEXPShow (EXPSampleManager.SID_USER_ATTR_ADD_EXP, attrAddInfo.oldAttExp);
		int attDownExp = (int)EXPSampleManager.Instance.getNowEXPShow (EXPSampleManager.SID_USER_ATTR_ADD_EXP, attrAddInfo.oldAttExp);
		attIncbarCtrl.updateValue (attDownExp, attrAddInfo.AttExp, attUpExp);
	}

	/** 更新防御经验条 */
	void UpdateDefIncbarCtrl ()
	{
		int deftUpExp = (int)EXPSampleManager.Instance.getMaxEXPShow (EXPSampleManager.SID_USER_ATTR_ADD_EXP, attrAddInfo.oldDefExp);
		int defDownExp = (int)EXPSampleManager.Instance.getNowEXPShow (EXPSampleManager.SID_USER_ATTR_ADD_EXP, attrAddInfo.oldDefExp);
		defIncbarCtrl.updateValue (defDownExp, attrAddInfo.DefExp, deftUpExp);
	}

	/** 更新魔法经验条 */
	void UpdateMagIncbarCtrl ()
	{
		int magUpExp = (int)EXPSampleManager.Instance.getMaxEXPShow (EXPSampleManager.SID_USER_ATTR_ADD_EXP, attrAddInfo.oldMagExp);
		int magDownExp = (int)EXPSampleManager.Instance.getNowEXPShow (EXPSampleManager.SID_USER_ATTR_ADD_EXP, attrAddInfo.oldMagExp);
		magIncbarCtrl.updateValue (magDownExp, attrAddInfo.MagExp, magUpExp);
	}

	/** 更新敏捷经验条 */
	void UpdateDexIncbarCtrl ()
	{
		int dextUpExp = (int)EXPSampleManager.Instance.getMaxEXPShow (EXPSampleManager.SID_USER_ATTR_ADD_EXP, attrAddInfo.oldDexExp);
		int dexDownExp = (int)EXPSampleManager.Instance.getNowEXPShow (EXPSampleManager.SID_USER_ATTR_ADD_EXP, attrAddInfo.oldDexExp);
		dexIncbarCtrl.updateValue (dexDownExp, attrAddInfo.DexExp, dextUpExp);
	}
}