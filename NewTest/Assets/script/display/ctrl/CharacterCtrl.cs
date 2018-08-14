using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/** 
  * 卡片控制类
  * @author 李程
  * */ 
public enum enum_character_state
{
	awaking,//启动
	awaked,
	standing,
	deading,
	deaded,
	attacking,
	attacked,
	helping,//援护
	helped,
	medicaling,//急救
	medicaled,
	fightBacking,//反击
	fightBacked,
	groupAttacking,//合击
	groupAttacked,
	comboAttacking,//连击
	comboAttacked,	
}

//[RequireComponent (typeof(UIPanel))]
public class CharacterCtrl : MonoBase
{
	//UIPanel panel;
	const float comboDelay = 0.5f;
	public 	CardCtrl cardCtrl;
	public 	HpbarCtrl HpBar;
    public GameObject redCardFalg;
	public enum_character_state state;
//	public int FormationIndex; 用characterData的TeamPosIndex;
	public Transform hitPoint;
	public Animation anim;
	public bool isWorking;
	List<BuffCtrl> buffs;//总buff表
	BuffCtrl bodyBuff;
	public  GameObject[]  buffColumn;
	public  Material[]  buffMaterals;
	public	CharacterAction activeAction;
	public bool isHurting = false;
	public	CharacterData characterData;
	public GameObject shadow;
	public bool _isLastGroupAttacker = false;
	public bool waitForFightBack = false;
	public float DamageOffset = 0;//多重伤害字体偏移
	public List<BuffCtrl> damageBuff;//伤害 huff
	public CallBack helpOtherReturn;//援护后的回调

	public void dead ()
	{
		if (getState () == enum_character_state.deading | getState () == enum_character_state.deaded) {
			return; 
		}
		
		EffectManager.Instance.CreateEffect (hitPoint.transform, "Effect/Single/die_effect");

		CharacterAction ac = new CharacterAction (enum_character_Action.Dead);
		setState (ac);

	}
	//public int deep;
	void Awake ()
	{

		//panel = gameObject.GetComponent<UIPanel> ();
		gameObject.layer = 10;
		buffs = new List<BuffCtrl> ();

	}

	public List<BuffCtrl> GetBuffList ()
	{
		return buffs;
	}
	
	public void	outBattleField ()
	{
		characterData.isInBattleField = false;
		iTween[] tw = GetComponents<iTween> ();
		if (tw != null) {
			foreach (iTween each2 in tw)
				GameObject.DestroyImmediate (each2);
		}

		cardCtrl.MoveOutBattleField ();
		
	}
	
	void onOutBattleField ()
	{
		
		if (characterData.isNPC)
			DestoryCharacter ();
		
		
	}

	//开始变身
	public void 	beginTransform (string id)
	{
		//变身后的从高亮变正常的冷却
		cardCtrl.transfromCoolDown ();
		//修改图片
		cardCtrl.changePicture (id);
	}

	void onStandInBattleField ()
	{
		characterData.isInBattleField = true;
		if (HpBar != null) {
			HpBar.updateValue (characterData.server_hp, characterData.server_max_hp);
			HpBar.setDeadCallBack (dead); 
		}
		setStateImmediate (enum_character_state.standing);


	}

	void Start ()
	{
		//召唤兽没进场动画, 直接回调
		if (characterData.isGuardianForce) {
			onStandInBattleField ();
		}
		
		gameObject.layer = 9;
//		panel.depth = (int)(characterData. orgPosition.y * 400);
		
		//如果是召唤兽，不做进场动画
		if (characterData.isGuardianForce == false)
			inToBattleField ();

	

	}

	public void  inToBattleField ()
	{
		//	MonoBase.print (characterData.serverID + " is move in battle");
		cardCtrl.MoveToBattleField (); 
	}

