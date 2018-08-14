using UnityEngine;
using System.Collections;

/// <summary>
/// 召唤兽属性升级信息
/// </summary>
public class BeastAttrLevelInfo : WindowBase
{
	/** 等级 */
	public UILabel levelBeforeValue;
	public UILabel levelAfterValue;
	public UILabel levelAddedValue;
	/** 生命 */
	public UILabel lifeBeforeValue;
	public UILabel lifeAfterValue;
	public UILabel lifeAddedValue;
	/** 攻击 */
	public UILabel attackBeforeValue;
	public UILabel attackAfterValue;
	public UILabel attackAddedValue;
	/** 防御 */
	public UILabel defendBeforeValue;
	public UILabel defendAfterValue;
	public UILabel defendAddedValue;
	/** 魔力 */
	public UILabel magicBeforeValue;
	public UILabel magicAfterValue;
	public UILabel magicAddedValue;
	/** 敏捷 */
	public UILabel agileBeforeValue;
	public UILabel agileAfterValue;
	public UILabel agileAddedValue;
	/** 关闭提示 */
	public UILabel closeLabel;
	private float time = 0;
	/** 等级上限增量值 */
	int levelAdded;
	/** 生命增量值 */
	int lifeAdded;
	/** 攻击增量值 */
	int attackAdded;
	/** 防御增量值 */
	int defendAdded;
	/** 魔力增量值 */
	int magicAdded;
	/** 敏捷增量值 */
	int agileAdded;
	/** 回调 */
	CallBack callback;
	/** 主技能 */
	public ButtonSkill beforeMainSkill;
	public ButtonSkill afterMainSkill;
	public UILabel beforeMainSkillLv;
	public UILabel afterMainSkillLv;
	/** update帧 */
	int setp;
	int nextSetp;
	/** 召唤兽老经验 */
	private long exp;
	
