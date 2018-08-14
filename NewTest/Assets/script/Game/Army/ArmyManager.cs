using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

// 队伍数据管理器 
// @author 张海山

public class ArmyManager
{
	/** 自动复制冒险队伍等级 */
	public const int COPY_ARMY_LEVEL = 11;
	public const int TEAM_LENGTH = 6;//玩家最大队伍数量
	/** 冒险队伍ID */
	public const int PVE_TEAMID = 1;
	/** 竞技队伍ID */
	public const int PVP_TEAMID = 3;
	/** 公会战队伍ID */
	public const int PVP_GUILD = 6;
	//单例
	private Army[] armys = new Army[TEAM_LENGTH];
	public int  activeID = 0;//本次程序周期内副本激活的方案;
	public int  activeGuildID = 0;//本次程序周期内工会boss激活的方案;

	private int lastId = 1;//上次用的ID ,后台返回
	private int  lockID = 0;//正在副本的ID
	
	//队伍编辑窗口中的方案数据
	public Army ActiveEditArmy;
	public Army EditArmy1;
	public Army EditArmy2;
	public Army EditArmy3;

	//挖矿队伍
	public Army EditArmy4;
	public Army EditArmy5;

   // public Army EditArmy6;//爬塔队伍
	#region gc

	//公会战队伍
	public Army EditArmy6;
	//全队(0:挂掉 1：满血)
	public int isdeath = 999;

	//公会战队伍血量信息
	public List<CardHp> hp;

	public const int TYPE_1 = 1,//队伍1，2，3，6
					  TYPE_2 = 2;//队伍4，5

	public bool teamEditInMissonWin = false;

	public class CardHp
	{
		public string cardUid;
		public int currentHp;
		public int maxHp;

	}
	/// <summary>
	/// 通过卡牌Uid获得血条信息
	/// </summary>
	public int[] getCardHpByUid(string uid)
	{
		int[] _hp = new int[2];

		if(isdeath==1)
		{
			_hp[0] = 1;
			_hp[1] = 1;

		}
		else if(isdeath==0)
		{
			_hp[0] = 0;
			_hp[1] = 1;
		}
		else if(hp!=null)
		{
			foreach( CardHp c in hp)
			{
				if(c.cardUid == uid)
				{
					_hp[0] = c.currentHp;
					_hp[1] = c.maxHp;
				}
			}
		}

		return _hp;
	}
	/// <summary>
	/// Sets the state of the army.
	/// </summary>
	public void setArmyState(int armyid,int state)
	{
		armys[armyid-1].state = state;
			
	}
	#endregion

	//更换替补
	public Card activeInstandCard;//想要替换的队员

	//全队伍数据
	List<string>  allIAlternateList;
	List<string>  allIPlayerList;
	ArrayList teamCards;

	public static ArmyManager Instance {
		get{ return SingleManager.Instance.getObj ("ArmyManager") as ArmyManager;}
	}

	void changeLastId (int last)
	{
		if (last != 0)
			this.lastId = last;
		else
			this.lastId = 1;
	}

	/// <summary>
	/// 锁定队伍，-1为不锁定
	/// </summary>
	/// <param name="id">Identifier.</param>
	public void lockArmyID (int id)
	{
		if (id == -1) {
			return;
		}
		setActive (id);
		lockID = id;

		foreach (Army each in armys){
			if(each == null)
				continue;
			
			if (each.armyid == id)
				each.state = 1;
			else
				each.state = 0;
		}
	}

	public int  getlockArmyID ()
	{
		foreach (Army each in armys) {
			if(each == null)
				continue;
			if (each.state == 1)
				return each.armyid;
		}
		return 0;
	}

	public int getLastId ()
	{
		return lastId;
	}
	
	//清除队伍状态
	public void clearArmyState ()
	{
		//cleanAllEditArmy ();
		for (int i = 0; i < 5; i++) {
			if(armys[i] !=null)
			armys [i].state = 0;
		}
	}
	
	public void cleanAllEditArmy ()
	{
		ActiveEditArmy = null;
		EditArmy1 = null;
		EditArmy2 = null;
		EditArmy3 = null;
			
	}
	//相同返回True
	public bool compareArmy (Army army1, Army army2)
	{ 
		if (army1 == null || army2 == null)
			return false;
		if (army1.armyid != army2.armyid)
			return false;
		if (army1.formationID != army2.formationID)
			return false;		
		if (army1.beastid != army2.beastid)
			return false;
		//判断角色是否一样
		if (!checkLoction (army1.players, army2.players))
			return false;
		//判断角色是否一样
		if (!checkLoction (army1.alternate, army2.alternate))
			return false; 
		return true;		
	}
	
