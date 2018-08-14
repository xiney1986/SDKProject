using System;
using UnityEngine;
using System.Collections.Generic;
 
/**
 * 战报服务
 * @author longlingquan
 * */
public class BattleReportService:BaseFPort
{
	public const string WINNER = "1";//获胜者
	public const string ABILITY_ATTACK = "2";//攻击行为 xxx 使用技能 对 xxx
	public const string BUFFER_CHECK = "3";//buffer检查
	public const string FIGHTER_INFO = "4";//战斗角色初始化信息
	public const string ATTR_CHANGE = "5";//改变属性 xxx 的 aaa属性 改变 bbb
	public const string BUFFER_ADD = "6";//添加buffer
	public const string BUFFER_REPLACE = "7";//替换buffer
	public const string INTERVENE = "8";//援护 xxx 使用 aaa援护技能 对 xxx2  本次攻击者 xxx3
	public const string REBOUND = "9";//反击 xxx 使用 aaa 对 xxx2
	public const string FIRSTAID = "10";//急救 xxx 使用 aaa 对 xxx2
	public const string DOUBLE_ATTACK = "11";//连击
	public const string TOGERTHER_ATTACK = "12";//合击
	public const string PARTICIPANT = "13";//参与者 合击参与者
	public const string BUFFER_REMOVE = "14";//移除buffer
	public const string BUFFER_ABILITY = "15";//buffer生效
	public const string ADD_PLOT_NPC = "16";//剧情NPC登场
	public const string DEL_PLOT_NPC = "17";//剧情NPC退场
	public const string PLOT_TALK = "18";//对话
	public const string EFFECT_EXIT = "19";//播放特效退出战斗
	
	public const string NPC = "plot_npc";
	public const string HP = "1";//属性 hp 
	public const string ANGER = "2";//怒气 anger
	
	
	private const string BATTLE_TEN = "all_fighter";//十人战
	private const string BATTLE_FIVE = "lead_fighter";//五人战
	private const string BATTLE_SUBSTITUTE = "bloody_battle";//替补战
	 
	
	public BattleReportService ()
	{
		
	}
	
