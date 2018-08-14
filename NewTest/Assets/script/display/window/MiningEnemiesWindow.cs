using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MiningEnemiesWindow : WindowBase {

	public MineralEnemyContent content;
	public GameObject enemyPanel;
	public GameObject chooseMineralPanel;
	public ButtonBase M0;
	public ButtonBase M1;
	//断线重新连接
	public override void OnNetResume () {
		base.OnNetResume ();

	}

	protected override void begin () {
		base.begin ();
		updateUI ();
		MaskWindow.UnlockUI();
	}
	/** 更新UI */
	public void updateUI () {
		List<PillageEnemyInfo> enemyList = MiningManagement.Instance.GetEnemyInfoList ();
		content.reLoad (enemyList.ToArray ());
		M0.disableButton (true);
		M0.textLabel.text = LanguageConfigManager.Instance.getLanguage("mining_none");
		M1.disableButton (true);
		M1.textLabel.text =  LanguageConfigManager.Instance.getLanguage("mining_none");
	}

	public override void buttonEventBase (GameObject gameObj) {   
		if (gameObj.name == "close") {
			finishWindow ();
		}	
		if (gameObj.name == "back") {

			enemyPanel.GetComponent<UITweener> ().onFinished.Clear ();
			chooseMineralPanel.GetComponent<UITweener> ().SetOnFinished (() => {
				enemyPanel.GetComponent<UITweener> ().Play (false);
			});
			chooseMineralPanel.GetComponent<UITweener> ().Play (true);
		}	

		if (gameObj.name == "button_fight") {
                chooseMineralPanel.GetComponent<UITweener>().onFinished.Clear();
                enemyPanel.GetComponent<UITweener>().SetOnFinished(() =>
                {
                    chooseMineralPanel.GetComponent<UITweener>().Play(false);
                });

                Hashtable ex = gameObj.GetComponent<ButtonBase>().exFields;
                uid = ex["roleUid"].ToString();
                SetMineral(uid);
                enemyPanel.GetComponent<UITweener>().Play(true);
		}

		if (gameObj.name == "button_replay") {
			MaskWindow.instance.setServerReportWait(true);
			GameManager.Instance.battleReportCallback=GameManager.Instance.intoBattleNoSwitchWindow;
			Hashtable ex = gameObj.GetComponent<ButtonBase>().exFields;
			uid = ex ["roleUid"].ToString ();
			string servername =  ex ["serverName"].ToString ();
            long time = (long)ex["time"];
            FPortManager.Instance.getFPort<GetMineralReportFport>().access(servername, uid, time, CloseWindow);
		}

        if (gameObj.name == "M0" || gameObj.name == "M1")
        {
                string local = gameObj.GetComponent<ButtonBase>().exFields["local"].ToString();
                string node = MiningManagement.Instance.GetEnemyInfoByRoleUid(uid).node;
                FPortManager.Instance.getFPort<ShowEnemyMineralInfoFport>().access(node, uid, local, FightCallback);
		}
		
	}

    void CloseWindow() {
        dialogCloseUnlockUI = false;
        finishWindow();
    }
	void FightCallback(){
		EventDelegate.Add(OnHide,()=>{
			(fatherWindow as MiningWindow).ShowEnemyInfo(true);
			(fatherWindow as MiningWindow).SetWarType(1);
            (fatherWindow as MiningWindow).isSearchEnemy = false;
		});
		finishWindow();
	}


	string uid ;

	//设置矿坑
	void SetMineral (string uid) {

		PillageEnemyInfo info = MiningManagement.Instance.GetEnemyInfoByRoleUid (uid);
		if (info.minerals != null && info.minerals.Count != 0) {
			int i = 0;
			foreach (var tmp in info.minerals) {
				ButtonBase button;
				if (i == 0) {
					button = M0;
				}
				else {
					button = M1;
				}

				int type = MiningManagement.Instance.GetMiningSampleBySid (tmp.Value).type;
				if (type == (int)MiningTypePage.MiningGold) {
					button.GetComponent<UIButton> ().normalSprite = "gold";
					button.textLabel.text = LanguageConfigManager.Instance.getLanguage("mining_gold");
				}
				else {
					button.GetComponent<UIButton> ().normalSprite = "gem";
					button.textLabel.text = LanguageConfigManager.Instance.getLanguage("mining_gem");
				}
				button.exFields = new Hashtable();
				button.exFields["local"] = tmp.Key;
				button.disableButton(false);
				i++;
			}
		}
	}

}
