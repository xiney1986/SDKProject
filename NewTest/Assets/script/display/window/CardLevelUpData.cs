using UnityEngine;
using System.Collections;

public class CardLevelUpData
{
	
	public EXPAward expAward;//之后得奖	
	public CardBaseAttribute oldAttr;//旧数值
	CardBaseAttribute _newAttr;

	public CardBaseAttribute newAttr {
		get {
			return _newAttr;
		}
		set {
			_newAttr = value;
			differenceAttr = new CardBaseAttribute ();
			differenceAttr.attack = _newAttr.attack - oldAttr.attack;
			differenceAttr.agile = _newAttr.agile - oldAttr.agile;
			differenceAttr.defecse = _newAttr.defecse - oldAttr.defecse;
			differenceAttr.hp = _newAttr.hp - oldAttr.hp;
			differenceAttr.magic = _newAttr.magic - oldAttr.magic;
		}
	}

	public CardBaseAttribute differenceAttr;//差值
	
	public  LevelupInfo levelInfo  ;
 
 
}