	//检查阵型位置
	public bool checkLoction (string[] locs1, string[] locs2)
	{
		if (locs1 == null || locs2 == null)
			return false;
		if (locs1.Length != locs2.Length)
			return false;
		for (int i = 0; i < locs1.Length; i++) {
			if (locs1 [i] != locs2 [i])
				return false;
		}
		return true;
	}
	
	//比较队伍信息
	public bool compareIntArray (int[] array1, int[] array2)
	{
		if (array1 == null || array2 == null)
			return false;
		if (array1.Length != array2.Length)
			return false;
		for (int i = 0; i < array1.Length; i++) {
			if (!checkIntInArray (array1 [i], array2))
				return false;
		}
		return true;
	}
	
	//检查数组中是否有指定数字
	private bool checkIntInArray (int a, int[] array)
	{
		if (array == null || array.Length < 1) {
			return false;
		}
		for (int i = 0; i < array.Length; i++) {
			if (array [i] == a)
				return true;
		}
		return false;
	}

	public bool CheckActiveEditArrayIsEmpty(){
		for(int i=0;i<ActiveEditArmy.players.Length;i++){
			if(ActiveEditArmy.players[i] != "0" )
				return false;
		}
		return true;
	}
	
	public void createArmy (int armyid, int arrayid, string beastid, string[] team, string[] alternate, int state)
	{
		if (armys == null)
			armys = new Army[TEAM_LENGTH];

		armys [armyid - 1] = new Army (armyid, arrayid, beastid, team, alternate, state);  
	}
	
	public void updateArmy (int id, Army army2)
	{
		copyData (army2, armys [id - 1]);
	}
	
	public Army DeepClone (Army org)
	{
		if (org == null)
			return null;
		Army tmp = new Army ();
		
		tmp.beastid = org.beastid;
		tmp.armyid = org.armyid;
		tmp.formationID = org.formationID;
		tmp.state = org.state;
		tmp.players = new string[org.players.Length];
		
		for (int i=0; i<org.players.Length; i++) {
			tmp.players [i] = org.players [i];
		}
		
		tmp.alternate = new string[org.alternate.Length];
		for (int i=0; i<org.alternate.Length; i++) {
			tmp.alternate [i] = org.alternate [i];
		}	  
			
		return tmp;
		
	}
	
	public void copyData (Army score, Army target)
	{
		if (score == null||target==null)
			return;
        Debug.Log(score.GetHashCode() + "," + target.GetHashCode());
		target.armyid = score.armyid;
		target.formationID = score.formationID;
		target.beastid = score.beastid;
		target.players = new string[score.players.Length];
        
		
		for (int i=0; i<score.players.Length; i++) {
			target.players [i] = score.players [i];
		}
		 
		target.alternate = new string[score.alternate.Length];
		for (int i=0; i<score.alternate.Length; i++) {
			target.alternate [i] = score.alternate [i];
		}
	}

	public Army getArmy (int armyid)
	{
		return armys [armyid - 1];
	}

	bool isFirstCloneMiningTeam = false;
	bool isFirstCloneGuildFightTeam = false;

	public void InitMiningTeam(){
		if(getArmy(4) == null){
			EditArmy4  = DeepClone(getArmy(3));
			EditArmy4.armyid = 4;
			EditArmy4.ResetAlternate();
            armys[3] = DeepClone(EditArmy4);
			isFirstCloneMiningTeam = true;
		}

		if(getArmy(5) == null){
			EditArmy5  = DeepClone(getArmy(3));
			EditArmy5.armyid = 5;
            EditArmy5.beastid = "0";
			EditArmy5.AlternateTransformPlayer();
            ActiveEditArmy = EditArmy5;
            //检查5队的卡片是否在4队已经上阵
            for (int i = 0; i < EditArmy5.players.Length;i++ ) {
                string uid = EditArmy5.players[i];

                if (IsHaveSameSIDCardInMineralTeam(StorageManagerment.Instance.getRole(uid))) {
                    EditArmy5.players[i] = "0";
                }
            }
            armys[4] = DeepClone(EditArmy5);
            isFirstCloneMiningTeam = true;
		}

	}

	public void InitGuildFightTeam(){

		if(getArmy(6) == null){
			EditArmy6  = DeepClone(getArmy(3));
			EditArmy6.armyid = 6;
			EditArmy6.ResetAlternate();
			armys[5] = DeepClone(EditArmy6);
			isFirstCloneGuildFightTeam = true;
		}
	}


	private int showTeamType = 1;//主界面展示战斗力的队伍

	public int getShowTeamType ()
	{
		return showTeamType;
	}

