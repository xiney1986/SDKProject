using UnityEngine;
using System.Collections;

public class battleTypeBarCtrl : MonoBehaviour{
	public GameObject battleTenEffect;//10战(血战到底)特效
	public GameObject battleSubEffect;//替补战特效
	// Use this for initialization
	public 	void init ()
	{
		if (BattleManager.battleData.battleType == BattleType.BATTLE_TEN && !BattleManager.battleData.isLastBattle &&!BattleManager.battleData.isLastBattleBossBattle) {
			
			battleTenEffect.SetActive(true);
			
		} else if (BattleManager.battleData.battleType == BattleType.BATTLE_SUBSTITUTE && !BattleManager.battleData.isLastBattle &&!BattleManager.battleData.isLastBattleBossBattle) {
			
			battleSubEffect.SetActive(true);
		}

		//只回调用
		iTween.ValueTo (gameObject, iTween.Hash ("from", 1, "to", 0,"onupdate","", "easetype", iTween.EaseType.easeInOutCubic,  "oncomplete", "buffComplete", "time", 2f  ));
	 
	}

	void buffComplete ()
	{
		battleTenEffect.SetActive(false);
		battleSubEffect.SetActive(false);
	}

}