	public void Initialize (Card beforeCard,Card afterCard,long exp,CallBack callback)
	{
		CardBaseAttribute beforeEvoAttr = CardManagerment.Instance.getCardWholeAttr (beforeCard);
		CardBaseAttribute afterEvoAttr = CardManagerment.Instance.getCardWholeAttr (afterCard);
		this.callback = callback;
		this.exp = exp;
		computeAddedValue (beforeCard,beforeEvoAttr,afterCard,afterEvoAttr);
		showUI(beforeCard,beforeEvoAttr,afterCard,afterEvoAttr);
	}
	protected override void begin ()
	{
		base.begin ();
		StartCoroutine (Utils.DelayRun (() => {
			NextSetp ();}, 0.2f));
		MaskWindow.UnlockUI();
	}
	/** 计算属性附加数据 */
	void computeAddedValue(Card beforeCard,CardBaseAttribute beforeEvoAttr,Card afterCard,CardBaseAttribute afterEvoAttr)
	{
		levelAdded=afterCard.getMaxLevel ()-beforeCard.getMaxLevel ();
		lifeAdded=afterEvoAttr.getWholeHp ()-beforeEvoAttr.getWholeHp ();
		attackAdded=afterEvoAttr.getWholeAtt ()-beforeEvoAttr.getWholeAtt ();
		defendAdded=afterEvoAttr.getWholeDEF ()-beforeEvoAttr.getWholeDEF ();
		magicAdded=afterEvoAttr.getWholeMAG ()-beforeEvoAttr.getWholeMAG ();
		agileAdded=afterEvoAttr.getWholeAGI ()-beforeEvoAttr.getWholeAGI ();
	}
	public void showUI(Card beforeCard,CardBaseAttribute beforeEvoAttr,Card afterCard,CardBaseAttribute afterEvoAttr)
	{
		showLevelValue (beforeCard.getMaxLevel (), beforeCard.getMaxLevel ());
		showLifeValue(beforeEvoAttr.getWholeHp (),beforeEvoAttr.getWholeHp ());
		showAttackValue(beforeEvoAttr.getWholeAtt (),beforeEvoAttr.getWholeAtt ());
		showDefendValue(beforeEvoAttr.getWholeDEF (),beforeEvoAttr.getWholeDEF ());
		showMagicValue(beforeEvoAttr.getWholeMAG (),beforeEvoAttr.getWholeMAG ());	
		showAgileValue(beforeEvoAttr.getWholeAGI (),beforeEvoAttr.getWholeAGI ());
		showMainSkill (beforeCard,afterCard);
	}
	void showMainSkill(Card beforeCard,Card afterCar)
	{
		initBeforeSkill (beforeCard);
		initAfterSkill (afterCar);
	}
	private void initBeforeSkill (Card _card)
	{ 
		Skill[] mSkill = _card .getSkills ();
		int mainSkillLv = EXPSampleManager.Instance.getLevel(EXPSampleManager.SID_HALLOW_EXP,exp);
		if (mSkill == null || mSkill [0] == null)
			return;
		if(mainSkillLv == 0)
			mainSkillLv = 1;
		if(mainSkillLv >= mSkill [0].getMaxLevel())
			mainSkillLv = mSkill [0].getMaxLevel();
		beforeMainSkillLv.text = "Lv." + mainSkillLv;
		string mainSkillDescript = mSkill [0].getDescribeByLv(mainSkillLv);
		beforeMainSkill.initBeastSkill(mSkill [0], ButtonSkill.STATE_BEAST,mSkill[0].getName(),mainSkillDescript,exp,EXPSampleManager.SID_HALLOW_EXP,beforeMainSkillLv.text);
	}
	private void initAfterSkill (Card _card)
	{
		Skill[] mSkill = _card .getSkills ();
		int mainSkillLv = EXPSampleManager.Instance.getLevel(EXPSampleManager.SID_HALLOW_EXP,exp);
		if (mSkill == null || mSkill [0] == null)
			return;
		if(mainSkillLv == 0)
			mainSkillLv = 1;
		if(mainSkillLv >= mSkill [0].getMaxLevel())
			mainSkillLv = mSkill [0].getMaxLevel();
		afterMainSkillLv.text = "Lv." + mainSkillLv;
		string mainSkillDescript = mSkill [0].getDescribeByLv(mainSkillLv);
		afterMainSkill.initBeastSkill(mSkill [0], ButtonSkill.STATE_BEAST,mSkill[0].getName(),mainSkillDescript,exp,EXPSampleManager.SID_HALLOW_EXP,afterMainSkillLv.text);
	}
	void showLevelValue(int beforeValue,int afterValue)
	{
		levelBeforeValue.text=beforeValue.ToString();
		levelAfterValue.text=afterValue.ToString();
		levelAddedValue.text="+"+levelAdded.ToString();
	}
	void showLifeValue(int beforeValue,int afterValue)
	{
		lifeBeforeValue.text=beforeValue.ToString();
		lifeAfterValue.text=afterValue.ToString();
		lifeAddedValue.text="+"+lifeAdded.ToString();
	}
	void showAttackValue(int beforeValue,int afterValue)
	{
		attackBeforeValue.text=beforeValue.ToString();
		attackAfterValue.text=afterValue.ToString();
		attackAddedValue.text="+"+attackAdded.ToString();
	}
	void showDefendValue(int beforeValue,int afterValue)
	{
		defendBeforeValue.text=beforeValue.ToString();
		defendAfterValue.text=afterValue.ToString();
		defendAddedValue.text="+"+defendAdded.ToString();
	}
	void showMagicValue(int beforeValue,int afterValue)
	{
		magicBeforeValue.text=beforeValue.ToString();
		magicAfterValue.text=afterValue.ToString();
		magicAddedValue.text="+"+magicAdded.ToString();
	}
	void showAgileValue(int beforeValue,int afterValue)
	{
		agileBeforeValue.text=beforeValue.ToString();
		agileAfterValue.text=afterValue.ToString();
		agileAddedValue.text="+"+agileAdded.ToString();
	}
	void Update ()
	{
		if(closeLabel.gameObject.activeSelf)
		{
			float offset = Mathf.Sin (time * 6); 
			closeLabel.alpha =sin();
		}
		if (setp == nextSetp)
			return;
		//评级
		if (setp == 0) {
			bool isShowEffect=showFlashEffect();
			if(isShowEffect) {
				StartCoroutine(Utils.DelayRun(()=>{
					NextSetp();},0.15f));
			}
			else {
				NextSetp ();
			}
		}
		setp++;
	}
	/** 显示增加属性数值后爆开的特效 */
	bool showFlashEffect()
	{
		bool isShowEffect = false;
		if(levelAdded>0){
			createFlashEffect(levelAddedValue.gameObject);
			isShowEffect=true;
		}
		if(lifeAdded>0){
			createFlashEffect(lifeAddedValue.gameObject);
			isShowEffect=true;
		}
		if(attackAdded>0){
			createFlashEffect(attackAddedValue.gameObject);
			isShowEffect=true;
		}
		if(defendAdded>0){
			createFlashEffect(defendAddedValue.gameObject);
			isShowEffect=true;
		}
		if(magicAdded>0){
			createFlashEffect(magicAddedValue.gameObject);
			isShowEffect=true;
		}
		if(agileAdded>0){
			createFlashEffect(agileAddedValue.gameObject);
			isShowEffect=true;
		}
		return isShowEffect;
	}
	/** 创建爆开的特效 */
	void createFlashEffect(GameObject effectPoint)
	{
		GameObject obj = Create3Dobj ("Effect/Other/Flash").obj;
		obj.transform.parent=effectPoint.transform;
		obj.transform.localScale=Vector3.one;
		obj.transform.localPosition=new Vector3(0,0,-600);
	}
	void showFlashEffect(GameObject effectPoint)
	{
		GameObject obj = Create3Dobj ("Effect/Other/Flash").obj;
		obj.transform.parent=effectPoint.transform;
		obj.transform.localScale=Vector3.one;
		obj.transform.localPosition=new Vector3(0,0,-600);
	}
	void updateCloseLabelEffect()
	{
		if(closeLabel.gameObject.activeSelf)
		{
			float offset = Mathf.Sin (time * 6); 
			closeLabel.alpha =sin();
		}
	}
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
			finishWindow();
			EventDelegate.Add (OnHide, () => {	
				fatherWindow.finishWindow();
			});
			if (callback != null) {
				callback ();
				callback=null;
			}
		}
	}
	public void NextSetp ()
	{
		nextSetp++;
	}
}
