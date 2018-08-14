using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/** 
  * 特效图形控制器 
  * @author 李程
  * */
public enum bulletDisplayTypeEnum
{
	normal,
	lightBeam, //打到目标位置的光炮
	lightBeamInfinite,//打穿屏幕的光炮
	throughAOE,//穿透子弹,直接飞出屏幕的
	fixAOE,//原地播放的爆炸特效
}

public class BulletCtrl : EffectCtrl
{
	public float blastingTime;//爆炸时间点,必须小于生命
	public bulletDisplayTypeEnum displayType = bulletDisplayTypeEnum.normal;
	public LineRenderer[] renders;
	public List<BuffCtrl> buffs;
	bool lastBullet = false;	//多发子弹中的最后一个
	Vector3 dir;
	Vector3 _position;

	public bool isLastBullet ()
	{
		return lastBullet;
	}

	// Use this for initialization
	public virtual void	initBullet (CharacterData  _target, CharacterData _owner, List<BuffCtrl> buffs, bool _isLastBullet)
	{
	    
        //
		target = _target;
		owner = _owner;
		lastBullet = _isLastBullet;
		this.buffs = buffs;
        if (owner.characterCtrl.activeAction.Skill.serverData.sample != null) {
           // Debug.LogError("sample.sid" + owner.characterCtrl.activeAction.Skill.serverData.sample.sid);
            if (owner.characterCtrl.activeAction.Skill.serverData.sample.sid == 23005) {
            //    Debug.LogError("displayType===" + displayType);
            }
        }
		if (displayType == bulletDisplayTypeEnum.lightBeam) { //光线类
			if(renders==null || renders.Length==0) {
				LineRenderer render = transform.FindChild ("line").GetComponent<LineRenderer> ();
				render.SetPosition (0, owner.characterCtrl.hitPoint.transform.position);
				if (owner.characterCtrl.activeAction.Skill.serverData.sample.getType () == SkillType.bulletMultipleAOE) {
					//多发子弹不会有援护,所以直接取目标位置
					render.SetPosition (1, target.characterCtrl.transform.position + new Vector3 (0, 0.1f, 0));
				} else {
					//单发可能有援护,要取修正地址
					render.SetPosition (1, owner.characterCtrl.activeAction.Skill.position);
				}
			} else {
				LineRenderer render;
				for(int i=0,len=renders.Length;i<len;i++) {
					render=renders[i];
					render.SetPosition (0, owner.characterCtrl.hitPoint.transform.position);
					if (owner.characterCtrl.activeAction.Skill.serverData.sample.getType () == SkillType.bulletMultipleAOE) {
						//多发子弹不会有援护,所以直接取目标位置
						render.SetPosition (1, target.characterCtrl.transform.position + new Vector3 (0, 0.1f, 0));
					} else {
						//单发可能有援护,要取修正地址
						render.SetPosition (1, owner.characterCtrl.activeAction.Skill.position);
					}
				}
			}
			iTween.ValueTo (gameObject, iTween.Hash ("from", 1, "to", 0, "onupdate", "", "easetype", iTween.EaseType.linear, "oncomplete", "hit", "time", blastingTime));
			if (isLastBullet ())
				iTween.ValueTo (gameObject, iTween.Hash ("from", 1, "to", 0, "onupdate", "", "easetype", iTween.EaseType.linear, "oncomplete", "destoryAndActionOver", "time", life));
			else
				iTween.ValueTo (gameObject, iTween.Hash ("from", 1, "to", 0, "onupdate", "", "easetype", iTween.EaseType.linear, "oncomplete", "destory", "time", life));
		} else if (displayType == bulletDisplayTypeEnum.lightBeamInfinite) {
			if (owner.characterCtrl.activeAction.Skill.serverData.sample.getType () == SkillType.bulletMultipleAOE) {
				dir = target.characterCtrl.transform.position - owner.characterCtrl.hitPoint.transform.position;
			} else {
				dir = owner.characterCtrl.activeAction.Skill.position - owner.characterCtrl.hitPoint.transform.position;
			}
			dir.Normalize ();
			if(renders==null || renders.Length==0) {
				//打通屏幕型光线枪
				LineRenderer render = transform.FindChild ("line").GetComponent<LineRenderer> ();
				render.SetPosition (0, owner.characterCtrl.hitPoint.transform.position);
				render.SetPosition (1, owner.characterCtrl.hitPoint.transform.position + dir * 2f);
			}
			else {
				LineRenderer render;
				for(int i=0,len=renders.Length;i<len;i++) {
					render=renders[i];
					render.SetPosition (0, owner.characterCtrl.hitPoint.transform.position);
					render.SetPosition (1, owner.characterCtrl.hitPoint.transform.position + dir * 2f);
				}
			}
			hit ();
			target.characterCtrl.cardCtrl.longShake (life + 0.2f);
			iTween.ValueTo (gameObject, iTween.Hash ("from", 1, "to", 0, "onupdate", "", "easetype", iTween.EaseType.linear, "oncomplete", "destoryAndActionOver", "time", life));
		} else if (displayType == bulletDisplayTypeEnum.throughAOE) {
			//类EZ大招型子弹
			CharacterCtrl.playCharacterAnim (owner.characterCtrl, "cardThrowAnim");
			transform.localScale = Vector3.one;
			if (target.parentTeam.isGamePlayerTeam == true) {
				dir = BattleManager.Instance.playerTeam.TeamHitPoint.position - owner.characterCtrl.hitPoint.transform.position;
				transform.LookAt (BattleManager.Instance.playerTeam.TeamHitPoint.position, transform.up);
				transform.eulerAngles = new Vector3 (0, transform.eulerAngles.y, transform.eulerAngles.z);
			} else {
				dir = BattleManager.Instance.enemyTeam.TeamHitPoint.position - owner.characterCtrl.hitPoint.transform.position;
				transform.LookAt (BattleManager.Instance.enemyTeam.TeamHitPoint.position, transform.up);
				transform.eulerAngles = new Vector3 (0, transform.eulerAngles.y, transform.eulerAngles.z);
			}
			dir.Normalize ();
			//offset
			transform.position = owner.characterCtrl.transform.position + new Vector3 (0, 0.3f, 0);
			_position = owner.characterCtrl.transform.position + dir * 5;
			iTween.MoveTo (gameObject, iTween.Hash ("position", _position, "easetype", iTween.EaseType.linear, "oncomplete", "destory", "time", life));
			iTween.ValueTo (gameObject, iTween.Hash ("from", 1, "to", 0, "onupdate", "", "easetype", iTween.EaseType.linear, "oncomplete", "hitAndActionOver", "time", blastingTime));
		} else if (displayType == bulletDisplayTypeEnum.fixAOE) {
			setOffset ();
			iTween.ValueTo (gameObject, iTween.Hash ("from", 1, "to", 0, "onupdate", "", "easetype", iTween.EaseType.linear, "oncomplete", "hitAndActionOver", "time", blastingTime));
			iTween.ValueTo (gameObject, iTween.Hash ("from", 1, "to", 0, "onupdate", "", "easetype", iTween.EaseType.linear, "oncomplete", "destory", "time", life));
		} else {
			//普通子弹
			setOffset ();
			lookAtTarget ();
			float dis = Vector3.Distance (owner.characterCtrl.transform.localPosition, _target.characterCtrl.transform.localPosition);

			if (lastBullet)
				dis = 960;

			iTween.MoveTo (gameObject, iTween.Hash ("position", _position, "easetype", iTween.EaseType.linear, "oncomplete", "Blasting", "time", Mathf.Clamp (life * dis / 960f, 0.03f, life)));
		}
	}