	public override void read (ErlKVMessage message)
	{
        ErlList ls;
		ErlType type = message.getValue ("type") as ErlType;
       // ErlList ls = message.getValue("report") as ErlList;
	    ErlType Rportnum = message.getValue("report") as ErlType;
		ErlType emeyType = message.getValue ("enemy_array") as ErlType;
		ErlType mineType = message.getValue ("mine_array") as ErlType;
		ErlType pvpType = message.getValue ("bonus_attack") as ErlType;
		ErlType emeyResonance = message.getValue ("enemy_resonance") as ErlType;
		ErlType attResonance = message.getValue ("attacker_resonance") as ErlType;
		ErlType replayAttackerName = message.getValue ("attacker_name") as ErlType;
		ErlType replayEnemyName = message.getValue ("enemy_name") as ErlType;
		ErlType cardEvo = message.getValue ("card_evo") as ErlType; //卡片进化等级
		ErlType emeyNum = message.getValue ("fore") as ErlType;//pve敌人阵形人数
	    GameManager.Instance.isCanBeSecondSkill = false;
	    
	    if (Rportnum is ErlList)
	    {
	        ls = Rportnum as ErlList;
	    }
	    else
	    {
            int reportNum = StringKit.toInt(Rportnum.getValueString());
			//ls=message.getValue("1") as ErlList;
			List<ErlType> ttt=new List<ErlType>();
            for (int i = 0; i < reportNum; i++) {
                string kk = (i + 1).ToString();
				ErlList te=message.getValue(kk) as ErlList;
				for(int j=0;j<te.Value.Length;j++){
					ttt.Add(te.Value[j]);
				}
            }
			ErlType[] kks=new ErlType[ttt.Count];
			for(int i=0;i<ttt.Count;i++){
				kks[i]=ttt[i];
			}
			ls=new ErlList(kks);
	    }
	    
	    if (message.getValue("seckill") != null)
	    {
	        if (MissionInfoManager.Instance.isBossFight)
	        {
	            MissionInfoManager.Instance.isBossFight = false;
	        }
	        else
	        {
                GameManager.Instance.isCanBeSecondSkill = (message.getValue("seckill") as ErlType).getValueString() == "1";
	        }
           
	    }
		ErlType damageValue = message.getValue ("damage") as ErlType;//单挑boss伤害值//
		if(damageValue != null)
		{
			AttackBossOneOnOneManager.Instance.damageValue = StringKit.toLong (damageValue.getValueString ());
		}

			
		BattleDataErlang battleData = new BattleDataErlang ();
		if (emeyNum != null)
			battleData.pveTeamNum = StringKit.toInt (emeyNum.getValueString ());
		if (emeyType != null)
			battleData.enemyFormationID = StringKit.toInt (emeyType.getValueString ());
		if (mineType != null)
			battleData.playerFormationID = StringKit.toInt (mineType.getValueString ());
		if (pvpType != null)
			battleData.pvpType = StringKit.toInt (pvpType.getValueString ());

		if (emeyResonance != null) {
			ErlArray eaER = emeyResonance as ErlArray;
			//上方召唤兽共鸣
			battleData.enemyBeastEffect = BeastEvolveManagerment.Instance.getBestResonanceByNums (StringKit.toInt (eaER.Value [1].getValueString ()), StringKit.toInt (eaER.Value [0].getValueString ()));

		}
		if (attResonance != null) {
			ErlArray eaER = attResonance as ErlArray;
			//下方召唤兽共鸣
			battleData.playerBeastEffect = BeastEvolveManagerment.Instance.getBestResonanceByNums (StringKit.toInt (eaER.Value [1].getValueString ()), StringKit.toInt (eaER.Value [0].getValueString ()));
		} else {
			battleData.playerBeastEffect = BeastEvolveManagerment.Instance.getBestResonance ();
		}
		if (replayAttackerName != null) {
			battleData.replayAttackerName = replayAttackerName.getValueString ();
		}
		if (replayEnemyName != null) {
			battleData.replayEnemyName = replayEnemyName.getValueString ();
		}
		if (cardEvo != null) {//卡片进化等级
			battleData.evo = new Dictionary<string,int> ();
			ErlArray eaCEs = cardEvo as ErlArray;
			for (int i=eaCEs.Value.Length-1; i>=0; i--) {
				ErlArray eaCE = eaCEs.Value [i] as ErlArray;
				battleData.evo.Add (eaCE.Value [0].getValueString (), StringKit.toInt (eaCE.Value [1].getValueString ()));
			}
		}
		battleData.hpMap = new Dictionary<int, BattleHpInfo> ();
		parseBattleType (battleData, type);
		BattleManager.battleData = battleData; 
		//倒转顺序 战报需要倒转 每个erllist都需要倒转
		Array.Reverse (ls.Value);
		//获取队伍信息
		createTeamInfo (battleData, ls.Value [0] as ErlList);
		//获取开场buff
		createOpenBuff (battleData, ls.Value [1]);
		//解析回合战斗
		ErlList el;
		int frame = 0;
		for (int i = 2; i<ls.Value.Length-1; i+=3) {
			frame = (i - 2) / 3 + 1;//(i-2)/3+1 回合的算法
			//如果执行到最后一个 则不进行序列化 最后一个是战斗胜利
			if (i == ls.Value.Length - 1)
				break;
			
			//剧情npc
			if (!(ls.Value [i] is ErlNullList)) { 
				el = ls.Value [i] as ErlList;  
				//erllist 需要倒转顺序
				Array.Reverse (el.Value);
				createFight (battleData, el, frame); 
			}
			if (i + 1 >= ls.Value.Length - 1)
				break;
			//回合buffer检查
			if (!(ls.Value [i + 1] is ErlNullList)) { 
				el = ls.Value [i + 1] as ErlList;  
				//erllist 需要倒转顺序
				Array.Reverse (el.Value);
				createRoundBuffer (battleData, el, frame);
			}
			if (i + 2 >= ls.Value.Length - 1)
				break;
			//有可能当前回合，双方都没有出手
			if (ls.Value [i + 2] is ErlNullList) {
				createFight (battleData, ls.Value [i + 2] as ErlNullList, frame);
			} else {
				//回合战斗
				el = ls.Value [i + 2] as ErlList;
				//erllist 需要倒转顺序
				Array.Reverse (el.Value);
				createFight (battleData, el, frame);
			}
		}
		setLastAttack (battleData);//设置最后一击，攻击伤害行为完成后调用
		//获取胜利失败
		createWinner (battleData, ls.Value [ls.Value.Length - 1] as ErlList, frame + 1);

        ErlType award = message.getValue("award") as ErlType;//获取奖励
        
        //这里处理副本战斗
	    if (MissionInfoManager.Instance.mission!=null&&
            MissionInfoManager.Instance.mission.getChapterType()==ChapterType.STORY&&
            GameManager.Instance.isCanBeSecondSkill&&
             PlayerPrefs.GetInt(UserManager.Instance.self.uid + "miaosha", 1) == 1&&
            battleData.isPve&&
             UserManager.Instance.self.getUserLevel() >= 15)
	    {
            AwardService service12 = ServiceManager.Instance.getServiceByCmd(FPortService.AWARD) as AwardService;
            if (award is ErlArray)
                service12.parseMAward(award as ErlArray);
            else if (award is ErlList)
                service12.parseMAward(award as ErlList);
            GameManager.Instance.battleReportCallback();
            GameManager.Instance.battleReportCallback = null;
	    }else if (GameManager.Instance.isCanBeSecondSkill&&
            PlayerPrefs.GetInt(UserManager.Instance.self.uid+"miaosha",1)==1&&
            battleData.isPvP&&
            UserManager.Instance.self.getUserLevel()>=15)//这里处理副本PK
	    {
            AwardService service12 = ServiceManager.Instance.getServiceByCmd(FPortService.AWARD) as AwardService;
            if (award is ErlArray)
                service12.parseMAward(award as ErlArray);
            else if (award is ErlList)
                service12.parseMAward(award as ErlList);
            GameManager.Instance.battleReportCallback();
            GameManager.Instance.battleReportCallback = null;
	    }
	    else
	    {
            GameManager.Instance.battleReportCallback();
            GameManager.Instance.battleReportCallback = null;
            AwardService service1 = ServiceManager.Instance.getServiceByCmd(FPortService.AWARD) as AwardService;
            if (award is ErlArray)
                service1.parseAward(award as ErlArray);
            else if (award is ErlList)
                service1.parseAward(award as ErlList);
	    }
		
//		(ServiceManager.Instance.getServiceByCmd (FPortService.AWARD) as AwardService).parseAward ((award as ErlArray) != null ? award as ErlArray : award as ErlList);
		//战斗失败处理
		if (message.getValue ("event") != null)
			(ServiceManager.Instance.getServiceByCmd (FPortService.FUBEN_EVENT) as FuBenDoEventService).parseFightLose (message.getValue ("action") as ErlType, message.getValue ("pstep") as ErlType);
	}

