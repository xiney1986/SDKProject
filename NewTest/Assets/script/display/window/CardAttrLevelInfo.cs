using UnityEngine;
using System.Collections;

/// <summary>
/// 卡片属性升级信息
/// </summary>
public class CardAttrLevelInfo : WindowBase
{

	/** 觉醒描述 */
	public UILabel talentString;
	/** 觉醒对象 */
	public GameObject awaken;
	/** 等级 */
	public UILabel levelBeforeValue;
	public UILabel levelAfterValue;
	public UILabel levelAddValue;
	/** 战斗力 */
	public UILabel combatBeforeValue;
	public UILabel combatAfterValue;
	public UILabel combatAddValue;
	/** 生命 */
	public UILabel lifeBeforeValue;
	public UILabel lifeAfterValue;
	public UILabel lifeAddValue;
	/** 攻击 */
	public UILabel attackBeforeValue;
	public UILabel attackAfterValue;
	public UILabel attackAddValue;
	/** 防御 */
	public UILabel defendBeforeValue;
	public UILabel defendAfterValue;
	public UILabel defendAddValue;
	/** 魔力 */
	public UILabel magicBeforeValue;
	public UILabel magicAfterValue;
	public UILabel magicAddValue;
	/** 敏捷 */
	public UILabel agileBeforeValue;
	public UILabel agileAfterValue;
	public UILabel agileAddValue;
	/** 关闭提示 */
	public UILabel closeLabel;
	private float time = 0;
	/** 等级上限增量值 */
	int levelAdded;
	/** 战斗力增量值 */
	int combatAdded;
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
	/** update帧 */
	int setp;
	int nextSetp;

	public void Initialize (Card beforeCard, Card afterCard, CallBack callback)
	{
		CardBaseAttribute beforeEvoAttr = CardManagerment.Instance.getCardWholeAttr (beforeCard);	
		CardBaseAttribute afterEvoAttr = CardManagerment.Instance.getCardWholeAttr (afterCard);
		this.callback = callback;
		computeAddedValue (beforeCard, beforeEvoAttr, afterCard, afterEvoAttr);
		showUI (beforeCard, beforeEvoAttr, afterCard, afterEvoAttr);
	}

	protected override void begin ()
	{
		base.begin ();
		StartCoroutine (Utils.DelayRun (() => {
			NextSetp ();
		}, 0.2f));
		MaskWindow.UnlockUI();
	}
	/** 计算属性附加数据 */
	void computeAddedValue (Card beforeCard, CardBaseAttribute beforeEvoAttr, Card afterCard, CardBaseAttribute afterEvoAttr)
	{
		levelAdded = afterCard.getMyMaxLevel () - beforeCard.getMyMaxLevel ();
		combatAdded = afterCard.getCardCombat () - beforeCard.getCardCombat ();
		lifeAdded = afterEvoAttr.getWholeHp () - beforeEvoAttr.getWholeHp ();
		attackAdded = afterEvoAttr.getWholeAtt () - beforeEvoAttr.getWholeAtt ();
		defendAdded = afterEvoAttr.getWholeDEF () - beforeEvoAttr.getWholeDEF ();
		magicAdded = afterEvoAttr.getWholeMAG () - beforeEvoAttr.getWholeMAG ();
		agileAdded = afterEvoAttr.getWholeAGI () - beforeEvoAttr.getWholeAGI ();
	}

	public void showUI (Card beforeCard, CardBaseAttribute beforeEvoAttr, Card afterCard, CardBaseAttribute afterEvoAttr)
	{
        showLevelValue(beforeCard.getMyMaxLevel(), beforeCard.getMyMaxLevel());
		showCombatValue (beforeCard.getCardCombat (), beforeCard.getCardCombat ());
		showLifeValue (beforeEvoAttr.getWholeHp (), beforeEvoAttr.getWholeHp ());
		showAttackValue (beforeEvoAttr.getWholeAtt (), beforeEvoAttr.getWholeAtt ());
		showDefendValue (beforeEvoAttr.getWholeDEF (), beforeEvoAttr.getWholeDEF ());
		showMagicValue (beforeEvoAttr.getWholeMAG (), beforeEvoAttr.getWholeMAG ());	
		showAgileValue (beforeEvoAttr.getWholeAGI (), beforeEvoAttr.getWholeAGI ());
		showAwaken (afterCard,beforeCard);
	}

	void showLevelValue (int beforeValue, int afterValue)
	{
		levelBeforeValue.text = "Lv."+beforeValue.ToString ();
		levelAfterValue.text = afterValue.ToString ()  ;
		levelAddValue.text ="+"+ levelAdded.ToString ();
	}

