using UnityEngine;
using System.Collections;

public class ContentSkillLearn : dynamicContent
{
	ArrayList RoleList;
	public ButtonSkillLearnCard mainCardButton;
	
	public  void Initialize (ArrayList Roles)
	{
		RoleList = Roles;
		base.reLoad (Roles.Count);

	}
	public ArrayList getRoleList()
	{
		return RoleList;
	}

	public  void reLoad (ArrayList Roles)
	{
		RoleList = Roles;
			base.reLoad (Roles.Count);
	}
	
	public override void updateItem (GameObject item, int index)
	{
		//	base.updateItem (item, index);
		ButtonSkillLearnCard button = item.GetComponent<ButtonSkillLearnCard> ();
		button.updateButton (RoleList [index] as Card);
		if (button.card.uid == UserManager.Instance.self.mainCardUid) {
			mainCardButton = button;
		}
		
	}


	public override void initButton (int  i)
	{
		if (nodeList [i] == null)
			nodeList [i] = Create3Dobj ("UI/learnSkillCardChooseButton").obj;
			
		nodeList [i].transform.parent = transform;
		nodeList [i].transform.localPosition = Vector3 .zero;
		nodeList [i].transform.localScale = new Vector3 (1f, 1f, 1f);
		nodeList [i].name = StringKit. intToFixString (i + 1);
		ButtonSkillLearnCard button = nodeList [i].GetComponent<ButtonSkillLearnCard> ();
		button.fatherWindow = fatherWindow;
		button.Initialize (RoleList [i] as Card);
		if (button.card.uid == UserManager.Instance.self.mainCardUid) {
			mainCardButton = button;
		}
	}

	public void updateButton(Card card)
	{
		for (int i = 0; i < nodeList.Count; i++) {
			ButtonSkillLearnCard button = nodeList [i].GetComponent<ButtonSkillLearnCard> ();
			if(button.card.uid == card.uid)
			{
				button.DoClickEvent();
				return;
			}
		}
		
	}
}

