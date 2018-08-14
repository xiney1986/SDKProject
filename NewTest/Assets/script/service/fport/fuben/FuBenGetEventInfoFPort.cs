using System;
using System.Collections.Generic;
 
/**
 * 获得副本战前事件信息接口
 * @author longlingquan
 * */
public class FuBenGetEventInfoFPort:BaseFPort
{
	public FuBenGetEventInfoFPort ()
	{
		
	}
	
	private CallBack callback;
	
	public void getInfo (CallBack callback)
	{
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.FUBEN_GET_EVENT_INFO);  
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{
		ErlType type = message.getValue ("msg") as ErlType;		 
		ErlArray mine = (type as ErlArray).Value [0] as ErlArray;
		ErlArray enemy = (type as ErlArray).Value [1] as ErlArray;
		 
		BattleFormationCard[] mineCards = parseBattleFormation (mine, true);
		BattleFormationCard[] enemyCards = parseBattleFormation (enemy, false); 
		MissionInfoManager.Instance.mission.mine = mineCards;
		MissionInfoManager.Instance.mission.enemy = enemyCards; 
		if (callback != null)
			callback ();
	}
	
	//解析战前阵型信息
	private BattleFormationCard[] parseBattleFormation (ErlArray arr, bool isMine)
	{
		//只有是自己阵型才可以为空 
		if (arr.Value.Length < 1) {
			if (isMine) {
				return parseMyCards (null);
			} else 
				return null; 
		}
		BattleFormation[] bfs;
		if (!isMine) {
			int max = 15;
			bfs = new BattleFormation[15];
			for (int i = 0; i < max; i++) {
				ErlArray erlArr = arr.Value [i] as ErlArray;
				if (erlArr == null || erlArr.Value.Length < 1)
					continue;
				ErlArray array1 = erlArr.Value [0] as ErlArray;//主力 如果erlArr 数组长度大于1 则后面的部分是替补
			 
				if (array1 != null || array1.Value.Length < 1) {
					bfs [i] = new BattleFormation ();
					bfs [i].id = array1.Value [0].getValueString ();
					bfs [i].hp = StringKit.toInt (array1.Value [1].getValueString ());
					bfs [i].maxHp = StringKit.toInt (array1.Value [2].getValueString ());
					bfs [i].lv = StringKit.toInt (array1.Value [3].getValueString ());
				}
			}
		} else {
			int max = arr.Value.Length;
			bfs = new BattleFormation[max];
			for (int i = 0; i < max; i++) {
				ErlArray erlArr = arr.Value [i] as ErlArray;
		   
				bfs [i] = new BattleFormation ();
				bfs [i].id = erlArr.Value [0].getValueString ();
				bfs [i].hp = StringKit.toInt (erlArr.Value [1].getValueString ());
				bfs [i].maxHp = StringKit.toInt (erlArr.Value [2].getValueString ());  
			}
		}
		if (isMine) {
			return parseMyCards (bfs);
		}
		return parseEnemycards (bfs); 
	}
	
	//获得当前上阵卡片信息
	private BattleFormationCard[] parseMyCards (BattleFormation[] bfs)
	{ 
		BattleFormationCard[] bfCards; 
		bfCards = new BattleFormationCard[10];
		for (int i = 0; i < 5; i++) {
			
			Card card1 = StorageManagerment.Instance.getRole (ArmyManager.Instance.getActiveArmy ().players [i]);
			if (card1 != null) {
				bfCards [i] = new BattleFormationCard ();
				bfCards [i].card = card1;
				bfCards [i].loc = FormationManagerment.Instance.getLoctionByIndex (ArmyManager.Instance.getActiveArmy ().formationID, i); 
			}
			
			Card card2 = StorageManagerment.Instance.getRole (ArmyManager.Instance.getActiveArmy ().alternate [i]);
			if (card2 != null) {
				bfCards [i + 5] = new BattleFormationCard ();
				bfCards [i + 5].card = card2; 
				bfCards [i + 5].loc = FormationManagerment.Instance.getLoctionByIndex (ArmyManager.Instance.getActiveArmy ().formationID, i);
			}
		}  


		if (bfs != null) {
			for (int i = 0; i < bfCards.Length; i++) {
				for (int j = 0; j < bfs.Length; j++) {
					if (bfCards [i] != null && bfCards [i].card.uid == bfs [j].id + "") {
						bfCards [i].setHp (bfs [j].hp);
						bfCards [i].setHpMax (bfs [j].maxHp);
						bfCards [i].setLevel (bfs [j].lv);
					}
				}
			}
		}  
		return bfCards; 
	}
	
	//获得敌方卡片信息
	private BattleFormationCard[] parseEnemycards (BattleFormation[] bfs)
	{ 
		List<BattleFormationCard> list = new List<BattleFormationCard> ();
		for (int i = 0; i < bfs.Length; i++) {
			if (bfs [i] != null) {
				BattleFormationCard bfc = new BattleFormationCard ();
				bfc.card = CardManagerment.Instance.createCard (StringKit.toInt(bfs [i].id));
				bfc.setLevel (bfs [i].lv);
				bfc.loc = i  + 1;
				bfc.setHp (bfs [i].hp);
				bfc.setHpMax (bfs [i].maxHp);
                list.Add(bfc);
			}
		}
        for (int k = 0; k < list.Count -1; k++) {
            for (int j = k + 1; j < list.Count; j++) {
                BattleFormationCard tempBfc = list[k];
                BattleFormationCard tmpBFC = list[j];
                if ((list[k].loc - 1) % 5 + 1 > (tmpBFC.loc - 1) % 5 + 1) {
                    list[k] = tmpBFC;
                    list[j] = tempBfc;
                }
            }
        }
        return list.ToArray();
	}
	
}  