	void showCombatValue (int beforeValue, int afterValue)
	{
		combatBeforeValue.text = beforeValue.ToString ();
		combatAfterValue.text = afterValue.ToString () ;
		combatAddValue.text = "+"+combatAdded.ToString ();
	}

	void showLifeValue (int beforeValue, int afterValue)
	{
		lifeBeforeValue.text = beforeValue.ToString ();
		lifeAfterValue.text = afterValue.ToString () ;
		lifeAddValue.text =  "+"+lifeAdded.ToString ();
	}

	void showAttackValue (int beforeValue, int afterValue)
	{
		attackBeforeValue.text = beforeValue.ToString ();
		attackAfterValue.text = afterValue.ToString () ;
		attackAddValue.text = "+"+attackAdded.ToString ();
	}

	void showDefendValue (int beforeValue, int afterValue)
	{
		defendBeforeValue.text = beforeValue.ToString ();
		defendAfterValue.text = afterValue.ToString () ;
		defendAddValue.text = "+"+defendAdded.ToString ();
	}

	void showMagicValue (int beforeValue, int afterValue)
	{
		magicBeforeValue.text = beforeValue.ToString ();
		magicAfterValue.text = afterValue.ToString ();
		magicAddValue.text =  "+"+magicAdded.ToString ();
	}

	void showAgileValue (int beforeValue, int afterValue)
	{
		agileBeforeValue.text = beforeValue.ToString ();
		agileAfterValue.text = afterValue.ToString () ;
		agileAddValue.text =  "+"+agileAdded.ToString ();
	}

	void showAwaken (Card afterCard,Card beforeCard)
	{
		if (EvolutionManagerment.Instance.isOpenTalentByThisEvoLv (afterCard,beforeCard.getEvoLevel())) {
			awaken.gameObject.SetActive (true);
			talentString.text = EvolutionManagerment.Instance.getOpenTalentString (afterCard,beforeCard.getEvoLevel());
		}
	}

	void Update ()
	{
		if (closeLabel.gameObject.activeSelf) {
			float offset = Mathf.Sin (time * 6); 
			closeLabel.alpha = sin ();
		}
		if (setp == nextSetp)
			return;
		//评级
		if (setp == 0) {
			bool isShowEffect = showFlashEffect ();
			if (isShowEffect) {
				StartCoroutine (Utils.DelayRun (() => {
					NextSetp ();}, 0.15f));
			} else {
				NextSetp ();
			}
		}
		setp++;
	}
	/** 显示增加属性数值后爆开的特效 */
	bool showFlashEffect ()
	{
		bool isShowEffect = false;
		if (levelAdded > 0) {
			createFlashEffect (levelAfterValue.gameObject);
			isShowEffect = true;
		}
		if (combatAdded > 0) {
			createFlashEffect (combatAfterValue.gameObject);
			isShowEffect = true;
		}
		if (lifeAdded > 0) {
			createFlashEffect (lifeAfterValue.gameObject);
			isShowEffect = true;
		}
		if (attackAdded > 0) {
			createFlashEffect (attackAfterValue.gameObject);
			isShowEffect = true;
		}
		if (defendAdded > 0) {
			createFlashEffect (defendAfterValue.gameObject);
			isShowEffect = true;
		}
		if (magicAdded > 0) {
			createFlashEffect (magicAfterValue.gameObject);
			isShowEffect = true;
		}
		if (agileAdded > 0) {
			createFlashEffect (agileAfterValue.gameObject);
			isShowEffect = true;
		}
		return isShowEffect;
	}
	/** 创建爆开的特效 */
	void createFlashEffect (GameObject effectPoint)
	{
		GameObject obj = Create3Dobj ("Effect/Other/Flash").obj;
		obj.transform.parent = effectPoint.transform;
		obj.transform.localScale = Vector3.one;
		obj.transform.localPosition = new Vector3 (0, 0, -600);
	}

	void showFlashEffect (GameObject effectPoint)
	{
		GameObject obj = Create3Dobj ("Effect/Other/Flash").obj;
		obj.transform.parent = effectPoint.transform;
		obj.transform.localScale = Vector3.one;
		obj.transform.localPosition = new Vector3 (0, 0, -600);
	}

	void updateCloseLabelEffect ()
	{
		if (closeLabel.gameObject.activeSelf) {
			float offset = Mathf.Sin (time * 6); 
			closeLabel.alpha = sin ();
		}
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
			finishWindow ();
			EventDelegate.Add (OnHide, () => {	
				EffectBlackWindow effectBlackWindow = fatherWindow as EffectBlackWindow;
				effectBlackWindow.hideEvoComponent ();
				effectBlackWindow.finishWindow ();
			});
			if (callback != null) {
				callback ();
				callback = null;
			}
		}
	}

	public void NextSetp ()
	{
		nextSetp++;
	}
}
