using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 战斗片段类
// @author 李程
//每场战报以每个主动技能开头，划分为n个战斗片段进行播放，播放下一个的触发为：标记了isOverSkill的技能释放完毕、。
public class BattleInfo
{
	/** 战斗片段的技能列表 */
	public List<SkillCtrl> SkillList;
	/** 战斗片段的buff列表 */
	public List<BuffCtrl> BuffList;
	List<SkillCtrl> GroupAttackList;
	int  GroupAttackerIndex;
	SkillCtrl lastSkill;
	BuffCtrl lastBuff;
	SkillCtrl GroupCombineSkill;

	/// <summary>
	/// 根据战斗技能数据穿件buff控制列表
	/// </summary>
	/// <param name="each">战斗技能数据</param>
	public int createBuffCtrlList (BattleSkillErlang each) {
		//buff
		if (each.skillMsg .operationType == BattleReportService.ATTR_CHANGE || each.skillMsg .operationType == BattleReportService.BUFFER_ADD
			|| each.skillMsg .operationType == BattleReportService.BUFFER_REPLACE || each.skillMsg .operationType == BattleReportService.BUFFER_REMOVE) {
				
			//如果没有上次技能，说明第一个是buff，不合格式，跳过
			BuffCtrl newBuff = new BuffCtrl ();
			if (lastSkill == null) {
				Debug.Log ("error format!!!!!");
				return 0;
			}
			//关于Hp的buff
			if (each.skillMsg .operationType == BattleReportService.ATTR_CHANGE && each.skillMsg.valueType == StringKit.toInt (BattleReportService.HP))
				each.skillMsg.skillSID = BuffSampleManager.SID_HP;
			//关于怒气的buff
			if (each.skillMsg .operationType == BattleReportService.ATTR_CHANGE && each.skillMsg.valueType == StringKit.toInt (BattleReportService.ANGER))
				each.skillMsg.skillSID = BuffSampleManager.SID_ANGER;
				
			//如果伤害类型是召唤兽怒气
			if (each.skillMsg.valueType == StringKit.toInt (BattleReportService.ANGER)) {  
				newBuff.init (BattleManager.Instance.getBattleCharacterData (each.skillMsg .targets [0]), lastSkill.user, BuffManager.Instance.CreateBuffData (each.skillMsg.skillSID, each.skillMsg.damage, lastSkill.serverData.sample.getDamageEffect ()), false, each.skillMsg.skillID, lastSkill);
				lastSkill.AddBuffCtrl (newBuff);
			} else {
				CharacterData buffOwner;
				if (lastSkill.serverData.sample.getType () == SkillType.HelpOther) {
					//如果是援护
					buffOwner = BattleManager.Instance.getBattleCharacterData (each.skillMsg .targets [0]);
					newBuff.init (buffOwner, lastSkill.trigger, BuffManager.Instance.CreateBuffData (each.skillMsg.skillSID, each.skillMsg.damage, lastSkill.serverData.sample.getDamageEffect ()), false, each.skillMsg.skillID, lastSkill);
					lastSkill.triggerSkill.AddBuffCtrl (newBuff);
					newBuff.serverData.sample.changeDamageType (BuffDamageType.beIntervene);

				} else if (lastSkill.serverData.sample.getType () == SkillType.GroupAttack) {
					
					buffOwner = BattleManager.Instance.getBattleCharacterData (each.skillMsg .targets [0]);
					newBuff.init (buffOwner, lastSkill.user, BuffManager.Instance.CreateBuffData (each.skillMsg.skillSID, each.skillMsg.damage, lastSkill.serverData.sample.getDamageEffect ()), false, each.skillMsg.skillID, lastSkill);
					lastSkill.AddBuffCtrl (newBuff);
					
				} else if (lastSkill.serverData.sample.getType () == SkillType.ComboAttack) {
					buffOwner = BattleManager.Instance.getBattleCharacterData (each.skillMsg .targets [0]);
					newBuff.init (buffOwner, lastSkill.user, BuffManager.Instance.CreateBuffData (each.skillMsg.skillSID, each.skillMsg.damage, lastSkill.serverData.sample.getDamageEffect ()), false, each.skillMsg.skillID, lastSkill);
					lastSkill.AddBuffCtrl (newBuff);
					
				} else {
					//普通伤害buff
					buffOwner = BattleManager.Instance.getBattleCharacterData (each.skillMsg .targets [0]);
//					MonoBase.print("skillMsg.skillID:"+each.skillMsg.skillID);
                    //Debug.LogError("lastSkill==="+lastSkill.serverData.sample.sid);
                    
					newBuff.init (buffOwner, lastSkill.user, BuffManager.Instance.CreateBuffData (each.skillMsg.skillSID, each.skillMsg.damage, lastSkill.serverData.sample.getDamageEffect ()), false, each.skillMsg.skillID, lastSkill);
					lastSkill.AddBuffCtrl (newBuff);
					if (lastSkill.serverData.sample.getType () == SkillType.FightBack) {
						newBuff.serverData.sample.changeDamageType (BuffDamageType.beRebound);
					}
 
					
					if (lastBuff != null) {
						if (lastBuff.option == buff_option.Add || lastBuff.option == buff_option.active) {
							//如果上个buff操作类型是增加buff
							//检查是否为特殊伤害类型，比如毒
							newBuff.serverData.sample.changeDamageType (lastBuff.serverData.sample.getDamageType ());

						}
					}
					 
				}
			}
			//lastSkill.AddBuffCtrl(newBuff);
			
			//根据操作类型不同做不同操作
			switch (each.skillMsg .operationType) {
			case BattleReportService.ATTR_CHANGE:
				addBuff (newBuff);
				newBuff.option = buff_option.none;
				break;
			case  BattleReportService.BUFFER_REPLACE:
				newBuff.serverData.replaceID = each.skillMsg.oldSkillID;
				newBuff.option = buff_option.Replace;
				//	replaceBuff (newBuff);
				break;
			case BattleReportService.BUFFER_REMOVE:		
				addBuff (newBuff);
//				MonoBase.	print(newBuff.buffData.database.buffType);
				newBuff.option = buff_option.Remove;
				break;
			case  BattleReportService.BUFFER_ADD:	
			//	newBuff.buffData.IsDurationBuff = true;
				newBuff.option = buff_option.Add;
				addBuff (newBuff);

				newBuff.serverData.serverAttr_attack = each.skillMsg.getValueByEffectType (BuffEffectType.ATTACK);
				newBuff.serverData.serverAttr_defend = each.skillMsg.getValueByEffectType (BuffEffectType.DEFENSE);
				newBuff.serverData.serverAttr_magic = each.skillMsg.getValueByEffectType (BuffEffectType.MAGIC);
				newBuff.serverData.serverAttr_dex = each.skillMsg.getValueByEffectType (BuffEffectType.AGILE);
				break;		
			case  BattleReportService.BUFFER_ABILITY:	
			//	newBuff.buffData.IsDurationBuff = true;
				newBuff.option = buff_option.active;
				addBuff (newBuff);
				newBuff.serverData.serverAttr_attack = each.skillMsg.getValueByEffectType (BuffEffectType.ATTACK);
				newBuff.serverData.serverAttr_defend = each.skillMsg.getValueByEffectType (BuffEffectType.DEFENSE);
				newBuff.serverData.serverAttr_magic = each.skillMsg.getValueByEffectType (BuffEffectType.MAGIC);
				newBuff.serverData.serverAttr_dex = each.skillMsg.getValueByEffectType (BuffEffectType.AGILE);
				break;					
			default:
				addBuff (newBuff);
				break;
			}
			lastBuff = newBuff;
			//下一个skill
			return 1;
		}
		//继续往下
		return 2; 
	}

