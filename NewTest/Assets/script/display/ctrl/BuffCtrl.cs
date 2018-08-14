using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum buff_option
{
	none,
	Add,
	Remove,
	Replace,
	active,
}

public class BuffCtrl
{ 
	public BuffData serverData;
	public CharacterData owner;
	public CharacterData trigger;
	public GameObject effect;//触发效果关联
	public GameObject DisplayEffect;	//存在时候的效果关联
	public buff_option  option = buff_option.none;	//操作类型
	public SkillCtrl  parentSkill;	//父技能
	public int 	bulletID;//能激活这个buff的子弹Ctrl的instanceID;

	private string[] buffEffectPath = new string[] {
		"Effect/Buff/attrUp",
		"Effect/Buff/attrUp_boss",
		"Effect/Buff/attrDown",
		"Effect/Buff/attrDown_boss"
	};

	public void init (CharacterData _owner, CharacterData _trigger, BuffData _buff, bool _duration, int _id, SkillCtrl  _parentSkill)
	{
		owner = _owner;
		serverData = _buff;
		serverData.id = _id;
		parentSkill = _parentSkill;
		trigger = _trigger;
//	MonoBase.	print(serverData.id);
		if (serverData.id == 0)
			serverData.id = this.GetHashCode ();
	}

	public void removeEffect ()
	{
//		MonoBase.print ("I remove buff effect");
		GameObject.Destroy (DisplayEffect); 
	}
	/// <summary>
	/// 激活特定的伤害buff
	/// </summary>
	public void activeDamageBuff (int ID)
	{
		makeDamage ();
	}
	/// <summary>
	///激活人物身上的所有伤害buff
	/// </summary>
	public void activeDamageBuff ()
	{
		activeDamageBuff (0);
	}

	/// <summary>
	/// 激活伤害以外的其他类型的buff
	/// </summary>
	public void activeOtherBuff () {    
		if (serverData.sample.getType () == BuffType.power) { 
			//召唤兽怒气
			UiManager.Instance.battleWindow .changeMonsterBarValue ((float)serverData.serverDamage * 0.01f, owner.parentTeam.isGamePlayerTeam);
		} else { 
			// 战斗中有些特殊buff， 功能为：增加，移除特定buff
			if (option == buff_option.Remove) {
				owner.characterCtrl.RemoveBuff (this);
			}
			if (option == buff_option.Add) {
				string durationEffectUrl = serverData.sample.getDurationEffect ();
				string effectUrl = GetBuffEffectPath ();
				EffectManager.Instance.CreateEffect (owner.characterCtrl.hitPoint, effectUrl);
				ShowBuffTip ();
				//如果是变身卡
				if (serverData.sample.getTransformID () != "0") {
					owner.characterCtrl.beginTransform (serverData.sample.getTransformID ());
				}
				option = buff_option.none;
			}	
			if (option == buff_option.active) {
				if (string.IsNullOrEmpty (serverData.sample.getDurationEffect ()) == false) {

					if (owner.isBoss)
						EffectManager.Instance.CreateEffect (owner.characterCtrl.hitPoint, serverData.sample.getDurationEffectForBOSS ());
					else
						EffectManager.Instance.CreateEffect (owner.characterCtrl.hitPoint, serverData.sample.getDurationEffect ());
				}
				makeDurationDamage ();
				option = buff_option.none;
			}				
	
			if (option == buff_option.Replace) {
				//TODO  替换buff
				Debug.LogWarning ("ready to Replace buff !!!!!!");
			}					
			
		}
	}
	/// <summary>
	/// 显示Buff 提示文字
	/// </summary>
	private void ShowBuffTip ()
	{
		string[] tips = serverData.getBuffDesc ();
		for (int i=0,length=tips.Length; i<tips.Length; i++) {
			//TODO 如果BUFF影响的属性大于一个 应该分时间片来显示
			EffectManager.Instance.CreateBuffTipText (owner.characterCtrl.transform, tips [i], 0.1f);
		}
	}
	/// <summary>
	/// 返回Buff特效的路径 分为增益 减益，BOSS，非BOSS
	/// </summary>
	/// <returns>The buff effect path.</returns>
	private string GetBuffEffectPath ()
	{
		bool isSelfSide = (owner.camp == trigger.camp);
		if (owner.isBoss) {
			if (isSelfSide) {
				return buffEffectPath [1];
			} else {
				return buffEffectPath [3];
			}
			
		} else {
			
			if (isSelfSide) {
				return buffEffectPath [0];
			} else {
				return buffEffectPath [2];
			}
		}
	}

	public void CreateDurationBuffEffect ()
	{
		if (serverData.sample.getDisplayType () == BuffIconType.BodyEffect) {
			GameObject _tmp = MonoBase.Create3Dobj (serverData.sample.getResPath ()).obj;
			//buff也可以不配激活时候的特效
			if (_tmp == null)
				return;
			_tmp.transform.parent = owner.characterCtrl.hitPoint;
			_tmp.transform.localPosition = Vector3.zero;
			_tmp.transform.localScale = Vector3.one;
			DisplayEffect = _tmp;
		}
	}
	
	void makeDamage ()
	{ 
		//把伤害buff从总buff表添加到伤害buff队列
		owner.characterCtrl.addDamage (this); 
		owner.characterCtrl.RemoveBuff (this); 
		if (!string.IsNullOrEmpty (serverData.damageEffect)) {
			EffectManager.Instance.CreateEffect (owner.characterCtrl.hitPoint, serverData.damageEffect);
		}

	}

	void makeDurationDamage ()
	{ 
		owner.characterCtrl.addDamage (this); 
	}	
 
}
