using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//技能基类
// @author 李程

public class SkillCtrl
{
	
	public SkillData serverData;
	public List< CharacterData > targets;//技能目标
	public CharacterData user;//技能的拥有者
	public CharacterData trigger; //被动技能的触发者	
	public  SkillCtrl triggerSkill;//触发者(skill)
	public Vector3 position; // 技能播放的位置
	public bool isOverSkill = false; //是否为本轮battleinfo技能表的最后一个技能;
	public List<BuffCtrl> buffs; // 技能相关的buff列表
	public BuffAttrChange[] changes;//属性改变集合,用于18buff检查skill
	public BattleSkillMsg skillmsg;//属性改变集合,用于18buff检查skill
	public bool isFinalSkill = false ;//是否是终结技[用于处理战斗减速效果]
	
	public void init (BattleSkillMsg skillmsg, List< CharacterData > _target, CharacterData _user)
	{
		init (skillmsg, _target, _user, null, null); 
	}
	//检查buff 用的技能初始化
	public void init (BattleSkillMsg skillmsg)
	{
		changes = skillmsg.changes;
		init (skillmsg, null, null, null, null); 
	}

	public void AddBuffCtrl (BuffCtrl _buff)
	{
		buffs.Add (_buff); 
	}

	public void RemoveBuffCtrl (BuffCtrl _buff)
	{ 
		buffs.Remove (_buff); 
	}
	
	public void init (BattleSkillMsg skillmsg, List< CharacterData > _target, CharacterData _user, CharacterData _trigger, SkillCtrl EditSkill)
	{ 
		if (skillmsg.skillSID == 0)
			Debug.LogError ("error skillSid");
		
		this.skillmsg = skillmsg;
		targets = _target;
		user = _user;
		trigger = _trigger;
		serverData = SkillManager.Instance.CreateSkillData (skillmsg.skillID, skillmsg.skillSID);
		
		if (serverData == null)
			Debug.LogError ("lost skill:" + skillmsg.skillSID);
		
		buffs = new List<BuffCtrl> (); 

		if (serverData.sample.getType () == SkillType.fixAOE || serverData.sample.getType () == SkillType.MeleeAOE ||
			serverData.sample.getType () == SkillType.MeleeJumpAOE || serverData.sample.getType () == SkillType.MeleeChargeAOE ||
			serverData.sample.getType () == SkillType.bulletMultipleAOE) {
			position = user.parentTeam.TeamHitPoint.position;
		} else	if (targets != null && targets.Count >= 1) {
			if (serverData.sample.getAttackNum () > 1 && serverData.sample.getType () == SkillType.Melee) {
				//近战细分攻击,坐标有偏移
				if (_user.camp == TeamInfo.OWN_CAMP)
					position = targets [0].orgPosition + new Vector3 (0, 0.1f, -0.3f);
				else
					position = targets [0].orgPosition + new Vector3 (0, 0.1f, 0.3f);
			} else {
				position = targets [0].orgPosition + new Vector3 (0, 0.1f, 0);
			}
		}
		
		if (serverData.sample.getType () == SkillType.HelpOther) {
			//确认援护位置
			position = BattleManager.Instance.getOffset (targets [0].orgPosition, targets [0].parentTeam.isGamePlayerTeam);
			//修改攻击者的攻击位置
			EditSkill.position = BattleManager.Instance.getOffset (position, user.parentTeam.isGamePlayerTeam);
			//修改攻击者的目标
			EditSkill.targets [0] = user;
			if (EditSkill != null)
				triggerSkill = EditSkill;
		}		
		if (serverData.sample.getType () == SkillType.FightBack || serverData.sample.getType () == SkillType.FightBackAfterHelp) {
			//确认攻击位置
			position = BattleManager.Instance.getOffset (targets [0].orgPosition, targets [0].parentTeam.isGamePlayerTeam);
			if (EditSkill != null)
				triggerSkill = EditSkill;
		}			
		if (serverData.sample.getType () == SkillType.MedicalAfterHelp) { 
			if (EditSkill != null)
				triggerSkill = EditSkill; 
		}	
		
		if (serverData.sample.getType () == SkillType.BuffCheck) { 

		}	
		if (serverData.sample.getType () == SkillType.AddCard) { 
			//产生新的出来
			if (skillmsg.card.camp == TeamInfo.OWN_CAMP)
				serverData.changeRole = BattleCharacterManager.Instance.CreateBattleCharacterData (skillmsg.card, BattleManager.Instance.playerTeam, BattleManager.battleData.battleType);
			else
				serverData.changeRole = BattleCharacterManager.Instance.CreateBattleCharacterData (skillmsg.card, BattleManager.Instance.enemyTeam, BattleManager.battleData.battleType);	
			
			BattleManager.Instance.addCardToActionCharacters (serverData.changeRole);

		}	
		if (serverData.sample.getType () == SkillType.DelNPC) { 


		}	
		
		if (serverData.sample.getType () == SkillType.GroupCombine) { 
			//如果EditSkill为空，则取消这次技能加载 
			if (EditSkill == null)
				return;
			//如果EditSkill不空，则是participant，修改上个技能目标
			if (EditSkill != null)
				triggerSkill = EditSkill;
			//		triggerSkill.skillData .dataBase= _data;
			triggerSkill.targets = _target;
			triggerSkill.user = _user;
			triggerSkill.trigger = _trigger;  
		}	
		
	}
	//技能播放完成后应用buff到目标身上
	 public  void applyBuff (List<BuffCtrl> buffs)
	{
		applyBuff (buffs, 0);
	}