	public void setShowTeamType ()
	{
		if (getShowTeamType () == 1) {
			showTeamType = 2;
			return;
		} else if (getShowTeamType () == 2) {
			showTeamType = 1;
			return;
		}
	}

	/// <summary>
	/// 返回队伍战斗力 默认是主力的战斗力
	/// </summary>
	public int getTeamCombat (int teamId)
	{
		return (armys [teamId - 1]).getMainCombat ();
	}
	/// <summary>
	/// 返回队伍战斗力 替补的战斗力
	/// </summary>
	/// <returns>The team substitute combat.</returns>
	/// <param name="teamId">Team identifier.</param>
	public int getTeamSubstituteCombat(int teamId)
	{
		return (armys [teamId - 1]).getSubCombat ();
	}
	/// <summary>
	/// 返回队伍所有战斗力
	/// </summary>
	/// <returns>The team all combat.</returns>
	/// <param name="teamId">Team identifier.</param>
	public int getTeamAllCombat(int teamId)
	{
		return (armys[teamId -1]).getAllCombat();
	}


	/// <summary>
	/// 获得指定队伍的上阵卡片
	/// </summary>
	/// <returns>所有该队伍上阵的卡片</returns>
	/// <param name="id">队伍Id.</param>
	public List<string> getCardsByTeam (int id)
	{
		List<string> tempList = new List<string>();
		if (getArmy (id) != null && getArmy (id).players != null) {
			for (int i = 0; i < getArmy(id).players.Length; i++) {
				tempList.Add (getArmy (id).players [i]);
			}
		}
		if (getArmy (id) != null && getArmy (id).alternate != null) {
			for (int i = 0; i < getArmy(id).alternate.Length; i++) {
				tempList.Add (getArmy (id).alternate [i]);
			}
		}
		return tempList;
	}
	
	//重新计算上证卡片信息(主力)
	private void recalculateAllArmyPlayersIds ()
	{ 
		allIPlayerList = new List<string> ();
		if (getArmy (1) != null && getArmy (1).players != null) {
			for (int i = 0; i < getArmy(1).players.Length; i++) {
				allIPlayerList.Add (getArmy (1).players [i]);
			}
		}
		if (getArmy (2) != null && getArmy (2).players != null) {
			for (int i = 0; i < getArmy(2).players.Length; i++) {
				if (!allIPlayerList.Contains (getArmy (2).players [i]))
					allIPlayerList.Add (getArmy (2).players [i]);
			}
		} 
		if (getArmy (3) != null && getArmy (3).players != null) {
			for (int i = 0; i < getArmy(3).players.Length; i++) {
				if (!allIPlayerList.Contains (getArmy (3).players [i]))
					allIPlayerList.Add (getArmy (3).players [i]);
			}
		}
		if (getArmy (4) != null && getArmy (4).players != null) {
			for (int i = 0; i < getArmy(4).players.Length; i++) {
				if (!allIPlayerList.Contains (getArmy (4).players [i]))
					allIPlayerList.Add (getArmy (4).players [i]);
			}
		}
		if (getArmy (5) != null && getArmy (5).players != null) {
			for (int i = 0; i < getArmy(5).players.Length; i++) {
				if (!allIPlayerList.Contains (getArmy (5).players [i]))
					allIPlayerList.Add (getArmy (5).players [i]);
			}
		}
		if (getArmy (6) != null && getArmy (6).players != null) {
			for (int i = 0; i < getArmy(6).players.Length; i++) {
				if (!allIPlayerList.Contains (getArmy (6).players [i]))
					allIPlayerList.Add (getArmy (6).players [i]);
			}
		}
	}
	/// <summary>
	/// 激活的队伍中是否存在此uid的卡
	/// 如果是4队或5对，则相互检查是否存在此uid的卡
	/// </summary>
	/// <param name="uid">Uid.</param>
	public bool isExistByActiveEditArmy(string uid) {
		if (ArmyManager.Instance.ActiveEditArmy == null)
			return false;
		if(ArmyManager.Instance.ActiveEditArmy != null) {
			foreach (string each in ArmyManager.Instance.ActiveEditArmy.players) {
				if (each == "0")
					continue;
				if (each == uid)
					return true;
			}
			foreach (string each in ArmyManager.Instance.ActiveEditArmy.alternate) {
				if (each == "0")
					continue;
				if (each == uid) {
					return true;
				}
			}
		}

		//守矿队伍相互检查
		if(ArmyManager.Instance.ActiveEditArmy.armyid == 4  &&  ArmyManager.Instance.getArmy(5) != null){
			foreach (string each in ArmyManager.Instance.getArmy(5).players) {
				if (each == "0")
					continue;
				if (each == uid)
					return true;
			}
		}else if(ArmyManager.Instance.ActiveEditArmy.armyid == 5){
			foreach (string each in ArmyManager.Instance.getArmy(4).players) {
				if (each == "0")
					continue;
				if (each == uid)
					return true;
			}
		}


		return false;
	}
	