	int  findLastSkill (BattleInfoErlang info)
	{
		for (int i=info.battleSkill.Count-1; i>=0; i--) {
			if (info.battleSkill [i].skillMsg.operationType != BattleReportService.ATTR_CHANGE && info.battleSkill [i].skillMsg.operationType != BattleReportService.BUFFER_ADD 
				&& info.battleSkill [i].skillMsg.operationType != BattleReportService.BUFFER_REPLACE && info.battleSkill [i].skillMsg.operationType != BattleReportService.BUFFER_REMOVE
				&& info.battleSkill [i].skillMsg.operationType != BattleReportService.BUFFER_ABILITY) {
				return  i;
			}
		}
		return -1;
	}

	/// <summary>
	/// 初始化战斗信息片段[将战斗数据转换为战斗播放时的显示数据]
	/// </summary>
	/// <param name="info">战斗片段信息数据</param>
	/// <param name="finalInfo">是否为整场战斗的最后一次攻击</param>
	public BattleInfo (BattleInfoErlang info, bool  finalInfo)
	{
		int _lastIndex = -1;
		if (SkillList == null)
			SkillList = new List<SkillCtrl> ();
		if (BuffList == null)
			BuffList = new List<BuffCtrl> ();		
		//找出最后一个技能
		_lastIndex = findLastSkill (info);
		if (_lastIndex == -1) {
			Debug.LogError ("no main skill");
			BattleManager.Instance.nextBattle ();
			return;
		}
		//处理本次info的所有skill
		for (int i=0; i< info.battleSkill.Count; i++) {
			BattleSkillErlang each = info.battleSkill [i];
			List<CharacterData> _lst = new List<CharacterData> ();
			CharacterData _user;
			CharacterData _trigger;
			//注入使用者
            
			_user = BattleManager.Instance.getBattleCharacterData (each.skillMsg .userID);
			int res = createBuffCtrlList (each);
			if (res == 0)
				break;
			else if (res == 1)
				continue;
			else {
			} 
			//以下是技能，buff已经处理干净
			SkillCtrl newSkill = new SkillCtrl (); 
			//标注最后一个技能
			if (i == _lastIndex) {
				newSkill.isOverSkill = true;
			}
			//注入目标
			if (each.skillMsg.targets != null) {
				foreach (int eachTarget in each.skillMsg.targets) {
					CharacterData _target = BattleManager.Instance.getBattleCharacterData (eachTarget);
					_lst.Add (_target);	
				}
			}   
			if (each.skillMsg .operationType == BattleReportService.INTERVENE) {
				//这里开始是援护
				_trigger = BattleManager.Instance.getBattleCharacterData (each.skillMsg .trigger); 
				//找到对应要修改的攻击技能 
				newSkill.init (each.skillMsg, _lst, _user, _trigger, lastSkill);		 
			} else if (each.skillMsg .operationType == BattleReportService.REBOUND) {
				//	反击 
				newSkill.init (each.skillMsg, _lst, _user, _user, lastSkill); 
			} else if (each.skillMsg .operationType == BattleReportService.DOUBLE_ATTACK) {
				//	连击
				each.skillMsg.skillID = 0;
                if (each.skillMsg.skillSID == 25118 || each.skillMsg.skillSID == 26118 || each.skillMsg.skillSID == 22118 || each.skillMsg.skillSID == 23118 || each.skillMsg.skillSID == 24118)
                {
                    each.skillMsg.skillSID = SkillSampleManager.SID_COMBO_ATTACKJ;
                }
                else
				each.skillMsg.skillSID = SkillSampleManager.SID_COMBO_ATTACK;
				newSkill.init (each.skillMsg, _lst, _user, _user, null);	
				//每个连击服务器返回统一id，所以前端用instanceID区分;
				newSkill.serverData.id = newSkill.GetHashCode (); 
			} else if (each.skillMsg .operationType == BattleReportService.PARTICIPANT) {
				//	参与合击
				each.skillMsg.skillSID = lastSkill.serverData.sample.sid;
				newSkill.init (each.skillMsg, _lst, lastSkill.user, lastSkill.user, lastSkill);		
				GroupCombineSkill = newSkill; 
			} else if (each.skillMsg .operationType == BattleReportService.TOGERTHER_ATTACK) { 
				//	参与合击,合击技能自创
				each.skillMsg.skillSID = SkillSampleManager.SID_GROUP_ATTACK;
				newSkill.init (each.skillMsg, _lst, _user, GroupCombineSkill.user, null);		 
			} else if (each.skillMsg .operationType == BattleReportService.BUFFER_CHECK) { 
				//特殊:这个是回合开始前的buff检查skill,用于已有buff的移除，或者激活
				each.skillMsg.skillSID = SkillSampleManager.SID_CHECK_BUFF;
				newSkill.init (each.skillMsg);		 
			} else if (each.skillMsg .operationType == BattleReportService.FIGHTER_INFO) { 
				//特殊:替补上阵
				//sid = SID_ADD_CARD;	
				newSkill.init (each.skillMsg, null, _user);	 
			} else if (each.skillMsg.operationType == BattleReportService.ADD_PLOT_NPC) {
				newSkill.init (each.skillMsg, _lst, _user);
			} else if (each.skillMsg.operationType == BattleReportService.DEL_PLOT_NPC) {
				newSkill.init (each.skillMsg, _lst, _user);
			} else {  
				newSkill.init (each.skillMsg, _lst, _user); 
			} 
			addSkill (newSkill); 
			//记录上次技能，便于下次技能是援护之类的修改目标
			//	if (each.skillMsg .operationType != BattleReportService.ATTR_CHANGE) {
			lastSkill = newSkill; 
		} 
		if (finalInfo)
			lastSkill.isFinalSkill = true;
	}