	public void AddBuff (BuffCtrl _buff)
	{
		buffs.Add (_buff);
		if (_buff.serverData.sample.getType () == BuffType.damage)
			return;
		if (_buff.serverData.sample.getType () != BuffType.attr_change&&_buff.serverData.sample.getType()!=BuffType.jinghua) {
			RecalculateBodyBuff(_buff);
		}
	    if (_buff.serverData.sample.getType() == BuffType.jinghua)
	    {
	        removeAllchangeBuff();
	    }
		RecalculateBuffData (); 
	}
    /// <summary>
    /// 移除目标身上的所有增益buff
    /// </summary>
    void removeAllchangeBuff()
    {
        List<BuffCtrl> tempList=new List<BuffCtrl>();
            foreach (BuffCtrl each in buffs) {
                if (each.serverData.sample.getType() == BuffType.attr_change)
                {
                    if (each.serverData.serverAttr_attack > 0 || each.serverData.serverAttr_defend > 0 || each.serverData.serverAttr_magic > 0 || each.serverData.serverAttr_dex>0)continue;
                }
                 tempList.Add(each);
            }
        buffs.Clear();
        buffs = tempList;

    }
	/** 重新计算BodyBuff */
	void RecalculateBodyBuff (BuffCtrl _buff) {
		if (_buff.serverData.sample.getDisplayType () != BuffIconType.BodyEffect)
			return;
		//如果是其他类型，创建bodyEffect
		if (bodyBuff != null) {
			bodyBuff.removeEffect ();
			bodyBuff = null;
		}
		bodyBuff = _buff;
		bodyBuff.CreateDurationBuffEffect ();
	}
	/** 根据buff改变属性数值构建buff_icon[比如:攻击提升,防御提升,敏捷提升等的图标] */
	void RecalculateBuffData () {
		if (characterData.isGuardianForce)
			return;
		int _lastAttack = 0;
		int _lastDefend = 0;
		int _lastMagic = 0;	
		int _lastDex = 0;	
		//这里可能需要数据反推，如降低最大生命值的buff
		foreach (BuffCtrl each in buffs) { 
			if (each.serverData.sample.getType () == BuffType.attr_change) {
				_lastAttack += each.serverData.serverAttr_attack;
				_lastDefend += each.serverData.serverAttr_defend;
				_lastMagic += each.serverData.serverAttr_magic;
				_lastDex += each.serverData.serverAttr_dex;
			} 
		}
		//		print (characterData.serverID +  " effect:" + _lastAttack);
		if (_lastAttack != 0) {
			buffColumn [0].gameObject.SetActive (true);
			if (_lastAttack > 0)
				buffColumn [0].renderer.material = buffMaterals [1];
			else
				buffColumn [0].renderer.material = buffMaterals [1 + 4];
		} else {
			buffColumn [0].gameObject.SetActive (false);
		}
		if (_lastDefend != 0) {
			buffColumn [1].gameObject.SetActive (true);
			if (_lastDefend > 0)
				buffColumn [1].renderer.material = buffMaterals [2];
			else
				buffColumn [1].renderer.material = buffMaterals [2 + 4];
	
		} else {
			buffColumn [1].gameObject.SetActive (false);
		}
		if (_lastMagic != 0) {
			buffColumn [2].gameObject.SetActive (true);
			if (_lastMagic > 0)
				buffColumn [2].renderer.material = buffMaterals [3];
			else
				buffColumn [2].renderer.material = buffMaterals [3 + 4];
		} else {
			buffColumn [2].gameObject.SetActive (false);
		}
		if (_lastDex != 0) {
			buffColumn [3].gameObject.SetActive (true);
			if (_lastDex > 0)
				buffColumn [3].renderer.material = buffMaterals [4];
			else
				buffColumn [3].renderer.material = buffMaterals [4 + 4];
		} else {
			buffColumn [3].gameObject.SetActive (false);
		}
	}
	/// <summary>
	/// 移除buff
	/// </summary>
	/// <param name="_buff">_buff.</param>
	public void RemoveBuff (BuffCtrl _buff) {
		for(int i=0;i<buffs.Count;i++)
		{
			if(buffs[i].serverData.sid == _buff.serverData.sid)
			{
				buffs.Remove(buffs[i]);
			}
		}
		buffs.Remove (_buff);
		if (_buff.serverData.sample.getType () == BuffType.damage)
			return; 
		removeBuffByEffect(_buff);
		RecalculateBuffData (); 
	}
	/// <summary>
	/// 移除buff后相应的效果处理
	/// </summary>
	/// <param name="_buff">_buff.</param>
	public void removeBuffByEffect(BuffCtrl _buff) {
		if (_buff.serverData.sample.getType () == BuffType.vertigo)
			EffectManager.Instance.CreateEffect (hitPoint, "Effect/Single/RemoveVertigo");
		if (_buff.serverData.sample.getType () == BuffType.frozen)
			EffectManager.Instance.CreateEffect (hitPoint, "Effect/Single/RemoveFrozen");	
		if (_buff.serverData.sample.getType () == BuffType.silence)
			EffectManager.Instance.CreateEffect (hitPoint, "Effect/Single/RemoveSilence");		
		if(_buff.serverData.sample.getDisplayType () == BuffIconType.BodyEffect) {
			// 移除之前的bodyEffect效果
			if (bodyBuff == _buff) {
				bodyBuff.removeEffect ();
				bodyBuff = null;
				//这里重新遍历身上的BUFF，创建特效出来
				for (int i = 0; i < buffs.Count; i++) {
					if (buffs[i].serverData.sample.getType () != BuffType.attr_change) {
						RecalculateBodyBuff(buffs[i]);
					}
				}
			}
		}
		if (_buff.serverData.sample.getTransformID () != "0") {
			//变形卡还原
			cardCtrl.changePicture (null);
		}
	}
	/** 获取当前卡片上buff的总数量 */
	public int GetBuffCount () { 
		return buffs.Count; 
	}
	
	public void init (CharacterData p_info, BattleTeamManager  _team)
	{
		characterData = p_info;
		p_info.characterCtrl = this;

		if (hitPoint == null)
			hitPoint = transform.FindChild ("hitPoint");

		characterData.parentTeam = _team; 
        if(redCardFalg!=null)redCardFalg.SetActive(characterData.role.getQualityId()>5);
		cardCtrl.init ();
	}

	public int getMaxHp ()
	{
		if (characterData == null)
			return 0;
		return characterData.server_max_hp; 
	}
	
	public void setMaxHp (int p_hp)
	{
		if (characterData == null)
			return;
		characterData.server_max_hp = p_hp;
	}
	
	public int getNowHp ()
	{
		if (characterData == null)
			return 0;
		return characterData.server_hp;
		
	}

	public void setNowHp (int p_hp)
	{
		if (characterData == null)
			return;
		characterData.server_hp = p_hp;
	}
	
	public  enum_character_state getState ()
	{
		return state;
	}
	
	//正常的角色动作输入,根据动作行为播放战斗
	public   void  setState (CharacterAction _action)
	{ 
		//如果是死亡动作，那么攻击他的人继续他的连击
		if (_action.action == enum_character_Action.Dead) {
			if (activeAction != null && activeAction.Skill != null) {
				//连击 目标死，连击者继续
				if (activeAction.Skill.triggerSkill.serverData.sample.getType () == SkillType.ComboAttack && activeAction.Skill.trigger.characterCtrl.waitForFightBack == true) {
					StartCoroutine (continueCombo (activeAction.Skill.trigger, comboDelay));
					activeAction.Skill.trigger.characterCtrl.waitForFightBack = false;
				} else if (activeAction.Skill.serverData.sample.getType () == SkillType.HelpOther || activeAction.Skill.serverData.sample.getType () == SkillType.FightBack || activeAction.Skill.serverData.sample.getType () == SkillType.FightBackAfterHelp) {
					//援护死了,攻击者waitForFightBack取消
					//这里把反击的情况也包含了,理论上反击不回导致死的
					activeAction.Skill.trigger.characterCtrl.waitForFightBack = false;
				} else if (activeAction.Skill.serverData.sample.getType () == SkillType.ComboAttack) {
					//连击者被反击死
					activeAction.Skill.user.characterCtrl.waitForFightBack = false; 
				} 
			}
		} 
		if (activeAction != null) {
		
			activeAction.Remove ();
		}
		
		activeAction = _action;
		isWorking = true;

		
		state = changeState ();
		
		if (state == enum_character_state.attacked || state == enum_character_state.fightBacked || state == enum_character_state.standing || state == enum_character_state.awaked
			|| state == enum_character_state.groupAttacked || state == enum_character_state.helped || state == enum_character_state.helped || state == enum_character_state.standing) {
			isWorking = false;
		} 
	}
	