	//设置最后一击，攻击伤害行为完成后调用
	private void setLastAttack (BattleDataErlang battleData)
	{
		List<BattleClipErlang> bces = battleData.battleClip;
        if(bces.Count<=0)return;
		List<BattleInfoErlang> bies = bces [bces.Count - 1].battleInfo;
	    if (bies.Count <= 0) return;
		List<BattleSkillErlang> bses = bies [bies.Count - 1].battleSkill;
		for (int i = bses.Count-1; i >= 0; i--) {
			if (bses [i].skillMsg.operationType != ATTR_CHANGE) {
				bses [i].skillMsg.isLastAttack = true;
				return;

			}
		}
	}

	private void parseBattleType (BattleDataErlang data, ErlType type)
	{
		//  完成数据为[pve,all_fighter,nr_monster_hp,nr_self_hp]
		ErlArray et = type as ErlArray;
		string ispvp = et.Value [0].getValueString ();//战斗方式
		if (ispvp == "pvp") { 
			data.isPvP = true;
        } else if (ispvp == "pve")
        {
            data.isPve = true;
        } 
        else if (ispvp == "arena_challenge") {
			data.isArenaMass = true;
		} else if (ispvp == "arena_fight") {
			data.isArenaFinal = true;
		} else if (ispvp == "ladder_fight") {
			data.isLadders = true;
		} else if (ispvp == "ladder_fight_record") {		
			//data.isArenaFinal=true;
			data.isLadders = true;
			data.isLaddersRecord = true;
		} else if (ispvp == "guide") {
			data.isGuide = true;
			//非pvp战不发阵形,在这里补
			data.battleType = StringKit.toInt (et.Value [1].getValueString ());
			data.playerFormationID = StringKit.toInt (et.Value [2].getValueString ());
			//这里的enemyFormationID的值为人数
			BattleManager.lastMissionEvent.battleNum = StringKit.toInt (et.Value [3].getValueString ());
		} else if (ispvp == "guild_boss") {

		} else if (ispvp == "crusade_boss") {//讨伐boss

        } else if (ispvp == "fight_boss") {//恶魔挑战
            data.isOneOnOneBossFight = true;
        }
		else if(ispvp == "armageddon_fight")// 末日决战挑战小怪//
		{
			data.isLastBattle = true;
		}
		else if(ispvp == "armageddon_fight_boss")// 末日决战boss战//
		{
			data.isLastBattleBossBattle = true;
		}
		else if(ispvp == "hero_road")
		{
			data.isHeroRoad = true;
		}
		/** 公会战 */
		else if (ispvp == "center_guild_war") {
			data.isGuildFight = true;
		} else if (ispvp == "mineral_fight") {
			data.isMineralFight = true;
		} else if (ispvp == "mineral_fight_report") {
			data.isMineralFight = true;
			data.isMineralFightRecord = true;
		}
		else if (ispvp == "god_war_fight") {
			data.isGodsWarGroupFight= true;
			data.isGodsWarGroupAward= true;
		}
		else if(ispvp == "god_war_fight2")
		{
			data.isGodsWarFinal = true;
		}
		else if(ispvp == "gm_fight")
		{
			GameManager.Instance.battleReportCallback = GameManager.Instance.intoBattle;
			data.isGMFight = true;
		}

		string str = (type as ErlArray).Value [1].getValueString ();//战斗类型
		if (str == BATTLE_TEN) {
			data.battleType = BattleType.BATTLE_TEN;
		} else if (str == BATTLE_FIVE) {
			data.battleType = BattleType.BATTLE_FIVE;
		} else if (str == BATTLE_SUBSTITUTE) {
			data.battleType = BattleType.BATTLE_SUBSTITUTE;
		} 
	}
	