	public List<string> getAllArmyPlayersIds ()
	{
		return allIPlayerList;
	}
	
	public List<string> getAllArmyAlternateIds ()
	{
		return allIAlternateList;
	}

	public ArrayList getTeamCardUidList ()
	{
		return teamCards;
	}

	/** 获得所有替补 */
	public List<string> getAllArmyAlternateIdsNoNull ()
	{
		List<string> alllist = new List<string> ();
		Card card = null;
		foreach (string each in allIAlternateList) {
			if (string.IsNullOrEmpty (each) || each == "0")
				continue;
			card = StorageManagerment.Instance.getRole (each);
			if (card == null)
				continue;
			alllist.Add (each);
		}
		return alllist;
	}
	
	//重新计算上阵卡片信息
	public List<string> recalculateAllArmyIds ()
	{
		List<string> alllist = new List<string> ();
		recalculateAllArmyAlternateIds ();
		recalculateAllArmyPlayersIds ();
		
		teamCards = new ArrayList ();
		
		foreach (string each in allIPlayerList) {
			if (string.IsNullOrEmpty (each) || each == "0")
				continue;
			if (teamCards.Contains (each))
				continue;
			Card card = StorageManagerment.Instance.getRole (each);
			if (card == null)
				continue;
			alllist.Add (each);
			teamCards.Add (each);
//			alllist.Add (each);
//			if (card != null) {
//				
//				bool has = false;//看循环里是否有相同的
//				foreach (string each2 in teamCards) {
//					if (each2 == each) {
//						has = true;
//						break;
//					}
//				}
//				if (has == false)
//					teamCards.Add (card.uid);
//			}
			
		}
		
		foreach (string each in allIAlternateList) {
			if (string.IsNullOrEmpty (each) || each == "0")
				continue;
			if (teamCards.Contains (each))
				continue;
			Card card = StorageManagerment.Instance.getRole (each);
			if (card == null)
				continue;
			alllist.Add (each);
			teamCards.Add (each);
//			Card card = StorageManagerment.Instance.getRole (each);
//			alllist.Add (each);
//			if (card != null) {
//				
//				bool has = false;//看循环里是否有相同的
//				foreach (string each2 in teamCards) {
//				
//					if (each2 == each) {
//						has = true;
//						break;
//					}
//				}
//				
//				if (has == false)
//					teamCards.Add (card.uid);	
//			}
		} 
		
		return alllist;
	}
	
	//重新计算上证卡片信息(替补)
	private void recalculateAllArmyAlternateIds ()
	{
		allIAlternateList = new List<string> ();
		if (getArmy (1) != null && getArmy (1).alternate != null) {
			for (int i = 0; i < getArmy(1).alternate.Length; i++) {
				allIAlternateList.Add (getArmy (1).alternate [i]);
			}
		}
		if (getArmy (2) != null && getArmy (2).alternate != null) {
			for (int i = 0; i < getArmy(2).alternate.Length; i++) {
				if (!allIAlternateList.Contains (getArmy (2).alternate [i]))
					allIAlternateList.Add (getArmy (2).alternate [i]);
			}
		}
		if (getArmy (3) != null && getArmy (3).alternate != null) {
			for (int i = 0; i < getArmy(3).alternate.Length; i++) {
				if (!allIAlternateList.Contains (getArmy (3).alternate [i]))
					allIAlternateList.Add (getArmy (3).alternate [i]);
			}
		}
		if (getArmy (4) != null && getArmy (4).alternate != null) {
			for (int i = 0; i < getArmy(4).alternate.Length; i++) {
				if (!allIAlternateList.Contains (getArmy (4).alternate [i]))
					allIAlternateList.Add (getArmy (4).alternate [i]);
			}
		}
		if (getArmy (5) != null && getArmy (5).alternate != null) {
			for (int i = 0; i < getArmy(5).alternate.Length; i++) {
				if (!allIAlternateList.Contains (getArmy (5).alternate [i]))
					allIAlternateList.Add (getArmy (5).alternate [i]);
			}
		}
		if (getArmy (6) != null && getArmy (6).alternate != null) {
			for (int i = 0; i < getArmy(6).alternate.Length; i++) {
				if (!allIAlternateList.Contains (getArmy (6).alternate [i]))
					allIAlternateList.Add (getArmy (6).alternate [i]);
			}
		}

	}

