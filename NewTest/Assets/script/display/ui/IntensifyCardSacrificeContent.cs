using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IntensifyCardSacrificeContent : MonoBase
{
	public SacrificeRotCtrl sacrificeRotCtrl;//旋转控制器，容器都在上面哦
	public ButtonBase buttonMain;
	public UILabel skillExpValue;//技能经验值
	public UILabel combatValue;//经验值
	public UILabel oldCombatValue;
	public UITexture skillIcon;
	public UILabel skillName;
	/**技能等级升级经验条 */
	public IncbarCtrl skillIncbarCtrl;
	/**技能等级标签 */
	public UILabel skillLvLabel;
	/**技能经验增加标签 */
	//public UILabel skillAddLvLabel;
	/**技能等级 */
	public UILabel skillLvValue; //生命等级
	/** 附加属性信息对象 */
	AttrAddInfo attrAddInfo = new AttrAddInfo();
	private IntensifyCardWindow win;
	private LevelupInfo[] mskillsLvUpInfo;
	private LevelupInfo[] bskillsLvUpInfo;
	private LevelupInfo[] askillsLvUpInfo;
	private LevelupInfo cardLvUpInfo;
	/**属性加成切换显示的时间*/
	private float time; 
	public const float SWITCHTIME = 3f;

	void Update ()
	{
		if(sacrificeRotCtrl.mainShower.card != null){
			time -= Time.deltaTime;
			if (time <= 0) {
				time = SWITCHTIME;
				updateInfo ();
			}
		}
		float alphaValue= sin();
		updateAttrInfoAlpha (alphaValue);
	}
	/// <summary>
	/// 更新闪烁label
	/// </summary>
	public void updateInfo(){
		clearShowInfo();
		updateLabelShow();
	}
	/**清除数据 */
	private void clearShowInfo(){
		skillLvLabel.text="";
		//skillAddLvLabel.text="";
		skillLvValue.text="";
		skillName.text="";
		skillIcon.mainTexture=null;
		//skillExpValue.text="";
		updateAttrInfoAlpha(1);
	}
	//把alpha置1
	private void updateAttrInfoAlpha(float alphaValue){
		//skillAddLvLabel.alpha=alphaValue;
		skillIncbarCtrl.incBar.SliderBar.alpha=alphaValue;
		if(attrAddInfo.SkillGrade>0)
			skillLvLabel.alpha=alphaValue;
		else
			skillLvLabel.alpha=1;
		if(StringKit.toInt(skillExpValue.text)>0)
			skillExpValue.alpha=alphaValue;
		else
			skillExpValue.alpha=1;
	}
	public void initInfo (IntensifyCardWindow win)
	{
		this.win = win;
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
			//bugFix:先干掉list里没的.可能存在满8人,但8人完全不同了
			for (int i = 0; i <sacrificeRotCtrl.castShowers.Length; i++) {
				if (sacrificeRotCtrl.castShowers [i].card == null)
					continue;
				for (int ii = 0; ii < list.Count; ii++) {
					if (list [ii].uid == sacrificeRotCtrl.castShowers [i].card.uid) {
						continue;
					}
				}
				//遍历完都没,说明选择的时候已经移除了此人
				sacrificeRotCtrl.castShowers [i].cleanAll ();
			}
			for (int i = 0; i < list.Count; i++) {
				if (sacrificeRotCtrl.isOneOfTheCaster (list [i]))
					continue;
				sacrificeRotCtrl.selectOneEmptyCastShower ().updateShower (list [i]);
			}
		}
	}

	public int getCardCombatUp(Card _oldMainCard , int inheritSkillExp )
	{
		Card tempNewCard = _oldMainCard.Clone () as Card;
		//tempNewCard.uid = "-1";
		bool isFull = tempNewCard.isSkillExpUpFull (inheritSkillExp);
		if (isFull) {
			if (tempNewCard.getSkills () != null) {
				tempNewCard.getSkills()[0].setLevel(Mathf.Min(tempNewCard.getSkills()[0].getMaxLevel(),tempNewCard.getSkills()[0].getLevel() + 5));
			}
			if (tempNewCard.getBuffSkills () != null) {
				tempNewCard.getBuffSkills()[0].setLevel(Mathf.Min(tempNewCard.getBuffSkills()[0].getMaxLevel(),tempNewCard.getBuffSkills()[0].getLevel() + 5));		
			}
			if (tempNewCard.getAttrSkills () != null) {
				tempNewCard.getAttrSkills()[0].setLevel(Mathf.Min(tempNewCard.getAttrSkills()[0].getMaxLevel(),tempNewCard.getAttrSkills()[0].getLevel() + 5));
			}
		} else {
			if (tempNewCard.getSkills () != null) {
				tempNewCard.getSkills () [0].updateExp (tempNewCard.getSkills () [0].getEXP () + inheritSkillExp);
			}
			if (tempNewCard.getBuffSkills () != null) {
				tempNewCard.getBuffSkills () [0].updateExp (tempNewCard.getBuffSkills () [0].getEXP () + inheritSkillExp);
			}
			if (tempNewCard.getAttrSkills () != null) {
				tempNewCard.getAttrSkills () [0].updateExp (tempNewCard.getAttrSkills () [0].getEXP () + inheritSkillExp);
			}
		}

		return tempNewCard.getCardCombat () -  _oldMainCard.getCardCombat();
	}
	public void oneKey ()
	{
		OneKeyChoose ();
	}

	public void OneKeyChoose(){
		IntensifyCardManager.Instance.clearFood ();
		IntensifyCardManager.Instance.clearFoodCard ();
		win.sacrificeContent.sacrificeRotCtrl.cleanCastShower ();
		List<Card> list = IntensifyCardManager.Instance.getOneKeySacrifice ();
		if (list == null)
			return;
		
		for (int i = 0; i < list.Count; i++) {
			if (win.sacrificeContent.sacrificeRotCtrl.isOneOfTheCaster (list [i]))
				continue;				
			//没空巢就断掉
			SacrificeShowerCtrl ctrl = win.sacrificeContent.sacrificeRotCtrl.selectOneEmptyCastShower ();
			if (ctrl == null)
				break;				
			win.sacrificeContent.sacrificeRotCtrl.selectOneEmptyCastShower ().updateShower (list [i]);				
			IntensifyCardManager.Instance.setFoodCard (list [i]);
		}
		list = IntensifyCardManager.Instance.getFoodCard ();
		if (list == null || list.Count <= 0) {
			UiManager.Instance.createMessageLintWindow (LanguageConfigManager.Instance.getLanguage ("Intensify5"));
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

	public void updateExp ()
	{
		skillExpValue.text ="+"+sacrificeRotCtrl.recalculateSkillEXP ().ToString ();	
		Card mainCard = IntensifyCardManager.Instance.getMainCard ();
		if (mainCard == null) {
			skillLvLabel.text = "+0";
			combatValue.text = "+0";
			oldCombatValue.text="0";
		} else {
			oldCombatValue.text=mainCard.getCardCombat().ToString();
			combatValue.text = "+"+getCardCombatUp(mainCard,sacrificeRotCtrl.recalculateSkillEXP()).ToString();
		}
		updateBarShow();	

	}
	/**更新经验条 */
	private void updateBarShow(){
		Card mainCard = IntensifyCardManager.Instance.getMainCard();
		if(mainCard!=null){
			Skill[] skills= mainCard.getSkills();
			if(skills==null){
				skills=mainCard.getBuffSkills();
			}
			if(skills!=null&&skills.Length>=1){
				attrAddInfo.OldSkillGrade=skills[0].getLevel();
				attrAddInfo.SkillGrade=EXPSampleManager.Instance.getLevel (skills[0].getEXPSid (), (int)skills[0].getEXP()
				                                                           + sacrificeRotCtrl.recalculateSkillEXP (), skills[0].getLevel())-attrAddInfo.OldSkillGrade;
				if((attrAddInfo.OldSkillGrade+attrAddInfo.SkillGrade-5)>mainCard.getLevel()){
					attrAddInfo.SkillGrade=mainCard.getLevel()+5-attrAddInfo.OldSkillGrade;
				}
				//祭献前的经验
				int oldSkillEXP=(int)skills[0].getEXP();
				int skillUpExp=(int)EXPSampleManager.Instance.getMaxEXPShow(skills[0].getEXPSid (), oldSkillEXP);
				int skillDownExp = (int)EXPSampleManager.Instance.getNowEXPShow(skills[0].getEXPSid (),oldSkillEXP);
				ResourcesManager.Instance.LoadAssetBundleTexture (skills[0].getIcon (), skillIcon);
				skillLvValue.text = "Lv."+skills[0].getLevel().ToString();
				skillName.text=skills[0].getName();
				skillLvLabel.text =  "+"+attrAddInfo.SkillGrade.ToString();
				skillIncbarCtrl.updateValue (skillDownExp,sacrificeRotCtrl.recalculateSkillEXP (), skillUpExp);
			}
		}else{
			attrAddInfo.OldSkillGrade=0;
			attrAddInfo.SkillGrade=0;
			skillIncbarCtrl.updateValue (0,0, 1);
		}
	}
	/**更新label */
	private void updateLabelShow(){
		Card mainCard = IntensifyCardManager.Instance.getMainCard();
		if(mainCard!=null){
			Skill[] skills= mainCard.getSkills();
			if(skills==null){
				skills=mainCard.getBuffSkills();
			}
			if(skills!=null&&skills.Length>=1){
			    //祭献前的经验
				int oldSkillEXP=(int)skills[0].getEXP();
				int skillUpExp=(int)EXPSampleManager.Instance.getMaxEXPShow(skills[0].getEXPSid (), oldSkillEXP);
				int skillDownExp = (int)EXPSampleManager.Instance.getNowEXPShow(skills[0].getEXPSid (),oldSkillEXP);
				ResourcesManager.Instance.LoadAssetBundleTexture (skills[0].getIcon (), skillIcon);
				skillLvValue.text ="Lv."+skills[0].getLevel().ToString();
				skillName.text=skills[0].getName();
				skillLvLabel.text = "+"+attrAddInfo.SkillGrade.ToString();
			}
		}

	}

	public void intensify ()
	{
		IntensifyCardManager.Instance.sacrificeRestorePrize = null;
		win.intensifyButton.disableButton (true);
		win.selectFoodButton.disableButton (true);
		win.oneKeyButton.disableButton (true);
		getOldSkillInfo ();
		getOldCardInfo ();
		CardAdvanceSkillFPort cas = FPortManager.Instance.getFPort ("CardAdvanceSkillFPort") as CardAdvanceSkillFPort;
		cas.access (sacrificeRotCtrl.mainShower.card.uid, sacrificeRotCtrl.createFoodList (), sacrificeFinish);
	}

	public void getOldCardInfo ()
	{
		if (IntensifyCardManager.Instance.getMainCard () == null)
			return;
		Card tmpCard = IntensifyCardManager.Instance.getMainCard ();
		cardLvUpInfo = new LevelupInfo ();
		cardLvUpInfo.oldLevel = tmpCard.getLevel ();
		cardLvUpInfo.oldExp = tmpCard.getEXP ();			
		cardLvUpInfo.oldExpUp = tmpCard.getEXPUp ();
		cardLvUpInfo.oldExpDown = tmpCard.getEXPDown ();
		cardLvUpInfo.orgData = tmpCard.Clone ();
	}

	public void getOldSkillInfo ()
	{
		if (IntensifyCardManager.Instance.getMainCard () == null)
			return;
		Card tmpCard = IntensifyCardManager.Instance.getMainCard ();
		Skill[] mskills = tmpCard.getSkills ();
		if (mskills != null && mskills.Length > 0) {
			
			mskillsLvUpInfo = new LevelupInfo[mskills.Length];
			for (int i=0; i<mskills.Length; i++) {
				if (mskillsLvUpInfo [i] == null)
					mskillsLvUpInfo [i] = new LevelupInfo ();
				mskillsLvUpInfo [i].oldLevel = mskills [i].getLevel ();
				mskillsLvUpInfo [i].oldExp = mskills [i].getEXP ();			
				mskillsLvUpInfo [i].oldExpUp = mskills [i].getEXPUp ();	
				mskillsLvUpInfo [i].oldExpDown = mskills [i].getEXPDown ();	
				
			}
		}
		Skill[] bskills = tmpCard.getBuffSkills ();
		if (bskills != null && bskills.Length > 0) {
			
			bskillsLvUpInfo = new LevelupInfo[bskills.Length];		
			for (int i=0; i<bskills.Length; i++) {
				if (bskillsLvUpInfo [i] == null)
					bskillsLvUpInfo [i] = new LevelupInfo ();
				bskillsLvUpInfo [i].oldLevel = bskills [i].getLevel ();
				bskillsLvUpInfo [i].oldExp = bskills [i].getEXP ();			
				bskillsLvUpInfo [i].oldExpUp = bskills [i].getEXPUp ();	
				bskillsLvUpInfo [i].oldExpDown = bskills [i].getEXPDown ();	
			}
			
		}
		Skill[] askills = tmpCard.getAttrSkills ();
		if (askills != null && askills.Length > 0) {
			askillsLvUpInfo = new LevelupInfo[askills.Length];	
			for (int i=0; i<askills.Length; i++) {
				if (askillsLvUpInfo [i] == null)
					askillsLvUpInfo [i] = new LevelupInfo ();
				askillsLvUpInfo [i].oldLevel = askills [i].getLevel ();
				askillsLvUpInfo [i].oldExp = askills [i].getEXP ();			
				askillsLvUpInfo [i].oldExpUp = askills [i].getEXPUp ();	
				askillsLvUpInfo [i].oldExpDown = askills [i].getEXPDown ();	
			}
		}
		
	}

	void sacrificeFinish (string i)
	{
		getNewSkillInfo (i);
		getNewCardInfo ();
		computeRestoreCardPrize ();
		StartCoroutine (showEffect ());
		return;
	}

	/** 计算食物卡返还奖品 */
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
			IntensifyCardManager.Instance.sacrificeRestorePrize = prizeSamples;
	}

	IEnumerator showEffect ()
	{
        MaskWindow.LockUI();
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
        if (sacrificeRotCtrl.mainShower.card != null) {
            //打开技能升级窗口
            //UiManager.Instance.openWindow<SkillLevelUpWindow>((window) => {
            //    window.Initialize(sacrificeRotCtrl.mainShower.card, cardLvUpInfo, mskillsLvUpInfo, bskillsLvUpInfo, askillsLvUpInfo);
            //    mskillsLvUpInfo = null;
            //    bskillsLvUpInfo = null;
            //    askillsLvUpInfo = null;
            //    cardLvUpInfo = null;
            //});
            UiManager.Instance.openDialogWindow<SkillLvUpWindow>((win) => {
                win.Initialize(sacrificeRotCtrl.mainShower.card, cardLvUpInfo, mskillsLvUpInfo, bskillsLvUpInfo, askillsLvUpInfo);
                mskillsLvUpInfo = null;
                bskillsLvUpInfo = null;
                askillsLvUpInfo = null;
                cardLvUpInfo = null;
            });
            IntensifyCardManager.Instance.setMainCard(StorageManagerment.Instance.getRole(sacrificeRotCtrl.mainShower.card.uid));
        }
		IntensifyCardManager.Instance.clearFood ();
		sacrificeRotCtrl.hideCastShowerbase ();
        if (sacrificeRotCtrl.mainShower.card == null) {
            MaskWindow.UnlockUI();
        }

	}

	public void getNewCardInfo ()
	{
		if (sacrificeRotCtrl.mainShower.card == null)
			return;
		Card tmpCard = sacrificeRotCtrl.mainShower.card;
		cardLvUpInfo.newLevel = tmpCard.getLevel ();
		cardLvUpInfo.newExp = tmpCard.getEXP ();			
		cardLvUpInfo.newExpUp = tmpCard.getEXPUp ();
		cardLvUpInfo.newExpDown = tmpCard.getEXPDown ();
	}

	public void getNewSkillInfo (string cardID)
	{
		sacrificeRotCtrl.mainShower.card = StorageManagerment.Instance.getRole (cardID);
		if (sacrificeRotCtrl.mainShower.card == null)
			return;
		Skill[] mskills = sacrificeRotCtrl.mainShower.card.getSkills ();
		if (mskills != null && mskills.Length > 0) {		
			for (int i=0; i<mskills.Length; i++) {
				mskillsLvUpInfo [i].newLevel = mskills [i].getLevel ();
				mskillsLvUpInfo [i].newExp = mskills [i].getEXP ();			
				mskillsLvUpInfo [i].newExpUp = mskills [i].getEXPUp ();	
				mskillsLvUpInfo [i].newExpDown = mskills [i].getEXPDown ();	
				mskillsLvUpInfo [i].orgData = mskills [i];
			}
		}
		Skill[] bskills = sacrificeRotCtrl.mainShower.card.getBuffSkills ();	
		if (bskills != null && bskills.Length > 0) {	
			for (int i=0; i<bskills.Length; i++) {
				bskillsLvUpInfo [i].newLevel = bskills [i].getLevel ();
				bskillsLvUpInfo [i].newExp = bskills [i].getEXP ();			
				bskillsLvUpInfo [i].newExpUp = bskills [i].getEXPUp ();	
				bskillsLvUpInfo [i].newExpDown = bskills [i].getEXPDown ();	
				bskillsLvUpInfo [i].orgData = bskills [i];
			}
		}
		Skill[] askills = sacrificeRotCtrl.mainShower.card.getAttrSkills ();
		if (askills != null && askills.Length > 0) {	
			for (int i=0; i<askills.Length; i++) {
				askillsLvUpInfo [i].newLevel = askills [i].getLevel ();
				askillsLvUpInfo [i].newExp = askills [i].getEXP ();			
				askillsLvUpInfo [i].newExpUp = askills [i].getEXPUp ();	
				askillsLvUpInfo [i].newExpDown = askills [i].getEXPDown ();	
				askillsLvUpInfo [i].orgData = askills [i];
			}
			
		}
	}
}
