using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 献祭管理器
 * @author 汤琦
 **/
public class SacrificeManager
{
	private Card mainCard;//主卡
	private List<Card> foodCard;//食物卡
	private LevelupInfo[] mskillsLvUpInfo;
	private LevelupInfo[] bskillsLvUpInfo;
	private LevelupInfo[] askillsLvUpInfo;

	public static SacrificeManager Instance {
		get{return SingleManager.Instance.getObj("SacrificeManager") as SacrificeManager;}
	}
	//设置主卡
	public void setMainCard(Card card)
	{
		mainCard = card;
	}
	//移除主卡
	public void removeMainCard()
	{
		mainCard = null;
	}
	//设置食物卡
	public void setFoodCard(Card card)
	{
		if(foodCard == null)
			foodCard = new List<Card>();
		foodCard.Add(card);
	}
	//移除一个食物卡
	public void removeFoodCard(Card card)
	{
		foodCard.Remove(card);
	}
	//清空食物卡
	public void clearFoodCard()
	{
		foodCard.Clear();
	}
	//是否有食物卡
	public bool isHaveFood()
	{
		return foodCard == null || foodCard.Count == 0;
	}
	//是否可以强化,根据玩家的游戏币判断
	public bool isIntesify()
	{
		int cost = 0;
		for (int i = 0; i < foodCard.Count; i++) {
			cost += foodCard[i].getCardSkillLevelUpCast();
		}
		if(UserManager.Instance.self.getMoney () < cost)
		{
			//提示
			return false;
		}
		return true;
	}
	//是否可以点击强化按钮
	public bool isClickIntesifyBtn()
	{
		int cost = 0;
		for (int i = 0; i < foodCard.Count; i++) {
			cost += foodCard[i].getCardSkillLevelUpCast();
		}
		return cost > 0 && mainCard != null && isHaveFood();
	}
	//设置强化前主卡技能信息
	public void setOldSkillInfo ()
	{
		Skill[] mskills = mainCard.getSkills ();
		if (mskills != null && mskills.Length > 0) {
			mskillsLvUpInfo = new LevelupInfo[mskills.Length];
			for (int i=0; i<mskills.Length; i++) {
				if (mskillsLvUpInfo [i] == null)
					mskillsLvUpInfo [i] = new LevelupInfo ();
				mskillsLvUpInfo [i].oldLevel = mskills [i].getLevel ();
				mskillsLvUpInfo [i].oldExp = mskills [i].getEXP ();			
				mskillsLvUpInfo [i].oldExpUp = mskills [i].getEXPUp ();	
				mskillsLvUpInfo [i].oldExpDown = mskills [i].getEXPDown ();	
			}
		}
		Skill[] bskills = mainCard.getBuffSkills ();
		if (bskills != null && bskills.Length > 0) {
			bskillsLvUpInfo = new LevelupInfo[bskills.Length];		
			for (int i=0; i<bskills.Length; i++) {
				if (bskillsLvUpInfo [i] == null)
					bskillsLvUpInfo [i] = new LevelupInfo ();
				bskillsLvUpInfo [i].oldLevel = bskills [i].getLevel ();
				bskillsLvUpInfo [i].oldExp = bskills [i].getEXP ();			
				bskillsLvUpInfo [i].oldExpUp = bskills [i].getEXPUp ();	
				bskillsLvUpInfo [i].oldExpDown = bskills [i].getEXPDown ();	
			}
		}
		Skill[] askills = mainCard.getAttrSkills ();
		if (askills != null && askills.Length > 0) {
			askillsLvUpInfo = new LevelupInfo[askills.Length];	
			for (int i=0; i<askills.Length; i++) {
				if (askillsLvUpInfo [i] == null)
					askillsLvUpInfo [i] = new LevelupInfo ();
				askillsLvUpInfo [i].oldLevel = askills [i].getLevel ();
				askillsLvUpInfo [i].oldExp = askills [i].getEXP ();			
				askillsLvUpInfo [i].oldExpUp = askills [i].getEXPUp ();	
				askillsLvUpInfo [i].oldExpDown = askills [i].getEXPDown ();	
			}
		}
		
	}
	//设置强化后主卡技能信息
	public void getNewSkillInfo (string cardID)
	{
		mainCard = StorageManagerment.Instance.getRole (cardID);
		Skill[] mskills = mainCard.getSkills ();
		if (mskills != null && mskills.Length > 0) {		
			for (int i=0; i<mskills.Length; i++) {
				mskillsLvUpInfo [i].newLevel = mskills [i].getLevel ();
				mskillsLvUpInfo [i].newExp = mskills [i].getEXP ();			
				mskillsLvUpInfo [i].newExpUp = mskills [i].getEXPUp ();	
				mskillsLvUpInfo [i].newExpDown = mskills [i].getEXPDown ();	
				mskillsLvUpInfo [i].orgData = mskills [i];
			}
		}
		Skill[] bskills = mainCard.getBuffSkills ();	
		if (bskills != null && bskills.Length > 0) {	
			for (int i=0; i<bskills.Length; i++) {
				bskillsLvUpInfo [i].newLevel = bskills [i].getLevel ();
				bskillsLvUpInfo [i].newExp = bskills [i].getEXP ();			
				bskillsLvUpInfo [i].newExpUp = bskills [i].getEXPUp ();	
				bskillsLvUpInfo [i].newExpDown = bskills [i].getEXPDown ();	
				bskillsLvUpInfo [i].orgData = bskills [i];
			}
		}
		Skill[] askills = mainCard.getAttrSkills ();
		if (askills != null && askills.Length > 0) {	
			for (int i=0; i<askills.Length; i++) {
				askillsLvUpInfo [i].newLevel = askills [i].getLevel ();
				askillsLvUpInfo [i].newExp = askills [i].getEXP ();			
				askillsLvUpInfo [i].newExpUp = askills [i].getEXPUp ();	
				askillsLvUpInfo [i].newExpDown = askills [i].getEXPDown ();	
				askillsLvUpInfo [i].orgData = askills [i];
			}
		}
	}
	public LevelupInfo[] getMSkillsLvUpInfo()
	{
		return mskillsLvUpInfo;
	}
	public LevelupInfo[] getBSkillsLvUpInfo()
	{
		return bskillsLvUpInfo;
	}
	public LevelupInfo[] getASkillsLvUpInfo()
	{
		return askillsLvUpInfo;
	}

}
