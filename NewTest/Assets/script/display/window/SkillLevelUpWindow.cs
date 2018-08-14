using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillLevelUpWindow : WindowBase
{
	public UITexture topBackGround;
	public UILabel cardName;
	public ExpbarCtrl expBar;
	public UITexture cardimage;
	public UILabel levelLabel;
	public UILabel expLabel;
	public UISprite evoSprite;
	public UILabel evoLabel;
	public UISprite jobSprite;
	public UILabel[] attrLabels;//0生命，1攻击，2防御，3魔法，4敏捷
	public UILabel[] addAttrLabels;//0生命，1攻击，2防御，3魔法，4敏捷
	public UILabel[] addEvoTitle;//增加属性标题
	public UILabel combat;//战斗力
	public ButtonSkillLevelUp[] buttonList;
	public Card card;
	public Card oldCard;
	private LevelupInfo[] mskillsLvUpInfo;
	private LevelupInfo[] bskillsLvUpInfo;
	private LevelupInfo[] askillsLvUpInfo;
	private LevelupInfo cardLvUpInfo;
	private int nowLv;
	private CallBack callBack;
	float time; //属性加成切换显示的时间
	public const float SWITCHTIME = 3f;
	bool showAttrTime = false;//true就显示附加属性数据，false显示附加等级
	bool canSeeAddon = false;

	protected override void begin ()
	{
		base.begin ();
		GuideManager.Instance.guideEvent ();
		MaskWindow.UnlockUI ();
	}

	public void Initialize (Card _oldCard, Card _newCard, CallBack _callBack)
	{
		this.callBack = _callBack;
		//先存好经验
		LevelupInfo info = new LevelupInfo ();
		info.oldLevel = _oldCard.getLevel ();
		info.oldExp = _oldCard.getEXP ();			
		info.oldExpUp = _oldCard.getEXPUp ();
		info.oldExpDown = _oldCard.getEXPDown ();
		info.orgData = _oldCard.Clone ();

		info.newLevel = _newCard.getLevel ();
		info.newExp = _newCard.getEXP ();			
		info.newExpUp = _newCard.getEXPUp ();
		info.newExpDown = _newCard.getEXPDown ();

		//然后存卡片技能经验
		LevelupInfo[] msk = null;
		Skill[] oldMskills = _oldCard.getSkills ();
		Skill[] newMskills = _newCard.getSkills ();
		if (oldMskills != null && oldMskills.Length > 0) {
			msk = new LevelupInfo[oldMskills.Length];
			for (int i=0; i<oldMskills.Length; i++) {
				if (msk [i] == null)
					msk [i] = new LevelupInfo ();
				msk [i].oldLevel = oldMskills [i].getLevel ();
				msk [i].oldExp = oldMskills [i].getEXP ();
				msk [i].oldExpUp = oldMskills [i].getEXPUp ();
				msk [i].oldExpDown = oldMskills [i].getEXPDown ();

				msk [i].newLevel = newMskills [i].getLevel ();
				msk [i].newExp = newMskills [i].getEXP ();
				msk [i].newExpUp = newMskills [i].getEXPUp ();
				msk [i].newExpDown = newMskills [i].getEXPDown ();
				msk [i].orgData = newMskills [i];
			}
		}

		LevelupInfo[] bsk = null;
		Skill[] oldBskills = _oldCard.getBuffSkills ();
		Skill[] newBskills = _newCard.getBuffSkills ();
		if (oldBskills != null && oldBskills.Length > 0) {
			bsk = new LevelupInfo[oldBskills.Length];
			for (int i=0; i<oldBskills.Length; i++) {
				if (bsk [i] == null)
					bsk [i] = new LevelupInfo ();
				bsk [i].oldLevel = oldBskills [i].getLevel ();
				bsk [i].oldExp = oldBskills [i].getEXP ();
				bsk [i].oldExpUp = oldBskills [i].getEXPUp ();
				bsk [i].oldExpDown = oldBskills [i].getEXPDown ();
				
				bsk [i].newLevel = newBskills [i].getLevel ();
				bsk [i].newExp = newBskills [i].getEXP ();
				bsk [i].newExpUp = newBskills [i].getEXPUp ();
				bsk [i].newExpDown = newBskills [i].getEXPDown ();
				bsk [i].orgData = newBskills [i];
			}
		}

		LevelupInfo[] ask = null;
		Skill[] oldAskills = _oldCard.getAttrSkills ();
		Skill[] newAskills = _newCard.getAttrSkills ();
		if (oldAskills != null && oldAskills.Length > 0) {
			ask = new LevelupInfo[oldAskills.Length];
			for (int i=0; i<oldAskills.Length; i++) {
				if (ask [i] == null)
					ask [i] = new LevelupInfo ();
				ask [i].oldLevel = oldAskills [i].getLevel ();
				ask [i].oldExp = oldAskills [i].getEXP ();
				ask [i].oldExpUp = oldAskills [i].getEXPUp ();
				ask [i].oldExpDown = oldAskills [i].getEXPDown ();
				
				ask [i].newLevel = newAskills [i].getLevel ();
				ask [i].newExp = newAskills [i].getEXP ();
				ask [i].newExpUp = newAskills [i].getEXPUp ();
				ask [i].newExpDown = newAskills [i].getEXPDown ();
				ask [i].orgData = newAskills [i];
			}
		}

		Initialize (_newCard, info, msk ,bsk ,ask);
	}

	//初始化，带入3大类技能的升级信息
	public void Initialize (Card role, LevelupInfo info, LevelupInfo[] msk, LevelupInfo[] bsk, LevelupInfo[] ask)
	{
		this.card = role;
		this.mskillsLvUpInfo = msk;
		this.bskillsLvUpInfo = bsk;
		this.askillsLvUpInfo = ask;
		this.cardLvUpInfo = info;
		oldCard = info.orgData as Card;
		cardimage.alpha = 0;
		ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.CARDIMAGEPATH + role.getImageID (), cardimage);
		CardBaseAttribute attrOldEff = CardManagerment.Instance.getCardWholeAttr (oldCard);
		CardBaseAttribute attrNewEff = CardManagerment.Instance.getCardWholeAttr (role);
		CardBaseAttribute attrAddon = CardManagerment.Instance.getCardAppendEffectNoSuit (role);
		_oldAttr = new int[5] {
			attrOldEff.getWholeHp (),
			attrOldEff.getWholeAtt (),
			attrOldEff.getWholeDEF (),
			attrOldEff.getWholeMAG (),
			attrOldEff.getWholeAGI ()
		};
		_newAttr = new int[5] {
			attrNewEff.getWholeHp (),
			attrNewEff.getWholeAtt (),
			attrNewEff.getWholeDEF (),
			attrNewEff.getWholeMAG (),
			attrNewEff.getWholeAGI ()
		};
		_addAttr = new int[5] {
			(attrNewEff.getWholeHp () - attrOldEff.getWholeHp ()),
			(attrNewEff.getWholeAtt () - attrOldEff.getWholeAtt ()),
			(attrNewEff.getWholeDEF () - attrOldEff.getWholeDEF ())
			,
			(attrNewEff.getWholeMAG () - attrOldEff.getWholeMAG ()),
			(attrNewEff.getWholeAGI () - attrOldEff.getWholeAGI ())
		};
		_step = new int[5] {
			setStep (attrOldEff.getWholeHp (), attrNewEff.getWholeHp ()),
			setStep (attrOldEff.getWholeAtt (), attrNewEff.getWholeAtt ()),
			setStep (attrOldEff.getWholeDEF (), attrNewEff.getWholeDEF ()),
			setStep (attrOldEff.getWholeMAG (), attrNewEff.getWholeMAG ()),
			setStep (attrOldEff.getWholeAGI (), attrNewEff.getWholeAGI ())
		};
		_addonAttr = new int[5] {
			attrAddon.getWholeHp (),
			attrAddon.getWholeAtt (),
			attrAddon.getWholeDEF (),
			attrAddon.getWholeMAG (),
			attrAddon.getWholeAGI ()
		};
		_addonAttrLv = new int[] {
			CardManagerment.Instance.getCardAttrAppendLevel (role, AttributeType.hp),
			CardManagerment.Instance.getCardAttrAppendLevel (role, AttributeType.attack),
			CardManagerment.Instance.getCardAttrAppendLevel (role, AttributeType.defecse),
			CardManagerment.Instance.getCardAttrAppendLevel (role, AttributeType.magic),
			CardManagerment.Instance.getCardAttrAppendLevel (role, AttributeType.agile),
		};

		cardName.text = QualityManagerment.getQualityColor (role.getQualityId ()) + role.getName ();
		int jobId = role.getJob ();
		jobSprite.spriteName = CardManagerment.Instance.qualityIconTextToBackGround (jobId) + "s";
		//属性界面“力”系背景（力系和毒系职业用）
		if (jobId == 1 || jobId == 4) {
			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.BACKGROUNDPATH + "card_topBackGround_1", topBackGround);
		}
		//属性界面“敏”系背景（反和敏职业用）
		else if (jobId == 3 || jobId == 5) {
			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.BACKGROUNDPATH + "card_topBackGround_2", topBackGround);
		}
		//属性界面“魔”系背景（魔和辅职业用）
		else {
			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.BACKGROUNDPATH + "card_topBackGround_3", topBackGround);
		}
		nowLv = oldCard.getLevel ();
		levelLabel.text = "Lv." + nowLv + "/" + role.getMaxLevel ();
		evoSprite.spriteName = card.isMainCard () ? "attr_evup" : "attr_ev";
		if (EvolutionManagerment.Instance.getMaxLevel(role) == 0)
			evoLabel.text = LanguageConfigManager.Instance.getLanguage("Evo10");
		else
			evoLabel.text = role.isMainCard()?  
				role.getSurLevel() + "/" + SurmountManagerment.Instance.getMaxSurLevel (role):
				role.getEvoLevel () + "/" + role.getMaxEvoLevel ();

		combat.text = " " + CombatManager.Instance.getCardCombat (oldCard);

		for (int i=0; i<attrLabels.Length; i++) {
			attrLabels [i].text = _oldAttr [i] + "";
		}

		for (int i=0; i<addAttrLabels.Length; i++) {
			addAttrLabels [i].text = "";
		}

		StartCoroutine (Utils.DelayRun (() => {
			canSeeAddon = false;
			step = 0;
			nextSetp = 1;
			canRefresh = true;
			MaskWindow.LockUI ();
		}, 0.5f));

	}

	private int setStep (int newCombat, int oldCombat)
	{
		int a = 1;
		if (newCombat >= oldCombat)
			a = (int)((float)(newCombat - oldCombat) / 50);
		else
			a = (int)((float)(oldCombat - newCombat) / 50);
		if (a < 1)
			return 1;
		else
			return a;
	}

	private void updateSkills ()
	{
		List<LevelupInfo> newSkillsInfo = new List<LevelupInfo> ();
		List<LevelupInfo> newMaxLvSkillsInfo = new List<LevelupInfo> ();

		if (mskillsLvUpInfo != null) {
			for (int i= 0; i< mskillsLvUpInfo.Length; i++) {
				if ((mskillsLvUpInfo [i].orgData as Skill).isMAxLevel ())
					newMaxLvSkillsInfo.Add (mskillsLvUpInfo [i]);
				else
					newSkillsInfo.Add (mskillsLvUpInfo [i]);
			}
		}
		if (bskillsLvUpInfo != null) {
			for (int i= 0; i< bskillsLvUpInfo.Length; i++) {
				if ((bskillsLvUpInfo [i].orgData as Skill).isMAxLevel ()) {
					if (!card.isMainCard ()) {
						newMaxLvSkillsInfo.Add (bskillsLvUpInfo [i]);
					} else {
						continue;
					}
				}
				else
					newSkillsInfo.Add (bskillsLvUpInfo [i]);
			}
		}
		if (askillsLvUpInfo != null) {
			for (int i= 0; i< askillsLvUpInfo.Length; i++) {
				if ((askillsLvUpInfo [i].orgData as Skill).getShowType () == 2) {
					continue;
				}
				if ((askillsLvUpInfo [i].orgData as Skill).isMAxLevel ())
					newMaxLvSkillsInfo.Add (askillsLvUpInfo [i]);
				else
					newSkillsInfo.Add (askillsLvUpInfo [i]);
			}
		}

		if (newMaxLvSkillsInfo != null) {
			for (int i= 0; i< newMaxLvSkillsInfo.Count; i++) {
				newSkillsInfo.Add (newMaxLvSkillsInfo [i]);
			}
		}

		for (int i= 0; i < newSkillsInfo.Count && i < buttonList.Length; i++) {
			if (newSkillsInfo [i] == null)
				continue;
			Skill skillData = (Skill)newSkillsInfo [i].orgData;
//			Debug.LogError ("--->>" + skillData.getName () + ",i=" + i);
			if (skillData != null && skillData.getSkillStateType () == SkillStateType.ATTR) {
				buttonList [i].gameObject.SetActive (false);
			} else {
				buttonList [i].gameObject.SetActive (true);
				buttonList [i].updateButton (newSkillsInfo [i], card);
				buttonList [i].setNum (i);
			}

		}
	}

	int[] _oldAttr;
	int[] _newAttr;
	int[] _openRefresh;
	int[] _step;
	int[] _addAttr;
	int[] _addonAttr;//附加属性
	int[] _addonAttrLv;//附加属性等级
	int step = 0;
	int nextSetp = 0;
	bool canRefresh = false;
	int oldCombat = 0;//初始战斗力
	int newCombat = 0;//最新战斗力
	int stepCombat;//步进跳跃值
	private bool isRefreshCombat = false;//刷新战斗力开关
	
	//刷新战斗力
	public void rushCombat (Card _old)
	{
		oldCombat = CombatManager.Instance.getCardCombat (_old);
		if (card != null)
			newCombat = CombatManager.Instance.getCardCombat (card);
		else {
			combat.text = "0";
			return;
		}
		isRefreshCombat = true;
		if (newCombat >= oldCombat)
			stepCombat = (int)((float)(newCombat - oldCombat) / 50);
		else
			stepCombat = (int)((float)(oldCombat - newCombat) / 50);
		if (stepCombat < 1)
			stepCombat = 1;
	}
	
	private void refreshCombat ()
	{
		if (oldCombat != newCombat) {
			if (oldCombat < newCombat) {
				oldCombat += stepCombat;
				if (oldCombat >= newCombat)
					oldCombat = newCombat;
			} else if (oldCombat > newCombat) {
				oldCombat -= stepCombat;
				if (oldCombat <= newCombat)
					oldCombat = newCombat;
			}
			combat.text = " " + oldCombat;
		} else {
			isRefreshCombat = false;
			combat.text = " " + newCombat;
			oldCombat = newCombat;
		}
	}

	//刷新属性值
	private void refreshAttr (int i)
	{
		if (_oldAttr [i] != _newAttr [i]) {
			
			if (_oldAttr [i] < _newAttr [i]) {
				_oldAttr [i] += _step [i];
				if (_oldAttr [i] >= _newAttr [i])
					_oldAttr [i] = _newAttr [i];
			} else if (_oldAttr [i] > _newAttr [i]) {
				_oldAttr [i] -= _step [i];
				if (_oldAttr [i] <= _newAttr [i])
					_oldAttr [i] = _newAttr [i];
			}
			attrLabels [i].text = _oldAttr [i] + "";
		} else {
			_openRefresh [i] = 1;
			attrLabels [i].text = _newAttr [i] + "";
		}
	}

	//可以关闭刷新没
	private bool isCloseRefresh ()
	{
		int a = 0;
		if (_openRefresh == null || _openRefresh.Length <= 0) {
			return true;
		}
		for (int i = 0; i < _openRefresh.Length; i++) {
			if (_openRefresh [i] != 0)
				a++;
		}
		if (a >= _openRefresh.Length)
			return true;
		else
			return false;
	}

	private void playerEffect (UILabel _labelTitle, UILabel _labelDesc, int _desc)
	{
		_labelTitle.text = "+";
		TweenScale ts = TweenScale.Begin (_labelTitle.gameObject, 0.12f, Vector3.one);
		ts.method = UITweener.Method.EaseIn;
		ts.from = new Vector3 (5, 5, 1);
		_labelDesc.text = "";
		TweenScale ts2 = TweenScale.Begin (_labelDesc.gameObject, 0.1f, Vector3.one);
		ts2.method = UITweener.Method.EaseIn;
		ts2.from = new Vector3 (5, 5, 1);
		EventDelegate.Add (ts2.onFinished, () => {
			TweenLabelNumber tln = TweenLabelNumber.Begin (_labelDesc.gameObject, 0.1f, _desc);
			tln.from = 0;
			EventDelegate.Add (tln.onFinished, () => {
				GameObject obj = Create3Dobj ("Effect/Other/Flash").obj;
				obj.transform.parent = _labelDesc.transform;
				obj.transform.localScale = Vector3.one;
				obj.transform.localPosition = new Vector3 (0, 0, -600);
				StartCoroutine (Utils.DelayRun (() => {
					nextSetp++;}, 0.1f));
			}, true);
		}, true);
	}

	private void SwitchShow ()
	{
		if (showAttrTime == true) {

			for (int i = 0; i < addAttrLabels.Length; i++) {
				addAttrLabels[i].text = _addonAttr[i] == 0 ? "" : (" + " + _addonAttr[i]);
			}
		} else {
			for (int i = 0; i < addAttrLabels.Length; i++) {
				addAttrLabels[i].text = _addonAttrLv[i] == 0 ? "" : (" + Lv." + _addonAttrLv[i]);
			}
		}
	}
	
	void Update ()
	{
		if (!canRefresh && canSeeAddon) {
			time -= Time.deltaTime;
			
			if (time <= 0) {
				showAttrTime = !showAttrTime;
				time = SWITCHTIME;
				SwitchShow ();
			}
		}

		if (isRefreshCombat) {
			refreshCombat ();
		}

		if (!isCloseRefresh ()) {
			refreshAttr (0);
			refreshAttr (1);
			refreshAttr (2);
			refreshAttr (3);
			refreshAttr (4);
		}

		if (canRefresh == true) {
			
			if (step == nextSetp)
				return;

			//人物进场
			if (step == 0) {
				cardimage.alpha = 1;
				TweenScale ts = TweenScale.Begin (cardimage.gameObject, 0.2f, Vector3.one);
				ts.method = UITweener.Method.EaseIn;
				ts.from = new Vector3 (5, 5, 1);
				EventDelegate.Add (ts.onFinished, () => {
					iTween.ShakePosition (cardimage.gameObject, iTween.Hash ("amount", new Vector3 (0.03f, 0.03f, 0.03f), "time", 0.2f));
					iTween.ShakePosition (cardimage.gameObject, iTween.Hash ("amount", new Vector3 (0.01f, 0.01f, 0.01f), "time", 0.2f));
					StartCoroutine (Utils.DelayRun (() => {
						nextSetp++;}, 0.3f));
				}, true);
			}

			//计算人物经验
			else if (step == 1) {
				if (cardLvUpInfo != null) {
					expBar.init (cardLvUpInfo);
					expBar.setLevelUpCallBack (showLevelupSign);
					if (card.getLevel () != oldCard.getLevel ()) {
						StartCoroutine (Utils.DelayRun (() => {
							if (expBar.getIsUp ()) {
								EffectCtrl effect = EffectManager.Instance.CreateEffect (cardimage.transform, "Effect/UiEffect/levelupEffect");
								effect.transform.GetChild (0).particleSystem.Play ();
							}
						}, 1f));
					}
					expLabel.text = EXPSampleManager.Instance.getExpBarShow (card.getEXPSid (), card.getEXP ());
					if (EvolutionManagerment.Instance.getMaxLevel(card) == 0)
						evoLabel.text = LanguageConfigManager.Instance.getLanguage("Evo10");
					else
						evoLabel.text = card.isMainCard()?  
							card.getSurLevel() + "/" + SurmountManagerment.Instance.getMaxSurLevel (card):
							card.getEvoLevel () + "/" + card.getMaxEvoLevel ();
					if (card.getLevel () == oldCard.getLevel ()) {
						nextSetp++;
					}
				} else {
					StartCoroutine (Utils.DelayRun (() => {
						nextSetp++;}, 0.3f));
				}
				updateSkills ();

			} else if (step == 2) {
				if (card.getLevel () != oldCard.getLevel ())
					playerEffect (addEvoTitle [0], addAttrLabels [0], _addAttr [0]);
				else
					nextSetp++;
			} else if (step == 3) {
				if (card.getLevel () != oldCard.getLevel ())
					playerEffect (addEvoTitle [1], addAttrLabels [1], _addAttr [1]);
				else
					nextSetp++;
			} else if (step == 4) {
				if (card.getLevel () != oldCard.getLevel ())
					playerEffect (addEvoTitle [2], addAttrLabels [2], _addAttr [2]);
				else
					nextSetp++;
			} else if (step == 5) {
				if (card.getLevel () != oldCard.getLevel ())
					playerEffect (addEvoTitle [3], addAttrLabels [3], _addAttr [3]);
				else
					nextSetp++;
			} else if (step == 6) {
				if (card.getLevel () != oldCard.getLevel ())
					playerEffect (addEvoTitle [4], addAttrLabels [4], _addAttr [4]);
				else
					nextSetp++;
			} else if (step == 7) {
				if (card.getLevel () != oldCard.getLevel ()) {
					_openRefresh = new int[5]{0,0,0,0,0};
					for (int i = 0; i<addAttrLabels.Length; i++) {
						TweenScale ts = TweenScale.Begin (addAttrLabels [i].gameObject, 0.5f, Vector3.zero);
						TweenScale ts2 = TweenScale.Begin (addEvoTitle [i].gameObject, 0.5f, Vector3.zero);
					}
					StartCoroutine (Utils.DelayRun (() => {
						nextSetp++;}, 2f));
				} else {
					nextSetp++;
				}
			} else if (step == 8) {
				rushCombat (oldCard);
				canRefresh = false;
				StartCoroutine (Utils.DelayRun (() => {
					canSeeAddon = true;
					for (int i = 0; i < addAttrLabels.Length; i++) {
						if(string.IsNullOrEmpty(addAttrLabels[i].text)) continue;
						addAttrLabels[i].transform.localScale = Vector3.one;
						GameObject obj = Create3Dobj ("Effect/Other/Flash").obj;
						obj.transform.parent = addAttrLabels[i].transform;
						obj.transform.localScale = Vector3.one;
						obj.transform.localPosition = new Vector3 (0, 0, -600);
					}
					nextSetp++;}, 0.2f));
				MaskWindow.UnlockUI ();
//				updateSkills ();
//				StartCoroutine (Utils.DelayRun (() => {
//					nextSetp++;}, 0.2f));
			}

			step++;
		}
	}

	//经验条满后调用
	public void showLevelupSign (int now)
	{
		nowLv += 1;
		levelLabel.text = "Lv." + nowLv + "/" + card.getMaxLevel ();
		if (now == card.getLevel ()) {
			StartCoroutine (Utils.DelayRun (() => {
				nextSetp++;}, 0.5f));
		}
	}

	public override void DoDisable ()
	{
		base.DoDisable ();
	
	}

	void returnIntensifyWindow ()
	{
		IntensifyCardManager.Instance.setMainCard (card);
		IntensifyCardManager.Instance.intoIntensify (IntensifyCardManager.INTENSIFY_CARD_SACRIFICE, (fatherWindow as IntensifyCardWindow).getCallBack ());
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "continue") {
			//继续强化
			finishWindow ();
			if (callBack != null) {
				callBack ();
			}
		}
		if (gameObj.name == "close") {

			//这里特殊操作会到角色选择

			if (!GuideManager.Instance.isGuideComplete ()) {
				ArmyManager.Instance.cleanAllEditArmy ();
				GuideManager.Instance.doGuide ();
				UiManager.Instance.openMainWindow ();
				return;
			} else {

				//这里特殊操作
				//TODO
				UiManager.Instance.openMainWindow ();
			}
		}
	}
}