	public void	addDamage (BuffCtrl buff)
	{
		
		if (damageBuff == null)
			damageBuff = new List<BuffCtrl> ();
		
		
		damageBuff.Add (buff);
		
		
	}

	public void 	BeHit ()
	{
		BeHit (false);
	}
	
	public void	BeHit (bool hitBack)
	{
		if (damageBuff == null || damageBuff.Count < 1)
			return;
		isHurting = true;
		if (damageBuff [0].serverData.serverDamage <= 0) {//如果是扣血，就抖动
		
			cardCtrl.shake (hitBack);
		} else {
			
			//大于0是加血
			
			cardCtrl.shakeAnimComplete ();
			
		}
		
 
	}

	public void stand ()
	{
		CharacterAction ac = new CharacterAction (enum_character_Action.Standy);
		setState (ac); 
	}

	public 	static void playCharacterAnim (CharacterCtrl player, string animName)
	{
		if (player.anim != null) {
			if (player.characterData.parentTeam.isGamePlayerTeam == true)
				player.anim.Play (animName);
			else
				player.anim.Play (animName + "_enemy");
		}
	}

	//只变换状态，一般用于动画结束后的回调
	public void setStateImmediate (enum_character_state _state)
	{
		state = _state;
		if (state == enum_character_state.attacked || state == enum_character_state.fightBacked || state == enum_character_state.standing || state == enum_character_state.awaked
			|| state == enum_character_state.groupAttacked || state == enum_character_state.helped || state == enum_character_state.helped) {
			isWorking = false;
		}
	}
	//其他连击特效
	IEnumerator otherMeleeAttack()
	{
		//BuffCtrl damageBuff = null;
		int damageAll = 0; 	
		//找到对应的伤害buff,单体目标肯定只有1
		foreach (BuffCtrl each in activeAction.Skill.buffs) { // 先统计所有buff伤害
			if (each.serverData.sample.getType () == BuffType.damage) {	
				//算出单次伤害 
				damageAll += each.serverData.serverDamage;
			}
		}
		activeAction.Skill.applyBuff (activeAction.Skill.buffs);
		SkillCtrl sc=activeAction.Skill;//释放者的
		CharacterData cd=sc.targets[0];//其中一个技能目标
		List<BuffCtrl> damageBuff = cd.characterCtrl.damageBuff;//这个目标中的伤害buff
		if(damageBuff!=null&&damageBuff.Count>1){
			for(int i=0;i<damageBuff.Count;i++){//细分为多少次伤害
				UiManager.Instance.battleWindow .comboBar.init (damageBuff.Count,damageAll);
				yield return new WaitForSeconds (0.1f);
			}
		}

		//StartCoroutine (cardCtrl.goBack ());
	}
	IEnumerator subMeleeAttack ()
	{
		BuffCtrl damageBuff = null;
		int damageAll = 0; 	
		//找到对应的伤害buff,单体目标肯定只有1
        foreach (BuffCtrl each in activeAction.Skill.buffs) {// 先统计所有buff伤害
		    if (activeAction.Skill.serverData.sample.getAttackNum() > 1)
		    {
                if (each.serverData.sample.getType() == BuffType.damage&&each.serverData.serverDamage<=0) {
                    //算出单次伤害 
                    damageAll += each.serverData.serverDamage;
                }
		    }
		    else
		    {
                if (each.serverData.sample.getType() == BuffType.damage) {
                    //算出单次伤害 
                    damageAll += each.serverData.serverDamage;
                }
		    }
			
		}
		foreach (BuffCtrl each in activeAction.Skill.buffs) { // 细分累计的伤害数据
		    if (activeAction.Skill.serverData.sample.getAttackNum() > 1)
		    {
                if (each.serverData.sample.getType() == BuffType.damage&&each.serverData.serverDamage <= 0) {
                    int subDamage = (int)(damageAll / activeAction.Skill.serverData.sample.getAttackNum());
                    each.serverData.serverDamage = subDamage;
                    damageBuff = each;
                }
		    }
		    else
		    {
                if (each.serverData.sample.getType() == BuffType.damage) {
                    int subDamage = (int)(damageAll / activeAction.Skill.serverData.sample.getAttackNum());
                    each.serverData.serverDamage = subDamage;
                    damageBuff = each;
                }
		    }
			
		}

		//原来的一个再加上复制的几个
		damageBuff.bulletID = 1;
		for (int i=1; i<activeAction.Skill.serverData.sample.getAttackNum(); i++) {
			BuffCtrl ctrl = copyDamageBuff (damageBuff);
			ctrl.bulletID = i + 1;//设置子弹id,对应攻击ID爆对应伤害
			activeAction.Skill.AddBuffCtrl (ctrl);
			
			if (i == activeAction.Skill.serverData.sample.getAttackNum () - 1) {
				//最后一个伤害补足
				ctrl.serverData.serverDamage = damageAll - damageBuff.serverData.serverDamage * (activeAction.Skill.serverData.sample.getAttackNum () - 1);
			}
			
		}
		
		cardCtrl.attackScaleDown ();
		//动画表现
        List<BuffCtrl> bufftemp=null;
		for (int i=0; i<activeAction.Skill.serverData.sample.getAttackNum(); i++) {

			//中途人死了中断细分攻击
			if (activeAction.Skill.targets [0].characterCtrl.state == enum_character_state.deaded || activeAction.Skill.targets [0].characterCtrl.state == enum_character_state.deading) {
				break;
			}

			playCharacterAnim (this, "cardHitAnim");

            if (i == 0)
            {
                for (int j = 0; j < activeAction.Skill.buffs.Count; j++)
                {
                    if (activeAction.Skill.buffs[j].serverData.sample.getDisplayType() == BuffIconType.BodyEffect||
                        (activeAction.Skill.buffs[j].trigger == activeAction.Skill.buffs[j].owner&&
                        activeAction.Skill.buffs[j].serverData.serverDamage>0&&
                        activeAction.Skill.buffs[j].serverData.sample.getType()== BuffType.damage))
                    {
                        if (bufftemp == null) bufftemp = new List<BuffCtrl>();
                        bufftemp.Add(activeAction.Skill.buffs[j]);
                    }
                }
            }
            if (i==1&&bufftemp != null)
            {
                for (int m = 0; m < bufftemp.Count; m++)
                {
                    activeAction.Skill.buffs.Remove(bufftemp[m]);
                }
            }
            activeAction.Skill.applyBuff (activeAction.Skill.buffs, i + 1);
			UiManager.Instance.battleWindow .comboBar.init (i + 1,damageAll);
			yield return new WaitForSeconds (0.3f);
		}
		StartCoroutine (cardCtrl.goBack ());//AttackAnimComplete()
		//触发反击
		BattleManager.Instance.getActiveBattleInfo ().FightBack (characterData, SkillActiveType.SkillHit);
		//触发急救
		BattleManager.Instance.getActiveBattleInfo ().Medical (characterData);
		
	}

