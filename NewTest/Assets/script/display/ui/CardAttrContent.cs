using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 
/// </summary>
public class CardAttrContent : MonoBase {
	/** 交换时间 */
	public const float SWITCHTIME = 3f;

	/** 经验条 */
	public barCtrl expBar;
	/** 图片 */
	public UITexture cardimage;
//    /** 经验标签 */
//    public UILabel expLabel;
	/** 卡片名字 */
	public UILabel cardName;
	/** 卡片等级 */
	public UILabel cardLevelLabel;
	/** 战斗力标签 */
	public UILabel combat;
	/** 进化标签 */
	public UILabel evoTimes;
	/**进化**/
	public UISprite evoLabel;
	/** 品质图标 */
	public UISprite qualityIco;
	/** 左上角的圆形标签 */
	public UISprite jobLabel;
	/** 属性标签(0生命,1攻击,2防御,3魔法,4敏捷) */
	public UILabel[] attrLabels;
	/** 附加属性标签(0生命,1攻击,2防御,3魔法,4敏捷) */
	public UILabel[] addAttrLabels;
	/** 品质特效 */
	public GameObject qualityEffectPoint;  
	/** 顶部背景 */
	public UITexture topBackGround;
	/** 被动技容器 */
	public GameObject root_passiveSkill;
	/** 被动技能样板 */
	public GameObject passiveSkillObj;
	/** 被动技容器组 */
	public GameObject root_passiveSkillGroup;
	/** 去进化按钮 */
	public ButtonBase buttonGotoEvo;
	/** 图鉴按钮 */
	public ButtonBase buttonPicture;

	/** 卡片对象 */
	Card card;
	/** 父窗口 */
	WindowBase parent;
	/** 属性显示bool */
	bool showAttrTime;
	/** 初始战斗力 */
	int oldCombat = 0;
	/** 最新战斗力 */
	int newCombat = 0;
	/** 步进跳跃值 */
	int stepCombat;
	/** 刷新战斗力开关 */
	private bool isRefreshCombat = false;
	/** 属性加成切换显示的时间 */
	float time; 

