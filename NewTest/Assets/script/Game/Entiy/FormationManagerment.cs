using UnityEngine;
using System.Collections.Generic;

/**
 * 阵型管理器
 * @author 汤琦
 * */
public class FormationManagerment
{
	//单例
	public static FormationManagerment Instance {
		get{ return SingleManager.Instance.getObj ("FormationManagerment") as FormationManagerment;}
	}
	//获得所有符合玩家的阵型
	public List<int> getPlayerFormation ()
	{
		List<int> list = new List<int> ();
		int[] sids = FormationSampleManager.Instance.getAllFormation ();
		for (int i = 0; i < sids.Length; i++) {
			if (checkFormation (sids [i])) {
				list.Add (sids [i]);
			}
		}
		
		return list;
	}
	//获得升级后的新阵形sid
	public List<int> getPlayerNewFormationAfterLevelup (int oldlevel, int newlevel)
	{
		List<int> list = new List<int> ();
		List<int> list2 = new List<int> ();
		int[] sids = FormationSampleManager.Instance.getAllFormation ();

		for (int i = 0; i < sids.Length; i++) {
			if (checkFormation (sids [i], oldlevel)) {
				list.Add (sids [i]);
			}
			if (checkFormation (sids [i], newlevel)) {
				list2.Add (sids [i]);
			}
		}
		if (list2.Count > list.Count) {
			list2.RemoveRange (0, list.Count);
			return list2;
		}

		return null;
	}

	public 	passObj getPlayerTeamPrepareObj (int formationLength)
	{
		passObj go = new passObj ();
 
		if (formationLength == 3) {
			go.obj = MonoBase.Create3Dobj ("TeamPrepare/Player/TeamPreparePlayerThreeLos").obj as GameObject;
		} else if (formationLength == 4) {
			go.obj = MonoBase.Create3Dobj ("TeamPrepare/Player/TeamPreparePlayerFourLos").obj as GameObject;
		} else if (formationLength == 5) {
			go.obj = MonoBase.Create3Dobj ("TeamPrepare/Player/TeamPreparePlayerFiveLos").obj as GameObject;
		} else {
			go.obj = MonoBase.Create3Dobj ("TeamPrepare/Player/TeamPreparePlayerTenLos").obj as GameObject;
		}
		return go;
	}

	public 	passObj getEnemyTeamPrepareObj (int formationLength)
	{
		passObj go = new passObj ();
		
		if (formationLength == 3) {
			go.obj = MonoBase.Create3Dobj ("TeamPrepare/Enemy/TeamPrepareEnemyThreeLos").obj as GameObject;
		} else if (formationLength == 4) {
			go.obj = MonoBase.Create3Dobj ("TeamPrepare/Enemy/TeamPrepareEnemyFourLos").obj as GameObject;
		} else if (formationLength == 5) {
			go.obj = MonoBase.Create3Dobj ("TeamPrepare/Enemy/TeamPrepareEnemyFiveLos").obj as GameObject;
		} else {
			go.obj = MonoBase.Create3Dobj ("TeamPrepare/Enemy/TeamPrepareEnemyTenLos").obj as GameObject;
		}
		return go;
	}

	public 	passObj getPlayerBattleFormationObj (int length)
	{
		passObj go = new passObj ();

		if (length == 3) {
			go.obj = MonoBase.Create3Dobj ("Formation/Player/FormationPlayerThreeLos").obj as GameObject;
		} else if (length == 4) {
			go.obj = MonoBase.Create3Dobj ("Formation/Player/FormationPlayerFourLos").obj as GameObject;
		} else if (length == 5) {
			go.obj = MonoBase.Create3Dobj ("Formation/Player/FormationPlayerFiveLos").obj as GameObject;
		} else {
			go.obj = MonoBase.Create3Dobj ("Formation/Player/FormationPlayerTenLos").obj as GameObject;
		}
		return go;
	}