	//构建战报队伍信息
	private void createTeamInfo (BattleDataErlang battleData, ErlList el)
	{  
		//战斗双方 
		battleData.playerTeamInfo = new TeamInfo (TeamInfo.OWN_CAMP);
		battleData.enemyTeamInfo = new TeamInfo (TeamInfo.ENEMY_CAMP);
//								[{"4",{2,15,30,3000,3000,"npc",15}},
//                                {"4",{2,15,27,3000,3000,"npc",11}},
//                                {"4",{2,12,24,2986,2986,"npc",9}},
//                                {"4",{2,12,21,2986,2986,"npc",7}},
//                                {"4",{2,17,18,3000,3000,"npc",3}},
//                                {"4",{1,10,15,4168,4168,"role",10}},
//                                {"4",{1,9,12,3307,3307,"role",8}},`
//                                {"4",{1,8,9,3188,3188,"role",6}},
//                                {"4",{1,7,6,3188,3188,"role",4}},
//                                {"4",{1,6,3,3098,3098,"role",2}},
//                                {"4",{{1,1},29,1,1,1,"role",0}}]
		TeamInfoPlayer card;
		ErlArray array;
		for (int i = 0; i < el.Value.Length; i++) {
			card = new TeamInfoPlayer (); 
			array = (el.Value [i] as ErlArray).Value [1] as ErlArray;
			if (array.Value [0] is ErlArray) {  	
				//召唤兽
				string str1 = (array.Value [0] as ErlArray).Value [0].getValueString ();//camp 阵营  
				card.camp = StringKit.toInt (str1);
				string str2 = array.Value [1].getValueString ();//sid 模板 
				card.sid = StringKit.toInt (str2);
				string str3 = array.Value [2].getValueString ();//id 唯一标示 
				card.id = StringKit.toInt (str3);
				string strUid = array.Value [3].getValueString ();//uid 卡片唯一id 可能为空
				card.uid = strUid;
				string str4 = array.Value [4].getValueString ();//hp 
				card.hp = StringKit.toInt (str4);
				string str5 = array.Value [5].getValueString ();//maxhp int 
				card.maxHp = StringKit.toInt (str5);
				string str6 = array.Value [6].getValueString ();//master 拥有者名字 
				card.master = str6;
				string str7 = array.Value [7].getValueString ();//embattle 阵位 编号
				card.embattle = StringKit.toInt (str7); 
				card.isGuardianForce = true;
					
				battleData.playerTeamInfo.setGuardianForce (card);	
				battleData.enemyTeamInfo.setGuardianForce (card); 
			} else {	//普通卡片
				string str1 = array.Value [0].getValueString ();//camp 阵营  
				card.camp = StringKit.toInt (str1);
				string str2 = array.Value [1].getValueString ();//sid 模板 
				card.sid = StringKit.toInt (str2);
				string str3 = array.Value [2].getValueString ();//id 唯一标示 
				card.id = StringKit.toInt (str3);
				string strUid = array.Value [3].getValueString ();//uid 卡片唯一id 可能为空
				card.uid = strUid;
				string str4 = array.Value [4].getValueString ();//hp 
				card.hp = StringKit.toInt (str4);
				string str5 = array.Value [5].getValueString ();//maxhp int 
				card.maxHp = StringKit.toInt (str5);
				string str6 = array.Value [6].getValueString ();//master 拥有者名字 
				card.master = str6;
				string str7 = array.Value [7].getValueString ();//embattle 阵位 编号
				card.embattle = StringKit.toInt (str7);
				card.evoLevel = battleData.getCardEvoLevel (card.uid);//卡片进化等级
				if(array.Value.Length > 9)
				{
					string str8 = array.Value [8].getValueString ();// 卡片进化等级//
					card.evoLevel = StringKit.toInt (str8);
					string str9 = array.Value [9].getValueString ();// 卡片突破等级//
					card.surLevel = StringKit.toInt (str9);
				}
				
				//在添加卡片的时候进行判断
				battleData.playerTeamInfo.addTeamInfoPlayer (card);
				battleData.enemyTeamInfo.addTeamInfoPlayer (card);   
			}
		}
	} 
	//构建开场buff
	public void createOpenBuff (BattleDataErlang battleData, ErlType et)
	{
		if (et is ErlList) {
			battleData.isStart = true;
			ErlList el = et as ErlList;
			Array.Reverse (el.Value);
			// 开场技不计入回合
			createFight (battleData, el);
		}
	}



	//构建战斗组
	public void createFight (BattleDataErlang battleData, ErlList el)
	{
		createFight (battleData, el, 0);
	}

	public void createFight (BattleDataErlang battleData, ErlNullList el, int frame)
	{
		BattleClipErlang battleClip = new BattleClipErlang (); 
		battleClip.frame = frame;
		battleData.battleClip.Add (battleClip);
	}