	//重新计算编辑中队伍卡片信息
	public List<Card> getAllEditArmyCards ()
	{
		List<Card> allIEditArmyCards = new List<Card> ();
		if (EditArmy1 != null && EditArmy1.getCardList () != null) {
			for (int i = 0; i < EditArmy1.getCardList().Count; i++) {
				allIEditArmyCards.Add (EditArmy1.getCardList () [i]);
			}
		}
		if (EditArmy2 != null && EditArmy2.getCardList () != null) {
			for (int i = 0; i < EditArmy2.getCardList().Count; i++) {
				if (!allIEditArmyCards.Contains (EditArmy2.getCardList () [i]))
					allIEditArmyCards.Add (EditArmy2.getCardList () [i]);
			}
		}
		if (EditArmy3 != null && EditArmy3.getCardList () != null) {
			for (int i = 0; i < EditArmy3.getCardList().Count; i++) {
				if (!allIEditArmyCards.Contains (EditArmy3.getCardList () [i]))
					allIEditArmyCards.Add (EditArmy3.getCardList () [i]);
			}
		}

		return allIEditArmyCards;
	}

	/** 获取当前副本中激活的队伍 */
	public Army getActiveArmy ()
	{
		//没有激活就取上次用的
		if (activeID <= 0) 
			return armys [lastId - 1];
 
		return armys [activeID - 1];
	}

	/** 当前编辑的队伍是否正在冒险中 */
	public bool isEditArmyActive ()
	{
	
		int id = ArmyManager.Instance.getlockArmyID ();
		//公会战队伍单独处理，因为以前锁队伍只适用123队伍
		if(ActiveEditArmy.armyid==6)
			return ActiveEditArmy.state==1;

		if (ActiveEditArmy.armyid == id && id > 0  && !(id == 4 || id == 5))
			return true;

		return false;
		//	return ActiveEditArmy != null && activeArmy != null && activeArmy.armyid == ActiveEditArmy.armyid;
	}

	public bool saveArmy () {
		return saveArmy (null);
	}

	public bool saveArmy (CallBack callBack) {
		Army[] arr = getSaveArmys ();
		if (arr.Length > 0) {
			List<string> oldCards = getAllArmyCardsExceptMining ();//改变队伍前的卡片上阵情况
			List<string> oldBeasts = getFightBeasts ();//改变队伍前的召唤兽情况
			ArmyUpdateFPort port = FPortManager.Instance.getFPort ("ArmyUpdateFPort") as ArmyUpdateFPort;
			port.access (arr, () => {
				updateArmy (1, ArmyManager.Instance.EditArmy1);
				updateArmy (2, ArmyManager.Instance.EditArmy2);
				updateArmy (3, ArmyManager.Instance.EditArmy3);
				recalculateAllArmyIds ();
				updateBackChangeState (oldCards, oldBeasts,TYPE_1);
				IncreaseManagerment.Instance.clearData (IncreaseManagerment.TYPE_CARD);
				if(callBack!=null)
					callBack ();
			});
			return true;
		}
		return false;
	}

	public bool saveGuildFightArmy (CallBack callBack) {

		Army[] arr = getSaveGuildFightArmys ();
		if (arr.Length > 0) {
			List<string> oldCards = getAllArmyCardsExceptMining ();//改变队伍前的卡片上阵情况
			List<string> oldBeasts = getFightBeasts ();//改变队伍前的召唤兽情况
			ArmyUpdateFPort port = FPortManager.Instance.getFPort ("ArmyUpdateFPort") as ArmyUpdateFPort;
			port.access (arr, () => {
				updateArmy (6, ArmyManager.Instance.EditArmy6);
				recalculateAllArmyIds ();
				updateBackChangeState (oldCards, oldBeasts,TYPE_1);
				IncreaseManagerment.Instance.clearData (IncreaseManagerment.TYPE_CARD);
				if(callBack!=null)
					callBack ();
			});
			return true;
		}
		return false;
	}

	public bool SaveMiningArmy(CallBack callBack){
		Army[] arr = getSaveMiningArmys ();
         Army aa = ArmyManager.Instance.EditArmy4;
		if (arr.Length > 0) {
			List<string> oldCards = getMiningCards ();//改变队伍前的卡片上阵情况
			List<string> oldBeasts = getFightBeasts ();//改变队伍前的召唤兽情况
			MiningArmyUpdateFPort port = FPortManager.Instance.getFPort<MiningArmyUpdateFPort>();
			port.access (arr, () => {
				updateArmy (4, ArmyManager.Instance.EditArmy4);
                
				updateArmy (5, ArmyManager.Instance.EditArmy5);
                
				recalculateAllArmyIds ();
				updateBackChangeState (oldCards, oldBeasts,TYPE_2);
				IncreaseManagerment.Instance.clearData (IncreaseManagerment.TYPE_CARD);
               
                FPortManager.Instance.getFPort<GetMineralsFport>().access(() => {
                    if (callBack != null)
                        callBack();
                });
				
			});
            aa = ArmyManager.Instance.EditArmy4;       
			return true;
		}
		return false;
	}

