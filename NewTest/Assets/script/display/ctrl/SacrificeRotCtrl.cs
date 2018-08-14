using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SacrificeRotCtrl : MonoBehaviour
{
	float angel = 0;
	public GameObject[] pointList;
	public SacrificeShowerCtrl[] castShowers;
	public SacrificeShowerCtrl	mainShower;
	//public int depth;
	public int size;
	// Use this for initialization
	float loopTime = 1f;

	void Start ()
	{
		mainShower.changeDepth (0);
	}

	//修复闪烁bug;
	public	void flashingBugFix()
	{
		loopTime=1f;
		foreach (SacrificeShowerCtrl each in castShowers) {
			if(each.background!=null&&each.background.enabled==true){
				each.background.enabled=false;
				each.background.enabled=true;
			}
			if(each.cardImage.enabled==true){
				each.cardImage.enabled=false;
				each.cardImage.enabled=true;
			}
		}
	
	}

	public string createFoodList ()
	{
		CardsIntensifyData data = new CardsIntensifyData ();

		foreach (SacrificeShowerCtrl each in castShowers) {
			if (each.card != null) {
				data.addFood (each.card.uid);
			}
		}
		return	data.ToFooding ();
	}

	public long recalculateEXP ()
	{
		long exp = 0;
		foreach (SacrificeShowerCtrl each in castShowers) {
			if (each.card != null) {
				exp += (each.card.getEXP() * each.card.getCardExpLevelUpCast() / 10000);
			}
		}

		return exp;
	}
	/// <summary>
	/// 食物卡牌中是否存在非特殊的高级卡牌(紫色以上的卡牌--特殊卡牌包括:技能经验卡牌,装备经验卡牌,游戏币卡牌,附加属性卡牌)
	/// </summary>
	public bool isSeniorQualityByFoods()
	{
		foreach (SacrificeShowerCtrl each in castShowers) {
			if (each.card == null)
				continue;
			if(ChooseTypeSampleManager.Instance.isToEat(each.card))
				continue;
			if(each.card.getQualityId () >= QualityType.EPIC)
				return true;
		}
		return false;
	}
    public bool isHasBloodLvByFoods() {
        foreach (SacrificeShowerCtrl each in castShowers) {
            if (each.card == null)
                continue;
            if (ChooseTypeSampleManager.Instance.isToEat(each.card))
                continue;
            if (CardSampleManager.Instance.checkBlood(each.card.sid,each.card.uid) && each.card.cardBloodLevel >0)
                return true;
        }
        return false;
    }
	/** sid是否相同 */
	public bool isNamesake (int sid) {
		if (castShowers == null)
			return false;
		foreach (SacrificeShowerCtrl each in castShowers) {
			if (each.card == null)
				continue;
			if (each.getCard ().sid == sid) {
				return true;
			}
		}
		return false;
	}
	public int recalculateSkillEXP ()
	{
		int exp = 0;
		long skillExp = 0;
		foreach (SacrificeShowerCtrl each in castShowers) {
			if (each.card != null) {
				exp += each.card.getEatenExp ();
				//主技能加分
				Skill[] mskill = each.card.getSkills ();
				if (mskill != null && mskill.Length > 0) {
					foreach (Skill emk in mskill) {
						skillExp += emk.getEXP ();
					}
				}
				//buff技能加分
				Skill[] bskill = each.card.getBuffSkills ();
				if (bskill != null && bskill.Length > 0) {
					foreach (Skill emk in bskill) {
						skillExp += emk.getEXP ();
					}
				}				
				//被动技能加分
				Skill[] askill = each.card.getAttrSkills ();
				if (askill != null && askill.Length > 0) {
					foreach (Skill emk in askill) {
						skillExp += emk.getEXP ();
					}
				}
			}
		}	
		double baseExp = exp + skillExp;
		double rateValue = 1.0f;
		//vip额外经验值=10000+vip额外值）/10000
		int vipExpValue;
		if (UserManager.Instance.self.getVipLevel () > 0) {
			Vip vip=VipManagerment.Instance.getVipbyLevel (UserManager.Instance.self.getVipLevel ());
			rateValue += (float)(vip.privilege.skillExpAdd) / 10000.0f;
		}
		double totalExp = (double)(baseExp * rateValue);
		return (int)totalExp;
	}

	//通过card来选中sacrificeShowerCtrl
	public SacrificeShowerCtrl selectShowerByCard (Card _card)
	{
		foreach (SacrificeShowerCtrl each in castShowers) {
			if (each.card == _card) {
				return each;
			}
		}
		return null;
	}
	//祭品满没
	public bool isCasterFull ()
	{
		foreach (SacrificeShowerCtrl each in castShowers) {
			if (each.card == null) {
				return false;
			}
		}
		return true;
	}

	//祭品台是否是空的
	public bool isCasterEmpty ()
	{
		foreach (SacrificeShowerCtrl each in castShowers) {
			if (each.card != null) {
				return false;
			}
		}
		return true;
	}


	//清理主祭台
	public void cleanMainShower ()
	{
		mainShower.cleanData ();
	}
	//清理 献祭台
	public void cleanCastShower ()
	{
		foreach (SacrificeShowerCtrl each in castShowers) {
			each.cleanData ();
		}
	}	

	/** 刷新旋转控制台 */
	public void refreshShowerCtrl(List<Card> list){
		foreach (SacrificeShowerCtrl each in castShowers) {
			if(each.card==null) continue;
			if(list!=null&&list.Contains(each.card)) continue;
			each.cleanData();
		}
	}

	//是否为8个选中的献祭者 中的一个
	public SacrificeShowerCtrl  isOneOfTheCaster (Card _card)
	{
		foreach (SacrificeShowerCtrl each in castShowers) {
			if (each == null)
				continue;
			if (each.card == null)
				continue;
			if (each.card == _card)
				return each;
		}
		return null;
	}
	//是否有主角
	public bool hasMainRole ()
	{
		if (mainShower.card == null)
			return false;
		else
			return  true;
	}

	//设置献祭者底座开关
	public void showCastShowerbase ()
	{
		foreach (SacrificeShowerCtrl each in castShowers) {
			if (each.card == null)
				each.cleanData ();
		}
	}

	public void hideCastShowerbase ()
	{
		foreach (SacrificeShowerCtrl each in castShowers) {
			if(each.card == null)
				each.cleanAll ();
		}
	}

	public SacrificeShowerCtrl  selectOneEmptyCastShower ()
	{
		if (castShowers [4].card == null) {
			return castShowers [4];
		}
		if (castShowers [3].card == null) {
			return castShowers [3];
		}
		if (castShowers [2].card == null) {
			return castShowers [2];
		}
		if (castShowers [6].card == null) {
			return castShowers [6];
		}
		if (castShowers [1].card == null) {
			return castShowers [1];
		}
		if (castShowers [5].card == null) {
			return castShowers [5];
		}
		if (castShowers [7].card == null) {
			return castShowers [7];
		}
		if (castShowers [0].card == null) {
			return castShowers [0];
		}

		return null;
	}
	// Update is called once per frame
	void Update ()
	{

		loopTime -= Time.deltaTime;
		angel += 0.1f;
		transform.localRotation = Quaternion.AngleAxis (angel, Vector3.up);
		changeDepth(false);
		if (loopTime <= 0)
			loopTime = 0.5f;
	}

	public void changeDepth(bool immediately)
	{
		if(immediately)
			loopTime=0;


		for (int i=0; i<pointList.Length; i++) {
			//忽略主卡,主卡不动的
			if (castShowers [i].card == mainShower.card && mainShower.card!=null){
				continue;
			}

			castShowers [i].transform.position = pointList [i].transform.position;
			if (loopTime <= 0) {
				int offset=(int)(- castShowers [i].transform.localPosition.z);
				castShowers [i].changeDepth (offset);
				castShowers [i].changeColorByDepth(offset);
			}
		}
	}
}
