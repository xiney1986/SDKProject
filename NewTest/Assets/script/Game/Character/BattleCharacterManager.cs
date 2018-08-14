using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/** 
  * 角色管理器 
  * @author 李程
  * */ 
public class BattleCharacterManager
{ 
	List<CharacterData> CharacterDataList;

	public static  BattleCharacterManager Instance {
		get{ return SingleManager.Instance.getObj ("BattleCharacterManager") as BattleCharacterManager;}
	}


	
	//创建战斗用的角色数据实例，列表管理在battleManager
	public CharacterData CreateBattleCharacterData (TeamInfoPlayer playerData, BattleTeamManager parentTeam, int battleType)
	{

		CharacterData _data = new CharacterData ();
		_data.serverID = playerData.id;
		_data.storeID = playerData.uid;
		_data.TeamPosIndex = playerData.embattle;//临时用
		_data.server_hp = playerData.hp;
		_data.fixedServer_hp = playerData.hp;
		_data.server_max_hp = playerData.maxHp;
		_data.parentTeam = parentTeam;
		_data.orgScale = Vector3.one;
		_data.camp = playerData.camp; 
		_data.EvoLevel=playerData.evoLevel;
		//是否是npc
		if (playerData.master == BattleReportService.NPC)
			_data.isNPC = true;
		else
			_data.isNPC = false;
 
		if (playerData.camp == TeamInfo.OWN_CAMP) {
			if (playerData.isGuardianForce) {
//				if (BattleManager.battleData.isArenaFinal || 
//					BattleManager.battleData.isArenaMass || 
//					BattleManager.battleData.isGuide ||
//				    BattleManager.battleData.isLadders ||
//					BattleManager.battleData.isGuildBoss||
//				    BattleManager.battleData.isPvP||
//                    BattleManager.battleData.isMineralFightRecord) //公会boss在选择队伍的时候保存了一次活动队伍，看需求可能不能保存
					_data.role = CardManagerment.Instance.createCard (playerData.sid);
//				else
//					_data.role = StorageManagerment.Instance.getBeast (ArmyManager.Instance.getActiveArmy ().beastid);

			} else { 
				
				if (_data.isNPC) {
					//npc
					_data.orgScale = BattleManager.SACLEOFNPC;
					_data.role = CardManagerment.Instance.createCard ("", playerData.sid, 0, 0); 
					
					
				} else {
					//人
					if (battleType == BattleType.BATTLE_TEN) {
					
						//十人战大小不同
						_data.orgScale = BattleManager.SACLEOF10V10;

                        if (BattleManager.battleData.isArenaFinal || BattleManager.battleData.isGuide || BattleManager.battleData.isLadders || BattleManager.battleData.isGodsWarGroupFight) {
							//_data.role = CardManagerment.Instance.createCard ("", playerData.sid, 0, 0);
							_data.role = CardManagerment.Instance.createCard (playerData.sid, playerData.surLevel);
						} else {
							//bugfix 竞技场固定用3
							int armyID;
                            if (BattleManager.battleData.isArenaMass || BattleManager.battleData.isArenaFinal || BattleManager.battleData.isLadders || BattleManager.battleData.isGodsWarGroupFight) {
								armyID = ArmyManager.PVP_TEAMID;
							}
							else if (BattleManager.battleData.isGuildFight) {
								armyID = ArmyManager.PVP_GUILD;
                            }
                            else {
								armyID = ArmyManager.Instance.getActiveArmy ().armyid;
							}
							//十人战本方卡片获取方式不一样  站位 主力固定1 2 3 4 5，替补固定11 12 13 14 15
							if (_data.TeamPosIndex < 6) {
								string id;
								if(BattleManager.battleData.isOneOnOneBossFight)
								{
									id = playerData.uid;
								}
								else
								{
									id = ArmyManager.Instance.getArmy (armyID).players [_data.TeamPosIndex - 1];
								}
								_data.role = StorageManagerment.Instance.getRole (id);
							} else {
								string id;
								if(BattleManager.battleData.isOneOnOneBossFight)
								{
									id = playerData.uid;
								}
								else
								{
									id = ArmyManager.Instance.getArmy (armyID).alternate [_data.TeamPosIndex - 11];
								}
								_data.role = StorageManagerment.Instance.getRole (id);
							}
						}
					} else {
						if (BattleManager.battleData.playerFormationID == 1) {
							_data.orgScale = BattleManager.SACLEOF3V3;
						} else if (BattleManager.battleData.playerFormationID == 2) {
							//todo 4人阵放大不
                        } 
                        if (BattleManager.battleData.isArenaFinal || BattleManager.battleData.isGuide || BattleManager.battleData.isLadders || BattleManager.battleData.isMineralFightRecord || BattleManager.battleData.isGodsWarFinal || BattleManager.battleData.isGodsWarGroupFight) {
							//_data.role = CardManagerment.Instance.createCard ("", playerData.sid, 0, 0);
							_data.role = CardManagerment.Instance.createCard (playerData.sid, playerData.surLevel);
						} else if (BattleManager.battleData.isOneOnOneBossFight) {//恶魔挑战（只有一个人）很危险的操作
                            _data.role = StorageManagerment.Instance.getRole(playerData.uid);
                            //return _data;
                        } else {
							_data.role = StorageManagerment.Instance.getRole (_data.storeID); 
						}
				
					}
				}
			}
		} else {
			//敌人
			
			//todo pvp单独处理,下面是pve的情况
			
			if (playerData.isGuardianForce) { 
				_data.role = CardManagerment.Instance.createCard (playerData.sid); 
			} else { 
				
				if (BattleManager.battleData.battleType == BattleType.BATTLE_TEN) {
					//十人战大小不同
					_data.orgScale = BattleManager.SACLEOF10V10;
				}
				
				//_data.role = CardManagerment.Instance.createCard ("", playerData.sid, 0, 0);
				_data.role = CardManagerment.Instance.createCard (playerData.sid, playerData.surLevel);
			}
		}
		
		if (playerData.isGuardianForce) {
			
			if (playerData.camp == TeamInfo.OWN_CAMP)
				_data.orgPosition = new Vector3 (0, 0, -3);
			else
				_data.orgPosition = new Vector3 (0, 0, 3);		
	
		} else {
			if (_data.isNPC) {
				_data.orgPosition = BattleManager.Instance.npcPoint.FindChild (_data.TeamPosIndex.ToString ()).position;
				
			} else if (_data.isBoss) {
				_data.orgPosition = parentTeam.TeamHitPoint.position;

			} else {
				//如果是人
				_data.orgPosition = parentTeam.formation.getPosition (_data.TeamPosIndex); 
			}
			if (_data.role != null) {
				_data.isBoss = _data.role.isBoss ();
			}
		}

		if (_data.role == null) {
			MonoBase.print ("role is null  sid=" + playerData.sid);
			return null;
		}
		 
		return _data; 
	}
	
	public void  cleanCharacterData ()
	{
		CharacterDataList.Clear ();
	}
  
	public void AddCharacterData (CharacterData _data)
	{
		if (CharacterDataList == null)
			CharacterDataList = new List<CharacterData> ();
		CharacterDataList.Add (_data);
	}
	 
	public void RemoveCharacterData (CharacterData _data)
	{
		if (CharacterDataList == null)
			return;
		
		CharacterDataList.Remove (_data); 
	}

	public int CharacterDataLength ()
	{ 
		return CharacterDataList.Count;
	}
	 
}
