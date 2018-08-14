using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/**
 * 学习技能选择副卡技能窗口
 * @author 杨小珑
 **/
public class LearnSkillSelectWindow : WindowBase {

	public learningSkillSelectContent content;

	[HideInInspector]
	public LearnSkillWindow learnWindow;

	Card mainCard;
	int skillStateType;
	ArrayList cardList;
	Dictionary<Card,List<Skill>> map;

	protected override void begin ()
	{
		base.begin ();
		content.reLoad (map);
		MaskWindow.UnlockUI ();
	}

	public void init(Card mainCard,int skillStateType)
	{
		this.mainCard = mainCard;
		this.skillStateType = skillStateType;
		map = new Dictionary<Card, List<Skill>> ();
		cardList = new ArrayList ();

		
		//筛选出可用卡片和技能
		List<string> armyIds =  ArmyManager.Instance.recalculateAllArmyIds ();
		ArrayList list = StorageManagerment.Instance.getAllRole ();
		list = SortManagerment.Instance.cardSort (list, SortConditionManagerment.Instance.initDefaultSort(SiftWindowType.SIFT_CARDSTORE_WINDOW));
		foreach (object obj in list) {
			Card card = obj as Card;
			if(card.uid == UserManager.Instance.self.mainCardUid)
				continue;
			if(card.uid == mainCard.uid)
				continue;
			if(armyIds.Contains (card.uid))
				continue;
			List<Skill> skillList = new List<Skill>();
			if(skillStateType == SkillStateType.BUFF)
			{
				Skill[] ss = card.getBuffSkills();
				for(int i = 0; ss != null && i < ss.Length; i++)
				{
					skillList.Add(ss[i]);
				}
			}
			else if(skillStateType == SkillStateType.ACTIVE)
			{
				Skill[] ss = card.getSkills();
				for(int i = 0; ss != null && i < ss.Length; i++)
				{
					skillList.Add(ss[i]);
				}
			}
			else if(skillStateType == SkillStateType.ATTR)
			{
				Skill[] ss = card.getAttrSkills();
				for(int i = 0; ss != null && i < ss.Length; i++)
				{
					skillList.Add(ss[i]);
				}
			}
			if(skillList.Count > 0)
			{
				map.Add(card,skillList);
			}
		}
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
			finishWindow();
		}
	}


	public void OnItemSelect(Card card,Skill skill)
	{
		learnWindow.resultSelect (card, skill);
		finishWindow ();
	}
}