	public void createFight (BattleDataErlang battleData, ErlList el, int frame)
	{
		BattleClipErlang battleClip = new BattleClipErlang (); 
		battleClip.frame = frame;
		battleData.battleClip.Add (battleClip); 
		
		//建立每个战斗条目
		BattleInfoErlang battleInfo; 
		ErlList el2; 
		
		BattleSkillErlang skillerlang;
		BattleSkillMsg msg;
		for (int i = 0; i < el.Value.Length; i++) {
			battleInfo = new BattleInfoErlang (); 
			battleClip.battleInfo.Add (battleInfo);
			el2 = el.Value [i] as ErlList;
			//erllist需要倒转顺序
			Array.Reverse (el2.Value);
			for (int j=0; j<el2.Value.Length; j++) {
				skillerlang = new BattleSkillErlang ();
				battleInfo.battleSkill.Add (skillerlang);
								
				msg = new BattleSkillMsg ();
				skillerlang.skillMsg = msg;
					
				//建立条目中的战斗技能 
				if (el2.Value [j] is ErlArray) {
					ErlArray arr = el2.Value [j] as ErlArray; 
					//单个战斗回合具体信息
					// MonoBehaviour.print ("report[" + i + "][" + j + "][" + k + "] is ErlArray length = " + arr.Value.Length); 
									
					//得到当前战斗回合指令
					string command = (arr.Value [0] as ErlString).Value;
					msg.operationType = command;
					//MonoBehaviour.print ("===command=====" + command);
									
					//攻击行为 xxx 使用技能 对 xxx
					if (command == ABILITY_ATTACK) {  
						ErlArray arr2 = arr.Value [1] as ErlArray;  
						createAbiltyAttack (msg, arr2); 
					} 
					//援护 xxx 使用 aaa援护技能 对 xxx2  本次攻击者 xxx3
					else if (command == INTERVENE) {
						ErlArray arr2 = arr.Value [1] as ErlArray;
						createIntervene (msg, arr2);  
					}
					//改变属性 xxx 的 aaa属性 改变 bbb
					else if (command == ATTR_CHANGE) {
						ErlArray arr2 = arr.Value [1] as ErlArray; 
						createAttrChange (battleData, msg, arr2);
					}
					//反击 xxx 使用 aaa 对 xxx2
					else if (command == REBOUND) { 
						ErlArray arr2 = arr.Value [1] as ErlArray; 
						createRebound (msg, arr2); 
					} 
					//急救 xxx 使用 aaa 对 xxx2
					else if (command == FIRSTAID) { 
						ErlArray arr2 = arr.Value [1] as ErlArray; 
						createFirstaid (msg, arr2); 
					} 
					//参与者 合击参与者
					else if (command == PARTICIPANT) {
						//erlArray 被攻击者 编号可以是多个 
						ErlArray arr2 = arr.Value [1] as ErlArray;
						createParticipant (msg, arr2); 
					} 
					//合击 xxx对xxx合击
					else if (command == TOGERTHER_ATTACK) {
						ErlArray arr2 = arr.Value [1] as ErlArray;
						createTogertherAttack (msg, arr2);
					}
					//连击 xxx使用技能 aaa 对  xxx
					else if (command == DOUBLE_ATTACK) {
						ErlArray arr2 = arr.Value [1] as ErlArray;
						createDoubleAttack (msg, arr2);
					}
					//添加buffer xxx使用技能aaa 对xxx
					else if (command == BUFFER_ADD) {
						ErlArray arr2 = arr.Value [1] as ErlArray;
						createBufferAdd (msg, arr2);
					}
					//移除buffer xxx技能aaa 被移除
					else if (command == BUFFER_REMOVE) { 
						ErlArray arr2 = arr.Value [1] as ErlArray;
						createBufferRemove (msg, arr2);
					}
					//替换buffer xxx的aaa buffer 替换成 bbb buffer
					else if (command == BUFFER_REPLACE) {
						ErlArray arr2 = arr.Value [1] as ErlArray;
						createBufferReplace (msg, arr2);
					}
					//buffer生效 xxx的aaa buffer 生效
					else if (command == BUFFER_ABILITY) {
						ErlArray arr2 = arr.Value [1] as ErlArray;
						createBufferAbility (msg, arr2);
					}
					//剧情NPC登场
					else if (command == ADD_PLOT_NPC) {
						createAddNPC (msg);
					}
					//剧情NPC退场
					else if (command == DEL_PLOT_NPC) {
						createDelNPC (msg);
					}
                    //对话
                    else if (command == PLOT_TALK) {
						createTalk (msg, arr.Value [1]);
					} else if (command == EFFECT_EXIT) {
						createEffectExit (msg, arr.Value [1]);
					} else if (command == FIGHTER_INFO) { 
						ErlArray arr2 = arr.Value [1] as ErlArray;
						addCard (msg, arr2);
					} else {
						//MonoBehaviour.print ("======unused ============ command = " + command);
					} 
				} else {
					//这里如果信息结构不是ErlArray 可能是结构有所改动 
					//MonoBehaviour.print ("error  report[" + i + "][" + j + "][" + k + "] is not ErlArray  type=" + el2.Value [k]);
				}
			}
		}
	}
	//构建攻击行为
	private void createAbiltyAttack (BattleSkillMsg msg, ErlArray array)
	{
		//攻击者 id 
		string str1 = array.Value [0].getValueString ();
		msg.userID = StringKit.toInt (str1);
		// 技能sid编号 
		string str2 = array.Value [1].getValueString ();
		msg.skillSID = StringKit.toInt (str2); 
		// 技能编号 
		string str3 = array.Value [2].getValueString ();
		msg.skillID = StringKit.toInt (str3);
		//erlArray 被攻击者 编号可以是多个
		ErlArray arr4 = array.Value [3] as ErlArray;
		int[] strarr = new int[arr4.Value.Length];
		//生成被攻击者编号数组
		for (int m =0; m<strarr.Length; m++) {
			strarr [m] = StringKit.toInt (arr4.Value [m].getValueString ()); 
		} 
		msg.targets = strarr; 
	}
	//构建援护
	private void createIntervene (BattleSkillMsg msg, ErlArray array)
	{	
		//援护者id
		string str1 = array.Value [0].getValueString (); 
		msg.userID = StringKit.toInt (str1);
		//援护技能sid
		string str2 = array.Value [1].getValueString ();
		msg.skillSID = StringKit.toInt (str2); 
		//援护技能id
		string str3 = array.Value [2].getValueString ();
		msg.skillID = StringKit.toInt (str3);
		//援护对象id
		string str4 = array.Value [3].getValueString ();
		int [] arrStr4 = new int[]{StringKit.toInt (str4)};
		msg.targets = arrStr4;
		//本次攻击者id
		string str5 = array.Value [4].getValueString (); 
		msg.trigger = StringKit.toInt (str5);
	}
	//构建改变属性
	private void createAttrChange (BattleDataErlang battleData, BattleSkillMsg msg, ErlArray array)
	{ 
		//被改变者id
		string str1 = array.Value [0].getValueString ();
		int[] arrInt1 = new int[1]{StringKit.toInt (str1)}; 
		msg.targets = arrInt1; 
											
		//改变的具体属性 数字代替
		string str2 = array.Value [1].getValueString ();
		msg.valueType = StringKit.toInt (str2);
											
		//具体数值
		string str3 = array.Value [2].getValueString ();  
		msg.damage = StringKit.toInt (str3);
		if (msg.valueType == 1) {
			BattleHpInfo info = null;
			if (battleData.hpMap.ContainsKey (msg.targets [0])) {
				info = battleData.hpMap [msg.targets [0]];
			} else {
				info = new BattleHpInfo ();
				battleData.hpMap.Add (msg.targets [0], info);
			}
			info.hp += msg.damage;

		}
	}
	//构建反击
	private void createRebound (BattleSkillMsg msg, ErlArray array)
	{
		//反击者id
		string str1 = array.Value [0].getValueString ();
		msg.userID = StringKit.toInt (str1); 
		//反击技能sid
		string str2 = array.Value [1].getValueString ();
		msg.skillSID = StringKit.toInt (str2);
		//反击技能id
		string str3 = array.Value [2].getValueString ();
		msg.skillID = StringKit.toInt (str3);
		//被反击者id
		string str4 = array.Value [3].getValueString ();
		int[] arrInt4 = new int[1]{StringKit.toInt (str4)};
		msg.targets = arrInt4;
	}
	//构建胜利者
	private void createWinner (BattleDataErlang battleData, ErlList el, int frame)
	{ 
		//[{"1",1}]
		BattleClipErlang battleClip = new BattleClipErlang (); 
		battleClip.frame = frame;
		battleData.battleClip.Add (battleClip);
		battleClip.isWinnerClip = true;
		battleData.winnerID = StringKit.toInt ((el.Value [0] as ErlArray).Value [1].getValueString ());
	}
	//构建急救
	private void createFirstaid (BattleSkillMsg msg, ErlArray array)
	{
		//急救者id
		string str1 = array.Value [0].getValueString ();
		msg.userID = StringKit.toInt (str1);
											
		//急救技能sid
		string str2 = array.Value [1].getValueString ();
		msg.skillSID = StringKit.toInt (str2);
		//急救技能id
		string str3 = array.Value [2].getValueString ();
		msg.skillID = StringKit.toInt (str3);
					
		//被急救者id
		string str4 = array.Value [3].getValueString ();
		int[] arrInt4 = new int[1]{StringKit.toInt (str4)};
		msg.targets = arrInt4;
	}
	//构建合击参与者
	private void createParticipant (BattleSkillMsg msg, ErlArray array)
	{
		int[] strarr = new int[array.Value.Length];  
		//生成被攻击者编号数组
		for (int m =0; m<strarr.Length; m++) {
			strarr [m] = StringKit.toInt (array.Value [m].getValueString ()); 
		}  
		msg.targets = strarr;
	}
	//构建合击 TOGERTHER_ATTACK
	private void createTogertherAttack (BattleSkillMsg msg, ErlArray array)
	{
		//攻击者
		string str1 = array.Value [0].getValueString ();
		msg.userID = StringKit.toInt (str1);
		//被攻击者
		string str2 = array.Value [1].getValueString ();
		int[] arrInt2 = new int[1]{StringKit.toInt (str2)};
		msg.targets = arrInt2;
	}
	//构建连击
	private void createDoubleAttack (BattleSkillMsg msg, ErlArray array)
	{
		//攻击者
		string str1 = array.Value [0].getValueString ();
		msg.userID = StringKit.toInt (str1);
		//技能编号sid
		string str2 = array.Value [1].getValueString ();
		msg.skillSID = StringKit.toInt (str2);
		//技能编号id
		string str3 = array.Value [2].getValueString ();
		msg.skillID = StringKit.toInt (str3);
		//被攻击者
		string str4 = array.Value [3].getValueString (); 
		int[] arrInt4 = new int[1]{StringKit.toInt (str4)};
		msg.targets = arrInt4;
	}
	//构建添加buffer
	private void createBufferAdd (BattleSkillMsg msg, ErlArray array)
	{
		//技能释放者
		string str1 = array.Value [0].getValueString ();
		msg.userID = StringKit.toInt (str1);
		//技能编号sid
		string str2 = array.Value [1].getValueString ();
		msg.skillSID = StringKit.toInt (str2);
		//技能编号id
		string str3 = array.Value [2].getValueString ();

		msg.skillID = StringKit.toInt (str3);
		//buff影响效果
		if (!(array.Value [3] is ErlNullList)) { //控制型buffer无效果
			ErlList list = array.Value [3] as ErlList;
			if (list.Value.Length > 0) {
				BuffEffectType[] effects = new BuffEffectType[list.Value.Length];
				for (int i=0; i<list.Value.Length; i++) {
					ErlArray arr = list.Value [i] as ErlArray;
					string str41 = (arr.Value [0] as ErlAtom).Value;
					int effect = StringKit.toInt (arr.Value [1].getValueString ());
					BuffEffectType eff = new BuffEffectType ();
					eff.type = str41;
					eff.effect = effect;
					effects [i] = eff;
				}
				msg.effects = effects;
			}
		}
		
		//被添加buffer者
		string str5 = array.Value [4].getValueString ();
		int[] arrInt4 = new int[1]{StringKit.toInt (str5)};
		msg.targets = arrInt4;
	}
	//构建移除buffer
	private void createBufferRemove (BattleSkillMsg msg, ErlArray array)
	{
		//被移除buffer者
		string str1 = array.Value [0].getValueString ();
		int[] arrInt1 = new int[1]{StringKit.toInt (str1)};
		msg.targets = arrInt1;
		//被移除技能编号sid
		string str2 = array.Value [1].getValueString (); 
		msg.skillSID = StringKit.toInt (str2);
		//被移除技能编号id
		string str3 = array.Value [2].getValueString (); 
		msg.skillID = StringKit.toInt (str3);
	}
	//替换buffer
	private void createBufferReplace (BattleSkillMsg msg, ErlArray array)
	{
		//被替换buffer者
		string str1 = array.Value [0].getValueString ();
		int[] arrInt1 = new int[1]{StringKit.toInt (str1)};
		msg.targets = arrInt1;
		//旧技能sid
		string str2 = array.Value [1].getValueString (); 
		msg.oldSkillSID = StringKit.toInt (str2);
		//旧技能id
		string str3 = array.Value [2].getValueString (); 
		msg.oldSkillID = StringKit.toInt (str3);
		//替换后的buffer sid
		string str4 = array.Value [3].getValueString (); 
		msg.skillSID = StringKit.toInt (str4); 
		//替换后的buffer id
		string str5 = array.Value [4].getValueString (); 
		msg.skillID = StringKit.toInt (str5);
	}	
	//buffer生效 BUFFER_ABILITY
	private void createBufferAbility (BattleSkillMsg msg, ErlArray array)
	{
		//buffer生效者
		string str1 = array.Value [0].getValueString ();
		msg.userID = StringKit.toInt (str1);
		//buffer技能编号sid
		string str2 = array.Value [1].getValueString ();
		msg.skillID = StringKit.toInt (str2);
		//buffer技能编号id
		string str3 = array.Value [2].getValueString ();
		msg.skillID = StringKit.toInt (str3);
	}
	
