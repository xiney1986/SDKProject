using UnityEngine;
using System.Collections;

/**
 * PVP结果窗口
 * @author 汤琦
 * */
public class PvpResultWindow : WindowBase
{
	public UITexture resultIcon;//连胜图标
	public UILabel resultLabel;//还剩几场胜利
	public UILabel resultValue;//连胜数量1-3
	public ButtonPvpInfo button;
	public ButtonBase nextButton;
	public ButtonBase friendButton;
	public GameObject cardPrefab;
	private bool isWin;
	private int winNum;
	private string uid;
    public GameObject effectGameObject;
	
	protected override void begin ()
	{ 
		base.begin ();
		PvpOppInfo opp = PvpInfoManagerment.Instance.getCurrentOpp ();
		uid = opp.uid;
		ResourcesManager.Instance.LoadAssetBundleTexture (UserManager.Instance.getIconPath (opp.headIcon), button.headIcon);

		button.initInfo (opp,this);
		loadFormationGB (button, FormationSampleManager.Instance.getFormationSampleBySid (opp.formation).getLength (), button.gameObject);
		CreateFormation (button, opp);

		MaskWindow.UnlockUI ();
	    if (PvpInfoManagerment.Instance.isMs)
	    {
            if (UiManager.Instance.getWindow<BattlePrepareWindowNew>() != null) {
                DestroyImmediate(UiManager.Instance.getWindow<BattlePrepareWindowNew>().gameObject);
            }
            Award award = AwardManagerment.Instance.miaoShaAward == null ? null : Award.mergeAward(AwardManagerment.Instance.miaoShaAward);
            AwardManagerment.Instance.miaoShaAward = null;
	        PvpInfoManagerment.Instance.isMs = false;
            BattleManager.isWaitForBattleData = false;
            EffectManager.Instance.CreateEffectCtrlByCache(effectGameObject.transform, "Effect/UiEffect/Miaosha", null);
            MaskWindow.LockUI();
            StartCoroutine(Utils.DelayRun(() => {
                MaskWindow.UnlockUI();
            }, 2.2f));
            if (award != null)
            {
                StartCoroutine(Utils.DelayRun(() => {
                    UiManager.Instance.openDialogWindow<MKillAwardWindow>((win) => {
                        win.init(award);
                    });
                }, 2f));   
            }

	    }
	}
	
	private void rankBack ()
	{
		if (isWin) { 
			resultIcon.gameObject.SetActive (true);
//			resultLabel.gameObject.SetActive (true);
			setTitle (LanguageConfigManager.Instance.getLanguage ("s0219"));
			if (PvpInfoManagerment.Instance.getCurrentRound () < 3) {
				resultValue.text = PvpInfoManagerment.Instance.getPvpRankInfo ().win + PvpInfoManagerment.Instance.getCurrentRound () + "";
//				resultLabel.text = LanguageConfigManager.Instance.getLanguage ("s0230", (3 - PvpInfoManagerment.Instance.getCurrentRound ()).ToString ());
			} else {
				resultValue.text = PvpInfoManagerment.Instance.getPvpRankInfo ().win + 3 + "";
//				resultLabel.text = LanguageConfigManager.Instance.getLanguage ("s0231");
			}
			StartCoroutine (Utils.DelayRun (() => {
				playEffect (resultValue.gameObject);
			}, 0.8f));
		} else {
			resultValue.gameObject.SetActive (false);
			setTitle (LanguageConfigManager.Instance.getLanguage ("s0220"));
			nextButton.gameObject.SetActive (false);
			friendButton.transform.localPosition = new Vector3 (0, friendButton.transform.localPosition.y, 0);
			resultIcon.gameObject.SetActive (false);
//			resultLabel.text = LanguageConfigManager.Instance.getLanguage ("s0221");
		}
	}

	void playEffect (GameObject obj)
	{
		resultValue.gameObject.SetActive (true);
		TweenScale ts = TweenScale.Begin (obj, 0.3f, new Vector3 (1f, 1f, 1));   
		ts.method = UITweener.Method.EaseIn;
		ts.from = new Vector3 (5, 5, 1);
	}
	