	public Army[] getSaveArmys ()
	{
		List<Army> list = new List<Army> ();
		// 取消在冒险中不能换阵//
		if(ArmyManager.Instance.EditArmy1 != null && !ArmyManager.Instance.compareArmy (ArmyManager.Instance.EditArmy1, ArmyManager.Instance.getArmy (1)))
		{
			list.Add (ArmyManager.Instance.EditArmy1);
		}
		if (ArmyManager.Instance.EditArmy2 != null && !ArmyManager.Instance.compareArmy (ArmyManager.Instance.EditArmy2, ArmyManager.Instance.getArmy (2))) {
			list.Add (ArmyManager.Instance.EditArmy2);
		}
		if (ArmyManager.Instance.EditArmy3 != null && !ArmyManager.Instance.compareArmy (ArmyManager.Instance.EditArmy3, ArmyManager.Instance.getArmy (3))) {
			list.Add (ArmyManager.Instance.EditArmy3);
		}

//		if (ArmyManager.Instance.EditArmy1 != null && ArmyManager.Instance.EditArmy1.state != 1 && !ArmyManager.Instance.compareArmy (ArmyManager.Instance.EditArmy1, ArmyManager.Instance.getArmy (1))) {
//			list.Add (ArmyManager.Instance.EditArmy1);
//		}
//		if (ArmyManager.Instance.EditArmy2 != null && ArmyManager.Instance.EditArmy2.state != 1 && !ArmyManager.Instance.compareArmy (ArmyManager.Instance.EditArmy2, ArmyManager.Instance.getArmy (2))) {
//			list.Add (ArmyManager.Instance.EditArmy2);
//		}
//		if (ArmyManager.Instance.EditArmy3 != null && ArmyManager.Instance.EditArmy3.state != 1 && !ArmyManager.Instance.compareArmy (ArmyManager.Instance.EditArmy3, ArmyManager.Instance.getArmy (3))) {
//			list.Add (ArmyManager.Instance.EditArmy3);
//		}
		return list.ToArray ();
	}

	public Army[] getSaveGuildFightArmys ()
	{
		List<Army> list = new List<Army> ();
		if (isFirstCloneGuildFightTeam||ArmyManager.Instance.EditArmy6 != null && ArmyManager.Instance.EditArmy6.state != 1 && !ArmyManager.Instance.compareArmy (ArmyManager.Instance.EditArmy6, ArmyManager.Instance.getArmy (6))) {
			list.Add (ArmyManager.Instance.EditArmy6);
		}
		return list.ToArray ();
	}

	public Army[] getSaveMiningArmys ()
	{
		List<Army> list = new List<Army> ();
		if (isFirstCloneMiningTeam || ArmyManager.Instance.EditArmy4 != null && !ArmyManager.Instance.compareArmy (ArmyManager.Instance.EditArmy4, ArmyManager.Instance.getArmy (4))) {
			list.Add (ArmyManager.Instance.EditArmy4);
		}
        if (isFirstCloneMiningTeam && ArmyManager.Instance.EditArmy5 != null && ArmyManager.Instance.EditArmy5.getPlayerNum() !=0 || 
            ArmyManager.Instance.EditArmy5 != null && !ArmyManager.Instance.compareArmy(ArmyManager.Instance.EditArmy5, ArmyManager.Instance.getArmy(5))) {
			list.Add (ArmyManager.Instance.EditArmy5);
		}

		return list.ToArray ();
	}


	
	public void setActive (int armyid)
	{ 
		//unActiveArmy ();
		//armys [armyid - 1].state = 1;
		activeID = armyid;
		lastId = armyid;
	}
	
	public void unActiveArmy ()
	{
		armys [0].state = 0;
		armys [1].state = 0;
		armys [2].state = 0;
		//activeID = -1;
	}

	public void clean ()
	{
		armys = null;
		cleanAllEditArmy ();
		activeID = 0;
		activeInstandCard = null;
		allIAlternateList = null;
		allIPlayerList = null;
		teamCards = null;
 

	}