	/// <summary>
	/// 初始化
	/// </summary>
	/// <param name="role">卡片</param>
	public void Initialize (WindowBase parent, Card role) {
		this.parent = parent;
		this.card = role;
		loadShow ();
	}
	/// <summary>
	/// 加载显示
	/// </summary>
	public void loadShow () {
		if (card == null)
			return;
		ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.CARDIMAGEPATH + card.getImageID (), cardimage);
		cardName.text = card.getName ();
		UpdateTopBackGround ();
		initButton ();
		UpdateQualityEffect ();
		expBar.reset ();
		expBar.updateValue (EXPSampleManager.Instance.getNowEXPShow (card.getEXPSid (), card.getEXP ()), EXPSampleManager.Instance.getMaxEXPShow (card.getEXPSid (), card.getEXP ()));
//        expLabel.text = EXPSampleManager.Instance.getNowEXPShow(card.getEXPSid(), card.getEXP()) + "/" + EXPSampleManager.Instance.getMaxEXPShow(card.getEXPSid(), card.getEXP());
		jobLabel.spriteName = CardManagerment.Instance.qualityIconTextToBackGround (card.getJob ()) + "s";
		qualityIco.spriteName = QualityManagerment.qualityIDToString (card.getQualityId ()) + "Bg";
		qualityIco.alpha = 1;
		updateAttributes ();
		updateSkill ();
		parent.setTitle (QualityManagerment.getQualityColor (card.getQualityId ()) + card.getName ());
		cardLevelLabel.text = "Lv." + card.getLevel () + "/" + card.getMaxLevel ();
		evoLabel.spriteName = card.isMainCard () ? "attr_evup" : "attr_ev";
		if (EvolutionManagerment.Instance.getMaxLevel (card) == 0)
			evoTimes.text = LanguageConfigManager.Instance.getLanguage ("Evo10");
		else
			evoTimes.text = card.isMainCard () ? card.getSurLevel () + "/" + SurmountManagerment.Instance.getMaxSurLevel (card) : card.getEvoLevel () + "/" + card.getMaxEvoLevel ();
	}
	// jordenwu::展示卡片质量效果 蓝色 黄色 紫色
	public void UpdateQualityEffect ()
	{
		if (card == null) {
			qualityEffectPoint.SetActive (false);
			return;
		}
		int qualityId = card.getQualityId ();
		switch (qualityId) {
		case QualityType.GOOD:
			showEffectByQuality (qualityId);
			return;
		case QualityType.EPIC:
			showEffectByQuality (qualityId);
			return;
		case QualityType.LEGEND:
			showEffectByQuality (qualityId);
			return;
		default:
			if (qualityEffectPoint != null && qualityEffectPoint.transform.childCount > 0)
				Utils.RemoveAllChild (qualityEffectPoint.transform);
			qualityEffectPoint.SetActive (false);
			return;
		}
	}
	/** 显示卡片本身品质  */
	public void showEffectByQuality (int qualityId)
	{
		if (qualityEffectPoint == null)
			return;
		if (qualityId < QualityType.GOOD)
			return;
		if (qualityEffectPoint.transform.childCount > 0)
			Utils.RemoveAllChild (qualityEffectPoint.transform);
		qualityEffectPoint.SetActive (true);
		EffectManager.Instance.CreateEffect (qualityEffectPoint.transform, "Effect/UiEffect/CardQualityEffect" + qualityId);
	}
	private void initButton () {
		if (card == null)
			return;
		buttonPicture.fatherWindow = parent;
		buttonPicture.gameObject.SetActive (PictureManagerment.Instance.mapType.ContainsKey (card.getEvolveNextSid ()));
		buttonPicture.onClickEvent = HandleButtonPicture;
		//达到自身等级上限（非主角等级限制）且未进化满10次的卡片： 需进化
		if (!EvolutionManagerment.Instance.isMaxEvoLevel (card) && card.isMaxLevel ()) {
			buttonGotoEvo.fatherWindow=parent;
			buttonGotoEvo.gameObject.SetActive (true);
			buttonGotoEvo.onClickEvent = HandleButtonGotoEvo;
		}
		else {
			buttonGotoEvo.gameObject.SetActive (false);
		}
	}
	private void HandleButtonPicture (GameObject go) {
		UiManager.Instance.openWindow<CardPictureWindow> ((win) => {
			win.init (PictureManagerment.Instance.mapType [card.getEvolveNextSid ()], 0);
		});
	}
	private void HandleButtonGotoEvo (GameObject go) {
		IntensifyCardManager.Instance.setMainCard (card);
		IntensifyCardManager.Instance.intoIntensify (IntensifyCardManager.INTENSIFY_CARD_EVOLUTION, null);
	}
	public void UpdateTopBackGround () {
		if (card == null)
			return;
		//属性界面“力”系背景（力系和毒系职业用）
		if (card.getJob () == 1 || card.getJob () == 4) {
			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.BACKGROUNDPATH + "card_topBackGround_1", topBackGround);
		}
		//属性界面“敏”系背景（反和敏职业用）
		else if (card.getJob () == 3 || card.getJob () == 5) {
			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.BACKGROUNDPATH + "card_topBackGround_2", topBackGround);
		}
		//属性界面“魔”系背景（魔和辅职业用）
		else {
			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.BACKGROUNDPATH + "card_topBackGround_3", topBackGround);
		}
	}
	/// <summary>
	/// 更新属性
	/// </summary>
	private void updateAttributes () {
		rushCombat ();
		CardBaseAttribute attr = CardManagerment.Instance.getCardWholeAttr (card);
		CardBaseAttribute attr2 = CardManagerment.Instance.getCardAppendEffectNoSuit (card);
		attrLabels [0].text = attr.getWholeHp ().ToString ();
		attrLabels [1].text = attr.getWholeAtt ().ToString ();
		attrLabels [2].text = attr.getWholeDEF ().ToString ();
		attrLabels [3].text = attr.getWholeMAG ().ToString ();
		attrLabels [4].text = attr.getWholeAGI ().ToString ();
		if (showAttrTime == true) {
			addAttrLabels [0].text = " + " + attr2.getWholeHp ().ToString ();
			addAttrLabels [1].text = " + " + attr2.getWholeAtt ().ToString ();
			addAttrLabels [2].text = " + " + attr2.getWholeDEF ().ToString ();
			addAttrLabels [3].text = " + " + attr2.getWholeMAG ().ToString ();
			addAttrLabels [4].text = " + " + attr2.getWholeAGI ().ToString ();
		}
		else {
			string fj = LanguageConfigManager.Instance.getLanguage ("s0141");
			addAttrLabels [0].text = CardManagerment.Instance.getCardAttrAppendLevel (card, AttributeType.hp) != 0 ? fj + "Lv." + CardManagerment.Instance.getCardAttrAppendLevel (card, AttributeType.hp) : "";
			addAttrLabels [1].text = CardManagerment.Instance.getCardAttrAppendLevel (card, AttributeType.attack) != 0 ? fj + "Lv." + CardManagerment.Instance.getCardAttrAppendLevel (card, AttributeType.attack) : "";
			addAttrLabels [2].text = CardManagerment.Instance.getCardAttrAppendLevel (card, AttributeType.defecse) != 0 ? fj + "Lv." + CardManagerment.Instance.getCardAttrAppendLevel (card, AttributeType.defecse) : "";
			addAttrLabels [3].text = CardManagerment.Instance.getCardAttrAppendLevel (card, AttributeType.magic) != 0 ? fj + "Lv." + CardManagerment.Instance.getCardAttrAppendLevel (card, AttributeType.magic) : "";
			addAttrLabels [4].text = CardManagerment.Instance.getCardAttrAppendLevel (card, AttributeType.agile) != 0 ? fj + "Lv." + CardManagerment.Instance.getCardAttrAppendLevel (card, AttributeType.agile) : "";
		}
	}
	//初始化技能
	private void updateSkill () {
		if (card == null)
			return;
		Skill[] attrSkill = card.getAttrSkills ();
		//被动技能显示
		if (attrSkill != null && attrSkill.Length > 0) {
            
			bool isShowAttrTitle = false;
			GameObject passiveSkill;
			ButtonSkill skillComponent;
            int k = 0;
			for (int i = 0; i < attrSkill.Length; i++) {
                if (attrSkill[i].getShowType() == 2) {
                    continue;
                }
				passiveSkill = NGUITools.AddChild (root_passiveSkill.gameObject, passiveSkillObj);
				passiveSkill.SetActive (true);
				passiveSkill.transform.localScale = Vector3.one;
				passiveSkill.transform.localPosition = new Vector3 (k * 80, 0f, 0f);
                k++;
				skillComponent = passiveSkill.GetComponent<ButtonSkill> ();
				skillComponent.gameObject.SetActive (true);
				skillComponent.initSkillData (attrSkill [i], ButtonSkill.STATE_LEARNED);
				isShowAttrTitle = true;
			}
			root_passiveSkillGroup.SetActive (isShowAttrTitle);
		}
		else {
			root_passiveSkillGroup.SetActive (false);
		}
	}
	void Update () {
		time -= Time.deltaTime;
		if (time <= 0) {
			showAttrTime = !showAttrTime;
			time = SWITCHTIME;
			updateAttributes ();
		}
		if (isRefreshCombat) {
			refreshCombat ();
		}
	}
	/// <summary>
	/// 刷新战斗力
	/// </summary>
	public void rushCombat () {
		newCombat = CombatManager.Instance.getCardCombat (card);
		isRefreshCombat = true;
		if (newCombat >= oldCombat)
			stepCombat = (int)((float)(newCombat - oldCombat) / 20);
		else
			stepCombat = (int)((float)(oldCombat - newCombat) / 20);
		if (stepCombat < 1)
			stepCombat = 1;
	}
	/// <summary>
	/// 刷新战斗力
	/// </summary>
	private void refreshCombat () {
		if (oldCombat != newCombat) {
			if (oldCombat < newCombat) {
				oldCombat += stepCombat;
				if (oldCombat >= newCombat)
					oldCombat = newCombat;
			}
			else if (oldCombat > newCombat) {
				oldCombat -= stepCombat;
				if (oldCombat <= newCombat)
					oldCombat = newCombat;
			}
			combat.text = oldCombat + "";
		}
		else {
			isRefreshCombat = false;
			combat.text = newCombat + "";
			oldCombat = newCombat;
		}
	}
}
