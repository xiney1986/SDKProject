using UnityEngine;
using System.Collections;
using System.Collections.Generic;


 //队伍数据管理器 
// @author 李程

public class BattleTeamManager
{ 
	public  List<CharacterData>	players;
//	public  List<CharacterData> substitute;//(10人战的第二排)替补队员

	public Formation  formation;//阵形
	public CharacterData GuardianForce;//召唤兽
	public int playerTitle;//称号
	public bool isGamePlayerTeam;
	public Transform TeamSide;
	public Transform TeamHitPoint;
	public Transform[] GroupPoint;
	public TeamInfo teamInfo;
	
	public BattleTeamManager (bool isPlayerTeam, TeamInfo info, int battleType)
	{ 
		isGamePlayerTeam = isPlayerTeam;
		teamInfo = info; 
		
		if (isGamePlayerTeam) {
			//设置已方队伍阵行
			TeamSide = BattleManager.Instance.playerSide.FindChild ("team");
			setGamePlayerTeam (isGamePlayerTeam, "player");
			
		} else {
			//设置敌方队伍阵行
			TeamSide = BattleManager.Instance.enemySide.FindChild ("team");
			setGamePlayerTeam (isGamePlayerTeam, "enemy");
		}
		
		//实例化所有队员characterData
		foreach (TeamInfoPlayer each in info.list) { 
			CharacterData data = BattleCharacterManager.Instance.CreateBattleCharacterData (each, this, battleType);
			if (data != null)
				addPlayer (data);
			else
				MonoBase.print ("no characterData:" + each.id + "   " + each.sid);
		} 

		//实例化队员characterCtrl.并放入队形点位 
		foreach (CharacterData each in players) { 
			
			if (each != null)
				CreateCharacterInstance (each); 

		}	 
		if (teamInfo.guardianForce != null)
			GuardianForce = CreateGuardianForce (battleType); 
	}
	
	//创建召唤兽
	public CharacterData CreateGuardianForce (int battleType)
	{ 
		CharacterData _data = BattleCharacterManager.Instance.CreateBattleCharacterData (teamInfo.guardianForce, this, battleType); 
		if (_data == null)
			return null;
		_data.isGuardianForce = true;  		
		_data.characterCtrl = CreateCharacterInstance (_data);

		return _data;
	}
	
	
	//往队伍中添加上场队员
	public void addPlayer (CharacterData _player)
	{ 
		if (players == null)
			players = new List<CharacterData> ();
		if (players.Contains (_player) == true)
			return;
		if (players.Count >= 10)
			return; 
		players.Add (_player);
	}
	//往队伍中添加替补
	public void Addsubstitute (CharacterData _player)
	{
		//目前的替补上阵是从players中把死人移走增加新人进去 

//		if (substitute.Contains (_player) == true)
//			return;
//		if (substitute.Count >= 10)
//			return;
//		
//		substitute.Add (_player);
		
	}	

	//创建队员表演实例
	public CharacterCtrl CreateCharacterInstance (CharacterData player)
	{
		//create characterCtrl
		passObj _obj; 
		if (player.role.isBoss ())
			_obj = MonoBase.Create3Dobj ("character/bossCard");
		else if (player.role.isBeast ())
			_obj = MonoBase.Create3Dobj ("character/beastCard");
		else
			_obj = MonoBase.Create3Dobj ("character/normalCard");  
		
		_obj.obj.transform.parent = TeamSide;
		
		if (BattleManager.battleData.battleType == BattleType.BATTLE_TEN)
			_obj.obj.transform.localScale = player.orgScale;
		else
			_obj.obj.transform.localScale = Vector3.one;	

		CharacterCtrl _ctrl = _obj.obj.GetComponent<CharacterCtrl> ();

		if(player.camp==TeamInfo.OWN_CAMP)
			_obj.obj.transform.position = player.orgPosition+new Vector3(0,0,-10);
		else
			_obj.obj.transform.position = player.orgPosition+new Vector3(0,0,10);

		_ctrl.init (player, this);
		_ctrl.gameObject.name = player.serverID.ToString ();
		return _ctrl;
	}
	
	public void PlayMonsterBuff () {
		string str = "";
		if (isGamePlayerTeam)
			str = GuardianForce.role.getFeatures () [1] + "\n" + LanguageConfigManager.Instance.getLanguage ("s0396", BattleManager.battleData.playerBeastEffect.ToString ());
		else
			str = GuardianForce.role.getFeatures () [1] + "\n" + LanguageConfigManager.Instance.getLanguage ("s0396", BattleManager.battleData.enemyBeastEffect.ToString ());

		UiManager.Instance.battleWindow .playBeastBuff (str, GuardianForce.role.getImageID ().ToString (), isGamePlayerTeam);
	}

	public void showGuardianForce ()
	{
		if (GuardianForce == null)
			return;
		
		GuardianForce.characterCtrl. GuardianForceShow ();
	}

	public void hideAllParter(){
		foreach (CharacterData each in players) {
			each.isInBattleField = false;
			iTween[] tw=	each.characterCtrl.GetComponents<iTween>();
			if(tw!=null)
			{
				foreach (iTween each2 in tw)
					GameObject.DestroyImmediate(each2);
			}

			each.characterCtrl.transform.position= new Vector3(100,100,100);
		}
	}

	void hideChild (Transform tran)
	{
		tran.gameObject.layer = 10;

		if (tran.childCount > 0) {
			foreach (Transform each in tran) {
				hideChild (each); 
			} 
		} 
	}

	void showChild (Transform tran)
	{
		tran.gameObject.layer = 9;

		if (tran.childCount > 0) {
			foreach (Transform each in tran) {
				showChild (each); 
			} 
		} 
		
	}
	
	public void showAllParter ()
	{
		
		foreach (CharacterData each in players) {
 
			each.characterCtrl.transform.position= each.orgPosition;
			
			each.isInBattleField = true;
		}

	}
	
	public void allParterOutBattleField ()
	{
		
		foreach (CharacterData each in players) {
			each.characterCtrl.outBattleField ();
		}
		
	}
	
	public void allParterInToBattleField ()
	{
		
		foreach (CharacterData each in players) {
			each.characterCtrl.inToBattleField ();
		}
		
	}
	
	//设置游戏队伍
	private void setGamePlayerTeam (bool isPlayerTeam, string teamName)
	{
		formation = new Formation ();
		
		formation.loadPoint (isPlayerTeam);//以人数来初始化阵型信息

		TeamHitPoint = GameObject.Find ("3Dscreen/root/" + teamName + "TeamHitPoint").transform;
		GroupPoint = new Transform[3];
		GroupPoint [0] = GameObject.Find ("3Dscreen/root/" + teamName + "GroupAttackPoint/1").transform;
		GroupPoint [1] = GameObject.Find ("3Dscreen/root/" + teamName + "GroupAttackPoint/2").transform;
		GroupPoint [2] = GameObject.Find ("3Dscreen/root/" + teamName + "GroupAttackPoint/3").transform;
	}

}