	void setOffset ()
	{
		if (displayType == bulletDisplayTypeEnum.fixAOE) {
			//增加高度，避免子弹被卡片遮挡;
			
			//AOE子弹 在队伍中心点爆炸
			if (target.characterCtrl.characterData.parentTeam.isGamePlayerTeam == true) {
				transform.position = BattleManager.Instance.playerTeam.TeamHitPoint.position + new Vector3 (0, 0.3f, 0);	
			} else {
				transform.position = BattleManager.Instance.enemyTeam.TeamHitPoint.position + new Vector3 (0, 0.3f, 0);
			}
			_position = transform.position;
	
		} else {

			if (owner.parentTeam.isGamePlayerTeam) {
				transform.position = owner.characterCtrl.hitPoint.position + new Vector3 (0, 0.3f, 0.2f);
			} else {
				transform.position = owner.characterCtrl.hitPoint.position + new Vector3 (0, 0.3f, -0.2f);
			}


			if (owner.characterCtrl.activeAction.Skill.serverData.sample.getType () == SkillType.bulletMultipleAOE) {
				_position = target.characterCtrl.hitPoint.position + new Vector3 (0, 0.3f, 0);
			} else {
				//正常子弹
				_position = owner.characterCtrl.activeAction.Skill.position + new Vector3 (0, 0.3f, 0);
			}
		 

		}
		transform.localScale = Vector3.one;
	}