	public void addSkill (SkillCtrl _skill)
	{ 
		SkillList.Add (_skill);
//		MonoBase.print ("skill added,length:" + SkillList.Count);
	}
	
	public void addBuff (BuffCtrl _buff)
	{ 
		BuffList .Add (_buff);
		//	MonoBase.print ("buff add,length:" + BuffList.Count);
	}

	public void replaceBuff (BuffCtrl _buff)
	{

		
	}

	public void removeSkill (SkillCtrl _skill)
	{ 
		if (_skill != null) { 
			SkillList.Remove (_skill);
			//Debug.LogWarning ("skill Remove:   " + _skill.serverData.sample.getName () + "    " + _skill.serverData.id + "legth:" + SkillList.Count); 
		}
	}

	public void removeBuff (BuffCtrl _buff)
	{ 
		if (_buff != null) {
			BuffList .Remove (_buff); 
		}
	}

	void turnBeginTalkCallBack ()
	{
		//BattleManager.Instance.talkSid=0;
		//如果技能是胜利，那就胜利..
		int sType = SkillList [0].serverData.sample.getType ();
		if (sType == SkillType.GameWin) {
			
			
			BattleManager.Instance.battleOver ();
			return;
		}
		
		//如果技能是17 buff检查
		else if (SkillList [0].serverData.sample.getType () == SkillType.BuffCheck) { 
			
			changeBuff (SkillList [0]); 
			BattleManager.Instance.nextBattle ();
			return; 
		} 

		//添加替补
		else if (SkillList [0].serverData.sample.getType () == SkillType.AddCard) {
			//换人
			if (changeParter ()) {
				//播放对话
				BattleManager.Instance.StartCoroutine (BattleManager.Instance.showTalk (2, true, continueBattleAfterTalk));
				return;	
			}
			
			BattleManager.Instance.battleInfoWaitTime += 1f;
			continueBattleAfterTalk ();
			
			return;
			
		} 
		
		//NPC上场标志
		else if (SkillList [0].serverData.sample.getType () == SkillType.AddNPC) {
			//隐藏在长场人员
			BattleManager.Instance.talkSid = BattleManager.Instance.getNPCPlotSid (BattleManager.Instance.getClipFrame (), true);
			BattleManager.Instance.playerTeam.allParterOutBattleField ();
			BattleManager.Instance.npcShowTime = true;
			BattleManager.Instance.nextBattle ();
			return;
		} 	
		
		//NPC下场标志
		else if (SkillList [0].serverData.sample.getType () == SkillType.DelNPC) {
			
			BattleManager.Instance.talkSid = BattleManager.Instance.getNPCPlotSid (BattleManager.Instance.getClipFrame (), false);
			if (BattleManager.Instance.talkSid != 0) {
	
				//	BattleManager.Instance.hasTalk = true;
				BattleManager.Instance.StartCoroutine (BattleManager.Instance.showTalk (2, false, continueNpcGoDownAfterTalk));
				return;
			}
			continueNpcGoDownAfterTalk ();
			return;
		} else if (SkillList [0].serverData.sample.getType () == SkillType.DelNPC || SkillList [0].serverData.sample.getType () == SkillType.AddNPC) {
			BattleManager.Instance.nextBattle ();
			return;
		}
		
		//演播战斗用 回合中途出现
		else if (SkillList [0].serverData.sample.getType () == SkillType.Talk) {
			//	Debug.LogError ("------------->>>Talk 03");
			BattleManager.Instance.talkSid = SkillList [0].skillmsg.plotSID;
			BattleManager.Instance.StartCoroutine (BattleManager.Instance.showTalk (2, true, continueBattleAfterTalk));
			return;
		} else if (SkillList [0].serverData.sample.getType () == SkillType.EffectExit) {
			BattleManager.Instance.effectExit = true;
			BattleManager.Instance.effectSid = SkillList [0].skillmsg.exitEffectId;
			UiManager.Instance.battleWindow .guideEffectExit ();
			return;
		}
		
		SkillList [0].user.characterCtrl.beginAction (enum_character_Action.UseSkill, SkillList [0]);


	}