	private void createRoundBuffer (BattleDataErlang battleData, ErlList el, int frame)
	{
		// 处理回合开始buffer
//		[{"15",{9,27003,34}},
//                                        {"5",{9,1,-258}},
//                                        {"15",{15,27003,33}},
//                                        {"5",{15,1,-266}},
//                                        {"14",{15,27003,33}}]	
		//建立每一个战斗回合
		BattleClipErlang battleClip = new BattleClipErlang (); 
		battleClip.frame = frame;
		battleData.battleClip.Add (battleClip); 
		//建立每个战斗条目
		BattleInfoErlang battleInfo = new BattleInfoErlang (); 
		battleClip.battleInfo.Add (battleInfo);
				
		BattleSkillErlang skillerlang = new BattleSkillErlang ();
		battleInfo.battleSkill.Add (skillerlang);
								
		BattleSkillMsg msg = new BattleSkillMsg ();
		skillerlang.skillMsg = msg;
		msg.operationType = BUFFER_CHECK;
		
		List<BuffAttrChange> bcs = new List<BuffAttrChange> ();
		BuffAttrChange bc;
		List<BattleAttrChange> acs;
		BattleAttrChange ac;
		ErlArray ea1;
		ErlArray ea2;
		string type; //类型
		int length = el.Value.Length;
		for (int i = 0; i < length; i++) {
			ea1 = el.Value [i] as ErlArray;
			type = (ea1.Value [0] as ErlString).Value;
			if (type == BUFFER_ABILITY) {//buffer生效
				bc = new BuffAttrChange ();
				ea2 = ea1.Value [1] as ErlArray;
				bc.operationType = type;
				bc.skillSID = StringKit.toInt (ea2.Value [1].getValueString ());
//				MonoBase. print("bc.skillID "+bc.skillID );
				bc.skillID = StringKit.toInt (ea2.Value [2].getValueString ());
				acs = new List<BattleAttrChange> ();//装在效果
				do {
					ea1 = el.Value [++i] as ErlArray;
					ea2 = ea1.Value [1] as ErlArray;
					ac = new BattleAttrChange ();
					ac.damageType = StringKit.toInt (ea2.Value [1].getValueString ());
					ac.damage = StringKit.toInt (ea2.Value [2].getValueString ());
					//BattleDataErlang battleDate=BattleManager.battleData;
					BattleHpInfo info = null;
					if (battleData.hpMap.ContainsKey (StringKit.toInt (ea2.Value [0].getValueString ()))) {
						info = battleData.hpMap [StringKit.toInt (ea2.Value [0].getValueString ())];
					} else {
						info = new BattleHpInfo ();
						battleData.hpMap.Add (StringKit.toInt (ea2.Value [0].getValueString ()), info);
					}
					info.hp += ac.damage;
					acs.Add (ac);
				} while(i < length-1 &&
					((el.Value[i+1] as ErlArray).Value[0] as ErlString).Value != BUFFER_ABILITY &&
					((el.Value[i+1] as ErlArray).Value[0] as ErlString).Value != BUFFER_REMOVE);
				bc.changes = acs.ToArray ();
				bcs.Add (bc);
			} else if (type == BUFFER_REMOVE) {//移除buffer
				bc = new BuffAttrChange ();
				ea2 = ea1.Value [1] as ErlArray;
				bc.operationType = type;
				bc.skillSID = StringKit.toInt (ea2.Value [1].getValueString ());
				bc.skillID = StringKit.toInt (ea2.Value [2].getValueString ());
				bcs.Add (bc);
			} else {
			}
		}
		msg.changes = bcs.ToArray ();
	}
	