	//击中效果
	void hit ()
	{
		if (displayType == bulletDisplayTypeEnum.fixAOE || displayType == bulletDisplayTypeEnum.normal || displayType == bulletDisplayTypeEnum.lightBeam || displayType == bulletDisplayTypeEnum.throughAOE) {
			//细分攻击传对应id 对应出伤害
			if (owner.characterCtrl.activeAction.Skill.serverData.sample.getAttackNum () > 1)
				owner.characterCtrl.activeAction.Skill.applyBuff (buffs, GetInstanceID ());
			else {
				//一下是普通攻击
				if (owner.characterCtrl.activeAction.Skill.targets.Count > 1) {
					//多发子弹
					owner.characterCtrl.activeAction.Skill.applyBuff (buffs);
				} else {
					//单发子弹
					owner.characterCtrl.activeAction.Skill.applyBuff (owner.characterCtrl.activeAction.Skill.buffs);
				}
			}
		} else if (displayType == bulletDisplayTypeEnum.lightBeamInfinite) {
			subDamage (owner.characterCtrl.activeAction.Skill);
			owner.characterCtrl.activeAction.Skill.applyBuff (buffs);
		}
		int damageAll = 0; 	
		//找到对应的伤害buff,单体目标肯定只有1
		foreach (BuffCtrl each in owner.characterCtrl.activeAction.Skill.buffs) { // 先统计所有buff伤害
			if (each.serverData.sample.getType () == BuffType.damage) {	
				//算出单次伤害 
				damageAll += each.serverData.serverDamage;
			}
		}
		SkillCtrl sc=owner.characterCtrl.activeAction.Skill;//释放者的
		CharacterData cd=sc.targets[0];//其中一个技能目标
		List<BuffCtrl> damageBuff = cd.characterCtrl.damageBuff;//这个目标中的伤害buff
		if(damageBuff!=null&&damageBuff.Count>1){
			UiManager.Instance.battleWindow .comboBar.init (damageBuff.Count,damageAll,true);
		}
		owner.characterCtrl.helpBackCheck ();

	}
	void  subDamage (SkillCtrl skillctrl)
	{
		foreach (BuffCtrl each in buffs) {
			if (each.serverData.sample.getType () == BuffType.damage && each.owner == target) {
				int subDamage = (int)(each.serverData.serverDamage * 0.2f);
				for (int i=0; i<4; i++) {
					BuffCtrl ctrl = new BuffCtrl ();
					ctrl.owner = each.owner;
					ctrl.trigger = each.trigger;
					ctrl.DisplayEffect = each.DisplayEffect;
					
					ctrl.option = each.option;
					ctrl.parentSkill = each.parentSkill;
					ctrl.serverData = new BuffData (each.serverData.sid, subDamage);
					ctrl.serverData.damageEffect = "";
					buffs.Add (ctrl);
				}
				each.serverData.serverDamage = subDamage;
				break;
			}
		}
	}

	//不击中. 光线类用
	void destoryAndActionOver ()
	{
		destory ();
		actionOver ();
	}

	//不销毁.类是ez的大,继续穿透
	void hitAndActionOver ()
	{
		hit ();
		actionOver ();
	}
 
	void actionOver ()
	{
		owner.characterCtrl.SkillScaleDownComplete ();
	}

	void lookAtTarget ()
	{
		if (displayType == bulletDisplayTypeEnum.fixAOE)
			return;
		if (owner.characterCtrl.activeAction.Skill.serverData.sample.getType () == SkillType.bulletMultipleAOE) {
			transform.LookAt (target.characterCtrl.hitPoint.position, transform.up);
			transform.eulerAngles = new Vector3 (0, transform.eulerAngles.y, transform.eulerAngles.z);
		} else {
			transform.LookAt (owner.characterCtrl.activeAction.Skill.position, transform.up);
			transform.eulerAngles = new Vector3 (0, transform.eulerAngles.y, transform.eulerAngles.z);
		}
	}
	//击中并且销毁
	void Blasting ()
	{
		hit ();
		if (isLastBullet ())
			actionOver ();
		destory ();
	}
	// Update is called once per frame
	void Update ()
	{ 
		return;
	}
}