	//开始播放第一个技能
	public void Play ()
	{
		if (SkillList.Count == 0) { 
			return; 
		}

		if (BattleManager.Instance.activeInfoIndex == 0 && BattleManager.Instance.talkSid != 0) {
			//如果是大回合里面的第一个人

			BattleManager.Instance.StartCoroutine (BattleManager.Instance.showTalk (1, false, () => {
				BattleManager.Instance.talkSid = 0;
				BattleManager.Instance.activeInfoIndex = 0;
				turnBeginTalkCallBack ();
			}));

			return;
		} else {
			turnBeginTalkCallBack ();

		}



	}
	
	void continueBattleAfterTalk ()
	{
		BattleManager.Instance.talkSid = 0;
		BattleManager.Instance.nextBattle (); 
	}
	
	void continueNpcGoDownAfterTalk ()
	{
		//	UiManager.Instance.switchWindow<BattleWindow> ();
		//	BattleManager.Instance.hasTalk = false;
		BattleManager.Instance.talkSid = 0;
		//显示正常队员
		BattleManager.Instance.playerTeam.allParterInToBattleField ();
		BattleManager.Instance.npcShowTime = false;
		npcGoDown ();
		//BattleManager.Instance.changeBackGroundColor (Color.white);
		BattleManager.Instance.battleInfoWaitTime += 1f;
		BattleManager.Instance.nextBattle ();
		//BattleManager.Instance.talkIndex += 1; 
	}