	public void initInfo (bool isWin)
	{
		this.isWin = isWin;

		if (BattleGlobal.pvpMode == EnumPvp.sweep) {
			if (isWin && PvpInfoManagerment.Instance.getCurrentRound () == 3) {
				//SweepManagement.Instance.usePvpNum();
			}
		}
		rankBack ();
	}
	
	//加载阵型对象
	private void loadFormationGB (ButtonPvpInfo button, int formationLength, GameObject root)
	{
		passObj go = FormationManagerment.Instance.getPlayerInfoFormationObj (formationLength);
		go.obj.transform.parent = root.transform;
		go.obj .transform.localPosition = Vector3.zero;
		go.obj .transform.localScale = Vector3.one;
		
		if (go.obj != null) {
			button.formationRoot = go.obj;
			go.obj.transform.localPosition = new Vector3 (0, 185, 0);
		}
		
	}
	
	void CreateFormation (ButtonPvpInfo button, PvpOppInfo info)
	{
		TeamPrepareCardCtrl card;
		CardSample cs;
		Card tmpCard;

		for (int i = 0; i < info.opps.Length; i++) {
			GameObject psObj = NGUITools.AddChild (button.formationRoot.gameObject, cardPrefab);
			if (psObj == null) {
				print ("contentTeamPrepare no res!");
				return;
			}

			card = psObj.GetComponent<TeamPrepareCardCtrl> ();
			cs = CardSampleManager.Instance.getRoleSampleBySid (info.opps [i].sid);
			tmpCard = CardManagerment.Instance.createCard(info.opps [i].sid, info.opps [i].evoLevel, info.opps [i].surLevel);
			//找到对应的阵形点位
			Transform formationPoint = null;
			formationPoint = button.formationRoot.transform .FindChild (FormationManagerment.Instance.getLoctionByIndex (info.formation, info.opps [i].index).ToString ());
			card.transform.position = formationPoint.position;
			card.updateButton (info.opps [i]);
			card.initInfo (info.uid, info.opps [i].uid, null);
			card.fatherWindow = fatherWindow;
		}
	}
	
	public override void DoDisable ()
	{ 
		base.DoDisable ();
		
	}

	//申请好友
	public void applyFriend ()
	{
		FriendsFPort fport = FPortManager.Instance.getFPort ("FriendsFPort") as FriendsFPort;
		fport.applyFriend (uid, applyOk);
	}
	
	//申请后回调
	public void applyOk ()
	{
		friendButton.textLabel.text = LanguageConfigManager.Instance.getLanguage ("FriendAPPLY_already_apply");
		friendButton.disableButton (true);
	}
	
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
	
