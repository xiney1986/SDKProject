using UnityEngine;
using System.Collections;
/**
 * 学习技能窗口
 * @author 杨小珑
 **/
public class LearnSkillWindow : WindowBase 
{
	public RoleView roleViewMain;
	public RoleView roleViewFood;
	public ButtonBase buttonSetFood; //选择副卡按钮
	public ButtonSkill skillButtonFood; //选择好的学习技能
	public ButtonSkill skillButtonMain; //准备替换的技能
	public UILabel userMoney; //持有
	public UILabel castMoney; //消耗
	public UILabel lblExplanation; //说明
	public ButtonBase buttonOk; //确定按钮
//	public GameObject mask; //遮罩,用于动画期间不能操作

	Card mainCard;
	Card foodCard;
	Skill mainSkill;
	int skillType;
	Skill foodSkill;

	public override void OnAwake ()
	{
		base.OnAwake ();
		buttonOk.disableButton(true);
	}
	protected override void begin ()
	{
		base.begin ();
		GuideManager.Instance.guideEvent();
		MaskWindow.UnlockUI ();
	}

	public void init(Card mainCard,Skill mainSkill,int skillType)
	{
		this.mainCard = mainCard;
		this.mainSkill = mainSkill;
		this.skillType = skillType;
		buttonOk.disableButton(true);
		roleViewMain.init (mainCard, this, null);
		skillButtonMain.initSkillData (mainSkill, ButtonSkill.STATE_CANLEARN,skillType);
		UpdateMoney ();
	}

	void UpdateMoney()
	{
		lblExplanation.text = LanguageConfigManager.Instance.getLanguage("s0419");
		userMoney.text = UserManager.Instance.self.getMoney ().ToString();
		if (foodCard == null) {
			castMoney.text = "0";
		} else {
			castMoney.text = foodCard.getCardSkillLearnCast().ToString();
		}
	}

	bool isCanLearn()
	{
		if(foodCard == null)
			return false;
		else
			return UserManager.Instance.self.getMoney () >= foodCard.getCardSkillLearnCast();
	}

	void sendLearnData ()
	{

		CardStudySkillsFPort css = FPortManager.Instance.getFPort ("CardStudySkillsFPort") as CardStudySkillsFPort;
		css.access (mainCard.uid, foodCard.uid,skillType,mainSkill == null ? 0 : mainSkill.sid, foodSkill.sid, learnOver);
	}

	void learnOver(string str)
	{
		if (str != null) {
			MessageWindow.ShowAlert(str);
			return;
		}
		TweenPosition tp = TweenPosition.Begin(skillButtonFood.gameObject,0.4f,skillButtonMain.transform.localPosition);
		EventDelegate.Add(tp.onFinished,()=>{
			EffectCtrl effect = EffectManager.Instance.CreateEffect (skillButtonFood.gameObject.transform, "Effect/Other/player_shengji");
			effect.transform.localPosition += new Vector3 (0, 0, -5000);
			effect.transform.GetChild(0).localPosition = Vector3.zero;
			StartCoroutine(Utils.DelayRun(backToCardAttrWindow,2f));
		},true);
	}

	void backToCardAttrWindow()
	{
		finishWindow();
		TextTipWindow.Show (LanguageConfigManager.Instance.getLanguage("s0401"));
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "buttonSetFood") {
			OnRoleViewFoodClick (null);
		} else if (gameObj.name == "close") {
			finishWindow();
		} else if (gameObj.name == "buttonLearn") {

			//不能重复学习相同的技能
			if(mainSkill != null && mainSkill.sid == foodSkill.sid)
			{
				MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("s0404"));
				return;
			}
			if(mainSkill != null)
			{
				string str = LanguageConfigManager.Instance.getLanguage("s0403");
				str = string.Format(str,mainSkill.getName(),mainSkill.getLevel(),foodSkill.getName(),foodSkill.getLevel());
				MessageWindow.ShowConfirm(str,(msg)=>{
					if(msg.buttonID == MessageHandle.BUTTON_RIGHT){
					//	mask.SetActive(true);
						sendLearnData();
					}
				});
				return;
			}

			skillButtonMain.gameObject.SetActive(false);
			sendLearnData();
		}
	}



	public void resultSelect(Card card,Skill skill)
	{
		this.foodSkill = skill;
		this.foodCard = card;

		buttonSetFood.gameObject.SetActive (false);
		roleViewFood.gameObject.SetActive (true);
		skillButtonFood.gameObject.SetActive (true);
		roleViewFood.init (card, this, OnRoleViewFoodClick);
		skillButtonFood.initSkillData (skill, ButtonSkill.STATE_LEARNED);

		if(isCanLearn())
			buttonOk.disableButton(false);
		else
			buttonOk.disableButton(true);

		UpdateMoney ();
	}



	void OnRoleViewFoodClick(RoleView roleView)
	{
        UiManager.Instance.openWindow<LearnSkillSelectWindow>((win)=>{
            win.learnWindow = this;
            win.init (mainCard, skillType);
        });
	}
}