	public void npcGoDown ()
	{
		foreach (CharacterData each in BattleManager.Instance.getActionCharacters()) {
			
			if (each.isNPC && each.characterCtrl != null) {
				
				if (each.isInBattleField == true) {
					each.characterCtrl.outBattleField ();
				}
			}
			
		}
	}

	public void npcComeIn ()
	{
		foreach (SkillCtrl each in  SkillList) {
			if (each.serverData.sample.getType () == SkillType.AddCard) {

				if (each.skillmsg.card != null) {
					BattleTeamManager tmpTeam = null;
					if (each.skillmsg.card.camp == TeamInfo.ENEMY_CAMP) {
						tmpTeam = BattleManager.Instance.enemyTeam;
					} else {
						tmpTeam = BattleManager.Instance.playerTeam;
					}
						
					tmpTeam.CreateCharacterInstance (each.serverData.changeRole);
					
	
				}
			}
		}
	}
	//返回true暂停播放剧情
	public bool changeParter ()
	{
		bool hasNPC = false;
		foreach (SkillCtrl each in  SkillList) {
			if (each.serverData.sample.getType () == SkillType.AddCard) {

				if (each.skillmsg.card != null) {
					
					
					BattleTeamManager tmpTeam = null;
					if (each.skillmsg.card.camp == TeamInfo.ENEMY_CAMP) {
						tmpTeam = BattleManager.Instance.enemyTeam;
					} else {
						tmpTeam = BattleManager.Instance.playerTeam;
					}
					//npc登场
					if (each.skillmsg.card .master == BattleReportService.NPC) {
						tmpTeam.CreateCharacterInstance (each.serverData.changeRole);
						hasNPC = true;
						continue;
					}
					//下场处理
					if (each.serverData.changeRole.role.isBoss ()) {
						//boss全下场
						each.serverData.changeRole.parentTeam.hideAllParter ();
						//	eachRole.characterCtrl.outBattleField();
					} else {
						//不是boss下场指定位置的战斗者，没有就不管
						for (int i=0; i<tmpTeam.players.Count; i++) {
							CharacterData eachRole = tmpTeam.players [i];
							//移走原来点位的人
							if (eachRole.TeamPosIndex == each.skillmsg.card.embattle) {
								eachRole.characterCtrl.outBattleField ();
								tmpTeam.players.RemoveAt (i);
								break;
							}
						}
					}
					//替补上阵
					tmpTeam.players.Add (each.serverData.changeRole);
					tmpTeam.CreateCharacterInstance (each.serverData.changeRole);
				}
			}
			
			
			
		}
		return hasNPC;
	}
	
