using UnityEngine;
using System.Collections.Generic;

/**
 * 学习技能选择副卡技能容器
 * @author 杨小珑
 **/
public class learningSkillSelectContent : dynamicContent {

	public GameObject itemPrefab;
	Dictionary<Card,List<Skill>> map;
	List<Card> keys;

	public void reLoad(Dictionary<Card,List<Skill>> map)
	{
		this.map = map;
		keys = new List<Card> ();
		foreach (Card c in map.Keys) {
			keys.Add(c);
		}
		 base.reLoad(map.Count);
	}

	public override void initButton (int i)
	{
		if (nodeList [i] == null) 
			nodeList [i] = NGUITools.AddChild (gameObject, itemPrefab);
		nodeList [i].name = StringKit.intToFixString (i + 1);
		LearningSkillSelectItem sc = nodeList [i].GetComponent<LearningSkillSelectItem> ();
		sc.window = fatherWindow as LearnSkillSelectWindow;
		Card c = keys [i];
		sc.init (c, map [c]);
	}

	public override void updateItem (GameObject item, int index)
	{
		item.name = StringKit.intToFixString (index + 1);
		LearningSkillSelectItem sc = item.GetComponent<LearningSkillSelectItem> ();
		Card c = keys [index];
		sc.init (c, map [c]);
	}
}