	BuffCtrl copyDamageBuff (BuffCtrl orgBuff)
	{
		BuffCtrl ctrl = new BuffCtrl ();
		ctrl.owner = orgBuff.owner;
		ctrl.trigger = orgBuff.trigger;
		ctrl.DisplayEffect = orgBuff.DisplayEffect;
		
		ctrl.option = orgBuff.option;
		ctrl.parentSkill = orgBuff.parentSkill;
		ctrl.serverData = new BuffData (orgBuff.serverData.sid, orgBuff.serverData.serverDamage);
		ctrl.serverData.damageEffect = orgBuff.serverData.damageEffect;
		return ctrl;
	}
	/// <summary>
/// 远程单体攻击的细分伤害
/// </summary>
	IEnumerator subRemoteAttack ()
	{
		BuffCtrl damageBuff = null;
		int damageAll = 0; 
		//找到对应的伤害buff,单体目标肯定只有1
		foreach (BuffCtrl each in activeAction.Skill.buffs) {  // 先统计所有buff伤害
			if (each.serverData.sample.getType () == BuffType.damage) {
				//算出单次伤害
				damageAll += each.serverData.serverDamage;
			}
		}
        List<BuffCtrl> tempTotalBuffs = new List<BuffCtrl>();
		//找到对应的伤害buff,单体目标肯定只有1
		foreach (BuffCtrl each in activeAction.Skill.buffs) { // 细分累计的伤害数据
			if (each.serverData.sample.getType () == BuffType.damage) {
				int subDamage = (int)(damageAll / activeAction.Skill.serverData.sample.getAttackNum ());
				each.serverData.serverDamage = subDamage;
				damageBuff = each;
            } else {
                tempTotalBuffs.Add(each);
            }
		}	
		//移除本来的,后面会补完
		activeAction.Skill.RemoveBuffCtrl (damageBuff);
		//动画表现
 
		for (int i=0; i<activeAction.Skill.serverData.sample.getAttackNum(); i++) {
			//一颗子弹一个buff
			BuffCtrl newbuff = copyDamageBuff (damageBuff);
			activeAction.Skill.AddBuffCtrl (newbuff);
			BulletCtrl bullet = null;
			List<BuffCtrl > tmplist = new List<BuffCtrl> ();
			tmplist.Add (newbuff);
			//表现连击
			UiManager.Instance.battleWindow .comboBar.init (i + 1,damageAll);
			playCharacterAnim (this, "cardThrowAnim");

			if (i == activeAction.Skill.serverData.sample.getAttackNum () - 1) {
				//最后一个伤害补足
                tempTotalBuffs.Add(newbuff);
				newbuff.serverData.serverDamage = damageAll - damageBuff.serverData.serverDamage * (activeAction.Skill.serverData.sample.getAttackNum () - 1);
                bullet = EffectManager.Instance.CreateBulletEffect(characterData, activeAction.Skill.targets[0], tempTotalBuffs, activeAction.Skill.serverData.sample.getBulletEffect(), true);
				newbuff.bulletID = bullet.GetInstanceID ();
				yield break;
			} else {
				bullet = EffectManager.Instance.CreateBulletEffect (characterData, activeAction.Skill.targets [0], tmplist, activeAction.Skill.serverData.sample.getBulletEffect (), false);
				newbuff.bulletID = bullet.GetInstanceID ();
				yield return new WaitForSeconds (0.2f);
			}
		}

	}
	//攻击回来结束skill
	void goAttackAnimComplete ()
	{
		if (activeAction.Skill.serverData.sample.getAttackNum () > 1) {
			//细分攻击次数
			StartCoroutine (subMeleeAttack ());
		} else {
			//一下是单次攻击的情况
			StartCoroutine (cardCtrl.goBack ());
			//散播技能buff
			activeAction.Skill.applyBuff (activeAction.Skill.buffs);
			cardCtrl.attackScaleDown ();
		
			//触发反击
			BattleManager.Instance.getActiveBattleInfo ().FightBack (characterData, SkillActiveType.SkillHit);
			//触发急救
			BattleManager.Instance.getActiveBattleInfo ().Medical (characterData);
		}
	}
	
	//召唤兽进场展示升级
	public void GuardianForceShow ()
	{
		cardCtrl.GuardianForceMoveToShow ();
		
	}
	
	//召唤兽进场后
	public void GuardianForceReady ()
	{


		EffectCtrl effect = EffectManager.Instance.CreateEffect (hitPoint, activeAction.Skill.serverData.sample.getSpellEffect ());
		//放施法特效,加技能板
		cardCtrl.SkillGrowUp (effect.life);
		//EffectManager.Instance.CreateSkillNameEffect (this, activeAction.Skill.serverData.sample.getName ());			
	}
	