	private void addCard (BattleSkillMsg msg, ErlArray array)
	{
		TeamInfoPlayer card = new TeamInfoPlayer (); 
	 
		if (array.Value [0] is ErlArray) {  //剧情npc
			string str1 = (array.Value [0] as ErlArray).Value [0].getValueString ();//camp 阵营  
			card.camp = StringKit.toInt (str1);
		} else {
			//替补
			string str1 = array.Value [0].getValueString ();//camp 阵营  
			card.camp = StringKit.toInt (str1);
		}
	 	 
		string str2 = array.Value [1].getValueString ();//sid 模板 
		card.sid = StringKit.toInt (str2);
		string str3 = array.Value [2].getValueString ();//id 唯一标示 
		card.id = StringKit.toInt (str3);
		string strUid = array.Value [3].getValueString ();//uid 卡片唯一id 可能为空
		card.uid = strUid;
		string str4 = array.Value [4].getValueString ();//hp 
		card.hp = StringKit.toInt (str4);
		string str5 = array.Value [5].getValueString ();//maxhp int 
		card.maxHp = StringKit.toInt (str5);
		string str6 = array.Value [6].getValueString ();//master 拥有者名字 
		card.master = str6;
		string str7 = array.Value [7].getValueString ();//embattle 阵位 编号
		card.embattle = StringKit.toInt (str7);
		card.evoLevel = BattleManager.battleData.getCardEvoLevel (card.uid);//卡片进化等级
		
		card.isAlternate = true;
		msg.card = card; 

		if (card.camp == TeamInfo.OWN_CAMP)
			BattleManager.battleData.playerTeamInfo.addTeamInfoSub (card);
		else
			BattleManager.battleData.enemyTeamInfo.addTeamInfoSub (card);

		msg.skillSID = SkillSampleManager.SID_ADD_CARD;

	}
	
	//npc出场
	private void createAddNPC (BattleSkillMsg msg)
	{ 
		msg.skillSID = SkillSampleManager.SID_ADD_NPC;
	}
	
	//npc退场
	private void createDelNPC (BattleSkillMsg msg)
	{
		msg.skillSID = SkillSampleManager.SID_DEL_NPC;
	}

	//对话
	private void createTalk (BattleSkillMsg msg, ErlType type)
	{
		msg.skillSID = SkillSampleManager.SID_TALK;
		msg.plotSID = StringKit.toInt (type.getValueString ());
	}
	//特效退出战斗
	private void createEffectExit (BattleSkillMsg msg, ErlType type)
	{
		msg.skillSID = SkillSampleManager.SID_EFFECT_EXIT;
		msg.exitEffectId = StringKit.toInt (type.getValueString ());
	}

} 