	//技能播放完成后应用buff到目标身上
	public  void applyBuff (List<BuffCtrl> buffs,int bulletID)
	{
		if (isFinalSkill == true && serverData.sample.getAttackNum () == 1)
			BattleManager.Instance.StartCoroutine (BattleManager.Instance.setTimeScale ());

		//aoe抖屏,目标大于1,并且目标不是自己人
		if ( targets.Count > 1 && buffs.Count >= 1 && user.camp !=buffs [0].owner.camp)
			BattleManager.Instance.shakeCamera ();

		applyBuff_Src(buffs,bulletID);
		/*
		//添加buff,trigger是buff的施加者 
		for (int i=0; i<buffs.Count; i++) {
			BuffCtrl each = buffs [i]; 
			//如果是指定的施加者相同并且施加者的目标包含buff的使用者 
			each.owner.characterCtrl.AddBuff (each);

			// 如果是伤害buff,需要根据ID号激活,为了细分伤害
			if (each.serverData.sample.getType () == BuffType.damage) {
				if (bulletID == 0 || bulletID == each.bulletID) {
					//激活相同ID的伤害buff,0全激活
					each.activeDamageBuff (bulletID);
				} else {
					continue;
				}
			} else {
				each.activeOtherBuff (); //激活buff
			}
			each.owner.characterCtrl.BeHit (serverData.sample.CanHitBack ());//引爆伤害
		}*/
	
	}
	/// <summary>
	/// 外部调用的话 只针对自己的Buff
	/// </summary>
	/// <param name="buffs">Buffs.</param>
	/// <param name="bulletID">Bullet I.</param>
	public void applyBuff_Src(List<BuffCtrl> buffs,int bulletID)
	{
		//添加buff,trigger是buff的施加者 
		for (int i=0; i<buffs.Count; i++) {
			BuffCtrl each = buffs [i]; 
			//如果是指定的施加者相同并且施加者的目标包含buff的使用者 
			each.owner.characterCtrl.AddBuff (each);
			
			// 如果是伤害buff,需要根据ID号激活,为了细分伤害
			if (each.serverData.sample.getType () == BuffType.damage) {
				if (bulletID == 0 || bulletID == each.bulletID||each.trigger==each.owner) {
					//激活相同ID的伤害buff,0全激活
					each.activeDamageBuff (bulletID);
				}
                else {
					continue;
				}
			} else {
				each.activeOtherBuff (); //激活buff
			}
			each.owner.characterCtrl.BeHit (serverData.sample.CanHitBack ());//引爆伤害
		}
	}

	/// <summary>
	/// 加召唤兽怒气
	/// </summary>
	public void applyMonsterBuffOnly (CharacterData trigger)
	{
		//trigger是buff的施加者 
		for (int i=0; i<trigger.characterCtrl.activeAction.Skill.buffs.Count; i++) {
			BuffCtrl each = trigger.characterCtrl.activeAction.Skill.buffs [i]; 
			//如果是指定的施加者相同并且施加者的目标包含buff的使用者 
			if (each.serverData.sample.getType () == BuffType.power)
				each.activeOtherBuff (); 
		} 
	} 
} 