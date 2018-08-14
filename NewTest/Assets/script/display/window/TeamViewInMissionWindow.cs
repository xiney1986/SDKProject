using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//副本队伍查看敞口
public class TeamViewInMissionWindow : WindowBase
{
	public RoleView[] teamCardButton;//正式队员
	public RoleView[] teamSubstituteButton;//替补列表
	public barCtrl[] playerHps;
    public barCtrl[] subHps;
	public GameObject[] swapButtons;
	public ButtonBase buttonOneKeyRest;
	public ButtonBase buttonOneKeyRebirth;
	public Army savingArmy;
	List<int> idds=new List<int>();

	private void saveArmy ()
	{  
		//相同直接关闭窗口不保存
		if (ArmyManager.Instance.compareArmy (savingArmy, ArmyManager.Instance.getActiveArmy ())) {
			close ();
			return;
		}  
		List<string> oldCards = ArmyManager.Instance.getAllArmyCardsExceptMining ();//改变队伍前的卡片上阵情况
		List<string> oldBeasts = ArmyManager.Instance.getFightBeasts ();//改变队伍前的召唤兽情况
		Army[] arr = new Army[1]{savingArmy};
		ArmyUpdateFPort port = FPortManager.Instance.getFPort ("ArmyUpdateFPort") as ArmyUpdateFPort;
		port.access (arr, () => {
			saveArmyBack ();
			ArmyManager.Instance.updateBackChangeState (oldCards, oldBeasts,1);
		}); 
	}

	void close ()
	{
		finishWindow ();
	}

	void saveArmyBack ()
	{
		ArmyManager.Instance.updateArmy (savingArmy.armyid, savingArmy);
		ArmyManager.Instance.recalculateAllArmyIds ();
		close ();
	}

	public void switchParter (BattleFormationCard main, BattleFormationCard sub, int index)
	{ 
		//主卡不空,判断下主卡是否相同
		if (main != null) {
			if (savingArmy.players [index] != main.card.uid) {
				savingArmy.players [index] = main.card.uid;
			}
		} else {
			savingArmy.players [index] = "0";
		}
		//找出替补卡
		if (sub != null) {
			if (savingArmy.alternate [index] != sub.card.uid) {
				savingArmy.alternate [index] = sub.card.uid;
			}
		} else {
			savingArmy.alternate [index] = "0";
		}
	}
	protected override void begin ()
	{ 
		base.begin ();
		TeamEmtpyInfoFPort fport = FPortManager.Instance.getFPort<TeamEmtpyInfoFPort> ();
        fport.access(getSelfHp);

	}
    void getSelfHp(List<int> ids) {
        idds = ids;
        FuBenGetSelfHpFPort fport = FPortManager.Instance.getFPort<FuBenGetSelfHpFPort>();
        fport.getInfo(updateUI);
    }
    void updateUI() {
        if (savingArmy == null) {
            //复制用于编辑阵形
            savingArmy = ArmyManager.Instance.DeepClone(ArmyManager.Instance.getActiveArmy());
            reLoadTeam();
            MaskWindow.UnlockUI();
        } else {
            MaskWindow.UnlockUI();
            return;
        }
    }
	private void reLoadTeam ()
	{
		//获得正式队员   
		for (int i=0; i<5; i++) {
			if(idds.Contains(i+1)){
				swapButtons[i].SetActive(true);
			}
			else swapButtons[i].SetActive(false);
			//当3人或者4人阵,移除一些座位
			if (ArmyManager.Instance.getActiveArmy ().formationID == 1) {
				if (i == 0 || i == 4) {
					continue;
				}
			}
			if (ArmyManager.Instance.getActiveArmy ().formationID == 2) {
				if (i == 4) {
					continue;
				}
			}	
			teamCardButton [i].hideInBattle = true;
			if (MissionInfoManager.Instance.mission.mine [i] == null) {
				if (MissionInfoManager.Instance.mission.mine [i + 5] != null) {
					teamCardButton [i].gameObject.SetActive (true);
					SetRoleViewActiveNotBg (teamCardButton [i], false);
				}
				continue;
			}
			teamCardButton [i].gameObject.SetActive (true);
            float max=(float)MissionInfoManager.Instance.mission.mine[i].getHpMax();
            float currect = (float)MissionInfoManager.Instance.mission.mine[i].getHp();
            playerHps[i].updateValue(currect,max);
			teamCardButton [i].init (MissionInfoManager.Instance.mission.mine [i].card, this, (role)=>{
				CardBookWindow.Show (role.card, CardBookWindow.VIEW, null);
			});
		}
		for (int i=0; i<5; i++) {
			if(idds.Contains(i+1)){
				swapButtons[i].SetActive(true);
			}
			else swapButtons[i].SetActive(false);
			//当3人或者4人阵,移除一些座位
			if (ArmyManager.Instance.getActiveArmy ().formationID == 1) {
				if (i == 0 || i == 4) {
					continue;
				}
			}
			if (ArmyManager.Instance.getActiveArmy ().formationID == 2) {
				if (i == 4) {
					continue;
				}
			}	
			teamSubstituteButton [i].hideInBattle = true;
			if (MissionInfoManager.Instance.mission.mine [i + 5] == null) {
				if (MissionInfoManager.Instance.mission.mine [i] != null) {
					teamSubstituteButton [i].gameObject.SetActive (true);
					SetRoleViewActiveNotBg (teamSubstituteButton [i], false);
				}
				continue;
			}
			teamSubstituteButton [i].gameObject.SetActive (true);
            float tmax = (float)MissionInfoManager.Instance.mission.mine[i+5].getHpMax();
            float tcurrect = (float)MissionInfoManager.Instance.mission.mine[i+5].getHp();

            subHps[i].updateValue(tcurrect, tmax);
			teamSubstituteButton [i].init (MissionInfoManager.Instance.mission.mine [i + 5].card, this, (role)=>{
				CardBookWindow.Show (role.card, CardBookWindow.VIEW, null);
			});
		}	
		if (getRecoverCardIds ().Length < 1)
			buttonOneKeyRest.disableButton (true);
		else
			buttonOneKeyRest.disableButton (false);
		
		if (getRebirthCardIds () .Length < 1)
			buttonOneKeyRebirth.disableButton (true);
		else
			buttonOneKeyRebirth.disableButton (false);  
	}
	//一键复活
	private void doAllRebirth (string[] arr)
	{ 
		if (arr .Length < 1)
			return;
		FuBenRebirthFPort port = FPortManager.Instance.getFPort ("FuBenRebirthFPort") as FuBenRebirthFPort;
		port.rebirth (arr, rebirthAndRecoverCallBack);
	}
	 