	public 	passObj getPlayerInfoFormationObj (int length)
	{
		passObj go = new passObj ();
		
		if (length == 3) {
			go.obj = MonoBase.Create3Dobj ("TeamPrepare/PlayerInfo/TeamPreparePlayerInfoThreeLos").obj as GameObject;
		} else if (length == 4) {
			go.obj = MonoBase.Create3Dobj ("TeamPrepare/PlayerInfo/TeamPreparePlayerInfoFourLos").obj as GameObject;
		} else if (length == 5) {
			go.obj = MonoBase.Create3Dobj ("TeamPrepare/PlayerInfo/TeamPreparePlayerInfoFiveLos").obj as GameObject;
		} else {
			go.obj = MonoBase.Create3Dobj ("TeamPrepare/PlayerInfo/TeamPreparePlayerInfoTenLos").obj as GameObject;
		}
		return go;
	}

	public passObj getEnemyBattleFormationObj (int length)
	{
		passObj go = new passObj ();
		
		if (length == 3) {
			go.obj = MonoBase.Create3Dobj ("Formation/Enemy/FormationEnemyThreeLos").obj as GameObject;
		} else if (length == 4) {
			go.obj = MonoBase.Create3Dobj ("Formation/Enemy/FormationEnemyFourLos").obj as GameObject;
		} else if (length == 5) {
			go.obj = MonoBase.Create3Dobj ("Formation/Enemy/FormationEnemyFiveLos").obj as GameObject;
		} else {
			go.obj = MonoBase.Create3Dobj ("Formation/Enemy/FormationEnemyTenLos").obj as GameObject;
		}
		return go;
	}

	//加载阵型对象
	public GameObject loadFormationPrefab (int length, GameObject root, bool isPlayer)
	{
 
		//大于13都按10人阵处理
		passObj go = null;

		if (isPlayer) {
			go = getPlayerTeamPrepareObj (length);
		} else {
			go = getEnemyTeamPrepareObj (length);
		}
		go.obj.transform.parent = root.transform;
		go.obj .transform.localPosition = Vector3.zero;
		go.obj .transform.localScale = Vector3.one;
		return go.obj;

		
	}