	//战斗完成后出场
	public void GuardianForceOut ()
	{
		//完全退场后显示玩家
		if (characterData.isGuardianForce == true) {
			BattleManager.Instance.playerTeam.showAllParter ();
			BattleManager.Instance.enemyTeam.showAllParter ();
			BattleManager.Instance.changeBackGroundColor (new Color(0.1f,0.1f,0.1f,1f));
		}	
		
	}

	void hideOther ()
	{
		HpBar.gameObject.SetActive (false);
		buffColumn [0].transform.parent.gameObject.SetActive (false);
		if(shadow!=null)
			shadow.SetActive(false);
	}


	/// <summary>
	/// 开始播放对应行为的战斗
	/// </summary>
	/// <param name="_ac"></param>
	/// <param name="skill">当前的技能控制</param>
	public void beginAction (enum_character_Action _ac, SkillCtrl skill)
	{ 
		setState (new CharacterAction (_ac, skill)); 
	}

	public void deadComplete ()
	{
		setStateImmediate (enum_character_state.deaded);
		activeAction.Remove ();
	}
	//角色状态变换
	enum_character_state  changeState ()
	{
//		panel.depth = (int)(characterData.orgPosition.y * 400);
		
		if (state == enum_character_state.deaded || state == enum_character_state.deading || state == enum_character_state.attacking) {
			return state;//这里说明此人很忙或躺.
		}


		switch (activeAction.action) {
		case enum_character_Action.Standy:
			return enum_character_state.standing;		
		case enum_character_Action.Dead:
			cardCtrl.gray ();
		//	hideOther ();
			return enum_character_state.deading;		
		case enum_character_Action.UseSkill:
			if (activeAction.Skill.serverData.sample.getType () == SkillType.Normal) {
				//触发援护事件;
				BattleManager.Instance.getActiveBattleInfo ().HelpOther (characterData);	
				cardCtrl.goToAttack ();
				//	cardCtrl.goToAttack ();
			} else	if (activeAction.Skill.serverData.sample.getType () == SkillType.NormalRemote) {
				//触发援护事件;
				BattleManager.Instance.getActiveBattleInfo ().HelpOther (characterData);	
				launchBullet ();
			 

			} else if (activeAction.Skill.serverData.sample.getType () == SkillType.fixAOE || activeAction.Skill.serverData.sample.getType () == SkillType.Remote ||
				activeAction.Skill.serverData.sample.getType () == SkillType.MeleeAOE || activeAction.Skill.serverData.sample.getType () == SkillType.MeleeChargeAOE ||
				activeAction.Skill.serverData.sample.getType () == SkillType.MeleeJumpAOE || activeAction.Skill.serverData.sample.getType () == SkillType.bulletMultipleAOE) {
				//Remote触发援护事件;
				if (activeAction.Skill.serverData.sample.getType () == SkillType.Remote)
					BattleManager.Instance.getActiveBattleInfo ().HelpOther (characterData);	

				useSkill ();
				
			} else if (activeAction.Skill.serverData.sample.getType () == SkillType.GroupCombine) {
				
				//召唤合击
				useSkill ();
				
			} else if (activeAction.Skill.serverData.sample.getType () == SkillType.AddNumber) {
				
				useSkill ();
				
			}
			if (activeAction.Skill.serverData.sample.getType () == SkillType.Melee) {
				//单体近战技能攻击
				useSkill ();
			
			}
			if (activeAction.Skill.serverData.sample.getType () == SkillType.MedicalAfterHelp) {
				
				useSkill ();
			
			}
			if (activeAction.Skill.serverData.sample.getType () == SkillType.BuffCheck) {
				
				activeAction.Skill.applyBuff (activeAction.Skill.buffs);
				
				if (state == enum_character_state.attacking)
					setStateImmediate (enum_character_state.attacked);
				
				stand ();
				return enum_character_state.standing;
			}			
			
			return enum_character_state.attacking;		
		case enum_character_Action.ComboAttack:
			//连击也会触发援护哦亲
			BattleManager.Instance.getActiveBattleInfo ().HelpOther (characterData);
			cardCtrl.goToAttack (true);
			
			return enum_character_state.comboAttacking;				
			
		case enum_character_Action.FightBack:
		 //反击不触发援护急救=.=;
			EffectManager.Instance.CreateReboundLabel (gameObject.transform.position);
            //cardCtrl.goToAttack ();SkillSampleManager.Instance.getSkillSampleBySid
            int tempSid = SkillSampleManager.Instance.getSkillSampleBySid(activeAction.Skill.serverData.sample.sid).spellEffect;
            if (SkillSampleManager.Instance.getSkillSampleBySid(tempSid)==null)
            {
                cardCtrl.goToAttack();
            }
		    else useBackSkill();
			
			return enum_character_state.fightBacking;				
				
		case enum_character_Action.HelpOther:
			cardCtrl.goToHelp ();
			return enum_character_state.helping;		
			
		case enum_character_Action.Medical:
			//在这里开始急救
			useSkill ();
			return enum_character_state.medicaling;				
			
		case enum_character_Action.GroupAttack:			 //合击也不触发援护急救哦
			//先到操场集合啦！
			cardCtrl.moveToAttackPoint ();
			return enum_character_state.groupAttacking;		  
		} 
		return enum_character_state.deaded;
	}

	void useSkill ()
	{ 
//		print ("my name is " + characterData.serverID + ", I use skill:" + activeAction.Skill.serverData.sample.sid); 
		//如果是召唤兽o
		if (characterData.isGuardianForce == true) {
			cardCtrl.GuardianForceMoveToBattle ();
			return;
		}

		EffectCtrl effect = EffectManager.Instance.CreateEffect (hitPoint, activeAction.Skill.serverData.sample.getSpellEffect ());
		float time=effect.life-effect.startlife;
		if(time<0) time=0;
		//变大,放施法特效,加技能板
        cardCtrl.SkillGrowUp(time);// 完成后调用 SkillGrowUpComplete();
        EffectManager.Instance.CreateSkillNameEffect(this, activeAction.Skill.serverData.sample.getName());

        if (characterData.role.sid == 3 && activeAction.Skill.serverData.sample.isFemaleSoldiersSkill())
            hideCard(); 
	}

