using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChangeAlternateWindow : WindowBase
{
	public UILabel playerCombat;
	public UILabel alternateCombat;
	public barCtrl player_hpbar;
	public barCtrl alternate_hpbar;
	public barCtrl player_expbar;
	public barCtrl alternate_expbar;
	public UILabel playerCardName;
	public UILabel alternateCardName;
	public UILabel playerHpNum;
	public UILabel alternateHpNum;
	public UILabel playerExpNum;
	public UILabel alternateExpNum;
	public UILabel playerLv;
	public UILabel alternateLv;
	public ButtonBase playerMedicalButton;
	public ButtonBase alternateMedicalButton;
	public ButtonBase switchButton;
	public UITexture playerCardImage;
	public UITexture alternateCardImage;
	public AlternateSwitchCtrl switchCtrl;
	int index;
	BattleFormationCard player;
	BattleFormationCard alternate;
	protected override void begin ()
	{
		base.begin ();
		MaskWindow.UnlockUI ();
	}

	void cleanPlayer ()
	{
		playerMedicalButton.gameObject.SetActive (false);
		playerCardImage.gameObject.SetActive (false);
		playerCardName.text = "";
		player_hpbar.reset ();
		playerHpNum.text = "";
		playerExpNum.text = "";
		playerLv.text = "";
		playerCombat.text = "";
		player_expbar.reset ();
	}
	
	void cleanalternate ()
	{
		alternateMedicalButton.gameObject.SetActive (false);
		alternateCardImage.gameObject.SetActive (false);
		alternateCardName.text = "";
		alternate_hpbar.reset ();
		alternateHpNum.text = "";
		alternateExpNum.text = "";
		alternateLv.text = "";
		alternateCombat.text = "";
		alternate_expbar.reset ();	
	}

	public void Initialize (BattleFormationCard main, BattleFormationCard sub, int teamIndex,List<int> ids)
	{
		index = teamIndex;
		player = main;
		alternate = sub;
		
		//初始化显示数据
		cleanPlayer ();
		cleanalternate ();
		
		updatePlayerInfo ();
		updateAlternateInfo ();
		
		
		if (fatherWindow.GetType () == typeof(BattlePrepareWindow)) {
			if ((fatherWindow as BattlePrepareWindow).getBattleMode () == BattleType.BATTLE_SUBSTITUTE) {
				if (sub == null) {
					switchButton.disableButton (true);
				} else {
					switchButton.disableButton (false);	
				}
			}
			if(index<5){
				if(!ids.Contains(index+1))switchButton.disableButton (true);
				else switchButton.disableButton (false);
			}
		}
		
	}

	void updateAlternateInfo ()
	{
			
		if (alternate != null) {
			alternateCardName.text = alternate.card.getName ();
			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.CARDIMAGEPATH + alternate.card.getImageID () , alternateCardImage);
		 
			alternate_hpbar.updateValue (alternate.getHp (), alternate.getHpMax ());
			int per = (int)(alternate.getHp () * 100 / alternate.getHpMax ());
			if (per == 0)
				per++;
			alternateHpNum.text = per + "%"; 
			alternateLv.text ="Lv." + alternate.getLevel () + "";
			alternateCombat.text = alternate.card.getCardCombat() + "";
//			alternate_expbar.updateValue (alternate.card.getEXP (), alternate.card.getEXPUp ());
			alternate_expbar.updateValue (EXPSampleManager.Instance.getNowEXPShow(alternate.card.getEXPSid(),alternate.card.getEXP())
				,EXPSampleManager.Instance.getMaxEXPShow(alternate.card.getEXPSid(),alternate.card.getEXP()));
			
			per = (int)(EXPSampleManager.Instance.getNowEXPShow(alternate.card.getEXPSid(),alternate.card.getEXP()) * 100 / EXPSampleManager.Instance.getMaxEXPShow(alternate.card.getEXPSid(),alternate.card.getEXP()));
			if(per == 0)
				per ++;
			alternateExpNum.text = per + "%";
			//0<hp<max 
			alternateMedicalButton.gameObject.SetActive (true);
			if (alternate.getHp () == alternate.getHpMax ()) {
				alternateMedicalButton.gameObject.SetActive (false);
			}
			if (alternate.getHp () == 0)
				alternateMedicalButton.textLabel.text = LanguageConfigManager.Instance.getLanguage ("s0185");
			else
				alternateMedicalButton.textLabel.text = LanguageConfigManager.Instance.getLanguage ("s0184"); 
			 
		} else{
			alternateCardImage.gameObject.SetActive(false);
			alternateHpNum.text ="";
			alternateExpNum.text ="";
			alternateLv.text = "";
			alternate_expbar.reset();
			alternate_hpbar.reset();
			alternateCardName.text ="";
			alternateCombat.text = "";
		}
	}

	void updatePlayerInfo ()
	{
		if (player != null) {
			//name
			playerCardName.text = player.card.getName ();
			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.CARDIMAGEPATH + player.card.getImageID (), playerCardImage);

			 
			player_hpbar.updateValue (player.getHp (), player.getHpMax ()); 
			int per = (int)(player.getHp () * 100 / player.getHpMax ());
			if (per == 0)
				per++;
			playerHpNum.text = per + "%"; 
		
			playerLv.text = "Lv."+player.getLevel () + "";
			playerCombat.text = player.card.getCardCombat() + "";
			//exp
//			player_expbar.updateValue (player.card.getEXP (), player.card.getEXPUp ());
			player_expbar.updateValue (EXPSampleManager.Instance.getNowEXPShow(player.card.getEXPSid(),player.card.getEXP())
				,EXPSampleManager.Instance.getMaxEXPShow(player.card.getEXPSid(),player.card.getEXP()));
			per = (int)(EXPSampleManager.Instance.getNowEXPShow(player.card.getEXPSid(),player.card.getEXP()) * 100 / EXPSampleManager.Instance.getMaxEXPShow(player.card.getEXPSid(),player.card.getEXP()));
			if(per == 0)
				per ++;
			playerExpNum.text = per + "%";
		 
		//	playerMedicalButton.gameObject.SetActive (true); //功能未开放
//			if (player.getHp () == player.getHpMax ()) {
//				playerMedicalButton.gameObject.SetActive (false);
//			}
			if (player.getHp () == 0)
				playerMedicalButton.textLabel.text = LanguageConfigManager.Instance.getLanguage ("s0185");
			else
				playerMedicalButton.textLabel.text = LanguageConfigManager.Instance.getLanguage ("s0184");
			 
	
		} else{
			playerCardImage.gameObject.SetActive(false);
			playerHpNum.text ="";
			playerExpNum.text ="";
			playerLv.text = "";
			player_expbar.reset();
			player_hpbar.reset();
			playerCardName.text="";
			playerCombat.text = "";
		}
	}
	
	//复活
	private void doRebirth (string[] arr)
	{ 
		if (arr .Length < 1)
			return ;
		FuBenRebirthFPort port = FPortManager.Instance.getFPort ("FuBenRebirthFPort") as FuBenRebirthFPort;
		port.rebirth (arr, rebirthAndRecoverCallBack);
	}
	 
	//加血
	private void doRecover (string[] arr)
	{ 
		if (arr .Length < 1)
			return ;
		FuBenRecoverFPort port = FPortManager.Instance.getFPort ("FuBenRecoverFPort") as FuBenRecoverFPort;
		port.recover (arr, rebirthAndRecoverCallBack); 
	}
	
	private void rebirthAndRecoverCallBack (string[] ids)
	{

		int max = ids.Length;
		for (int i = 0; i < max; i++) {
			for (int j = 0; j < MissionInfoManager.Instance.mission.mine.Length; j++) {
				if (MissionInfoManager.Instance.mission.mine [j] != null && MissionInfoManager.Instance.mission.mine [j].card.uid == ids [i]) {
					MissionInfoManager.Instance.mission.mine [j].setHp (-1);//-1代表满血
					MissionInfoManager.Instance.mission.mine [j].setHpMax (-1);
				}
			}
		}
		updatePlayerInfo ();
		updateAlternateInfo ();
	}
	
	bool checkSwitch (BattleFormationCard main, BattleFormationCard sub, Army savingArmy)
	{
		//空置超过4位,禁止交换
		int emptyCount = 0;
		for (int i=0; i<savingArmy.players.Length; i++) {
			
			if (savingArmy.players [i] == "0") {
				emptyCount += 1;
			}
		}
		if (emptyCount >= 4) {
			if (sub == null) {
				UiManager.Instance.openDialogWindow<MessageWindow>((win)=>{
					win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0093"), "", LanguageConfigManager.Instance.getLanguage ("s0069"), null);
				});
				return false;
			}
		}
		
		return true;
	}

	void switchOver ()
	{

		BattleFormationCard tmp = player;
		player = alternate;
		alternate = tmp;
		
		updateAlternateInfo ();
		updatePlayerInfo ();
		
		MaskWindow.UnlockUI();
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
	
		if (gameObj.name == "switchButton") {
//			if (fatherWindow.GetType () == typeof(TeamViewInMissionWindow)) {
//				if (checkSwitch (player, alternate, (fatherWindow as TeamViewInMissionWindow).savingArmy) == false)
//					return;
//				
//				if(player!=null && alternate!=null)
//				switchCtrl.beginSwitch (playerCardImage.mainTexture, alternateCardImage.mainTexture, switchOver);
//				
//				if(player!=null && alternate==null)
//				switchCtrl.beginSwitch (playerCardImage.mainTexture, null, switchOver);			
//				
//				if(player==null && alternate!=null)
//				switchCtrl.beginSwitch (null, alternateCardImage.mainTexture, switchOver);
//				
//				playerCardImage.gameObject.SetActive (false);
//				alternateCardImage.gameObject.SetActive (false);
//			}
			if (UserManager.Instance.self.getUserLevel() < TeamEditWindow.SUBLV) {
				MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("FUNMSG",TeamEditWindow.SUBLV.ToString()));
				return;
			}
			if (player != null && player.card.isMainCard()) {
				TextTipWindow.Show(LanguageConfigManager.Instance.getLanguage("teamEdit_err01"));
				return;
			}
			if (fatherWindow.GetType () == typeof(BattlePrepareWindow)) {
				if (checkSwitch (player, alternate, (fatherWindow as BattlePrepareWindow).savingArmy) == false)
					return;
				
				if(player!=null && alternate!=null)
				switchCtrl.beginSwitch (playerCardImage.mainTexture, alternateCardImage.mainTexture, switchOver);
				
				if(player!=null && alternate==null)
				switchCtrl.beginSwitch (playerCardImage.mainTexture, null, switchOver);			
				
				if(player==null && alternate!=null)
				switchCtrl.beginSwitch (null, alternateCardImage.mainTexture, switchOver);

				playerCardImage.gameObject.SetActive (false);
				alternateCardImage.gameObject.SetActive (false);
			}
			
		}else if (gameObj.name == "close") {
			if (fatherWindow.GetType () == typeof(TeamViewInMissionWindow)) {
				(fatherWindow as TeamViewInMissionWindow).switchParter (player, alternate, index);
			}
			if (fatherWindow.GetType () == typeof(BattlePrepareWindow)) {
				(fatherWindow as BattlePrepareWindow).switchParter (player, alternate, index);	
			}
			MaskWindow.UnlockUI();
			finishWindow();
		}else if (gameObj.GetInstanceID () == playerMedicalButton.gameObject.GetInstanceID ()) {
			string[] ids = new string[1];
			ids [0] = player.card.uid;
			 UiManager.Instance.openWindow<PropUseInChangeAlternateWindow>(
				(win)=>{
			if (player.getHp () == 0)
				win.Initialize (PropType.PROP_REBIRTH, PropUseInChangeAlternateWindow.SINGLEREBIRTH, ids, doRebirth);
			else
				win.Initialize (PropType.PROP_RECOVER, PropUseInChangeAlternateWindow.SINGLEREST, ids, doRecover); 

				});
		}else if (gameObj.GetInstanceID () == alternateMedicalButton.gameObject.GetInstanceID ()) {
			string[] ids = new string[1];
			ids [0] = alternate.card.uid;
			UiManager.Instance.openWindow<PropUseInChangeAlternateWindow>(
				(win)=>{
			if (alternate.getHp () == 0)
				win.Initialize (PropType.PROP_REBIRTH, PropUseInChangeAlternateWindow.SINGLEREBIRTH, ids, doRebirth);
			else
				win.Initialize (PropType.PROP_RECOVER, PropUseInChangeAlternateWindow.SINGLEREST, ids, doRecover);
			});
		}		
	}
}