	//一键加血
	private void doAllRecover (string[] arr)
	{ 
		if (arr .Length < 1)
			return;
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
		reLoadTeam (); 
	}
	
	//获得需要复活的卡片信息
	private string[] getRebirthCardIds ()
	{
		if (MissionInfoManager.Instance.mission.mine == null)
			return null;
		List<string> list = new List<string> ();
		int max = MissionInfoManager.Instance.mission.mine.Length;
		for (int i = 0; i < max; i++) {
			if (MissionInfoManager.Instance.mission.mine [i] != null && MissionInfoManager.Instance.mission.mine [i].getHp () == 0)
				list.Add (MissionInfoManager.Instance.mission.mine [i].card.uid);
		}
		return list.ToArray ();
	}
	
	//获得需要加血的卡片信息
	private string[] getRecoverCardIds ()
	{
		if (MissionInfoManager.Instance.mission.mine == null)
			return null;
		List<string> list = new List<string> ();
		int max = MissionInfoManager.Instance.mission.mine.Length;
		for (int i = 0; i < max; i++) {
			if (MissionInfoManager.Instance.mission.mine [i] != null && MissionInfoManager.Instance.mission.mine [i].getHp () > 0 && MissionInfoManager.Instance.mission.mine [i].getHp () != MissionInfoManager.Instance.mission.mine [i].getHpMax ()) 
				list.Add (MissionInfoManager.Instance.mission.mine [i].card.uid);
		}
		return list.ToArray ();
	}

	protected override void DoEnable ()
	{
		//2014.7.2 9:50 modified
		base.DoEnable ();
		UiManager.Instance.backGround.switchBackGround ("ChouJiang_BeiJing");
		//UiManager.Instance.backGroundWindow.switchToDark();
		if (MissionManager.instance != null)
			MissionManager.instance.hideAll ();
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);

