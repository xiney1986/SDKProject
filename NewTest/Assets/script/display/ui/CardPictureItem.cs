using UnityEngine;
using System.Collections.Generic;

/**
 * 图鉴卡片信息显示
 * yxl
 * */
public class CardPictureItem : MonoBehaviour {

	public UITexture CardImage;
	public UILabel cardName;
	public UILabel cardLv;
	public UILabel jinhua;
	public UILabel hp;
	public UILabel combat;
	public UILabel mag;
	public UILabel att;
	public UILabel dex;
	public UILabel def;
	public UILabel hpAdd;
	public UILabel magAdd;
	public UILabel attAdd;
	public UILabel dexAdd;
	public UILabel defAdd;
	public UISprite quality;
	public UISprite jobSign;
    public UISprite passiveSkilltexture;

	/** 技能样板 */
	public GameObject skillObj;
	/** 主动技能容器 */
	public GameObject root_mainSkill;
	/** 被动技容器 */
	public GameObject root_passiveSkill;
	/** 被动技能样板 */
	public GameObject passiveSkillObj;
	/** 被动技容器组 */
	public GameObject root_passiveSkillGroup;

	/** 天赋容器 */
	public GameObject talentContent;
	/** 天赋样板 */
	public GameObject talentPrefab;
	/** 天赋背景 */
	public UISprite talentBg;
	/** 天赋技能容器 */
	public UIPanel panlObj;

	/** 临时缓存的主动技能 */
	private List<ButtonSkill> mainSkillList;
	/** 临时缓存的被动技能 */
	private List<ButtonSkill> attrSkillList;
	/** 临时缓存的天赋 */
	private List<CardAttrTalentItem> talentList;
	private Card showCard;
	private CardSample sample;
	[HideInInspector]
	public CardPictureWindow window;

	
	//初始化技能
	private void updateSkill ()
	{ 
		Card skillCard = showCard;
		//获得3类技能
		Skill[] mainSkil = skillCard.getSkills ();
		Skill[] attrSkill = skillCard.getAttrSkills ();
		Skill[] buffSkill = skillCard.getBuffSkills ();
		Skill[] totalSkill = null;

		if (mainSkil != null) {
			for (int i=0; i<mainSkil.Length; i++) {
				mainSkil[i].updateExp(EXPSampleManager.Instance.getEXPDown(SkillSampleManager.Instance.getSkillSampleBySid(mainSkil[i].sid).levelId,
				                                                         Mathf.Min(skillCard.getLevel() + 5,mainSkil[i].getMaxLevel())));
			}
		}
		if (attrSkill != null) {
			for (int i=0; i<attrSkill.Length; i++) {
				attrSkill[i].updateExp(EXPSampleManager.Instance.getEXPDown(SkillSampleManager.Instance.getSkillSampleBySid(attrSkill[i].sid).levelId,
				                                                         Mathf.Min(skillCard.getLevel() + 5,attrSkill[i].getMaxLevel())));
			}
		}
		if (buffSkill != null) {
			for (int i=0; i<buffSkill.Length; i++) {
				buffSkill[i].updateExp(EXPSampleManager.Instance.getEXPDown(SkillSampleManager.Instance.getSkillSampleBySid(buffSkill[i].sid).levelId,
				                                                         Mathf.Min(skillCard.getLevel() + 5,buffSkill[i].getMaxLevel())));
			}
		}

		List<Skill> tmpSkills = new List<Skill> ();
		if (mainSkil != null) {
			for (int i = 0; i < mainSkil.Length; i++) {
				tmpSkills.Add (mainSkil[i]);
			}
		}
		if (buffSkill != null) {
			for (int i = 0; i < buffSkill.Length; i++) {
				tmpSkills.Add (buffSkill[i]);
			}
		}

		//主动技能显示
		if (tmpSkills != null && tmpSkills.Count > 0) {
			GameObject passiveSkill;
			ButtonSkill skillComponent;
			
			if (mainSkillList == null) {
				mainSkillList = new List<ButtonSkill>();
			} else {
				for (int i = 0; i < mainSkillList.Count; i++) {
					mainSkillList[i].gameObject.SetActive (false);
				}
			}
			
			for (int i = 0,j = 0; i < tmpSkills.Count; i++) {
				if (tmpSkills [i].getShowType () == 2) {
					continue;
				}
				//如果缓存有，就读缓存，没有就创建出来丢进缓存
				if (j >= mainSkillList.Count) {
					passiveSkill = NGUITools.AddChild (root_mainSkill.gameObject, skillObj);
					passiveSkill.SetActive (true);
					passiveSkill.transform.localScale = Vector3.one;
					passiveSkill.transform.localPosition = new Vector3 (j * 270, 0f, 0f);
					skillComponent = passiveSkill.GetComponent<ButtonSkill> ();
					skillComponent.fatherWindow = window;
					mainSkillList.Add (skillComponent);
				}
				mainSkillList[j].gameObject.SetActive (true);
				mainSkillList[j].initSkillData (tmpSkills [i], ButtonSkill.STATE_LEARNED);
				
				j++;
			}
		}

		//被动技能显示
		if (attrSkill != null && attrSkill.Length > 0) {
			bool isShowAttrTitle = false;
			GameObject passiveSkill;
			ButtonSkill skillComponent;
			if (attrSkillList == null) {
				attrSkillList = new List<ButtonSkill>();
			} else {
				for (int i = 0; i < attrSkillList.Count; i++) {
					attrSkillList[i].gameObject.SetActive (false);
				}
			}
			
			for (int i = 0,j = 0; i < attrSkill.Length; i++) {
				if (attrSkill [i].getShowType () == 2) {
					continue;
				}
				//如果缓存有，就读缓存，没有就创建出来丢进缓存
				if (j >= attrSkillList.Count) {
					passiveSkill = NGUITools.AddChild (root_passiveSkill.gameObject, passiveSkillObj);
					passiveSkill.SetActive (true);
					passiveSkill.transform.localScale = Vector3.one;
					passiveSkill.transform.localPosition = new Vector3 (j * 80, 0f, 0f);
					skillComponent = passiveSkill.GetComponent<ButtonSkill> ();
					skillComponent.fatherWindow = window;
					attrSkillList.Add (skillComponent);
				}
				attrSkillList[j].gameObject.SetActive (true);
				attrSkillList[j].initSkillData (attrSkill [i], ButtonSkill.STATE_LEARNED);
				
				j++;
				isShowAttrTitle = true;
			}
            if(showCard.getQualityId()==QualityType.MYTH)
            {
                passiveSkilltexture.spriteName = "icon_mythSkillTitle";
            }
                root_passiveSkillGroup.SetActive(isShowAttrTitle);
			
		} else {
			root_passiveSkillGroup.SetActive (false);
		}
	}