	public void changeBuff (SkillCtrl skill)
	{
 
		foreach (BuffAttrChange each in  skill.changes) { 

			bool foundBuff = false;
			//在已有buff中找到对应buff
			foreach (CharacterData  man in  BattleManager.Instance.getAllBattleCharacterData()) { 	
				foreach (BuffCtrl eachBuff in  man.characterCtrl.GetBuffList()) { 
					if (each.skillID == eachBuff.serverData.id) {
//						MonoBase.print (man.role.getName () + ":" + eachBuff.serverData.sample.getName () + " " + eachBuff.serverData.id+",operationType="+each.operationType);
						foundBuff = true;
						//如果buff操作类型是移除
						if (each.operationType == BattleReportService.BUFFER_REMOVE) {
							man.characterCtrl.RemoveBuff (eachBuff);
						} else if (each.operationType == BattleReportService.BUFFER_ABILITY) {
							eachBuff.option = buff_option.active;
							eachBuff.serverData.serverDamage = each.changes [0].damage;
							eachBuff.activeOtherBuff ();
						}
						break;
					}
				}
				if (foundBuff) {
					man.characterCtrl.BeHit ();
					break;
				}
			}
		}
	}
 
	//触发援护
	public SkillCtrl HelpOther (CharacterData _trigger)
	{
		foreach (SkillCtrl each in SkillList) {
			if (each.serverData.sample.getType () != SkillType.HelpOther)
				continue;
			if (each.trigger == _trigger && each.triggerSkill.serverData.id == _trigger.characterCtrl.activeAction.Skill.serverData.id) {
				each.user.characterCtrl.beginAction (enum_character_Action.HelpOther, each);
				_trigger.characterCtrl.helpOtherReturn = each.user.characterCtrl.helpBack;
				//如果攻击者在连击，那他就等到.援护后可能会反击
				_trigger.characterCtrl.waitForFightBack = true;

				return each;
			}
		} 

		return null;
	}
	
	//触发医疗
	public SkillCtrl Medical (CharacterData _trigger)
	{
		foreach (SkillCtrl each in SkillList) {
			if (each.serverData.sample.getType () != SkillType.MedicalAfterHelp)
				continue;
			if (each.trigger == _trigger && each.triggerSkill.serverData.id == _trigger.characterCtrl.activeAction.Skill.serverData.id) {
				each.user.characterCtrl.beginAction (enum_character_Action.Medical, each);
				//如果攻击者在连击，那他就等到
				_trigger.characterCtrl.waitForFightBack = true;
				return each;
			}
		} 
		return null;
	}
  
	//触发反击
	public SkillCtrl FightBack (CharacterData _trigger, int activeType)
	{
		foreach (SkillCtrl each in SkillList) {

			if (each.serverData.sample.getType () != SkillType.FightBack && each.serverData.sample.getType () != SkillType.FightBackAfterHelp)
				continue;
			if (each.serverData.sample.getActiveType () != activeType)
				continue;			
		
			if (activeType == SkillActiveType.HelpOtherOver && each.trigger == _trigger && each.triggerSkill.serverData.id != 0 && each.triggerSkill.serverData.id == _trigger.characterCtrl.activeAction.Skill.serverData.id) {
				BattleManager.Instance.StartCoroutine (delayFightBack (each, 0.3f));
				_trigger.characterCtrl.waitForFightBack = true;
				return each;
			}
			
			if (activeType == SkillActiveType.SkillHit && each.targets [0] == _trigger && each.triggerSkill.serverData.id != 0 && each.triggerSkill.serverData.id == _trigger.characterCtrl.activeAction.Skill.serverData.id) {
				BattleManager.Instance.StartCoroutine (delayFightBack (each, 0.6f));
				_trigger.characterCtrl.waitForFightBack = true;
				return each;
			}		
			
		} 
		return null; 
	}
	