		if (gameObj.name == "close") {	
			saveArmy (); 
			if (MissionManager.instance != null)
			{
				MissionManager.instance.showAll ();
				MissionManager.instance.setBackGround();
			}
			MaskWindow.UnlockUI ();
		}else if (gameObj.name == "buttonOneKeyRebirth") {	
			UiManager.Instance.openWindow<PropUseInChangeAlternateWindow> ((win) => {
				win.Initialize (PropType.PROP_REBIRTH, PropUseInChangeAlternateWindow.ONEKEYREBIRTH, getRebirthCardIds (), doAllRebirth);
			});
		}else if (gameObj.name == "buttonOneKeyRest") {
			UiManager.Instance.openWindow<PropUseInChangeAlternateWindow> ((win) => {
				win.Initialize (PropType.PROP_RECOVER, PropUseInChangeAlternateWindow.ONEKEYREST, getRecoverCardIds (), doAllRecover); 
			});
		}else if(gameObj.name=="1"||gameObj.name=="2"||gameObj.name=="3"||gameObj.name=="4"||gameObj.name=="5"){
			OnSwapButtonClick(gameObj);
		}		  
	}

	public void OnSwapButtonClick (GameObject obj)
	{
		int index = StringKit.toInt (obj.name) - 1;

		if (UserManager.Instance.self.getUserLevel () < TeamEditWindow.SUBLV) {
			MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("FUNMSG", TeamEditWindow.SUBLV.ToString ()));
			return;
		}
		//至少保留一名上阵英雄
		if (savingArmy.getPlayerNum () == 1 && MissionInfoManager.Instance.mission.mine [index + 5] == null) {
			MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("s0390"));
			return;
		}

		RoleView main = teamCardButton [index];
		RoleView sub = teamSubstituteButton [index];

		if (main.card != null && main.card.isMainCard ()) {
			MaskWindow.UnlockUI ();
			TextTipWindow.Show(LanguageConfigManager.Instance.getLanguage("teamEdit_err01"));
			return;
		} else if (sub.card != null && sub.card.isMainCard ()) {
			MaskWindow.UnlockUI ();
			TextTipWindow.Show(LanguageConfigManager.Instance.getLanguage("teamEdit_err01"));
			return;
		}
		if (main.card == null) {
			SetRoleViewActiveNotBg (main, true);
			SetRoleViewActiveNotBg (sub, false);
		} else if (sub.card == null) {
			SetRoleViewActiveNotBg (main, false);
			SetRoleViewActiveNotBg (sub, true); 
		}

		string id = savingArmy.players [index];
		savingArmy.players [index] = savingArmy.alternate [index];
		savingArmy.alternate [index] = id;


		Card c = main.card;
        main.init(sub.card, this, (role) => {
            CardBookWindow.Show(role.card, CardBookWindow.VIEW, null);
        });
        sub.init(c, this, (role) => {
            CardBookWindow.Show(role.card, CardBookWindow.VIEW, null);
        });
		if (main.card == null)
			main.qualityBg.spriteName = "roleBack_0";
		else if (sub.card == null)
			sub.qualityBg.spriteName = "roleBack_0";

		BattleFormationCard bfc = MissionInfoManager.Instance.mission.mine [index];
		MissionInfoManager.Instance.mission.mine [index] = MissionInfoManager.Instance.mission.mine [index + 5];
		MissionInfoManager.Instance.mission.mine [index + 5] = bfc;
        if (MissionInfoManager.Instance.mission.mine[index + 5]!=null) {
            float tmax = (float)MissionInfoManager.Instance.mission.mine[index + 5].getHpMax();
            float tcurrect = (float)MissionInfoManager.Instance.mission.mine[index + 5].getHp();
            subHps[index].updateValue(tcurrect, tmax);
        }
        if (MissionInfoManager.Instance.mission.mine[index]!=null) {
            float max = (float)MissionInfoManager.Instance.mission.mine[index].getHpMax();
            float currect = (float)MissionInfoManager.Instance.mission.mine[index].getHp();
            playerHps[index].updateValue(currect, max);
        }
		TweenPosition.Begin (main.gameObject, 0.2f, main.transform.localPosition).from = new Vector3 (sub.transform.localPosition.x, sub.transform.parent.localPosition.y - main.transform.parent.localPosition.y, 0);
		TweenPosition.Begin (sub.gameObject, 0.2f, sub.transform.localPosition).from = new Vector3 (main.transform.localPosition.x, main.transform.parent.localPosition.y - sub.transform.parent.localPosition.y, 0);
		StartCoroutine (Utils.DelayRun (()=>{
			MaskWindow.UnlockUI ();
		},0.2f));
	}

	private void SetRoleViewActiveNotBg (RoleView view, bool state)
	{
		int count = view.transform.childCount;
		for (int i = 0; i < count; i++) {
			Transform trans = view.transform.GetChild (i);
			trans.gameObject.SetActive (state || trans.gameObject.name == "quality_bg");
		}
	}


}