	//设置该队伍阵形
	public GameObject setFormationType (bool isPlayerSide)
	{
		GameObject root = null;
		if (isPlayerSide) {
			if (BattleManager.battleData.battleType == BattleType.BATTLE_TEN) {
				root = FormationManagerment.Instance.loadBattleFormationPrefab (10, GameObject.Find ("3Dscreen/root"), isPlayerSide);
			} else {
				if (BattleManager.battleData.isPvP == true) {
					//pvp阵形方案id 由战报决定
					root = FormationManagerment.Instance.loadBattleFormationPrefab (FormationSampleManager.Instance.getFormationSampleBySid (BattleManager.battleData.playerFormationID) .getLength (), GameObject.Find ("3Dscreen/root"), isPlayerSide);
				} else {
					if (BattleManager.battleData.isGuide == true) {
						//新手演播战用后台阵形数据
						root = FormationManagerment.Instance.loadBattleFormationPrefab (FormationSampleManager.Instance.getFormationSampleBySid (BattleManager.battleData.playerFormationID) .getLength (), GameObject.Find ("3Dscreen/root"), isPlayerSide);
                    } else if (BattleManager.battleData.isMineralFight == true) {
                        root = FormationManagerment.Instance.loadBattleFormationPrefab(ArmyManager.Instance.getActiveArmy().getLength(), GameObject.Find("3Dscreen/root"), isPlayerSide);
                    } else {
						root = FormationManagerment.Instance.loadBattleFormationPrefab (ArmyManager.Instance.getActiveArmy () .getLength (), GameObject.Find ("3Dscreen/root"), isPlayerSide);
					}
				}
			}
		} else {
			//pvp
			if (BattleManager.battleData.isPvP == true || BattleManager.battleData.isArenaMass || BattleManager.battleData.isLadders || BattleManager.battleData.isLaddersRecord || BattleManager.battleData.isArenaFinal || BattleManager.battleData.isMineralFightRecord||
                BattleManager.battleData.isGodsWarFinal || BattleManager.battleData.isGodsWarGroupFight || BattleManager.battleData.isMineralFight) {
				//演播战也按战报
				if (BattleManager.battleData.battleType == BattleType.BATTLE_TEN) { 

					root = FormationManagerment.Instance.loadBattleFormationPrefab (10, GameObject.Find ("3Dscreen/root"), isPlayerSide);
				} else {
					if (BattleManager.battleData.enemyFormationID > 0) {
						//取对面玩家阵形
						root = FormationManagerment.Instance.loadBattleFormationPrefab (FormationSampleManager.Instance.getFormationSampleBySid (BattleManager.battleData.
					enemyFormationID).getLength (), GameObject.Find ("3Dscreen/root"), isPlayerSide);
					} else {
                        if (BattleManager.battleData.isGodsWarFinal || BattleManager.battleData.isGodsWarGroupFight || BattleManager.battleData.isMineralFight) root = loadBattleFormationPrefab(5, GameObject.Find("3Dscreen/root"), isPlayerSide);
						//无阵形按pve处理
						else root = FormationManagerment.Instance.loadBattleFormationPrefab (BattleManager.battleData.pveTeamNum, GameObject.Find ("3Dscreen/root"), isPlayerSide);
					}
				}
			} else {
				//pve中按配置好的人数来确定阵形
				root = FormationManagerment.Instance.loadBattleFormationPrefab (BattleManager.battleData.pveTeamNum, GameObject.Find ("3Dscreen/root"), isPlayerSide);

			}
		}

		return root;
	}
	//加载阵型对象
	public GameObject loadBattleFormationPrefab (int length, GameObject root, bool isplayer)
	{
		passObj go;
 
		
		if (isplayer) {
			go = getPlayerBattleFormationObj (length);
		} else {
			go = getEnemyBattleFormationObj (length);
		}
		
		go.obj.transform.parent = root.transform;
		go.obj .transform.localPosition = Vector3.zero;
		go.obj .transform.localScale = Vector3.one;
		return go.obj;

		
	}
	
	
	//获得所有符合玩家的阵型的数量
	public int getFormationCounts ()
	{
		return getPlayerFormation ().Count;
	}

	/// <summary>
	/// 根据队伍编辑点位获得阵型所在位置
	/// </summary>
	/// <param name="sid">阵型SID</param>
	/// <param name="index">队伍编辑中的点位0-4</param>
	public int getLoctionByIndex (int sid, int index)
	{
		FormationSample sample = FormationSampleManager.Instance.getFormationSampleBySid (sid);
		if (index >= sample.formations.Length)
			return -1;
		return sample.formations [index];
	}

	/// <summary>
	/// 根据所在阵型中位置获得队伍编辑点位
	/// </summary>
	/// <param name="sid">阵型SID</param>
	/// <param name="loction">阵型中所在的位置1-15</param>
	public int getIndexByLoction (int sid, int loction)
	{
		FormationSample sample = FormationSampleManager.Instance.getFormationSampleBySid (sid);
		for (int i = 0; i < sample.formations.Length; i++) {
			if (loction == sample.formations [i])
				return i;
		}
		return 0;
	}
	//效验符合玩家要求的阵型
	private bool checkFormation (int sid)
	{
		int lv = UserManager.Instance.self.getUserLevel ();
		return checkFormation (sid, lv);
	}
	//效验符合玩家要求的阵型
	private bool checkFormation (int sid, int lv)
	{
	
		FormationSample sample = FormationSampleManager.Instance.getFormationSampleBySid (sid);
		if (lv >= sample.openLevel) {
			if (sample.closeLevel == 0) {
				return true;
			}
			if (lv < sample.closeLevel) {
				return true;
			}
		}
		return false;
	}
	
}