	IEnumerator delayFightBack (SkillCtrl each, float time)
	{
		yield return new WaitForSeconds (time);
		each.user.characterCtrl.beginAction (enum_character_Action.FightBack, each);
	}

	//触发连击
	public void comboAttack (CharacterData _trigger)
	{
		if (_trigger.characterCtrl.waitForFightBack == false) {
			bool hasCombo = false;
			foreach (SkillCtrl each in SkillList) {
//				MonoBase.print ("check comboskill:" + each.serverData.id);
				if (each.serverData.sample.getType () != SkillType.ComboAttack)
					continue; 		
				if (_trigger == each.trigger) {
		
					each.user.characterCtrl.setStateImmediate (enum_character_state.standing);
					each.user.characterCtrl.beginAction (enum_character_Action.ComboAttack, each);
					hasCombo = true;
					break;
				}
			} 

			//没有连击了,连击等待取消
			if (hasCombo == false) {
				_trigger.characterCtrl.waitForFightBack = false;
			}

		} 
	}
	
	//触发合击
	public void GroupCombineTargets (CharacterData _trigger)
	{
		int index = 0;
		CharacterData _last = null;
		GroupAttackerIndex = 0;
		
		if (GroupAttackList == null)
			GroupAttackList = new List<SkillCtrl> ();
		else
			GroupAttackList.Clear ();
		
		//从片段中取出合击相关的条目
		foreach (SkillCtrl each in  SkillList) {
			if (each.serverData.sample.getType () == SkillType.GroupAttack && each.trigger == _trigger) {
				GroupAttackList.Add (each);
				_last = each.user;
			}
		}
		
		//遍历条目使卡片移动到合击位置;
		foreach (SkillCtrl each in GroupAttackList) { 
			if (_trigger.parentTeam.isGamePlayerTeam) {
				each.position = BattleManager.Instance.playerTeam.GroupPoint [index].position;
				each.user.characterCtrl.beginAction (enum_character_Action.GroupAttack, each);
					
			} else {
				each.position = BattleManager.Instance.enemyTeam.GroupPoint [index].position;
				each.user.characterCtrl.beginAction (enum_character_Action.GroupAttack, each);
			}
			index += 1;
			//只有3个位置，如果出击人数超了找周杰 =.=
		} 
		if (_last != null)
			_last.characterCtrl._isLastGroupAttacker = true; 
	}
	 
	//合击循环
	public void NextGroupAttack ()
	{ 
		BattleManager.Instance.StartCoroutine (delayGroupAttack ()); 
	}
	
	IEnumerator delayGroupAttack ()
	{
		if (GroupAttackerIndex >= GroupCombineSkill.targets.Count)
			yield break;
		float _time = 0.2f;
		if (GroupAttackerIndex == 0)
			_time = 0.1f;
		//不等待的话一回来又马上出去很累你懂？
		yield return new  WaitForSeconds (_time);
		//这里重定位skill的position，是因为合击要两个位置，第一次是准备合击的位置，第二次是目标点位，所以很尴尬
		GroupAttackList [GroupAttackerIndex].user.characterCtrl._isLastGroupAttacker = false;
		GroupAttackList [GroupAttackerIndex].position = BattleManager.Instance.getOffset (GroupAttackList [GroupAttackerIndex].targets [0].characterCtrl.transform.position, !GroupAttackList [GroupAttackerIndex].user.parentTeam.isGamePlayerTeam);
		GroupAttackList [GroupAttackerIndex].user.characterCtrl.cardCtrl.goToAttack ();
		GroupAttackerIndex += 1;
	} 
} 