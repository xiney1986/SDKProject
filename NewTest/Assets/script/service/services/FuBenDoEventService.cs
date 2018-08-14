using System;

/**
 * 副本事件服务
 * @author longlingquan
 * */
public class FuBenDoEventService:BaseFPort
{
	private const string KEY = "event";
	private const string KEY2 = "action";
	private const string KEY3 = "pstep";//当前修炼副本的最高进度
	private const string KEY_PVP = "get_pvp";
	private const string COMPLETE_STEP = "complete_s";//完成当前步骤(事件)
	private const string COMPLETE_POINT = "complete_p";//完成全部步骤(事件)
	private const string ERROR = "error";//异常 
	private const string COMPLETE_FB = "complete_fb";//完成整个副本
	private const string DIALOGUE = "dialogue_ok";//对话成功
	private const string FIGHT_LOSE = "fight_lose";//战斗失败
	private const string GET_CHEST = "get_mngv";//获得宝箱
	private const string JUMP = "jump";
	private const string EXIT = "exit";
    private const string TOW_FIGHTER="fighter";//
    private const string TOW_DEAD = "dead";
	private const string TIME_LIMIT = "time_limit";//pvp时间限制
    private const string TOW_AWARD_FAIL="tower_box_not_over";//爬塔开宝箱拿奖失败 这里手动退一步
    private const string TOW_BOX_OVER="tower_box_over";
	
	public FuBenDoEventService ()
	{
	}
	
	public override void read (ErlKVMessage message)
	{   
		//解析kvmessage  
		ErlType e = message.getValue (KEY) as ErlType; 
		if (e != null) {
			if (e is ErlArray) {
				string type = (e as ErlArray).Value [0].getValueString (); 
				//这个解析貌似没用处了
				parseArr (type, e as ErlArray);
			} else { 
				string key = e.getValueString ();
				if (key == COMPLETE_POINT) {
					MissionInfoManager.Instance.mission.updateMission (1);
				} else if (key == DIALOGUE) {
					
				} else if (key == FIGHT_LOSE) {
					parseFightLose (message.getValue (KEY2) as ErlType, message.getValue (KEY3) as ErlType);
				} else if (key == COMPLETE_FB) {
					if (message.getValue ("dead") != null) {
						MissionInfoManager.Instance.mission.deadNum = StringKit.toInt ((message.getValue ("dead") as ErlType).getValueString ());
					}
					MissionInfoManager.Instance.mission.completeMission ();
                } else if (key == TOW_AWARD_FAIL) {
                    MissionInfoManager.Instance.mission.getPlayerPoint().fightLoss();
                } else if (key == TOW_BOX_OVER) {
                    UiManager.Instance.missionMainWindow.updateTowerLottey();
                }
			}
		}
		ErlType pvp = message.getValue (KEY_PVP) as ErlType; 
		if (pvp != null) {
			if (pvp is ErlAtom) {
				if (MissionInfoManager.Instance != null && MissionInfoManager.Instance.mission != null) {
					string str = pvp.getValueString ();
					if (str == TIME_LIMIT) { 
						MissionInfoManager.Instance.mission.doPvP (2);
					} else if (str == FPortGlobal.SYSTEM_OK) {
						MissionInfoManager.Instance.mission.doPvP (1);
					} else {
						MissionInfoManager.Instance.mission.doPvP (3);
					}
				}
			}
		}
	}
	
	//解析erlArray
	private void parseArr (string key, ErlArray arr)
	{ 
		if (key == GET_CHEST) {
			int sid = (arr.Value [1] as ErlInt).Value;	//宝箱sid
			int num = StringKit.toInt (arr.Value [2].getValueString ());  //宝箱数量  
		} else if (key == COMPLETE_STEP) {
			int step = StringKit.toInt (arr.Value [1].getValueString ());   //当前点下一个事件索引
		} else if (key == JUMP) {
			int sid_p = StringKit.toInt (arr.Value [1].getValueString ());   //下一个点sid 
		}else if(MissionInfoManager.Instance.isTowerFuben()){
            if (key == TOW_FIGHTER) {
                if (arr.Value.Length > 1) {
                    if (arr.Value[1].getValueString() == TOW_DEAD) {
                        MissionInfoManager.Instance.mission.getPlayerPoint().fightLoss();
                        UiManager.Instance.openDialogWindow<MessageWindow>((win) => {
                            win.initWindow(1, LanguageConfigManager.Instance.getLanguage("ladderButton"), "", LanguageConfigManager.Instance.getLanguage("towerShowWindow06"), (msgHandle) => {
                                ClmbTowerManagerment.Instance.turnSpriteData = null;
                                ClmbTowerManagerment.Instance.getAwardSuccessCallBack = null;
                                ClmbTowerManagerment.Instance.getGiveUpCallBack = null;
                                ClmbTowerManagerment.Instance.isCanGetAward = false;
                                LoadingWindow.isShowProgress = false;
                                //切换到空窗口后回调
                                UiManager.Instance.switchWindow<EmptyWindow>(
                                    (winn) => {
                                        MissionManager.instance.cleanCache();
                                        ScreenManager.Instance.loadScreen(1, MissionManager.instance.missionClean, () => {
                                            MissionInfoManager.Instance.clearMission();
                                            UiManager.Instance.emptyWindow.finishWindow();
                                        });
                                    });
                            });
                        });
                    }
                }
            }
        }
	}
	
	//目前只有修炼副本用到这个
	public void parseFightLose (ErlType e2, ErlType e3)
	{
		bool isExit = false;
		if (e2 != null) {
			string key2 = e2.getValueString ();
			if (key2 == EXIT)
				isExit = true;
		} 
		//更新最高记录
		if (e3 != null) {
			int currentPracticePoint = StringKit.toInt (e3.getValueString ());
			int historyPracticeHightPoint = UserManager.Instance.self.practiceHightPoint;
			if (currentPracticePoint > UserManager.Instance.self.practiceHightPoint)
				UserManager.Instance.self.practiceHightPoint = currentPracticePoint;
			MissionInfoManager.Instance.mission.updatePracticeRecode (currentPracticePoint, historyPracticeHightPoint);
		} 
		
		if (MissionInfoManager.Instance != null && MissionInfoManager.Instance.mission != null)
			MissionInfoManager.Instance.mission.fightLoss (isExit);
	}
} 