    //隐藏卡片
    private void hideCard()
    {
        gameObject.transform.localRotation = new Quaternion(0, 0, 90, 0);
        StartCoroutine(Utils.DelayRun(() => {
            gameObject.transform.localRotation = new Quaternion(0, 0, 0, 0);
        }, 2.5f));
    }

    /// <summary>
    /// 使用反击技能
    /// </summary>
    void useBackSkill() {
        //		print ("my name is " + characterData.serverID + ", I use skill:" + activeAction.Skill.serverData.sample.sid); 
        //如果是召唤兽o
        if (characterData.isGuardianForce == true) {
            cardCtrl.GuardianForceMoveToBattle();
            return;
        }
        Skill tempSkill=activeAction.Skill.serverData.sample.getBackSkillSid();
        EffectCtrl effect = EffectManager.Instance.CreateEffect(hitPoint, tempSkill.getSpellEffect());
        float time = effect.life - effect.startlife;
        if (time < 0) time = 0;
        //变大,放施法特效,加技能板
        cardCtrl.BackSkillGrowUp(time);// 完成后调用 BackSkillGrowUpComplete();
        EffectManager.Instance.CreateSkillNameEffect(this, tempSkill.getName());
    }
    void BackSkillGrowUpComplete() {
        Skill tempSkill = activeAction.Skill.serverData.sample.getBackSkillSid();
        if (tempSkill.getType() == SkillType.AddNumber) {
            //使用技能放大后有个情况就是：我要去连击！
            StartCoroutine(continueCombo(characterData, 0f));
            //之后的连击都不会带召唤兽怒气buff，所以这里要激活buff
            activeAction.Skill.applyMonsterBuffOnly(characterData);
        } else if (tempSkill.getType() == SkillType.MedicalAfterHelp) {

            if (activeAction.Skill.trigger != null && activeAction.Skill.trigger.characterCtrl.waitForFightBack == true) {
                activeAction.Skill.trigger.characterCtrl.waitForFightBack = false;
                StartCoroutine(continueCombo(activeAction.Skill.trigger, comboDelay * 1.5f));
            }

            cardCtrl.SKillScaleDown();
        } else if (tempSkill.getType() == SkillType.Melee) {

            BattleManager.Instance.getActiveBattleInfo().HelpOther(characterData);
            cardCtrl.goToAttack(true);

        } else if (tempSkill.getType() == SkillType.MeleeAOE) {

            //近战AOE不能触发援护和反击
            if (anim != null)
                anim.Play("cardThrowAnim");
            EffectManager.Instance.CreateBulletEffect(characterData, activeAction.Skill.targets[0], activeAction.Skill.buffs, tempSkill.getBulletEffect(), true);
            cardCtrl.SKillScaleDown();

        } else if (tempSkill.getType() == SkillType.MeleeChargeAOE) {

            //冲锋AOE不能触发援护和反击
            cardCtrl.ChargeAOE();
            EffectManager.Instance.CreateEffect(hitPoint, tempSkill.getAroundEffectPath());
            if (anim != null)
                anim.Play("cardAroundAnim");
        } else if (tempSkill.getType() == SkillType.MeleeJumpAOE) {

            //跳斩AOE不能触发援护和反击
            cardCtrl.jumpAOE();
        } else if (tempSkill.getType() == SkillType.GroupCombine) {
            cardCtrl.groupCombieSKillScaleDown();

        } else {
            //不然你就发射子弹缩小over
            backLaunchBullet();

            cardCtrl.SKillScaleDown();

        }
    }
    //发射子弹,急救自己人也在这里哦！！！
    void backLaunchBullet() {
        Skill tempSkill = activeAction.Skill.serverData.sample.getBackSkillSid();
        if (tempSkill.getType() == SkillType.Remote) {
            if (tempSkill.getAttackNum() > 1) {
                //有细分伤害的情况
                StartCoroutine(subRemoteAttack());
            } else {
                //单发
                EffectManager.Instance.CreateBulletEffect(characterData, activeAction.Skill.targets[0], activeAction.Skill.buffs, tempSkill.getBulletEffect(), true);
                playCharacterAnim(this, "cardThrowAnim");
            }
            return;
        } else if (tempSkill.getType() == SkillType.fixAOE) {
            //这里目标随便选个好了，反正选哪个是打全组,有细分伤害的情况
            EffectCtrl _bullet = EffectManager.Instance.CreateBulletEffect(characterData, activeAction.Skill.targets[0], activeAction.Skill.buffs, tempSkill.getBulletEffect(), true);
            playCharacterAnim(this, "cardThrowAnim");

        } else if (tempSkill.getType() == SkillType.bulletMultipleAOE) {
            playCharacterAnim(this, "cardThrowAnim");

            List<BuffCtrl> srcBuffList = new List<BuffCtrl>();
            srcBuffList.AddRange(activeAction.Skill.buffs);

            //散射子弹型AOE
            foreach (CharacterData each in activeAction.Skill.targets) {

                //收集属于目标的buff
                List<BuffCtrl> buffs = new List<BuffCtrl>();
                foreach (BuffCtrl buff in activeAction.Skill.buffs) {
                    buff.bulletID = 0;
                    if (buff.owner == each) {
                        buffs.Add(buff);
                        srcBuffList.Remove(buff);
                    }
                }

                if (each == activeAction.Skill.targets[activeAction.Skill.targets.Count - 1])
                    EffectManager.Instance.CreateBulletEffect(characterData, each, buffs, tempSkill.getBulletEffect(), true);
                else
                    EffectManager.Instance.CreateBulletEffect(characterData, each, buffs, tempSkill.getBulletEffect(), false);
            }
            //如果是多发子弹,并且多发后srcBuffList还有剩余Buff，则说明自己应该附加上剩余的Buff
            //ex:攻击多个目标后，增加自己的属性！原有的逻辑不支持，只能先这样做
            if (activeAction.Skill.targets.Count > 1 && srcBuffList.Count > 0) {
                activeAction.Skill.applyBuff_Src(srcBuffList, 0);
            }



        } else if (tempSkill.getType() == SkillType.NormalRemote) {
            //远程普通
            playCharacterAnim(this, "cardThrowAnim");
            EffectManager.Instance.CreateBulletEffect(characterData, activeAction.Skill.targets[0], activeAction.Skill.buffs, tempSkill.getBulletEffect(), true);
            return;

        }



    }
	void SkillGrowUpComplete ()
	{ 
	
		if (activeAction.Skill.serverData.sample.getType () == SkillType.AddNumber) {
			//使用技能放大后有个情况就是：我要去连击！
			StartCoroutine (continueCombo (characterData, 0f));
			//之后的连击都不会带召唤兽怒气buff，所以这里要激活buff
			activeAction.Skill.applyMonsterBuffOnly (characterData);
		} else if (activeAction.Skill.serverData.sample.getType () == SkillType.MedicalAfterHelp) {
			
			if (activeAction.Skill.trigger != null && activeAction.Skill.trigger.characterCtrl.waitForFightBack == true) {
				activeAction.Skill.trigger .characterCtrl.waitForFightBack = false; 		
				StartCoroutine (continueCombo (activeAction.Skill.trigger, comboDelay * 1.5f));
			}

			cardCtrl.SKillScaleDown ();
		} else if (activeAction.Skill.serverData.sample.getType () == SkillType.Melee) {

			BattleManager.Instance.getActiveBattleInfo ().HelpOther (characterData);
			cardCtrl.goToAttack (true);
			
		} else if (activeAction.Skill.serverData.sample.getType () == SkillType.MeleeAOE) {

			//近战AOE不能触发援护和反击
			if (anim != null)
				anim.Play ("cardThrowAnim");
			EffectManager.Instance.CreateBulletEffect (characterData, activeAction.Skill.targets [0], activeAction.Skill.buffs, activeAction.Skill.serverData.sample.getBulletEffect (), true);
			cardCtrl.SKillScaleDown ();
		
		} else if (activeAction.Skill.serverData.sample.getType () == SkillType.MeleeChargeAOE) {
			
			//冲锋AOE不能触发援护和反击
			cardCtrl.ChargeAOE ();
			EffectManager.Instance.CreateEffect (hitPoint, activeAction.Skill.serverData.sample.getAroundEffectPath ());
			if (anim != null)
				anim.Play ("cardAroundAnim");
		} else if (activeAction.Skill.serverData.sample.getType () == SkillType.MeleeJumpAOE) {
			
			//跳斩AOE不能触发援护和反击
			cardCtrl.jumpAOE ();
		} else if (activeAction.Skill.serverData.sample.getType () == SkillType.GroupCombine) {
			cardCtrl.groupCombieSKillScaleDown ();

		} else {
			//不然你就发射子弹缩小over
			launchBullet ();
		
			cardCtrl.SKillScaleDown ();
		
		} 
	}

