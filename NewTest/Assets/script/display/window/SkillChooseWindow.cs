using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class chooseSkill
{
	public	Skill skill;
	public	Card card;

	public chooseSkill (Card _card, Skill _skill)
	{
		card = _card;
		skill = _skill;
		
	}
}

/// <summary>
/// 已废弃的学习技能窗口
/// </summary>
public class SkillChooseWindow : WindowBase
{

	public UIGrid contentCanLearnSkills;
	public UIGrid contentLearnedSkills;
	public UILabel castMoneyLabel;
	public UITexture roleImage;
	public UILabel roleLevel;
	public	Card role;
	public	Card offering;
	List<chooseSkill> allSkillsNorep;
	List<ButtonSkill> allButtonSkills;
	List<Skill> learnedSkills;
	List<ButtonSkill> ButtonLearnedSkills;
	int  showType;
	public const int TYPE_EVOLUTION = 1;//进化模式
	public const int TYPE_SKILLLEARN = 2;//技能学习模式

	protected override void begin ()
	{
		base.begin ();
		MaskWindow.UnlockUI ();
	}

	public void Initialize (Card _role, Card _offering, string cast, int type)
	{
		showType = type;
		role = _role;
		offering = _offering;
		castMoneyLabel.text = cast;
		roleLevel.text = role.getLevel () + "";
		ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.CARDIMAGEPATH + role.getImageID (), roleImage);
		initAllButtons ();
	}
	
	void initAllButtons ()
	{

		List<chooseSkill> allSkills = new 	List<chooseSkill> ();
		//得主人技能
		Skill[] aSkills = role.getAttrSkills ();
		Skill[] mSkills = role.getSkills ();		
		Skill[] bSkills = role.getBuffSkills ();	
		
		//得祭品技能
		Skill[] aSkills2 = offering.getAttrSkills ();
		Skill[] mSkills2 = offering.getSkills ();		
		Skill[] bSkills2 = offering.getBuffSkills ();	


		if (role.uid == UserManager.Instance.self.mainCardUid) {
			//只有主卡可以学主技能和被动技能
			if (mSkills != null)
				foreach (Skill each in mSkills)
					allSkills.Add (new chooseSkill (role, each));
			if (mSkills2 != null)
				foreach (Skill each in mSkills2)
					allSkills.Add (new chooseSkill (offering, each));
		
			if (bSkills != null)
				foreach (Skill each in bSkills)
					allSkills.Add (new chooseSkill (role, each));
			if (bSkills2 != null)
				foreach (Skill each in bSkills2)
					allSkills.Add (new chooseSkill (offering, each));
		}
		
		if (aSkills != null)
			foreach (Skill each in aSkills)
				allSkills.Add (new chooseSkill (role, each));
		if (aSkills2 != null)
			foreach (Skill each in aSkills2)
				allSkills.Add (new chooseSkill (offering, each));
		
		allSkillsNorep = new List<chooseSkill> ();
		
		//如果有相同sid的技能 .选最高级那个
		foreach (chooseSkill each in allSkills) {
			
			//确认新队列里没有这种技能
			bool has = false;
			foreach (chooseSkill repEach in allSkillsNorep) {
				if (each.skill.sid == repEach.skill.sid) {
					has = true;
					break;
				}
			}
			if (has == true)
				continue;
			
			//老队列中找出最高级的这种技能
			chooseSkill maxSkill = each;
			foreach (chooseSkill each2 in allSkills) {
				if (each.skill.sid == each2.skill.sid && each2.skill.getLevel () > maxSkill.skill.getLevel ()) {
					maxSkill = each2;
				}
			}
	
			allSkillsNorep.Add (maxSkill);
 
		}
		
		
		
		allButtonSkills = new List<ButtonSkill> ();
		
		for (int i=0; i<allSkillsNorep.Count; i++) {
			chooseSkill each = allSkillsNorep [i];
			GameObject m = Create3Dobj ("ui/skillButton").obj;
			ButtonSkill button = m.GetComponent<ButtonSkill> ();
			button.fatherWindow = this;
			button.transform.parent = contentCanLearnSkills.transform;
			button.transform.localScale = Vector3.one;
			button.name = "skillButton_" + i;
			button.initSkillData (each.skill, ButtonSkill.STATE_LEARNED);
			button.owner = each.card;
			button.useInSkillChoose = true;
			allButtonSkills.Add (button);
		}
		
		contentCanLearnSkills.repositionNow = true;
		
		//下面初始化主卡可学技能的按钮:
		
		int[] max = role.getSkillMaxSlot ();
		int bSkillmax = max [0];
		int mSkillmax = max [1];
		int aSkillmax = max [2];
		learnedSkills = new List<Skill> ();
		ButtonLearnedSkills = new List<ButtonSkill> ();
		
		int index = 0;
		
		for (int i=0; i<mSkillmax; i++) {
			index += 1;
			createLearnedButton (SkillStateType.ACTIVE, index);
			learnedSkills.Add (null);
		}

		for (int i=0; i<bSkillmax; i++) {
			index += 1;
			createLearnedButton (SkillStateType.BUFF, index);
			learnedSkills.Add (null);
		}	
 
		for (int i=0; i<aSkillmax; i++) {
			index += 1;
			createLearnedButton (SkillStateType.ATTR, index);
			learnedSkills.Add (null);
		}	
		
		contentLearnedSkills.repositionNow = true;
	}

	void createLearnedButton (int type, int index)
	{

		GameObject m = Create3Dobj ("ui/skillButton").obj;
		ButtonSkill button = m.GetComponent<ButtonSkill> ();
		button.fatherWindow = this;
		button.transform.parent = contentLearnedSkills .transform;
		button.transform.localScale = Vector3.one;
		button.name = "skillButton_" + StringKit.intToFixString (index);
		button.initSkillData (null, ButtonSkill.STATE_CANLEARN, type);
		ButtonLearnedSkills.Add (button);
	}

	public void learnOver (string id)
	{ 
		UiManager.Instance.openWindow<CardBookWindow>((cardwin)=>{
			cardwin.init (role, CardBookWindow.CONTINUE, showLearnSkillWindow);
		});
		UiManager.Instance.destoryWindowByName ("effectBlackWindow");
		return;
	}

	public void evolutionOver (string id)
	{
		UiManager.Instance.openWindow<CardBookWindow>((cardwin)=>{
			cardwin.init (StorageManagerment.Instance.getRole (id), CardBookWindow.CONTINUE, showEvolutionWindow);
		});
		UiManager.Instance.destoryWindowByName ("effectBlackWindow");
		return;
	}
	
	private void showEvolutionWindow ()
	{
//		UiManager.Instance.openWindow<EvolutionWindow>((win)=>{
//			win.reloadAfterEvolution (); 
//		});
	}
		
	private void showLearnSkillWindow ()
	{
		UiManager.Instance.openWindow<LearningSkillWindow>((win)=>{
			win.reloadAfterLearning (); 
		});
	}
	
	
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
			finishWindow();
			if (showType == TYPE_SKILLLEARN) {
				UiManager.Instance.openWindow<IntensifyCardWindow>((intenWin)=>{
					IntensifyCardManager.Instance.setMainCard(role);
					intenWin.updateInfo();
				});
				return;
			} else {
				UiManager.Instance.openWindow<IntensifyCardWindow>();
				return;			
			}
		}	
		
		if (gameObj.name == "buttonLearn") {

			if (showType == TYPE_SKILLLEARN) {
				
				//检查技能列表，不允许学习技能列表为空
				bool hasSkill = false;
				foreach (ButtonSkill each in 	ButtonLearnedSkills) {
					if (each.skillData != null) {
						hasSkill = true;
						break;
					}
				}	
				if (hasSkill == false) {
					UiManager.Instance.openDialogWindow<MessageWindow>((win)=>{
						win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0093"), "", LanguageConfigManager.Instance.getLanguage ("s0174"), null);
					});
					return;
				}
				UiManager.Instance.openWindow<EffectBlackWindow>((win222) => {
					win222.playLearnSkillEffect (role, offering, sendLearnData);
				});
				return;
				
			} else {
				UiManager.Instance.openWindow<EffectBlackWindow>((win222) => {
					win222.playEvoEffect (role, offering, sendEvoData);
				});
				return;
				
		
				
			}
		}
		
		
		//如果是可以学习的技能池里的技能按钮
		if (gameObj.transform.parent != null && gameObj.transform.parent.name == contentCanLearnSkills.name) {
			
			moveToLearn (gameObj.GetComponent<ButtonSkill> ());
			return;
		}
		//如果是已经学习的技能池里的技能按钮
		if (gameObj.transform.parent != null && gameObj.transform.parent.name == contentLearnedSkills .name) {
			
			removeFromLearn (gameObj.GetComponent<ButtonSkill> ());
			return;	
		}		
		
		
		
		
	}

	void sendEvoData ()
	{
		CardEvolveFPort ces = FPortManager.Instance.getFPort ("CardEvolveFPort") as CardEvolveFPort;
		CardIntensifyData data = new CardIntensifyData ();
		foreach (ButtonSkill each in 	ButtonLearnedSkills) {
			if (each.skillData == null || each.owner == null)
				continue;
			data.addFood (each.owner.uid, each.skillData.sid);
			
		}
		ces.access (role.uid, offering.uid, data.ToFooding (), evolutionOver);
				
	}

	void sendLearnData ()
	{
		CardStudySkillsFPort css = FPortManager.Instance.getFPort ("CardStudySkillsFPort") as CardStudySkillsFPort;
		CardIntensifyData data = new CardIntensifyData ();
		foreach (ButtonSkill each in 	ButtonLearnedSkills) {
			if (each.skillData == null || each.owner == null)
				continue;
			data.addFood (each.owner.uid, each.skillData.sid);
			
		}
		//css.access (role.id, offering.id, data.ToFooding (), learnOver);
	}

	void moveToLearn (ButtonSkill button)
	{
		if (button.skillData == null) 
			return;	
		foreach (ButtonSkill each in ButtonLearnedSkills) {
			//如果是空而且类型同，才能转移过去
			if (each.skillType == button.skillData.getSkillStateType () && each.skillData == null) {
				button.gameObject.SetActive (false);
				contentCanLearnSkills.repositionNow = true;
				each.initSkillData (button.skillData, ButtonSkill.STATE_LEARNED);
				each.owner = button.owner;
				break;
			}
		}

	}

	void removeFromLearn (ButtonSkill button)
	{
		Skill data = button.skillData;
		if (data == null) 
			return;
		
		button.initSkillData (null, ButtonSkill.STATE_CANLEARN, button.skillData.getSkillStateType ());
		button.owner = null;
		foreach (ButtonSkill each in allButtonSkills) {
			if (each.skillData == data) {
				each.gameObject.SetActive (true);
				contentCanLearnSkills.repositionNow = true;
				break;
			}
		}
	}
	
	
}
