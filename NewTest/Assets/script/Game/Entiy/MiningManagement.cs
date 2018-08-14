using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class MineralInfo{
	public int sid ;//产出类型
	public int armyId;//驻守队伍id
	public int localId;//矿坑id
	public long startTime;
	public long balanceTime;//剩余时间
	public int balanceCount;
}

public class EnemyMineralInfo{
	public string serverName;//服务器名字
	public string playerName;//玩家名字
	public string playerLevel;//玩家等级
	public int HeadIconId;//玩家头像id
	public int sid ;//产出类型
	public int localId;//矿坑id
	public int count;//抢劫收益
    public int rmb;//抢劫到的钻石
	public int combat;//战斗力
	public string[] roles = new string[5];//队伍sid
    public string[] evoLv = new string[5];//进化等级
}

public class PillageEnemyInfo{
	public string node;
	public string RoleUid;
	public int HeadIconId;//玩家头像id
	public string serverName;//服务器名字
	public string playerName;//玩家名字
	public long time;
	public int sid ;//产出类型
    public int rmb;//抢劫到的钻石
	public int count;//被抢劫
	public Dictionary<int,int> minerals = new Dictionary<int, int>(); 
}

public class MiningManagement {
	const int MINERAL_NUM = 2;
	/* static methods */
	public static MiningManagement Instance {
		get {
			MiningManagement manager = SingleManager.Instance.getObj ("MiningManagement") as MiningManagement;
			return manager;
		}
	}

	public MineralInfo[] minerals = new MineralInfo[MINERAL_NUM];
	public EnemyMineralInfo enemyMineralInfo = null;
	public List<PillageEnemyInfo> enemyInfoList = new List<PillageEnemyInfo>();

	public List<PillageEnemyInfo> GetEnemyInfoList(){
		return enemyInfoList;
	}

	public PillageEnemyInfo GetEnemyInfoByRoleUid(string uid){
		foreach(PillageEnemyInfo tmp in enemyInfoList){
			if(tmp.RoleUid.Equals(uid))
				return tmp;
		}
		return null;
	}

	public void AddEnemyInfoList( PillageEnemyInfo info){
		enemyInfoList.Add(info);
	}
    public void ClearEnemyInfoList() {
        enemyInfoList.Clear();
    }
	public void AddMineral(MineralInfo info){
		minerals[info.localId] = info;
	}

	public void AddEnemyMineral(EnemyMineralInfo enemyInfo){
		this.enemyMineralInfo = enemyInfo;
	}

	public EnemyMineralInfo GetEnemyMineral(){
		return this.enemyMineralInfo;
	}

	public MineralInfo[] GetMinerals(){
		return minerals;
	}


	public MiningSample GetMiningSampleBySid(int sid){
		return 	MiningSampleManager.Instance.GetMiningSampleBySid(sid);
	} 

	public MiningSample GetMiningSample(int localId){
		int sid = minerals[localId].sid;
		return 	MiningSampleManager.Instance.GetMiningSampleBySid(sid);
	}

	public int GetAvailableLocal(){
		for(int i=0;i<minerals.Length ;i++){
			if(minerals[i] == null)
				return i;
		}
		return -1;
	}

	public long GetRemainTime(int localId){

		//获取结束时间
        if (minerals == null || minerals[localId] == null)
            return 0;
		long endTime = minerals[localId].balanceTime;

		long nowTime=ServerTimeKit.getSecondTime();//现在的时间
		long timeLoading=endTime-nowTime;//时间差
		if(timeLoading<0){
			timeLoading=0;
		}
		return timeLoading;
	}

	public string GetMineralBackground(int localId){
		MineralInfo mineralInfo =  minerals[localId];
		return GetMiningSampleBySid(mineralInfo.sid).background;
	}

	public string GetMineralBackgroundBySid(int sid){
		return GetMiningSampleBySid(sid).background;
	}
	public float GetSpeed(int localId){
		MineralInfo mineralInfo =  minerals[localId];
        if (GetMiningSample(localId).type == (int)MiningTypePage.MiningGold) {
            float temp = GetMiningSampleBySid(mineralInfo.sid).outputRate + ArmyManager.Instance.getArmy(mineralInfo.armyId).getAllCombat() / 300000f;
            float maxNum = (float)CommandConfigManager.Instance.getMoneySpeedOfArean();
            return temp > maxNum ? maxNum : temp;
            //return GetMiningSampleBySid(mineralInfo.sid).outputRate + ArmyManager.Instance.getArmy(mineralInfo.armyId).getAllCombat() / 300000f;
        }
		return GetMiningSampleBySid(mineralInfo.sid).outputRate;
	}
	public int GetBalance(int localId){
		MineralInfo mineralInfo =  minerals[localId];
		float balance = mineralInfo.balanceCount;

		if(GetMiningSample(localId).type == (int)MiningTypePage.MiningGold){
			if(GetRemainTime(localId) > 0){
                float time = ServerTimeKit.getSecondTime() - mineralInfo.startTime;
				balance += (float)(time<0?0:time *GetSpeed(localId));
			}else{
                balance += (float)((mineralInfo.balanceTime - mineralInfo.startTime)*GetSpeed(localId));
			}
		}else{
			if(GetRemainTime(localId) > 0){
                float time = ServerTimeKit.getSecondTime() - mineralInfo.startTime;
                balance += (float)(time < 0 ? 0 : time * (GetSpeed(localId)));
			}else{
				balance += (float)((mineralInfo.balanceTime - mineralInfo.startTime) *(GetSpeed(localId)));
			}
		}

        if (balance < 0) {
            Debug.Log(12312);
        
        }
		return (int)balance;
	}

	public void ClearMineral(int local){
		ArmyManager.Instance.getArmy(minerals[local].armyId).state = 0;
		minerals[local] = null;
	}
    PrizeSample[] fightPrizes;

    public PrizeSample[] FightPrizes {
        get { return fightPrizes; }
        set { fightPrizes = value; }
    }

    public void ClearFightPrizes() {
        fightPrizes = null;
    }

    public bool HaveFightPrizes() {
        if (fightPrizes != null && fightPrizes.Length > 0) {
            return true;
        }
        return false;
    }

    int newEnemyNum = 0;

    public int NewEnemyNum {
        get { return newEnemyNum; }
        set { newEnemyNum = value; }
    }

    int searchTimes = 0;

    public int SearchTimes {
        get { return searchTimes; }
        set { searchTimes = value; }
    }

    public string GetSearchMineralConsume() {
     string[] consumeArr =   CommandConfigManager.Instance.GetMiningSearchConsume();
     if (searchTimes > consumeArr.Length -1)
         searchTimes = consumeArr.Length -1;
     string consume =  consumeArr[SearchTimes];
     return consume;
    }

   


}