	void  jumpOver ()
	{
		BattleManager.Instance.shakeCamera ();
		//播放一个普通特效
		EffectManager.Instance.CreateEffect (hitPoint, activeAction.Skill.serverData.sample.getBulletEffect ());
		activeAction.Skill.applyBuff (activeAction.Skill.buffs);
		cardCtrl.meleeAOEgoBack (1f);

	}

	void beginMeleeAOE ()
	{

		//	BulletCtrl _bullet = EffectManager.Instance.CreateBulletEffect (characterData, activeAction.Skill.targets [0], activeAction.Skill.skillData.skill.getBulletEffect (), true);
		activeAction.Skill.applyBuff (activeAction.Skill.buffs);
		cardCtrl.meleeAOEgoBack (1f);

	}

	IEnumerator continueCombo (CharacterData data, float _time)
	{
//		MonoBase.print ("begin comboskill:");
		yield return new WaitForSeconds (_time);
		//触发者继续连击： 
		BattleManager.Instance.getActiveBattleInfo ().comboAttack (data); 
	}

	void moveToAttackPointComplete ()
	{ 
		if (_isLastGroupAttacker == true) {
			//开始合击循环
			BattleManager.Instance.getActiveBattleInfo ().NextGroupAttack ();
		}
	}
	//发射子弹,急救自己人也在这里哦！！！
	void launchBullet ()
	{
			if (activeAction.Skill.serverData.sample.getType () == SkillType.Remote) {
				if (activeAction.Skill.serverData.sample.getAttackNum () > 1) {
					//有细分伤害的情况
					StartCoroutine (subRemoteAttack ());
				} else {
					//单发
					EffectManager.Instance.CreateBulletEffect (characterData, activeAction.Skill.targets [0], activeAction.Skill.buffs, activeAction.Skill.serverData.sample.getBulletEffect (), true);
					playCharacterAnim (this, "cardThrowAnim");
				}
				return;
			} else	if (activeAction.Skill.serverData.sample.getType () == SkillType.fixAOE) {
				//这里目标随便选个好了，反正选哪个是打全组,有细分伤害的情况
				EffectCtrl _bullet = EffectManager.Instance.CreateBulletEffect (characterData, activeAction.Skill.targets [0], activeAction.Skill.buffs, activeAction.Skill.serverData.sample.getBulletEffect (), true);
				playCharacterAnim (this, "cardThrowAnim");

			} else  if (activeAction.Skill.serverData.sample.getType () == SkillType.bulletMultipleAOE) {
				playCharacterAnim (this, "cardThrowAnim");

				List<BuffCtrl> srcBuffList=new List<BuffCtrl>();
							   srcBuffList.AddRange(activeAction.Skill.buffs);

				//散射子弹型AOE
				foreach (CharacterData each in activeAction.Skill.targets) {

					//收集属于目标的buff
					List<BuffCtrl> buffs = new List<BuffCtrl> ();
					foreach (BuffCtrl buff in activeAction.Skill.buffs) {
						buff.bulletID = 0;
						if (buff.owner == each)
						{
							buffs.Add (buff); 
							srcBuffList.Remove(buff);
						}
					}

					if (each == activeAction.Skill.targets [activeAction.Skill.targets.Count - 1])
						EffectManager.Instance.CreateBulletEffect (characterData, each, buffs, activeAction.Skill.serverData.sample.getBulletEffect (), true);
					else
						EffectManager.Instance.CreateBulletEffect (characterData, each, buffs, activeAction.Skill.serverData.sample.getBulletEffect (), false);	
				}
				//如果是多发子弹,并且多发后srcBuffList还有剩余Buff，则说明自己应该附加上剩余的Buff
				//ex:攻击多个目标后，增加自己的属性！原有的逻辑不支持，只能先这样做
				if(activeAction.Skill.targets.Count>1 && srcBuffList.Count>0)
				{
					activeAction.Skill.applyBuff_Src (srcBuffList,0);
				}



			} else if (activeAction.Skill.serverData.sample.getType () == SkillType.NormalRemote) {
				//远程普通
				playCharacterAnim (this, "cardThrowAnim");
				EffectManager.Instance.CreateBulletEffect (characterData, activeAction.Skill.targets [0], activeAction.Skill.buffs, activeAction.Skill.serverData.sample.getBulletEffect (), true);
				return;

			}



	}
	
