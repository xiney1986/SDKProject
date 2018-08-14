using UnityEngine;
using System.Collections;

public class CardAttrItem : MonoBehaviour {

	public WindowBase fawin;
	/** 卡片形象 */
	public RoleView roleView;
	/** 等级上限 */
	public UILabel labelLevel;
	/** 进化等级 */
	public UILabel labelEvoLevel;
	/** 战斗力 */
	public UILabel labelCombat;
	/** 生命 */
	public UILabel labelHp,labelHpAdd;
	/** 攻击力 */
	public UILabel labelAtt,labelAttAdd;
	/** 防御 */
	public UILabel labelDef,labelDefAdd;
	/** 魔力 */
	public UILabel labelMag,labelMagAdd;
	/** 敏捷 */
	public UILabel labelAgi,labelAgiAdd;
	/** 技能 */
	public ButtonSkill mainSkillButton;

	Card showCard;//展示卡片
	CardBaseAttribute attrAdd;//附加属性值
	float time; //属性加成切换显示的时间
	float SWITCHTIME = 3f;//循环时间
	bool showAttrTime = false;//true就显示附加属性数据，false显示附加等级

	public void initUI (Card _showCard)
	{
		initUI (_showCard, "");
	}

	public void initUI (Card _showCard, string _color)
	{
		if (_showCard == null) {
			return;
		}
		this.showCard = _showCard;

		if (roleView != null) {
			roleView.init (showCard,null,null);
		}
		if (labelLevel != null) {
			labelLevel.text = "" + showCard.getMaxLevel ();
		}
		if (labelEvoLevel != null) {
			labelEvoLevel.text =showCard.getEvoLevel () + " / " + showCard.getMaxEvoLevel ();
		}
		if (labelCombat != null) {
			labelCombat.text = showCard.getCardCombat () + "";
		}

		CardBaseAttribute attr = CardManagerment.Instance.getCardWholeAttr (showCard);

		labelHp.text =  attr.getWholeHp ().ToString();
		labelAtt.text =  attr.getWholeAtt ().ToString();
		labelDef.text = attr.getWholeDEF ().ToString();
		labelMag.text = attr.getWholeMAG ().ToString();
		labelAgi.text =  attr.getWholeAGI ().ToString();

		attrAdd = CardManagerment.Instance.getCardAppendEffectNoSuit (showCard);

		Skill[] mainSkil = showCard.getSkills ();
		Skill[] buffSkill = showCard.getBuffSkills ();
		if (buffSkill != null && mainSkil == null) {		
			mainSkillButton.initSkillData (buffSkill [0], ButtonSkill.STATE_LEARNED);
		} else if (buffSkill == null && mainSkil != null) {
			mainSkillButton.initSkillData (mainSkil [0], ButtonSkill.STATE_LEARNED);
		}
	}

	void SwitchShow ()
	{
		if (attrAdd == null) {
			return;
		}
		if (showAttrTime == true) {
			
			labelHpAdd.text = attrAdd.getWholeHp () == 0 ? "" : ( " + " + attrAdd.getWholeHp ());
			labelAttAdd.text = attrAdd.getWholeAtt () == 0 ? "" : (" + " + attrAdd.getWholeAtt ());
			labelDefAdd.text = attrAdd.getWholeDEF () == 0 ? "" : ( " + " + attrAdd.getWholeDEF ());
			labelMagAdd.text = attrAdd.getWholeMAG () == 0 ? "" : ( " + " + attrAdd.getWholeMAG ());
			labelAgiAdd.text = attrAdd.getWholeAGI () == 0 ? "" : ( " + " + attrAdd.getWholeAGI ());
		} else {
			int hpl = CardManagerment.Instance.getCardAttrAppendLevel (showCard, AttributeType.hp);
			labelHpAdd.text = hpl == 0 ? "" : ( " + Lv." + hpl);
			
			int attl = CardManagerment.Instance.getCardAttrAppendLevel (showCard, AttributeType.attack);
			labelAttAdd.text = attl == 0 ? "" : (" + Lv." + attl);
			
			int defl = CardManagerment.Instance.getCardAttrAppendLevel (showCard, AttributeType.defecse);
			labelDefAdd.text = defl == 0 ? "" : ( " + Lv." + defl);
			
			int magl = CardManagerment.Instance.getCardAttrAppendLevel (showCard, AttributeType.magic);
			labelMagAdd.text = magl == 0 ? "" : ( " + Lv." + magl);
			
			int dexl = CardManagerment.Instance.getCardAttrAppendLevel (showCard, AttributeType.agile);
			labelAgiAdd.text = dexl == 0 ? "" : ( " + Lv." + dexl);
		}
	}

	void Update ()
	{
		if (showCard != null && attrAdd != null) {
			time -= Time.deltaTime;
			
			if (time <= 0) {
				showAttrTime = !showAttrTime;
				time = SWITCHTIME;
				SwitchShow ();
			}
		}
	}
}
