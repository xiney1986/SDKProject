using UnityEngine;
using System.Collections.Generic;

public class LearningSkillSelectItem : MonoBehaviour {

	public RoleView roleView;
	public ButtonSkill[] buttons;
	public LearnSkillSelectWindow window;

	public void init(Card card, List<Skill> skillList)
	{
		roleView.init (card, window, null);
		for (int i = 0; i < buttons.Length; i++) {
			if(i < skillList.Count)
			{
				buttons[i].gameObject.SetActive(true);
				buttons[i].initSkillData(skillList[i],ButtonSkill.STATE_LEARNED);
				buttons[i].OnLongPassCallback = OnItemLongPass;
			}
			else
			{
				buttons[i].gameObject.SetActive(false);
			}
		}
	}
	
	void OnItemClick(GameObject obj)
	{
		if (UiManager.Instance.isWindowShowByName ("skillInfoWindow"))
			return;
		ButtonSkill button = obj.GetComponent<ButtonSkill> ();
		window.OnItemSelect (roleView.card, button.skillData);
	}

	
	void OnItemLongPass(ButtonSkill button)
	{
		UiManager.Instance.openDialogWindow<SkillInfoWindow>((win)=>{
			win.Initialize (button.skillData,roleView.card);
		});
	}
}
