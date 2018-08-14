using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ButtonArenaItem : ButtonBase
{ 
	public UILabel label_Rank;
	public UILabel label_ChallengeTimes;
	public UILabel label_Time;
	public UILabel label_Limit;
	public UILabel label_prefix;
	public UILabel label_arena_times;
	public GameObject root_userInfo;
	public UITexture texture_Bg;
	public ArenaActivityInfo data;//竞技场活动
    //*天国宝藏*//
    public GameObject tips;//奖励未领取标志
    public GameObject[] infoLabels;//文字提示

	private bool isEnable;
	private bool timeUp;
    private int myRank;
    List<ArenaFinalInfo>[] allInfoList;

	public void OnNetResume()
	{
		begin();
	}
	public override void begin ()
	{
		switch (data.type) {
		case EnumArena.arena:
			label_Time.effectColor = new Color(0.85f,0f,0f);
			if (isEnable) {
				FPortManager.Instance.getFPort<ArenaGetStateFPort> ().access ((state,time) => {
					if (state == ArenaManager.STATE_MASS) {
						if (ArenaManager.instance.self == null) {
							FPortManager.Instance.getFPort<ArenaMassFPort> ().access (M_updateView_arena);
						} else
							M_updateView_arena ();
					}
					if(2 <= state && 7>= state){
						if (ArenaManager.instance.self == null) {
							FPortManager.Instance.getFPort<ArenaFinalFPort> ().access(M_updateView_arena_final);
						} else
							M_updateView_arena_final ();
					}
				});
			}
			break;
		case EnumArena.ladders:
			label_Time.effectColor = new Color(0f,0.24f,0.47f);
			if (isEnable) {
				FPortManager.Instance.getFPort<LaddersStateFPort> ().apply (null);
				if (LaddersManagement.Instance.Chests.M_getChests () [0] == null||ServerTimeKit.getSecondTime() > LaddersManagement.Instance.nextTime) {
					FPortManager.Instance.getFPort<LaddersGetInfoFPort> ().apply ((hasApply) => {
						if (hasApply)
							M_updateView_ladders ();
						else
							FPortManager.Instance.getFPort<LaddersApplyFPort> ().apply ((msg) => {
								if(msg.Equals("ok"))
								{
									FPortManager.Instance.getFPort<LaddersGetInfoFPort> ().apply (M_updateView_ladders);
								}
							});
					});
				} else {
					M_updateView_ladders ();
				}
			}
			break;

            case EnumArena.mineral: {
                texture_Bg.transform.localScale = new Vector3(1.03f, 1.08f, 1f);
                root_userInfo.SetActive(false);
                updateArenaTips();
			}
            

            break;
		}
	}
    /// <summary>
    /// 宝藏开采完成提示显示 
    /// </summary>
    public void updateArenaTips() {
        if (tips != null && infoLabels != null) {
            tips.SetActive(false);
            for (int i = 0; i < infoLabels.Length; i++) {
                infoLabels[i].SetActive(false);
            }
        }
        for (int i = 0; i < 2; i++) {
            if (MiningManagement.Instance.minerals[i] == null) continue;
            if (MiningManagement.Instance.GetRemainTime(i) > 0) continue;
            tips.SetActive(true);
            //if (MiningManagement.Instance.minerals[0] != null && MiningManagement.Instance.minerals[1] != null
            //    && i > 0 && MiningManagement.Instance.GetMiningSampleBySid(MiningManagement.Instance.minerals[i].sid).type ==
            //    MiningManagement.Instance.GetMiningSampleBySid(MiningManagement.Instance.minerals[i - 1].sid).type) {//相同秘境只显示一条提示
            //    return;
            //}
            if (MiningManagement.Instance.GetMiningSampleBySid(MiningManagement.Instance.minerals[i].sid).type == (int)MiningTypePage.MiningGold) {
                for (int k = 0; k < infoLabels.Length; k++) {
                    if (!infoLabels[k].activeSelf) {
                        infoLabels[k].transform.FindChild("Label").GetComponent<UILabel>().text = LanguageConfigManager.Instance.getLanguage("prefabzc49");//金币
                        infoLabels[k].SetActive(true);
                        break;
                    }
                }
            } else if (MiningManagement.Instance.GetMiningSampleBySid(MiningManagement.Instance.minerals[i].sid).type == (int)MiningTypePage.MiningGem) {
                for (int k = 0; k < infoLabels.Length; k++) {
                    if (!infoLabels[k].activeSelf) {
                        infoLabels[k].transform.FindChild("Label").GetComponent<UILabel>().text = LanguageConfigManager.Instance.getLanguage("prefabzc50");//结晶
                        infoLabels[k].SetActive(true);
                        break;
                    }
                }
            }
        }
    }
	public void updateTime ()
	{
		if (!isEnable || timeUp) {
			return;
		}
		string str = "";
		switch (data.type) {
		case EnumArena.arena:
			if (isEnable) {
				if (ArenaManager.instance.stateEndTime <= ServerTimeKit.getSecondTime ()) {
					timeUp = true;
				}
				string timeStr = TimeKit.timeTransformDHMS (ArenaManager.instance.stateEndTime - ServerTimeKit.getSecondTime ());
				int state = ArenaManager.instance.state;
				if (state == ArenaManager.STATE_WAIT || state == ArenaManager.STATE_RESET) {
					str = LanguageConfigManager.Instance.getLanguage ("Arena58", timeStr);
				} else if (state == ArenaManager.STATE_MASS_RESET || state == ArenaManager.STATE_MASS) {
					str = LanguageConfigManager.Instance.getLanguage ("Arena59", timeStr);
				} else if (state == ArenaManager.STATE_64_32) {
					str = LanguageConfigManager.Instance.getLanguage ("Arena60", timeStr);
				} else if (state == ArenaManager.STATE_32_16) {
					str = LanguageConfigManager.Instance.getLanguage ("Arena61", timeStr);
				}else if (state == ArenaManager.STATE_16_8) {
					str = LanguageConfigManager.Instance.getLanguage ("Arena62", timeStr);
				} else if (state == ArenaManager.STATE_8_4) {
					str = LanguageConfigManager.Instance.getLanguage ("Arena65", timeStr);
				} else if (state == ArenaManager.STATE_4_2) {
					str = LanguageConfigManager.Instance.getLanguage ("Arena63", timeStr);
				} else if (state == ArenaManager.STATE_CHAMPION) {
					str = LanguageConfigManager.Instance.getLanguage ("Arena64", timeStr);
				} else if (state == ArenaManager.STATE_RESET) {
					str = LanguageConfigManager.Instance.getLanguage ("Arena59", timeStr);
				}
			}
//			
			label_Time.text = str;
			break;
		case EnumArena.ladders:
//			
			if (isEnable) {
				if(!LaddersManagement.Instance.Award.hasReceive)
				{
					label_Time.text = Language ("laddersTip_19");
				}else
				{
					if (LaddersManagement.Instance.nextTime <= ServerTimeKit.getSecondTime ()) {
						timeUp = true;
						label_Time.text = Language ("laddersTip_34");
					} else {
						string timeStr = TimeKit.timeTransformDHMS (Mathf.Max (LaddersManagement.Instance.nextTime - ServerTimeKit.getSecondTime (), 0));
						label_Time.text = Language ("ArenaNavigateTip_4", timeStr);
					}
				}
			}
			break;

        case EnumArena.mineral:
            root_userInfo.SetActive(false);

            break;
		}
	}

	public void updateActive (ArenaActivityInfo _data, Texture _bg)
	{	
		timeUp = false;
		data = _data;
		isEnable = UserManager.Instance.self.getUserLevel () >= data.RequestMinLevel;
		texture_Bg.mainTexture = _bg;
		M_updateMainView ();

	}

	private void M_updateMainView ()
	{
		if (!isEnable) {
			label_Limit.gameObject.SetActive (true);
			label_Limit.text = LanguageConfigManager.Instance.getLanguage ("warchoose04", data.RequestMinLevel.ToString ());
			root_userInfo.SetActive (false);
			texture_Bg.alpha = 0.5f;
		} else {
			label_Limit.gameObject.SetActive (false);
			root_userInfo.SetActive (true);
			texture_Bg.alpha = 1f;
		}
	}

	private void M_updateView_arena ()
	{
		if (ArenaManager.instance.self == null)
			return;
		//int count = am.getChallengeCount ();
		label_Rank.text = ArenaManager.instance.self.rank.ToString ();//LanguageConfigManager.Instance.getLanguage ("ArenaNavigateTip_1", ArenaManager.instance.getTeamNameById (ArenaManager.instance.self.team), ArenaManager.instance.self.rank.ToString ());
		//label_ChallengeTimes.text =Mathf.Max(0,(ArenaManager.instance.maxChallengeCount - ArenaManager.instance.challengeCount)).ToString ();//LanguageConfigManager.Instance.getLanguage ("ArenaNavigateTip_3", (ArenaManager.instance.maxChallengeCount - ArenaManager.instance.challengeCount).ToString ());
		label_ChallengeTimes.text=ArenaManager.instance.getChallengeCount().ToString();
	}
	/// <summary>
	/// M_updateView_arena_final
	/// </summary>
	private void M_updateView_arena_final () {
		if( ArenaManager.computeGuessNumber () >0)
			label_arena_times.gameObject.SetActive (true);
		label_arena_times.text = ArenaManager.computeGuessNumber ().ToString ();
		label_prefix.text = LanguageConfigManager.Instance.getLanguage ("ArenaNavigateTip_6");
		//label_Rank.text = ArenaManager.instance.self.rank.ToString ();
		label_ChallengeTimes.text = ArenaManager.computeGuessNumber ().ToString ();
        initRankInfo();
        //label_Rank.text = LanguageConfigManager.Instance.getLanguage("Arena14_" + myRank + "_2");
        //if(ArenaManager.instance.state == ArenaManager.STATE_64_32)
        //    label_Rank.text = LanguageConfigManager.Instance.getLanguage("Arena14_1_2");
        //else if(ArenaManager.instance.state == ArenaManager.STATE_32_16)
        //    label_Rank.text = LanguageConfigManager.Instance.getLanguage("Arena14_2_2");
        //else if(ArenaManager.instance.state == ArenaManager.STATE_16_8)
        //    label_Rank.text = LanguageConfigManager.Instance.getLanguage("Arena14_3_2");
        //else if(ArenaManager.instance.state == ArenaManager.STATE_8_4)
        //    label_Rank.text = LanguageConfigManager.Instance.getLanguage("Arena14_4_2");
        //else if(ArenaManager.instance.state == 6 || ArenaManager.instance.state == 7)
        //    label_Rank.text = LanguageConfigManager.Instance.getLanguage("Arena14_5_2");
        //else 
        //    label_Rank.text = LanguageConfigManager.Instance.getLanguage("Arena14_0_2");
	}

	private void M_updateView_ladders (bool _value)
	{
		label_Rank.text = LaddersManagement.Instance.currentPlayerRank.ToString ();//LanguageConfigManager.Instance.getLanguage ("ArenaNavigateTip_2", LaddersManagement.Instance.currentPlayerRank.ToString ());
		label_ChallengeTimes.text = (LaddersManagement.Instance.maxFightTime - LaddersManagement.Instance.currentChallengeTimes).ToString ();//LanguageConfigManager.Instance.getLanguage ("ArenaNavigateTip_3", (LaddersManagement.Instance.maxFightTime - LaddersManagement.Instance.currentChallengeTimes).ToString ());
	}

	private void M_updateView_ladders ()
	{
		M_updateView_ladders (true);
	}
    private void initRankInfo()
    {
        bool findMe = false;
        allInfoList = new List<ArenaFinalInfo>[5];
        for (int i = 0; i < 5; i++)
        {
            List<ArenaFinalInfo> infoList = new List<ArenaFinalInfo>();
            ArenaFinalInfo[][] infoListArr;
            if (i <= 3)
            {
                infoListArr = new ArenaFinalInfo[5][];
                addArenaFinalInfo(infoList, infoListArr[0] = ArenaManager.instance.getFinalInfo(i * 16, 16));
                addArenaFinalInfo(infoList, infoListArr[1] = ArenaManager.instance.getFinalInfo(i * 8 + 64, 8));
                addArenaFinalInfo(infoList, infoListArr[2] = ArenaManager.instance.getFinalInfo(i * 4 + 96, 4));
                addArenaFinalInfo(infoList, infoListArr[3] = ArenaManager.instance.getFinalInfo(i * 2 + 112, 2));
                addArenaFinalInfo(infoList, infoListArr[4] = ArenaManager.instance.getFinalInfo(i + 120, 1));

            }
            else
            {
                infoListArr = new ArenaFinalInfo[3][];
                addArenaFinalInfo(infoList, infoListArr[0] = ArenaManager.instance.getFinalInfo(120, 4));
                addArenaFinalInfo(infoList, infoListArr[1] = ArenaManager.instance.getFinalInfo(124, 2));
                addArenaFinalInfo(infoList, infoListArr[2] = ArenaManager.instance.getFinalInfo(126, 1));
            }
            allInfoList[i] = infoList;
            for (int j = 0; j < infoListArr.Length; j++)
            {
                foreach (ArenaFinalInfo info in infoListArr[j])
                {
                    if (i <= 3)
                    {
                        info.finalState = ArenaManager.STATE_64_32 + j;
                    }
                    else
                    {
                        info.finalState = ArenaManager.STATE_4_2 + j;
                    }

                    if (!findMe && info.uid == UserManager.Instance.self.uid)
                    {
                        findMe = true;
                        myRank = info.finalState - 1;
                    }
                }
            }
        }
        label_Rank.text = LanguageConfigManager.Instance.getLanguage("Arena14_" + myRank + "_2");
    }
	void FixedUpdate ()
	{  
		/*
		if (chapter != null) {
			timeLabel.text = chapter.getTimeDesc ();
		} 
		*/

	}
    void addArenaFinalInfo(List<ArenaFinalInfo> infoList, ArenaFinalInfo[] info)
    {
        foreach (ArenaFinalInfo each in info)
        {
            infoList.Add(each);
        }

    }
	public override void DoClickEvent ()
	{
		if (!isEnable) {
			MaskWindow.UnlockUI ();
			return;
		}
		switch (data.type) {
		case EnumArena.arena:
			M_clickArena ();
			break;
		case EnumArena.ladders:
			//M_requestLaddersInfo ();
			UiManager.Instance.switchWindow<LaddersWindow> ((win) => {
				win.init ();
				win.lastPrestigeLevel = LaddersManagement.Instance.M_getCurrentPlayerTitle ().index;
				if (LaddersManagement.Instance.Award.canReceive) {
					UiManager.Instance.openDialogWindow<LaddersRankRewardWindow> ((win1) => {
						win1.closeCallback = win.showPrestigeLevel;
					});
				}
			});
			break;
		case EnumArena.mineral:

			UiManager.Instance.openWindow<MiningWindow>(win =>{
				win.InitPage();
			});
			break;
		}	
	}

	private void M_clickLadders ()
	{
		LaddersApplyFPort fport = FPortManager.Instance.getFPort<LaddersApplyFPort> ();
		fport.apply (M_requestLaddersInfo);		

	}

	private void M_requestLaddersInfo (string msg)
	{
		if(!msg.Equals("ok"))
		{
			return;
		}
		LaddersGetInfoFPort fport = FPortManager.Instance.getFPort<LaddersGetInfoFPort> ();
		fport.apply ((hasApply) => {
			if (hasApply)
				UiManager.Instance.switchWindow<LaddersWindow> ((win) => {
					win.init ();
					win.lastPrestigeLevel = LaddersManagement.Instance.M_getCurrentPlayerTitle ().index;
					if (LaddersManagement.Instance.Award.canReceive) {
						UiManager.Instance.openDialogWindow<LaddersRankRewardWindow> ((win1) => {
							win1.closeCallback = win.showPrestigeLevel;
						});
					}
				});
			else
				M_clickLadders ();
		});
	}

	private void M_clickArena ()
	{
		int openLevel = GameConfig.Instance.getInt (GameConfig.SID_ARENA_OPEN_LEVEL);
		if (UserManager.Instance.self.getUserLevel () < openLevel) {
			MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("Arena32", openLevel.ToString ()));
			return;
		}
		
		FPortManager.Instance.getFPort<ArenaGetStateFPort> ().access ((state,time) => {
			ArenaManager.instance.state = state;
			ArenaManager.instance.stateEndTime = time;
			// UserManager.getArenaFinal中存在此判断,请一并修改.
			if (!ArenaManager.instance.isStateCorrect ()) {
				MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("Arena44"));
			} else if (state == ArenaManager.STATE_WAIT) {
				System.DateTime dt = TimeKit.getDateTime (time);
				MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("Arena23", dt.Month.ToString (), dt.Day.ToString (), dt.Hour.ToString (), dt.Minute.ToString ()));
			} else if (state == ArenaManager.STATE_MASS_RESET) {
				MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("Arena25"));
			} else if (state == ArenaManager.STATE_MASS) {
				UiManager.Instance.switchWindow<ArenaAuditionsWindow> ((win) => {
					win.showTeamDialog = true;
				});
			}else if(state == ArenaManager.STATE_RESET)
			{
				UiManager.Instance.switchWindow<ArenaFinalWindow>((win)=>{
					win.setArenaManagerTapIndex(4);
					win.lookChanPosition();
				});
			}
			else {
				UiManager.Instance.switchWindow<ArenaFinalWindow> ();
			}
		});
	}
}