	/// <summary>
	/// 加载天赋
	/// </summary>
	void loadTalent ()
	{
		EvolutionSample sample = EvolutionManagerment.Instance.getEvoInfoByType (showCard);
		if (sample == null)
			return;
		string[] strs = sample.getAddTalentString ();
		if (strs == null || strs.Length == 0)
			return;
		//			switchButton.gameObject.SetActive (true);
		CardAttrTalentItem tempTalent;
		if (talentList == null) {
			talentList = new List<CardAttrTalentItem> ();
		} else {
			for (int i = 0; i < talentList.Count; i++) {
				talentList[i].gameObject.SetActive (false);
				talentList[i].text1.text = "";
				talentList[i].text2.text = "";
			}
		}
		for (int i = 0, j = 0; i < strs.Length; i++) {
			//如果缓存有，就读缓存，没有就创建出来丢进缓存
			if (i >= talentList.Count) {
				tempTalent = NGUITools.AddChild (talentContent,talentPrefab).GetComponent<CardAttrTalentItem> ();
				tempTalent.transform.localPosition = new Vector3 (0, i * -115, 0);
				tempTalent.transform.localScale = Vector3.one;
				talentList.Add (tempTalent);
			}
			talentList[i].gameObject.SetActive (true);
			talentList[i].text2.text = strs [j];
			if (sample.getTalentNeedTimes () [j] > showCard.getEvoLevel ()) {
				talentList[i].text1.text = "[C65843]" + LanguageConfigManager.Instance.getLanguage ("s0386",sample.getTalentNeedTimes () [j].ToString ());
			} else {
				talentList[i].text1.text = Colors.GREEN + LanguageConfigManager.Instance.getLanguage ("goddess11");
			}
			j++;
		}
		talentBg.height = 25 + 115 * strs.Length;
	}