	//效验阵型 副本结束后升级强制新手指引换阵型,
	//没新手引导这里就会出问题,他就能跳过阵形调整继续去副本打
	public bool checkFormation ()
	{
		bool hasChange = false;
		for (int i = 0; i < armys.Length; i++) {
			if (armys [i] == null) {
				return true;
			}
			int tmpID = armys [i].formationID;
			armys [i].formationID = recursion (armys [i].formationID);
			if (tmpID != armys [i].formationID)
				hasChange = true;
		}

		return hasChange;
		 
	}
	//递归
	private int recursion (int sid)
	{
		FormationSample sample = FormationSampleManager.Instance.getFormationSampleBySid (sid);
		if (sample.closeLevel == 0) {
			return sid;
		}
		int lv = UserManager.Instance.self.getUserLevel ();
		if (lv < sample.closeLevel)
			return sid;
		return recursion (sample.upSid);
	}

	public string getActiveArmyName ()
	{
		string armyName = "";
		if (getActiveArmy () == getArmy (1)) {
			armyName = LanguageConfigManager.Instance.getLanguage ("s0066");
		} else if (getActiveArmy () == getArmy (2)) {
			armyName = LanguageConfigManager.Instance.getLanguage ("s0067");
		} else if (getActiveArmy () == getArmy (3)) {
			armyName = LanguageConfigManager.Instance.getLanguage ("s0068");
		} else {
			armyName = LanguageConfigManager.Instance.getLanguage ("s0066");
		}
		return armyName;
	}

	//true表示上满
	public bool checkArmyMemberCount ()
	{
		if (getArmy (PVP_TEAMID) != null) {
			Army army = getArmy (PVP_TEAMID);
			int length = army.getLength ();
			int playerNum = army.getPlayerNum ();
			int alternateNum = army.getAlternateNum ();
			if (playerNum + alternateNum < length * 2 || !army.isFightBeast ()) {
				return false;
			}
		}
		return true;
	}

	//获得上阵的卡片
	public List<string> getFightCards ()
	{
		List<string> cards = new List<string> ();
		for (int i = 0; i < armys.Length; i++) {
			Debug.LogError("armys.Length==="+armys.Length);
			if( armys [i] != null){
				addUid (cards, armys [i].players);//主力
				addUid (cards, armys [i].alternate);//替补
			}

		}
		return cards;
	}
	//获得除了天国宝藏的上阵的卡片
	public List<string> getAllArmyCardsExceptMining ()
	{
		List<string> cards = new List<string> ();
		for (int i = 0; i < armys.Length; i++) {
			if( armys [i] != null && i!=3 && i!=4){
				addUid (cards, armys [i].players);//主力
				addUid (cards, armys [i].alternate);//替补
			}
			
		}
		return cards;
	}
	//获得上阵的召唤兽
	public List<string> getFightBeasts ()
	{
		List<string> beasts = new List<string> ();
		for (int i = 0; i < armys.Length; i++) {
			if(armys [i]!=null)
			addUid (beasts, armys [i].beastid);//召唤兽
		}
		return beasts;
	}

	private void addUid (List<string> list, string[] uids)
	{
		for (int i = 0; i < uids.Length; i++)
			addUid (list, uids [i]);
	}

	private void addUid (List<string> list, string  uid)
	{

		if (!list.Contains (uid) && !string.IsNullOrEmpty (uid) && !uid.Equals ("0"))
			list.Add (uid);
	}

	//向后台通讯更新队伍后，前台自己计算需要改变的配置的状态
	public void updateBackChangeState (List<string> oldCards, List<string> oldBeasts,int type)
	{
		updateBackChangeCardsState (oldCards,type);
		updateBackChangeBeastsState (oldBeasts);
	}

	//改变卡片状态 返回新上阵的卡片集合
	public void updateBackChangeCardsState (List<string> oldCards,int type)
	{
		List<string> newCards = new List<string> ();
		if(type== TYPE_1)
			newCards = getAllArmyCardsExceptMining ();//新收集的除天国宝藏外上阵卡片
		else if(type== TYPE_2)
		    newCards = getMiningCards ();//新收集的天国宝藏上阵卡片

		removeDuplicate (oldCards, newCards);
		Card card;
		//下阵处理
		for (int i = 0; i < oldCards.Count; i++) {
			card = StorageManagerment.Instance.getRole (oldCards [i]);
			if(type==TYPE_2)
				card.delState (CardStateType.STATE_MINING);
			else if(type== TYPE_1)
				card.delState (CardStateType.STATE_USING);
//			card.delStarSoulBoreByAll();
		}
		//上阵处理
		for (int i = 0; i < newCards.Count; i++) {
			card = StorageManagerment.Instance.getRole (newCards [i]);
			if(type==TYPE_2)
				card.addState (CardStateType.STATE_MINING);
			else if(type== TYPE_1)
				card.addState (CardStateType.STATE_USING);
		}
	}

