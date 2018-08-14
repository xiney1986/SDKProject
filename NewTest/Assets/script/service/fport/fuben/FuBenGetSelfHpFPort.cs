using System;
using System.Collections.Generic;
 
/**
 * 获得副本当前卡片血量
 * @author longlingquan
 * */
public class FuBenGetSelfHpFPort:BaseFPort
{
	public FuBenGetSelfHpFPort ()
	{
		
	}
	 
	private CallBack callback;
    private CallBack<BattleFormationCard[]> call;
    private bool flag=false;
	
	public void getInfo (CallBack callback)
	{ 
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.FUBEN_GET_SELF_HP);  
		access (message);
	}
    public void getInfobyOut(CallBack<BattleFormationCard[]> callback, bool bo) {
        this.call = callback;
        this.flag=bo;
        ErlKVMessage message = new ErlKVMessage(FrontPort.FUBEN_GET_SELF_HP);
        access(message);
    }
	
	public override void read (ErlKVMessage message)
	{
		ErlType type = message.getValue ("msg") as ErlType;
		ErlArray mine = (type as ErlArray) as ErlArray;  
		if(!flag)
            MissionInfoManager.Instance.mission.mine=parseBattleFormation (mine);
        else {
            if (call != null)
                call(parseBattleFormation(mine));
            flag = false;
            call = null;
        }
		if (callback != null)
			callback ();
        
	}
	
	//解析战前阵型信息
	private BattleFormationCard[] parseBattleFormation (ErlArray arr)
	{
		
		//只有是自己阵型才可以为空 
		if (arr == null || arr.Value.Length < 1) {
            if (flag) return null;
			return parseMyCards (null); 
		}
		int max = arr.Value.Length;
		BattleFormation[] bfs = new BattleFormation[max];
		for (int i = 0; i < max; i++) {
			ErlArray erlArr = arr.Value [i] as ErlArray;
		   
			bfs [i] = new BattleFormation ();
			bfs [i].id = erlArr.Value [0].getValueString ();
			bfs [i].hp = StringKit.toInt (erlArr.Value [1].getValueString ());
			bfs [i].maxHp = StringKit.toInt (erlArr.Value [2].getValueString ());  
		}
		return parseMyCards (bfs);
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
				bfCards [i + 5].card = StorageManagerment.Instance.getRole (ArmyManager.Instance.getActiveArmy ().alternate [i]); 
				bfCards [i + 5].loc = FormationManagerment.Instance.getLoctionByIndex (ArmyManager.Instance.getActiveArmy ().formationID, i); 
			}
		}


		if (bfs != null) {
			for (int i = 0; i < bfCards.Length; i++) {
				for (int j = 0; j < bfs.Length; j++) {
					if (bfCards [i] != null && bfCards [i].card.uid == bfs [j].id + "") {
						bfCards [i].setHp(bfs [j].hp);
						bfCards [i].setHpMax(bfs [j].maxHp);
						bfCards [i].setLevel (bfs [j].lv);
					}
				}
			}
		}  
		return bfCards; 
	}  
}  