	public  void SkillScaleDownComplete ()
	{

		if (state == enum_character_state.attacking)
			setStateImmediate (enum_character_state.attacked);
		if (state == enum_character_state.fightBacking)
			setStateImmediate (enum_character_state.fightBacked);		
		if (state == enum_character_state.medicaling)
			setStateImmediate (enum_character_state.medicaled);				
		if (activeAction.Skill!=null&&activeAction.Skill.serverData.sample.getType () == SkillType.GroupCombine) {	
			//叫合击男们到操场集合准备合击咯
			BattleManager.Instance.getActiveBattleInfo ().GroupCombineTargets (characterData);
		} else if (activeAction.Skill!=null&&activeAction.Skill.serverData.sample.getType () == SkillType.MedicalAfterHelp) {	
			activeAction.Skill.applyBuff (activeAction.Skill.buffs);
			//这里是急救skill特效播放完后的后续哦
		} else {
			BattleManager.Instance.getActiveBattleInfo ().FightBack (characterData, SkillActiveType.SkillHit);
			stand ();			
		} 
	}

	void GuardianForceShake ()
	{
		BattleManager.Instance.shakeCamera ();
	}

	void goHelpReady ()
	{
		if (activeAction.Skill != null)
			activeAction.Skill.applyMonsterBuffOnly (characterData);
	}

	public void helpBack ()
	{
		//死人就没必要去反击了
		if (state == enum_character_state.deaded || state == enum_character_state.deading) {
			return;
		}

		//触发援护反击事件:
		if (BattleManager.Instance.getActiveBattleInfo ().FightBack (characterData, SkillActiveType.HelpOtherOver) == null) {
			
			//无反击，则触发者继续连击：
			if (activeAction.Skill.trigger.characterCtrl.waitForFightBack == true) {	
				activeAction.Skill.trigger .characterCtrl.waitForFightBack = false; 		
				StartCoroutine (continueCombo (activeAction.Skill.trigger, comboDelay * 1.5f));
			}

			iTween.MoveTo (gameObject, iTween.Hash ("delay", 0.15f, "position", characterData.orgPosition, "easetype", "easeOutQuad", "oncomplete", "goHelpComplete", "time", 0.1f));

		}
		

	}

	void goHelpComplete ()
	{
		stand ();
	}

	void DestoryCharacter ()
	{
		Destroy (HpBar.gameObject);
		Destroy (gameObject, 1f);
		
	}

	void SideChange (float _data)
	{
		
		transform.localScale = new Vector3 (_data, _data, _data);
	}
	
	void AttackGrowUpComplete ()
	{ 
		
	}

	void AttackScaleDownComplete ()
	{
		 
	}
	
	void AttackAnimComplete ()
	{

		//攻击结束,改变下状态先咯;
		if (state == enum_character_state.attacking)
			setStateImmediate (enum_character_state.attacked);
		if (state == enum_character_state.fightBacking) {
			setStateImmediate (enum_character_state.fightBacked);	
			
			if (activeAction.Skill.targets [0].characterCtrl.waitForFightBack == true) {
				activeAction.Skill.targets [0] .characterCtrl.waitForFightBack = false; 		
				//	if (activeAction.Skill.targets [0] .characterCtrl.state != enum_character_state.deaded && activeAction.Skill.targets [0] .characterCtrl.state != enum_character_state.deading)
				StartCoroutine (continueCombo (activeAction.Skill.targets [0], comboDelay));
			}
			
		}
		
		//如果是合击，那就下个人上！
		if (state == enum_character_state.groupAttacking) {
			BattleManager.Instance.getActiveBattleInfo ().NextGroupAttack ();
			setStateImmediate (enum_character_state.groupAttacked);	
			
		}
		
		//如果是连击，那就再打一炮！
		if (state == enum_character_state.comboAttacking) {
			
			if (waitForFightBack == false)
				StartCoroutine (continueCombo (characterData, comboDelay));
		 
		}		
		
		stand ();

		helpBackCheck ();
 
	
	}

	public void helpBackCheck ()
	{
		//援护人员回位
		if (helpOtherReturn != null) {
			helpOtherReturn ();
			helpOtherReturn = null;
		}
	}

	void reBirth ()
	{
		setStateImmediate (enum_character_state.standing);
	}
	
	//改变HP,数值在技能所带buff的数据里哦
	public void changeHP ()
	{
		 
		int startHp = characterData.server_hp;
		
		foreach (BuffCtrl each in damageBuff) {
			characterData.server_hp += each.serverData.serverDamage;
			if (characterData.server_hp > characterData.server_max_hp)
				characterData.server_hp = characterData.server_max_hp;
			if (characterData.server_hp < 0)
				characterData.server_hp = 0;
			
		}
		
		if (startHp <= 0 && characterData.server_hp > 0) {
			
			//对死人加血我们认为这是个复活技能
			reBirth ();
		}
	 
		
		if (HpBar != null) {
			HpBar.updateValue (characterData.server_hp, characterData.server_max_hp);
		}
	} 
}