	//改变召唤兽状态
	public void updateBackChangeBeastsState (List<string> oldBeasts)
	{
		List<string> newBeasts = getFightBeasts ();
		removeDuplicate (oldBeasts, newBeasts);
		Card beast;
		//下阵处理
		for (int i = 0; i < oldBeasts.Count; i++) {
			beast = StorageManagerment.Instance.getBeast (oldBeasts [i]);
			beast.delState (CardStateType.STATE_USING);
		}
		//上阵处理
		for (int i = 0; i < newBeasts.Count; i++) {
			beast = StorageManagerment.Instance.getBeast (newBeasts [i]);
			beast.addState (CardStateType.STATE_USING);
		}
	}

	private void removeDuplicate (List<string> list1, List<string> list2)
	{
		for (int i = 0; i < list1.Count; i++) {
			if (list1.Count < 1 || list2.Count < 1)
				return;
			if (list2.Contains (list1 [i])) {
				//先删除list2的，避免list1[i]的数据删除丢失
				list2.Remove (list1 [i]);
				list1.Remove (list1 [i]);
				i--;
			}
		}
	}
	/// <summary>
	/// 获得天国宝藏上阵卡牌
	/// </summary>
	/// <returns>The mining cards.</returns>
    public List<string> getMiningCards ()
	{
		List<string> cards = new List<string> ();
		if(getArmy(4)!=null)
		{

			addUid (cards, getArmy(4).players);//主力
			addUid (cards, getArmy(4).alternate);//替补
		}
		if(getArmy(5)!=null)
		{
			
			addUid (cards, getArmy(5).players);//主力
			addUid (cards, getArmy(5).alternate);//替补
		}
		return cards;
	}

    public bool IsHaveSameSIDCardInMineralTeam(Card aCard)
    {
        //比较sid
        //判断是否是编辑的守矿队伍
        
        if (aCard == null || ActiveEditArmy.armyid != 4 && ActiveEditArmy.armyid != 5)
            return false;
        int armyid = 4;
        if (ActiveEditArmy.armyid == 4) {
            armyid = 5;
        }

        if (getArmy(armyid) == null) {
            return false;
        }
        List<Card> clist = getArmy(armyid).getPlayersByCard();
        if (clist == null || clist.Count <= 0) return false;
        bool retValue = false;
        for (int i = 0; i < clist.Count; i++)
        {
            Card cc = clist[i];
            if (cc.sid == aCard.sid)
            {
                retValue = true;
                break;
            }
        }
        return retValue;
    }

	/// <summary>
	/// 11级的时候偷偷复制一次冒险队伍，方便玩家PVP
	/// </summary>
	public void copyPveArmyTOPvp ()
	{
		int num = PlayerPrefs.GetInt (UserManager.Instance.self.uid + PlayerPrefsComm.COPY_ARMY, 0);
		if (num == 0 && UserManager.Instance.self.getUserLevel () >= COPY_ARMY_LEVEL && UserManager.Instance.self.getUserLevel () < (COPY_ARMY_LEVEL + 4)) {
			Army arenaArmy = getArmy (ArmyManager.PVP_TEAMID);
			if (arenaArmy.state == 0) {
				int count = 0;
				for (int i = 0; i<arenaArmy.players.Length; i++) {
					if (arenaArmy.players [i] != "0") {
						count++;
						if (count > 1)
							break;
					}
				}
				if (count <= 1) {
					for (int i = 0; i<arenaArmy.alternate.Length; i++) {
						if (arenaArmy.alternate [i] != "0") {
							count++;
							if (count > 1)
								break;
						}
					}
				}
				if (count <= 1) {
					List<string> oldCards = getAllArmyCardsExceptMining ();//改变队伍前的卡片上阵情况
					List<string> oldBeasts = getFightBeasts ();//改变队伍前的召唤兽情况
					Army temp = getArmy (ArmyManager.PVE_TEAMID);//获得队伍一的配置
					arenaArmy.formationID = temp.formationID;
					arenaArmy.beastid = temp.beastid;
					arenaArmy.players = temp.players;
					arenaArmy.alternate = temp.alternate;
					ArmyUpdateFPort port = FPortManager.Instance.getFPort ("ArmyUpdateFPort") as ArmyUpdateFPort;
					port.access (new Army[]{arenaArmy}, () => {
						updateBackChangeState (oldCards, oldBeasts,TYPE_1);
						PlayerPrefs.SetInt (UserManager.Instance.self.uid + PlayerPrefsComm.COPY_ARMY, 1);
						PlayerPrefs.Save ();
					});
				}
			}
		}
	}
}