	public void initJustImage (Card card,CardSample sample,CardPictureWindow win)
	{
		this.sample = sample;
		showCard = card;
		window = win;
		if (showCard == null)
			ResourcesManager.Instance.LoadAssetBundleTexture ("texture/card/1", CardImage);
		else
			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.CARDIMAGEPATH + sample.imageID, CardImage);
	}
	
	public void init (Card card,CardSample sample,CardPictureWindow win)
	{
		this.sample = sample;
		showCard = card;
		window = win;
		if (CardImage != null) {
			if (showCard == null)
				ResourcesManager.Instance.LoadAssetBundleTexture ("texture/card/1", CardImage);
			else
				ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.CARDIMAGEPATH + sample.imageID, CardImage);
		}
		cardName.text = QualityManagerment.getQualityColor (sample.qualityId) + sample.name;
		int jobId = sample.job;
		jobSign.spriteName = CardManagerment.Instance.qualityIconTextToBackGround (jobId) + "s";

		quality.spriteName = QualityManagerment.qualityIDToString (sample.qualityId)+"Bg";
		updateAttributes ();

		cardLv.text = "Lv." + sample.maxLevel + "/" + sample.maxLevel;
		jinhua.text = sample.evoStarLevel+ "/" + card.getMaxEvoLevel ();
		
		updateSkill ();
		loadTalent ();
		//浏览到下一个卡牌的时需要重置容器的位置
		SpringPanel.Begin (panlObj.gameObject,new Vector3(0,0,0),9);
	}
	
	private void updateAttributes ()
	{  
		rushCombat ();
		CardBaseAttribute attr = CardManagerment.Instance.getCardWholeAttr (showCard);
		CardBaseAttribute attr2 = CardManagerment.Instance.getCardAppendEffectNoSuit (showCard);
		hp.text = attr.getWholeHp ().ToString ();
		att.text = attr.getWholeAtt ().ToString ();
		def.text = attr.getWholeDEF ().ToString ();
		mag.text = attr.getWholeMAG ().ToString ();
		dex.text = attr.getWholeAGI ().ToString ();
		if (window != null) {
			hpAdd.text = "";
			attAdd.text = "";
			defAdd.text = "";
			magAdd.text = "";
			dexAdd.text = "";
		} else {
			if (showAttrTime == true) {
				hpAdd.text = " + " + attr2.getWholeHp ().ToString ();
				attAdd.text = " + " + attr2.getWholeAtt ().ToString ();
				defAdd.text = " + " + attr2.getWholeDEF ().ToString ();
				magAdd.text = " + " + attr2.getWholeMAG ().ToString ();
				dexAdd.text = " + " + attr2.getWholeAGI ().ToString ();
			} else {
				
				hpAdd.text = " + Lv." + CardManagerment.Instance.getCardAttrAppendLevel (showCard, AttributeType.hp);
				attAdd.text = " + Lv." + CardManagerment.Instance.getCardAttrAppendLevel (showCard, AttributeType.attack);
				defAdd.text = " + Lv." + CardManagerment.Instance.getCardAttrAppendLevel (showCard, AttributeType.defecse);
				magAdd.text = " + Lv." + CardManagerment.Instance.getCardAttrAppendLevel (showCard, AttributeType.magic);
				dexAdd.text = " + Lv." + CardManagerment.Instance.getCardAttrAppendLevel (showCard, AttributeType.agile);
			}
		}
	}
	
	int oldCombat = 0;//初始战斗力
	int newCombat = 0;//最新战斗力
	int step;//步进跳跃值
	private bool isRefreshCombat = false;//刷新战斗力开关
	
	//刷新战斗力
	public void rushCombat ()
	{
		newCombat = CombatManager.Instance.getCardCombat (showCard);
		isRefreshCombat = true;
		if (newCombat >= oldCombat)
			step = (int)((float)(newCombat - oldCombat) / 20);
		else
			step = (int)((float)(oldCombat - newCombat) / 20);
		if (step < 1)
			step = 1;
	}
	
	private void refreshCombat ()
	{
		if (oldCombat != newCombat) {
			if (oldCombat < newCombat) {
				oldCombat += step;
				if (oldCombat >= newCombat)
					oldCombat = newCombat;
			} else if (oldCombat > newCombat) {
				oldCombat -= step;
				if (oldCombat <= newCombat)
					oldCombat = newCombat;
			}
			combat.text = oldCombat + "";
		} else {
			isRefreshCombat = false;
			combat.text = newCombat + "";
			oldCombat = newCombat;
		}
	}
	
	float time; //属性加成切换显示的时间
	float continueTime;
	public const float SWITCHTIME = 3f;
	bool showAttrTime = false;//true就显示附加属性数据，false显示附加等级
	
	void Update ()
	{
		//没有卡片形象才会有其他的关联
		if (CardImage == null) {
			time -= Time.deltaTime;
			
			if (time <= 0) {
				showAttrTime = !showAttrTime;
				time = SWITCHTIME;
				if(showCard!=null)
					updateAttributes ();
			} 
			
			if (isRefreshCombat) {
				refreshCombat ();
			}
		}
	}
}