		if (gameObj.name == "close") {
			FPortManager.Instance.getFPort<PvpGetInfoFPort> ().access ((hasPvp) =>
			{
			    if (BattleManager.Instance != null)
			    {
			        if (isWin && !hasPvp)
			        {
			            setAwardCallBack();
			        }
			        else
			            BattleManager.Instance
			                .awardFinfish();
			    }
			    else
			    {
			        finishWindow();
			        MaskWindow.UnlockUI();
			    }
			    if (MissionManager.instance != null)
			    {
			        MissionManager.instance.showAll();
                    MissionManager.instance.setBackGround();
			    }
			});
		} else if (gameObj.name == "friendButton") {
			if (FriendsManagerment.Instance.isFull ())
				return;
			applyFriend ();
		} else if (gameObj.name == "nextButton") {
			if (!isWin) {
				PvpInfoManagerment.Instance.clearDate ();
				BattleManager.Instance.awardFinfish (); 
				return;
			}
			FPortManager.Instance.getFPort<PvpGetInfoFPort> ().access ((hasPvp) => {
				//如果没pvp，表示三胜出局
				if (!hasPvp) {
					setAwardCallBack ();
				} else if (PvpInfoManagerment.Instance.getPvpInfo ().rule == "cup") {
					if (PvpInfoManagerment.Instance.getCurrentRound () < 3) {
						//这里可能一直等到pvp超时
						if (PvpInfoManagerment.Instance.getPvpTime (null) <= 0) {
							MessageWindow.ShowAlert (Language ("s0215"), (msg) => {
								if (BattleManager.Instance != null)
									BattleManager.Instance.awardFinfish ();
								else
									finishWindow ();
							});
						} else {
							UiManager.Instance.switchWindow<PvpCupWindow> ();
						}
					} else {
						setAwardCallBack ();
					}
				} else if (PvpInfoManagerment.Instance.getPvpInfo ().rule == "match") {
					if (PvpInfoManagerment.Instance.getCurrentRound () < 3) {
						//这里可能一直等到pvp超时
						if (PvpInfoManagerment.Instance.getPvpTime (null) <= 0) {
							MessageWindow.ShowAlert (Language ("s0215"), (msg) => {
								if (BattleManager.Instance != null)
									BattleManager.Instance.awardFinfish ();
								else
									finishWindow ();
							});
						} else {
							UiManager.Instance.switchWindow<PvpMainWindow> ();
						}
					} else {
						setAwardCallBack ();
					}
				}
			});
		}
	}
	
	private void windowBack ()
	{	
		BattleManager.Instance.awardFinfish (); 
	}
	
	private void setAwardCallBack ()
	{
		//PVP奖励发放回调，分挂机和非挂机情况
	    if (BattleGlobal.pvpMode == EnumPvp.nomal)
	    {
	        MissionMainWindow win = UiManager.Instance.getWindow<MissionMainWindow>();
	        if (win != null)
	        {
	            AwardManagerment.Instance.addFunc(AwardManagerment.AWARDS_PVP_DOUBLE, ((awards) =>
	            {
	                UiManager.Instance.openDialogWindow<PvpAwardWindow>((pvpwin) =>
	                {
	                    pvpwin.init(StringKit.toInt(resultValue.text), awards);
	                    PvpInfoManagerment.Instance.clearDate();
	                });
	                finishWindow();
	            }));
	        }
	        else
	        {
	            FuBenManagerment.Instance.cacheFinishCallBack = CallBackPvpAwardWindow;
                if(BattleManager.Instance != null)
                    BattleManager.Instance.awardFinfish();
	        }
	    }
		else if (BattleGlobal.pvpMode == EnumPvp.sweep) {
			AwardManagerment.Instance.addFunc (AwardManagerment.AWARDS_PVP_DOUBLE, (awards) => {
				SweepManagement.Instance.initPvpAward (awards [0]);
                if (UiManager.Instance.getWindow<SweepAwardWindow>() != null)
                    DestroyImmediate(UiManager.Instance.getWindow<SweepAwardWindow>().gameObject);
                UiManager.Instance.switchWindow<EmptyWindow>((win) => {
                    ScreenManager.Instance.loadScreen(1, null, GameManager.Instance.outSweepPvpBattle);
                });
			});
		}
        //if (BattleGlobal.pvpMode == EnumPvp.sweep) {
        //    BattleGlobal.pvpMode = EnumPvp.nomal;
        //    if(UiManager.Instance.getWindow<SweepAwardWindow>() != null)
        //        DestroyImmediate(UiManager.Instance.getWindow<SweepAwardWindow>().gameObject); 
        //    UiManager.Instance.switchWindow<EmptyWindow>((win) => {
        //        ScreenManager.Instance.loadScreen(1, null, GameManager.Instance.outSweepPvpBattle);
        //    });
        //} else {
        //    finishWindow();
        //    MaskWindow.UnlockUI();
        //}
        if (MissionManager.instance != null) {
            MissionManager.instance.showAll();
            MissionManager.instance.setBackGround();
        }
	}

	void CallBackPvpAwardWindow ()
	{
		AwardManagerment.Instance.addFunc (AwardManagerment.AWARDS_PVP_DOUBLE, ((awards) => {
			UiManager.Instance.openDialogWindow<PvpAwardWindow> ((pvpwin) => {
				pvpwin.init (StringKit.toInt (resultValue.text), awards);
				PvpInfoManagerment.Instance.clearDate ();
			});
		}));
	}
	
	protected override void DoEnable ()
	{
		resultValue.gameObject.SetActive (false);
		resultLabel.gameObject.SetActive (false);
		resultIcon.gameObject.SetActive (false);
		UiManager.Instance.backGround.switchBackGround ("ChouJiang_BeiJing");
	}
	
}
